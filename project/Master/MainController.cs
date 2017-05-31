using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeMiner.Core;
using TimeMiner.Master.Frontend;
using TimeMiner.Master.Frontend.Plugins;

namespace TimeMiner.Master
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
                if (self == null)
                    self = new MainController();
                return self;
            }
        }

        private FrontendServer frontendServer;
        private SlaveServer slaveServer;
        private LogsDB logsDb;
        private MainController()
        {
            frontendServer = FrontendServer.Self;
            slaveServer = SlaveServer.Self;
            logsDb = LogsDB.Self;

            //bind connections
            slaveServer.onLogRecordCame += delegate(LogRecord rec)
            {
                logsDb.PutRecord(rec);
            };
        }
        /// <summary>
        /// Called when program is started
        /// </summary>
        public void OnStartup()
        {
            PluginRepository.Self.Init();
            slaveServer.Start();
            frontendServer.Start();
        }

        public void OnExit()
        {
            slaveServer.Stop();
            frontendServer.Stop();
        }
    }
}
