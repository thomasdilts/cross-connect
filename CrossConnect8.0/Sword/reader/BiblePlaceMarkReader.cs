#region Header

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

namespace Sword.reader
{
    using System;
    using System.Runtime.Serialization;
    using System.Threading.Tasks;

    using System.Collections.Generic;

    /// <summary>
    ///     Load from a file all the book and verse pointers to the bzz file so that
    ///     we can later read the bzz file quickly and efficiently.
    /// </summary>
    [DataContract(Name = "BiblePlaceMarkReader")]
    [KnownType(typeof(ChapterPos))]
    [KnownType(typeof(BookPos))]
    [KnownType(typeof(VersePos))]
    public class BiblePlaceMarkReader : BibleZtextReader
    {
        #region Fields

        [DataMember(Name = "serial2")]
        public BibleZtextReaderSerialData Serial2 = new BibleZtextReaderSerialData(
            false, string.Empty, string.Empty, 0, 0, string.Empty, string.Empty);

        [DataMember]
        public List<BiblePlaceMarker> BookMarksToShow;

        [DataMember]
        protected string _title;

        [DataMember]
        protected bool ShowDate = true;

        protected string _displayText = string.Empty;

        protected string _fontFamily = string.Empty;

        protected HtmlColorRgba _htmlBackgroundColor;

        protected double _htmlFontSize;

        protected HtmlColorRgba _htmlForegroundColor;

        protected HtmlColorRgba _htmlPhoneAccentColor;

        protected HtmlColorRgba _htmlWordsOfChristColor;

        #endregion

        public delegate void BiblePlaceMarkChangedDelegate(List<BiblePlaceMarker> bookMarksToShow, DisplaySettings displaySettings);
        
        public virtual async void BiblePlaceMarkSourceChanged(List<BiblePlaceMarker> biblePlaceMarkToShow, DisplaySettings displaySettings)
        {
            this.BookMarksToShow = BiblePlaceMarker.Clone(biblePlaceMarkToShow);
            this._displayText =
                await
                this.MakeListDisplayText(
                    Serial2.Iso2DigitLangCode,
                    displaySettings,
                    this.BookMarksToShow,
                    this._htmlBackgroundColor,
                    this._htmlForegroundColor,
                    this._htmlPhoneAccentColor,
                    this._htmlWordsOfChristColor,
                    this._htmlFontSize,
                    this._fontFamily,
                    false,
                    string.Empty,
                    this.ShowDate);
            this.RaiseSourceChangedEvent();
        }

        #region Constructors and Destructors

        public BiblePlaceMarkReader(string path, string iso2DigitLangCode, bool isIsoEncoding, string cipherKey, string configPath, string versification, List<BiblePlaceMarker> bookMarksToShow, string title, bool ShowDate)
            : base(path, iso2DigitLangCode, isIsoEncoding, cipherKey, configPath, versification)
        {
            this.Serial2.CloneFrom(this.Serial);
            this.BookMarksToShow = bookMarksToShow;
            this._title = title;
            this.ShowDate = ShowDate;
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

        public override async Task<IBrowserTextSource> Clone()
        {
            var cloned = new BiblePlaceMarkReader(this.Serial.Path, this.Serial.Iso2DigitLangCode, this.Serial.IsIsoEncoding, this.Serial.CipherKey, this.Serial.ConfigPath, this.Serial.Versification, BiblePlaceMarker.Clone(this.BookMarksToShow),this._title,this.ShowDate);
            await cloned.Resume();
            return cloned;
        }

        public override async Task<string> GetTTCtext(bool isVerseOnly)
        {
            var text = await MakeListTtcHearingText(Serial2.Iso2DigitLangCode, this.BookMarksToShow);
            return string.IsNullOrEmpty(text) ? "empty" : text;
        }
        public override void MoveNext(bool isVerseMove)
        {

        }
        public override void MovePrevious(bool isVerseMove)
        {

        }

        public override async Task<string> GetChapterHtml(       
            string isoLangCode,
            DisplaySettings displaySettings,
            HtmlColorRgba htmlBackgroundColor,
            HtmlColorRgba htmlForegroundColor,
            HtmlColorRgba htmlPhoneAccentColor,
            HtmlColorRgba htmlWordsOfChristColor,
            HtmlColorRgba[] htmlHighlightColor,
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
            this._htmlWordsOfChristColor = htmlWordsOfChristColor;
            this._fontFamily = fontFamily;
            const double epsilon = 0.000001;
            if (forceReload || mustUpdate || Math.Abs(this._htmlFontSize - htmlFontSize) > epsilon)
            {
                this._displayText =
                    await
                    this.MakeListDisplayText(
                        isoLangCode,
                        displaySettings,
                        this.BookMarksToShow,
                        htmlBackgroundColor,
                        htmlForegroundColor,
                        htmlPhoneAccentColor,
                        htmlWordsOfChristColor,
                        htmlFontSize,
                        fontFamily,
                        false,
                        string.Empty,
                        this.ShowDate);
            }

            this._htmlFontSize = htmlFontSize;
            return this._displayText;
        }

        public override void GetInfo(
            string isoLangCode,
            out string bookShortName,
            out int relChaptNum,
            out int verseNum,
            out string fullName,
            out string title)
        {
            verseNum = 0;
            bookShortName = string.Empty;
            relChaptNum = 0;
            fullName = string.Empty;
            title = this._title;
        }

        public override async Task Resume()
        {
            this.Serial.CloneFrom(this.Serial2);
            await base.Resume();
        }

        public override void SerialSave()
        {
            this.Serial2.CloneFrom(this.Serial);
        }

        #endregion
    }
}