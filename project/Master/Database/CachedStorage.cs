using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;
using MsgPack.Serialization;
using TimeMiner.Core;

namespace TimeMiner.Master.Database
{
    /// <summary>
    /// Works with file and stores it's data in memory (is thread safe)
    /// </summary>
    class CachedStorage
    {
        /// <summary>
        /// Extension of files with logs
        /// </summary>
        private const string LOG_EXT = ".storage";
        /// <summary>
        /// Extension of storage descriptor files
        /// </summary>
        private const string DESCRIPTOR_EXT = ".descriptor";
        /// <summary>
        /// Pattern for generating log storage name. Format: {0} - userid, {1} - date in format DDMMYYYY
        /// </summary>
        const string LOG_FNAME_PATTERN = "log_u{0}_{1}" + LOG_EXT;

        /// <summary>
        /// Are results cached or not by default
        /// </summary>
        public const bool CACHE_RESULTS_DEFAULT = true;
        /// <summary>
        /// Object used to lock threads
        /// </summary>
        private readonly object _lock = new object();
        /// <summary>
        /// Name of file with log
        /// </summary>
        private string fname;
        /// <summary>
        /// List with cached records
        /// </summary>
        private List<LogRecord> cache;
        /// <summary>
        /// Serializer for records
        /// </summary>
        private MessagePackSerializer<LogRecord> serializer;
        /// <summary>
        /// Descriptor of this storage
        /// </summary>
        public StorageDescriptor Descriptor { get; }
        /// <summary>
        /// Create new cached storage
        /// </summary>
        /// <param name="fname">Name of file to store records</param>
        private CachedStorage(string fname)
        {
            if (!File.Exists(fname))
            {
                throw new Exception("File does not exists");
            }
            if (!File.Exists(MakeStorageDescriptorFilePath(fname)))
            {
                throw new Exception($"Storage {fname} does not have descriptor");
            }
            this.fname = fname;
            serializer = MessagePackSerializer.Get<LogRecord>();
            try
            {
                Descriptor = StorageDescriptor.LoadFromFile(MakeStorageDescriptorFilePath(fname));
            }
            catch (Exception e)
            {
                throw new Exception("Wrong storage descriptor",e);
            }
        }
        /// <summary>
        /// Creates new storage with given parameters
        /// </summary>
        /// <param name="userId">Id of user-owner</param>
        /// <param name="date">Date of storage</param>
        /// <param name="storageDir">Directory with storages</param>
        /// <returns></returns>
        public static CachedStorage CreateNewStorage(Guid userId, DateTime date, string storageDir)
        {
            if (!Directory.Exists(storageDir))
            {
                throw new Exception("Storage directory does not exist");
            }
            if (date != date.Date)
            {
                throw new Exception("Given timestamp is not a date");
            }
            string fname = storageDir + "/" + MakeStorageFileName(userId, date);

            if (File.Exists(fname))
            {
                throw new Exception("Given log" + fname + " already exists");
            }

            StorageDescriptor desc = new StorageDescriptor(userId,date, Util.GetEmptyMD5Hash());
            desc.SaveToFile(MakeStorageDescriptorFilePath(fname));
            File.Create(fname).Close();
            return new CachedStorage(fname);
        }
        /// <summary>
        /// Loads all existing storages in given directory  
        /// </summary>
        /// <param name="storageDir">Directory with storages</param>
        /// <returns></returns>
        public static List<CachedStorage> LoadAllStorages(string storageDir)
        {
            if (!Directory.Exists(storageDir))
            {
                throw new Exception("Storage directory does not exist");
            }
            List<CachedStorage> storages = new List<CachedStorage>();
            List<string > failed = new List<string >();
            foreach (var fname in Directory.GetFiles(storageDir, "*" + LOG_EXT))
            {
                try
                {
                    CachedStorage storage = new CachedStorage(fname);
                    storages.Add(storage);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    failed.Add(fname);
                }
            }
            if (failed.Count > 0)
            {
                Console.WriteLine($"Failed to load {failed.Count} storages");
                Console.WriteLine("List of failed files:");
                Console.WriteLine(failed.Aggregate("",((r, t) => r += "\r\n" + t)));
            }
            return storages;
        }
        /// <summary>
        /// Make name of storage file for given user
        /// </summary>
        /// <param name="userid">id of user</param>
        /// <param name="date">date of log record</param>
        /// <returns>relative path to the log file</returns>
        private static string MakeStorageFileName(Guid userid, DateTime date)
        {
            string dateStr = date.ToString("ddMMyy");
            return string.Format(LOG_FNAME_PATTERN, userid, dateStr);
        }

        public static string MakeStorageDescriptorFilePath(string storageFilePath)
        {
            return storageFilePath + DESCRIPTOR_EXT;
        }
        /// <summary>
        /// Get collection of records in storage
        /// </summary>
        /// <param name="cacheResults">Should results be cached or not</param>
        /// <returns>List of all records in storage</returns>
        public List<LogRecord> GetRecords(bool cacheResults = CACHE_RESULTS_DEFAULT)
        {
            lock (_lock)
            {
                if(cache!=null)
                    return new List<LogRecord>(cache);
                var list = ReadRecordsFromFile();
                if (!cacheResults)
                {
                    return list;
                }
                else
                {
                    cache = list;
                    return new List<LogRecord>(cache);
                }
            }
        }
        /// <summary>
        /// Put new record to the storage
        /// </summary>
        /// <param name="rec">Record to add</param>
        public void PutRecord(LogRecord rec)
        {
            lock (_lock)
            {
                AppendToFile(rec);
                if (cache != null)
                    cache.Add(rec);
            }
        }
        /// <summary>
        /// Erace cache of given storage
        /// </summary>
        public void EraceCache()
        {
            //forget about existing cache            
            lock (_lock)
            {
                cache = null;
            }
        }
        /// <summary>
        /// Read log records from the file
        /// </summary>
        /// <returns></returns>
        private List<LogRecord> ReadRecordsFromFile()
        {

            if (!File.Exists(fname))
            {
                return new List<LogRecord>(); //nothing to read
            }

            List<LogRecord> res = new List<LogRecord>();
//                        using (var stream = File.OpenRead(fname))
//                        {
//                            while (stream.Position != stream.Length)
//                            {
//                                LogRecord rec = serializer.Unpack(stream);
//                                res.Add(rec);
//                            }
//                            stream.Close();
//                        }
            //improved version of file read (we suppose that file are little enough, ex. logs for 1 day)
            using (var fstream = File.OpenRead(fname))
            {
                using (MemoryStream stream = new MemoryStream())
                {
                    //read file to memory and close
                    fstream.CopyTo(stream);
                    fstream.Close();
                    stream.Position = 0;
                    //and now log is parsed, so there is no little periodi
                    while (stream.Position != stream.Length)
                    {
                        LogRecord rec = serializer.Unpack(stream);
                        res.Add(rec);
                    }
                    stream.Close();
                }
                
            }
            return res;
        }

        //TODO: make larger transactions of numbers of records
        //this will save the time and can be useful for import or late sending
        /// <summary>
        /// Append log record to the file
        /// </summary>
        /// <param name="rec">Log record to append</param>
        private void AppendToFile(LogRecord rec)
        {
            using (var stream = File.Open(fname, FileMode.Append))
            {
                serializer.Pack(stream, rec);
                stream.Flush();
                stream.Close();
            }
        }

        private void UpdateDescriptor()
        {
            if (!File.Exists(fname))
            {
                Descriptor.FileMD5 = Util.GetEmptyMD5Hash();
            }
            else
            {
                string hash = Util.ComputeFileMD5Hash(fname);
                Descriptor.FileMD5 = hash;
            }
            Descriptor.SaveToFile(MakeStorageDescriptorFilePath(fname));
        }
    }
}
