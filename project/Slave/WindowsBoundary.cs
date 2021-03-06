﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using TimeMiner.Core;

namespace TimeMiner.Slave
{
    public static class WindowsBoundary
    {
        /// <summary>
        /// Get current cursor position
        /// </summary>
        /// <returns></returns>
        public static Win32Point GetMousePosition()
        {
            Win32Point pt = new Win32Point();
            GetCursorPos(ref pt);
            return pt;
        }
        /// <summary>
        /// Get process-owner of foreground window
        /// </summary>
        /// <returns></returns>
        public static Process GetWindowProcess(IntPtr hWnd)
        {
            uint pid;
            GetWindowThreadProcessId(hWnd, out pid);
            return Process.GetProcessById((int)pid);
        }
        /// <summary>
        /// Get title of foreground window
        /// </summary>
        /// <returns></returns>
        public static string GetWindowText(IntPtr hWnd)
        {
            const int nChars = 256;
            StringBuilder buff = new StringBuilder(nChars);
            int hr = GetWindowText(hWnd, buff, nChars);
            if (hr > 0)
            {
                return buff.ToString();
            }
            return null;
        }
        /// <summary>
        /// Get rectangle of foreground window
        /// </summary>
        /// <returns></returns>
        public static Rect GetWindowRect(IntPtr hWnd)
        {

            Rect r = new Rect();
            GetWindowRect(hWnd, ref r);
            return r;
        }
        [StructLayout(LayoutKind.Sequential)]
        public struct Rect
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;

            public override string ToString()
            {
                return $"Left: {Left}, Top: {Top}, Right: {Right}, Bottom: {Bottom}";
            }
        }
        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool GetWindowRect(IntPtr hWnd, ref Rect lpRect);
        [StructLayout(LayoutKind.Sequential)]
        public struct Win32Point
        {
            public int X;
            public int Y;
        };
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool GetCursorPos(ref Win32Point pt);
        [DllImport("user32.dll")]
        public static extern IntPtr GetForegroundWindow();
        [DllImport("user32.dll")]
        static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int count);
        [DllImport("user32.dll")]
        static extern int GetWindowThreadProcessId(IntPtr hWnd, out uint ProcessId);
    }
}
