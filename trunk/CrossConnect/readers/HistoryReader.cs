#region Header

// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HistoryReader.cs" company="">
//
// </copyright>
// <summary>
//   Load from a file all the book and verse pointers to the bzz file so that
//   we can later read the bzz file quickly and efficiently.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
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

    using Sword.reader;

    /// <summary>
    /// Load from a file all the book and verse pointers to the bzz file so that
    ///   we can later read the bzz file quickly and efficiently.
    /// </summary>
    [DataContract(Name = "HistoryReader")]
    [KnownType(typeof(ChapterPos))]
    [KnownType(typeof(BookPos))]
    [KnownType(typeof(VersePos))]
    public class HistoryReader : BibleZtextReader
    {
        #region Fields

        /// <summary>
        /// The serial 2.
        /// </summary>
        [DataMember(Name = "serial2")]
        public BibleZtextReaderSerialData Serial2 = new BibleZtextReaderSerialData(false, string.Empty, string.Empty, 0, 0);

        /// <summary>
        /// The _display text.
        /// </summary>
        private string _displayText = string.Empty;

        /// <summary>
        /// The _font family.
        /// </summary>
        private string _fontFamily = string.Empty;

        /// <summary>
        /// The _html background color.
        /// </summary>
        private string _htmlBackgroundColor = string.Empty;

        /// <summary>
        /// The _html font size.
        /// </summary>
        private double _htmlFontSize;

        /// <summary>
        /// The _html foreground color.
        /// </summary>
        private string _htmlForegroundColor = string.Empty;

        /// <summary>
        /// The _html phone accent color.
        /// </summary>
        private string _htmlPhoneAccentColor = string.Empty;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="HistoryReader"/> class.
        /// </summary>
        /// <param name="path">
        /// The path.
        /// </param>
        /// <param name="iso2DigitLangCode">
        /// The iso 2 digit lang code.
        /// </param>
        /// <param name="isIsoEncoding">
        /// The is iso encoding.
        /// </param>
        public HistoryReader(string path, string iso2DigitLangCode, bool isIsoEncoding)
            : base(path, iso2DigitLangCode, isIsoEncoding)
        {
            Serial2.CloneFrom(Serial);
            App.HistoryChanged += AppHistoryChanged;
        }

        // destructor
        /// <summary>
        /// Finalizes an instance of the <see cref="HistoryReader"/> class. 
        /// </summary>
        ~HistoryReader()
        {
            App.HistoryChanged -= AppHistoryChanged;
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets a value indicating whether IsHearable.
        /// </summary>
        public override bool IsHearable
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets a value indicating whether IsPageable.
        /// </summary>
        public override bool IsPageable
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets a value indicating whether IsSearchable.
        /// </summary>
        public override bool IsSearchable
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets a value indicating whether IsSynchronizeable.
        /// </summary>
        public override bool IsSynchronizeable
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets a value indicating whether IsTranslateable.
        /// </summary>
        public override bool IsTranslateable
        {
            get
            {
                return false;
            }
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// The app history changed.
        /// </summary>
        public void AppHistoryChanged()
        {
            _displayText = MakeListDisplayText(
                App.DisplaySettings,
                App.PlaceMarkers.History,
                _htmlBackgroundColor,
                _htmlForegroundColor,
                _htmlPhoneAccentColor,
                _htmlFontSize,
                _fontFamily,
                false,
                string.Empty);
            RaiseSourceChangedEvent();
        }

        /// <summary>
        /// The get info.
        /// </summary>
        /// <param name="bookNum">
        /// The book num.
        /// </param>
        /// <param name="absoluteChaptNum">
        /// The absolute chapt num.
        /// </param>
        /// <param name="relChaptNum">
        /// The rel chapt num.
        /// </param>
        /// <param name="verseNum">
        /// The verse num.
        /// </param>
        /// <param name="fullName">
        /// The full name.
        /// </param>
        /// <param name="title">
        /// The title.
        /// </param>
        public override void GetInfo(
            out int bookNum,
            out int absoluteChaptNum,
            out int relChaptNum,
            out int verseNum,
            out string fullName,
            out string title)
        {
            verseNum = 0;
            bookNum = 0;
            absoluteChaptNum = 0;
            relChaptNum = 0;
            fullName = string.Empty;
            if (App.PlaceMarkers.History.Count > 0)
            {
                BiblePlaceMarker place = App.PlaceMarkers.History[App.PlaceMarkers.History.Count - 1];
                absoluteChaptNum = place.ChapterNum;
                verseNum = place.VerseNum;
            }

            title = Translations.Translate("History");
        }

        /// <summary>
        /// The resume.
        /// </summary>
        public override void Resume()
        {
            Serial.CloneFrom(Serial2);
            App.HistoryChanged += AppHistoryChanged;
            base.Resume();
        }

        /// <summary>
        /// The serial save.
        /// </summary>
        public override void SerialSave()
        {
            Serial2.CloneFrom(Serial);
        }

        /// <summary>
        /// The get chapter html.
        /// </summary>
        /// <param name="displaySettings">
        /// The display settings.
        /// </param>
        /// <param name="htmlBackgroundColor">
        /// The html background color.
        /// </param>
        /// <param name="htmlForegroundColor">
        /// The html foreground color.
        /// </param>
        /// <param name="htmlPhoneAccentColor">
        /// The html phone accent color.
        /// </param>
        /// <param name="htmlFontSize">
        /// The html font size.
        /// </param>
        /// <param name="fontFamily">
        /// The font family.
        /// </param>
        /// <param name="isNotesOnly">
        /// The is notes only.
        /// </param>
        /// <param name="addStartFinishHtml">
        /// The add start finish html.
        /// </param>
        /// <param name="forceReload">
        /// The force reload.
        /// </param>
        /// <returns>
        /// The get chapter html.
        /// </returns>
        protected override string GetChapterHtml(
            DisplaySettings displaySettings,
            string htmlBackgroundColor,
            string htmlForegroundColor,
            string htmlPhoneAccentColor,
            double htmlFontSize,
            string fontFamily,
            bool isNotesOnly,
            bool addStartFinishHtml,
            bool forceReload)
        {
            bool mustUpdate = string.IsNullOrEmpty(_htmlBackgroundColor);
            _htmlBackgroundColor = htmlBackgroundColor;
            _htmlForegroundColor = htmlForegroundColor;
            _htmlPhoneAccentColor = htmlPhoneAccentColor;
            _fontFamily = fontFamily;
            const double epsilon = 0.00000001;
            if (forceReload || mustUpdate || Math.Abs(_htmlFontSize - htmlFontSize) > epsilon)
            {
                _displayText = MakeListDisplayText(
                    displaySettings,
                    App.PlaceMarkers.History,
                    htmlBackgroundColor,
                    htmlForegroundColor,
                    htmlPhoneAccentColor,
                    htmlFontSize,
                    fontFamily,
                    false,
                    string.Empty);
            }

            _htmlFontSize = htmlFontSize;
            return _displayText;
        }

        #endregion Methods
    }
}