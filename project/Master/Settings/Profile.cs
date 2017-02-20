using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiteDB;

namespace TimeMiner.Master.Settings
{
    class Profile
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        [BsonIgnore]
        public List<ProfileApplicationRelevance> Relevances { get; set; }

        public Profile()
        {
            Relevances = new List<ProfileApplicationRelevance>();
        }
    }
}
