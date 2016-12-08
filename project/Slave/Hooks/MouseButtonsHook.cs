using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeMiner.Slave.Hooks
{
    public class MouseButtonsHook : MouseHook
    {
        protected override IntPtr MouseHookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0)
            {
                MouseMessages message = (MouseMessages) wParam;
                //check the action
                if (message == MouseMessages.WM_LBUTTONDOWN || message == MouseMessages.WM_RBUTTONDOWN)
                {
                    ActionsCount++;
                }
            }
            return CallNextHookEx(_hookID, nCode, wParam, lParam);
        }
    }
}
