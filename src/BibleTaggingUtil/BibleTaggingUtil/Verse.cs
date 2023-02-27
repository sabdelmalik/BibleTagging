using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BibleTaggingUtil
{
    /// <summary>
    /// Represents a verse as a dictionary of verse words
    /// </summary>
    public class Verse
    {
        private Dictionary<int, VerseWord> verse = new Dictionary<int, VerseWord>();

        public Verse() { }  
       
        /// <summary>
        /// Creates a deep copy of itself
        /// </summary>
        /// <param name="verseToClone"></param>
        public Verse(Verse verseToClone)
        {
            for(int i = 0; i < verseToClone.Count; i++)
            {
                verse[i] = (VerseWord)verseToClone[i].Clone();
            }
        }

        /// <summary>
        /// returns the number of words in the verse
        /// </summary>
        public int Count
        {
            get
            {
                return verse.Count;
            }
        }

        /// <summary>
        /// puts or removes the word at a specific index
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public VerseWord this[int index]
        {
            get 
            { 
                return verse[index]; 
            }
            set 
            { 
                verse[index] = value; 
            }
        }

        /// <summary>
        /// Merges two verse words together
        /// </summary>
        /// <param name="start">index of the starting word</param>
        /// <param name="count">number of words to merge</param>
        public void Merge(int start, int count)
        {
            Dictionary<int, VerseWord> temp = new Dictionary<int, VerseWord>();
            int newCount = verse.Count - count + 1;

            int columnIndex = 0;
            for (int i = 0; i < newCount; i++)
            {
                VerseWord newWord = verse[columnIndex];
                if (columnIndex == start)
                {
                    for (int j = 1; j < count; j++)
                    {
                        newWord += verse[++columnIndex];
                    }
                }

                temp[i] = newWord;
                columnIndex++;
            }

            verse = temp;
        }

        /// <summary>
        /// Splits the word at index into two
        /// </summary>
        /// <param name="index"></param>
        public void split(int index)
        {
            string[] splitWords = verse[index].Word.Split(' ');
            if (splitWords.Length == 1)
            {
                // nothing to do
                return;
            }

            Dictionary<int, VerseWord> temp = new Dictionary<int, VerseWord>();
            int newCount = verse.Count + splitWords.Length - 1;

            string[] tagsToSplit = verse[index].Strong;
            List<string>[] strongs = new List<string>[splitWords.Length];
            strongs[1] = new List<string>();
            int k;
            for (k = 0; k < strongs.Length; k++)
            {
                strongs[k] = new List<string>();
                if (k < tagsToSplit.Length)
                    strongs[k].Add(tagsToSplit[k]);
                else
                    strongs[k].Add("");
            }
            for (int j = k; j < tagsToSplit.Length; j++)
            {
                strongs[k -1].Add(tagsToSplit[j]);
            }



            int columnIndex = 0;

            // copy from to start to the index
            for (int i = 0; i < index; i++)
            {
                temp[i] = verse[i];
                columnIndex++;
            }
            // create the new split words 
            for (int i = index; i < (index + splitWords.Length); i++)
            {
                temp[i] = new VerseWord(splitWords[i - index], strongs[i - index].ToArray(), verse[0].Reference); 
            }
            // copy the remaining words
            for (int i = index + splitWords.Length; i < newCount; i++)
            {
                temp[i] = verse[++columnIndex];
            }
            verse = temp;
        }



    }
}
