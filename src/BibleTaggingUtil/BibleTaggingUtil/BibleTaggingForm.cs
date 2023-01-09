using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

using WeifenLuo.WinFormsUI.Docking;
using System.Text.Json;
using static System.Windows.Forms.LinkLabel;
using System.Collections.Immutable;
using System.Collections.Specialized;
using System.Reflection.Emit;
using Microsoft.VisualBasic.ApplicationServices;
using Microsoft.VisualBasic;

namespace BibleTagging
{
    public enum VersionToLoad
    {
        REFEENCE,
        TARGET
    }

    public partial class BibleTaggingForm : Form
    {
        private const bool dev = false;

        private BrowserPanel browserPanel;
        private EditorPanel editorPanel;
        private VerseSelectionPanel verseSelectionPanel;

        private const string biblesConfigFile = "BiblesConfig.txt";
        private TOTHTReaderEx totht = new TOTHTReaderEx();
        private TAGNTReader tagnt = new TAGNTReader();

        private bool m_bSaveLayout = true;
        private DeserializeDockContent m_deserializeDockContent;

        // to save updated dictionary
        // https://stackoverflow.com/questions/36333567/saving-a-dictionaryint-object-in-c-sharp-serialization

        Dictionary<string, string> referenceVersion = new Dictionary<string, string>();
        Dictionary<string, string> targetVersion = new Dictionary<string, string>();
        Dictionary<string, string> targetVersionUpdates = new Dictionary<string, string>();

        private string untaggedBible=string.Empty;
        private string taggedBible = string.Empty;
        private string kjv= string.Empty;
        private List<string> hebrewReferences = new List<string>();
        private List<string> greekReferences = new List<string>();
        public BibleTaggingForm()
        {
            InitializeComponent();

            dockPanel.DocumentStyle = DocumentStyle.DockingWindow;
            m_deserializeDockContent = new DeserializeDockContent(GetContentFromPersistString);

            if(!dev)
            {
                nextVerseToolStripMenuItem.Visible = false;
                saveHebrewToolStripMenuItem.Visible = false;
                saveKJVPlainToolStripMenuItem.Visible = false;
                settingsToolStripMenuItem.Visible = false;  
            }
        }


        #region Form Events

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BibleTaggingForm_Load(object sender, EventArgs e)
        {
            #region WinFormUI setup
            this.dockPanel.Theme = this.vS2013BlueTheme1;

            referenceVersion.Clear();
            targetVersion.Clear();
            targetVersionUpdates.Clear();

            string configFile = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "DockPanel.config");

            if (File.Exists(configFile))
                dockPanel.LoadFromXml(configFile, m_deserializeDockContent);

            browserPanel = new BrowserPanel(); //CreateNewDocument();
            browserPanel.Text = "BlueBible Lexicon";
            browserPanel.TabText = browserPanel.Text;
            if (dockPanel.DocumentStyle == DocumentStyle.SystemMdi)
            {
                browserPanel.MdiParent = this;
                browserPanel.Show(dockPanel, DockState.DockRight);
            }
            else
                browserPanel.Show(dockPanel, DockState.DockRight);
            browserPanel.CloseButtonVisible = false;

            browserPanel.LexiconWebsite = "https://www.blueletterbible.org/lexicon/{0}/kjv/wlc/0-1/";
            browserPanel.NavigateToTag("h1");
            //browserPanel.NavigateTo("https://www.blueletterbible.org/lexicon/h1/kjv/wlc/0-1/");

            verseSelectionPanel = new VerseSelectionPanel();
            verseSelectionPanel.Text = "Verse Selection";
            verseSelectionPanel.TabText = verseSelectionPanel.Text;

            editorPanel = new EditorPanel(this, browserPanel, verseSelectionPanel);
            editorPanel.Text = "Verse Editor";
            editorPanel.TabText = editorPanel.Text;

            if (dockPanel.DocumentStyle == DocumentStyle.SystemMdi)
            {
                editorPanel.MdiParent = this;
                editorPanel.Show(dockPanel, DockState.Document);
            }
            else
                editorPanel.Show(dockPanel, DockState.Document);

            editorPanel.CloseButton = false;
            editorPanel.CloseButtonVisible = false;

            if (dockPanel.DocumentStyle == DocumentStyle.SystemMdi)
            {
                verseSelectionPanel.MdiParent = this;
                verseSelectionPanel.Show(dockPanel, DockState.DockLeft);
            }
            else
                verseSelectionPanel.Show(dockPanel, DockState.DockLeft);

            verseSelectionPanel.CloseButtonVisible = false;

            #endregion WinFormUI setup
            LoadBibles();
        }

        private void LoadBibles()
        { 
            string biblesFolder = Properties.Settings.Default.BiblesFolder;
            if (string.IsNullOrEmpty(biblesFolder) && !Directory.Exists(biblesFolder))
            {
                biblesFolder = GetBiblesFolder();
                if (string.IsNullOrEmpty(biblesFolder))
                {
                    this.Close();
                    return;
                }
            }
            if (!ReadBiblesConfig(biblesFolder))
            {
                MessageBox.Show("Can not continue because config file is missing");
                biblesFolder = GetBiblesFolder();
                if (string.IsNullOrEmpty(biblesFolder))
                {
                    this.Close();
                    return;
                }
                else
                    ReadBiblesConfig(biblesFolder);
            }


            this.Closing += BibleTaggingForm_Closing;

            bool taggedBibleOk = false;
            bool untaggedBibleOk = false;
            string taggedFolder = Path.GetDirectoryName(taggedBible);

            if (!string.IsNullOrEmpty(untaggedBible) && File.Exists(untaggedBible))
            {
                untaggedBibleOk = true;
            }

            if(!Directory.Exists(taggedFolder))
            {
                MessageBox.Show("Tagged folder does not exist");
                this.Close();
                return;
            }

            if ((Directory.GetFiles(taggedFolder).Length == 1))
            {
                taggedBibleOk = true;
            }

            if (!taggedBibleOk && !untaggedBibleOk)
            {
                MessageBox.Show("Need either tagged or untagged Bible configured");
                this.Close();
                return;
            }

            if (string.IsNullOrEmpty(kjv) || !File.Exists(kjv))
            {
                MessageBox.Show("KJV is missing");
                this.Close();
                return;
            }


            if(untaggedBibleOk) LoadBible(untaggedBible, targetVersion);
            if (taggedBibleOk)
            {
                string[] files = Directory.GetFiles(taggedFolder);
                LoadBible(files[0], targetVersionUpdates);
            }
            LoadBible(kjv, referenceVersion);

            bool result = false;
            if (hebrewReferences.Count > 0)
            {
                for (int i = 0; i < hebrewReferences.Count; i++)
                {
                    result = totht.LoadBibleFile(hebrewReferences[i]);
                }
            }
            if(!result)
            {
                MessageBox.Show("Loading Hebrew reference  failed");
                this.Close();
                return;
            }

            if (greekReferences.Count > 0)
            {
                result = tagnt.LoadBibleFile(greekReferences[0]);
                for (int i = 1; i < greekReferences.Count; i++)
                {
                    result = tagnt.AddBibleFile(greekReferences[i]);
                }
            }
            if (!result)
            {
                MessageBox.Show("Loading Greek reference  failed");
                this.Close();
                return;
            }

            verseSelectionPanel.CurrentBook = Properties.Settings.Default.LastBook;
            verseSelectionPanel.CurrentChapter = Properties.Settings.Default.LastChapter;
            verseSelectionPanel.CurrentVerse = Properties.Settings.Default.LastVerse;
            verseSelectionPanel.FireVerseChanged();
        }

        private bool ReadBiblesConfig(string biblesFolder)
        {
            string configFilePath = Path.Combine(biblesFolder, biblesConfigFile);
            if (!File.Exists(configFilePath))
                return false;

            Properties.Settings.Default.TargetTextDirection = "LTR";
            using (StreamReader sr = new StreamReader(configFilePath))
            {
                while (sr.Peek() >= 0)
                {
                    string line = sr.ReadLine().Trim();
                    if (string.IsNullOrEmpty(line))
                        continue;

                    string[] parts = line.Split('=');
                    if (parts.Length != 2)
                        continue;

                    switch(parts[0].Trim().ToLower())
                    {
                        case "untaggedbible":
                            untaggedBible = Path.Combine(biblesFolder, parts[1].Trim());
                            break;
                        case "taggedbible":
                            taggedBible = Path.Combine(biblesFolder, "tagged\\" + parts[1].Trim());
                            break;
                        case "kjv":
                            kjv = Path.Combine(biblesFolder, parts[1].Trim());
                            break;
                        case "hebrewreferences":
                            {
                                string[] hebParts = parts[1].Split(',');
                                for (int i = 0; i < hebParts.Length; i++)
                                {
                                    hebrewReferences.Add(Path.Combine(biblesFolder, hebParts[i]));
                                }
                            }
                            break;
                        case "greekreferences":
                            {
                                string[] grkParts = parts[1].Split(',');
                                for (int i = 0; i < grkParts.Length; i++)
                                {
                                    greekReferences.Add(Path.Combine(biblesFolder, grkParts[i]));
                                }
                            }
                            break;
                        case "targettextdirection":
                            Properties.Settings.Default.TargetTextDirection = parts[1].Trim();
                            break;
                     }

                }
            }
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BibleTaggingForm_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // Save the Verses Updates

            SaveUpdates();

            string configFile = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "DockPanel.config");
            if (m_bSaveLayout)
                dockPanel.SaveAsXml(configFile);
            else if (File.Exists(configFile))
                File.Delete(configFile);
        }

        #endregion Form Events

        #region public properties
        public Dictionary<string, string> ReferenceVersion
        {
            get
            {
                return referenceVersion;
            }
        }

        public Dictionary<string, string> TargetVersion
        {
            get
            {
                return targetVersion;
            }
        }

        public Dictionary<string, Dictionary<int, VerseWord>> TOTHTDict
        {
            get
            {
                return totht.TOTHTDict;
            }
        }
        public Dictionary<string, Dictionary<int, VerseWord>> TAGNTDict
        {
            get
            {
                return tagnt.TAGNTDict;
            }
        }

        public Dictionary<string, string> TargetVersionUpdates
        {
            get
            {
                return targetVersionUpdates;
            }
        }
        #endregion public properties

        private IDockContent FindDocument(string text)
        {
            if (dockPanel.DocumentStyle == DocumentStyle.SystemMdi)
            {
                foreach (Form form in MdiChildren)
                    if (form.Text == text)
                        return form as IDockContent;

                return null;
            }
            else
            {
                foreach (IDockContent content in dockPanel.Documents)
                    if (content.DockHandler.TabText == text)
                        return content;

                return null;
            }
        }

        private BrowserPanel CreateNewDocument()
        {
            BrowserPanel dummyDoc = new BrowserPanel();

            int count = 1;
            string text = $"Document{count}";
            while (FindDocument(text) != null)
            {
                count++;
                text = $"Document{count}";
            }

            dummyDoc.Text = text;
            return dummyDoc;
        }

        private IDockContent GetContentFromPersistString(string persistString)
        {
            // DummyDoc overrides GetPersistString to add extra information into persistString.
            // Any DockContent may override this value to add any needed information for deserialization.

            string[] parsedStrings = persistString.Split(new char[] { ',' });
            if (parsedStrings.Length != 3)
                return null;

            if (parsedStrings[0] != typeof(DummyDoc).ToString())
                return null;

            DummyDoc dummyDoc = new DummyDoc();
            if (parsedStrings[1] != string.Empty)
                dummyDoc.FileName = parsedStrings[1];
            if (parsedStrings[2] != string.Empty)
                dummyDoc.Text = parsedStrings[2];

            return dummyDoc;
        }

        #region Updates File  Saving / Loading

        public void SaveUpdates()
        {
            if (!editorPanel.TargetDirty)
                return;

            editorPanel.TargetDirty = false;
            editorPanel.SaveCurrentVerse();
            if (targetVersionUpdates.Count > 0)
            {
                // construce Updates fileName
                string taggedFolder = Path.GetDirectoryName(taggedBible);
                string oldTaggedFolder = Path.Combine(taggedFolder, "OldTagged");
                if(!Directory.Exists(oldTaggedFolder))  
                    Directory.CreateDirectory(oldTaggedFolder);

                // move existing tagged files to the old folder
                String[] existingTagged = Directory.GetFiles(taggedFolder, "*.*");
                foreach (String existingTaggedItem in existingTagged)
                {
                    string fName = Path.GetFileName(existingTaggedItem);
                    string src = Path.Combine(taggedFolder, fName);
                    string dst = Path.Combine(oldTaggedFolder, fName); 
                    if(File.Exists(dst))
                        File.Delete(src);
                    else
                        File.Move(src, dst);
                }

                string baseName = Path.GetFileNameWithoutExtension(taggedBible);
                string updatesFileName = string.Format("{0:s}_{1:s}.txt", baseName, DateTime.Now.ToString("yyyy_MM_dd_HH_mm"));
                using (StreamWriter outputFile = new StreamWriter(Path.Combine(taggedFolder, updatesFileName)))
                {

                    for (int i = 0; i < Constants.osisNames.Length; i++)
                    {
                        // construct reference
                        string bookName = Constants.osisNames[i];
                        BibleBook book = verseSelectionPanel.BibleBooks[bookName];
                        if (verseSelectionPanel.UseAltNames)
                            bookName = book.BookAltName;
                        int[] lastVerses = book.LastVerse;
                        //int idx = 0;
                        for (int chapter = 0; chapter < lastVerses.Length; chapter++)
                        {
                            try
                            {
                                int lastVerse = book.LastVerse[chapter];
                                for (int verse = 1; verse <= lastVerse; verse++)
                                {
                                    string verseRef = string.Format("{0:s} {1:d}:{2:d}", bookName, chapter + 1, verse);
                                    if (targetVersionUpdates.ContainsKey(verseRef))
                                    {
                                        string line = string.Format("{0:s} {1:s}", verseRef, targetVersionUpdates[verseRef]);
                                        outputFile.WriteLine(line);
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                string x = ex.Message;
                            }
                        }


                    }
                }

            }

            Properties.Settings.Default.LastBook = verseSelectionPanel.CurrentBook;
            Properties.Settings.Default.LastChapter = verseSelectionPanel.CurrentChapter;
            Properties.Settings.Default.LastVerse = verseSelectionPanel.CurrentVerse;
            Properties.Settings.Default.Save();
        }

        #endregion Updates File  Saving / Loading



        #region Bible File Loading

        private string GetBiblesFolder()
        {
            string folderPath = string.Empty;
            DialogResult res = folderBrowserDialog1.ShowDialog(this);
            if(res == DialogResult.OK)
            {
                folderPath = folderBrowserDialog1.SelectedPath;
            }
            Properties.Settings.Default.BiblesFolder = folderPath;
            return folderPath;
        }

        public void LoadReferenceFile()
        {
            string referenceBibleFileFolder = Properties.Settings.Default.referenceBibleFileFolder;
            if (string.IsNullOrEmpty(referenceBibleFileFolder))
            {
                referenceBibleFileFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory + "bibles");
            }

            string refFile = GetBibleFilePath(referenceBibleFileFolder, "Select Reference File");
            string referenceBibleFileName = Path.GetFileName(refFile);
            Properties.Settings.Default.ReferenceBibleFileName = referenceBibleFileName;
            referenceBibleFileFolder = Path.GetDirectoryName(refFile);
            Properties.Settings.Default.referenceBibleFileFolder = referenceBibleFileFolder;

            Properties.Settings.Default.Save();

            LoadBible(refFile, referenceVersion);
        }

        public void LoadTargetFile()
        {
            string targetBibleFileFolder = Properties.Settings.Default.TargetBibleFileFolder;
            if (string.IsNullOrEmpty(targetBibleFileFolder))
            {
                targetBibleFileFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory + "bibles");
            }

            string targetFile = GetBibleFilePath(targetBibleFileFolder, "Select Target File");
            string targetBibleFileName = Path.GetFileName(targetFile);
            Properties.Settings.Default.TargetBibleFileName = targetBibleFileName;
            targetBibleFileFolder = Path.GetDirectoryName(targetFile);
            Properties.Settings.Default.TargetBibleFileFolder = targetBibleFileFolder;

            Properties.Settings.Default.Save();

            LoadBible(targetFile, targetVersion);


        }


        string GetBibleFilePath(string directory, string title)
        {
            string biblePath = string.Empty;

            openFileDialog.Title = title;
            openFileDialog.InitialDirectory = directory;
            openFileDialog.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";

            openFileDialog.RestoreDirectory = false;

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                //Read the contents of the file into a stream
                biblePath = openFileDialog.FileName;
            }

            return biblePath;
        }

        void LoadBible(string path, Dictionary<string, string> versionDict)
        {
            //Read the contents of the file into a stream
            using (var fileStream = new FileStream(path, FileMode.Open))
            {

                using (StreamReader reader = new StreamReader(fileStream))
                {
                    versionDict.Clear();
                    this.UseWaitCursor = true;
                    while (reader.Peek() >= 0)
                    {
                        string line = reader.ReadLine();
                        int space = line.IndexOf(' ');
                        space = line.IndexOf(' ', space + 1);

                        try
                        {
                            versionDict.Add(line.Substring(0, space), line.Substring(space + 1));
                        }
                        catch(Exception ex)
                        {
                            string msg = ex.Message;
                        }
                    }

                    //ShowVerses();
                    this.UseWaitCursor = false;
                }
            }
        }

        void LoadBible(string path, OrderedDictionary versionDict)
        {
            //Read the contents of the file into a stream
            using (var fileStream = new FileStream(path, FileMode.Open))
            {

                using (StreamReader reader = new StreamReader(fileStream))
                {
                    versionDict.Clear();
                    this.UseWaitCursor = true;
                    while (reader.Peek() >= 0)
                    {
                        string line = reader.ReadLine();
                        int space = line.IndexOf(' ');
                        space = line.IndexOf(' ', space + 1);

                        versionDict.Add(line.Substring(0, space), line.Substring(space + 1));
                    }

                    //ShowVerses();
                    this.UseWaitCursor = false;
                }
            }
        }


        private void setBibleFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {

            this.Enabled = false;
            Cursor cursor = this.Cursor;
//            this.UseWaitCursor = true;
            this.Cursor = Cursors.WaitCursor;
            string folder = GetBiblesFolder();
            if (!string.IsNullOrEmpty(folder))
            {
                SaveUpdates();
                editorPanel.ClearCurrentVerse();
                LoadBibles();
            }
            //this.UseWaitCursor = false;
            this.Enabled = true;
        }

        private void loadTargetVersionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LoadTargetFile();
        }

        #endregion Bible File Loading

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SettingsForm settingsForm = new SettingsForm();

            settingsForm.ShowDialog();
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveUpdates();
        }

        private string GetPlainVerseText(string verse)
        {
            string[] verseParts = verse.Trim().Split(' ');
            // count verse words
            List<string> words = new List<string>();
            List<string> tags = new List<string>();
            string tempWord = string.Empty;
            string tmpTag = string.Empty;
            for (int i = 0; i < verseParts.Length; i++)
            {
                string versePart = verseParts[i].Trim();
                if (versePart[0] != '<' && versePart[0] != '(')
                {
                    if (!string.IsNullOrEmpty(tmpTag))
                        tags.Add(tmpTag);
                    tmpTag = string.Empty;
                    tempWord += (string.IsNullOrEmpty(tempWord)) ? verseParts[i] : (" " + verseParts[i]);
                    if (i == verseParts.Length - 1)
                    {
                        // last word
                        words.Add(tempWord);
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(tempWord))
                        words.Add(tempWord);
                    tempWord = string.Empty;
                    string thisTag = verseParts[i].Trim().
                        Replace(",", "").
                        Replace(".", "").
                        Replace(";", "").
                        Replace(":", "");
                    if (thisTag[0] == '<')
                        tmpTag += (string.IsNullOrEmpty(tmpTag)) ? thisTag : (" " + thisTag);
                    if (i == verseParts.Length - 1)
                    {
                        // last word
                        if (tmpTag.EndsWith('.'))
                            tmpTag.Remove(tmpTag.Length - 1, 1);
                        tags.Add(tmpTag);
                    }

                }
            }

            string[] verseWords = words.ToArray();
            string[] verseTags = tags.ToArray();

            string result = verseWords[0];
            for (int i = 1; i < verseWords.Length; i++)
            {
                result += (" " + verseWords[i]);
            }

            return result;
        }
        private void saveKJVPlainToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // construce Updates fileName
            string targetBibleFileFolder = Properties.Settings.Default.TargetBibleFileFolder;
            if (string.IsNullOrEmpty(targetBibleFileFolder))
            {
                targetBibleFileFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory + "bibles");
            }

            using (StreamWriter outputFile = new StreamWriter(Path.Combine(targetBibleFileFolder, "kjv_plain.txt")))
            {

                for (int i = 0; i < Constants.osisNames.Length; i++)
                {
                    // construct reference
                    string bookName = Constants.osisNames[i];
                    if (bookName == "Exod")
                        break;
                    BibleBook book = verseSelectionPanel.BibleBooks[bookName];
                    if (verseSelectionPanel.UseAltNames)
                        bookName = book.BookAltName;
                    int[] lastVerses = book.LastVerse;
                    //int idx = 0;
                    for (int chapter = 0; chapter < lastVerses.Length; chapter++)
                    {
                        try
                        {
                            int lastVerse = book.LastVerse[chapter];
                            for (int verse = 1; verse <= lastVerse; verse++)
                            {
                                string verseRef = string.Format("{0:s} {1:d}:{2:d}", bookName, chapter + 1, verse);
                                if (ReferenceVersion.ContainsKey(verseRef))
                                {
                                    string line = string.Format("{0:s} {1:s}", verseRef, GetPlainVerseText(ReferenceVersion[verseRef]));
                                    outputFile.WriteLine(line);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            string x = ex.Message;
                        }
                    }


                }

            }
        }

        private string[] GetHebrewLine(Dictionary<int, VerseWord> verseWords)
        {
            string[] lines = new string[2];

            List<string> words = new List<string>();
            List<string> hebrew = new List<string>();
            List<string> transliteration = new List<string>();
            List<string> tags = new List<string>();

            for (int i = 0; i < verseWords.Count; i++)
            {
                int key = verseWords.Keys.ToArray()[i];
                VerseWord verseWord = verseWords[key];
                words.Add(verseWord.English);
                hebrew.Add(verseWord.Hebrew);
                transliteration.Add(verseWord.Transliteration);
                if (verseWord.Strong.Length > 0)
                {
                    string s = String.Empty;
                    bool E = (verseWord.Hebrew.Trim() == "אֱלֹהִים");
                    bool Y = (verseWord.Hebrew.Trim() == "יהוה");
                    bool strongIsE = (verseWord.Strong[0].Trim() == "0430");
                    bool strongIsY = (verseWord.Strong[0].Trim() == "3068");

                    if (E || Y)
                    {
                        // special treatment for אֱלֹהִים & יהוה
                        if ((E && strongIsE) || (Y && strongIsY))
                            s = "<" + verseWord.Strong[0] + ">";
                    }
                    else
                    {
                        s = verseWord.Strong[0];
                    }

                    if (verseWord.Strong.Length > 1)
                    {
                        for (int j = 1; j < verseWord.Strong.Length; j++)
                        {
                            strongIsE = (verseWord.Strong[j].Trim() == "0430");
                            strongIsY = (verseWord.Strong[j].Trim() == "3068");
                            if (E || Y)
                            {
                                // special treatment for אֱלֹהִים & יהוה
                                if ((E && strongIsE) || (Y && strongIsY))
                                {
                                    if (!string.IsNullOrEmpty(s))
                                        s += " ";
                                    s += verseWord.Strong[j];
                                }
                            }
                            else
                            {
                                if (!string.IsNullOrEmpty(s))
                                    s += " ";
                                s += "<" + verseWord.Strong[j] + ">";
                            }
                        }
                    }
                    tags.Add(s.Trim());
                }
                else
                    tags.Add("");
            }
            string[] hebrewA = hebrew.ToArray();
            string[] tagsA = tags.ToArray();
            string line0 = string.Empty;
            string line1 = string.Empty;
            for (int i = 0; i < hebrewA.Length; i++)
            {
                if (tagsA[i].Trim()[0] == '9')
                    continue;
                line0 += string.IsNullOrWhiteSpace(hebrewA[i]) ? hebrewA[i] : (" " + hebrewA[i]);
                line1 += string.IsNullOrWhiteSpace(tagsA[i]) ? tagsA[i] : (" " + tagsA[i]);
            }
            lines[0] = line0;
            lines[1] = line1;
            return lines;
        }
        private void saveHebrewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string targetBibleFileFolder = Properties.Settings.Default.TargetBibleFileFolder;
            if (string.IsNullOrEmpty(targetBibleFileFolder))
            {
                targetBibleFileFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory + "bibles");
            }

            using (StreamWriter outputFileH = new StreamWriter(Path.Combine(targetBibleFileFolder, "hebrew.txt")))
            {
                using (StreamWriter outputFileT = new StreamWriter(Path.Combine(targetBibleFileFolder, "tags.txt")))
                {
                    for (int i = 0; i < Constants.osisNames.Length; i++)
                    {
                        // construct reference
                        string bookName = Constants.osisNames[i];
                        if (bookName == "Exod")
                            break;

                        BibleBook book = verseSelectionPanel.BibleBooks[bookName];
                        if (verseSelectionPanel.UseAltNames)
                            bookName = book.BookAltName;
                        int[] lastVerses = book.LastVerse;
                        //int idx = 0;
                        for (int chapter = 0; chapter < lastVerses.Length; chapter++)
                        {
                            try
                            {
                                int lastVerse = book.LastVerse[chapter];
                                for (int verse = 1; verse <= lastVerse; verse++)
                                {
                                    string verseRef = string.Format("{0:s} {1:d}:{2:d}", bookName, chapter + 1, verse);
                                    if (TOTHTDict.ContainsKey(verseRef))
                                    {
                                        ///
                                        string[] lines = GetHebrewLine(TOTHTDict[verseRef]);
                                        outputFileH.WriteLine(string.Format("{0:s} {1:s}", verseRef, lines[0]));
                                        outputFileT.WriteLine(string.Format("{0:s} {1:s}", verseRef, lines[1]));
                                        ///
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                string x = ex.Message;
                            }
                        }
                    }

                }
            }
        }


        public void NextUnknownVerse ()
        {
            string newRef = editorPanel.CurrentVerse;
            while (true)
            {
                // reference version has
                // Sng as Sol
                // Ezk as Eze
                // Jol as Joe
                // Nam as Nah
                newRef = newRef.Replace("Sol", "Sng").
                                Replace("Eze", "Ezk").
                                Replace("Joe", "Jol").
                                Replace("Nah", "Nam");

                newRef = verseSelectionPanel.GetNextRef(newRef);
                if(newRef.Contains("Mat"))
                {
                    int stop = 0;
                }

                string text = string.Empty;

                // reference version has
                // Sng as Sol
                // Ezk as Eze
                // Jol as Joe
                // Nam as Nah
               newRef = newRef.Replace("Sng", "Sol").
                                Replace("Ezk", "Eze").
                                Replace("Jol", "Joe").
                                Replace("Nam", "Nah");

                try
                {
                    text = referenceVersion[newRef];
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }

                //if (text.Contains("0410>"))
                //{
                //    verseSelectionPanel.GotoVerse(newRef);
                //    break;
                //}

                text = targetVersionUpdates[newRef];
                //try
                //{
                //    int idx = newRef.IndexOf(' ');
                //    string book = newRef.Substring(0, idx);
                //    if (Constants.osisAltNames.Contains(book))
                //    {
                //        if (Array.IndexOf(Constants.osisAltNames, book) > 38)
                //        {
                //            if (text.Contains("<3588>"))
                //                targetVersionUpdates[newRef] = text.Replace("<3588>", "<>");
                //        }
                //    }
                //    else
                //    {
                //        MessageBox.Show(book + " Not found!");
                //    }
                //    if(newRef == "Rev 22:21")
                //    {
                //        MessageBox.Show("Done!");
                //    }
                //}
                //catch (Exception ex)
                //{
                //    MessageBox.Show(ex.Message);
                //}

                if (text.Contains("???") || text.Contains("0000"))
                {
                    verseSelectionPanel.GotoVerse(newRef);
                    break;
                }
            }
        }

        private void nextVerseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NextUnknownVerse();
        }
    }
}
