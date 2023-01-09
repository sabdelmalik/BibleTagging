using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BibleTagging
{
    public class VerseWord
    {
        public VerseWord(string hebrew, string english, string[] strong, string transliteration)
        {
            this.Hebrew = hebrew;
            this.English = english;
            this.Strong = strong;
            this.Transliteration = transliteration;
        }

        // For Greek we only allow one strong reference
        public VerseWord(string greek, string english, string strong, string transliteration)
        {
            this.Greek = greek;
            this.English = english;
            this.StrongG = strong;
            this.Transliteration = transliteration;
        }

        public string Hebrew { get; private set; }
        public string Greek { get; private set; }
        public string English { get; private set; }
        public string[] Strong { get; private set; }
        public string StrongG { get; private set; }
        public string Transliteration { get; private set; }

    }
}
