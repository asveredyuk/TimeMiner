using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using TimeMiner.Master.Settings;

namespace TimeMiner.Master.Analysis
{
    public class IndexedProfile:Profile
    {
        private Dictionary<string, ProfileApplicationRelevance> index;
        private IndexedProfile(Guid id, string name, IReadOnlyList<ProfileApplicationRelevance> relevances)
        {
            Id = id;
            Name = name;
            Relevances = relevances;
            index = new Dictionary<string, ProfileApplicationRelevance>();
            foreach (var profileApplicationRelevance in Relevances)
            {
                index[profileApplicationRelevance.App.ProcName] = profileApplicationRelevance;
            }
        }

        public ProfileApplicationRelevance this[string key]
        {
            get
            {
                key = key.ToLower();
                ProfileApplicationRelevance rel;
                if (index.TryGetValue(key, out rel))
                {
                    return rel;
                }
                return null;
            }
        }

        public Relevance GetExtendedRelevance(string key)
        {
            ProfileApplicationRelevance rel = this[key];
            if(rel == null)
                return Relevance.unknown;
            return rel.Rel;
        }
        /// <summary>
        /// Make indexed profile from existing profile
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public static IndexedProfile FromProfile(Profile p)
        {
            return new IndexedProfile(p.Id,p.Name,p.Relevances);
        }
    }
}
