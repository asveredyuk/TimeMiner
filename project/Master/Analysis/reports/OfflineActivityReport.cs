using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using TimeMiner.Core.MetaInfoClasses;

namespace TimeMiner.Master.Analysis.reports
{
    public class OfflineActivityReport : BaseReport<OfflineActivityReport.ReportResult>
    {
        public class ReportResult : BaseReportResultCollection<OfflineActivity>
        {
            public override OfflineActivity[] Items { get; set; }

            public ReportResult()
            {
            }

            public ReportResult(OfflineActivity[] items)
            {
                Items = items;
            }
        }

        public OfflineActivityReport(ILog log) : base(log)
        {
        }

        public override ReportResult Calculate()
        {
            var itemsJson = log.Records.Select(t => t.GetMetaString(OfflineActivity.TAG));
            var itemsJsonNonNull = itemsJson.Where(t => t != null);
            var items = itemsJsonNonNull.Select(t => JsonConvert.DeserializeObject<OfflineActivity>(t));
            var report = new ReportResult(items.ToArray());
            //try cache result
            return report;
        }
    }
}
