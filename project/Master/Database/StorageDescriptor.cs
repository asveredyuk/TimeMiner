using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeMiner.Core;

namespace TimeMiner.Master.Database
{
    /// <summary>
    /// Describes storage
    /// </summary>
    public class StorageDescriptor
    {
        /// <summary>
        /// Id of user
        /// </summary>
        public int UserId { get; }
        /// <summary>
        /// Date of logs (includes Year Month and Day)
        /// </summary>
        public DateTime Date { get; }
        /// <summary>
        /// Create new storage descriptor
        /// </summary>
        /// <param name="fpath">Path to storage file</param>
        public StorageDescriptor(string fpath)
        {
            string name = Path.GetFileNameWithoutExtension(fpath);
            string[] split = name.Split('_');
            UserId = int.Parse(split[1].TrimStart('u'));
            Date = DateTime.ParseExact(split[2], "ddMMyy",null);
        }
        /// <summary>
        /// Check if given storage is appropriate for given record
        /// </summary>
        /// <param name="rec"></param>
        /// <returns></returns>
        public bool CheckAcceptsLogRecord(LogRecord rec)
        {
            return UserId == rec.UserId && Date.ToString("ddMMyy") == rec.Time.ToString("ddMMyy");
        }
    }
}
