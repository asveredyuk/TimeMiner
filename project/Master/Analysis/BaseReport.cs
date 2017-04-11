﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeMiner.Master.Database;

namespace TimeMiner.Master.Analysis
{
    /// <summary>
    /// Base class for all reports
    /// </summary>
    /// <typeparam name="T">Type of report result</typeparam>
    public abstract class BaseReport<T> where T:BaseReportResult
    {
        protected Log log;

        protected BaseReport(Log log)
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
        public T GetFromCacheOrCalculate()
        {
            T rep = null;
            if(log.StorageDescriptor != null)
                rep = CacheDB.Self.FindReportInCache<T>(log.StorageDescriptor.FileMD5);
            if (rep == null)
                rep = Calculate();
            return rep;
        }

        /// <summary>
        /// Add result to cache
        /// </summary>
        /// <param name="result"></param>
        /// <param name="descriptor"></param>
        protected void CacheResult(T result, StorageDescriptor descriptor)
        {
            CacheDB.Self.PutToCache(result,descriptor);
        }
    }
}
