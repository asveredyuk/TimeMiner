using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeMiner.Master.Frontend.Plugins
{
    [AttributeUsage(AttributeTargets.Method)]
    public class ApiPathAttribute : Attribute
    {
        public readonly string path;
        public ApiPathAttribute(string path)
        {
            this.path = path;
        }
    }
}
