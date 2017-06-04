using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Animation;
using Newtonsoft.Json;
using TimeMiner.Core.MetaInfoClasses;

namespace TimeMiner.Master.Analysis.reports
{
    public class TasksReport : BaseReport<TasksReport.ReportResult>
    {
        public class ReportItem
        {
            public DateTime Begin { get; set; }
            public DateTime End { get; set; }
            public TaskDescription Task { get; set; }

            public ReportItem(DateTime begin, DateTime end, TaskDescription task)
            {
                Begin = begin;
                End = end;
                Task = task;
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
            //TODO: more complex separation
            //EX : task is set, then nothing, then same task. This will look like one task
            //EX : problem with the night (or other break). Try to check minimal distance between objects and split by
            //TODO: implement "splitBy", which splits collections if condition happened
            var taskOnly = log.Records.Where(t => t.GetMetaString("task") != null);
            var composed = taskOnly.ComposeBy(t => t.GetMetaString("task"));
            List<ReportItem> results = new List<ReportItem>();
            foreach (var arr in composed)
            {
                if(arr.Length == 1)
                    continue;
                DateTime begin = arr[0].Time;
                DateTime end = arr[arr.Length - 1].Time;
                var taskDesc = JsonConvert.DeserializeObject<TaskDescription>(arr[0].GetMetaString("task"));
                //string name = arr[0].GetMetaString("task");
                results.Add(new ReportItem(begin,end,taskDesc));
            }
            var res = new ReportResult(results.ToArray());
            TryCacheResult(res);
            return res;
        }
    }
}
