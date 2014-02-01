#region Header

// <copyright file="HistoryReader.cs" company="Thomas Dilts">
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
    [DataContract(Name = "HistoryReader")]
    [KnownType(typeof(ChapterPos))]
    [KnownType(typeof(BookPos))]
    [KnownType(typeof(VersePos))]
    public class HistoryReader : BibleZtextReader
    {
        #region Fields

        [DataMember(Name = "serial2")]
        public BibleZtextReaderSerialData Serial2 = new BibleZtextReaderSerialData(
            false, string.Empty, string.Empty, 0, 0, string.Empty, string.Empty);

        private string _displayText = string.Empty;

        private string _fontFamily = string.Empty;

        private HtmlColorRgba _htmlBackgroundColor;

        private double _htmlFontSize;

        private HtmlColorRgba _htmlForegroundColor;

        private HtmlColorRgba _htmlPhoneAccentColor;

        private HtmlColorRgba _htmlWordsOfChristColor;

        #endregion

        #region Constructors and Destructors

        public HistoryReader(string path, string iso2DigitLangCode, bool isIsoEncoding, string cipherKey, string configPath, string versification)
            : base(path, iso2DigitLangCode, isIsoEncoding, cipherKey, configPath, versification)
        {
            this.Serial2.CloneFrom(this.Serial);
            App.HistoryChanged += this.AppHistoryChanged;
        }

        // destructor
        ~HistoryReader()
        {
            App.HistoryChanged -= this.AppHistoryChanged;
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

        public async void AppHistoryChanged()
        {
            this._displayText =
                await
                this.MakeListDisplayText(
                    App.DisplaySettings,
                    App.PlaceMarkers.History,
                    this._htmlBackgroundColor,
                    this._htmlForegroundColor,
                    this._htmlPhoneAccentColor,
                    this._htmlWordsOfChristColor,
                    this._htmlFontSize,
                    this._fontFamily,
                    false,
                    string.Empty);
            this.RaiseSourceChangedEvent();
        }

        public override async Task<string> GetTTCtext(bool isVerseOnly)
        {
            var text = await MakeListTtcHearingText(App.PlaceMarkers.History);
            return string.IsNullOrEmpty(text) ? "empty" : text;
        }

        public override void MoveNext(bool isVerseMove)
        {

        }
        public override void MovePrevious(bool isVerseMove)
        {

        }


        public override async Task<string> GetChapterHtml(
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
            const double epsilon = 0.00000001;
            if (forceReload || mustUpdate || Math.Abs(this._htmlFontSize - htmlFontSize) > epsilon)
            {
                this._displayText =
                    await
                    this.MakeListDisplayText(
                        displaySettings,
                        App.PlaceMarkers.History,
                        htmlBackgroundColor,
                        htmlForegroundColor,
                        htmlPhoneAccentColor,
                        htmlWordsOfChristColor,
                        htmlFontSize,
                        fontFamily,
                        false,
                        string.Empty);
            }

            this._htmlFontSize = htmlFontSize;
            return this._displayText;
        }

        public override void GetInfo(
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
            if (App.PlaceMarkers.History.Count > 0)
            {
                BiblePlaceMarker place = App.PlaceMarkers.History[App.PlaceMarkers.History.Count - 1];
                bookShortName = place.BookShortName;
                relChaptNum = place.ChapterNum;
                verseNum = place.VerseNum;
            }

            title = Translations.Translate("History");
        }

        public override async Task Resume()
        {
            this.Serial.CloneFrom(this.Serial2);
            App.HistoryChanged += this.AppHistoryChanged;
            await base.Resume();
        }

        public override async Task<IBrowserTextSource> Clone()
        {
            var cloned = new HistoryReader(this.Serial.Path, this.Serial.Iso2DigitLangCode, this.Serial.IsIsoEncoding, this.Serial.CipherKey, this.Serial.ConfigPath, this.Serial.Versification);
            await cloned.Resume();
            return cloned;
        }
        public override void SerialSave()
        {
            this.Serial2.CloneFrom(this.Serial);
        }

        #endregion
    }
}