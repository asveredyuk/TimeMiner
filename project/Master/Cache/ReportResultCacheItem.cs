using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiteDB;
using TimeMiner.Master.Analysis;
using TimeMiner.Master.Database;

namespace TimeMiner.Master.Cache
{
    public class ReportResultCacheItem
    {
        public string Id
        {
            get { return DataHash + ReportTypeGuid.ToString() + ProfileHash; }
        }
        [BsonIndex]
        public string DataHash { get; set; }
        [BsonIndex]
        public string ProfileHash { get; set; }
        [BsonIndex]
        public Guid ReportTypeGuid { get; set; }
        public BaseReportResult Result { get; set; }

        public ReportResultCacheItem()
        {
        }

        public ReportResultCacheItem(string dataHash, string profileHash, Guid reportTypeGuid, BaseReportResult result)
        {
            DataHash = dataHash;
            ProfileHash = profileHash;
            ReportTypeGuid = reportTypeGuid;
            Result = result;
        }
    }
}
