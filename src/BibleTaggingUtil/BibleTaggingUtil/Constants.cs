using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BibleTaggingUtil
{
    class Constants
    {
        public static string[] fileNames =
        {
            "01-gen",
            "02-Exodus",
            "03-Leviticus",
            "04-Numbers",
            "05-Deut",
            "06-Joshua",
            "07-Judges",
            "08-Ruth",
            "09-1-Samuel",
            "10-2-Samuel",
            "11-1-Kings",
            "12-2-Kings",
            "13-1-Chronicles",
            "14-2-Chronicles",
            "15-Ezra",
            "16-Nehmiah",
            "17-Esther",
            "18-Job",
            "19-Psalms",
            "20-Proverbs",
            "21-Ecclesiastes",
            "22-Sos",
            "23-Isiah",
            "24-Jeremiah",
            "25-Lamentations",
            "26-Ezekiel",
            "27-Daniel",
            "28-Hosea",
            "29-Joel",
            "30-Amos",
            "31-Obadiah",
            "32-Jonah",
            "33-Micah",
            "34-Nahum",
            "35-Habakuk",
            "36-Zephaniah",
            "37-Haggai",
            "38-Zechariah",
            "39-Malachi",
            "40-Matthew",
            "41-Mark",
            "42-Luke",
            "43-John",
            "44-Acts",
            "45-Romans",
            "46-1-Corinthians",
            "47-2-Corinthians",
            "48-Galatians",
            "49-Ephesians",
            "50-Philipians",
            "51-Colossians",
            "52-1-thessalonians",
            "53-2-thessalonians",
            "54-1-Timothy",
            "55-2-Timothy",
            "56-Titus",
            "57-Phillemon",
            "58-Hebrews",
            "59-James",
            "60-1-peter",
            "61-2-peter",
            "62-1-John",
            "63-2-John",
            "64-3-John",
            "65-Jude",
            "66-Revelation"
        };

        public static string[] usfmFileNamePrefixes =
        {
            "02-GEN",
            "03-EXO",
            "04-LEV",
            "05-NUM",
            "06-DEU",
            "07-JOS",
            "08-JDG",
            "09-RUT",
            "10-1SA",
            "11-2SA",
            "12-1KI",
            "13-2KI",
            "14-1CH",
            "15-2CH",
            "16-EZR",
            "17-NEH",
            "18-EST",
            "19-JOB",
            "20-PSA",
            "21-PRO",
            "22-ECC",
            "23-SNG",
            "24-ISA",
            "25-JER",
            "26-LAM",
            "27-EZK",
            "28-DAN",
            "29-HOS",
            "30-JOL",
            "31-AMO",
            "32-OBA",
            "33-JON",
            "34-MIC",
            "35-NAM",
            "36-HAB",
            "37-ZEP",
            "38-HAG",
            "39-ZEC",
            "40-MAL",
            "70-MAT",
            "71-MRK",
            "72-LUK",
            "73-JHN",
            "74-ACT",
            "75-ROM",
            "76-1CO",
            "77-2CO",
            "78-GAL",
            "79-EPH",
            "80-PHP",
            "81-COL",
            "82-1TH",
            "83-2TH",
            "84-1TI",
            "85-2TI",
            "86-TIT",
            "87-PHM",
            "88-HEB",
            "89-JAS",
            "90-1PE",
            "91-2PE",
            "92-1JN",
            "93-2JN",
            "94-3JN",
            "95-JUD",
            "96-REV",
        };


        /// <summary>
        /// Bible reference abbreviations as defined by CrossWire Bible Society
        /// in the OSIS (Open Scriptural Information Standard) specs
        /// </summary>
        public static string[] osisNames =
        {
            "Gen",
            "Exod",
            "Lev",
            "Num",
            "Deut",
            "Josh",
            "Judg",
            "Ruth",
            "1Sam",
            "2Sam",
            "1Kgs",
            "2Kgs",
            "1Chr",
            "2Chr",
            "Ezra",
            "Neh",
            "Esth",
            "Job",
            "Ps",
            "Prov",
            "Eccl",
            "Song",
            "Isa",
            "Jer",
            "Lam",
            "Ezek",
            "Dan",
            "Hos",
            "Joel",
            "Amos",
            "Obad",
            "Jonah",
            "Mic",
            "Nah",
            "Hab",
            "Zeph",
            "Hag",
            "Zech",
            "Mal",
            "Matt",
            "Mark",
            "Luke",
            "John",
            "Acts",
            "Rom",
            "1Cor",
            "2Cor",
            "Gal",
            "Eph",
            "Phil",
            "Col",
            "1Thess",
            "2Thess",
            "1Tim",
            "2Tim",
            "Titus",
            "Phlm",
            "Heb",
            "Jas",
            "1Pet",
            "2Pet",
            "1John",
            "2John",
            "3John",
            "Jude",
            "Rev"
        };

        /// <summary>
        /// American Bible Society & Paratext abbreviations
        /// </summary>
        public static string[] ubsNames =
        {
        "Gen",
        "Exo",
        "Lev",
        "Num",
        "Deu",
        "Jos",
        "Jdg",
        "Rut",
        "1Sa",
        "2Sa",
        "1Ki",
        "2Ki",
        "1Ch",
        "2Ch",
        "Ezr",
        "Neh",
        "Est",
        "Job",
        "Psa",
        "Pro",
        "Ecc",
        "Sng",
        "Isa",
        "Jer",
        "Lam",
        "Ezk",
        "Dan",
        "Hos",
        "Jol",
        "Amo",
        "Oba",
        "Jon",
        "Mic",
        "Nam",
        "Hab",
        "Zep",
        "Hag",
        "Zec",
        "Mal",
        "Mat",
        "Mrk",
        "Luk",
        "Jhn",
        "Act",
        "Rom",
        "1Co",
        "2Co",
        "Gal",
        "Eph",
        "Php",
        "Col",
        "1Th",
        "2Th",
        "1Ti",
        "2Ti",
        "Tit",
        "Phm",
        "Heb",
        "Jas",
        "1Pe",
        "2Pe",
        "1Jn",
        "2Jn",
        "3Jn",
        "Jud",
        "Rev"
        };

        public static string[] osisAltNames =
        {
            "Gen",
            "Exo",
            "Lev",
            "Num",
            "Deu",
            "Jos",
            "Jdg",
            "Rut",
            "1Sa",
            "2Sa",
            "1Ki",
            "2Ki",
            "1Ch",
            "2Ch",
            "Ezr",
            "Neh",
            "Est",
            "Job",
            "Psa",
            "Pro",
            "Ecc",
            "Sng",
            "Isa",
            "Jer",
            "Lam",
            "Ezk",
            "Dan",
            "Hos",
            "Jol",
            "Amo",
            "Oba",
            "Jon",
            "Mic",
            "Nam",
            "Hab",
            "Zep",
            "Hag",
            "Zec",
            "Mal",
            "Mat",
            "Mar",
            "Luk",
            "Joh",
            "Act",
            "Rom",
            "1Co",
            "2Co",
            "Gal",
            "Eph",
            "Phi",
            "Col",
            "1Th",
            "2Th",
            "1Ti",
            "2Ti",
            "Tit",
            "Phm",
            "Heb",
            "Jam",
            "1Pe",
            "2Pe",
            "1Jo",
            "2Jo",
            "3Jo",
            "Jud",
            "Rev"
        };

        /* protected */
        public static int[][] LAST_VERSE =
{
        // Genesis
        new int[]
        {
           31,  25,  24,  26,  32,  22,  24,  22,  29,  32,
           32,  20,  18,  24,  21,  16,  27,  33,  38,  18,
           34,  24,  20,  67,  34,  35,  46,  22,  35,  43,
           55,  32,  20,  31,  29,  43,  36,  30,  23,  23,
           57,  38,  34,  34,  28,  34,  31,  22,  33,  26,
        },
        // Exodus
        new int[]
        {
           22,  25,  22,  31,  23,  30,  25,  32,  35,  29,
           10,  51,  22,  31,  27,  36,  16,  27,  25,  26,
           36,  31,  33,  18,  40,  37,  21,  43,  46,  38,
           18,  35,  23,  35,  35,  38,  29,  31,  43,  38,
        },
        // Leviticus
        new int[]
        {
           17,  16,  17,  35,  19,  30,  38,  36,  24,  20,
           47,   8,  59,  57,  33,  34,  16,  30,  37,  27,
           24,  33,  44,  23,  55,  46,  34,
        },
        // Numbers
        new int[]
        {
           54,  34,  51,  49,  31,  27,  89,  26,  23,  36,
           35,  16,  33,  45,  41,  50,  13,  32,  22,  29,
           35,  41,  30,  25,  18,  65,  23,  31,  40,  16,
           54,  42,  56,  29,  34,  13,
        },
        // Deuteronomy
        new int[]
        {
           46,  37,  29,  49,  33,  25,  26,  20,  29,  22,
           32,  32,  18,  29,  23,  22,  20,  22,  21,  20,
           23,  30,  25,  22,  19,  19,  26,  68,  29,  20,
           30,  52,  29,  12,
        },
        // Joshua
        new int[]
        {
           18,  24,  17,  24,  15,  27,  26,  35,  27,  43,
           23,  24,  33,  15,  63,  10,  18,  28,  51,   9,
           45,  34,  16,  33,
        },
        // Judges
        new int[]
        {
           36,  23,  31,  24,  31,  40,  25,  35,  57,  18,
           40,  15,  25,  20,  20,  31,  13,  31,  30,  48,
           25,
        },
        // Ruth
        new int[]
        {
           22,  23,  18,  22,
        },
        // I Samuel
        new int[]
        {
           28,  36,  21,  22,  12,  21,  17,  22,  27,  27,
           15,  25,  23,  52,  35,  23,  58,  30,  24,  42,
           15,  23,  29,  22,  44,  25,  12,  25,  11,  31,
           13,
        },
        // II Samuel
        new int[]
        {
           27,  32,  39,  12,  25,  23,  29,  18,  13,  19,
           27,  31,  39,  33,  37,  23,  29,  33,  43,  26,
           22,  51,  39,  25,
        },
        // I Kings
        new int[]
        {
           53,  46,  28,  34,  18,  38,  51,  66,  28,  29,
           43,  33,  34,  31,  34,  34,  24,  46,  21,  43,
           29,  53,
        },
        // II Kings
        new int[]
        {
           18,  25,  27,  44,  27,  33,  20,  29,  37,  36,
           21,  21,  25,  29,  38,  20,  41,  37,  37,  21,
           26,  20,  37,  20,  30,
        },
        // I Chronicles
        new int[]
        {
           54,  55,  24,  43,  26,  81,  40,  40,  44,  14,
           47,  40,  14,  17,  29,  43,  27,  17,  19,   8,
           30,  19,  32,  31,  31,  32,  34,  21,  30,
        },
        // II Chronicles
        new int[]
        {
           17,  18,  17,  22,  14,  42,  22,  18,  31,  19,
           23,  16,  22,  15,  19,  14,  19,  34,  11,  37,
           20,  12,  21,  27,  28,  23,   9,  27,  36,  27,
           21,  33,  25,  33,  27,  23,
        },
        // Ezra
        new int[]
        {
           11,  70,  13,  24,  17,  22,  28,  36,  15,  44,
        },
        // Nehemiah
        new int[]
        {
           11,  20,  32,  23,  19,  19,  73,  18,  38,  39,
           36,  47,  31,
        },
        // Esther
        new int[]
        {
           22,  23,  15,  17,  14,  14,  10,  17,  32,   3,
        },
        // Job
        new int[]
        {
           22,  13,  26,  21,  27,  30,  21,  22,  35,  22,
           20,  25,  28,  22,  35,  22,  16,  21,  29,  29,
           34,  30,  17,  25,   6,  14,  23,  28,  25,  31,
           40,  22,  33,  37,  16,  33,  24,  41,  30,  24,
           34,  17,
        },
        // Psalms
        new int[]
        {
            6,  12,   8,   8,  12,  10,  17,   9,  20,  18,
            7,   8,   6,   7,   5,  11,  15,  50,  14,   9,
           13,  31,   6,  10,  22,  12,  14,   9,  11,  12,
           24,  11,  22,  22,  28,  12,  40,  22,  13,  17,
           13,  11,   5,  26,  17,  11,   9,  14,  20,  23,
           19,   9,   6,   7,  23,  13,  11,  11,  17,  12,
            8,  12,  11,  10,  13,  20,   7,  35,  36,   5,
           24,  20,  28,  23,  10,  12,  20,  72,  13,  19,
           16,   8,  18,  12,  13,  17,   7,  18,  52,  17,
           16,  15,   5,  23,  11,  13,  12,   9,   9,   5,
            8,  28,  22,  35,  45,  48,  43,  13,  31,   7,
           10,  10,   9,   8,  18,  19,   2,  29, 176,   7,
            8,   9,   4,   8,   5,   6,   5,   6,   8,   8,
            3,  18,   3,   3,  21,  26,   9,   8,  24,  13,
           10,   7,  12,  15,  21,  10,  20,  14,   9,   6,
        },
        // Proverbs
        new int[]
        {
           33,  22,  35,  27,  23,  35,  27,  36,  18,  32,
           31,  28,  25,  35,  33,  33,  28,  24,  29,  30,
           31,  29,  35,  34,  28,  28,  27,  28,  27,  33,
           31,
        },
        // Ecclesiastes
        new int[]
        {
           18,  26,  22,  16,  20,  12,  29,  17,  18,  20,
           10,  14,
        },
        // Song of Solomon
        new int[]
        {
           17,  17,  11,  16,  16,  13,  13,  14,
        },
        // Isaiah
        new int[]
        {
           31,  22,  26,   6,  30,  13,  25,  22,  21,  34,
           16,   6,  22,  32,   9,  14,  14,   7,  25,   6,
           17,  25,  18,  23,  12,  21,  13,  29,  24,  33,
            9,  20,  24,  17,  10,  22,  38,  22,   8,  31,
           29,  25,  28,  28,  25,  13,  15,  22,  26,  11,
           23,  15,  12,  17,  13,  12,  21,  14,  21,  22,
           11,  12,  19,  12,  25,  24,
        },
        // Jeremiah
        new int[]
        {
           19,  37,  25,  31,  31,  30,  34,  22,  26,  25,
           23,  17,  27,  22,  21,  21,  27,  23,  15,  18,
           14,  30,  40,  10,  38,  24,  22,  17,  32,  24,
           40,  44,  26,  22,  19,  32,  21,  28,  18,  16,
           18,  22,  13,  30,   5,  28,   7,  47,  39,  46,
           64,  34,
        },
        // Lamentations
        new int[]
        {
           22,  22,  66,  22,  22,
        },
        // Ezekiel
        new int[]
        {
           28,  10,  27,  17,  17,  14,  27,  18,  11,  22,
           25,  28,  23,  23,   8,  63,  24,  32,  14,  49,
           32,  31,  49,  27,  17,  21,  36,  26,  21,  26,
           18,  32,  33,  31,  15,  38,  28,  23,  29,  49,
           26,  20,  27,  31,  25,  24,  23,  35,
        },
        // Daniel
        new int[]
        {
           21,  49,  30,  37,  31,  28,  28,  27,  27,  21,
           45,  13,
        },
        // Hosea
        new int[]
        {
           11,  23,   5,  19,  15,  11,  16,  14,  17,  15,
           12,  14,  16,   9,
        },
        // Joel
        new int[]
        {
           20,  32,  21,
        },
        // Amos
        new int[]
        {
           15,  16,  15,  13,  27,  14,  17,  14,  15,
        },
        // Obadiah
        new int[]
        {
           21,
        },
        // Jonah
        new int[]
        {
           17,  10,  10,  11,
        },
        // Micah
        new int[]
        {
           16,  13,  12,  13,  15,  16,  20,
        },
        // Nahum
        new int[]
        {
           15,  13,  19,
        },
        // Habakkuk
        new int[]
        {
           17,  20,  19,
        },
        // Zephaniah
        new int[]
        {
           18,  15,  20,
        },
        // Haggai
        new int[]
        {
           15,  23,
        },
        // Zechariah
        new int[]
        {
           21,  13,  10,  14,  11,  15,  14,  23,  17,  12,
           17,  14,   9,  21,
        },
        // Malachi
        new int[]
        {
           14,  17,  18,   6,
        },
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
           19,  17,  18,  20,   8,  21,  18,  24,  21,  15,
           27,  21,
        },
    };

    }
}
