using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace TimeMiner.Master.Frontend
{
    class ResponseMaker
    {
        private Dictionary<string, IFrontendServerHandler> handlers;
        private ZipResourceContainer res;
        public ResponseMaker(ZipResourceContainer res)
        {
            handlers = new Dictionary<string, IFrontendServerHandler>();
            handlers[""] = new MainPageHandler();
            handlers["login"] = new LoginHandler();
            this.res = res;
        }

        public void OnRequest(HttpListenerRequest req, HttpListenerResponse resp)
        {
            string q = req.Url.AbsolutePath.Trim('/');
            string template = res.GetString("template.html");
            PageBuilder hbu = handlers[q].Handle(req, resp);
            if (hbu == null)
            {
                Console.Out.WriteLine("No page builder returned from handler");
                return;
            }
            PageBuilder bu = new PageBuilder(template);
            bu.Put("PAGE", hbu);
            using (StreamWriter sw = new StreamWriter(resp.OutputStream))
            {
                sw.Write(bu.Build());
                sw.Close();
            }

        }
    }
}
