using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeMiner.Core;

namespace TimeMiner.Master.Analysis
{
    public interface ILog
    {
        LogRecord[] Records { get; }
        string DataHash { get; }
        IndexedProfile Prof { get; }
        DateTime Date { get; }
    }
}
