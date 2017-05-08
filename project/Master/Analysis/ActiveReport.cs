using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeMiner.Core;

namespace TimeMiner.Master.Analysis
{
    /// <summary>
    /// Report for decting when user was active or not
    /// </summary>
    public class ActiveReport : BaseReport<ActiveReport.ReportResult>
    {
        public class ReportItem
        {
            public LogRecord Rec { get; set; }
            public bool IsActive { get; set; }

            public ReportItem(LogRecord rec, bool isActive)
            {
                Rec = rec;
                IsActive = isActive;
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
        /// <summary>
        /// Create new report
        /// </summary>
        /// <param name="log">log to analyze</param>
        public ActiveReport(ILog log):base(log)
        {
        }

        public override ReportResult Calculate()
        {
            ReportItem[] items = GetActivities().ToArray();
            return new ReportResult(items);
        }
        /// <summary>
        /// Get collection of activities per each log record
        /// </summary>
        /// <returns></returns>
        public IEnumerable<ReportItem> GetActivities()
        {
            for (int i = 0; i < log.Records.Length; i++)
            {
                bool res = AnalyzeRecord(i);
                yield return new ReportItem(log.Records[i],res);
            }
        }
        /// <summary>
        /// Get collection of activities per each log record
        /// </summary>
        /// <returns></returns>
        public IEnumerable<bool> GetActivitiesOnly()
        {
            for (int i = 0; i < log.Records.Length; i++)
            {
                bool res = AnalyzeRecord(i);
                yield return res;
            }
        }
        /// <summary>
        /// Analyze one log record
        /// </summary>
        /// <param name="index">index of log record to analyze</param>
        /// <returns></returns>
        private bool AnalyzeRecord(int index)
        {
            const int LAST_NUM = 20;
            const float MIN_ACTIVITY_SCORE = 1f;

            const float KEYPRESS_DIVIDER = 5;
            const float MOUSE_ACTION_DIVIDER = 5;
            const float MOUSEDIF_DIVIDER = 300;
            //const int MIN_KEYS = 2;
            //const int MIN_MOUSEDIF = 50;
            LogRecord current = log.Records[index];
            IEnumerable<LogRecord> prev = GetPreviouses(index, LAST_NUM);
            int keysPressed = prev.Sum(e => e.Keystrokes) + current.Keystrokes;
            int mouseActions = prev.Sum(e => e.MouseButtonActions + e.MouseWheelActions) + current.MouseButtonActions + current.MouseWheelActions;
            IntPoint mousepos = current.MousePosition;
            int mouseDif = 0;
            foreach (var previousElement in prev)
            {
                mouseDif += Math.Abs(mousepos.X - previousElement.MousePosition.X) +
                            Math.Abs(mousepos.Y - previousElement.MousePosition.Y);
                mousepos = previousElement.MousePosition;
            }
            //float keypress_score = keysPressed/KEYPRESS_DIVIDER;
            float score = Math.Min(keysPressed / KEYPRESS_DIVIDER, 1) + Math.Min(mouseActions / MOUSE_ACTION_DIVIDER, 1) +
                          Math.Min(mouseDif / MOUSEDIF_DIVIDER, 1);
            return score >= MIN_ACTIVITY_SCORE;
        }
        /// <summary>
        /// Get N previous elements
        /// </summary>
        /// <param name="index">index of current element</param>
        /// <param name="num">number of previous elements</param>
        /// <returns></returns>
        private IEnumerable<LogRecord> GetPreviouses(int index, int num)
        {
            for (int i = index-1; i >= index - num && i >= 0; i--)
            {
                yield return log.Records[i];
            }
        }

        
    }
}
