using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BibleTagging
{
    public class TOTHTReader
    {

        /// <summary>
        /// Verse words dictionary
        /// key: the word number in the verse
        /// Value: a populated VerseWord instance
        /// </summary>
        private Dictionary<int, VerseWord> verseWords = null;

        /// <summary>
        /// Bible Dictionary
        /// Key: verse reference (xxx c:v) xxx = book name, c = chapter number, v = verse number
        /// Value: a dictionary containing the verse words, with the key being the word number (zero based)
        ///         and the value a VarseWord instance
        /// </summary>
        private Dictionary<string, Dictionary<int, VerseWord>> bible = new Dictionary<string, Dictionary<int, VerseWord>>();

        string textFilePath = string.Empty;

        public Dictionary<string, Dictionary<int, VerseWord>> TOTHTDict
            { get { return bible; } }   

        public void LoadBibleFile(string textFilePath)
        {

            if (File.Exists(textFilePath))
            {
                using (var fileStream = new FileStream(textFilePath, FileMode.Open))
                {

                    using (StreamReader reader = new StreamReader(fileStream))
                    {
                        bible.Clear();
                        while (reader.Peek() >= 0)
                        {
                            var line = reader.ReadLine();
                            #region Skip the header
                            if (string.IsNullOrEmpty(line))
                                continue;
                            int c = line.Count(f => (f == '\t'));
                            if (c != 14)
                            {
                                continue;
                            }

                            if (line.StartsWith("Ref in") || line.StartsWith("======"))
                                continue;
                            #endregion Skip the header

                            ParseLine(line);
                        }

                    }
                }

            }
            //SaveUpdates();
        }

        enum STATE
        {
            START,
            STRONG,
            HEBREW
        }
        private void SaveUpdates()
        {
            using (StreamWriter outputFile = new StreamWriter(@"C:\temp\toth.txt"))
            {

                for (int i = 0; i < bible.Keys.Count; i++)
                {
                    string[] keys = bible.Keys.ToArray();
                    string verRef = keys[i];


                    try
                    {
                        Dictionary<int, VerseWord> words = bible[verRef];
                        int[] wordKeys = words.Keys.ToArray();
                        VerseWord vw = words[wordKeys[0]];
                        string verse = vw.English + " [" + vw.Hebrew + "]";
                        for (int j = 0; j < vw.Strong.Length; j++)
                        {
                            verse += (" <" + vw.Strong[j])+ ">";
                        }
                        for (int k = 1; k < wordKeys.Length; k++)
                        {
                            verse += " " + (words[wordKeys[k]].English + " [" + words[wordKeys[k]].Hebrew + "]");
                            for (int j = 0; j < words[wordKeys[k]].Strong.Length; j++)
                            {
                                verse += (" <" + words[wordKeys[k]].Strong[j]) + ">";
                            }
                        }
                        string line = string.Format("{0:s} {1:s}", verRef, verse);
                        outputFile.WriteLine(line);
                    }
                    catch (Exception ex)
                    {
                        string x = ex.Message;
                    }


                }


                //for (int i = 0; i < targetVersionUpdates.Keys.Count; i++)
                //{
                //    string key = targetVersionUpdates.Keys.ToArray()[i];
                //    string line = string.Format("{0:s} {1:s}", key, targetVersionUpdates[key]); 
                //    outputFile.WriteLine(line); 
                //}

            }


        }

        string currentVerseRef = string.Empty;
        /// <summary>
        /// Parses the word line into a VerseWord class instance 
        /// then adds to word verses dictionary
        /// </summary>
        /// <param name="Line">A line representing a word in a verse</param>
        private void ParseLine(string Line)
        {
            string[] lineParts = Line.Split('\t');
            string verseRef = string.Empty;
            int wordNumber = 0;
            string englishWord = string.Empty;
            string[] strongRefs = null;
            string hebrew = string.Empty;
            string transliteration = string.Empty;
            List<string> strongList = new List<string>();

            for (int idx = 0; idx < lineParts.Length; idx++)
            {
                string part = lineParts[idx];

                switch (idx)
                {
                    case 0:
                        // verse Reference index 0 is for Hebrew Bible Versification
                        break;

                    case 1:
                        {
                            // verse Reference index 1 is for KJV versification
                            // verse ref may be followed by K for Ketiv or Q for Qere
                            // we ignore those, we only use verse references ending in T
                            if (!part.EndsWith("WLT"))
                                return;

                            string verRef = part.Substring(0, part.IndexOf(" : WLT"));
                            string[] verRefParts = verRef.Split('.');
                            string[] verNumParts = verRefParts[2].Split('#');
                            int chapter = 0;
                            int verse = 0;
                            if (!int.TryParse(verRefParts[1], out chapter))
                                Console.WriteLine("++++++++++++++++++++++++ Failed to parse chapter number: {0:s}", verRefParts[1]);
                            if (!int.TryParse(verNumParts[0], out verse))
                                Console.WriteLine("++++++++++++++++++++++++ Failed to parse verse number: {0:s}", verNumParts[0]);
                            if (!int.TryParse(verNumParts[1].Replace("w","").Replace("p", "").Replace("+", ""), out wordNumber))
                                Console.WriteLine("++++++++++++++++++++++++ Failed to parse word number: {0:s}", verNumParts[1]);

                            verseRef = string.Format("{0:s} {1:d}:{2:d}", verRefParts[0], chapter, verse);
                            //Console.WriteLine(string.Format("{0:s} [{1:d}]", verseRef, wordNumber));

                            if (string.IsNullOrEmpty(currentVerseRef))
                            {
                                // very first verse
                                currentVerseRef = verseRef;
                                verseWords = new Dictionary<int, VerseWord>();
                            }

                            if (verseRef != currentVerseRef)
                            {
                                bible.Add(currentVerseRef, verseWords);
                                currentVerseRef = verseRef;
                                verseWords = new Dictionary<int, VerseWord>();
                            }
                        }
                        break;

                    case 2:
                        // this is the Hebrew word
                        //Console.WriteLine("Hebrew Word: {0:s}", part);
                        int e = part.IndexOf("=");
                        if (e >= 0)
                        {
                            int p = part.IndexOf("+", e);
                            if (p > 0)
                                hebrew = part.Substring(e + 1, p - e - 1);
                        }
                        break;

                    case 3:
                        // English word
                        englishWord = part.Trim();
                        //Console.WriteLine(englishWord);
                        break;

                    case 5:
                        // strong
                        string refr = lineParts[1];
                        int slashes = part.Split('/').Length - 1;
                        int idx1 = part.IndexOf('{');
                        if (idx1 == -1)
                        {
                            idx1 = part.IndexOf('«');
                            if (idx1 == -1)
                            {
                                //Console.WriteLine("++++++++++++++++++++++++ could not find '«' or '{': {0:s}", part);
                            }
                        }
                        int idx2 = part.IndexOf('}');
                        if (idx2 == -1)
                        {
                            idx2 = part.IndexOf('»');
                            if (idx2 == -1)
                            {
                                //Console.WriteLine("++++++++++++++++++++++++ could not find '»' or '}': {0:s}", part);
                                idx2 = part.Length - 1;
                            }
                        }
                        // H0935G|H0935G«H0935#4=?????=: come»to come (in)|1_come||go_in
                        // H3068G|H0430G«H0430#4=אֱלֹהִים=God»LORD@Gen.1.1-Heb
                        string strong = part.Substring(idx1 + 1, idx2 - idx1 - 1);
                        STATE state = STATE.START;
                        if (strong.IndexOf("#4=") >0 || strong.IndexOf("#2=") > 0)
                        {
                            string localStrong = string.Empty;
                            string localHebrew = string.Empty;
                            for (int c = 0; c < strong.Length; c++)
                            {
                                switch (state)
                                {
                                    case STATE.START:
                                        if (strong[c] == 'H')
                                        {
                                            localStrong = string.Empty;
                                            state = STATE.STRONG;
                                        }
                                        else if (strong[c] == '=')
                                        {
                                            localHebrew = string.Empty;
                                            state = STATE.HEBREW;
                                        }
                                        break;

                                    case STATE.STRONG:
                                        if (char.IsDigit(strong[c]))
                                            localStrong += strong[c];
                                        else
                                        {
                                            //Console.WriteLine("Strong = {0:s}", localStrong);
                                            if (!strongList.Contains(localStrong) && !string.IsNullOrEmpty(localStrong))
                                                strongList.Add(localStrong);
                                            localStrong = string.Empty;
                                            state = STATE.START;
                                        }
                                        break;

                                    case STATE.HEBREW:
                                        if (strong[c] != '=')
                                            localHebrew += strong[c];
                                        else
                                        {
                                            //Console.WriteLine("Hebrew = {0:s}", localHebrew);
                                            hebrew = localHebrew;
                                            localHebrew = string.Empty;
                                            state = STATE.START;
                                        }
                                        break;

                                }
                            }
                        }
                        else
                        {
                            strong = "";
                        }
                        //Console.WriteLine(strong);
                        break;

                    case 13:
                        transliteration = part.Trim();
                        //Console.WriteLine(part);
                        break;

                    default:
                        break;
                }
            }

            strongRefs = strongList.ToArray();


            try
            {

                verseWords.Add(wordNumber, new VerseWord(hebrew, englishWord, strongRefs, transliteration));
            }
            catch (Exception ex)
            {
                string e = ex.Message;
            }
        }


    }
}
