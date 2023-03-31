using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.IO;

using WeifenLuo.WinFormsUI.Docking;
using System.Diagnostics;
using System.Threading;

using BibleTaggingUtil.Editor;
using SM.Bible.Formats.USFM;
using SM.Bible.Formats.OSIS;
using System.Reflection;

using BibleTaggingUtil.BibleVersions;
using System.Linq.Expressions;
using static System.Net.WebRequestMethods;
using Microsoft.VisualBasic.Logging;
using System.Xml.Linq;

namespace BibleTaggingUtil
{
    public partial class BibleTaggingForm : Form
    {
        private const bool dev = false;

        private BrowserPanel browserPanel;
        private EditorPanel editorPanel;
        private VerseSelectionPanel verseSelectionPanel;
        private ProgressForm progressForm;

        private TargetVersion target;
        private ReferenceVersionKJV referenceKJV;
        private ReferenceVersionTOTHT referenceTOTHT;
        private ReferenceVersionTAGNT referenceTAGNT;



        private bool m_bSaveLayout = true;
        private DeserializeDockContent m_deserializeDockContent;

        // to save updated dictionary
        // https://stackoverflow.com/questions/36333567/saving-a-dictionaryint-object-in-c-sharp-serialization

        private ConfigurationHolder config = new ConfigurationHolder();

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
            saveHebrewToolStripMenuItem.Visible = false;
            saveKJVPlainToolStripMenuItem.Visible = false;
            usfmToolStripMenuItem.Visible = false;
            oSISToolStripMenuItem.Visible = false;
#endif

            target = new TargetVersion(this);
            referenceKJV = new ReferenceVersionKJV(this);
            referenceTOTHT = new ReferenceVersionTOTHT(this);
            referenceTAGNT = new ReferenceVersionTAGNT(this);
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

            progressForm= new ProgressForm(this);

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

            string execFolder = Path.GetDirectoryName(assembly.Location);
            Tracing.InitialiseTrace(execFolder);

            editorPanel.TargetVersion = target;

            new Thread(() => { LoadBibles(); }).Start();
        }

        private void BibleTaggingForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.S && e.Modifiers == Keys.Control)
            {
                target.SaveUpdates();
            }
        }

        private void BibleTaggingForm_Resize(object sender, EventArgs e)
        {
            progressForm.Location = new Point(
                this.Location.X + (this.Width / 2) - (progressForm.Width / 2),
                this.Location.Y + (this.Height / 2) - (progressForm.Height / 2));
            //waitCursorAnimation.Location = new Point(
            //    (this.Width / 2) - (waitCursorAnimation.Width / 2), 
            //    (this.Height / 2) - (waitCursorAnimation.Height / 2));
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BibleTaggingForm_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                if (MessageBox.Show("This will close down the application. Confirm?", "Close Bible Edit", MessageBoxButtons.YesNo) == DialogResult.No)
                {
                    e.Cancel = true;
                    this.Activate();
                }
                else
                {

                    // Save the Verses Updates

                    target.SaveUpdates();
                    Properties.Settings.Default.Save();

                    string configFile = Path.Combine(Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath), "DockPanel.config");
                    if (m_bSaveLayout)
                        dockPanel.SaveAsXml(configFile);
                    else if (System.IO.File.Exists(configFile))
                        System.IO.File.Delete(configFile);
                }
            }
            catch(Exception ex)
            {
                Tracing.TraceException(MethodBase.GetCurrentMethod().Name, ex.Message);
            }
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
            Tracing.TraceEntry(MethodBase.GetCurrentMethod().Name);
  
            string taggedFolder = string.Empty;

            WaitCursorControl(true);

            string biblesFolder = Properties.Settings.Default.BiblesFolder;
            try
            {
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

                    editorPanel.TargetBibleName(Path.GetFileName(biblesFolder));
                    target.BibleName = Path.GetFileName(biblesFolder);
                    referenceKJV.BibleName = "KJV";
                    referenceTAGNT.BibleName = "TAGNT";
                    referenceTOTHT.BibleName = "TOTHT";

                    config = new ConfigurationHolder();
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

                    if (!Directory.Exists(taggedFolder))
                    {
                        DialogResult res = ShowMessageBox("Tagged folder does not exist\r\nSelect another Bible Folder?", "Error!", MessageBoxButtons.YesNo);
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
            }
            catch(Exception ex)
            {
                Tracing.TraceException(MethodBase.GetCurrentMethod().Name, ex.Message);
            }

            string[] files = Directory.GetFiles(taggedFolder);
            if (files.Length > 0)
            {
                target.LoadBibleFile(files[0], true, false);
                VerseSelectionPanel.SetBookCount(target.BookCount);
            }
            else
            {
                DialogResult res = ShowMessageBox("Tagged folder is empty\r\nSelect another Bible Folder?", "Error!", MessageBoxButtons.YesNo);
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

            StartGui();

            if (string.IsNullOrEmpty(config.KJV) || !System.IO.File.Exists(config.KJV))
            {
                MessageBox.Show("KJV is missing");
                CloseForm();
                return;
            }

            this.Closing += BibleTaggingForm_Closing;

            referenceKJV.LoadBibleFile(config.KJV, true, false);

            StartGui();

            bool result = false;
            if (config.HebrewReferences.Count > 0)
            {
                for (int i = 0; i < config.HebrewReferences.Count; i++)
                {
                    result = referenceTOTHT.LoadBibleFile(config.HebrewReferences[i], i == 0, i != (config.HebrewReferences.Count -1));
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
                for (int i = 0; i < config.GreekReferences.Count; i++)
                {
                    result = referenceTAGNT.LoadBibleFile(config.GreekReferences[i], i == 0, i != (config.GreekReferences.Count - 1));
                }
            }
            if (!result)
            {
                MessageBox.Show("Loading Greek reference  failed");
                CloseForm();
                return;
            }

            StartGui();

            target.ActivatePeriodicTimer();

            WaitCursorControl(false);
            //verseSelectionPanel.CurrentBook = Properties.Settings.Default.LastBook;
            //verseSelectionPanel.CurrentChapter = Properties.Settings.Default.LastChapter;
            //verseSelectionPanel.CurrentVerse = Properties.Settings.Default.LastVerse;
            //verseSelectionPanel.FireVerseChanged();
        }

        private void StartGui()
        {
            Tracing.TraceEntry(MethodBase.GetCurrentMethod().Name);

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
            Tracing.TraceEntry(MethodBase.GetCurrentMethod().Name);

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
                    //waitCursorAnimation.Visible = true;
                    //aitCursorAnimation.BringToFront();
                    progressForm.Clear();

                    //progressForm.Location = new Point((this.Width / 2) - (progressForm.Width / 2),
                    //                                  (this.Height / 2) - (progressForm.Height / 2));
                     
                    progressForm.Show();

                    progressForm.Location = new Point(
                        this.Location.X + (this.Width / 2) - (progressForm.Width / 2),
                        this.Location.Y + (this.Height / 2) - (progressForm.Height / 2));


                    menuStrip1.Enabled = false;
                    verseSelectionPanel.Enabled = false;
                    editorPanel.Enabled = false;
                }
                else
                {
                    this.Cursor = Cursors.Default;
                    //waitCursorAnimation.Visible = false;
                    progressForm.Visible = false;
                    menuStrip1.Enabled = true;
                    verseSelectionPanel.Enabled = true;
                    editorPanel.Enabled = true;
                }
            }
        }

        public void UpdateProgress(string label, int progress)
        {
            if (InvokeRequired)
            {
                try
                {
                    // Call this same method but append THREAD2 to the text
                    Action safeWrite = delegate { UpdateProgress(label, progress); };
                    Invoke(safeWrite);
                }
                catch (Exception ex){ }   
            }
            else
            {
                progressForm.Label = "Loading " + label;
                progressForm.Progress = progress;
            }
        }

        private DialogResult ShowMessageBox(string text, string caption, MessageBoxButtons buttons)
        {
            DialogResult result = DialogResult.OK;

            if (InvokeRequired)
            {
                // Call this same method but append THREAD2 to the text
                //Action safeWrite = delegate { ShowMessageBox(text, caption, buttons); };
                result = (DialogResult) Invoke(new Func<DialogResult>(() => ShowMessageBox(text, caption, buttons)));
            }
            else
            {
                result = MessageBox.Show(text, caption, buttons);
            }
            return result;
        }

        #region public properties

        public ConfigurationHolder Config { get { return config; } }    
        public EditorPanel EditorPanel { get { return editorPanel; } }  
        public VerseSelectionPanel VerseSelectionPanel { get { return verseSelectionPanel; } }
        public TargetVersion Target { get { return target; } }
        public ReferenceVersionKJV KJV { get {return referenceKJV; } }
        public ReferenceVersionTOTHT TOTHT { get { return referenceTOTHT; } }
        public ReferenceVersionTAGNT TAGNT { get { return referenceTAGNT; } }

        #endregion public properties

        #region WinFormUI
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

        #endregion WinFormUI


        #region Bible File Loading

        /// <summary>
        /// 
        /// </summary>
        private void GetBiblesFolder()
        {
            Tracing.TraceEntry(MethodBase.GetCurrentMethod().Name);

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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="directory"></param>
        /// <param name="title"></param>
        /// <returns></returns>
        public string GetBibleFilePath(string directory, string title)
        {
            Tracing.TraceEntry(MethodBase.GetCurrentMethod().Name, directory, title);

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

            Tracing.TraceExit(MethodBase.GetCurrentMethod().Name, biblePath);
            return biblePath;
        }

        /// <summary>
        /// Get Bible folder to work with
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void setBibleFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {

            this.Enabled = false;
            GetBiblesFolder();
            string folder = Properties.Settings.Default.BiblesFolder;
            if (!string.IsNullOrEmpty(folder))
            {
                target.SaveUpdates();
                editorPanel.ClearCurrentVerse();
                new Thread(() => { LoadBibles(); }).Start();
            }
            this.Enabled = true;
        }

        #endregion Bible File Loading

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
                target.ActivatePeriodicTimer();
            }
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
                                    if (referenceKJV.Bible.ContainsKey(verseRef))
                                    {
                                        string line = string.Format("{0:s} {1:s}", verseRef, Utils.GetVerseText(referenceKJV.Bible[verseRef], false));
                                        outputFile.WriteLine(line);
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                Tracing.TraceException(MethodBase.GetCurrentMethod().Name, ex.Message);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Tracing.TraceException(MethodBase.GetCurrentMethod().Name, ex.Message);
            }
        }

        /// <summary>
        /// Get Hebrew Line
        /// </summary>
        /// <param name="verseWords"></param>
        /// <returns></returns>
        private string[] GetHebrewLine(Verse verseWords)
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
                    VerseWord verseWord = verseWords[i];
                    words.Add(verseWord.Word);
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
                Tracing.TraceException(MethodBase.GetCurrentMethod().Name, ex.Message);
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
                                        if (TOTHT.Bible.ContainsKey(verseRef))
                                        {
                                            ///
                                            string[] lines = GetHebrewLine(TOTHT.Bible[verseRef]);
                                            outputFileH.WriteLine(string.Format("{0:s} {1:s}", verseRef, lines[0]));
                                            outputFileT.WriteLine(string.Format("{0:s} {1:s}", verseRef, lines[1]));
                                            ///
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Tracing.TraceException(MethodBase.GetCurrentMethod().Name, ex.Message);
                                }
                            }
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                Tracing.TraceException(MethodBase.GetCurrentMethod().Name, ex.Message);
            }
        }

        public void FindVerse (string tag)
        {
            try
            {
                string newRef = editorPanel.CurrentVerse;
                while (true)
                {
                    newRef = verseSelectionPanel.GetNextRef(newRef);
                    if (newRef == "Rev 22:21")
                    {
                        verseSelectionPanel.GotoVerse(newRef);
                        break;
                    }

                    string text = string.Empty;

                    try
                    {
                        string bookName = newRef.Substring(0, 3);
                        string targetRef = newRef.Replace(bookName, Target[bookName]);
                        text = Utils.GetVerseText(Target.Bible[targetRef], true);
                    }
                    catch (Exception ex)
                    {
                        Tracing.TraceException(MethodBase.GetCurrentMethod().Name, ex.Message);
                    }

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
            catch(Exception ex)
            {
                Tracing.TraceException(MethodBase.GetCurrentMethod().Name, ex.Message);
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
            target.SaveUpdates();
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
            target.SaveUpdates();
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
                UpdateProgress("Creating Module", 50);
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
                        DialogResult res = ShowMessageBox("Overwrite old backup", "Do you want to overwrite existing Backup folder\r\n" + backupPath, MessageBoxButtons.YesNo);
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

                WaitCursorControl(false);
                MessageBox.Show("Bible Generation completed!");

            }
            catch (Exception ex)
            {
                Tracing.TraceException(MethodBase.GetCurrentMethod().Name, ex.Message);
            }
            WaitCursorControl(false);
        }
        #endregion usfm2osis

        private void saveUpdatedTartgetToolStripMenuItem_Click(object sender, EventArgs e)
        {
                target.SaveUpdates();
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutForm aboutForm = new AboutForm();

            aboutForm.ShowDialog();

        }

        private void reloadTargetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string taggedFolder = Path.GetDirectoryName(config.TaggedBible);
            string[] files = Directory.GetFiles(taggedFolder);
            if (files.Length > 0)
            {
                target.LoadBibleFile(files[0], true, false);
                VerseSelectionPanel.SetBookCount(target.BookCount);

            }
        }
    }
}
