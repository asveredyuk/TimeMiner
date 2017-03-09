using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeMiner.Master.Settings.ApplicationIdentifiers;

namespace TimeMiner.Master.Settings
{
    /// <summary>
    /// Keeps description and identification information about an application 
    /// </summary>
    public class ApplicationDescriptor
    { 
        public Guid Id { get; set; }
        public string Name { get; set; }
        public List<ApplicationIdentifierBase> Identifiers { get; set; }

        public string ProcName
        {
            get { return (Identifiers[0] as ProcessNameIdetifier).ProcessName; }
        }

        public ApplicationDescriptor()
        {
        }

        public ApplicationDescriptor(string name, params ApplicationIdentifierBase[] identifiers)
        {
            Id = Guid.NewGuid();
            Name = name;
            Identifiers = new List<ApplicationIdentifierBase>(identifiers);
        }
    }
}
