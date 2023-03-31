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

using BibleTaggingUtil.BibleVersions;
using System.Security.Cryptography.Xml;
using System.Reflection;

namespace BibleTaggingUtil.Editor
{
    public partial class EditorPanel : DockContent
    {



        private BibleTaggingForm container;
        private BrowserPanel browser;
        private VerseSelectionPanel verse;

        private string strongsPrefix = string.Empty;
        private TestamentEnum testament = TestamentEnum.NEW;

        private System.Timers.Timer tempTimer = null;


        public bool TargetDirty { get; set; }

        public TargetVersion TargetVersion
        {
            get { return dgvTarget.Bible; }
            set { dgvTarget.Bible = value; }
        }

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
            dgvKJV.CellContentDoubleClick += Dgv_CellContentDoubleClick;
            dgvTarget.CellContentDoubleClick += Dgv_CellContentDoubleClick;
            dgvTOTHT.CellContentDoubleClick += Dgv_CellContentDoubleClick;
            //dgvTOTHT.CellContentDoubleClick += DgvTOTHTView_CellContentDoubleClick;

            dgvTarget.VerseViewChanged += DgvTarget_VerseViewChanged;
            dgvTarget.RefernceHighlightRequest += DgvTarget_RefernceHighlightRequest;
            dgvTarget.GotoVerseRequest += DgvTarget_GotoVerseRequest;

            dgvTarget.KeyDown += DgvTarget_KeyDown;
            dgvTarget.KeyUp += DgvTarget_KeyUp;
            dgvKJV.KeyUp += DgvTarget_KeyUp;
            dgvTOTHT.KeyUp += DgvTarget_KeyUp;

            dgvTarget.SearchTag = cbTagToFind.Text;

            verse.VerseChanged += Verse_VerseChanged;

        }

        private void DgvTarget_GotoVerseRequest(object sender, string reference)
        {
            container.VerseSelectionPanel.GotoVerse(reference);
        }

        private void DgvTarget_VerseViewChanged(object sender, EventArgs e)
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
            dgvTarget.Rows.Clear();
            dgvTarget.ColumnCount = 0;
            dgvKJV.Rows.Clear();
            dgvKJV.ColumnCount = 0;
            dgvTOTHT.Rows.Clear();
            dgvTOTHT.ColumnCount = 0;
        }

        public void SaveCurrentVerse()
        {
            if (!string.IsNullOrEmpty(tbCurrentReference.Text) && dgvTarget.Columns.Count > 0)
            {
                // save current target verse
                dgvTarget.SaveVerse(tbCurrentReference.Text);
            }
        }
        private void Verse_VerseChanged(object sender, VerseChangedEventArgs e)
        {
            strongsPrefix = e.StrongsPrefix;
            testament = e.Testament;

            string oldReference = tbCurrentReference.Text;
            tbCurrentReference.Text = e.VerseReference;
            dgvTarget.CurrentVerseReferece = e.VerseReference;
            string bookName = e.VerseReference.Substring(0, 3);

            string actualBookName = string.Empty;
            try
            {
                // KJV view
                actualBookName = container.KJV[bookName];
                if (!string.IsNullOrEmpty(actualBookName))
                {
                    string reference = e.VerseReference.Replace(bookName, actualBookName);
                    if (container.KJV.Bible.ContainsKey(reference))
                        dgvKJV.Update(container.KJV.Bible[reference]);
                    else
                        dgvKJV.Update(null);
                }
            }
            catch (Exception ex)
            {
                Tracing.TraceException(MethodBase.GetCurrentMethod().Name + "-KJV", ex.Message);
            }

            Verse verseWords = null;
            try
            {
                // TOTHT view
                if (testament == TestamentEnum.OLD) //  && container.TOTHT.Bible.ContainsKey(e.VerseReferenceAlt))
                {
                    actualBookName = container.TOTHT[bookName];
                    if (!string.IsNullOrEmpty(actualBookName))
                    {
                        string reference = e.VerseReference.Replace(bookName, actualBookName);
                        if (container.TOTHT.Bible.ContainsKey(reference))
                        {
                            verseWords = container.TOTHT.Bible[reference];
                            dgvTOTHT.Update(verseWords, BibleTestament.OT);
                        }
                        else
                            dgvTOTHT.Update(null, BibleTestament.OT);
                    }
                }
                else if (testament == TestamentEnum.NEW)// && container.TAGNT.Bible.ContainsKey(e.VerseReferenceUBS))
                {
                    actualBookName = container.TAGNT[bookName];
                    if (!string.IsNullOrEmpty(actualBookName))
                    {
                        string reference = e.VerseReference.Replace(bookName, actualBookName);
                        if (container.TAGNT.Bible.ContainsKey(reference))
                        {
                            verseWords = container.TAGNT.Bible[reference];
                            dgvTOTHT.Update(verseWords, BibleTestament.NT);
                        }
                        else
                            dgvTOTHT.Update(null, BibleTestament.NT);
                    }
                }
                else
                {
                    if (tbCurrentReference.Text.Contains("Not"))
                        tbCurrentReference.Text += ", " + tbTH.Text;
                    else
                        tbCurrentReference.Text += " Not in " + tbTarget.Text;
                }
            }
            catch (Exception ex)
            {
                Tracing.TraceException(MethodBase.GetCurrentMethod().Name + "-TOTHT", ex.Message);
            }

            try
            { 
            // Target view
            if (!string.IsNullOrEmpty(oldReference) && dgvTarget.Columns.Count > 0)
            {
                dgvTarget.SaveVerse(oldReference);
            }

            actualBookName = container.Target[bookName];
            if (!string.IsNullOrEmpty(actualBookName))
            {
                string reference = e.VerseReference.Replace(bookName, actualBookName);
                //targetUpdatedVerse = Utils.GetVerseText(container.Target.Bible[targetRef], true);
                if (tbTarget.Text.ToLower().Contains("arabic"))
                    DoSepecialHandling(reference);
                Verse v = container.Target.Bible[reference];
                
                if (dgvTarget.IsCurrentTextAramaic)
                {
                    for(int i = 0; i < v.Count; i++)
                    {
                        VerseWord vw = v[i];
                        if(vw.Strong.Length == 1 && !string.IsNullOrEmpty(vw.Strong[0]) && verseWords != null)
                        {
                            string st = vw.Strong[0].Trim();
                            for(int j = 0; j < verseWords.Count; j++)
                            {
                                if (verseWords[j].Strong.Contains(st))
                                {
                                    if (verseWords[j].Strong.Length == 2)
                                    {
                                        vw.Strong[0] = verseWords[j].Strong[1];
                                    }
                                    break;
                                }
                            }
                        }
                    }

                }

                dgvTarget.Update(v);
            }
            else
            {
                //targetVerse = e.VerseReferenceAlt + " NotFound";
                if (tbCurrentReference.Text.Contains("Not"))
                    tbCurrentReference.Text += ", " + tbTarget.Text;
                else
                    tbCurrentReference.Text += " Not in " + tbTarget.Text;
            }
            }
            catch (Exception ex)
            {
                Tracing.TraceException(MethodBase.GetCurrentMethod().Name + "-Target", ex.Message);
            }

        }

        private void DoSepecialHandling(string verseReference)
        {
            bool changed = false;
            Verse v = container.Target.Bible[verseReference];
            List<int> merges = new List<int>();
            while (true)
            {
                for (int i = 0; i < v.Count; i++)
                {
                    VerseWord vw = v[i];
                    if (vw.Word == "*" && vw.Strong.Length == 1 && vw.Strong[0].Contains("???"))
                    {
                        vw.Strong[0] = "";
                        changed = true;
                        break;
                    }
                    else if (vw.Strong.Length == 1 && vw.Strong[0].Contains("0000"))
                    {
                        vw.Strong[0] = "";
                        changed = true;
                        break;
                    }
                    else if (i == 0 && vw.Word == "لإِمَامِ" && v[1].Word == "الْمُغَنِّينَ")
                    {
                        v.Merge(i, 2);
                        changed = true;
                        break;
                    }
                    else if (i < v.Count - 1 && vw.Word == "يَا")
                    {
                        v.Merge(i, 2);
                        changed = true;
                        break;
                    }
                    else if (i < v.Count - 1 && vw.Strong.Length == 1 && (string.IsNullOrEmpty(vw.Strong[0])  || vw.Strong[0].Contains("???")) &&
                        (vw.Word == "مِنَ" || vw.Word == "مِنْ" ||  vw.Word == "إِلَى" || vw.Word == "أَمَّا"))
                    {
                        v.Merge(i, 2);
                        changed = true;
                        break;
                    }
                    //else if (i < v.Count - 1 && vw.Strong.Length == 1 && v[i+1].Strong.Length == 1 &&
                    //                            vw.Strong[0] == v[i + 1].Strong[0] && 
                    //                            vw.Word.Replace(".","") != v[i + 1].Word.Replace(".", ""))
                    //{
                    //    v.Merge(i, 2);
                    //    changed = true;
                    //    break;
                    //}

                }
                if (changed)
                {
                    container.Target.Bible[verseReference] = v;
                    TargetDirty = true;
                    changed = false;
                }
                else
                    break;
            }
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
            string tag = (String)(dgv.Rows[dgv.RowCount-1].Cells[e.ColumnIndex].Value);
            if (!string.IsNullOrEmpty(tag))
            {
                tag = tag.Replace("<", "").Replace(">", "").Replace(",", "").Replace(".", "").Replace(":", "");

                string[] tags = tag.Split(' ');
                if (tags.Length == 1)
                {
                    browser.NavigateToTag((testament == TestamentEnum.OLD ? "H" : "G") + tags[0]);
                }
                else
                {

                    DataGridViewCell cell = dgv[e.ColumnIndex, e.RowIndex];
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
                    for (int i = 0; i < tags.Length; i++)
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
            if (!string.IsNullOrEmpty(tag))
            {
                tag = tag.Replace("<", "").Replace(">", "").Replace(",", "").Replace(".", "").Replace(":", "");

                string[] tags = tag.Split(' ');
                if (tags.Length == 1)
                {
                    browser.NavigateToTag((testament == TestamentEnum.OLD? "H": "G") + tags[0]);
                }
                else
                {

                    DataGridViewCell cell = dgv[e.ColumnIndex, e.RowIndex];
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
                    for (int i = 0; i < tags.Length; i++)
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
            browser.NavigateToTag((testament == TestamentEnum.OLD ? "H" : "G") + tag);
        }

        #endregion Reference verse events

        #region Higlight same tag
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="tag"></param>
        private void DgvTarget_RefernceHighlightRequest(object sender, string tag)
        {
            new Thread(() => { SelectReferenceTags(tag.Trim()); }).Start();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tag"></param>
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
                    dgvKJV.ClearSelection();
                    dgvTOTHT.ClearSelection();

                    if (string.IsNullOrEmpty(tag))
                    {
                        SetHighlightedCell(dgvTOTHT, null, null);
                        SetHighlightedCell(dgvKJV, null, null);
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

                    SetHighlightedCell(dgvTOTHT, tags1, tags2);
                    SetHighlightedCell(dgvKJV, tags1, tags2);

                }
                catch (Exception ex)
                {
                    Tracing.TraceException(MethodBase.GetCurrentMethod().Name, ex.Message);
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


        private void DgvTarget_KeyDown(object sender, KeyEventArgs e)
        {
            Keys forward = Keys.Right;
            Keys back = Keys.Left;
            if(tbTarget.Text.ToLower().Contains("arabic"))
            {
            forward = Keys.Left;
            back = Keys.Right;
            }
            if (e.KeyCode == forward)
            {
                if (dgvTarget.IsLastWord)
                {
                    verse.MoveToNext();
                    e.Handled = true;
                }
            }
            else if (e.KeyCode == back)
            {
                if (dgvTarget.IsFirstWord)
                {
                    verse.MoveToPrevious();
                    e.Handled = true;
                }
            }
        }

        private void DgvTarget_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.PageUp)
            {
                verse.MoveToPrevious();
            }
            else if (e.KeyCode == Keys.PageDown || e.KeyCode == Keys.Next)
            {
                verse.MoveToNext();
            }
            else if (e.Control) 
            {
                if (e.KeyCode == Keys.S) container.Target.SaveUpdates();
                if (e.KeyCode == Keys.Y) dgvTarget.Redo();
                if (e.KeyCode == Keys.Z) dgvTarget.Undo();
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
            dgvTarget.Redo();
        }

        private void picUndo_Click(object sender, EventArgs e)
        {
            dgvTarget.Undo();
        }

        private void picSave_Click(object sender, EventArgs e)
        {
            container.Target.SaveUpdates();
        }

        private void picDecreaseFont_Click(object sender, EventArgs e)
        {
            Font font = dgvKJV.DefaultCellStyle.Font;
            dgvKJV.DefaultCellStyle.Font = new Font(font.Name, font.Size - 1);
            font = dgvKJV.DefaultCellStyle.Font;
            dgvTarget.DefaultCellStyle.Font = new Font(font.Name, font.Size - 1);
            font = dgvTOTHT.DefaultCellStyle.Font;
            dgvTOTHT.DefaultCellStyle.Font = new Font(font.Name, font.Size - 1);

        }
        private void picIncreaseFont_Click(object sender, EventArgs e)
        {
            Font font = dgvKJV.DefaultCellStyle.Font;
            dgvKJV.DefaultCellStyle.Font = new Font(font.Name, font.Size + 1);
            font = dgvKJV.DefaultCellStyle.Font;
            dgvTarget.DefaultCellStyle.Font = new Font(font.Name, font.Size + 1);
            font = dgvTOTHT.DefaultCellStyle.Font;
            dgvTOTHT.DefaultCellStyle.Font = new Font(font.Name, font.Size + 1);
        }

        private void picEnableEdit_Click(object sender, EventArgs e)
        {
            dgvTarget.Rows[0].ReadOnly = false;
        }

        private void picFindTagForward_Click(object sender, EventArgs e)
        {
            dgvTarget.SearchTag = cbTagToFind.Text;
            container.FindVerse(cbTagToFind.Text);
        }


        #endregion Buttons

        private void tbCurrentReference_TextChanged(object sender, EventArgs e)
        {
            dgvTarget.CurrentVerseReferece = tbCurrentReference.Text;
            int b = tbCurrentReference.Text.IndexOf(' ');
            if(b > 0)
            {
                string book = tbCurrentReference.Text.Substring(0, b);
                int bookIndex = Array.IndexOf(Constants.osisNames, book);
                if(bookIndex < 0) 
                {
                    bookIndex = Array.IndexOf(Constants.osisAltNames, book);
                    if (bookIndex < 0)
                        bookIndex = Array.IndexOf(Constants.ubsNames, book);
                }
                if(bookIndex >= 0)
                {
                    if (bookIndex > 38)
                        tbTH.Text = "TAGNT";
                    else
                        tbTH.Text = "TOTHT";
                }
            }
        }

        public void TargetBibleName(string name)
        {
            if (InvokeRequired)
            {
                Action safeWrite = delegate { TargetBibleName(name); };
                Invoke(safeWrite);
            }
            else
            {
                tbTarget.Text = name;
            }
        }
        private void cbTagToFind_SelectedIndexChanged(object sender, EventArgs e)
        {
            dgvTarget.SearchTag = cbTagToFind.Text;
        }
    }
}
