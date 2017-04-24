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

        public T FindReportInCache<T>(string fileMd5, string profileMd5) where T: BaseReportResult
        {
            Type t = typeof(T);
            var item = FindItemInCache(fileMd5,profileMd5, t);
            return item?.Result as T;
        }
        public ReportResultCacheItem FindItemInCache(string fileMd5, string profileMd5, Type reportType)
        {
            var item = col.FindOne(t => t.FileHash == fileMd5 && t.ProfileHash == profileMd5 && t.ReportTypeGuid == reportType.GUID);
            return item;
        }
        public void PutToCache<T>(T report, string fileMD5, string profileMd5) where T : BaseReportResult
        {
            var item = new ReportResultCacheItem(fileMD5, profileMd5, typeof(T).GUID, report);
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
