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
    public partial class BrowserPanel : DockContent
    {

        private string lexiconWebsite = string.Empty;

            
        public BrowserPanel()
        {
            InitializeComponent();
            this.ControlBox = false;
        }

        public string LexiconWebsite
        {
            set
            {
                lexiconWebsite = value;
            }
        }

        public void NavigateTo(string url)
        {
            webView21.Source = new System.Uri(url);
        }

        public void NavigateToTag(string tag)
        {
            if(!string.IsNullOrEmpty(lexiconWebsite))
            {
                webView21.Source = new System.Uri(string.Format(lexiconWebsite, tag));
            }

        }
    }
}
