using System;
using System.CodeDom;
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
            var handler = FrontendExtensionLoader.Self.GetRequestHandler(q);
            if (handler == null)
            {
                resp.StatusCode = 404;
                using (StreamWriter sw = new StreamWriter(resp.OutputStream))
                {
                    sw.Write("404, not found");
                    sw.Close();
                }
                return;
            }
            if (!handler.IsPublic && !CheckAuth(req, resp, handler))
            {
                //Access denied!
                resp.StatusCode = 403;

                using (var sw = new StreamWriter(resp.OutputStream))
                {
                    sw.Write("403 - forbidden");
                }
                resp.Close();
                return;
            }
            HandlerPageDescriptor hbu = handler.Handler(req, resp);
            if (hbu == null)
            {
                Console.Out.WriteLine("No page builder returned from handler");
                if (resp.OutputStream.CanWrite)
                {
                    resp.OutputStream.Close();
                }
                return;
            }
            string res = CompilePage(hbu);
            using (StreamWriter sw = new StreamWriter(resp.OutputStream))
            {
                sw.Write(res);
                sw.Close();
            }
        }

        public bool CheckAuth<T>(HttpListenerRequest req, HttpListenerResponse resp,
            FrontendExtensionLoader.HandlerMethodDescriptor<T> handler)
        {
            //public requests do not need any auth
            if (handler.IsPublic)
                return true;
            //check if there is a cookie
            var cookie = req.Cookies["auth_token"];
            if (cookie != null)
            {
                // for now, valid token is 'MasterToken';
                return cookie.Value == "MasterToken";
            }
            //check if there is a header
            var header = req.Headers["X-Auth-Token"];
            if (header != null)
            {
                return header == "MasterToken";
            }
            return false;

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
            q = q.Substring(q.IndexOf("/") + 1, q.Length - q.IndexOf("/") - 1);
            var handler = FrontendExtensionLoader.Self.GetApiRequestHandler(q);
            if (handler == null)
            {
                resp.StatusCode = 404;
                using (StreamWriter sw = new StreamWriter(resp.OutputStream))
                {
                    sw.Write("404, not found");
                    sw.Close();
                }
                return;
            }

            if (!handler.IsPublic && !CheckAuth(req, resp, handler))
            {
                //Access denied!
                resp.StatusCode = 403;

                using (var sw = new StreamWriter(resp.OutputStream))
                {
                    sw.Write("403 - forbidden");
                }
                resp.Close();
                return;
            }
            handler.Handler(req, resp);
            resp.Close();
        }

        private string CompilePage(HandlerPageDescriptor hdesc)
        {
            var menu = FrontendExtensionLoader.Self.Menu;
            TemplatePageDescriptor de = new TemplatePageDescriptor(hdesc, menu);
            return mustacheGenerator.Render(de);
        }
    }
}
