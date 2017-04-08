using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeMiner.Core;
using TimeMiner.Master.Settings;

namespace TimeMiner.Master.Analysis
{
    public class Log
    {

        public IndexedProfile Prof { get; }
        public LogRecord[] Records { get; }

        public Log(LogRecord[] records, IndexedProfile prof)
        {
            this.Records = records;
            this.Prof = prof;
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
        //TODO: make better make log with user id and dates
        public static Log GetLog()
        {
            IndexedProfile prof = IndexedProfile.FromProfile(SettingsContainer.Self.GetBaseProfile());
            LogRecord[] recs = MasterDB.Logs.GetAllRecordsForUser(Guid.Empty).ToArray();
            return new Log(recs,prof);
        }

        public static Log GetLog(DateTime begin, DateTime end)
        {
            IndexedProfile prof = IndexedProfile.FromProfile(SettingsContainer.Self.GetBaseProfile());
            LogRecord[] recs = MasterDB.Logs.GetLogRecordsForUserForPeriod(Guid.Empty, begin,end).ToArray();
            return new Log(recs, prof);
        }
    }
}
