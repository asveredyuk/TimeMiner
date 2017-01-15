using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
using TimeMiner.Core;

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

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            MainController.Self.OnStartup();
        }
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            MainController.Self.OnExit();
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            MasterDB db = MasterDB.Self;
            var records = db.GetAllRecordsForUser(0);
            List<string> lines = new List<string>();
            lines.Add("Time;Process;MousePos;Keystrokes;");
            lines.AddRange(records.OrderBy(t=>t.Time).Select(t=>RecordToCsvString(t)));
            File.WriteAllLines("out.csv",lines);
        }

        private string RecordToCsvString(LogRecord rec)
        {
            return $"{rec.Time};{rec.Process.ProcessName};{rec.MousePosition.ToString()};{rec.Keystrokes};";
        }
    }
}
