using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeMiner.Core;
using TimeMiner.Core.Plugging;

namespace TimeMiner.Slave
{
    /// <summary>
    /// Main controller class
    /// </summary>
    class MainController
    {
        #region singletone
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
        #endregion

        /// <summary>
        /// Database for storing cache
        /// </summary>
        private SlaveDB db;
        /// <summary>
        /// Logger that handles user activity
        /// </summary>
        private Logger logger;
        /// <summary>
        /// Boundary to communicate with master
        /// </summary>
        private MasterBoundary boundary;

        private object _sendLock = new object();

        private TrayView trayView;
        private MainController()
        {
            //Initialize plugin repository
            SlavePluginRepository.Self.Init();

            //all them are singletones
            //TODO: remove singletone
            db = SlaveDB.Self;
            logger = Logger.Self;
            boundary = MasterBoundary.Self;
            trayView = new TrayView();

            logger.onLogRecord += delegate (LogRecord record)
            {
                //when recort is captured
                //it is added to database
                db.AddLogRecord(record);
            };
            db.onLogRecordAdded += delegate (LogRecord item, SlaveDB slaveDb)
            {
                //when log is added to database
                //we try to send it immediately
                //(if it fails, we will try to send it in SendCachedRecords coroutine
                //boundary.SendOne(item);
                Task.Run(async delegate
                {
                    var record = item;
                    bool sent = await boundary.SendOne(record);
                    if (sent)
                        db.RemoveLogRecord(record);
                });
            };
        }

        /// <summary>
        /// Called when application starts
        /// </summary>
        public void OnStartup()
        {
            logger.StartLogging();
            SendCachedRecordsAsync();
            UpdatePluginsPeriodicallyAsync();
        }

        const int CACHE_SEND_DELAY = 30 * 1000;
        const int PLUGIN_SYNC_DELAY = 30 * 1000;
        /// <summary>
        /// Start sending records in database
        /// </summary>
        private async void SendCachedRecordsAsync()
        {
            while (true)
            {
                //TODO: FIX: possibly, sending items twice
                //when one sending record is still in db, 
                //this method will get it and send to server to server again (duplicate will appear)
                var all = db.GetAllLogs();
                bool sent = await boundary.SendMany(all.ToArray());
                if (sent)
                {
                    foreach (var logRecord in all)
                    {
                        db.RemoveLogRecord(logRecord);
                    }
                }
                await Task.Delay(CACHE_SEND_DELAY);
            }
        }
        /// <summary>
        /// Called when application is exiting
        /// </summary>
        public void OnExit()
        {
            logger.StopLogging();
        }

        private async void UpdatePluginsPeriodicallyAsync()
        {
            while (true)
            {
                await SlavePluginRepository.Self.SyncWithServer();
                await Task.Delay(PLUGIN_SYNC_DELAY);
            }
        }

    }
}
