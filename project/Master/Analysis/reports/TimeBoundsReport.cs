using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;
using TimeMiner.Master.Frontend.Plugins;

namespace TimeMiner.Master.Analysis.reports
{
    /// <summary>
    /// Report that helps to find bounds of active periods
    /// </summary>
    public class TimeBoundsReport : ParametrizedBaseReport<TimeBoundsReport.ReportResult, TimeBoundsReport.ReportParameters>
    {
        public class ReportItem
        {
            public DateTime Start { get; set; }
            public DateTime End { get; set; }

            public TimeSpan Length
            {
                get { return End - Start; }
            }

            public ReportItem()
            {
            }

            public ReportItem(DateTime start, DateTime end)
            {
                Start = start;
                End = end;
            }
        }

        public class ReportResult : BaseReportResultCollection<ReportItem>
        {
            public override ReportItem[] Items { get; set; }

            public ReportResult(ReportItem[] items)
            {
                Items = items;
            }

            public ReportResult()
            {
            }
        }

        public class ReportParameters
        {
            /// <summary>
            /// Periods with whole of less than given seconds will be merged to big one
            /// </summary>
            public int PeriodMergeIntervalSecs { get; set; } = 60 * 5;
            /// <summary>
            /// After merging, periods with lengths less than given will be removed
            /// </summary>
            public int PeriodIgnoreMinSecs { get; set; } = 60 * 5;
        }


        public TimeBoundsReport(ILog log) : base(log)
        {
            parameters = new ReportParameters();
        }

        public override ReportResult Calculate()
        {
            ActiveReport activeReport = new ActiveReport(log);
            var activeItems = activeReport.Calculate().Items;
            var periods = GetActivePeriods(activeItems).ToList();
            List<ReportItem> resultItems = new List<ReportItem>(periods);

            /*int firstActive = changing.FindIndex(t => t.IsActive);
            for (int i = firstActive; i < changing.Count-1; i+=2)
            {
                resultItems.Add(new ReportItem(changing[i].Rec.Time, changing[i+1].Rec.Time));
            }*/
            resultItems = new List<ReportItem>(MergePeriods(resultItems, parameters.PeriodMergeIntervalSecs));
            resultItems = new List<ReportItem>(FilterPeriods(resultItems, parameters.PeriodIgnoreMinSecs));
//            DateTime begin = default(DateTime);
//            ActiveReport.ReportItem previous = null;
//            foreach (var activity in activities)
//            {
//                //check if this record is too far from previous
//                if (previous != null && (activity.Rec.Time - previous.Rec.Time).TotalSeconds > MAX_RECORD_TIME_DELTA)
//                {
//                    //it seems we are not active
//                }
//                if (activity.IsActive)
//                {
//                    //given record is active
//                    if (previous == null || !previous.IsActive)
//                    {
//                        begin = activity.Rec.Time;
//                    }
//                }
//                else
//                {
//                    //this is inactive
//                    if (previous != null && previous.IsActive)
//                    {
//                        result.Add(new ReportItem(begin, activity.Rec.Time));
//                    }
//                }
//                previous = activity;
//            }
            var result = new ReportResult(resultItems.ToArray());
            TryCacheResult(result);
            return result;
//            List<ReportItem> items = new List<ReportItem>();
//            items.Add(new ReportItem(DateTime.Now.StartOfDay().AddHours(9), DateTime.Now.StartOfDay().AddHours(10)));
//            items.Add(new ReportItem(DateTime.Now.StartOfDay().AddHours(12), DateTime.Now.StartOfDay().AddHours(17)));
//            items.Add(new ReportItem(DateTime.Now.StartOfDay().AddHours(19), DateTime.Now.StartOfDay().AddHours(21)));
//            return new ReportResult(items.ToArray());
        }

        private IEnumerable<ReportItem> FilterPeriods(IEnumerable<ReportItem> items, int minLenSeconds)
        {
            return items.Where(t => t.Length.TotalSeconds > minLenSeconds);
        }
        private IEnumerable<ReportItem> MergePeriods(IEnumerable<ReportItem> items, int maxSkipSeconds)
        {
            Queue<ReportItem> itemsQueue = new Queue<ReportItem>(items);
            while (itemsQueue.Count > 0)
            {
                ReportItem item = itemsQueue.Dequeue();
                while (itemsQueue.Count > 0)
                {
                    ReportItem nextItem = itemsQueue.Peek();
                    if ((nextItem.Start - item.End).TotalSeconds < maxSkipSeconds)
                    {
                        //this one is near, append it
                        item.End = nextItem.End;
                        //remove from queue
                        itemsQueue.Dequeue();
                    }
                    else
                    {
                        //this one is too far
                        break;
                    }
                }
                yield return item;
            }
        }

        private IEnumerable<ReportItem> GetActivePeriods(ActiveReport.ReportItem[] items)
        {
            if(items.Length==0)
                yield break;
            const int MAX_DELTA_SECONDS = 10;
            const int MIN_ITEMS = 10;
            var composed= items.ComposeBy(t => t.IsActive);
            var splitted =
                composed.SelectMany(
                    t => t.SplitBy((a, b) => (Math.Abs((a.Rec.Time - b.Rec.Time).TotalSeconds) > MAX_DELTA_SECONDS)));
            foreach (var arr in splitted)
            {
                if(arr.Length == 0)
                    continue;
                if(!arr[0].IsActive)
                    continue;
                DateTime start = arr[0].Rec.Time;
                DateTime end = arr[arr.Length - 1].Rec.Time;
                yield return new ReportItem(start,end);
            }
        }
    }
}
