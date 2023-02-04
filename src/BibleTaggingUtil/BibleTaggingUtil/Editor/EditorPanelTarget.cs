using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BibleTaggingUtil.Editor
{
    /// <summary>
    /// This part of the class deals with Hebrew Target Verse
    /// </summary>
    partial class EditorPanel
    {
        /// <summary>
        /// Updates the target verse display when the verse contains tags already
        /// </summary>
        /// <param name="verse">tagged verse</param>
        private void PopulateTargetUpdatedVerseView(string verse)
        {
            try
            {
                string direction = Properties.Settings.Default.TargetTextDirection;
                dgvTargetVerse.Rows.Clear();
                string[] verseParts = verse.Trim().Split(' ');
                // count verse words
                List<string> words = new List<string>();
                List<string> tags = new List<string>();
                string tempWord = string.Empty;
                string tmpTag = string.Empty;
                int test = 0;
                for (int i = 0; i < verseParts.Length; i++)
                {
                    if (!string.IsNullOrEmpty(verseParts[i]) && verseParts[i][0] != '<' && verseParts[i][0] != '(')
                    {
                        if (!string.IsNullOrEmpty(tmpTag))
                        {
                            if (tmpTag == "<>")
                                tmpTag = string.Empty;
                            tags.Add(tmpTag);
                        }
                        tmpTag = string.Empty;
                        tempWord += (string.IsNullOrEmpty(tempWord)) ? verseParts[i].Replace(":«", ": «") : (" " + verseParts[i].Replace(":«", ": «"));
                        if (i == verseParts.Length - 1)
                        {
                            // last word
                            if (direction.ToLower() == "rtl")
                            {
                                if (tempWord.Contains('÷'))
                                    tempWord = tempWord.Replace("÷", "");
                                if (tempWord.Contains('E'))
                                    tempWord = tempWord.Replace("E", "");
                            }
                            words.Add(tempWord);
                        }
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(tempWord))
                        {
                            if (direction.ToLower() == "rtl")
                            {
                                if (tempWord.Contains('÷'))
                                    tempWord = tempWord.Replace("÷", "");
                                if (tempWord.Contains('E'))
                                    tempWord = tempWord.Replace("E", "");
                            }

                            words.Add(tempWord);
                        }
                        tempWord = string.Empty;
                        if (verseParts[i] == "<>")
                        {
                            tmpTag = "<>";
                        }
                        else
                        {
                            tmpTag += (string.IsNullOrEmpty(tmpTag)) ? verseParts[i] : (" " + verseParts[i]);
                            tmpTag = tmpTag.Replace(".", "");
                            if (i == verseParts.Length - 1)
                            {
                                // last word
                                if (tmpTag.EndsWith('.'))
                                    tmpTag.Remove(tmpTag.Length - 1, 1);
                                tags.Add(tmpTag);
                            }
                        }
                    }
                }

                string[] verseWords = words.ToArray();
                string[] verseTags = tags.ToArray();
                int col = -1;
                for (int i = 0; i < verseTags.Length; i++)
                {
                    if (verseTags[i].Contains("3068")) //verseTags[i].Contains("???") || verseTags[i].Contains("0000"))
                    {
                        col = i;
                        break;
                    }
                }


                dgvTargetVerse.ColumnCount = verseWords.Length;
                dgvTargetVerse.Rows.Add(verseWords);
                dgvTargetVerse.Rows.Add(verseTags);

                for (int i = 0; i < verseWords.Length; i++)
                {
                    string word = (string)dgvTargetVerse.Rows[0].Cells[i].Value;
                    string tag = (string)dgvTargetVerse.Rows[1].Cells[i].Value;
                    if (tag == null)
                        continue;
                    if (tag.Contains("3068"))
                    {
                        dgvTargetVerse.Rows[1].Cells[i].Style.ForeColor = Color.Red;
                    }
                    else if (tag.Contains("0430>"))
                    {
                        dgvTargetVerse.Rows[1].Cells[i].Style.BackColor = Color.Green;
                        dgvTargetVerse.Rows[1].Cells[i].Style.ForeColor = Color.White;
                    }
                    else if (tag.Contains("0410>"))
                        dgvTargetVerse.Rows[1].Cells[i].Style.BackColor = Color.Yellow;
                    else if (tag.Contains("???") || tag.Contains("0000"))
                        dgvTargetVerse.Rows[1].Cells[i].Style.BackColor = Color.Gray;
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

        /// <summary>
        /// Updates the target verse display for a verse without any tagging
        /// </summary>
        /// <param name="verse">verse with no taggs</param>
        private void PopulateTargetVerseView(string verse)
        {

            dgvTargetVerse.Rows.Clear();

            string[] verseParts = verse.Split(' ');
            dgvTargetVerse.ColumnCount = verseParts.Length;


            dgvTargetVerse.Rows.Add(verseParts);
            dgvTargetVerse.Rows.Add();

            string direction = Properties.Settings.Default.TargetTextDirection;
            if (direction.ToLower() == "rtl")
            {
                for (int i = 0; i < verseParts.Length; i++)
                {
                    dgvTargetVerse.Columns[i].DisplayIndex = verseParts.Length - i - 1;
                }
            }

            dgvTargetVerse.ClearSelection();

            dgvTargetVerse.Rows[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            dgvTargetVerse.Rows[0].ReadOnly = true;


        }

    }
}
