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
            storages0.AddRange(CachedStorage.LoadAllStorages(LOGS_DIR));
        }
        /// <summary>
        /// Create new storage, appropriate for given record
        /// </summary>
        /// <param name="rec"></param>
        /// <returns></returns>
        private CachedStorage CreateNewStoreageForRecord(LogRecord rec)
        {
            Guid userId = rec.UserId;
            DateTime date = rec.Time.Date;
            CachedStorage storage = CachedStorage.CreateNewStorage(userId, date, LOGS_DIR);
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
            //convert time to local
            rec.Time = rec.Time.ToLocalTime();
            //put record to concrete storage
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
        public List<LogRecord> GetAllRecordsForUser(Guid userid, bool cacheResults = true)
        {
            //TODO: filter by user
            lock (storages0)
            {
                var all = storages0.Select(t => t.GetRecords(cacheResults)).SelectMany(t => t);
                return all.ToList();
            }
        }

        //TODO: make ability to get arrays per each file
        //public public List<LogRecord[]> GetSplitrecordsForUserForPeriod(int userid)

        public List<LogRecord> GetLogRecordsForUserForPeriod(Guid userid, DateTime timeFrom, DateTime timeTo,
            bool cacheResults = true)
        {
            //TODO: filter by user
            List<CachedStorage> neededStorages;
            lock (storages0)
            {
                var needed = storages0.Where(t => t.Descriptor.CheckInterceptionWithPeriod(timeFrom, timeTo));
                neededStorages = needed.ToList();
            }
            var all = neededStorages.Select(t => t.GetRecords(cacheResults)).SelectMany(t=>t);
            var inPeriod = all.Where(t => Util.CheckDateInPeriod(t.Time, timeFrom, timeTo));
            return inPeriod.ToList();
        }
        
    }
}
