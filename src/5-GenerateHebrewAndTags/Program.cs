using System.Reflection.PortableExecutable;

internal class Program
{
    enum STATE
    {
        START,
        STRONG,
        HEBREW
    }
    private string currentVerseRef = string.Empty;
    private Dictionary<int, VerseWord> verseWords = null;

    private string SourceFolder = @"C:\Users\samim\Documents\MyProjects\STEP\AutoTagging\BibleFiles\SourcesFiles";
    private string outputFolder = @"C:\Users\samim\Documents\MyProjects\STEP\AutoTagging\BibleFiles\IntermediateFiles";
    private string sourceText = @"HebOT+GkAdds+dStrongs-2022f.txt";
    private string outputHebrewText = "hebrew.txt";
    private string outputTagsFile = "hebrew_tags.txt";
    private string ExceptionsFile = "TagsExceptions.txt";

    private static void Main(string[] args)
    {
        Program p = new Program();
        p.ReadBible();
    }

    public void ReadBible()
    {
        using (StreamReader reader = new StreamReader(Path.Combine(SourceFolder, sourceText)))
        {
            using (StreamWriter outputFileH = new StreamWriter(Path.Combine(outputFolder, outputHebrewText)))
            {
                using (StreamWriter outputFileT = new StreamWriter(Path.Combine(outputFolder, outputTagsFile)))
                {
                    using (StreamWriter outputFileExcept = new StreamWriter(Path.Combine(outputFolder, ExceptionsFile)))
                    {
                        while (reader.Peek() >= 0)
                        {
                            var line = reader.ReadLine();
                            #region Skip the header
                            if (string.IsNullOrEmpty(line))
                                continue;
                            int c = line.Count(f => (f == '\t'));
                            if (c != 14)
                            {
                                continue;
                            }

                            if (line.StartsWith("Ref in") || line.StartsWith("======"))
                                continue;
                            #endregion Skip the header

                            if (line.Contains("Jos.021.034") || line.Contains("Jos.021.035") || line.Contains("Jos.021.036") || line.Contains("Jos.021.037"))
                            {
                                int x = 0;
                            }

                            string[] lines = ParseLine(line);

                            if (lines != null)
                            {
                                string verseRef = lines[2];
                                //if (verseRef.ToUpper().Contains("JOS"))
                                //return;
                                outputFileH.WriteLine(string.Format("{0:s} {1:s}", verseRef, lines[0]));
                                outputFileT.WriteLine(string.Format("{0:s} {1:s}", verseRef, lines[1]));
                                if(!string.IsNullOrEmpty(lines[3]))
                                {
                                    outputFileExcept.WriteLine(lines[3]);
                                }
                            }
                        }
                    }

                }
            }
        }
    }

    private string[] ParseLine(string Line)
    {
        string[] lines = null;

        string[] lineParts = Line.Split('\t');
        string verseRef = string.Empty;
        int wordNumber = 0;
        string englishWord = string.Empty;
        string[] strongRefs = null;
        string hebrew = string.Empty;
        string transliteration = string.Empty;
        List<string> strongList = new List<string>();

        for (int idx = 0; idx < lineParts.Length; idx++)
        {
            string part = lineParts[idx];

            switch (idx)
            {
                case 0:
                    // verse Reference index 0 is for Hebrew Bible Versification
                    break;

                case 1:
                    {
                        // verse Reference index 1 is for KJV versification
                        // verse ref may be followed by K for Ketiv or Q for Qere
                        // we ignore those, we only use verse references ending in T
                        if (!part.EndsWith("WLT") && !part.EndsWith("WRT"))
                            return null;

                        string verRef = part.Substring(0, part.IndexOf(" : W"));
                        string[] verRefParts = verRef.Split('.');
                        string[] verNumParts = verRefParts[2].Split('#');
                        int chapter = 0;
                        int verse = 0;
                        if (!int.TryParse(verRefParts[1], out chapter))
                            Console.WriteLine("++++++++++++++++++++++++ Failed to parse chapter number: {0:s}", verRefParts[1]);
                        if (!int.TryParse(verNumParts[0], out verse))
                            Console.WriteLine("++++++++++++++++++++++++ Failed to parse verse number: {0:s}", verNumParts[0]);
                        if (!int.TryParse(verNumParts[1].Replace("w", "").Replace("p", "").Replace("+", ""), out wordNumber))
                            Console.WriteLine("++++++++++++++++++++++++ Failed to parse word number: {0:s}", verNumParts[1]);

                        verseRef = string.Format("{0:s} {1:d}:{2:d}", verRefParts[0], chapter, verse);
                        //Console.WriteLine(string.Format("{0:s} [{1:d}]", verseRef, wordNumber));

                        if (string.IsNullOrEmpty(currentVerseRef))
                        {
                            // very first word of the verse
                            currentVerseRef = verseRef;
                            verseWords = new Dictionary<int, VerseWord>();
                            break;
                        }

                        if (verseRef != currentVerseRef)
                        {
                            if (verseRef == "Gen 11:9")
                            {
                                int x = 0;
                            }

                            lines = new string[4];
                            lines[2] = currentVerseRef;
                            lines[3] = string.Empty;
                            currentVerseRef = verseRef;
                            string hebrewLine = string.Empty;
                            string tagsLine = string.Empty;
                            for(int v = 0; v < verseWords.Count; v++)
                            {
                                int[] keys = verseWords.Keys.ToArray();
                                VerseWord vw = (VerseWord)verseWords[keys[v]];
                                string st = string.Empty;
                                if (vw.Strong.Length > 0)
                                {
                                    if (vw.Strong.Length == 1)
                                    {
                                        st = vw.Strong[0];
                                    }
                                    else if ((vw.Strong.Length == 2) && vw.Strong.Contains("3068"))
                                    {
                                        // we are dealing with the name of God
                                        if (vw.Hebrew == "יהוה")
                                            st = "3068";
                                        else
                                        {
                                            st = vw.Strong[0];
                                            if (st == "3068")
                                                st = vw.Strong[1];
                                        }
                                    }
                                    else if (vw.Strong.Length == 2)
                                    {
                                        st = vw.Strong[1];
                                    }
                                    else if (vw.Strong.Length == 3)
                                    {
                                        st = vw.Strong[2];
                                    }
                                    else
                                    {
                                        // report an exception
                                        string strongs = vw.Strong[0];
                                        for (int i = 1; i < vw.Strong.Length; i++)
                                        {
                                            strongs += " " + vw.Strong[i];
                                        }
                                        lines[3] += String.Format("{0}#{1}\t{2}\r\n", currentVerseRef, v, strongs);
                                    }
                                }
                                if (string.IsNullOrEmpty(st))
                                    st = "0000";

                                if (st != "0853") // remove אֵת
                                {
                                    tagsLine += (tagsLine.Length == 0) ? st : (" " + st);
                                    hebrewLine += (hebrewLine.Length == 0) ? vw.Hebrew : (" " + vw.Hebrew);
                                }
                                
                            }

                            lines[0] = hebrewLine;
                            lines[1] = tagsLine;

                            verseWords = new Dictionary<int, VerseWord>();
                            //return lines;
                        }
                    }
                    break;

                case 2:
                    // this is the Hebrew word
                    //Console.WriteLine("Hebrew Word: {0:s}", part);
                    if (currentVerseRef == "Gen 11:9")
                    {
                        int x = 0;
                    }    
                    int e = part.IndexOf("=");
                    if (e >= 0)
                    {
                        int p = part.IndexOf("+", e);
                        if (p > 0)
                            hebrew = part.Substring(e + 1, p - e - 1);
                    }
                    break;

                case 3:
                    // English word
                    englishWord = part.Trim();
                    //Console.WriteLine(englishWord);
                    break;

                case 5:
                    // strong
                    string refr = lineParts[1];
                    int slashes = part.Split('/').Length - 1;
                    int idx1 = part.IndexOf('{');
                    if (idx1 == -1)
                    {
                        idx1 = part.IndexOf('«');
                        if (idx1 == -1)
                        {
                            //Console.WriteLine("++++++++++++++++++++++++ could not find '«' or '{': {0:s}", part);
                        }
                    }
                    int idx2 = part.IndexOf('}');
                    if (idx2 == -1)
                    {
                        idx2 = part.IndexOf('»');
                        if (idx2 == -1)
                        {
                            //Console.WriteLine("++++++++++++++++++++++++ could not find '»' or '}': {0:s}", part);
                            idx2 = part.Length - 1;
                        }
                    }
                    // H0935G|H0935G«H0935#4=?????=: come»to come (in)|1_come||go_in
                    // H3068G|H0430G«H0430#4=אֱלֹהִים=God»LORD@Gen.1.1-Heb
                    string strong = part.Substring(idx1 + 1, idx2 - idx1 - 1);
                    STATE state = STATE.START;
                    if (strong.IndexOf("#4=") > 0 || strong.IndexOf("#2=") > 0)
                    {
                        string localStrong = string.Empty;
                        string localHebrew = string.Empty;
                        for (int c = 0; c < strong.Length; c++)
                        {
                            switch (state)
                            {
                                case STATE.START:
                                    if (strong[c] == 'H')
                                    {
                                        localStrong = string.Empty;
                                        state = STATE.STRONG;
                                    }
                                    else if (strong[c] == '=')
                                    {
                                        localHebrew = string.Empty;
                                        state = STATE.HEBREW;
                                    }
                                    break;

                                case STATE.STRONG:
                                    if (char.IsDigit(strong[c]))
                                        localStrong += strong[c];
                                    else
                                    {
                                        //Console.WriteLine("Strong = {0:s}", localStrong);
                                        if (!strongList.Contains(localStrong) && !string.IsNullOrEmpty(localStrong))
                                            strongList.Add(localStrong);
                                        localStrong = string.Empty;
                                        state = STATE.START;
                                    }
                                    break;

                                case STATE.HEBREW:
                                    if (strong[c] != '=')
                                        localHebrew += strong[c];
                                    else
                                    {
                                        //Console.WriteLine("Hebrew = {0:s}", localHebrew);
                                        //hebrew = localHebrew;
                                        localHebrew = string.Empty;
                                        state = STATE.START;
                                    }
                                    break;

                            }
                        }
                    }
                    else
                    {
                        strong = "";
                    }
                    //Console.WriteLine(strong);
                    break;

                case 13:
                    transliteration = part.Trim();
                    //Console.WriteLine(part);
                    break;

                default:
                    break;
            }
        }

        strongRefs = strongList.ToArray();


        try
        {

            verseWords.Add(wordNumber, new VerseWord(hebrew, englishWord, strongRefs, transliteration));
        }
        catch (Exception ex)
        {
            string e = ex.Message;
        }

        return lines;
    }
}
public class VerseWord
{
    public VerseWord(string hebrew, string english, string[] strong, string transliteration)
    {
        this.Hebrew = hebrew;
        this.English = english;
        this.Strong = strong;
        this.Transliteration = transliteration;
    }

    public string Hebrew { get; private set; }
    public string English { get; private set; }
    public string[] Strong { get; private set; }
    public string Transliteration { get; private set; }

}
