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
        /// Hash of storage file
        /// </summary>
        public string FileMD5 { get; set; }
        /// <summary>
        /// Date of file being modified
        /// </summary>
        public DateTime LastModified { get; set; }

        public StorageDescriptor(Guid userId, DateTime date, string fileMd5, DateTime lastModified)
        {
            UserId = userId;
            Date = date;
            FileMD5 = fileMd5;
            LastModified = lastModified;
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
            string text = JsonConvert.SerializeObject(desc, Formatting.Indented);
            File.WriteAllText(path,text);
        }

        protected bool Equals(StorageDescriptor other)
        {
            return UserId.Equals(other.UserId) && Date.Equals(other.Date);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((StorageDescriptor) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (UserId.GetHashCode() * 397) ^ Date.GetHashCode();
            }
        }

        public static bool operator ==(StorageDescriptor left, StorageDescriptor right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(StorageDescriptor left, StorageDescriptor right)
        {
            return !Equals(left, right);
        }
    }
}
