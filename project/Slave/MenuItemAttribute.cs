using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeMiner.Slave
{
    /// <summary>
    /// Makes method visible in try context menu
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class MenuItemAttribute : Attribute
    {
        /// <summary>
        /// Text of item
        /// </summary>
        public readonly string label;
        /// <summary>
        /// Order of item. Default order is 10
        /// </summary>
        public readonly int order = 10;
        public MenuItemAttribute(string label)
        {
            this.label = label;
        }

        public MenuItemAttribute(string label, int order)
        {
            this.label = label;
            this.order = order;
        }
    }
}
