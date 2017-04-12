using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeMiner.Core;

namespace TimeMiner.Master.Settings.ApplicationIdentifiers
{
    /// <summary>
    /// Identifier for website
    /// </summary>
    public class WebsiteIdentifier : ApplicationIdentifierBase
    {
        /// <summary>
        /// Host of website
        /// </summary>
        public string Host { get; set; }

        public WebsiteIdentifier()
        {
        }

        public WebsiteIdentifier(string host)
        {
            Host = host;
        }

        /// <inheritdoc />
        public override int CheckRecord(LogRecord record)
        {
            string url = record.GetMetaString("url");
            //if there is no url - no relation
            if (url == null)
                return 0;
            string host = Util.GetHostFromUrl(url);
            //if url is broken - no relation
            if (host == null)
                return 0;
            if (host.EndsWith(Host))
            {
                return Host.Length;
                //the bigger host is - it wins
                //Ex. two identifiers vk.com and m.vk.com
                //real host is m.vk.com
                //it has larger length and it wins
            }
            return 0;
        }
    }
}
