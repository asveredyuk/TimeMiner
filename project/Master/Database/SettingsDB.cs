using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiteDB;

namespace TimeMiner.Master.Database
{
    public class SettingsDB
    {
        public static string DB_PATH = "settings.db";

        /// <summary>
        /// Lock, to prevent multiple initialization
        /// </summary>
        private static readonly object _lock = new object();

        /// <summary>
        /// Singleton instance
        /// </summary>
        private static SettingsDB self;

        /// <summary>
        /// Singleton instance
        /// </summary>
        public static SettingsDB Self
        {
            get
            {
                lock (_lock)
                {
                    if (self == null)
                    {
                        self = new SettingsDB();
                    }
                    return self;
                }
            }
        }

        /// <summary>
        /// Database connection
        /// </summary>
        LiteDatabase db;
        /// <summary>
        /// Get the settings database. Be careful with this
        /// </summary>
        public LiteDatabase Database
        {
            get { return db; }
        }
        internal SettingsDB()
        {
            db = new LiteDatabase(DB_PATH);
        }
        

    }
}
