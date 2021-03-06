﻿using System;
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
using System.Windows.Shapes;
using TimeMiner.Core;
using TimeMiner.Slave.Hooks;

namespace TimeMiner.Slave
{
    /// <summary>
    /// Interaction logic for TestWindow.xaml
    /// </summary>
    public partial class TestWindow : Window
    {
        IWinHook hk;
        public TestWindow()
        {
            InitializeComponent();
            //hk = new KeyboardHook();
            //hk.Hook();
        }

        private void btTest_Click(object sender, RoutedEventArgs e)
        {
            //move this to controller
            /*Logger.Self.onLogRecord += delegate(LogRecord record)
            {
                SlaveDB.Self.AddLogRecord(record);                
            };
            SlaveDB.Self.onLogRecordAdded += delegate(LogRecord item, SlaveDB db)
            {
                
            };
           // MessageBox.Show(SlaveDB.Self.GetAllLogs().Count.ToString());
            string[] arr = SlaveDB.Self.GetAllLogs().Select(t => t.ToString()).ToArray();
            File.WriteAllLines("out.log",arr);


            //start logging
            Logger.Self.StartLogging();*/
            string[] arr = SlaveDB.Self.GetAllLogs().Select(t => t.ToString()).ToArray();
            File.WriteAllLines("out.log", arr);
            btTest.Content = arr.Length.ToString();

            //MessageBox.Show(hk.ActionsCount.ToString());
            //hk.Reset();
            //System.Threading.Thread.Sleep(5000);
            //MessageBox.Show(WindowsBoundary.GetForegroundWindowProcess().ProcessName);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            MainController.Self.OnStartup();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            MainController.Self.OnExit();
        }
    }
}
