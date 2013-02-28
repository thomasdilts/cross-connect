#region Header

// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DailyPlanReader.cs" company="">
//
// </copyright>
// <summary>
//   Load from a file all the book and verse pointers to the bzz file so that
//   we can later read the bzz file quickly and efficiently.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
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


    using Sword;
    using Sword.reader;

    /// <summary>
    /// Load from a file all the book and verse pointers to the bzz file so that
    ///   we can later read the bzz file quickly and efficiently.
    /// </summary>
    [DataContract]
    public class DailyPlanReader : BibleZtextReader
    {
        #region Fields

        [DataMember(Name = "serial2")]
        public BibleZtextReaderSerialData Serial2 = new BibleZtextReaderSerialData(false, string.Empty, string.Empty, 0, 0);

        #endregion Fields

        #region Constructors

        public DailyPlanReader(string path, string iso2DigitLangCode, bool isIsoEncoding)
            : base(path, iso2DigitLangCode, isIsoEncoding)
        {
            Serial2.CloneFrom(Serial);
            SetToFirstChapter();
        }

        #endregion Constructors

        #region Properties

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

        #endregion Properties

        #region Methods

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
                    butText[i] = (i + 1).ToString(CultureInfo.InvariantCulture);
                    values[i] = i;
                }

                // do a nice transition
                return new ButtonWindowSpecs(stage, "Select day", count, butColors, butText, values, ButtonSize.Small);
            }

            return null;
        }

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

        public override void MoveChapterVerse(int chapter, int verse, bool isLocalLinkChange, IBrowserTextSource source)
        {
            if (!(source is BibleZtextReader))
            {
                return;
            } 
            
            if (isLocalLinkChange)
            {
                Serial.PosChaptNum = chapter;
                Serial.PosVerseNum = verse;
            }
            else
            {
                App.DailyPlan.PlanDayNumber = chapter;

                Serial.PosChaptNum = DailyPlans.ZAllPlans[App.DailyPlan.PlanNumber][App.DailyPlan.PlanDayNumber][0];
                Serial.PosVerseNum = 0;
            }
        }

        public override void MoveNext()
        {
            Serial.PosVerseNum = 0;
            int count = DailyPlans.ZAllPlans[App.DailyPlan.PlanNumber].GetUpperBound(0) + 1;
            App.DailyPlan.PlanDayNumber++;
            if (App.DailyPlan.PlanDayNumber >= count)
            {
                App.DailyPlan.PlanDayNumber = 0;
            }

            Serial.PosChaptNum = DailyPlans.ZAllPlans[App.DailyPlan.PlanNumber][App.DailyPlan.PlanDayNumber][0];
        }

        public override void MovePrevious()
        {
            Serial.PosVerseNum = 0;
            int count = DailyPlans.ZAllPlans[App.DailyPlan.PlanNumber].GetUpperBound(0) + 1;
            App.DailyPlan.PlanDayNumber--;
            if (App.DailyPlan.PlanDayNumber < 0)
            {
                App.DailyPlan.PlanDayNumber = count - 1;
            }

            Serial.PosChaptNum = DailyPlans.ZAllPlans[App.DailyPlan.PlanNumber][App.DailyPlan.PlanDayNumber][0];
        }

        public override void Resume()
        {
            Serial.CloneFrom(Serial2);
            base.Resume();
        }

        public override void SerialSave()
        {
            Serial2.CloneFrom(Serial);
        }

        public override string GetChapterHtml(
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
            const string ChapterEndHtml = "</body></html>";
            var sb = new StringBuilder(chapterStartHtml);
            string id = "CHAP_" + DailyPlans.ZAllPlans[App.DailyPlan.PlanNumber][App.DailyPlan.PlanDayNumber][0]
                        + "_VERS_0";
            string firstVerseForTheDayRedirect = "<a name=\"" 
                + id
                + "\" id=\"ID" + id + "\" ></a>";
            sb.Append(
                firstVerseForTheDayRedirect + "<h3>" + Translations.Translate("Day") + " " + (App.DailyPlan.PlanDayNumber + 1) + ", "
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
                GetInfo(
                    DailyPlans.ZAllPlans[App.DailyPlan.PlanNumber][App.DailyPlan.PlanDayNumber][i],
                    0,
                    out bookNum,
                    out relChaptNum,
                    out fullName,
                    out title);

                sb.Append("<h2>" + fullName + " " + (relChaptNum + 1) + "</h2>");
                sb.Append(
                    GetChapterHtml(
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

            sb.Append(ChapterEndHtml);
            return sb.ToString();
        }

        public override void SetToFirstChapter()
        {
            int count = DailyPlans.ZAllPlans[App.DailyPlan.PlanNumber].GetUpperBound(0) + 1;
            if (App.DailyPlan.PlanDayNumber >= count)
            {
                App.DailyPlan.PlanDayNumber = 0;
            }
        }

        #endregion Methods
    }
}