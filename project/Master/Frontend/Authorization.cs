using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace TimeMiner.Master.Frontend
{
    public class Authorization
    {
        /// <summary>
        /// Lock, to prevent multiple initialization
        /// </summary>
        private static readonly object _lock = new object();

        /// <summary>
        /// Singleton instance
        /// </summary>
        private static Authorization self;

        /// <summary>
        /// Singleton instance
        /// </summary>
        public static Authorization Self
        {
            get
            {
                lock (_lock)
                {
                    if (self == null)
                    {
                        self = new Authorization();
                    }
                    return self;
                }
            }
        }
        //TODO: make integration with settings db
         
        public string Authorize(string login, string password)
        {
            if (login == ConfigManager.Self.GetString("admin-login") && password == ConfigManager.Self.GetString("admin-password"))
            {
                return GetToken();
            }
            return null;
        }

        private string GetToken()
        {
            string login = ConfigManager.Self.GetString("admin-login");
            string password = ConfigManager.Self.GetString("admin-password");
            return Util.ComputeStringMD5Hash(login + password);
        }
        public bool ValidateToken(string token)
        {
            return token == GetToken();
        }
    }
}
