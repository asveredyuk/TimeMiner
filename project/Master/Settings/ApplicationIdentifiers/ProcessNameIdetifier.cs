using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeMiner.Core;

namespace TimeMiner.Master.Settings.ApplicationIdentifiers
{
    public class ProcessNameIdetifier:ApplicationIdentifierBase
    {
        public string ProcessName { get; set; }

        public ProcessNameIdetifier()
        {
        }

        public ProcessNameIdetifier(string processName)
        {
            ProcessName = processName;
        }

        public override bool CheckRecord(LogRecord record)
        {
            return record.Process.ProcessName.Equals(ProcessName, StringComparison.CurrentCultureIgnoreCase);
        }
    }
}
