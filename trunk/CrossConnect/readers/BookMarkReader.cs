// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BookMarkReader.cs" company="">
//   
// </copyright>
// <summary>
//   Load from a file all the book and verse pointers to the bzz file so that
//   we can later read the bzz file quickly and efficiently.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

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

namespace CrossConnect.readers
{
    using System;
    using System.Runtime.Serialization;

    using Sword.reader;

    /// <summary>
    /// Load from a file all the book and verse pointers to the bzz file so that
    ///   we can later read the bzz file quickly and efficiently.
    /// </summary>
    [DataContract(Name = "BookMarkReader")]
    [KnownType(typeof(ChapterPos))]
    [KnownType(typeof(BookPos))]
    [KnownType(typeof(VersePos))]
    public class BookMarkReader : BibleZtextReader
    {
        #region Constants and Fields

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

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="BookMarkReader"/> class.
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
        public BookMarkReader(string path, string iso2DigitLangCode, bool isIsoEncoding)
            : base(path, iso2DigitLangCode, isIsoEncoding)
        {
            this.Serial2.CloneFrom(this.Serial);
            App.BookMarksChanged += this.AppBookMarksChanged;
        }

        // destructor
        /// <summary>
        /// Finalizes an instance of the <see cref="BookMarkReader"/> class. 
        /// </summary>
        ~BookMarkReader()
        {
            App.BookMarksChanged -= this.AppBookMarksChanged;
        }

        #endregion

        #region Public Properties

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

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The app book marks changed.
        /// </summary>
        public void AppBookMarksChanged()
        {
            this._displayText = this.MakeListDisplayText(
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
            absoluteChaptNum = 0;
            bookNum = 0;
            relChaptNum = 0;
            fullName = string.Empty;
            title = Translations.Translate("Bookmarks");
        }

        /// <summary>
        /// The resume.
        /// </summary>
        public override void Resume()
        {
            this.Serial.CloneFrom(this.Serial2);
            App.BookMarksChanged += this.AppBookMarksChanged;
            base.Resume();
        }

        /// <summary>
        /// The serial save.
        /// </summary>
        public override void SerialSave()
        {
            this.Serial2.CloneFrom(this.Serial);
        }

        #endregion

        #region Methods

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
            bool mustUpdate = string.IsNullOrEmpty(this._htmlBackgroundColor);
            this._htmlBackgroundColor = htmlBackgroundColor;
            this._htmlForegroundColor = htmlForegroundColor;
            this._htmlPhoneAccentColor = htmlPhoneAccentColor;
            this._fontFamily = fontFamily;
            const double epsilon = 0.000001;
            if (forceReload || mustUpdate || Math.Abs(this._htmlFontSize - htmlFontSize) > epsilon)
            {
                this._displayText = this.MakeListDisplayText(
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

        #endregion
    }
}