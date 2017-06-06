using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using LiteDB;
using Newtonsoft.Json;
using TimeMiner.Core;
using TimeMiner.Master.Analysis;
using TimeMiner.Master.Database;
using TimeMiner.Master.Frontend;
using TimeMiner.Master.Settings;

namespace TimeMiner.Master
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        [DllImport("Kernel32.dll")]
        public static extern bool AttachConsole(int processId);

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            AttachConsole(-1);
            MainController.Self.OnStartup();
        }
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            MainController.Self.OnExit();
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("http://localhost:" + ConfigManager.Self.WebInterfacePort);
            /*MasterDB db = MasterDB.Self;
            var records = db.GetAllRecordsForUser(0);
            List<string> lines = new List<string>();
            lines.Add("Time;Process;MousePos;Keystrokes;");
            lines.AddRange(records.OrderBy(t=>t.Time).Select(t=>RecordToCsvString(t)));
            File.WriteAllLines("out.csv",lines);*/
        }

        private string RecordToCsvString(LogRecord rec)
        {
            return $"{rec.Time};{rec.Process.ProcessName};{rec.MousePosition.ToString()};{rec.Keystrokes};";
        }

       

        private void btClearData_Click(object sender, RoutedEventArgs e)
        {
            CacheDB.Self.ClearAndShrink();
        }

        private void btUnloadCollections_Click(object sender, RoutedEventArgs e)
        {
            LogsDB.Self.UnloadAllCollections();
        }
    }
}
