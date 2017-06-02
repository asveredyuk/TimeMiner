using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TimeMiner.Master.Frontend.Plugins;

namespace TimeMiner.Master.Frontend.BuiltInExtensions
{
    public class SlaveBoundaryExtension : FrontendServerExtensionBase
    {
        [PublicHandler]
        [ApiPath("slave/plugins/slave_getassembly")]
        public void GetAssembly(HttpListenerRequest req, HttpListenerResponse resp)
        {
            string json = ReadPostString(req);
            var jobj = JObject.Parse(json);
            Guid guid;
            if (jobj["Guid"] == null || !Guid.TryParse(jobj["Guid"].Value<string>(), out guid))
            {
                CloseWithCode(resp, 400);
                return;
            }
            byte[] bytes = MasterPluginRepository.Self.GetAssemblyBytes(guid);
            if (bytes == null)
            {
                CloseWithCode(resp,400);
                return;
            }
            WriteBytesAndClose(resp,bytes);
        }
        [PublicHandler]
        [ApiPath("slave/plugins/slave_getlist")]
        public void GetPluginList(HttpListenerRequest req, HttpListenerResponse resp)
        {
            var descs = MasterPluginRepository.Self.GetDescriptors().Select(p => p.Value).ToArray();
            string json = JsonConvert.SerializeObject(descs);
            WriteStringAndClose(resp,json);
        }
    }
}
