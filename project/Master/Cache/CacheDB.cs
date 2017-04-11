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
        private static CacheDB self;

        public static CacheDB Self
        {
            get
            {
                if(self == null)
                    self = new CacheDB();
                return self;
            }
        }

        private LiteDatabase db;
        private LiteCollection<ReportResultCacheItem> col;
        private CacheDB()
        {
            db = new LiteDatabase(DB_PATH);
            col = db.GetCollection<ReportResultCacheItem>("cache");
        }

        public T FindReportInCache<T>(string fileMd5) where T: BaseReportResult
        {
            Type t = typeof(T);
            var item = FindItemInCache(fileMd5, t);
            return item?.Result as T;
        }
        public ReportResultCacheItem FindItemInCache(string fileMd5, Type reportType)
        {
            var item = col.FindOne(t => t.FileHash == fileMd5 && t.ReportTypeGuid == reportType.GUID);
            return item;
        }
        public void PutToCache<T>(T report, StorageDescriptor desc) where T : BaseReportResult
        {
            var item = new ReportResultCacheItem(desc.FileMD5, "", typeof(T).GUID, report);
            col.Upsert(item);
        }
    }
}
