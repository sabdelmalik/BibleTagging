using System.Collections.Generic;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using System.Reflection;

namespace BibleTagging
{
    public partial class BibleTaggingPreperationForm : Form
    {
        private string workFolder = string.Empty;
        private string alignerPath = string.Empty;
        private string bibleFileName = string.Empty;
        private string bibleFilePath = string.Empty;
        private string otFilePath = string.Empty;
        private string ntFilePath = string.Empty;

        private const char sourceBkChSeperator = ' ';
        private const char sourceChVsSeperator = ':';
        private const char sourceVsTxSeperator = ' ';

        SpecialFunctions sf;

        private BibleVersification bibleVersification = null;

        // java -server -mx1000m -cp berkeleyaligner.jar edu.berkeley.nlp.wordAlignment.Main ++confs/NT.conf

        public BibleTaggingPreperationForm()
        {
            InitializeComponent();
        }

        #region Trace
        delegate void TraceDelegate(string text, Color color);

        public void Trace(string text, Color color)
        {
            if (InvokeRequired)
            {
                Invoke(new TraceDelegate(Trace), new object[] { text, color });
            }
            else
            {
                traceBox.SelectionColor = color;
                if (text.Length > 0)
                {
                    string txt = string.Format("{0}: {1}\r\n", DateTime.Now.ToString("dd-MMM-yyyy hh:mm:ss.fff"), text);
                    //traceBox.AppendText(txt);
                    traceBox.SelectedText = txt;
                }
                else
                {
                    traceBox.AppendText("\r\n");
                }
                traceBox.ScrollToCaret();
            }
        }

        public void TraceError(string method, string text)
        {
            Trace(string.Format("Error: {0}::{1}",method, text), Color.Red);
        }


        #endregion Trace

        private void Form1_Load(object sender, EventArgs e)
        {
            Trace("Loding ...", Color.Blue);

            bibleVersification = new BibleVersification(this);

            PrepareFolders();           
        }

        private void PrepareFolders()
        {
            string docsFolder = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            string homeFolder = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

            // Work folder
            workFolder = Path.Combine(homeFolder, ".BibleTagging");
            if (!Directory.Exists(workFolder))
            {
                Directory.CreateDirectory(workFolder);
            }

            // Aligner
            BerkeleyAligner aligner = new BerkeleyAligner();
            //aligner.Align();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {

            openFileDialog1.RestoreDirectory = true;
            openFileDialog1.Title = "Select Bible Text";
            openFileDialog1.Filter = "Bible files (*.txt)|*.txt";
            DialogResult result = openFileDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                bibleFilePath = openFileDialog1.FileName;

                sf = new SpecialFunctions(this);
                bibleFilePath = sf.ConvertToUbsNames(bibleFilePath);
                Trace("\r\nProcessing: " + bibleFilePath, Color.Blue);
                if (!string.IsNullOrEmpty(bibleFilePath))
                {
                    ValidateBibleFile(bibleFilePath);
                    Split(bibleFilePath);
                }
            }
        }

        private void ValidateBibleFile(string bibleFile)
        {
            int errors = 0;
            string currentBook = string.Empty;
            int currentChapter = 0;
            int currentVerse = 0;
            
            Dictionary<string, Dictionary<int, int>> bible = new Dictionary<string, Dictionary<int, int>>();
            Dictionary<int,int> chapters= null;

            using (StreamReader sr = new StreamReader(bibleFile))
            {
                while (!sr.EndOfStream)
                {
                    if (errors > 10)
                    {
                        Trace("Too many errors", Color.Red);
                        return;
                    }

                    var line = sr.ReadLine();
                    if (string.IsNullOrEmpty(line))
                        continue;
                    int sep1 = line.IndexOf(sourceBkChSeperator);
                    if(sep1 < 0)
                    {
                        Trace(string.Format("Bad Formating (sep1): {0}", line), Color.Red);
                        errors++;
                        continue;
                    }

                    int sep2 = line.IndexOf(sourceChVsSeperator, sep1 +1);
                    if (sep2 < 0)
                    {
                        Trace(string.Format("Bad Formating (sep2): {0}", line), Color.Red);
                        errors++;
                        continue;
                    }

                    int sep3 = line.IndexOf(sourceVsTxSeperator, sep2+1);
                    if (sep3 < 0)
                    {
                        Trace(string.Format("Bad Formating (sep3): {0}", line), Color.Red);
                        errors++;
                        continue;
                    }

                    if (!int.TryParse(line.Substring(sep1 + 1, sep2 - sep1 - 1), out currentChapter))
                    {
                        Trace(string.Format("Bad Formating (chapter): {0}", line), Color.Red);
                        errors++;
                        continue;
                    }
                    if (!int.TryParse(line.Substring(sep2 + 1, sep3 - sep2 - 1), out currentVerse))
                    {
                        Trace(string.Format("Bad Formating (chapter): {0}", line), Color.Red);
                        errors++;
                        continue;
                    }

                    currentBook = line.Substring(0,sep1);
                    if(!bible.ContainsKey(currentBook))
                    {
                        // this is a new book
                        bible[currentBook] = new Dictionary<int, int>();

                    }
                    Dictionary<int, int> verses = bible[currentBook];
                    if(verses.ContainsKey(currentChapter))
                        verses[currentChapter]++;
                    else
                        verses[currentChapter]  = 1;
                }
            }
        }

        private void Split(string bibleFile)
        {
            List<string> ot = new List<string>();
            List<string> nt = new List<string>();

            bibleFileName = Path.GetFileNameWithoutExtension(bibleFile);

            otFilePath = Path.Combine(workFolder, bibleFileName + "_OT.txt");
            ntFilePath = Path.Combine(workFolder, bibleFileName + "_NT.txt");

            if (File.Exists(otFilePath)) File.Delete(otFilePath);
            if (File.Exists(ntFilePath)) File.Delete(ntFilePath);

            string[] bibleNames = Constants.ubsNames;


            using (StreamReader sr = new StreamReader(bibleFile))
            {
                while (!sr.EndOfStream)
                {
                    var line = sr.ReadLine();
                    if (!string.IsNullOrEmpty(line))
                    {
                        try
                        {
                            int idx = line.IndexOf(' ');
                            if (idx < 2)
                            {
                                TraceError(MethodBase.GetCurrentMethod().Name, "Bad line: " + line);
                            }
                            string bookName = line.Substring(0, idx); ;
                            if (!bibleNames.Contains(bookName))
                            {
                                bibleNames = Constants.osisAltNames;
                                if (!bibleNames.Contains(bookName))
                                {
                                    bibleNames = Constants.ubsNames;
                                    if (!bibleNames.Contains(bookName))
                                    {
                                        TraceError(MethodBase.GetCurrentMethod().Name, "Failed to find book name: " + line);
                                        continue;
                                    }
                                }
                            }
                            int bookIndex = Array.IndexOf(bibleNames, bookName);
                            if (bookIndex < 39)
                                ot.Add(line);
                            else
                                nt.Add(line);
                        }
                        catch (Exception ex)
                        {
                            TraceError(MethodBase.GetCurrentMethod().Name, ex.Message);
                        }
                    }
                }
            }

            bool otNRSV_OK = (ot.Count == bibleVersification.GetVerseCount(BibleVersion.NRSV, Testament.OT));
            bool ntNRSV_OK = (nt.Count == bibleVersification.GetVerseCount(BibleVersion.NRSV, Testament.NT));
            bool otKJV_OK = (ot.Count == bibleVersification.GetVerseCount(BibleVersion.KJV, Testament.OT));
            bool ntKJV_OK = (nt.Count == bibleVersification.GetVerseCount(BibleVersion.KJV, Testament.NT));

            if (otKJV_OK && ntKJV_OK)
            {
                Trace("Verse Count Matched KJV", Color.Green);
            }
            else if (otNRSV_OK && ntNRSV_OK)
            {
                Trace("Verse Count Matched NRSV", Color.Green);
            }
            else
            {
                TraceError(MethodBase.GetCurrentMethod().Name, string.Format("Verse Counts did not match! OT = {0}, NT = {1}", ot.Count, nt.Count));

                if (sf != null)
                {
                    if (!otNRSV_OK || !otKJV_OK) sf.ReportMismatchOT();
                    if (!ntNRSV_OK || !ntKJV_OK) sf.ReportMismatchNT();
                }
                //return;
            }
            using (StreamWriter swOT = new StreamWriter(otFilePath))
            {
                for (int i = 0; i < ot.Count; i++)
                {
                    swOT.WriteLine(ot[i]);
                }

            }
            using (StreamWriter swNT = new StreamWriter(ntFilePath))
            {
                for (int i = 0; i < nt.Count; i++)
                {
                    swNT.WriteLine(nt[i]);
                }

            }
        }
    }
}