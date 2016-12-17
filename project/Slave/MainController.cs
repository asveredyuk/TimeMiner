using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeMiner.Core;

namespace TimeMiner.Slave
{
    /// <summary>
    /// Main controller class
    /// </summary>
    class MainController
    {
        private static MainController self;

        public static MainController Self
        {
            get
            {
                if(self == null)
                    self = new MainController();
                return self;
            }
        }

        private SlaveDB db;
        private Logger logger;
        private MasterBoundary boundary;
        private MainController()
        {
            db = SlaveDB.Self;
            logger = Logger.Self;
            boundary = MasterBoundary.Self;
            logger.onLogRecord += delegate(LogRecord record)
            {
                db.AddLogRecord(record);
            };
            db.onLogRecordAdded += delegate(LogRecord item, SlaveDB slaveDb)
            {
                boundary.SendOne(item);
            };
            boundary.onRecordSent += delegate(LogRecord record)
            {
                db.RemoveLogRecord(record);
            };
        }

        /// <summary>
        /// Called when application starts
        /// </summary>
        public void OnStartup()
        {
            logger.StartLogging();
            SendCachedRecordsAsync();
            //TODO: send some ping if server is not available, do not try to send all the things in cache
        }

        const int DELAY = 30*1000;
        /// <summary>
        /// Start sending records in database
        /// </summary>
        private async Task SendCachedRecordsAsync()
        {
            while (true)
            {
                var all = db.GetAllLogs();
                foreach (var logRecord in all)
                {
                    await boundary.SendOne(logRecord);
                }
                await Task.Delay(DELAY);
            }
        }

        public void OnExit()
        {
            logger.StopLogging();
        }
        
    }
}
