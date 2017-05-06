using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeMiner.Master.Settings
{
    public class UserInfo
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }

        public UserInfo()
        {
        }

        public UserInfo(Guid id, string name, string surname)
        {
            Id = id;
            Name = name;
            Surname = surname;
        }
    }
}
