using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace StemmingArabicBible
{
    public class ParatextMorphologyParser
    {

        Dictionary<string, int> stemsCounter = new Dictionary<string, int>();
        Dictionary<string, string> morphs = new Dictionary<string, string>();
        Dictionary<string, string> morphsX = new Dictionary<string, string>();

        int stems = 0;
        int words = 0;

        public Dictionary<string, string> Morphs
        {
            get { return morphs; }
        }
        public void Parse(string morphFilePath)
        {
            XmlNodeReader reader = null;
            var localDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var logFile = Path.Combine(localDir, "morp.log");
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(morphFilePath);

                XmlNode root = doc.DocumentElement;

                XmlNodeList nodeList = root.SelectNodes("MorphGroup");
                foreach (XmlNode node in nodeList)
                {
                    string stem = string.Empty;
                    XmlNodeList rt = node.SelectNodes("root");
                    if (rt.Count == 1 && rt[0].Attributes["type"].Value == "Stem")
                    {
                        stem = rt[0].Attributes["Stem"].Value.Trim();
                        if (stemsCounter.ContainsKey(stem))
                        {
                            stemsCounter[stem] = stemsCounter[stem] + 1;
                        }
                        else
                        {
                            stemsCounter[stem] = 1;
                        }
                        stems++;
                        XmlNodeList infls = node.SelectNodes("morphGroupInfls");
                        foreach (XmlNode inflNode in infls)
                        {
                            XmlNodeList surfForms = inflNode.SelectNodes("SurfForms");
                            if (surfForms.Count == 1)
                            {
                                string surfForm = inflNode.LastChild.InnerText.Trim();
                                words++;
                                if (surfForm != stem)
                                {
                                    if (morphs.ContainsKey(surfForm))
                                    {
                                        if (morphsX.ContainsKey(surfForm))
                                        {
                                            morphsX[surfForm] = morphsX[surfForm] + ", " + stem;
                                        }
                                        else
                                        {
                                            morphsX[surfForm] = morphs[surfForm] + "; " + stem;
                                        }
                                    }
                                    else
                                    {
                                        morphs.Add(surfForm, stem);
                                    }
                                }
                            }
                            else
                            {
                                Console.WriteLine(string.Format("============= {0} has morphGroupInfls with no or more than one SurfForms", stem));
                            }
                        }
                    }
                }
                using (StreamWriter sw = new StreamWriter(logFile))
                {
                    sw.WriteLine(string.Format("Total unique stems = {0}", stemsCounter.Count));
                    sw.WriteLine(string.Format("Duplicate Stems:"));
                    foreach (string key in stemsCounter.Keys)
                    {
                        if (stemsCounter[key] > 1)
                        {
                            sw.WriteLine(string.Format("{0} = {1}", key, stemsCounter[key]));
                        }

                    }


                    sw.WriteLine(string.Format("{0} forms with multiple stems", morphsX.Count));
                    foreach (string key in morphsX.Keys)
                    {
                        sw.WriteLine(string.Format("{0}\t{1}", key, morphsX[key]));
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                if (reader != null)
                    reader.Close();
            }
        }

    }
}
