using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeMiner.Core
{
    /// <summary>
    /// One item for 
    /// </summary>
    public class LogRecord
    {
        /// <summary>
        /// Time of the record
        /// </summary>
        public DateTime Time { get; set; }
        /// <summary>
        /// Time of the previous record
        /// </summary>
        public DateTime PreviousRecordTime { get; set; }
        /// <summary>
        /// Name of current foreground process
        /// </summary>
        public ProcessDescriptor Process{ get; set; }
        /// <summary>
        /// Window title of the foreground process
        /// </summary>
        public WindowDescriptor Window { get; set; }
        /// <summary>
        /// Mouse position at the time of record
        /// </summary>
        public IntPoint MousePosition { get; set; }
        /// <summary>
        /// Number of mouse button actions, performed after the previous record
        /// </summary>
        public int MouseButtonActions { get; set; }
        /// <summary>
        /// Number of mouse wheel actions, performed after the previous record
        /// </summary>
        public int MouseWheelActions { get; set; }
        /// <summary>
        /// Number of keys, pressed after the previous record
        /// </summary>
        public int Keystrokes { get; set; }

    }
}
