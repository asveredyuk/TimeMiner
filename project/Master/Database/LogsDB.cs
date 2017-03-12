using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MsgPack.Serialization;
using TimeMiner.Core;

namespace TimeMiner.Master
{
    public class LogsDB
    {
        public static string LOGS_DIR = "logs";
        const string LOG_FNAME_PATTERN= "log_u{0}.storage";

        private MessagePackSerializer<LogRecord> serializer;
        public LogsDB()
        {
            if (!Directory.Exists(LOGS_DIR))
                Directory.CreateDirectory(LOGS_DIR);
            serializer = MessagePackSerializer.Get<LogRecord>();
        }

        /// <summary>
        /// Put new record to the database to the table of record user
        /// </summary>
        /// <param name="rec"></param>
        public void PutRecord(LogRecord rec)
        {
            //TODO: attention! multithread & lock!
            string fname = MkFname(rec.UserId);
            var stream = File.Open(fname, FileMode.Append);
            serializer.Pack(stream,rec);
            stream.Flush();
            stream.Close();
        }

        /// <summary>
        /// Get all records for given user
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
        public List<LogRecord> GetAllRecordsForUser(int userid)
        {
            string fname = MkFname(userid);
            if(!File.Exists(fname))
                return new List<LogRecord>();           //return empty list

            List<LogRecord> res = new List<LogRecord>();
            using (var stream = File.OpenRead(fname))
            {
                while (stream.Position != stream.Length)
                {
                    LogRecord rec = serializer.Unpack(stream);
                    res.Add(rec);
                }
                stream.Close();
            }
            return res;

        }

        private string MkFname(int userid)
        {
            return LOGS_DIR + "/" + string.Format(LOG_FNAME_PATTERN, userid);
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
