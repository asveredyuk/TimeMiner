using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace TimeMiner.Master
{
    public class ConfigManager
    {
        private const string FNAME = "config.json";
        /// <summary>
        /// Lock, to prevent multiple initialization
        /// </summary>
        private static readonly object _lock = new object();

        /// <summary>
        /// Singleton instance
        /// </summary>
        private static ConfigManager self;

        /// <summary>
        /// Singleton instance
        /// </summary>
        public static ConfigManager Self
        {
            get
            {
                lock (_lock)
                {
                    if (self == null)
                    {
                        self = new ConfigManager();
                    }
                    return self;
                }
            }
        }

        private Dictionary<string, string> data = new Dictionary<string, string>();

        private void LoadConfig()
        {
            lock (_lock)
            {
                if (!File.Exists(FNAME))
                {
                    File.WriteAllText(FNAME, "{ }");//create empty object
                }
                string text = File.ReadAllText(FNAME);
                data = JsonConvert.DeserializeObject<Dictionary<string, string>>(text);
            }
        }

        private void SaveConfig()
        {
            lock (_lock)
            {
                string text = JsonConvert.SerializeObject(data, Formatting.Indented);
                File.WriteAllText(FNAME,text);
            }
        }

        public string GetString(string key)
        {
            lock (_lock)
            {
                string res;
                if (!data.TryGetValue(key, out res))
                    return null;
                return res;
            }
        }

        public int? GetInt(string key)
        {
            string res = GetString(key);
            if (res == null)
                return null;
            int r;
            if (!int.TryParse(res, out r))
                return null;
            return r;
        }

        public void PutString(string key, string value)
        {
            lock (_lock)
            {
                data[key] = value;
                SaveConfig();
            }
            
        }

        public void PutInt(string key, int value)
        {
            PutString(key, value.ToString());
        }
    }
}
