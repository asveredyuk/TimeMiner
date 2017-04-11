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
        /// <param name="skipDescriptorUpdate">Should descriptor be updated or not</param>
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

            StorageDescriptor desc = new StorageDescriptor(userId,date, Util.GetEmptyMD5Hash(), DateTime.MinValue);
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
        public IEnumerable<LogRecord> GetRecords(bool cacheResults = CACHE_RESULTS_DEFAULT)
        {
            lock (_lock)
            {
                if (cache != null)
                {
                    foreach (var logRecord in cache)
                    {
                        yield return logRecord;
                    }
                }
                else
                {
                    var fileEnum = ReadRecordsFromFile();
                    //if this is called, we really need to read items
                    if (cacheResults)
                    {
                        cache = new List<LogRecord>(fileEnum);
                        foreach (var logRecord in cache)
                        {
                            yield return logRecord;
                        }

                    }
                    else
                    {
                        foreach (var logRecord in fileEnum)
                        {
                            yield return logRecord;
                        }
                    }
                }
                /*if (!cacheResults)
                {
                    return list;
                }
                else
                {
                    cache = list;
                    return new List<LogRecord>(cache);
                }*/
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
        /// Put many records at once. This method is better for large amounts of records
        /// </summary>
        /// <param name="recs"></param>
        public void PutManyRecords(IEnumerable<LogRecord> recs)
        {
            lock (_lock)
            {
                var logRecords = recs as IList<LogRecord> ?? recs.ToList();
                AppendToFileMany(logRecords);
                if(cache!=null)
                    cache.AddRange(logRecords);
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
        private IEnumerable<LogRecord> ReadRecordsFromFile()
        {

            if (!File.Exists(fname))
            {
                //nothing to return/
                yield break;
                //return new List<LogRecord>(); //nothing to read
            }

//            List<LogRecord> res = new List<LogRecord>();
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
            using (MemoryStream mstream = new MemoryStream())
            {
                using (var fstream = File.OpenRead(fname))
                {
                    Console.WriteLine($"File {fname} opened");
                    fstream.CopyTo(mstream);
                    fstream.Close();
                }
                mstream.Position = 0;
                while (mstream.Position != mstream.Length)
                {
                    LogRecord rec = serializer.Unpack(mstream);
                    yield return rec;
                    //res.Add(rec);
                }
                mstream.Close();
            }
//            using (var fstream = File.OpenRead(fname))
//            {
//                using (MemoryStream stream = new MemoryStream())
//                {
//                    //read file to memory and close
//                    fstream.CopyTo(stream);
//                    fstream.Close();
//                    stream.Position = 0;
//                    //and now log is parsed, so there is no little periodi
//                    while (stream.Position != stream.Length)
//                    {
//                        LogRecord rec = serializer.Unpack(stream);
//                        res.Add(rec);
//                    }
//                    stream.Close();
//                }
//                
//            }
//            return res;
        }

        /// <summary>
        /// Append many log records to the file
        /// </summary>
        /// <param name="recs"></param>
        private void AppendToFileMany(IEnumerable<LogRecord> recs)
        {
            using (var stream = File.Open(fname, FileMode.Append))
            {
                foreach (var rec in recs)
                {
                    serializer.Pack(stream, rec);
                }
                stream.Flush();
                stream.Close();
            }
        }
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

        public void RefreshDescriptor()
        {
            
            if (!File.Exists(fname))
            {
                Descriptor.FileMD5 = Util.GetEmptyMD5Hash();
            }
            else
            {
                DateTime lastModifiedReal = File.GetLastWriteTime(fname);
                if (lastModifiedReal.ToUniversalTime() > Descriptor.LastModified.ToUniversalTime())
                {
                    string hash = Util.ComputeFileMD5Hash(fname);
                    Console.WriteLine("Hash recomputed : " + Path.GetFileName(fname));
                    Descriptor.FileMD5 = hash;
                    Descriptor.LastModified = lastModifiedReal;
                }
            }
            Descriptor.SaveToFile(MakeStorageDescriptorFilePath(fname));
        }
    }
}
