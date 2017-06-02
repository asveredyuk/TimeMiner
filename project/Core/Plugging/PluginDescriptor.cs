using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeMiner.Core.Plugging
{
    /// <summary>
    /// Describes assembly
    /// </summary>
    public class PluginDescriptor
    {
        /// <summary>
        /// Guid of an assembly
        /// </summary>
        public Guid Guid { get; set; }
        /// <summary>
        /// Version of an assembly
        /// </summary>
        public Version Version { get; set; }

        public PluginDescriptor()
        {
        }

        public PluginDescriptor(Guid guid, Version version)
        {
            Guid = guid;
            Version = version;
        }
    }
}
