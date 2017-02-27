using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Mustache;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TimeMiner.Master.Frontend.Plugins;
using TimeMiner.Master.Settings;

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
            if (root == "add")
            {
                SettingsContainer container = SettingsContainer.Self;
                container.PutNewApp(new ProfileApplicationRelevance(Relevance.good, new ApplicationDescriptor("Microsoft word","winword.exe")));
                container.PutNewApp(new ProfileApplicationRelevance(Relevance.bad, new ApplicationDescriptor("Telegram messenger", "telegram.exe")));
                container.PutNewApp(new ProfileApplicationRelevance(Relevance.neutral,
                    new ApplicationDescriptor("Windows explorer", "rexplorer.exe")));
            }
            var res = new HandlerPageDescriptor(WWWRes.GetString("apps/table/tablepage.html"),WWWRes.GetString("apps/table/tablehead.html"));
            return res;
        }
        [ApiPath("apps")]
        public void ApiHandler(HttpListenerRequest req, HttpListenerResponse resp)
        {
            string path = SkipApiAndRoot(req.Url.AbsolutePath);
            Thread.Sleep(1000);
            switch (path)
            {
                case "gettable":
                    WriteStringAndClose(resp, GetTableString());
                    break;
                case "updateapp":
                    UpdateApp(req,resp);
                    break;
                case "addapp":
                    AddApp(req,resp);
                    break;
                case "rmapp":
                    RemoveApp(req,resp);
                    break;
                default:
                    CloseWithCode(resp, 404);
                    break;
            }
        }

        private void RemoveApp(HttpListenerRequest req, HttpListenerResponse resp)
        {
            string str = ReadPostString(req);
            JObject obj = JObject.Parse(str);
            if (obj["Id"] == null)
            {
                CloseWithCode(resp, 400);
                return;
            }
            Guid id;
            if (!Guid.TryParse(obj["Id"].Value<string>(), out id))
            {
                CloseWithCode(resp, 400);
                return;
            }
            SettingsContainer.Self.RemoveAppAnRelevances(id);
            resp.Close();

        }
        
        private void AddApp(HttpListenerRequest req, HttpListenerResponse resp)
        {
            string str = ReadPostString(req);
            JObject obj = JObject.Parse(str);
            if (obj["AppName"] == null || obj["ProcName"] == null)
            {
                WriteStringAndClose(resp,"", 400);
                return;
            }
            string appName = obj["AppName"].Value<string>();
            string procName = obj["ProcName"].Value<string>();
            ApplicationDescriptor desc = new ApplicationDescriptor(appName,procName);
            ProfileApplicationRelevance rel = new ProfileApplicationRelevance(Relevance.neutral, desc);
            SettingsContainer.Self.PutNewApp(rel);
            resp.Close();

        }
        private void UpdateApp(HttpListenerRequest req, HttpListenerResponse resp)
        {
            string str = ReadPostString(req);
            ProfileApplicationRelevance rel = JsonConvert.DeserializeObject<ProfileApplicationRelevance>(str);
            SettingsContainer.Self.UpdateApp(rel.App);
            SettingsContainer.Self.UpdateRelevance(rel);
            Console.WriteLine($"changed to {rel.Rel}");
            resp.Close();
        }
      /*  /// <summary>
        /// Is called when /ajax path is handled
        /// </summary>
        /// <param name="req"></param>
        /// <param name="resp"></param>
        private void HandleAjax(HttpListenerRequest req, HttpListenerResponse resp)
        {
            //now we have only one ajax request
            
        }*/

        private string GetTableString()
        {
            /*string res = "";
            foreach (var rel in SettingsContainer.Self.GetBaseProfile().Relevances)
            {
                res += mustacheGenerator.Render(rel);
            }
            return res;*/
            
            return JsonConvert.SerializeObject(SettingsContainer.Self.GetBaseProfile().Relevances);

            //return WWWRes.GetString("txt.html");
        }
    }
}
