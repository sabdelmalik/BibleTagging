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
        private void PopulateTAGNTView(Verse verseWords)
        {
            List<string> words = new List<string>();
            List<string> greek = new List<string>();
            List<string> transliteration = new List<string>();
            List<string> tags = new List<string>();

            for (int i = 0; i < verseWords.Count; i++)
            {
                VerseWord verseWord = verseWords[i];
                words.Add(verseWord.Word);
                greek.Add(verseWord.Greek);
                transliteration.Add(verseWord.Transliteration);
                tags.Add("<" + verseWord.Strong[0] + ">");
            }

            dgvTOTHTView.Rows.Clear();
            dgvTOTHTView.ColumnCount = verseWords.Count;

            dgvTOTHTView.Rows.Add(words.ToArray());
            dgvTOTHTView.Rows.Add(greek.ToArray());
            dgvTOTHTView.Rows.Add(transliteration.ToArray());
            dgvTOTHTView.Rows.Add(tags.ToArray());

            dgvReferenceVerse.ClearSelection();

            dgvReferenceVerse.Rows[0].ReadOnly = true;
            dgvReferenceVerse.Rows[1].ReadOnly = true;

        }

    }
}
