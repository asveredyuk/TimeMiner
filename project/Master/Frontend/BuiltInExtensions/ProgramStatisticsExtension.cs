using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
            var head = WWWRes.GetString("stat/appusage/tablehead.html");
            var page = WWWRes.GetString("stat/appusage/tablepage.html");
            var res = new HandlerPageDescriptor(page,head);
            return res;
        }

        [ApiPath("stat/overall_productivity")]
        public void ApiOverallProd(HttpListenerRequest req, HttpListenerResponse resp)
        {
            StatRequestData reqData = ParseStatRequestDataAndLocalize(req);
            if (reqData == null)
            {
                WriteStringAndClose(resp,"Wrong request data",400);
                return;
            }
            ILog[] logs = LogsDB.Self.GetLogsForUserForPeriodSeparate(reqData.UserId, reqData.Begin, reqData.End);
            ILog log;
            if (logs.Length > 1)
            {
                List<LogRecord> records = new List<LogRecord>();
                foreach (var log1 in logs)
                {
                    records.AddRange(log1.Records);
                }
                log = new Log(records, logs[0].Prof, null);
            }
            else if (logs.Length < 1)
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
        public void ApiOverallProdPerDay(HttpListenerRequest req, HttpListenerResponse resp)
        {
            StatRequestData reqData = ParseStatRequestDataAndLocalize(req);
            if (reqData == null)
            {
                WriteStringAndClose(resp, "Wrong request data",400);
                return;
            }
            ILog[] logs = LogsDB.Self.GetLogsForUserForPeriodSeparate(reqData.UserId, reqData.Begin, reqData.End);
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
        /// <summary>
        /// Parse statistics request data and localize time
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        private StatRequestData ParseStatRequestDataAndLocalize(HttpListenerRequest req)
        {
            try
            {
                string postString = ReadPostString(req);
                StatRequestData reqData = JsonConvert.DeserializeObject<StatRequestData>(postString);
                return reqData;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }
        [ApiPath("stat/appusage")]
        public void ApiAppsUsage(HttpListenerRequest req, HttpListenerResponse resp)
        {
            StatRequestData reqData = ParseStatRequestDataAndLocalize(req);
            if (reqData == null)
            {
                WriteStringAndClose(resp, "Wrong request data", 400);
                return;
            }
            Stopwatch w2 = Stopwatch.StartNew();
            //temporarly solved by local time
            ILog log = LogsDB.Self.GetLogRecordsForUserForPeriod(reqData.UserId, reqData.Begin,reqData.End);
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
            public Guid UserId { get; set; }
        }
        
    }
}
