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
            List<string> lines = new List<string>();
            lines.Add("Time;Process;MousePos;Keystrokes;");
            lines.AddRange(records.OrderBy(t => t.Time).Select(t => RecordToCsvString(t)));
            File.WriteAllLines("out.csv", lines);
        }
        private string RecordToCsvString(LogRecord rec)
        {
            return $"{rec.Time};{rec.Process.ProcessName};{rec.MousePosition.ToString()};{rec.Keystrokes};";
        }

    }
}
