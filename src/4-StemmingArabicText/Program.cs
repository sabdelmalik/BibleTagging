using StemmingArabicBible;
using System.Security.Cryptography.X509Certificates;

internal class Program
{
    ParatextMorphologyParser pmp = new ParatextMorphologyParser();
    private static void Main(string[] args)
    {
        string morphFileFolder = @"C:\Users\samim\Documents\MyProjects\STEP\AutoTagging\BibleFiles\SourcesFiles";
        string otMmorphFileName = "OT_Morphology_5c74e6a4bdda893df502fd69f3da4f824ccac641_Affix.xml";
        string ntMmorphFileName = "NT_Morphology_a05b5fb9ae2433d4334aa6303d6ca5e6ee363770_Affix.xml";
        string arabicSourceFolder = @"C:\Users\samim\Documents\MyProjects\STEP\AutoTagging\BibleFiles\IntermediateFiles";
        string otArabicBibleText = "AraSVD_OT_Clean.txt";
        string ntArabicBibleText = "AraSVD_NT_Clean.txt";
        string otAarabicBibleStemmmed = "AraSVD_OT_Stemmed.txt";
        string ntAarabicBibleStemmmed = "AraSVD_NT_Stemmed.txt";

        Program p = new Program();
        p.ApplyStems(Path.Combine(morphFileFolder, otMmorphFileName),
            Path.Combine(arabicSourceFolder, otArabicBibleText),
            Path.Combine(arabicSourceFolder, otAarabicBibleStemmmed));

        p.ApplyStems(Path.Combine(morphFileFolder, ntMmorphFileName),
            Path.Combine(arabicSourceFolder, ntArabicBibleText),
            Path.Combine(arabicSourceFolder, ntAarabicBibleStemmmed));
    }

    public void ApplyStems(string morpfFile, string arabicBibleText , string arabicBibleStemmmed)
    { 
        
        pmp.Parse(morpfFile);

        string currentChapter = string.Empty;

        using (StreamReader sr = new StreamReader(arabicBibleText))
        {
            using (StreamWriter sw = new StreamWriter(arabicBibleStemmmed))
            {
                while (sr.Peek() >= 0)
                {
                    string line = sr.ReadLine();
                    string[] lineParts = line.Split(' ');
                    if (lineParts.Length < 3)
                        continue;
                    int idx = line.IndexOf(':');
                    if (idx >= 0)
                    {
                        string chapter = line.Substring(0, idx);
                        if (currentChapter != chapter)
                        {
                            currentChapter = chapter;
                            Console.WriteLine(currentChapter + "...");
                        }
                    }

                    string stemmed = lineParts[0] + " " + lineParts[1]; // get the verse reference
                    for (int i = 2; i < lineParts.Length; i++)
                    {
                        string word = lineParts[i].Trim();
                        if (word == "والأرض")
                        {
                            int x = 0;
                        }
                        if (word.Length < 3)
                        {
                            stemmed += (" " + word);
                        }
                        else
                        {
                            if (pmp.Morphs.ContainsKey(word))
                            {
                                stemmed += (" " + pmp.Morphs[word]);
                            }
                            else
                            {
                                stemmed += (" " + word);
                            }
                        }
                    }
                    sw.WriteLine(stemmed);
                }

            }
        }

    }
}