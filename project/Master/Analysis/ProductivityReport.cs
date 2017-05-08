using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using TimeMiner.Master.Settings;

namespace TimeMiner.Master.Analysis
{
    /// <summary>
    /// Report, that gives overall time statistics
    /// </summary>
    public class ProductivityReport : BaseReport<ProductivityReport.ReportResult>
    {
        /// <summary>
        /// Result of report
        /// </summary>
        public class ReportResult:BaseReportResult
        {
            /// <summary>
            /// Number of seconds, that were productive
            /// </summary>
            public int ProductiveTime { get; set; }
            /// <summary>
            /// Number of seconds, that were distractions
            /// </summary>
            public int DistractionsTime { get; set; }
            /// <summary>
            /// Total time spent
            /// </summary>
            public int TotalTime { get { return ProductiveTime + DistractionsTime; } }
        }
        /// <summary>
        /// Crete new productivity report
        /// </summary>
        /// <param name="log">Log to analyze</param>
        public ProductivityReport(Log log):base(log)
        {
        }

        public override ReportResult Calculate()
        {
            ActiveReport active = new ActiveReport(log);
            ProgramUsageReport usage = new ProgramUsageReport(log);
            usage.Parameters.ActiveReport = active;
            ProgramUsageReport.ReportItem[] usages = usage.Calculate().Items;

            int productive = usages.Where(t => t.Desc.Rel == Relevance.good).Select(t => t.SecondsSpent).Sum();
            int neutral = usages.Where(t => t.Desc.Rel == Relevance.neutral).Select(t => t.SecondsSpent).Sum();
            int totalProductive = productive + neutral /2;
            int totalDistractions = usages.Select(t => t.SecondsSpent).Sum() - totalProductive;

            var result = new ReportResult()
            {
                ProductiveTime = totalProductive,
                DistractionsTime = totalDistractions
            };
            CacheResult(result);
            return result;
        }
    }
}
