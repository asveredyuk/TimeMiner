using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Automation;

namespace TimeMiner.Slave
{
    public class BrowserUrlExtractor : MetaExtractor
    {
        public static readonly string[] ACEPTABLE_BROWSERS = {"chrome","firefox","opera","iexplore"};
        public const string META_KEY = "url";
        public override bool CanAccept(Process process, IntPtr wHandle)
        {
            return ACEPTABLE_BROWSERS.Contains(process.ProcessName.ToLower());
        }

        public override KeyValuePair<string, byte[]> Extract(Process process, IntPtr wHandle)
        {
            string url = GetUrlBrowser(wHandle);
            byte[] arr = Encoding.UTF8.GetBytes(url);
            return new KeyValuePair<string, byte[]>(META_KEY,arr);
        }

        private static string GetUrlBrowser(IntPtr handle)
        {
            AutomationElement element = AutomationElement.FromHandle(handle);
            if (element == null)
                return null;
            AutomationElement edit = element.FindFirst(TreeScope.Subtree,
                new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Edit));
            if (edit != null)
                return ((ValuePattern)edit.GetCurrentPattern(ValuePattern.Pattern)).Current.Value;
            return null;
        }
    }
}
