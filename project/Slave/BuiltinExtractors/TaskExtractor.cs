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
    /// Extractor, that adds task to metadata of records
    /// </summary>
    public class TaskExtractor : MetaExtractor
    {
        /// <summary>
        /// Interval of inserting task to meta data
        /// </summary>
        private const int INSERT_INTERVAL_SECS = 10;
        /// <summary>
        /// Current task
        /// </summary>
        public static TaskDescription CurrentTask { get; set; }
        /// <summary>
        /// Time of last insert task to meta
        /// </summary>
        private DateTime LastInserted = DateTime.MinValue;
        public override bool CanAccept(Process process, IntPtr wHandle)
        {
            //check if there is no task to send
            if (CurrentTask == null)
                return false;
            //check if it is to early to send info about task again
            if ((DateTime.Now - LastInserted).TotalSeconds < INSERT_INTERVAL_SECS)
                return false;
            return true;
        }

        public override KeyValuePair<string, byte[]> Extract(Process process, IntPtr wHandle)
        {
            if(CurrentTask == null)
                throw new Exception("Something went wrong, no data to insert");

            string json = JsonConvert.SerializeObject(CurrentTask);
            byte[] data = Encoding.UTF8.GetBytes(json);
            LastInserted = DateTime.Now;
            return new KeyValuePair<string, byte[]>(TaskDescription.TAG, data);
        }
    }
}
