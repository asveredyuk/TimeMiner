using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using TimeMiner.Core.Plugging;

namespace TimeMiner.Slave
{
    public class SlavePluginRepository : PluginRepository
    {
        /// <summary>
        /// Lock, to prevent multiple initialization
        /// </summary>
        private static readonly object _lock = new object();

        /// <summary>
        /// Singleton instance
        /// </summary>
        private static SlavePluginRepository self;

        /// <summary>
        /// Singleton instance
        /// </summary>
        public static SlavePluginRepository Self
        {
            get
            {
                lock (_lock)
                {
                    if (self == null)
                    {
                        self = new SlavePluginRepository();
                    }
                    return self;
                }
            }
        }

        public async Task SyncWithServer()
        {
            PluginDescriptor[] newDescriptors = await MasterBoundary.Self.GetPluginDescriptors();
            if (newDescriptors == null)
                return; //failed to connect to server or other problems
            //Delete uninstalled and outdated elements
            List<KeyValuePair<Assembly, PluginDescriptor>> descriptors = GetDescriptors().ToList();
            foreach (var p in descriptors)
            {
                var inNew = newDescriptors.FirstOrDefault(t => t.Guid == p.Value.Guid);
                if (inNew == null)
                {
                    //this plugin is not needed anymore
                    TryUninstallAssembly(p.Value.Guid);
                    continue;
                }
                //this plugin is okay
                //may be wrong version?
                if (inNew.Version != p.Value.Version)
                {
                    TryUninstallAssembly(p.Value.Guid);
                    continue;
                }
            }
            //Install new elements
            descriptors = GetDescriptors().ToList();
            foreach (var newDesc in newDescriptors)
            {
                if (!descriptors.Any(t => t.Value.Guid == newDesc.Guid))
                {
                    //no such item
                    byte[] data = await MasterBoundary.Self.GetAssembly(newDesc.Guid);
                    if (data == null)
                    {
                        //failed to load, will be loaded later
                        continue;
                    }
                    TryInstallAssembly(data);
                }
            }
        }
        protected override IEnumerable<Assembly> GetAdditionalAssemblies()
        {
            yield return Assembly.GetExecutingAssembly();
        }
    }
}
