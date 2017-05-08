using System;
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

        public static string ComputeStringMD5Hash(string str)
        {
            byte[] arr = Encoding.UTF8.GetBytes(str);
            return ComputeMD5Hash(arr);
        }
        public static string GetEmptyMD5Hash()
        {
            return ComputeMD5Hash(new byte[0]);
        }

        public static string GetHostFromUrl(string url)
        {
            if (url == null)
                return null;
            if (url.StartsWith("localhost"))
                return "localhost";
            Uri uri;
            if (!Uri.TryCreate(url, UriKind.Absolute, out uri))
                return null;
            return uri.Host;
        }

        /// <summary>
        /// Get the DateTime, which is in the middle of given period
        /// </summary>
        /// <param name="dateFrom"></param>
        /// <param name="dateTo"></param>
        /// <returns></returns>
        public static DateTime GetDateTimeMiddle(DateTime dateFrom, DateTime dateTo)
        {
            TimeSpan ts;
            return GetDateTimeMiddle(dateFrom, dateTo, out ts);
        }

        public static DateTime GetDateTimeMiddle(DateTime dateFrom, DateTime dateTo, out TimeSpan length)
        {
            length = dateTo.Subtract(dateFrom);
            return dateFrom.AddSeconds(length.TotalSeconds / 2);
        }

        public static DateTime StartOfDay(this DateTime theDate)
        {
            return theDate.Date;
        }

        public static DateTime EndOfDay(this DateTime theDate)
        {
            return theDate.Date.AddDays(1).AddTicks(-1);
        }

        public static bool IsDefault(this DateTime theDate)
        {
            return theDate == default(DateTime);
        }

        public static DateTime StartOfMonth(this DateTime theDate)
        {
            return new DateTime(theDate.Year,theDate.Month,1, 0,0,0, theDate.Kind);
        }

        public static DateTime EndOfMonth(this DateTime theDate)
        {
            return theDate.StartOfMonth().AddMonths(1).AddSeconds(-1);
        }

//        public static DateTime StartOfMonth(this DateTime theDate, DateTimeKind kind)
//        {
//            return new DateTime(theDate.Year, theDate.Month, 1, 0, 0, 0, kind);
//        }
//
//        public static DateTime EndOfMonth(this DateTime theDate, DateTimeKind kind)
//        {
//            return theDate.StartOfMonth(kind).AddMonths(1).AddSeconds(-1);
//        }
    }
}
