using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Mustache;
using TimeMiner.Master.Frontend.Plugins;
using TimeMiner.Master.Settings;

namespace TimeMiner.Master.Frontend.BuiltInExtensions
{
    class ApplicationListExtension: FrontendServerExtensionBase
    {
        private Generator mustacheGenerator;
        private const string TABLE_ROW_TEMPLATE_PATH = "table/tablerow.html";
        
        public ApplicationListExtension()
        {
            MenuItems.Add(new TemplatePageMenuItem("Apps","/apps"));
            var mustacheCompiler = new FormatCompiler();
            mustacheGenerator = mustacheCompiler.Compile(WWWRes.GetString(TABLE_ROW_TEMPLATE_PATH));
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
            if (root == "add")
            {
                SettingsContainer container = SettingsContainer.Self;
                container.PutNewApp(new ProfileApplicationRelevance(ProfileApplicationRelevance.Relevance.good, new ApplicationDescriptor("Microsoft word","winword.exe")));
                container.PutNewApp(new ProfileApplicationRelevance(ProfileApplicationRelevance.Relevance.bad, new ApplicationDescriptor("Telegram messenger", "telegram.exe")));
                container.PutNewApp(new ProfileApplicationRelevance(ProfileApplicationRelevance.Relevance.neutral, new ApplicationDescriptor("Windows explorer", "rexplorer.exe")));
            }
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
            string res = "";
            foreach (var rel in SettingsContainer.Self.GetBaseProfile().Relevances)
            {
                res += mustacheGenerator.Render(rel);
            }
            return res;
            //return JsonConvert.SerializeObject(SettingsContainer.Self.GetBaseProfile().Relevances)

            //return WWWRes.GetString("txt.html");
        }
    }
}
