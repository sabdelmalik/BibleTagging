
using BibleTagging;
using System.Globalization;
using System.Text;
internal class Program
{
    private static void Main(string[] args)
    {
        string baseFolder = "C:\\Users\\samim\\Documents\\MyProjects\\STEP\\AutoTagging\\BibleFiles\\SourcesFiles";
        string destinationFile = "C:\\Users\\samim\\Documents\\MyProjects\\STEP\\AutoTagging\\BibleFiles\\IntermediateFiles\\greek_tags.txt";
        string mat2jhn = Path.Combine(baseFolder, "TAGNT Mat-Jhn - Translators Amalgamated Greek NT - STEPBible.org CC-BY.txt");
        string act2rev = Path.Combine(baseFolder, "TAGNT Act-Rev - Translators Amalgamated Greek NT - STEPBible.org CC-BY.txt");
        TAGNT_Parser parser = new TAGNT_Parser();
        using (StreamWriter sw = new StreamWriter(destinationFile))
        {
            parser.Parse(mat2jhn, sw);
            parser.Parse(act2rev, sw);
        }

    }
}