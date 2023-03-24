using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BibleTaggingUtil
{
    public class BibleBook
    {
        public BibleBook(string fileName, string bookAltName, string bookAltName2, string bookUbsName, int[] lastVerse)
        {
            this.FileName = fileName;
            this.BookAltName = bookAltName;
            this.BookAltName2 = bookAltName2;
            this.BookUbsName = bookUbsName;
            this.LastVerse = lastVerse;
        }

        public string FileName { get; private set; }
        public string BookAltName { get; private set; }
        public string BookAltName2 { get; private set; }
        public string BookUbsName { get; private set; }
        public int[] LastVerse { get; private set; }

        public int Chapters { get { return LastVerse.Length; } }
    }
}
