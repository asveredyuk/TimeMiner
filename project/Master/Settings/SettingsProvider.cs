using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiteDB;
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

        public const string APPS_LIST_COL_NAME = "apps";
        public const string PROFILE_COL_PREFIX = "profiledata_";
        public const string PROFILES_COL_NAME = "profiles";
        private LiteDatabase db;
        private SettingsProvider()
        {
            
            db = MasterDB.Settings.Database;
        }
        

    }
}
