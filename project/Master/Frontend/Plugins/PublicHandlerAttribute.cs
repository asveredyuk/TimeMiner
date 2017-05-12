using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeMiner.Master.Frontend.Plugins
{
    /// <summary>
    /// makes handler method available without authorization
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    class PublicHandlerAttribute:Attribute
    {
    }
}
