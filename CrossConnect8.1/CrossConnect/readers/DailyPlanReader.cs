#region Header

// <copyright file="DailyPlanReader.cs" company="Thomas Dilts">
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
    using System.Globalization;
    using System.Runtime.Serialization;
    using System.Text;
    using System.Threading.Tasks;

    using Sword;
    using Sword.reader;
    using Sword.versification;

    /// <summary>
    ///     Load from a file all the book and verse pointers to the bzz file so that
    ///     we can later read the bzz file quickly and efficiently.
    /// </summary>
    [DataContract]
    public class DailyPlanReader : BibleZtextReader
    {
        #region Fields

        [DataMember(Name = "serial2")]
        public BibleZtextReaderSerialData Serial2 = new BibleZtextReaderSerialData(
            false, string.Empty, string.Empty, 0, 0, string.Empty, string.Empty);

        #endregion

        #region Constructors and Destructors

        public DailyPlanReader(string path, string iso2DigitLangCode, bool isIsoEncoding, string cipherKey, string configPath, string versification)
            : base(path, iso2DigitLangCode, isIsoEncoding, cipherKey, configPath, versification)
        {
            this.Serial2.CloneFrom(this.Serial);
            this.SetToFirstChapter();
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
                return true;
            }
        }

        public override bool IsLocalChangeDuringLink
        {
            get
            {
                return true;
            }
        }

        public override bool IsPageable
        {
            get
            {
                return true;
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
                return true;
            }
        }

        #endregion

        #region Public Methods and Operators

        public override ButtonWindowSpecs GetButtonWindowSpecs(int stage, int lastSelectedButton, string isoLangCode)
        {
            if (stage == 0)
            {
                var schedule = DailyPlans.ZAllPlans(App.DailyPlan.PlanNumber);
                int count = schedule.GetUpperBound(0) + 1;

                var butColors = new int[count];
                var values = new int[count];
                var butText = new string[count];
                for (int i = 0; i < count; i++)
                {
                    butColors[i] = 0;
                    butText[i] = (i + 1).ToString(CultureInfo.InvariantCulture);
                    values[i] = i;
                }

                // do a nice transition
                return new ButtonWindowSpecs(stage, "Select day", count, butColors, butText, values, ButtonSize.Small);
            }

            return null;
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
            var canonKjv = CanonManager.GetCanon("KJV");
            string chapterStartHtml = HtmlHeader(
                displaySettings,
                htmlBackgroundColor,
                htmlForegroundColor,
                htmlPhoneAccentColor,
                htmlWordsOfChristColor,
                htmlFontSize,
                fontFamily,
                this.Serial.Iso2DigitLangCode);
            var schedule = DailyPlans.ZAllPlans(App.DailyPlan.PlanNumber);
            const string ChapterEndHtml = "</body></html>";
            var sb = new StringBuilder(chapterStartHtml);
            var bookStart = canonKjv.GetBookFromAbsoluteChapter(schedule[App.DailyPlan.PlanDayNumber][0]);
            string firstVerseForTheDayRedirect = "<a name=\"" + bookStart.ShortName1 + "_"
                                                 + (schedule[App.DailyPlan.PlanDayNumber][0] - bookStart.VersesInChapterStartIndex) + "_0" + "\"></a>";
            sb.Append(
                firstVerseForTheDayRedirect + "<h3>" + Translations.Translate("Day") + " "
                + (App.DailyPlan.PlanDayNumber + 1) + ", "
                + App.DailyPlan.PlanStartDate.AddDays(App.DailyPlan.PlanDayNumber).ToString("yyyy-MM-dd") + "</h3>");
            sb.Append(
                "<h3>" + Translations.Translate(DailyPlans.ZzAllPlansNames[App.DailyPlan.PlanNumber][0]) + "; "
                + DailyPlans.ZzAllPlansNames[App.DailyPlan.PlanNumber][1] + " " + Translations.Translate("Days")
                + "</h3>");
            for (int i = 0;
                 i <= schedule[App.DailyPlan.PlanDayNumber].GetUpperBound(0);
                 i++)
            {
                int bookNum;
                int relChaptNum;
                string fullName;
                string title;
                var book = canonKjv.GetBookFromAbsoluteChapter(schedule[App.DailyPlan.PlanDayNumber][i]);
                this.GetInfo(
                    isoLangCode,
                    book.ShortName1,
                    schedule[App.DailyPlan.PlanDayNumber][i]-book.VersesInChapterStartIndex,
                    0,
                    out fullName,
                    out title);

                sb.Append("<h2>" + fullName + " " + (schedule[App.DailyPlan.PlanDayNumber][i] - book.VersesInChapterStartIndex + 1) + "</h2>");
                sb.Append(
                    await
                    this.GetChapterHtml(
                        isoLangCode,
                        displaySettings,
                        book.ShortName1,
                        schedule[App.DailyPlan.PlanDayNumber][i]-book.VersesInChapterStartIndex,
                        htmlBackgroundColor,
                        htmlForegroundColor,
                        htmlPhoneAccentColor,
                        htmlWordsOfChristColor,
                        htmlHighlightColor,
                        htmlFontSize,
                        fontFamily,
                        false,
                        false,
                        forceReload));
            }

            sb.Append(ChapterEndHtml);
            return sb.ToString();
        }

        public override void GetInfo(
            string isoLangCode,
            out string bookShortName,
            out int relChaptNum,
            out int verseNum,
            out string fullName,
            out string title)
        {
            var schedule = DailyPlans.ZAllPlans(App.DailyPlan.PlanNumber);
            int count = schedule.GetUpperBound(0) + 1;
            if (App.DailyPlan.PlanDayNumber >= count)
            {
                App.DailyPlan.PlanDayNumber = 0;
            }

            base.GetInfo(isoLangCode, out bookShortName, out relChaptNum, out verseNum, out fullName, out title);
            title = App.DailyPlan.PlanStartDate.AddDays(App.DailyPlan.PlanDayNumber).ToString("yyyy-MM-dd") + " "
                    + Translations.Translate("Daily plan");
        }

        public override bool MoveChapterVerse(string bookShortName, int chapter, int verse, bool isLocalLinkChange, IBrowserTextSource source)
        {
            if (!(source is BibleZtextReader) || !string.IsNullOrEmpty(bookShortName))
            {
                return false;
            }
            if (isLocalLinkChange)
            {
                this.Serial.PosBookShortName = bookShortName;
                this.Serial.PosChaptNum = chapter;
                this.Serial.PosVerseNum = verse;
            }
            else
            {
                App.DailyPlan.PlanDayNumber = chapter;
                var schedule = DailyPlans.ZAllPlans(App.DailyPlan.PlanNumber);
                this.Serial.PosChaptNum =
                    schedule[App.DailyPlan.PlanDayNumber][0];
                this.Serial.PosVerseNum = 0;
            }
            return true;
        }

        public override void MoveNext(bool isVerse)
        {
            this.Serial.PosVerseNum = 0;
            var schedule = DailyPlans.ZAllPlans(App.DailyPlan.PlanNumber);
            int count = schedule.GetUpperBound(0) + 1;
            App.DailyPlan.PlanDayNumber++;
            if (App.DailyPlan.PlanDayNumber >= count)
            {
                App.DailyPlan.PlanDayNumber = 0;
            }

            this.Serial.PosChaptNum = schedule[App.DailyPlan.PlanDayNumber][0];
        }

        public override void MovePrevious(bool isVerse)
        {
            var schedule = DailyPlans.ZAllPlans(App.DailyPlan.PlanNumber);
            this.Serial.PosVerseNum = 0;
            int count = schedule.GetUpperBound(0) + 1;
            App.DailyPlan.PlanDayNumber--;
            if (App.DailyPlan.PlanDayNumber < 0)
            {
                App.DailyPlan.PlanDayNumber = count - 1;
            }

            this.Serial.PosChaptNum = schedule[App.DailyPlan.PlanDayNumber][0];
        }

        public override async Task Resume()
        {
            this.Serial.CloneFrom(this.Serial2);
            await base.Resume();
        }

        public override void SerialSave()
        {
            this.Serial2.CloneFrom(this.Serial);
        }

        #endregion

        #region Methods

        public override void SetToFirstChapter()
        {
            var schedule = DailyPlans.ZAllPlans(App.DailyPlan.PlanNumber);
            int count = schedule.GetUpperBound(0) + 1;
            if (App.DailyPlan.PlanDayNumber >= count)
            {
                App.DailyPlan.PlanDayNumber = 0;
            }
        }

        #endregion
    }
}