using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Intrinsics.Arm;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace SM.Bible.Formats.OSIS
{
    /// <summary>
    /// This code is based on the Python code by Chris Little https://github.com/chrislit/usfm2osis
    /// which is available under GNU General Public License v3.0.
    /// </summary>
    public partial class USFM2OSIS
    {
        /// <summary>
        /// Open a USFM file and return a string consisting of its OSIS equivalent.
        /// </summary>
        /// <param name="filename">Path to the USFM file to be converted</param>
        /// <returns></returns>
        private string ConvertToOSIS(string filename)
        {
            string osis = string.Empty;


            verbose_print(("Processing: " + filename), verbose);

            using (StreamReader sr = new StreamReader(filename, encoding))
            {
                osis = sr.ReadToEnd().Trim() + "\n";
            }

            osis = osis.TrimStart(new char[] { '\xFEFF' });

            // call individual conversion processors in series
            osis = cvt_preprocess(osis);
            osis = cvt_relaxed_conformance_remaps(osis);
            osis = cvt_identification(osis);
            osis = cvt_introductions(osis);
            osis = cvt_titles(osis);
            osis = cvt_chapters_and_verses(osis);
            osis = cvt_paragraphs(osis);
            osis = cvt_poetry(osis);
            osis = cvt_tables(osis);
            osis = cvt_footnotes(osis);
            osis = cvt_cross_references(osis);
            osis = cvt_special_text(osis);
            osis = cvt_character_styling(osis);
            osis = cvt_spacing_and_breaks(osis);
            osis = cvt_special_features(osis);
            osis = cvt_peripherals(osis);
            osis = cvt_study_bible_content(osis);
            osis = cvt_private_use_extensions(osis);

            osis = process_osisIDs(osis);
            osis = osis_reorder_and_cleanup(osis);

            // change type on special books
            foreach (string sb in SPECIAL_BOOKS)
            {
                osis = osis.Replace("<div type=\"book\" osisID=\"" + sb + "\">",
                                    "<div type=\"" + sb.ToLower() + "\">");
            }

            if (debug)
            {
                MatchCollection matches = Regex.Matches(@"(\\[^\s]*)", osis);
                List<string> local_unhandled_tags = new List<string>();
                foreach (Match tag in matches)
                {
                    local_unhandled_tags.Add(tag.Value);
                }


                if (local_unhandled_tags.Count > 0)
                    Console.WriteLine(
                        (
                            "Unhandled USFM tags in "
                            + filename
                            + ": "
                            + string.Join(",", local_unhandled_tags)
                            + " ("
                            + local_unhandled_tags.Count().ToString()
                            + " total)"
                        )
                    );
            }

            return osis;
        }

        /// <summary>
        /// Perform preprocessing on a USFM document, returning the processed
        /// text as a string.
        /// Removes excess spaces & CRs and escapes XML entities.
        /// </summary>
        /// <param name=\"osis_in">The document as a string.</param>
        /// <returns></returns>
        private string cvt_preprocess(string osis_in)
        {
            string osis = osis_in;

            // lines should never start with non-tags
            osis = Regex.Replace(osis, @"\n\s*([^\\\s])", " $1"); //TODO: test this
            // convert CR to LF
            osis = osis.Replace("\r", "\n");
            // lines should never end with whitespace (other than \n)
            osis = Regex.Replace(osis, @"\s+\n", "\n");
            // Replace with XML entities, as necessary
            osis = osis.Replace("&", "&amp;");
            osis = osis.Replace("<", "&lt;");
            osis = osis.Replace(">", "&gt;");

            // osis = Regex.Replace(osis,'\n' + r'(\\[^\s]+\b\*)', r' $1', osis_in)

            return osis;
        }

        /// <summary>
        /// Perform preprocessing on a USFM document, returning the processed
        /// text as a string.
        /// Remaps certain deprecated USFM tags to recommended alternatives.
        /// </summary>
        /// <param name=\"osis_in">The document as a string</param>
        /// <returns></returns>
        private string cvt_relaxed_conformance_remaps(string osis_in)
        {
            string osis = osis_in;

            if (!relaxed_conformance)
                return osis;

            // \tr#: DEP: map to \tr
            osis = Regex.Replace(osis, @"\\tr\d\b", "\\tr");

            // remapped 2.0 periphs
            // \pub
            osis = Regex.Replace(osis, @"\\pub\b\s", "\\periph Publication Data\n");
            // \toc : \periph Table of Contents
            osis = Regex.Replace(osis, @"\\toc\b\s", "\\periph Table of Contents\n");
            // \pref
            osis = Regex.Replace(osis, @"\\pref\b\s", "\\periph Preface\n");
            // \maps
            osis = Regex.Replace(osis, @"\\maps\b\s", "\\periph Map Index\n");
            // \cov
            osis = Regex.Replace(osis, @"\\cov\b\s", "\\periph Cover\n");
            // \spine
            osis = Regex.Replace(osis, @"\\spine\b\s", "\\periph Spine\n");
            // \pubinfo
            osis = Regex.Replace(osis, @"\\pubinfo\b\s", "\\periph Publication Information\n");

            // \intro
            osis = Regex.Replace(osis, @"\\intro\b\s", "\\id INT\n");
            // \conc
            osis = Regex.Replace(osis, @"\\conc\b\s", "\\id CNC\n");
            // \glo
            osis = Regex.Replace(osis, @"\\glo\b\s", "\\id GLO\n");
            // \idx
            osis = Regex.Replace(osis, @"\\idx\b\s", "\\id TDX\n");


            return osis;
        }

        /// <summary>
        /// Converts USFM **Identification** tags to OSIS, returning the
        /// processed text as a string.
        /// Supported tags: \id, \ide, \sts, \rem, \h, \toc1, \toc2, \toc3
        /// </summary>
        /// <param name=\"osis_in">The document as a string</param>
        /// <returns></returns>
        private string cvt_identification(string osis_in)
        {
            string osis = osis_in;

            // \id_<CODE>_(Name of file, Book name, Language, Last edited, Date,
            // etc.);
            osis = Regex.Replace(osis,
                            @"\\id\s+([A-Z0-9]{3})\b\s*([^\\" + "\n]*?)\n" + @"(.*)(?=\\id|$)",
                            m => "\uFDD0<div type=\"book\" osisID=\""
                            + BOOK_DICT[m.Groups[1].Value]
                            + "\">\n"
                            + m.Groups[3].Value
                            + (string.IsNullOrEmpty(m.Groups[2].Value) ? "" : ("<!-- id comment - " + m.Groups[2].Value + " -->\n"))
                            + "</div type=\"book\">\uFDD0\n",
                            RegexOptions.Singleline);

            // \ide_<ENCODING>
            // delete, since this was handled above
            osis = Regex.Replace(osis, @"\\ide\b.*" + "\n", "");
            // \sts_<STATUS CODE>
            osis = Regex.Replace(osis,
                @"\\sts\b\s+(.+)\s*" + "\n",
                "<milestone type=\"x-usfm-sts\" n=\"$1\"/>" + "\n");

            // \rem_text...
            osis = Regex.Replace(osis, @"\\rem\b\s+(.+)", "<!-- rem - $1 -->");

            // \restore_text...
            if (relaxed_conformance)
                osis = Regex.Replace(osis, @"\\restore\b\s+(.+)", "<!-- restore - $1 -->");

            // \h//_text...
            osis = Regex.Replace(osis,
                @"\\h\b\s+(.+)\s*" + "\n",
                "<title type=\"runningHead\">$1</title>" + "\n");
            osis = Regex.Replace(osis,
                @"\\h(\d)\b\s+(.+)\s*" + "\n",
                "<title type=\"runningHead\" n=\"$1\">$2</title>" + "\n");

            // \toc1_text...
            osis = Regex.Replace(osis,
                @"\\toc1\b\s+(.+)\s*" + "\n",
                "<milestone type=\"x-usfm-toc1\" n =\"$1\" />" + "\n");

            // \toc2_text...
            osis = Regex.Replace(osis,
                @"\\toc2\b\s+(.+)\s*" + "\n",
                "<milestone type=\"x-usfm-toc2\" n =\"$1\" />" + "\n");

            // \toc3_text...
            osis = Regex.Replace(osis,
                @"\\toc3\b\s+(.+)\s*" + "\n",
                "<milestone type=\"x-usfm-toc3\" n =\"$1\" />" + "\n");


            return osis;
        }

        /// <summary>
        /// Converts USFM **Introduction** tags to OSIS, returning the processed
        /// text as a string.
        /// 
        /// Supported tags: \imt#, \is#, \ip, \ipi, \im, \imi, \ipq, \imq, \ipr,
        ///                 \iq#, \ib, \ili#, \iot, \io#, \ior...\ior*, \iex,
        ///                 \iqt...\iqt*, \imte, \ie
        /// </summary>
        /// <param name=\"osis_in">The document as a string</param>
        /// <returns></returns>
        private string cvt_introductions(string osis_in)
        {
            string osis = osis_in;

            // \imt#_text...
            osis = Regex.Replace(osis,
                 @"\\imt(\d?)\s+(.+)",
                 m => "<title "
                 + (string.IsNullOrEmpty(m.Groups[1].Value) ? "" : "level=\"" + m.Groups[1].Value + "\" ")
             + "type=\"main\" subType=\"x-introduction\">"
             + m.Groups[2].Value
             + "</title>");

            // \imte#_text...
            osis = Regex.Replace(osis,
                @"\\imte(\d?)\b\s+(.+)",
                m => "<title "
                + (string.IsNullOrEmpty(m.Groups[1].Value) ? "" : "level=\"" + (m.Groups[1].Value + "\" "))
                + "type=\"main\" subType=\"x-introduction-end\">"
                + m.Groups[2].Value
                + "</title>");

            // \is#_text...
            osis = Regex.Replace(osis,
                @"\\is1?\s+(.+)",
                m => "\uFDE2<div type=\"section\" subType=\"x-introduction\"><title>"
                + m.Groups[1].Value
                + "</title>");
            osis = Regex.Replace(osis,
                "(\uFDE2<div type=\"section\" subType=\"x-introduction\">[^\uFDE2]+)"
                + @"(?!\\c\b)",
                "$1" + "</div>\uFDE2\n",
                RegexOptions.Singleline);
            osis = Regex.Replace(osis,
                @"\\is2\s+(.+)",
                m => "\uFDE3<div type=\"subSection\" subType=\"x-introduction\"><title>"
                + m.Groups[1].Value
                + "</title>");
            osis = Regex.Replace(osis,
                "(\uFDE3<div type=\"subSection\" subType=\"x-introduction\">[^\uFDE2\uFDE3]+)"
                + @"(?!\\c\b)",
                "$1" + "</div>\uFDE3\n",
                RegexOptions.Singleline);
            osis = Regex.Replace(osis,
                @"\\is3\s+(.+)",
                m => "\uFDE4<div type=\"x-subSubSection\" subType=\"x-introduction\"><title>"
                + m.Groups[1].Value
                + "</title>");
            osis = Regex.Replace(osis,
                "(\uFDE4<div type=\"subSubSection\" subType=\"x-introduction\">[^\uFDE2\uFDE3\uFDE4]+)"
                + @"(?!\\c\b)",
                "$1" + "</div>\uFDE4\n",
                RegexOptions.Singleline);
            osis = Regex.Replace(osis,
                @"\\is4\s+(.+)",
                m => "\uFDE5<div type=\"x-subSubSubSection\" subType=\"x-introduction\"><title>"
                + m.Groups[1].Value
                + "</title>");
            osis = Regex.Replace(osis,
                "(\uFDE5<div type=\"subSubSubSection\" subType=\"x-introduction\">[^\uFDE2\uFDE3\uFDE4\uFDE5]+)"
                + @"(?!\\c\b)",
                "$1" + "</div>\uFDE5\n",
                RegexOptions.Singleline);
            osis = Regex.Replace(osis,
                @"\\is5\s+(.+)",
                m => "\uFDE6<div type=\"x-subSubSubSubSection\" subType=\"x-introduction\"><title>"
                + m.Groups[1].Value
                + "</title>");
            osis = Regex.Replace(osis,
                "(\uFDE6<div type=\"subSubSubSubSection\" subType=\"x-introduction\">[^\uFDE2\uFDE3\uFDE4\uFDE5\uFDE6]+?)"
                + @"(?!\\c\b)",
                "$1" + "</div>\uFDE6\n",
                RegexOptions.Singleline);

            // \ip_text...
            osis = Regex.Replace(osis,
                @"\\ip\s+(.*?)(?=(\\(i?m|i?p|lit|cls|tr|io[t\d]?|iq|i?li|iex?|s|c)\b|<(/?div|p|closer)\b))",
                m => "\uFDD3<p subType=\"x-introduction\">\n"
                + m.Groups[1].Value
                + "\uFDD3</p>\n",
                RegexOptions.Singleline);

            // \ipi_text...
            // \im_text...
            // \imi_text...
            // \ipq_text...
            // \imq_text...
            // \ipr_text...
            Dictionary<string, string> p_type = new Dictionary<string, string>()
            {
                { "ipi", "x-indented"},
                {"im", "x-noindent"},
                {"imi", "x-noindent-indented"},
                {"ipq", "x-quote"},
                {"imq", "x-noindent-quote"},
                {"ipr", "x-right"}
            };
            osis = Regex.Replace(osis,
                @"\\(ipi|im|ipq|imq|ipr)\s+(.*?)(?=(\\(i?m|i?p|lit|cls|tr|io[t\d]?|ipi|iq|i?li|iex?|s|c)\b|<(/?div|p|closer)\b))",
                m => "\uFDD3<p type=\""
                + p_type[m.Groups[1].Value]
                + "\" subType=\"x-introduction\">\n"
                + m.Groups[2].Value
                + "\uFDD3</p>\n",
                RegexOptions.Singleline);

            // \iq#_text...
            osis = Regex.Replace(osis,
                @"\\iq\b\s*(.*?)(?=(["
                + "\uFDD0\uFDD1\uFDD3\uFDD4"
                + @"]|\\(iq\d?|fig|q\d?|b)\b|<title\b))",
                "<l level=\"1\" subType=\"x-introduction\">$1</l>",
                RegexOptions.Singleline);
            osis = Regex.Replace(osis,
                @"\\iq(\d)\b\s*(.*?)(?=(["
                + "\uFDD0\uFDD1\uFDD3\uFDD4"
                + @"]|\\(iq\d?|fig|q\d?|b)\b|<title\b))",
                "<l level=\"$1\" subType=\"x-introduction\">$2</l>",
                RegexOptions.Singleline);

            // \ib
            osis = Regex.Replace(osis, @"\\ib\b\s?", "<lb type=\"x-p\"/>");
            osis = osis.Replace("\n</l>", "</l>\n");
            // osis = Regex.Replace(osis,"(<l [^\uFDD0\uFDD1\uFDD3\uFDD4]+</l>)", r"<lg>$1</lg>", osis, flags=re.DOTALL)
            // osis = Regex.Replace(osis,"(<lg>.+?</lg>)", m => m.Groups[1].Value.replace("<lb type=\"x-p"/>", "</lg><lg>"), osis, flags=re.DOTALL) // re-handle \b that occurs within <lg>

            // \ili#_text...
            osis = Regex.Replace(osis,
                @"\\ili\b\s*(.*?)(?=(["
                + "\uFDD0\uFDD1\uFDD3\uFDD4"
                + @"]|\\(ili\d?|c|p|io[t\d]?|iex?)\b|<(lb|title|item|\?div)\b))",
                "<item type=\"x-indent-1\" subType=\"x-introduction\">\uFDE0"
                + "$1"
                + "\uFDE0</item>",
                RegexOptions.Singleline);
            osis = Regex.Replace(osis,
                @"\\ili(\d)\b\s*(.*?)(?=(["
                + "\uFDD0\uFDD1\uFDD3\uFDD4"
                + @"]|\\(ili\d?|c|p|io[t\d]?|iex?)\b|<(lb|title|item|\?div)\b))",
                "<item type=\"x-indent-$1\" subType=\"x-introduction\">"
                + "\uFDE0"
                + "$2"
                + "\uFDE0</item>",
                RegexOptions.Singleline);
            osis = osis.Replace("\n</item>", "</item>\n");
            osis = Regex.Replace(osis,
                "(<item [^\uFDD0\uFDD1\uFDD3\uFDD4]+</item>)",
                "\uFDD3<list>" + "$1" + "</list>\uFDD3",
                RegexOptions.Singleline);

            // \iot_text...
            // \io#_text...(references range)
            osis = Regex.Replace(osis,
                @"\\io\b\s*(.*?)(?=(["
                + "\uFDD0\uFDD1\uFDD3\uFDD4"
                + @"]|\\(io[t\d]?|iex?|c|p)\b|<(lb|title|item|\?div)\b))",
                "<item type=\"x-indent-1\" subType=\"x-introduction\">\uFDE1"
                + "$1"
                + "\uFDE1</item>",
                RegexOptions.Singleline);
            osis = Regex.Replace(osis,
                @"\\io(\d)\b\s*(.*?)(?=(["
                + "\uFDD0\uFDD1\uFDD3\uFDD4"
                + @"]|\\(io[t\d]?|iex?|c|p)\b|<(lb|title|item|\?div)\b))",
                "<item type=\"x-indent-$1\" subType=\"x-introduction\" >"
                + "\uFDE1"
                + "$2"
                + "\uFDE1</item>",
                RegexOptions.Singleline);
            osis = Regex.Replace(osis,
                @"\\iot\b\s*(.*?)(?=(["
                + "\uFDD0\uFDD1\uFDD3\uFDD4"
                + @"]|\\(io[t\d]?|iex?|c|p)\b|<(lb|title|item|\?div)\b))",
                "<item type=\"head\">\uFDE1" + "$1" + "\uFDE1</item type=\"head\">",
                RegexOptions.Singleline);
            osis = osis.Replace("\n</item>", "</item>\n");
            osis = Regex.Replace(osis,
                "(<item [^\uFDD0\uFDD1\uFDD3\uFDD4\uFDE0]+</item>)",
                "\uFDD3<div type=\"outline\"><list>" + "$1" + "</list></div>\uFDD3",
                RegexOptions.Singleline);
            osis = Regex.Replace(osis, "item type=\"head\"", "head");

            // \ior_text...\ior*
            osis = Regex.Replace(osis,
                @"\\ior\b\s+(.+?)\\ior\*",
                @"<reference>$1</reference>",
                RegexOptions.Singleline);

            // \iex  // TODO: look for example; I have no idea what this would look like in context
            osis = Regex.Replace(osis,
                @"\\iex\b\s*(.+?)" + @"?=(\s*(\\c|" + "</div type=\"book\">" + "\uFDD0))",
                "<div type=\"bridge\">$1</div>",
                RegexOptions.Singleline);

            // \iqt_text...\iqt*
            osis = Regex.Replace(osis,
                @"\\iqt\s+(.+?)\\iqt\*",
                "<q subType=\"x-introduction\">$1</q>",
                RegexOptions.Singleline);

            // \ie
            osis = Regex.Replace(osis, @"\\ie\b\s*", "<milestone type=\"x-usfm-ie\"/>");

            return osis;
        }

        /// <summary>
        /// Converts USFM **Title, Heading, and Label** tags to OSIS, returning
        /// the processed text as a string.
        /// 
        /// Supported tags: \mt#, \mte#, \ms#, \mr, \s#, \sr, \r, \rq...\rq*, \d,
        ///                 \sp
        /// </summary>
        /// <param name=\"osis_in">The document as a string</param>
        /// <returns></returns>
        private string cvt_titles(string osis_in)
        {
            string osis = osis_in;

            // \ms#_text...
            osis = Regex.Replace(osis,
                @"\\ms1?\s+(.+)",
                m => "\uFDD5<div type=\"majorSection\"><title>"
                + m.Groups[1].Value
                + "</title>");

            osis = Regex.Replace(osis,
            "(\uFDD5[^\uFDD5\uFDD0]+)", "$1" + "</div>\uFDD5\n", RegexOptions.Singleline);

            osis = Regex.Replace(osis,
                @"\\ms2\s+(.+)",
                m => "\uFDD6<div type=\"majorSection\" n=\"2\"><title>"
                + m.Groups[1].Value
                + "</title>");
            osis = Regex.Replace(osis,
                "(\uFDD6[^\uFDD5\uFDD0\uFDD6]+)",
                "$1" + "</div>\uFDD6\n",
                RegexOptions.Singleline);
            osis = Regex.Replace(osis,
                @"\\ms3\s+(.+)",
                m => "\uFDD7<div type=\"majorSection\" n=\"3\"><title>"
                + m.Groups[1].Value
                + "</title>");
            osis = Regex.Replace(osis,
                "(\uFDD7[^\uFDD5\uFDD0\uFDD6\uFDD7]+)",
                "$1" + "</div>\uFDD7\n",
                RegexOptions.Singleline);
            osis = Regex.Replace(osis,
                @"\\ms4\s+(.+)",
                m => "\uFDD8<div type=\"majorSection\" n=\"4\"><title>"
                + m.Groups[1].Value
                + "</title>");
            osis = Regex.Replace(osis,
                "(\uFDD8[^\uFDD5\uFDD0\uFDD6\uFDD7\uFDD8]+)",
                "$1" + "</div>\uFDD8\n",
                RegexOptions.Singleline);
            osis = Regex.Replace(osis,
                @"\\ms5\s+(.+)",
                m => "\uFDD9<div type=\"majorSection\" n=\"5\"><title>"
                + m.Groups[1].Value
                + "</title>");
            osis = Regex.Replace(osis,
                "(\uFDD9[^\uFDD5\uFDD0\uFDD6\uFDD7\uFDD8\uFDD9]+)",
                "$1" + "</div>\uFDD9\n",
                RegexOptions.Singleline);

            // \mr_text...
            osis = Regex.Replace(osis,
                @"\\mr\s+(.+)",
                "\uFDD4<title type=\"scope\"><reference>" + "$1</reference></title>");

            // \s#_text...
            osis = Regex.Replace(osis,
                @"\\s1?\s+(.+)",
                m => "\uFDDA<div type=\"section\"><title>" + m.Groups[1].Value + "</title>");
            osis = Regex.Replace(osis,
                "(\uFDDA<div type=\"section\">[^\uFDD5\uFDD0\uFDD6\uFDD7\uFDD8\uFDD9\uFDDA]+)",
                "$1" + "</div>\uFDDA\n",
                RegexOptions.Singleline);
            if (relaxed_conformance)
            {
                osis = Regex.Replace(osis, @"\\ss\s+", @"\\s2 ");
                osis = Regex.Replace(osis, @"\\sss\s+", @"\\s3 ");
            }
            osis = Regex.Replace(osis,
                @"\\s2\s+(.+)",
                m => "\uFDDB<div type=\"subSection\"><title>" + m.Groups[1].Value + "</title>");
            osis = Regex.Replace(osis,
                "(\uFDDB<div type=\"subSection\">[^\uFDD5\uFDD0\uFDD6\uFDD7\uFDD8\uFDD9\uFDDA\uFDDB]+)",
                "$1" + "</div>\uFDDB\n",
                RegexOptions.Singleline);
            osis = Regex.Replace(osis,
                @"\\s3\s+(.+)",
                m => "\uFDDC<div type=\"x-subSubSection\"><title>"
                + m.Groups[1].Value
                + "</title>");
            osis = Regex.Replace(osis,
                "(\uFDDC<div type=\"x-subSubSection\">[^\uFDD5\uFDD0\uFDD6\uFDD7\uFDD8\uFDD9\uFDDA\uFDDB\uFDDC]+)",
                "$1" + "</div>\uFDDC\n",
                RegexOptions.Singleline);
            osis = Regex.Replace(osis,
                @"\\s4\s+(.+)",
                m => "\uFDDD<div type=\"x-subSubSubSection\"><title>"
                + m.Groups[1].Value
                + "</title>");
            osis = Regex.Replace(osis,
                "(\uFDDD<div type=\"x-subSubSubSection\">[^\uFDD5\uFDD0\uFDD6\uFDD7\uFDD8\uFDD9\uFDDA\uFDDB\uFDDC\uFDDD]+)",
                "$1" + "</div>\uFDDD\n",
                RegexOptions.Singleline);
            osis = Regex.Replace(osis,
                @"\\s5\s+(.+)",
                m => "\uFDDE<div type=\"x-subSubSubSubSection\"><title>"
                + m.Groups[1].Value
                + "</title>");
            osis = Regex.Replace(osis,
                "(\uFDDE<div type=\"x-subSubSubSubSection\">[^\uFDD5\uFDD0\uFDD6\uFDD7\uFDD8\uFDD9\uFDDA\uFDDB\uFDDC\uFDDD\uFDDE]+)",
                "$1" + "</div>\uFDDE\n",
                RegexOptions.Singleline);

            // \sr_text...
            osis = Regex.Replace(osis,
                @"\\sr\s+(.+)",
                "\uFDD4<title type=\"scope\"><reference>" + "$1</reference></title>");
            // \r_text...
            osis = Regex.Replace(osis,
                @"\\r\s+(.+)",
                "\uFDD4<title type=\"parallel\"><reference type=\"parallel\">"
                + "$1</reference></title>");
            // \rq_text...\rq*
            osis = Regex.Replace(osis,
                @"\\rq\s+(.+?)\\rq\*",
                "<reference type=\"source\">$1</reference>",
                RegexOptions.Singleline);

            // \d_text...
            osis = Regex.Replace(osis,
                @"\\d\s+(.+)",
                "\uFDD4<title canonical=\"true\" type=\"psalm\">" + "$1</title>");

            // \sp_text...
            osis = Regex.Replace(osis, @"\\sp\s+(.+)", @"<speaker>$1</speaker>");

            // \mt#_text...
            osis = Regex.Replace(osis,
                @"\\mt(\d?)\s+(.+)",
                m => "<title "
                + (string.IsNullOrEmpty(m.Groups[1].Value) ? "" : ("level=\"" + m.Groups[1].Value + "\" "))
                + "type=\"main\">"
                + m.Groups[2].Value
                + "</title>");
            // \mte#_text...
            osis = Regex.Replace(osis,
                @"\\mte(\d?)\s+(.+)",
                m => "<title "
                + (string.IsNullOrEmpty(m.Groups[1].Value) ? "" : ("level=\"" + m.Groups[1].Value + "\" "))
                + "type=\"main\" subType=\"x-end\">"
                + m.Groups[2].Value
                + "</title>");


            return osis;
        }

        #region cvt_chapters_and_verses 
        /// <summary>
        /// Converts USFM **Chapter and Verse** tags to OSIS, returning the
        /// processed text as a string.
        /// 
        /// Supported tags: \c, \ca...\ca*, \cl, \cp, \cd, \v, \va...\va*,
        /// \vp...\vp*
        /// </summary>
        /// <param name=\"osis_in">The document as a string</param>
        /// <returns></returns>
        private string cvt_chapters_and_verses(string osis_in)
        {
            string osis = osis_in;

            // \c_#
            osis = Regex.Replace(osis,
                @"\\c\s+([^\s]+)\b(.+?)(?=(\\c\s+|</div type=""book""))",
                m => "\uFDD1<chapter osisID=\"$BOOK$."
                + m.Groups[1].Value
                + "\" sID=\"$BOOK$."
                + m.Groups[1].Value
                + "\"/>"
                + m.Groups[2].Value
                + "<chapter eID=\"$BOOK$."
                + m.Groups[1].Value
                + "\"/>\uFDD3\n",
                RegexOptions.Singleline);

            // \cp_#
            // \ca_#\ca*

            osis = Regex.Replace(osis,
                @"(<chapter [^<]+sID[^<]+/>.+?<chapter eID[^>]+/>)",
                m => replace_chapter_number(m),
                RegexOptions.Singleline);

            // \cl_
            osis = Regex.Replace(osis, @"\\cl\s+(.+)", "\uFDD4<title>" + "$1</title>");

            // \cd_//   <--This // seems to be an error
            osis = Regex.Replace(osis,
                 @"\\cd\b\s+(.+)", "\uFDD4<title type=\"x-description\">" + "$1</title>");

            // \v_#
            osis = Regex.Replace(osis,
                @"\\v\s+([^\s]+)\b\s*(.+?)(?=(\\v\s+|</div" + " type=\"book\"|<chapter eID))",
                m => "\uFDD2<verse osisID=\"$BOOK$.$CHAP$."
                + m.Groups[1].Value
                + "\" sID =\"$BOOK$.$CHAP$."
                + m.Groups[1].Value
                + "\" /> "
                + m.Groups[2].Value
                + "<verse eID=\"$BOOK$.$CHAP$."
                + m.Groups[1].Value
                + "\" />\uFDD2\n",
                RegexOptions.Singleline);

            // \vp_#\vp*
            // \va_#\va*

            osis = Regex.Replace(osis,
                @"(<verse [^<]+sID[^<]+/>.+?<verse eID[^>]+/>)",
                m => replace_verse_number(m),
                RegexOptions.Singleline);

            return osis;
        }

        /// <summary>
        /// Regex helper function to replace chapter numbers from \c_// with
        /// values that appeared in \cp_// and \ca_#\ca*
        /// </summary>
        /// <param name=\"matchObject">a regex match object in which the first element is the chapter text</param>
        /// <returns>the chapter text as a string.</returns>
        private string replace_chapter_number(Match matchObject)
        {
            string ctext = matchObject.Groups[1].Value;
            Match cp = Regex.Match(ctext, @"\\cp\s+(.+?)(?=(\\|\s))");
            if (cp.Success)
            {
                ctext = Regex.Replace(ctext, @"\\cp\s+(.+?)(?=(\\|\s))", "", RegexOptions.Singleline);
                string cp1 = cp.Groups[1].Value;
                ctext = Regex.Replace(ctext, @"""\$BOOK\$\.([^""\.]+)""", "\"$BOOK$." + cp1 + "\"");
            }
            Match ca = Regex.Match(ctext, @"\\ca\s+(.+?)\\ca\*");
            if (ca.Success)
            {
                ctext = Regex.Replace(ctext, @"\\ca\s+(.+?)\\ca\*", "", RegexOptions.Singleline);
                string ca1 = ca.Groups[1].Value;
                ctext = Regex.Replace(ctext,
                    @"(osisID=\""\$BOOK\$\.[^""\.]+)""", "$1 $BOOK$." + ca1 + "\"");
            }
            return ctext;

        }

        /// <summary>
        /// Regex helper function to replace verse numbers from \v_// with
        /// values that appeared in \vp_#\vp* and \va_#\va*
        /// </summary>
        /// <param name=\"matchObject">matchObject-- a regex match object in which the first element is
        /// the verse text</param>
        /// <returns>the verse text as a string.</returns>
        private string replace_verse_number(Match matchObject)
        {
            string vtext = matchObject.Groups[1].Value;
            Match vp = Regex.Match(vtext, @"\\vp\s+(.+?)\\vp\*");
            if (vp.Success)
            {
                vtext = Regex.Replace(vtext, @"\\vp\s+(.+?)\\vp\*", "", RegexOptions.Singleline);
                string vp1 = vp.Groups[1].Value;
                vtext = Regex.Replace(vtext,
                    @"""\$BOOK\$\.\$CHAP\$\.([^""\.]+)""",
                    "\"$BOOK$.$CHAP$." + vp1 + "\"");
            }
            Match va = Regex.Match(vtext, @"\\va\s+(.+?)\\va\*");
            if (va.Success)
            {
                vtext = Regex.Replace(vtext, @"\\va\s+(.+?)\\va\*", "", RegexOptions.Singleline);
                string va1 = va.Groups[1].Value;
                vtext = Regex.Replace(vtext,
                    @"(osisID=""\$BOOK\$\.\$CHAP\$\.[^""\.]+)""",
                    "$1 $BOOK$.$CHAP$." + va1 + "\"");
            }
            return vtext;
        }

        #endregion cvt_chapters_and_verses

        /// <summary>
        /// Converts USFM **Paragraph** tags to OSIS, returning 
        /// Supported tags: \p, \m, \pmo, \pm, \pmc, \pmr, \pi#, \mi, \nb, \cls,
        ///               \li#, \pc, \pr, \ph#, \b
        /// </summary>
        /// <param name=\"osis_in">The document as a string</param>
        /// <returns>the processed text as a string.</returns>
        private string cvt_paragraphs(string osis_in)
        {
            string osis = osis_in;

            string paragraphregex = "pc|pr|m|pmo|pm|pmc|pmr|pi|pi1|pi2|pi3|pi4|pi5|mi|nb";
            if (relaxed_conformance)
                paragraphregex += "|phi|ps|psi|p1|p2|p3|p4|p5";

            // \p(_text...)
            osis = Regex.Replace(osis,
                @"\\p\s+(.*?)(?=(\\(i?m|i?p|lit|cls|tr|p|"
                + paragraphregex
                + @")\b|<chapter eID|<(/?div|p|closer)\b))",
                m => "\uFDD3<p>\n" + m.Groups[1].Value + "\uFDD3</p>\n",
                RegexOptions.Singleline);

            // \pc(_text...)
            // \pr(_text...)
            // \m(_text...)
            // \pmo(_text...)
            // \pm(_text...)
            // \pmc(_text...)
            // \pmr_text...  // deprecated: map to same as \pr
            // \pi#(_Sample text...)
            // \mi(_text...)
            // \nb
            // \phi // deprecated
            // \ps // deprecated
            // \psi // deprecated
            // \p// // deprecated
            Dictionary<string, string> p_type = new Dictionary<string, string>()
        {
            {"pc", "x-center"},
            {"pr", "x-right"},
            {"m", "x-noindent"},
            {"pmo", "x-embedded-opening"},
            {"pm", "x-embedded"},
            {"pmc", "x-embedded-closing"},
            {"pmr", "x-right"},
            {"pi", "x-indented-1"},
            {"pi1", "x-indented-1"},
            {"pi2", "x-indented-2"},
            {"pi3", "x-indented-3"},
            {"pi4", "x-indented-4"},
            {"pi5", "x-indented-5"},
            {"mi", "x-noindent-indented"},
            {"nb", "x-nobreak"},
            {"phi", "x-indented-hanging"},
            {"ps", "x-nobreakNext"},
            {"psi", "x-nobreakNext-indented"},
            {"p1", "x-level-1"},
            {"p2", "x-level-2"},
            {"p3", "x-level-3"},
            {"p4", "x-level-4"},
            {"p5", "x-level-5"},
        };
            osis = Regex.Replace(osis,
                @"\\("
                + paragraphregex
                + @")\s+(.*?)(?=(\\(i?m|i?p|lit|cls|tr|"
                + paragraphregex
                + @")\b|<chapter eID|<(/?div|p|closer)\b))",
                m => "\uFDD3<p type=\""
                + p_type[m.Groups[1].Value]
                + "\">\n"
                + m.Groups[2].Value
                + "\uFDD3</p>\n",
                RegexOptions.Singleline);

            // \cls_text...
            osis = Regex.Replace(osis,
                @"\\m\s+(.+?)(?=(\\(i?m|i?p|lit|cls|tr)\b|<chapter eID|<(/?div|p|closer)\b))",
                m => "\uFDD3<closer>" + m.Groups[1].Value + "\uFDD3</closer>\n",
                RegexOptions.Singleline);

            // \ph#(_text...)
            // \li#(_text...)
            osis = Regex.Replace(osis, @"\\ph\b\s*", "\\li ");
            osis = Regex.Replace(osis, @"\\ph(\d)\b\s*", "\\li$1 ");
            osis = Regex.Replace(osis,
                @"\\li\b\s*(.*?)(?=(["
                + "\uFDD0\uFDD1\uFDD3\uFDD4\uFDE0\uFDE1\uFDD5\uFDD6\uFDD7\uFDD8\uFDD9\uFDDA\uFDDB\uFDDC\uFDDD\uFDDE"
                + @"]|\\li\d?\b|<(lb|title|item|/?div|/?chapter)\b))",
                "<item type=\"x-indent-1\">$1</item>",
                RegexOptions.Singleline);
            osis = Regex.Replace(osis,
                @"\\li(\d)\b\s*(.*?)(?=(["
                + "\uFDD0\uFDD1\uFDD3\uFDD4\uFDE0\uFDE1\uFDD5\uFDD6\uFDD7\uFDD8\uFDD9\uFDDA\uFDDB\uFDDC\uFDDD\uFDDE"
                + @"]|\\li\d?\b|<(lb|title|item|/?div|/?chapter)\b))",
                "<item type=\"x-indent-$1\">$2</item>",
                RegexOptions.Singleline);
            osis = osis.Replace("\n</item>", "</item>\n");
            osis = Regex.Replace(osis,
                "(<item [^\uFDD0\uFDD1\uFDD3\uFDD4\uFDE0\uFDE1\uFDD5\uFDD6\uFDD7\uFDD8\uFDD9\uFDDA\uFDDB\uFDDC\uFDDD\uFDDE]+</item>)",
                "\uFDD3<list>" + "$1" + "</list>\uFDD3",
                RegexOptions.Singleline);

            // \b
            osis = Regex.Replace(osis, @"\\b\b\s?", "<lb type=\"x-p\"/>");


            return osis;
        }

        /// <summary>
        /// Converts USFM **Poetry** tags to OSIS, 
        /// Supported tags: \q#, \qr, \qc, \qs...\qs*, \qa, \qac...\qac*, \qm#, \b
        /// </summary>
        /// <param name=\"osis_in">The document as a string</param>
        /// <returns>returning the processed text as a string.</returns>
        private string cvt_poetry(string osis_in)
        {
            string osis = osis_in;

            // \qa_text...
            osis = Regex.Replace(osis,
                @"\\qa\s+(.+)", "\uFDD4<title type=\"acrostic\">" + "$1</title>");

            // \qac_text...\qac*
            osis = Regex.Replace(osis,
                @"\\qac\s+(.+?)\\qac\*",
                "<hi type=\"acrostic\" >$1 </ hi > ",
                RegexOptions.Singleline);

            // \qs_(Selah)\qs*
            osis = Regex.Replace(osis,
                @"\\qs\b\s(.+?)\\qs\*", "<l type=\"selah\">$1</l>", RegexOptions.Singleline);

            // \q#(_text...)
            osis = Regex.Replace(osis,
                @"\\q\b\s*(.*?)(?=(["
                + "\uFDD0\uFDD1\uFDD3\uFDD4\uFDD5\uFDD6\uFDD7\uFDD8\uFDD9\uFDDA\uFDDB\uFDDC\uFDDD\uFDDE"
                + @"]|\\(q\d?|fig)\b|<(l|lb|title|list|/?div)\b))",
                "<l level=\"1\">$1</l>",
                RegexOptions.Singleline);

            osis = Regex.Replace(osis,
                @"\\q(\d)\b\s*(.*?)(?=(["
                + "\uFDD0\uFDD1\uFDD3\uFDD4\uFDD5\uFDD6\uFDD7\uFDD8\uFDD9\uFDDA\uFDDB\uFDDC\uFDDD\uFDDE"
                + @"]|\\(q\d?|fig)\b|<(l|lb|title|list|/?div)\b))",
                "<l level=\"$1\" >$2 </ l > ",
                RegexOptions.Singleline);

            // \qr_text...
            // \qc_text...
            // \qm#(_text...)
            Dictionary<string, string> qType = new Dictionary<string, string>()
        {
            {"q@", "x-right"},
            {"qc", "x-cente@"},
            {"qm", "x-embedded\" level=\"1"},
            {"qm1", "x-embedded\" level=\"1"},
            {"qm2", "x-embedded\" level=\"2"},
            {"qm3", "x-embedded\" level=\"3"},
            {"qm4", "x-embedded\" level=\"4"},
            {"qm5", "x-embedded\" level=\"5"},
        };
            osis = Regex.Replace(osis,
                @"\\(qr|qc|qm\d)\b\s*(.*?)(?=(["
                + "\uFDD0\uFDD1\uFDD3\uFDD4\uFDD5\uFDD6\uFDD7\uFDD8\uFDD9\uFDDA\uFDDB\uFDDC\uFDDD\uFDDE"
                + @"]|\\(q\d?|fig)\b|<(l|lb|title|list|/?div)\b))",
                m => "<l type=\"" + qType[m.Groups[1].Value] + "\">" + m.Groups[2].Value + "</l>",
                RegexOptions.Singleline);

            osis = osis.Replace("\n</l>", "</l>\n");
            osis = Regex.Replace(osis,
                "(<l [^\uFDD0\uFDD1\uFDD3\uFDD4\uFDD5\uFDD6\uFDD7\uFDD8\uFDD9\uFDDA\uFDDB\uFDDC\uFDDD\uFDDE]+</l>)",
                @"<lg>\1</lg>",
                RegexOptions.Singleline);

            // \b
            osis = Regex.Replace(osis,
                "(<lg>.+?</lg>)",
                m => m.Groups[1].Value.Replace("<lb type=\"x-p\"/>", "</lg><lg>"),
                RegexOptions.Singleline);
            // re-handle \b that occurs within <lg>

            return osis;
        }

        /// <summary>
        /// Converts USFM **Table** tags to OSIS
        /// </summary>
        /// <param name=\"osis_in">The document as a string</param>
        /// <returns>the processed text as a string</returns>
        private string cvt_tables(string osis_in)
        {
            string osis = osis_in;
            //\tr_
            osis = Regex.Replace(osis,
                @"\\tr\b\s*(.*?)(?=(["
                + "\uFDD0\uFDD1\uFDD3\uFDD4"
                + @"]|\\tr\s|<(lb|title)\b))",
                "<row>$1</row>",
                RegexOptions.Singleline);

            //\th#_text...
            //\thr#_text...
            //\tc#_text...
            //\tcr#_text...
            Dictionary<string, string> t_type = new Dictionary<string, string>()
            {
                { "th", " role=\"label\"" },
                { "th@", " role=\"label\" type=\"x-right\"" },
                { "tc", "" },
                { "tc@", " type=\"x-right\"" },
            };
            osis = Regex.Replace(osis,
            @"\\(thr?|tcr?)\d*\b\s*(.*?)(?=(\\t[hc]|</row))",
            m => "<cell" + t_type[m.Groups[1].Value] + ">" + m.Groups[2].Value + "</cell>",
            RegexOptions.Singleline);

            osis = Regex.Replace(osis,
                @"(<row>.*?</row>)(?=(["
                + "\uFDD0\uFDD1\uFDD3\uFDD4"
                + @"]|\\tr\s|<(lb|title)\b))",
                "<table>$1</table>",
                RegexOptions.Singleline);

            return osis;
        }

        /// <summary>
        /// Convert note-internal USFM tags to OSIS
        /// </summary>
        /// <param name="note_in">The note as a string</param>
        /// <returns>the note as a string</returns>
        private string process_note(string note_in)
        {
            string note = note_in;

            note = note.Replace("\n", " ");

            //\fdc_refs...\fdc*
            note = Regex.Replace(note, @"\\fdc\b\s(.+?)\\fdc\b\*", "<seg editions=\"dc\">$1</seg>");

            //\fq_
            note = Regex.Replace(note,
                @"\\fq\b\s(.+?)(?=(\\f|" + "\uFDDF))",
                "\uFDDF" + "<catchWord>$1</catchWord>");

            //\fqa_
            note = Regex.Replace(note,
                @"\\fqa\b\s(.+?)(?=(\\f|" + "\uFDDF))",
                "\uFDDF" + "<rdg type=\"alternate\">$1</rdg>");

            //\ft_
            note = Regex.Replace(note, @"\\ft\s", "");

            //\fr_##SEP##
            note = Regex.Replace(note,
                @"\\fr\b\s(.+?)(?=(\\f|" + "\uFDDF))",
                "\uFDDF" + "<reference type=\"annotateRef\">$1</reference>");

            //\fk_
            note = Regex.Replace(note,
                @"\\fk\b\s(.+?)(?=(\\f|" + "\uFDDF))",
                "\uFDDF" + "<catchWord>$1</catchWord>");
            //\fl_
            note = Regex.Replace(note,
                @"\\fl\b\s(.+?)(?=(\\f|" + "\uFDDF))", "\uFDDF" + "<label>$1</label>");

            //\fp_
            note = Regex.Replace(note, @"\\fp\b\s(.+?)(?=(\\fp|$))", "<p>$1</p>");
            note = Regex.Replace(note, @"(<note\b[^>]*?>)(.*?)<p>", "$1<p>$2</p><p>");

            //\fv_
            note = Regex.Replace(note,
                @"\\fv\b\s(.+?)(?=(\\f|" + "\uFDDF))",
                "\uFDDF" + "<hi type=\"super\">$1</hi>");

            //\fq*,\fqa*,\ft*,\fr*,\fk*,\fl*,\fp*,\fv*
            note = Regex.Replace(note, @"\\f(q|qa|t|r|k|l|p|v)\*", "");

            note = note.Replace("\uFDDF", "");

            return note;
        }

        /// <summary>
        /// Converts USFM **Footnote** tags to OSIS
        /// 
        /// Supported tags: \f...\f*, \fe...\fe*, \fr, \fk, \fq, \fqa, \fl, \fp,
        ///                 \fv, \ft, \fdc...\fdc*, \fm...\fm*
        /// </summary>
        /// <param name=\"osis_in">The document as a string</param>
        /// <returns>the processed text as a string.</returns>
        private string cvt_footnotes(string osis_in)
        {
            string osis = osis_in;

            //\f_+_...\f*
            osis = Regex.Replace(osis,
                @"\\f\s+([^\s\\]+)?\s*(.+?)\s*\\f\*",
                m => "<note"
                + (
                (m.Groups[1].Value == "-") ? (" n=\"\"") :
                (m.Groups[1].Value == "+") ? "" : (" n=\"" + m.Groups[1].Value + "\""))
            + " placement=\"foot\">"
            + m.Groups[2].Value
            + "\uFDDF</note>",
            RegexOptions.Singleline);

            //\fe_+_...\fe*
            osis = Regex.Replace(osis,
                @"\\fe\s+([^\s\\]+?)\s*(.+?)\s*\\fe\*",
                m => "<note"
                + (
                (m.Groups[1].Value == "-") ? (" n=\"\"") :
                (m.Groups[1].Value == "+") ? "" : (" n=\"" + m.Groups[1].Value + "\""))
                + " placement=\"end\">"
                + m.Groups[2].Value
                + "\uFDDF</note>",
                RegexOptions.Singleline);

            osis = Regex.Replace(osis,
                @"(<note\b[^>]*?>.*?</note>)",
                m => process_note(m.Groups[1].Value),
                RegexOptions.Singleline);

            //\fm_...\fm*
            osis = Regex.Replace(osis, @"\\fm\b\s(.+?)\\fm\*", "<hi type=\"super\">$1</hi>");


            return osis;
        }

        /// <summary>
        /// Convert cross-reference note-internal USFM tags to OSIS
        /// </summary>
        /// <param name="note_in">The cross-reference note as a string</param>
        /// <returns>the cross-reference note as a string</returns>
        private string process_xref(string note_in)
        {
            string note = note_in;

            note = note.Replace("\n", " ");

            // \xot_refs...\xot*
            note = Regex.Replace(note,
              @"\\xot\b\s(.+?)\\xot\b\*", "\uFDDF" + "<seg editions=\"ot\">$1</seg>");

            // \xnt_refs...\xnt*
            note = Regex.Replace(note,
              @"\\xnt\b\s(.+?)\\xnt\b\*", "\uFDDF" + "<seg editions=\"nt\">$1</seg>");

            // \xdc_refs...\xdc*
            note = Regex.Replace(note,
              @"\\xdc\b\s(.+?)\\xdc\b\*", "\uFDDF" + "<seg editions=\"dc\">$1</seg>");

            // \xq_
            note = Regex.Replace(note,
              @"\\xq\b\s(.+?)(?=(\\x|" + "\uFDDF))",
                "\uFDDF" + "<catchWord>$1</catchWord>");

            // \xo_##SEP##
            note = Regex.Replace(note,
              @"\\xo\b\s(.+?)(?=(\\x|" + "\uFDDF))",
                "\uFDDF" + "<reference type=\"annotateRef\">$1</reference>");

            // \xk_
            note = Regex.Replace(note,
              @"\\xk\b\s(.+?)(?=(\\x|" + "\uFDDF))",
                "\uFDDF" + "<catchWord>$1</catchWord>");

            // \xt_  // This isn't guaranteed to be *the* reference, but it's a good guess.
            note = Regex.Replace(note,
              @"\\xt\b\s(.+?)(?=(\\x|" + "\uFDDF))",
                "\uFDDF" + "<reference>$1</reference>");

            if (relaxed_conformance)
            {
                // TODO: move this to a concorance/index-specific section?
                // \xtSee..\xtSee*: Concordance and Names Index markup for an
                //                  alternate entry target reference.
                note = Regex.Replace(note,
                  @"\\xtSee\b\s(.+?)\\xtSee\b\*",
                    "\uFDDF" + "<reference osisRef=\"$1\">See: $1</reference>");

                // \xtSeeAlso...\xtSeeAlso: Concordance and Names Index markup for
                //                          an additional entry target reference.
                note = Regex.Replace(note,
                  @"\\xtSeeAlso\b\s(.+?)\\xtSeeAlso\b\*",
                    "\uFDDF" + "<reference osisRef=\"$1\">See also: $1</reference>");
            }

            // \xq*,\xt*,\xo*,\xk*
            note = Regex.Replace(note, @"\\x(q|t|o|k)\*", "");

            note = note.Replace("\uFDDF", "");
            return note;
        }


        /// <summary>
        /// onverts USFM **Cross Reference** tags to OSIS
        /// 
        /// Supported tags: \\x...\\x*, \\xo, \\xk, \\xq, \\xt, \\xot...\\xot*,
        ///                 \\xnt...\\xnt*, \\xdc...\\xdc*
        /// </summary>
        /// <param name=\"osis_in">The document as a string</param>
        /// <returns>the processed text as a string</returns>
        private string cvt_cross_references(string osis_in)
        {
            string osis = osis_in;

            // \x_+_...\x*
            osis = Regex.Replace(osis,
              @"\\x\s+([^\s]+?)\s+(.+?)\s*\\x\*",
                m => "<note"
                + (
                (m.Groups[1].Value == "-") ? (" n=\"\"") :
                (m.Groups[1].Value == "+") ? "" : (" n=\"" + m.Groups[1].Value + "\""))
            + " type=\"crossReference\">"
            + m.Groups[2].Value
            + "\uFDDF</note>",
            RegexOptions.Singleline);

            osis = Regex.Replace(osis,
                @"(<note [^>]*?type=""crossReference""[^>]*>.*?</note>)",
                m => process_xref(m.Groups[1].Value));

            return osis;
        }

        /// <summary>
        /// Converts USFM **Special Text** tags to OSIS
        /// 
        /// Supported tags: \add...\add*, \bk...\bk*, \dc...\dc*, \k...\k*, \lit,
        ///                 \nd...\nd*, \ord...\ord*, \pn...\pn*, \qt...\qt*,
        ///                 \sig...\sig*, \sls...\sls*, \tl...\tl*, \wj...\wj*
        /// </summary>
        /// <param name=\"osis_in">The document as a string</param>
        /// <returns>the processed text as a string</returns>
        private string cvt_special_text(string osis_in)
        {
            string osis = osis_in;

            // \add_...\add*
            osis = Regex.Replace(osis,
              @"\\add\s+(.+?)\\add\*",
                "<transChange type=\"added\">$1</transChange>",
                RegexOptions.Singleline);

            // \wj_...\wj*
            osis = Regex.Replace(osis,
              @"\\wj\s+(.+?)\\wj\*",
                "<q who=\"Jesus\" marker=\"\">$1</q>",
                RegexOptions.Singleline);

            // \nd_...\nd*
            osis = Regex.Replace(osis,
              @"\\nd\s+(.+?)\\nd\*", "<divineName>$1</divineName>", RegexOptions.Singleline);

            // \pn_...\pn*
            osis = Regex.Replace(osis, @"\\pn\s+(.+?)\\pn\*", "<name>$1</name>", RegexOptions.Singleline);

            // \qt_...\qt* // TODO:should this be <q>?
            osis = Regex.Replace(osis,
              @"\\qt\s+(.+?)\\qt\*",
                "<seg type=\"otPassage\">$1</seg>",
                RegexOptions.Singleline);

            // \sig_...\sig*
            osis = Regex.Replace(osis,
              @"\\sig\s+(.+?)\\sig\*", "<signed>$1</signed>", RegexOptions.Singleline);

            // \ord_...\ord*
            osis = Regex.Replace(osis,
              @"\\ord\s+(.+?)\\ord\*",  // semantic incongruity:
                "<hi type=\"super\">$1</hi>",  // (ordinal -> superscript)
                RegexOptions.Singleline);

            // \tl_...\tl*
            osis = Regex.Replace(osis,
              @"\\tl\s+(.+?)\\tl\*", "<foreign>$1</foreign>", RegexOptions.Singleline);

            // \bk_...\bk*
            osis = Regex.Replace(osis,
              @"\\bk\s+(.+?)\\bk\*",
                "<name type=\"x-workTitle\">$1</name>",
                RegexOptions.Singleline);

            // \k_...\k*
            osis = Regex.Replace(osis,
              @"\\k\s+(.+?)\\k\*", "<seg type=\"keyword\">$1</seg>", RegexOptions.Singleline);

            // \lit
            osis = Regex.Replace(osis,
              @"\\lit\s+(.*?)(?=(\\(i?m|i?p|nb|lit|cls|tr)\b|<(chapter eID|/?div|p|closer)\b))",
                m => "\uFDD3<p type=\"x-liturgical\">\n" + m.Groups[1].Value + "\uFDD3</p>\n",
                RegexOptions.Singleline);

            // \dc_...\dc*
            // TODO: Find an example---should this really be transChange?
            osis = Regex.Replace(osis,
              @"\\dc\b\s*(.+?)\\dc\*",
                "<transChange type=\"added\" editions=\"dc\">$1</transChange>",
                RegexOptions.Singleline);

            // \sls_...\sls*
            // TODO: find a better mapping than <foreign>?
            osis = Regex.Replace(osis,
              @"\\sls\b\s*(.+?)\\sls\*", "<foreign>/1</foreign>", RegexOptions.Singleline);

            if (relaxed_conformance)
            {
                // \addpn...\addpn*
                osis = Regex.Replace(osis,
                  @"\\addpn\s+(.+?)\\addpn\*",
                    "<hi type=\"x-dotUnderline\">$1</hi>",
                    RegexOptions.Singleline);
                // \k// // TODO: unsure of this tag's purpose
                osis = Regex.Replace(osis,
                  @"\\k1\s+(.+?)\\k1\*",
                    "<seg type=\"keyword\" n=\"1\">$1</seg>",
                    RegexOptions.Singleline);
                osis = Regex.Replace(osis,
              @"\\k2\s+(.+?)\\k2\*",
              "<seg type=\"keyword\" n=\"2\">$1</seg>",
                RegexOptions.Singleline);
                osis = Regex.Replace(osis,
              @"\\k3\s+(.+?)\\k3\*",
              "<seg type=\"keyword\" n=\"3\">$1</seg>",
                RegexOptions.Singleline);
                osis = Regex.Replace(osis,
              @"\\k4\s+(.+?)\\k4\*",
              "<seg type=\"keyword\" n=\"4\">$1</seg>",
                RegexOptions.Singleline);
                osis = Regex.Replace(osis,
              @"\\k5\s+(.+?)\\k5\*",
              "<seg type=\"keyword\" n=\"5\">$1</seg>",
                RegexOptions.Singleline);
            }

            return osis;
        }

        /// <summary>
        /// Converts USFM **Character Styling** tags to OSIS, returning the
        /// processed text as a string.
        /// Supported tags: \em...\em *, \bd...\bd *, \it...\it *, \bdit...\bdit *,
        ///                 \no...\no*, \sc...\sc*
        /// </summary>
        /// <param name=\"osis_in">The document as a string</param>
        /// <returns></returns>
        private string cvt_character_styling(string osis_in)
        {
            string osis = osis_in;
            // \em_...\em*
            osis = Regex.Replace(osis,
                @"\\em\s+(.+?)\\em\*", "<hi type=\"emphasis\">1</hi>", RegexOptions.Singleline
            );

            // \bd_...\bd*
            osis = Regex.Replace(osis,
                @"\\bd\s+(.+?)\\bd\*", "<hi type=\"bold\">1</hi>", RegexOptions.Singleline
            );

            // \it_...\it*
            osis = Regex.Replace(osis,
                @"\\it\s+(.+?)\\it\*", "<hi type=\"italic\">1</hi>", RegexOptions.Singleline
            );

            // \bdit_...\bdit*
            osis = Regex.Replace(osis,
                @"\\bdit\s+(.+?)\\bdit\*",
                "<hi type=\"bold\"><hi type=\"italic\">1</hi></hi>",
                RegexOptions.Singleline
            );

            // \no_...\no*
            osis = Regex.Replace(osis,
                @"\\no\s+(.+?)\\no\*", "<hi type=\"normal\">1</hi>", RegexOptions.Singleline
            );

            // \sc_...\sc*
            osis = Regex.Replace(osis,
                @"\\sc\s+(.+?)\\sc\*",
                "<hi type=\"small - caps\">1</hi>",
                RegexOptions.Singleline
            );


            return osis;
        }

        /// <summary>
        /// Converts USFM **Spacing and Breaks** tags to OSIS, returning the
        /// processed text as a string.
        ///
        /// Supported tags: ~, //, \pb
        /// </summary>
        /// <param name=\"osis_in">The document as a string</param>
        /// <returns></returns>
        private string cvt_spacing_and_breaks(string osis_in)
        {
            string osis = osis_in;

            osis = osis.Replace("~", "\u00A0");

            // //
            osis = osis.Replace("//", "<lb type=\"x-optional\"/>");

            // \pb
            osis = Regex.Replace(osis, @"\\pb\s*", "<milestone type=\"pb\"/>\n", RegexOptions.Singleline);


            return osis;
        }


        #region cvt_special_features
        /// <summary>
        /// 
        /// </summary>
        /// <param name=\"osis_in">The document as a string</param>
        /// <returns></returns>
        private string cvt_special_features(string osis_in)
        {
            string osis = osis_in;

            osis = Regex.Replace(osis,
                @"\\fig\b\s+([^\|]*)\s*\|([^\|]*)\s*\|([^\|]*)\s*\|([^\|]*)\s*\|([^\|]*)\s*\|([^\|]*)\s*\|([^\\]*)\s*\\fig\*",
                m => make_figure(m));

            // \ndx_...\ndx*
            // TODO: tag with x-glossary instead of <index/>? Is <index/>
            // containerable?
            osis = Regex.Replace(osis,
                @"\\ndx\s+(.+?)(\s*)\\ndx\*",
                "$1<index index=\"Index\" level1=\"$1\"/>$2",
                RegexOptions.Singleline);

            // \pro_...\pro*
            osis = Regex.Replace(osis,
                @"([^\s]+)(\s*)\\pro\s+(.+?)(\s*)\\pro\*",
                "<w xlit=\"$3\">$1</w>$2$4",
                RegexOptions.Singleline);

            // \w_...\w*
            osis = Regex.Replace(osis,
                @"\\w\s+(.+?)(?!\|)\\w\*",
                m => process_w(m),
                RegexOptions.Singleline);
            /*            osis = Regex.Replace(osis,
                            @"\\w\s+(.+?)(\s*)\\w\*",
                            "$1<index index=\"Glossary\" level1=\"$1\"/>$2",
                            RegexOptions.Singleline);*/

            // \wg_...\wg*
            osis = Regex.Replace(osis,
                @"\\wg\s+(.+?)(\s*)\\wg\*",
                "$1<index index=\"Greek\" level1=\"$1\"/>$2",
                RegexOptions.Singleline);

            // \wh_...\wh*
            osis = Regex.Replace(osis,
                @"\\wh\s+(.+?)(\s*)\\wh\*",
                "$1<index index=\"Hebrew\" level1=\"$1\"/>$2",
                RegexOptions.Singleline);

            if (relaxed_conformance)
            {
                // \wr...\wr*
                osis = Regex.Replace(osis,
                    @"\\wr\s+(.+?)(\s*)\\wr\*",
                    "$1<index index=\"Reference\" level1=\"$1\"/>$2",
                    RegexOptions.Singleline);
            }

            return osis;
        }

        /// <summary>
        /// Handel \w \w* 
        /// </summary>
        /// <param name="m2"></param>
        /// <returns></returns>
        private string process_w(Match m2)
        {
            return m2.Groups[0].Value.Contains("|") ?
                "<w lemma=\"" +
                Regex.Match(m2.Groups[0].Value, @"\\w\s+(.+?)\|(.+?)\=""(.+?)""(\s*)\\w\*").Groups[2] + ":" +
                Regex.Match(m2.Groups[0].Value, @"\\w\s+(.+?)\|(.+?)\=""(.+?)""(\s*)\\w\*").Groups[3] + "\">" +
                Regex.Match(m2.Groups[0].Value, @"\\w\s+(.+?)\|(.+?)\=""(.+?)""(\s*)\\w\*").Groups[1] + "</w>" :
                "<w>" + m2.Groups[1].Value + "</w>";
        }

        /// <summary>
        /// Regex helper function to convert USFM \fig to OSIS <figure/>
        /// </summary>
        /// <param name="matchObject">matchObject -- a regex match object containing the elements of a
        /// USFM \fig tag
        /// </param>
        /// <returns>the OSIS element as a string</returns>
        private string make_figure(Match matchObject)
        {
            string fig_desc = (matchObject.Groups.Count > 0) ? matchObject.Groups[0].Value : string.Empty;
            string fig_file = (matchObject.Groups.Count > 1) ? matchObject.Groups[1].Value : string.Empty;
            string fig_size = (matchObject.Groups.Count > 2) ? matchObject.Groups[2].Value : string.Empty;
            string fig_loc = (matchObject.Groups.Count > 3) ? matchObject.Groups[3].Value : string.Empty;
            string fig_copy = (matchObject.Groups.Count > 4) ? matchObject.Groups[4].Value : string.Empty;
            string fig_cap = (matchObject.Groups.Count > 5) ? matchObject.Groups[5].Value : string.Empty;
            string fig_ref = (matchObject.Groups.Count > 6) ? matchObject.Groups[6].Value : string.Empty;

            string figure = "<figure";
            if (string.IsNullOrEmpty(fig_file))
                figure += " src=\"" + fig_file + "\"";
            if (string.IsNullOrEmpty(fig_size))
                figure += " size=\"" + fig_size + "\"";
            if (string.IsNullOrEmpty(fig_copy))
                figure += " rights=\"" + fig_copy + "\"";
            // TODO)) implement parsing in osisParse(Bible reference string)
            // if(fig_ref))
            //    figure += " annotateRef=\"" + osisParse(fig_ref) + "\""
            figure += ">\n";
            if (string.IsNullOrEmpty(fig_cap))
                figure += "<caption>\" + fig_cap + \"</caption>\n";
            if (string.IsNullOrEmpty(fig_ref))
                figure += "<reference type=\"annotateRef\">" + fig_ref + "</reference>\n";
            if (string.IsNullOrEmpty(fig_desc))
                figure += "<!-- fig DESC - \" + fig_desc + \" -->\n";
            if (string.IsNullOrEmpty(fig_loc))
                figure += "<!-- fig LOC - \" + fig_loc + \" -->\n";
            figure += "</figure>";
            return figure;
        }

        #endregion cvt_special_features


        #region cvt_peripherals
        /// <summary>
        /// 
        /// </summary>
        /// <param name=\"osis_in">The document as a string</param>
        /// <returns></returns>
        private string cvt_peripherals(string osis_in)
        {
            string osis = osis_in;

            osis = Regex.Replace(osis,
                    @"\\periph\s+([^"
                    + "\n"
                    + @"]+)\s*"
                    + "\n"
                    + @"(.+?)(?=(</div type=""book"">|\\periph\s+))",
                    m => tag_periph(m),
                    RegexOptions.Singleline);

            return osis;
        }

        /// <summary>
        /// Regex helper function to tag peripherals
        /// </summary>
        /// <param name="matchObject">a regex match object containing the peripheral type and contents</param>
        /// <returns>a div tag - encapsulated string</returns>
        private string tag_periph(Match matchObject)
        {
            string periph_type = matchObject.Groups[1].Value;
            string contents = matchObject.Groups[2].Value;

            string periph = "<div type=\"";

            if (PERIPHERALS.Keys.Contains(periph_type))
                periph += PERIPHERALS[periph_type];
            else if (INTRO_PERIPHERALS.Keys.Contains(periph_type))
                periph += "introduction\" subType=\"x-" + INTRO_PERIPHERALS[periph_type];
            else
                periph += "x-unknown";
            periph += "\">\n" + contents + "</div>\n";

            return periph;
        }

        #endregion cvt_peripherals

        /// <summary>
        /// Converts USFM **Study Bible Content** tags to OSIS
        /// 
        /// Supported tags: \ef...\ef*, \ex...\ex*, \esb...\esbe, \cat
        /// </summary>
        /// <param name=\"osis_in">The document as a string</param>
        /// <returns>the processed text as a string</returns>
        private string cvt_study_bible_content(string osis_in)
        {
            string osis = osis_in;

            // \ef...\ef*
            osis = Regex.Replace(osis,
                @"\\ef\s+([^\s\\]+?)\s*(.+?)\s*\\ef\*",
                m => "<note"
                + (
                (m.Groups[1].Value == "-") ? (" n=\"\"") :
                (m.Groups[1].Value == "+") ? "" : (" n=\"" + m.Groups[1].Value + "\""))
                + " type=\"study\">"
                + m.Groups[2].Value
                + "\uFDDF</note>",
                RegexOptions.Singleline);

            osis = Regex.Replace(osis,
                @"(<note\b[^>]*?>.*?</note>)",
                m => process_note(m.Groups[1].Value),
                RegexOptions.Singleline);

            // \ex...\ex*
            osis = Regex.Replace(osis,
                @"\\ex\s+([^\s]+?)\s+(.+?)\s*\\ex\*",
                m => "<note"
                + (
                (m.Groups[1].Value == "-") ? (" n=\"\"") :
                (m.Groups[1].Value == "+") ? "" : (" n=\"" + m.Groups[1].Value + "\""))
                + " type=\"crossReference\" subType=\"x-study\"><reference>"
                + m.Groups[2].Value
                + "</reference>\uFDDF</note>",
                RegexOptions.Singleline);

            osis = Regex.Replace(osis,
                @"(<note [^>]*?type=""crossReference""[^>]*>.*?</note>)",
                m => process_xref(m.Groups[1].Value),
                RegexOptions.Singleline);

            // \esb...\esbex
            // TODO: this likely needs to go much earlier in the process
            osis = Regex.Replace(osis,
                @"\\esb\b\s*(.+?)\\esbe\b\s*",
                "\uFDD5<div type=\"x-sidebar\">" + "$1" + "</div>\uFDD5\n",
                RegexOptions.Singleline);

            // \cat_<TAG>\cat*
            osis = Regex.Replace(osis,
                @"\\cat\b\s+(.+?)\\cat\*", "<index index=\"category\" level1=\"$1\"/>");

            return osis;
        }

        /// <summary>
        /// "Converts USFM **\z namespace** tags to OSIS
        /// 
        /// Supported tags: \z&lt;Extension&gt;
        /// </summary>
        /// <param name=\"osis_in">The document as a string</param>
        /// <returns>the processed text as a string</returns>
        private string cvt_private_use_extensions(string osis_in)
        {
            string osis = osis_in;
            // -- We can't really know what these mean, but will preserve them as
            // -- <milestone/> elements.

            // publishing assistant markers
            // \zpa-xb...\zpa-xb* : \periph Book
            // \zpa-xc...\zpa-xc* : \periph Chapter
            // \zpa-xv...\zpa-xv* : \periph Verse
            // \zpa-xd...\zpa-xd* : \periph Description
            // TODO: Decide how these should actually be encoded. In lieu of that,
            // these can all be handled by the default \z Namespace handlers:

            // \z{X}...\z{X}*
            osis = Regex.Replace(osis,
                @"\\z([^\s]+)\s(.+?)(\\z\1\*)",
                "<seg type=\"x-$1\">$2</seg>",
                RegexOptions.Singleline);

            // \z{X}
            osis = Regex.Replace(osis, @"\\z([^\s]+)", "<milestone type=\"x-usfm-z-$1\"/>");

            return osis;
        }

        #region  process_osisIDs
        /// <summary>
        /// Perform postprocessing on an OSIS document, returning the processed
        /// text as a string.
        /// Recurses through chapter & verses, substituting acutal book IDs &
        /// chapter numbers for placeholders.
        /// </summary>
        /// <param name=\"osis_in">The document as a string</param>
        /// <returns></returns>
        private string process_osisIDs(string osis_in)
        {
            string osis = osis_in;

            // TODO: add support for subverses, including in ranges/series,
            // e.g. Matt.1.1!b-Matt.2.5,Matt.2.7!a

            // TODO: make sure that descending ranges generate invalid markup
            //       (osisID="") expand verse ranges, series

            osis = Regex.Replace(osis,
                @"\$BOOK\$\.\$CHAP\$\.(\d+-\d+)""",
                m => expand_range(m.Groups[1].Value) + '"');

            osis = Regex.Replace(osis,
                @"\$BOOK\$\.\$CHAP\$\.(\d+(,\d+)+)""",
                m => expand_series(m.Groups[1].Value) + "\"");
            // fill in book & chapter values
            string[] book_chunks = osis.Split("\uFDD0");
            osis = string.Empty;
            foreach (string b_c in book_chunks)
            {
                string bc = b_c;
                Match match = Regex.Match(bc, @"<div type=""book"" osisID=""([^""]+?)""");
                if (match.Success)
                {
                    string book_value = match.Groups[1].Value;
                    bc = bc.Replace("$BOOK$", book_value);
                    string[] chap_chunks = bc.Split("\uFDD1");
                    string newbc = string.Empty;
                    foreach (string c_c in chap_chunks)
                    {
                        string cc = c_c;
                        Match match1 = Regex.Match(cc, @"<chapter osisID=""[^\.""]+\.([^""]+)");
                        // chap_value = re.search(r'<chapter osisID="[^\."]+\.([^"]+)', cc)
                        if (match1.Success)
                        {
                            string chap_value = match1.Groups[1].Value;
                            cc = cc.Replace("$CHAP$", chap_value);
                        }
                        newbc += cc;
                    }
                    bc = newbc;
                }
                osis += bc;
            }

            return osis;
        }

        /// <summary>
        /// Expands a verse range into its constituent verses as a string
        /// </summary>
        /// <param name=\"v_range">A string of the lower & upper bounds of the range, with a hypen in between</param>
        private string expand_range(string v_range)
        {
            List<string> osisID = new List<string>();

            MatchCollection matches = Regex.Matches(@"\d+", v_range);
            List<int> range = new List<int>();
            foreach (Match match in matches)
            {
                range.Add(int.Parse(match.Value));
            }
            foreach (int n in Enumerable.Range(range[0], range[1] + 1))
                osisID.Add("$BOOK$.$CHAP$." + n.ToString());

            return string.Join(" ", osisID);
        }

        /// <summary>
        /// Expands a verse series (list) into its constituent verses as a string
        /// </summary>
        /// <param name="v_series">A comma-separated list of verses</param>
        /// <returns></returns>
        private string expand_series(string v_series)
        {
            List<string> osisID = new List<string>();

            MatchCollection matches = Regex.Matches(@"\d+", v_series);
            List<int> range = new List<int>();
            foreach (Match match in matches)
            {
                range.Add(int.Parse(match.Value));
            }
            foreach (int n in range)
                osisID.Add("$BOOK$.$CHAP$." + n.ToString());
            return string.Join(" ", osisID);
        }

        #endregion  process_osisIDs

        /// <summary>
        /// Perform postprocessing on an OSIS document, returning the processed
        /// text as a string.
        /// Reorders elements, strips non-characters, and cleans up excess spaces
        /// & newlines
        /// </summary>
        /// <param name=\"osis_in">The document as a string</param>
        /// <returns></returns>
        private string osis_reorder_and_cleanup(string osis_in)
        {
            string osis = osis_in;

            // assorted re-orderings
            osis = Regex.Replace(osis,
                @"(\uFDD3<chapter eID=.+?\n)(<verse eID=.+?>\uFDD2)\n?",
                "$2" + "\n" + "$1");
            osis = Regex.Replace(osis,
                @"([\uFDD5\uFDD6\uFDD7\uFDD8\uFDD9]</div>)([^\uFDD5\uFDD6\uFDD7\uFDD8\uFDD9]*<chapter eID.+?>)",
                "$2$1");
            osis = Regex.Replace(osis,
                @"(\uFDD3</p>\n?\uFDD3<p>)\n?(<verse eID=.+?>\uFDD2)\n?",
                "$2" + "\n" + "$1" + "\n");
            osis = Regex.Replace(osis, "\n(<verse eID=.+?>\uFDD2)", "$1" + "\n");
            osis = Regex.Replace(osis,
                @"\n*(<l.+?>)(<verse eID=.+?>[\uFDD2\n]*<verse osisID=.+?>)", "$2$1");
            osis = Regex.Replace(osis, "(</l>)(<note .+?</note>)", "$2$1");

            // delete attributes from end tags (since they are invalid)
            osis = Regex.Replace(osis, @"(</[^\s>]+) [^>]*>", "$1>");
            osis = osis.Replace("<lb type=\"x-p\"/>", "<lb/>");

            // delete Unicode non-characters
            foreach (char c in "\uFDD0\uFDD1\uFDD2\uFDD3\uFDD4\uFDD5\uFDD6\uFDD7\uFDD8\uFDD9\uFDDA\uFDDB\uFDDC\uFDDD\uFDDE\uFDDF\uFDE0\uFDE1\uFDE2\uFDE3\uFDE4\uFDE5\uFDE6\uFDE7\uFDE8\uFDE9\uFDEA\uFDEB\uFDEC\uFDED\uFDEE\uFDEF")
                osis = osis.Replace(c, ' ');

            foreach (string end_block in new string[]{
                "p",
            "div",
            "note",
            "l",
            "lg",
            "chapter",
            "verse",
            "head",
            "title",
            "item",
            "list" })
            {
                osis = Regex.Replace(osis, @"\s+</" + end_block + ">", "</" + end_block + ">\n");
                osis = Regex.Replace(osis,
                    @"\s+<" + end_block + "( eID=[^/>]+/>)",
                    "<" + end_block + "$1" + "\n");
            }
            osis = Regex.Replace(osis, @" +((</[^>]+>)+) *", "$1 ");

            // strip extra spaces & newlines
            osis = Regex.Replace(osis, @"  +", " ");
            osis = Regex.Replace(osis, @" ?\n\n+", "\n");

            return osis;
        }


    }
}
