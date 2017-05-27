using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeMiner.Master.Database;

namespace TimeMiner.Master.Analysis
{
    /// <summary>
    /// Base class for all reports, use only if there is no parameters
    /// </summary>
    /// <typeparam name="T">Type of report result</typeparam>
    public abstract class BaseReport<T> where T:BaseReportResult
    {
        protected ILog log;

        protected BaseReport(ILog log)
        {
            this.log = log;
        }

        /// <summary>
        /// Calculate the result of report
        /// </summary>
        /// <returns>Report result</returns>
        public abstract T Calculate();

        /// <summary>
        /// Get report from cache, or calculate, if it does not exist
        /// </summary>
        /// <param name="d"></param>
        /// <returns></returns>
        public virtual T GetFromCacheOrCalculate()
        {
            T rep = null;
            if(log.DataHash!= null)
                rep = CacheDB.Self.FindReportInCache<T>(log.DataHash, log.Prof.ComputeHash(), GetParamsHash());
            if (rep == null)
                rep = Calculate();
            return rep;
        }

        /// <summary>
        /// Add result to cache
        /// </summary>
        /// <param name="result">Report result</param>
        protected virtual void TryCacheResult(T result)
        {
            if (log.DataHash == null)
            {
                return;
               //throw new Exception("Tried to add to cache log without data hash");
                //cannot cache this
                //return;
            }
            if (log.Prof == null)
            {
                return;
                //throw new Exception("Tried to add to cache log without profile");
                //cannot cache this
                //return;
            }
            CacheDB.Self.PutToCache(result,log.DataHash, log.Prof.ComputeHash(), GetParamsHash());
        }

        protected virtual string GetParamsHash()
        {
            return "";
        }
    }
}
