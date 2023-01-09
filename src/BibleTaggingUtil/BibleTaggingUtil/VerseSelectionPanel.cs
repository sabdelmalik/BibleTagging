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
using static System.Net.Mime.MediaTypeNames;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace BibleTaggingUtil
{

    public partial class VerseSelectionPanel : DockContent
    {
        private Dictionary<string, BibleBook> bibleBooks = new Dictionary<string, BibleBook>();

        public event VerseChangedEventHandler VerseChanged;

        bool useAltNames = true;

        public VerseSelectionPanel()
        {
            InitializeComponent();
            this.ControlBox = false;
            for (int i = 0; i < Constants.osisNames.Length; i++)
            {
                bibleBooks.Add(Constants.osisNames[i], new BibleBook(Constants.fileNames[i], Constants.osisAltNames[i], Constants.LAST_VERSE[i]));
            }
        }

        private void VerseSelectionPanel_Load(object sender, EventArgs e)
        {
            lbBookNames.Items.AddRange(Constants.osisNames);
            lbBookNames.SelectedIndex = 0;

            toolStrip1.Cursor = Cursors.Default;
        }


        public Dictionary<string, BibleBook> BibleBooks
        {
            get { return bibleBooks; }  
        }

        public bool UseAltNames
        {
            get { return useAltNames; }
        } 
        private void lbBookNames_SelectedIndexChanged(object sender, EventArgs e)
        {
            string book = lbBookNames.SelectedItem.ToString();
            int[] lastVerse = bibleBooks[book].LastVerse;
            string[] chapters = new string[lastVerse.Length];
            for (int i = 0; i < lastVerse.Length; i++)
            {
                chapters[i] = (i + 1).ToString();
            }
            lbChapters.Items.Clear();
            lbChapters.Items.AddRange(chapters);
            lbChapters.SelectedIndex = 0;
        }

        private void lbChapters_SelectedIndexChanged(object sender, EventArgs e)
        {
            string book = lbBookNames.SelectedItem.ToString();
            int[] lastVerse = bibleBooks[book].LastVerse;
            int chapter = lbChapters.SelectedIndex;
            int verses = lastVerse[chapter];
            string[] versesStr = new string[verses];
            for (int i = 0; i < verses; i++)
            {
                versesStr[i] = (i + 1).ToString();
            }
            lbVerses.Items.Clear();
            lbVerses.Items.AddRange(versesStr);
            lbVerses.SelectedIndex = 0;
        }

        private void lbVerses_SelectedIndexChanged(object sender, EventArgs e)
        {
            FireVerseChanged();
        }


        public void FireVerseChanged()
        {
            string book = lbBookNames.SelectedItem.ToString();
            int[] lastVerse = bibleBooks[book].LastVerse;
            int chapter = lbChapters.SelectedIndex + 1;
            int verse = lbVerses.SelectedIndex + 1;

            string verseRef = string.Format("{0:s} {1:d}:{2:d}", (useAltNames ? bibleBooks[book].BookAltName : book), chapter, verse);
            int bookIdx = Array.IndexOf(Constants.osisNames, book);
            if (this.VerseChanged != null)
            {
                this.VerseChanged(this, new VerseChangedEventArgs(verseRef, (bookIdx < 39) ? TestamentEnum.OLD : TestamentEnum.NEW));
            }

        }


        public int CurrentBook
        {
            get { return lbBookNames.SelectedIndex; }
            set { lbBookNames.SelectedIndex = value; }
        }
        public int CurrentChapter
        {
            get { return lbChapters.SelectedIndex; }
            set { lbChapters.SelectedIndex = value; }
        }
        public int CurrentVerse
        {
            get { return lbVerses.SelectedIndex; }
            set { lbVerses.SelectedIndex = value; }
        }

        public void MoveToPrevious()
        {
            string book = lbBookNames.SelectedItem.ToString();
            int[] lastVerse = bibleBooks[book].LastVerse;

            int currentVerse = lbVerses.SelectedIndex;
            if (currentVerse > 0)
            {
                lbVerses.SelectedIndex = currentVerse - 1;
            }
            else
            {
                int currentChapter = lbChapters.SelectedIndex;
                if (currentChapter > 0)
                {
                    lbChapters.SelectedIndex = currentChapter - 1;
                    lbVerses.SelectedIndex = lastVerse[currentChapter - 1] - 1;
                }
                else
                {
                    int currentBook = lbBookNames.SelectedIndex;
                    if (currentBook > 0)
                    {
                        lbBookNames.SelectedIndex = currentBook - 1;
                        book = (string)lbBookNames.Items[currentBook - 1];
                        lastVerse = bibleBooks[book].LastVerse;
                        int newChapter = lastVerse.Length - 1;
                        lbChapters.SelectedIndex = newChapter;
                        lbVerses.SelectedIndex = lastVerse[newChapter] - 1;
                    }

                }

            }

            int chapter = lbChapters.SelectedIndex + 1;

        }

        public void MoveToNext()
        {
            string book = lbBookNames.SelectedItem.ToString();
            int[] lastVerse = bibleBooks[book].LastVerse;
            int currentChapter = lbChapters.SelectedIndex;

            int currentVerse = lbVerses.SelectedIndex;
            if (currentVerse < (lastVerse[currentChapter] - 1))
            {
                lbVerses.SelectedIndex = currentVerse + 1;
            }
            else
            {
                if (currentChapter < (lastVerse.Length - 1))
                {
                    lbChapters.SelectedIndex = currentChapter + 1;
                    lbVerses.SelectedIndex = 0;
                }
                else
                {
                    int currentBook = lbBookNames.SelectedIndex;
                    if (currentBook < (lbBookNames.Items.Count - 1))
                    {
                        lbBookNames.SelectedIndex = currentBook + 1;
                        lbChapters.SelectedIndex = 0;
                        lbVerses.SelectedIndex = 0;
                    }

                }
            }
        }

        public string GetNextRef(string currentReference)
        {
            string result = string.Empty;
            bool osisName = false;
            bool altName = false;
            try
            {
                if (string.IsNullOrEmpty(currentReference))
                    return result;

                int idx1 = currentReference.IndexOf(' ');
                if (idx1 < 0)
                    return result;
                int idx2 = currentReference.IndexOf(':', idx1 + 1);
                if (idx2 < 0)
                    return result;

                string book = currentReference.Substring(0, idx1);
                string chapter = currentReference.Substring(idx1 + 1, idx2 - idx1 - 1);
                string verse = currentReference.Substring(idx2 + 1);
                int currentChapter = 0;
                int currentVerse = 0;
                if (!int.TryParse(chapter, out currentChapter))
                    return result;
                if (!int.TryParse(verse, out currentVerse))
                    return result;

                string newBook = book;
                int newChapter = currentChapter;
                int newVerse = currentVerse;

                if(Constants.osisNames.Contains(newBook))
                    osisName = true;

                if(Constants.osisAltNames.Contains(newBook))
                    altName = true;

                // bibleBooks keys are osis
                if(!osisName)
                    book = Constants.osisNames[Array.IndexOf(Constants.osisAltNames, book)];

                int[] lastVerse = bibleBooks[book].LastVerse;
                if (currentVerse < (lastVerse[currentChapter - 1]))
                {
                    newVerse = currentVerse + 1;
                }
                else
                {
                    if ((currentChapter) < (lastVerse.Length))
                    {
                        newChapter = currentChapter + 1;
                        newVerse = 1;
                    }
                    else
                    {
                        int currentBook = Array.IndexOf(Constants.osisNames, book);
                        if (currentBook < (Constants.osisNames.Length - 1))
                        {
                            book = Constants.osisNames[currentBook + 1];
                            newChapter = 1;
                            newVerse = 1;
                        }

                    }
                }

                if (altName)
                {
                    newBook = Constants.osisAltNames[Array.IndexOf(Constants.osisNames, book)];
                }

                result = string.Format("{0} {1}:{2}", newBook, newChapter, newVerse);
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return result;

        }

        public void GotoVerse(string verseRef)
        {
            if (string.IsNullOrEmpty(verseRef))
                return;

            int idx1 = verseRef.IndexOf(' ');
            if (idx1 < 0)
                return;
            int idx2 = verseRef.IndexOf(':', idx1 + 1);
            if (idx2 < 0)
                return;

            string book = verseRef.Substring(0, idx1);
            string chapter = verseRef.Substring(idx1 + 1, idx2 - idx1 - 1);
            string verse = verseRef.Substring(idx2 + 1);
            int currentChapter = 0;
            int currentVerse = 0;
            if (!int.TryParse(chapter, out currentChapter))
                return;
            if (!int.TryParse(verse, out currentVerse))
                return;

            string[] names = Constants.osisNames;
            if (useAltNames)
                names = Constants.osisAltNames;

            // reference version has
            // Sng as Sol
            // Ezk as Eze
            // Jol as Joe
            // Nam as Nah
            if (book == "Sol") book = "Sng";
            else if (book == "Eze") book = "Ezk";
            else if (book == "Joe") book = "Jol";
            else if (book == "Nah") book = "Nam";


            int currentBook = Array.IndexOf(names, book);

            SetSelectedIndex(currentBook, currentChapter-1, currentVerse-1);



        }

        private void SetSelectedIndex(int currentBook, int currentChapter, int currentVerse)
        {
            if (InvokeRequired)
            {
                // Call this same method but append THREAD2 to the text
                Invoke(delegate { SetSelectedIndex(currentBook, currentChapter, currentVerse); });
            }
            else
            {
                lbBookNames.SelectedIndex = currentBook;
                lbChapters.SelectedIndex = currentChapter;
                lbVerses.SelectedIndex = currentVerse;
            }
        }


        private void toolStripPrevious_Click(object sender, EventArgs e)
        {
            MoveToPrevious();
        }

        private void toolStripNext_Click(object sender, EventArgs e)
        {
            MoveToNext();
        }
    }

    public delegate void VerseChangedEventHandler(object sender, VerseChangedEventArgs e);

    public enum TestamentEnum
    {
        OLD,
        NEW
    }

    public class VerseChangedEventArgs : EventArgs
    {
        public VerseChangedEventArgs(string verseReference, TestamentEnum testament)
        {
            this.VerseReference = verseReference;
            Testament = (testament == TestamentEnum.NEW)? "G" : "H";
        }
        public string VerseReference { get; private set; }
        public string Testament { get; private set; }

    }



}
