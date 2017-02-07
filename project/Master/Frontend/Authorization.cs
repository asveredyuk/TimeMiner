using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace TimeMiner.Master.Frontend
{
    class Authorization
    {
        const string ADMIN_USERNAME = "admin";
        const string ADMIN_PASSWORD = "qwerty";
        /// <summary>
        /// Authorize the user and return token if ok
        /// </summary>
        /// <param name="login">Login</param>
        /// <param name="password">Password</param>
        /// <returns>Token if authorized, else null</returns>
        public static string Authorize(string login, string password)
        {
            if (login == ADMIN_USERNAME && password == ADMIN_PASSWORD)
            {
                return GetMd5Hash(login + password);
            }
            return null;
        }
        /// <summary>
        /// Verify given user token
        /// </summary>
        /// <param name="token">Token from cookie</param>
        /// <returns></returns>
        public static bool VerifyToken(string token)
        {
            string goodToken = GetMd5Hash(ADMIN_USERNAME + ADMIN_PASSWORD);
            return goodToken == token;
        }

        static string GetMd5Hash(string input)
        {
            using (MD5 md5 = MD5.Create())
            {
                return GetMd5Hash(md5, input);
            }
        }
        static string GetMd5Hash(MD5 md5Hash, string input)
        {

            // Convert the input string to a byte array and compute the hash.
            byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));

            // Create a new Stringbuilder to collect the bytes
            // and create a string.
            StringBuilder sBuilder = new StringBuilder();

            // Loop through each byte of the hashed data 
            // and format each one as a hexadecimal string.
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            // Return the hexadecimal string.
            return sBuilder.ToString();
        }
    }
}
