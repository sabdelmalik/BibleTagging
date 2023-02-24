using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BibleTagging
{public class BerkeleyAligner
    {
        private string alignerFolderName = "berkeleyaligner";
        private string alignerFolderpath = string.Empty;

        private string otMapFolder = "OT_Map";
        private string ntMapFolder = "NT_Map";

        private string otMapPath = string.Empty;
        private string ntMapPath = string.Empty;

        public BerkeleyAligner() 
        {
            string currentFolder = Application.StartupPath;
            alignerFolderpath = Path.Combine(currentFolder, alignerFolderName);
            otMapPath = Path.Combine(alignerFolderpath, otMapFolder);
            ntMapPath = Path.Combine(alignerFolderpath, ntMapFolder);
        }

        public bool Align ()
        {
            bool result = false;

            var dir = new DirectoryInfo(otMapPath);
            if (dir.Exists) dir.Delete(true); // true => recursive delete

            dir = new DirectoryInfo(ntMapPath);
            if (dir.Exists) dir.Delete(true); // true => recursive delete

            RunBerkelyAligner("OT.conf");
            RunBerkelyAligner("NT.conf");

            return result;
        }

        private void RunBerkelyAligner(string confFile)
        {
            try
            {
                //WaitCursorControl(true);
                // java -server -mx1000m -cp berkeleyaligner.jar edu.berkeley.nlp.wordAlignment.Main ++confs/NT.conf\r\n

                string executable = "java";
                
                Process process = new Process();
                process.StartInfo.FileName = executable;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.WorkingDirectory = alignerFolderpath;


                process.StartInfo.Arguments = "-server -mx1000m -cp berkeleyaligner.jar edu.berkeley.nlp.wordAlignment.Main ++confs/" + confFile;
                process.Start();
                while (!process.HasExited) ;

                //MessageBox.Show("Bible Generation completed!");

            }
            catch (Exception ex)
            {
                MessageBox.Show("Bible Generation Failed \r\n" + ex);
            }
            //WaitCursorControl(false);
        }

    }
}
