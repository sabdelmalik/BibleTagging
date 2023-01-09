using System.Transactions;

internal class Program
{

    string bibleFolder = @"C:\Users\samim\Documents\MyProjects\STEP\AutoTagging\BibleFiles\IntermediateFiles";
    string bibleOTFile = @"AraSVD_OT_Clean.txt";
    string bibleNTFile = @"AraSVD_NT_Clean.txt";
    string usfmFolder;
    string usfmFolderOT;
    string usfmFolderNT;
    StreamWriter sw = null;

    int bookNumber = 1;

    Dictionary<string, string> arabicBibleNames = new Dictionary<string, string>();
    Dictionary<string, string> englishBibleNames = new Dictionary<string, string>();
    private static void Main(string[] args)
    {
        Program p = new Program();
        p.GenerateUSFM();
    }

    public void GenerateUSFM()
    {
        PopulateEnglishNameDictionary();
        PopulateArabicNameDictionary();

        usfmFolder = Path.Combine(bibleFolder, "USFM");
        if (!Directory.Exists(usfmFolder))
            Directory.CreateDirectory(usfmFolder);
        usfmFolderOT = Path.Combine(usfmFolder, "OT");
        if (!Directory.Exists(usfmFolderOT))
            Directory.CreateDirectory(usfmFolderOT);
        usfmFolderNT = Path.Combine(usfmFolder, "NT");
        if (!Directory.Exists(usfmFolderNT))
            Directory.CreateDirectory(usfmFolderNT);

        try
        {
            using (StreamReader sr = new StreamReader(Path.Combine(bibleFolder, bibleOTFile)))
            {
                while (sr.Peek() >= 0)
                {
                    WriteUSFM(usfmFolderOT, sr.ReadLine());

                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine("Exception: " + e.Message);
        }
        finally
        {
            if (sw != null)
            {
                sw.Close();
                sw = null;
            }
        }

        try
        {
            using (StreamReader sr = new StreamReader(Path.Combine(bibleFolder, bibleNTFile)))
            {
                while (sr.Peek() >= 0)
                {
                    WriteUSFM(usfmFolderNT, sr.ReadLine());

                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine("Exception: " + e.Message);
        }
        finally
        {
            if (sw != null)
            {
                sw.Close();
                sw = null;
            }
        }

    }

    string currentBook = string.Empty;
    int currentChapter = 0;

    private void WriteUSFM(string outFolder, string line)
    {
        string bookName = line.Substring(0, 3).ToUpper();
        if (bookName == "SOL")
            bookName = "SNG";
        else if (bookName == "EZE")
            bookName = "EZK";
        else if (bookName == "JOE")
            bookName = "JOL";
        else if (bookName == "NAH")
            bookName = "NAM";
        else if (bookName == "MAR")
            bookName = "MRK";
        else if (bookName == "JOH")
            bookName = "JHN";
        else if (bookName == "PHI")
            bookName = "PHP";
        else if (bookName == "JAM")
            bookName = "JAS";
        else if (bookName == "1JO")
            bookName = "1JN";
        else if (bookName == "2JO")
            bookName = "2JN";
        else if (bookName == "3JO")
            bookName = "3JN";


        if (currentBook != bookName)
        {
            // this is a new book
            if (sw != null)
            {
                sw.Close();
                sw = null;
            }
            currentBook = bookName;
            string filename = Path.Combine(outFolder, string.Format("{0}_{1}.txt", bookNumber++, englishBibleNames[currentBook]));
            if (File.Exists(filename))
                File.Delete(filename);

            sw = new StreamWriter(filename, true); // open file for append

            // write file header
            sw.WriteLine("\\id " + currentBook + " Smith & Van Dyck Arabic translation");
            sw.WriteLine("\\ide UTF-8");
            sw.WriteLine("\\h " + arabicBibleNames[currentBook]);
            sw.WriteLine("\\p");
        }

        int idx = line.IndexOf(":");
        int chapter = int.Parse(line.Substring(4, idx - 4));
        if (chapter != currentChapter)
        {
            currentChapter = chapter;
            sw.WriteLine("\\p");
            sw.WriteLine("\\c " + currentChapter.ToString());
            sw.WriteLine("\\p");
        }
        int idx1 = line.IndexOf(" ", idx);
        sw.WriteLine(String.Format("\\v {0} {1}", line.Substring(idx + 1, idx1 - idx - 1), line.Substring(idx1)));
    }

    public void PopulateArabicNameDictionary()
    {
        arabicBibleNames.Clear();
        arabicBibleNames.Add("GEN", " تكوين");
        arabicBibleNames.Add("EXO", "خروج");
        arabicBibleNames.Add("LEV", "لاويين");
        arabicBibleNames.Add("NUM", "عدد");
        arabicBibleNames.Add("DEU", "تثنية");
        arabicBibleNames.Add("JOS", "يشوع");
        arabicBibleNames.Add("JDG", "قضاة");
        arabicBibleNames.Add("RUT", "راعوث");
        arabicBibleNames.Add("1SA", "صموئيل الاول");
        arabicBibleNames.Add("2SA", "صموئيل الثاني");
        arabicBibleNames.Add("1KI", "ملوك الاول");
        arabicBibleNames.Add("2KI", "ملوك الثاني");
        arabicBibleNames.Add("1CH", "أخبار الأيام الاول");
        arabicBibleNames.Add("2CH", "أخبار الأيام الثاني");
        arabicBibleNames.Add("EZR", "عزرا");
        arabicBibleNames.Add("NEH", "نحميا");
        arabicBibleNames.Add("EST", "أستير");
        arabicBibleNames.Add("JOB", "أيوب");
        arabicBibleNames.Add("PSA", "امزامير");
        arabicBibleNames.Add("PRO", "أمثال");
        arabicBibleNames.Add("ECC", "جامعة");
        arabicBibleNames.Add("SNG", "نشيد الاناشيد");
        arabicBibleNames.Add("ISA", "إشعياء");
        arabicBibleNames.Add("JER", "إرميا");
        arabicBibleNames.Add("LAM", "مراثي إرميا");
        arabicBibleNames.Add("EZK", "حزقيال");
        arabicBibleNames.Add("DAN", "دانيال");
        arabicBibleNames.Add("HOS", "هوشع");
        arabicBibleNames.Add("JOL", "يوئيل");
        arabicBibleNames.Add("AMO", "عاموس");
        arabicBibleNames.Add("OBA", "عوبديا");
        arabicBibleNames.Add("JON", "يونان");
        arabicBibleNames.Add("MIC", "ميخا");
        arabicBibleNames.Add("NAM", "ناحوم");
        arabicBibleNames.Add("HAB", "حبقوق");
        arabicBibleNames.Add("ZEP", "صفنيا");
        arabicBibleNames.Add("HAG", "حجي)");
        arabicBibleNames.Add("ZEC", "زكريا");
        arabicBibleNames.Add("MAL", "ملاخي");
        arabicBibleNames.Add("MAT", "انجيل متى");
        arabicBibleNames.Add("MRK", "انجيل مرقس");
        arabicBibleNames.Add("LUK", "إنجيل لوقا");
        arabicBibleNames.Add("JHN", "إنجيل يوحنا");
        arabicBibleNames.Add("ACT", "أعمال الرسل");
        arabicBibleNames.Add("ROM", "رسالة بولس الرسول إلى أهل رومية");
        arabicBibleNames.Add("1CO", "رسالة بولس الرسول الأولى إلى أهل كورنثوس");
        arabicBibleNames.Add("2CO", "رسالة بولس الرسول الثانية إلى أهل كورنثوس");
        arabicBibleNames.Add("GAL", "رسالة بولس الرسول إلى أهل غلاطية");
        arabicBibleNames.Add("EPH", "رسالة بولس الرسول إلى أهل أفسس");
        arabicBibleNames.Add("PHP", "رسالة بولس الرسول إلى أهل فيلبي");
        arabicBibleNames.Add("COL", "رسالة بولس الرسول إلى أهل كولوسي");
        arabicBibleNames.Add("1TH", "رسالة بولس الرسول الأولى إلى أهل تسالونيكي");
        arabicBibleNames.Add("2TH", "رسالة بولس الرسول الثانية إلى أهل تسالونيكي");
        arabicBibleNames.Add("1TI", "رسالة بولس الرسول الأولى إلى تيموثاوس");
        arabicBibleNames.Add("2TI", "رسالة بولس الرسول الثانية إلى تيموثاوس");
        arabicBibleNames.Add("TIT", "رسالة بولس الرسول إلى تيطس");
        arabicBibleNames.Add("PHM", "رسالة بولس الرسول إلى فليمون");
        arabicBibleNames.Add("HEB", "الرسالة إلى العبرانيين");
        arabicBibleNames.Add("JAS", "رسالة يعقوب");
        arabicBibleNames.Add("1PE", "رسالة بطرس الرسول الأولى");
        arabicBibleNames.Add("2PE", "رسالة بطرس الرسول الثانية");
        arabicBibleNames.Add("1JN", "رسالة يوحنا الرسول الأولى");
        arabicBibleNames.Add("2JN", "رسالة يوحنا الرسول الثانية");
        arabicBibleNames.Add("3JN", "رسالة يوحنا الرسول الثالثة");
        arabicBibleNames.Add("JUD", "رسالة يهوذا");
        arabicBibleNames.Add("REV", "رؤيا يوحنا الاهوتي");
    }

    public void PopulateEnglishNameDictionary()
    {
        englishBibleNames.Clear();
        englishBibleNames.Add("GEN", "Genesis");
        englishBibleNames.Add("EXO", "Exodus");
        englishBibleNames.Add("LEV", "Leviticus");
        englishBibleNames.Add("NUM", "Numbers");
        englishBibleNames.Add("DEU", "Deuteronomy");
        englishBibleNames.Add("JOS", "Joshua");
        englishBibleNames.Add("JDG", "Judges");
        englishBibleNames.Add("RUT", "Ruth");
        englishBibleNames.Add("1SA", "1_Samuel");
        englishBibleNames.Add("2SA", "2_Samuel");
        englishBibleNames.Add("1KI", "1_Kings");
        englishBibleNames.Add("2KI", "2_Kings");
        englishBibleNames.Add("1CH", "1_Chronicles");
        englishBibleNames.Add("2CH", "2_Chronicles");
        englishBibleNames.Add("EZR", "Ezra");
        englishBibleNames.Add("NEH", "Nehemiah");
        englishBibleNames.Add("EST", "Esther");
        englishBibleNames.Add("JOB", "Job");
        englishBibleNames.Add("PSA", "Psalms");
        englishBibleNames.Add("PRO", "Proverbs");
        englishBibleNames.Add("ECC", "Ecclesiastes");
        englishBibleNames.Add("SNG", "Song_of_Songs");
        englishBibleNames.Add("ISA", "Isaiah");
        englishBibleNames.Add("JER", "Jeremiah");
        englishBibleNames.Add("LAM", "Lamentations");
        englishBibleNames.Add("EZK", "Ezekiel");
        englishBibleNames.Add("DAN", "Daniel");
        englishBibleNames.Add("HOS", "Hosea");
        englishBibleNames.Add("JOL", "Joel");
        englishBibleNames.Add("AMO", "Amos");
        englishBibleNames.Add("OBA", "Obadiah");
        englishBibleNames.Add("JON", "Jonah");
        englishBibleNames.Add("MIC", "Micah");
        englishBibleNames.Add("NAM", "Nahum");
        englishBibleNames.Add("HAB", "Habakkuk");
        englishBibleNames.Add("ZEP", "Zephaniah");
        englishBibleNames.Add("HAG", "Haggai");
        englishBibleNames.Add("ZEC", "Zechariah");
        englishBibleNames.Add("MAL", "Malachi");
        englishBibleNames.Add("MAT", "Matthew");
        englishBibleNames.Add("MRK", "Mark");
        englishBibleNames.Add("LUK", "Luke");
        englishBibleNames.Add("JHN", "John");
        englishBibleNames.Add("ACT", "Acts");
        englishBibleNames.Add("ROM", "Romans");
        englishBibleNames.Add("1CO", "1_Corinthians");
        englishBibleNames.Add("2CO", "2_Corinthians");
        englishBibleNames.Add("GAL", "Galatians");
        englishBibleNames.Add("EPH", "Ephesians");
        englishBibleNames.Add("PHP", "Philippians");
        englishBibleNames.Add("COL", "Colossians");
        englishBibleNames.Add("1TH", "1_Thessalonians");
        englishBibleNames.Add("2TH", "2_Thessalonians");
        englishBibleNames.Add("1TI", "1_Timothy");
        englishBibleNames.Add("2TI", "2_Timothy");
        englishBibleNames.Add("TIT", "Titus");
        englishBibleNames.Add("PHM", "Philemon");
        englishBibleNames.Add("HEB", "Hebrews");
        englishBibleNames.Add("JAS", "James");
        englishBibleNames.Add("1PE", "1_Peter");
        englishBibleNames.Add("2PE", "2_Peter");
        englishBibleNames.Add("1JN", "1_John");
        englishBibleNames.Add("2JN", "2_John");
        englishBibleNames.Add("3JN", "3_John");
        englishBibleNames.Add("JUD", "Jude");
        englishBibleNames.Add("REV", "Revelation");
    }

    public void ReadConfig(string path)
    {

    }
}