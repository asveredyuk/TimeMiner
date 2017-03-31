using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeMiner.Master.Frontend.Plugins
{
    /// <summary>
    /// Attribute, to mark methods that handle api requests
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class ApiPathAttribute : Attribute
    {
        /// <summary>
        /// Relative path
        /// </summary>
        public readonly string path;
        /// <summary>
        /// Marks method that handles api requests
        /// </summary>
        /// <param name="path">Relative path, in real it will be /api/...</param>
        public ApiPathAttribute(string path)
        {
            this.path = path;
        }
    }
}
