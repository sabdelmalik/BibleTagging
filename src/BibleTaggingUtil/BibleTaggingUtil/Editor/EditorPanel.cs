using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Forms;

using WeifenLuo.WinFormsUI.Docking;

namespace BibleTaggingUtil.Editor
{
    public partial class EditorPanel : DockContent
    {
        private const string   MERGE_CONTEXT_MENU = "Merge";
        private const string   SPLIT_CONTEXT_MENU = "Split";
        private const string   DELETE_CONTEXT_MENU = "Delete Tag";
        private const string   REVERSE_CONTEXT_MENU = "Reverse Tags";
        private const string  DELETE_LEFT_CONTEXT_MENU = "Delete Left Tags";
        private const string DELETE_RIGHT_CONTEXT_MENU = "Delete Right Tags";



        private BibleTaggingForm parent;
        private BrowserPanel browser;
        private VerseSelectionPanel verse;

        private string testament = string.Empty;

        private System.Timers.Timer tempTimer = null;

        public bool TargetDirty { get; set; }

        #region Constructors
        public EditorPanel()
        {
            InitializeComponent();
            this.ControlBox = false;
            this.CloseButtonVisible = false;
            this.CloseButton = false;
        }

        public EditorPanel(BibleTaggingForm parent, BrowserPanel browser, VerseSelectionPanel verse)
        {
            InitializeComponent();

            this.ControlBox = false;
            this.CloseButtonVisible = false;
            this.CloseButton = false;

            this.parent = parent;
            this.browser = browser;
            this.verse = verse;

        }
        #endregion Constructors


        private void EditorPanel_Load(object sender, EventArgs e)
        {
            // Define a context menu for the Target Verse grid
            // to merge or delete contentd
            ContextMenuStrip dgvTargetVerseContextMenu = new ContextMenuStrip();

            dgvTargetVerseContextMenu.Opening += DgvTargetVerseContextMenu_Opening;
            dgvTargetVerseContextMenu.ItemClicked += DgvTargetVerseContextMenu_ItemClicked;
            dgvTargetVerse.ContextMenuStrip = dgvTargetVerseContextMenu;

            dgvTargetVerse.ColumnAdded += DgvTargetVerse_ColumnAdded;
            dgvTargetVerse.ColumnRemoved += DgvTargetVerse_ColumnRemoved;
           dgvTargetVerse.CellValueChanged += DgvTargetVerse_CellValueChanged;
            // Drag
            // Subscribe to the Reference Verse click event for drag & drop
            dgvReferenceVerse.MouseDown += DgvReferenceVerse_MouseDown;
            dgvTOTHTView.MouseDown += DgvTOTHTView_MouseDown;
            dgvTargetVerse.MouseDown += DgvTargetVerse_MouseDown;
            // And Drop
            dgvTargetVerse.DragEnter += DgvTargetVerse_DragEnter;
            dgvTargetVerse.DragDrop += DgvTargetVerse_DragDrop;



            this.tbReferenceBible.DoubleClick += TbReferenceBible_DoubleClick;
            this.tbTargetBible.DoubleClick += TbTargetBible_DoubleClick;

            dgvReferenceVerse.CellContentDoubleClick += Dgv_CellContentDoubleClick;
            dgvTargetVerse.CellContentDoubleClick += Dgv_CellContentDoubleClick;
            dgvTOTHTView.CellContentDoubleClick += DgvTOTHTView_CellContentDoubleClick;
            verse.VerseChanged += Verse_VerseChanged;

        }

        private void DgvTargetVerse_ColumnRemoved(object sender, DataGridViewColumnEventArgs e)
        {
            TargetDirty = true;
        }

        private void DgvTargetVerse_ColumnAdded(object sender, DataGridViewColumnEventArgs e)
        {
//            TargetDirty = true;
        }

        private void DgvTargetVerse_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            TargetDirty = true;
        }

         public string CurrentVerse
        {
            get { return tbCurrentReference.Text; }
        }

        public void ClearCurrentVerse()
        {
            tbCurrentReference.Text = string.Empty;
        }

        public void SaveCurrentVerse()
        {
            if (!string.IsNullOrEmpty(tbCurrentReference.Text) && dgvTargetVerse.Columns.Count > 0)
            {
                // save current target verse
                SaveVerse(tbCurrentReference.Text);
            }
        }
        public void SaveVerse(string reference)
        {
            string verse = (string)dgvTargetVerse[0, 0].Value + " ";
            string tag = (string)dgvTargetVerse[0, 1].Value;
            if (string.IsNullOrEmpty(tag))
                verse += "<>";
            else
                verse += tag;

            for (int i = 1; i < dgvTargetVerse.Columns.Count; i++)
            {
                verse += " " + (string)dgvTargetVerse[i, 0].Value + " ";
                tag = (string)dgvTargetVerse[i, 1].Value;
                if (string.IsNullOrEmpty(tag))
                    verse += "<>";
                else
                    verse += tag;
            }

            if (parent.TargetVersionUpdates.ContainsKey(reference))
            {
                parent.TargetVersionUpdates[reference] = verse;
            }
            else
            {
                parent.TargetVersionUpdates.Add(reference, verse);
            }
        }

        public void SaveCurrentVerse1()
        {
            if (!string.IsNullOrEmpty(tbCurrentReference.Text) && dgvTargetVerse.Columns.Count > 0)
            {
                // save current target verse
                Dictionary<int, string[]> verseDict = new Dictionary<int, string[]>();
                for (int i = 0; i < dgvTargetVerse.Columns.Count; i++)
                {
                    string[] wordColumn = new string[2];
                    wordColumn[0] = (string)dgvTargetVerse[i, 0].Value;
                    wordColumn[1] = (string)dgvTargetVerse[i, 1].Value;
                    verseDict.Add(i, wordColumn);
                }
            }

        }



        private void Verse_VerseChanged(object sender, VerseChangedEventArgs e)
        {
            string referenceVerse= string.Empty;
            string targetVerse = string.Empty;
            string targetUpdatedVerse = string.Empty;

            testament = e.Testament;

            string oldReference = tbCurrentReference.Text;
            tbCurrentReference.Text = e.VerseReference;


            if (parent.ReferenceVersion.ContainsKey(e.VerseReference))
            {
                referenceVerse = parent.ReferenceVersion[e.VerseReference];
                PopulateReferenceVerseView(referenceVerse);
            }
            else
            {
                referenceVerse = e.VerseReference + " NotFound";
            }

            if (!string.IsNullOrEmpty(oldReference) && dgvTargetVerse.Columns.Count > 0)
            {
                SaveVerse(oldReference);
            }
            if (parent.TargetVersionUpdates.ContainsKey(e.VerseReference))
            {
                targetUpdatedVerse = parent.TargetVersionUpdates[e.VerseReference];
                PopulateTargetUpdatedVerseView(targetUpdatedVerse);
            }
            else 
            if (parent.TargetVersion.ContainsKey(e.VerseReference))
            {
                targetVerse = parent.TargetVersion[e.VerseReference];
                PopulateTargetVerseView(targetVerse);
            }
            else
            {
                targetVerse = e.VerseReference + " NotFound";
            }

            if (parent.TOTHTDict.ContainsKey(e.VerseReference))
            {
                Dictionary<int, VerseWord> verseWords = parent.TOTHTDict[e.VerseReference];
                PopulateTOHTHView(verseWords);
            }
            else if (parent.TAGNTDict.ContainsKey(e.VerseReference))
            {
                Dictionary<int, VerseWord> verseWords = parent.TAGNTDict[e.VerseReference];
                PopulateTAGNTView(verseWords);
            }
            else
            {
                string bookName = e.VerseReference.Substring(0,3);
                if (bookName == "Phi") bookName = e.VerseReference.Replace("Phi", "Php");
                if (bookName == "Jam") bookName = e.VerseReference.Replace("Jam", "Jas");
                if (bookName == "1Jo") bookName = e.VerseReference.Replace("1Jo", "1Jn");
                if (bookName == "2Jo") bookName = e.VerseReference.Replace("2Jo", "2Jn");
                if (bookName == "3Jo") bookName = e.VerseReference.Replace("3Jo", "3Jn");
                if (parent.TAGNTDict.ContainsKey(bookName))
                {
                    Dictionary<int, VerseWord> verseWords = parent.TAGNTDict[bookName];
                    PopulateTAGNTView(verseWords);
                }
            }


                       //if( tempTimer == null)
                       //{
                       //    tempTimer = new System.Timers.Timer();
                       //    tempTimer.AutoReset = false;
                       //    tempTimer.Elapsed += TempTimer_Elapsed;
                       //}
                       //tempTimer.Interval = 100;
                       //tempTimer.Start();
        }

        private void TempTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
//            parent.NextUnknownVerse();
        }


        /// <summary>
        /// Sets the reference bible text box to the path of the file (just as info)
        /// </summary>
        public string ReferenceBible
        {
            set
            {
                tbReferenceBible.Text = value;
            }
        }

        /// <summary>
        /// Sets the target bible text box to the path of the file (just as info)
        /// </summary>
        public string TargetBible
        {
            set
            {
                tbTargetBible.Text = value;
            }
        }


        /// <summary>
        /// Populates the reference verse data grid view with the 
        /// verse words and tags
        /// </summary>
        /// <param name="verse">The reference verse including tags</param>
        private void PopulateReferenceVerseView(string verse)
        {
            dgvReferenceVerse.Rows.Clear();

            string[] verseParts = verse.Trim().Split(' ');
            // count verse words
            List<string> words = new List<string>();
            List<string> tags = new List<string>();
            string tempWord = string.Empty;
            string tmpTag = string.Empty;
            for (int i = 0; i < verseParts.Length; i++)
            {
                string versePart = verseParts[i].Trim();
                if (i == 0 || (versePart[0] != '<' && versePart[0] != '(')) // add i == 0 test because a verse can not start with a tag.
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
                    if(thisTag[0] == '<')
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


            dgvReferenceVerse.ColumnCount = verseWords.Length;
            dgvReferenceVerse.Rows.Add(verseWords);
            dgvReferenceVerse.Rows.Add(verseTags);

            for (int i = 0; i < verseTags.Length; i++)
            {
                string tag = (string)dgvReferenceVerse.Rows[1].Cells[i].Value;
                if (tag == null)
                    continue;
                if (tag.Contains("0410>"))
                {
                    dgvReferenceVerse.Rows[1].Cells[i].Style.BackColor = Color.Yellow;
                }
            }

            dgvReferenceVerse.ClearSelection();

            dgvReferenceVerse.Rows[0].ReadOnly = true;
            dgvReferenceVerse.Rows[1].ReadOnly = true;

        }



        #region Reference verse events
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Dgv_CellContentDoubleClick(object sender, System.Windows.Forms.DataGridViewCellEventArgs e)
        {
            DataGridView dgv = (DataGridView)sender;
                string tag = (String)(dgv.Rows[1].Cells[e.ColumnIndex].Value);
            if(!string.IsNullOrEmpty(tag))
            {
                tag = tag.Replace("<", "").Replace(">", "").Replace(",", "").Replace(".", "").Replace(":", "");

                string[] tags = tag.Split(' ');
                if (tags.Length == 1)
                {
                    browser.NavigateToTag(testament + tags[0]);
                }
                else
                {

                    DataGridViewCell cell = dgv[1, e.RowIndex];
                    ContextMenuStrip cms = cell.ContextMenuStrip;
                    if (cms != null)
                    {
                        cms.ItemClicked -= Cms_ItemClicked;
                        cell.ContextMenuStrip.Dispose();
                        cell.ContextMenuStrip = null;

                    }
                    cms = new ContextMenuStrip();
                    cms.ItemClicked += Cms_ItemClicked;
                    cell.ContextMenuStrip = cms;
                    for(int i = 0; i < tags.Length; i++)
                    {
                        cell.ContextMenuStrip.Items.Add(new ToolStripMenuItem(tags[i]));

                        Rectangle r = cell.DataGridView.GetCellDisplayRectangle(cell.ColumnIndex, cell.RowIndex, false);
                        Point p = new Point(r.X, r.Y + r.Height);
                        cms.Show(cell.DataGridView, p);
                    }
                }
            }

        }

        private void DgvTOTHTView_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            DataGridView dgv = (DataGridView)sender;
                string tag = (String)(dgv.Rows[dgv.Rows.Count - 1].Cells[e.ColumnIndex].Value);
            if(!string.IsNullOrEmpty(tag))
            {
                tag = tag.Replace("<", "").Replace(">", "").Replace(",", "").Replace(".", "").Replace(":", "");

                string[] tags = tag.Split(' ');
                if (tags.Length == 1)
                {
                    browser.NavigateToTag(testament + tags[0]);
                }
                else
                {

                    DataGridViewCell cell = dgv[1, e.RowIndex];
                    ContextMenuStrip cms = cell.ContextMenuStrip;
                    if (cms != null)
                    {
                        cms.ItemClicked -= Cms_ItemClicked;
                        cell.ContextMenuStrip.Dispose();
                        cell.ContextMenuStrip = null;

                    }
                    cms = new ContextMenuStrip();
                    cms.ItemClicked += Cms_ItemClicked;
                    cell.ContextMenuStrip = cms;
                    for(int i = 0; i < tags.Length; i++)
                    {
                        cell.ContextMenuStrip.Items.Add(new ToolStripMenuItem(tags[i]));

                        Rectangle r = cell.DataGridView.GetCellDisplayRectangle(cell.ColumnIndex, cell.RowIndex, false);
                        Point p = new Point(r.X, r.Y + r.Height);
                        cms.Show(cell.DataGridView, p);
                    }
                }
            }

        }

        private void Cms_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            string tag = e.ClickedItem.Text;
            browser.NavigateToTag(testament + tag);
        }


        #endregion Reference verse events


        private void DgvTargetVerseContextMenu_Opening(object sender, CancelEventArgs e)
        {
            e.Cancel = true;
            if (dgvTargetVerse.SelectedCells.Count == 0)
                return;

            if (dgvTargetVerse.SelectedCells.Count > 1)
            {
                bool sameRow = true;
                foreach (DataGridViewCell cell in dgvTargetVerse.SelectedCells)
                {
                    // we only merge in the top row
                    if (cell.RowIndex != 0)
                    {
                        sameRow = false;
                        break;
                    }
                }

                if (sameRow)
                {
                    dgvTargetVerse.ContextMenuStrip.Items.Clear();
                    ToolStripMenuItem mergeMenuItem = new ToolStripMenuItem(MERGE_CONTEXT_MENU);
 
                    dgvTargetVerse.ContextMenuStrip.Items.Add(mergeMenuItem);
                    e.Cancel = false;
                }
            }
            else
            {
                if (dgvTargetVerse.SelectedCells[0].RowIndex == 1)
                {
                    string text = (String)dgvTargetVerse.SelectedCells[0].Value;

                    if (string.IsNullOrEmpty(text))
                        return;

                    dgvTargetVerse.ContextMenuStrip.Items.Clear();

                    dgvTargetVerse.ContextMenuStrip.Items.Clear();

                    string[] strings = text.Split(' ');
                    if (strings.Length > 1)
                    {
                        ToolStripMenuItem reverseMenuItem = new ToolStripMenuItem(REVERSE_CONTEXT_MENU);
                        dgvTargetVerse.ContextMenuStrip.Items.Add(reverseMenuItem);

                        ToolStripMenuItem deleteLeftMenuItem = new ToolStripMenuItem(DELETE_LEFT_CONTEXT_MENU);
                        dgvTargetVerse.ContextMenuStrip.Items.Add(deleteLeftMenuItem);

                        ToolStripMenuItem deleteRightMenuItem = new ToolStripMenuItem(DELETE_RIGHT_CONTEXT_MENU);
                        dgvTargetVerse.ContextMenuStrip.Items.Add(deleteRightMenuItem);
                    }

                    ToolStripMenuItem deleteMenuItem = new ToolStripMenuItem(DELETE_CONTEXT_MENU);
                    dgvTargetVerse.ContextMenuStrip.Items.Add(deleteMenuItem);


                    e.Cancel = false;
                }
                else
                {
                    dgvTargetVerse.ContextMenuStrip.Items.Clear();
                    ToolStripMenuItem deleteMenuItem = new ToolStripMenuItem(SPLIT_CONTEXT_MENU);

                    dgvTargetVerse.ContextMenuStrip.Items.Add(deleteMenuItem);
                    e.Cancel = false;
                }
            }
        }

        private void DgvTargetVerseContextMenu_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            if (e.ClickedItem.Text == MERGE_CONTEXT_MENU)
            {
                string mergedText = string.Empty;

                // the selected cells array is not in column index order
                // find the first column index
                int firstMergeIndex = dgvTargetVerse.SelectedCells[0].ColumnIndex;
                for (int i = 1; i < dgvTargetVerse.SelectedCells.Count; i++)
                {
                    if (dgvTargetVerse.SelectedCells[i].ColumnIndex < firstMergeIndex)
                        firstMergeIndex = dgvTargetVerse.SelectedCells[i].ColumnIndex;
                }

                string[] newWords = new string[dgvTargetVerse.Columns.Count - (dgvTargetVerse.SelectedCells.Count - 1)];
                string[] newTags = new string[dgvTargetVerse.Columns.Count - (dgvTargetVerse.SelectedCells.Count - 1)];

                int columnIndex = 0;
                for (int i = 0; i < newWords.Length; i++)
                {
                    string newText = (string)dgvTargetVerse.Rows[0].Cells[columnIndex].Value;
                    string newTag = (string)dgvTargetVerse.Rows[1].Cells[columnIndex].Value;
                    if (string.IsNullOrEmpty(newTag) || newTag.Contains("???")) newTag = string.Empty;
                    if (columnIndex == firstMergeIndex)
                    {
                        for (int j = 1; j < dgvTargetVerse.SelectedCells.Count; j++)
                        {
                            columnIndex += 1;
                            newText += " " + (string)dgvTargetVerse.Rows[0].Cells[columnIndex].Value;
                            string currentTag = (string)dgvTargetVerse.Rows[1].Cells[columnIndex].Value;
                            if (!string.IsNullOrEmpty(currentTag) && !currentTag.Contains("???"))
                            {
                                if(!newTag.Contains(currentTag))
                                    newTag += " " + currentTag;
                            }
                        }
                    }

                    newWords[i] = newText;
                    newTags[i] = newTag;
                    columnIndex++;
                }

                dgvTargetVerse.ColumnCount = newWords.Length;

                dgvTargetVerse.Rows.RemoveAt(0);
                dgvTargetVerse.Rows.Insert(0, newWords);

                dgvTargetVerse.Rows.RemoveAt(1);
                dgvTargetVerse.Rows.Insert(1, newTags);

            }
            else if (e.ClickedItem.Text == SPLIT_CONTEXT_MENU)
            {
                // the selected cells array is not in column index order
                // find the first column index
                int splitIndex = dgvTargetVerse.SelectedCells[0].ColumnIndex;

                string stringToSplit = (string)dgvTargetVerse.Rows[0].Cells[splitIndex].Value;
                string[] splitWords = stringToSplit.Split(' ');
                if (splitWords.Length == 1)
                {
                    // nothing to do
                    return;
                }

                string tagToSplit = (string)dgvTargetVerse.Rows[1].Cells[splitIndex].Value;
                string[] splitTags;
                if (string.IsNullOrEmpty(tagToSplit))
                {
                    splitTags = new string[1];
                    splitTags[0] = String.Empty;
                }
                else
                    splitTags = tagToSplit.Split(' ');

                string[] newWords = new string[dgvTargetVerse.Columns.Count + splitWords.Length - 1];
                string[] newTags = new string[dgvTargetVerse.Columns.Count + splitWords.Length - 1];

                int columnIndex = 0;
                for (int i = 0; i < splitIndex; i++)
                {
                    newWords[i] = (string)dgvTargetVerse.Rows[0].Cells[columnIndex].Value;
                    newTags[i] = (string)dgvTargetVerse.Rows[1].Cells[columnIndex].Value;
                    columnIndex++;
                }
                for (int i = splitIndex; i < (splitIndex + splitWords.Length); i++)
                {
                    newWords[i] = splitWords[i - splitIndex];
                    if (splitTags.Length - 1 >= (i - splitIndex))
                        newTags[i] = splitTags[i - splitIndex];
                    else
                        newTags[i] = "";
                }
                columnIndex++;

                for (int i = splitIndex + splitWords.Length; i < newWords.Length; i++)
                {
                    newWords[i] = (string)dgvTargetVerse.Rows[0].Cells[columnIndex].Value;
                    newTags[i] = (string)dgvTargetVerse.Rows[1].Cells[columnIndex].Value;
                    columnIndex++;
                }

                dgvTargetVerse.Rows.Clear();

                dgvTargetVerse.ColumnCount = newWords.Length;
                dgvTargetVerse.Rows.Add(newWords);
                dgvTargetVerse.Rows.Add(newTags);
                string direction = Properties.Settings.Default.TargetTextDirection;
                if (direction.ToLower() == "rtl")
                {
                    for (int i = 0; i < newWords.Length; i++)
                    {
                        dgvTargetVerse.Columns[i].DisplayIndex = newWords.Length - i - 1;
                    }
                }

                dgvTargetVerse.ClearSelection();

                dgvTargetVerse.Rows[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                dgvTargetVerse.Rows[0].ReadOnly = true;

                //                dgvTargetVerse.Rows.RemoveAt(0);
                //                dgvTargetVerse.Rows.Insert(0, newWords);

                //                dgvTargetVerse.Rows.RemoveAt(1);
                //                dgvTargetVerse.Rows.Insert(1, newTags);

            }
            else if (e.ClickedItem.Text == DELETE_CONTEXT_MENU)
            {
                dgvTargetVerse.SelectedCells[0].Value = string.Empty;
            }
            else
            {
                string[] strings = ((string)dgvTargetVerse.SelectedCells[0].Value).Split(' ');
                if (strings.Length < 2) return;

                string newText = string.Empty;
                if (e.ClickedItem.Text == REVERSE_CONTEXT_MENU)
                {
                    newText= strings[strings.Length - 1];
                    for(int i = strings.Length - 2; i >= 0; i--)
                    {
                        newText += " " + strings[i];
                    }
                    dgvTargetVerse.SelectedCells[0].Value = newText;
                }
                else if (e.ClickedItem.Text == DELETE_LEFT_CONTEXT_MENU)
                {
                    for(int i = 1; i < strings.Length; i++)
                    {
                        newText += string.IsNullOrEmpty(newText)? strings[i] : " " + strings[i];
                    }
                    dgvTargetVerse.SelectedCells[0].Value = newText;
                }
                else if (e.ClickedItem.Text == DELETE_RIGHT_CONTEXT_MENU)
                {
                    for (int i = 0; i < strings.Length -1; i++)
                    {
                        newText += string.IsNullOrEmpty(newText) ? strings[i] : " " + strings[i];
                    }
                    dgvTargetVerse.SelectedCells[0].Value = newText;
                }
            }



        }

        #region Drag & Dop
        class DragData
       {
            public DragData(int rowIndex, int columnIndex, string text, DataGridView source)
            {
                Text = text;
                Source = source;
                ColumnIndex = columnIndex;
                RowIndex= rowIndex;
            }

            public string Text { get; private set; }
            public DataGridView Source { get; private set; }
            public int ColumnIndex { get; private set; }
            public int RowIndex { get; private set; }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DgvReferenceVerse_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && e.Clicks == 1)
            {
                DataGridView.HitTestInfo info = dgvReferenceVerse.HitTest(e.X, e.Y);
                if (info.RowIndex >= 0)
                {
                    if (info.RowIndex >= 0 && info.ColumnIndex >= 0)
                    {
                        string text = (String) dgvReferenceVerse.Rows[1].Cells[info.ColumnIndex].Value;
                        if (text != null)
                        {
                            DragData data = new DragData(1, info.ColumnIndex, text.Trim(), dgvReferenceVerse);
                            dgvReferenceVerse.DoDragDrop(data, DragDropEffects.Copy);
                        }
                    }
                }
            }
        }

        private void DgvTOTHTView_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && e.Clicks == 1)
            {
                DataGridView.HitTestInfo info = dgvTOTHTView.HitTest(e.X, e.Y);
                if (info.RowIndex >= 0)
                {
                    if (info.RowIndex >= 0 && info.ColumnIndex >= 0)
                    {
                        string text = ((String)dgvTOTHTView.Rows[3].Cells[info.ColumnIndex].Value).Trim();
                        DragData data = new DragData(1, info.ColumnIndex, text, dgvTOTHTView);
                        if (text != null)
                        {
                            //Need to put braces here  CHANGE
                            dgvTOTHTView.DoDragDrop(data, DragDropEffects.Copy);
                        }
                    }
                }
            }
        }

        private void DgvTargetVerse_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && e.Clicks == 1)
            {
                DataGridView.HitTestInfo info = dgvTargetVerse.HitTest(e.X, e.Y);
                if (info.RowIndex > 0)
                {
                    if (info.RowIndex >= 0 && info.ColumnIndex >= 0)
                    {
                        string text = (String)dgvTargetVerse.Rows[1].Cells[info.ColumnIndex].Value;
                        DragData data = new DragData(1, info.ColumnIndex, text, dgvTargetVerse);
                        if (text != null)
                        {
                            //Need to put braces here  CHANGE
                            dgvTargetVerse.DoDragDrop(data, DragDropEffects.Copy);
                        }
                    }
                }
            }
        }

        private void DgvTargetVerse_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Copy;
        }

        private void DgvTargetVerse_DragDrop(object sender, DragEventArgs e)
        {
            DragData data = e.Data.GetData(typeof(DragData)) as DragData;
            string newValue = data.Text;
            Point cursorLocation = dgvTargetVerse.PointToClient(new Point(e.X, e.Y));

            System.Windows.Forms.DataGridView.HitTestInfo hittest = dgvTargetVerse.HitTest(cursorLocation.X, cursorLocation.Y);
            if (hittest.ColumnIndex != -1
                && hittest.RowIndex != -1)
            {  //CHANGE
                if (data.Source.Equals(dgvTargetVerse)) 
                {
                    if (data.ColumnIndex == hittest.ColumnIndex)
                        return;
                }
                newValue = newValue.Replace("+G", "> <");
                string[] strings= newValue.Split(' ');
                newValue = string.Empty;
                string currentText = (string)dgvTargetVerse[hittest.ColumnIndex, 1].Value;
                if (!string.IsNullOrEmpty(currentText) && !currentText.Contains("???"))
                    newValue = currentText;
                for (int i = 0; i < strings.Length; i++)
                {
                    string tmp = strings[i].Trim().Replace("<", "").Replace(">", "");
                    tmp = ("0000" + tmp).Substring(tmp.Length);
                    int val = Convert.ToInt32(tmp);
                    if (val > 0)
                    {
                        tmp = "<" + tmp + ">";
                        if (!newValue.Contains(tmp))
                            newValue += string.IsNullOrEmpty(newValue) ? tmp : (" " + tmp);
                    }
                }
                dgvTargetVerse[hittest.ColumnIndex, 1].Value = newValue.Trim();

                if (data.Source.Equals(dgvTargetVerse))
                {
                    dgvTargetVerse[data.ColumnIndex, 1].Value = string.Empty;
                }

                dgvTargetVerse.ClearSelection();

                dgvTargetVerse.Rows[0].ReadOnly = true;
            }
        }

        #endregion Drag & Dop

        private void TbTargetBible_DoubleClick(object sender, System.EventArgs e)
        {
            parent.LoadTargetFile();
        }

        private void TbReferenceBible_DoubleClick(object sender, System.EventArgs e)
        {
            parent.LoadReferenceFile();
        }


        private void lblPrevious_Click(object sender, EventArgs e)
        {
            verse.MoveToPrevious();
        }
        private void lblNext_Click(object sender, EventArgs e)
        {
            verse.MoveToNext();
        }

        private void btnResetVerse_Click(object sender, EventArgs e)
        {
            string caption = "Reset Current Verse";
            string message = "Reseting clears all tags, Continu?";
            MessageBoxButtons buttons = MessageBoxButtons.YesNo;
            DialogResult result = MessageBox.Show(message, caption, buttons);

            if (result == System.Windows.Forms.DialogResult.Yes)
            {

                string targetVerse = tbCurrentReference.Text;
                if(parent.TargetVersionUpdates.ContainsKey(targetVerse))
                {
                    string targetUpdatedVerse = parent.TargetVersionUpdates[targetVerse];
                    PopulateTargetUpdatedVerseView(targetUpdatedVerse);

                }
                else if (parent.TargetVersion.ContainsKey(targetVerse))
                {
                    targetVerse = parent.TargetVersion[targetVerse];
                    PopulateTargetVerseView(targetVerse);
                }
                else
                {
                    targetVerse = targetVerse + " NotFound";
                }
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            parent.SaveUpdates();
        }

        private void btnNextUnknownTag_Click(object sender, EventArgs e)
        {
            parent.NextUnknownVerse();
        }

        private void btnDecreseFont_Click(object sender, EventArgs e)
        {
            Font  font = dgvReferenceVerse.DefaultCellStyle.Font;
            dgvReferenceVerse.DefaultCellStyle.Font = new Font(font.Name, font.Size - 1);
            font = dgvReferenceVerse.DefaultCellStyle.Font;
            dgvTargetVerse.DefaultCellStyle.Font = new Font(font.Name, font.Size - 1);
            font = dgvTOTHTView.DefaultCellStyle.Font;
            dgvTOTHTView.DefaultCellStyle.Font = new Font(font.Name, font.Size - 1);

        }

        private void btnIncreasFont_Click(object sender, EventArgs e)
        {
            Font font = dgvReferenceVerse.DefaultCellStyle.Font;
            dgvReferenceVerse.DefaultCellStyle.Font = new Font(font.Name, font.Size + 1);
            font = dgvReferenceVerse.DefaultCellStyle.Font;
            dgvTargetVerse.DefaultCellStyle.Font = new Font(font.Name, font.Size + 1);
            font = dgvTOTHTView.DefaultCellStyle.Font;
            dgvTOTHTView.DefaultCellStyle.Font = new Font(font.Name, font.Size + 1);
        }

        private void btnEbaleEdit_Click(object sender, EventArgs e)
        {
            dgvTargetVerse.Rows[0].ReadOnly = false;
        }

        private void tbCurrentReference_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
