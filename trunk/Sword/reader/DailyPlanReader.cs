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
/// <copyright file="App.xaml.cs" company="Thomas Dilts">
///     Thomas Dilts. All rights reserved.
/// </copyright>
/// <author>Thomas Dilts</author>
namespace SwordBackend
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.IO.IsolatedStorage;
    using System.Net;
    using System.Runtime.Serialization;

    using ComponentAce.Compression.Libs.zlib;

    using SwordBackend;
    using System.Text;

    /// <summary>
    /// Load from a file all the book and verse pointers to the bzz file so that
    /// we can later read the bzz file quickly and efficiently.
    /// </summary>
    /// <param name="path">The path to where the ot.bzs,ot.bzv and ot.bzz and nt files are</param>
    [DataContract]
    [KnownType(typeof(ChapterPos))]
    [KnownType(typeof(BookPos))]
    [KnownType(typeof(VersePos))]
    public class DailyPlanReader : BibleZtextReader
    {
        #region Fields

        private int dailyPlanNumber = 0;
        private string transDay = "Day";
        private string dailyPlanText = "Daily reading";
        private DateTime startPlan;

        BibleNames bibleNames=null;

        #endregion Fields

        #region Constructors

        public DailyPlanReader(string path, string iso2DigitLangCode, bool isIsoEncoding, int dailyPlanNumber, string dailyPlanText, string transDay, DateTime startPlan)
            : base(path, iso2DigitLangCode, isIsoEncoding)
        {
            this.dailyPlanNumber = dailyPlanNumber;
            this.dailyPlanText = dailyPlanText;
            this.startPlan = startPlan;
            this.transDay = transDay;
            bibleNames = new BibleNames(iso2DigitLangCode);
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

        public override int getChapterStartNumber()
        {
            DateTime now=DateTime.Now;
            TimeSpan ts=now.Subtract(startPlan);
            int days = ts.Days;
            if (days < 0)
            {
                days = -days;
            }
            int returnNum = days % getMaxNumChapters();
            return days % getMaxNumChapters();
        }

        public override string GetChapterHtml(int chapterNumber, string htmlBackgroundColor, string htmlForegroundColor, string htmlPhoneAccentColor, double htmlFontSize)
        {
            string chapterStartHtml = "<html>" + HtmlHeader(htmlBackgroundColor, htmlForegroundColor, htmlPhoneAccentColor, htmlFontSize);
            string chapterEndHtml = "</body></html>";
            StringBuilder sb = new StringBuilder(chapterStartHtml);
            int bookNum;
            int relChaptNum;
            string fullName;
            string title;
            sb.Append("<h3>" + transDay + " " + (chapterNumber + 1) + " " + startPlan.AddDays(chapterNumber).ToShortDateString() + "</h3>");
            for (int i = 0; i <= DailyPlans.zAllPlans[dailyPlanNumber][chapterNumber].GetUpperBound(0); i++)
            {
                base.GetInfo(DailyPlans.zAllPlans[dailyPlanNumber][chapterNumber][i], 0, out bookNum, out relChaptNum, out fullName, out title);
                sb.Append("<h2>" + fullName + " " + (relChaptNum + 1) + "</h2>");
                sb.Append(base.GetChapterHtml(DailyPlans.zAllPlans[dailyPlanNumber][chapterNumber][i], htmlBackgroundColor, htmlForegroundColor, htmlPhoneAccentColor, htmlFontSize, false, false));
            }
            sb.Append(chapterEndHtml);
            return sb.ToString();
        }

        public override void GetInfo(int chaptNum, int verseNum, out int bookNum, out int relChaptNum, out string fullName, out string title)
        {
            base.GetInfo(chaptNum, verseNum, out bookNum, out relChaptNum, out fullName, out title);
            title = startPlan.AddDays(chaptNum).ToShortDateString() + " " + dailyPlanText;
        }

        public override int getMaxNumChapters()
        {
            return DailyPlans.zAllPlans[dailyPlanNumber].GetUpperBound(0)+1;
        }

        #endregion Methods
    }
}