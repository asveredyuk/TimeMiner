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
        public CachedStorage(string fname)
        {
            this.fname = fname;
            serializer = MessagePackSerializer.Get<LogRecord>();
            try
            {
                Descriptor = new StorageDescriptor(fname);
            }
            catch (Exception e)
            {
                throw new Exception("Wrong file name, cannot parse descriptor",e);
            }
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
        /// <summary>
        /// Append log record to the file
        /// </summary>
        /// <param name="rec">Log record to append</param>
        private void AppendToFile(LogRecord rec)
        {
            var stream = File.Open(fname, FileMode.Append);
            serializer.Pack(stream, rec);
            stream.Flush();
            stream.Close();
        }
    }
}
