using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using TimeMiner.Core;
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
            List<LogRecord> records = MasterDB.Self.GetAllRecordsForUser(0);
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
            return new HandlerPageDescriptor(pg);
        }

        
    }
}
