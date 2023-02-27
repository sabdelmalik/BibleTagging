using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Eventing.Reader;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace BibleTaggingUtil.BibleVersions
{
    public class ReferenceVersionTOTHT : BibleVersion
    {

        public ReferenceVersionTOTHT(BibleTaggingForm container) : base(container) { }

        /// <summary>
        /// Verse words dictionary
        /// key: the word number in the verse
        /// Value: a populated VerseWord instance
        /// </summary>
        private Verse verseWords = null;

        /// <summary>
        /// Bible Dictionary
        /// Key: verse reference (xxx c:v) xxx = book name, c = chapter number, v = verse number
        /// </summary>
        // private Dictionary<string, Dictionary<int, VerseWord>> bible = new Dictionary<string, Dictionary<int, VerseWord>>();

        string textFilePath = string.Empty;

        
/*        public bool LoadBibleFile(string textFilePath)
        {
            bool result = false;
            if (File.Exists(textFilePath))
            {
                result = true;
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
            return result;
        }
*/
        enum STATE
        {
            START,
            STRONG,
            HEBREW,
            ENGLISH
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
                        Verse words = bible[verRef];
                        VerseWord vw = words[0];
                        string verse = vw.Word + " [" + vw.Hebrew + "]";
                        for (int j = 0; j < vw.Strong.Length; j++)
                        {
                            verse += (" <" + vw.Strong[j]) + ">";
                        }
                        for (int k = 1; k < words.Count; k++)
                        {
                            verse += " " + (words[k].Word + " [" + words[k].Hebrew + "]");
                            for (int j = 0; j < words[k].Strong.Length; j++)
                            {
                                verse += (" <" + words[k].Strong[j]) + ">";
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


                //for (int i = 0; i < targetVersion.Keys.Count; i++)
                //{
                //    string key = targetVersion.Keys.ToArray()[i];
                //    string line = string.Format("{0:s} {1:s}", key, targetVersion[key]); 
                //    outputFile.WriteLine(line); 
                //}

            }


        }

        string currentVerseRef = string.Empty;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="wordLine"></param>
        override protected void ParseLine(string wordLine)
        {
            #region Skip the header
            if (string.IsNullOrEmpty(wordLine))
                return;
            int cnt = wordLine.Count(f => (f == '\t'));
            if (cnt != 14)
            {
                return;
            }

            if (wordLine.StartsWith("Ref in") || wordLine.StartsWith("======"))
                return;
            #endregion Skip the header

            string[] lineParts = wordLine.Split('\t');
            string verseRef = string.Empty;

            // we only consider WLT words
            if (!lineParts[1].EndsWith("WLT"))
                return;

            // get verse reference
            string verRef = lineParts[1].Substring(0, lineParts[1].IndexOf(" : WLT"));
            string[] verRefParts = verRef.Split('.');
            string[] verNumParts = verRefParts[2].Split('#');
            int chapter = 0;
            int verse = 0;
            if (!int.TryParse(verRefParts[1], out chapter))
                Console.WriteLine("++++++++++++++++++++++++ Failed to parse chapter number: {0:s}", verRefParts[1]);
            if (!int.TryParse(verNumParts[0], out verse))
                Console.WriteLine("++++++++++++++++++++++++ Failed to parse verse number: {0:s}", verNumParts[0]);
//            if (!int.TryParse(verNumParts[1].Replace("w", "").Replace("p", "").Replace("+", ""), out wordNumber))
//                Console.WriteLine("++++++++++++++++++++++++ Failed to parse word number: {0:s}", verNumParts[1]);

            verseRef = string.Format("{0:s} {1:d}:{2:d}", verRefParts[0], chapter, verse);
            //Console.WriteLine(string.Format("{0:s} [{1:d}]", verseRef, wordNumber));

            if (string.IsNullOrEmpty(currentVerseRef))
            {
                // very firdst verse
                currentVerseRef = verseRef;
                verseWords = new Verse();
            }


            if (verseRef != currentVerseRef)
            {
                bible.Add(currentVerseRef, verseWords);
                currentVerseRef = verseRef;
                verseWords = new Verse();
            }

            // 1. get Hebrew word parts
            string hebrewWord = string.Empty;
            int equalSign = lineParts[2].IndexOf('=');
            if (equalSign != -1)
            {
                hebrewWord = lineParts[2].Substring(0, equalSign);
            }
            string[] hebrewWordParts = hebrewWord.Split('/');
            string[] extendedStrongParts = lineParts[5].Split('/');

            for (int i = 0; i < extendedStrongParts.Length; i++)
            {
                string englishWord = string.Empty;
                string[] strongRefs = null;
                string hebrew = string.Empty;
                string transliteration = string.Empty;
                List<string> strongList = new List<string>();
                string part = string.Empty;
                try
                {
                    hebrew = hebrewWordParts[i];
                    part = extendedStrongParts[i];
                }
                catch (Exception ex)
                {
                    string s = ex.Message;
                }


                //string refr = lineParts[1];
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
                string strong = string.Empty;
                if (idx1 == -1 || idx2 == -1)
                    strong = part;
                else
                {
                    strong = part.Substring(idx1 + 1, idx2 - idx1 - 1);
                    transliteration = lineParts[13];
                }

                STATE state = STATE.START;
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
                                if (strong[c] == '=')
                                    state = STATE.HEBREW;
                                else
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
                                state = STATE.ENGLISH;
                            }
                            break;

                        case STATE.ENGLISH:
                            if (strong[c] == '»')
                            {
                                c = strong.Length; // exit the for loop
                                break;
                            }
                            else
                            {
                                englishWord += strong[c];
                            }
                            break;

                    }
                }
                strongRefs = strongList.ToArray();
                try
                {
                    int wordNumber = verseWords.Count;

                    verseWords[wordNumber] = new VerseWord(hebrew, englishWord, strongRefs, transliteration, currentVerseRef);
                }
                catch (Exception ex)
                {
                    string e = ex.Message;
                }

            }

        }


    }
}
        
