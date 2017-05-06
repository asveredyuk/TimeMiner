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
    public class HandlerPageDescriptor
    {
        /// <summary>
        /// Page of descriptor with keys to replace with another pagebuilders
        /// </summary>
        public string Page { get; set; }
        /// <summary>
        /// Html code, included into the head
        /// </summary>
        public string Head { get; set; }
        public HandlerPageDescriptor(string page)
        {
            Page = page;
        }

        public HandlerPageDescriptor(string page, string head)
        {
            Page = page;
            Head = head;
        }
    }
}
