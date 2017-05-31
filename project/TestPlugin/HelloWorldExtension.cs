using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using TimeMiner.Master.Frontend;
using TimeMiner.Master.Frontend.Plugins;

namespace TestPlugin
{
    public class HelloWorldExtension : FrontendServerExtensionBase
    {
        [MenuItem("Hello World", "hwrld", 1000)]
        [HandlerPath("helloworld")]
        public HandlerPageDescriptor Handler(HttpListenerRequest req, HttpListenerResponse resp)
        {
            return new HandlerPageDescriptor("<h1>Hello World!</h1>");
        }
    }
}
