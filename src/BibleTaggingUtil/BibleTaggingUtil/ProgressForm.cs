using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BibleTaggingUtil
{
    public partial class ProgressForm : Form
    {
        BibleTaggingForm container;
        public ProgressForm()
        {
            InitializeComponent();
        }

        public ProgressForm(BibleTaggingForm container)
        {
            InitializeComponent();
            this.container = container;
        }

        public string Label { set { label.Text = value; } }

        public int Progress { set { progressBar.Value = value; } }

        public void Clear()
        {
            progressBar.Value = 0;
            label.Text = string.Empty;
        }

        private void ProgressForm_Load(object sender, EventArgs e)
        {
            //label.Location = new Point(
            //        this.Location.X + (this.Width / 2) - (label.Width / 2),
            //        this.Location.Y + (this.Height / 2) - (label.Height / 2));
        }
    }
}
