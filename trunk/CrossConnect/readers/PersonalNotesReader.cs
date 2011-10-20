#region Header

// <copyright file="PersonalNotesReader.cs" company="Thomas Dilts">
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
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;

    using Sword.reader;

    /// <summary>
    ///   Load from a file all the book and verse pointers to the bzz file so that
    ///   we can later read the bzz file quickly and efficiently.
    /// </summary>
    [DataContract(Name = "PersonalNotesReader")]
    [KnownType(typeof (ChapterPos))]
    [KnownType(typeof (BookPos))]
    [KnownType(typeof (VersePos))]
    public class PersonalNotesReader : BibleZtextReader
    {
        #region Fields

        [DataMember]
        public BibleZtextReaderSerialData Serial2 = new BibleZtextReaderSerialData(false, "", "", 0, 0);

        private const double Epsilon = 0.00001;
        private const int LimitForPaging = 60;

        private string _displayText = string.Empty;
        private string _htmlBackgroundColor = string.Empty;
        private double _htmlFontSize;
        private string _htmlForegroundColor = string.Empty;
        private string _htmlPhoneAccentColor = string.Empty;

        #endregion Fields

        #region Constructors

        public PersonalNotesReader(string path, string iso2DigitLangCode, bool isIsoEncoding)
            : base(path, iso2DigitLangCode, isIsoEncoding)
        {
            Serial2.CloneFrom(Serial);
            App.PersonalNotesChanged += AppPersonalNotesChanged;
            SetToFirstChapter();
        }

        ~PersonalNotesReader()
        {
            App.PersonalNotesChanged -= AppPersonalNotesChanged;
        }

        #endregion Constructors

        #region Properties

        public override bool IsHearable
        {
            get { return false; }
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
            get { return false; }
        }

        public override bool IsSynchronizeable
        {
            get { return true; }
        }

        public override bool IsTranslateable
        {
            get { return false; }
        }

        #endregion Properties

        #region Methods

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

        public void AppPersonalNotesChanged()
        {
            if (IsPageable)
            {
                //show just the one chapter.
                _displayText = MakeListDisplayText(App.DisplaySettings, getSortedList(Serial.PosChaptNum),
                                                  _htmlBackgroundColor, _htmlForegroundColor, _htmlPhoneAccentColor,
                                                  _htmlFontSize, false, Translations.Translate("Added notes"));
            }
            else
            {
                //put it all into one window
                _displayText = MakeListDisplayText(App.DisplaySettings, getSortedList(-1), _htmlBackgroundColor,
                                                  _htmlForegroundColor, _htmlPhoneAccentColor, _htmlFontSize, true,
                                                  Translations.Translate("Added notes"));
            }
            RaiseSourceChangedEvent();
        }

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

                var buttonNamesStart = BookNames.GetAllShortNames();
                if (!BookNames.ExistsShortNames)
                {
                    buttonNamesStart = BookNames.GetAllFullNames();
                }

                for (int i = 0; i < count; i++)
                {
                    int bookNum = getBookNumForChapterNum(sortedKeys[i]);
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
                    int bookNum = getBookNumForChapterNum(sortedKeys[i]);
                    if (lastSelectedButton == bookNum)
                    {
                        butColors[chapterCount] = 0;
                        butText[chapterCount] = (sortedKeys[i] - FirstChapternumInBook[bookNum] + 1).ToString();
                        values[chapterCount] = sortedKeys[i] - lastSelectedButton;
                        chapterCount++;
                    }
                }
                return new ButtonWindowSpecs(
                    stage,
                    "Select chapter",
                    chapterCount,
                    butColors,
                    butText,
                    values,
                    ButtonSize.Small);
            }
        }

        public override void GetInfo(out int bookNum, out int absoluteChaptNum, out int relChaptNum, out int verseNum,
            out string fullName, out string title)
        {
            base.GetInfo(out bookNum, out absoluteChaptNum, out relChaptNum, out verseNum, out fullName, out title);
            title = Translations.Translate("Added notes");
        }

        public override void MoveChapterVerse(int chapter, int verse, bool isLocalLinkChange)
        {
            MoveChapterVerse(chapter, verse, isLocalLinkChange, false);
        }

        public override void MoveNext()
        {
            int nextChapter = Serial.PosChaptNum + 1;
            if (nextChapter >= ChaptersInBible)
            {
                nextChapter = 0;
            }
            MoveChapterVerse(nextChapter, 0, false, false);
        }

        public override void MovePrevious()
        {
            int prevChapter = Serial.PosChaptNum - 1;
            if (prevChapter < 0)
            {
                prevChapter = ChaptersInBible;
            }
            MoveChapterVerse(prevChapter, 0, false, true);
        }

        public override void Resume()
        {
            Serial.CloneFrom(Serial2);
            App.PersonalNotesChanged += AppPersonalNotesChanged;
            base.Resume();
        }

        public override void SerialSave()
        {
            Serial2.CloneFrom(Serial);
        }

        protected override string GetChapterHtml(DisplaySettings displaySettings, string htmlBackgroundColor,
            string htmlForegroundColor, string htmlPhoneAccentColor,
            double htmlFontSize, bool isNotesOnly, bool addStartFinishHtml = true)
        {
            bool mustUpdate = string.IsNullOrEmpty(_htmlBackgroundColor);
            _htmlBackgroundColor = htmlBackgroundColor;
            _htmlForegroundColor = htmlForegroundColor;
            _htmlPhoneAccentColor = htmlPhoneAccentColor;
            bool isPageable = IsPageable;
            if (isPageable || mustUpdate || Math.Abs(_htmlFontSize - htmlFontSize) > Epsilon)
            {
                if (isPageable)
                {
                    //show just the one chapter.
                    _displayText = MakeListDisplayText(displaySettings, getSortedList(Serial.PosChaptNum),
                                                      htmlBackgroundColor, htmlForegroundColor, htmlPhoneAccentColor,
                                                      htmlFontSize, false, Translations.Translate("Added notes"));
                }
                else
                {
                    //put it all into one window
                    _displayText = MakeListDisplayText(displaySettings, getSortedList(-1), htmlBackgroundColor,
                                                      htmlForegroundColor, htmlPhoneAccentColor, htmlFontSize, true,
                                                      Translations.Translate("Added notes"));
                }
            }
            _htmlFontSize = htmlFontSize;
            return _displayText;
        }

        private int getBookNumForChapterNum(int chapterNum)
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

        private List<BiblePlaceMarker> getSortedList(int chapterNumber)
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
                    var dict = App.DailyPlan.PersonalNotes[key];
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
                    //we must get the previous which would be the last
                    Serial.PosChaptNum = sortedKeys[count - 1];
                }
            }
        }

        private void SetToFirstChapter()
        {
            MoveChapterVerse(0, 0, false, false);
        }

        #endregion Methods
    }
}