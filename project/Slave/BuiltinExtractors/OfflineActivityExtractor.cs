using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using TimeMiner.Core.MetaInfoClasses;

namespace TimeMiner.Slave.BuiltinExtractors
{
    /// <summary>
    /// Extractor, used to add info about offline activity to log records
    /// </summary>
    public class OfflineActivityExtractor : MetaExtractor
    {
        /// <summary>
        /// Queue to send objects
        /// </summary>
        public static Queue<OfflineActivity> SendQueue { get; } = new Queue<OfflineActivity>();
        public override bool CanAccept(Process process, IntPtr wHandle)
        {
            return SendQueue.Count > 0;
        }

        public override KeyValuePair<string, byte[]> Extract(Process process, IntPtr wHandle)
        {
            var activity = SendQueue.Dequeue();
            string json = JsonConvert.SerializeObject(activity);
            var bytes = Encoding.UTF8.GetBytes(json);
            return new KeyValuePair<string, byte[]>(OfflineActivity.TAG, bytes);
        }
    }
}
