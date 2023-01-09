using System.IO;
using System.Reflection.PortableExecutable;
using static System.Net.Mime.MediaTypeNames;

internal class Program
{
    public class Statistics
    {
        public int TotalWords { get; set; }
        public int UnmappedWords { get; set; }


    }
    private static void Main(string[] args)
    {
        Dictionary<string, Statistics> detailedStatistics = new Dictionary<string, Statistics>();
        Dictionary<string, Statistics> bookStatistics = new Dictionary<string, Statistics>();
        List<string> exceptions = new List<string>();

        string currentVerseReference = string.Empty;
        string currentChapterReference = string.Empty;
        string currentBook = string.Empty;
        Statistics chapterStats = null;
        Statistics bookStats = null;
        string unknown = "???";
        string lastBook = string.Empty; // "Exo";

        string sourceFolder = @"C:\Users\samim\Documents\MyProjects\STEP\AutoTagging\BibleFiles\SourcesFiles";
        string intermediateFolder = @"C:\Users\samim\Documents\MyProjects\STEP\AutoTagging\BibleFiles\IntermediateFiles";
        string outputfolder = @"C:\Users\samim\Documents\MyProjects\STEP\AutoTagging\BibleFiles\OutputFiles";
        
        string referencesFile = "nt_refrences.txt";
        string hebrewNoRefFile = "greekNtNoRef.txt";
        string arabicNoRefFile = "arabicNtNoRef.txt";

        string alignerTrainingFolder = @"C:\Users\samim\Documents\MyProjects\STEP\AutoTagging\Berkeley Word Aligner\BerkeleyAligner\berkeleyaligner\myNtDataset\train";
        string alignerTagsFile = "newtestament.g";

        string alignerAlignFolder = @"C:\Users\samim\Documents\MyProjects\STEP\AutoTagging\Berkeley Word Aligner\BerkeleyAligner\berkeleyaligner\ArabicNT_Map";
        string alignerAlignFile = "training.g-a.align";

        string mapFile = "nt_map.txt";
        string otTaggedText = "nt_tagged_text.txt";
        string mapExceptions = "nt_mapExceptions.txt";
        string statsFile = "nt_stats.txt";



        int bibleTotal = 0;
        int bibleUnmapped = 0;

        int offset = 0;

        string verse = String.Empty;

        using (StreamReader reader_Arabic = new StreamReader(Path.Combine(intermediateFolder, arabicNoRefFile)))
        using (StreamReader reader_Tags = new StreamReader(Path.Combine(alignerTrainingFolder, alignerTagsFile)))
        using (StreamReader reader_Hebrew = new StreamReader(Path.Combine(intermediateFolder, hebrewNoRefFile)))
        using (StreamReader reader_Align = new StreamReader(Path.Combine(alignerAlignFolder, alignerAlignFile)))
        using (StreamReader reader_Refs = new StreamReader(Path.Combine(intermediateFolder, referencesFile)))
        using (StreamWriter writer_mapFile = new StreamWriter(Path.Combine(outputfolder, mapFile)))
        using (StreamWriter writer_arabicTaggedFile = new StreamWriter(Path.Combine(outputfolder, otTaggedText)))
        {
            while (!reader_Arabic.EndOfStream && !reader_Tags.EndOfStream && !reader_Hebrew.EndOfStream && !reader_Align.EndOfStream && !reader_Refs.EndOfStream)
            {
                var arabicLine = reader_Arabic.ReadLine();
                var hebrewLine = reader_Hebrew.ReadLine();
                var tagsLine = reader_Tags.ReadLine();
                var alignerLine = reader_Align.ReadLine();
                var refrenceLine = reader_Refs.ReadLine();

                string verseRef = refrenceLine;
                string bookName = verseRef.Substring(0, 3);

                int idx = verseRef.IndexOf(':');
                if (idx != -1)
                {
                    string chapterRef = verseRef.Substring(0, idx);
                    if (currentVerseReference != verseRef)
                    {
                        if (string.IsNullOrEmpty(currentVerseReference))
                        {
                            // this is the very first verse
                            chapterStats = new Statistics();
                            bookStats = new Statistics();

                            currentChapterReference = chapterRef;
                            currentBook = bookName;
                            currentVerseReference = verseRef;
                            verse = currentVerseReference;
                            writer_mapFile.WriteLine(String.Format("====\t{0}\t====", currentVerseReference));
                        }
                        else
                        {
                            // We are changing Verse
                            // Are we changing chapter
                            if (currentChapterReference != chapterRef)
                            {
                                // we are changing chapter
                                // save the previous chapter
                                detailedStatistics.Add(currentChapterReference, chapterStats);
                                chapterStats = new Statistics();
                                currentChapterReference = chapterRef;
                                if (bookName != currentBook)
                                {
                                    bookStatistics.Add(currentBook, bookStats);
                                    bookStats = new Statistics();
                                    currentBook = bookName;
                                    if (currentBook == lastBook)
                                        break;
                                }
                            }
                            writer_arabicTaggedFile.WriteLine(verse);
                            currentVerseReference = verseRef;
                            verse = currentVerseReference;
                            writer_mapFile.WriteLine(String.Format("====\t{0}\t====", currentVerseReference));
                        }
                    }
                }

                string[] arabic_parts = arabicLine.Split(' ');
                string[] hebrew_parts = hebrewLine.Split(' ');
                string[] aligner_parts = alignerLine.Split(' ');
                string[] tag_parts = tagsLine.Split(' ');
                string[] translation = new string[arabic_parts.Length];
                string[] tagTranslation = new string[arabic_parts.Length];
                for (int i = 0; i < translation.Length; i++)
                {
                    translation[i] = unknown;
                    tagTranslation[i] = unknown;
                }
                for (int i = 0; i < aligner_parts.Length; i++)
                {
                    int ai, hi;
                    try
                    {
                        if (!string.IsNullOrEmpty(aligner_parts[i]) && (i + offset) < aligner_parts.Length)
                        {
                            string[] map = aligner_parts[i].Split('-');
                            ai = int.Parse(map[0].Trim());
                            hi = int.Parse(map[1].Trim());
                            translation[ai + offset] = hebrew_parts[hi + offset];
                            tagTranslation[ai + offset] = tag_parts[hi + offset];
                        }
                    }
                    catch (Exception ex)
                    {
                        //Console.WriteLine(ex.Message);
                    }
                }


                string outLine = string.Empty;
                for (int j = 0; j < arabic_parts.Length; j++)
                {
                    if (j >= translation.Length)
                    {
                        outLine = String.Format("{0}\t{1}\t{2}", arabic_parts[j], unknown, unknown);
                    }
                    else
                    {
                        string arabicWord = arabic_parts[j];
                        string hebrewWord = translation[j];
                        string tagWord = tagTranslation[j];
                        //if (translation[j] == "אֵת" || translation[j] == "וְאֵת" || j < 2)
                        //{
                        //    hebrewWord = string.Empty;
                        //    tagWord = string.Empty;
                        //}
                        outLine = String.Format("{0}\t{1}\t{2}", arabic_parts[j], translation[j], tagTranslation[j]);
                        verse += string.Format(" {0} <{1}>", arabicWord, tagWord);
                    }
                    writer_mapFile.WriteLine(outLine);
                    if (outLine.Contains(unknown))
                    {
                        if (!exceptions.Contains(outLine))
                            exceptions.Add(outLine);
                    }
                    try
                    {
                        chapterStats.TotalWords++;
                    }
                    catch(Exception ex)
                    {
                        Console.WriteLine(ex.ToString());  
                    }
                    bookStats.TotalWords++;
                    bibleTotal++;

                    if (outLine.Contains(unknown))
                    {
                        chapterStats.UnmappedWords++;
                        bookStats.UnmappedWords++;
                        bibleUnmapped++;
                    }
                }

            }
        }

        using (StreamWriter outputFileEx = new StreamWriter(Path.Combine(outputfolder, mapExceptions)))
        {
            for(int i = 0; i < exceptions.Count; i++)
            {
                outputFileEx.WriteLine(exceptions[i]);
            }
        }

        using (StreamWriter outputFile = new StreamWriter(Path.Combine(outputfolder, statsFile)))
        {
            outputFile.WriteLine("Total Stats");
            outputFile.WriteLine(string.Format("Total Bible words = {0}", bibleTotal));
            outputFile.WriteLine(string.Format("Total unmapped words = {0} ({1}%)", bibleUnmapped, (100 * bibleUnmapped / bibleTotal)));
            outputFile.WriteLine("");
            outputFile.WriteLine("Book Stats");
            foreach (string bookName in bookStatistics.Keys)
            {
                outputFile.WriteLine(string.Format("{0}: words = {1}, unmapped = {2} ({3}%)",
                    bookName,
                    bookStatistics[bookName].TotalWords,
                    bookStatistics[bookName].UnmappedWords,
                    (100 * bookStatistics[bookName].UnmappedWords / bookStatistics[bookName].TotalWords)));

            }
            outputFile.WriteLine("");
            outputFile.WriteLine("Chapter Stats");
            foreach (string chapter in detailedStatistics.Keys)
            {
                outputFile.WriteLine(string.Format("{0}: words = {1}, unmapped = {2} ({3}%)",
                    chapter,
                    detailedStatistics[chapter].TotalWords,
                    detailedStatistics[chapter].UnmappedWords,
                    (100 * detailedStatistics[chapter].UnmappedWords / detailedStatistics[chapter].TotalWords)));
            }
        }
    }
}