using System;
using System.Net;
using System.Collections.Generic;
using System.IO;
using ComponentAce.Compression.Libs.zlib;
using System.Globalization;
using System.Runtime.Serialization;
using System.IO.IsolatedStorage;
using SwordBackend;

///
/// <summary> Distribution License:
/// JSword is free software; you can redistribute it and/or modify it under
/// the terms of the GNU General Public License, version 3 as published by
/// the Free Software Foundation. This program is distributed in the hope
/// that it will be useful, but WITHOUT ANY WARRANTY; without even the
/// implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
/// See the GNU General Public License for more details.
///
/// The License is available on the internet at:
///       http://www.gnu.org/copyleft/gpl.html
/// or by writing to:
///      Free Software Foundation, Inc.
///      59 Temple Place - Suite 330
///      Boston, MA 02111-1307, USA
///
/// Copyright: 2011
///     The copyright to this program is held by Thomas Dilts
///  
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
    public class HistoryReader : BibleZtextReader
    {
        private string displayText = "";
        private string htmlBackgroundColor="";
        private string htmlForegroundColor = "";
        private string htmlPhoneAccentColor = "";
        private double htmlFontSize=0;
        public HistoryReader(string path, string iso2DigitLangCode, bool isIsoEncoding)
            :base(path, iso2DigitLangCode, isIsoEncoding)
        {
            App.HistoryChanged += App_HistoryChanged;
        }
        ~HistoryReader()  // destructor
        {
            App.HistoryChanged -= App_HistoryChanged;
        }
        public override bool isSynchronizeable { get { return false; } }
        public override bool isSearchable { get { return false; } }
        public override bool isPageable { get { return false; } }
        public override bool isBookmarkable { get { return false; } }
        public void App_HistoryChanged()
        {
            displayText=makeListDisplayText(App.placeMarkers.history,htmlBackgroundColor, htmlForegroundColor, htmlPhoneAccentColor, htmlFontSize);
            raiseSourceChangedEvent();
        }

        public override void getInfo(int chaptNum,int verseNum, out int bookNum, out int relChaptNum, out string fullName, out string title)
        {
            base.getInfo(chaptNum, verseNum, out bookNum, out relChaptNum, out fullName, out title);
            title = Translations.translate("History");
        }

        public override string GetChapterHtml(int chapterNumber, string htmlBackgroundColor, string htmlForegroundColor, string htmlPhoneAccentColor, double htmlFontSize)
        {
            bool mustUpdate = (this.htmlBackgroundColor.Length == 0);
            this.htmlBackgroundColor = htmlBackgroundColor;
            this.htmlForegroundColor = htmlForegroundColor;
            this.htmlPhoneAccentColor = htmlPhoneAccentColor;
            this.htmlFontSize = htmlFontSize;
            if (mustUpdate)
            {
                displayText = makeListDisplayText(App.placeMarkers.history, htmlBackgroundColor, htmlForegroundColor, htmlPhoneAccentColor, htmlFontSize);
            }
            return displayText;
        }
    }
}