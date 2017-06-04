using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;
using TimeMiner.Core.MetaInfoClasses;
using TimeMiner.Slave.BuiltinExtractors;

namespace TimeMiner.Slave.BuiltinExtensions
{
    public partial class OfflineActivityExtensionForm : Form
    {
        private const int MIN_SHORT_DESCRIPTION_LEN = 3;
        private const int MIN_INTERVAL_SECS = 5*60;

        public OfflineActivityExtensionForm()
        {
            Application.EnableVisualStyles();
            InitializeComponent();
        }

        private void dateTimePickerFrom_ValueChanged(object sender, EventArgs e)
        {
            if (dateTimePickerFrom.Value.AddSeconds(MIN_INTERVAL_SECS) > dateTimePickerTo.Value)
            {
                dateTimePickerTo.Value = dateTimePickerFrom.Value.AddSeconds(MIN_INTERVAL_SECS);
            }
        }

        private void dateTimePickerTo_ValueChanged(object sender, EventArgs e)
        {
            if (dateTimePickerFrom.Value.AddSeconds(MIN_INTERVAL_SECS) > dateTimePickerTo.Value)
            {
                dateTimePickerFrom.Value = dateTimePickerTo.Value.AddSeconds(-MIN_INTERVAL_SECS);
            }
        }

        private void OfflineActivityExtensionForm_Load(object sender, EventArgs e)
        {
            var now = DateTime.Now;
            var min = now.Date;
            var max = now.Date.AddDays(1).AddSeconds(-1);
            dateTimePickerFrom.MinDate = min;
            dateTimePickerFrom.MaxDate = max.AddSeconds(-MIN_INTERVAL_SECS);
            dateTimePickerTo.MinDate = min.AddSeconds(MIN_INTERVAL_SECS);
            dateTimePickerTo.MaxDate = max;

            var valTo = now;
            if (valTo > dateTimePickerTo.MaxDate)
            {
                valTo = dateTimePickerTo.MaxDate;
            }
            if (valTo < dateTimePickerTo.MinDate)
            {
                valTo = dateTimePickerTo.MinDate;
            }
            dateTimePickerTo.Value = valTo;
            dateTimePickerTo_ValueChanged(dateTimePickerTo, new EventArgs());
        }

        private void btOk_Click(object sender, EventArgs e)
        {
            bool valid = ValidateForm();
            if (!valid)
            {
                MessageBox.Show("Some fields are not filled properly");
                return;
            }
            var shortDescription = tbShortName.Text;
            var longDescription = tbDescription.Text;
            var type = (OfflineActivity.ActivityType) cbType.SelectedIndex;
            var begin = dateTimePickerFrom.Value;
            var end = dateTimePickerTo.Value;
            var activity = new OfflineActivity(begin,end,shortDescription,longDescription, type);
            OfflineActivityExtractor.SendQueue.Enqueue(activity);
            this.Close();

        }

        private bool ValidateForm()
        {
            bool valid = true;
            var shortDescription = tbShortName.Text;
            if (shortDescription.Length < MIN_SHORT_DESCRIPTION_LEN)
            {
                labelShortName.ForeColor = Color.Red;
                valid = false;
            }
            else
            {
                labelShortName.ForeColor = Color.Black;
            }
            var selectedIndex = cbType.SelectedIndex;
            if (selectedIndex != 0 && selectedIndex != 1)
            {
                labelType.ForeColor = Color.Red;
                valid = false;
            }
            else
            {
                labelType.ForeColor = Color.Black;
            }
            return valid;
        }

    }
}
