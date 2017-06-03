using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace TimeMiner.Slave.BuiltinExtensions
{
    /// <summary>
    /// Extension for exiting from the app
    /// </summary>
    public class ExitExtension:ClientInterfaceExtension
    {
        [MenuItem("Exit",100)]
        public void DoExit()
        {
            Application.Current.Shutdown();
        }
    }
}
