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
    public class ProfileApplicationRelevance
    {
        public enum Relevance
        {
            good,
            neutral,
            bad
        }

        public Guid Id { get; set; }
        public Relevance Rel { get; set; }
        [BsonRef(SettingsContainer.APPS_LIST_COL_NAME)]
        public ApplicationDescriptor App { get; set; }

        public ProfileApplicationRelevance()
        {
        }

        public ProfileApplicationRelevance(Relevance rel, ApplicationDescriptor app)
        {
            Id = Guid.NewGuid();
            Rel = rel;
            App = app;
        }
    }
}
