using BibleTagging.Versification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BibleTagging
{
    public enum Testament
    {
        OT,
        NT
    }

    public enum BibleVersion
    {
        KJV,
        NRSV
    }
    public class BibleVersification
    {
        BibleTaggingPreperationForm container;

        /// <summary>
        /// key: ubs book name
        /// value: dictionary - Key: chapter number
        ///                     Value: verse count (last verse number)
        /// </summary>
        Dictionary<string, Dictionary<int, int>> kjvOT = new Dictionary<string, Dictionary<int, int>>();
        Dictionary<string, Dictionary<int, int>> kjvNT = new Dictionary<string, Dictionary<int, int>>();
        Dictionary<string, Dictionary<int, int>> nrsvOT = new Dictionary<string, Dictionary<int, int>>();
        Dictionary<string, Dictionary<int, int>> nrsvNT = new Dictionary<string, Dictionary<int, int>>();

        private int otCountKJV = 0;
        private int ntCountKJV = 0;
        private int otCountNRSV = 0;
        private int ntCountNRSV = 0;


        public BibleVersification(BibleTaggingPreperationForm container)
        {
            this.container = container;

            for (int i = 0; i < Constants.ubsNames.Length; i++)
            {
                PopulateDictionary(i, BibleVersion.KJV);
                PopulateDictionary(i, BibleVersion.NRSV);
            }

            otCountKJV = GetVerseCount(kjvOT);
            ntCountKJV = GetVerseCount(kjvNT);
            otCountNRSV = GetVerseCount(nrsvOT);
            ntCountNRSV = GetVerseCount(nrsvNT);

            container.Trace(string.Format("KJV\tOT={0}, NT={1}, Total ={2}", otCountKJV, ntCountKJV, otCountKJV + ntCountKJV), Color.Blue);
            container.Trace(string.Format("NRSV\tOT={0}, NT={1}, Total ={2}", otCountNRSV, ntCountNRSV, otCountNRSV + ntCountNRSV), Color.Blue);
            container.Trace(string.Format("NRSV\tOT={0}, NT={1}, Total ={2}", otCountNRSV, ntCountNRSV, otCountNRSV + ntCountNRSV), Color.Blue);
        }

        private void PopulateDictionary(int index, BibleVersion v)
        {
            Dictionary<string, Dictionary<int, int>> bible = null;
            int[][] lastVerse = null;

            if (v == BibleVersion.KJV)
            {
                bible = (index < 39) ? kjvOT : kjvNT;
                lastVerse = (index < 39) ? Versification.KJV.LAST_VERSE_OT : Versification.KJV.LAST_VERSE_NT;
            }
            else if (v == BibleVersion.NRSV)
            {
                bible = (index < 39) ? nrsvOT : nrsvNT;
                lastVerse = (index < 39) ? Versification.NRSV.LAST_VERSE_OT : Versification.NRSV.LAST_VERSE_NT;
            }

            try
            {
                int[] chapters = lastVerse[index < 39 ? index : index - 39];
                Dictionary<int, int> chaptersDict = new Dictionary<int, int>();
                for (int i = 0; i < chapters.Length; i++)
                {
                    chaptersDict[i + 1] = chapters[i];
                }
                bible[Constants.ubsNames[index]] = chaptersDict;
            }
            catch (Exception e)
            {
                container.Trace(e.Message, Color.Red);
            }
        }

        private int GetVerseCount(Dictionary<string, Dictionary<int, int>> testament)
        {
            int count = 0;

            foreach (Dictionary<int, int> chapter in testament.Values)
            {
                foreach (int verses in chapter.Values)
                    count += verses;

            }

            return count;
        }

        public int GetVerseCount(BibleVersion version, Testament testament)
        {
            if (version == BibleVersion.KJV && testament == Testament.OT) return otCountKJV;
            if (version == BibleVersion.KJV && testament == Testament.NT) return ntCountKJV;
            if (version == BibleVersion.NRSV && testament == Testament.OT) return otCountNRSV;
            if (version == BibleVersion.NRSV && testament == Testament.NT) return ntCountNRSV;

            return 0;
        }
    }
}
