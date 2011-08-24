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
/// <copyright file="BookMarkReader.cs" company="Thomas Dilts">
///     Thomas Dilts. All rights reserved.
/// </copyright>
/// <author>Thomas Dilts</author>
namespace CrossConnect.readers
{
    using System.Runtime.Serialization;

    using SwordBackend;

    /// <summary>
    /// Load from a file all the book and verse pointers to the bzz file so that
    /// we can later read the bzz file quickly and efficiently.
    /// </summary>
    /// <param name="path">The path to where the ot.bzs,ot.bzv and ot.bzz and nt files are</param>
    [DataContract(Name = "BookMarkReader")]
    [KnownType(typeof(ChapterPos))]
    [KnownType(typeof(BookPos))]
    [KnownType(typeof(VersePos))]
    public class BookMarkReader : BibleZtextReader
    {
        #region Fields

        [DataMember]
        public BibleZtextReaderSerialData serial2 = new BibleZtextReaderSerialData(false, "", "", 0, 0);

        private string displayText = string.Empty;
        private string htmlBackgroundColor = string.Empty;
        private double htmlFontSize = 0;
        private string htmlForegroundColor = string.Empty;
        private string htmlPhoneAccentColor = string.Empty;

        #endregion Fields

        #region Constructors

        public BookMarkReader(string path, string iso2DigitLangCode, bool isIsoEncoding)
            : base(path, iso2DigitLangCode, isIsoEncoding)
        {
            this.serial2.cloneFrom(base.serial);
            App.BookMarksChanged += this.App_BookMarksChanged;
        }

        // destructor
        ~BookMarkReader()
        {
            App.BookMarksChanged -= this.App_BookMarksChanged;
        }

        #endregion Constructors

        #region Properties

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

        public void App_BookMarksChanged()
        {
            this.displayText = MakeListDisplayText(App.displaySettings, App.placeMarkers.bookmarks, this.htmlBackgroundColor, this.htmlForegroundColor, this.htmlPhoneAccentColor, this.htmlFontSize,false,"");
            raiseSourceChangedEvent();
        }

        public override void GetInfo(out int bookNum, out int absoluteChaptNum, out int relChaptNum, out int verseNum, out string fullName, out string title)
        {
            verseNum = 0;
            absoluteChaptNum = 0;
            bookNum = 0;
            relChaptNum = 0;
            fullName = "";
            title = Translations.translate("Bookmarks");
        }

        public override void Resume()
        {
            base.serial.cloneFrom(this.serial2);
            App.BookMarksChanged += this.App_BookMarksChanged;
            base.Resume();
        }

        public override void SerialSave()
        {
            this.serial2.cloneFrom(base.serial);
        }

        protected override string GetChapterHtml(DisplaySettings displaySettings, string htmlBackgroundColor, string htmlForegroundColor, string htmlPhoneAccentColor, double htmlFontSize,bool isNotesOnly, bool addStartFinishHtml=true)
        {
            bool mustUpdate = string.IsNullOrEmpty(this.htmlBackgroundColor);
            this.htmlBackgroundColor = htmlBackgroundColor;
            this.htmlForegroundColor = htmlForegroundColor;
            this.htmlPhoneAccentColor = htmlPhoneAccentColor;

            if (mustUpdate || this.htmlFontSize != htmlFontSize)
            {
                this.displayText = MakeListDisplayText(displaySettings, App.placeMarkers.bookmarks, htmlBackgroundColor, htmlForegroundColor, htmlPhoneAccentColor, htmlFontSize,false,"");
            }
            this.htmlFontSize = htmlFontSize;
            return this.displayText;
        }

        #endregion Methods
    }
}