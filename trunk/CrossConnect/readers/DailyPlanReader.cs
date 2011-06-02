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
/// <copyright file="DailyPlanReader.cs" company="Thomas Dilts">
///     Thomas Dilts. All rights reserved.
/// </copyright>
/// <author>Thomas Dilts</author>
namespace CrossConnect
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.IO.IsolatedStorage;
    using System.Net;
    using System.Runtime.Serialization;
    using System.Text;

    using ComponentAce.Compression.Libs.zlib;

    using SwordBackend;

    /// <summary>
    /// Load from a file all the book and verse pointers to the bzz file so that
    /// we can later read the bzz file quickly and efficiently.
    /// </summary>
    /// <param name="path">The path to where the ot.bzs,ot.bzv and ot.bzz and nt files are</param>
    [DataContract]
    public class DailyPlanReader : BibleZtextReader
    {
        #region Fields

        [DataMember]
        public BibleZtextReaderSerialData serial2 = new BibleZtextReaderSerialData(false,"","",0,0);

        #endregion Fields

        #region Constructors

        public DailyPlanReader(string path, string iso2DigitLangCode, bool isIsoEncoding)
            : base(path, iso2DigitLangCode, isIsoEncoding)
        {
            this.serial2.cloneFrom(base.serial);
        }

        #endregion Constructors

        #region Properties

        public override bool IsBookmarkable
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

        #endregion Properties

        #region Methods

        public override ButtonWindowSpecs GetButtonWindowSpecs(int stage, int lastSelectedButton)
        {
            if (stage == 0)
            {
                int count = DailyPlans.zAllPlans[App.dailyPlan.planNumber].GetUpperBound(0) + 1;

                int[] butColors = new int[count];
                int[] values = new int[count];
                string[] butText = new string[count];
                for (int i = 0; i < count; i++)
                {
                    butColors[i] = 0;
                    butText[i] = (i + 1).ToString();
                    values[i] = i;
                }

                // do a nice transition

                return new ButtonWindowSpecs(
                    stage,
                    "Select day",
                    count,
                    butColors,
                    butText,
                    values,
                    ButtonSize.SMALL);
            }
            else
            {
                return null;
            }
        }

        public override void GetInfo(out int bookNum, out int absoluteChaptNum, out int relChaptNum, out int verseNum, out string fullName, out string title)
        {
            int count = DailyPlans.zAllPlans[App.dailyPlan.planNumber].GetUpperBound(0) + 1;
            if (App.dailyPlan.planDayNumber >= count)
            {
                App.dailyPlan.planDayNumber = 0;
            }
            base.GetInfo(out bookNum, out absoluteChaptNum, out relChaptNum, out verseNum, out fullName, out title);
            title = App.dailyPlan.planStartDate.AddDays(App.dailyPlan.planDayNumber).ToShortDateString() + " " + Translations.translate("Daily plan");
        }

        public override void moveChapterVerse(int chapter, int verse, bool isLocalLinkChange)
        {
            if (isLocalLinkChange)
            {
                base.serial.posChaptNum = chapter;
                base.serial.posVerseNum = verse;
            }
            else
            {
                App.dailyPlan.planDayNumber = chapter;
            }
        }

        public override void moveNext()
        {
            int count = DailyPlans.zAllPlans[App.dailyPlan.planNumber].GetUpperBound(0) + 1;
            App.dailyPlan.planDayNumber++;
            if (App.dailyPlan.planDayNumber>=count)
            {
                App.dailyPlan.planDayNumber = 0;
            }
        }

        public override void movePrevious()
        {
            int count = DailyPlans.zAllPlans[App.dailyPlan.planNumber].GetUpperBound(0) + 1;
            App.dailyPlan.planDayNumber--;
            if (App.dailyPlan.planDayNumber < 0)
            {
                App.dailyPlan.planDayNumber = count-1;
            }
        }

        public override void Resume()
        {
            base.serial.cloneFrom(this.serial2);
            base.Resume();
        }

        public override void SerialSave()
        {
            this.serial2.cloneFrom(base.serial);
        }

        public override void SetToFirstChapter()
        {
            int count = DailyPlans.zAllPlans[App.dailyPlan.planNumber].GetUpperBound(0) + 1;
            if (App.dailyPlan.planDayNumber >= count)
            {
                App.dailyPlan.planDayNumber = 0;
            }
        }

        protected override string GetChapterHtml(string htmlBackgroundColor, string htmlForegroundColor, string htmlPhoneAccentColor, double htmlFontSize, bool isNotesOnly, bool addStartFinishHtml)
        {
            string chapterStartHtml = "<html>" + HtmlHeader(htmlBackgroundColor, htmlForegroundColor, htmlPhoneAccentColor, htmlFontSize);
            string chapterEndHtml = "</body></html>";
            StringBuilder sb = new StringBuilder(chapterStartHtml);
            int bookNum;
            int relChaptNum;
            string fullName;
            string title;

            sb.Append("<h3>" + Translations.translate("Day") + " " + (App.dailyPlan.planDayNumber + 1) + ", " + App.dailyPlan.planStartDate.AddDays(App.dailyPlan.planDayNumber).ToShortDateString() + "</h3>");
            sb.Append("<h3>" + Translations.translate(DailyPlans.zzAllPlansNames[App.dailyPlan.planNumber][0]) + "; " + DailyPlans.zzAllPlansNames[App.dailyPlan.planNumber][1] + " " + Translations.translate("days") + "</h3>");
            for (int i = 0; i <= DailyPlans.zAllPlans[App.dailyPlan.planNumber][App.dailyPlan.planDayNumber].GetUpperBound(0); i++)
            {
                base.GetInfo(DailyPlans.zAllPlans[App.dailyPlan.planNumber][App.dailyPlan.planDayNumber][i], 0, out bookNum, out relChaptNum, out fullName, out title);
                sb.Append("<h2>" + fullName + " " + (relChaptNum + 1) + "</h2>");
                sb.Append(base.GetChapterHtml(DailyPlans.zAllPlans[App.dailyPlan.planNumber][App.dailyPlan.planDayNumber][i], htmlBackgroundColor, htmlForegroundColor, htmlPhoneAccentColor, htmlFontSize, false, false));
            }
            sb.Append(chapterEndHtml);
            return sb.ToString();
        }

        #endregion Methods
    }
}