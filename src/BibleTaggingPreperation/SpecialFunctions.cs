using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Reflection.Metadata.Ecma335;
using System.Data;
using System.Diagnostics.Eventing.Reader;
using System.Diagnostics;

namespace BibleTagging
{
    public class SpecialFunctions
    {

        BibleTaggingPreperationForm container;
        List<string> bookNames;

        Dictionary<string, Dictionary<int, int>> otVersification = new Dictionary<string, Dictionary<int, int>>();
        Dictionary<string, Dictionary<int, int>> ntVersification = new Dictionary<string, Dictionary<int, int>>();


        string bookName = string.Empty;
        int bookIndex = -1;
        string chapter = string.Empty;
        string verseNumber = string.Empty;
        Dictionary<int, int> chaptersVerseCounts;
        int totalVerseCount = 0;
        int wrongVerseCount = 0;

        public SpecialFunctions() { }
        public SpecialFunctions(BibleTaggingPreperationForm container)
        {
            this.container = container;
        }

        public string? ConvertToUbsNames(string path)
        {
            bookNames = new List<string>();

            using (StreamReader sr = new StreamReader(path))
            {
                while (!sr.EndOfStream)
                {
                    string? line = sr.ReadLine();
                    if (line == null)
                    {
                        container.TraceError(MethodBase.GetCurrentMethod().Name, "ReadLine returned null");
                        return null;
                    }
                    else if (line.Trim() == string.Empty)
                    {
                        continue;
                    }
                    else
                    {
                        line = line.Trim().Replace("�", "").Replace("  ", " ");
                        int b = line.IndexOf(" ");
                        if (b > 0)
                        {
                            int c = line.IndexOf(":", b + 1);
                            if (c > 0 && (c - b) < 5)
                                bookName = line.Substring(0, b);
                            if (!bookNames.Contains(bookName))
                            {
                                bookNames.Add(bookName);
                            }
                        }
                    } 
                }
            }
            if (bookNames.Count != 66)
            {
                container.TraceError(MethodBase.GetCurrentMethod().Name, "book counr is not 66");
                return null;
            }

            string outPath = Path.GetDirectoryName(path);
            outPath = Path.Combine(outPath, Path.GetFileNameWithoutExtension(path) + "_ubs.txt");
            bookName = string.Empty;
            chapter = string.Empty;
            verseNumber= string.Empty;
            chaptersVerseCounts = new Dictionary<int, int>();
            wrongVerseCount = 0;
            using (StreamWriter sw = new StreamWriter(outPath))
            using (StreamReader sr = new StreamReader(path))
            {
                while (!sr.EndOfStream)
                {
                    string? line = sr.ReadLine();
                    if (!string.IsNullOrEmpty(line))
                    {
                        string newLine = UpdateVerseLine(line);
                        if (!string.IsNullOrEmpty(newLine))
                        {
                            wrongVerseCount++;
                            if(newLine.StartsWith("Mat"))
                            {
                                int x = 0;
                            }
                            int z = newLine.IndexOf(":0");
                            bool ignore = false; 
                            if ( z>0 && z < 15)
                            {
                                ignore = true;
                                container.TraceError(MethodBase.GetCurrentMethod().Name, newLine);
                                wrongVerseCount--;
                            }  
                            else if (newLine.Length < 15)
                                container.Trace("Short Line: " + newLine, Color.Orange);
                            if (!ignore)
                                sw.WriteLine(newLine);
                        }
                    }

                }
            }
            return outPath;
        }

        private string UpdateVerseLine(string line)
        {
            string verse = string.Empty;
            string text = string.Empty;
            string newChapter = string.Empty;
            string newVerseNumber = string.Empty;

            if (line.Contains("Gam khempeuh aw, Topa tungah lungdamna aw gingsak un."))
            {
                int x = 0;
            }
            line = line.Trim().Replace("�", "").Replace("  ", " ").Replace("<br>", "");
            int b = line.IndexOf(" ");
            if (b > 0)
            {
                int c = line.IndexOf(":", b + 1);
                if (c > 0 && (c - b) < 5)
                {
                    string newBookName = line.Substring(0, b);
                    if(newBookName != bookName)
                    {
                        if(bookName != string.Empty && bookIndex != -1)
                        {
                            if(bookName == "2Co") 
                            {
                                int co = 0;
                            }
                            //  add last chapter
                            chaptersVerseCounts.Add(int.Parse(chapter), int.Parse(verseNumber));
                            totalVerseCount += int.Parse(verseNumber);
                            if (totalVerseCount != wrongVerseCount)
                            {
                                int z = 0;
                            }
                            try
                            {
                                if (bookIndex < 39)
                                    otVersification.Add(Constants.ubsNames[bookIndex], chaptersVerseCounts);
                                else
                                    ntVersification.Add(Constants.ubsNames[bookIndex], chaptersVerseCounts);
                            }
                            catch(Exception e)
                            {
                                container.TraceError(MethodBase.GetCurrentMethod().Name, e.Message);
                            }
                            chaptersVerseCounts = new Dictionary<int, int>();
                            chapter = string.Empty;
                            newVerseNumber = string.Empty;
                        }

                    }
                    bookName = newBookName;

                    newChapter = line.Substring(b + 1, c - (b + 1));
                    text = string.Empty;
                    int v = line.IndexOf(' ', c + 1);
                    if (v > 0)
                    {
                        newVerseNumber = line.Substring(c + 1, v - (c + 1));
                        text = line.Substring(v + 1);
                    }
                    else
                        newVerseNumber = line.Substring(c + 1);
                }
                else
                {
                    c = line.IndexOf(":");
                    if (c > 0 && c < 4)
                    {
                        newChapter = line.Substring(0, c);
                        int v = line.IndexOf(' ', c + 1);
                        if (v > 0)
                        {
                            newVerseNumber = line.Substring(c + 1, v - (c + 1));
                            text = line.Substring(v + 1);
                        }
                        else
                            newVerseNumber = line.Substring(c + 1);
                    }
                    else
                    {
                        newVerseNumber = line.Substring(0, b);
                        text = line.Substring(b + 1);
                    }
                }

                bookIndex = Array.IndexOf(bookNames.ToArray(), bookName);
                if (bookIndex == -1 || bookIndex > Constants.ubsNames.Length)
                {
                    container.TraceError(MethodBase.GetCurrentMethod().Name, "something wrong with book " + bookName);
                }
                else
                {
                    if (newChapter == string.Empty)
                        newChapter = chapter;

                    if(newChapter != chapter)
                    {
                        if(chapter != string.Empty)
                        {
                            // this is a new chapter
                            chaptersVerseCounts.Add(int.Parse(chapter), int.Parse(verseNumber));
                            totalVerseCount += int.Parse(verseNumber);
                            if (totalVerseCount != wrongVerseCount)
                            {
                                int z = 0;
                            }
                        }

                    }
                    chapter = newChapter;
                    verseNumber = newVerseNumber;
                    verse = string.Format("{0} {1}:{2} {3}",
                        Constants.ubsNames[bookIndex],
                        chapter, verseNumber, text);
                }
            }
            return verse;
        } 

        public void ReportMismatchOT()
        {
            int x = 0;
            foreach(string book in otVersification.Keys)
            {
                Dictionary<int, int> verseCounts = otVersification[book];
                int bookIndex = Array.IndexOf(Constants.ubsNames, book);
                // check KJV
                int[] refVerseCounts = Versification.KJV.LAST_VERSE_OT[bookIndex];
                foreach(int i in verseCounts.Keys)
                {
                    x+= verseCounts[i];
                    if (verseCounts[i] != refVerseCounts[i-1])
                    {
                        container.TraceError(MethodBase.GetCurrentMethod().Name,
                            string.Format("{0} {1} verses = {2}, KJV = {3}", book, i, verseCounts[i], refVerseCounts[i - 1]));
                    }
                }

                // check NRSV
                refVerseCounts = Versification.NRSV.LAST_VERSE_OT[bookIndex];
                foreach (int i in verseCounts.Keys)
                {
                    if (verseCounts[i] != refVerseCounts[i - 1])
                    {
                        container.TraceError(MethodBase.GetCurrentMethod().Name,
                            string.Format("{0} {1} verses = {2}, NRSV = {3}", book, i, verseCounts[i], refVerseCounts[i - 1]));
                    }
                }
            }
        }

        public void ReportMismatchNT()
        {
            foreach (string book in ntVersification.Keys)
            {
                Dictionary<int, int> verseCounts = ntVersification[book];
                int bookIndex = Array.IndexOf(Constants.ubsNames, book) - 39;
                // check KJV
                int[] refVerseCounts = Versification.KJV.LAST_VERSE_NT[bookIndex];
                foreach (int i in verseCounts.Keys)
                {
                    if (verseCounts[i] != refVerseCounts[i - 1])
                    {
                        container.TraceError(MethodBase.GetCurrentMethod().Name,
                            string.Format("{0} {1} verses = {2}, NRSV = {3}", book, i, verseCounts[i], refVerseCounts[i - 1]));
                    }
                }

                // check NRSV
                refVerseCounts = Versification.NRSV.LAST_VERSE_NT[bookIndex];
                foreach (int i in verseCounts.Keys)
                {
                    if (verseCounts[i] != refVerseCounts[i - 1])
                    {
                        container.TraceError(MethodBase.GetCurrentMethod().Name,
                            string.Format("{0} {1} verses = {2}, NRSV = {3}", book, i, verseCounts[i], refVerseCounts[i - 1]));
                    }
                }
            }
        }

    }
}

