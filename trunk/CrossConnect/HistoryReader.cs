/// <summary>
/// Distribution License:
/// CrossConnect is free software; you can redistribute it and/or modify it under
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
/// </summary>
/// <copyright file="App.xaml.cs" company="Thomas Dilts">
///     Thomas Dilts. All rights reserved.
/// </copyright>
/// <author>Thomas Dilts</author>
namespace CrossConnect
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.IO.IsolatedStorage;
    using System.Net;
    using System.Runtime.Serialization;

    using ComponentAce.Compression.Libs.zlib;

    using SwordBackend;

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
        #region Fields

        private string displayText = string.Empty;
        private string htmlBackgroundColor = string.Empty;
        private double htmlFontSize = 0;
        private string htmlForegroundColor = string.Empty;
        private string htmlPhoneAccentColor = string.Empty;

        #endregion Fields

        #region Constructors

        public HistoryReader(string path, string iso2DigitLangCode, bool isIsoEncoding)
            : base(path, iso2DigitLangCode, isIsoEncoding)
        {
            App.HistoryChanged += this.App_HistoryChanged;
        }

        // destructor
        ~HistoryReader()
        {
            App.HistoryChanged -= this.App_HistoryChanged;
        }

        #endregion Constructors

        #region Properties

        public override bool IsBookmarkable
        {
            get
            {
                return false;
            }
        }

        public override bool IsPageable
        {
            get
            {
                return false;
            }
        }

        public override bool IsSearchable
        {
            get
            {
                return false;
            }
        }

        public override bool IsSynchronizeable
        {
            get
            {
                return false;
            }
        }

        #endregion Properties

        #region Methods

        public void App_HistoryChanged()
        {
            this.displayText = MakeListDisplayText(App.placeMarkers.history, this.htmlBackgroundColor, this.htmlForegroundColor, this.htmlPhoneAccentColor, this.htmlFontSize);
            raiseSourceChangedEvent();
        }

        public override string GetChapterHtml(int chapterNumber, string htmlBackgroundColor, string htmlForegroundColor, string htmlPhoneAccentColor, double htmlFontSize)
        {
            bool mustUpdate = this.htmlBackgroundColor.Length == 0;
            this.htmlBackgroundColor = htmlBackgroundColor;
            this.htmlForegroundColor = htmlForegroundColor;
            this.htmlPhoneAccentColor = htmlPhoneAccentColor;
            
            if (mustUpdate || this.htmlFontSize != htmlFontSize)
            {
                this.displayText = MakeListDisplayText(App.placeMarkers.history, htmlBackgroundColor, htmlForegroundColor, htmlPhoneAccentColor, htmlFontSize);
            }
            this.htmlFontSize = htmlFontSize;
            return this.displayText;
        }

        public override void GetInfo(int chaptNum, int verseNum, out int bookNum, out int relChaptNum, out string fullName, out string title)
        {
            base.GetInfo(chaptNum, verseNum, out bookNum, out relChaptNum, out fullName, out title);
            title = Translations.translate("History");
        }

        #endregion Methods
    }
}