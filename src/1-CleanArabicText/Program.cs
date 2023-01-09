using BibleTagging;
using System.Globalization;
using System.Text;


internal class Program
{
    private static void Main(string[] args)
    {
        //string sourceFolder = @"C:\Users\samim\Documents\MyProjects\STEP\AutoTagging\BibleFiles\SourcesFiles";
        //string outputFolder = @"C:\Users\samim\Documents\MyProjects\STEP\AutoTagging\BibleFiles\IntermediateFiles";

        Configuration conf = new Configuration(@"C:\Users\samim\Documents\MyProjects\STEP\AutoTagging\BibleFiles\arabic.config.txt");

        Program p = new Program();
//        p.CleanText(Path.Combine(sourceFolder, "AraSVD_Full.txt"), Path.Combine(outputFolder, "AraSVD_Full_Clean.txt"));
//        p.CleanText(Path.Combine(sourceFolder, "AraSVD_NT.txt"), Path.Combine(outputFolder, "AraSVD_NT_Clean.txt"));
//        p.CleanText(Path.Combine(sourceFolder, "AraSVD_OT.txt"), Path.Combine(outputFolder, "AraSVD_OT_Clean.txt"));

        string baseFolder = conf.GetConfigValue(Configuration.base_folder);
        string bible_file = conf.GetConfigValue(Configuration.bible_file);
        string bible_clean_file = conf.GetConfigValue(Configuration.bible_clean_file);
        string ot_file = conf.GetConfigValue(Configuration.ot_file);
        string ot_clean_file = conf.GetConfigValue(Configuration.ot_clean_file);
        string nt_file = conf.GetConfigValue(Configuration.nt_file);
        string nt_clean_file = conf.GetConfigValue(Configuration.nt_clean_file);
        
        p.CleanText(Path.Combine(baseFolder, bible_file), Path.Combine(baseFolder, bible_clean_file));
        p.CleanText(Path.Combine(baseFolder, ot_file), Path.Combine(baseFolder, ot_clean_file));
        p.CleanText(Path.Combine(baseFolder, nt_file), Path.Combine(baseFolder, nt_clean_file));

    }

    public void CleanText(string sourcePath, string destinationPath)
    {
        /* 
         * https://www.loc.gov/marc/specifications/codetables/BasicArabic.html 
         * https://unicode.org/charts/PDF/U0600.pdf
        */

        using (StreamReader reader = new StreamReader(sourcePath))
        {
            using (StreamWriter outputFile = new StreamWriter(destinationPath))
            {
                while (reader.Peek() >= 0)
                {
                    string line = reader.ReadLine();
                    // 1.seperate verse reference
                    int idx1 = line.IndexOf(' ');
                    int idx2 = -1;
                    if (idx1 >= 0)
                        idx2 = line.IndexOf(' ', idx1 +1);
                    if (idx2 >= 0)
                    {
                        string verseRef = line.Substring(0, idx2 + 1);
                        string lineToCkean = line.Substring(idx2 + 1);

                        // Remove diacretics
                        string clean = lineToCkean.
                            Replace("\u064B", "").  // ARABIC FATHATAN
                            Replace("\u064C", "").  // ARABIC DAMMATAN
                            Replace("\u064D", "").  // ARABIC KASRATAN
                            Replace("\u064E", "").  // ARABIC FATHA
                            Replace("\u064F", "").  // ARABIC DAMMA
                            Replace("\u0650", "").  // ARABIC KASRA
                            Replace("\u0651", "").  // ARABIC SHADDA
                            Replace("\u0652", "").  // ARABIC SUKUN
                            Replace("\u0653", "").  // Madda
                            Replace("\u0654", "").  // Hamza above
                            Replace("\u0655", "").  // Hamza below
                            Replace("\u0656", "").  // 
                            Replace("\u0657", "").
                            Replace("\u0658", "").
                            Replace("\u0659", "").
                            Replace("\u065A", "").
                            Replace("\u065B", "").
                            Replace("\u065C", "").
                            Replace("\u065D", "").
                            Replace("\u065E", "").
                            Replace("\u065F", "");
                        // Reduce variances of Alef to just one form
                        clean = clean.
                            Replace("\u0622", "\u0627").  //ALEF WITH MADDA ABOVE
                            Replace("\u0623", "\u0627").  //ALEF WITH HAMZA ABOVE
                            Replace("\u0625", "\u0627").  //ALEF WITH HAMZA BELOW
                            Replace("\u0671", "\u0627");  //ALEF WASLA
                        // remove unnecessary punctuation
                        clean = clean.
                            Replace(".", "").
                            Replace("«", "").
                            Replace("»", "").
                            Replace(":", "").
                            Replace("؟", "").
                            Replace("،", "");

                        outputFile.WriteLine(verseRef + clean);
                    }
                }

            }
        }
    }
}