using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BibleTaggingUtil.Editor
{
    partial class EditorPanel
    {
        /// <summary>
        /// Populates the reference verse data grid view with the 
        /// verse words and tags
        /// </summary>
        /// <param name="verse">The reference verse including tags</param>
        private void PopulateReferenceVerseView(Verse verse)
        {
            dgvReferenceVerse.Rows.Clear();

            string[] verseWords = new string[verse.Count];
            string[] verseTags = new string[verse.Count];

            for (int i = 0; i < verse.Count; i++)
            {
                verseWords[i] = verse[i].Word;

                string thisTag = string.Empty;
                for (int j = 0; j < verse[i].Strong.Length; j++)
                {
                    if (verse[i].Strong[j].Contains('('))
                        thisTag += verse[i].Strong[j] + " ";
                    else
                        thisTag += "<" + verse[i].Strong[j] + "> ";
                }

                thisTag = thisTag.Trim().
                        Replace(",", "").
                        Replace(".", "").
                        Replace(";", "").
                        Replace(":", "");
                if (thisTag == "<>")
                    thisTag = string.Empty;
                verseTags[i] = thisTag;
            }


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



    }
}
