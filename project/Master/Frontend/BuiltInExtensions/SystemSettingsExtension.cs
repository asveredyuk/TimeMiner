using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using TimeMiner.Master.Frontend.Plugins;

namespace TimeMiner.Master.Frontend.BuiltInExtensions
{
    public class SystemSettingsExtension : FrontendServerExtensionBase
    {
        [MenuItem("Settings", "config/settings", 16)]
        [HandlerPath("config/settings")]
        public HandlerPageDescriptor Handler(HttpListenerRequest req, HttpListenerResponse resp)
        {
            return new HandlerPageDescriptor("Settings page");
        }
    }
}
