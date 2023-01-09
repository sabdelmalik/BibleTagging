using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace GenerateOSIS
{
    internal class OSIS_Generator
    {
        string sourceFolder = string.Empty;

        Dictionary<string, string> osisConf = new Dictionary<string, string>();


        public OSIS_Generator(string sourceFolder)
        {
            this.sourceFolder = sourceFolder;
        }

        
        /// <summary>");
        /// 
        /// </summary>");
        /// <param name="vplFile">");Verse perline file</param>");
        public void Generate(string confFile)
        {
            string bibleVplFile = string.Empty;
            string otVplFile = string.Empty;
            string ntVplFile = string.Empty;
            string outputFile = string.Empty;

            try
            {
                ReadConfiguration(confFile);

                if (osisConf.ContainsKey(Constants.bible_vpl_file))
                    bibleVplFile = osisConf[Constants.bible_vpl_file];

                if (osisConf.ContainsKey(Constants.ot_vpl_file))
                    otVplFile = osisConf[Constants.ot_vpl_file];

                if (osisConf.ContainsKey(Constants.nt_vpl_file))
                    ntVplFile = osisConf[Constants.nt_vpl_file];

                if (osisConf.ContainsKey(Constants.output_file))
                    outputFile = osisConf[Constants.output_file];

                if (string.IsNullOrEmpty(outputFile))
                    throw new Exception("Configuration must contain an entry for output-file");

                if (string.IsNullOrEmpty(otVplFile) && string.IsNullOrEmpty(ntVplFile))
                    throw new Exception("Configuration must contain atleast one entry of ot-vpl-file and nt-vpl-file");

                if(!string.IsNullOrEmpty(bibleVplFile))
                {
                    // we have a complete bible file that we need split into ot and nt
                    if(string.IsNullOrEmpty(otVplFile) || string.IsNullOrEmpty(ntVplFile))
                        throw new Exception("Configuration must contain ot-vpl-file and nt-vpl-file defined because bible-vpl=file is defined");
                    string otFilePath = Path.Combine(sourceFolder, otVplFile);
                    if (File.Exists(otFilePath))
                    {
                        string buFileName = string.Format("{0}_{1}.txt",Path.GetFileNameWithoutExtension(otVplFile), DateTime.Now.ToString("yyyy_MM_dd_HH_mm"));
                        File.Move(otFilePath, Path.Combine(sourceFolder, buFileName)); 
                    }
                    string ntFilePath = Path.Combine(sourceFolder, ntVplFile);
                    if (File.Exists(ntFilePath))
                    {
                        string buFileName = string.Format("{0}_{1}.txt", Path.GetFileNameWithoutExtension(ntVplFile), DateTime.Now.ToString("yyyy_MM_dd_HH_mm"));
                        File.Move(ntFilePath, Path.Combine(sourceFolder, buFileName));
                    }
                    
                    using(StreamReader sr = new StreamReader(Path.Combine(sourceFolder, bibleVplFile)))
                    using(StreamWriter otWriter = new StreamWriter(otFilePath))
                        using(StreamWriter ntWriter = new StreamWriter(ntFilePath))
                    {
                        while(!sr.EndOfStream)
                        {
                            string line = sr.ReadLine();
                            int space = line.IndexOf(' ');
                            string bookName = line.Substring(0, space);
                            int bookIndex = Array.IndexOf(Constants.osisAltNames, bookName);
                            if(bookIndex < 0)
                            {
                                throw new Exception("book name not found");
                            }
                            if(bookIndex > 38)
                            {
                                ntWriter.WriteLine(line);
                            }
                            else
                            {
                                otWriter.WriteLine(line);
                            }


                        }

                    }

                }

                string osisFile = Path.Combine(sourceFolder, outputFile);
                if (File.Exists(osisFile))
                    File.Delete(osisFile);

                using (StreamWriter sw = new StreamWriter(osisFile))
                {
                    sw.WriteLine("<?xml version='1.0' encoding='UTF-8'?>");
                    WriteOsisStartTag(sw);
                    WriteOsisTextStartTag(sw);

                    WriteHeader(sw);

                    if (!string.IsNullOrEmpty(otVplFile))
                    {
                        using (StreamReader sr = new StreamReader(Path.Combine(sourceFolder, otVplFile)))
                        {

                            WriteBible(sr, sw, "H");
                        }
                    }


                    if (!string.IsNullOrEmpty(ntVplFile))
                    {
                        using (StreamReader sr = new StreamReader(Path.Combine(sourceFolder, ntVplFile)))
                        {

                            WriteBible(sr, sw, "G");
                        }
                    }

                    WriteOsisTextEndTag(sw);
                    WriteOsisEndTag(sw);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }

        private void WriteBible(StreamReader sr, StreamWriter sw, string strongPrefix)
        {
            sw.WriteLine("<div type=\"bookGroup\">");

            WriteBibleBooks(sr, sw, strongPrefix);

            sw.WriteLine("</div>");

            currentBook = string.Empty;
            currentChapter = string.Empty;
        }

        string currentBook = string.Empty;
        string currentChapter = string.Empty;
        private void WriteBibleBooks(StreamReader sr, StreamWriter sw, string strongPrefix)
        {
            try
            {
                while (!sr.EndOfStream)
                {
                    string line = sr.ReadLine();
                    string bookName = line.Substring(0, 3);
                    string bookOsisName = Constants.osisNames[Array.IndexOf(Constants.osisAltNames, bookName)];
                    int idx1 = line.IndexOf(' ');
                    int idx2 = line.IndexOf(':', idx1 + 1);
                    int idx3 = line.IndexOf(' ', idx2 + 1);
                    string chapter = line.Substring(idx1 + 1, idx2 - idx1 - 1);
                    string verse = line.Substring(idx2 + 1, idx3 - idx2 - 1);
                    string verseText = line.Substring(idx3 + 1);

                    if (currentBook != bookOsisName)
                    {
                        // a new book


                        if (!string.IsNullOrEmpty(currentBook))
                        {
                            if (!string.IsNullOrEmpty(currentChapter))
                            {
                                // we are at the end of a chapter
                                sw.WriteLine("</chapter>");
                                currentChapter = string.Empty;
                            }
                            // and we are at the end of a book
                            if (strongPrefix == "G")
                            {
                                string colophonStart = "<div type=\"colophon\">";
                                if (currentBook == "Rom")
                                {
                                    sw.WriteLine(colophonStart);
                                    sw.WriteLine("كُتِبَتْ إِلَى أَهْلِ رُومِيَةَ مِنْ كُورِنْثُوسَ عَلَى يَدِ فِيبِي خَادِمَةِكَنِيسَةِكَنْخَرِيَا");
                                    sw.WriteLine("</div>");
                                }
                                else if (currentBook == "Eph")
                                {
                                    sw.WriteLine(colophonStart);
                                    sw.WriteLine("كُتِبَتْ إِلَى أَهْلِ أَفَسُسَ مِنْ رُومِيَةَ عَلَى يَدِ تِيخِيكُسَ");
                                    sw.WriteLine("</div>");
                                }
                                else if (currentBook == "Phil")
                                {
                                    sw.WriteLine(colophonStart);
                                    sw.WriteLine("كُتِبَتْ إِلَى أَهْلِ فِيلِبِّي مِنْ رُومِيَةَ عَلَى يَدِ أَبَفْرُودِتُسَ");
                                    sw.WriteLine("</div>");
                                }
                                else if (currentBook == "Col")
                                {
                                    sw.WriteLine(colophonStart);
                                    sw.WriteLine("كُتِبَتْ إِلَى أَهْلِ كُولُوسِّي مِنْ رُومِيَةَ بِيَدِ تِيخِيكُسَ وأُنِسِيمُسَ");
                                    sw.WriteLine("</div>");
                                }
                                else if (currentBook == "Phlm")
                                {
                                    sw.WriteLine(colophonStart);
                                    sw.WriteLine("إِلَى فِلِيمُونَ كُتِبَتْ مِنْ رُومِيَةَ عَلَى يَدِ أُنِسِيمُسَ اٌلْخَادِمِ");
                                    sw.WriteLine("</div>");
                                }
                            }
                            sw.WriteLine("</div>");
                        }
                        currentBook = bookOsisName;
                        // start tag for the new book
                        sw.WriteLine(string.Format("<div type='book' osisID='{0}'>", currentBook));
                    }

                    if (currentChapter != chapter)
                    {
                        // this is a new chapter
                        if (!string.IsNullOrEmpty(currentChapter))
                        {
                            // we are at the end of a chapter
                            sw.WriteLine("</chapter>");
                        }
                        currentChapter = chapter;
                        // start tag for the new chapter
                        sw.WriteLine(string.Format("<chapter osisID='{0}.{1}'>", currentBook, currentChapter));
                    }
                    sw.WriteLine(string.Format("<verse osisID='{0}.{1}.{2}'>{3}</verse>", currentBook, currentChapter, verse, GetTaggedVerse(verseText, strongPrefix)));
                }
                // end tags for the lastchapter / last book
                sw.WriteLine("</chapter>");
                sw.WriteLine("</div>");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }

        }

        private string GetTaggedVerse(string verseText, string strongPrefix)
        {
            string result = string.Empty;

            string text = verseText.Replace("  ", " ").Replace("  ", " ").Replace("  ", " ").Trim();
            string[] verseParts = text.Split(' ');
            string word = string.Empty;
            string tag = string.Empty;
            List<string> words = new List<string>();

            try
            {
                for (int i = 0; i < verseParts.Length; i++)
                {
                     string part = verseParts[i].Trim();
                    if (part[0] == '<')
                    {
                        try
                        {
                            // this is the tag
                            int idx = part.IndexOf('>');
                            tag = part.Substring(1, idx - 1);
                            if (string.IsNullOrEmpty(tag) || tag.Contains("?"))
                                tag = "?";
                            else
                            {
                                switch (tag.Length)
                                {
                                    case 1: tag += "000" + tag; break;
                                    case 2: tag += "00" + tag; break;
                                    case 3: tag += "0" + tag; break;
                                }
                            }
                        }
                        catch(Exception ex)
                        {
                            int x = 0;
                        }


                    }
                    else
                    {
                        // this is a new word
                        if (!string.IsNullOrEmpty(tag))
                        {
                            // this is a new word
                            if (tag.Length < 4)
                                words.Add(string.Format("<w>{0}</w>", word));
                            else
                                words.Add(string.Format("<w lemma=\"strong:{0}{1}\">{2}</w>", strongPrefix, tag, word));
                            word = string.Empty;
                            tag = string.Empty;
                        }
                        word += (string.IsNullOrEmpty(word) ? part : " " + part);
                    }
                }
                if (!string.IsNullOrEmpty(word))
                {
                    if (tag.Length < 4)
                        words.Add(string.Format("<w>{0}</w>", word));
                    else
                        words.Add(string.Format("<w lemma=\"strong:{0}{1}\">{2}</w>", strongPrefix,tag, word));
                }

                for (int i = 0; i < words.Count; i++)
                {
                    result += (string.IsNullOrEmpty(result)? words[i] : " " + words[i]);
                }

                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }

        private void WriteOsisStartTag(StreamWriter sw)
        {
            sw.WriteLine("<osis xmlns=\"http://www.bibletechnologies.net/2003/OSIS/namespace\"");
            sw.WriteLine("xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\"");
            sw.WriteLine("xsi:schemaLocation=\"http://www.bibletechnologies.net/2003/OSIS/namespace http://www.bibletechnologies.net/osisCore.2.1.1.xsd\">");
        }

        private void WriteOsisEndTag(StreamWriter sw)
        {
            sw.WriteLine("</osis>");
        }

        private void WriteOsisTextStartTag(StreamWriter sw)
        {
            try
            {
                sw.WriteLine(string.Format("<osisText osisIDWork=\"{0}\" osisRefWork=\"{1}\" xml:lang=\"{2}\" canonical=\"true\">",
                    osisConf[Constants.osisIDWork],
                    osisConf[Constants.osisRefWork],
                    osisConf[Constants.language]));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }
        private void WriteOsisTextEndTag(StreamWriter sw)
        {
            sw.WriteLine("</osisText>");
        }


        private void WriteHeader(StreamWriter sw)
        {
            try
            {
                sw.WriteLine(String.Format("<header>"));

                sw.WriteLine("<revisionDesc>");
                sw.WriteLine(String.Format("<date>{0}</date>", DateTime.Now.ToString("yyyy-MM-dd")));
                sw.WriteLine("<p>initial OSIS 2.1.1 version</p>");
                sw.WriteLine("</revisionDesc>");

                sw.WriteLine(String.Format("<work osisWork=\"{0}\">", osisConf[Constants.osisIDWork]));
                // title
                if(osisConf.ContainsKey(Constants.title))
                    sw.WriteLine(String.Format("<title>{0}</title>", osisConf[Constants.title]));
                // contributor
                if (osisConf.ContainsKey(Constants.contributor_role))
                    if(osisConf.ContainsKey(Constants.contributor_name))
                        sw.WriteLine(String.Format("<contributor role=\"{0}\">{1}</contributor>", osisConf[Constants.contributor_role], osisConf[Constants.contributor_name]));
                    else
                        sw.WriteLine(String.Format("<contributor>{0}</contributor>", osisConf[Constants.contributor_name]));
                // creator
                if (osisConf.ContainsKey(Constants.creator_role))
                    if(osisConf.ContainsKey(Constants.creator_name))
                        sw.WriteLine(String.Format("<creator role=\"{0}\">{1}</creator>", osisConf[Constants.creator_name], osisConf[Constants.creator_name]));
                    else
                        sw.WriteLine(String.Format("<creator role=\"{0}\">{1}</creator>", osisConf[Constants.creator_name], osisConf[Constants.creator_name]));
                // subject
                if (osisConf.ContainsKey(Constants.subject))
                    sw.WriteLine(String.Format("<subject>{0}</subject>", osisConf[Constants.subject]));
                // date
                sw.WriteLine(String.Format("<date>{0}</date>", DateTime.Now.ToString("yyyy-MM-dd")));
                // type
                if (osisConf.ContainsKey(Constants.type))
                    sw.WriteLine(String.Format("<type type=\"OSIS\">{0}</type>", osisConf[Constants.type]));
                // identifier
                if (osisConf.ContainsKey(Constants.identifier))
                    sw.WriteLine(String.Format("<identifier type=\"OSIS\">{0}</identifier>", osisConf[Constants.identifier]));
                // description
                sw.WriteLine(String.Format("<description>This work adds Strong's references to Smith Van Dyck Arabic Bible.</description>"));
                // language
                if(osisConf.ContainsKey(Constants.language) && osisConf.ContainsKey(Constants.language_type))
                    sw.WriteLine(String.Format("<language type=\"{0}\">{1}</language>", osisConf[Constants.language_type], osisConf[Constants.language]));
                // rights
                if(osisConf.ContainsKey(Constants.rights))
                    sw.WriteLine(String.Format("<rights type=\"x-copyright\">{0}</rights>", osisConf[Constants.rights]));
                // refSystem
                if(osisConf.ContainsKey(Constants.refSystem))
                    sw.WriteLine(String.Format("<refSystem>{0}</refSystem>", osisConf[Constants.refSystem]));

                sw.WriteLine(String.Format("</work>"));

                sw.WriteLine(String.Format("</header>"));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }

        /// <summary>");
        /// 
        /// </summary>");
        /// <param name="osisConfFile">");</param>");
        private void ReadConfiguration(string osisConfFile)
        {
            try
            {
                string confPath = Path.Combine(sourceFolder, osisConfFile);
                if (File.Exists(confPath))
                {
                    using (StreamReader sr = new StreamReader(confPath))
                    {
                        while (!sr.EndOfStream)
                        {
                            string line = sr.ReadLine();
                            string[] lineParts = line.Split('=');
                            osisConf[lineParts[0]] = lineParts[1];
                        }
                    }
                }
                else
                {
                    throw new FileLoadException(string.Format("{0} Does not exixt", confPath));
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }

    }
}
