using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BibleTaggingUtil
{
    internal class CofigurationHolder
    {
        public string UntaggedBible { get; set; }
        private string TaggedBible { get; set; }

        private string KJV { get; set; }
        private List<string> HebrewReferences { get; set; }
        private List<string> GreekReferences { get; set; }


    }
}
