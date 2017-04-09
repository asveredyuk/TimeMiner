using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeMiner.Master.Analysis
{
    /// <summary>
    /// Base class for all reports
    /// </summary>
    /// <typeparam name="T">Type of report result</typeparam>
    public abstract class BaseReport<T> where T:BaseReportResult
    {
        /// <summary>
        /// Calculate the result of report
        /// </summary>
        /// <returns>Report result</returns>
        public abstract T Calculate();
    }
}
