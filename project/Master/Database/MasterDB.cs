using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiteDB;
using TimeMiner.Core;
using TimeMiner.Master;
using TimeMiner.Master.Database;

namespace TimeMiner.Master
{
    /// <summary>
    /// Dabase for master
    /// </summary>
    public static class MasterDB
    {
        private static LogsDB logs;

        public static LogsDB Logs
        {
            get
            {
                if (logs == null)
                {
                    logs = new LogsDB();
                }
                return logs;
            }
        }

        private static SettingsDB settings;

        public static SettingsDB Settings
        {
            get
            {
                if (settings == null)
                {
                    settings = new SettingsDB();
                }
                return settings;
            }
        }

    }
}
