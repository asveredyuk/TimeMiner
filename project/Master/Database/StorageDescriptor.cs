using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
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
        public Guid UserId { get; set; }
        /// <summary>
        /// Date of logs (includes Year Month and Day)
        /// </summary>
        public DateTime Date { get; set; }
        /// <summary>
        /// Create new storage descriptor
        /// </summary>
        /// <param name="userId">Id of user</param>
        /// <param name="date">Date</param>
        public StorageDescriptor(Guid userId, DateTime date)
        {
            UserId = userId;
            Date = date;
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
        /// <summary>
        /// Check if this log intercepts with given period
        /// </summary>
        /// <param name="begin">Start of period</param>
        /// <param name="end">End of period</param>
        /// <returns></returns>
        public bool CheckInterceptionWithPeriod(DateTime begin, DateTime end)
        {
            DateTime myBegin = Date;
            DateTime myEnd = Date.AddDays(1);
            return Util.CheckPeriodsIntercept(myBegin, myEnd, begin, end);
        }
        /// <summary>
        /// Save descriptor to file
        /// </summary>
        /// <param name="fpath"></param>
        public void SaveToFile(string fpath)
        {
            SaveToFile(this,fpath);
        }
        /// <summary>
        /// Load descriptor from file
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static StorageDescriptor LoadFromFile(string path)
        {
            string text = File.ReadAllText(path);
            return JsonConvert.DeserializeObject<StorageDescriptor>(text);
        }
        /// <summary>
        /// Save descriptor to file
        /// </summary>
        /// <param name="desc"></param>
        /// <param name="path"></param>
        public static void SaveToFile(StorageDescriptor desc, string path)
        {
            string text = JsonConvert.SerializeObject(desc);
            File.WriteAllText(path,text);
        }
        
    }
}
