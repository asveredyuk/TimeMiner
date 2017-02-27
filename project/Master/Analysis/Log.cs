using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeMiner.Core;
using TimeMiner.Master.Settings;

namespace TimeMiner.Master.Analysis
{
    public class Log
    {
        public enum ARelevance
        {
            good,
            neutral,
            bad,
            unknown
        }
        public Profile Prof { get; set; }
        private LogRecord[] records;

        public Log(LogRecord[] records)
        {
            this.records = records;
        }

        public Dictionary<ARelevance, int> GetRelevanceTimes()
        {
            if(Prof == null)
                throw new Exception("Profile is not set");
            Dictionary<ARelevance,int> dict = new Dictionary<ARelevance, int>();
            dict[ARelevance.bad] = dict[ARelevance.good] = dict[ARelevance.neutral] = dict[ARelevance.unknown] = 0;
            var rels = IndexRels(Prof.Relevances);
            foreach (var aRelevance in GetRelevances(rels))
            {
                dict[aRelevance]++;
            }
            return dict;

        }
        public IEnumerable<ARelevance> GetRelevances(Dictionary<string, Relevance> rels)
        {
            foreach (var logRecord in records)
            {
                Relevance rel;
                if (!rels.TryGetValue(logRecord.Process.ProcessName.ToLower(), out rel))
                {
                    yield return ARelevance.unknown;
                }
                else
                {
                    yield return (ARelevance)rel;
                }
            }
        }

        public Dictionary<string, Relevance> IndexRels(IEnumerable<ProfileApplicationRelevance> rels)
        {
            Dictionary<string, Relevance> dict = new Dictionary<string, Relevance>();
            foreach (var profileApplicationRelevance in rels)
            {
                dict[profileApplicationRelevance.App.ProcName] = profileApplicationRelevance.Rel;
            }
            return dict;
        }

    }
}
