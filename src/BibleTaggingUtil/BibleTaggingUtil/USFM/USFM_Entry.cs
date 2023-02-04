using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SM.Bible.Formats.USFM
{
    public class USFM_Entry
    {
        private string code = string.Empty;
        private string text = string.Empty;
        private int number = -1;    


        public string Marker { get; set; } = string.Empty;
        public bool HasCode { get; private set; } = false;
        public string Code { get { return code; } set { HasCode = true; code = value; } }
        public bool HasNumber { get; private set; } = false;
        public int Number { get { return number; } set { HasNumber = true; number = value; } }
        public bool HasText { get; private set; } = false;
        public string Text {
            get { return text; }
            set { HasText = true; text = value; } }
        }

    public class EntryMap
    {
        public string Book { get; set; } = string.Empty;
        public int Index { get; set; } = -1;
    }
}
