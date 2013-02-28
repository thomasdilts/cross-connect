﻿#region Header

// <copyright file="SearchReader.cs" company="Thomas Dilts">
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
    using System.Diagnostics;
    using System.IO;
    using System.Runtime.Serialization;
    using System.Text;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Windows;

    using Hoot;

    using Sword.reader;

    /// <summary>
    ///     Load from a file all the book and verse pointers to the bzz file so that
    ///     we can later read the bzz file quickly and efficiently.
    /// </summary>
    [DataContract(Name = "SearchReader")]
    [KnownType(typeof(ChapterPos))]
    [KnownType(typeof(BookPos))]
    [KnownType(typeof(VersePos))]
    public class SearchReader : BibleZtextReader
    {
        #region Fields

        [DataMember]
        public string DisplayText = string.Empty;

        [DataMember]
        public string DisplayTextHtmlBody = string.Empty;

        [DataMember]
        public int SearchChapter;

        [DataMember]
        public string SearchText = string.Empty;

        [DataMember]
        public int SearchTypeIndex;

        [DataMember]
        public int Found;

        #endregion

        #region Constructors and Destructors

        public SearchReader(string path, string iso2DigitLangCode, bool isIsoEncoding)
            : base(path, iso2DigitLangCode, isIsoEncoding)
        {
        }

        #endregion

        #region Delegates

        public delegate void ShowProgress(double percent, int totalFound, bool isAbort, bool isFinished);

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
                return false;
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
                return false;
            }
        }

        #endregion

        #region Public Methods and Operators

        public void DoSearch(
            DisplaySettings displaySettings,
            int searchTypeIndex,
            string searchText,
            bool isIgnoreCase,
            List<int> chaptListToSearch,
            bool isFastSearch,
            ShowProgress progress)
        {
            this.Found = 0;
            this.SearchTypeIndex = searchTypeIndex;
            this.SearchText = searchText;
            if (isFastSearch)
            {
                var indexes = new string[] { "_deleted.idx", "index.mgbmp", "index.mgbmr", "index.words" };
                bool existsIndexes = true;
                var xx = Hoot.File.GetFiles(this.Serial.Path.Replace("/", "\\"));
                foreach (var index in indexes)
                {
                    if (!Hoot.File.Exists(Path.Combine(this.Serial.Path.Replace("/", "\\"), index)))
                    {
                        existsIndexes = false;
                        break;
                    }
                }
                progress(0, this.Found, false, false);
                hOOt.Hoot hoot = null;
                for (int i = 0; i < 2; i++)
                {
                    try
                    {
                        hoot = new hOOt.Hoot();
                        hoot.Initialize(this.Serial.Path.Replace("/", "\\"), "index");
                        break;
                    }
                    catch (Exception ee)
                    {
                    }

                    foreach (var index in indexes)
                    {
                        Hoot.File.Delete(Path.Combine(this.Serial.Path.Replace("/", "\\"), index));
                    }
                }
                progress(10, this.Found, false, false);
                if (!existsIndexes)
                {
                    double lastReportedProgress = 0;
                    for (int i = 0; i < BibleZtextReader.ChaptersInBible; i++)
                    {
                        byte[] chapterBuffer = this.GetChapterBytes(i);
                        if (chapterBuffer.Length > 20)
                        {
                            // now we must search each verse in the chapter
                            for (int j = 0; j < this.Chapters[i].Verses.Count; j++)
                            {
                                VersePos verse = this.Chapters[i].Verses[j];
                                if (verse.Length > 0 && verse.StartPos < chapterBuffer.Length)
                                {
                                    //these next 3 rows are a faster method of getting text that proved to have too many errors.
                                    var verseBytes = new byte[verse.Length];
                                    Array.Copy(chapterBuffer, (int)verse.StartPos, verseBytes, 0, verse.Length);
                                    var verseText = RawGenTextReader.CleanXml(Encoding.UTF8.GetString(verseBytes, 0, verseBytes.Length), true);
                                    //This is the safer method, but MUCH slower
                                    //int noteMarker = 'a';
                                    //bool isInPoetry = false;
                                    //string verseText = this.ParseOsisText(
                                    //    displaySettings,
                                    //    string.Empty,
                                    //    string.Empty,
                                    //    chapterBuffer,
                                    //    (int)verse.StartPos,
                                    //    verse.Length,
                                    //    this.Serial.IsIsoEncoding,
                                    //    false,
                                    //    true,
                                    //    ref noteMarker,
                                    //    ref isInPoetry); 
                                    hoot.Index(i * 1000 + j, verseText);
                                }
                            }
                        }

                        double percent = i * 80 / BibleZtextReader.ChaptersInBible;
                        if (percent > lastReportedProgress)
                        {
                            progress(percent, 0, false, false);
                        }

                        lastReportedProgress = percent;
                    }

                    hoot.Save();
                    //hoot.OptimizeIndex();
                }
                progress(80, this.Found, false, false);
                // time to do the search
                var foundIndexes = hoot.Query(this.SearchText, 10000);

                var displayTextBody = new StringBuilder();

                var foundBitIndexes = foundIndexes.GetBitIndexes();
                this.Found = 0;
                progress(85, this.Found, false, false);
                var counter = 0;

                foreach (var index in foundBitIndexes)
                {
                    var chapterNum = index / 1000;
                    if (!chaptListToSearch.Contains(chapterNum))
                    {
                        continue;
                    }
                    this.Found++;
                    if (counter <= 200)
                    {
                        counter++;
                        var verseNum = index % 1000;
                        byte[] chapterBuffer = this.GetChapterBytes(chapterNum);
                        var chapt = this.Chapters[chapterNum];
                        VersePos verse = chapt.Verses[verseNum];
                        // clean up the verse and make sure the text is still there.
                        string textId = "CHAP_" + chapterNum + "_VERS_" + verseNum;
                        string s = "<p><a name=\"" + textId
                                   + "\"></a><a class=\"normalcolor\" href=\"#\" onclick=\"window.external.notify('"
                                   + textId + "'); event.returnValue=false; return false;\" >"
                                   + (displaySettings.SmallVerseNumbers ? "<sup>" : "(")
                                   + this.GetFullName(this.Chapters[chapterNum].Booknum) + " "
                                   + (this.Chapters[chapterNum].BookRelativeChapterNum + 1) + ":" + (verseNum + 1)
                                   + (displaySettings.SmallVerseNumbers ? " </sup>" : ")");
                        const string htmlSuffix = "</a></p><hr />";
                        //int noteMarker = 'a';
                        //bool isInPoetry = false;
                        //string verseTxt = this.ParseOsisText(
                        //    displaySettings,
                        //    string.Empty,
                        //    string.Empty,
                        //    chapterBuffer,
                        //    (int)verse.StartPos,
                        //    verse.Length,
                        //    this.Serial.IsIsoEncoding,
                        //    false,
                        //    true,
                        //    ref noteMarker,
                        //    ref isInPoetry);
                        var verseBytes = new byte[verse.Length];
                        Array.Copy(chapterBuffer, (int)verse.StartPos, verseBytes, 0, verse.Length);
                        var verseText =
                            RawGenTextReader.CleanXml(Encoding.UTF8.GetString(verseBytes, 0, verseBytes.Length), true);
                        displayTextBody.Append(s + verseText + htmlSuffix);
                        progress(85 + (15 * counter / this.Found), this.Found, false, false);
                    }
                }

                this.DisplayText = displayTextBody.ToString();
                progress(100, this.Found, this.Found > 200, true);
            }
            else
            {
                DoSlowSearch(displaySettings, isIgnoreCase, chaptListToSearch, progress);
            }
        }

        public void DoSlowSearch(DisplaySettings displaySettings,
            bool isIgnoreCase,
            List<int> chaptListToSearch,
            ShowProgress progress)
        {
            try
            {
                var displayTextBody = new StringBuilder();
                var regex = new Regex(this.SearchText, isIgnoreCase ? RegexOptions.IgnoreCase : RegexOptions.None);
                int numFoundMatches = 0;
                int i;
                double lastProcent = 0;
                for (i = 0; i < chaptListToSearch.Count; i++)
                {
                    byte[] chapterBuffer = this.GetChapterBytes(chaptListToSearch[i]);
                    string chapter = Encoding.UTF8.GetString(chapterBuffer, 0, chapterBuffer.Length);
                    MatchCollection matches = regex.Matches(chapter);
                    if (matches.Count > 0)
                    {
                        bool isInPoetry = false;
                        // now we must search each verse in the chapter
                        for (int j = 0; j < this.Chapters[chaptListToSearch[i]].Verses.Count; j++)
                        {
                            VersePos verse = this.Chapters[chaptListToSearch[i]].Verses[j];

                            // clean up the verse and make sure the text is still there.
                            string textId = "CHAP_" + chaptListToSearch[i] + "_VERS_" + j;
                            string s = "<p><a name=\"" + textId
                                       + "\"></a><a class=\"normalcolor\" href=\"#\" onclick=\"window.external.notify('"
                                       + textId + "'); event.returnValue=false; return false;\" >"
                                       + (displaySettings.SmallVerseNumbers ? "<sup>" : "(")
                                       + this.GetFullName(this.Chapters[chaptListToSearch[i]].Booknum) + " "
                                       + (this.Chapters[chaptListToSearch[i]].BookRelativeChapterNum + 1) + ":"
                                       + (j + 1) + (displaySettings.SmallVerseNumbers ? " </sup>" : ")");
                            const string htmlSuffix = "</a></p><hr />";
                            int noteMarker = 'a';
                            string verseTxt = this.ParseOsisText(
                                displaySettings,
                                string.Empty,
                                string.Empty,
                                chapterBuffer,
                                (int)verse.StartPos,
                                verse.Length,
                                this.Serial.IsIsoEncoding,
                                false,
                                true,
                                ref noteMarker,
                                ref isInPoetry);
                            matches = regex.Matches(verseTxt);
                            Match lastMatch = null;
                            bool foundMatch = false;
                            foreach (Match match in matches)
                            {
                                // if the match goes over into the previous match then skip it
                                if (lastMatch != null
                                    && (match.Index > lastMatch.Index
                                        && match.Index < lastMatch.Index + lastMatch.Length))
                                {
                                    continue;
                                }

                                lastMatch = match;
                                foundMatch = true;
                                verseTxt = verseTxt.Substring(0, match.Index) + "<b>"
                                           + verseTxt.Substring(match.Index, match.Length) + "</b>"
                                           + verseTxt.Substring(match.Index + match.Length);
                            }

                            if (foundMatch)
                            {
                                displayTextBody.Append(s + verseTxt + htmlSuffix);
                                numFoundMatches++;
                            }
                        }
                    }

                    double procent = i * (double)100 / chaptListToSearch.Count;
                    if (((int)procent) != ((int)lastProcent))
                    {
                        progress(procent, numFoundMatches, false, false);
                    }

                    lastProcent = procent;
                    if (numFoundMatches > 200)
                    {
                        break;
                    }
                }

                this.DisplayText = displayTextBody.ToString();
                this.Found = numFoundMatches;
                progress(100, numFoundMatches, chaptListToSearch.Count > i, true);
                Debug.WriteLine("Done searching.");
            }
            catch (Exception)
            {
                this.Found = 0;
                progress(100, 0, false, true);
            }
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
            // Debug.WriteLine("SearchReader GetChapterHtml.text=" + displayText);
            return HtmlHeader(
                displaySettings,
                htmlBackgroundColor,
                htmlForegroundColor,
                htmlPhoneAccentColor,
                htmlFontSize,
                fontFamily) + this.DisplayText + "</body></html>";
        }

        public override void GetInfo(
            out int bookNum,
            out int absouteChaptNum,
            out int relChaptNum,
            out int verseNum,
            out string fullName,
            out string title)
        {
            verseNum = 0;
            absouteChaptNum = 0;
            bookNum = 0;
            relChaptNum = 0;
            fullName = string.Empty;
            string extraText = string.Empty;
            switch (this.SearchTypeIndex)
            {
                case 0:
                    extraText = Translations.Translate("Whole bible");
                    break;
                case 1:
                    extraText = Translations.Translate("The Old Testement");
                    break;
                case 2:
                    extraText = Translations.Translate("The New Testement");
                    break;
                case 3:
                    extraText = fullName;
                    break;
            }

            title = Translations.Translate("Search") + "; " + this.SearchText + "; " + extraText + "; " + Translations.Translate("Found") + "(" + this.Found + ")";
        }

        #endregion
    }
}