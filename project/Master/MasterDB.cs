using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiteDB;
using TimeMiner.Core;

namespace TimeMiner.Master
{
    class MasterDB : IDisposable
    {
        private static MasterDB self;

        public static MasterDB Self
        {
            get
            {
                if (self == null)
                {
                    self = new MasterDB();
                }
                return self;
            }
        }

        const string LOGS_TABLES_PREFIX = "log_u";

        /// <summary>
        /// Database connection
        /// </summary>
        LiteDatabase db;

        private MasterDB()
        {
            db = new LiteDatabase("logstorage.db");
        }
        /// <summary>
        /// Put new record to the database to the table of record user
        /// </summary>
        /// <param name="rec"></param>
        public void PutRecord(LogRecord rec)
        {
            var col = db.GetCollection<LogRecord>(LOGS_TABLES_PREFIX + rec.UserId);
            col.EnsureIndex(x => x.Id);
            if (col.Exists(x => x.Id == rec.Id))
            {
                throw new Exception("Such item ");
            }
            col.Insert(rec);
        }

        /// <summary>
        /// Get all records for given user
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
        public List<LogRecord> GetAllRecordsForUser(int userid)
        {
            var col = db.GetCollection<LogRecord>(LOGS_TABLES_PREFIX + userid);
            return new List<LogRecord>(col.FindAll());
        }

        ~MasterDB()
        {
            Dispose();
        }

        public void Dispose()
        {
            if (db != null)
            {
                db.Dispose();
                db = null;
            }
        }
    }
}
