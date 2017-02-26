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
using TimeMiner.Core;
using TimeMiner.Master;
using Excel = Microsoft.Office.Interop.Excel;

namespace MasterDatabaseExplorer
{
    public partial class Form1 : Form
    {
        LogsDB db;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            LogsDB.LOG_DB_PATH = @"C:\dev\TimeMiner\project\Master\bin\Debug\logstorage.db";
            try
            {
                db = MasterDB.Logs;
                lbStatus.Text = "LogDatabase successfully loaded";
            }
            catch (Exception ex)
            {
                lbStatus.Text = "Cannot find database";
            }
        }

        private void btExcelExport_Click(object sender, EventArgs e)
        {
            int userId = (int) numExcelExportUserId.Value;
            var records = db.GetAllRecordsForUser(userId);

            Excel.Application app = new Excel.Application();
            try
            {
                app.DisplayAlerts = false;
                var book = app.Workbooks.Add(Type.Missing);

                Excel.Worksheet sheet = book.ActiveSheet;
                PutValuesToRow(sheet, 1, ParsTitles().Cast<object>().ToArray());
                //PutValuesToRow(sheet,1,"Time","Process","MousePos","KeyStrokes");
                for (int i = 0; i < records.Count; i++)
                {
                    var rec = records[i];
                    PutValuesToRow(sheet, i + 2, ParsValues(rec).ToArray());
                }

                app.Columns.AutoFit();
                app.Visible = true;
            }
            catch (Exception ex)
            {
                app.Quit();//close app and only then fall
                throw;
            }
            //book.SaveAs(@"C:\Users\alex\Documents\Visual Studio 2015\Projects\ExcelTest\ExcelTest\bin\Debug\out.xlsx");
            //book.Close();

            //app.Quit();

            //List<string> lines = new List<string>();
            //lines.Add("Time;Process;MousePos;Keystrokes;");
            //lines.AddRange(records.OrderBy(t => t.Time).Select(t => RecordToCsvString(t)));
            //File.WriteAllLines("out.csv", lines);s
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
            Type t = typeof (LogRecord);
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
                sheet.Cells[row, i + 1] = values[i]?.ToString()??"null";
            }
        }
        
        private string RecordToCsvString(LogRecord rec)
        {
            return $"{rec.Time};{rec.Process.ProcessName};{rec.MousePosition.ToString()};{rec.Keystrokes};";
        }

        private void btClearDb_Click(object sender, EventArgs e)
        {
            foreach (var collectionName in db.Database.GetCollectionNames().ToList())
            {
                db.Database.DropCollection(collectionName);
            }
        }

        private void btImportOldLog_Click(object sender, EventArgs e)
        {
            OpenFileDialog d = new OpenFileDialog();
            var res = d.ShowDialog();
            if (res == DialogResult.OK)
            {
                ImportOldLog(d.FileName);
            }
        }

        private void ImportOldLog(string fname)
        {
            string[] lines = File.ReadAllLines(fname);
            int failedCount = 0;
            LogRecord prev = null;
            foreach (var line in lines)
            {
                try
                {
                    OldLogItem ite = OldLogItem.FromCSVRow(line);
                    LogRecord rec = ToNewLogRecord(ite);
                    if (prev != null)
                        rec.PreviusRecordId = prev.Id;
                    db.PutRecord(rec);
                    prev = rec;
                }
                catch (Exception)
                {
                    failedCount++;
                }
            }
            if (failedCount > 0)
            {
                MessageBox.Show($"{failedCount} items failed");
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
            DateTime time = ConvertToDatetime(ite.time);
            LogRecord rec = new LogRecord()
            { 
                Id = Guid.NewGuid(),
                Keystrokes = ite.keypressCount,
                MouseButtonActions = ite.mouseActionsCount, //now we suppose all mouse actions are button
                MousePosition = new IntPoint(ite.cursorPos.X,ite.cursorPos.Y),
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
    }
}
