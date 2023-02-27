using System;
using System.CodeDom;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.Design;


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

        public BibleVersion(BibleTaggingForm container)
        {
            this.container = container;
        }

        public bool LoadBibleFile(string textFilePath, bool newBible)
        {
            if(newBible)
                bible.Clear();
            return LoadBibleFile(textFilePath);
        }

        private bool LoadBibleFile(string textFilePath)
        {
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
                            var line = reader.ReadLine();
                            ParseLine(line.Trim(' '));
                        }

                    }
                }

            }
            return result;
        }

        public Dictionary<string, Verse> Bible
        { get { return bible; } }

        protected virtual void ParseLine(string line)
        {
            // find spcae between book and Chapter
            int spaceB = line.IndexOf(' ');
            // find spcae between Verse number and Text
            int spaceV = line.IndexOf(' ', spaceB + 1);
            if (spaceV == -1)
            {
                throw new Exception(string.Format("Ill formed verse line!"));
            }

            string book = line.Substring(0, spaceB);
            string reference = line.Substring(0, spaceV);
            string verse = line.Substring(spaceV + 1);

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
        }

    }
}
