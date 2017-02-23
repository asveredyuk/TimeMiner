using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace TimeMiner.Master
{
    /*public interface IResourceContainer : IReadOnlyDictionary<string, byte[]>
    {
        
    }*/
    /// <summary>
    /// Contains files from zip archive as dictionary in memory
    /// </summary>
    class ZipResourceContainer : IReadOnlyDictionary<string,byte[]>
    {
        /// <summary>
        /// Dictionary with file names and their data
        /// </summary>
        private Dictionary<string, byte[]> dict;
        /// <summary>
        /// Create new container from given zip archive
        /// </summary>
        /// <param name="zipArchiveContents">Data of zip archive</param>
        public ZipResourceContainer(byte[] zipArchiveContents)
        {
            dict = new Dictionary<string, byte[]>();
            //read contents of zip archive
            using (MemoryStream ms = new MemoryStream(zipArchiveContents))
            {
                using (ZipArchive arch = new ZipArchive(ms, ZipArchiveMode.Read))
                {
                    foreach (var entry in arch.Entries)
                    {
                        //directories have no name, skip them
                        if(entry.Name == "")
                            continue;
                        using (Stream entryStream = entry.Open())
                        {
                            dict[entry.FullName] = getBytesFromStream(entryStream);
                        }
                    }
                }
            }
        }
        /// <summary>
        /// Gets all bytes from given stream
        /// </summary>
        /// <param name="stream">Stream to read</param>
        /// <returns></returns>
        private byte[] getBytesFromStream(Stream stream)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                stream.CopyTo(ms);
                return ms.ToArray();
            }
        }
        /// <summary>
        /// Get string resource
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string GetString(string key)
        {
            byte[] arr = this[key];
            return Encoding.UTF8.GetString(arr);
        }
        /// <summary>
        /// Try get string resource
        /// </summary>
        /// <param name="key"></param>
        /// <param name="res"></param>
        /// <returns></returns>
        public bool TryGetString(string key, out string res)
        {
            byte[] arr;
            if (!TryGetValue(key, out arr))
            {
                res = null;
                return false;
            }
            res = Encoding.UTF8.GetString(arr);
            return true;
        }
        #region IReadOnlyDictionary interface implementation
        public IEnumerator<KeyValuePair<string, byte[]>> GetEnumerator()
        {
            return dict.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable) dict).GetEnumerator();
        }

        public int Count
        {
            get { return dict.Count; }
        }

        public bool ContainsKey(string key)
        {
            return dict.ContainsKey(key);
        }

        public bool TryGetValue(string key, out byte[] value)
        {
            return dict.TryGetValue(key, out value);
        }

        public byte[] this[string key]
        {
            get { return dict[key]; }
        }

        public IEnumerable<string> Keys
        {
            get { return dict.Keys; }
        }

        public IEnumerable<byte[]> Values
        {
            get { return dict.Values; }
        }
        #endregion
    }
}
