using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sword.versification
{
    class CanonLuther : Canon
    {
        private CanonBookDef[] _OldTestBooks =
        {
            new CanonBookDef("Genesis", "Gen", "Gen", 50),
            new CanonBookDef("Exodus", "Exod", "Exod", 40),
            new CanonBookDef("Leviticus", "Lev", "Lev", 27),
            new CanonBookDef("Numbers", "Num", "Num", 36),
            new CanonBookDef("Deuteronomy", "Deut", "Deut", 34),
            new CanonBookDef("Joshua", "Josh", "Josh", 24),
            new CanonBookDef("Judges", "Judg", "Judg", 21),
            new CanonBookDef("Ruth", "Ruth", "Ruth", 4),
            new CanonBookDef("I Samuel", "1Sam", "1Sam", 31),
            new CanonBookDef("II Samuel", "2Sam", "2Sam", 24),
            new CanonBookDef("I Kings", "1Kgs", "1Kgs", 22),
            new CanonBookDef("II Kings", "2Kgs", "2Kgs", 25),
            new CanonBookDef("I Chronicles", "1Chr", "1Chr", 29),
            new CanonBookDef("II Chronicles", "2Chr", "2Chr", 36),
            new CanonBookDef("Ezra", "Ezra", "Ezra", 10),
            new CanonBookDef("Nehemiah", "Neh", "Neh", 13),
            new CanonBookDef("Esther", "Esth", "Esth", 10),
            new CanonBookDef("Job", "Job", "Job", 42),
            new CanonBookDef("Psalms", "Ps", "Ps", 150),
            new CanonBookDef("Proverbs", "Prov", "Prov", 31),
            new CanonBookDef("Ecclesiastes", "Eccl", "Eccl", 12),
            new CanonBookDef("Song of Solomon", "Song", "Song", 8),
            new CanonBookDef("Isaiah", "Isa", "Isa", 66),
            new CanonBookDef("Jeremiah", "Jer", "Jer", 52),
            new CanonBookDef("Lamentations", "Lam", "Lam", 5),
            new CanonBookDef("Ezekiel", "Ezek", "Ezek", 48),
            new CanonBookDef("Daniel", "Dan", "Dan", 12),
            new CanonBookDef("Hosea", "Hos", "Hos", 14),
            new CanonBookDef("Joel", "Joel", "Joel", 4),
            new CanonBookDef("Amos", "Amos", "Amos", 9),
            new CanonBookDef("Obadiah", "Obad", "Obad", 1),
            new CanonBookDef("Jonah", "Jonah", "Jonah", 4),
            new CanonBookDef("Micah", "Mic", "Mic", 7),
            new CanonBookDef("Nahum", "Nah", "Nah", 3),
            new CanonBookDef("Habakkuk", "Hab", "Hab", 3),
            new CanonBookDef("Zephaniah", "Zeph", "Zeph", 3),
            new CanonBookDef("Haggai", "Hag", "Hag", 2),
            new CanonBookDef("Zechariah", "Zech", "Zech", 14),
            new CanonBookDef("Malachi", "Mal", "Mal", 3),
            new CanonBookDef("Judith", "Jdt", "Jdt", 16),
            new CanonBookDef("Wisdom", "Wis", "Wis", 19),
            new CanonBookDef("Tobit", "Tob", "Tob", 14),
            new CanonBookDef("Sirach", "Sir", "Sir", 51),
            new CanonBookDef("Baruch", "Bar", "Bar", 6),
            new CanonBookDef("I Maccabees", "1Macc", "1Macc", 16),
            new CanonBookDef("II Maccabees", "2Macc", "2Macc", 15),
            new CanonBookDef("Additions to Esther", "AddEsth", "AddEsth", 7),
            new CanonBookDef("Additions to Daniel", "AddDan", "AddDan", 3),
            new CanonBookDef("Prayer of Manasses", "PrMan", "PrMan", 1)
        };

        private CanonBookDef[] _NewTestBooks =
        {
            new CanonBookDef("Matthew", "Matt", "Matt", 28),
            new CanonBookDef("Mark", "Mark", "Mark", 16),
            new CanonBookDef("Luke", "Luke", "Luke", 24),
            new CanonBookDef("John", "John", "John", 21),
            new CanonBookDef("Acts", "Acts", "Acts", 28),
            new CanonBookDef("Romans", "Rom", "Rom", 16),
            new CanonBookDef("I Corinthians", "1Cor", "1Cor", 16),
            new CanonBookDef("II Corinthians", "2Cor", "2Cor", 13),
            new CanonBookDef("Galatians", "Gal", "Gal", 6),
            new CanonBookDef("Ephesians", "Eph", "Eph", 6),
            new CanonBookDef("Philippians", "Phil", "Phil", 4),
            new CanonBookDef("Colossians", "Col", "Col", 4),
            new CanonBookDef("I Thessalonians", "1Thess", "1Thess", 5),
            new CanonBookDef("II Thessalonians", "2Thess", "2Thess", 3),
            new CanonBookDef("I Timothy", "1Tim", "1Tim", 6),
            new CanonBookDef("II Timothy", "2Tim", "2Tim", 4),
            new CanonBookDef("Titus", "Titus", "Titus", 3),
            new CanonBookDef("Philemon", "Phlm", "Phlm", 1),
            new CanonBookDef("I Peter", "1Pet", "1Pet", 5),
            new CanonBookDef("II Peter", "2Pet", "2Pet", 3),
            new CanonBookDef("I John", "1John", "1John", 5),
            new CanonBookDef("II John", "2John", "2John", 1),
            new CanonBookDef("III John", "3John", "3John", 1),
            new CanonBookDef("Hebrews", "Heb", "Heb", 13),
            new CanonBookDef("James", "Jas", "Jas", 5),
            new CanonBookDef("Jude", "Jude", "Jude", 1),
            new CanonBookDef("Revelation of John", "Rev", "Rev", 22)
        };

        private int[] _VersesInChapter =
        {
            // Genesis
            31, 25, 24, 26, 32, 22, 24, 22, 29, 32,
            32, 20, 18, 24, 21, 16, 27, 33, 38, 18,
            34, 24, 20, 67, 34, 35, 46, 22, 35, 43,
            54, 33, 20, 31, 29, 43, 36, 30, 23, 23,
            57, 38, 34, 34, 28, 34, 31, 22, 33, 26,
            // Exodus
            22, 25, 22, 31, 23, 30, 29, 28, 35, 29,
            10, 51, 22, 31, 27, 36, 16, 27, 25, 26,
            37, 30, 33, 18, 40, 37, 21, 43, 46, 38,
            18, 35, 23, 35, 35, 38, 29, 31, 43, 38,
            // Leviticus
            17, 16, 17, 35, 26, 23, 38, 36, 24, 20,
            47, 8, 59, 57, 33, 34, 16, 30, 37, 27,
            24, 33, 44, 23, 55, 46, 34,
            // Numbers
            54, 34, 51, 49, 31, 27, 89, 26, 23, 36,
            35, 16, 33, 45, 41, 35, 28, 32, 22, 29,
            35, 41, 30, 25, 19, 65, 23, 31, 39, 17,
            54, 42, 56, 29, 34, 13,
            // Deuteronomy
            46, 37, 29, 49, 33, 25, 26, 20, 29, 22,
            32, 31, 19, 29, 23, 22, 20, 22, 21, 20,
            23, 29, 26, 22, 19, 19, 26, 69, 28, 20,
            30, 52, 29, 12,
            // Joshua
            18, 24, 17, 24, 15, 27, 26, 35, 27, 43,
            23, 24, 33, 15, 63, 10, 18, 28, 51, 9,
            45, 34, 16, 33,
            // Judges
            36, 23, 31, 24, 31, 40, 25, 35, 57, 18,
            40, 15, 25, 20, 20, 31, 13, 31, 30, 48,
            25,
            // Ruth
            22, 23, 18, 22,
            // I Samuel
            28, 36, 21, 22, 12, 21, 17, 22, 27, 27,
            15, 25, 23, 52, 35, 23, 58, 30, 24, 42,
            16, 23, 28, 23, 44, 25, 12, 25, 11, 31,
            13,
            // II Samuel
            27, 32, 39, 12, 25, 23, 29, 18, 13, 19,
            27, 31, 39, 33, 37, 23, 29, 32, 44, 26,
            22, 51, 39, 25,
            // I Kings
            53, 46, 28, 20, 32, 38, 51, 66, 28, 29,
            43, 33, 34, 31, 34, 34, 24, 46, 21, 43,
            29, 54,
            // II Kings
            18, 25, 27, 44, 27, 33, 20, 29, 37, 36,
            20, 22, 25, 29, 39, 20, 41, 37, 37, 21,
            26, 20, 37, 20, 30,
            // I Chronicles
            54, 55, 24, 43, 41, 66, 40, 40, 44, 14,
            47, 41, 14, 17, 29, 43, 27, 17, 19, 8,
            30, 19, 32, 31, 31, 32, 34, 21, 30,
            // II Chronicles
            18, 17, 17, 22, 14, 42, 22, 18, 31, 19,
            23, 16, 23, 14, 19, 14, 19, 34, 11, 37,
            20, 12, 21, 27, 28, 23, 9, 27, 36, 27,
            21, 33, 25, 33, 27, 23,
            // Ezra
            11, 70, 13, 24, 17, 22, 28, 36, 15, 44,
            // Nehemiah
            11, 20, 38, 17, 19, 19, 73, 18, 37, 40,
            36, 47, 31,
            // Esther
            22, 23, 15, 17, 14, 14, 10, 17, 32, 3,
            // Job
            22, 13, 26, 21, 27, 30, 21, 22, 35, 22,
            20, 25, 28, 22, 35, 22, 16, 21, 29, 29,
            34, 30, 17, 25, 6, 14, 23, 28, 25, 31,
            40, 22, 33, 37, 16, 33, 24, 41, 30, 32,
            26, 17,
            // Psalms
            6, 12, 9, 9, 13, 11, 18, 10, 21, 18,
            7, 9, 6, 7, 5, 11, 15, 51, 15, 10,
            14, 32, 6, 10, 22, 12, 14, 9, 11, 13,
            25, 11, 22, 23, 28, 13, 40, 23, 14, 18,
            14, 12, 5, 27, 18, 12, 10, 15, 21, 23,
            21, 11, 7, 9, 24, 14, 12, 12, 18, 14,
            9, 13, 12, 11, 14, 20, 8, 36, 37, 6,
            24, 20, 28, 23, 11, 13, 21, 72, 13, 20,
            17, 8, 19, 13, 14, 17, 7, 19, 53, 17,
            16, 16, 5, 23, 11, 13, 12, 9, 9, 5,
            8, 29, 22, 35, 45, 48, 43, 14, 31, 7,
            10, 10, 9, 8, 18, 19, 2, 29, 176, 7,
            8, 9, 4, 8, 5, 6, 5, 6, 8, 8,
            3, 18, 3, 3, 21, 26, 9, 8, 24, 14,
            10, 8, 12, 15, 21, 10, 20, 14, 9, 6,
            // Proverbs
            33, 22, 35, 27, 23, 35, 27, 36, 18, 32,
            31, 28, 25, 35, 33, 33, 28, 24, 29, 30,
            31, 29, 35, 34, 28, 28, 27, 28, 27, 33,
            31,
            // Ecclesiastes
            18, 26, 22, 17, 19, 12, 29, 17, 18, 20,
            10, 14,
            // Song of Solomon
            17, 17, 11, 16, 16, 12, 14, 14,
            // Isaiah
            31, 22, 26, 6, 30, 13, 25, 23, 20, 34,
            16, 6, 22, 32, 9, 14, 14, 7, 25, 6,
            17, 25, 18, 23, 12, 21, 13, 29, 24, 33,
            9, 20, 24, 17, 10, 22, 38, 22, 8, 31,
            29, 25, 28, 28, 25, 13, 15, 22, 26, 11,
            23, 15, 12, 17, 13, 12, 21, 14, 21, 22,
            11, 12, 19, 11, 25, 24,
            // Jeremiah
            19, 37, 25, 31, 31, 30, 34, 23, 25, 25,
            23, 17, 27, 22, 21, 21, 27, 23, 15, 18,
            14, 30, 40, 10, 38, 24, 22, 17, 32, 24,
            40, 44, 26, 22, 19, 32, 21, 28, 18, 16,
            18, 22, 13, 30, 5, 28, 7, 47, 39, 46,
            64, 34,
            // Lamentations
            22, 22, 66, 22, 22,
            // Ezekiel
            28, 10, 27, 17, 17, 14, 27, 18, 11, 22,
            25, 28, 23, 23, 8, 63, 24, 32, 14, 44,
            37, 31, 49, 27, 17, 21, 36, 26, 21, 26,
            18, 32, 33, 31, 15, 38, 28, 23, 29, 49,
            26, 20, 27, 31, 25, 24, 23, 35,
            // Daniel
            21, 49, 33, 34, 30, 29, 28, 27, 27, 21,
            45, 13,
            // Hosea
            9, 25, 5, 19, 15, 11, 16, 14, 17, 15,
            11, 15, 15, 10,
            // Joel
            20, 27, 5, 21,
            // Amos
            15, 16, 15, 13, 27, 14, 17, 14, 15,
            // Obadiah
            21,
            // Jonah
            16, 11, 10, 11,
            // Micah
            16, 13, 12, 14, 14, 16, 20,
            // Nahum
            14, 14, 19,
            // Habakkuk
            17, 20, 19,
            // Zephaniah
            18, 15, 20,
            // Haggai
            15, 23,
            // Zechariah
            17, 17, 10, 14, 11, 15, 14, 23, 17, 12,
            17, 14, 9, 21,
            // Malachi
            14, 17, 24,
            // Judith
            11, 18, 12, 14, 26, 20, 24, 28, 15, 21,
            17, 21, 31, 16, 16, 31,
            // Wisdom
            16, 25, 19, 20, 24, 27, 30, 21, 19, 21,
            26, 27, 19, 31, 19, 29, 21, 25, 21,
            // Tobit
            25, 23, 25, 22, 29, 23, 20, 23, 12, 13,
            20, 22, 22, 17,
            // Sirach
            38, 23, 34, 36, 18, 37, 40, 22, 25, 34,
            35, 19, 32, 27, 21, 30, 31, 33, 27, 33,
            31, 33, 37, 47, 34, 28, 33, 30, 35, 27,
            40, 28, 32, 31, 26, 28, 34, 39, 41, 32,
            29, 26, 37, 26, 32, 23, 31, 28, 20, 31,
            38,
            // Baruch
            22, 35, 38, 37, 9, 73,
            // I Maccabees
            68, 70, 60, 61, 68, 63, 50, 32, 73, 89,
            74, 54, 54, 49, 41, 24,
            // II Maccabees
            36, 33, 40, 50, 27, 31, 42, 36, 29, 38,
            38, 46, 26, 46, 40,
            // Additions to Esther
            4, 8, 12, 12, 16, 9, 8,
            // Additions to Daniel
            64, 41, 66,
            // Prayer of Manasses
            16,
            // Matthew
            25, 23, 17, 25, 48, 34, 29, 34, 38, 42,
            30, 50, 58, 36, 39, 28, 27, 35, 30, 34,
            46, 46, 39, 51, 46, 75, 66, 20,
            // Mark
            45, 28, 35, 41, 43, 56, 37, 38, 50, 52,
            33, 44, 37, 72, 47, 20,
            // Luke
            80, 52, 38, 44, 39, 49, 50, 56, 62, 42,
            54, 59, 35, 35, 32, 31, 37, 43, 48, 47,
            38, 71, 56, 53,
            // John
            51, 25, 36, 54, 47, 71, 53, 59, 41, 42,
            57, 50, 38, 31, 27, 33, 26, 40, 42, 31,
            25,
            // Acts
            26, 47, 26, 37, 42, 15, 60, 40, 43, 48,
            30, 25, 52, 28, 41, 40, 34, 28, 40, 38,
            40, 30, 35, 27, 27, 32, 44, 31,
            // Romans
            32, 29, 31, 25, 21, 23, 25, 39, 33, 21,
            36, 21, 14, 23, 33, 27,
            // I Corinthians
            31, 16, 23, 21, 13, 20, 40, 13, 27, 33,
            34, 31, 13, 40, 58, 24,
            // II Corinthians
            24, 17, 18, 18, 21, 18, 16, 24, 15, 18,
            33, 21, 13,
            // Galatians
            24, 21, 29, 31, 26, 18,
            // Ephesians
            23, 22, 21, 32, 33, 24,
            // Philippians
            30, 30, 21, 23,
            // Colossians
            29, 23, 25, 18,
            // I Thessalonians
            10, 20, 13, 18, 28,
            // II Thessalonians
            12, 17, 18,
            // I Timothy
            20, 15, 16, 16, 25, 21,
            // II Timothy
            18, 26, 17, 22,
            // Titus
            16, 15, 15,
            // Philemon
            25,
            // I Peter
            25, 25, 22, 19, 14,
            // II Peter
            21, 22, 18,
            // I John
            10, 29, 24, 21, 21,
            // II John
            13,
            // III John
            15,
            // Hebrews
            14, 18, 19, 16, 14, 20, 28, 13, 28, 39,
            40, 29, 25,
            // James
            27, 26, 18, 17, 20,
            // Jude
            25,
            // Revelation of John
            20, 29, 22, 11, 14, 17, 17, 13, 21, 11,
            19, 18, 18, 20, 8, 21, 18, 24, 21, 15,
            27, 21
        };

        public override CanonBookDef[] OldTestBooks
        {
            get
            {
                return _OldTestBooks;
            }
        }

        public override CanonBookDef[] NewTestBooks
        {
            get
            {
                return _NewTestBooks;
            }
        }

        public override int[] VersesInChapter
        {
            get
            {
                return _VersesInChapter;
            }
        }
    }
}
