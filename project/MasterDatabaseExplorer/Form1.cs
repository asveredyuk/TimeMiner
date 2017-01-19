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
        MasterDB db;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            MasterDB.DB_PATH = @"C:\dev\TimeMiner\project\Master\bin\Debug\logstorage.db";
            try
            {
                db = MasterDB.Self;
                lbStatus.Text = "Database successfully loaded";
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

    }
}
