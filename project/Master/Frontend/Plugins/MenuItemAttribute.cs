using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeMiner.Master.Frontend.Plugins
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public class MenuItemAttribute : Attribute
    {
        /// <summary>
        /// Text, that will be shown in menu
        /// </summary>
        public readonly string label;
        /// <summary>
        /// Path in menu tree
        /// </summary>
        public readonly string menuPath;
        /// <summary>
        /// Order in menu
        /// </summary>
        public readonly int order;
        /// <summary>
        /// If set, given url will be parsed
        /// </summary>
        public readonly string url;
        /// <summary>
        /// Create new attribute
        /// </summary>
        /// <param name="label">Text to show in menu</param>
        /// <param name="menuPath">Path in menu tree</param>
        /// <param name="order">Order in menu</param>
        /// <param name="url">If set, given url is used for href. Else handled path is used</param>
        public MenuItemAttribute(string label, string menuPath, int order = FrontendPageMenuItem.DEFAULT_ORDER, string url = null)
        {
            this.label = label;
            this.menuPath = menuPath;
            this.order = order;
            this.url = url;
        }
    }
}
