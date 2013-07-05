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
    using System.Runtime.Serialization;
    using System.Threading.Tasks;

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

        public BibleNoteReader(string path, string iso2DigitLangCode, bool isIsoEncoding, string titleBrowserWindow, string cipherKey, string configPath)
            : base(path, iso2DigitLangCode, isIsoEncoding, cipherKey, configPath)
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
                    this.Serial.PosChaptNum,
                    htmlBackgroundColor,
                    htmlForegroundColor,
                    htmlPhoneAccentColor,
                    htmlFontSize,
                    fontFamily,
                    true,
                    addStartFinishHtml,
                    forceReload);
        }

        public override void GetInfo(
            out int bookNum,
            out int absoluteChaptNum,
            out int relChaptNum,
            out int verseNum,
            out string fullName,
            out string title)
        {
            verseNum = this.Serial.PosVerseNum;
            absoluteChaptNum = this.Serial.PosChaptNum;
            this.GetInfo(
                this.Serial.PosChaptNum, this.Serial.PosVerseNum, out bookNum, out relChaptNum, out fullName, out title);
            title = this.TitleBrowserWindow + " " + fullName + ":" + (relChaptNum + 1);
        }

        public override async Task<string> GetVerseTextOnly(
            DisplaySettings displaySettings, int chapterNumber, int verseNumber)
        {
            //give them the notes if you can.
            Task<byte[]> chapterBuffer = this.GetChapterBytes(chapterNumber);
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
                                 UseInternetGreekHebrewDict = this.UseInternetGreekHebrewDict
                             };
            return cloned;
        }

        #endregion
    }
}