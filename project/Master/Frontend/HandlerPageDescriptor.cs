using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeMiner.Master.Frontend
{
    /// <summary>
    /// Allows to make composition of pages
    /// </summary>
    class HandlerPageDescriptor
    {
        /// <summary>
        /// Page of descriptor with keys to replace with another pagebuilders
        /// </summary>
        public string Page { get; private set; }

        public HandlerPageDescriptor(string page)
        {
            Page = page;
        }
    }
}
