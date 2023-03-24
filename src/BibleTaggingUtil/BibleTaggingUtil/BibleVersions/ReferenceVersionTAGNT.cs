using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static System.Windows.Forms.AxHost;

namespace BibleTaggingUtil.BibleVersions
{
    internal enum ParseState
    {
        Initial,
        HeaderFound,
        RefLineFound,
        WordFound,
        RefLineContFound,
        WordContFound,
        WordsHeaderFound

    }

    public class ReferenceVersionTAGNT : BibleVersion
    {
        /// <summary>
        /// Verse words dictionary
        /// key: the word number in the verse
        /// Value: a populated VerseWord instance
        /// </summary>
        private Verse verseWords = null;

        string textFilePath = string.Empty;

        ParseState pState = ParseState.Initial;

        public ReferenceVersionTAGNT(BibleTaggingForm container) : base(container) { }

        string verseReference = string.Empty;
        string verseRef = string.Empty;
        string bookName = string.Empty;

        int verseWordCount = 0;
        int strongsCount = 0;
        int wordsLineCounter = 0;
        List<string> strongList = new List<string>();

        override protected void ParseLine(string line)
        {
            try
            {
                line = line.Trim();
                switch (pState)
                {
                    case ParseState.Initial:
                        if (line.Contains("Reference + word\tWord #"))
                            pState = ParseState.HeaderFound;
                        break;
                    case ParseState.HeaderFound:
                        if (line.StartsWith("#"))
                        {
                            string[] lineparts = line.Substring(2).Split('\t');
                            verseReference = lineparts[0];
                            string[] refParts = verseReference.Split('.');
                            bookName = refParts[0];
                            //                        if (bookName == "Mrk")
                            //                            bookName = "Mar";
                            //                        else if (bookName == "Jhn")
                            //                            bookName = "Joh";
                            verseRef = string.Format("{0} {1}:{2}", bookName, refParts[1], refParts[2]);

                            verseWordCount = lineparts.Length - 1;
                            if (verseReference == "Tit.2.7")
                            {
                                int x = 0;
                            }
                            for (int i = 1; i < lineparts.Length; i++)
                            {
                                if (string.IsNullOrEmpty(lineparts[i].Trim(' ')) || char.IsAscii(lineparts[i].Trim(' ')[0]))
                                    verseWordCount--;
                            }
                            pState = ParseState.RefLineFound;
                        }
                        break;
                    case ParseState.RefLineFound:
                        if (line.StartsWith("#_Word=Grammar"))
                        {
                            verseWords = new Verse();

                            // strong Numbers
                            string[] lineparts = line.Substring(2).Split('\t');
                            strongsCount = lineparts.Length - 1;
                            for (int i = 1; i < lineparts.Length; i++)
                            {
                                if (!lineparts[i].Trim().StartsWith("G"))
                                    strongsCount--;
                                else
                                {
                                    string[] s = lineparts[i].Split('=');
                                    strongList.Add(s[0].Substring(1));
                                }
                            }

                            if (verseWordCount != strongsCount)
                            {
                                throw new Exception("word count mismatch");
                            }
                            pState = ParseState.WordFound;
                        }
                        break;
                    case ParseState.WordFound:
                        if (line.StartsWith("#_" + verseReference))
                        {
                            string[] lineparts = line.Substring(2).Split('\t');
                            verseWordCount += lineparts.Length - 1;
                            for (int i = 1; i < lineparts.Length; i++)
                            {
                                if (char.IsAscii(lineparts[i].Trim()[0]))
                                    verseWordCount--;
                            }
                            pState = ParseState.RefLineContFound;
                        }
                        else if (line.StartsWith("#_REFERENCE"))
                        {
                            pState = ParseState.WordsHeaderFound;
                        }
                        break;
                    case ParseState.RefLineContFound:
                        if (line.StartsWith("#_Word=Grammar"))
                        {
                            // strong Numbers
                            string[] lineparts = line.Substring(2).Split('\t');
                            strongsCount += lineparts.Length - 1;
                            for (int i = 1; i < lineparts.Length; i++)
                            {
                                if (!lineparts[i].StartsWith("G"))
                                    strongsCount--;
                                else
                                {
                                    string[] s = lineparts[i].Split('=');
                                    strongList.Add(s[0].Substring(1));
                                }
                            }
                            if (verseWordCount != strongsCount)
                            {
                                throw new Exception("word count mismatch");
                            }
                            pState = ParseState.WordFound;
                        }
                        break;
                    case ParseState.WordsHeaderFound:
                        if (string.IsNullOrEmpty(line))
                        {
                            if (verseWordCount != wordsLineCounter)
                            {
                                throw new Exception("word lines count mismatch");
                            }

                            string strongsline = strongList[0];
                            for (int i = 1; i < strongList.Count; i++)
                            {
                                strongsline += " " + strongList[i];
                            }

                            //sw.WriteLine(string.Format("{0} {1}:{2} {3}", bookName, refParts[1], refParts[2], strongsline));
                            bible[verseRef] = verseWords;
                            verseReference = string.Empty;
                            verseWordCount = 0;
                            strongsCount = 0;
                            wordsLineCounter = 0;
                            verseWords = null;
                            strongList.Clear();

                            pState = ParseState.HeaderFound;
                        }
                        else
                        {
                            string[] lineParts = line.Split('\t');
                            string strong = lineParts[4].Trim().Substring(1);
                            string[] strongA = new string[1];
                            strongA[0] = strong;
                            string greek = lineParts[2].Trim();
                            string english = lineParts[3].Trim();

                            verseWords[wordsLineCounter++] = new VerseWord(greek, english, strongA, string.Empty, verseRef);
                        }
                        break;
                }
            }
            catch(Exception ex)
            {
                Tracing.TraceException(MethodBase.GetCurrentMethod().Name, ex.Message);
            }
        }
    }
}
