#region Header

// <copyright file="BibleNoteReader.cs" company="Thomas Dilts">
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

namespace Sword.reader
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Runtime.Serialization;
    using System.Text;
    using System.Threading.Tasks;
    using System.Xml;

    /// <summary>
    ///     Load from a file all the book and verse pointers to the bzz file so that
    ///     we can later read the bzz file quickly and efficiently.
    /// </summary>
    [DataContract(Name = "BibleNoteReader")]
    [KnownType(typeof(ChapterPos))]
    [KnownType(typeof(BookPos))]
    [KnownType(typeof(VersePos))]
    public class BibleNoteReader : BibleZtextReader
    {
        #region Fields

        [DataMember(Name = "serial2")]
        public BibleZtextReaderSerialData Serial2 = new BibleZtextReaderSerialData(false, "", "", 0, 0, string.Empty, string.Empty);

        [DataMember(Name = "titleBrowserWindow")]
        public string TitleBrowserWindow = string.Empty;

        #endregion

        #region Constructors and Destructors

        public BibleNoteReader(string path, string iso2DigitLangCode, bool isIsoEncoding, string titleBrowserWindow, string cipherKey, string configPath, string versification)
            : base(path, iso2DigitLangCode, isIsoEncoding, cipherKey, configPath, versification)
        {
            this.Serial2.CloneFrom(this.Serial);
            this.TitleBrowserWindow = titleBrowserWindow;
        }

        #endregion

        #region Public Properties

        public override bool IsHearable
        {
            get
            {
                return false;
            }
        }

        public override bool IsLocalChangeDuringLink
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

        public override bool IsTranslateable
        {
            get
            {
                return true;
            }
        }

        #endregion

        #region Public Methods and Operators

        public override async Task<string> GetChapterHtml(
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
                    true,
                    addStartFinishHtml,
                    forceReload);
        }

        public override void GetInfo(
            out string bookShortName,
            out int relChaptNum,
            out int verseNum,
            out string fullName,
            out string title)
        {
            verseNum = this.Serial.PosVerseNum;
            relChaptNum = this.Serial.PosChaptNum;
            bookShortName = this.Serial.PosBookShortName;
            this.GetInfo(bookShortName,
                this.Serial.PosChaptNum, this.Serial.PosVerseNum, out fullName, out title);
            title = this.TitleBrowserWindow + " " + fullName + ":" + (relChaptNum + 1);
        }

        public override async Task<string> GetVerseTextOnly(
            DisplaySettings displaySettings, string bookShortName, int chapterNumber, int verseNumber)
        {
            //give them the notes if you can.
            var book = canon.BookByShortName[bookShortName];
            Task<byte[]> chapterBuffer = this.GetChapterBytes(chapterNumber + book.VersesInChapterStartIndex);
            VersePos verse = this.Chapters[chapterNumber].Verses[verseNumber];
            int noteMarker = 'a';
            bool isInPoetry = false;
            return this.ParseOsisText(
                displaySettings,
                "",
                "",
                await chapterBuffer,
                (int)verse.StartPos,
                verse.Length,
                this.Serial.IsIsoEncoding,
                true,
                true,
                ref noteMarker,
                ref isInPoetry);
        }

        public override async Task Resume()
        {
            this.Serial.CloneFrom(this.Serial2);
            await base.Resume();
        }

        public override void SerialSave()
        {
            this.Serial2.CloneFrom(this.Serial);
        }

        #endregion
    }

    [DataContract(Name = "DisplaySettings")]
    public class DisplaySettings
    {
        #region Fields

        [DataMember(Name = "customBibleDownloadLinks")]
        public string CustomBibleDownloadLinks = @"www.cross-connect.se,/bibles/raw,/bibles/biblelist";

        [DataMember(Name = "eachVerseNewLine")]
        public bool EachVerseNewLine;

        [DataMember(Name = "greekDictionaryLink")]
        public string GreekDictionaryLink = @"http://www.eliyah.com/cgi-bin/strongs.cgi?file=greeklexicon&isindex={0}";

        [DataMember(Name = "hebrewDictionaryLink")]
        public string HebrewDictionaryLink = @"http://www.eliyah.com/cgi-bin/strongs.cgi?file=hebrewlexicon&isindex={0}";

        [DataMember(Name = "highlightMarkings")]
        public bool HighlightMarkings;

        [DataMember]
        public int NumberOfScreens = 3;

        [DataMember(Name = "showAddedNotesByChapter")]
        public bool ShowAddedNotesByChapter;

        [DataMember(Name = "showBookName")]
        public bool ShowBookName;

        [DataMember(Name = "showChapterNumber")]
        public bool ShowChapterNumber;

        [DataMember(Name = "showHeadings")]
        public bool ShowHeadings = true;

        [DataMember(Name = "showMorphology")]
        public bool ShowMorphology;

        [DataMember(Name = "showNotePositions")]
        public bool ShowNotePositions;

        [DataMember(Name = "showStrongsNumbers")]
        public bool ShowStrongsNumbers;

        [DataMember(Name = "showVerseNumber")]
        public bool ShowVerseNumber = true;

        [DataMember(Name = "smallVerseNumbers")]
        public bool SmallVerseNumbers = true;

        [DataMember(Name = "soundLink")]
        public string SoundLink =
            @"http://www.cross-connect.se/bibles/talking/getabsolutechapter.php?chapternum={0}&language={1}";

        [DataMember(Name = "useInternetGreekHebrewDict")]
        public bool UseInternetGreekHebrewDict;

        [DataMember(Name = "userUniqueGuuid")]
        public string UserUniqueGuuid = "";

        [DataMember(Name = "wordsOfChristRed")]
        public bool WordsOfChristRed;

        [DataMember]
        public string HighlightName1 = "Highlight 1";

        [DataMember]
        public string HighlightName2 = "Highlight 2";

        [DataMember]
        public string HighlightName3 = "Highlight 3";

        [DataMember]
        public string HighlightName4 = "Highlight 4";

        [DataMember]
        public string HighlightName5 = "Highlight 5";

        [DataMember]
        public string HighlightName6 = "Highlight 6";

        [DataMember]
        public bool UseHighlights = true;

        [DataMember]
        public bool UseRemoteStorage = true;

        public Highlighter highlighter = new Highlighter();
        #endregion

        #region Public Methods and Operators

        public void CheckForNullAndFix()
        {
            var fixer = new DisplaySettings();
            if (this.SoundLink == null
                || this.SoundLink.Equals(
                    @"http://www.chaniel.se/crossconnect/bibles/talking/getabsolutechapter.php?chapternum={0}&language={1}"))
            {
                this.SoundLink = fixer.SoundLink;
            }
            if (this.GreekDictionaryLink == null)
            {
                this.GreekDictionaryLink = fixer.GreekDictionaryLink;
            }
            if (this.HebrewDictionaryLink == null)
            {
                this.HebrewDictionaryLink = fixer.HebrewDictionaryLink;
            }
            if (this.CustomBibleDownloadLinks == null
                || this.CustomBibleDownloadLinks.Equals(
                    @"www.chaniel.se,/crossconnect/bibles/raw,/crossconnect/bibles/biblelist"))
            {
                this.CustomBibleDownloadLinks = fixer.CustomBibleDownloadLinks;
            }
            if (string.IsNullOrEmpty(this.UserUniqueGuuid))
            {
                this.UserUniqueGuuid = Guid.NewGuid().ToString();
            }
            if (this.NumberOfScreens == 0)
            {
                this.NumberOfScreens = 3;
            }
            if (string.IsNullOrEmpty(this.HighlightName1))
            {
                this.HighlightName1 = "Highlight 1";
            }
            if (string.IsNullOrEmpty(this.HighlightName2))
            {
                this.HighlightName2 = "Highlight 2";
            }
            if (string.IsNullOrEmpty(this.HighlightName3))
            {
                this.HighlightName3 = "Highlight 3";
            }
            if (string.IsNullOrEmpty(this.HighlightName4))
            {
                this.HighlightName4 = "Highlight 4";
            }
            if (string.IsNullOrEmpty(this.HighlightName5))
            {
                this.HighlightName5 = "Highlight 5";
            }
            if (string.IsNullOrEmpty(this.HighlightName6))
            {
                UseHighlights = true;
                UseRemoteStorage = true;
                this.HighlightName6 = "Highlight 6";
            }
        }

        public DisplaySettings Clone()
        {
            var cloned = new DisplaySettings
                             {
                                 CustomBibleDownloadLinks = this.CustomBibleDownloadLinks,
                                 EachVerseNewLine = this.EachVerseNewLine,
                                 GreekDictionaryLink = this.GreekDictionaryLink,
                                 HebrewDictionaryLink = this.HebrewDictionaryLink,
                                 HighlightMarkings = this.HighlightMarkings,
                                 NumberOfScreens = this.NumberOfScreens,
                                 ShowAddedNotesByChapter = this.ShowAddedNotesByChapter,
                                 ShowBookName = this.ShowBookName,
                                 ShowChapterNumber = this.ShowChapterNumber,
                                 ShowHeadings = this.ShowHeadings,
                                 ShowMorphology = this.ShowMorphology,
                                 ShowNotePositions = this.ShowNotePositions,
                                 ShowStrongsNumbers = this.ShowStrongsNumbers,
                                 ShowVerseNumber = this.ShowVerseNumber,
                                 SmallVerseNumbers = this.SmallVerseNumbers,
                                 SoundLink = this.SoundLink,
                                 WordsOfChristRed = this.WordsOfChristRed,
                                 UserUniqueGuuid = this.UserUniqueGuuid,
                                 UseInternetGreekHebrewDict = this.UseInternetGreekHebrewDict,
                                 HighlightName1 = this.HighlightName1,
                                 HighlightName2 = this.HighlightName2,
                                 HighlightName3 = this.HighlightName3,
                                 HighlightName4 = this.HighlightName4,
                                 HighlightName5 = this.HighlightName5,
                                 HighlightName6 = this.HighlightName6,
                                 UseHighlights = this.UseHighlights,
                                 UseRemoteStorage = this.UseRemoteStorage
                             };
            cloned.highlighter = this.highlighter;
            return cloned;
        }

        #endregion
    }

    public class Highlighter
    {
        public enum Highlight
        {
            COLOR_1 = 0,
            COLOR_2 = 1,
            COLOR_3 = 2,
            COLOR_4 = 3,
            COLOR_5 = 4,
            COLOR_6 = 5,
            COLOR_NONE = 6
        }

        private Dictionary<string, Highlight> HighlightedVerses = new Dictionary<string, Highlight>();

        public Highlight GetHighlightForChapter(int chapterNumber, int verseNumber)
        {
            var highlight = Highlight.COLOR_NONE;
            if (!HighlightedVerses.TryGetValue(GetChapterVerseKey(chapterNumber, verseNumber), out highlight))
            {
                return Highlight.COLOR_NONE;
            }

            return highlight;
        }

        public void AddHighlight(int chapter, int verse, Highlight color)
        {
            HighlightedVerses[GetChapterVerseKey(chapter, verse)] = color;
        }

        public void RemoveHighlight(int chapter, int verse)
        {
            HighlightedVerses.Remove(GetChapterVerseKey(chapter, verse));
        }

        private string GetChapterVerseKey(int chapter, int verse)
        {
            return chapter + "_" + verse;
        }

        public string ToString()
        {
            var xmlReturn = new StringBuilder("<highlightedverses>");
            foreach (var item in HighlightedVerses)
            {
                xmlReturn.Append("<hl key=\"" + item.Key + "\" val=\"" + item.Value + "\" />");
            }

            xmlReturn.Append("</highlightedverses>");
            return xmlReturn.ToString();
        }

        public void FromStream(Stream stream)
        {
            HighlightedVerses = new Dictionary<string, Highlight>();
            using (XmlReader reader = XmlReader.Create(stream, new XmlReaderSettings { IgnoreWhitespace = true }))
            {
                // Parse the file and get each of the nodes.
                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            if (reader.Name.ToLower().Equals("hl") && reader.HasAttributes)
                            {
                                string key = string.Empty;
                                string value = string.Empty;
                                reader.MoveToFirstAttribute();
                                do
                                {
                                    switch (reader.Name.ToLower())
                                    {
                                        case "key":
                                            key = reader.Value;
                                            break;
                                        case "val":
                                            value = reader.Value;
                                            break;
                                    }
                                }
                                while (reader.MoveToNextAttribute());
                                Highlight foundHighlight;
                                if (Enum.TryParse(value, out foundHighlight))
                                {
                                    HighlightedVerses[key] = foundHighlight;
                                }
                            }
                            break;
                    }
                }
            }
        }

        public void FromString(string buffer)
        {
            this.FromByteArray(Encoding.UTF8.GetBytes(buffer));
        }

        private void FromByteArray(byte[] buffer)
        {
            var stream = new MemoryStream(buffer);
            this.FromStream(stream);
            stream.Dispose();
        }
    }
}