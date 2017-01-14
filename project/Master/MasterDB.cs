using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiteDB;
using TimeMiner.Core;

namespace TimeMiner.Master
{
    class MasterDB : IDisposable
    {
        private MasterDB self;

        public MasterDB Self
        {
            get
            {
                if (self == null)
                {
                    self = new MasterDB();
                }
                return self;
            }
        }
        LiteDatabase db;

        private MasterDB()
        {
            db = new LiteDatabase("logstorage.db");

        }

        public void PutRecord(LogRecord rec)
        {
            var col = db.GetCollection<LogRecord>("log_u" + rec.UserId);
            col.EnsureIndex(x => x.Id);
            if (col.Exists(x => x.Id == rec.Id))
            {
                throw new Exception("Such item ");
            }
            col.Insert(rec);
        }
        ~MasterDB()
        {
            Dispose();
        }

        public void Dispose()
        {
            if (db != null)
            {
                db.Dispose();
                db = null;
            }
        }
    }
}
