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
        /// <summary>
        /// Guid of given descriptor
        /// </summary>
        public Guid Id { get; set; }
        /// <summary>
        /// Application name
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// List of identifiers
        /// </summary>
        public List<ApplicationIdentifierBase> Identifiers { get; set; }

        /// <summary>
        /// Create new application descriptor
        /// </summary>
        public ApplicationDescriptor()
        {
        }
        /// <summary>
        /// Create new application descriptor
        /// </summary>
        /// <param name="name">Application name</param>
        /// <param name="identifiers">Identifiers</param>
        public ApplicationDescriptor(string name, params ApplicationIdentifierBase[] identifiers)
        {
            Id = Guid.NewGuid();
            Name = name;
            Identifiers = new List<ApplicationIdentifierBase>(identifiers);
        }
    }
}
