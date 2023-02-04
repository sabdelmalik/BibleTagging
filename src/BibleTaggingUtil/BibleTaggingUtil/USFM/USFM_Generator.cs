using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

using BibleTaggingUtil;

namespace SM.Bible.Formats.USFM
{
    public class USFM_Generator
    {
        Dictionary<string, Dictionary<int, USFM_Entry>> bible = new Dictionary<string, Dictionary<int, USFM_Entry>>();
        Dictionary<string, EntryMap> verseMap = new Dictionary<string, EntryMap>();
        string bibleVplFile = string.Empty;
        BibleTaggingForm parent;

        Dictionary<string, string> usfmConf;
        string taggedFolder = string.Empty;
        string VersionTitle = string.Empty;
        string usfmRefFolder = string.Empty;

        public USFM_Generator(BibleTaggingForm parent, ConfigurationHolder config )
        {
            this.parent = parent;

            this.usfmConf = config.USFM;
        }


        //****
        string currentFileName = string.Empty;
        string currentBook = string.Empty;
        int currentChapter = 0;

        int lineIndex = 0;
        //****
         public void Generate()
        {
            ReadUSFM();

            foreach (string key in parent.TargetVersionUpdates.Keys)
            {
                string reference = key;
                int idx = reference.IndexOf(' ');
                string altName = reference.Substring(0, idx);
                string usbName = string.Empty;
                int bookIndex = Array.IndexOf(Constants.osisAltNames, altName);
                if (Constants.osisAltNames.Contains(altName))
                    usbName = Constants.ubsNames[bookIndex];
                else
                {
                    Console.WriteLine("Error parsing " +  key);
                }

                reference = reference.Replace(altName, usbName).ToUpper();

                string taggedText = parent.TargetVersionUpdates[key];
                EntryMap map = verseMap[reference];
                USFM_Entry entry = bible[usbName.ToUpper()][map.Index];
                string usfmText = entry.Text;

                entry.Text = MergeVerse(taggedText, usfmText, bookIndex);

            }
            SaveUSFMBible();
        }

        private string MergeVerse(string taggedVerse, string usfm, int bookIndex)
        {
            string result = string.Empty;

            string[] verseParts = taggedVerse.Trim().Split(' ');
            // count verse words
            List<string> words = new List<string>();
            List<string> tags = new List<string>();
            string tempWord = string.Empty;
            string tmpTag = string.Empty;
            for (int i = 0; i < verseParts.Length; i++)
            {
                if (!string.IsNullOrEmpty(verseParts[i]) && verseParts[i][0] != '<' && verseParts[i][0] != '(')
                {
                    if (!string.IsNullOrEmpty(tmpTag))
                    {
                        if (tmpTag == "<>")
                            tmpTag = string.Empty;
                        tags.Add(tmpTag);
                    }
                    tmpTag = string.Empty;
                    tempWord += (string.IsNullOrEmpty(tempWord)) ? verseParts[i].Replace(":«", ": «") : (" " + verseParts[i].Replace(":«", ": «"));
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


            // \v 1 \w فِي الْبَدْءِ|strong="H7225"\w* \w خَلَقَ|strong="H1254"\w* \w اللهُ|strong="H0430"\w* \w السَّمَاوَاتِ|strong="H8064"\w* \w وَالأَرْضَ|strong="H0776"\w*. 
            for (int i = 0; i < words.Count; i++)
            {
                if (i >= tags.Count || string.IsNullOrEmpty(tags[i]))
                    result += string.Format(" \\w {0}\\w*", words[i]);
                else
                    result += string.Format(" \\w {0}|strong=\"{2}{1}\"\\w*", words[i], tags[i].Replace("<","").Replace(">",""), (bookIndex > 38) ? "G" : "H");
            }
    
            return result.Trim();
        }

        private void ReadUSFM()
        {
            string usfmRefFolder = Path.Combine(BibleTaggingUtil.Properties.Settings.Default.BiblesFolder, usfmConf["usfmRefFolder"]);
            string[] files = Directory.GetFiles(usfmRefFolder, "*.usfm");
            foreach (string file in files)
            {
                ParseBook(file);
            }
        }

        private void ParseBook(string file)
        {
            currentFileName = Path.GetFileName(file);
            Dictionary<int, USFM_Entry> book = new Dictionary<int, USFM_Entry>();

            using (StreamReader sr = new StreamReader(file))
            {
                while (!sr.EndOfStream)
                {
                    string? line = sr.ReadLine();
                    if (line is not null)
                        ParseLine(line.Trim(), book);
                }

              if (!string.IsNullOrEmpty(currentBook) && !bible.ContainsKey(currentBook))
                    bible[currentBook] = book;
            }
        }

        private void ParseLine(string line, Dictionary<int, USFM_Entry> book)
        {
            if (string.IsNullOrEmpty(line))
                return;
            USFM_Entry entry = new USFM_Entry();

            string marker = line;
            int space1 = line.IndexOf(' ');
            if (space1 > -1)
            {
                marker = line.Substring(0, space1);
            }
            entry.Marker = marker;
            int space2 = -1;
            switch (marker)
            {
                case @"\id":
                    if (line.Length > space1 + 1)
                    {
                        string bookLine = line.Substring(space1 + 1).Trim();
                        currentBook = bookLine;
                        string otherBookInfo = string.Empty;
                        space2 = bookLine.IndexOf(' ');
                        if (space2 > -1)
                        {
                            currentBook = bookLine.Substring(0, space2);
                            otherBookInfo = bookLine.Substring(space2 + 1).Trim();
                        }
                        else
                        {
                            otherBookInfo = string.Format("{0}, {1}", currentFileName, VersionTitle);
                        }
                        entry.Code = currentBook;
                        entry.Text = otherBookInfo;
                    }
                    break;

                case @"\usfm":
                case @"\ide":
                case @"\h":
                case @"\p":
                    if (space1 >= 0 && line.Length > space1 + 1)
                        entry.Text = line.Substring(space1 + 1);
                    break;
                case @"\c":
                    if (line.Length > 3)
                    {
                        int.TryParse(line.Substring(3), out currentChapter);
                        entry.Number = currentChapter;
                    }
                    break;
                case @"\v":
                    space2 = line.IndexOf(' ', 3);
                    if (space2 > -1)
                    {
                        int verseNum = 0;
                        bool success = int.TryParse(line.Substring(3, space2 - 3), out verseNum);
                        entry.Number = verseNum;
                        entry.Text = line.Substring(space2 + 1);
                        string verseRef = string.Format("{0} {1}:{2}", currentBook, currentChapter, verseNum);
                        EntryMap map = new EntryMap();
                        map.Book = currentBook;
                        map.Index = lineIndex;
                        verseMap[verseRef] = map;
                    }
                    break;
                default:
                    if (marker.StartsWith(@"\toc") ||
                        marker.StartsWith(@"\mt") ||
                        marker.StartsWith(@"\s"))
                    {
                        if (line.Length > space1 + 1)
                            entry.Text = line.Substring(space1 + 1);
                    }
                    break;
            }

            book[lineIndex++] = entry;
        }

        public void SaveUSFMBible()
        {
            string usfmOutFolder = Path.Combine(BibleTaggingUtil.Properties.Settings.Default.BiblesFolder, usfmConf["usfmOutFolder"]);
            if (!Directory.Exists(usfmOutFolder))
            {
                Directory.CreateDirectory(usfmOutFolder);
            }

            foreach(Dictionary<int, USFM_Entry> book in bible.Values)
            {
                string fileName = string.Empty;

                List<USFM_Entry> entries = book.Values.ToList();

                USFM_Entry id = entries[0];
                TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
                string code = textInfo.ToTitleCase(id.Code.ToLower());
                  
                if (Constants.ubsNames.Contains(code))
                {
                    string fileNamePrefix = Constants.usfmFileNamePrefixes[Array.IndexOf(Constants.ubsNames, code)];
                    fileName = string.Format("{0}{1}-{2}.{3}",
                                        fileNamePrefix.ToUpper(),
                                        usfmConf["usfmLang"].ToLower(),
                                        usfmConf["usfmId"].ToLower(),
                                        usfmConf["fileExtension"]);
                }

                id.Text = fileName + ", " + usfmConf["versionTitle"];

                if(string.IsNullOrEmpty(fileName))
                {
                    Console.WriteLine("Could not identify file name: id = " + id.Code);
                    continue;
                }

                string filePath = Path.Combine(usfmOutFolder, fileName);
                if (File.Exists(filePath))
                    File.Delete(filePath);

                using (StreamWriter sw = new StreamWriter(new FileStream(filePath, FileMode.Create), Encoding.UTF8))
                {
                    for(int idx = 0; idx < entries.Count; idx++)
                    {
                        string line = entries[idx].Marker;
                        if (entries[idx].HasNumber)
                            line += (" " + entries[idx].Number.ToString());
                        if (entries[idx].HasCode)
                            line += (" " + entries[idx].Code);
                        if (entries[idx].HasText)
                            line += (" " + entries[idx].Text);

                        sw.WriteLine(line);
                    }

                }
             }
        }
    }
}

