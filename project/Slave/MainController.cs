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
            //TODO: start sending old records from database to master
        }

        public void OnExit()
        {
            logger.StopLogging();
        }
        
    }
}
