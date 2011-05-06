using System;
using System.Net;
using System.Collections.Generic;
using System.IO;
using ComponentAce.Compression.Libs.zlib;
using System.Globalization;
using System.Runtime.Serialization;
using System.IO.IsolatedStorage;


namespace SwordBackend
{

    /// <summary>
    /// Load from a file all the book and verse pointers to the bzz file so that
    /// we can later read the bzz file quickly and efficiently.
    /// </summary>
    /// <param name="path">The path to where the ot.bzs,ot.bzv and ot.bzz and nt files are</param>
    [DataContract]
    [KnownType(typeof(ChapterPos))]
    [KnownType(typeof(BookPos))]
    [KnownType(typeof(VersePos))]
    public class SearchReader : BibleZtextReader
    {
        //[DataMember] Tried it but it took 10 times longer then re-reading the mod-file
        public List<ChapterPos> chapters = new List<ChapterPos>();
        //[DataMember] Tried it but it took 10 times longer then re-reading the mod-file
        public List<BookPos> bookPositions = new List<BookPos>();
        [DataMember]
        public string path = "";
        [DataMember]
        public string iso2DigitLangCode = "";

        private BibleNames bookNames=null;

        public string[] getAllShortNames()
        {
            if (bookNames == null)
            {
                bookNames = new BibleNames(iso2DigitLangCode);
            }
            return bookNames.getAllShortNames();
        }
        public bool existsShortNames 
        {
            get
            {
                if (bookNames == null)
                {
                    bookNames = new BibleNames(iso2DigitLangCode);
                }
                return bookNames.existsShortNames;
            }
        }
        public string getShortName(int bookNum)
        {
            if (bookNames == null)
            {
                bookNames = new BibleNames(iso2DigitLangCode);
            }
            return bookNames.getShortName(bookNum);
        }
        public string getFullName(int bookNum)
        {
            if (bookNames == null)
            {
                bookNames = new BibleNames(iso2DigitLangCode);
            }
            return bookNames.getFullName(bookNum);
        }

        public void getInfo(int chaptNum, out int bookNum, out int relChaptNum, out string fullName)
        {
            ChapterPos chaptPos= chapters[chaptNum];
            bookNum = chaptPos.booknum;
            relChaptNum = chaptPos.bookRelativeChapterNum;
            fullName=getFullName(bookNum);
        }
        public void ReloadSettingsFile()
        {
        }
        /// <summary>
        /// Load from a file all the book and verse pointers to the bzz file so that
        /// we can later read the bzz file quickly and efficiently.
        /// </summary>
        /// <param name="path">The path to where the ot.bzs,ot.bzv and ot.bzz and nt files are</param>
        public SearchReader(string path, string iso2DigitLangCode, bool isIsoEncoding)
            : base(path, iso2DigitLangCode, isIsoEncoding)
        {
            this.iso2DigitLangCode = iso2DigitLangCode;
            this.path = path;
            ReloadSettingsFile();
        }

        /// <summary>
        /// Return the entire chapter
        /// </summary>
        /// <param name="bookNumber">Chaptern number beginning with zero for the first chapter and 1188 for the last chapter in the bible.</param>
        /// <returns>Entire Chapter</returns>
        public string GetChapter(int chapterNumber)
        {
            return "";
        }

        
        
    }
}