using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeMiner.Core;

namespace TimeMiner.Master.Settings.ApplicationIdentifiers
{
    public class WebsiteIdentifier : ApplicationIdentifierBase
    {
    

        public string Host { get; set; }

        public WebsiteIdentifier()
        {
        }

        public WebsiteIdentifier(string host)
        {
            Host = host;
        }

        public override int CheckRecord(LogRecord record)
        {
            string site = record.GetMetaString("site");
            if (site == null)
                return 0;
            string host = GetHost(site);
            if (host == null)
            {
                return 0;
            }

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
        private string GetHost(string site)
        {

            Uri uri;
            if (!Uri.TryCreate(site, UriKind.Absolute, out uri))
                return null;
            return uri.Host;
        }
    }
}
