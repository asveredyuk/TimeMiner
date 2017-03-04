using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
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
            Log log = Log.GetLog();
            Stopwatch w = Stopwatch.StartNew();
            ActiveReport active = new ActiveReport(log);
            bool[] actives = active.GetActivities().Select(t => t.Value).ToArray();
            ProgramUsageReport report = new ProgramUsageReport(log);
            //report.Calculate();
            report.CalculateWithActives(actives);
            w.Stop();

            string res = JsonConvert.SerializeObject(report.GetItems(), Formatting.Indented);
            WriteStringAndClose(resp,res);
            Console.Out.WriteLine($"Elapsed {w.ElapsedMilliseconds} ms"); 
            
        }

        
    }
}
