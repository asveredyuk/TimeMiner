using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TimeMiner.Core;

namespace TimeMiner.Master.Settings.ApplicationIdentifiers
{
    /// <summary>
    /// Identifier via process name
    /// </summary>
    public class ProcessNameIdentifier:ApplicationIdentifierBase
    {
        /// <summary>
        /// Name of process or mask
        /// </summary>
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
        /// <summary>
        /// Regex for checking, used only for mask
        /// </summary>
        private Regex regex;
        /// <summary>
        /// Name of process or mask
        /// </summary>
        private string _processName;
        public ProcessNameIdentifier()
        {
        }

        public ProcessNameIdentifier(string processName)
        {
            ProcessName = processName;
        }

        /// <inheritdoc />
        public override int CheckRecord(LogRecord record)
        {
            if (regex != null)
            {
                if(CheckMaskRegex(regex, record.Process.ProcessName.ToLower()))
                return 1;
            }
            //process name does not contain any special character
            if (record.Process.ProcessName.Equals(ProcessName, StringComparison.CurrentCultureIgnoreCase))
            {
                //simple process name match has higher priority
                return 2;
            }
            return 0;
        }
        /// <summary>
        /// Make regex from mask
        /// </summary>
        /// <param name="mask"></param>
        /// <returns></returns>
        private static Regex MakeMaskRegex(string mask)
        {
            return new Regex("^" + mask.ToLower().Replace(".", "[.]").Replace("*", ".*").Replace("?", ".") + "$", RegexOptions.Singleline);
        }
        /// <summary>
        /// Check if process name fits regex 
        /// </summary>
        /// <param name="regex">Regex with mask</param>
        /// <param name="processName">Process name</param>
        /// <returns></returns>
        private static bool CheckMaskRegex(Regex regex, string processName)
        {
            return regex.IsMatch(processName);
        }
    }
}
