#region Header

// <copyright file="BookMarkReader.cs" company="Thomas Dilts">
//
// CrossConnect Bible and Bible Commentary Reader for CrossWire.org
// Copyright (C) 2011 Thomas Dilts
//
// This program is free software: you can redistribute it and/or modify
// it under the +terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see http://www.gnu.org/licenses/.
// </copyright>
// <summary>
// Email: thomas@chaniel.se
// </summary>
// <author>Thomas Dilts</author>

#endregion Header

namespace CrossConnect.readers
{
    using System;
    using System.Runtime.Serialization;

    using Sword.reader;

    /// <summary>
    ///   Load from a file all the book and verse pointers to the bzz file so that
    ///   we can later read the bzz file quickly and efficiently.
    /// </summary>
    [DataContract(Name = "BookMarkReader")]
    [KnownType(typeof (ChapterPos))]
    [KnownType(typeof (BookPos))]
    [KnownType(typeof (VersePos))]
    public class BookMarkReader : BibleZtextReader
    {
        #region Fields

        [DataMember(Name = "serial2")]
        public BibleZtextReaderSerialData Serial2 = new BibleZtextReaderSerialData(false, "", "", 0, 0);

        private string _displayText = string.Empty;
        private string _htmlBackgroundColor = string.Empty;
        private double _htmlFontSize;
        private string _htmlForegroundColor = string.Empty;
        private string _htmlPhoneAccentColor = string.Empty;
        private string _fontFamily = string.Empty;

        #endregion Fields

        #region Constructors

        public BookMarkReader(string path, string iso2DigitLangCode, bool isIsoEncoding)
            : base(path, iso2DigitLangCode, isIsoEncoding)
        {
            Serial2.CloneFrom(Serial);
            App.BookMarksChanged += AppBookMarksChanged;
        }

        // destructor
        ~BookMarkReader()
        {
            App.BookMarksChanged -= AppBookMarksChanged;
        }

        #endregion Constructors

        #region Properties

        public override bool IsHearable
        {
            get { return false; }
        }

        public override bool IsPageable
        {
            get { return false; }
        }

        public override bool IsSearchable
        {
            get { return false; }
        }

        public override bool IsSynchronizeable
        {
            get { return false; }
        }

        public override bool IsTranslateable
        {
            get { return false; }
        }

        #endregion Properties

        #region Methods

        public void AppBookMarksChanged()
        {
            _displayText = MakeListDisplayText(App.DisplaySettings, App.PlaceMarkers.Bookmarks, _htmlBackgroundColor,
                                              _htmlForegroundColor, _htmlPhoneAccentColor, _htmlFontSize, _fontFamily, false, "");
            RaiseSourceChangedEvent();
        }

        public override void GetInfo(out int bookNum, out int absoluteChaptNum, out int relChaptNum, out int verseNum,
            out string fullName, out string title)
        {
            verseNum = 0;
            absoluteChaptNum = 0;
            bookNum = 0;
            relChaptNum = 0;
            fullName = "";
            title = Translations.Translate("Bookmarks");
        }

        public override void Resume()
        {
            Serial.CloneFrom(Serial2);
            App.BookMarksChanged += AppBookMarksChanged;
            base.Resume();
        }

        public override void SerialSave()
        {
            Serial2.CloneFrom(Serial);
        }

        protected override string GetChapterHtml(DisplaySettings displaySettings, string htmlBackgroundColor,
            string htmlForegroundColor, string htmlPhoneAccentColor,
            double htmlFontSize, string fontFamily, bool isNotesOnly, bool addStartFinishHtml = true)
        {
            bool mustUpdate = string.IsNullOrEmpty(_htmlBackgroundColor);
            _htmlBackgroundColor = htmlBackgroundColor;
            _htmlForegroundColor = htmlForegroundColor;
            _htmlPhoneAccentColor = htmlPhoneAccentColor;
            _fontFamily = fontFamily;
            const double epsilon = 0.000001;
            if (mustUpdate || Math.Abs(_htmlFontSize - htmlFontSize) > epsilon)
            {
                _displayText = MakeListDisplayText(displaySettings, App.PlaceMarkers.Bookmarks, htmlBackgroundColor,
                                                  htmlForegroundColor, htmlPhoneAccentColor, htmlFontSize, fontFamily, false, "");
            }
            _htmlFontSize = htmlFontSize;
            return _displayText;
        }

        #endregion Methods
    }
}