﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TimeMiner.Core;
using TimeMiner.Slave.Hooks;

namespace TimeMiner.Slave
{
    class Logger
    {
        /// <summary>
        /// Singletone self
        /// </summary>
        private static Logger self;
        /// <summary>
        /// Singletone self
        /// </summary>
        public static Logger Self
        {
            get
            {
                if(self == null)
                    self = new Logger();
                return self;
            }
        }

        /// <summary>
        /// Interval of making log records
        /// </summary>
        private const int LOG_INTERVAL = 1000;
        /// <summary>
        /// Delegate for handling onLogRecord events
        /// </summary>
        /// <param name="record"></param>
        public delegate void OnLogRecordHandler(LogRecord record);
        /// <summary>
        /// Happens when new LogRecord is captured
        /// </summary>
        public OnLogRecordHandler onLogRecord;
        /// <summary>
        /// Hook for mouse button activity
        /// </summary>
        private IWinHook mouseButtonsHook;
        /// <summary>
        /// Hook for mouse wheel activity
        /// </summary>
        private IWinHook mouseWheelHook;
        /// <summary>
        /// Hook for keyboard activity
        /// </summary>
        private IWinHook keyboardHook;
        /// <summary>
        /// Thread, that produces log records
        /// </summary>
        private Thread runnerThread;
        /// <summary>
        /// Make new logger
        /// </summary>
        private Logger()
        {
            mouseButtonsHook = new MouseButtonsHook();
            mouseWheelHook = new MouseWheelHook();
            keyboardHook = new KeyboardHook();
        }
        /// <summary>
        /// Destructor
        /// </summary>
        ~Logger()
        {
            StopLogging();
        }
        /// <summary>
        /// Begin logging
        /// </summary>
        public void StartLogging()
        {
            if (runnerThread != null)
            {
                throw new Exception("already running");
            }
            HookAll();
            runnerThread = new Thread(Run);
            runnerThread.Start();
        }
        /// <summary>
        /// End logging
        /// </summary>
        public void StopLogging()
        {
            if(runnerThread == null)
                return; //already stopped
            runnerThread = null;
            UnHookAll();
        }
        /// <summary>
        /// Runned in logger thread
        /// </summary>
        private void Run()
        {
            while (true)
            {
                Thread.Sleep(LOG_INTERVAL);
                LogRecord rec = MakeLogRecord();
                RaiseOnLogRecord(rec);
                ResetAllHooks();
            }
        }
        /// <summary>
        /// Enable all hooks
        /// </summary>
        private void HookAll()
        {
            mouseButtonsHook.Hook();
            mouseWheelHook.Hook();
            keyboardHook.Hook();
        }
        /// <summary>
        /// Disable all hooks
        /// </summary>
        private void UnHookAll()
        {
            mouseButtonsHook.UnHook();
            mouseWheelHook.UnHook();
            keyboardHook.UnHook();
        }
        /// <summary>
        /// Reset all hooks
        /// </summary>
        private void ResetAllHooks()
        {
            mouseButtonsHook.Reset();
            mouseWheelHook.Reset();
            keyboardHook.Reset();
        }
        /// <summary>
        /// Raise onLogRecord event if it exists
        /// </summary>
        /// <param name="rec">Log record to pass to handler</param>
        private void RaiseOnLogRecord(LogRecord rec)
        {
            if (onLogRecord != null)
                onLogRecord(rec);
        }
        /// <summary>
        /// Make new log record for current time
        /// </summary>
        /// <returns></returns>
        private LogRecord MakeLogRecord()
        {
            //make process descriptor
            ProcessDescriptor proc = new ProcessDescriptor()
            {
                ProcessName = WindowsBoundary.GetForegroundWindowProcess().ProcessName
            };
            //get rectangle of foreground window
            WindowsBoundary.Rect windRect = WindowsBoundary.GetForegroundWindowRect();
            //make window descriptor
            WindowDescriptor wind = new WindowDescriptor()
            {
                Title = WindowsBoundary.GetForegroundWindowText(),
                Location = new IntPoint(windRect.Left, windRect.Top),
                Size = new IntPoint(windRect.Right - windRect.Left, windRect.Top - windRect.Bottom)
            };
            //get current mouse position
            WindowsBoundary.Win32Point curPos = WindowsBoundary.GetMousePosition();
            //make log record
            LogRecord record = new LogRecord()
            {
                Time = DateTime.Now,
                PreviousRecordTime = default(DateTime),
                Process = proc,
                Window = wind,
                MousePosition = new IntPoint(curPos.X, curPos.Y),
                Keystrokes = keyboardHook.ActionsCount,
                MouseButtonActions = mouseButtonsHook.ActionsCount,
                MouseWheelActions = mouseWheelHook.ActionsCount
            };
            return record;
        }
    }
}
