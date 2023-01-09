using GenerateOSIS;

internal class Program
{
    private static void Main(string[] args)
    {
        OSIS_Generator osis_Generator = new OSIS_Generator(@"C:\Users\samim\Documents\MyProjects\STEP\CROSSWIRE\BibleModuleDev");

        osis_Generator.Generate("osisAraSVD_T.conf");
    }
}