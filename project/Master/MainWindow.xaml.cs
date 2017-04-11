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
            System.Diagnostics.Process.Start("http://localhost:" + FrontendServer.LISTENER_PORT);
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

        struct FakeDataDesc
        {
            public string procName;
            public string windowTitle;

            public FakeDataDesc(string procName, string windowTitle)
            {
                this.procName = procName;
                this.windowTitle = windowTitle;
            }
        }
        private void btMakeFakeData_Click(object sender, RoutedEventArgs e)
        {

            /*LiteDatabase db = MasterDB.Logs.Database;

            const int NUM_USERS = 1;

            const int NUM_RECORDS_PER_USER = 60*60*5;

            const int NUM_RECORDS_PER_USER_RAND_DELTA = 60*20;

            const int DATE_SUBSTR_SEC = NUM_RECORDS_PER_USER*5; //one day back
            const int DATE_SUBSTR_RAND_DELTA = 60*60;//one hour

           FakeDataDesc[] descs = new FakeDataDesc[]
           {
               new FakeDataDesc("winword","Hello.txt - Word"),
               new FakeDataDesc("notepad","Noname - notepad"),
               new FakeDataDesc("webstom","Webstorm"),
               new FakeDataDesc("github","Github"),
               new FakeDataDesc("meridian","meridian vk player"),
               new FakeDataDesc("devenv","visual studio")      
           };

            Random r = new Random();
            for (int i = 0; i < NUM_USERS; i++)
            {
                var col = db.GetCollection<LogRecord>("log_u" + i);

                int numRec = NUM_RECORDS_PER_USER +
                             r.Next(-NUM_RECORDS_PER_USER_RAND_DELTA, NUM_RECORDS_PER_USER_RAND_DELTA);
                DateTime dt =
                    DateTime.Now.AddSeconds(-DATE_SUBSTR_SEC + r.Next(-DATE_SUBSTR_RAND_DELTA, DATE_SUBSTR_RAND_DELTA));
                List<int> favorites = new List<int>();
                for (int j = 0; j < 3; j++)
                {
                    int v = r.Next(0, descs.Length);
                    if (favorites.Contains(v))
                    {
                        j--;
                        continue;
                    }
                    favorites.Add(v);
                }
                int now = 0;
                const float CHANCE_TO_CHANGE = 0.05f;
                LogRecord last = null;
                for (int j = 0; j < numRec; j++)
                {
                    float changeProb = CHANCE_TO_CHANGE;
                    if (favorites.Contains(now))
                    {
                        changeProb /= 2;
                    }
                    if (r.NextDouble() < changeProb)
                    {
                        //we are changing program!
                        now = r.Next(0, descs.Length);
                    }
                    LogRecord newRec = new LogRecord()
                    {
                        Id = Guid.NewGuid(),
                        Keystrokes = r.Next(0, 20),
                        MouseButtonActions = r.Next(0, 5),
                        MousePosition = new IntPoint(0, 0),
                        MouseWheelActions = 0,
                        PreviusRecordId = last == null ? Guid.Empty : last.Id,
                        Process = new ProcessDescriptor()
                        {
                            ProcessName = descs[now].procName
                        },
                        Time = dt,
                        UserId = i,
                        Window = new WindowDescriptor()
                        {
                            Location = new IntPoint(0, 0),
                            Size = new IntPoint(0, 0),
                            Title = descs[now].windowTitle
                        }
                    };
                    //put record
                    col.Insert(newRec);
                    last = newRec;
                    //increment time
                    dt.AddSeconds(1);
                }
                col.EnsureIndex(x => x.Id);
            }*/
        }

        private void btClearData_Click(object sender, RoutedEventArgs e)
        {
            /*Profile prof = SettingsContainer.Self.GetBaseProfile();
            IndexedProfile iprof = IndexedProfile.FromProfile(prof);
            Log log = new Log(MasterDB.Logs.GetAllRecordsForUser(0).ToArray(), iprof);
            var res = log.GetRelevanceTimes();
            string str = res.Aggregate("", (q, t) => q += $"{t.Key}:{t.Value}\r\n");
            MessageBox.Show(str);*/
//            Log log = Log.GetLog();
//            Stopwatch w = Stopwatch.StartNew();
//
//            /*ProgramUsageReport report = new ProgramUsageReport(log);
//            report.Calculate();*/
//            ActiveReport report = new ActiveReport(log);
//            int total = report.GetActivities().Select(t => t.IsActive ? 1 : 0).Sum();
//            w.Stop();
//            MessageBox.Show($"Total active seconds = {total}");
//            MessageBox.Show($"{w.ElapsedMilliseconds}ms");
            
            //string res = JsonConvert.SerializeObject(report.GetItems(), Formatting.Indented);
            //WriteStringAndClose(resp, res);
            //Console.Out.WriteLine($"Elapsed {w.ElapsedMilliseconds} ms");
            /*LiteDatabase db = MasterDB.Logs.Database;
            foreach (var collectionName in db.GetCollectionNames())
            {
                db.DropCollection(collectionName);
            }*/
        }
    }
}
