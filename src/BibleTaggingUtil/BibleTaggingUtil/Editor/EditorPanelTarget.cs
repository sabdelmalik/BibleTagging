using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TextBox;

namespace BibleTaggingUtil.Editor
{
    /// <summary>
    /// This part of the class deals with Hebrew Target Verse
    /// </summary>
    partial class EditorPanel
    {

        private FixedSizeStack<Verse> undoStack = new FixedSizeStack<Verse>();
        private FixedSizeStack<Verse> redoStack = new FixedSizeStack<Verse>();

        private Verse currentVerse = null;

        /// <summary>
        /// Updates the target verse display when the verse contains tags already
        /// </summary>
        /// <param name="verse">tagged verse</param>
        private void UpdateTargetView(Verse verse)
        {
            try
            {
                currentVerse = verse;
                bool oldTestament = false;

                string direction = Properties.Settings.Default.TargetTextDirection;
                dgvTargetVerse.Rows.Clear();

                string[] verseWords = new string[verse.Count];
                string[] verseTags = new string[verse.Count];
                for(int i = 0; i < verse.Count; i++)
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


                dgvTargetVerse.ColumnCount = verseWords.Length;
                dgvTargetVerse.Rows.Add(verseWords);
                dgvTargetVerse.Rows.Add(verseTags);

                string highlightedTag = cbTagToFind.Text;
                if (string.IsNullOrEmpty(highlightedTag) || highlightedTag.ToLower() == "<blank>")
                    highlightedTag = "<>";
                for (int i = 0; i < verseWords.Length; i++)
                {
                    string word = (string)dgvTargetVerse.Rows[0].Cells[i].Value;
                    string tag = (string)dgvTargetVerse.Rows[1].Cells[i].Value;
                    if (tag == null)
                        continue;

                    if (tag.Contains("3068") && oldTestament)
                    {
                        dgvTargetVerse.Rows[1].Cells[i].Style.ForeColor = Color.Red;
                    }
                    else if (tag.Contains("0430>") && oldTestament)
                    {
                        dgvTargetVerse.Rows[1].Cells[i].Style.BackColor = Color.Green;
                        dgvTargetVerse.Rows[1].Cells[i].Style.ForeColor = Color.White;
                    }
                    else if (tag.Contains("0410>") && oldTestament)
                        dgvTargetVerse.Rows[1].Cells[i].Style.BackColor = Color.Yellow;
                    else if (tag.Contains(highlightedTag) || tag.Contains("0000") || (tag == string.Empty && highlightedTag == "<>"))
                        dgvTargetVerse.Rows[1].Cells[i].Style.BackColor = Color.LightGray;
                    else
                        dgvTargetVerse.Rows[1].Cells[i].Style.ForeColor = Color.Black;

                    if (direction.ToLower() == "rtl")
                        dgvTargetVerse.Columns[i].DisplayIndex = verseWords.Length - i - 1;
                }

                if (col >= 0)
                    dgvTargetVerse.CurrentCell = dgvTargetVerse.Rows[1].Cells[col];

                dgvTargetVerse.ClearSelection();

                dgvTargetVerse.Rows[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                dgvTargetVerse.Rows[0].ReadOnly = true;
                //dgvTargetVerse.Rows[1].ReadOnly = true;

            }
            catch (Exception ex)
            {
                MessageBox.Show("Updating the verse Failed.\r\nSave your work and try again.\r\n" + ex.Message);
            }
        }


        public void SaveVerse(Verse verse)
        {
            container.Target.Bible[verse[0].Reference] = verse;
            if (!Utils.AreReferencesEqual(tbCurrentReference.Text, verse[0].Reference))
            {
                container.VerseSelectionPanel.GotoVerse(verse[0].Reference);
            }
        }

        public void SaveVerse(string reference)
        {
            Verse verse = new Verse();

            for (int i = 0; i < dgvTargetVerse.Columns.Count; i++)
            {
                string[] tags;
                string tag = ((string)dgvTargetVerse[i, 1].Value).Trim();
                if (string.IsNullOrEmpty(tag))
                    tags = new string[] { "<>" };
                else
                    tags = tag.Split(' ');

                // remove <> from tags
                for (int j = 0; j < tags.Length; j++)
                    tags[j] = tags[j].Replace("<", "").Replace(">", "");

                verse[i] = new VerseWord((string)dgvTargetVerse[i, 0].Value, tags, reference);
            }

            if (container.Target.Bible.ContainsKey(reference))
            {
                container.Target.Bible[reference] = verse;
            }
            else
            {
                string newRef = reference.Replace("Mar", "Mrk").Replace("Joh", "Jhn");
                if (container.Target.Bible.ContainsKey(newRef))
                {
                    container.Target.Bible[newRef] = verse;
                }
                else
                {
                    container.Target.Bible.Add(reference, verse);
                }
            }
        }


    }
}
