using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Mustache;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TimeMiner.Master.Analysis;
using TimeMiner.Master.Frontend.Plugins;
using TimeMiner.Master.Settings;
using TimeMiner.Master.Settings.ApplicationIdentifiers;

namespace TimeMiner.Master.Frontend.BuiltInExtensions
{
    [MenuItem("Configuration","config", 15)]
    class ApplicationListExtension: FrontendServerExtensionBase
    {
        private class ApplicationIdentifierTypesBinder : SerializationBinder
        {
            private List<Type> knownTypes;

            public override Type BindToType(string assemblyName, string typeName)
            {
                return knownTypes.SingleOrDefault(t => t.Name == typeName);
            }

            public override void BindToName(Type serializedType, out string assemblyName, out string typeName)
            {
                assemblyName = null;
                if (knownTypes.Contains(serializedType))
                    typeName = serializedType.Name;
                else
                    typeName = null;
            }

            private ApplicationIdentifierTypesBinder(List<Type> types)
            {
                knownTypes = types;
            }

            public static ApplicationIdentifierTypesBinder MakeFromCurrentAssembly()
            {
                List<Type> types = new List<Type>(
                    Assembly.GetAssembly(typeof(ApplicationIdentifierTypesBinder))
                    .GetTypes()
                    .Where(t=>t.IsSubclassOf(typeof(ApplicationIdentifierBase)))
                    );
                return new ApplicationIdentifierTypesBinder(types);
            }
        }

        private ApplicationIdentifierTypesBinder binder;
        public ApplicationListExtension()
        {
            binder = ApplicationIdentifierTypesBinder.MakeFromCurrentAssembly();
            /*MenuItems.Add(new FrontendPageMenuItem("Apps",
                    new FrontendPageMenuItem("Applications","/apps/apps"),
                    new FrontendPageMenuItem("Sites","/apps/sites")
                ));*/
        }
        [MenuItem("Applications","config/apps")]
        [HandlerPath("config/apps")]
        public HandlerPageDescriptor HandleApps(HttpListenerRequest req, HttpListenerResponse resp)
        {
            var arg = new { Type = "Application", type = "application", isapplication = true, Title = "Applications" };
            return MakeTablepage(arg);
        }
        [MenuItem("Sites","config/sites")]
        [HandlerPath("config/sites")]
        public HandlerPageDescriptor HandleSites(HttpListenerRequest req, HttpListenerResponse resp)
        {
            var arg = new { Type = "Site", type = "site", isapplication = false, Title = "Sites" };
            return MakeTablepage(arg);
        }
        [MenuItem("Unknown apps & sites", "config/unknown")]
        [HandlerPath("config/unknown")]
        public HandlerPageDescriptor UnknownHandler(HttpListenerRequest req, HttpListenerResponse resp)
        {
            string page = WWWRes.GetString("apps/unknown/page.html");
            string head = WWWRes.GetString("apps/unknown/head.html");
            var descriptor = new HandlerPageDescriptor(page,head);
            return descriptor;
        }
        //TODO: remove this method
        [HandlerPath("config")]
        public HandlerPageDescriptor HandleMain(HttpListenerRequest req, HttpListenerResponse resp)
        {
            return new HandlerPageDescriptor("Main config page");
        }

        private HandlerPageDescriptor MakeTablepage(object arg)
        {
            string page = WWWRes.GetString("apps/table/tablepage.html");
            var mustacheCompiler = new FormatCompiler();
            var generator = mustacheCompiler.Compile(page);
            string rendered = generator.Render(arg);
            var res = new HandlerPageDescriptor(rendered, WWWRes.GetString("apps/table/tablehead.html"));
            return res;
        }
        [ApiPath("apps/gettable")]
        public void ApiGetTableCall(HttpListenerRequest req, HttpListenerResponse resp)
        {
            string path = SkipApiAndRoot(req.Url.AbsolutePath);// /gettable/?
            string type = GetPathRoot(GetSubPath(path));
            WriteStringAndClose(resp, GetTableString(type));
        }
        [ApiPath("apps/rmapp")]
        public void RemoveApp(HttpListenerRequest req, HttpListenerResponse resp)
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
        [ApiPath("apps/addapp")]
        public void AddApp(HttpListenerRequest req, HttpListenerResponse resp)
        {
            string str = ReadPostString(req);
            JObject obj = JObject.Parse(str);
            if (obj["AppName"] == null || obj["Type"] == null)
            {
                CloseWithCode(resp, 400);
                return;
            }
            string appName = obj["AppName"].Value<string>();
            ApplicationIdentifierBase identifier = null;
            if (obj["ProcName"] != null)
            {
                string procName = obj["ProcName"].Value<string>();
                identifier=new ProcessNameIdentifier(procName);
            }
            if (obj["DomainName"] != null)
            {
                string domainName = obj["DomainName"].Value<string>();
                identifier = new WebsiteIdentifier(domainName);
            }
            if (identifier == null)
            {
                CloseWithCode(resp,400);
            }
            Relevance type;
            if (!Enum.TryParse(obj["Type"].Value<string>(), out type))
            {
                CloseWithCode(resp, 400);
            }
            ApplicationDescriptor desc = new ApplicationDescriptor(appName,identifier);
            ProfileApplicationRelevance rel = new ProfileApplicationRelevance(type, desc);
            SettingsContainer.Self.PutNewApp(rel);
            resp.Close();

        }
        [ApiPath("apps/updateapp")]
        public void UpdateApp(HttpListenerRequest req, HttpListenerResponse resp)
        {
            //TODO:MAKE NORMAL REL PARSE

            string str = ReadPostString(req);
            ProfileApplicationRelevance rel = JsonConvert.DeserializeObject<ProfileApplicationRelevance>(str, new JsonSerializerSettings()
            {
                TypeNameHandling = TypeNameHandling.Auto,
                Binder = binder
            });
            SettingsContainer.Self.UpdateApp(rel.App);
            SettingsContainer.Self.UpdateRelevance(rel);
            Console.WriteLine($"changed to {rel.Rel}");
            resp.Close();
        }

        [ApiPath("apps/unknown")]
        public void GetUnknownApps(HttpListenerRequest req, HttpListenerResponse resp)
        {
            Log log = LogsDB.Self.GetLogRecordsForUserForPeriod(Guid.Empty, new DateTime(2017, 4, 1),
                new DateTime(2017, 4, 15));
            UnknownIdentificationReport report = new UnknownIdentificationReport(log);
            var result = report.Calculate().Items;
            WriteObjectJsonAndClose(resp,result);
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

        private string GetTableString(string type)
        {
            //TODO: bad code here, refactor
            IReadOnlyList<ProfileApplicationRelevance> rels = SettingsContainer.Self.GetBaseProfile().Relevances;
            switch (type)
            {
                case "application":
                    rels = rels.Where(t => t.App.Identifiers.Where(q => q is ProcessNameIdentifier).Count() > 0).ToList();
                    break;
                case "site":
                    rels = rels.Where(t => t.App.Identifiers.Where(q => q is WebsiteIdentifier).Count() > 0).ToList();
                    break;
            }
            /*string res = "";
            foreach (var rel in SettingsContainer.Self.GetBaseProfile().Relevances)
            {
                res += mustacheGenerator.Render(rel);
            }
            return res;*/
            
            return JsonConvert.SerializeObject(rels, Formatting.Indented, new JsonSerializerSettings()
                {
                    TypeNameHandling = TypeNameHandling.Auto,
                    Binder = binder
                });

            //return WWWRes.GetString("txt.html");
        }
    }
}
