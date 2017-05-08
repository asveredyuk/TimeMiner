using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MsgPack.Serialization;
using TimeMiner.Core;
using TimeMiner.Master.Analysis;
using TimeMiner.Master.Database;
using TimeMiner.Master.Settings;

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
        /// Lock, to prevent multiple initialization
        /// </summary>
        private static readonly object _lock = new object();

        /// <summary>
        /// Singleton instance
        /// </summary>
        private static LogsDB self;

        /// <summary>
        /// Singleton instance
        /// </summary>
        public static LogsDB Self
        {
            get
            {
                lock (_lock)
                {
                    if (self == null)
                    {
                        self = new LogsDB();
                    }
                    return self;
                }
            }
        }
        
        /// <summary>
        /// Temporary, storages for the user #0 for all dates
        /// </summary>
        private List<CachedStorage> storages0;
        /// <summary>
        /// Create new LogsDB object
        /// </summary>
        private LogsDB()
        {
            if (!Directory.Exists(LOGS_DIR))
                Directory.CreateDirectory(LOGS_DIR);
            storages0 = new List<CachedStorage>();
            //create cached storages for all existing files
            storages0.AddRange(CachedStorage.LoadAllStorages(LOGS_DIR));
            //Refresh all descriptors at start
            foreach (var cachedStorage in storages0)
            {
                cachedStorage.RefreshDescriptor();
            }
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
            rec.Time = ConvertDatetimeToStorage(rec.Time);
            //put record to concrete storage
            CachedStorage storage = FindStorageForRecord(rec);
            if (storage == null)
            {
                storage = CreateNewStoreageForRecord(rec);
            }
            storage.PutRecord(rec);
            //descriptor is refreshed
            storage.RefreshDescriptor();
        }

        /// <summary>
        /// Put many records in one transaction. This method is better for large amounts of records
        /// </summary>
        /// <param name="recs"></param>
        public void PutManyRecords(IEnumerable<LogRecord> recs)
        {
            //Accumulates log records per cached storage
            Dictionary<CachedStorage, List<LogRecord>> dict = new Dictionary<CachedStorage, List<LogRecord>>();

            foreach (var logRecord in recs)
            {
                logRecord.Time = ConvertDatetimeToStorage(logRecord.Time);
                CachedStorage storage = FindStorageForRecord(logRecord);
                if (storage == null)
                {
                    storage = CreateNewStoreageForRecord(logRecord);
                }
                if (!dict.ContainsKey(storage))
                    dict[storage] = new List<LogRecord>();
                dict[storage].Add(logRecord);
            }
            foreach (var pair in dict)
            {
                pair.Key.PutManyRecords(pair.Value);
            }
            //After all logs were imported, refresh descriptors
            foreach (var pair in dict)
            {
                pair.Key.RefreshDescriptor();
            }
        }
        /// <summary>
        /// Get all records for given user
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="cacheResults">Should read results be cached or not</param>
        /// <returns></returns>
        [Obsolete("Database is large, do not try to load it all")]
        public List<LogRecord> GetAllRecordsForUser(Guid userid, bool cacheResults = true)
        {
            //TODO: filter by user
            lock (storages0)
            {
                var all = storages0.Select(t => t.GetRecords(cacheResults)).SelectMany(t => t);
                return all.ToList();
            }
        }
        /// <summary>
        /// Get logs for user for given period. Logs are separated by storage
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="timeFrom"></param>
        /// <param name="timeTo"></param>
        /// <param name="cacheResults"></param>
        /// <returns></returns>
        public ILog[]  GetLogsForUserForPeriodSeparate(Guid userId, DateTime timeFrom, DateTime timeTo,
            bool cacheResults = true)
        {
            timeFrom = ConvertDatetimeToStorage(timeFrom);
            timeTo = ConvertDatetimeToStorage(timeTo);
            List<CachedStorage> neededStorages;
            lock (storages0)
            {
                var forThisUser = storages0.Where(t => t.Descriptor.UserId == userId);
                var needed = forThisUser.Where(t => t.Descriptor.CheckInterceptionWithPeriod(timeFrom, timeTo));
                neededStorages = needed.ToList();
            }
            var inPeriod = neededStorages.Select(
                t=>MakeLogFromStorageInPeriod(t,timeFrom,timeTo,cacheResults)
                ).ToArray();
            return inPeriod;
        }

        private ILog MakeLogFromStorageInPeriod(CachedStorage storage, DateTime timeFrom, DateTime timeTo,
            bool cacheResults)
        {
            var recs =
                storage.GetRecords(cacheResults).Where(q => Util.CheckDateInPeriod(q.Time, timeFrom, timeTo));
            ILog log = new SingleStorageLog(recs,TMPMakeProfile(),storage.Descriptor.FileMD5, storage.Descriptor.Date);
            return log;
        }

        private IndexedProfile TMPMakeProfile()
        {
            IndexedProfile prof = IndexedProfile.FromProfile(SettingsContainer.Self.GetBaseProfile());
            return prof;
        }
        /// <summary>
        /// Get log for user for given period
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="timeFrom"></param>
        /// <param name="timeTo"></param>
        /// <param name="cacheResults"></param>
        /// <returns></returns>
        public ILog GetLogRecordsForUserForPeriod(Guid userId, DateTime timeFrom, DateTime timeTo,
            bool cacheResults = true)
        {
            timeFrom = ConvertDatetimeToStorage(timeFrom);
            timeTo = ConvertDatetimeToStorage(timeTo);
            //TODO: filter by user
            List<CachedStorage> neededStorages;
            lock (storages0)
            {
                var forThisUser = storages0.Where(t => t.Descriptor.UserId == userId);
                var needed = forThisUser.Where(t => t.Descriptor.CheckInterceptionWithPeriod(timeFrom, timeTo));
                neededStorages = needed.ToList();
            }
            var all = neededStorages.Select(t => t.GetRecords(cacheResults)).SelectMany(t => t);
            var inPeriod = all.Where(t => Util.CheckDateInPeriod(t.Time, timeFrom, timeTo));
            ILog log = new Log(inPeriod.ToArray(),TMPMakeProfile(),null);
            return log;
        }
        /// <summary>
        /// Clears cache of all cached storages
        /// </summary>
        public void UnloadAllCollections()
        {
            foreach (var cachedStorage in storages0)
            {
                cachedStorage.EraceCache();
            }
        }

        private DateTime ConvertDatetimeToStorage(DateTime dt)
        {
            return dt.ToLocalTime();
        }

    }
}
