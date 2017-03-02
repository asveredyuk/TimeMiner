using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using TimeMiner.Core;
using TimeMiner.Master.Analysis;
using TimeMiner.Master.Frontend.Plugins;

namespace TimeMiner.Master.Frontend.BuiltInExtensions
{
    class ProgramStatisticsExtension : FrontendServerExtensionBase
    {
        public ProgramStatisticsExtension()
        {
            MenuItems.Add(new TemplatePageMenuItem("Statistics","/stat"));
        }
        [HandlerPath("stat")]
        public HandlerPageDescriptor Handle(HttpListenerRequest req, HttpListenerResponse resp)
        {
            /*List<LogRecord> records = MasterDB.Logs.GetAllRecordsForUser(0);
            Dictionary<string,int> programs = new Dictionary<string, int>();
            foreach (var rec in records)
            {
                if (!programs.ContainsKey(rec.Process.ProcessName))
                {
                    programs[rec.Process.ProcessName] = 0;
                }
                programs[rec.Process.ProcessName]++;
            }
            string pg = "";
            foreach (var p in programs.OrderByDescending(p=>p.Value))
            {
                pg += p.Key + ":" + p.Value + "s<br>";
            }
            return new HandlerPageDescriptor(pg);*/
            var res = new HandlerPageDescriptor(WWWRes.GetString("stat/appusage/tablepage.html"), WWWRes.GetString("stat/appusage/tablehead.html"));
            return res;
        }

        [ApiPath("stat")]
        public void HandleApi(HttpListenerRequest req, HttpListenerResponse resp)
        {
            string path = SkipApiAndRoot(req.Url.AbsolutePath);
            /*switch (path)
            {
                    case ""
            }*/
            Log log = Log.GetLog();
            ProgramUsageReport report = new ProgramUsageReport(log);
            string res = JsonConvert.SerializeObject(report.GetReport(), Formatting.Indented);
            WriteStringAndClose(resp,res);

        }

        
    }
}
