using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiteDB;
using Newtonsoft.Json;

namespace TimeMiner.Master.Settings
{
    public class Profile
    {
        public Guid Id { get; set; }
        public string Name { get; set; }

        [BsonIgnore]
        public IReadOnlyList<ProfileApplicationRelevance> Relevances
        {
            get;
            set;
        }

        public Profile()
        {
            Relevances = new List<ProfileApplicationRelevance>();
        }

        public string ComputeHash()
        {
            string json = JsonConvert.SerializeObject(this);
            return Util.ComputeStringMD5Hash(json);
        }
    }
}
