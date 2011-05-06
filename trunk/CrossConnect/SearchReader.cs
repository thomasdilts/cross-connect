using System;
using System.Net;
using System.Collections.Generic;
using System.IO;
using ComponentAce.Compression.Libs.zlib;
using System.Globalization;
using System.Runtime.Serialization;
using System.IO.IsolatedStorage;
using SwordBackend;
using System.Text;
using System.Text.RegularExpressions;


namespace CrossConnect
{

    /// <summary>
    /// Load from a file all the book and verse pointers to the bzz file so that
    /// we can later read the bzz file quickly and efficiently.
    /// </summary>
    /// <param name="path">The path to where the ot.bzs,ot.bzv and ot.bzz and nt files are</param>
    [DataContract]
    [KnownType(typeof(ChapterPos))]
    [KnownType(typeof(BookPos))]
    [KnownType(typeof(VersePos))]
    public class SearchReader : BibleZtextReader
    {
        [DataMember]
        public string displayTextHtmlBody="";
        [DataMember]
        public int searchChapter = 0;
        [DataMember]
        public int searchTypeIndex = 0;
        [DataMember]
        public string searchText = "";
        [DataMember]
        public string displayText = "";
            
        public delegate void ShowProgress(double percent, int totalFound, bool isAbort);

        public SearchReader(string path,
            string iso2DigitLangCode,
            bool isIsoEncoding)
            : base(path, iso2DigitLangCode, isIsoEncoding)
        {
        }
        public void doSearch(
            int searchChapter, 
            int searchTypeIndex, 
            string searchText, 
            bool isIgnoreCase, 
            List<int> chapters,
            ShowProgress progress)
        {
            this.searchChapter = searchChapter;
            this.searchTypeIndex = searchTypeIndex;
            this.searchText = searchText;

            StringBuilder displayTextBody = new StringBuilder();
            Regex regex = new Regex(searchText, isIgnoreCase ? RegexOptions.IgnoreCase : RegexOptions.None);
            int numFoundMatches = 0;
            for (int i = 0; i < chapters.Count; i++)
            {
                byte[] chapterBuffer=getChapterBytes(i);
                string chapter = System.Text.UTF8Encoding.UTF8.GetString(chapterBuffer, 0, chapterBuffer.Length);
                var matches = regex.Matches(chapter);
                if (matches != null && matches.Count > 0)
                {
                    BibleZtextReader.VersePos lastVerse=new BibleZtextReader.VersePos();
                    lastVerse.startPos=-1;
                    foreach (Match match in matches)
                    {
                        BibleZtextReader.VersePos verse=new BibleZtextReader.VersePos();
                        //find the verse!
                        int j=0;
                        for(j=0;j<base.chapters[i].verses.Count;j++)
                        {
                            if((base.chapters[i].verses[j].startPos + base.chapters[i].verses[j].length >match.Index))
                            {
                                //we found it
                                verse = base.chapters[i].verses[j];
                                break;
                            }
                        }
                        //if it is the same as the last verse then skip it
                        if (lastVerse.startPos == verse.startPos)
                        {
                            continue;
                        }
                        lastVerse = verse;
                        //if the match goes over into the next verse then skip it
                        if (match.Index + match.Length > verse.startPos + verse.length)
                        {
                            continue;
                        }
                        //clean up the verse and make sure the text is still there.
                        string textId = "CHAP_" + i + "_VERS_" + j;
                        string prefix="<p><a name=\"" + textId +
                            "\"></a><a class=\"normalcolor\" href=\"#\" onclick=\"window.external.Notify('" +
                            textId + "'); event.returnValue=false; return false;\" ><sup>" + base.getFullName(base.chapters[i].booknum) + " " +
                            (base.chapters[i].bookRelativeChapterNum + 1) + ":" +
                            (j + 1) + " </sup>";
                        string suffix = "</a></p><hr />";
                        string verseTxt = base.parseOsisText(
                            "",
                            "",
                            chapterBuffer,
                            (int)verse.startPos,
                            verse.length,
                            base.isIsoEncoding,
                            false,
                            true) ;
                        var matches2 = regex.Matches(verseTxt);
                        if (matches2 != null && matches2.Count > 0)
                        {
                            for (int k = matches2.Count-1; k >=0; k--)
                            {
                                //make sure the matches don't overlap.
                                if (k != (matches2.Count - 1))
                                {
                                    if (matches2[k].Index + matches2[k].Length > matches2[k + 1].Index)
                                    {
                                        //the match overlaps, skip it
                                        continue;
                                    }
                                }
                                verseTxt = verseTxt.Substring(0, matches2[k].Index) + "<b>" + 
                                    verseTxt.Substring(matches2[k].Index, matches2[k].Length) + "</b>" + 
                                    verseTxt.Substring(matches2[k].Index+matches2[k].Length);
                            }
                            displayTextBody.Append(prefix + verseTxt + suffix);
                            numFoundMatches++;
                        }
                    }
                }
                progress(i * 100 / chapters.Count, numFoundMatches, false);
                if (numFoundMatches > 200)
                {
                    progress(i * 100 / chapters.Count, numFoundMatches, true);
                    break;
                }
            }
            displayText = displayTextBody.ToString();
        }

        public override bool isSynchronizeable { get { return false; } }
        public override bool isSearchable { get { return false; } }
        public override bool isPageable { get { return false; } }
        public override bool isBookmarkable { get { return false; } }

        public override void getInfo(int chaptNum, int verseNum, out int bookNum, out int relChaptNum, out string fullName, out string title)
        {
            base.getInfo(searchChapter, verseNum, out bookNum, out relChaptNum, out fullName, out title);
            string extraText="";
            switch (searchTypeIndex)
            {
                case 0:
                    extraText="Whole Bible";
                    break;
                case 1:
                    extraText="Old Testement";
                    break;
                case 2:
                    extraText="New Testement";
                    break;
                case 3:
                    extraText = fullName;
                    break;
            }
            title = "Search; " + searchText + "; " + extraText;
        }

        public override string GetChapterHtml(int chapterNumber, string htmlBackgroundColor, string htmlForegroundColor, string htmlPhoneAccentColor, double htmlFontSize)
        {
            string chapterStartHtml = HtmlHeader(htmlBackgroundColor, htmlForegroundColor, htmlPhoneAccentColor, htmlFontSize);
            return HtmlHeader(htmlBackgroundColor, htmlForegroundColor, htmlPhoneAccentColor, htmlFontSize)
                + displayText + "</body></html>";
        }
    }
}