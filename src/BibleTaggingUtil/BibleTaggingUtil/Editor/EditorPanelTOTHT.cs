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
        private void PopulateTOHTHView(Verse verseWords)
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
                        s = "<" + verseWord.Strong[0] + ">";
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
                                    s += "<" + verseWord.Strong[j] + ">";
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

            dgvTOTHTView.Rows.Clear();
            dgvTOTHTView.ColumnCount = verseWords.Count;

            dgvTOTHTView.Rows.Add(words.ToArray());
            dgvTOTHTView.Rows.Add(hebrew.ToArray());
            dgvTOTHTView.Rows.Add(transliteration.ToArray());
            dgvTOTHTView.Rows.Add(tags.ToArray());

            for (int i = 0; i < words.Count; i++)
            {
                string word = (string)dgvTOTHTView.Rows[1].Cells[i].Value;
                if (word.Contains("יהוה"))
                    dgvTOTHTView.Rows[1].Cells[i].Style.ForeColor = Color.Red;
                else
                    dgvTOTHTView.Rows[1].Cells[i].Style.ForeColor = Color.Black;
            }

            dgvReferenceVerse.ClearSelection();

            dgvReferenceVerse.Rows[0].ReadOnly = true;
            dgvReferenceVerse.Rows[1].ReadOnly = true;

        }

    }
}
