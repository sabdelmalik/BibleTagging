using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace BibleTaggingUtil
{
    public partial class SettingsForm : DockContent
    {
        public SettingsForm()
        {
            InitializeComponent();

            // https://learn.microsoft.com/en-us/dotnet/desktop/winforms/advanced/how-to-read-settings-at-run-time-with-csharp?view=netframeworkdesktop-4.8

            string referenceBiblePath = string.Empty;

            //referenceBiblePath = Properties.Settings.Default.ReferenceBibleFileName;
            //Properties.Settings.Default.ReferenceBibleFileName = "xyz";
            //Properties.Settings.Default.Save();


        }

        public bool PeriodicSaveEnabled
        {
            get { return cbPeriodicSave.Checked; }
            set { cbPeriodicSave.Checked = value; }
        }

        public int SavePeriod
        {
            get
            {
                return (int)nudSavePeriod.Value;
            }
            set
            {
                nudSavePeriod.Value = value;
            }
        }

        private void cbPeriodicSave_CheckedChanged(object sender, EventArgs e)
        {
            nudSavePeriod.Enabled = cbPeriodicSave.Checked;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            this.Close();
            this.DialogResult = DialogResult.OK;
        }
    }
}
