using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Threading.Tasks;

namespace BibleTaggingUtil
{

    public class VerseWord : ICloneable
    {
        public VerseWord(string ancientWord, string english, string[] strong, string transliteration, string reference)
        {
            this.Reference = reference;
            Testament = Utils.GetTestament(reference);

            if (this.Testament == BibleTestament.OT)
                this.Hebrew = ancientWord;
            else if(this.Testament == BibleTestament.NT)
                this.Greek = ancientWord;

            this.Word = english;
            this.Strong = strong;
            this.Transliteration = transliteration;
        }

        public VerseWord(string word, string[] strong, string reference)
        {
            this.Reference = reference;
            Testament = Utils.GetTestament(reference);

            this.Word = word;
            this.Strong = strong;
        }

        public VerseWord(string word, string strong, string reference)
        {
            this.Reference = reference;
            Testament = Utils.GetTestament(reference);

            this.Word = word;
            this.Strong = strong.Replace("<", "").Replace(">", "").Trim().Split(' ');
        }


        public BibleTestament Testament { get; private set; }
        public string Hebrew { get; private set; }
        public string Greek { get; private set; }
        public string Word { get; set; }
        public string[] Strong { get; set; }

        public String StrongString
        {
            get
            {
                string temp = string.Empty;
                if (this.Strong != null)
                {
                    for(int i= 0; i < this.Strong.Length; i++)
                    {
                        temp += "<" + (this.Strong[i].Contains("???")? "" : this.Strong[i]) + "> ";
                    }
                }
                if (temp != "<>")
                    temp = temp.Replace("<>", "").Trim();
                return temp;
            }
            set
            {
                string strong = value.Replace("<", "").Replace(">", "").Trim();
                Strong = strong.Split(' ');
            }
        }
        public string Transliteration { get; private set; }
        public string Reference { get; private set; }

        public override string ToString()
        {
            string strng = string.Empty;
            for (int i = 0; i < Strong.Length; i++)
            {
                strng += " " + Strong[i];
            }
            return Reference + ": " + Word + strng;
        }

        public static VerseWord operator +(VerseWord a, VerseWord b)
        {
            List<string> st = new List<string>();
            bool blankDetected = false;
            for(int i = 0; i < a.Strong.Length; i++)
            {
                if (string.IsNullOrEmpty(a.Strong[i]) || a.Strong[i].Contains("???")|| a.Strong[i].Contains("0000"))
                    blankDetected = true;
                else
                    st.Add(a.Strong[i]);
            }
            for (int i = 0; i < b.Strong.Length; i++)
            {
                if (string.IsNullOrEmpty(b.Strong[i]) || b.Strong[i].Contains("???") || b.Strong[i].Contains("0000"))
                    blankDetected = true;
                else if(!st.Contains(b.Strong[i]))
                    st.Add(b.Strong[i]);
            }
            if (st.Count == 0 && blankDetected)
                st.Add("");

            return new VerseWord(
                (a.Word + " " + b.Word).Trim(),
                st.ToArray(),
                a.Reference
                );
        }
        public object Clone()
        {
            return MemberwiseClone();
        }


    }
}
