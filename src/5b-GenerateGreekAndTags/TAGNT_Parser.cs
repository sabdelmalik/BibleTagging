using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BibleTagging
{

    public enum State
    {
        Initial,
        HeaderFound,
        RefLineFound,
        WordFound,
        RefLineContFound,
        WordContFound,
        WordsHeaderFound

    }
    public class TAGNT_Parser
    {

        State s = State.Initial;

        public void Parse(string path, StreamWriter sw)
        {
            using (StreamReader sr = new StreamReader(path))
            {
                string verseReference = string.Empty;
                int verseWordCount = 0;
                int strongsCount = 0;
                int wordsLineCounter = 0;
                List<string> strongList = new List<string>();
               while (!sr.EndOfStream)
                {
                     string line = sr.ReadLine().Trim();
                    switch (s)
                    {
                        case State.Initial:
                            if (line.Contains("Reference + word\tWord #"))
                                s = State.HeaderFound;
                            break;
                        case State.HeaderFound:
                            if (line.StartsWith("#"))
                            {
                                string[] lineparts = line.Substring(2).Split('\t');
                                verseReference = lineparts[0];
                                verseWordCount = lineparts.Length - 1;
                                if(verseReference == "Tit.2.7")
                                {
                                    int x = 0;
                                }
                                for(int i = 1; i < lineparts.Length; i++)
                                {
                                    if (char.IsAscii(lineparts[i].Trim()[0]))
                                        verseWordCount--;
                                }
                                s = State.RefLineFound;
                            }
                            break;
                        case State.RefLineFound:
                            if (line.StartsWith("#_Word=Grammar"))
                            {
                                // strong Numbers
                                string[] lineparts = line.Substring(2).Split('\t');
                                strongsCount = lineparts.Length - 1;
                                for(int i = 1; i < lineparts.Length; i++)
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
                                s = State.WordFound;
                            }
                            break;
                        case State.WordFound:
                            if(line.StartsWith("#_" + verseReference))
                            {
                                string[] lineparts = line.Substring(2).Split('\t');
                                verseWordCount += lineparts.Length - 1;
                                for (int i = 1; i < lineparts.Length; i++)
                                {
                                    if (char.IsAscii(lineparts[i].Trim()[0]))
                                        verseWordCount--;
                                }
                                s = State.RefLineContFound;
                            }
                            else if (line.StartsWith("#_REFERENCE"))
                            {
                                s = State.WordsHeaderFound;
                            }
                            break;
                        case State.RefLineContFound:
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
                                s = State.WordFound;
                            }
                            break;
                        case State.WordsHeaderFound:
                            if(string.IsNullOrEmpty(line))
                            {
                                if (verseWordCount != wordsLineCounter)
                                {
                                    throw new Exception("word lines count mismatch");
                                }

                                string strongsline = strongList[0];
                                for(int i = 1; i < strongList.Count; i++)
                                {
                                    strongsline += " " + strongList[i];
                                }
                                string[] refParts = verseReference.Split('.');
                                string bookName = refParts[0];
                                if (bookName == "Mrk")
                                    bookName = "Mar";
                                else if (bookName == "Jhn")
                                    bookName = "Joh";

                                sw.WriteLine(string.Format("{0} {1}:{2} {3}", bookName, refParts[1], refParts[2], strongsline));
                                verseReference = string.Empty;
                                verseWordCount = 0;
                                strongsCount = 0;
                                wordsLineCounter = 0;
                                strongList.Clear();

                                s = State.HeaderFound;
                            }
                            else
                            {
                                wordsLineCounter++;
                            }
                            break;
                    }

                }
            }
        }
    }
}
