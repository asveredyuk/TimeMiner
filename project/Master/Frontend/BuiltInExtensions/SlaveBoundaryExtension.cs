using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TimeMiner.Master.Analysis;
using TimeMiner.Master.Frontend.Plugins;
using TimeMiner.Master.Settings;

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
        [PublicHandler]
        [ApiPath("slave/get_now_status")]
        public void GetStatus(HttpListenerRequest req, HttpListenerResponse resp)
        {
            //TODO: validate if input is too wrong to parse jobject
            const int LAST_SECONDS = 60;
            string postString = ReadPostString(req);
            var jobj = JObject.Parse(postString);
            Guid userGuid;
            if (jobj["Guid"] == null || !Guid.TryParse(jobj["Guid"].Value<string>(), out userGuid))
            {
                WriteStringAndClose(resp, "No or invalid guid", 400);
                return;
            }
            DateTime now = DateTime.Now;
            DateTime begin = now.AddSeconds(-LAST_SECONDS);
            DateTime end = now.AddSeconds(1); //for exclusive boundary
            ILog log = LogsDB.Self.GetCompositeLog(userGuid, begin, end);
            if (!log.Records.Any())
            {
                //nothing to analyze
                //TODO: write "non active!"
                var aobj = new
                {
                    Rel = 2
                };
                WriteStringAndClose(resp, JsonConvert.SerializeObject(aobj));
                return;
            }
            var activeReport = new ActiveReport(log);
            var lastActive = activeReport.GetActivitiesOnly().Last();
            var identifier = log.Prof.FindIdentifier(log.Records.Last());
            Relevance rel = Relevance.bad;
            if (identifier != null && lastActive)
            {
                var relevance = log.Prof[identifier];
                rel = relevance.Rel;
            }
            var answObj = new
            {
                Rel = rel
            };
            WriteStringAndClose(resp, JsonConvert.SerializeObject(answObj));
        }
        [PublicHandler]
        [ApiPath("slave/get_personal_stats")]
        public void GetPersonalStats(HttpListenerRequest req, HttpListenerResponse resp)
        {
            string postString = ReadPostString(req);
            var jobj = JObject.Parse(postString);
            Guid userGuid;
            if (jobj["Guid"] == null || !Guid.TryParse(jobj["Guid"].Value<string>(), out userGuid))
            {
                WriteStringAndClose(resp, "No or invalid guid", 400);
                return;
            }
            DateTime dtFrom = jobj["Begin"].Value<DateTime>();
            DateTime dtTo = jobj["End"].Value<DateTime>();
            if ((dtFrom - dtTo).TotalDays > 1.0)
            {
                //max range - 1 day
                dtTo = dtFrom.AddDays(1);
            }
            ILog log = LogsDB.Self.GetCompositeLog(userGuid, dtFrom, dtTo);
            ProductivityReport rep = new ProductivityReport(log);
            var result = rep.GetFromCacheOrCalculate();
            WriteObjectJsonAndClose(resp, result);

        }
    }
}
