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
    class PageBuilder
    {
        /// <summary>
        /// Stays before key in body
        /// </summary>
        public const string KEY_PREFIX = "{%";
        /// <summary>
        /// Stays after key in body
        /// </summary>
        public const string KEY_POSTFIX = "%}";
        /// <summary>
        /// Included page builders
        /// </summary>
        private Dictionary<string, PageBuilder> links;
        /// <summary>
        /// Body of builder with keys to replace with another pagebuilders
        /// </summary>
        public string Body { get; private set; }
        /// <summary>
        /// Create new pagebuilder with given body
        /// </summary>
        /// <param name="body">body</param>
        public PageBuilder(string body)
        {
            links = new Dictionary<string, PageBuilder>();
            Body = body;
        }
        /// <summary>
        /// Put new page builder with given key
        /// </summary>
        /// <param name="key"></param>
        /// <param name="builder"></param>
        public void Put(string key, PageBuilder builder)
        {
            if (links.ContainsKey(key))
            {
                throw new Exception("such key already exists");
            }
            links[key] = builder;
        }
        /// <summary>
        /// Build resulting page
        /// </summary>
        /// <returns></returns>
        public string Build()
        {
            string res = Body;
            foreach (var pair in links)
            {
                res = res.Replace(KEY_PREFIX + pair.Key + KEY_POSTFIX, pair.Value.Build());
            }
            return res;
        }
    }
}
