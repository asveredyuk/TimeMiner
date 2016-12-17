using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using LiteDB;
using TimeMiner.Core;

namespace TimeMiner.Slave
{
    public class SlaveDB : IDisposable
    {
        private static SlaveDB self;

        public static SlaveDB Self
        {
            get
            {
                if(self == null)
                    self = new SlaveDB();
                return self;
            }
        }

        /// <summary>
        /// Handler for onLogRecordAdded event
        /// </summary>
        /// <param name="item"></param>
        /// <param name="db"></param>
        public delegate void OnLogRecordAdded(LogRecord item, SlaveDB db);
        /// <summary>
        /// Database instance
        /// </summary>
        LiteDatabase db;
        /// <summary>
        /// Main collection of records to send to master
        /// </summary>
        LiteCollection<LogRecord> col; 
        /// <summary>
        /// Happens when log record is added
        /// </summary>
        public event OnLogRecordAdded onLogRecordAdded;
        private SlaveDB()
        {
            db = new LiteDatabase("logstorage.db");
            col = db.GetCollection<LogRecord>("log");
            col.EnsureIndex(x => x.Id);
            //col.EnsureIndex(x => x.Time);
        }
        ~SlaveDB()
        {
            Dispose();
        }
        /// <summary>
        /// Get all logs in local storage
        /// </summary>
        /// <returns></returns>
        public List<LogRecord> GetAllLogs()
        {
            return new List<LogRecord>(col.FindAll().OrderBy(t=>t.Time));
        }
        /// <summary>
        /// Get one record by given guid
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public LogRecord GetRecordById(Guid id)
        {
            if (id == Guid.Empty)
                return null;
            LogRecord rec = col.FindById(id);
            return rec;
        }
        /// <summary>
        /// Add new log record
        /// </summary>
        /// <param name="rec"></param>
        public void AddLogRecord(LogRecord rec)
        {
            if (col.Exists(x => x.Id == rec.Id))
            {
                throw new Exception("Such item already exist" + rec.ToString());
            }
            col.Insert(rec);
            RaiseOnLogRecordAdded(rec);
        }
        /// <summary>
        /// Remove log record from local storage
        /// </summary>
        /// <param name="rec"></param>
        public void RemoveLogRecord(LogRecord rec)
        {
            col.Delete(rec.Id);
            //var res = col.Delete(t=>t.Id == rec.Id);
        }
        public void Dispose()
        {
            if (db != null)
            {
                db.Dispose();
                db = null;
            }
        }
        /// <summary>
        /// Raise log record added event
        /// </summary>
        /// <param name="rec"></param>
        private void RaiseOnLogRecordAdded(LogRecord rec)
        {
            if (onLogRecordAdded != null)
                onLogRecordAdded(rec, this);
        }
    }
}
