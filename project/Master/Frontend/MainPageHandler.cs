using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using TimeMiner.Master.Frontend.Plugins;

namespace TimeMiner.Master.Frontend
{
    [HandlerPath("")]
    class MainPageHandler:IFrontendServerHandler
    {
        public PageBuilder Handle(HttpListenerRequest req, HttpListenerResponse resp)
        {
            return new PageBuilder("ThisIsMainPage");
        }
    }
}
