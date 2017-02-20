using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeMiner.Master.Database;

namespace TimeMiner.Master.Settings
{
    class SettingsProvider
    {
        private static SettingsProvider self;

        public static SettingsProvider Self
        {
            get
            {
                if (self == null)
                {
                    self = new SettingsProvider();
                }
                return self;
            }
        }

        private SettingsDB db;
        private SettingsProvider()
        {
            db = MasterDB.Settings;
        }
    }
}
