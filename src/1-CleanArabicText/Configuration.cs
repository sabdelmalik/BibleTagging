using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BibleTagging
{
    public class Configuration
    {
        private Dictionary<string, string> _configuration = null;

        public Configuration(string configurationFile)
        {
            if (!File.Exists(configurationFile))
                return;

            _configuration = new Dictionary<string, string>();
            using(StreamReader sr = new StreamReader(configurationFile))
            {
                while(!sr.EndOfStream)
                {
                    string line = sr.ReadLine();
                    if (string.IsNullOrEmpty(line))
                        continue;
                    if (line.StartsWith("#"))
                        continue;
                    string[] lineParts = line.Split('=');
                    string key = lineParts[0].Trim();
                    string value = lineParts[1].Trim();
                    _configuration[key] = value;
                }
            }
        }

        public string GetConfigValue(string key)
        {
            string result = null;

            if (_configuration != null && _configuration.ContainsKey(key))
            {
                result = _configuration[key];
            }

            return result;
        }

        // ======================================
        // constants
        public const string base_folder = "base_folder";
        public const string bible_file = "bible_file";
        public const string ot_file = "ot_file";
        public const string nt_file = "nt_file";
        public const string bible_clean_file = "bible_clean_file";
        public const string ot_clean_file = "ot_clean_file";
        public const string nt_clean_file = "nt_clean_file";

    }
}
