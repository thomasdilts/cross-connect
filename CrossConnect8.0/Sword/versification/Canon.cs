﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sword.versification
{
    public class Canon
    {
        // this is for the KJV. This is the default
        // Book order: Gen Exod Lev Num Deut Josh Judg Ruth 1Sam 2Sam 1Kgs 2Kgs 1Chr 2Chr Ezra Neh Esth Job Ps Prov Eccl Song Isa Jer Lam Ezek Dan Hos Joel Amos Obad Jonah Mic Nah Hab Zeph Hag Zech Mal !!!!!!!! Matt Mark Luke John Acts Rom 1Cor 2Cor Gal Eph Phil Col 1Thess 2Thess 1Tim 2Tim Titus Phlm Heb Jas 1Pet 2Pet 1John 2John 3John Jude Rev

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
            new CanonBookDef("Joel", "Joel", "Joel", 3),
            new CanonBookDef("Amos", "Amos", "Amos", 9),
            new CanonBookDef("Obadiah", "Obad", "Obad", 1),
            new CanonBookDef("Jonah", "Jonah", "Jonah", 4),
            new CanonBookDef("Micah", "Mic", "Mic", 7),
            new CanonBookDef("Nahum", "Nah", "Nah", 3),
            new CanonBookDef("Habakkuk", "Hab", "Hab", 3),
            new CanonBookDef("Zephaniah", "Zeph", "Zeph", 3),
            new CanonBookDef("Haggai", "Hag", "Hag", 2),
            new CanonBookDef("Zechariah", "Zech", "Zech", 14),
            new CanonBookDef("Malachi", "Mal", "Mal", 4)
        };
        private CanonBookDef[] _NewTestBooks =
        {
            new CanonBookDef("Matthew", "Matt", "Mt,Mat", 28),
            new CanonBookDef("Mark", "Mark", "Mrk,Mk,Mr", 16),
            new CanonBookDef("Luke", "Luke", "Luk,Lk,Lu", 24),
            new CanonBookDef("John", "John", "Jn,Jhn,Joh", 21),
            new CanonBookDef("Acts", "Acts", "Act,Ac", 28),
            new CanonBookDef("Romans", "Rom", "Ro,Rm,Rmn,Rmns", 16),
            new CanonBookDef("I Corinthians", "1Cor", "1 Cor,1 Co,I Co,1Co,I Cor", 16),
            new CanonBookDef("II Corinthians", "2Cor", "2 Cor,2 Co,II Co,2Co,II Cor", 13),
            new CanonBookDef("Galatians", "Gal", "Ga,Galt,Gas", 6),
            new CanonBookDef("Ephesians", "Eph", "Ephes,Ephs", 6),
            new CanonBookDef("Philippians", "Phil", "Php", 4),
            new CanonBookDef("Colossians", "Col", "Co", 4),
            new CanonBookDef("I Thessalonians", "1Thess", "1 Thess,1 Th,I Th,1Th,I Thes,1Thes,I Thess", 5),
            new CanonBookDef("II Thessalonians", "2Thess", "2 Thess,2 Th,II Th,2Th,II Thes,2Thes,II Thess", 3),
            new CanonBookDef("I Timothy", "1Tim", "1 Tim,1 Ti,I Ti,1Ti,I Tim", 6),
            new CanonBookDef("II Timothy", "2Tim", "2 Tim,2 Ti,II Ti,2Ti,II Tim", 4),
            new CanonBookDef("Titus", "Titus", "Tit", 3),
            new CanonBookDef("Philemon", "Phlm", "Phm", 1),
            new CanonBookDef("Hebrews", "Heb", "Hebs,He,Hebw,Hbrw", 13),
            new CanonBookDef("James", "Jas", "Jm", 5),
            new CanonBookDef("I Peter", "1Pet", "1 Pet,1 Pe,I Pe,1Pe,I Pet,I Pt,1 Pt,1Pt", 5),
            new CanonBookDef("II Peter", "2Pet", "2 Pet,2 Pe,II Pe,2Pe,II Pet,II Pt,2 Pt,2Pt", 3),
            new CanonBookDef("I John", "1John", "1 John,1 Jn,I Jn,1Jn,I Jo,1Jo,I Joh,1Joh,I Jhn,1 Jhn,1Jhn", 5),
            new CanonBookDef("II John", "2John", "2 John,2 Jn,II Jn,2Jn,II Jo,2Jo,II Joh,2Joh,II Jhn,2 Jhn,2Jhn", 1),
            new CanonBookDef("III John", "3John", "3 John,3 Jn,III Jn,3Jn,III Jo,3Jo,III Joh,3Joh,III Jhn,3 Jhn,3Jhn", 1),
            new CanonBookDef("Jude", "Jude", "Jud,Jd", 1),
            new CanonBookDef("Revelation of John", "Rev", "Re,Revelation", 22)
        };
        private int[] _VersesInChapter =
        {
            // Genesis
            31, 25, 24, 26, 32, 22, 24, 22, 29, 32,
            32, 20, 18, 24, 21, 16, 27, 33, 38, 18,
            34, 24, 20, 67, 34, 35, 46, 22, 35, 43,
            55, 32, 20, 31, 29, 43, 36, 30, 23, 23,
            57, 38, 34, 34, 28, 34, 31, 22, 33, 26,
            // Exodus
            22, 25, 22, 31, 23, 30, 25, 32, 35, 29,
            10, 51, 22, 31, 27, 36, 16, 27, 25, 26,
            36, 31, 33, 18, 40, 37, 21, 43, 46, 38,
            18, 35, 23, 35, 35, 38, 29, 31, 43, 38,
            // Leviticus
            17, 16, 17, 35, 19, 30, 38, 36, 24, 20,
            47, 8, 59, 57, 33, 34, 16, 30, 37, 27,
            24, 33, 44, 23, 55, 46, 34,
            // Numbers
            54, 34, 51, 49, 31, 27, 89, 26, 23, 36,
            35, 16, 33, 45, 41, 50, 13, 32, 22, 29,
            35, 41, 30, 25, 18, 65, 23, 31, 40, 16,
            54, 42, 56, 29, 34, 13,
            // Deuteronomy
            46, 37, 29, 49, 33, 25, 26, 20, 29, 22,
            32, 32, 18, 29, 23, 22, 20, 22, 21, 20,
            23, 30, 25, 22, 19, 19, 26, 68, 29, 20,
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
            15, 23, 29, 22, 44, 25, 12, 25, 11, 31,
            13,
            // II Samuel
            27, 32, 39, 12, 25, 23, 29, 18, 13, 19,
            27, 31, 39, 33, 37, 23, 29, 33, 43, 26,
            22, 51, 39, 25,
            // I Kings
            53, 46, 28, 34, 18, 38, 51, 66, 28, 29,
            43, 33, 34, 31, 34, 34, 24, 46, 21, 43,
            29, 53,
            // II Kings
            18, 25, 27, 44, 27, 33, 20, 29, 37, 36,
            21, 21, 25, 29, 38, 20, 41, 37, 37, 21,
            26, 20, 37, 20, 30,
            // I Chronicles
            54, 55, 24, 43, 26, 81, 40, 40, 44, 14,
            47, 40, 14, 17, 29, 43, 27, 17, 19, 8,
            30, 19, 32, 31, 31, 32, 34, 21, 30,
            // II Chronicles
            17, 18, 17, 22, 14, 42, 22, 18, 31, 19,
            23, 16, 22, 15, 19, 14, 19, 34, 11, 37,
            20, 12, 21, 27, 28, 23, 9, 27, 36, 27,
            21, 33, 25, 33, 27, 23,
            // Ezra
            11, 70, 13, 24, 17, 22, 28, 36, 15, 44,
            // Nehemiah
            11, 20, 32, 23, 19, 19, 73, 18, 38, 39,
            36, 47, 31,
            // Esther
            22, 23, 15, 17, 14, 14, 10, 17, 32, 3,
            // Job
            22, 13, 26, 21, 27, 30, 21, 22, 35, 22,
            20, 25, 28, 22, 35, 22, 16, 21, 29, 29,
            34, 30, 17, 25, 6, 14, 23, 28, 25, 31,
            40, 22, 33, 37, 16, 33, 24, 41, 30, 24,
            34, 17,
            // Psalms
            6, 12, 8, 8, 12, 10, 17, 9, 20, 18,
            7, 8, 6, 7, 5, 11, 15, 50, 14, 9,
            13, 31, 6, 10, 22, 12, 14, 9, 11, 12,
            24, 11, 22, 22, 28, 12, 40, 22, 13, 17,
            13, 11, 5, 26, 17, 11, 9, 14, 20, 23,
            19, 9, 6, 7, 23, 13, 11, 11, 17, 12,
            8, 12, 11, 10, 13, 20, 7, 35, 36, 5,
            24, 20, 28, 23, 10, 12, 20, 72, 13, 19,
            16, 8, 18, 12, 13, 17, 7, 18, 52, 17,
            16, 15, 5, 23, 11, 13, 12, 9, 9, 5,
            8, 28, 22, 35, 45, 48, 43, 13, 31, 7,
            10, 10, 9, 8, 18, 19, 2, 29, 176, 7,
            8, 9, 4, 8, 5, 6, 5, 6, 8, 8,
            3, 18, 3, 3, 21, 26, 9, 8, 24, 13,
            10, 7, 12, 15, 21, 10, 20, 14, 9, 6,
            // Proverbs
            33, 22, 35, 27, 23, 35, 27, 36, 18, 32,
            31, 28, 25, 35, 33, 33, 28, 24, 29, 30,
            31, 29, 35, 34, 28, 28, 27, 28, 27, 33,
            31,
            // Ecclesiastes
            18, 26, 22, 16, 20, 12, 29, 17, 18, 20,
            10, 14,
            // Song of Solomon
            17, 17, 11, 16, 16, 13, 13, 14,
            // Isaiah
            31, 22, 26, 6, 30, 13, 25, 22, 21, 34,
            16, 6, 22, 32, 9, 14, 14, 7, 25, 6,
            17, 25, 18, 23, 12, 21, 13, 29, 24, 33,
            9, 20, 24, 17, 10, 22, 38, 22, 8, 31,
            29, 25, 28, 28, 25, 13, 15, 22, 26, 11,
            23, 15, 12, 17, 13, 12, 21, 14, 21, 22,
            11, 12, 19, 12, 25, 24,
            // Jeremiah
            19, 37, 25, 31, 31, 30, 34, 22, 26, 25,
            23, 17, 27, 22, 21, 21, 27, 23, 15, 18,
            14, 30, 40, 10, 38, 24, 22, 17, 32, 24,
            40, 44, 26, 22, 19, 32, 21, 28, 18, 16,
            18, 22, 13, 30, 5, 28, 7, 47, 39, 46,
            64, 34,
            // Lamentations
            22, 22, 66, 22, 22,
            // Ezekiel
            28, 10, 27, 17, 17, 14, 27, 18, 11, 22,
            25, 28, 23, 23, 8, 63, 24, 32, 14, 49,
            32, 31, 49, 27, 17, 21, 36, 26, 21, 26,
            18, 32, 33, 31, 15, 38, 28, 23, 29, 49,
            26, 20, 27, 31, 25, 24, 23, 35,
            // Daniel
            21, 49, 30, 37, 31, 28, 28, 27, 27, 21,
            45, 13,
            // Hosea
            11, 23, 5, 19, 15, 11, 16, 14, 17, 15,
            12, 14, 16, 9,
            // Joel
            20, 32, 21,
            // Amos
            15, 16, 15, 13, 27, 14, 17, 14, 15,
            // Obadiah
            21,
            // Jonah
            17, 10, 10, 11,
            // Micah
            16, 13, 12, 13, 15, 16, 20,
            // Nahum
            15, 13, 19,
            // Habakkuk
            17, 20, 19,
            // Zephaniah
            18, 15, 20,
            // Haggai
            15, 23,
            // Zechariah
            21, 13, 10, 14, 11, 15, 14, 23, 17, 12,
            17, 14, 9, 21,
            // Malachi
            14, 17, 18, 6,
            // -----------------------------------------------------------------
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
            30, 25, 52, 28, 41, 40, 34, 28, 41, 38,
            40, 30, 35, 27, 27, 32, 44, 31,
            // Romans
            32, 29, 31, 25, 21, 23, 25, 39, 33, 21,
            36, 21, 14, 23, 33, 27,
            // I Corinthians
            31, 16, 23, 21, 13, 20, 40, 13, 27, 33,
            34, 31, 13, 40, 58, 24,
            // II Corinthians
            24, 17, 18, 18, 21, 18, 16, 24, 15, 18,
            33, 21, 14,
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
            // Hebrews
            14, 18, 19, 16, 14, 20, 28, 13, 28, 39,
            40, 29, 25,
            // James
            27, 26, 18, 17, 20,
            // I Peter
            25, 25, 22, 19, 14,
            // II Peter
            21, 22, 18,
            // I John
            10, 29, 24, 21, 21,
            // II John
            13,
            // III John
            14,
            // Jude
            25,
            // Revelation of John
            20, 29, 22, 11, 14, 17, 17, 13, 21, 11,
            19, 17, 18, 20, 8, 21, 18, 24, 21, 15,
            27, 21
        };

        public Dictionary<string, CanonBookDef> BookByShortName;

        public Canon()
        {
            var oldTestBooks = OldTestBooks;
            int absoluteStartChapter = 0;
            int bookNum = 0;
            BookByShortName = new Dictionary<string, CanonBookDef>();
            foreach (var book in oldTestBooks)
            {
                BookByShortName[book.ShortName1] = book;
                book.VersesInChapterStartIndex = absoluteStartChapter;
                book.BookNum = bookNum;
                bookNum++;
                absoluteStartChapter += book.NumberOfChapters;
            }

            var newTestBooks = NewTestBooks;
            foreach (var book in newTestBooks)
            {
                BookByShortName[book.ShortName1] = book;
                book.VersesInChapterStartIndex = absoluteStartChapter;
                book.BookNum = bookNum;
                bookNum++;
                absoluteStartChapter += book.NumberOfChapters;
            }
        }

        public void SortNames(List<string> bookNames)
        {
            bookNames.Sort(SortByBibleName);
        }

        protected int SortByBibleName(string name1, string name2)
        {
            CanonBookDef book;
            int nameNum1;
            int nameNum2;
            if (!BookByShortName.TryGetValue(name1, out book))
            {
                nameNum1 = 999;
            }
            else
            {
                nameNum1 =  book.BookNum;
            }
            if (!BookByShortName.TryGetValue(name2, out book))
            {
                nameNum2 = 999;
            }
            else
            {
                nameNum2 = book.BookNum;
            }
            return nameNum1.CompareTo(nameNum2);
        }

        public CanonBookDef GetBookFromAbsoluteChapter(int absoluteChapter)
        {
            var values = BookByShortName.Values.ToArray();
            foreach (var book in values)
            {
                if (absoluteChapter >= book.VersesInChapterStartIndex && absoluteChapter < (book.VersesInChapterStartIndex + book.NumberOfChapters))
                {
                    return book;
                }
            }

            return values[values.Count() - 1];
        }

        public CanonBookDef GetBookFromBookNumber(int booknum)
        {
            CanonBookDef book;
            if (booknum >= this.OldTestBooks.Count())
            {
                book = this.NewTestBooks[booknum - this.OldTestBooks.Count()];
            }
            else
            {
                book = this.OldTestBooks[booknum];
            }

            return book;
        }

        public int GetNumChaptersInBible()
        {
            CanonBookDef book = null;
            if(NewTestBooks.Any())
            {
                book = NewTestBooks[NewTestBooks.Count() - 1];
            }
            else
            {
                book = OldTestBooks[OldTestBooks.Count() - 1];
            }

            return book.NumberOfChapters + book.VersesInChapterStartIndex;
        }


        public int GetNumChaptersInOldTestement()
        {
            CanonBookDef book = null;
            if (OldTestBooks.Any())
            {
                book = OldTestBooks[OldTestBooks.Count() - 1];
                return book.NumberOfChapters + book.VersesInChapterStartIndex;
            }
            return 0;
        }

        public virtual CanonBookDef[] OldTestBooks 
        {
            get
            {
                return _OldTestBooks;
            }
        }

        public virtual CanonBookDef[] NewTestBooks
        {
            get
            {
                return _NewTestBooks;         
            }
        }

        public virtual int[] VersesInChapter
        {
            get
            {
                return _VersesInChapter;
            }
        }
    }

    public class CanonBookDef
    {
        public string FullName { get; set; }
        public string ShortName1 { get; set; }
        public string ShortName2 { get; set; }
        public int NumberOfChapters { get; set; }
        public int VersesInChapterStartIndex { get; set; }
        public int BookNum { get; set; }
        public CanonBookDef(string FullName, string ShortName1, string ShortName2, int NumberOfChapters)
        {
            this.FullName = FullName;
            this.ShortName1 = ShortName1;
            this.ShortName2 = ShortName2;
            this.NumberOfChapters = NumberOfChapters;
        }
    }
}
