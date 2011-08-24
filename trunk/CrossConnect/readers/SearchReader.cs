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
/// <copyright file="SearchReader.cs" company="Thomas Dilts">
///     Thomas Dilts. All rights reserved.
/// </copyright>
/// <author>Thomas Dilts</author>
namespace CrossConnect.readers
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.Serialization;
    using System.Text;
    using System.Text.RegularExpressions;

    using SwordBackend;

    /// <summary>
    /// Load from a file all the book and verse pointers to the bzz file so that
    /// we can later read the bzz file quickly and efficiently.
    /// </summary>
    /// <param name="path">The path to where the ot.bzs,ot.bzv and ot.bzz and nt files are</param>
    [DataContract(Name = "SearchReader")]
    [KnownType(typeof(ChapterPos))]
    [KnownType(typeof(BookPos))]
    [KnownType(typeof(VersePos))]
    public class SearchReader : BibleZtextReader
    {
        #region Fields

        [DataMember]
        public string displayText = string.Empty;
        [DataMember]
        public string displayTextHtmlBody = string.Empty;
        [DataMember]
        public int searchChapter = 0;
        [DataMember]
        public string searchText = string.Empty;
        [DataMember]
        public int searchTypeIndex = 0;

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

        #endregion Properties

        #region Methods

        public void doSearch(
            DisplaySettings displaySettings,
            int searchTypeIndex,
            string searchText,
            bool isIgnoreCase,
            List<int> chaptListToSearch,
            ShowProgress progress)
        {
            this.searchTypeIndex = searchTypeIndex;
            this.searchText = searchText;

            StringBuilder displayTextBody = new StringBuilder();
            Regex regex = new Regex(searchText, isIgnoreCase ? RegexOptions.IgnoreCase : RegexOptions.None);
            int numFoundMatches = 0;
            int i = 0;
            double lastProcent = 0;
            for (i = 0; i < chaptListToSearch.Count; i++)
            {
                byte[] chapterBuffer = getChapterBytes(chaptListToSearch[i]);
                string chapter = System.Text.UTF8Encoding.UTF8.GetString(chapterBuffer, 0, chapterBuffer.Length);
                var matches = regex.Matches(chapter);
                if (matches != null && matches.Count > 0)
                {
                    // now we must search each verse in the chapter
                    for (int j = 0; j < this.chapters[chaptListToSearch[i]].verses.Count; j++)
                    {
                        BibleZtextReader.VersePos verse = base.chapters[chaptListToSearch[i]].verses[j];
                        // clean up the verse and make sure the text is still there.
                        string textId = "CHAP_" + chaptListToSearch[i] + "_VERS_" + j;
                        string prefix = "<p><a name=\"" + textId +
                            "\"></a><a class=\"normalcolor\" href=\"#\" onclick=\"window.external.Notify('" +
                            textId + "'); event.returnValue=false; return false;\" >" + (displaySettings.smallVerseNumbers ? "<sup>" : "(") + base.getFullName(base.chapters[chaptListToSearch[i]].booknum) + " " +
                            (base.chapters[chaptListToSearch[i]].bookRelativeChapterNum + 1) + ":" +
                            (j + 1) + (displaySettings.smallVerseNumbers ? " </sup>" : ")");
                        string suffix = "</a></p><hr />";
                        int noteMarker = 'a';
                        string verseTxt = parseOsisText(displaySettings,
                            "",
                            "",
                            chapterBuffer,
                            (int)verse.startPos,
                            verse.length,
                            base.serial.isIsoEncoding,
                            false,
                            true,
                            ref noteMarker);
                        matches = regex.Matches(verseTxt);
                        Match lastMatch = null;
                        bool foundMatch = false;
                        foreach (Match match in matches)
                        {
                            // if the match goes over into the previous match then skip it
                            if (lastMatch != null && (match.Index > lastMatch.Index && match.Index < lastMatch.Index + lastMatch.Length))
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
                            displayTextBody.Append(prefix + verseTxt + suffix);
                            numFoundMatches++;
                        }
                    }
                }

                double procent = i * 100 / chaptListToSearch.Count;
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

            this.displayText = displayTextBody.ToString();
            progress(100, numFoundMatches, chaptListToSearch.Count > i, true);
            Debug.WriteLine("Done searching.");
        }

        public override void GetInfo(out int bookNum, out int absouteChaptNum, out int relChaptNum, out int verseNum, out string fullName, out string title)
        {
            verseNum = 0;
            absouteChaptNum = 0;
            bookNum = 0;
            relChaptNum = 0;
            fullName = "";
            string extraText = string.Empty;
            switch (searchTypeIndex)
            {
                case 0:
                    extraText = Translations.translate("Whole bible");
                    break;
                case 1:
                    extraText = Translations.translate("The Old Testement");
                    break;
                case 2:
                    extraText = Translations.translate("The New Testement");
                    break;
                case 3:
                    extraText = fullName;
                    break;
            }

            title = Translations.translate("Search") + "; " + searchText + "; " + extraText;
        }

        protected override string GetChapterHtml(DisplaySettings displaySettings, string htmlBackgroundColor, string htmlForegroundColor, string htmlPhoneAccentColor, double htmlFontSize, bool isNotesOnly, bool addStartFinishHtml = true)
        {
            //Debug.WriteLine("SearchReader GetChapterHtml.text=" + displayText);
            return HtmlHeader(displaySettings, htmlBackgroundColor, htmlForegroundColor, htmlPhoneAccentColor, htmlFontSize)
                + displayText + "</body></html>";
        }

        #endregion Methods
    }
}