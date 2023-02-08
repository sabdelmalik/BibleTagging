using BibleTaggingUtil;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SM.Bible.Formats.OSIS
{
    /// <summary>
    /// This code is based on the Python code by Chris Little https://github.com/chrislit/usfm2osis
    /// which is available under GNU General Public License v3.0.
    /// </summary>
    public partial class USFM2OSIS
    {
        string usfmVersion = "2.35";  // http://ubs-icap.org/chm/usfm/2.35/index.html
        string osisVersion = "2.1.1";  // http://www.bibletechnologies.net/osisCore.2.1.1.xsd
        string scriptVersion = "0.6.1";

        Dictionary<string, string> osis_to_loc_book = new Dictionary<string, string>();
        Dictionary<string, string> loc_to_osis_book = new Dictionary<string, string>();

        bool relaxed_conformance = false;
        bool debug = false;
        bool verbose = false;
        bool validate_xml = false;
        string lang_code = "und"; // undefined
        string encodingStr = string.Empty;
        Encoding encoding = Encoding.UTF8;
        string osisWork = string.Empty;
        string osis_filename = string.Empty;
        int input_files_index = 1;  // This marks the point in the sys.argv array, after which all values represent USFM files to be converted.
        List<string> work_usfm_doc_list = new List<string>();
        List<string> usfm_doc_list = new List<string>();

        Dictionary<string, string> osisSegment;

        List<string> aliases = new List<string>();

        Dictionary<string, string> usfm2osisConf;

        public USFM2OSIS(BibleTaggingForm parent, ConfigurationHolder config)
        {
            this.usfm2osisConf = config.USFM2OSIS;
            osisWork = usfm2osisConf[Usfm2OsisConstants.osisIDWork];
            osis_filename = Path.Combine(BibleTaggingUtil.Properties.Settings.Default.BiblesFolder, usfm2osisConf[Usfm2OsisConstants.outputFileName]);
            lang_code = usfm2osisConf[Usfm2OsisConstants.language];
        }

        public void Convert()
        {
            osisSegment = new Dictionary<string, string>(); 
            
            string folder = Path.Combine(BibleTaggingUtil.Properties.Settings.Default.BiblesFolder, usfm2osisConf[Usfm2OsisConstants.usfmSourceFolder]);
            string[] docs = Directory.GetFiles(folder, "*.usfm");
            
            foreach (string doc in docs)
            {
                usfm_doc_list.Add(doc);
                read_identifiers_from_osis(doc);
                string osis = ConvertToOSIS(doc);
                osisSegment[doc] = osis;
            }

            string osis_doc = (
                "<osis xmlns=\"http://www.bibletechnologies.net/2003/OSIS/namespace\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xsi:schemaLocation=\"http://www.bibletechnologies.net/2003/OSIS/namespace http://www.bibletechnologies.net/osisCore."
                + osisVersion
                + ".xsd\">\n<osisText osisRefWork=\"Bible\" xml:lang=\""
                + lang_code
                + "\" osisIDWork=\""
                + osisWork
                + "\">\n<header>\n<work osisWork=\""
                + osisWork
                + "\"/>\n</header>\n"
            );
            List<string> unhandled_tags = new List<string>();
            foreach (string doc in usfm_doc_list)
            {
                Regex rx = new Regex(@"(\\[^\s]*)");
                string text = rx.Match(osisSegment[doc]).Value;
                if (!string.IsNullOrEmpty(text))
                    unhandled_tags.Add(text);
                osis_doc += osisSegment[doc];
            }
            osis_doc += "</osisText>\n</osis>\n";

            if (validate_xml)
            {
                try
                {

                }
                catch { }
            }

            using (StreamWriter sw = new StreamWriter(osis_filename, false, Encoding.UTF8))
            {
                sw.WriteLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
                sw.Write(osis_doc);
            }

/*            if (unhandled_tags.Count > 0)
            {
                unhandled_tags.Sort();
                Console.WriteLine("");
                Console.WriteLine(
                    (
                    "Unhandled USFM tags: "
                    + string.Join(",", unhandled_tags)
                    + " ("
                    + unhandled_tags.Count().ToString()
                    + " total)"
                    )
                );
                if (!relaxed_conformance)
                    Console.WriteLine("Consider using the -r option for relaxed markup processing");

            }*/
        }



        /// <summary>
        /// Reads the USFM file and stores information about which Bible book it
        /// represents and localized abbreviations in global variables.
        /// </summary>
        /// <param name="filename">a USFM filename</param>
        private void read_identifiers_from_osis(string filename)
        {
            if (!File.Exists(filename))
            {
                return;
            }
            string osis = string.Empty;

            if (!string.IsNullOrEmpty(encodingStr))
            {
                try
                {
                    encoding = Encoding.GetEncoding(encodingStr);
                }
                catch
                {
                    encoding = Encoding.UTF8;
                }

                using (StreamReader sr = new StreamReader(filename, encoding))
                {
                    osis = sr.ReadToEnd().Trim() + "\n";
                }
            }
            else
            {
                encoding = Encoding.UTF8;
                using (StreamReader sr = new StreamReader(filename, encoding))
                {
                    osis = sr.ReadToEnd().Trim() + "\n";
                }
 
                Match match = Regex.Match(osis, @"\\ide\s+(.+)" + "\n");
                if (match.Success)
                { 
                    encodingStr = match.Groups[1].Value.ToLower().Trim();

                    if (!encodingStr.Equals("utf-8"))
                    {
                        if (aliases.Contains(encodingStr))
                        {
                            try
                            {
                                encoding = Encoding.GetEncoding(encodingStr);
                            }
                            catch
                            {
                                encoding = Encoding.UTF8;
                            }


                            using (StreamReader sr = new StreamReader(filename, encoding))
                            {
                                osis = sr.ReadToEnd().Trim() + "\n";
                            }
                        }
                        else
                        {
                            Console.WriteLine(("WARNING: Encoding \"" + encodingStr +
                               "\" unknown, processing " + filename + " as UTF-8"));
                        }
                    }
                }

                // keep a copy of the OSIS book abbreviation for below (\toc3 processing)
                // to store for mapping localized book names to/from OSIS
                string osis_book = string.Empty;
                match = Regex.Match(osis, @"\\id\s+([A-Z0-9]+)");
                if (match.Success)
                    osis_book = match.Groups[1].Value;

                if (!string.IsNullOrEmpty(osis_book))
                {
                    osis_book = BOOK_DICT[osis_book];
                    FILENAME_TO_OSIS[filename] = osis_book;
                }

                string loc_book = string.Empty;
                match = Regex.Match(osis, @"\\toc3\b\s+(.+)\s*");
                if (match.Success)
                    loc_book = match.Groups[1].Value;
                if (!string.IsNullOrEmpty(loc_book))
                {
                    osis_to_loc_book[osis_book] = loc_book;
                    loc_to_osis_book[loc_book] = osis_book;
                }

            }
        }

        /// <summary>
        /// Prints usage statement.
        /// </summary>
        private void print_usage()
        {
            Console.WriteLine(
                (
                    "usfm2osis -- USFM "
                    + usfmVersion
                    + " to OSIS "
                    + osisVersion
                    + " converter version "
                    + scriptVersion
                )
            );
            Console.WriteLine("");
            Console.WriteLine("Usage: usfm2osis <osisWork> [OPTION] ...  <USFM filename|wildcard> ...");
            Console.WriteLine("");
            Console.WriteLine("  -h, --help       print this usage information");
            Console.WriteLine("  -d               debug mode (single-threaded, verbose output)");
            Console.WriteLine("  -e ENCODING      input encodingStr override (default is to read the USFM file's");
            Console.WriteLine("                     \\ide value or assume UTF-8 encodingStr in its absence)");
            Console.WriteLine("  -o FILENAME      output filename (default is: <osisWork>.osis.xml)");
            Console.WriteLine("  -r               enable relaxed markup processing (for non-standard USFM)");
            Console.WriteLine("  -s MODE          set book sorting mode: natural (default), alpha, canonical,");
            Console.WriteLine("                     usfm, random, none");
            Console.WriteLine("  -t NUM           set the number of separate processes to use (your maximum");
            Console.WriteLine("                     thread count by default)");
            Console.WriteLine("  -l LANG          set the language value to a BCP 47 code ('und' by default)");
            Console.WriteLine("  -v               verbose feedback");
            Console.WriteLine("  -x               disable XML validation");
            Console.WriteLine("");
            Console.WriteLine("As an example, if you want to generate the osisWork <Bible.KJV> and your USFM");
            Console.WriteLine("  are located in the ./KJV folder, enter:");
            Console.WriteLine("    python usfm2osis Bible.KJV ./KJV/*.usfm");
            verbose_print("", verbose);

            verbose_print("Supported encodings: " + string.Join(",", aliases), verbose);
        }
    }
}
