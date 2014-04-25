#region Header

// <copyright file="PersonalNotesReader.cs" company="Thomas Dilts">
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
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Threading.Tasks;

    using Sword.reader;
    using Sword.versification;

    /// <summary>
    ///     Load from a file all the book and verse pointers to the bzz file so that
    ///     we can later read the bzz file quickly and efficiently.
    /// </summary>
    [DataContract(Name = "PersonalNotesReader")]
    [KnownType(typeof(ChapterPos))]
    [KnownType(typeof(BookPos))]
    [KnownType(typeof(VersePos))]
    public class PersonalNotesReader : BibleZtextReader
    {
        #region Constants

        private const double Epsilon = 0.00001;

        private const int LimitForPaging = 60;

        #endregion

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

        public PersonalNotesReader(string path, string iso2DigitLangCode, bool isIsoEncoding, string cipherKey, string configPath, string versification)
            : base(path, iso2DigitLangCode, isIsoEncoding, cipherKey, configPath, versification)
        {
            this.Serial2.CloneFrom(this.Serial);
            App.PersonalNotesChanged += this.AppPersonalNotesChanged;
            this.SetToFirstChapter();
        }

        ~PersonalNotesReader()
        {
            App.PersonalNotesChanged -= this.AppPersonalNotesChanged;
        }

        #endregion

        #region Public Properties


        public override bool IsTTChearable
        {
            get
            {
                return false;
            }
        }
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
                int count = App.DailyPlan.PersonalNotesVersified.Sum(dict => dict.Value.Sum(dict2 => dict2.Value.Count));
                return App.DisplaySettings.ShowAddedNotesByChapter || count > LimitForPaging;
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
                return true;
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

        public static int CompareIntegersAssending(int a, int b)
        {
            if (a == b)
            {
                return 0;
            }

            if (a < b)
            {
                return 1;
            }

            return -1;
        }

        public static int CompareIntegersDescending(int a, int b)
        {
            if (a == b)
            {
                return 0;
            }

            if (a > b)
            {
                return 1;
            }

            return -1;
        }

        public async void AppPersonalNotesChanged()
        {
            if (this.IsPageable)
            {
                var book = canon.BookByShortName[Serial.PosBookShortName];
                // show just the one chapter.
                this._displayText =
                    await
                    this.MakeListDisplayText(
                        Translations.IsoLanguageCode,
                        App.DisplaySettings,
                        GetSortedList(this.Serial.PosBookShortName, this.Serial.PosChaptNum, false, true),
                        this._htmlBackgroundColor,
                        this._htmlForegroundColor,
                        this._htmlPhoneAccentColor,
                        this._htmlWordsOfChristColor,
                        this._htmlFontSize,
                        this._fontFamily,
                        false,
                        Translations.Translate("Added notes"));
            }
            else
            {
                // put it all into one window
                this._displayText =
                    await
                    this.MakeListDisplayText(
                        Translations.IsoLanguageCode,
                        App.DisplaySettings,
                        GetSortedList(string.Empty, 0, true, true),
                        this._htmlBackgroundColor,
                        this._htmlForegroundColor,
                        this._htmlPhoneAccentColor,
                        this._htmlWordsOfChristColor,
                        this._htmlFontSize,
                        this._fontFamily,
                        true,
                        Translations.Translate("Added notes"));
            }

            this.RaiseSourceChangedEvent();
        }

        /// <returns>
        /// </returns>
        public override ButtonWindowSpecs GetButtonWindowSpecs(int stage, int lastSelectedButton, string isoLangCode)
        {
            if (stage == 0)
            {
                var books = new Dictionary<int, int>();
                int count = App.DailyPlan.PersonalNotesVersified.Count;
                var butColors = new int[count];
                var values = new int[count];
                var butText = new string[count];

                var keys = new string[count];
                App.DailyPlan.PersonalNotesVersified.Keys.CopyTo(keys, 0);
                var sortedKeys = new List<string>(keys);
                canon.SortNames(sortedKeys);

                for (int i = 0; i < count; i++)
                {

                    int bookNum = canon.BookByShortName[sortedKeys[i]].BookNum;
                    if (!books.ContainsKey(bookNum))
                    {
                        butColors[books.Count] = ChapterCategories[sortedKeys[i]];
                        butText[books.Count] = this.BookNames(isoLangCode).GetShortName(sortedKeys[i]);
                        values[books.Count] = bookNum;

                        books.Add(bookNum, bookNum);
                    }
                }

                return new ButtonWindowSpecs(
                    stage,
                    "Select a book to view",
                    books.Count,
                    butColors,
                    butText,
                    values,
                    !this.BookNames(Translations.IsoLanguageCode).ExistsShortNames ? ButtonSize.Large : ButtonSize.Medium);
            }
            else
            {
                CanonBookDef book = canon.GetBookFromBookNumber(lastSelectedButton);
                var allInBook = App.DailyPlan.PersonalNotesVersified[book.ShortName1];
                int count = allInBook.Keys.Count();
                var keys = new int[count];
                allInBook.Keys.CopyTo(keys, 0);
                var sortedKeys = new List<int>(keys);
                sortedKeys.Sort();

                var butColors = new int[count];
                var values = new int[count];
                var butText = new string[count];

                for (int i = 0; i < count; i++)
                {
                    butColors[i] = 0;
                    butText[i] = (sortedKeys[i] + 1).ToString();
                    values[i] = sortedKeys[i];
                }

                return new ButtonWindowSpecs(
                    stage, "Select a chapter to view", count, butColors, butText, values, ButtonSize.Small);
            }
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
            bool isPageable = this.IsPageable;
            if (forceReload || isPageable || mustUpdate || Math.Abs(this._htmlFontSize - htmlFontSize) > Epsilon)
            {
                if (isPageable)
                {
                    var book = canon.BookByShortName[Serial.PosBookShortName];
                    // show just the one chapter.
                    this._displayText =
                        await
                        this.MakeListDisplayText(
                            isoLangCode,
                            displaySettings,
                            GetSortedList(this.Serial.PosBookShortName, this.Serial.PosChaptNum, false, true),
                            htmlBackgroundColor,
                            htmlForegroundColor,
                            htmlPhoneAccentColor,
                            htmlWordsOfChristColor,
                            htmlFontSize,
                            fontFamily,
                            false,
                            Translations.Translate("Added notes"));
                }
                else
                {
                    // put it all into one window
                    this._displayText =
                        await
                        this.MakeListDisplayText(
                            Translations.IsoLanguageCode,
                            displaySettings,
                            GetSortedList(string.Empty, 0, true, true),
                            htmlBackgroundColor,
                            htmlForegroundColor,
                            htmlPhoneAccentColor,
                            htmlWordsOfChristColor,
                            htmlFontSize,
                            this._fontFamily,
                            true,
                            Translations.Translate("Added notes"));
                }
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
            base.GetInfo(isoLangCode, out bookShortName, out relChaptNum, out verseNum, out fullName, out title);
            title = Translations.Translate("Added notes");
        }

        public override void MoveChapterVerse(string bookShortName, int chapter, int verse, bool isLocalLinkChange, IBrowserTextSource source)
        {
            if (!IsPageable)
            {
                return;
            }

            this.MoveChapterVerse(bookShortName, chapter, verse, isLocalLinkChange, false);
        }

        public override void MoveNext(bool isVerse)
        {
            int nextChapter = this.Serial.PosChaptNum + 1;
            var book = canon.BookByShortName[this.Serial.PosBookShortName];
            if (nextChapter >= book.NumberOfChapters)
            {
                // must go to the next book.
                if((book.NumberOfChapters + book.VersesInChapterStartIndex + 1)>canon.GetNumChaptersInBible())
                {
                    // no more books left. Go to the first one.
                    var nextBook = canon.GetBookFromAbsoluteChapter(0);
                    this.MoveChapterVerse(nextBook.ShortName1, 0, 0, false, false);
                }
                else
                {
                    var nextBook = canon.GetBookFromAbsoluteChapter(book.NumberOfChapters + book.VersesInChapterStartIndex + 1);
                    this.MoveChapterVerse(nextBook.ShortName1, 0, 0, false, false);
                }
            }
            else
            {
                this.MoveChapterVerse(this.Serial.PosBookShortName, nextChapter, 0, false, false);
            }
        }

        public override void MovePrevious(bool isVerse)
        {
            int prevChapter = this.Serial.PosChaptNum - 1;
            var book = canon.BookByShortName[this.Serial.PosBookShortName];

            if (prevChapter < 0)
            {
                // must go to the previous book.
                if (book.VersesInChapterStartIndex == 0)
                {
                    // no more previous books left. Go to the last one.
                    book = canon.NewTestBooks.Any() ? canon.NewTestBooks[canon.NewTestBooks.Count() - 1] : canon.OldTestBooks[canon.OldTestBooks.Count() - 1];
                    prevChapter = book.NumberOfChapters - 1;
                }
            }


            var testMarker = new BiblePlaceMarker(book.ShortName1, prevChapter, 999, DateTime.Now);
            var sortedNotes = GetSortedList(string.Empty, 0, true, false);
            for (int i = sortedNotes.Count()-1; i >=0 ; i--)
            {
                var marker = sortedNotes[i];
                if (Highlighter.SortByBibleChapterVerse(testMarker, marker) > 0)
                {
                    this.Serial.PosBookShortName = marker.BookShortName;
                    this.Serial.PosChaptNum = marker.ChapterNum;
                    this.Serial.PosVerseNum = 0;
                    return;
                }
            }
            // If we are here we found nothing. go to the last
            var marker2 = sortedNotes.LastOrDefault();
            if (marker2 != null)
            {
                this.Serial.PosBookShortName = marker2.BookShortName;
                this.Serial.PosChaptNum = marker2.ChapterNum;
                this.Serial.PosVerseNum = 0;
            }

        }

        public override async Task Resume()
        {
            this.Serial.CloneFrom(this.Serial2);
            App.PersonalNotesChanged += this.AppPersonalNotesChanged;
            await base.Resume();
        }

        public override void SerialSave()
        {
            this.Serial2.CloneFrom(this.Serial);
        }

        #endregion

        #region Methods

        private List<BiblePlaceMarker> GetSortedList(string bookShortName, int chapterNumber, bool inludeAll, bool reverseOrder)
        {
            var returnList = new List<BiblePlaceMarker>();
            
            foreach (var book in App.DailyPlan.PersonalNotesVersified)
            {
                foreach (var chapter in book.Value)
                {
                    foreach (var verse in chapter.Value)
                    {
                        if(inludeAll || (book.Key.Equals(bookShortName) && chapter.Key.Equals(chapterNumber)))
                        {
                            var marker = new BiblePlaceMarker(book.Key,chapter.Key,verse.Key, DateTime.Now){Note = verse.Value.Note};
                            returnList.Add(marker);
                        }
                    }
                }
            }

            returnList.Sort(Highlighter.SortByBibleChapterVerse);
            if (reverseOrder)
            {
                returnList.Reverse();
            }

            return returnList;
        }

        private void MoveChapterVerse(string bookShortName, int chapter, int verse, bool isLocalLinkChange, bool forcePrevious)
        {
            if (IsPageable)
            {
                var testMarker = new BiblePlaceMarker(bookShortName, chapter, verse, DateTime.Now);
                var sortedNotes = GetSortedList(string.Empty, 0, true,false);
                foreach (var marker in sortedNotes)
                {
                    if (marker.BookShortName.Equals(bookShortName) && marker.ChapterNum.Equals(chapter))
                    {
                        this.Serial.PosBookShortName = bookShortName;
                        this.Serial.PosChaptNum = chapter;
                        this.Serial.PosVerseNum = 0;
                        return;
                    }
                    else if (Highlighter.SortByBibleChapterVerse(testMarker, marker) < 0)
                    {
                        this.Serial.PosBookShortName = marker.BookShortName;
                        this.Serial.PosChaptNum = marker.ChapterNum;
                        this.Serial.PosVerseNum = 0;
                        return;
                    }
                }
                // If we are here we found nothing. go to the first
                var marker2 = sortedNotes.FirstOrDefault();
                if(marker2!=null)
                {
                    this.Serial.PosBookShortName = marker2.BookShortName;
                    this.Serial.PosChaptNum = marker2.ChapterNum;
                    this.Serial.PosVerseNum = 0;
                }
            }
        }

        public override void SetToFirstChapter()
        {
            var book = canon.OldTestBooks.Any() ? canon.OldTestBooks[0] : canon.NewTestBooks[0];
            this.Serial.PosBookShortName = book.ShortName1;
        }

        #endregion
    }
}