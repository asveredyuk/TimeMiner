﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace TimeMiner.Master
{
    static class Util
    {
        public static bool CheckPeriodsIntercept(DateTime aBegin, DateTime aEnd, DateTime bBegin, DateTime bEnd)
        {
            return aBegin < bEnd && bBegin < aEnd;
        }
        public static bool CheckDateInPeriod(DateTime date, DateTime periodBegin, DateTime periodEnd)
        {
            return periodBegin <= date && date <= periodEnd;
        }

        public static string ComputeMD5Hash(Stream stream)
        {
            using (MD5 md5 = MD5.Create())
            {
                byte[] hash = md5.ComputeHash(stream);
                return BitConverter.ToString(hash).Replace("-", "");
            }
        }

        public static string ComputeFileMD5Hash(string fpath)
        {
            using (Stream stream = File.OpenRead(fpath))
            {
                return ComputeMD5Hash(stream);
            }
        }

        public static string ComputeMD5Hash(byte[] arr)
        {
            using (MD5 md5 = MD5.Create())
            {
                byte[] hash = md5.ComputeHash(arr);
                return BitConverter.ToString(hash).Replace("-", "");
            }
        }

        public static string GetEmptyMD5Hash()
        {
            return ComputeMD5Hash(new byte[0]);
        }
    }
}
