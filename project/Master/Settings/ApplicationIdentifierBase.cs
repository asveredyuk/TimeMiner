using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiteDB;
using Newtonsoft.Json;
using TimeMiner.Core;

namespace TimeMiner.Master.Settings
{
    /// <summary>
    /// Information for application identification
    /// </summary>
    public abstract class ApplicationIdentifierBase
    { 
        public abstract int CheckRecord(LogRecord record);
    }
}
