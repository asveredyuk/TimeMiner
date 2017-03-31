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
        /// <summary>
        /// Check if given record fits identifier
        /// </summary>
        /// <param name="record">Log record to check</param>
        /// <returns>Relation of record to given app</returns>
        public abstract int CheckRecord(LogRecord record);
    }
}
