using System;
using System.Net;
using System.Collections.Generic;
using System.IO;
using ComponentAce.Compression.Libs.zlib;
using System.Globalization;
using System.Runtime.Serialization;
using System.IO.IsolatedStorage;
using System.Text;


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
    public class BibleNoteReader : BibleZtextReader
    {
        public BibleNoteReader(string path, string iso2DigitLangCode, bool isIsoEncoding)
            :base(path, iso2DigitLangCode, isIsoEncoding)
        {
        }

        public override string GetChapterHtml(int chapterNumber, string htmlBackgroundColor, string htmlForegroundColor, string htmlPhoneAccentColor, double htmlFontSize)
        {
            return GetChapterHtml(chapterNumber, htmlBackgroundColor, htmlForegroundColor, htmlPhoneAccentColor, htmlFontSize, true);
        }
    }
}