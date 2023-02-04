using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SM.Bible.Formats.OSIS
{
    /// <summary>
    /// This code is based on the Python code by Chris Little https://github.com/chrislit/usfm2osis
    /// which is available under GNU General Public License v3.0.
    /// </summary>
    public partial class USFM2OSIS
    {

        // BEGIN PSF-licensed segment
        // keynat from:
        // http://code.activestate.com/recipes/285264-natural-string-sorting/
        /// <summary>
        /// A natural sort helper function for sort() and sorted() without using
        /// regular expressions or exceptions.
        ///
        /// >>> items = ('Z', 'a', '10th', '1st', '9')
        /// >>> sorted(items)
        /// ['10th', '1st', '9', 'Z', 'a']
        /// >>> sorted(items, key = keynat)
        /// ['1st', '9', '10th', 'a', 'Z']
        /// </summary>
        /// <param name=""></param>
  //        private void key_natural(string theString) {
        /*
            item = type(1)
            sorted_list = []
            for char in theString:
                if char.isdigit():
                    digit = int(char)
                    if sorted_list and type(sorted_list[-1]) == item:
                        sorted_list[-1] = sorted_list[-1] * 10 + digit
                    else:
                        sorted_list.append(digit)
                else:
                    sorted_list.append(char.lower())
            return sorted_list*/
        //        }


        // END PSF-licensed segment

        /// <summary>
        /// Sort helper function that orders according to canon position (defined in
        /// canonicalOrder list), returning canonical position or infinity if not in
        /// the list.
        /// </summary>
        /// <param name=""></param>
        private void key_canon(string filename) {
            /*
             * if filename in FILENAME_TO_OSIS:
                    return CANONICAL_ORDER.index(FILENAME_TO_OSIS[filename])
                return float("inf")"*/
        }
        /// <summary>
        /// Sort helper function that orders according to USFM book number (defined
        /// in usfmNumericOrder list), returning USFM book number or infinity if not in
        /// the list.
        /// </summary>
        /// <param name=""></param>
        private void key_usfm(string filename) {
            /*    if filename in FILENAME_TO_OSIS:
                    return USFM_NUMERIC_ORDER.index(FILENAME_TO_OSIS[filename])
                return float("inf")*/
        }

        /// <summary>
        /// Sort helper function that keeps the items in the order in which they
        /// were supplied (i.e.it doesn't sort at all), returning the number of times
        /// the function has been called.
        /// </summary>
        /// <param name=""></param>
        private void key_supplied(string dummy_val) {

            /*    if not hasattr(key_supplied, "counter"):
                    key_supplied.counter = 0
                key_supplied.counter += 1
                return key_supplied.counter */

        }

    }

 
}
