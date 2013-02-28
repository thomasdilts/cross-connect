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
            false, string.Empty, string.Empty, 0, 0);

        private string _displayText = string.Empty;

        private string _fontFamily = string.Empty;

        private HtmlColorRgba _htmlBackgroundColor;

        private double _htmlFontSize;

        private HtmlColorRgba _htmlForegroundColor;

        private HtmlColorRgba _htmlPhoneAccentColor;

        #endregion

        #region Constructors and Destructors

        public PersonalNotesReader(string path, string iso2DigitLangCode, bool isIsoEncoding)
            : base(path, iso2DigitLangCode, isIsoEncoding)
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
                int count = App.DailyPlan.PersonalNotes.Sum(dict => dict.Value.Count);
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
                // show just the one chapter.
                this._displayText =
                    await
                    this.MakeListDisplayText(
                        App.DisplaySettings,
                        GetSortedList(this.Serial.PosChaptNum),
                        this._htmlBackgroundColor,
                        this._htmlForegroundColor,
                        this._htmlPhoneAccentColor,
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
                        App.DisplaySettings,
                        GetSortedList(-1),
                        this._htmlBackgroundColor,
                        this._htmlForegroundColor,
                        this._htmlPhoneAccentColor,
                        this._htmlFontSize,
                        this._fontFamily,
                        true,
                        Translations.Translate("Added notes"));
            }

            this.RaiseSourceChangedEvent();
        }

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

                string[] buttonNamesStart = this.BookNames.GetAllShortNames();
                if (!this.BookNames.ExistsShortNames)
                {
                    buttonNamesStart = this.BookNames.GetAllFullNames();
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
                    "Select a book to view",
                    books.Count,
                    butColors,
                    butText,
                    values,
                    !this.BookNames.ExistsShortNames ? ButtonSize.Large : ButtonSize.Medium);
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
                    stage, "Select a chapter to view", chapterCount, butColors, butText, values, ButtonSize.Small);
            }
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
            bool isPageable = this.IsPageable;
            if (forceReload || isPageable || mustUpdate || Math.Abs(this._htmlFontSize - htmlFontSize) > Epsilon)
            {
                if (isPageable)
                {
                    // show just the one chapter.
                    this._displayText =
                        await
                        this.MakeListDisplayText(
                            displaySettings,
                            GetSortedList(this.Serial.PosChaptNum),
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
                    this._displayText =
                        await
                        this.MakeListDisplayText(
                            displaySettings,
                            GetSortedList(-1),
                            htmlBackgroundColor,
                            htmlForegroundColor,
                            htmlPhoneAccentColor,
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

        public override void MoveChapterVerse(int chapter, int verse, bool isLocalLinkChange, IBrowserTextSource source)
        {
            if (!(source is BibleZtextReader))
            {
                return;
            } 
            
            this.MoveChapterVerse(chapter, verse, isLocalLinkChange, false);
        }

        public override void MoveNext()
        {
            int nextChapter = this.Serial.PosChaptNum + 1;
            if (nextChapter >= ChaptersInBible)
            {
                nextChapter = 0;
            }

            this.MoveChapterVerse(nextChapter, 0, false, false);
        }

        public override void MovePrevious()
        {
            int prevChapter = this.Serial.PosChaptNum - 1;
            if (prevChapter < 0)
            {
                prevChapter = ChaptersInBible;
            }

            this.MoveChapterVerse(prevChapter, 0, false, true);
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

        private void MoveChapterVerse(int chapter, int verse, bool isLocalLinkChange, bool forcePrevious)
        {
            int count = App.DailyPlan.PersonalNotes.Count;
            if (count > 0)
            {
                var keys = new int[count];
                App.DailyPlan.PersonalNotes.Keys.CopyTo(keys, 0);
                var sortedKeys = new List<int>(keys);
                sortedKeys.Sort(CompareIntegersDescending);
                this.Serial.PosChaptNum = sortedKeys[0];
                this.Serial.PosVerseNum = verse;
                int i;
                for (i = 0; i < count; i++)
                {
                    if (sortedKeys[i] == chapter)
                    {
                        this.Serial.PosChaptNum = sortedKeys[i];
                        break;
                    }

                    if (sortedKeys[i] > chapter)
                    {
                        if (forcePrevious)
                        {
                            if (i > 0)
                            {
                                this.Serial.PosChaptNum = sortedKeys[i - 1];
                            }
                        }
                        else
                        {
                            this.Serial.PosChaptNum = sortedKeys[i];
                        }

                        break;
                    }
                }

                if (i == count && count > 1 && forcePrevious)
                {
                    // we must get the previous which would be the last
                    this.Serial.PosChaptNum = sortedKeys[count - 1];
                }
            }
        }

        public override void SetToFirstChapter()
        {
            this.MoveChapterVerse(0, 0, false, false);
        }

        #endregion
    }
}