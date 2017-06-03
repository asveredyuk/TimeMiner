using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TimeMiner.Slave.Properties;


namespace TimeMiner.Slave
{
    /// <summary>
    /// Adds tray icon and tray context menu and extensions
    /// </summary>
    public class TrayView
    {
        /// <summary>
        /// Icon in tray
        /// </summary>
        private NotifyIcon notifyIcon;
        /// <summary>
        /// Context menu for tray
        /// </summary>
        private ContextMenuStrip contextMenu;
        /// <summary>
        /// List of extensions
        /// </summary>
        private IReadOnlyList<ClientInterfaceExtension> extensions;
        /// <summary>
        /// Handlers and their attributes
        /// </summary>
        private Dictionary<MenuItemAttribute, Action> menuHandlers;
        public TrayView()
        {
            //create context menu
            contextMenu = new ContextMenuStrip();

            notifyIcon = new NotifyIcon();
            notifyIcon.Icon = Resources.TrayIconDefault;
            notifyIcon.Text = "TimeMiner";
            notifyIcon.ContextMenuStrip = contextMenu;
            notifyIcon.MouseClick+= delegate(object sender, MouseEventArgs args)
            {
                if (args.Button != MouseButtons.Left)
                    return;
                MessageBox.Show("TimeMiner!");
            };
            notifyIcon.Visible = true;
            InitExtensions();
            SlavePluginRepository.Self.onAssembliesChanged += InitExtensions;

            UpdateStatus();
        }
        /// <summary>
        /// Periodically refreshes icon of current status
        /// </summary>
        private async void UpdateStatus()
        {
            while (true)
            {
                await Task.Delay(ConfigManager.Self.StatusRefreshInterval);
                if (ConfigManager.Self.StatusRefreshEnabled)
                {
                    MasterBoundary.Relevance rel = await MasterBoundary.Self.GetLastStatus();
                    Icon icon = null;
                    switch (rel)
                    {
                        case MasterBoundary.Relevance.good:
                            icon = Resources.TrayStatusGood;
                            break;
                        case MasterBoundary.Relevance.neutral:
                            icon = Resources.TrayStatusNeutral;
                            break;
                        case MasterBoundary.Relevance.bad:
                            icon = Resources.TrayStatusBad;
                            break;
                        case MasterBoundary.Relevance.unknown:
                            icon = Resources.TrayIconDefault;
                            break;
                    }
                    notifyIcon.Icon = icon;
                }
            }
        }
        /// <summary>
        /// Initialize extensions from plugins
        /// </summary>
        private void InitExtensions()
        {
            List<ClientInterfaceExtension> exts = new List<ClientInterfaceExtension>();
            foreach (var exType in SlavePluginRepository.Self.GetInstantiatableTypesDerivedFrom<ClientInterfaceExtension>())
            {
                ClientInterfaceExtension ext = (ClientInterfaceExtension) Activator.CreateInstance(exType);
                exts.Add(ext);
            }
            extensions = exts;
            Dictionary<MenuItemAttribute, Action> handlers = new Dictionary<MenuItemAttribute, Action>();
            foreach (var ext in exts)
            {
                var type = ext.GetType();
                var allMethods = type.GetMethods(BindingFlags.Instance | BindingFlags.Public);
                foreach (var method in allMethods)
                {
                    var attr = method.GetCustomAttribute<MenuItemAttribute>();
                    if (attr != null)
                    {
                        //menu item attribute attached
                        var h = Delegate.CreateDelegate(typeof(Action),ext, method) as Action;
                        if (h != null)
                        {
                            //successfully fitted
                            handlers[attr] = h;
                        }
                        else
                        {
                            Console.WriteLine("Failed to fit delegate");
                        }
                    }
                }
                menuHandlers = handlers;
            }
            BuildContextMenu();
        }

        /// <summary>
        /// Build context menu from given handlers
        /// </summary>
        private void BuildContextMenu()
        {
            contextMenu.Items.Clear();
            var items = menuHandlers.OrderBy(t => t.Key.order);
            foreach (var pair in items)
            {
                var action = pair.Value;
                var text = pair.Key.label;
                contextMenu.Items.Add(text).Click+= delegate {
                    action();
                };
            }
        }
    }
}
