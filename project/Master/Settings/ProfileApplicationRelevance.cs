using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiteDB;

namespace TimeMiner.Master.Settings
{
    /// <summary>
    /// Describes relevance of application to profile
    /// </summary>
    class ProfileApplicationRelevance
    {
        public enum Relevance
        {
            Good,
            Neutral,
            Bad
        }
        public Relevance Rel { get; set; }
        [BsonRef(SettingsProvider.APPS_LIST_COL_NAME)]
        public ApplicationDescriptor App { get; set; }
    }
}
