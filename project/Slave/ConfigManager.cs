using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeMiner.Slave
{
    class ConfigManager
    {
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
        public Guid UserId { get; }
        public bool StatusRefreshEnabled { get; } = true;
        public int StatusRefreshInterval { get; } = 2000;
        public ConfigManager()
        {
            
            string[] lines = File.ReadAllLines("config.txt");
            Server = lines[0];
            UserId = Guid.Parse(lines[1]);
        }
    }
}
