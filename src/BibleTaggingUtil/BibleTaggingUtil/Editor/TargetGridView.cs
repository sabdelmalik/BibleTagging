using Microsoft.VisualBasic.Devices;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using BibleTaggingUtil.BibleVersions;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.ComponentModel;
using System.Xml.Linq;
using static WeifenLuo.WinFormsUI.Docking.DockPanel;
using System.Reflection;
using System.Text.RegularExpressions;

namespace BibleTaggingUtil.Editor
{
    public class TargetGridView : DataGridView
    {

        public event VerseViewChangedEventHandler VerseViewChanged;
        public event RefernceHighlightRequestEventHandler RefernceHighlightRequest;
        public event GotoVerseRequestEventHandler GotoVerseRequest;

        private FixedSizeStack<Verse> undoStack = new FixedSizeStack<Verse>();
        private FixedSizeStack<Verse> redoStack = new FixedSizeStack<Verse>();

        public TargetGridView()
        {
            this.ContextMenuStrip = new ContextMenuStrip();
            this.ContextMenuStrip.Opening += ContextMenuStrip_Opening;
            this.ContextMenuStrip.ItemClicked += ContextMenuStrip_ItemClicked;
        }

        public string CurrentVerseReferece { get; set; }

        public string SearchTag { get; set; }

        public Verse CurrentVerse { get; set; }

        public TargetVersion Bible { get; set; }

        public bool IsLastWord
        {
            get
            {
                if (this.SelectedCells.Count == 1 &&
                    this.SelectedCells[0].ColumnIndex == Columns.Count - 1)
                    return true;
                else
                    return false;
            }
        }

        public bool IsFirstWord
        {
            get
            {
                if (this.SelectedCells.Count == 1 &&
                    this.SelectedCells[0].ColumnIndex == 0)
                    return true;
                else
                    return false;
            }
        }

        public bool IsCurrentTextAramaic
        {
            get
            {
                bool result = false;

                string referencePattern = @"^([0-9A-Za-z]+)\s([0-9]+):([0-9]+)";
                Match mTx = Regex.Match(CurrentVerseReferece, referencePattern);
                if (!mTx.Success)
                {
                    Tracing.TraceError(MethodBase.GetCurrentMethod().Name, "Incorrect reference format: " + CurrentVerseReferece);
                    return result;
                }

                String book = mTx.Groups[1].Value;
                string chapter = mTx.Groups[2].Value;
                string verse = mTx.Groups[3].Value;
                int ch = 0;
                int vs = 0;
                if (!int.TryParse(chapter, out ch))
                    return result;
                if (!int.TryParse(verse, out vs))
                    return result;
                if ((book == "Gen" && ch == 31 && vs == 47) ||
                    (book == "Ezr" && ((ch == 4 && vs >= 8) || (ch == 5) || (ch == 6 && vs <= 18))) ||
                    (book == "Ezr" && (ch >= 7 && vs >= 12 && vs <= 26)) ||
                    (book == "Pro" && ch == 31 && vs == 2) ||
                    (book == "Jer" && ch == 10 && vs == 11) ||
                    (book == "Dan" && ((ch == 2 && vs >= 4) || (ch > 2 && ch < 7) || (ch == 7 && vs <= 28))))
                    result = true;

                return result;
            }
        }

        #region Context Menue

        private const string MERGE_CONTEXT_MENU = "Merge";
        private const string SWAP_CONTEXT_MENU = "Swap Tags";
        private const string SPLIT_CONTEXT_MENU = "Split";
        private const string DELETE_CONTEXT_MENU = "Delete Tag";
        private const string REVERSE_CONTEXT_MENU = "Reverse Tags";
        private const string DELETE_LEFT_CONTEXT_MENU = "Delete Left Tags";
        private const string DELETE_RIGHT_CONTEXT_MENU = "Delete Right Tags";

        private void ContextMenuStrip_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            if (this.SelectedCells.Count == 0)
                return;

            if (this.SelectedCells.Count > 1)
            {
                bool sameRow = true;
                foreach (DataGridViewCell cell in this.SelectedCells)
                {
                    // we only merge in the top row
                    if (cell.RowIndex != 0)
                    {
                        sameRow = false;
                        break;
                    }
                }

                bool mergeOk = true; // we only merge adjacent cells
                int colIndex = SelectedCells[0].ColumnIndex;
                for (int i = 1; i < this.SelectedCells.Count; i++)
                {
                    if(Math.Abs(SelectedCells[i].ColumnIndex - colIndex) != 1)
                    {
                        mergeOk = false;
                        break;
                    }
                    colIndex = SelectedCells[i].ColumnIndex;
                }


                    if (sameRow)
                {
                    this.ContextMenuStrip.Items.Clear();
                    ToolStripMenuItem mergeMenuItem = new ToolStripMenuItem(MERGE_CONTEXT_MENU);
                    ToolStripMenuItem swapMenuItem = new ToolStripMenuItem(SWAP_CONTEXT_MENU);

                    if(mergeOk)
                        this.ContextMenuStrip.Items.Add(mergeMenuItem);
                    if(this.SelectedCells.Count == 2)
                        this.ContextMenuStrip.Items.Add(swapMenuItem);
                    e.Cancel = false;
                }
            }
            else
            {
                if (this.SelectedCells[0].RowIndex == 1)
                {
                    string text = (String)this.SelectedCells[0].Value;

                    if (string.IsNullOrEmpty(text))
                        return;

                    this.ContextMenuStrip.Items.Clear();

                    this.ContextMenuStrip.Items.Clear();

                    string[] strings = text.Trim().Split(' ');
                    if (strings.Length > 1)
                    {
                        ToolStripMenuItem reverseMenuItem = new ToolStripMenuItem(REVERSE_CONTEXT_MENU);
                        this.ContextMenuStrip.Items.Add(reverseMenuItem);

                        ToolStripMenuItem deleteLeftMenuItem = new ToolStripMenuItem(DELETE_LEFT_CONTEXT_MENU);
                        this.ContextMenuStrip.Items.Add(deleteLeftMenuItem);

                        ToolStripMenuItem deleteRightMenuItem = new ToolStripMenuItem(DELETE_RIGHT_CONTEXT_MENU);
                        this.ContextMenuStrip.Items.Add(deleteRightMenuItem);
                    }

                    ToolStripMenuItem deleteMenuItem = new ToolStripMenuItem(DELETE_CONTEXT_MENU);
                    this.ContextMenuStrip.Items.Add(deleteMenuItem);


                    e.Cancel = false;
                }
                else
                {
                    this.ContextMenuStrip.Items.Clear();
                    ToolStripMenuItem deleteMenuItem = new ToolStripMenuItem(SPLIT_CONTEXT_MENU);

                    this.ContextMenuStrip.Items.Add(deleteMenuItem);
                    e.Cancel = false;
                }
            }
        }

        private void ContextMenuStrip_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            if (e.ClickedItem.Text == MERGE_CONTEXT_MENU)
            {
                if (this.CurrentVerse != null)
                    undoStack.Push(new Verse(this.CurrentVerse));

                int firstMergeIndex = this.SelectedCells[0].ColumnIndex;
                for (int i = 1; i < this.SelectedCells.Count; i++)
                {
                    if (this.SelectedCells[i].ColumnIndex < firstMergeIndex)
                        firstMergeIndex = this.SelectedCells[i].ColumnIndex;
                }
                this.CurrentVerse.Merge(firstMergeIndex, this.SelectedCells.Count);

                SaveVerse(CurrentVerseReferece);
                this.Update(CurrentVerse);

                this[firstMergeIndex, 1].Selected = true;
                this.CurrentCell = this[firstMergeIndex, 1];
                if (!string.IsNullOrEmpty((string)this[firstMergeIndex, 1].Value))
                    FireRefernceHighlightRequest((string)this[firstMergeIndex, 1].Value);
                FireVerseViewChanged();
            }
            else if (e.ClickedItem.Text == SWAP_CONTEXT_MENU)
            {
                if (this.CurrentVerse != null)
                    undoStack.Push(new Verse(this.CurrentVerse));

                int firstSwapIndex = this.SelectedCells[0].ColumnIndex;
                int secondSwapIndex = this.SelectedCells[1].ColumnIndex;
                //for (int i = 1; i < this.SelectedCells.Count; i++)
                //{
                //    if (this.SelectedCells[i].ColumnIndex < firstSwapIndex)
                //        firstSwapIndex = this.SelectedCells[i].ColumnIndex;
                //}
                this.CurrentVerse.SwapTags(firstSwapIndex, secondSwapIndex);

                SaveVerse(CurrentVerseReferece);
                this.Update(CurrentVerse);

                this[firstSwapIndex, 1].Selected = true;
                this.CurrentCell = this[firstSwapIndex, 1];
                if (!string.IsNullOrEmpty((string)this[firstSwapIndex, 1].Value))
                    FireRefernceHighlightRequest((string)this[firstSwapIndex, 1].Value);
                FireVerseViewChanged();
            }
            else if (e.ClickedItem.Text == SPLIT_CONTEXT_MENU)
            {
                if (this.CurrentVerse != null)
                    undoStack.Push(new Verse(this.CurrentVerse));

                int splitIndex = this.SelectedCells[0].ColumnIndex;
                for (int i = 1; i < this.SelectedCells.Count; i++)
                {
                    if (this.SelectedCells[i].ColumnIndex < splitIndex)
                        splitIndex = this.SelectedCells[i].ColumnIndex;
                }
                this.CurrentVerse.split(splitIndex);

                SaveVerse(CurrentVerseReferece);
                this.Update(CurrentVerse);

                this[splitIndex, 1].Selected = true;
                this.CurrentCell = this[splitIndex, 1];
                if (!string.IsNullOrEmpty((string)this[splitIndex, 1].Value))
                    FireRefernceHighlightRequest((string)this[splitIndex, 1].Value);
                FireVerseViewChanged();
            }
            else if (e.ClickedItem.Text == DELETE_CONTEXT_MENU)
            {
                if (this.CurrentVerse != null)
                    undoStack.Push(new Verse(this.CurrentVerse));

                int col = this.SelectedCells[0].ColumnIndex;
                this.CurrentVerse[col].Strong = new string[] { string.Empty };
                SaveVerse(CurrentVerseReferece);
                this.Update(CurrentVerse);

                this[col, 1].Selected = true;
                this.CurrentCell = this[col, 1];
                FireVerseViewChanged();
            }
            else
            {
                string[] strings = ((string)this.SelectedCells[0].Value).Split(' ');
                if (strings.Length < 2) return;

                if (this.CurrentVerse != null)
                    undoStack.Push(new Verse(this.CurrentVerse));

                string newText = string.Empty;
                int col = 0;
                if (e.ClickedItem.Text == REVERSE_CONTEXT_MENU)
                {
                    newText = strings[strings.Length - 1];
                    for (int i = strings.Length - 2; i >= 0; i--)
                    {
                        newText += " " + strings[i];
                    }
                    this.SelectedCells[0].Value = newText;

                    col = this.SelectedCells[0].ColumnIndex;
                    this.CurrentVerse[col].StrongString = newText;
                    SaveVerse(CurrentVerseReferece);
                    FireVerseViewChanged();
                }
                else if (e.ClickedItem.Text == DELETE_LEFT_CONTEXT_MENU)
                {
                    for (int i = 1; i < strings.Length; i++)
                    {
                        newText += string.IsNullOrEmpty(newText) ? strings[i] : " " + strings[i];
                    }
                    this.SelectedCells[0].Value = newText;

                    col = this.SelectedCells[0].ColumnIndex;
                    this.CurrentVerse[col].StrongString = newText;
                    SaveVerse(CurrentVerseReferece);
                    FireVerseViewChanged();
                }
                else if (e.ClickedItem.Text == DELETE_RIGHT_CONTEXT_MENU)
                {
                    for (int i = 0; i < strings.Length - 1; i++)
                    {
                        newText += string.IsNullOrEmpty(newText) ? strings[i] : " " + strings[i];
                    }
                    this.SelectedCells[0].Value = newText;

                    col = this.SelectedCells[0].ColumnIndex;
                    this.CurrentVerse[col].StrongString = newText;
                    SaveVerse(CurrentVerseReferece);
                    FireVerseViewChanged();
                }
                this[col, 1].Selected = true;
                this.CurrentCell = this[col, 1];
                if (!string.IsNullOrEmpty((string)this[col, 1].Value))
                    FireRefernceHighlightRequest((string)this[col, 1].Value);
                FireVerseViewChanged();
            }
        }

        #endregion Context Menue

        #region Save & Update
        /// <summary>
        /// Updates the target verse display when the verse contains tags already
        /// </summary>
        /// <param name="verse">tagged verse</param>
        public void Update(Verse verse)
        {
            try
            {
                if (verse == null)
                {
                    return;
                }

                this.CurrentVerse = verse;
                bool oldTestament = false;

                string direction = Properties.Settings.Default.TargetTextDirection;
                this.Rows.Clear();

                string[] verseWords = new string[verse.Count];
                string[] verseTags = new string[verse.Count];
                for (int i = 0; i < verse.Count; i++)
                {
                    verseWords[i] = verse[i].Word;
                    for (int j = 0; j < verse[i].Strong.Length; j++)
                        verseTags[i] += "<" + verse[i].Strong[j] + "> ";
                    verseTags[i] = verseTags[i].Trim();
                }

                int col = -1;
                for (int i = 0; i < verseTags.Length; i++)
                {
                    // if (verseTags[i].Contains("3068")) //verseTags[i].Contains("???") || verseTags[i].Contains("0000"))
                    //{
                    //    col = i;
                    //    break;
                    // }
                    if (verseTags[i] == "<>")
                        verseTags[i] = string.Empty;
                }


                this.ColumnCount = verseWords.Length;
                this.Rows.Add(verseWords);
                this.Rows.Add(verseTags);

                string tagToHighlight = SearchTag;
                if (string.IsNullOrEmpty(tagToHighlight) || tagToHighlight.ToLower() == "<blank>")
                    tagToHighlight = "<>";
                for (int i = 0; i < verseWords.Length; i++)
                {
                    string word = (string)this.Rows[0].Cells[i].Value;
                    string tag = (string)this.Rows[1].Cells[i].Value;
                    if (tag == null)
                        continue;

                    if (tag.Contains("3068") && oldTestament)
                    {
                        this.Rows[1].Cells[i].Style.ForeColor = Color.Red;
                    }
                    else if (tag.Contains("0430>") && oldTestament)
                    {
                        this.Rows[1].Cells[i].Style.BackColor = Color.Green;
                        this.Rows[1].Cells[i].Style.ForeColor = Color.White;
                    }
                    else if (tag.Contains("0410>") && oldTestament)
                        this.Rows[1].Cells[i].Style.BackColor = Color.Yellow;
                    else if (tag.Contains(tagToHighlight) || tag.Contains("0000") || (tag == string.Empty && tagToHighlight == "<>"))
                        this.Rows[1].Cells[i].Style.BackColor = Color.LightGray;
                    else
                        this.Rows[1].Cells[i].Style.ForeColor = Color.Black;

                    if (direction.ToLower() == "rtl")
                        this.Columns[i].DisplayIndex = verseWords.Length - i - 1;
                }

                if (col >= 0)
                    this.CurrentCell = this.Rows[1].Cells[col];

                this.ClearSelection();

                this.Rows[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                this.Rows[0].ReadOnly = true;
                this[0,1].Selected = true;
                this.CurrentCell = this[0, 1];
                if(!string.IsNullOrEmpty((string)this[0, 1].Value))
                    FireRefernceHighlightRequest((string)this[0, 1].Value);
                //this.Rows[1].ReadOnly = true;

            }
            catch (Exception ex)
            {
                Tracing.TraceException(MethodBase.GetCurrentMethod().Name, ex.Message);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="verse"></param>
        public void SaveVerse(Verse verse)
        {
            Bible.Bible[verse[0].Reference] = verse;
            if (!Utils.AreReferencesEqual(CurrentVerseReferece, verse[0].Reference))
            {
                FireGotoVerseRequest(verse[0].Reference);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="reference"></param>
        public void SaveVerse(string reference)
        {
            Verse verse = new Verse();

            for (int i = 0; i < this.Columns.Count; i++)
            {
                string[] tags;
                string tag = ((string)this[i, 1].Value);
                if (string.IsNullOrEmpty(tag))
                    tags = new string[] { "<>" };
                else
                    tags = tag.Split(' ');

                // remove <> from tags
                for (int j = 0; j < tags.Length; j++)
                    tags[j] = tags[j].Replace("<", "").Replace(">", "");

                verse[i] = new VerseWord((string)this[i, 0].Value, tags, reference);
            }

            if (Bible.Bible.ContainsKey(reference))
            {
                Bible.Bible[reference] = verse;
            }
            else
            {
                Bible.Bible.Add(reference, verse);
            }
        }
        #endregion Save & Update


        #region overrides
        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnCellValueChanged(DataGridViewCellEventArgs e)
        {
            if (e.RowIndex == 0)
            {
                string newWord = (string)this[e.ColumnIndex, e.RowIndex].Value;
                CurrentVerse.UpdateWord(e.ColumnIndex, newWord);
//                FireVerseViewChanged();
            }
            base.OnCellValueChanged(e);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnCellEnter(DataGridViewCellEventArgs e)
        {
            if (this.Rows.Count > 1)
            {
                // during initialsation, we may come here
                // when the grid rows are not fully initialised
                //if (this.SelectedRows.Count > 1)
                //{
                //}
                if(e.RowIndex == 1)
                {
                    this.ClearSelection();
                    this[e.ColumnIndex, e.RowIndex].Selected = true;
                }
                FireRefernceHighlightRequest((string)this.Rows[1].Cells[e.ColumnIndex].Value);

            }
            //base.OnCellEnter(e);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        //protected override void OnColumnRemoved(DataGridViewColumnEventArgs e)
        //{
        //    FireVerseViewChanged();
        //    base.OnColumnRemoved(e);
        //}

        protected override void OnKeyUp(KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                if(this.SelectedCells.Count == 1)
                {
                    if (this.CurrentVerse != null)
                        undoStack.Push(new Verse(this.CurrentVerse));

                    int col = this.SelectedCells[0].ColumnIndex;
                    this[col, 1].Value = string.Empty;
                    this.CurrentVerse[col].Strong = new string[] { string.Empty };
                    SaveVerse(CurrentVerseReferece);
                    this.Update(CurrentVerse);

                }
            }

            base.OnKeyUp(e);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="drgevent"></param>
        protected override void OnDragEnter(DragEventArgs drgevent)
        {
            drgevent.Effect = DragDropEffects.Copy;
            base.OnDragEnter(drgevent);
        }


        protected override void OnDragDrop(DragEventArgs drgevent)
        {
            DragData data = drgevent.Data.GetData(typeof(DragData)) as DragData;
            string newValue = data.Text;
            Point cursorLocation = this.PointToClient(new Point(drgevent.X, drgevent.Y));

            System.Windows.Forms.DataGridView.HitTestInfo hittest = this.HitTest(cursorLocation.X, cursorLocation.Y);
            if (hittest.ColumnIndex != -1
                && hittest.RowIndex == 1)
            {  //CHANGE
                if (data.Source.Equals(this))
                {
                    if (data.ColumnIndex == hittest.ColumnIndex)
                        return;
                }
                newValue = newValue.Replace("+G", "> <");
                string[] strings = newValue.Split(' ');
                newValue = string.Empty;
                string currentText = (string)this[hittest.ColumnIndex, 1].Value;
                if (!string.IsNullOrEmpty(currentText) && !currentText.Contains("???"))
                    newValue = currentText;

                // special Handling for Aramaic
                if (strings.Length == 2 && IsCurrentTextAramaic)
                {
                    strings = new string[1] { strings[0] };
                    if(Control.ModifierKeys != Keys.Alt)
                        newValue = string.Empty;
                }

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
                        if (!newValue.Contains(tmp) || Control.ModifierKeys == Keys.Alt)
                            newValue += string.IsNullOrEmpty(newValue) ? tmp : (" " + tmp);
                    }
                }
                if (this.CurrentVerse != null)
                    undoStack.Push(new Verse(CurrentVerse));

                this[hittest.ColumnIndex, 1].Value = newValue.Trim();
                this.CurrentVerse[hittest.ColumnIndex].StrongString = newValue.Trim();

                FireRefernceHighlightRequest(newValue);

                if (data.Source.Equals(this))
                {
                    this[data.ColumnIndex, 1].Value = string.Empty;
                    this.CurrentVerse[data.ColumnIndex].StrongString = string.Empty;
                }
                SaveVerse(CurrentVerseReferece);
                FireVerseViewChanged();

                this.ClearSelection();
                this[hittest.ColumnIndex, 1].Selected = true;
                this.Rows[0].ReadOnly = true;

                this[hittest.ColumnIndex, 1].Selected = true;
                this.CurrentCell = this[hittest.ColumnIndex, 1];
                if (!string.IsNullOrEmpty((string)this[hittest.ColumnIndex, 1].Value))
                    FireRefernceHighlightRequest((string)this[hittest.ColumnIndex, 1].Value);

                this.Focus();
            }

            base.OnDragDrop(drgevent);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && e.Clicks == 1)
            {
                DataGridView.HitTestInfo info = this.HitTest(e.X, e.Y);
                if (info.RowIndex > 0)
                {
                    if (info.RowIndex >= 0 && info.ColumnIndex >= 0)
                    {
                        string text = (String)this.Rows[1].Cells[info.ColumnIndex].Value;
                        DragData data = new DragData(1, info.ColumnIndex, text, this);
                        if (text != null)
                        {
                            //Need to put braces here  CHANGE
                            this.DoDragDrop(data, DragDropEffects.Copy);
                        }
                    }
                }
            }
            base.OnMouseDown(e);
        }

        #endregion overrides

        #region Undo / Redo
        public void Undo()
        {
            if (undoStack.Count > 0)
            {
                redoStack.Push(new Verse(CurrentVerse));
                Verse verse = undoStack.Pop();
                if (Utils.AreReferencesEqual(CurrentVerseReferece, verse[0].Reference))
                {
                    Update(verse);
                }
                SaveVerse(verse);
            }
        }

        public void Redo()
        {
            if (redoStack.Count > 0)
            {
                Verse verse = redoStack.Pop();
                if (Utils.AreReferencesEqual(CurrentVerseReferece, verse[0].Reference))
                {
                    Update(verse);
                }
                SaveVerse(verse);
            }
        }

        #endregion Undo / Redo

        #region Firing events
        public void FireVerseViewChanged()
        {
            if (this.VerseViewChanged != null)
            {
                this.VerseViewChanged(this, EventArgs.Empty);
            }

        }

        public void FireRefernceHighlightRequest(string tag)
        {
            if (this.RefernceHighlightRequest != null)
            {
                this.RefernceHighlightRequest(this, tag);
            }

        }

        public void FireGotoVerseRequest(string tag)
        {
            if (this.GotoVerseRequest != null)
            {
                this.GotoVerseRequest(this, tag);
            }

        }
        #endregion Firing events

    }
    public delegate void VerseViewChangedEventHandler(object sender, EventArgs e);
    public delegate void RefernceHighlightRequestEventHandler(object sender, string tag);
    public delegate void GotoVerseRequestEventHandler(object sender, string reference);

}
