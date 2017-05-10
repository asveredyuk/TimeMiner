using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeMiner.Master.Analysis
{
    /// <summary>
    /// Base class for all report results, that represent collections of results
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class BaseReportResultCollection<T> : BaseReportResult
    {
        public abstract T[] Items { get; set; }
    }
}
