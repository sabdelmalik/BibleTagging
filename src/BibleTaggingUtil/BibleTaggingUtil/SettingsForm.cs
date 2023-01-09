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

namespace BibleTagging
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

     }
}
