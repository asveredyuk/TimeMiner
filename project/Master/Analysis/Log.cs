using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeMiner.Core;
using TimeMiner.Master.Database;
using TimeMiner.Master.Settings;

namespace TimeMiner.Master.Analysis
{
    public class Log
    {

        public IndexedProfile Prof { get; }
        public LogRecord[] Records { get; }
        public StorageDescriptor StorageDescriptor { get; }

        public DateTime Date
        {
            get
            {
                if (Records.Length == 0)
                    throw new Exception("empty log, no datetime");
                return Records[0].Time.Date;
            }
        }
        /// <summary>
        /// Create new log
        /// </summary>
        /// <param name="records">Array of records</param>
        /// <param name="prof">Profile for indexing</param>
        /// <param name="desc">Descriptor of storage. Null for compound logs (records from different storages)</param>
        public Log(LogRecord[] records, IndexedProfile prof, StorageDescriptor desc)
        {
            this.Records = records;
            this.Prof = prof;
            this.StorageDescriptor = desc;
        }

        public Dictionary<Relevance, int> GetRelevanceTimes()
        {
            Dictionary<Relevance,int> dict = new Dictionary<Relevance, int>();
            dict[Relevance.bad] = dict[Relevance.good] = dict[Relevance.neutral] = dict[Relevance.unknown] = 0;
            foreach (var aRelevance in GetRelevances())
            {
                dict[aRelevance]++;
            }
            return dict;
            
        }
        
        public IEnumerable<Relevance> GetRelevances()
        {
            foreach (var logRecord in Records)
            {
                yield return Prof.GetExtendedRelevance(Prof.FindIdentifier(logRecord));
            }
        }

        private ProfileApplicationRelevance GetRel(LogRecord rec)
        {
            return Prof[Prof.FindIdentifier(rec)];
        }
        
    }
}
