#region Header

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
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;

    using Hoot;

    using Sword.reader;

    /// <summary>
    ///     Load from a file all the book and verse pointers to the bzz file so that
    ///     we can later read the bzz file quickly and efficiently.
    /// </summary>
    [DataContract]
    [KnownType(typeof(ChapterData))]
    public class RawGenSearchReader : RawGenTextReader
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

        public RawGenSearchReader(string path, string iso2DigitLangCode, bool isIsoEncoding)
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

        public override bool IsTTChearable
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

        public async void DoSearch(
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
                foreach (var index in indexes)
                {
                    if (!await Hoot.File.Exists(Path.Combine(this.Serial.Path.Replace("/", "\\"), index)))
                    {
                        existsIndexes = false;
                        break;
                    }
                }
                progress(0, this.Found, false, false);
                var hoot = new hOOt.Hoot();

                await hoot.Initialize(this.Serial.Path.Replace("/", "\\"), "index");
                progress(10, this.Found, false, false);
                if (!existsIndexes)
                {
                    double lastReportedProgress = 0;
                    for (int i = 0; i < Chapters.Count; i++)
                    {
                        byte[] chapterBuffer = await this.GetChapterBytes(i);
                        if (chapterBuffer.Length > 20)
                        {
                            string chapter =
                                RawGenTextReader.CleanXml(
                                    Encoding.UTF8.GetString(chapterBuffer, 0, chapterBuffer.Length), true);
                            await hoot.Index(i, chapter);
                        }

                        double percent = i * 80 / Chapters.Count;
                        if (percent > lastReportedProgress)
                        {
                            progress(percent, 0, false, false);
                        }

                        lastReportedProgress = percent;
                    }

                    await hoot.Save();
                    //await hoot.OptimizeIndex();
                }
                progress(80, this.Found, false, false);
                // time to do the search
                var foundIndexes = await hoot.Query(this.SearchText, 1000);

                var displayTextBody = new StringBuilder();
                var foundBitIndexes = foundIndexes.GetBitIndexes();
                this.Found = foundBitIndexes.Count();
                progress(85, this.Found, false, false);
                var counter = 0;

                foreach (var index in foundBitIndexes)
                {
                    if (counter <= 200)
                    {
                        counter++;
                        var chapt = Chapters[index];
                        byte[] chapterBuffer = await this.GetChapterBytes(index);
                        string chapter =
                            RawGenTextReader.CleanXml(
                                Encoding.UTF8.GetString(chapterBuffer, 0, chapterBuffer.Length), true);


                        string textId = "RAWBOOK_" + chaptListToSearch[index] + "_0";
                        string s = "<p><a name=\"" + textId
                                   + "\"></a><a class=\"normalcolor\" href=\"#\" onclick=\"window.external.notify('"
                                   + textId + "'); event.returnValue=false; return false;\" >";
                        const string htmlSuffix = "</a></p><hr />";
                        displayTextBody.Append(s  + chapt.Title + htmlSuffix);
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
        public async void DoSlowSearch(DisplaySettings displaySettings,
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
                    var chapt = Chapters[chaptListToSearch[i]];
                    if (chapt.NumCharacters > 100000)
                    {
                        continue;
                    }

                    byte[] chapterBuffer = await this.GetChapterBytes(chaptListToSearch[i]);
                    string chapter = RawGenTextReader.CleanXml(Encoding.UTF8.GetString(chapterBuffer, 0, chapterBuffer.Length),true);
                    MatchCollection matches = regex.Matches(chapter);
                    if (matches.Count > 0)
                    {
                        bool isInPoetry = false;
                        var parent = this.Chapters[chaptListToSearch[i]];
                        var titleText = string.Empty;
                        while (parent != null)
                        {
                            titleText = parent.Title + " ; " + titleText;
                            parent = parent.Parent;
                        }
                        foreach (Match match in matches)
                        {
                            // clean up the verse and make sure the text is still there.
                            string textId = "RAWBOOK_" + chaptListToSearch[i] + "_0";
                            string s = "<p><a name=\"" + textId
                                       + "\"></a><a class=\"normalcolor\" href=\"#\" onclick=\"window.external.notify('"
                                       + textId + "'); event.returnValue=false; return false;\" >"
                                       + (displaySettings.SmallVerseNumbers ? "<sup>" : "(") + titleText
                                       + (displaySettings.SmallVerseNumbers ? " </sup>" : ")");
                            const string htmlSuffix = "</a></p><hr />";
                            int noteMarker = 'a';
                            int sizeOfVisualText = 80;
                            var startMatch = match.Index > sizeOfVisualText ? match.Index - sizeOfVisualText : 0;
                            var aroundMatch = chapter.Substring(
                                startMatch,
                                startMatch + sizeOfVisualText * 2 < chapter.Length
                                    ? sizeOfVisualText * 2
                                    : chapter.Length - startMatch);
                            var convertedStartPos = match.Index > sizeOfVisualText
                                                        ? sizeOfVisualText
                                                        : match.Index;
                            aroundMatch = aroundMatch.Substring(
                                0, convertedStartPos) + "<b>"
                                          + aroundMatch.Substring(convertedStartPos, match.Length)
                                          + "</b>"
                                          + aroundMatch.Substring(
                                              (convertedStartPos + match.Length));
                            displayTextBody.Append(s + aroundMatch + htmlSuffix);
                            numFoundMatches++;
                        }
                    }

                    this.Found = numFoundMatches;
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
                progress(100, numFoundMatches, chaptListToSearch.Count > i, true);
                Debug.WriteLine("Done searching.");
            }
            catch (Exception)
            {
                progress(100, 0, false, true);
            }
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
            // Debug.WriteLine("SearchReader GetChapterHtml.text=" + displayText);
            return BibleZtextReader.HtmlHeader(
                displaySettings,
                htmlBackgroundColor,
                htmlForegroundColor,
                htmlPhoneAccentColor,
                htmlWordsOfChristColor,
                htmlFontSize,
                fontFamily) + this.DisplayText + "</body></html>";
        }

        public override void GetInfo(
            string isoLangCode,
            out string bookShortName,
            out int relChaptNum,
            out int verseNum,
            out string fullName,
            out string title)
        {
            verseNum = 0;
            bookShortName = string.Empty;
            relChaptNum = 0;
            fullName = string.Empty;
            string extraText = string.Empty;
            title = Translations.Translate("Search") + "; " + this.SearchText + "; " + extraText + "; " + Translations.Translate("Found") + "(" + this.Found + ")";
        }

        #endregion
    }
}