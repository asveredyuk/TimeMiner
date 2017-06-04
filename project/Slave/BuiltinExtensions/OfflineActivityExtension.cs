using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeMiner.Slave.BuiltinExtensions
{
    /// <summary>
    /// Extension for adding info about offline activity
    /// </summary>
    public class OfflineActivityExtension:ClientInterfaceExtension
    {
        private OfflineActivityExtensionForm form;
        [MenuItem("Offline activity")]
        public void OfflineActivity()
        {
            if (form == null || form.IsDisposed)
            {
                form = new OfflineActivityExtensionForm();
                form.Show();
            }
        }
    }
}
