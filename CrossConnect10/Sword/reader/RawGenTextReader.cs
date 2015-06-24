#region Header

// <copyright file="RawGenTextReader.cs" company="Thomas Dilts">
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

    [DataContract]
    public class RawGenTextPlaceMarker
    {
        #region Fields

        [DataMember(Name = "chapterNum")]
        public int ChapterNum = 1;

        [DataMember(Name = "when")]
        public DateTime When;

        #endregion

        #region Constructors and Destructors

        public RawGenTextPlaceMarker(int chapterNum, DateTime when)
        {
            this.ChapterNum = chapterNum;
            this.When = when;
        }

        #endregion

        #region Public Methods and Operators

        public string ToString()
        {
            return this.ChapterNum.ToString();
        }

        public static BiblePlaceMarker Clone(BiblePlaceMarker toClone)
        {
            var newMarker = new BiblePlaceMarker(toClone.BookShortName, toClone.ChapterNum, toClone.VerseNum, toClone.When)
                                {
                                    Note =
                                        toClone
                                        .Note
                                };
            return newMarker;
        }

        #endregion
    }

    /// <summary>
    ///     Load from a file all the book and verse pointers to the bzz file so that
    ///     we can later read the bzz file quickly and efficiently.
    /// </summary>
    [DataContract]
    public class RawGenTextReader : IBrowserTextSource
    {
        public class ChapterData
        {
            public string Title;

            public long PositionInBdt;

            public int NextBrother;

            public int PositionInChapters;

            public long NumCharacters;

            public ChapterData Parent = null;

            public List<ChapterData> Children = new List<ChapterData>();
        }

        protected static byte[] Prefix = Encoding.UTF8.GetBytes("<?xml version=\"1.0\" encoding=\"UTF-8\"?>\n<versee>");

        protected static byte[] PrefixIso =
            Encoding.UTF8.GetBytes("<?xml version=\"1.0\" encoding=\"ISO-8859-1\"?>\n<versee>");

        protected static byte[] Suffix = Encoding.UTF8.GetBytes("\n</versee>");


        #region Fields

        public List<ChapterData> Chapters = new List<ChapterData>();

        [DataMember(Name = "serial")]
        public BibleZtextReaderSerialData Serial = new BibleZtextReaderSerialData(
            false, string.Empty, string.Empty, 0, 0, string.Empty, string.Empty);

        private int _lastShownChapterNumber = -1;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Load from a file all the book and verse pointers to the bzz file so that
        ///     we can later read the bzz file quickly and efficiently.
        /// </summary>
        /// <param name="path">The path to where the ot.bzs,ot.bzv and ot.bzz and nt files are</param>
        /// <param name="iso2DigitLangCode"></param>
        /// <param name="isIsoEncoding"></param>
        public RawGenTextReader(string path, string iso2DigitLangCode, bool isIsoEncoding)
        {
            this.Serial.Iso2DigitLangCode = iso2DigitLangCode;
            this.Serial.Path = path;
            this.Serial.IsIsoEncoding = isIsoEncoding;
        }

        public async Task Initialize()
        {
            await this.ReloadSettingsFile();
            this.SetToFirstChapter();
        }

        #endregion

        #region Public Events

        public event WindowSourceChanged SourceChanged;

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
                return true;
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
                return true;
            }
        }

        public virtual bool IsSearchable
        {
            get
            {
                return true;
            }
        }

        public virtual bool IsSynchronizeable
        {
            get
            {
                return true;
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

        public async Task<IBrowserTextSource> Clone()
        {
            var cloned = new RawGenTextReader(this.Serial.Path, this.Serial.Iso2DigitLangCode, this.Serial.IsIsoEncoding);
            await cloned.Resume();
            cloned.MoveChapterVerse(this.Serial.PosBookShortName, this.Serial.PosChaptNum, this.Serial.PosVerseNum, false, cloned);
            return cloned;
        }

        public static async Task<bool> FileExists(string filePath)
        {
            try
            {
                StorageFile file = await ApplicationData.Current.LocalFolder.GetFileAsync(filePath.Replace("/", "\\"));
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        /*
        public static string HtmlHeader(
            DisplaySettings displaySettings,
            HtmlColorRgba htmlBackgroundColor,
            HtmlColorRgba htmlForegroundColor,
            HtmlColorRgba htmlPhoneAccentColor,
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
                    htmlForegroundColor));

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

</script>");
            head.Append("</head><body>");
            return head.ToString();
        }*/
        public static string HtmlHeader(
            DisplaySettings displaySettings,
            HtmlColorRgba htmlBackgroundColor,
            HtmlColorRgba htmlForegroundColor,
            HtmlColorRgba htmlPhoneAccentColor,
                    HtmlColorRgba htmlWordsOfChristColor,
            double htmlFontSize,
            string fontFamily,
            bool isSmallScreen)
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

</script>");
            head.Append("</head><body>");
            return head.ToString();
        }



        public virtual ButtonWindowSpecs GetButtonWindowSpecs(int stage, int lastSelectedButton,string isoLangCode)
        {
            switch (stage)
            {
                case 0:
                    {
                        // books
                        var colors = new List<int>();
                        var values = new List<int>();
                        var buttonNames = new List<string>();

                        for (int i = 1; i < Chapters.Count; i++)
                        {
                            if (Chapters[i].Parent == null)
                            {
                                colors.Add(0);
                                values.Add(i);
                                buttonNames.Add(Chapters[i].Title);
                            }                        
                        }

                        return new ButtonWindowSpecs(
                            stage,
                            "Select a chapter to view",
                            colors.Count,
                            colors.ToArray(),
                            buttonNames.ToArray(),
                            values.ToArray(),
                            ButtonSize.Large);
                    }
                default:
                    {
                        var parentChapter = Chapters[lastSelectedButton];

                        // set up the array for the chapter selection
                        int numOfChapters = parentChapter.Children.Count;

                        if (numOfChapters <= 0)
                        {
                            return null;
                        }

                        var butColors = new int[numOfChapters];
                        var values = new int[numOfChapters];
                        var butText = new string[numOfChapters];
                        for (int i = 0; i < numOfChapters; i++)
                        {
                            butColors[i] = 0;
                            butText[i] = parentChapter.Children[i].Title;
                            values[i] = parentChapter.Children[i].PositionInChapters;
                        }

                        // do a nice transition
                        return new ButtonWindowSpecs(
                            stage,
                            "Select a chapter to view",
                            numOfChapters,
                            butColors,
                            butText,
                            values,
                            ButtonSize.Large);
                    }
            }
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
            bool forceReload,
            bool isSmallScreen)
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
                    forceReload,
                    isSmallScreen);
        }

        /// <summary>
        ///     Return the entire chapter
        /// </summary>
        /// <param name="chapterNumber">Chaptern number beginning with zero for the first chapter and 1188 for the last chapter in the bible.</param>
        /// <returns>Entire Chapter</returns>
        public async Task<string> GetChapterRaw(int chapterNumber)
        {
            byte[] chapterBuffer = await this.GetChapterBytes(chapterNumber);
            string retValue = Encoding.UTF8.GetString(chapterBuffer, 0, chapterBuffer.Length);
            return retValue;
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
            if (this.Chapters.Count == 0)
            {
                return;
            }
            try
            {
                var chapt = this.Chapters[chapterNum];
                var parent = chapt;
                var titleText = string.Empty;
                while (parent != null)
                {
                    titleText = parent.Title + " ; " + titleText;
                    parent = parent.Parent;
                }

                fullName = chapt.Title;
                title = titleText;
            }
            catch (Exception ee)
            {
                Debug.WriteLine("BibleZtextReader.GetInfo; " + ee.Message);
            }
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
            try
            {
                if (!(source is RawGenTextReader))
                {
                    return false;
                }

                if (!((RawGenTextReader)source).Serial.Path.Equals(this.Serial.Path))
                {
                    return false;
                }

                // see if the chapter exists, if not, then don't do anything.
                if (this.Chapters != null && chapter < this.Chapters.Count && this.Chapters[chapter].NumCharacters > 0)
                {
                    var chapnum = verse != 0 && verse > chapter ? verse : chapter;
                    this.Serial.PosChaptNum = chapnum;
                    this.Serial.PosVerseNum = 0;
                    return true;
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("moveChapterVerse " + e.Message + " ; " + e.StackTrace);
            }

            return false;
        }

        public virtual void MoveNext(bool isVerse)
        {
            this.Serial.PosChaptNum++;
            this.Serial.PosVerseNum = 0;
            if (this.Serial.PosChaptNum >= this.Chapters.Count)
            {
                this.Serial.PosChaptNum = 0;
            }

            for (;
                this.Serial.PosChaptNum < this.Chapters.Count && this.Chapters[this.Serial.PosChaptNum].NumCharacters == 0;
                this.Serial.PosChaptNum++)
            {
            }

            if (this.Serial.PosChaptNum >= this.Chapters.Count)
            {
                this.Serial.PosChaptNum = 0;
            }
            else
            {
                return;
            }

            for (;
                this.Serial.PosChaptNum < this.Chapters.Count && this.Chapters[this.Serial.PosChaptNum].NumCharacters == 0;
                this.Serial.PosChaptNum++)
            {
            }
        }

        public virtual void MovePrevious(bool isVerse)
        {
            this.Serial.PosChaptNum--;
            this.Serial.PosVerseNum = 0;
            if (this.Serial.PosChaptNum < 0)
            {
                this.Serial.PosChaptNum = this.Chapters.Count - 1;
            }

            for (;
                this.Serial.PosChaptNum >= 0 && this.Chapters[this.Serial.PosChaptNum].NumCharacters == 0;
                this.Serial.PosChaptNum--)
            {
            }

            if (this.Serial.PosChaptNum < 0)
            {
                this.Serial.PosChaptNum = this.Chapters.Count - 1;
            }
            else
            {
                return;
            }

            for (;
                this.Serial.PosChaptNum >= 0 && this.Chapters[this.Serial.PosChaptNum].NumCharacters == 0;
                this.Serial.PosChaptNum--)
            {
            }
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
            Debug.WriteLine("putHtmlTofile start");

            ApplicationData appData = ApplicationData.Current;
            StorageFolder folder = await appData.LocalFolder.GetFolderAsync(filePath.Replace("/", "\\"));

            // Find a new file name.
            // Must change the file name, otherwise the browser may or may not update.
            string fileCreate = "web" + (int)(new Random().NextDouble() * 10000) + ".html";

            IReadOnlyList<StorageFile> files = await folder.CreateFileQuery().GetFilesAsync();

            // the name must be unique of course
            while (files.Any(p => p.Name.Equals(fileCreate)))
            {
                fileCreate = "web" + (int)(new Random().NextDouble() * 10000) + ".html";
            }

            // delete the old file
            string fileToErase = Path.GetFileName(fileErase);
            if (files.Any(p => p.Name.Equals(fileToErase)))
            {
                if (this.Serial.PosChaptNum == this._lastShownChapterNumber && !forceReload)
                {
                    // we dont need to rewrite everything. Just rename the file.
                    try
                    {
                        StorageFile fileRenaming = await folder.GetFileAsync(fileToErase);
                        await fileRenaming.RenameAsync(fileCreate);
                        return fileCreate;
                    }
                    catch (Exception ee)
                    {
                        // should never crash here but I have noticed any file rename is a risky business when you have more then one thread.
                        Debug.WriteLine("BibleZtextReader.putHtmlTofile; " + ee.Message);

                        // problems. lets just remake the file.
                    }
                }
                else
                {
                    try
                    {
                        StorageFile fileErasing = await folder.GetFileAsync(fileToErase);
                        await fileErasing.DeleteAsync();
                    }
                    catch (Exception ee)
                    {
                        // should never crash here but I have noticed any file delete is a risky business when you have more then one thread.
                        Debug.WriteLine("BibleZtextReader.putHtmlTofile; " + ee.Message);
                    }
                }
            }

            this._lastShownChapterNumber = this.Serial.PosChaptNum;

            StorageFile file = await folder.CreateFileAsync(fileCreate);
            Stream fs = await file.OpenStreamForWriteAsync();
            var tw = new StreamWriter(fs);
            string fileContent =
                await
                this.GetChapterHtml(
                    isoLangCode,
                    displaySettings,
                    htmlBackgroundColor,
                    htmlForegroundColor,
                    htmlPhoneAccentColor,
                    htmlWordsOfChristColor,
                    htmlHighlightColor,
                    htmlFontSize,
                    fontFamily,
                    false,
                    true,
                    forceReload,
                    false);
            tw.Write(fileContent);
            tw.Flush();
            tw.Dispose();
            fs.Dispose();

            Debug.WriteLine("putHtmlTofile end");
            return fileCreate;
        }

        public void RegisterUpdateEvent(WindowSourceChanged sourceChangedMethod, bool isRegister = true)
        {
            if (isRegister)
            {
                this.SourceChanged += sourceChangedMethod;
            }
            else
            {
                this.SourceChanged -= sourceChangedMethod;
            }
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

        protected static void AppendText(string text, StringBuilder plainText, StringBuilder noteText, bool isInElement)
        {
            if (!isInElement)
            {
                plainText.Append(text);
            }
            else
            {
                noteText.Append(text);
            }
        }

        protected int GetByteFromStream(Stream fs, out bool isEnd)
        {
            var buf = new byte[1];
            isEnd = fs.Read(buf, 0, 1) != 1;
            return buf[0];
        }

        protected async Task<byte[]> GetChapterBytes(int chapterNumber)
        {
            Debug.WriteLine("getChapterBytes start");
            var chapterdata = new byte[0];
            try
            {
                var chapt = Chapters[chapterNumber];
                if (chapt.NumCharacters > 100000)
                {
                    return new byte[0];
                }

                string filenameComplete = this.Serial.Path + ".bdt";
                var fs =
                    await
                    ApplicationData.Current.LocalFolder.OpenStreamForReadAsync(filenameComplete.Replace("/", "\\"));
                fs.Seek(chapt.PositionInBdt, SeekOrigin.Begin);
                chapterdata = new byte[chapt.NumCharacters];
                fs.Read(chapterdata, 0, (int)chapt.NumCharacters);
            }
            catch (Exception ee)
            {
                // does not exist
                return Encoding.UTF8.GetBytes("Does not exist");
            }
            return chapterdata;
        }

        public virtual async Task<string> GetTTCtext(bool isVerseOnly)
        {
            if (this.Chapters.Count == 0)
            {
                return string.Empty;
            }

            Debug.WriteLine("GetChapterHtml start");
            byte[] chapterBuffer = await this.GetChapterBytes(this.Serial.PosChaptNum);
            return RawGenTextReader.CleanXml(Encoding.UTF8.GetString(chapterBuffer, 0, chapterBuffer.Length), true).Replace(".", ". ");
        }

        /// <summary>
        ///     Return the entire chapter without notes and with lots of html markup
        /// </summary>
        /// <param name="displaySettings"></param>
        /// <param name="chapterNumber">Chaptern number beginning with zero for the first chapter and 1188 for the last chapter in the bible.</param>
        /// <param name="htmlBackgroundColor"></param>
        /// <param name="htmlForegroundColor"></param>
        /// <param name="htmlFontSize"></param>
        /// <param name="htmlPhoneAccentColor"></param>
        /// <param name="fontFamily"></param>
        /// <param name="isNotesOnly"></param>
        /// <param name="addStartFinishHtml"></param>
        /// <param name="forceReload"></param>
        /// <returns>Entire Chapter without notes and with lots of html markup for each verse</returns>
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
            bool forceReload,
            bool isSmallScreen)
        {
            if (this.Chapters.Count == 0)
            {
                return string.Empty;
            }

            Debug.WriteLine("GetChapterHtml start");
            byte[] chapterBuffer = await this.GetChapterBytes(chapterNumber);

            // for debug
            //string xxxxxx = Encoding.UTF8.GetString(chapterBuffer, 0, chapterBuffer.Length);
            //Debug.WriteLine("RawChapter: " + xxxxxx);
            var htmlChapter = new StringBuilder();
            var chapter = this.Chapters[chapterNumber];
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
                    fontFamily,
                    isSmallScreen);
                chapterEndHtml = "</body></html>";
                htmlChapter.Append(chapterStartHtml);
            }
            int noteIdentifier=0;
            bool isInPoetry = false;
            try
            {
                var texts = await BibleZtextReader.ParseOsisText(
                    displaySettings,
                    string.Empty,
                    string.Empty,
                    chapterBuffer,
                    0,
                    chapterBuffer.Count(),
                    this.Serial.IsIsoEncoding,
                    isNotesOnly,
                    false,
                    noteIdentifier,
                    isInPoetry);
                htmlChapter.Append(texts[0]);
            }
            catch (Exception e)
            {
                Debug.WriteLine(chapterBuffer.Count() + ";" + chapterNumber + ";" + e);
            }


            htmlChapter.Append(chapterEndHtml);
            Debug.WriteLine("GetChapterHtml end");
            return htmlChapter.ToString();
        }

        protected long GetInt48FromStream(Stream fs, out bool isEnd)
        {
            var buf = new byte[6];
            isEnd = fs.Read(buf, 0, 6) != 6;
            if (isEnd)
            {
                return 0;
            }

            return buf[1] * 0x100000000000 + buf[0] * 0x100000000 + buf[5] * 0x1000000 + buf[4] * 0x10000
                   + buf[3] * 0x100 + buf[2];
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

        public static string CleanXml(string xml, bool removeAllXml = false)
        {
            var cleaned = string.Empty;
            var parts = xml.Split("<>".ToCharArray());
            if (parts.Length > 1)
            {
                var partsAndSeperators = new string[parts.Length * 2 - 1];
                var characterCount = 0;
                for (int i = 0; i < parts.Length; i++)
                {
                    characterCount += parts[i].Length;
                    partsAndSeperators[i * 2] = parts[i];
                    if (i != (parts.Length - 1))
                    {
                        var seperationchar = xml.Substring(characterCount, 1);
                        partsAndSeperators[(i * 2) + 1] = seperationchar;
                    }
                    characterCount++;
                }

                var elements = new Dictionary<string, List<int>>();
                var startElementPos = -1;
                var partsValid = new bool[partsAndSeperators.Length];
                for (int i = 0; i < partsAndSeperators.Length; i++)
                {
                    partsValid[i] = true;
                    if (partsAndSeperators[i].Equals("<"))
                    {
                        if (startElementPos >= 0)
                        {
                            // double start.. this is really bad ignore last 2 parts
                            partsValid[i - 2] = false;
                            partsValid[i - 1] = false;
                        }
                        startElementPos = i;
                    }
                    else if (partsAndSeperators[i].Equals(">"))
                    {
                        if (startElementPos >= 0)
                        {
                            // now we have an element
                            var completeElement = partsAndSeperators[i - 1];
                            var name = completeElement.Trim()
                                   .Split(" ".ToCharArray());
                            if (completeElement.Trim().StartsWith("/"))
                            {
                                //we must match with a previous or ignore it.
                                List<int> foundElement;
                                if (elements.TryGetValue(name[0].Substring(1), out foundElement))
                                {
                                    // found a match, good..
                                    foundElement.RemoveAt(foundElement.Count - 1);
                                    if (foundElement.Count == 0)
                                    {
                                        elements.Remove(name[0].Substring(1));
                                    }
                                }
                                else
                                {
                                    // no matching element, delete this one.
                                    partsValid[i - 2] = false;
                                    partsValid[i - 1] = false;
                                    partsValid[i] = false;
                                }
                            }
                            else if (!completeElement.Trim().EndsWith("/"))
                            {
                                List<int> foundElement;
                                if (!elements.TryGetValue(name[0].Trim(), out foundElement))
                                {
                                    foundElement = new List<int>();
                                    elements[name[0].Trim()] = foundElement;
                                }

                                foundElement.Add(i);
                            }
                        }
                        else
                        {
                            // bad, close with no open
                            partsValid[i] = false;
                            partsValid[i - 1] = false;
                        }
                        startElementPos = -1;
                    }
                }

                if (startElementPos >= 0)
                {
                    // ended with an open element
                    partsValid[partsAndSeperators.Length - 1] = false;
                    partsValid[partsAndSeperators.Length - 2] = false;
                }

                foreach (var element in elements)
                {
                    foreach (var i in element.Value)
                    {
                        partsValid[i] = false;
                        partsValid[i - 1] = false;
                        partsValid[i - 2] = false;
                    }
                }

                // put it back together
                characterCount = 0;
                bool nowRemovingXml = false;
                bool noteRemoving = false;
                for (int i = 0; i < partsAndSeperators.Length; i++)
                {
                    if (partsValid[i])
                    {
                        if (removeAllXml)
                        {
                            if(partsAndSeperators[i].Equals("<"))
                            {
                                nowRemovingXml = true;
                                if ((i + 1) < partsAndSeperators.Length && ((partsAndSeperators[i + 1].StartsWith("note ") || partsAndSeperators[i + 1].StartsWith("title")) && !partsAndSeperators[i + 1].EndsWith("/")))
                                {
                                    noteRemoving = true;
                                }
                                else if ((i + 1) < partsAndSeperators.Length && noteRemoving &&(partsAndSeperators[i + 1].Equals("/note") || partsAndSeperators[i + 1].Equals("/title")))
                                {
                                    noteRemoving = false;
                                }
                            }

                            if (!nowRemovingXml && !noteRemoving)
                            {
                                // add an extra space to make sure there is seperation between words
                                cleaned += " " + partsAndSeperators[i];
                            }

                            if (partsAndSeperators[i].Equals(">"))
                            {
                                nowRemovingXml = false;
                            }
                        }
                        else
                        {
                            cleaned += partsAndSeperators[i];
                        }
                    }
                }
            }
            else
            {
                return xml;
            }
            return cleaned.Trim();
        }


        protected void RaiseSourceChangedEvent()
        {
            if (this.SourceChanged != null)
            {
                this.SourceChanged();
            }
        }

        private string ReadStringToNull(Stream fs)
        {
            var totalBuf = new List<byte>();
            var buf = new byte[1];
            while (fs.Read(buf, 0, 1) == 1)
            {
                if (buf[0] == 0)
                {
                    break;
                }
                totalBuf.Add(buf[0]);
            }
            var totBuf = totalBuf.ToArray();
            return Encoding.UTF8.GetString(totBuf, 0, totBuf.Count());
        }

        private async Task<bool> ReloadSettingsFile()
        {
            this.Chapters = new List<ChapterData>();
            if (string.IsNullOrEmpty(this.Serial.Path))
            {
                return false;
            }
            try
            {
                Stream fs =
                    await
                    ApplicationData.Current.LocalFolder.OpenStreamForReadAsync(
                        this.Serial.Path.Replace("/", "\\")  + ".idx");

                var indexes = new List<long>();
                bool isEnd;
                long index;
                while ((index = GetintFromStream(fs, out isEnd)) > -100 && !isEnd)
                {
                    indexes.Add(index);
                }
                fs.Dispose();

                fs =
                    await
                    ApplicationData.Current.LocalFolder.OpenStreamForReadAsync(
                        this.Serial.Path.Replace("/", "\\") + ".dat");
                Chapters = new List<ChapterData>();
                int id = 0;
                foreach (var i in indexes)
                {
                    fs.Seek(i, SeekOrigin.Begin);
                    // if 0, then no parent, otherwise parent pointer
                    var parentPointer = GetintFromStream(fs, out isEnd);
                    // pointer to next brotherChild or FFFF if last child
                    var nextBrother = GetintFromStream(fs, out isEnd);
                    // FFFF if childless, pointer to first child?
                    var firstChild = GetintFromStream(fs, out isEnd);
                    if (id > 0)
                    {
                        var chapterTitle = ReadStringToNull(fs);
                        var always8 = GetShortIntFromStream(fs, out isEnd);
                        var pointerToChapterInBdt = GetintFromStream(fs, out isEnd);
                        var numberCharInChapter = GetintFromStream(fs, out isEnd);
                        var chap = new ChapterData { NextBrother = (int)(nextBrother / 4), PositionInBdt = pointerToChapterInBdt, Title = chapterTitle, NumCharacters = numberCharInChapter, PositionInChapters = Chapters.Count };
                        Chapters.Add(chap);
                        if (parentPointer > 0)
                        {
                            chap.Parent = Chapters[(int)(parentPointer / 4)];
                            chap.Parent.Children.Add(chap);
                        }
                    }
                    else
                    {
                        //this is just a root dummy object
                        Chapters.Add(new ChapterData());
                    }
                    id++;
                }
                fs.Dispose();
            }
            catch (Exception ee)
            {
                
            }
            return true;
        }

        public virtual void SetToFirstChapter()
        {
            // find the first available chapter.
            this.Serial.PosChaptNum = 0;
            this.MoveNext(false);
            if (this.Serial.PosChaptNum == 1)
            {
                this.MovePrevious(false);
                if (this.Serial.PosChaptNum != 0)
                {
                    this.MoveNext(false);
                }
            }
        }

        private string convertNoteNumToId(int noteIdentifier)
        {
            string noteReturned = string.Empty;
            string startChar = ((char)((noteIdentifier - 'a') % 24 + 'a')).ToString();
            int numChars = (noteIdentifier - 'a') / 24;
            for (int i = 0; i <= numChars; i++)
            {
                noteReturned += startChar;
            }

            return "(" + noteReturned + ")";
        }

        #endregion
    }

}