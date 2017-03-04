using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeMiner.Core;
using TimeMiner.Master.Settings;

namespace TimeMiner.Master.Analysis
{
    /// <summary>
    /// Report to detect which programs are used
    /// </summary>
    public class ProgramUsageReport
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
        /// <summary>
        /// Log to analyze
        /// </summary>
        private Log log;
        /// <summary>
        /// Results of report
        /// </summary>
        private Dictionary<string, int> results;
        /// <summary>
        /// Results of report
        /// </summary>
        public Dictionary<string, int> Results
        {
            get
            {
                if (results == null)
                {
                    throw new Exception("This report was not calculated");
                }
                return results;
            }
        }
        /// <summary>
        /// Are results calculated or not
        /// </summary>
        public bool IsCalculated
        {
            get { return results != null; }
        }

        public ProgramUsageReport(Log log)
        {
            this.log = log;
        }
        /// <summary>
        /// Make calculations
        /// </summary>
        public void Calculate()
        {
            results = CalculateSpentTimesPerApp(log.Records);
        }
        /// <summary>
        /// Make calculatios only for active elements
        /// </summary>
        /// <param name="actives"></param>
        public void CalculateWithActives(bool[] actives)
        {
            if(actives.Length != log.Records.Length)
                throw new ArgumentException("Length of activities does not match length of log");
            IEnumerable<LogRecord> records = log.Records.Where((t, index) => actives[index]);
            results = CalculateSpentTimesPerApp(records);
        }
        /// <summary>
        /// Get item of resulting report (for api)
        /// </summary>
        /// <returns></returns>
        public IEnumerable<ProgramUsageReportItem> GetItems()
        {
            if (!IsCalculated)
            {
                throw new Exception("Report was not calculated");
            }

            
            int totalTime = results.Select(t => t.Value).Sum();
            foreach (var pair in results.OrderByDescending(t=>t.Value))
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
        /// <summary>
        /// Calculate times spent per application
        /// </summary>
        /// <param name="records"></param>
        /// <returns></returns>
        private Dictionary<string, int> CalculateSpentTimesPerApp(IEnumerable<LogRecord> records)
        {
            Dictionary<string,int> dict = new Dictionary<string, int>();
            foreach (var logRecord in records)
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
