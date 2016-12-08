using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace TimeMiner.Slave.Hooks
{
    public abstract class MouseHook : BaseHook
    {
        private const int WH_MOUSE_LL = 14;

        protected readonly LowLevelMouseProc _callback;

        protected MouseHook()
        {
            _callback = MouseHookCallback;
        }

        public override void Hook()
        {
            if(_hookID != IntPtr.Zero)
                throw new Exception("Already hooked");

            using (Process curProcess = Process.GetCurrentProcess())
            using (ProcessModule curModule = curProcess.MainModule)
            {
                _hookID = SetWindowsHookEx(WH_MOUSE_LL, _callback,
                    GetModuleHandle(curModule.ModuleName), 0);
            }
        }


        /// <summary>
        /// Happes when user interacts with mouse
        /// </summary>
        /// <param name="nCode"></param>
        /// <param name="wParam"></param>
        /// <param name="lParam"></param>
        /// <returns></returns>
        protected abstract IntPtr MouseHookCallback(
            int nCode, IntPtr wParam, IntPtr lParam);

        protected enum MouseMessages
        {
            WM_LBUTTONDOWN = 0x0201,
            WM_LBUTTONUP = 0x0202,
            WM_MOUSEMOVE = 0x0200,
            WM_MOUSEWHEEL = 0x020A,
            WM_RBUTTONDOWN = 0x0204,
            WM_RBUTTONUP = 0x0205
        }
        /// <summary>
        /// Hook callback delegate
        /// </summary>
        /// <param name="nCode"></param>
        /// <param name="wParam"></param>
        /// <param name="lParam"></param>
        /// <returns></returns>
        protected delegate IntPtr LowLevelMouseProc(int nCode, IntPtr wParam, IntPtr lParam);
        /// <summary>
        /// External function for setting hook
        /// </summary>
        /// <param name="idHook"></param>
        /// <param name="lpfn"></param>
        /// <param name="hMod"></param>
        /// <param name="dwThreadId"></param>
        /// <returns></returns>
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        protected static extern IntPtr SetWindowsHookEx(int idHook,
            LowLevelMouseProc lpfn, IntPtr hMod, uint dwThreadId);
    }
}
