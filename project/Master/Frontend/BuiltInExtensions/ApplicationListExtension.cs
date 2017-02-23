﻿using System;
using System.Collections.Generic;
using System.IO;
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
            //return new HandlerPageDescriptor($"subpath:{subpath}<br>root:{root}");*/
            if (root == "ajax")
            {
                HandleAjax(req,resp);
                return null;
            }
            var res = new HandlerPageDescriptor(WWWRes.GetString("table/tablepage.html"),WWWRes.GetString("table/tablehead.html"));
            return res;
        }

        /// <summary>
        /// Is called when /ajax path is handled
        /// </summary>
        /// <param name="req"></param>
        /// <param name="resp"></param>
        private void HandleAjax(HttpListenerRequest req, HttpListenerResponse resp)
        {
            //now we have only one ajax request
            using (StreamWriter sw = new StreamWriter(resp.OutputStream))
            {
                sw.Write(GetTableString());
                sw.Close();
            }
        }

        private string GetTableString()
        {
            return WWWRes.GetString("txt.html");
        }
    }
}
