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
using TimeMiner.Master.Frontend.Plugins;
using TimeMiner.Master.Settings;
using TimeMiner.Master.Settings.ApplicationIdentifiers;

namespace TimeMiner.Master.Frontend.BuiltInExtensions
{
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
            MenuItems.Add(new TemplatePageMenuItem("Apps","/apps"));
        }

        [HandlerPath("apps")]
        public HandlerPageDescriptor Handle(HttpListenerRequest req, HttpListenerResponse resp)
        {
//            string path = req.Url.AbsolutePath;
//            string subpath = GetSubPath(path);
//            string root = GetPathRoot(subpath);
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
            if (obj["AppName"] == null || obj["ProcName"] == null || obj["Type"] == null)
            {
                WriteStringAndClose(resp,"", 400);
                return;
            }
            string appName = obj["AppName"].Value<string>();
            string procName = obj["ProcName"].Value<string>();
            Relevance type;
            if (!Enum.TryParse(obj["Type"].Value<string>(), out type))
            {
                WriteStringAndClose(resp,"", 400);
            }
            ApplicationDescriptor desc = new ApplicationDescriptor(appName,new ProcessNameIdetifier(procName));
            ProfileApplicationRelevance rel = new ProfileApplicationRelevance(type, desc);
            SettingsContainer.Self.PutNewApp(rel);
            resp.Close();

        }
        private void UpdateApp(HttpListenerRequest req, HttpListenerResponse resp)
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
            
            return JsonConvert.SerializeObject(SettingsContainer.Self.GetBaseProfile().Relevances, Formatting.Indented, new JsonSerializerSettings()
                {
                    TypeNameHandling = TypeNameHandling.Auto,
                    Binder = binder
                });

            //return WWWRes.GetString("txt.html");
        }
    }
}
