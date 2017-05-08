using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeMiner.Core;

namespace TimeMiner.Master.Analysis
{
    public class CompositeLog : ILog
    {
        private LogRecord[] records;
        public LogRecord[] Records
        {
            get
            {
                if (records == null)
                {
                    var allRecords = innerLogs.SelectMany(t => t.Records);
                    var forGivenPeriod = allRecords.Where(t => Util.CheckDateInPeriod(t.Time, timeFrom, timeTo));
                    records = forGivenPeriod.ToArray();
                }
                return records;
            }
        }

        public string DataHash { get; }

        public IndexedProfile Prof { get; }
        public DateTime Date { get; }

        private ILog[] innerLogs;
        private DateTime timeFrom;
        private DateTime timeTo;
        public CompositeLog(ILog[] innerLogs, IndexedProfile prof,DateTime timeFrom, DateTime timeTo, DateTime date = default(DateTime))
        {
            if(innerLogs.Length == 0)
                throw new ArgumentException("Cannot create composite log of nothing");
            this.innerLogs = innerLogs;
            this.Date = date;
            this.timeFrom = timeFrom;
            this.timeTo = timeTo;
            this.Prof = prof;
            //make data hash as hash of sum of all hashes
            DataHash = "";
            foreach (var innerLog in innerLogs)
            {
                if (innerLog.DataHash == null)
                {
                    DataHash = null;
                    break;
                }
                DataHash += innerLog.DataHash;
            }
            if (DataHash != null)
            {
                //dates are counted too!
                DataHash += timeFrom.ToString();
                DataHash += timeTo.ToString();
                DataHash = Util.ComputeStringMD5Hash(DataHash);
            }
        }
    }
}
