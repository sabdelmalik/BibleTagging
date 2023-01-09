using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BibleTagging
{
    public class BibleBook
    {
        public BibleBook(string fileName, string bookAltName, int[] lastVerse)
        {
            this.FileName = fileName;
            this.BookAltName = bookAltName;
            this.LastVerse = lastVerse;
        }

        public string FileName { get; private set; }
        public string BookAltName { get; private set; }
        public int[] LastVerse { get; private set; }

        public int Chapters { get { return LastVerse.Length; } }
    }
}
