using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Navigation;
using TimeMiner.Core;

namespace TimeMiner.Master.Analysis
{
    class UnknownIdentificationReport : BaseReport<UnknownIdentificationReport.ReportResult>
    {
        public struct Descriptor
        {
            public string ProcessName { get; set; }
            public string Domain { get; set; }

            public Descriptor(string processName, string domain)
            {
                ProcessName = processName;
                Domain = domain;
            }

            public bool Equals(Descriptor other)
            {
                return string.Equals(ProcessName, other.ProcessName) && string.Equals(Domain, other.Domain);
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                return obj is Descriptor && Equals((Descriptor) obj);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    return ((ProcessName != null ? ProcessName.GetHashCode() : 0) * 397) ^ (Domain != null ? Domain.GetHashCode() : 0);
                }
            }

            public static bool operator ==(Descriptor left, Descriptor right)
            {
                return left.Equals(right);
            }

            public static bool operator !=(Descriptor left, Descriptor right)
            {
                return !left.Equals(right);
            }
        }
        public class ReportItem
        {
            public string Identifier { get; set; }
            public string Type { get; set; }
            public int SecondsSpent { get; set; }
            public int Percent { get; set; }
            public Descriptor Descriptor { get; set; }

            public ReportItem() { }
            public ReportItem(string identifier, string type, int secondsSpent, Descriptor desc)
            {
                Identifier = identifier;
                Type = type;
                SecondsSpent = secondsSpent;
                Descriptor = desc;
            }

            public static ReportItem MakeItemFromDescriptor(Descriptor desc, int secondsSpent)
            {
                if (desc.Domain == null)
                {
                    //this is process
                    return new ReportItem(desc.ProcessName, "process", secondsSpent, desc);
                }
                else
                {
                    return new ReportItem(desc.Domain, "site", secondsSpent, desc);
                }
            }
        }

        public class ReportResult : BaseReportResultCollection<ReportItem>
        {
            public override ReportItem[] Items { get; set; }

            public ReportResult() { }
            public ReportResult(ReportItem[] items)
            {
                Items = items;
            }

            public static ReportResult Merge(IEnumerable<ReportResult> results)
            {
                Dictionary<Descriptor, int> dict = new Dictionary<Descriptor, int>();
                var allItems = results.SelectMany(t => t.Items);
                foreach (var item in allItems)
                {
                    if (!dict.ContainsKey(item.Descriptor))
                        dict[item.Descriptor] = 0;
                    dict[item.Descriptor] += item.SecondsSpent;
                }
                var pairs = dict.OrderByDescending(t => t.Value);
                List<ReportItem> newItems = new List<ReportItem>();
                foreach (var pair in pairs)
                {
                    newItems.Add(ReportItem.MakeItemFromDescriptor(pair.Key, pair.Value));
                }
                ReportItem[] arr = newItems.ToArray();
                RecalculatePercentages(arr);
                return new ReportResult(arr);
            }

            public static void RecalculatePercentages(ReportItem[] items)
            {
                int total = items.Sum(t => t.SecondsSpent);
                foreach (var reportItem in items)
                {
                    reportItem.Percent = reportItem.SecondsSpent * 100 / total;
                }
            }
        }

        public class Params
        {
            public int MinTime { get; set; } = 60 * 3; //3 mins
        }

        public Params Parameters { get; }
        public UnknownIdentificationReport(ILog log) : base(log)
        {
            Parameters = new Params();
        }

        public override ReportResult Calculate()
        {
            LogRecord[] records = log.Records;
            //get only active records
            bool[] actives = new ActiveReport(log).GetActivitiesOnly().ToArray();
            records = records.Where((t, index) => actives[index]).ToArray();

            Dictionary<Descriptor,int> times = new Dictionary<Descriptor, int>();
            foreach (var logRecord in records)
            {
                if (log.Prof.FindIdentifier(logRecord) == null)
                {
                    //this app is unknown
                    var url = logRecord.GetMetaString("url");
                    string domain = Util.GetHostFromUrl(url);
                    if (domain == "")
                    {
                        domain = null;
                    }
                    Descriptor desc = new Descriptor(logRecord.Process.ProcessName,domain);
                    if (!times.ContainsKey(desc))
                    {
                        times[desc] = 0;
                    }
                    times[desc]++;
                }
            }
            var orderedPairs = times.Where(t=>t.Value >= Parameters.MinTime).OrderByDescending(t => t.Value);
            List<ReportItem> items = new List<ReportItem>();
            foreach (var keyValuePair in orderedPairs)
            {
                items.Add(ReportItem.MakeItemFromDescriptor(keyValuePair.Key,keyValuePair.Value));
            }
            ReportItem[] resArr = items.ToArray();
            //fill percents
            ReportResult.RecalculatePercentages(resArr);

            var result = new ReportResult(resArr);
            TryCacheResult(result);
            return result;

        }
    }
}
