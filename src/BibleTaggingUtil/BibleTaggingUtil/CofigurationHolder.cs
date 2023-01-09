using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BibleTaggingUtil
{
    public class ConfigurationHolder
    {
        private const string biblesConfigFile = "BiblesConfig.txt";

        private enum ParseState
        {
            None,
            Tagging,
            OSIS,
        }
        public ConfigurationHolder()
        {
            HebrewReferences = new List<string>();
            GreekReferences = new List<string>();
            OSIS = new Dictionary<string, string>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="biblesFolder"></param>
        /// <returns></returns>
        public string ReadBiblesConfig(string biblesFolder)
        {
            string configFilePath = Path.Combine(biblesFolder, biblesConfigFile);
            if (!File.Exists(configFilePath))
                return string.Format("File not found: {0}", configFilePath);

            Properties.Settings.Default.TargetTextDirection = "LTR";

            ParseState state = ParseState.None;
            using (StreamReader sr = new StreamReader(configFilePath))
            {
                while (sr.Peek() >= 0)
                {
                    string line = sr.ReadLine().Trim();
                    if (string.IsNullOrEmpty(line))
                        continue;

                    if(line.StartsWith('['))
                    {
                        int end= line.IndexOf(']');
                        if (end == -1)
                            continue;
                        string section = line.Substring(1, end - 1);
                        switch (section.ToLower())
                        {
                            case "tagging":
                                state= ParseState.Tagging; break;
                            case "osis":
                                state= ParseState.OSIS; break;
                        }
                        continue;
                    }

                    if (state == ParseState.Tagging)
                    {
                        string[] parts = line.Split('=');
                        if (parts.Length != 2)
                            continue;

                        switch (parts[0].Trim().ToLower())
                        {
                            case "untaggedbible":
                                UnTaggedBible = Path.Combine(biblesFolder, parts[1].Trim());
                                break;
                            case "taggedbible":
                                TaggedBible = Path.Combine(biblesFolder, "tagged\\" + parts[1].Trim());
                                break;
                            case "kjv":
                                KJV = Path.Combine(biblesFolder, parts[1].Trim());
                                break;
                            case "hebrewreferences":
                                {
                                    string[] hebParts = parts[1].Split(',');
                                    for (int i = 0; i < hebParts.Length; i++)
                                    {
                                        HebrewReferences.Add(Path.Combine(biblesFolder, hebParts[i]));
                                    }
                                }
                                break;
                            case "greekreferences":
                                {
                                    string[] grkParts = parts[1].Split(',');
                                    for (int i = 0; i < grkParts.Length; i++)
                                    {
                                        GreekReferences.Add(Path.Combine(biblesFolder, grkParts[i]));
                                    }
                                }
                                break;
                            case "targettextdirection":
                                Properties.Settings.Default.TargetTextDirection = parts[1].Trim();
                                break;
                        }
                    }
                    else if(state == ParseState.OSIS)
                    {
                        string[] lineParts = line.Split('=');
                        OSIS[lineParts[0]] = lineParts[1];
                    }

                }
            }
            return string.Empty;
        }

        public string UnTaggedBible { get; private set; }
        public string TaggedBible { get; private set; }

        public string KJV { get; private set; }
        public List<string> HebrewReferences { get; private set; }
        public List<string> GreekReferences { get; private set; }

        public Dictionary<string, string> OSIS { get; private set; }

}


}
