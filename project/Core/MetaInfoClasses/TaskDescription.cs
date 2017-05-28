using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeMiner.Core.MetaInfoClasses
{
    /// <summary>
    /// Describes current active task, added to log record each 1 minute
    /// </summary>
    public class TaskDescription
    {
        public const string TAG = "task";
        public string ShortName { get; set; }

        public TaskDescription()
        {
        }

        public TaskDescription(string shortName)
        {
            ShortName = shortName;
        }

    }
}
