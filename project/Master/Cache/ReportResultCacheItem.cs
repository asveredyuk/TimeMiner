using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeMiner.Master.Analysis;
using TimeMiner.Master.Database;

namespace TimeMiner.Master.Cache
{
    public class ReportResultCacheItem
    {
        public string FileHash { get; set; }
        public string ProfileHash { get; set; }
        public Type ReportType { get; set; }
        public BaseReportResult Result { get; set; }

        public ReportResultCacheItem()
        {
        }

        public ReportResultCacheItem(string fileHash, string profileHash, Type reportType, BaseReportResult result)
        {
            FileHash = fileHash;
            ProfileHash = profileHash;
            ReportType = reportType;
            Result = result;
        }
    }
}
