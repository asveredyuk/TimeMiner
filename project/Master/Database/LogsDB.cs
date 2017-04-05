using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MsgPack.Serialization;
using TimeMiner.Core;
using TimeMiner.Master.Database;

namespace TimeMiner.Master
{
    /// <summary>
    /// Storage for all logs in system
    /// </summary>
    public class LogsDB
    {
        /// <summary>
        /// Directory containing log files
        /// </summary>
        public static string LOGS_DIR = "logs";
        /// <summary>
        /// File name pattern. {0} - userid, {1} - date in format DDMMYYYY
        /// </summary>
        const string LOG_FNAME_PATTERN= "log_u{0}_{1}.storage";

        /// <summary>
        /// Temporary, storages for the user #0 for all dates
        /// </summary>
        private List<CachedStorage> storages0;
        /// <summary>
        /// Create new LogsDB object
        /// </summary>
        public LogsDB()
        {
            if (!Directory.Exists(LOGS_DIR))
                Directory.CreateDirectory(LOGS_DIR);
            storages0 = new List<CachedStorage>();
            //create cached storages for all existing files
            storages0.AddRange(Directory.GetFiles(LOGS_DIR,"*.storage").Select(t=>new CachedStorage(t)));
        }
        /// <summary>
        /// Create new storage, appropriate for given record
        /// </summary>
        /// <param name="rec"></param>
        /// <returns></returns>
        private CachedStorage CreateNewStoreageForRecord(LogRecord rec)
        {
            string fname = MakeStorageFileName(rec.UserId,rec.Time);
            if (File.Exists(fname))
            {
                throw new Exception("Given log already exists");
            }
            CachedStorage storage = new CachedStorage(fname);
            lock (storages0)
            {
                storages0.Add(storage);
            }
            return storage;
        }
        /// <summary>
        /// Find appropriate existing storage for given log record
        /// </summary>
        /// <param name="rec"></param>
        /// <returns>Storage or null if not found</returns>
        private CachedStorage FindStorageForRecord(LogRecord rec)
        {
            lock (storages0)
            {
                return storages0.Find(t => t.Descriptor.CheckAcceptsLogRecord(rec));
            }
        }
        /// <summary>
        /// Put new record to the storage of acossiated user
        /// </summary>
        /// <param name="rec"></param>
        public void PutRecord(LogRecord rec)
        {
            CachedStorage storage = FindStorageForRecord(rec);
            if (storage == null)
            {
                storage = CreateNewStoreageForRecord(rec);
            }
            storage.PutRecord(rec);
        }

        /// <summary>
        /// Get all records for given user
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="cacheResults">Should read results be cached or not</param>
        /// <returns></returns>
        public List<LogRecord> GetAllRecordsForUser(int userid, bool cacheResults = true)
        {
            lock (storages0)
            {
                var all = storages0.Select(t => t.GetRecords(cacheResults)).SelectMany(t => t);
                return new List<LogRecord>(all);
            }
        }

        /// <summary>
        /// Make name of storage file for given user
        /// </summary>
        /// <param name="userid">id of user</param>
        /// <param name="date">date of log record</param>
        /// <returns>relative path to the log file</returns>
        private static string MakeStorageFileName(int userid, DateTime date)
        {
            string dateStr = date.ToString("ddMMyy");
            return LOGS_DIR + "/" + string.Format(LOG_FNAME_PATTERN, userid,dateStr);
        }
    }
}
