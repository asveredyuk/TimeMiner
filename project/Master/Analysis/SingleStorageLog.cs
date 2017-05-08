using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeMiner.Core;
using TimeMiner.Master.Settings;

namespace TimeMiner.Master.Analysis
{
    public class SingleStorageLog : ILog
    {
        private IEnumerable<LogRecord> recordsSource;
        private LogRecord[] records;
        public IndexedProfile Prof { get; }
        public LogRecord[] Records
        {
            get
            {
                //read records only if it is required
                if (records == null)
                    records = recordsSource.ToArray();
                return records;
            }
        }

        public string DataHash { get; }
        public DateTime Date { get; }

        public SingleStorageLog(IEnumerable<LogRecord> recordsSource, IndexedProfile prof, string dataHash, DateTime date)
        {
            this.recordsSource = recordsSource;
            Prof = prof;
            DataHash = dataHash;
            this.Date = date;
        }
    }
}
