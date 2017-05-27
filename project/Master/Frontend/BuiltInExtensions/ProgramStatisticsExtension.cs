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
using TimeMiner.Master.Analysis.reports;
using TimeMiner.Master.Database;
using TimeMiner.Master.Frontend.Plugins;
using TimeMiner.Master.Settings;

namespace TimeMiner.Master.Frontend.BuiltInExtensions
{
    [MenuItem("Statistics", "stat")]
    class ProgramStatisticsExtension : FrontendServerExtensionBase
    {
        public ProgramStatisticsExtension()
        {
            //MenuItems.Add(new FrontendPageMenuItem("Statistics","/stat"));
        }
        [MenuItem("Detailed statistics", "stat/stat")]
        [HandlerPath("stat/details")]
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
            ILog log = LogsDB.Self.GetCompositeLog(reqData.UserId, reqData.Begin, reqData.End);
//            ILog[] logs = LogsDB.Self.GetLogsForUserForPeriodPerStorage(reqData.UserId, reqData.Begin, reqData.End);
//            ILog log;
//            if (logs.Length > 1)
//            {
//                List<LogRecord> records = new List<LogRecord>();
//                foreach (var log1 in logs)
//                {
//                    records.AddRange(log1.Records);
//                }
//                log = new Log(records, logs[0].Prof, null);
//            }
//            else if (logs.Length < 1)
//            {
//                //make fake empty log
//                log = new Log(new List<LogRecord>(), null, null);
//            }
//            else
//            {
//                log = logs[0];
//            }
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
            if (reqData.Begin >= reqData.End)
            {
                WriteStringAndClose(resp, "Begin of interval must before end",400);
                return;
            }
            TimeSpan reqSpan;
            DateTime monthCenter = Util.GetDateTimeMiddle(reqData.Begin, reqData.End, out reqSpan);

            //TODO: check if request length is too low
            //TODO: remake this request to accept one datetime
            //ILog[] logs = LogsDB.Self.GetLogsForUserForPeriodPerStorage(reqData.UserId, reqData.Begin, reqData.End);
            //TODO: TIMEZONE IS HARDCODED!
            ILog[] logs = LogsDB.Self.GetDayByDayLogsForMonth(reqData.UserId, monthCenter, 3);
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
            ILog log = LogsDB.Self.GetLogForUserForPeriod(reqData.UserId, reqData.Begin,reqData.End);
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
        #region user table
        [MenuItem("Table of users", "stat/usertable")]
        [HandlerPath("stat/usertable")]
        public HandlerPageDescriptor Usertable(HttpListenerRequest req, HttpListenerResponse resp)
        {
            var head = WWWRes.GetString("stat/usertable/head.html");
            var page = WWWRes.GetString("stat/usertable/page.html");
            var res = new HandlerPageDescriptor(page, head);
            return res;
        }

        [ApiPath("stat/getusertable")]
        public void GetUserTable(HttpListenerRequest req, HttpListenerResponse resp)
        {
            var json = ReadPostString(req);
            //TODO: may fall here
            var period = JsonConvert.DeserializeObject<Period>(json);
            if (period.Begin.IsDefault() || period.End.IsDefault())
            {
                WriteStringAndClose(resp, "No dates",400);
                return;
            }
            //get all users
            var users = SettingsDB.Self.GetAllUsers();
            Dictionary<UserInfo, BaseReportResult> results = new Dictionary<UserInfo, BaseReportResult>();
            foreach (var userInfo in users)
            {
                var logForUser = LogsDB.Self.GetCompositeLog(userInfo.Id, period.Begin, period.End);
                var prodRep = new ProductivityReport(logForUser);
                var res = prodRep.GetFromCacheOrCalculate();
                results[userInfo] = res;
            }
            //TODO: bad code, refactor
            List<object> objects = results.Select(t => new
            {
                user = t.Key,
                report = t.Value
            })
            .Cast<object>().ToList();
            string result = JsonConvert.SerializeObject(objects, Formatting.Indented);
            WriteStringAndClose(resp,result);
        }

        [ApiPath("stat/taskstat")]
        public void HandleTaskStat(HttpListenerRequest req, HttpListenerResponse resp)
        {
            StatRequestData reqData = ParseStatRequestDataAndLocalize(req);
            if (reqData == null)
            {
                WriteStringAndClose(resp, "Wrong request data", 400);
                return;
            }
            ILog log = LogsDB.Self.GetCompositeLog(reqData.UserId, reqData.Begin, reqData.End);
            
            TasksReport treport = new TasksReport(log);
            var tres = treport.Calculate().Items;
            WriteObjectJsonAndClose(resp, tres);
        }

        [ApiPath("stat/activebounds")]
        public void HandleActiveBounds(HttpListenerRequest req, HttpListenerResponse resp)
        {
            StatRequestData reqData = ParseStatRequestDataAndLocalize(req);
            if (reqData == null)
            {
                WriteStringAndClose(resp, "Wrong request data", 400);
                return;
            }
            ILog log = LogsDB.Self.GetCompositeLog(reqData.UserId, reqData.Begin, reqData.End);
            TimeBoundsReport report = new TimeBoundsReport(log);
            var res = report.GetFromCacheOrCalculate();
            WriteObjectJsonAndClose(resp, res.Items);
        }

        #endregion
        private class StatRequestData
        {
            public DateTime Begin { get; set; }
            public DateTime End { get; set; }
            public Guid UserId { get; set; }
        }

        private class Period
        {
            public DateTime Begin { get; set; }
            public DateTime End { get; set; }
        }

        
    }
}
