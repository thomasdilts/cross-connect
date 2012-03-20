#region Header

// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PersonalNotesReader.cs" company="">
//
// </copyright>
// <summary>
//   Load from a file all the book and verse pointers to the bzz file so that
//   we can later read the bzz file quickly and efficiently.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
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

    using Sword.reader;

    /// <summary>
    /// Load from a file all the book and verse pointers to the bzz file so that
    ///   we can later read the bzz file quickly and efficiently.
    /// </summary>
    [DataContract(Name = "PersonalNotesReader")]
    [KnownType(typeof(ChapterPos))]
    [KnownType(typeof(BookPos))]
    [KnownType(typeof(VersePos))]
    public class PersonalNotesReader : BibleZtextReader
    {
        #region Fields

        /// <summary>
        /// The serial 2.
        /// </summary>
        [DataMember(Name = "serial2")]
        public BibleZtextReaderSerialData Serial2 = new BibleZtextReaderSerialData(false, string.Empty, string.Empty, 0, 0);

        /// <summary>
        /// The epsilon.
        /// </summary>
        private const double Epsilon = 0.00001;

        /// <summary>
        /// The limit for paging.
        /// </summary>
        private const int LimitForPaging = 60;

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
        /// Initializes a new instance of the <see cref="PersonalNotesReader"/> class.
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
        public PersonalNotesReader(string path, string iso2DigitLangCode, bool isIsoEncoding)
            : base(path, iso2DigitLangCode, isIsoEncoding)
        {
            Serial2.CloneFrom(Serial);
            App.PersonalNotesChanged += AppPersonalNotesChanged;
            SetToFirstChapter();
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="PersonalNotesReader"/> class. 
        /// </summary>
        ~PersonalNotesReader()
        {
            App.PersonalNotesChanged -= AppPersonalNotesChanged;
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
                int count = App.DailyPlan.PersonalNotes.Sum(dict => dict.Value.Count);
                return App.DisplaySettings.ShowAddedNotesByChapter || count > LimitForPaging;
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
                return true;
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
        /// The compare integers assending.
        /// </summary>
        /// <param name="a">
        /// The a.
        /// </param>
        /// <param name="b">
        /// The b.
        /// </param>
        /// <returns>
        /// The compare integers assending.
        /// </returns>
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

        /// <summary>
        /// The compare integers descending.
        /// </summary>
        /// <param name="a">
        /// The a.
        /// </param>
        /// <param name="b">
        /// The b.
        /// </param>
        /// <returns>
        /// The compare integers descending.
        /// </returns>
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

        /// <summary>
        /// The app personal notes changed.
        /// </summary>
        public void AppPersonalNotesChanged()
        {
            if (IsPageable)
            {
                // show just the one chapter.
                _displayText = MakeListDisplayText(
                    App.DisplaySettings,
                    GetSortedList(Serial.PosChaptNum),
                    _htmlBackgroundColor,
                    _htmlForegroundColor,
                    _htmlPhoneAccentColor,
                    _htmlFontSize,
                    _fontFamily,
                    false,
                    Translations.Translate("Added notes"));
            }
            else
            {
                // put it all into one window
                _displayText = MakeListDisplayText(
                    App.DisplaySettings,
                    GetSortedList(-1),
                    _htmlBackgroundColor,
                    _htmlForegroundColor,
                    _htmlPhoneAccentColor,
                    _htmlFontSize,
                    _fontFamily,
                    true,
                    Translations.Translate("Added notes"));
            }

            RaiseSourceChangedEvent();
        }

        /// <summary>
        /// The get button window specs.
        /// </summary>
        /// <param name="stage">
        /// The stage.
        /// </param>
        /// <param name="lastSelectedButton">
        /// The last selected button.
        /// </param>
        /// <returns>
        /// </returns>
        public override ButtonWindowSpecs GetButtonWindowSpecs(int stage, int lastSelectedButton)
        {
            if (stage == 0)
            {
                var books = new Dictionary<int, int>();
                int count = App.DailyPlan.PersonalNotes.Count;
                var butColors = new int[count];
                var values = new int[count];
                var butText = new string[count];

                var keys = new int[count];
                App.DailyPlan.PersonalNotes.Keys.CopyTo(keys, 0);
                var sortedKeys = new List<int>(keys);
                sortedKeys.Sort(CompareIntegersDescending);

                string[] buttonNamesStart = BookNames.GetAllShortNames();
                if (!BookNames.ExistsShortNames)
                {
                    buttonNamesStart = BookNames.GetAllFullNames();
                }

                for (int i = 0; i < count; i++)
                {
                    int bookNum = GetBookNumForChapterNum(sortedKeys[i]);
                    if (!books.ContainsKey(bookNum))
                    {
                        butColors[books.Count] = ChapterCategories[bookNum];
                        butText[books.Count] = buttonNamesStart[bookNum];
                        values[books.Count] = bookNum;

                        books.Add(bookNum, bookNum);
                    }
                }

                return new ButtonWindowSpecs(
                    stage,
                    "Select book",
                    books.Count,
                    butColors,
                    butText,
                    values,
                    !BookNames.ExistsShortNames ? ButtonSize.Large : ButtonSize.Medium);
            }
            else
            {
                int count = App.DailyPlan.PersonalNotes.Count;
                var butColors = new int[count];
                var values = new int[count];
                var butText = new string[count];

                var keys = new int[count];
                App.DailyPlan.PersonalNotes.Keys.CopyTo(keys, 0);
                var sortedKeys = new List<int>(keys);
                sortedKeys.Sort(CompareIntegersDescending);
                int chapterCount = 0;
                for (int i = 0; i < count; i++)
                {
                    int bookNum = GetBookNumForChapterNum(sortedKeys[i]);
                    if (lastSelectedButton == bookNum)
                    {
                        butColors[chapterCount] = 0;
                        butText[chapterCount] = (sortedKeys[i] - FirstChapternumInBook[bookNum] + 1).ToString();
                        values[chapterCount] = sortedKeys[i] - lastSelectedButton;
                        chapterCount++;
                    }
                }

                return new ButtonWindowSpecs(
                    stage, "Select chapter", chapterCount, butColors, butText, values, ButtonSize.Small);
            }
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
            base.GetInfo(out bookNum, out absoluteChaptNum, out relChaptNum, out verseNum, out fullName, out title);
            title = Translations.Translate("Added notes");
        }

        /// <summary>
        /// The move chapter verse.
        /// </summary>
        /// <param name="chapter">
        /// The chapter.
        /// </param>
        /// <param name="verse">
        /// The verse.
        /// </param>
        /// <param name="isLocalLinkChange">
        /// The is local link change.
        /// </param>
        public override void MoveChapterVerse(int chapter, int verse, bool isLocalLinkChange)
        {
            MoveChapterVerse(chapter, verse, isLocalLinkChange, false);
        }

        /// <summary>
        /// The move next.
        /// </summary>
        public override void MoveNext()
        {
            int nextChapter = Serial.PosChaptNum + 1;
            if (nextChapter >= ChaptersInBible)
            {
                nextChapter = 0;
            }

            MoveChapterVerse(nextChapter, 0, false, false);
        }

        /// <summary>
        /// The move previous.
        /// </summary>
        public override void MovePrevious()
        {
            int prevChapter = Serial.PosChaptNum - 1;
            if (prevChapter < 0)
            {
                prevChapter = ChaptersInBible;
            }

            MoveChapterVerse(prevChapter, 0, false, true);
        }

        /// <summary>
        /// The resume.
        /// </summary>
        public override void Resume()
        {
            Serial.CloneFrom(Serial2);
            App.PersonalNotesChanged += AppPersonalNotesChanged;
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
            bool isPageable = IsPageable;
            if (forceReload || isPageable || mustUpdate || Math.Abs(_htmlFontSize - htmlFontSize) > Epsilon)
            {
                if (isPageable)
                {
                    // show just the one chapter.
                    _displayText = MakeListDisplayText(
                        displaySettings,
                        GetSortedList(Serial.PosChaptNum),
                        htmlBackgroundColor,
                        htmlForegroundColor,
                        htmlPhoneAccentColor,
                        htmlFontSize,
                        fontFamily,
                        false,
                        Translations.Translate("Added notes"));
                }
                else
                {
                    // put it all into one window
                    _displayText = MakeListDisplayText(
                        displaySettings,
                        GetSortedList(-1),
                        htmlBackgroundColor,
                        htmlForegroundColor,
                        htmlPhoneAccentColor,
                        htmlFontSize,
                        _fontFamily,
                        true,
                        Translations.Translate("Added notes"));
                }
            }

            _htmlFontSize = htmlFontSize;
            return _displayText;
        }

        /// <summary>
        /// The get book num for chapter num.
        /// </summary>
        /// <param name="chapterNum">
        /// The chapter num.
        /// </param>
        /// <returns>
        /// The get book num for chapter num.
        /// </returns>
        private static int GetBookNumForChapterNum(int chapterNum)
        {
            for (int i = 0; i < FirstChapternumInBook.Length; i++)
            {
                if (FirstChapternumInBook[i] > chapterNum)
                {
                    return i - 1;
                }
            }

            return BooksInBible - 1;
        }

        /// <summary>
        /// The get sorted list.
        /// </summary>
        /// <param name="chapterNumber">
        /// The chapter number.
        /// </param>
        /// <returns>
        /// </returns>
        private static List<BiblePlaceMarker> GetSortedList(int chapterNumber)
        {
            var returnList = new List<BiblePlaceMarker>();

            int[] keys;
            if (chapterNumber >= 0)
            {
                keys = new int[1];
                keys[0] = chapterNumber;
            }
            else
            {
                keys = new int[App.DailyPlan.PersonalNotes.Count];
                App.DailyPlan.PersonalNotes.Keys.CopyTo(keys, 0);
            }

            var sortedKeys = new List<int>(keys);
            sortedKeys.Sort(CompareIntegersAssending);
            foreach (int key in sortedKeys)
            {
                if (App.DailyPlan.PersonalNotes.ContainsKey(key))
                {
                    Dictionary<int, BiblePlaceMarker> dict = App.DailyPlan.PersonalNotes[key];
                    keys = new int[dict.Count];
                    dict.Keys.CopyTo(keys, 0);
                    var sortedVerses = new List<int>(keys);
                    sortedVerses.Sort(CompareIntegersAssending);
                    returnList.AddRange(sortedVerses.Select(verse => dict[verse]));
                }
            }

            return returnList;
        }

        /// <summary>
        /// The move chapter verse.
        /// </summary>
        /// <param name="chapter">
        /// The chapter.
        /// </param>
        /// <param name="verse">
        /// The verse.
        /// </param>
        /// <param name="isLocalLinkChange">
        /// The is local link change.
        /// </param>
        /// <param name="forcePrevious">
        /// The force previous.
        /// </param>
        private void MoveChapterVerse(int chapter, int verse, bool isLocalLinkChange, bool forcePrevious)
        {
            int count = App.DailyPlan.PersonalNotes.Count;
            if (count > 0)
            {
                var keys = new int[count];
                App.DailyPlan.PersonalNotes.Keys.CopyTo(keys, 0);
                var sortedKeys = new List<int>(keys);
                sortedKeys.Sort(CompareIntegersDescending);
                Serial.PosChaptNum = sortedKeys[0];
                Serial.PosVerseNum = verse;
                int i;
                for (i = 0; i < count; i++)
                {
                    if (sortedKeys[i] == chapter)
                    {
                        Serial.PosChaptNum = sortedKeys[i];
                        break;
                    }

                    if (sortedKeys[i] > chapter)
                    {
                        if (forcePrevious)
                        {
                            if (i > 0)
                            {
                                Serial.PosChaptNum = sortedKeys[i - 1];
                            }
                        }
                        else
                        {
                            Serial.PosChaptNum = sortedKeys[i];
                        }

                        break;
                    }
                }

                if (i == count && count > 1 && forcePrevious)
                {
                    // we must get the previous which would be the last
                    Serial.PosChaptNum = sortedKeys[count - 1];
                }
            }
        }

        /// <summary>
        /// The set to first chapter.
        /// </summary>
        private void SetToFirstChapter()
        {
            MoveChapterVerse(0, 0, false, false);
        }

        #endregion Methods
    }
}