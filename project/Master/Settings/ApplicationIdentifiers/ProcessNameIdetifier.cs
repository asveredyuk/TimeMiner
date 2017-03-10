using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TimeMiner.Core;

namespace TimeMiner.Master.Settings.ApplicationIdentifiers
{
    public class ProcessNameIdetifier:ApplicationIdentifierBase
    {
        public string ProcessName
        {
            get { return _processName; }
            set
            {
                _processName = value;
                if (ProcessName.Contains("*") || ProcessName.Contains("?"))
                {
                    regex = MakeMaskRegex(ProcessName);
                }
            }
        }

        private Regex regex;
        private string _processName;

        public ProcessNameIdetifier()
        {
        }

        public ProcessNameIdetifier(string processName)
        {
            ProcessName = processName;
        }

        public override bool CheckRecord(LogRecord record)
        {
            if (regex != null)
            {
                return CheckMaskRegex(regex, record.Process.ProcessName.ToLower());
            }
            //process name does not contain any special character
            return record.Process.ProcessName.Equals(ProcessName, StringComparison.CurrentCultureIgnoreCase);
        }

        private static Regex MakeMaskRegex(string mask)
        {
            return new Regex("^" + mask.ToLower().Replace(".", "[.]").Replace("*", ".*").Replace("?", ".") + "$", RegexOptions.Singleline);
        }

        private static bool CheckMaskRegex(Regex regex, string fname)
        {
            return regex.IsMatch(fname);
        }
    }
}
