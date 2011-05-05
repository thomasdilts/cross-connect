using System;
using System.Net;
using System.Collections.Generic;
using System.IO;
using ComponentAce.Compression.Libs.zlib;
using System.Globalization;
using System.Runtime.Serialization;
using System.IO.IsolatedStorage;
using SwordBackend;
using System.Text;

namespace CrossConnect
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
    public class BookMarkReader : BibleZtextReader
    {
        private string displayText = "";
        private string htmlBackgroundColor="";
        private string htmlForegroundColor = "";
        private string htmlPhoneAccentColor = "";
        private double htmlFontSize=0;
        public BookMarkReader(string path, string iso2DigitLangCode, bool isIsoEncoding)
            :base(path, iso2DigitLangCode, isIsoEncoding)
        {
            App.BookMarksChanged+=App_BookMarksChanged;
        }
        ~BookMarkReader()  // destructor
        {
            App.BookMarksChanged -= App_BookMarksChanged;
        }

        public void App_BookMarksChanged()
        {
            displayText=makeListDisplayText(App.placeMarkers.bookmarks,htmlBackgroundColor, htmlForegroundColor, htmlPhoneAccentColor, htmlFontSize);
        }


        public override string GetChapterHtml(int chapterNumber, string htmlBackgroundColor, string htmlForegroundColor, string htmlPhoneAccentColor, double htmlFontSize)
        {
            bool mustUpdate = (htmlBackgroundColor.Length == 0);
            this.htmlBackgroundColor = htmlBackgroundColor;
            this.htmlForegroundColor = htmlForegroundColor;
            this.htmlPhoneAccentColor = htmlPhoneAccentColor;
            this.htmlFontSize = htmlFontSize;
            if (mustUpdate)
            {
                displayText = makeListDisplayText(App.placeMarkers.bookmarks, htmlBackgroundColor, htmlForegroundColor, htmlPhoneAccentColor, htmlFontSize);
            }
            return displayText;
        }
    }
}