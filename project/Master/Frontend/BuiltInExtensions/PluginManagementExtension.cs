using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using TimeMiner.Master.Frontend.Plugins;

namespace TimeMiner.Master.Frontend.BuiltInExtensions
{
    public class PluginManagementExtension: FrontendServerExtensionBase
    {
        [MenuItem("Plugins", "config/plugins", 15)]
        [HandlerPath("config/plugins")]
        public HandlerPageDescriptor Handler(HttpListenerRequest req, HttpListenerResponse resp)
        {
            string head = WWWRes.GetString("plugins/head.html");
            string page = WWWRes.GetString("plugins/page.html");
            return new HandlerPageDescriptor(page, head);
        }
    }
}
