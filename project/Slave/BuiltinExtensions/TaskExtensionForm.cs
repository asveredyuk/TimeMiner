using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TimeMiner.Core.MetaInfoClasses;
using TimeMiner.Slave.BuiltinExtractors;

namespace TimeMiner.Slave.BuiltinExtensions
{
    /// <summary>
    /// Form for specifying current task
    /// </summary>
    public partial class TaskExtensionForm : Form
    {
        public TaskExtensionForm()
        {
            Application.EnableVisualStyles();
            InitializeComponent();
            var task = TaskExtractor.CurrentTask;
            if (task != null)
                tbShortDescription.Text = task.ShortName;
        }

        private void btOk_Click(object sender, EventArgs e)
        {
            string desc = tbShortDescription.Text;
            if (desc.Length == 0)
            {
                //empty textbox
                TaskExtractor.CurrentTask = null;
            }
            else
            {
                TaskExtractor.CurrentTask = new TaskDescription(desc);
            }
            this.Close();
        }
    }
}
