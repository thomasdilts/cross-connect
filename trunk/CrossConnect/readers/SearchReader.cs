#region Header

// <copyright file="SearchReader.cs" company="Thomas Dilts">
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
// Email: thomas@cross-connect.se
// </summary>
// <author>Thomas Dilts</author>

#endregion Header

namespace CrossConnect.readers
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.Serialization;
    using System.Text;
    using System.Text.RegularExpressions;

    using Sword.reader;

    /// <summary>
    ///   Load from a file all the book and verse pointers to the bzz file so that
    ///   we can later read the bzz file quickly and efficiently.
    /// </summary>
    [DataContract(Name = "SearchReader")]
    [KnownType(typeof (ChapterPos))]
    [KnownType(typeof (BookPos))]
    [KnownType(typeof (VersePos))]
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

        #endregion Fields

        #region Constructors

        public SearchReader(string path,
            string iso2DigitLangCode,
            bool isIsoEncoding)
            : base(path, iso2DigitLangCode, isIsoEncoding)
        {
        }

        #endregion Constructors

        #region Delegates

        public delegate void ShowProgress(double percent, int totalFound, bool isAbort, bool isFinished);

        #endregion Delegates

        #region Properties

        public override bool IsHearable
        {
            get { return false; }
        }

        public override bool IsPageable
        {
            get { return false; }
        }

        public override bool IsSearchable
        {
            get { return false; }
        }

        public override bool IsSynchronizeable
        {
            get { return false; }
        }

        public override bool IsTranslateable
        {
            get { return false; }
        }

        #endregion Properties

        #region Methods

        public void DoSearch(
            DisplaySettings displaySettings,
            int searchTypeIndex,
            string searchText,
            bool isIgnoreCase,
            List<int> chaptListToSearch,
            ShowProgress progress)
        {
            SearchTypeIndex = searchTypeIndex;
            SearchText = searchText;

            var displayTextBody = new StringBuilder();
            var regex = new Regex(searchText, isIgnoreCase ? RegexOptions.IgnoreCase : RegexOptions.None);
            int numFoundMatches = 0;
            int i;
            double lastProcent = 0;
            for (i = 0; i < chaptListToSearch.Count; i++)
            {
                var chapterBuffer = GetChapterBytes(chaptListToSearch[i]);
                string chapter = Encoding.UTF8.GetString(chapterBuffer, 0, chapterBuffer.Length);
                var matches = regex.Matches(chapter);
                if (matches.Count > 0)
                {
                    // now we must search each verse in the chapter
                    for (int j = 0; j < Chapters[chaptListToSearch[i]].Verses.Count; j++)
                    {
                        var verse = Chapters[chaptListToSearch[i]].Verses[j];
                        // clean up the verse and make sure the text is still there.
                        string textId = "CHAP_" + chaptListToSearch[i] + "_VERS_" + j;
                        string s = "<p><a name=\"" + textId +
                                        "\"></a><a class=\"normalcolor\" href=\"#\" onclick=\"window.external.Notify('" +
                                        textId + "'); event.returnValue=false; return false;\" >" +
                                        (displaySettings.SmallVerseNumbers ? "<sup>" : "(") +
                                        GetFullName(Chapters[chaptListToSearch[i]].Booknum) + " " +
                                        (Chapters[chaptListToSearch[i]].BookRelativeChapterNum + 1) + ":" +
                                        (j + 1) + (displaySettings.SmallVerseNumbers ? " </sup>" : ")");
                        const string htmlSuffix = "</a></p><hr />";
                        int noteMarker = 'a';
                        string verseTxt = ParseOsisText(displaySettings,
                                                        "",
                                                        "",
                                                        chapterBuffer,
                                                        (int) verse.StartPos,
                                                        verse.Length,
                                                        Serial.IsIsoEncoding,
                                                        false,
                                                        true,
                                                        ref noteMarker);
                        matches = regex.Matches(verseTxt);
                        Match lastMatch = null;
                        bool foundMatch = false;
                        foreach (Match match in matches)
                        {
                            // if the match goes over into the previous match then skip it
                            if (lastMatch != null &&
                                (match.Index > lastMatch.Index && match.Index < lastMatch.Index + lastMatch.Length))
                            {
                                continue;
                            }
                            lastMatch = match;
                            foundMatch = true;
                            verseTxt = verseTxt.Substring(0, match.Index) + "<b>" +
                                       verseTxt.Substring(match.Index, match.Length) + "</b>" +
                                       verseTxt.Substring(match.Index + match.Length);
                        }
                        if (foundMatch)
                        {
                            displayTextBody.Append(s + verseTxt + htmlSuffix);
                            numFoundMatches++;
                        }
                    }
                }

                double procent = i*(double)100/chaptListToSearch.Count;
                if (((int) procent) != ((int) lastProcent))
                {
                    progress(procent, numFoundMatches, false, false);
                }

                lastProcent = procent;
                if (numFoundMatches > 200)
                {
                    break;
                }
            }

            DisplayText = displayTextBody.ToString();
            progress(100, numFoundMatches, chaptListToSearch.Count > i, true);
            Debug.WriteLine("Done searching.");
        }

        public override void GetInfo(out int bookNum, out int absouteChaptNum, out int relChaptNum, out int verseNum,
            out string fullName, out string title)
        {
            verseNum = 0;
            absouteChaptNum = 0;
            bookNum = 0;
            relChaptNum = 0;
            fullName = "";
            string extraText = string.Empty;
            switch (SearchTypeIndex)
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

            title = Translations.Translate("Search") + "; " + SearchText + "; " + extraText;
        }

        protected override string GetChapterHtml(DisplaySettings displaySettings, string htmlBackgroundColor,
            string htmlForegroundColor, string htmlPhoneAccentColor,
            double htmlFontSize, string fontFamily, bool isNotesOnly, bool addStartFinishHtml, bool forceReload)
        {
            //Debug.WriteLine("SearchReader GetChapterHtml.text=" + displayText);
            return HtmlHeader(displaySettings, htmlBackgroundColor, htmlForegroundColor, htmlPhoneAccentColor,
                              htmlFontSize, fontFamily)
                   + DisplayText + "</body></html>";
        }

        #endregion Methods
    }
}