using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BibleTagging
{
    internal enum Testament
    {
        OT,
        NT
    }

    internal enum VersificationEnum
    {
        KJV,
        NRSV
    }
    public class BibleVersification
    {
        BibleTaggingPreperationForm parent;

        /// <summary>
        /// key: ubs book name
        /// value: dictionary - Key: chapter number
        ///                     Value: verse count (last verse number)
        /// </summary>
        Dictionary<string, Dictionary<int, int>> kjvOT = new Dictionary<string, Dictionary<int, int>>();
        Dictionary<string, Dictionary<int, int>> kjvNT = new Dictionary<string, Dictionary<int, int>>();
        Dictionary<string, Dictionary<int, int>> nrsvOT = new Dictionary<string, Dictionary<int, int>>();
        Dictionary<string, Dictionary<int, int>> nrsvNT = new Dictionary<string, Dictionary<int, int>>();

        public BibleVersification(BibleTaggingPreperationForm parent)
        {
            this.parent = parent;
 
            for (int i = 0; i < Constants.ubsNames.Length; i++)
            {
                PopulateDictionary(i, VersificationEnum.KJV);
                PopulateDictionary(i, VersificationEnum.NRSV);
            }

        }

        private void PopulateDictionary(int index, VersificationEnum v)
        {
            Dictionary<string, Dictionary<int, int>> bible = null; 
            int[][] lastVerse = null;

            if (v == VersificationEnum.KJV)
            {
                bible = (index < 39) ? kjvOT : kjvNT;
                lastVerse = (index < 39) ? Versification.KJV.LAST_VERSE_OT : Versification.KJV.LAST_VERSE_NT;
            }
            else if (v == VersificationEnum.NRSV)
            {
                bible = (index < 39) ? nrsvOT : nrsvNT;
                lastVerse = (index < 39) ? Versification.NRSV.LAST_VERSE_OT : Versification.NRSV.LAST_VERSE_NT;
            }

            try
            {
                int[] chapters = lastVerse[index < 39? index: index -39];
                Dictionary<int, int> chaptersDict = new Dictionary<int, int>();
                for (int i = 0; i < chapters.Length; i++)
                {
                    chaptersDict[i + 1] = chapters[i];
                }
                bible[Constants.ubsNames[index]] = chaptersDict;
            }
            catch(Exception e)
            {
                parent.Trace(e.Message, Color.Red);
            }
        }
    }
}
