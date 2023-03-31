using System;
using System.CodeDom;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms.Design;
using System.Xml.Schema;

namespace BibleTaggingUtil.BibleVersions
{
    public abstract class BibleVersion
    {

        protected BibleTaggingForm container;

        /// <summary>
        /// Bible Dictionary
        /// Key: verse reference (xxx c:v) xxx = OSIS book name, v = verse number
        /// </summary>
        protected Dictionary<string, Verse> bible = new Dictionary<string, Verse>();

        /// <summary>
        /// Bible Dictionary
        /// Key: UBS book name
        /// value: loaded Bible book name
        /// </summary>
        protected Dictionary<string, string> bookNames = new Dictionary<string, string>();

        protected List<string> bookNamesList = new List<string>();

        private const string referencePattern1 = @"^([0-9A-Za-z]+)\s([0-9]+):([0-9]+)\s*(.*)";
        private const string referencePattern2 = @"^[0-9]+_([0-9A-Za-z]+)\.([0-9]+)\.([0-9]+)\s*(.*)";
        private const string referencePattern3 = @"^([0-9A-Za-z]{3})\.([0-9]+)\.([0-9]+)\s*(.*)";
        private string textReferencePattern = string.Empty;

        protected string bibleName = string.Empty;
        protected int totalVerses = 0;
        protected int currentVerseCount;

        public BibleVersion(BibleTaggingForm container, int totalVerses)
        {
            this.container = container;
            this.totalVerses = totalVerses;
        }

        public string BibleName { set { bibleName = value; } }

        public bool LoadBibleFile(string textFilePath, bool newBible, bool more)
        {
            if (newBible)
            {
                bible.Clear();
                bookNames.Clear();
                bookNamesList.Clear();
                currentVerseCount = 0;
            }
            return LoadBibleFileInternal(textFilePath, more);
        }

        private bool LoadBibleFileInternal(string textFilePath, bool more)
        {
            Tracing.TraceEntry(MethodBase.GetCurrentMethod().Name, textFilePath, more);
            bool result = false;

            if (File.Exists(textFilePath))
            {
                result = true;
                using (var fileStream = new FileStream(textFilePath, FileMode.Open))
                {
                    using (StreamReader reader = new StreamReader(fileStream))
                    {
                        while (reader.Peek() >= 0)
                        {
                            var line = reader.ReadLine().Trim(' ');
                            if (!string.IsNullOrEmpty(line))
                            {
                                if (string.IsNullOrEmpty(textReferencePattern))
                                {
                                    //if (!SetSearchPattern(line, out textReferencePattern))
                                    //    continue;
                                    SetSearchPattern(line, out textReferencePattern);
                                }
                                if (!string.IsNullOrEmpty(textReferencePattern))
                                {
                                    AddBookName(line);
                                }
                                ParseLine(line);
                            }
                        }

                    }
                }

            }

            if(!more && !(new int[] {66, 39, 27 }).Contains(bookNamesList.Count))
            {
                Tracing.TraceError(MethodBase.GetCurrentMethod().Name, string.Format("{0}:Book Names Count = {1}. Was expecting 66, 39 or 27",
                                        Path.GetFileName(textFilePath), bookNamesList.Count));
                return false;
            }


            if (bookNamesList.Count == 66 || bookNamesList.Count == 39)
            {
                for (int i = 0; i < bookNamesList.Count; i++)
                {
                    bookNames.Add(Constants.ubsNames[i], bookNamesList[i]);
                }
            }
            else if (bookNamesList.Count == 27)
            {
               for (int i = 0; i < bookNamesList.Count; i++)
                {
                    bookNames.Add(Constants.ubsNames[i+39], bookNamesList[i]);
                }
            }
            return result;
        }

        public int BookCount
        {
            get
            {
                return bookNamesList.Count;
            }
        }
        public Dictionary<string, Verse> Bible
        { get { return bible; } }


        public string this[string ubsName]
        {
            get
            {
                string bookName = string.Empty;
                try
                {
                    if(bookNames.Count > 0)
                    bookName = bookNames[ubsName];
                }
                catch(Exception ex)
                {
                    Tracing.TraceException(MethodBase.GetCurrentMethod().Name, ex.Message);
                }
                return bookName;

            }
        }

        private bool AddBookName(string line)
        {
            if(line.ToLower().Contains("gen"))
            {
                int o = 0;
            }
            Match mTx = Regex.Match(line, textReferencePattern);
            if (!mTx.Success)
            {
                //Tracing.TraceError(MethodBase.GetCurrentMethod().Name, "Could not detect text reference: " + line);
                return false;
            }

            String book = mTx.Groups[1].Value;
            if (!bookNamesList.Contains(book))
                bookNamesList.Add(book);

            return true;
        }

        private bool SetSearchPattern(string line, out string referancePattern)
        {

            Match mTx = Regex.Match(line, referencePattern1);
            if (mTx.Success)
            {
                referancePattern = referencePattern1;
                return true;
            }

            mTx = Regex.Match(line, referencePattern2);
            if (mTx.Success)
            {
                referancePattern = referencePattern2;
                return true;
            }

            mTx = Regex.Match(line, referencePattern3);
            if (mTx.Success)
            {
                referancePattern = referencePattern3;
                return true;
            }

            //Tracing.TraceError(MethodBase.GetCurrentMethod().Name, "Could not detect reference pattern: " + line);
            referancePattern = string.Empty;
            return false;
        }

        protected virtual void ParseLine(string line)
        {

            Match mTx = Regex.Match(line, @"^([0-9A-Za-z]+)\s([0-9]+):([0-9]+)\s*(.*)");

            /*            // find spcae between book and Chapter
                        int spaceB = line.IndexOf(' ');
                        // find spcae between Verse number and Text
                        int spaceV = line.IndexOf(' ', spaceB + 1);
                        if (spaceV == -1)
                        {
                            throw new Exception(string.Format("Ill formed verse line!"));
                        }

                        string book = line.Substring(0, spaceB);
                        string reference = line.Substring(0, spaceV);
                        string verse = line.Substring(spaceV + 1);*/

            string book = mTx.Groups[1].Value;
            string chapter = mTx.Groups[2].Value;
            string verseNo = mTx.Groups[3].Value;
            string verse = mTx.Groups[4].Value;
            string reference = string.Format("{0} {1}:{2}", book, chapter, verseNo);
            if(reference == "Jhn 1:1")
            { 
                int x = 0;
            }


            BibleTestament testament = Utils.GetTestament(reference);

            string[] verseParts = verse.Split(' ');
            List<string> words = new List<string>();
            List<string> tags= new List<string>();
            string tempWord = string.Empty;
            string tmpTag = string.Empty;
            for (int i = 0; i < verseParts.Length; i++)
            {
                string versePart = verseParts[i].Trim();
                if (string.IsNullOrEmpty(versePart))
                    continue; // some extra space
                if (i == 0 || (versePart[0] != '<' && versePart[0] != '(')) // add i == 0 test because a verse can not start with a tag.
                {
                    if (!string.IsNullOrEmpty(tmpTag))
                        tags.Add(tmpTag);
                    tmpTag = string.Empty;
                    tempWord += (string.IsNullOrEmpty(tempWord)) ? verseParts[i] : (" " + verseParts[i]);
                    if (i == verseParts.Length - 1)
                    {
                        // last word
                        words.Add(tempWord);
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(tempWord))
                        words.Add(tempWord);
                    tempWord = string.Empty;
                    if (verseParts[i] == "<>")
                    {
                        tmpTag = "<>";
                    }
                    else
                    {
                        tmpTag += (string.IsNullOrEmpty(tmpTag)) ? verseParts[i] : (" " + verseParts[i]);
                        tmpTag = tmpTag.Replace(".", "");
                        if (i == verseParts.Length - 1)
                        {
                            // last word
                            if (tmpTag.EndsWith('.'))
                                tmpTag.Remove(tmpTag.Length - 1, 1);
                            tags.Add(tmpTag);
                        }
                    }
                }
            }

            if(words.Count == (tags.Count + 1)) // last word was not tagged
                tags.Add(string.Empty);
            string[] vWords = words.ToArray();
            string[] vTags = tags.ToArray();
            if(vWords.Length != vTags.Length)
            {
                throw new Exception(string.Format("Word Count = {0}, Tags Count {1}", vWords, vTags.Length));
            }

            // remove <> from tags
            for (int i = 0; i < vTags.Length; i++)
                vTags[i] = vTags[i].Replace("<", "").Replace(">", "");

            Verse verseWords = new Verse();
            for (int i = 0; i < vWords.Length; i++)
            {
                string[] splitTags = vTags[i].Split(' ');
                verseWords[i] = new VerseWord(vWords[i], splitTags, reference);
            }

            bible.Add(reference, verseWords);
            currentVerseCount++;
            container.UpdateProgress(bibleName, (100 * currentVerseCount) / totalVerses);

        }

    }
}
