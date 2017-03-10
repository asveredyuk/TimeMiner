using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using TimeMiner.Core;
using TimeMiner.Master.Settings;
using TimeMiner.Master.Settings.ApplicationIdentifiers;

namespace TimeMiner.Master.Analysis
{
    public class IndexedProfile:Profile
    {
        private Dictionary<ApplicationIdentifierBase, ProfileApplicationRelevance> index;
        private List<ApplicationIdentifierBase> allIdentifiers;
        private IndexedProfile(Guid id, string name, IReadOnlyList<ProfileApplicationRelevance> relevances)
        {
            Id = id;
            Name = name;
            Relevances = relevances;

            index = new Dictionary<ApplicationIdentifierBase, ProfileApplicationRelevance>();
            allIdentifiers = new List<ApplicationIdentifierBase>();
            foreach (var profileApplicationRelevance in Relevances)
            {
                foreach (var identifier in profileApplicationRelevance.App.Identifiers)
                {
                    index[identifier] = profileApplicationRelevance;
                }
                allIdentifiers.AddRange(profileApplicationRelevance.App.Identifiers);
            }
        }

        public ApplicationIdentifierBase FindIdentifier(LogRecord record)
        {
            //TODO: optimize search
            //return allIdentifiers.Find(t => t.CheckRecord(record)); //allIdentifiers.Where(t => t.CheckRecord(record)).First();
            ApplicationIdentifierBase[] ress = /*Relevances
                .SelectMany(t => t.App.Identifiers)*/allIdentifiers
                .Where(t=>t.CheckRecord(record)>0)
                .ToArray();
//            if (ress.Length > 1)
//            {
//                string message = "Unambigious identification,\r\n";
//                message += "Conflicting apps:\r\n";
//                foreach (var applicationIdentifierBase in ress)
//                {
//                    message += this[applicationIdentifierBase].App.Name + "\r\n";
//                }
//                message += "Record (json):\r\n";
//                message += JsonConvert.SerializeObject(record);
//                Console.WriteLine(message);
//            }
//            if(ress.Length == 0)
//                return null;

//            return ress[0];
            return ress.OrderByDescending(t => t.CheckRecord(record)).FirstOrDefault();
        }
        public ProfileApplicationRelevance this[ApplicationIdentifierBase key]
        {
            get
            {
                ProfileApplicationRelevance rel;
                if (index.TryGetValue(key, out rel))
                {
                    return rel;
                }
                return null;
            }
        }

        public Relevance GetExtendedRelevance(ApplicationIdentifierBase key)
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
