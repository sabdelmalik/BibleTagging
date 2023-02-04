using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace SM.Bible.Formats.OSIS
{
    /// <summary>
    /// This code is based on the Python code by Chris Little https://github.com/chrislit/usfm2osis
    /// which is available under GNU General Public License v3.0.
    /// </summary>
    public partial class USFM2OSIS
    {
        private void verbose_print(string text, bool verbose)
        {
            if (verbose)
            {
                Console.WriteLine(text);
            }
        }
    }
}
