using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace TimeMiner.Slave
{
    class ConfigManager
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

        public string Server { get; }
        public string SendPort { get; }
        public string ApiPort { get; }
        public Guid UserId { get; }
        public bool StatusRefreshEnabled { get; } = true;
        public int StatusRefreshInterval { get; } = 2000;
        public ConfigManager()
        {
            try
            {
                string text = File.ReadAllText(FNAME);
                var data = JsonConvert.DeserializeObject<Dictionary<string, string>>(text);
                Server = data["server"];
                SendPort = data["send_port"];
                ApiPort = data["api_port"];
                UserId = Guid.Parse(data["user_id"]);
            }
            catch (Exception e)
            {
                throw new Exception("Failed to load config file \"" + FNAME +"\"");
            }
        }
    }
}
