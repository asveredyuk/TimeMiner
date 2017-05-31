using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
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
            var assemblies = PluginRepository.Self.GetAllAssemblies();
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
    }
}
