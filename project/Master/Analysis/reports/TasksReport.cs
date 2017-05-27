using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeMiner.Master.Analysis.reports
{
    public class TasksReport : BaseReport<TasksReport.ReportResult>
    {
        public class ReportItem
        {
            public DateTime Begin { get; set; }
            public DateTime End { get; set; }
            public string Name { get; set; }

            public ReportItem(DateTime begin, DateTime end, string name)
            {
                Begin = begin;
                End = end;
                Name = name;
            }

            public ReportItem()
            {
            }
        }

        public class ReportResult : BaseReportResultCollection<ReportItem>
        {
            public override ReportItem[] Items { get; set; }

            public ReportResult()
            {
            }

            public ReportResult(ReportItem[] items)
            {
                Items = items;
            }
        }


        public TasksReport(ILog log) : base(log)
        {
        }

        public override ReportResult Calculate()
        {
            var taskOnly = log.Records.Where(t => t.GetMetaString("task") != null);
            var composed = taskOnly.ComposeBy(t => t.GetMetaString("task"));
            List<ReportItem> results = new List<ReportItem>();
            foreach (var arr in composed)
            {
                if(arr.Length == 1)
                    continue;
                DateTime begin = arr[0].Time;
                DateTime end = arr[arr.Length - 1].Time;
                string name = arr[0].GetMetaString("task");
                results.Add(new ReportItem(begin,end,name));
            }
            var res = new ReportResult(results.ToArray());
            TryCacheResult(res);
            return res;
        }
    }
}
