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
