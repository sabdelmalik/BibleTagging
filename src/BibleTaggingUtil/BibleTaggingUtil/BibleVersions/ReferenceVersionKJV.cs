using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BibleTaggingUtil.BibleVersions
{
    public class ReferenceVersionKJV : BibleVersion
    {
        public ReferenceVersionKJV(BibleTaggingForm container) : base(container) { }

        public void Load()
        {
            try
            {
                string referenceBibleFileFolder = Properties.Settings.Default.referenceBibleFileFolder;
                if (string.IsNullOrEmpty(referenceBibleFileFolder))
                {
                    referenceBibleFileFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory + "bibles");
                }

                string refFile = container.GetBibleFilePath(referenceBibleFileFolder, "Select Reference File");
                string referenceBibleFileName = Path.GetFileName(refFile);
                Properties.Settings.Default.ReferenceBibleFileName = referenceBibleFileName;
                referenceBibleFileFolder = Path.GetDirectoryName(refFile);
                Properties.Settings.Default.referenceBibleFileFolder = referenceBibleFileFolder;

                Properties.Settings.Default.Save();

                LoadBibleFile(refFile, true, false);
            }
            catch (Exception ex)
            {
                Tracing.TraceException(MethodBase.GetCurrentMethod().Name, ex.Message);
            }
        }

    }
}
