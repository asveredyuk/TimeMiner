using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeMiner.Slave.BuiltinExtensions
{
    /// <summary>
    /// Allows user to set current task
    /// </summary>
    public class TaskExtension: ClientInterfaceExtension
    {
        private TaskExtensionForm taskForm;
        [MenuItem("Set task")]
        public void SetTask()
        {
            if (taskForm == null || taskForm.IsDisposed)
            {
                //form is not visible
                taskForm = new TaskExtensionForm();
                taskForm.Show();
            }
        }

    }
}
