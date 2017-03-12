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
        /// File name pattern. {0} - userid
        /// </summary>
        const string LOG_FNAME_PATTERN= "log_u{0}.storage";
        /// <summary>
        /// Temporary, storage for the user #0
        /// </summary>
        private CachedStorage storage0;
        /// <summary>
        /// Create new LogsDB object
        /// </summary>
        public LogsDB()
        {
            if (!Directory.Exists(LOGS_DIR))
                Directory.CreateDirectory(LOGS_DIR);
            storage0 = new CachedStorage(MkFname(0));
        }
        /// <summary>
        /// Put new record to the storage of acossiated user
        /// </summary>
        /// <param name="rec"></param>
        public void PutRecord(LogRecord rec)
        {
            //TODO: attention! multithread & lock!
            //string fname = MkFname(rec.UserId);
            storage0.PutRecord(rec);
        }

        /// <summary>
        /// Get all records for given user
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
        public List<LogRecord> GetAllRecordsForUser(int userid, bool cacheResults = true)
        {
            //string fname = MkFname(userid);
            return storage0.GetRecords(cacheResults);
        }
        /// <summary>
        /// Make name of storage file for given user
        /// </summary>
        /// <param name="userid">id of user</param>
        /// <returns>relative path to the log file</returns>
        private static string MkFname(int userid)
        {
            return LOGS_DIR + "/" + string.Format(LOG_FNAME_PATTERN, userid);
        }
    }
}
