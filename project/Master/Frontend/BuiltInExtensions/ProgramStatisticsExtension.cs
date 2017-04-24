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
            //MenuItems.Add(new FrontendPageMenuItem("Statistics","/stat"));
        }
        [MenuItem("Statistics","stat")]
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

        [ApiPath("stat/overall_productivity")]
        public void HandlOverallProdApi(HttpListenerRequest req, HttpListenerResponse resp)
        {
            StatRequestData reqData = ParseStatRequestDataAndLocalize(req);
            Log[] logs = LogsDB.Self.GetLogsForUserForPeriodSeparate(Guid.Empty, reqData.Begin, reqData.End);
            Log log;
            if(logs.Length > 1)
                throw new NotImplementedException("More than one log is not supported");
            if (logs.Length < 1)
            {
                //make fake empty log
                log = new Log(new List<LogRecord>(), null, null);
            }
            else
            {
                log = logs[0];
            }
            ProductivityReport rep = new ProductivityReport(log);
            var result = rep.GetFromCacheOrCalculate();
            WriteObjectJsonAndClose(resp,result);
        }

        [ApiPath("stat/overall_prod_per_day")]
        public void HandleOverallProdPerDayApi(HttpListenerRequest req, HttpListenerResponse resp)
        {
            StatRequestData reqData = ParseStatRequestDataAndLocalize(req);
            Log[] logs = LogsDB.Self.GetLogsForUserForPeriodSeparate(Guid.Empty, reqData.Begin, reqData.End);
            List<object> results = new List<object>();
            //List<ProductivityReport.ReportResult> reportResults = new List<ProductivityReport.ReportResult>();
            foreach (var log in logs)
            {
                ProductivityReport rep = new ProductivityReport(log);
                var res = rep.GetFromCacheOrCalculate();
                var wrap = new
                {
                    Date=log.Date,
                    ProductiveTime = res.ProductiveTime,
                    DistractionsTime = res.DistractionsTime,
                    TotalTime = res.TotalTime
                };
                results.Add(wrap);
            }
            

            WriteObjectJsonAndClose(resp,results);
        }
      
        private StatRequestData ParseStatRequestDataAndLocalize(HttpListenerRequest req)
        {
            string postString = ReadPostString(req);
            StatRequestData reqData = JsonConvert.DeserializeObject<StatRequestData>(postString);            
            reqData.Localize();
            return reqData;
        }
        [ApiPath("stat/appusage")]
        public void HandleApi(HttpListenerRequest req, HttpListenerResponse resp)
        {
            string path = SkipApiAndRoot(req.Url.AbsolutePath);
            StatRequestData reqData = ParseStatRequestDataAndLocalize(req);
            Stopwatch w2 = Stopwatch.StartNew();
            //temporarly solved by local time
            Log log = LogsDB.Self.GetLogRecordsForUserForPeriod(Guid.Empty, reqData.Begin,reqData.End);
            Console.WriteLine("Number of records:" + log.Records.Length);
            w2.Stop();
            Console.Out.WriteLine($"Log loading elapsed {w2.ElapsedMilliseconds}ms");
            Stopwatch w = Stopwatch.StartNew();
            ActiveReport active = new ActiveReport(log);
            //bool[] actives = active.GetActivities().Select(t => t.Value).ToArray();
            ProgramUsageReport report = new ProgramUsageReport(log);
            report.Parameters.ActiveReport = active;
            //report.Calculate();
            //report.CalculateWithActives(actives);
            

            string res = JsonConvert.SerializeObject(report.Calculate().Items, Formatting.Indented);
            w.Stop();
            WriteStringAndClose(resp,res);
            Console.Out.WriteLine($"Analyze elapsed {w.ElapsedMilliseconds}ms"); 
            
        }

        private class StatRequestData
        {
            public DateTime Begin { get; set; }
            public DateTime End { get; set; }

            public void Localize()
            {
                Begin = Begin.ToLocalTime();
                End = End.ToLocalTime();
            }
        }
        
    }
}
