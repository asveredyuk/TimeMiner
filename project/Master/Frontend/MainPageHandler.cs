using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace TimeMiner.Master.Frontend
{
    class MainPageHandler:IFrontendServerHandler
    {
        public static string[] QUERIES = { "" };

        public string[] HandledQueries
        {
            get { return QUERIES; }
        }

        public PageBuilder Handle(HttpListenerRequest req, HttpListenerResponse resp)
        {
            return new PageBuilder("ThisIsMainPage");
        }
    }
}
