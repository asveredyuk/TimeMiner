using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using TimeMiner.Master.Frontend.Plugins;

namespace TimeMiner.Master.Frontend.BuiltInExtensions
{
    class ApplicationListExtension: FrontendServerExtensionBase
    {
        public ApplicationListExtension()
        {
            MenuItems.Add(new TemplatePageMenuItem("Apps","/apps"));
        }

        [HandlerPath("apps")]
        public HandlerPageDescriptor Handle(HttpListenerRequest req, HttpListenerResponse resp)
        {
            string path = req.Url.AbsolutePath;
            string subpath = GetSubPath(path);
            string root = GetPathRoot(subpath);
            //here implement main page with table
            //and ajax responses at subpath /ajax
            return new HandlerPageDescriptor($"subpath:{subpath}<br>root:{root}");
        }
    }
}
