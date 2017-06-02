using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TimeMiner.Slave;

namespace SlaveTestPlugin
{
    public class HelloWorldExtension : ClientInterfaceExtension
    {
        [MenuItem("Hello world", -1000)]
        public void Hello()
        {
            MessageBox.Show("Hello World!");
        }
    }
}
