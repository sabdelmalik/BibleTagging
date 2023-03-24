using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BibleTaggingUtil.Editor
{
    internal class TOHTHGridView : DataGridView
    {

        protected override void OnCellMouseDown(DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && e.Clicks == 1)
            {
                if (e.RowIndex >= 0)
                {
                    string text = ((String)this.Rows[3].Cells[e.ColumnIndex].Value).Trim();
                    DragData data = new DragData(1, e.ColumnIndex, text, this);
                    if (text != null)
                    {
                        this.DoDragDrop(data, DragDropEffects.Copy);
                    }
                }
            }

            base.OnCellMouseDown(e);
        }

        public void Update(Verse verseWords, BibleTestament testament)
        {
            if (testament == BibleTestament.OT)
                UpdateOT(verseWords);
            else
                UpdateNT(verseWords);
        }
        private void UpdateOT(Verse verseWords)
        {
            this.Rows.Clear();
            if (verseWords == null)
                return;

            List<string> words = new List<string>();
            List<string> hebrew = new List<string>();
            List<string> transliteration = new List<string>();
            List<string> tags = new List<string>();

            try
            {
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

                this.ColumnCount = verseWords.Count;

                this.Rows.Add(words.ToArray());
                this.Rows.Add(hebrew.ToArray());
                this.Rows.Add(transliteration.ToArray());
                this.Rows.Add(tags.ToArray());

                for (int i = 0; i < words.Count; i++)
                {
                    string word = (string)this.Rows[1].Cells[i].Value;
                    if (word.Contains("יהוה"))
                        this.Rows[1].Cells[i].Style.ForeColor = Color.Red;
                    else
                        this.Rows[1].Cells[i].Style.ForeColor = Color.Black;
                }
            }
            catch (Exception ex)
            {
                Tracing.TraceException(MethodBase.GetCurrentMethod().Name, ex.Message);
            }

            this.ClearSelection();

            this.Rows[0].ReadOnly = true;
            this.Rows[1].ReadOnly = true;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="verseWords"></param>
        private void UpdateNT(Verse verseWords)
        {
            this.Rows.Clear();
            if (verseWords == null)
                return;

            List<string> words = new List<string>();
            List<string> greek = new List<string>();
            List<string> transliteration = new List<string>();
            List<string> tags = new List<string>();

            try
            { 
            for (int i = 0; i < verseWords.Count; i++)
            {
                VerseWord verseWord = verseWords[i];
                words.Add(verseWord.Word);
                greek.Add(verseWord.Greek);
                transliteration.Add(verseWord.Transliteration);
                tags.Add("<" + verseWord.Strong[0] + ">");
            }

            this.ColumnCount = verseWords.Count;

            this.Rows.Add(words.ToArray());
            this.Rows.Add(greek.ToArray());
            this.Rows.Add(transliteration.ToArray());
            this.Rows.Add(tags.ToArray());

            this.ClearSelection();

            this.Rows[0].ReadOnly = true;
            this.Rows[1].ReadOnly = true;
            }
            catch (Exception ex)
            {
                Tracing.TraceException(MethodBase.GetCurrentMethod().Name, ex.Message);
            }
        }
    }
}
