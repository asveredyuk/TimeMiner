using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Mustache;

namespace TimeMiner.Master.Frontend
{
    class ResponseMaker
    {
        const string TEMPLATE_FILE_NAME = "template.html";
        private ZipResourceContainer res;
        private Generator mustacheGenerator;
        public ResponseMaker(ZipResourceContainer res)
        {
            this.res = res;
            var mustacheCompiler = new FormatCompiler();
            mustacheGenerator = mustacheCompiler.Compile(res.GetString(TEMPLATE_FILE_NAME));
        }

        public void OnRequest(HttpListenerRequest req, HttpListenerResponse resp)
        {
            string q = req.Url.AbsolutePath.Trim('/');
            HandlerPageDescriptor hbu = FrontendPluginLoader.Self.Handlers[q].Handle(req, resp);
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

        private string CompilePage(HandlerPageDescriptor hdesc)
        {
            TemplatedPageDescriptor de = new TemplatedPageDescriptor(hdesc);
            return mustacheGenerator.Render(de);
        }
    }
}
