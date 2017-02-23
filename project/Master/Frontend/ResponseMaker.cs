using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using Mustache;

namespace TimeMiner.Master.Frontend
{
    class ResponseMaker
    {
        const string TEMPLATE_FILE_NAME = "template.html";
        private IResourceContainer res;
        private Generator mustacheGenerator;
        public ResponseMaker(IResourceContainer resources)
        {
            this.res = resources;
            var mustacheCompiler = new FormatCompiler();
            mustacheGenerator = mustacheCompiler.Compile(res.GetString(TEMPLATE_FILE_NAME));
        }

        public void OnRequest(HttpListenerRequest req, HttpListenerResponse resp)
        {
            string q = req.Url.AbsolutePath.Trim('/');
            //check if there is no only root
            if (q.Contains("/"))
            {
                //take only root
                q = q.Substring(0, q.IndexOf("/"));
            }
            HandlerPageDescriptor hbu = FrontendExtensionLoader.Self.RequestHandlers[q](req, resp);
            if (hbu == null)
            {
                Console.Out.WriteLine("No page builder returned from handler");
                return;
            }
            string res = CompilePage(hbu);
            using (StreamWriter sw = new StreamWriter(resp.OutputStream))
            {
                sw.Write(res);
                sw.Close();
            }

        }

        public void OnApiRequest(HttpListenerRequest req, HttpListenerResponse resp)
        {
            string q = req.Url.AbsolutePath.Trim('/');
            if (q == "api")
            {
                using (StreamWriter sw = new StreamWriter(resp.OutputStream))
                {
                    sw.Write("this is api page");
                    sw.Close();
                }
                return;
            }
            if (!q.StartsWith("api/"))
            {
                throw new Exception($"{q} is not api path, but api request handler was called");
            }
            //remove api part
            q = q.Substring(q.IndexOf("/")+1,q.Length - q.IndexOf("/")-1);
            //check if there is no only root
            if (q.Contains("/"))
            {
                //take only root
                q = q.Substring(0, q.IndexOf("/"));
            }
            FrontendExtensionLoader.Self.ApiHanlders[q](req, resp);
        }

        private string CompilePage(HandlerPageDescriptor hdesc)
        {
            TemplatePageDescriptor de = new TemplatePageDescriptor(hdesc);
            de.Menu.AddRange(FrontendExtensionLoader.Self.MenuItems);
            return mustacheGenerator.Render(de);
        }
    }
}
