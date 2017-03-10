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
        /// Id of user
        /// </summary>
        public int UserId { get; set; } 
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
        /// <summary>
        /// Identificator of log record
        /// </summary>
        public Guid Id { get; set; }
        /// <summary>
        /// Time of the previous record
        /// </summary>
        public Guid PreviusRecordId { get; set; }

        public Dictionary<string,byte[]> MetaData { get; set; }

        public LogRecord()
        {
            MetaData = new Dictionary<string, byte[]>();
        }

        public override string ToString()
        {
            return $"Id: {Id}, Keystrokes: {Keystrokes}, MouseButtonActions: {MouseButtonActions}, MousePosition: {MousePosition}, MouseWheelActions: {MouseWheelActions}, PreviusRecordId: {PreviusRecordId}, Process: {Process}, Time: {Time}, Window: {Window}";
        }
        /// <summary>
        /// Get string from metadata
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string GetMetaString(string key)
        {
            byte[] arr;
            if (!MetaData.TryGetValue(key, out arr))
            {
                return null;
            }
            return Encoding.UTF8.GetString(arr);
        }
        /// <summary>
        /// Put string to metadata
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void PutMetaString(string key, string value)
        {
            byte[] arr = Encoding.UTF8.GetBytes(value);
            MetaData[key] = arr;
        }
        //TODO: remove?
    }
}
