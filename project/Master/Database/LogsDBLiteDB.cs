﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiteDB;
using TimeMiner.Core;

namespace TimeMiner.Master
{
    public class LogsDBLiteDb
    {
        const string LOGS_TABLES_PREFIX = "log_u";
        public static string LOG_DB_PATH = "logstorage.db";

        [Obsolete("never work with database directly")]
        public LiteDatabase Database
        {
            get { return db; }
        }
        /// <summary>
        /// Database connection
        /// </summary>
        LiteDatabase db;

        internal LogsDBLiteDb()
        {
            db = new LiteDatabase(LOG_DB_PATH);
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
            //TODO: think about this
            return new List<LogRecord>(col.FindAll().OrderBy(t => t.Time));
        }


        /*public void Dispose()
        {
            if (db != null)
            {
                db.Dispose();
                db = null;
            }
        }*/
    }
}
