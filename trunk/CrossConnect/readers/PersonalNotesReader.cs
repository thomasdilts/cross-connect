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
/// <copyright file="PersonalNotesReader.cs" company="Thomas Dilts">
///     Thomas Dilts. All rights reserved.
/// </copyright>
/// <author>Thomas Dilts</author>
namespace CrossConnect.readers
{
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    using SwordBackend;

    /// <summary>
    /// Load from a file all the book and verse pointers to the bzz file so that
    /// we can later read the bzz file quickly and efficiently.
    /// </summary>
    /// <param name="path">The path to where the ot.bzs,ot.bzv and ot.bzz and nt files are</param>
    [DataContract(Name = "PersonalNotesReader")]
    [KnownType(typeof(ChapterPos))]
    [KnownType(typeof(BookPos))]
    [KnownType(typeof(VersePos))]
    public class PersonalNotesReader : BibleZtextReader
    {
        #region Fields

        [DataMember]
        public BibleZtextReaderSerialData serial2 = new BibleZtextReaderSerialData(false, "", "", 0, 0);

        private const int LIMIT_FOR_PAGING = 60;

        private string displayText = string.Empty;
        private string htmlBackgroundColor = string.Empty;
        private double htmlFontSize = 0;
        private string htmlForegroundColor = string.Empty;
        private string htmlPhoneAccentColor = string.Empty;

        #endregion Fields

        #region Constructors

        public PersonalNotesReader(string path, string iso2DigitLangCode, bool isIsoEncoding)
            : base(path, iso2DigitLangCode, isIsoEncoding)
        {
            serial2.cloneFrom(base.serial);
            App.PersonalNotesChanged += this.App_PersonalNotesChanged;
        }

        ~PersonalNotesReader()
        {
            App.PersonalNotesChanged -= this.App_PersonalNotesChanged;
        }

        #endregion Constructors

        #region Properties

        public override bool IsPageable
        {
            get
            {
                int count=0;
                foreach (var dict in App.dailyPlan.personalNotes)
                {
                    count += dict.Value.Count;
                }
                return App.displaySettings.showAddedNotesByChapter || count > LIMIT_FOR_PAGING;
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

        #endregion Properties

        #region Methods

        public static int CompareIntegersAssending(int a, int b)
        {
            if (a == b)
            {
                return 0;
            }
            else if (a < b)
            {
                return 1;
            }
            else
            {
                return -1;
            }
        }

        public static int CompareIntegersDescending(int a, int b)
        {
            if (a == b)
            {
                return 0;
            }
            else if (a > b)
            {
                return 1;
            }
            else
            {
                return -1;
            }
        }

        public void App_PersonalNotesChanged()
        {
            if (IsPageable)
            {
                //show just the one chapter.
                this.displayText = MakeListDisplayText(App.displaySettings, getSortedList(base.serial.posChaptNum), htmlBackgroundColor, htmlForegroundColor, htmlPhoneAccentColor, htmlFontSize, false, Translations.translate("Added notes"));
            }
            else
            {
                //put it all into one window
                this.displayText = MakeListDisplayText(App.displaySettings, getSortedList(-1), htmlBackgroundColor, htmlForegroundColor, htmlPhoneAccentColor, htmlFontSize, true, Translations.translate("Added notes"));
            }
            raiseSourceChangedEvent();
        }

        public override ButtonWindowSpecs GetButtonWindowSpecs(int stage, int lastSelectedButton)
        {
            if (stage == 0)
            {
                Dictionary<int, int> books = new Dictionary<int, int>();
                int count = App.dailyPlan.personalNotes.Count;
                int[] butColors = new int[count];
                int[] values = new int[count];
                string[] butText = new string[count];

                int[] keys = new int[count];
                App.dailyPlan.personalNotes.Keys.CopyTo(keys, 0);
                List<int> sortedKeys = new List<int>(keys);
                sortedKeys.Sort(CompareIntegersDescending);

                string[] buttonNamesStart = bookNames.getAllShortNames();
                if (!bookNames.existsShortNames)
                {
                    buttonNamesStart = bookNames.getAllFullNames();
                }

                for (int i = 0; i < count; i++)
                {
                    int bookNum = getBookNumForChapterNum(sortedKeys[i]);
                    if (!books.ContainsKey(bookNum))
                    {
                        butColors[books.Count] = CHAPTER_CATEGORIES[bookNum];
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
                    !bookNames.existsShortNames ? ButtonSize.LARGE : ButtonSize.MEDIUM);
            }
            else
            {
                int count = App.dailyPlan.personalNotes.Count;
                int[] butColors = new int[count];
                int[] values = new int[count];
                string[] butText = new string[count];

                int[] keys = new int[count];
                App.dailyPlan.personalNotes.Keys.CopyTo(keys, 0);
                List<int> sortedKeys = new List<int>(keys);
                sortedKeys.Sort(CompareIntegersDescending);
                int chapterCount = 0;
                for (int i = 0; i < count; i++)
                {
                    int bookNum = getBookNumForChapterNum(sortedKeys[i]);
                    if (lastSelectedButton == bookNum)
                    {
                        butColors[chapterCount] = 0;
                        butText[chapterCount] = (sortedKeys[i] - FIRST_CHAPTERNUM_IN_BOOK[bookNum] + 1).ToString();
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
                    ButtonSize.SMALL);
            }
        }

        public override void GetInfo(out int bookNum, out int absoluteChaptNum, out int relChaptNum, out int verseNum, out string fullName, out string title)
        {
            base.GetInfo(out bookNum, out absoluteChaptNum, out relChaptNum, out verseNum, out fullName, out title);
            title = Translations.translate("Added notes");
        }

        public override void moveChapterVerse(int chapter, int verse, bool isLocalLinkChange)
        {
            moveChapterVerse(chapter, verse, isLocalLinkChange, false);
        }

        public override void moveNext()
        {
            int nextChapter = base.serial.posChaptNum + 1;
            if (nextChapter >= CHAPTERS_IN_BIBLE)
            {
                nextChapter = 0;
            }
            moveChapterVerse(nextChapter, 0, false, false);
        }

        public override void movePrevious()
        {
            int prevChapter=base.serial.posChaptNum - 1;
            if (prevChapter < 0)
            {
                prevChapter = CHAPTERS_IN_BIBLE;
            }
            moveChapterVerse(prevChapter, 0, false, true);
        }

        public override void Resume()
        {
            base.serial.cloneFrom(this.serial2);
            App.PersonalNotesChanged += this.App_PersonalNotesChanged;
            base.Resume();
        }

        public override void SerialSave()
        {
            this.serial2.cloneFrom(base.serial);
        }

        public override void SetToFirstChapter()
        {
            moveChapterVerse(0, 0, false, false);
        }

        protected override string GetChapterHtml(DisplaySettings displaySettings, string htmlBackgroundColor, string htmlForegroundColor, string htmlPhoneAccentColor, double htmlFontSize,bool isNotesOnly, bool addStartFinishHtml=true)
        {
            bool mustUpdate = string.IsNullOrEmpty(this.htmlBackgroundColor);
            this.htmlBackgroundColor = htmlBackgroundColor;
            this.htmlForegroundColor = htmlForegroundColor;
            this.htmlPhoneAccentColor = htmlPhoneAccentColor;
            bool isPageable = IsPageable;
            if (isPageable || mustUpdate || this.htmlFontSize != htmlFontSize)
            {
                if (isPageable)
                {
                    //show just the one chapter.
                    this.displayText = MakeListDisplayText(displaySettings, getSortedList(base.serial.posChaptNum), htmlBackgroundColor, htmlForegroundColor, htmlPhoneAccentColor, htmlFontSize, false, Translations.translate("Added notes"));
                }
                else
                {
                    //put it all into one window
                    this.displayText = MakeListDisplayText(displaySettings, getSortedList(-1), htmlBackgroundColor, htmlForegroundColor, htmlPhoneAccentColor, htmlFontSize, true, Translations.translate("Added notes"));
                }

            }
            this.htmlFontSize = htmlFontSize;
            return this.displayText;
        }

        private int getBookNumForChapterNum(int chapterNum)
        {
            for (int i = 0; i < FIRST_CHAPTERNUM_IN_BOOK.Length; i++)
            {
                if (FIRST_CHAPTERNUM_IN_BOOK[i] > chapterNum)
                {
                    return i - 1;
                }
            }
            return BOOKS_IN_BIBLE-1;
        }

        private List<BiblePlaceMarker> getSortedList(int chapterNumber)
        {
            List<BiblePlaceMarker> returnList = new List<BiblePlaceMarker>();

            int[] keys = null;
            if (chapterNumber >= 0)
            {
                keys = new int[1];
                keys[0] = chapterNumber;
            }
            else
            {
                keys = new int[App.dailyPlan.personalNotes.Count];
                App.dailyPlan.personalNotes.Keys.CopyTo(keys, 0);
            }
            List<int> sortedKeys = new List<int>(keys);
            sortedKeys.Sort(CompareIntegersAssending);
            foreach (int key in sortedKeys)
            {
                if (App.dailyPlan.personalNotes.ContainsKey(key))
                {
                    var dict = App.dailyPlan.personalNotes[key];
                    keys = new int[dict.Count];
                    dict.Keys.CopyTo(keys, 0);
                    List<int> sortedVerses = new List<int>(keys);
                    sortedVerses.Sort(CompareIntegersAssending);
                    foreach (int verse in sortedVerses)
                    {
                        returnList.Add(dict[verse]);
                    }
                }
            }
            return returnList;
        }

        private void moveChapterVerse(int chapter, int verse, bool isLocalLinkChange, bool forcePrevious)
        {
            int count = App.dailyPlan.personalNotes.Count;
            if (count > 0)
            {
                int[] keys = new int[count];
                App.dailyPlan.personalNotes.Keys.CopyTo(keys, 0);
                List<int> sortedKeys = new List<int>(keys);
                sortedKeys.Sort(CompareIntegersDescending);
                base.serial.posChaptNum = sortedKeys[0];
                base.serial.posVerseNum = verse;
                int i = 0;
                for (i = 0; i < count; i++)
                {
                    if (sortedKeys[i] == chapter)
                    {
                        base.serial.posChaptNum = sortedKeys[i];
                        break;
                    }
                    else if (sortedKeys[i] > chapter)
                    {
                        if (forcePrevious)
                        {
                            if (i > 0)
                            {
                                base.serial.posChaptNum = sortedKeys[i-1];
                            }
                        }
                        else
                        {
                            base.serial.posChaptNum = sortedKeys[i];
                        }
                        break;
                    }
                }
                if (i == count && count > 1 && forcePrevious)
                {
                    //we must get the previous which would be the last
                    base.serial.posChaptNum = sortedKeys[count - 1];
                }
            }
        }

        #endregion Methods
    }
}