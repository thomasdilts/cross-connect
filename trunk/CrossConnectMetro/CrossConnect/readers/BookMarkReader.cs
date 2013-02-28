﻿#region Header

// <copyright file="BookMarkReader.cs" company="Thomas Dilts">
// CrossConnect Bible and Bible Commentary Reader for CrossWire.org
// Copyright (C) 2011 Thomas Dilts
// This program is free software: you can redistribute it and/or modify
// it under the +terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see http://www.gnu.org/licenses/.
// </copyright>
// <summary>
// Email: thomas@cross-connect.se
// </summary>
// <author>Thomas Dilts</author>

#endregion Header

namespace CrossConnect.readers
{
    using System;
    using System.Runtime.Serialization;
    using System.Threading.Tasks;

    using Sword.reader;

    /// <summary>
    ///     Load from a file all the book and verse pointers to the bzz file so that
    ///     we can later read the bzz file quickly and efficiently.
    /// </summary>
    [DataContract(Name = "BookMarkReader")]
    [KnownType(typeof(ChapterPos))]
    [KnownType(typeof(BookPos))]
    [KnownType(typeof(VersePos))]
    public class BookMarkReader : BibleZtextReader
    {
        #region Fields

        [DataMember(Name = "serial2")]
        public BibleZtextReaderSerialData Serial2 = new BibleZtextReaderSerialData(
            false, string.Empty, string.Empty, 0, 0);

        private string _displayText = string.Empty;

        private string _fontFamily = string.Empty;

        private HtmlColorRgba _htmlBackgroundColor;

        private double _htmlFontSize;

        private HtmlColorRgba _htmlForegroundColor;

        private HtmlColorRgba _htmlPhoneAccentColor;

        #endregion

        #region Constructors and Destructors

        public BookMarkReader(string path, string iso2DigitLangCode, bool isIsoEncoding)
            : base(path, iso2DigitLangCode, isIsoEncoding)
        {
            this.Serial2.CloneFrom(this.Serial);
            App.BookMarksChanged += this.AppBookMarksChanged;
        }

        // destructor
        ~BookMarkReader()
        {
            App.BookMarksChanged -= this.AppBookMarksChanged;
        }

        #endregion

        #region Public Properties

        public override bool IsHearable
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

        public override bool IsTranslateable
        {
            get
            {
                return false;
            }
        }

        #endregion

        #region Public Methods and Operators

        public async void AppBookMarksChanged()
        {
            this._displayText =
                await
                this.MakeListDisplayText(
                    App.DisplaySettings,
                    App.PlaceMarkers.Bookmarks,
                    this._htmlBackgroundColor,
                    this._htmlForegroundColor,
                    this._htmlPhoneAccentColor,
                    this._htmlFontSize,
                    this._fontFamily,
                    false,
                    string.Empty);
            this.RaiseSourceChangedEvent();
        }

        public override async Task<string> GetChapterHtml(
            DisplaySettings displaySettings,
            HtmlColorRgba htmlBackgroundColor,
            HtmlColorRgba htmlForegroundColor,
            HtmlColorRgba htmlPhoneAccentColor,
            double htmlFontSize,
            string fontFamily,
            bool isNotesOnly,
            bool addStartFinishHtml,
            bool forceReload)
        {
            bool mustUpdate = this._htmlBackgroundColor == null;
            this._htmlBackgroundColor = htmlBackgroundColor;
            this._htmlForegroundColor = htmlForegroundColor;
            this._htmlPhoneAccentColor = htmlPhoneAccentColor;
            this._fontFamily = fontFamily;
            const double epsilon = 0.000001;
            if (forceReload || mustUpdate || Math.Abs(this._htmlFontSize - htmlFontSize) > epsilon)
            {
                this._displayText =
                    await
                    this.MakeListDisplayText(
                        displaySettings,
                        App.PlaceMarkers.Bookmarks,
                        htmlBackgroundColor,
                        htmlForegroundColor,
                        htmlPhoneAccentColor,
                        htmlFontSize,
                        fontFamily,
                        false,
                        string.Empty);
            }

            this._htmlFontSize = htmlFontSize;
            return this._displayText;
        }

        public override void GetInfo(
            out int bookNum,
            out int absoluteChaptNum,
            out int relChaptNum,
            out int verseNum,
            out string fullName,
            out string title)
        {
            verseNum = 0;
            absoluteChaptNum = 0;
            bookNum = 0;
            relChaptNum = 0;
            fullName = string.Empty;
            title = Translations.Translate("Bookmarks");
        }

        public override async Task Resume()
        {
            this.Serial.CloneFrom(this.Serial2);
            App.BookMarksChanged += this.AppBookMarksChanged;
            await base.Resume();
        }

        public override void SerialSave()
        {
            this.Serial2.CloneFrom(this.Serial);
        }

        #endregion
    }
}