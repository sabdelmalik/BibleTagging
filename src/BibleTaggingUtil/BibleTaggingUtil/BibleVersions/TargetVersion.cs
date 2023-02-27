using BibleTaggingUtil.Editor;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BibleTaggingUtil.BibleVersions
{
    public class TargetVersion : BibleVersion
    {
        System.Timers.Timer saveTimer = null;


        public TargetVersion(BibleTaggingForm container) : base(container) { }

        public void SaveUpdates()
        {
            saveTimer.Stop();
            saveTimer.Start();

            if (!container.EditorPanel.TargetDirty)
                return;

            container.WaitCursorControl(true);
            container.EditorPanel.TargetDirty = false;
            container.EditorPanel.SaveCurrentVerse();

            if (bible.Count > 0)
            {
                // construce Updates fileName
                string taggedFolder = Path.GetDirectoryName(container.Config.TaggedBible);
                string oldTaggedFolder = Path.Combine(taggedFolder, "OldTagged");
                if (!Directory.Exists(oldTaggedFolder))
                    Directory.CreateDirectory(oldTaggedFolder);

                // move existing tagged files to the old folder
                String[] existingTagged = Directory.GetFiles(taggedFolder, "*.*");
                foreach (String existingTaggedItem in existingTagged)
                {
                    string fName = Path.GetFileName(existingTaggedItem);
                    string src = Path.Combine(taggedFolder, fName);
                    string dst = Path.Combine(oldTaggedFolder, fName);
                    if (System.IO.File.Exists(dst))
                        System.IO.File.Delete(src);
                    else
                        System.IO.File.Move(src, dst);
                }

                string baseName = Path.GetFileNameWithoutExtension(container.Config.TaggedBible);
                string updatesFileName = string.Format("{0:s}_{1:s}.txt", baseName, DateTime.Now.ToString("yyyy_MM_dd_HH_mm"));
                using (StreamWriter outputFile = new StreamWriter(Path.Combine(taggedFolder, updatesFileName)))
                {

                    for (int i = 0; i < Constants.osisNames.Length; i++)
                    {
                        // construct reference
                        string bookName = Constants.osisNames[i];
                        BibleBook book = container.VerseSelectionPanel.BibleBooks[bookName];
                        if (container.VerseSelectionPanel.UseAltNames)
                            bookName = book.BookAltName;
                        int[] lastVerses = book.LastVerse;
                        //int idx = 0;
                        for (int chapter = 0; chapter < lastVerses.Length; chapter++)
                        {
                            try
                            {
                                int lastVerse = book.LastVerse[chapter];
                                for (int verse = 1; verse <= lastVerse; verse++)
                                {
                                    string verseRef = string.Format("{0:s} {1:d}:{2:d}", bookName, chapter + 1, verse);
                                    if (container.Target.Bible.ContainsKey(verseRef))
                                    {
                                        string line = string.Format("{0:s} {1:s}", verseRef, Utils.GetVerseText(container.Target.Bible[verseRef], true));
                                        outputFile.WriteLine(line);
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                string x = ex.Message;
                            }
                        }

                    }
                }

            }

            Properties.Settings.Default.LastBook = container.VerseSelectionPanel.CurrentBook;
            Properties.Settings.Default.LastChapter = container.VerseSelectionPanel.CurrentChapter;
            Properties.Settings.Default.LastVerse = container.VerseSelectionPanel.CurrentVerse;
            Properties.Settings.Default.Save();
            container.WaitCursorControl(false);
        }

        #region Priodic Save
        public void ActivatePeriodicTimer()
        {
            if (container.InvokeRequired)
            {
                container.Invoke(new Action(() =>
                {
                    ActivatePeriodicTimer();
                }));
            }
            else
            {
                int priodicSaveTime = Properties.Settings.Default.PeriodicSaveTime;

                if (saveTimer == null)
                {
                    saveTimer = new System.Timers.Timer();
                    saveTimer.Elapsed += SaveTimer_Elapsed;
                }
                if (priodicSaveTime > 0)
                {
                    saveTimer.Interval = priodicSaveTime * 60000;
                    saveTimer.AutoReset = true;
                    saveTimer.Enabled = true;
                    saveTimer.Start();
                }
                else
                {
                    if (saveTimer != null)
                    {
                        saveTimer.Stop();
                        saveTimer.Enabled = false;
                    }
                }
            }
        }


        private void SaveTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            PeriodicSave();
        }

        private void PeriodicSave()
        {
            if (container.InvokeRequired)
            {
                container.Invoke(new Action(() =>
                {
                    PeriodicSave();
                }));
            }
            else
            {
                SaveUpdates();
            }
        }

        #endregion Priodic Save
    }
}
