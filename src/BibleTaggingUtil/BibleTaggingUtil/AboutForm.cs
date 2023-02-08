using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Reflection;


namespace BibleTaggingUtil
{
    public partial class AboutForm : Form
    {
        public AboutForm()
        {
            InitializeComponent();
        }

        private void About_Load(object sender, EventArgs e)
        {

            Assembly assembly = Assembly.GetExecutingAssembly();
            AssemblyName assemblyName = assembly.GetName();
            Version version = assemblyName.Version;
            string author = "Sami Abdel Malik";
            string copyright = "Copyright © 2023 by Sami Abdel Malik";
            string title = "Bible Text Tagging with Strong's Numbers";

            textBoxAbout.Text = "\r\n" +title + "\r\n\r\n";
            textBoxAbout.Text += copyright + "\r\n\r\n";
            textBoxAbout.Text += "Version # " + version.ToString() + "\r\n";

            textBoxAbout.DeselectAll();
            textBoxAbout.SelectedText = "";
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
