using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Forms;

using WeifenLuo.WinFormsUI.Docking;
using static System.Net.Mime.MediaTypeNames;

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



        private BibleTaggingForm container;
        private BrowserPanel browser;
        private VerseSelectionPanel verse;

        private string testament = string.Empty;

        private System.Timers.Timer tempTimer = null;

        public bool TargetDirty { get; set; }

        class VerseDetails
        {
            public VerseDetails()
            {
                OldTestament = false;
            }
            public bool OldTestament { get; set; }
            public string[] VerseWords { get; set; }
            public string[] VerseTags { get; set; }
        }

        #region Constructors
        public EditorPanel()
        {
            InitializeComponent();
            this.ControlBox = false;
            this.CloseButtonVisible = false;
            this.CloseButton = false;
        }

        public EditorPanel(BibleTaggingForm container, BrowserPanel browser, VerseSelectionPanel verse)
        {
            InitializeComponent();

            this.ControlBox = false;
            this.CloseButtonVisible = false;
            this.CloseButton = false;

            System.Drawing.Image img = picRedo.Image;
            img.RotateFlip(RotateFlipType.Rotate180FlipY);
            picRedo.Image = img;
            //picRedo.Invalidate();

            this.container = container;
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

            dgvTargetVerse.CellEnter += DgvTargetVerse_CellEnter;

            dgvReferenceVerse.CellContentDoubleClick += Dgv_CellContentDoubleClick;
            dgvTargetVerse.CellContentDoubleClick += Dgv_CellContentDoubleClick;
            dgvTOTHTView.CellContentDoubleClick += DgvTOTHTView_CellContentDoubleClick;
            verse.VerseChanged += Verse_VerseChanged;

            this.PreviewKeyDown += EditorPanel_PreviewKeyDown;
            this.KeyDown += EditorPanel_KeyDown;

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
        private void Verse_VerseChanged(object sender, VerseChangedEventArgs e)
        {
            string referenceVerse= string.Empty;
            string targetVerse = string.Empty;
            string targetUpdatedVerse = string.Empty;

            testament = e.Testament;

            string oldReference = tbCurrentReference.Text;
            tbCurrentReference.Text = e.VerseReference;


            if (container.KJV.Bible.ContainsKey(e.VerseReference))
            {
                referenceVerse = Utils.GetVerseText(container.KJV.Bible[e.VerseReference], true);
                PopulateReferenceVerseView(container.KJV.Bible[e.VerseReference]);
            }
            else
            {
                referenceVerse = e.VerseReference + " NotFound";
            }

            if (!string.IsNullOrEmpty(oldReference) && dgvTargetVerse.Columns.Count > 0)
            {
                SaveVerse(oldReference);
            }
            if (container.Target.Bible.ContainsKey(e.VerseReference))
            {
                targetUpdatedVerse = Utils.GetVerseText(container.Target.Bible[e.VerseReference], true);
                UpdateTargetView(container.Target.Bible[e.VerseReference]);
            }
            else
            {
                targetVerse = e.VerseReference + " NotFound";
            }

            if (container.TOTHT.Bible.ContainsKey(e.VerseReference))
            {
                Verse verseWords = container.TOTHT.Bible[e.VerseReference];
                PopulateTOHTHView(verseWords);
            }
            else if (container.TAGNT.Bible.ContainsKey(e.VerseReference))
            {
                Verse verseWords = container.TAGNT.Bible[e.VerseReference];
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
                if (container.TAGNT.Bible.ContainsKey(bookName))
                {
                    Verse verseWords = container.TAGNT.Bible[bookName];
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
            //            container.FindVerse(cbTagToFind.Text);
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

        #region Context Menu
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

                    string[] strings = text.Trim().Split(' ');
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
                if (currentVerse != null)
                    undoStack.Push(new Verse(currentVerse));

                int firstMergeIndex = dgvTargetVerse.SelectedCells[0].ColumnIndex;
                for (int i = 1; i < dgvTargetVerse.SelectedCells.Count; i++)
                {
                    if (dgvTargetVerse.SelectedCells[i].ColumnIndex < firstMergeIndex)
                        firstMergeIndex = dgvTargetVerse.SelectedCells[i].ColumnIndex;
                }
                currentVerse.Merge(firstMergeIndex, dgvTargetVerse.SelectedCells.Count);

                SaveVerse(currentVerse);
                UpdateTargetView(currentVerse);
            }
            else if (e.ClickedItem.Text == SPLIT_CONTEXT_MENU)
            {
                if (currentVerse != null)
                    undoStack.Push(new Verse(currentVerse));

                int splitIndex = dgvTargetVerse.SelectedCells[0].ColumnIndex;
                for (int i = 1; i < dgvTargetVerse.SelectedCells.Count; i++)
                {
                    if (dgvTargetVerse.SelectedCells[i].ColumnIndex < splitIndex)
                        splitIndex = dgvTargetVerse.SelectedCells[i].ColumnIndex;
                }
                currentVerse.split(splitIndex);

                SaveVerse(currentVerse);
                UpdateTargetView(currentVerse);
            }
            else if (e.ClickedItem.Text == DELETE_CONTEXT_MENU)
            {
                if (currentVerse != null)
                    undoStack.Push(new Verse(currentVerse));

                int col = dgvTargetVerse.SelectedCells[0].ColumnIndex;
                currentVerse[col].Strong = new string[] { string.Empty };
                SaveVerse(currentVerse);
                UpdateTargetView(currentVerse);
            }
            else
            {
                string[] strings = ((string)dgvTargetVerse.SelectedCells[0].Value).Split(' ');
                if (strings.Length < 2) return;

                if (currentVerse != null)
                    undoStack.Push(new Verse(currentVerse));

                string newText = string.Empty;
                if (e.ClickedItem.Text == REVERSE_CONTEXT_MENU)
                {
                    newText = strings[strings.Length - 1];
                    for (int i = strings.Length - 2; i >= 0; i--)
                    {
                        newText += " " + strings[i];
                    }
                    dgvTargetVerse.SelectedCells[0].Value = newText;

                    int col = dgvTargetVerse.SelectedCells[0].ColumnIndex;
                    currentVerse[col].StrongString = newText;
                    SaveVerse(currentVerse);
                }
                else if (e.ClickedItem.Text == DELETE_LEFT_CONTEXT_MENU)
                {
                    for (int i = 1; i < strings.Length; i++)
                    {
                        newText += string.IsNullOrEmpty(newText) ? strings[i] : " " + strings[i];
                    }
                    dgvTargetVerse.SelectedCells[0].Value = newText;

                    int col = dgvTargetVerse.SelectedCells[0].ColumnIndex;
                    currentVerse[col].StrongString = newText;
                    SaveVerse(currentVerse);
                }
                else if (e.ClickedItem.Text == DELETE_RIGHT_CONTEXT_MENU)
                {
                    for (int i = 0; i < strings.Length - 1; i++)
                    {
                        newText += string.IsNullOrEmpty(newText) ? strings[i] : " " + strings[i];
                    }
                    dgvTargetVerse.SelectedCells[0].Value = newText;

                    int col = dgvTargetVerse.SelectedCells[0].ColumnIndex;
                    currentVerse[col].StrongString = newText;
                    SaveVerse(currentVerse);
                }
            }



        }

        private void DgvTargetVerseContextMenu_ItemClicked1(object sender, ToolStripItemClickedEventArgs e)
        {
            if (e.ClickedItem.Text == MERGE_CONTEXT_MENU)
            {
                if (currentVerse != null)
                    undoStack.Push(new Verse(currentVerse));

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
                                if (!newTag.Contains(currentTag))
                                    newTag += " " + currentTag;
                            }
                        }
                    }

                    newWords[i] = newText;
                    newTags[i] = newTag;
                    currentVerse[i].Word = newText;
                    currentVerse[i].StrongString = newTag;
                    columnIndex++;
                }

                SaveVerse(currentVerse);
                UpdateTargetView(currentVerse);

                /*                dgvTargetVerse.ColumnCount = newWords.Length;

                                dgvTargetVerse.Rows.RemoveAt(0);
                                dgvTargetVerse.Rows.Insert(0, newWords);

                                dgvTargetVerse.Rows.RemoveAt(1);
                                dgvTargetVerse.Rows.Insert(1, newTags);*/
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

                if (currentVerse != null)
                    undoStack.Push(new Verse(currentVerse));

                for (int i = 0; i < newWords.Length; i++)
                {
                    currentVerse[i].Word = newWords[i];
                    currentVerse[i].StrongString = newTags[i];
                }
                SaveVerse(currentVerse);
                UpdateTargetView(currentVerse);
/*
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
                dgvTargetVerse.Rows[0].ReadOnly = true;*/

                //                dgvTargetVerse.Rows.RemoveAt(0);
                //                dgvTargetVerse.Rows.Insert(0, newWords);

                //                dgvTargetVerse.Rows.RemoveAt(1);
                //                dgvTargetVerse.Rows.Insert(1, newTags);

            }
            else if (e.ClickedItem.Text == DELETE_CONTEXT_MENU)
            {
                dgvTargetVerse.SelectedCells[0].Value = string.Empty;
                if (currentVerse != null)
                    undoStack.Push(new Verse(currentVerse));

                int col = dgvTargetVerse.SelectedCells[0].ColumnIndex;
                currentVerse[col].Strong = new string[] { string.Empty };
                SaveVerse(currentVerse);
            }
            else
            {
                string[] strings = ((string)dgvTargetVerse.SelectedCells[0].Value).Split(' ');
                if (strings.Length < 2) return;

                if (currentVerse != null)
                    undoStack.Push(new Verse(currentVerse));

                string newText = string.Empty;
                if (e.ClickedItem.Text == REVERSE_CONTEXT_MENU)
                {
                    newText = strings[strings.Length - 1];
                    for (int i = strings.Length - 2; i >= 0; i--)
                    {
                        newText += " " + strings[i];
                    }
                    dgvTargetVerse.SelectedCells[0].Value = newText;

                    int col = dgvTargetVerse.SelectedCells[0].ColumnIndex;
                    currentVerse[col].StrongString = newText;
                    SaveVerse(currentVerse);
                }
                else if (e.ClickedItem.Text == DELETE_LEFT_CONTEXT_MENU)
                {
                    for (int i = 1; i < strings.Length; i++)
                    {
                        newText += string.IsNullOrEmpty(newText) ? strings[i] : " " + strings[i];
                    }
                    dgvTargetVerse.SelectedCells[0].Value = newText;

                    int col = dgvTargetVerse.SelectedCells[0].ColumnIndex;
                    currentVerse[col].StrongString = newText;
                    SaveVerse(currentVerse);
                }
                else if (e.ClickedItem.Text == DELETE_RIGHT_CONTEXT_MENU)
                {
                    for (int i = 0; i < strings.Length - 1; i++)
                    {
                        newText += string.IsNullOrEmpty(newText) ? strings[i] : " " + strings[i];
                    }
                    dgvTargetVerse.SelectedCells[0].Value = newText;

                    int col = dgvTargetVerse.SelectedCells[0].ColumnIndex;
                    currentVerse[col].StrongString = newText;
                    SaveVerse(currentVerse);
                }
            }



        }
        #endregion Context Menu

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
                    if (strings[i].Contains('('))
                        continue; //skip morphology

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
                if (currentVerse != null)
                    undoStack.Push(new Verse(currentVerse));

                dgvTargetVerse[hittest.ColumnIndex, 1].Value = newValue.Trim();
                currentVerse[hittest.ColumnIndex].StrongString = newValue.Trim();
                SaveVerse(currentVerse);

                SelectReferenceTags(newValue.Trim());

                if (data.Source.Equals(dgvTargetVerse))
                {
                    dgvTargetVerse[data.ColumnIndex, 1].Value = string.Empty;
                }

                dgvTargetVerse.ClearSelection();
                dgvTargetVerse[hittest.ColumnIndex, 1].Selected= true;
                dgvTargetVerse.Rows[0].ReadOnly = true;
                new Thread(() => { SelectReferenceTags(newValue.Trim()); }).Start();
                dgvTargetVerse.Focus();
            }
        }

        #endregion Drag & Dop

        #region Higlight same tag
        private void DgvTargetVerse_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvTargetVerse.SelectedCells.Count == 1)
            {
                SelectReferenceTags((string)dgvTargetVerse.Rows[1].Cells[e.ColumnIndex].Value);
            }
        }


        private void SelectReferenceTags(string tag)
        {
            if (InvokeRequired)
            {
                Action safeWrite = delegate { SelectReferenceTags(tag); };
                Invoke(safeWrite);
            }
            else
            {
                try
                {
                    dgvReferenceVerse.ClearSelection();
                    dgvTOTHTView.ClearSelection();

                    if (string.IsNullOrEmpty(tag))
                    {
                        SetHighlightedCell(dgvTOTHTView, null, null);
                        SetHighlightedCell(dgvReferenceVerse, null, null);
                        return;
                    }

                    string[] tags = tag.Trim().Split(' ');
                    string[] tags1 = new string[tags.Length];
                    string[] tags2 = new string[tags.Length];
                    for (int i = 0; i < tags.Length; i++)
                    {
                        tags1[i] = tags[i].Replace("<", "").Replace(">", "");
                        tags2[i] = tags[i];
                        while (tags2[i][1] == '0')
                            tags2[i] = tags2[i].Remove(1, 1);
                    }

                    SetHighlightedCell(dgvTOTHTView, tags1, tags2);
                    SetHighlightedCell(dgvReferenceVerse, tags1, tags2);

                }
                catch (Exception ex)
                {
                    int x = 0;
                }
            }
        }

        private void SetHighlightedCell(DataGridView dgv, string[] tags1, string[] tags2)
        {
            int count = dgv.ColumnCount;
            int tagsRow = dgv.RowCount - 1;
            for (int i = 0; i < count; i++)
            {
                dgv.Rows[tagsRow].Cells[i].Style.BackColor = Color.White;
                dgv.Rows[tagsRow].Cells[i].Selected = false;

                if (tags1 == null)
                    continue;
                string refTag = (string)dgv.Rows[tagsRow].Cells[i].Value;

                if (!string.IsNullOrEmpty(refTag))
                    for (int j = 0; j < tags1.Length; j++)
                    {
                        if (refTag.Contains(tags1[j]) || refTag.Contains(tags2[j]))
                        {
                            dgv.Rows[tagsRow].Cells[i].Style.BackColor = Color.Yellow;
                        }
                    }
            }

        }
        #endregion Higlight same tag

        private void EditorPanel_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            e.IsInputKey = true;
        }
        private void EditorPanel_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Modifiers == Keys.PageUp)
            {
                verse.MoveToPrevious();
            }
            else if (e.Modifiers == Keys.PageDown)
            {
                verse.MoveToNext();
            }
        }

        #region Buttons
        private void picPrevVerse_Click(object sender, EventArgs e)
        {
            verse.MoveToPrevious();
        }

        private void picNextVerse_Click(object sender, EventArgs e)
        {
            verse.MoveToNext();
        }

        private void picRedo_Click(object sender, EventArgs e)
        {
            if (redoStack.Count > 0)
            {
                Verse verse = redoStack.Pop();
                if (Utils.AreReferencesEqual(tbCurrentReference.Text, verse[0].Reference))
                {
                    UpdateTargetView(verse);
                }
                SaveVerse(verse);
            }
        }

        private void picUndo_Click(object sender, EventArgs e)
        {
            if (undoStack.Count > 0)
            {
                redoStack.Push(new Verse(currentVerse));
                Verse verse = undoStack.Pop();
                if(Utils.AreReferencesEqual(tbCurrentReference.Text, verse[0].Reference))
                {
                    UpdateTargetView(verse);
                }
                SaveVerse(verse);
            }
        }

        private void picSave_Click(object sender, EventArgs e)
        {
            container.Target.SaveUpdates();
        }

        private void picDecreaseFont_Click(object sender, EventArgs e)
        {
            Font  font = dgvReferenceVerse.DefaultCellStyle.Font;
            dgvReferenceVerse.DefaultCellStyle.Font = new Font(font.Name, font.Size - 1);
            font = dgvReferenceVerse.DefaultCellStyle.Font;
            dgvTargetVerse.DefaultCellStyle.Font = new Font(font.Name, font.Size - 1);
            font = dgvTOTHTView.DefaultCellStyle.Font;
            dgvTOTHTView.DefaultCellStyle.Font = new Font(font.Name, font.Size - 1);

        }
        private void picIncreaseFont_Click(object sender, EventArgs e)
        {
            Font font = dgvReferenceVerse.DefaultCellStyle.Font;
            dgvReferenceVerse.DefaultCellStyle.Font = new Font(font.Name, font.Size + 1);
            font = dgvReferenceVerse.DefaultCellStyle.Font;
            dgvTargetVerse.DefaultCellStyle.Font = new Font(font.Name, font.Size + 1);
            font = dgvTOTHTView.DefaultCellStyle.Font;
            dgvTOTHTView.DefaultCellStyle.Font = new Font(font.Name, font.Size + 1);
        }

        private void picEnableEdit_Click(object sender, EventArgs e)
        {
            dgvTargetVerse.Rows[0].ReadOnly = false;
        }

        private void picFindTagForward_Click(object sender, EventArgs e)
        {
            container.FindVerse(cbTagToFind.Text);
        }


        #endregion Buttons

    }
}
