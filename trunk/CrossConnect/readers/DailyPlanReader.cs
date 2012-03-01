// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DailyPlanReader.cs" company="">
//   
// </copyright>
// <summary>
//   Load from a file all the book and verse pointers to the bzz file so that
//   we can later read the bzz file quickly and efficiently.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

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
    using System.Runtime.Serialization;
    using System.Text;

    using Sword;
    using Sword.reader;

    /// <summary>
    /// Load from a file all the book and verse pointers to the bzz file so that
    ///   we can later read the bzz file quickly and efficiently.
    /// </summary>
    [DataContract]
    public class DailyPlanReader : BibleZtextReader
    {
        #region Constants and Fields

        /// <summary>
        /// The serial 2.
        /// </summary>
        [DataMember(Name = "serial2")]
        public BibleZtextReaderSerialData Serial2 = new BibleZtextReaderSerialData(false, string.Empty, string.Empty, 0, 0);

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DailyPlanReader"/> class.
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
        public DailyPlanReader(string path, string iso2DigitLangCode, bool isIsoEncoding)
            : base(path, iso2DigitLangCode, isIsoEncoding)
        {
            this.Serial2.CloneFrom(this.Serial);
            this.SetToFirstChapter();
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
        /// Gets a value indicating whether IsLocalChangeDuringLink.
        /// </summary>
        public override bool IsLocalChangeDuringLink
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Gets a value indicating whether IsPageable.
        /// </summary>
        public override bool IsPageable
        {
            get
            {
                return true;
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
                return true;
            }
        }

        #endregion

        #region Public Methods and Operators

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
                int count = DailyPlans.ZAllPlans[App.DailyPlan.PlanNumber].GetUpperBound(0) + 1;

                var butColors = new int[count];
                var values = new int[count];
                var butText = new string[count];
                for (int i = 0; i < count; i++)
                {
                    butColors[i] = 0;
                    butText[i] = (i + 1).ToString();
                    values[i] = i;
                }

                // do a nice transition
                return new ButtonWindowSpecs(stage, "Select day", count, butColors, butText, values, ButtonSize.Small);
            }

            return null;
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
            int count = DailyPlans.ZAllPlans[App.DailyPlan.PlanNumber].GetUpperBound(0) + 1;
            if (App.DailyPlan.PlanDayNumber >= count)
            {
                App.DailyPlan.PlanDayNumber = 0;
            }

            base.GetInfo(out bookNum, out absoluteChaptNum, out relChaptNum, out verseNum, out fullName, out title);
            title = App.DailyPlan.PlanStartDate.AddDays(App.DailyPlan.PlanDayNumber).ToShortDateString() + " "
                    + Translations.Translate("Daily plan");
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
            if (isLocalLinkChange)
            {
                this.Serial.PosChaptNum = chapter;
                this.Serial.PosVerseNum = verse;
            }
            else
            {
                App.DailyPlan.PlanDayNumber = chapter;

                // Changing the chapter is only to fool the base class into understanding that we have changed pages
                this.Serial.PosChaptNum = this.Serial.PosChaptNum == 0 ? 1 : 0;
            }
        }

        /// <summary>
        /// The move next.
        /// </summary>
        public override void MoveNext()
        {
            this.Serial.PosVerseNum = 0;
            int count = DailyPlans.ZAllPlans[App.DailyPlan.PlanNumber].GetUpperBound(0) + 1;
            App.DailyPlan.PlanDayNumber++;
            if (App.DailyPlan.PlanDayNumber >= count)
            {
                App.DailyPlan.PlanDayNumber = 0;
            }

            // Changing the chapter is only to fool the base class into understanding that we have changed pages
            this.Serial.PosChaptNum = this.Serial.PosChaptNum == 0 ? 1 : 0;
        }

        /// <summary>
        /// The move previous.
        /// </summary>
        public override void MovePrevious()
        {
            this.Serial.PosVerseNum = 0;
            int count = DailyPlans.ZAllPlans[App.DailyPlan.PlanNumber].GetUpperBound(0) + 1;
            App.DailyPlan.PlanDayNumber--;
            if (App.DailyPlan.PlanDayNumber < 0)
            {
                App.DailyPlan.PlanDayNumber = count - 1;
            }

            // Changing the chapter is only to fool the base class into understanding that we have changed pages
            this.Serial.PosChaptNum = this.Serial.PosChaptNum == 0 ? 1 : 0;
        }

        /// <summary>
        /// The resume.
        /// </summary>
        public override void Resume()
        {
            this.Serial.CloneFrom(this.Serial2);
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
            string chapterStartHtml = HtmlHeader(
                displaySettings, 
                htmlBackgroundColor, 
                htmlForegroundColor, 
                htmlPhoneAccentColor, 
                htmlFontSize, 
                fontFamily);
            const string chapterEndHtml = "</body></html>";
            var sb = new StringBuilder(chapterStartHtml);

            sb.Append(
                "<h3>" + Translations.Translate("Day") + " " + (App.DailyPlan.PlanDayNumber + 1) + ", "
                + App.DailyPlan.PlanStartDate.AddDays(App.DailyPlan.PlanDayNumber).ToShortDateString() + "</h3>");
            sb.Append(
                "<h3>" + Translations.Translate(DailyPlans.ZzAllPlansNames[App.DailyPlan.PlanNumber][0]) + "; "
                + DailyPlans.ZzAllPlansNames[App.DailyPlan.PlanNumber][1] + " " + Translations.Translate("Days")
                + "</h3>");
            for (int i = 0;
                 i <= DailyPlans.ZAllPlans[App.DailyPlan.PlanNumber][App.DailyPlan.PlanDayNumber].GetUpperBound(0);
                 i++)
            {
                int bookNum;
                int relChaptNum;
                string fullName;
                string title;
                this.GetInfo(
                    DailyPlans.ZAllPlans[App.DailyPlan.PlanNumber][App.DailyPlan.PlanDayNumber][i], 
                    0, 
                    out bookNum, 
                    out relChaptNum, 
                    out fullName, 
                    out title);
                sb.Append("<h2>" + fullName + " " + (relChaptNum + 1) + "</h2>");
                sb.Append(
                    this.GetChapterHtml(
                        displaySettings, 
                        DailyPlans.ZAllPlans[App.DailyPlan.PlanNumber][App.DailyPlan.PlanDayNumber][i], 
                        htmlBackgroundColor, 
                        htmlForegroundColor, 
                        htmlPhoneAccentColor, 
                        htmlFontSize, 
                        fontFamily, 
                        false, 
                        false, 
                        forceReload));
            }

            sb.Append(chapterEndHtml);
            return sb.ToString();
        }

        /// <summary>
        /// The set to first chapter.
        /// </summary>
        private void SetToFirstChapter()
        {
            int count = DailyPlans.ZAllPlans[App.DailyPlan.PlanNumber].GetUpperBound(0) + 1;
            if (App.DailyPlan.PlanDayNumber >= count)
            {
                App.DailyPlan.PlanDayNumber = 0;
            }
        }

        #endregion
    }
}