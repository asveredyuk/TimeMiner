using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using TimeMiner.Core;
using TimeMiner.Master.Settings;
using TimeMiner.Master.Settings.ApplicationIdentifiers;

namespace TimeMiner.Master.Analysis
{
    /// <summary>
    /// Report to detect which programs are used
    /// </summary>
    public class ProgramUsageReport : BaseReport<ProgramUsageReport.ReportResult>
    {
        public struct AppDesc
        {
            public Guid RelevanceId { get; set; }
            public Guid AppId { get; set; }
            public Relevance Rel { get; set; }
            public string Name { get; set; }

            public bool Equals(AppDesc other)
            {
                return RelevanceId.Equals(other.RelevanceId) && AppId.Equals(other.AppId) && Rel == other.Rel && string.Equals(Name, other.Name);
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                return obj is AppDesc && Equals((AppDesc) obj);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    var hashCode = RelevanceId.GetHashCode();
                    hashCode = (hashCode*397) ^ AppId.GetHashCode();
                    hashCode = (hashCode*397) ^ (int) Rel;
                    hashCode = (hashCode*397) ^ (Name != null ? Name.GetHashCode() : 0);
                    return hashCode;
                }
            }

            public static bool operator ==(AppDesc left, AppDesc right)
            {
                return left.Equals(right);
            }

            public static bool operator !=(AppDesc left, AppDesc right)
            {
                return !left.Equals(right);
            }
        }
        public class ReportItem
        {
            /// <summary>
            /// Reference to the profile relevance to the current application
            /// </summary>
            public AppDesc Desc { get; set; }
            /// <summary>
            /// Number of seconds, spent in a given application
            /// </summary>
            public int SecondsSpent { get; set; }
            /// <summary>
            /// Percentage of the total time spent in all apps
            /// </summary>
            public int Percent { get; set; }

            public ReportItem(AppDesc desc, int secondsSpent, int percent)
            {
                Desc = desc;
                SecondsSpent = secondsSpent;
                Percent = percent;
            }

            public ReportItem()
            {
            }
        }

        public class ReportResult : BaseReportResultCollection<ReportItem>
        {
            public override ReportItem[] Items { get; }

            public ReportResult(ReportItem[] items)
            {
                Items = items;
            }
        }

        public class Params
        {
            /// <summary>
            /// If set, only active records are analyzed
            /// </summary>
            public ActiveReport ActiveReport { get; set; }
        }
       

        private Dictionary<AppDesc, int> spentTimes;
        public Params Parameters { get; }
        public ProgramUsageReport(Log log):base(log)
        {
            Parameters = new Params();
        }


        public void CalculateIt()
        {
            if(spentTimes != null)
                throw new Exception("Already calculated");
            LogRecord[] records = log.Records;
            if (Parameters.ActiveReport != null)
            {
                bool[] actives = Parameters.ActiveReport.GetActivitiesOnly().ToArray();
                if(actives.Length != records.Length)
                    throw new Exception("Wrong active report assigned");
                records = records.Where((t, index) => actives[index]).ToArray();
            }
            //logic here
            spentTimes = CalculateSpentTimes(records);
        }

        public override ReportResult Calculate()
        {
            ReportItem[] items = GetItems().ToArray();
            var result = new ReportResult(items);
            return result;
        }

        private IEnumerable<ReportItem> GetItems()
        {
            if(spentTimes == null)
                CalculateIt();
            //logic here
            int totalTime = spentTimes.Select(t => t.Value).Sum();
            foreach (var pair in spentTimes.OrderByDescending(t => t.Value))
            {
                int percentage = pair.Value * 100 / totalTime;
                yield return new ReportItem(pair.Key, pair.Value,percentage);
            }
        }
        private Dictionary<AppDesc, int> CalculateSpentTimes(IEnumerable<LogRecord> records)
        {
            Dictionary<AppDesc,int> dict = new Dictionary<AppDesc, int>();
            foreach (var logRecord in records)
            {
                ApplicationIdentifierBase identifier = log.Prof.FindIdentifier(logRecord);
                AppDesc desc;
                if (identifier == null)
                {
                    //was not found
                    desc = new AppDesc()
                    {
                        Rel = Relevance.unknown,
                        Name = $"Unknown app {logRecord.Process.ProcessName}"
                    };
                    /*desc.AppId = Guid.Empty;
                    desc.RelevanceId = Guid.Empty;
                    desc.Rel = Relevance.unknown;
                    desc.Name = $"Unknown app {logRecord.Process.ProcessName}";*/
                }
                else
                {
                    ProfileApplicationRelevance rel = log.Prof[identifier];
                    desc = new AppDesc()
                    {
                        RelevanceId = rel.Id,
                        AppId = rel.App.Id,
                        Name = rel.App.Name,
                        Rel = rel.Rel
                    };
                    /*desc.RelevanceId = rel.Id;
                    desc.AppId = rel.App.Id;
                    desc.Name = rel.App.Name;
                    desc.Rel = rel.Rel;*/
                }
                if (!dict.ContainsKey(desc))
                {
                    dict[desc] = 0;
                }
                dict[desc]++;
            }
            return dict;
        }
    }
}
