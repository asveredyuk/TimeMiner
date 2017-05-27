using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiteDB;
using TimeMiner.Master.Analysis;
using TimeMiner.Master.Cache;

namespace TimeMiner.Master.Database
{
    public class CacheDB
    {
        
        public static string DB_PATH = "cache.db";

        /// <summary>
        /// Lock, to prevent multiple initialization
        /// </summary>
        private static readonly object _lock = new object();

        /// <summary>
        /// Singleton instance
        /// </summary>
        private static CacheDB self;

        /// <summary>
        /// Singleton instance
        /// </summary>
        public static CacheDB Self
        {
            get
            {
                lock (_lock)
                {
                    if (self == null)
                    {
                        self = new CacheDB();
                    }
                    return self;
                }
            }
        }

        private LiteDatabase db;
        private LiteCollection<ReportResultCacheItem> col;
        private CacheDB()
        {
            db = new LiteDatabase(DB_PATH);
            col = db.GetCollection<ReportResultCacheItem>("cache");
        }

        public T FindReportInCache<T>(string dataMd5, string profileMd5, string paramsMd5) where T: BaseReportResult
        {
            Type t = typeof(T);
            var item = FindItemInCache(dataMd5, profileMd5, paramsMd5, t);
            return item?.Result as T;
        }
        public ReportResultCacheItem FindItemInCache(string dataMd5, string profileMd5, string paramsMd5, Type reportType)
        {
            var item = col.FindOne(t => t.DataHash == dataMd5 && t.ProfileHash == profileMd5 && t.ReportTypeGuid == reportType.GUID && t.ParametersHash == paramsMd5);
            return item;
        }
        public void PutToCache<T>(T report, string dataMd5, string profileMd5, string paramsMd5) where T : BaseReportResult
        {
            var item = new ReportResultCacheItem(dataMd5, profileMd5, paramsMd5, typeof(T).GUID, report);
            col.Upsert(item);
        }

        public void ClearAndShrink()
        {
            var cacheAll = col.FindAll().ToList();
            foreach (var item in cacheAll)
            {
                col.Delete(item.Id);
            }
            db.Shrink();
        }
    }
}
