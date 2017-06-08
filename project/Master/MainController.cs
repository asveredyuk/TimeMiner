using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
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

        private bool started;
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
            slaveServer.onManyLogRecordsCame += delegate(LogRecord[] records)
            {
                logsDb.PutManyRecords(records);
            };
        }
        /// <summary>
        /// Called when program is started
        /// </summary>
        public void OnStartup()
        {
            if (started)
                return;
            try
            {
                MasterPluginRepository.Self.Init();
                if(ConfigManager.Self.SlaveDataPort == ConfigManager.Self.WebInterfacePort)
                    throw new ArgumentException("Wrong config: slave port is the same as web interface");
                slaveServer.Start();
                frontendServer.Start();
                started = true;
            }
            catch (Exception e)
            {
                var res = MessageBox.Show("Application failed to start.\nEnsure that you have enough (admin) privelegies\nDisplay error info?", "Failed to start",
                    MessageBoxButton.YesNo);
                if (res == MessageBoxResult.Yes)
                {
                    string str = e.ToString();
                    MessageBox.Show(str);
                }
                Application.Current.Shutdown();
            }
        }

        public void OnExit()
        {
            if(!started)
                return;
            try
            {
                slaveServer.Stop();
                frontendServer.Stop();
                started = false;
            }
            catch (Exception e)
            {
                Application.Current.Shutdown();
            }
            
        }
    }
}
