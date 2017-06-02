using System;
using System.CodeDom;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Markup;
using Newtonsoft.Json.Linq;
using TimeMiner.Master.Frontend.Plugins;

namespace TimeMiner.Master.Frontend.BuiltInExtensions
{
    public class PluginManagementExtension: FrontendServerExtensionBase
    {
        [MenuItem("Plugins", "config/plugins", 15)]
        [HandlerPath("config/plugins")]
        public HandlerPageDescriptor Handler(HttpListenerRequest req, HttpListenerResponse resp)
        {
            string head = WWWRes.GetString("plugins/head.html");
            string page = WWWRes.GetString("plugins/page.html");
            return new HandlerPageDescriptor(page, head);
        }
        [PublicHandler]
        [ApiPath("config/plugins/get")]
        public void GetPlugins(HttpListenerRequest req, HttpListenerResponse resp)
        {
            var assemblies = MasterPluginRepository.Self.GetAllAssemblies();
            List<object> res = new List<object>();
            foreach (var assembly in assemblies)
            {
                //skip exe assembly
                if(assembly == Assembly.GetExecutingAssembly())
                    continue;
                var descriptor = new
                {
                    Guid = assembly.GetCustomAttribute<GuidAttribute>()?.Value,
                    Title = assembly.GetCustomAttribute<AssemblyTitleAttribute>()?.Title,
                    Description = assembly.GetCustomAttribute<AssemblyDescriptionAttribute>()?.Description,
                    Version = assembly.GetName().Version.ToString(),
                    Author = assembly.GetCustomAttribute<AssemblyCompanyAttribute>()?.Company
                };
                res.Add(descriptor);
            }
            WriteObjectJsonAndClose(resp, res);
        }
        [PublicHandler]
        [ApiPath("config/plugins/install")]
        public void InstallPlugin(HttpListenerRequest req, HttpListenerResponse resp)
        {
            var base64 = ReadPostString(req);
            var data = Convert.FromBase64String(base64);
            bool result = MasterPluginRepository.Self.TryInstallAssembly(data);
            object response = null;
            if (result)
            {
                response = new
                {
                    Success = "success"
                };
            }
            else
            {
                response = new
                {
                    Error = "failed to add plugin"
                };
            }
            WriteObjectJsonAndClose(resp, response);
        }
        [PublicHandler]
        [ApiPath("config/plugins/uninstall")]
        public void UninstallPlugin(HttpListenerRequest req, HttpListenerResponse resp)
        {
            var json = ReadPostString(req);
            var jobj = JObject.Parse(json);
            Guid guid;
            if (jobj["Guid"] == null || !Guid.TryParse(jobj["Guid"].Value<string>(), out guid))
            {
                WriteStringAndClose(resp, "No guid or it is invalid", 400);
                return;
            }
            var res = MasterPluginRepository.Self.TryUninstallAssembly(guid);
            if (!res)
            {
                CloseWithCode(resp, 500);
            }
        }
    }
}
