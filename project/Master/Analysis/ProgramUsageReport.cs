using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeMiner.Master.Settings;

namespace TimeMiner.Master.Analysis
{
    class ProgramUsageReport
    {
        public class ProgramUsageReportItem
        {
            /// <summary>
            /// Reference to the profile relevance to the current application
            /// </summary>
            public ProfileApplicationRelevance Rel { get; set; }
            /// <summary>
            /// Number of seconds, spent in a given application
            /// </summary>
            public int SecondsSpent { get; set; }
            /// <summary>
            /// Percentage of the total time spent in all apps
            /// </summary>
            public int Percent { get; set; }

            public ProgramUsageReportItem(ProfileApplicationRelevance rel, int secondsSpent, int percent)
            {
                Rel = rel;
                SecondsSpent = secondsSpent;
                Percent = percent;
            }

            public ProgramUsageReportItem()
            {
            }
        }
        private Log log;

        public ProgramUsageReport(Log log)
        {
            this.log = log;
        }

        public IEnumerable<ProgramUsageReportItem> GetReport()
        {
            Dictionary<string, int> times = CalculateSpentTimesPerApp();
            int totalTime = times.Select(t => t.Value).Sum();
            foreach (var pair in times.OrderByDescending(t=>t.Value))
            {
                ProfileApplicationRelevance rel = log.Prof[pair.Key];
                if (rel == null)
                {
                    ApplicationDescriptor desc = new ApplicationDescriptor("Unknown app " + pair.Key + ".exe",pair.Key);
                    rel = new ProfileApplicationRelevance(Relevance.unknown, desc);
                }
                int percentage = pair.Value*100/totalTime;
                yield return new ProgramUsageReportItem(rel,pair.Value,percentage);
            }
        }

        public Dictionary<string, int> CalculateSpentTimesPerApp()
        {
            Dictionary<string,int> dict = new Dictionary<string, int>();
            foreach (var logRecord in log.Records)
            {
                string id = logRecord.Process.ProcessName;
                if (!dict.ContainsKey(id))
                {
                    dict[id] = 0;
                }
                //now suppose it is second, TODO:redo this
                dict[id]++;
            }
            return dict;
        }

    }
}
