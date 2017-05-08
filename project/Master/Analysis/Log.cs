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
        private IEnumerable<LogRecord> recordsSource;
        private LogRecord[] records;
        public IndexedProfile Prof { get; }

        public LogRecord[] Records
        {
            get
            {
                if (records == null)
                    records = recordsSource.ToArray();
                return records;
            }
        }

        public string DataHash { get; }

        public DateTime Date
        {
            get
            {
                if (Records.Length == 0)
                    throw new Exception("empty log, no datetime");
                return Records[0].Time.Date;
            }
        }
        
        public Log(IEnumerable<LogRecord> recordsSource, IndexedProfile prof, string dataHash)
        {
            this.recordsSource = recordsSource;
            this.Prof = prof;
            this.DataHash = dataHash;

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
