using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeMiner.Core;

namespace TimeMiner.Slave
{
    /// <summary>
    /// Base class for meta extracting plugins
    /// </summary>
    public abstract class MetaExtractor
    {
        /// <summary>
        /// Can this extractor accept this process and window
        /// </summary>
        /// <param name="process"></param>
        /// <param name="wHandle"></param>
        /// <returns></returns>
        public abstract bool CanAccept(Process process, IntPtr wHandle);
        public abstract KeyValuePair<string, byte[]> Extract(Process process, IntPtr wHandle);
    }
}
