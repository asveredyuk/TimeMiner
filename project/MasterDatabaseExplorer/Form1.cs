using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CorutinesWorker;
using CorutinesWorker.Corutines;
using LiteDB;
using TimeMiner.Core;
using TimeMiner.Master;
using TimeMiner.Master.Database;
using TimeMiner.Master.Settings;
using TimeMiner.Master.Settings.ApplicationIdentifiers;
using Excel = Microsoft.Office.Interop.Excel;

namespace MasterDatabaseExplorer
{
    public partial class Form1 : Form
    {
        LogsDB db;
        SettingsContainer settings;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            LogsDB.LOGS_DIR = @"C:\dev\TimeMiner\project\Master\bin\Debug\Logs";
            //LogsDB.LOG_DB_PATH = @"C:\dev\TimeMiner\project\Master\bin\Debug\logstorage.db";
            SettingsDB.DB_PATH = @"C:\dev\TimeMiner\project\Master\bin\Debug\settings.db";
            try
            {
                db = MasterDB.Logs;
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
            int userId = (int)numExcelExportUserId.Value;
            Corutine corut = new Corutine(this, ExportToExcelCorut(userId));
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

        private IEnumerable<CorutineReport> ExportToExcelCorut(int userId)
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
                if(i > 1000)
                    break;
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
                sheet.Cells[row, i + 1] = values[i]?.ToString() ?? "null";
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
            int userId = (int) numExcelExportUserId.Value;
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

        private IEnumerable<CorutineReport> ImportManyLogs(string[] fnames, int userId)
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
        private IEnumerable<CorutineReport> ImportOldLog(string fname, int userId)
        {
            const int REPORT_EACH = 100;
            string[] lines = File.ReadAllLines(fname, Encoding.Default);
            int failedCount = 0;
            LogRecord prev = null;
            for (int i = 0; i < lines.Length; i++)
            {
                var line = lines[i];
                try
                {
                    OldLogItem ite = OldLogItem.FromCSVRow(line);
                    LogRecord rec = ToNewLogRecord(ite);
                    rec.UserId = userId;
                    if (prev != null)
                        rec.PreviusRecordId = prev.Id;
                    db.PutRecord(rec);
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
            yield return new CorutineReportPercentage(100);
            yield return new CorutineReportResult($"Failed : {failedCount}");
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
            DateTime time = ConvertToDatetime(ite.time);
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
            return new DateTime(1970, 1, 1).AddSeconds(time);
        }

        private void btImportAppsList_Click(object sender, EventArgs e)
        {
            OpenFileDialog d = new OpenFileDialog();
            var res = d.ShowDialog();
            if (res == DialogResult.OK)
            {
                Corutine corut = new Corutine(this,ImportApps(d.FileName));
                SimpleProgressForm form = new SimpleProgressForm(corut);
                form.Start();
            }
        }

        private IEnumerable<CorutineReport> ImportApps(string fname)
        {
            string[] lines = File.ReadAllLines(fname);
            int failedCount = 0;
            for (int i = 0; i < lines.Length; i++)
            {
                string line = lines[i];
                try
                {
                    string[] split = line.Split(';');
                    string name = split[0];
                    int type = int.Parse(split[1]);
                    ApplicationDescriptor desc = new ApplicationDescriptor(name,new ProcessNameIdetifier(name));
                    
                    ProfileApplicationRelevance rel = new ProfileApplicationRelevance(IntToRel(type),desc);
                    settings.PutNewApp(rel);
                }
                catch (Exception)
                {
                    failedCount++;
                }
                yield return new CorutineReportPercentage(i+1,lines.Length);
            }
            yield return new CorutineReportPercentage(100);
            yield return new CorutineReportResult($"{lines.Length} apps added");
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
            LiteDatabase db = MasterDB.Settings.Database;
            foreach (var collectionName in db.GetCollectionNames().ToList())
            {
                db.DropCollection(collectionName);
            }
        }

        private void btLogStat_Click(object sender, EventArgs e)
        {
            List<LogRecord> list = db.GetAllRecordsForUser(0);
            

            //int total = db.Database.GetCollection<LogRecord>("log_u0").Count();
            //MessageBox.Show($"{total} total records");
            MessageBox.Show($"{list.Count} total records");
        }
    }
}
