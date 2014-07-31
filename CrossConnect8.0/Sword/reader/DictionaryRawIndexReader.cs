#region Header

// <copyright file="DictionaryRawReader.cs" company="Thomas Dilts">
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

namespace Sword.reader
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Runtime.Serialization;
    using System.Text;
    using System.Threading.Tasks;
    using System.Xml;

    //using ComponentAce.Compression.Libs.zlib;

    using Windows.Storage;

    /// <summary>
    ///     Load from a file all the book and verse pointers to the bzz file so that
    ///     we can later read the bzz file quickly and efficiently.
    /// </summary>
    [DataContract]
    public class DictionaryRawIndexReader : IBrowserTextSource
    {
        public class DictionaryEntry
        {
            public string Title;
            public long PositionInDat;
            public long Length;
        }

        protected static byte[] Prefix = Encoding.UTF8.GetBytes("<?xml version=\"1.0\" encoding=\"UTF-8\"?>\n<versee>");

        protected static byte[] PrefixIso =
            Encoding.UTF8.GetBytes("<?xml version=\"1.0\" encoding=\"ISO-8859-1\"?>\n<versee>");

        protected static byte[] Suffix = Encoding.UTF8.GetBytes("\n</versee>");


        #region Fields

        public List<DictionaryEntry> DictionaryEntries = new List<DictionaryEntry>();

        [DataMember(Name = "serial")]
        public BibleZtextReaderSerialData Serial = new BibleZtextReaderSerialData(
            false, string.Empty, string.Empty, 0, 0, string.Empty, string.Empty);

        [DataMember]
        public string WindowMatchingKey = string.Empty;

        private int _lastShownChapterNumber = -1;

        #endregion

        #region Constructors and Destructors

        public DictionaryRawIndexReader(string path, string iso2DigitLangCode, bool isIsoEncoding, string windowKey)
        {
            this.Serial.Iso2DigitLangCode = iso2DigitLangCode;
            this.Serial.Path = path;
            this.Serial.IsIsoEncoding = isIsoEncoding;
            this.WindowMatchingKey = windowKey;
        }

        public async Task Initialize()
        {
            await this.ReloadSettingsFile();
        }

        #endregion

        #region Public Properties

        public bool ExistsShortNames(string isoLangCode)
        {
            return false;
        }

        public virtual bool IsExternalLink
        {
            get
            {
                return false;
            }
        }

        public virtual bool IsHearable
        {
            get
            {
                return false;
            }
        }

        public virtual bool IsTTChearable
        {
            get
            {
                return false;
            }
        }

        public virtual bool IsLocalChangeDuringLink
        {
            get
            {
                return false;
            }
        }

        public virtual bool IsPageable
        {
            get
            {
                return false;
            }
        }

        public virtual bool IsSearchable
        {
            get
            {
                return false;
            }
        }

        public virtual bool IsSynchronizeable
        {
            get
            {
                return false;
            }
        }

        public virtual bool IsTranslateable
        {
            get
            {
                return false;
            }
        }

        public virtual bool IsLocked
        {
            get
            {
                return false;
            }
        }

        #endregion

        #region Public Methods and Operators

        public virtual async Task<string> GetTTCtext(bool isVerseOnly)
        {
            if (this.DictionaryEntries.Count == 0)
            {
                return string.Empty;
            }

            return string.Empty;
        }
        public async Task<IBrowserTextSource> Clone()
        {
            var cloned = new DictionaryRawIndexReader(this.Serial.Path, this.Serial.Iso2DigitLangCode, this.Serial.IsIsoEncoding, this.WindowMatchingKey);
            await cloned.Resume();
            cloned.MoveChapterVerse(this.Serial.PosBookShortName, this.Serial.PosChaptNum, this.Serial.PosVerseNum, false, cloned);
            return cloned;
        }

        public void RegisterUpdateEvent(WindowSourceChanged sourceChangedMethod, bool isRegister = true)
        {
        }
        public static string HtmlHeader(
            DisplaySettings displaySettings,
            HtmlColorRgba htmlBackgroundColor,
            HtmlColorRgba htmlForegroundColor,
            HtmlColorRgba htmlPhoneAccentColor,
            HtmlColorRgba htmlWordsOfChristColor,
            double htmlFontSize,
            string fontFamily)
        {
            var head = new StringBuilder();
            head.Append(
                "<html><head><meta http-equiv=\"Content-Type\" content=\"text/html; charset=UTF-8\" /><meta name=\"viewport\" content=\"width=device-width, initial-scale=1, maximum-scale=1\">");

            head.Append("<style>");

            head.Append(
                string.Format(
                    "body {{background:{0};color:{1};font-size:{2}pt;margin:0;padding:0;{3} }}",
                    htmlBackgroundColor.GetHtmlRgba(),
                    htmlForegroundColor.GetHtmlRgba(),
                    (int)(htmlFontSize + 0.5),
                    fontFamily)); // old fashioned way to round an integer

            head.Append(
                string.Format(
                    "sup,sub {{color:{0};font-size: .83em;}} "
                    + "a.strongsmorph,a.strongsmorph:link,span.strongsmorph{{color:{1};text-decoration:none;}} "
                    + "a.normalcolor,a.normalcolor:link {{color:{2};text-decoration:none;}}",
                    displaySettings.HighlightMarkings
                        ? htmlPhoneAccentColor.GetHtmlRgba()
                        : htmlForegroundColor.GetHtmlRgba(),
                    displaySettings.HighlightMarkings
                        ? htmlPhoneAccentColor.GetHtmlRgba()
                        : htmlForegroundColor.GetHtmlRgba(),
                    htmlForegroundColor.GetHtmlRgba()));

            head.Append(
                string.Format(
                    " a.normalcolor:link span.christ {{ color: {1}; }}  a.normalcolor span.christ:visited {{ color: {3}; }}  a.normalcolor span.christ:hover {{ color: {2}; }} a.normalcolor:hover {{ color: {0}; }} ",
                    htmlPhoneAccentColor.GetHtmlRgba(),
                    htmlWordsOfChristColor.GetHtmlRgba(),
                    htmlPhoneAccentColor.GetHtmlRgba(),
                    htmlPhoneAccentColor.GetHtmlRgba()));
            head.Append(@"
#footer {
position: fixed;
bottom: 0;
width: 100%;
background: lightgrey;		
text-align: center;
box-shadow: 0 0 15px #00214B;
}
");
            head.Append("</style>");
            head.Append(@"<script type=""text/javascript"">

function getVerticalScrollPosition() {
    return document.body.scrollTop.toString();
}
function setVerticalScrollPosition(position) {
    document.body.scrollTop = position;
}
function ScrollToAnchor(anchor, colorRgba) {
    window.location.hash=anchor;
    SetFontColorForElement(anchor, colorRgba);
}
function SetFontColorForElement(elemntId, colorRgba){
    var element = document.getElementById(elemntId);
    if(element!=null){
        element.style.color = colorRgba;
    }
}
function DoSearch(searchText){
	searchText = searchText.toUpperCase();
	var list = document.getElementsByTagName('a');
	var beginsWithElement = null;
	var searchTextLength= searchText.length;
	for (var i = 0; i < list.length; i++) {
		if(list[i].id.toUpperCase() == searchText){
			document.body.scrollTop = list[i].offsetTop;
			return;
		}
		if(beginsWithElement==null && list[i].id.toUpperCase().substring(0,searchTextLength) == searchText){
			beginsWithElement=list[i];
		}
	}
	if(beginsWithElement!=null){
		document.body.scrollTop = beginsWithElement.offsetTop;
	}
}
</script>");
            head.Append("</head><body>");
            return head.ToString();
        }



        public virtual ButtonWindowSpecs GetButtonWindowSpecs(int stage, int lastSelectedButton,string isoLangCode)
        {
            return new ButtonWindowSpecs();
        }
        
        public virtual async Task<string> GetChapterHtml(
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
            return
                await
                this.GetChapterHtml(
                    isoLangCode,
                    displaySettings,
                    this.Serial.PosChaptNum,
                    htmlBackgroundColor,
                    htmlForegroundColor,
                    htmlPhoneAccentColor,
                    htmlWordsOfChristColor,
                    htmlFontSize,
                    fontFamily,
                    isNotesOnly,
                    addStartFinishHtml,
                    forceReload);
        }

        public async Task<string> GetChapterRaw(int chapterNumber)
        {
            return string.Empty;
        }

        public virtual string GetExternalLink(DisplaySettings displaySettings)
        {
            return string.Empty;
        }

        public virtual void GetInfo(
            string isoLangCode,
            out string bookShortName,
            out int relChaptNum,
            out int verseNum,
            out string fullName,
            out string title)
        {
            verseNum = this.Serial.PosVerseNum;
            relChaptNum = this.Serial.PosChaptNum;
            bookShortName = this.Serial.PosBookShortName;
            this.GetInfo(isoLangCode, bookShortName,
                this.Serial.PosChaptNum, this.Serial.PosVerseNum, out fullName, out title);
        }

        public void GetInfo(string isoLangCode, string bookShortName,
            int chapterNum, int verseNum, out string fullName, out string title)
        {
            fullName = string.Empty;
            title = string.Empty;
        }

        public virtual string GetLanguage()
        {
            return this.Serial.Iso2DigitLangCode;
        }

        public virtual async Task<object[]> GetTranslateableTexts(string isoLangCode, DisplaySettings displaySettings, string bibleToLoad)
        {
            var toTranslate = new string[2];
            var isTranslateable = new bool[2];

            string bookShortName;
            int relChaptNum;
            string fullName;
            string titleText;
            int verseNum;

            this.GetInfo(isoLangCode, out bookShortName, out relChaptNum, out verseNum, out fullName, out titleText);
            string verseText = await this.GetVerseTextOnly(displaySettings, bookShortName, relChaptNum, verseNum);

            toTranslate[0] = "<p>" + fullName + " " + (relChaptNum + 1) + ":" + (verseNum + 1) + " - " + bibleToLoad
                             + "</p>";
            toTranslate[1] =
                verseText.Replace("<p>", string.Empty)
                         .Replace("</p>", string.Empty)
                         .Replace("<br />", string.Empty)
                         .Replace("\n", " ");
            isTranslateable[0] = false;
            isTranslateable[1] = true;
            return new object[] { toTranslate, isTranslateable };
        }

        /// <summary>
        ///     Return a raw verse.  This is a very ineffective function so use it with caution.
        /// </summary>
        /// <param name="displaySettings"></param>
        /// <param name="chapterNumber">Chaptern number beginning with zero for the first chapter and 1188 for the last chapter in the bible.</param>
        /// <param name="verseNumber">Verse number beginning with zero for the first verse</param>
        /// <returns>Verse raw</returns>
        public virtual async Task<string> GetVerseTextOnly(
            DisplaySettings displaySettings, string shortBookName, int chapterNumber, int verseNumber)
        {
            return string.Empty;
        }

        public async Task<List<string>> MakeListDisplayText(string isoLangCode,
            DisplaySettings displaySettings, List<BiblePlaceMarker> listToDisplay)
        {
            var returnList = new List<string>();

            return returnList;
        }

        public virtual bool MoveChapterVerse(string bookShortName, int chapter, int verse, bool isLocalLinkChange, IBrowserTextSource source)
        {
            return false;
        }

        public virtual void MoveNext(bool isVerse)
        {
        }

        public virtual void MovePrevious(bool isVerse)
        {
        }

        public async Task<string> PutHtmlTofile(
            string isoLangCode,
            DisplaySettings displaySettings,
            HtmlColorRgba htmlBackgroundColor,
            HtmlColorRgba htmlForegroundColor,
            HtmlColorRgba htmlPhoneAccentColor,
            HtmlColorRgba htmlWordsOfChristColor,
            HtmlColorRgba[] htmlHighlightColor,
            double htmlFontSize,
            string fontFamily,
            string fileErase,
            string filePath,
            bool forceReload)
        {
            return string.Empty;
        }

        public virtual async Task Resume()
        {
            await this.ReloadSettingsFile();
        }

        public virtual void SerialSave()
        {
        }

        #endregion

        #region Methods

        protected async Task<string> GetChapterHtml(
            string isoLangCode,
            DisplaySettings displaySettings,
            int chapterNumber,
            HtmlColorRgba htmlBackgroundColor,
            HtmlColorRgba htmlForegroundColor,
            HtmlColorRgba htmlPhoneAccentColor,
            HtmlColorRgba htmlWordsOfChristColor,
            double htmlFontSize,
            string fontFamily,
            bool isNotesOnly,
            bool addStartFinishHtml,
            bool forceReload)
        {
            if (this.DictionaryEntries.Count == 0)
            {
                return string.Empty;
            }

            Debug.WriteLine("GetChapterHtml start");

            // for debug
            //string xxxxxx = Encoding.UTF8.GetString(chapterBuffer, 0, chapterBuffer.Length);
            //Debug.WriteLine("RawChapter: " + xxxxxx);
            var htmlChapter = new StringBuilder();
            var chapter = this.DictionaryEntries[chapterNumber];
            string chapterStartHtml = string.Empty;
            string chapterEndHtml = string.Empty;
            if (addStartFinishHtml)
            {
                chapterStartHtml = HtmlHeader(
                    displaySettings,
                    htmlBackgroundColor,
                    htmlForegroundColor,
                    htmlPhoneAccentColor,
                    htmlWordsOfChristColor,
                    htmlFontSize,
                    fontFamily);
                chapterEndHtml = "</body></html>";
                

                htmlChapter.Append(chapterStartHtml);
            }

            foreach (var entry in this.DictionaryEntries)
            {
                htmlChapter.Append(string.Format("<a class=\"normalcolor\" id=\"ID_{1}\" href=\"#\" onclick=\"window.external.notify('{0}¤{1}_{2}_{3}'); event.returnValue=false; return false;\" >{1}</a><br />\r\n",
                    this.WindowMatchingKey, entry.Title.Replace("'", "´"), entry.PositionInDat, entry.Length));
            }

            htmlChapter.Append(chapterEndHtml);
            Debug.WriteLine("GetChapterHtml end");
            return htmlChapter.ToString();
        }

        protected int GetShortIntFromStream(Stream fs, out bool isEnd)
        {
            var buf = new byte[2];
            isEnd = fs.Read(buf, 0, 2) != 2;
            if (isEnd)
            {
                return 0;
            }

            return buf[1] * 0x100 + buf[0];
        }

        protected long GetintFromStream(Stream fs, out bool isEnd)
        {
            var buf = new byte[4];
            isEnd = fs.Read(buf, 0, 4) != 4;
            if (isEnd)
            {
                return 0;
            }

            return buf[3] * 0x100000 + buf[2] * 0x10000 + buf[1] * 0x100 + buf[0];
        }

        protected async Task<string> MakeListDisplayText(
            string isoLangCode,
            DisplaySettings displaySettings,
            List<BiblePlaceMarker> listToDisplay,
            HtmlColorRgba htmlBackgroundColor,
            HtmlColorRgba htmlForegroundColor,
            HtmlColorRgba htmlPhoneAccentColor,
            double htmlFontSize,
            string fontFamily,
            bool showBookTitles,
            string notesTitle)
        {
            return string.Empty;
        }

        protected string ReadStringToChar(Stream fs,char[] stopChars)
        {
            var totalBuf = new List<byte>();
            var buf = new byte[1];
            while (fs.Read(buf, 0, 1) == 1)
            {
                int i;
                for (i = 0; i < stopChars.Count(); i++)
                {
                    if (buf[0] == stopChars[i])
                    {
                        var totBuf2 = totalBuf.ToArray();
                        return Encoding.UTF8.GetString(totBuf2, 0, totBuf2.Count());
                    }
                }
                totalBuf.Add(buf[0]);
            }
            var totBuf = totalBuf.ToArray();
            return Encoding.UTF8.GetString(totBuf, 0, totBuf.Count());
        }


        protected virtual async Task<bool> ReloadSettingsFile()
        {
            this.DictionaryEntries = new List<DictionaryEntry>();
            if (string.IsNullOrEmpty(this.Serial.Path))
            {
                return false;
            }
            try
            {
                Stream fs =
                    await
                    ApplicationData.Current.LocalFolder.OpenStreamForReadAsync(
                        this.Serial.Path.Replace("/", "\\") + ".idx");
                bool isEnd;
                long index;

                while ((index = GetintFromStream(fs, out isEnd)) > -100 && !isEnd)
                {
                    var entry = new DictionaryEntry();
                    entry.PositionInDat = index;
                    entry.Length = GetShortIntFromStream(fs, out isEnd);
                    this.DictionaryEntries.Add(entry);
                }
                fs.Dispose();

                fs =
                    await
                    ApplicationData.Current.LocalFolder.OpenStreamForReadAsync(
                        this.Serial.Path.Replace("/", "\\") + ".dat");
                var stopChars = new char[]{'\r','\n',(char)0,'\\'};
                for (int i = 0; i < this.DictionaryEntries.Count; i++)
                {

                    fs.Seek(this.DictionaryEntries[i].PositionInDat, SeekOrigin.Begin);
                    //get the title only. Ignore the rest.
                    this.DictionaryEntries[i].Title = ReadStringToChar(fs, stopChars);
                }
                fs.Dispose();
            }
            catch (Exception ee)
            {

            }
            return true;
        }

        #endregion
    }

}