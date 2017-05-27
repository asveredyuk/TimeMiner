using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CorutinesWorker;
using CorutinesWorker.Corutines;
using LiteDB;
using Newtonsoft.Json;
using TimeMiner.Core;
using TimeMiner.Master;
using TimeMiner.Master.Analysis;
using TimeMiner.Master.Analysis.reports;
using TimeMiner.Master.Database;
using TimeMiner.Master.Settings;
using TimeMiner.Master.Settings.ApplicationIdentifiers;
using Excel = Microsoft.Office.Interop.Excel;

namespace MasterDatabaseExplorer
{
    public partial class Form1 : Form
    {
        private int LogImportTimeShift;
        LogsDB db;
        SettingsContainer settings;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            userIdTb.Text = Guid.Empty.ToString();
            LogsDB.LOGS_DIR = @"C:\dev\TimeMiner\project\Master\bin\Debug\Logs";
            //LogsDB.LOG_DB_PATH = @"C:\dev\TimeMiner\project\Master\bin\Debug\logstorage.db";
            SettingsDB.DB_PATH = @"C:\dev\TimeMiner\project\Master\bin\Debug\settings.db";
            CacheDB.DB_PATH = @"C:\dev\TimeMiner\project\Master\bin\Debug\cache.db";
            try
            {
                db = LogsDB.Self;
                settings = SettingsContainer.Self;
                lbStatus.Text = "LogDatabase successfully loaded";
            }
            catch (Exception ex)
            {
                lbStatus.Text = "Cannot find database";
            }
        }

        private void btExcelExport_Click(object sender, EventArgs e)
        {
            Corutine corut = new Corutine(this, ExportToExcelCorut(Guid.Empty));
            SimpleProgressForm form = new SimpleProgressForm(corut)
            {
                showCompletedDialog = false
            };
            form.Start();
            //book.SaveAs(@"C:\Users\alex\Documents\Visual Studio 2015\Projects\ExcelTest\ExcelTest\bin\Debug\out.xlsx");
            //book.Close();

            //app.Quit();

            //List<string> lines = new List<string>();
            //lines.Add("Time;Process;MousePos;Keystrokes;");
            //lines.AddRange(records.OrderBy(t => t.Time).Select(t => RecordToCsvString(t)));
            //File.WriteAllLines("out.csv", lines);s
        }

        private IEnumerable<CorutineReport> ExportToExcelCorut(Guid userId)
        {
            const int REPORT_EACH = 100;

            var records = db.GetAllRecordsForUser(userId);

            Excel.Application app = new Excel.Application();
            //            try
            //            {
            app.DisplayAlerts = false;
            var book = app.Workbooks.Add(Type.Missing);

            Excel.Worksheet sheet = book.ActiveSheet;
            PutValuesToRow(sheet, 1, ParsTitles().Cast<object>().ToArray());
            //PutValuesToRow(sheet,1,"Time","Process","MousePos","KeyStrokes");
            for (int i = 0; i < records.Count; i++)
            {
//                if(i > 1000)
//                    break;
                var rec = records[i];
                if (i % REPORT_EACH == 0)
                    yield return new CorutineReportPercentage(i + 1, records.Count);
                PutValuesToRow(sheet, i + 2, ParsValues(rec).ToArray());
            }

            app.Columns.AutoFit();
            app.Visible = true;
            //            }
            //            catch (Exception ex)
            //            {
            //                app.Quit();//close app and only then fall
            //                throw;
            //            }
            yield return new CorutineReportPercentage(100);
            yield return new CorutineReportResult(0);
        }

        private IEnumerable<object> ParsValues(LogRecord rec)
        {
            Type t = typeof(LogRecord);
            foreach (var propertyInfo in t.GetProperties())
            {
                if (propertyInfo.PropertyType.Namespace == "TimeMiner.Core")
                {
                    var val = propertyInfo.GetValue(rec);
                    foreach (var pr2 in propertyInfo.PropertyType.GetProperties())
                    {
                        //yield return propertyInfo.Name + "." + pr2.Name;
                        yield return pr2.GetValue(val);
                    }
                }
                else
                {
                    yield return propertyInfo.GetValue(rec);
                    //yield return propertyInfo.Name;
                }
            }
        }
        private IEnumerable<string> ParsTitles()
        {
            Type t = typeof(LogRecord);
            foreach (var propertyInfo in t.GetProperties())
            {
                if (propertyInfo.PropertyType.Namespace == "TimeMiner.Core")
                {
                    foreach (var pr2 in propertyInfo.PropertyType.GetProperties())
                    {
                        yield return propertyInfo.Name + "." + pr2.Name;
                    }
                }
                else
                {
                    yield return propertyInfo.Name;
                }
            }
        }
        private void PutValuesToRow(Excel.Worksheet sheet, int row, params object[] values)
        {
            for (int i = 0; i < values.Length; i++)
            {
                string val = values[i]?.ToString() ?? "null";
                if (values[i] != null && values[i] is Dictionary<string,byte[]>)
                {
                    val = "";
                    foreach (var pair in values[i] as Dictionary<string,byte[]>)
                    {
                        val += $"[{pair.Key}:{Encoding.UTF8.GetString(pair.Value)}]";
                    }
                }
                sheet.Cells[row, i + 1] = val;
            }
        }

        private string RecordToCsvString(LogRecord rec)
        {
            return $"{rec.Time};{rec.Process.ProcessName};{rec.MousePosition.ToString()};{rec.Keystrokes};";
        }

        private void btClearDb_Click(object sender, EventArgs e)
        {
            //TEMP SOLUTION
            foreach (var file in Directory.GetFiles(LogsDB.LOGS_DIR))
            {
                File.Delete(file);
            }
        }

        private void btImportOldLog_Click(object sender, EventArgs e)
        {
            //int userId = (int) numExcelExportUserId.Value;
            Guid userId;
            if (!Guid.TryParse(userIdTb.Text, out userId))
            {
                MessageBox.Show("Wrong user id (GUID)");
                return;
            }
            LogImportTimeShift = (int) numberTimeShift.Value*60*60;
            OpenFileDialog d = new OpenFileDialog();
            d.Multiselect = true;
            var res = d.ShowDialog();
            if (res == DialogResult.OK)
            {
                if (d.FileNames.Length > 1)
                {
                    var corut = new Corutine(this, ImportManyLogs(d.FileNames, userId));
                    SimpleProgressForm f = new SimpleProgressForm(corut);
                    f.labelBindRole = SimpleProgressForm.TextBindRole.Text;
                    f.Start();
                }
                else
                {
                    var corut = new Corutine(this, ImportOldLog(d.FileName, userId));
                    SimpleProgressForm f = new SimpleProgressForm(corut);
                    f.Start();
                }
                
                //ImportOldLog(d.FileName);
            }
        }

        private IEnumerable<CorutineReport> ImportManyLogs(string[] fnames, Guid userId)
        {
            for (int i = 0; i < fnames.Length; i++)
            {
                yield return new CorutineReportText($"Importing {i+1} file of {fnames.Length}");
                foreach (var report in ImportOldLog(fnames[i],userId))
                {
                    if (report is CorutineReportResult)
                    {
                        //MessageBox.Show((report as CorutineReportResult).result.ToString());
                    }
                    else
                    {
                        yield return report;
                    }
                }
            }
            yield return new CorutineReportResult(0);
        }
        private IEnumerable<CorutineReport> ImportOldLog(string fname, Guid userId)
        {
            const int REPORT_EACH = 100;
            string[] lines = File.ReadAllLines(fname, Encoding.Default);
            List<LogRecord> parsed = new List<LogRecord>();
            int failedCount = 0;
            LogRecord prev = null;
            for (int i = 0; i < lines.Length; i++)
            {
                var line = lines[i];
                try
                {
                    OldLogItem ite = OldLogItem.FromCSVRow(line);
                    LogRecord rec = ToNewLogRecord(ite);
                    if(rec.Process.ProcessName == "factorio")
                        continue;
                    rec.UserId = userId;
                    if (ite.extraInfo != null && ite.extraInfo.Length > 2)
                    {
                        rec.PutMetaString("url",ite.extraInfo);
                    }
                    MakeFakeTask(rec);
                    if (prev != null)
                        rec.PreviusRecordId = prev.Id;
                    parsed.Add(rec);
                    prev = rec;
                }
                catch (Exception)
                {
                    failedCount++;
                }
                if (i % REPORT_EACH == 0)
                {
                    yield return new CorutineReportPercentage(i + 1, lines.Length);
                }
            }
            db.PutManyRecords(parsed);
            yield return new CorutineReportPercentage(100);
            yield return new CorutineReportResult($"Failed : {failedCount}");
        }

        void MakeFakeTask(LogRecord rec)
        {
            var hour = rec.Time.Hour;
            string meta = null;
            switch (hour)
            {
                case 8:
                case 9:
                case 10:
                case 11:
                case 12:
                    meta = "Writing code";
                    break;
                case 15:
                case 16:
                case 17:
                case 18:
                    meta = "Debugging";
                    break;
                case 20:
                case 21:
                case 22:
                case 23:
                    meta = "Release";
                    break;
            }
            if (meta != null)
            {
                rec.PutMetaString("task", meta);
            }
        }
        private LogRecord ToNewLogRecord(OldLogItem ite)
        {
            ProcessDescriptor pdesc = new ProcessDescriptor()
            {
                ProcessName = ite.activeWindowProcessName
            };
            WindowDescriptor wdesc = new WindowDescriptor()
            {
                Title = ite.activeWindowTitle
            };
            DateTime time = ConvertToDatetime(ite.time+LogImportTimeShift);

            LogRecord rec = new LogRecord()
            {
                Id = Guid.NewGuid(),
                Keystrokes = ite.keypressCount,
                MouseButtonActions = ite.mouseActionsCount, //now we suppose all mouse actions are button
                MousePosition = new IntPoint(ite.cursorPos.X, ite.cursorPos.Y),
                Process = pdesc,
                Window = wdesc,
                Time = time
                //user id now is 0
            };
            return rec;
        }
        public static DateTime ConvertToDatetime(int time)
        {
            System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Local);
            dtDateTime = dtDateTime.AddSeconds(time);
            return dtDateTime;
        }

        private void btImportAppsList_Click(object sender, EventArgs e)
        {
            OpenFileDialog d = new OpenFileDialog();
            var res = d.ShowDialog();
            if (res == DialogResult.OK)
            {
                Corutine corut = new Corutine(this, ImportApps(d.FileName));
                SimpleProgressForm form = new SimpleProgressForm(corut);
                form.Start();
            }
        }

        private IEnumerable<CorutineReport> ImportApps(string fname)
        {
            string[] lines = File.ReadAllLines(fname);
            int failedCount = 0;
            bool sites = false;
            int addedCount = 0;
            for (int i = 0; i < lines.Length; i++)
            {
                string line = lines[i];
                if (line.StartsWith("--"))
                {
                    if (line.EndsWith("processes"))
                        sites = false;
                    if (line.EndsWith("sites"))
                        sites = true;
                    continue;
                }
                try
                {
                    string[] split = line.Split(':');
                    string name = split[0];
                    int type = int.Parse(split[1]);
                    ApplicationIdentifierBase identifier = null;
                    if (sites)
                    {
                        identifier = new WebsiteIdentifier(name);
                    }
                    else
                    {
                        identifier = new ProcessNameIdentifier(name);
                    }
                    ApplicationDescriptor desc = new ApplicationDescriptor(name,identifier);
                    
                    ProfileApplicationRelevance rel = new ProfileApplicationRelevance(IntToRel(type),desc);
                    settings.PutNewApp(rel);
                    addedCount++;
                }
                catch (Exception)
                {
                    failedCount++;
                }
                yield return new CorutineReportPercentage(i+1,lines.Length);
            }
            yield return new CorutineReportPercentage(100);
            yield return new CorutineReportResult($"{addedCount} apps added");
        }

        private Relevance IntToRel(int val)
        {
            switch (val)
            {
                case 0:
                    return Relevance.bad;
                case 1:
                    return Relevance.neutral;
                case 2:
                    return Relevance.good;
                default:
                    throw new ArgumentException("val is not appropriate for relevance");
            }
        }

        private int RelToInt(Relevance rel)
        {
            switch (rel)
            {
                case Relevance.good:
                    return 2;
                case Relevance.neutral:
                    return 1;
                case Relevance.bad:
                    return 0;
                default:
                    throw new ArgumentOutOfRangeException(nameof(rel), rel, null);
            }
        }

        private void btClearApps_Click(object sender, EventArgs e)
        {
            LiteDatabase db = SettingsDB.Self.Database;
            foreach (var collectionName in db.GetCollectionNames().ToList())
            {
                db.DropCollection(collectionName);
            }
        }

        private void btLogStat_Click(object sender, EventArgs e)
        {
            List<LogRecord> list = db.GetAllRecordsForUser(Guid.Empty,false);
            if (list.Count == 0)
            {
                MessageBox.Show("Empty");
                return;
            }
            string stat = $"{list.Count} total records\r\n";
            int sitecount = list.Where(t => t.GetMetaString("url") != null).Count();
            int percent = sitecount * 100 / list.Count;
            stat += $"{percent}% sites";

            //int total = db.Database.GetCollection<LogRecord>("log_u0").Count();
            //MessageBox.Show($"{total} total records");
            MessageBox.Show(stat);
        }

        private void btTest_Click(object sender, EventArgs e)
        {
            //clear database
            /*btClearDb_Click(null,null);

            DateTime dt = new DateTime(2017,5,8,0,0,0,0, DateTimeKind.Local);

            DateTime till = dt.AddDays(2).AddSeconds(-10);
            int minutesInterval = 60*4;
            while (dt < till)
            {
                LogRecord rec = makeRecord(dt);
                LogsDB.Self.PutRecord(rec);
                dt = dt.AddMinutes(minutesInterval);
            }*/
            /*LogRecord rec = makeRecord(new DateTime(2017, 5, 8, 9, 20, 0, DateTimeKind.Local));
            LogsDB.Self.PutRecord(rec);
            //ILog[] logs = LogsDB.Self.GetLogsForUserForPeriodPerStorage(Guid.Empty, DateTime.MinValue, DateTime.MaxValue);
            DateTime timefrom = new DateTime(2017,5,8,9,0,0,DateTimeKind.Local);
            DateTime timeto = new DateTime(2017, 8, 8, 9, 0, 1, DateTimeKind.Local);

            ILog composite = LogsDB.Self.GetCompositeLog(Guid.Empty, timefrom, timeto);
            ExportLogsToExcel(composite);
            MessageBox.Show(composite.DataHash);*/
            //ExportLogsToExcel(logs);

            //MessageBox.Show(dt.ToUniversalTime().ToString());
            /*ApplicationIdentifierBase id = new WebsiteIdentifier() {Host = "vk.com"};
            ApplicationDescriptor desc = new ApplicationDescriptor("Вконтакте", id);
            ProfileApplicationRelevance rel = new ProfileApplicationRelevance(Relevance.bad,desc);
            settings.PutNewApp(rel);*/
            /*List<LogRecord> list = db.GetAllRecordsForUser(0);
            string[] allSites =
                list.Where(t => t.GetMetaString("site") != null).Select(t => t.GetMetaString("site")).ToArray();
            string[] sites =
                allSites.Distinct().ToArray();
            int count = allSites.Count(t => (GetHost(t) == "stackoverflow.com"));
            MessageBox.Show(count.ToString());*/
            //sites = sites.Where(t => (GetHost(t) == null)).ToArray();
            //File.WriteAllLines("sites.txt",sites);
            DateTime begin = new DateTime(2017,5,16,0,0,0, DateTimeKind.Local);
            DateTime end = new DateTime(2017, 5, 16, 23, 59, 59, DateTimeKind.Local);
            Guid userid = Guid.Parse("e86daa6e-decd-47e1-9d01-cd8c99586b8d");
            ILog log = LogsDB.Self.GetCompositeLog(userid, begin, end);
            TimeBoundsReport report = new TimeBoundsReport(log);
            report.Parameters.PeriodMergeIntervalSecs = 60;
            report.Parameters.PeriodIgnoreMinSecs = 60;
            var res = report.Calculate();
            TimeBoundsReport.ReportItem[] items = res.Items;
            Bitmap drawn = DrawBitmap(items);
            drawn.Save("test.png");
            Process.Start("test.png");
            foreach (var item in res.Items)
            {
                item.Begin = item.Begin.ToLocalTime();
                item.End = item.End.ToLocalTime();
            }
            string data = JsonConvert.SerializeObject(res, Formatting.Indented);
            File.WriteAllText("test.txt", data);
            Process.Start("test.txt");
        }

        private Bitmap DrawBitmap(TimeBoundsReport.ReportItem[] items)
        {
            const int W = 2000;
            const int H = 100;
            Bitmap bmp = new Bitmap(W, H);
            Graphics g = Graphics.FromImage(bmp);
            g.Clear(Color.White);
            DateTime begin = items[0].Begin;
            DateTime end = items[items.Length - 1].End;
            double totalSeconds = (end - begin).TotalSeconds;
            double ratio = W / totalSeconds;
            foreach (var reportItem in items)
            {
                //get pos of begin
                double pos = (reportItem.Begin - begin).TotalSeconds * ratio;
                double len = (reportItem.End - reportItem.Begin).TotalSeconds * ratio;
                g.FillRectangle(Brushes.Red, (float)pos,0, (float)len,H);
            }
            g.Flush();
            return bmp;
        }

        private void ExportLogsToExcel(params ILog[] logs)
        {
            Excel.Application app = new Excel.Application();
            app.DisplayAlerts = false;
            var book = app.Workbooks.Add(Type.Missing);
            Excel.Worksheet sheet = book.ActiveSheet;


            for (int i = 0; i < logs.Length; i++)
            {
                ILog log = logs[i];
                LogRecord[] records = log.Records;
                DateTime[] datetimes = records.Select(t => t.Time).ToArray();

                int pos = 1;
                foreach (var time in datetimes)
                {
                    sheet.Cells[pos, i+1] = time.ToString();
                    pos++;
                }
            }
            app.Columns.AutoFit();
            app.Visible = true;
        }

        private LogRecord makeRecord(DateTime time)
        {
            ProcessDescriptor pdesc = new ProcessDescriptor()
            {
                ProcessName = "test"
            };
            WindowDescriptor wdesc = new WindowDescriptor()
            {
                Title = "test"
            };

            LogRecord rec = new LogRecord()
            {
                Id = Guid.NewGuid(),
                Process = pdesc,
                Window = wdesc,
                Time = time
                //user id now is 0
            };
            return rec;
        }
        private string GetHost(string site)
        {
            
            //if (Uri.IsWellFormedUriString(site, UriKind.Absolute))
            //{
            /*    Uri uri = new Uri(site);
                return uri.Host;*/
            Uri uri;
            if (!Uri.TryCreate(site, UriKind.Absolute, out uri))
                return null;
            return uri.Host;
            //}
            //return null;
        }

        private void btCountPerDay_Click(object sender, EventArgs e)
        {
            var dict = CountLogsPerDay();

            Excel.Application app = new Excel.Application();
            app.DisplayAlerts = false;
            var book = app.Workbooks.Add(Type.Missing);

            Excel.Worksheet sheet = book.ActiveSheet;
            int pos = 1;
            foreach (var pair in dict.OrderBy(t=>t.Key))
            {
                sheet.Cells[pos, 1] = pair.Key;
                sheet.Cells[pos, 2] = pair.Value;
                pos++;
            }
            int count = dict.Sum(t => t.Value);
            sheet.Cells[pos, 1] = "Total";
            sheet.Cells[pos, 2] = count;
            app.Columns.AutoFit();
            app.Visible = true;
        }
        private Dictionary<DateTime, int> CountLogsPerDay()
        {
            var logs = db.GetAllRecordsForUser(Guid.Empty, false);
            Dictionary<DateTime,int> dict = new Dictionary<DateTime, int>();
            foreach (var logRecord in logs)
            {
                var date = logRecord.Time.Date;
                if (!dict.ContainsKey(date))
                    dict[date] = 0;
                dict[date]++;
            }
            return dict;
        }
    }
}
