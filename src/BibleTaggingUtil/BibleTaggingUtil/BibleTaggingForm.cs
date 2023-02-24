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
using System.Diagnostics;
using System.Threading;

using BibleTaggingUtil.Editor;
using SM.Bible.Formats.USFM;
using SM.Bible.Formats.OSIS;
using static System.Net.Mime.MediaTypeNames;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static System.Windows.Forms.DataFormats;
using Microsoft.VisualBasic.Devices;
using System.Reflection;
using static System.Net.WebRequestMethods;

namespace BibleTaggingUtil
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

        private TOTHTReaderEx totht = new TOTHTReaderEx();
        private TAGNTReader tagnt = new TAGNTReader();

        private bool m_bSaveLayout = true;
        private DeserializeDockContent m_deserializeDockContent;

        // to save updated dictionary
        // https://stackoverflow.com/questions/36333567/saving-a-dictionaryint-object-in-c-sharp-serialization

        private Dictionary<string, string> referenceVersion = new Dictionary<string, string>();
        private Dictionary<string, string> targetVersion = new Dictionary<string, string>();
        private Dictionary<string, string> targetVersionUpdates = new Dictionary<string, string>();

        private ConfigurationHolder config = new ConfigurationHolder();

        System.Timers.Timer saveTimer = null;

        public BibleTaggingForm()
        {
            InitializeComponent();

            dockPanel.DocumentStyle = DocumentStyle.DockingWindow;
            m_deserializeDockContent = new DeserializeDockContent(GetContentFromPersistString);

            this.Resize += BibleTaggingForm_Resize;

            // allow key press detection
            this.KeyPreview = true;
            // handel keypress
            this.KeyDown += BibleTaggingForm_KeyDown;
#if DEBUG
            generateSWORDFilesToolStripMenuItem.Visible = false;
#else
            nextVerseToolStripMenuItem.Visible = false;
            saveHebrewToolStripMenuItem.Visible = false;
            saveKJVPlainToolStripMenuItem.Visible = false;
            usfmToolStripMenuItem.Visible = false;
            oSISToolStripMenuItem.Visible = false;
#endif
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

            string configFile = Path.Combine(Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath), "DockPanel.config");

            if (System.IO.File.Exists(configFile))
                dockPanel.LoadFromXml(configFile, m_deserializeDockContent);

            browserPanel = new BrowserPanel(); //CreateNewDocument();
            browserPanel.Text = "Blue Letter Bible Lexicon";
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

            Assembly assembly = Assembly.GetExecutingAssembly();
            AssemblyName assemblyName = assembly.GetName();
            Version version = assemblyName.Version;
            this.Text = "Bible Tagging " + version.ToString();

            new Thread(() => { LoadBibles(); }).Start();
        }

        private void BibleTaggingForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.S && e.Modifiers == Keys.Control)
            {
                SaveUpdates();
            }
        }

        private void BibleTaggingForm_Resize(object sender, EventArgs e)
        {
            waitCursorAnimation.Location = new Point(
                (this.Width / 2) - (waitCursorAnimation.Width / 2), 
                (this.Height / 2) - (waitCursorAnimation.Height / 2));
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
            Properties.Settings.Default.Save();

            string configFile = Path.Combine(Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath), "DockPanel.config");
            if (m_bSaveLayout)
                dockPanel.SaveAsXml(configFile);
            else if (System.IO.File.Exists(configFile))
                System.IO.File.Delete(configFile);
        }

        #endregion Form Events

        private void CloseForm()
        {
            if (InvokeRequired)
            {
                // Call this same method but append THREAD2 to the text
                Action safeWrite = delegate { CloseForm(); };
                Invoke(safeWrite);
            }
            else
            {
                this.Close();
            }
        }

        /// <summary>
        /// Loading Bibles Thread
        /// </summary>
        private void LoadBibles()
        {
            bool taggedBibleOk = false;
            bool untaggedBibleOk = false;
            string taggedFolder = string.Empty;

            WaitCursorControl(true);

            string biblesFolder = Properties.Settings.Default.BiblesFolder;

            while (true)
            {
                if (string.IsNullOrEmpty(biblesFolder) && !Directory.Exists(biblesFolder))
                {
                    GetBiblesFolder();
                    biblesFolder = Properties.Settings.Default.BiblesFolder;
                    if (string.IsNullOrEmpty(biblesFolder))
                    {
                        CloseForm();
                        return;
                    }
                }

                string confResult = config.ReadBiblesConfig(biblesFolder);
                if (!string.IsNullOrEmpty(confResult))
                {
                    MessageBox.Show(confResult);
                    GetBiblesFolder();
                    biblesFolder = Properties.Settings.Default.BiblesFolder;
                    if (string.IsNullOrEmpty(biblesFolder))
                    {
                        CloseForm();
                        return;
                    }
                    else
                    {
                        confResult = config.ReadBiblesConfig(biblesFolder);
                        if (!string.IsNullOrEmpty(confResult))
                        {
                            MessageBox.Show(confResult);
                            CloseForm();
                            return;
                        }
                    }
                }

                taggedFolder = Path.GetDirectoryName(config.TaggedBible);

                if (!string.IsNullOrEmpty(config.UnTaggedBible) && System.IO.File.Exists(config.UnTaggedBible))
                {
                    untaggedBibleOk = true;
                }

                if (!Directory.Exists(taggedFolder))
                {
                    DialogResult res = MessageBox.Show("Tagged folder does not exist\r\nSelect another Bible Folder?", "Error!", MessageBoxButtons.YesNo);
                    if (res == DialogResult.Yes)
                    {
                        biblesFolder = string.Empty;
                    }
                    else
                    {
                        CloseForm();
                        return;
                    }
                }
                else
                {
                    break;
                }
            }
            this.Closing += BibleTaggingForm_Closing;

            if ((Directory.GetFiles(taggedFolder).Length == 1))
            {
                taggedBibleOk = true;
            }

            if (!taggedBibleOk && !untaggedBibleOk)
            {
                MessageBox.Show("Need either tagged or untagged Bible configured");
                CloseForm();
                return;
            }

            if (string.IsNullOrEmpty(config.KJV) || !System.IO.File.Exists(config.KJV))
            {
                MessageBox.Show("KJV is missing");
                CloseForm();
                return;
            }


            if (untaggedBibleOk) LoadBible(config.UnTaggedBible, targetVersion);
            if (taggedBibleOk)
            {
                string[] files = Directory.GetFiles(taggedFolder);
                LoadBible(files[0], targetVersionUpdates);
            }
            LoadBible(config.KJV, referenceVersion);

            bool result = false;
            if (config.HebrewReferences.Count > 0)
            {
                for (int i = 0; i < config.HebrewReferences.Count; i++)
                {
                    result = totht.LoadBibleFile(config.HebrewReferences[i]);
                }
            }
            if (!result)
            {
                MessageBox.Show("Loading Hebrew reference  failed");
                CloseForm();
                return;
            }

            if (config.GreekReferences.Count > 0)
            {
                result = tagnt.LoadBibleFile(config.GreekReferences[0]);
                for (int i = 1; i < config.GreekReferences.Count; i++)
                {
                    result = tagnt.AddBibleFile(config.GreekReferences[i]);
                }
            }
            if (!result)
            {
                MessageBox.Show("Loading Greek reference  failed");
                CloseForm();
                return;
            }

            StartGui();

            ActivatePeriodicTimer();

            WaitCursorControl(false);
            //verseSelectionPanel.CurrentBook = Properties.Settings.Default.LastBook;
            //verseSelectionPanel.CurrentChapter = Properties.Settings.Default.LastChapter;
            //verseSelectionPanel.CurrentVerse = Properties.Settings.Default.LastVerse;
            //verseSelectionPanel.FireVerseChanged();
        }

        private void StartGui()
        {
            if (InvokeRequired)
            {
                // Call this same method but append THREAD2 to the text
                Action safeWrite = delegate { StartGui(); };
                Invoke(safeWrite);
            }
            else
            {
                verseSelectionPanel.CurrentBook = Properties.Settings.Default.LastBook;
                verseSelectionPanel.CurrentChapter = Properties.Settings.Default.LastChapter;
                verseSelectionPanel.CurrentVerse = Properties.Settings.Default.LastVerse;
                verseSelectionPanel.FireVerseChanged();
                editorPanel.TargetDirty = false;
            }
        }

        public void WaitCursorControl(bool wait)
        {
            if (InvokeRequired)
            {
                // Call this same method but append THREAD2 to the text
                Action safeWrite = delegate { WaitCursorControl(wait); };
                Invoke(safeWrite);
            }
            else
            {
                if (wait)
                {
                    this.Cursor = Cursors.WaitCursor;
                    waitCursorAnimation.Visible = true;
                    waitCursorAnimation.BringToFront();
                    menuStrip1.Enabled = false;
                    verseSelectionPanel.Enabled = false;
                    editorPanel.Enabled = false;
                }
                else
                {
                    this.Cursor = Cursors.Default;
                    waitCursorAnimation.Visible = false;
                    menuStrip1.Enabled = true;
                    verseSelectionPanel.Enabled = true;
                    editorPanel.Enabled = true;
                }
            }
        }


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
            saveTimer.Stop();
            saveTimer.Start();  
            if (!editorPanel.TargetDirty)
                return;

            WaitCursorControl(true);
            editorPanel.TargetDirty = false;
            editorPanel.SaveCurrentVerse();
            if (targetVersionUpdates.Count > 0)
            {
                // construce Updates fileName
                string taggedFolder = Path.GetDirectoryName(config.TaggedBible);
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
                    if(System.IO.File.Exists(dst))
                        System.IO.File.Delete(src);
                    else
                        System.IO.File.Move(src, dst);
                }

                string baseName = Path.GetFileNameWithoutExtension(config.TaggedBible);
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
            WaitCursorControl(false);
        }

        #endregion Updates File  Saving / Loading



        #region Bible File Loading

        private void GetBiblesFolder()
        {
            if (InvokeRequired)
            {
                Action safeWrite = delegate { GetBiblesFolder(); };
                Invoke(safeWrite);
            }
            else
            {
                string folderPath = string.Empty;
                DialogResult res = folderBrowserDialog1.ShowDialog(this);
                if (res == DialogResult.OK)
                {
                    folderPath = folderBrowserDialog1.SelectedPath;
                }
                Properties.Settings.Default.BiblesFolder = folderPath;
            }
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
            GetBiblesFolder();
            string folder = Properties.Settings.Default.BiblesFolder;
            if (!string.IsNullOrEmpty(folder))
            {
                SaveUpdates();
                editorPanel.ClearCurrentVerse();
                new Thread(() => { LoadBibles(); }).Start();
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

            int priodicSaveTime = Properties.Settings.Default.PeriodicSaveTime;
            if(priodicSaveTime > 0)
            {
                settingsForm.PeriodicSaveEnabled = true;
                settingsForm.SavePeriod = priodicSaveTime;
            }
            else
            {
                settingsForm.PeriodicSaveEnabled = false;
            }
            DialogResult result = settingsForm.ShowDialog();
            if (result == DialogResult.OK)
            {
                if(settingsForm.PeriodicSaveEnabled)
                {
                    Properties.Settings.Default.PeriodicSaveTime = settingsForm.SavePeriod;
                }
                else
                {
                    Properties.Settings.Default.PeriodicSaveTime = 0;
                }
                ActivatePeriodicTimer();
            }
        }

        private void ActivatePeriodicTimer()
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() =>
                {
                    ActivatePeriodicTimer();
                }));
            }
            else
            {
                int priodicSaveTime = Properties.Settings.Default.PeriodicSaveTime;

                if (saveTimer == null)
                {
                    saveTimer = new System.Timers.Timer();
                    saveTimer.Elapsed += SaveTimer_Elapsed;
                }
                if (priodicSaveTime > 0)
                {
                    saveTimer.Interval = priodicSaveTime * 60000;
                    saveTimer.AutoReset = true;
                    saveTimer.Enabled = true;
                    saveTimer.Start();
                }
                else
                {
                    if (saveTimer != null)
                    {
                        saveTimer.Stop();
                        saveTimer.Enabled = false;
                    }
                }
            }
        }


        private void SaveTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            PeriodicSave();
        }

        private void PeriodicSave()
        {
            if(InvokeRequired) {
                Invoke(new Action(() => 
                {
                    PeriodicSave(); 
                }));
            }
            else
            {
                SaveUpdates();  
            }
        }

        /// <summary>
        /// Get Plain Verse Text
        /// </summary>
        /// <param name="verse"></param>
        /// <returns></returns>
        private string GetPlainVerseText(string verse)
        {
            string result = string.Empty;
            try
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

                result = verseWords[0];
                for (int i = 1; i < verseWords.Length; i++)
                {
                    result += (" " + verseWords[i]);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Get Plain Verse Text Exception\r\n" + ex.Message);
            }

            return result;
        }

        /// <summary>
        /// Save KJV Plain
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void saveKJVPlainToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                // construce Updates fileName
                string targetBibleFileFolder = Properties.Settings.Default.TargetBibleFileFolder;
                if (string.IsNullOrEmpty(targetBibleFileFolder))
                {
                    targetBibleFileFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory + "bibles");
                }

                if (Directory.Exists(targetBibleFileFolder))
                {
                    MessageBox.Show("Directorund\r\n" + targetBibleFileFolder);
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
                                MessageBox.Show("Save KJV Plain (1) Exception\r\n" + ex.Message);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Save KJV Plain Exception\r\n" + ex.Message);
            }
        }

        /// <summary>
        /// Get Hebrew Line
        /// </summary>
        /// <param name="verseWords"></param>
        /// <returns></returns>
        private string[] GetHebrewLine(Dictionary<int, VerseWord> verseWords)
        {
            string[] lines = new string[2];

            try
            {
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
            }
            catch (Exception ex)
            {
                MessageBox.Show("Get Hebrew Line Exception\r\n" + ex.Message);
            }
            return lines;
        }

        /// <summary>
        /// Save Hebrew
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void saveHebrewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
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
            catch (Exception ex)
            {
                MessageBox.Show("Save Hebrew Exception\r\n" + ex.Message);
            }
        }



        public void FindVerse (string tag)
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
                if (newRef == "Rev 22:21")
                {
                    verseSelectionPanel.GotoVerse(newRef);
                    break;
                }

                if (newRef.Contains("Mat"))
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

                if ((string.IsNullOrEmpty(tag) || tag.ToLower() == "<blank>"))
                {
                    if (text.Contains("<>"))
                    {
                        verseSelectionPanel.GotoVerse(newRef);
                        break;

                    }
                }
                else if (text.Contains(tag))
                {
                    verseSelectionPanel.GotoVerse(newRef);
                    break;
                }
            }
        }

        #region Generate SWORD Files Main Menu
        private void generateSWORDFilesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new Thread(
                () =>
                {
                    WaitCursorControl(true);
                    OSIS_Generator generator = new OSIS_Generator(config);
                    generator.Generate();
                    WaitCursorControl(false);
                    RunOsis2mod(config.OSIS[OsisConstants.output_file], config.OSIS[OsisConstants.osisIDWork]);
                }).Start();
        }
        #endregion Generate SWORD Files Main Menu

        #region OSIS Menue
        private void generateOSISToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new Thread(
                () =>
                {
                    WaitCursorControl(true);
                    OSIS_Generator generator = new OSIS_Generator(config);
                    generator.Generate();
                    WaitCursorControl(false);
                }).Start();
        }

        private void generateSWORDFilesOsisToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveUpdates();
            new Thread(
                () =>
                {
                    RunOsis2mod(config.OSIS[OsisConstants.output_file], config.OSIS[OsisConstants.osisIDWork]);
                }).Start();
        }

        #endregion OSIS Menue


        #region USFM Menu
        private void generateUSFMFilesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveUpdates();
            new Thread(
                () =>
                {
                    WaitCursorControl(true);
                    USFM_Generator generator = new USFM_Generator(this, config);
                    generator.Generate();
                    WaitCursorControl(false);
                }).Start();
        }

        private void convertUSFMToOSISToolStripMenuItem_Click(object sender, EventArgs e)
        {
            USFM2OSIS usfm2osis = new USFM2OSIS(this, config);
            new Thread(
               () => {
                   WaitCursorControl(true);
                   usfm2osis.Convert();
                   WaitCursorControl(false);
               }).Start();
        }

        private void generateSWORDFilesUsfmToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new Thread(() =>
            {
                RunOsis2mod(config.USFM2OSIS[Usfm2OsisConstants.outputFileName], config.USFM2OSIS[Usfm2OsisConstants.osisIDWork]);
            }).Start();

        }
        #endregion USFM Menu

        #region usfm2osis
        private void RunOsis2mod(string sourceFileName, string targetFolderName)
        {
            try
            {
                WaitCursorControl(true);
                string biblesFolder = Properties.Settings.Default.BiblesFolder;
                string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                string modulesFolder = Path.Combine(appData, "Sword\\modules\\texts\\ztext");
                string backupFolderName = string.Format("{0:s}_{1:s}", targetFolderName, DateTime.Now.ToString("yyyy_MM_dd_HH_mm"));
                string backupPath = Path.Combine(biblesFolder, backupFolderName);
                string targetFolder = Path.Combine(modulesFolder, targetFolderName);
                if (Directory.Exists(targetFolder))
                {
                    // backup currentModule
                    if(Directory.Exists(backupPath))
                    {
                        DialogResult res = MessageBox.Show("Overwrite old backup", "Do you want to overwrite existing Backup folder\r\n" + backupPath, MessageBoxButtons.YesNo);
                        if (res == DialogResult.Yes)
                        {
                            Directory.Delete(backupPath, true);
                        }
                        else
                        {
                            return;
                        }
                    }
                    Directory.CreateDirectory(backupPath);

                    // copy targetFolder to backupPath
                    var allFiles = Directory.GetFiles(targetFolder, "*.*");
                    foreach (string file in allFiles)
                    {
                        System.IO.File.Copy(file, file.Replace(targetFolder, backupPath));
                    }
                                    }


                string parentFolder = Directory.GetParent(biblesFolder).FullName;
                string executable = Path.Combine(parentFolder, "osis2mod.exe");
//                string targetFolder = Path.Combine(biblesFolder, targetFolderName);
                string xmlFile = Path.Combine(biblesFolder, sourceFileName);
                if (!Directory.Exists(targetFolder))
                {
                    Directory.CreateDirectory(targetFolder);
                }
                else
                {
                    string[] files = Directory.GetFiles(targetFolder);
                    foreach (string file in files)
                    {
                        System.IO.File.Delete(file);
                    }
                }

                Process process = new Process();
                process.StartInfo.FileName = executable;
                process.StartInfo.Arguments = targetFolder + " " + xmlFile + " -v NRSV -b 4 -z";
                process.Start();
                while (!process.HasExited) ;

                MessageBox.Show("Bible Generation completed!");

            }
            catch (Exception ex)
            {
                MessageBox.Show("Bible Generation Failed \r\n" + ex);
            }
            WaitCursorControl(false);
        }
        #endregion usfm2osis

        private void saveUpdatedTartgetToolStripMenuItem_Click(object sender, EventArgs e)
        {
                SaveUpdates();
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutForm aboutForm = new AboutForm();

            aboutForm.ShowDialog();

        }
    }
}
