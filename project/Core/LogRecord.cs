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
        /// Identificator of log record
        /// </summary>
        public Guid Id { get; set; }
        /// <summary>
        /// Time of the record
        /// </summary>
        public DateTime Time { get; set; }
        /// <summary>
        /// Id of user
        /// </summary>
        public int UserId { get; set; } 
        /// <summary>
        /// Time of the previous record
        /// </summary>
        public Guid PreviusRecordId{ get; set; }
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

//        public override string ToString()
//        {
//            return $"Time: {Time}, PreviousRecordId: {PreviusRecordId}, Process: {Process}," +
//                   $" Window: {Window}, MousePosition: {MousePosition}, MouseButtonActions: {MouseButtonActions}, " +
//                   $"MouseWheelActions: {MouseWheelActions}, Keystrokes: {Keystrokes}";
//        }
        public override string ToString()
        {
            return $"Id: {Id}, Keystrokes: {Keystrokes}, MouseButtonActions: {MouseButtonActions}, MousePosition: {MousePosition}, MouseWheelActions: {MouseWheelActions}, PreviusRecordId: {PreviusRecordId}, Process: {Process}, Time: {Time}, Window: {Window}";
        }
    }
}
