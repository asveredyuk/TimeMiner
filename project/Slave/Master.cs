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
    class Master
    {
        private static Master self;

        public static Master Self
        {
            get
            {
                if(self == null)
                    self = new Master();
                return self;
            }
        }

        private SlaveDB db;
        private Logger logger;
        private Master()
        {
            db = SlaveDB.Self;
            logger = Logger.Self;
            logger.onLogRecord += delegate(LogRecord record)
            {
                db.AddLogRecord(record);
            };
            db.onLogRecordAdded += delegate(LogRecord item, SlaveDB slaveDb)
            {
                
            };
        }

        /// <summary>
        /// Called when application starts
        /// </summary>
        public void OnStartup()
        {
            logger.StartLogging();
        }

        public void OnExit()
        {
            logger.StopLogging();
        }
        
    }
}
