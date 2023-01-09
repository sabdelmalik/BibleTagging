using System.Net.WebSockets;
using System.Runtime.Intrinsics.Arm;

internal class Program
{
    string sourceFolder = @"C:\Users\samim\Documents\MyProjects\STEP\AutoTagging\BibleFiles\SourcesFiles";
    string intermediateFolder = @"C:\Users\samim\Documents\MyProjects\STEP\AutoTagging\BibleFiles\IntermediateFiles";
    string alignerTrainingFolder;

    string sourceArabicFile;
    string sourceTagsFile;
    string sourceHebrewFile;
    string destinationArabicFile;
    string destinationTagsFile;
    string referencesFile;
    string hebrewNoRefFile;
    string arabicNoRefFile;
    string arabicBiblefile;


    private static void Main(string[] args)
    {
        Program p = new Program();
        //p.Generate(true);
        p.Generate(false);
    }

    public void Generate(bool ot)
    {
        if (ot)
        {
            alignerTrainingFolder = @"C:\Users\samim\Documents\MyProjects\STEP\AutoTagging\Berkeley Word Aligner\BerkeleyAligner\berkeleyaligner\myOtDataset\train";
            sourceArabicFile = "AraSVD_OT_Stemmed.txt";
            sourceTagsFile = "hebrew_tags.txt";
            sourceHebrewFile = "hebrew.txt";
            destinationArabicFile = "oldtestament.a";
            destinationTagsFile = "oldtestament.h";
            referencesFile = "ot_refrences.txt";
            hebrewNoRefFile = "hebrewOtNoRef.txt";
            arabicNoRefFile = "arabicOtNoRef.txt";
            arabicBiblefile = "AraSVD_OT.txt";
        }
        else
        {
            alignerTrainingFolder = @"C:\Users\samim\Documents\MyProjects\STEP\AutoTagging\Berkeley Word Aligner\BerkeleyAligner\berkeleyaligner\myNtDataset\train";
            sourceArabicFile = "AraSVD_NT_Stemmed.txt";
            sourceTagsFile = "greek_tags.txt";
            sourceHebrewFile = "greek_tags.txt";
            destinationArabicFile = "newtestament.a";
            destinationTagsFile = "newtestament.g";
            referencesFile = "nt_refrences.txt";
            hebrewNoRefFile = "greekNtNoRef.txt";
            arabicNoRefFile = "arabicNtNoRef.txt";
            arabicBiblefile = "AraSVD_NT.txt";
        }
        using (StreamReader arabicFile = new StreamReader(Path.Combine(intermediateFolder, sourceArabicFile)))
        using (StreamReader hebrewFile = new StreamReader(Path.Combine(intermediateFolder, sourceHebrewFile)))
        using (StreamReader tagsFile = new StreamReader(Path.Combine(intermediateFolder, sourceTagsFile)))
        using (StreamReader arabicOtFile = new StreamReader(Path.Combine(sourceFolder, arabicBiblefile)))
        using (StreamWriter refFile = new StreamWriter(Path.Combine(intermediateFolder, referencesFile)))
        using (StreamWriter hebNoRefFile = new StreamWriter(Path.Combine(intermediateFolder, hebrewNoRefFile)))
        using (StreamWriter arbNoRefFile = new StreamWriter(Path.Combine(intermediateFolder, arabicNoRefFile)))
        using (StreamWriter trainA = new StreamWriter(Path.Combine(alignerTrainingFolder, destinationArabicFile)))
        using (StreamWriter trainH = new StreamWriter(Path.Combine(alignerTrainingFolder, destinationTagsFile)))
        {
            while (!arabicFile.EndOfStream && !hebrewFile.EndOfStream && !tagsFile.EndOfStream)
            {
                var arabicLine = arabicFile.ReadLine().Trim();
                var hebrewLine = hebrewFile.ReadLine().Trim();
                var tagsLine = tagsFile.ReadLine().Trim();
                var arabicBibleLine = arabicOtFile.ReadLine().Trim();
                string reference = string.Empty;
                string referenceH = string.Empty;
                string referenceT = string.Empty;
                string referenceAOT = string.Empty;
                string arabicOut = string.Empty;
                string HebrewOut = string.Empty;
                string tagOut = string.Empty;
                string arabicOtOut = string.Empty;

                int firstSpace = arabicLine.IndexOf(' ');
                if (firstSpace > 0)
                {
                    int secondSpace = arabicLine.IndexOf(' ', firstSpace + 1);
                    reference = arabicLine.Substring(0, secondSpace);
                    arabicOut = arabicLine.Substring(secondSpace + 1);
                }

                firstSpace = hebrewLine.IndexOf(' ');
                if (firstSpace > 0)
                {
                    int secondSpace = hebrewLine.IndexOf(' ', firstSpace + 1);
                    referenceH = hebrewLine.Substring(0, secondSpace);
                    HebrewOut = hebrewLine.Substring(secondSpace + 1);
                }

                firstSpace = tagsLine.IndexOf(' ');
                if (firstSpace > 0)
                {
                    int secondSpace = tagsLine.IndexOf(' ', firstSpace + 1);
                    referenceT = tagsLine.Substring(0, secondSpace);
                    tagOut = tagsLine.Substring(secondSpace + 1);
                }

                firstSpace = arabicBibleLine.IndexOf(' ');
                if (firstSpace > 0)
                {
                    int secondSpace = arabicBibleLine.IndexOf(' ', firstSpace + 1);
                    referenceAOT = arabicBibleLine.Substring(0, secondSpace);
                    arabicOtOut = arabicBibleLine.Substring(secondSpace + 1);
                }
                if(reference != referenceH || reference != referenceT || reference != referenceAOT)
                {
                    Console.WriteLine(string.Format("reference = {0}, referenceH = {1}, referenceT = {2}, referenceAOT = {3}", (reference, referenceH, referenceT, referenceAOT)));
                    throw new Exception("Out of synch!");
                }

                refFile.WriteLine(reference);
                trainA.WriteLine(arabicOut);
                trainH.WriteLine(tagOut);
                hebNoRefFile.WriteLine(HebrewOut);
                arbNoRefFile.WriteLine(arabicOtOut);
            }

        }
    }
}