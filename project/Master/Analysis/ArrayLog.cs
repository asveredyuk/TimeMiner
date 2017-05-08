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
    public class ArrayLog : ILog
    {
        public IndexedProfile Prof { get; }

        public LogRecord[] Records { get; }
        public string DataHash { get { return null; } }
        private DateTime date;
        public DateTime Date
        {
            get
            {
                if (date != default(DateTime))
                    return date;
                if (Records.Length == 0)
                    throw new Exception("empty log, no datetime");
                return Records[0].Time.Date;
            }
        }
        
        public ArrayLog(IEnumerable<LogRecord> records, IndexedProfile prof, DateTime date = default(DateTime))
        {
            this.date = date;
            Records = records.ToArray();
        }

        public static ArrayLog CrateEmpty(IndexedProfile prof, DateTime date = default(DateTime))
        {
            return new ArrayLog(new LogRecord[0], prof, date);
        }
    }
}
