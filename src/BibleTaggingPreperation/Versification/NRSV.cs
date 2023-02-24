/**
 * This code is taken form https://github.com/crosswire/jsword repository
 * 
 * Distribution License:
 * JSword is free software; you can redistribute it and/or modify it under
 * the terms of the GNU Lesser General Public License, version 2.1 or later
 * as published by the Free Software Foundation. This program is distributed
 * in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even
 * the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
 * See the GNU Lesser General Public License for more details.
 *
 * The License is available on the internet at:
 *       http://www.gnu.org/copyleft/lgpl.html
 * or by writing to:
 *      Free Software Foundation, Inc.
 *      59 Temple Place - Suite 330
 *      Boston, MA 02111-1307, USA
 *
 * Copyright: 2012
 *     The copyright to this program is held by it's authors.
 *
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BibleTagging.Versification
{
    internal class NRSV
    {
        public static int[][] LAST_VERSE_OT = KJV.LAST_VERSE_OT;


        /* protected */
        public static int[][] LAST_VERSE_NT =
{
        // Matthew
        new int[]
        {
           25,  23,  17,  25,  48,  34,  29,  34,  38,  42,
           30,  50,  58,  36,  39,  28,  27,  35,  30,  34,
           46,  46,  39,  51,  46,  75,  66,  20,
        },
        // Mark
        new int[]
        {
           45,  28,  35,  41,  43,  56,  37,  38,  50,  52,
           33,  44,  37,  72,  47,  20,
        },
        // Luke
        new int[]
        {
           80,  52,  38,  44,  39,  49,  50,  56,  62,  42,
           54,  59,  35,  35,  32,  31,  37,  43,  48,  47,
           38,  71,  56,  53,
        },
        // John
        new int[]
        {
           51,  25,  36,  54,  47,  71,  53,  59,  41,  42,
           57,  50,  38,  31,  27,  33,  26,  40,  42,  31,
           25,
        },
        // Acts
        new int[]
        {
           26,  47,  26,  37,  42,  15,  60,  40,  43,  48,
           30,  25,  52,  28,  41,  40,  34,  28,  41,  38,
           40,  30,  35,  27,  27,  32,  44,  31,
        },
        // Romans
        new int[]
        {
           32,  29,  31,  25,  21,  23,  25,  39,  33,  21,
           36,  21,  14,  23,  33,  27,
        },
        // I Corinthians
        new int[]
        {
           31,  16,  23,  21,  13,  20,  40,  13,  27,  33,
           34,  31,  13,  40,  58,  24,
        },
        // II Corinthians
        new int[]
        {
           24,  17,  18,  18,  21,  18,  16,  24,  15,  18,
           33,  21,  14,
        },
        // Galatians
        new int[]
        {
           24,  21,  29,  31,  26,  18,
        },
        // Ephesians
        new int[]
        {
           23,  22,  21,  32,  33,  24,
        },
        // Philippians
        new int[]
        {
           30,  30,  21,  23,
        },
        // Colossians
        new int[]
        {
           29,  23,  25,  18,
        },
        // I Thessalonians
        new int[]
        {
           10,  20,  13,  18,  28,
        },
        // II Thessalonians
        new int[]
        {
           12,  17,  18,
        },
        // I Timothy
        new int[]
        {
           20,  15,  16,  16,  25,  21,
        },
        // II Timothy
        new int[]
        {
           18,  26,  17,  22,
        },
        // Titus
        new int[]
        {
           16,  15,  15,
        },
        // Philemon
        new int[]
        {
           25,
        },
        // Hebrews
        new int[]
        {
           14,  18,  19,  16,  14,  20,  28,  13,  28,  39,
           40,  29,  25,
        },
        // James
        new int[]
        {
           27,  26,  18,  17,  20,
        },
        // I Peter
        new int[]
        {
           25,  25,  22,  19,  14,
        },
        // II Peter
        new int[]
        {
           21,  22,  18,
        },
        // I John
        new int[]
        {
           10,  29,  24,  21,  21,
        },
        // II John
        new int[]
        {
           13,
        },
        // III John
        new int[]
        {
           15,
        },
        // Jude
        new int[]
        {
           25,
        },
        // Revelation of John
        new int[]
        {
           20,  29,  22,  11,  14,  17,  17,  13,  21,  11,
           19,  18,  18,  20,   8,  21,  18,  24,  21,  15,
           27,  21,
        },
    };

    }
}
