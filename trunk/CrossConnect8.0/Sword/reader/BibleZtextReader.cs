#region Header

// <copyright file="BibleZtextReader.cs" company="Thomas Dilts">
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

    using ComponentAce.Compression.Libs.zlib;

    using Windows.Storage;
    using Sword.versification;

    [DataContract]
    public class BiblePlaceMarker
    {
        #region Fields

        [DataMember(Name = "BookShortName")]
        public string BookShortName = string.Empty;

        [DataMember(Name = "chapterNum")]
        public int ChapterNum = 1;

        [DataMember(Name = "note")]
        public string Note = string.Empty;

        [DataMember(Name = "verseNum")]
        public int VerseNum = 1;

        [DataMember(Name = "when")]
        public DateTime When;

        #endregion

        #region Constructors and Destructors

        public BiblePlaceMarker(string bookShortName, int chapterNum, int verseNum, DateTime when)
        {
            this.BookShortName = bookShortName;
            this.ChapterNum = chapterNum;
            this.VerseNum = verseNum;
            this.When = when;
        }

        #endregion

        #region Public Methods and Operators

        public string ToString()
        {
            return this.ChapterNum + ";" + this.VerseNum;
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
    public class BibleZtextReader : IBrowserTextSource
    {
        #region Constants

        /// <summary>

        ///     * The configuration directory
        /// </summary>
        public const string DirConf = "mods.d";

        /// <summary>
        ///     * The data directory
        /// </summary>
        public const string DirData = "modules";

        /// <summary>
        ///     * Extension for config files
        /// </summary>
        public const string ExtensionConf = ".conf";

        /// <summary>
        ///     * Extension for data files
        /// </summary>
        public const string ExtensionData = ".dat";

        /// <summary>
        ///     * Extension for index files
        /// </summary>
        public const string ExtensionIndex = ".idx";

        /// <summary>
        ///     * Index file extensions
        /// </summary>
        public const string ExtensionVss = ".vss";

        /// <summary>
        ///     * New testament data files
        /// </summary>
        public const string FileNt = "nt";

        /// <summary>
        ///     * Old testament data files
        /// </summary>
        public const string FileOt = "ot";

        /// <summary>
        ///     Constant for the number of verses in the Bible
        /// </summary>
        //internal const short VersesInBible = 31102;

        protected const long SkipBookFlag = 68;

        #endregion

        #region Static Fields

        public static Dictionary<string, string> FontPropertiesStartHtml = new Dictionary<string, string> 
        { 
            { "acrostic", "<span style=\"text-shadow:1px 1px 5px white;\">" }, 
            { "bold", "<b>" }, 
            { "emphasis", "<em>" }, 
            { "illuminated", "<span style=\"text-shadow:1px 1px 5px white;\">" }, 
            { "italic", "<i>" }, 
            { "line-through", "<s>" }, 
            { "normal", "<span style=\"font-variant:normal;\">" }, 
            { "small-caps", "<span style=\"font-variant:small-caps;\">" }, 
            { "sub", "<sub>" }, 
            { "super", "<sup>" }, 
            { "underline", "<u>" } 
        };
        public static Dictionary<string, string> FontPropertiesEndHtml = new Dictionary<string, string>  
        { 
            { "acrostic", "</span>" }, 
            { "bold", "</b>" }, 
            { "emphasis", "</em>" }, 
            { "illuminated", "</span>" }, 
            { "italic", "</i>" }, 
            { "line-through", "</s>" }, 
            { "normal", "</span>" }, 
            { "small-caps", "</span>" }, 
            { "sub", "</sub>" }, 
            { "super", "</sup>" }, 
            { "underline", "</u>" } 
        };

        /// <summary>
        ///     Chapters divided into categories
        /// </summary>
        public static readonly Dictionary<string, int> ChapterCategories = new Dictionary<string, int> 
        {
            {"Gen",1},
            {"Exod",1},
            {"Lev",1},
            {"Num",1},
            {"Deut",1},
            {"Josh",2},
            {"Judg",2},
            {"Ruth",2},
            {"1Sam",2},
            {"2Sam",2},
            {"1Kgs",2},
            {"2Kgs",2},
            {"1Chr",2},
            {"2Chr",2},
            {"Ezra",2},
            {"Neh",2},
            {"Esth",2},
            {"Job",3},
            {"Ps",3},
            {"Prov",3},
            {"Eccl",3},
            {"Song",3},
            {"Isa",4},
            {"Jer",4},
            {"Lam",4},
            {"Ezek",4},
            {"Dan",4},
            {"Hos",5},
            {"Joel",5},
            {"Amos",5},
            {"Obad",5},
            {"Jonah",5},
            {"Mic",5},
            {"Nah",5},
            {"Hab",5},
            {"Zeph",5},
            {"Hag",5},
            {"Zech",5},
            {"Mal",5},
            {"1Esd",4},
            {"2Esd",4},
            {"Tob",4},
            {"Jdt",4},
            {"AddEsth",4},
            {"Wis",4},
            {"Sir",4},
            {"Bar",4},
            {"PrAzar",4},
            {"Sus",4},
            {"Bel",4},
            {"PrMan",4},
            {"1Macc",4},
            {"2Macc",4},
            {"EsthGr",4},
            {"AddPs",4},
            {"3Macc",4},
            {"4Macc",4},
            {"EpJer",4},
            {"AddDan",4},
            {"PssSol",4},
            {"1En",4},
            {"Odes",4},
            {"Matt",6},
            {"Mark",6},
            {"Luke",6},
            {"John",6},
            {"Acts",7},
            {"Rom",8},
            {"1Cor",8},
            {"2Cor",8},
            {"Gal",8},
            {"Eph",8},
            {"Phil",8},
            {"Col",8},
            {"1Thess",8},
            {"2Thess",8},
            {"1Tim",8},
            {"2Tim",8},
            {"Titus",8},
            {"Phlm",8},
            {"Heb",8},
            {"Jas",9},
            {"1Pet",9},
            {"2Pet",9},
            {"1John",9},
            {"2John",9},
            {"3John",9},
            {"Jude",9},
            {"Rev",10},
            {"EpLao",4}
            };

        protected static byte[] Prefix = Encoding.UTF8.GetBytes("<?xml version=\"1.0\" encoding=\"UTF-8\"?>\n<versee>");

        protected static byte[] PrefixIso =
            Encoding.UTF8.GetBytes("<?xml version=\"1.0\" encoding=\"ISO-8859-1\"?>\n<versee>");

        protected static byte[] Suffix = Encoding.UTF8.GetBytes("\n</versee>");

        #endregion

        #region Fields

        public List<ChapterPos> Chapters = new List<ChapterPos>();

        [DataMember(Name = "serial")]
        public BibleZtextReaderSerialData Serial = new BibleZtextReaderSerialData(
            false, string.Empty, string.Empty, 0, 0, string.Empty, string.Empty);

        protected IndexingBlockType BlockType = IndexingBlockType.Book;

        private BibleNames _bookNames = null;
        public BibleNames BookNames(string appChoosenIsoLangCode, bool reset = false)
        {
            if (this._bookNames == null || reset)
            {
                this._bookNames = new BibleNames(this.Serial.Iso2DigitLangCode, appChoosenIsoLangCode);
            }

            return this._bookNames;
        }

        private int _lastShownChapterNumber = -1;

        public Canon canon;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Load from a file all the book and verse pointers to the bzz file so that
        ///     we can later read the bzz file quickly and efficiently.
        /// </summary>
        /// <param name="path">The path to where the ot.bzs,ot.bzv and ot.bzz and nt files are</param>
        /// <param name="iso2DigitLangCode"></param>
        /// <param name="isIsoEncoding"></param>
        public BibleZtextReader(string path, string iso2DigitLangCode, bool isIsoEncoding, string cipherKey, string configPath, string versification)
        {
            this.Serial.Iso2DigitLangCode = iso2DigitLangCode;
            this.Serial.Path = path;
            this.Serial.IsIsoEncoding = isIsoEncoding;
            this.Serial.CipherKey = cipherKey;
            this.Serial.ConfigPath = configPath;
            this.Serial.Versification = versification;
            canon = CanonManager.GetCanon(this.Serial.Versification);
        }

        public async Task Initialize()
        {
            await this.ReloadSettingsFile();
            this.SetToFirstChapter();
        }

        public virtual async Task<IBrowserTextSource> Clone()
        {
            var cloned = new BibleZtextReader(this.Serial.Path, this.Serial.Iso2DigitLangCode, this.Serial.IsIsoEncoding, this.Serial.CipherKey, this.Serial.ConfigPath, this.Serial.Versification);
            await cloned.Resume();
            cloned.MoveChapterVerse(this.Serial.PosBookShortName, this.Serial.PosChaptNum, this.Serial.PosVerseNum, false, cloned);
            return cloned;
        }

        #endregion

        #region Public Events

        public event WindowSourceChanged SourceChanged;

        #endregion

        #region Enums

        protected enum IndexingBlockType
        {
            Book = 'b',

            Chapter = 'c'
        }

        #endregion

        #region Public Properties

        public bool ExistsShortNames(string appChoosenIsoLangCode)
        {
            return this.BookNames(appChoosenIsoLangCode).ExistsShortNames;

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
                return true;
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
                return true;
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
                return true;
            }
        }

        public virtual bool IsLocked
        {
            get
            {
                return this.Serial.CipherKey != null && this.Serial.CipherKey.Length == 0;
            }
        }

        #endregion

        #region Public Methods and Operators

        public bool ConvertOsisRefToAbsoluteChaptVerse(string osisRef, out string bookShortName, out int chaptNumLoc, out int verseNumLoc)
        {
            chaptNumLoc = 0;
            verseNumLoc = 0;
            bookShortName = string.Empty;
            if (osisRef.Contains(":"))
            {
                // remove everythign before :
                osisRef = osisRef.Substring(osisRef.IndexOf(":") + 1);
            }

            if (osisRef.Contains("@"))
            {
                // remove everythign after @
                osisRef = osisRef.Substring(0, osisRef.IndexOf("@"));
            }

            if (osisRef.Contains("-"))
            {
                // remove everythign after -
                osisRef = osisRef.Substring(0, osisRef.IndexOf("-"));
            }

            string[] osis = osisRef.Split(".".ToCharArray());
            if (osis.Length > 0)
            {
                CanonBookDef book = null;
                if (!canon.BookByShortName.TryGetValue(osis[0], out book))
                {
                    // try with tolower conversion
                    var osislower = osis[0].ToLower();
                    var key = canon.BookByShortName.Keys.FirstOrDefault(p => p.ToLower().Equals(osislower));
                    if (!string.IsNullOrEmpty(key))
                    {
                        book = canon.BookByShortName[key];
                    }
                }

                if (book != null)
                {
                    bookShortName = book.ShortName1;
                    if (osis.Length > 1)
                    {
                        int chapterRelative;
                        int.TryParse(osis[1], out chapterRelative);
                        chapterRelative--;
                        if (chapterRelative < 0)
                        {
                            chapterRelative = 0;
                        }
                        chaptNumLoc = chapterRelative;
                    }

                    if (osis.Length > 2)
                    {
                        int.TryParse(osis[2], out verseNumLoc);
                        verseNumLoc--;
                        if (verseNumLoc < 0)
                        {
                            verseNumLoc = 0;
                        }
                    }

                    return true;
                }

                return false;
            }

            return false;
        }

        public static async Task<bool> FileExists(StorageFolder folder, string filePath)
        {
            try
            {
                StorageFile file = await folder.GetFileAsync(filePath.Replace("/", "\\"));
                return true;
            }
            catch (Exception)
            {
                return false;
            }
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
                "<html><head><meta http-equiv=\"Content-Type\" content=\"text/html; charset=UTF-8\" /><meta name=\"viewport\" content=\"width=device-width, initial-scale=1, maximum-scale=1\" />");

            head.Append("<style>");
            head.Append(
                string.Format(
                    "body {{background:{0};color:{1};font-size:{2}pt;margin:{3};padding:0;{4} }}",
                    htmlBackgroundColor.GetHtmlRgba(),
                    htmlForegroundColor.GetHtmlRgba(),
                    (int)(htmlFontSize + 0.5),
                    displaySettings.MarginInsideTextWindow,
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
function ShowNode (elemntId) {
    var element = document.getElementById(""ID_"" + elemntId);
    document.documentElement.scrollTop = element.offsetTop;
}
function ShowNodePhone (elemntId) {
    var element = document.getElementById(""ID_"" + elemntId);
    document.body.scrollTop = element.offsetTop;
}
function ScrollToAnchor(anchor, colorRgba) {
    window.location.hash=anchor;
    SetFontColorForElement(anchor, colorRgba);
}
function SetFontColorForElement(elemntId, colorRgba){
    var element = document.getElementById(""ID_"" + elemntId);
    if(element!=null){
        element.style.color = colorRgba;
    }
}

</script>");
            head.Append("</head><body>");
            return head.ToString();
        }

        private bool ExistsBook(CanonBookDef book)
        {
            for (int i = 0; i < book.NumberOfChapters; i++)
            {
                if (this.Chapters[book.VersesInChapterStartIndex + i].Length != 0)
                {
                    return true;
                }
            }

            return false;
        }

        public virtual ButtonWindowSpecs GetButtonWindowSpecs(int stage, int lastSelectedButton, string appChoosenIsoLangCode)
        {
            switch (stage)
            {
                case 0:
                    {
                        // books
                        var colors = new List<int>();
                        var values = new List<int>();
                        var buttonNames = new List<string>();

                        // assumption. if the first chapter in the book does not exist then the book does not exist
                        int bookCounter = 0;
                        foreach (var book in canon.OldTestBooks)
                        {
                            if (ExistsBook(book))
                            {
                                colors.Add(ChapterCategories[book.ShortName1]);
                                values.Add(bookCounter);
                                buttonNames.Add(this.BookNames(appChoosenIsoLangCode).GetShortName(book.ShortName1));
                            }
                            bookCounter++;
                        }

                        foreach (var book in canon.NewTestBooks)
                        {
                            if (this.Chapters[book.VersesInChapterStartIndex].Length != 0)
                            {
                                colors.Add(ChapterCategories[book.ShortName1]);
                                values.Add(bookCounter);
                                buttonNames.Add(this.BookNames(appChoosenIsoLangCode).GetShortName(book.ShortName1));
                                bookCounter++;
                            }
                        }

                        return new ButtonWindowSpecs(
                            stage,
                            "Select a book to view",
                            colors.Count,
                            colors.ToArray(),
                            buttonNames.ToArray(),
                            values.ToArray(),
                            !this.BookNames(appChoosenIsoLangCode).ExistsShortNames ? ButtonSize.Large : ButtonSize.Medium);
                    }
                case 1:
                    {
                        CanonBookDef book = canon.GetBookFromBookNumber(lastSelectedButton);
                        //Chapters 

                        // set up the array for the chapter selection
                        int numOfChapters = book.NumberOfChapters;

                        if (numOfChapters <= 1)
                        {
                            return null;
                        }

                        var butColors = new List<int>();
                        var values = new List<int>();
                        var butText = new List<string>();
                        for (int i = 0; i < numOfChapters; i++)
                        {
                            if (!this.Chapters[book.VersesInChapterStartIndex + i].IsEmpty)
                            {
                                butColors.Add(0);
                                butText.Add((i + 1).ToString());
                                values.Add(book.VersesInChapterStartIndex + i);
                            }
                        }

                        // do a nice transition
                        return new ButtonWindowSpecs(
                            stage,
                            "Select a chapter to view",
                            butColors.Count(),
                            butColors.ToArray(),
                            butText.ToArray(),
                            values.ToArray(),
                            ButtonSize.Small);
                    }
                case 2:
                    {
                        // verses
                        int numOfVerses = canon.VersesInChapter[lastSelectedButton];

                        if (numOfVerses <= 1)
                        {
                            return null;
                        }

                        // Color butColor = (Color)Application.Current.Resources["PhoneForegroundColor"];
                        var butColors = new int[numOfVerses];
                        var values = new int[numOfVerses];
                        var butText = new string[numOfVerses];
                        for (int i = 0; i < numOfVerses; i++)
                        {
                            butColors[i] = 0;
                            butText[i] = (i + 1).ToString();
                            values[i] = i;
                        }

                        // do a nice transition
                        return new ButtonWindowSpecs(
                            stage, "Select a verse to view", numOfVerses, butColors, butText, values, ButtonSize.Small);
                    }
            }
            return null;
        }

        public virtual async Task<string> GetChapterHtml(
            string isoLangCode,
            DisplaySettings displaySettings,
            HtmlColorRgba htmlBackgroundColor,
            HtmlColorRgba htmlForegroundColor,
            HtmlColorRgba htmlPhoneAccentColor,
            HtmlColorRgba htmlWordsOfChristColor,
            HtmlColorRgba[] htmlHighlightingColor,
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
                    this.Serial.PosBookShortName,
                    this.Serial.PosChaptNum,
                    htmlBackgroundColor,
                    htmlForegroundColor,
                    htmlPhoneAccentColor,
                    htmlWordsOfChristColor,
                    htmlHighlightingColor,
                    htmlFontSize,
                    fontFamily,
                    isNotesOnly,
                    addStartFinishHtml,
                    forceReload);
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

        public string GetFullName(string bookShortName, string appChoosenIsoLangCode)
        {
            var book = canon.BookByShortName[bookShortName];
            return this.BookNames(appChoosenIsoLangCode).GetFullName(book.ShortName1, book.FullName);
        }

        public string GetFullName(int bookNum, string appChoosenIsoLangCode)
        {
            CanonBookDef book = canon.GetBookFromBookNumber(bookNum);
            return this.BookNames(appChoosenIsoLangCode).GetFullName(book.ShortName1, book.FullName);
        }

        public virtual void GetInfo(
            string isoLangCode,
            out string bookShortName,
            out int relChaptNum,
            out int verseNum,
            out string fullName,
            out string title)
        {
            bookShortName = this.Serial.PosBookShortName;
            relChaptNum = this.Serial.PosChaptNum;
            verseNum = this.Serial.PosVerseNum;
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

                fullName = GetFullName(bookShortName, isoLangCode);
                title = fullName + " " + (chapterNum + 1) + ":" + (verseNum + 1);
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

        public string GetShortName(int bookNum, string appChoosenIsoLangCode)
        {
            CanonBookDef book = null;
            if (bookNum >= canon.OldTestBooks.Count())
            {
                book = canon.NewTestBooks[bookNum - canon.OldTestBooks.Count()];
            }
            else
            {
                book = canon.OldTestBooks[bookNum];
            }

            return this.BookNames(appChoosenIsoLangCode).GetShortName(book.ShortName1);
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
            CanonBookDef book;
            if (!canon.BookByShortName.TryGetValue(shortBookName, out book) || chapterNumber >= book.NumberOfChapters || verseNumber >= canon.VersesInChapter[chapterNumber + book.VersesInChapterStartIndex])
            {
                return string.Empty;
            }

            byte[] chapterBuffer = await this.GetChapterBytes(chapterNumber + book.VersesInChapterStartIndex);

            // debug only
            // string all = System.Text.UTF8Encoding.UTF8.GetString(chapterBuffer, 0, chapterBuffer.Length);
            VersePos verse = this.Chapters[book.VersesInChapterStartIndex + chapterNumber].Verses[verseNumber];
            int noteMarker = 0;
            bool isInPoetry = false;
            return this.ParseOsisText(
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
                ref isInPoetry,
                true);
        }

        public virtual async Task<string> GetTTCtext(bool isVerseOnly)
        {
            CanonBookDef book;
            if (!canon.BookByShortName.TryGetValue(this.Serial.PosBookShortName, out book))
            {
                return string.Empty;
            }

            var chapterBuffer = await this.GetChapterBytes(this.Serial.PosChaptNum + book.VersesInChapterStartIndex);

            if (isVerseOnly)
            {
                VersePos verse = this.Chapters[book.VersesInChapterStartIndex + this.Serial.PosChaptNum].Verses[this.Serial.PosVerseNum];
                if (verse.Length == 0)
                {
                    return string.Empty;
                }

                if (verse.StartPos >= chapterBuffer.Length || (verse.StartPos + verse.Length) > chapterBuffer.Length)
                {
                    return " POSSIBLE ERROR IN BIBLE, TEXT MISSING HERE ";
                }
                return RawGenTextReader.CleanXml(Encoding.UTF8.GetString(chapterBuffer, (int)verse.StartPos, (int)verse.Length), true).Replace(".", ". ");
            }
            else
            {
                return RawGenTextReader.CleanXml(Encoding.UTF8.GetString(chapterBuffer, 0, chapterBuffer.Length), true).Replace(".", ". ");
            }
        }

        public virtual async Task<string> GetVerseTextOnly(DisplaySettings displaySettings, string bookShortName, int chapterNumber)
        {
            CanonBookDef book;
            if (!canon.BookByShortName.TryGetValue(bookShortName, out book) || chapterNumber >= book.NumberOfChapters)
            {
                return string.Empty;
            }

            //give them the notes if you can.

            var chapterBuffer = await this.GetChapterBytes(chapterNumber + book.VersesInChapterStartIndex);
            return RawGenTextReader.CleanXml(Encoding.UTF8.GetString(chapterBuffer, 0, chapterBuffer.Length), true);
        }
        public async Task<List<string>> MakeListDisplayText(string appChoosenIsoLangCode,
            DisplaySettings displaySettings, List<BiblePlaceMarker> listToDisplay)
        {
            var returnList = new List<string>();
            bool isInPoetry = false;
            for (int j = listToDisplay.Count - 1; j >= 0; j--)
            {
                BiblePlaceMarker place = listToDisplay[j];
                var book = canon.BookByShortName[place.BookShortName];
                ChapterPos chaptPos = this.Chapters[place.ChapterNum + book.VersesInChapterStartIndex];
                byte[] chapterBuffer = await this.GetChapterBytes(place.ChapterNum + book.VersesInChapterStartIndex);

                ChapterPos versesForChapterPositions = this.Chapters[place.ChapterNum + book.VersesInChapterStartIndex];

                VersePos verse = versesForChapterPositions.Verses[place.VerseNum];
                int noteMarker = 0;
                string verseTxt = this.ParseOsisText(
                    displaySettings,
                    this.GetFullName(chaptPos.Booknum, appChoosenIsoLangCode) + " " + (chaptPos.BookRelativeChapterNum + 1) + ":"
                    + (place.VerseNum + 1) + "  " + place.When.ToString("yyyy-MM-dd") + " "
                    + place.When.ToString("hh.mm.ss") + "---",
                    string.Empty,
                    chapterBuffer,
                    (int)verse.StartPos,
                    verse.Length,
                    this.Serial.IsIsoEncoding,
                    false,
                    true,
                    ref noteMarker,
                    ref isInPoetry);
                returnList.Add(verseTxt);
            }

            return returnList;
        }

        public virtual void MoveChapterVerse(string bookShortName, int chapter, int verse, bool isLocalLinkChange, IBrowserTextSource source)
        {
            if (!(source is BibleZtextReader))
            {
                return;
            }

            try
            {
                // see if the chapter exists, if not, then don't do anything.
                CanonBookDef book;
                if (canon.BookByShortName.TryGetValue(bookShortName, out book) && this.Chapters != null && chapter < book.NumberOfChapters && !this.Chapters[book.VersesInChapterStartIndex + chapter].IsEmpty)
                {
                    this.Serial.PosBookShortName = bookShortName;
                    this.Serial.PosChaptNum = chapter;
                    this.Serial.PosVerseNum = canon.VersesInChapter[book.VersesInChapterStartIndex + chapter] > verse
                        ? verse : (canon.VersesInChapter[book.VersesInChapterStartIndex + chapter] - 1);
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("moveChapterVerse " + e.Message + " ; " + e.StackTrace);
            }
        }

        public virtual void MoveNext(bool isVerseMove)
        {
            if (this.Serial == null || canon == null)
            {
                return;
            }

            var book = canon.BookByShortName[this.Serial.PosBookShortName];
            if (isVerseMove)
            {
                int nextVerse = this.Serial.PosVerseNum + 1;
                if (nextVerse >= canon.VersesInChapter[book.VersesInChapterStartIndex + this.Serial.PosChaptNum])
                {
                    this.MoveNext(false);
                }
                else
                {
                    this.MoveChapterVerse(this.Serial.PosBookShortName, this.Serial.PosChaptNum, nextVerse, false, this);
                }

                return;
            }

            int nextChapter = this.Serial.PosChaptNum + 1;
            if (nextChapter >= book.NumberOfChapters)
            {
                var absChapter = (book.NumberOfChapters + book.VersesInChapterStartIndex + 1) > this.Chapters.Count() ? 0 : book.NumberOfChapters + book.VersesInChapterStartIndex;
                var absAvailChapter = GetAvailableChapter(absChapter);
                var newBook = canon.GetBookFromAbsoluteChapter(absAvailChapter);
                this.MoveChapterVerse(newBook.ShortName1, absAvailChapter - newBook.VersesInChapterStartIndex, 0, false, this);
            }
            else
            {
                var absAvailChapter = GetAvailableChapter(book.VersesInChapterStartIndex + nextChapter);
                var newBook = canon.GetBookFromAbsoluteChapter(absAvailChapter);
                this.MoveChapterVerse(newBook.ShortName1, absAvailChapter - newBook.VersesInChapterStartIndex, 0, false, this);
            }
        }

        public virtual void MovePrevious(bool isVerseMove)
        {
            var book = canon.BookByShortName[this.Serial.PosBookShortName];
            if (isVerseMove)
            {
                int nextVerse = this.Serial.PosVerseNum - 1;
                if (nextVerse < 0)
                {
                    this.MovePrevious(false);
                }
                else
                {
                    this.MoveChapterVerse(this.Serial.PosBookShortName, this.Serial.PosChaptNum, nextVerse, false, this);
                }

                return;
            }

            int prevChapter = this.Serial.PosChaptNum - 1;
            if (prevChapter < 0)
            {
                var absChapter = book.VersesInChapterStartIndex == 0 ? this.Chapters.Count() - 1 : book.VersesInChapterStartIndex - 1;
                var absAvailChapter = GetAvailableChapter(absChapter, true);
                var newBook = canon.GetBookFromAbsoluteChapter(absAvailChapter);
                this.MoveChapterVerse(newBook.ShortName1, absAvailChapter - newBook.VersesInChapterStartIndex, 0, false, this);
            }
            else
            {
                var absAvailChapter = GetAvailableChapter(book.VersesInChapterStartIndex + prevChapter);
                var newBook = canon.GetBookFromAbsoluteChapter(absAvailChapter);
                this.MoveChapterVerse(newBook.ShortName1, absAvailChapter - newBook.VersesInChapterStartIndex, 0, false, this);
            }
        }
        private int GetAvailableChapter(int chapterNumber, bool IsReverseFind = false)
        {
            int returnChapterNum = chapterNumber;
            if (this.Chapters[chapterNumber].IsEmpty)
            {
                if (IsReverseFind)
                {
                    int i;
                    for (i = chapterNumber; i >= 0 && this.Chapters[i].IsEmpty; i--) ;
                    if (i < 0)
                    {
                        for (i = this.Chapters.Count() - 1; i >= 0 && this.Chapters[i].IsEmpty; i--) ;
                        if (i < 0)
                        {
                            i = 0;
                        }
                    }
                    returnChapterNum = i;
                }
                else
                {
                    int i;
                    for (i = chapterNumber; i < this.Chapters.Count() && this.Chapters[i].IsEmpty; i++) ;
                    if (i >= this.Chapters.Count())
                    {
                        for (i = 0; i < this.Chapters.Count() && this.Chapters[i].IsEmpty; i++) ;
                        if (i >= this.Chapters.Count())
                        {
                            i = 0;
                        }
                    }
                    returnChapterNum = i;
                }
            }

            return returnChapterNum;
        }

        public async Task<string> PutHtmlTofile(
            string isoLangCode,
            DisplaySettings displaySettings,
            HtmlColorRgba htmlBackgroundColor,
            HtmlColorRgba htmlForegroundColor,
            HtmlColorRgba htmlPhoneAccentColor,
            HtmlColorRgba htmlWordsOfChristColor,
            HtmlColorRgba[] htmlHighlightingColor,
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
            IReadOnlyList<StorageFile> files = null;
            try
            {
                files = await folder.CreateFileQuery().GetFilesAsync();

                // the name must be unique of course
                while (files.Any(p => p.Name.Equals(fileCreate)))
                {
                    fileCreate = "web" + (int)(new Random().NextDouble() * 10000) + ".html";
                }
            }
            catch (Exception)
            {
            }

            // delete the old file
            string fileToErase = Path.GetFileName(fileErase);
            if (files != null && files.Any(p => p.Name.Equals(fileToErase)))
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
                    htmlHighlightingColor,
                    htmlFontSize,
                    fontFamily,
                    false,
                    true,
                    forceReload);
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

        protected string[] GetAllShortNames(string appChoosenIsoLangCode)
        {
            return this.BookNames(appChoosenIsoLangCode).GetAllShortNames();
        }

        protected int GetByteFromStream(Stream fs, out bool isEnd)
        {
            var buf = new byte[1];
            isEnd = fs.Read(buf, 0, 1) != 1;
            return buf[0];
        }

        protected async Task<byte[]> GetChapterBytes(int chapterNumber)
        {
            //Debug.WriteLine("getChapterBytes start");
            int numberOfChapters = this.Chapters.Count;
            if (numberOfChapters == 0)
            {
                return Encoding.UTF8.GetBytes("Does not exist");
            }
            if (chapterNumber >= numberOfChapters)
            {
                chapterNumber = numberOfChapters - 1;
            }

            if (chapterNumber < 0)
            {
                chapterNumber = 0;
            }

            ChapterPos versesForChapterPositions = this.Chapters[chapterNumber];
            long bookStartPos = versesForChapterPositions.BookStartPos;
            long blockStartPos = versesForChapterPositions.StartPos;
            long blockLen = versesForChapterPositions.Length;
            Stream fs;
            var book = canon.OldTestBooks[canon.OldTestBooks.Count() - 1];
            string fileName = (chapterNumber < (book.VersesInChapterStartIndex + book.NumberOfChapters)) ? "ot." : "nt.";
            try
            {
                //Windows.Storage.ApplicationData appData = Windows.Storage.ApplicationData.Current;
                //var folder = await appData.LocalFolder.GetFolderAsync(Serial.Path.Replace("/", "\\"));
                string filenameComplete = this.Serial.Path + fileName + ((char)this.BlockType) + "zz";
                fs =
                    await
                    ApplicationData.Current.LocalFolder.OpenStreamForReadAsync(filenameComplete.Replace("/", "\\"));
                //fs = await file.OpenStreamForReadAsync(); 
            }
            catch (Exception ee)
            {
                // does not exist
                return Encoding.UTF8.GetBytes("Does not exist");
            }

            // adjust the start postion of the stream to where this book begins.
            // we must read the entire book up to the chapter we want even though we just want one chapter.
            fs.Position = bookStartPos;
            ZInputStream zipStream;
            zipStream = string.IsNullOrEmpty(this.Serial.CipherKey) ? new ZInputStream(fs) : new ZInputStream(new SapphireStream(fs, this.Serial.CipherKey));

            var chapterBuffer = new byte[blockLen];
            int totalBytesRead = 0;
            int totalBytesCopied = 0;
            int len = 0;
            try
            {
                var buffer = new byte[10000];
                while (true)
                {
                    try
                    {
                        len = zipStream.read(buffer, 0, 10000);
                    }
                    catch (Exception ee)
                    {
                        Debug.WriteLine("caught a unzip crash 4.2" + ee);
                    }

                    if (len <= 0)
                    {
                        // we should never come to this point.  Just here as a safety procaution
                        break;
                    }

                    totalBytesRead += len;
                    if (totalBytesRead >= blockStartPos)
                    {
                        // we are now inside of where the chapter we want is so we need to start saving it.
                        int startOffset = 0;
                        if (totalBytesCopied == 0)
                        {
                            // but our actual chapter might begin in the middle of the buffer.  Find the offset from the
                            // beginning of the buffer.
                            startOffset = len - (totalBytesRead - (int)blockStartPos);
                        }

                        int i;
                        for (i = totalBytesCopied; i < blockLen && (i - totalBytesCopied) < (len - startOffset); i++)
                        {
                            chapterBuffer[i] = buffer[i - totalBytesCopied + startOffset];
                        }

                        totalBytesCopied += len - startOffset;
                        if (totalBytesCopied >= blockLen)
                        {
                            // we are done. no more reason to read anymore of this book stream, just get out.
                            break;
                        }
                    }
                }
            }
            catch (Exception ee)
            {
                Debug.WriteLine("BibleZtextReader.getChapterBytes crash; " + ee.Message);
            }

            fs.Dispose();
            zipStream.Dispose();
            return chapterBuffer;
        }

        public async Task<bool> IsCipherKeyGood(string testKey)
        {
            var oldCipher = this.Serial.CipherKey;
            this.Serial.CipherKey = testKey;
            var book = canon.BookByShortName[this.Serial.PosBookShortName];
            byte[] chapterBuffer = await this.GetChapterBytes(this.Serial.PosChaptNum + book.VersesInChapterStartIndex);
            this.Serial.CipherKey = oldCipher;
            return chapterBuffer.Any(p => p != 0);
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
            string bookShortName,
            int chapterNumber,
            HtmlColorRgba htmlBackgroundColor,
            HtmlColorRgba htmlForegroundColor,
            HtmlColorRgba htmlPhoneAccentColor,
            HtmlColorRgba htmlWordsOfChristColor,
            HtmlColorRgba[] htmlHighlightingColor,
            double htmlFontSize,
            string fontFamily,
            bool isNotesOnly,
            bool addStartFinishHtml,
            bool forceReload)
        {
            if (this.Chapters.Count == 0)
            {
                return string.Empty;
            }

            Debug.WriteLine("GetChapterHtml start");
            var book = canon.BookByShortName[bookShortName];
            var htmlChapter = new StringBuilder();
            ChapterPos versesForChapterPositions = this.Chapters[chapterNumber + book.VersesInChapterStartIndex];
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
            }

            string bookName = string.Empty;
            if (displaySettings.ShowBookName)
            {
                bookName = this.GetFullName(versesForChapterPositions.Booknum, isoLangCode);
            }

            bool isVerseMarking = displaySettings.ShowBookName || displaySettings.ShowChapterNumber
                                  || displaySettings.ShowVerseNumber;
            string startVerseMarking = displaySettings.SmallVerseNumbers
                                           ? "<sup>"
                                           : (isVerseMarking ? "<span class=\"strongsmorph\">(" : string.Empty);
            string stopVerseMarking = displaySettings.SmallVerseNumbers
                                          ? "</sup>"
                                          : (isVerseMarking ? ")</span>" : string.Empty);
            int noteIdentifier = 0;

            // in some commentaries, the verses repeat. Stop these repeats from comming in!
            var verseRepeatCheck = new Dictionary<long, int>();
            bool isInPoetry = false;

            // if the bible is locked and there is no key. Look for a key.
            if (this.Serial.CipherKey != null && this.Serial.CipherKey.Length == 0)
            {
                try
                {
                    string filenameComplete = this.Serial.Path + "CipherKey.txt";
                    var fs =
                        await
                        ApplicationData.Current.LocalFolder.OpenStreamForReadAsync(filenameComplete.Replace("/", "\\"));
                    // get the key from the file.
                    var buf = new byte[1000];
                    var len = await fs.ReadAsync(buf, 0, 1000);
                    this.Serial.CipherKey = Encoding.UTF8.GetString(buf, 0, len);
                }
                catch (Exception ee)
                {
                }
                if (this.Serial.CipherKey.Length == 0)
                {
                    try
                    {
                        string filenameComplete = this.Serial.ConfigPath;
                        var fs =
                            await
                            ApplicationData.Current.LocalFolder.OpenStreamForReadAsync(
                                this.Serial.ConfigPath.Replace("/", "\\"));
                        // show the about information instead
                        var config = new SwordBookMetaData(fs, "xx");
                        fs.Dispose();
                        return chapterStartHtml + "This bible is locked. Go to the menu to enter the key.<br /><br />"
                               + ((string)config.GetCetProperty(ConfigEntryType.About)).Replace("\\par", "<br />")
                                                                                       .Replace("\\qc", "")
                               + chapterEndHtml;
                    }
                    catch (Exception e)
                    {
                        // does not exist
                    }
                }
            }
            byte[] chapterBuffer = await this.GetChapterBytes(chapterNumber + book.VersesInChapterStartIndex);

            // for debug
            //string xxxxxx = Encoding.UTF8.GetString(chapterBuffer, 0, chapterBuffer.Length);
            //Debug.WriteLine("RawChapter: " + xxxxxx);

            for (int i = 0; i < versesForChapterPositions.Verses.Count; i++)
            {
                VersePos verse = versesForChapterPositions.Verses[i];
                string htmlChapterText = startVerseMarking
                                         + (displaySettings.ShowBookName ? bookName + " " : string.Empty)
                                         + (displaySettings.ShowChapterNumber
                                                ? ((versesForChapterPositions.BookRelativeChapterNum + 1) + ":")
                                                : string.Empty)
                                         + (displaySettings.ShowVerseNumber ? (i + 1).ToString() : string.Empty)
                                         + stopVerseMarking;
                string verseTxt;
                string id = bookShortName + "_" + chapterNumber + "_" + i;
                string restartText = "<a name=\"" + id + "\"></a><a " + GetHighlightStyle(displaySettings, htmlHighlightingColor, bookShortName, chapterNumber, i) + " class=\"normalcolor\" id=\"ID_" + id
                                     + "\" href=\"#\" onclick=\"window.external.notify('" + id
                                     + "'); event.returnValue=false; return false;\" > ";
                string startText = htmlChapterText + restartText;
                if (!verseRepeatCheck.ContainsKey(verse.StartPos))
                {
                    verseRepeatCheck[verse.StartPos] = 0;

                    verseTxt = "*** ERROR ***";
                    try
                    {
                        verseTxt = this.ParseOsisText(
                            displaySettings,
                            startText,
                            restartText,
                            chapterBuffer,
                            (int)verse.StartPos,
                            verse.Length,
                            this.Serial.IsIsoEncoding,
                            isNotesOnly,
                            false,
                            ref noteIdentifier,
                            ref isInPoetry);
                        if (isInPoetry && (i == versesForChapterPositions.Verses.Count - 1))
                        {
                            // we must end the indentations
                            verseTxt += "</blockquote>";
                        }
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine(verse.Length + ";" + verse.StartPos + ";" + e);
                    }
                }
                else
                {
                    verseTxt = "<p>" + startText + "</p>";
                }

                // create the verse
                htmlChapter.Append(
                    (displaySettings.EachVerseNewLine ? "<p>" : string.Empty) + chapterStartHtml + verseTxt
                    + (verseTxt.Length > 0 ? (displaySettings.EachVerseNewLine ? "</a></p>" : "</a>") : string.Empty));
                chapterStartHtml = string.Empty;
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

        private string GetHighlightStyle(DisplaySettings displaySettings, HtmlColorRgba[] htmlHighlightingColor, string book, int chapter, int verse)
        {
            var style = string.Empty;
            var highlight = displaySettings.highlighter.GetHighlightForVerse(book, chapter, verse);
            if (highlight == Highlighter.Highlight.COLOR_NONE)
            {
                return string.Empty;
            }
            return "style=\"background-color:" + htmlHighlightingColor[(int)highlight].GetHtmlRgba() + ";\"";
        }

        protected async Task<string> MakeListTtcHearingText(string appChoosenIsoLangCode,
            List<BiblePlaceMarker> listToDisplay)
        {

            var listText = new StringBuilder();
            for (int j = listToDisplay.Count - 1; j >= 0; j--)
            {
                BiblePlaceMarker place = listToDisplay[j];
                CanonBookDef book;
                if (!canon.BookByShortName.TryGetValue(place.BookShortName, out book) || place.ChapterNum >= book.NumberOfChapters || place.VerseNum >= canon.VersesInChapter[place.ChapterNum + book.VersesInChapterStartIndex])
                {
                    continue;
                }
                ChapterPos chaptPos = this.Chapters[place.ChapterNum + book.VersesInChapterStartIndex];
                byte[] chapterBuffer = await this.GetChapterBytes(place.ChapterNum + book.VersesInChapterStartIndex);
                VersePos verse = chaptPos.Verses[place.VerseNum];
                if (verse.Length == 0)
                {
                    return string.Empty;
                }

                if (verse.StartPos >= chapterBuffer.Length || (verse.StartPos + verse.Length) > chapterBuffer.Length)
                {
                    return " POSSIBLE ERROR IN BIBLE, TEXT MISSING HERE ";
                }
                listText.Append(this.GetFullName(chaptPos.Booknum, appChoosenIsoLangCode) + " "
                    + (chaptPos.BookRelativeChapterNum + 1) + ":" + (place.VerseNum + 1) + " . " +
                    RawGenTextReader.CleanXml(Encoding.UTF8.GetString(chapterBuffer, (int)verse.StartPos, (int)verse.Length), true).Replace(".", ". "));
            }

            return listText.ToString();
        }
        protected async Task<string> MakeListDisplayText(
            string appChoosenIsoLangCode,
            DisplaySettings displaySettings,
            List<BiblePlaceMarker> listToDisplay,
            HtmlColorRgba htmlBackgroundColor,
            HtmlColorRgba htmlForegroundColor,
            HtmlColorRgba htmlPhoneAccentColor,
            HtmlColorRgba htmlWordsOfChristColor,
            double htmlFontSize,
            string fontFamily,
            bool showBookTitles,
            string notesTitle)
        {
            if (htmlBackgroundColor == null)
            {
                // must wait a little until we get these values.
                return string.Empty;
            }

            string chapterStartHtml = HtmlHeader(
                displaySettings,
                htmlBackgroundColor,
                htmlForegroundColor,
                htmlPhoneAccentColor,
                htmlWordsOfChristColor,
                htmlFontSize,
                fontFamily);
            const string chapterEndHtml = "</body></html>";
            var htmlListText = new StringBuilder(chapterStartHtml);
            int lastBookNum = -1;
            bool isVerseMarking = displaySettings.ShowBookName || displaySettings.ShowChapterNumber
                                  || displaySettings.ShowVerseNumber;
            string startVerseMarking = displaySettings.SmallVerseNumbers
                                           ? "<sup>"
                                           : (isVerseMarking ? "(" : string.Empty);
            string stopVerseMarking = displaySettings.SmallVerseNumbers
                                          ? "</sup>"
                                          : (isVerseMarking ? ")" : string.Empty);
            bool isInPoetry = false;
            for (int j = listToDisplay.Count - 1; j >= 0; j--)
            {
                BiblePlaceMarker place = listToDisplay[j];

                CanonBookDef book;
                if (!canon.BookByShortName.TryGetValue(place.BookShortName, out book) || place.ChapterNum >= book.NumberOfChapters || place.VerseNum >= canon.VersesInChapter[place.ChapterNum + book.VersesInChapterStartIndex])
                {
                    continue;
                }
                ChapterPos chaptPos = this.Chapters[place.ChapterNum + book.VersesInChapterStartIndex];
                byte[] chapterBuffer = await this.GetChapterBytes(place.ChapterNum + book.VersesInChapterStartIndex);
                ChapterPos versesForChapterPositions = this.Chapters[place.ChapterNum + book.VersesInChapterStartIndex];
                VersePos verse = versesForChapterPositions.Verses[place.VerseNum];

                // for debug
                // string all = System.Text.UTF8Encoding.UTF8.GetString(chapterBuffer, 0, chapterBuffer.Length);

                if (showBookTitles && lastBookNum != chaptPos.Booknum)
                {
                    htmlListText.Append("<h3>" + this.GetFullName(chaptPos.Booknum, appChoosenIsoLangCode) + "</h3>");
                    lastBookNum = chaptPos.Booknum;
                }

                string htmlChapterText = startVerseMarking + this.GetFullName(chaptPos.Booknum, appChoosenIsoLangCode) + " "
                                         + (chaptPos.BookRelativeChapterNum + 1) + ":" + (place.VerseNum + 1) + "  "
                                         + place.When.ToString("yyyy-MM-dd") + " " + place.When.ToString("hh.mm.ss")
                                         + stopVerseMarking;

                string textId = place.BookShortName + "_" + place.ChapterNum + "_" + place.VerseNum;
                int noteMarker = 0;
                string verseTxt = this.ParseOsisText(
                    displaySettings,
                    "<p><a name=\"" + textId + "\"></a><a class=\"normalcolor\" id=\"ID_" + textId
                    + "\"  href=\"#\" onclick=\"window.external.notify('" + textId
                    + "'); event.returnValue=false; return false;\" >" + htmlChapterText,
                    string.Empty,
                    chapterBuffer,
                    (int)verse.StartPos,
                    verse.Length,
                    this.Serial.IsIsoEncoding,
                    false,
                    true,
                    ref noteMarker,
                    ref isInPoetry);

                // create the verse
                if (string.IsNullOrEmpty(place.Note))
                {
                    htmlListText.Append(verseTxt + "</a></p><hr />");
                }
                else
                {
                    htmlListText.Append(
                        verseTxt + "</p><p>" + (displaySettings.SmallVerseNumbers ? "<sup>" : "(") + notesTitle
                        + (displaySettings.SmallVerseNumbers ? "</sup>" : ") ") + "<br />" + place.Note.Replace("\n", "<br />") + "</a></p><hr />");
                }
            }

            htmlListText.Append(chapterEndHtml);
            return htmlListText.ToString();
        }

        protected virtual string ParseOsisText(
            DisplaySettings displaySettings,
            string chapterNumber,
            string restartText,
            byte[] xmlbytes,
            int startPos,
            int length,
            bool isIsoText,
            bool isNotesOnly,
            bool noTitles,
            ref int noteIdentifier,
            ref bool isInPoetry,
            bool isRaw = false)
        {
            var ms = new MemoryStream();
            if (isIsoText)
            {
                ms.Write(PrefixIso, 0, PrefixIso.Length);
            }
            else
            {
                ms.Write(Prefix, 0, Prefix.Length);
            }

            // Some indexes are bad. make sure the startpos and length are not bad
            if (length == 0)
            {
                return string.Empty;
            }

            if (startPos >= xmlbytes.Length)
            {
                Debug.WriteLine("Bad startpos;" + xmlbytes.Length + ";" + startPos + ";" + length);
                return "*** POSSIBLE ERROR IN BIBLE, TEXT MISSING HERE ***";
            }

            if (startPos + length > xmlbytes.Length)
            {
                // we can fix this
                Debug.WriteLine("Fixed bad length;" + xmlbytes.Length + ";" + startPos + ";" + length);
                length = xmlbytes.Length - startPos - 1;
                if (length == 0)
                {
                    // this might be a problem or it might not. Put some stars here anyway.
                    return "***";
                }
            }

            try
            {
                ms.Write(xmlbytes, startPos, length);
                ms.Write(Suffix, 0, Suffix.Length);
                ms.Position = 0;

                // debug
                //byte[] buf = new byte[ms.Length]; ms.Read(buf, 0, (int)ms.Length);
                //string xxxxxx = System.Text.UTF8Encoding.UTF8.GetString(buf, 0, buf.Length);
                //System.Diagnostics.Debug.WriteLine("osisbuf: " + xxxxxx);
                //ms.Position = 0;
            }
            catch (Exception ee)
            {
                Debug.WriteLine("crashed in a strange place. err=" + ee.StackTrace);
            }

            var plainText = new StringBuilder();
            var noteText = new StringBuilder();
            var settings = new XmlReaderSettings { IgnoreWhitespace = false };

            bool isInElement = false;
            bool isInQuote = false;
            bool isInInjectionElement = false;
            bool isInTitle = false;
            var fontStylesEnd = new List<string>();
            bool isChaptNumGiven = false;
            bool isChaptNumGivenNotes = false;
            bool isReferenceLinked = false;
            int isLastElementLineBreak = 0;
            string lemmaText = string.Empty;
            string morphText = string.Empty;
            bool isFirstNoteInText = true;
            using (XmlReader reader = XmlReader.Create(ms, settings))
            {
                try
                {
                    // Parse the file and get each of the nodes.
                    while (reader.Read())
                    {
                        if (isLastElementLineBreak >= 1)
                        {
                            if (isLastElementLineBreak >= 2)
                            {
                                isLastElementLineBreak = 0;
                            }
                            else
                            {
                                isLastElementLineBreak = 2;
                            }
                        }

                        switch (reader.NodeType)
                        {
                            case XmlNodeType.SignificantWhitespace:
                                AppendText(reader.Value, plainText, noteText, isInElement);
                                break;
                            case XmlNodeType.Whitespace:
                                AppendText(reader.Value, plainText, noteText, isInElement);
                                break;
                            case XmlNodeType.Element:
                                switch (reader.Name)
                                {
                                    case "CM":
                                        if (!isRaw && !displaySettings.EachVerseNewLine && isLastElementLineBreak == 0)
                                        {
                                            AppendText("<br />", plainText, noteText, isInElement);
                                            isLastElementLineBreak = 1;
                                        }

                                        break;
                                    case "lb":
                                        if (!isRaw && !displaySettings.EachVerseNewLine)
                                        {
                                            string paragraphXml = isLastElementLineBreak == 0 ? "<br />" : " ";
                                            if (reader.HasAttributes)
                                            {
                                                reader.MoveToFirstAttribute();
                                                if (reader.Name.Equals("type"))
                                                {
                                                    {
                                                        paragraphXml = reader.Value.Equals("x-end-paragraph")
                                                                           ? "</p>"
                                                                           : (reader.Value.Equals("x-begin-paragraph")
                                                                                  ? "<p>"
                                                                                  : "<br />");
                                                    }
                                                }
                                            }

                                            AppendText(paragraphXml, plainText, noteText, isInElement);
                                            isLastElementLineBreak = 1;
                                        }

                                        break;
                                    case "title":
                                        isInTitle = true;
                                        if (!(noTitles || !displaySettings.ShowHeadings) && !isRaw)
                                        {
                                            AppendText("<h3>", plainText, noteText, isInElement);
                                        }

                                        break;
                                    case "reference":
                                        if (reader.HasAttributes)
                                        {
                                            reader.MoveToFirstAttribute();
                                            if (reader.Name.Equals("osisRef"))
                                            {
                                                int chaptNumLoc;
                                                int verseNumLoc;
                                                string bookShortName;
                                                if (ConvertOsisRefToAbsoluteChaptVerse(
                                                    reader.Value, out bookShortName, out chaptNumLoc, out verseNumLoc))
                                                {                                                    
                                                    isReferenceLinked = true;
                                                    string textId = bookShortName + "_" + chaptNumLoc + "_" + verseNumLoc;
                                                    noteText.Append(
                                                            "</a><a class=\"normalcolor\" id=\"ID_" + textId
                                                       + "\"  href=\"#\" onclick=\"window.external.notify('" + textId
                                                        + "'); event.returnValue=false; return false;\" >");
                                                }
                                            }
                                        }

                                        noteText.Append("  [");
                                        break;
                                    case "lg":
                                        if (!isRaw && !displaySettings.EachVerseNewLine)
                                        {
                                            if (isInPoetry)
                                            {
                                                isInPoetry = false;
                                                AppendText("</blockquote>", plainText, noteText, isInElement);
                                            }
                                            else
                                            {
                                                isInPoetry = true;
                                                AppendText(
                                                    "<blockquote style=\"margin: 0 0 0 1.5em;padding 0 0 0 0;\">",
                                                    plainText,
                                                    noteText,
                                                    isInElement);
                                            }

                                            isLastElementLineBreak = 1;
                                        }

                                        break;
                                    case "l":
                                        if (!isRaw && !displaySettings.EachVerseNewLine && isLastElementLineBreak == 0)
                                        {
                                            AppendText(isInPoetry ? "<br />" : " ", plainText, noteText, isInElement);
                                            isLastElementLineBreak = 1;
                                        }

                                        break;
                                    case "FI":
                                        if (!isRaw && !isNotesOnly && displaySettings.ShowNotePositions)
                                        {
                                            plainText.Append(
                                                (displaySettings.SmallVerseNumbers ? "<sup>" : string.Empty)
                                                + this.convertNoteNumToId(noteIdentifier)
                                                + (displaySettings.SmallVerseNumbers ? "</sup>" : string.Empty));
                                            noteIdentifier++;
                                        }

                                        if (!isChaptNumGivenNotes && !isRaw)
                                        {
                                            noteText.Append("<p>" + chapterNumber);
                                            isChaptNumGivenNotes = true;
                                        }
                                        if (!isRaw && displaySettings.ShowNotePositions)
                                        {
                                            noteText.Append(
                                                (displaySettings.SmallVerseNumbers ? "<sup>" : string.Empty)
                                                + this.convertNoteNumToId(noteIdentifier)
                                                + (displaySettings.SmallVerseNumbers ? "</sup>" : string.Empty));
                                            if (isNotesOnly)
                                            {
                                                noteIdentifier++;
                                            }
                                        }
                                        noteText.Append("(");
                                        isInInjectionElement = true;
                                        break;
                                    case "RF":
                                    case "note":
                                        if (!isRaw && !isNotesOnly && displaySettings.ShowNotePositions)
                                        {
                                            plainText.Append(
                                                (displaySettings.SmallVerseNumbers ? "<sup>" : string.Empty)
                                                + this.convertNoteNumToId(noteIdentifier)
                                                + (displaySettings.SmallVerseNumbers ? "</sup>" : string.Empty));
                                            noteIdentifier++;
                                        }

                                        if (!isChaptNumGivenNotes && !isRaw)
                                        {
                                            noteText.Append("<p>" + chapterNumber);
                                            isChaptNumGivenNotes = true;
                                        }
                                        
                                        if (!isRaw && displaySettings.ShowNotePositions)
                                        {
                                            if (!isFirstNoteInText && displaySettings.AddLineBetweenNotes)
                                            {
                                                noteText.Append("<br />");
                                                
                                            }
                                            isFirstNoteInText = false;
                                            noteText.Append(
                                                (displaySettings.SmallVerseNumbers ? "<sup>" : string.Empty)
                                                + this.convertNoteNumToId(noteIdentifier)
                                                + (displaySettings.SmallVerseNumbers ? "</sup>" : string.Empty));
                                            if(isNotesOnly)
                                            {
                                                noteIdentifier++;
                                            }
                                        }


                                        isInElement = true;
                                        break;
                                    case "hi":
                                        if (!isRaw)
                                        {
                                            if (reader.HasAttributes)
                                            {
                                                reader.MoveToFirstAttribute();
                                                if (reader.Name.ToLower().Equals("type"))
                                                {
                                                    var fontStyle = reader.Value.ToLower();
                                                    string startText;
                                                    if (FontPropertiesStartHtml.TryGetValue(fontStyle, out startText))
                                                    {
                                                        if (!isInElement && !isInInjectionElement && chapterNumber.Length > 0 && !isInTitle
                                                            && !isChaptNumGiven)
                                                        {
                                                            if (isInQuote)
                                                            {
                                                                AppendText("</span>", plainText, noteText, isInElement);
                                                            }

                                                            plainText.Append(chapterNumber);
                                                            if (isInQuote)
                                                            {
                                                                AppendText("<span class=\"christ\">", plainText, noteText, isInElement);
                                                            }

                                                            isChaptNumGiven = true;
                                                        }

                                                        AppendText(startText, plainText, noteText, isInElement);
                                                        fontStylesEnd.Add(FontPropertiesEndHtml[fontStyle]);
                                                    }
                                                }
                                            }
                                        }

                                        break;
                                    case "Rf":
                                        isInElement = false;
                                        break;
                                    case "Fi":
                                        noteText.Append(") ");
                                        isInInjectionElement = false;
                                        break;
                                    case "q":
                                        if (!isRaw && !isNotesOnly)
                                        {
                                            if (reader.HasAttributes)
                                            {
                                                reader.MoveToFirstAttribute();
                                                do
                                                {
                                                    if (displaySettings.WordsOfChristRed && reader.Name.Equals("who"))
                                                    {
                                                        if (reader.Value.ToLower().Equals("jesus"))
                                                        {
                                                            AppendText(
                                                                "<span class=\"christ\">",
                                                                plainText,
                                                                noteText,
                                                                isInElement);
                                                            isInQuote = true;
                                                        }
                                                    }

                                                    if (reader.Name.Equals("marker"))
                                                    {
                                                        AppendText(reader.Value, plainText, noteText, isInElement);
                                                    }
                                                }
                                                while (reader.MoveToNextAttribute());
                                            }
                                        }

                                        break;
                                    case "w":

                                        // <w lemma="strong:G1078" morph="robinson:N-GSF">γενεσεως</w>
                                        if ((displaySettings.ShowStrongsNumbers || displaySettings.ShowMorphology)
                                            && !isRaw && !isNotesOnly)
                                        {
                                            lemmaText = string.Empty;
                                            morphText = string.Empty;
                                            if (reader.HasAttributes)
                                            {
                                                reader.MoveToFirstAttribute();

                                                do
                                                {
                                                    if (displaySettings.ShowStrongsNumbers
                                                        && reader.Name.Equals("lemma"))
                                                    {
                                                        string[] lemmas = reader.Value.Split(' ');
                                                        foreach (string lemma in lemmas)
                                                        {
                                                            if (lemma.StartsWith("strong:"))
                                                            {
                                                                if (!string.IsNullOrEmpty(lemmaText))
                                                                {
                                                                    lemmaText += ",";
                                                                }

                                                                lemmaText +=
                                                                    "<a class=\"strongsmorph\" href=\"#\" onclick=\"window.external.notify('STRONG_"
                                                                    + lemma.Substring(7)
                                                                    + "'); event.returnValue=false; return false;\" >"
                                                                    + lemma.Substring(8) + "</a>";
                                                            }
                                                        }
                                                    }
                                                    else if (displaySettings.ShowMorphology
                                                             && reader.Name.Equals("morph"))
                                                    {
                                                        string[] morphs = reader.Value.Split(' ');
                                                        foreach (string morph in morphs)
                                                        {
                                                            if (morph.StartsWith("robinson:"))
                                                            {
                                                                string subMorph = morph.Substring(9);
                                                                if (!string.IsNullOrEmpty(morphText))
                                                                {
                                                                    morphText += ",";
                                                                }

                                                                morphText +=
                                                                    "<a class=\"strongsmorph\" href=\"#\" onclick=\"window.external.notify('MORPH_"
                                                                    + subMorph
                                                                    + "'); event.returnValue=false; return false;\" >"
                                                                    + subMorph + "</a>";
                                                            }
                                                        }
                                                    }
                                                }
                                                while (reader.MoveToNextAttribute());
                                            }
                                        }

                                        break;

                                    case "versee":
                                        //AppendText(" ", plainText, noteText, isInElement);
                                        break;
                                    default:
                                        //AppendText(" ", plainText, noteText, isInElement);
                                        Debug.WriteLine("Element untreated: " + reader.Name);
                                        break;
                                }

                                break;
                            case XmlNodeType.Text:
                                if (!isInElement && !isInInjectionElement && chapterNumber.Length > 0 && !isInTitle
                                    && !isChaptNumGiven)
                                {
                                    if (isInQuote)
                                    {
                                        AppendText("</span>", plainText, noteText, isInElement);
                                    }

                                    plainText.Append(chapterNumber);
                                    if (isInQuote)
                                    {
                                        AppendText("<span class=\"christ\">", plainText, noteText, isInElement);
                                    }

                                    isChaptNumGiven = true;
                                }

                                string text;
                                try
                                {
                                    text = reader.Value;
                                }
                                catch (Exception e1)
                                {
                                    Debug.WriteLine("error in text: " + e1.Message);
                                    try
                                    {
                                        text = reader.Value;
                                    }
                                    catch (Exception e2)
                                    {
                                        Debug.WriteLine("second error in text: " + e2.Message);
                                        text = "*error*";
                                    }
                                }

                                if ((!(noTitles || !displaySettings.ShowHeadings) || !isInTitle) && text.Length > 0)
                                {
                                    char firstChar = text[0];
                                    AppendText(/*
                                        ((!firstChar.Equals(',') && !firstChar.Equals('.') && !firstChar.Equals(':')
                                          && !firstChar.Equals(';') && !firstChar.Equals('?'))
                                             ? " "
                                             : string.Empty) +*/ text,
                                        plainText,
                                        noteText,
                                        isInElement || isInInjectionElement);
                                }

                                break;
                            case XmlNodeType.EndElement:
                                switch (reader.Name)
                                {
                                    case "title":
                                        if (!(noTitles || !displaySettings.ShowHeadings) && !isRaw)
                                        {
                                            AppendText("</h3>", plainText, noteText, isInElement);
                                        }

                                        isInTitle = false;
                                        break;
                                    case "reference":
                                        noteText.Append("] ");
                                        if (isReferenceLinked)
                                        {
                                            noteText.Append("</a>" + restartText);
                                        }

                                        isReferenceLinked = false;
                                        break;
                                    case "note":
                                        isInElement = false;
                                        break;
                                    case "hi":
                                        if (!isRaw && fontStylesEnd.Any())
                                        {
                                            string fontStyleEnd = fontStylesEnd[fontStylesEnd.Count() - 1];
                                            fontStylesEnd.RemoveAt(fontStylesEnd.Count() - 1);
                                            AppendText(fontStyleEnd, plainText, noteText, isInElement);
                                        }

                                        break;
                                    case "q":
                                        if (isInQuote)
                                        {
                                            AppendText("</span>", plainText, noteText, isInElement);
                                            isInQuote = false;
                                        }

                                        break;
                                    case "w":

                                        // <w lemma="strong:G1078" morph="robinson:N-GSF">γενεσεως</w>
                                        if ((displaySettings.ShowStrongsNumbers || displaySettings.ShowMorphology)
                                            && !isRaw && !isNotesOnly
                                            && (!string.IsNullOrEmpty(lemmaText) || !string.IsNullOrEmpty(morphText)))
                                        {
                                            plainText.Append(
                                                "</a>"
                                                + (displaySettings.SmallVerseNumbers
                                                       ? "<sub>"
                                                       : "<span class=\"strongsmorph\">(</span>"));
                                            if (!string.IsNullOrEmpty(lemmaText))
                                            {
                                                plainText.Append(lemmaText);
                                            }

                                            if (!string.IsNullOrEmpty(morphText))
                                            {
                                                plainText.Append(
                                                    (string.IsNullOrEmpty(lemmaText) ? string.Empty : ",") + morphText);
                                            }

                                            plainText.Append(
                                                (displaySettings.SmallVerseNumbers
                                                     ? "</sub>"
                                                     : "<span class=\"strongsmorph\">)</span>") + restartText);
                                            lemmaText = string.Empty;
                                            morphText = string.Empty;
                                        }

                                        //else
                                        //{
                                        //    AppendText(" ", plainText, noteText, isInElement);
                                        //}
                                        break;
                                    case "lg":
                                        if (!isRaw && !displaySettings.EachVerseNewLine)
                                        {
                                            isInPoetry = false;
                                            AppendText("</blockquote>", plainText, noteText, isInElement);
                                        }

                                        break;
                                    case "l":
                                        if (!isRaw && !displaySettings.EachVerseNewLine)
                                        {
                                            //AppendText(" ", plainText, noteText, isInElement);
                                        }

                                        break;
                                    case "versee":
                                        AppendText(" ", plainText, noteText, isInElement);
                                        break;
                                    default:
                                        //AppendText(" ", plainText, noteText, isInElement);
                                        Debug.WriteLine("EndElement untreated: " + reader.Name);
                                        break;
                                }

                                break;
                        }
                    }
                }
                catch (Exception e)
                {
                    Debug.WriteLine("BibleZtextReader.parseOsisText " + e.Message);
                }
            }

            if (isNotesOnly && !isRaw)
            {
                if (noteText.Length > 0)
                {
                    noteText.Append("</p>");
                }

                return noteText.ToString();
            }

            // this replace fixes a character translation problem for slanted apostrophy
            return plainText.ToString().Replace('\x92', '\'');
        }

        protected void RaiseSourceChangedEvent()
        {
            if (this.SourceChanged != null)
            {
                this.SourceChanged();
            }
        }

        private async Task<bool> ReloadOneIndex(string filename, int startBook, int endBook, CanonBookDef[] booksInTestement)
        {
            if (string.IsNullOrEmpty(this.Serial.Path))
            {
                return false;
            }
            try
            {
                var bookPositions = new List<BookPos>();
                //Windows.Storage.ApplicationData appData = Windows.Storage.ApplicationData.Current;
                Stream fs =
                    await
                    ApplicationData.Current.LocalFolder.OpenStreamForReadAsync(
                        this.Serial.Path.Replace("/", "\\") + filename + ((char)this.BlockType) + "zs");
                //var folder = await appData.LocalFolder.GetFolderAsync(Serial.Path.Replace("/", "\\"));
                //var file = await folder.CreateFileAsync(filename + ((char)BlockType) + "zs");
                //var fs = await file.OpenStreamForReadAsync(); 
                bool isEnd;

                // read book position index
                for (int i = 0; ; i++)
                {
                    long startPos = this.GetintFromStream(fs, out isEnd);
                    if (isEnd)
                    {
                        break;
                    }

                    long length = this.GetintFromStream(fs, out isEnd);
                    if (isEnd)
                    {
                        break;
                    }

                    long unused = this.GetintFromStream(fs, out isEnd);
                    if (isEnd)
                    {
                        break;
                    }

                    bookPositions.Add(new BookPos(startPos, length, unused));
                }

                fs.Dispose();

                // read the verse holder for versification bzv file
                //file = await folder.CreateFileAsync(filename + ((char)BlockType) + "zv");
                fs =
                    await
                    ApplicationData.Current.LocalFolder.OpenStreamForReadAsync(
                        this.Serial.Path.Replace("/", "\\") + filename + ((char)this.BlockType) + "zv");

                // dump the first 4 posts
                for (int i = 0; i < 4; i++)
                {
                    this.GetShortIntFromStream(fs, out isEnd);
                    this.GetInt48FromStream(fs, out isEnd);
                    this.GetShortIntFromStream(fs, out isEnd);
                }

                // now start getting each chapter in each book
                for (int i = startBook; i < endBook; i++)
                {
                    var bookDef = booksInTestement[i - startBook];
                    for (int j = 0; j < bookDef.NumberOfChapters; j++)
                    {
                        long chapterStartPos = 0;
                        ChapterPos chapt = null;
                        long lastNonZeroStartPos = 0;
                        long lastNonZeroLength = 0;
                        int length = 0;

                        for (int k = 0; k < canon.VersesInChapter[bookDef.VersesInChapterStartIndex + j]; k++)
                        {
                            int booknum = this.GetShortIntFromStream(fs, out isEnd);
                            long startPos = this.GetInt48FromStream(fs, out isEnd);
                            if (startPos != 0)
                            {
                                lastNonZeroStartPos = startPos;
                            }

                            length = this.GetShortIntFromStream(fs, out isEnd);

                            if (k == 0)
                            {
                                chapterStartPos = startPos;
                                long bookStartPos = 0;
                                if (booknum < bookPositions.Count)
                                {
                                    bookStartPos = bookPositions[booknum].StartPos;
                                }

                                if (this.BlockType == IndexingBlockType.Chapter)
                                {
                                    chapterStartPos = 0;
                                }

                                chapt = new ChapterPos(chapterStartPos, i, j, bookStartPos);
                                chapt.IsEmpty = true;
                            }

                            if (length != 0)
                            {
                                lastNonZeroLength = length;
                                chapt.IsEmpty = false;
                            }

                            if (booknum == 0 && startPos == 0 && length == 0)
                            {
                                if (chapt != null)
                                {
                                    chapt.Verses.Add(new VersePos(0, 0, i));
                                }
                            }
                            else
                            {
                                if (chapt != null)
                                {
                                    chapt.Verses.Add(new VersePos(startPos - chapterStartPos, length, i));
                                }
                            }
                        }

                        // update the chapter length now that we know it
                        if (chapt != null)
                        {
                            chapt.Length = (int)(lastNonZeroStartPos - chapterStartPos) + lastNonZeroLength;
                            this.Chapters.Add(chapt);
                        }

                        // dump a post for the chapter break
                        this.GetShortIntFromStream(fs, out isEnd);
                        this.GetInt48FromStream(fs, out isEnd);
                        this.GetShortIntFromStream(fs, out isEnd);
                    }

                    // dump a post for the book break
                    this.GetShortIntFromStream(fs, out isEnd);
                    this.GetInt48FromStream(fs, out isEnd);
                    this.GetShortIntFromStream(fs, out isEnd);
                }

                fs.Dispose();
            }
            catch (Exception)
            {
                // failed to load old testement.  Maybe it does not exist.
                // fill with fake data
                for (int i = startBook; i < endBook; i++)
                {
                    var bookDef = booksInTestement[i - startBook];
                    for (int j = 0; j < bookDef.NumberOfChapters; j++)
                    {
                        var chapt = new ChapterPos(0, i, j, 0);
                        chapt.IsEmpty = true;
                        for (int k = 0; k < canon.VersesInChapter[bookDef.VersesInChapterStartIndex + j]; k++)
                        {
                            chapt.Verses.Add(new VersePos(0, 0, i));
                        }

                        // update the chapter length now that we know it
                        chapt.Length = 0;
                        this.Chapters.Add(chapt);
                    }
                }
            }
            return true;
        }

        private async Task<bool> ReloadSettingsFile()
        {
            if (canon == null)
            {
                canon = CanonManager.GetCanon(this.Serial.Versification);
            }
            this.Chapters = new List<ChapterPos>();
            this.BlockType = IndexingBlockType.Book;
            // the name must be unique of course
            if (await FileExists(ApplicationData.Current.LocalFolder, this.Serial.Path + "ot.czs") || await FileExists(ApplicationData.Current.LocalFolder, this.Serial.Path + "nt.czs"))
            {
                this.BlockType = IndexingBlockType.Chapter;
            }

            await this.ReloadOneIndex("ot.", 0, canon.OldTestBooks.Count(), canon.OldTestBooks);
            await this.ReloadOneIndex("nt.", canon.OldTestBooks.Count(), canon.OldTestBooks.Count() + canon.NewTestBooks.Count(), canon.NewTestBooks);
            return true;
        }

        public virtual void SetToFirstChapter()
        {
            this.Serial.PosBookShortName = canon.BookByShortName.First().Value.ShortName1;
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

        //private string convertNoteNumToId(int noteIdentifier)
        //{
        //    string noteReturned = string.Empty;
        //    string startChar = ((char)((noteIdentifier - 'a') % 24 + 'a')).ToString();
        //    int numChars = (noteIdentifier - 'a') / 24;
        //    for (int i = 0; i <= numChars; i++)
        //    {
        //        noteReturned += startChar;
        //    }

        //    return "(" + noteReturned + ")";
        //}
        //private static string[] IntToBase24 = new string[] { "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z" };
        private string convertNoteNumToId(int noteIdentifier)
        {
            string base26 = string.Empty;
            do
            {
                base26 += (char)(noteIdentifier % 26 + 'a');// IntToBase24[noteIdentifier % 26];
                noteIdentifier = noteIdentifier / 26;
            }
            while (noteIdentifier > 0);

            return "(" + Reverse(base26) + ")";
        }
        public string Reverse(string input)
        {
            char[] output = new char[input.Length];

            int forwards = 0;
            int backwards = input.Length - 1;

            do
            {
                output[forwards] = input[backwards];
                output[backwards] = input[forwards];
            } while (++forwards <= --backwards);

            return new String(output);
        }

        #endregion

        [DataContract]
        public struct VersePos
        {
            #region Fields

            [DataMember(Name = "booknum")]
            public int Booknum;

            [DataMember(Name = "length")]
            public int Length;

            [DataMember(Name = "startPos")]
            public long StartPos;

            #endregion

            #region Constructors and Destructors

            public VersePos(long startPos, int length, int booknum)
            {
                this.StartPos = startPos;
                this.Length = length;
                this.Booknum = booknum;
            }

            #endregion

            #region Public Methods and Operators

            public bool Equals(VersePos equalsTo)
            {
                return equalsTo.Booknum == this.Booknum && equalsTo.Length == this.Length
                       && equalsTo.StartPos == this.StartPos;
            }

            #endregion
        }

        [DataContract(IsReference = true)]
        [KnownType(typeof(ChapterPos))]
        public class BookPos
        {
            #region Fields

            [DataMember(Name = "length")]
            public long Length;

            [DataMember(Name = "listChapters")]
            public List<ChapterPos> ListChapters = new List<ChapterPos>();

            [DataMember(Name = "startPos")]
            public long StartPos;

            [DataMember(Name = "unused")]
            public long Unused;

            #endregion

            #region Constructors and Destructors

            public BookPos(long startPos, long length, long unused)
            {
                this.StartPos = startPos;
                this.Length = length;
                this.Unused = unused;
            }

            #endregion
        }

        [DataContract(IsReference = true)]
        [KnownType(typeof(VersePos))]
        public class ChapterPos
        {
            #region Fields

            [DataMember(Name = "bookRelativeChapterNum")]
            public int BookRelativeChapterNum;

            [DataMember(Name = "bookStartPos")]
            public long BookStartPos;

            [DataMember(Name = "booknum")]
            public int Booknum;

            [DataMember(Name = "length")]
            public long Length;

            [DataMember(Name = "startPos")]
            public long StartPos;

            [DataMember(Name = "verses")]
            public List<VersePos> Verses = new List<VersePos>();

            public bool IsEmpty = false;

            #endregion

            #region Constructors and Destructors

            public ChapterPos(long startPos, int booknum, int bookRelativeChapterNum, long bookStartPos)
            {
                this.StartPos = startPos;
                this.Booknum = booknum;
                this.BookRelativeChapterNum = bookRelativeChapterNum;
                this.BookStartPos = bookStartPos;
            }

            #endregion
        }
    }

    [DataContract]
    public class BibleZtextReaderSerialData
    {
        #region Fields

        [DataMember(Name = "isIsoEncoding")]
        public bool IsIsoEncoding;

        [DataMember(Name = "iso2DigitLangCode")]
        public string Iso2DigitLangCode = string.Empty;

        [DataMember(Name = "path")]
        public string Path = string.Empty;

        [DataMember(Name = "posChaptNum")]
        public int PosChaptNum;

        [DataMember(Name = "posVerseNum")]
        public int PosVerseNum;

        [DataMember(Name = "posBookShortName")]
        public string PosBookShortName;

        [DataMember(Name = "CipherKey")]
        public string CipherKey;

        [DataMember(Name = "ConfigPath")]
        public string ConfigPath;

        [DataMember(Name = "Versification")]
        public string Versification;

        #endregion

        #region Constructors and Destructors

        public BibleZtextReaderSerialData(BibleZtextReaderSerialData from)
        {
            this.CloneFrom(from);
        }

        public BibleZtextReaderSerialData(
            bool isIsoEncoding, string iso2DigitLangCode, string path, int posChaptNum, int posVerseNum, string cipherKey, string configPath)
        {
            this.IsIsoEncoding = isIsoEncoding;
            this.Iso2DigitLangCode = iso2DigitLangCode;
            this.Path = path;
            this.PosChaptNum = posChaptNum;
            this.PosVerseNum = posVerseNum;
            this.CipherKey = cipherKey;
            this.ConfigPath = configPath;
        }

        #endregion

        #region Public Methods and Operators

        public void CloneFrom(BibleZtextReaderSerialData from)
        {
            this.IsIsoEncoding = from.IsIsoEncoding;
            this.Iso2DigitLangCode = from.Iso2DigitLangCode;
            this.Path = from.Path;
            this.PosChaptNum = from.PosChaptNum;
            this.PosVerseNum = from.PosVerseNum;
            this.CipherKey = from.CipherKey;
            this.ConfigPath = from.ConfigPath;
        }

        #endregion
    }
}