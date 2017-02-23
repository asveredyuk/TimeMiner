using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeMiner.Master
{
    public class DirResourceContainer:IResourceContainer
    {
        private string dirPath;

        public DirResourceContainer(string dirPath)
        {
            if (!dirPath.EndsWith("/"))
                dirPath += "/";
            this.dirPath = dirPath;
        }

        public byte[] GetResource(string key)
        {
            string path = dirPath + key;
            if (!File.Exists(path))
                return null;
            return File.ReadAllBytes(path);
        }

        public string GetString(string key)
        {
            byte[] arr = GetResource(key);
            if (arr == null)
                return null;
            return Encoding.UTF8.GetString(arr);
        }
    }
}
