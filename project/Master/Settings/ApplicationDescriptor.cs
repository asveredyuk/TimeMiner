using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeMiner.Master.Settings
{
    /// <summary>
    /// Keeps description and identification information about an application 
    /// </summary>
    public class ApplicationDescriptor
    { 
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string ProcName { get; set; }

        public ApplicationDescriptor()
        {
        }

        public ApplicationDescriptor(string name, string procName)
        {
            Id = Guid.NewGuid();
            Name = name;
            ProcName = procName;
        }
    }
}
