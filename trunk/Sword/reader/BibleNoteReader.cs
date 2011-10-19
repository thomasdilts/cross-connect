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
// Email: thomas@chaniel.se
// </summary>
// <author>Thomas Dilts</author>

#endregion Header

namespace Sword.reader
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    ///   Load from a file all the book and verse pointers to the bzz file so that
    ///   we can later read the bzz file quickly and efficiently.
    /// </summary>
    [DataContract(Name = "BibleNoteReader")]
    [KnownType(typeof (ChapterPos))]
    [KnownType(typeof (BookPos))]
    [KnownType(typeof (VersePos))]
    public class BibleNoteReader : BibleZtextReader
    {
        #region Fields

        [DataMember]
        public BibleZtextReaderSerialData Serial2 = new BibleZtextReaderSerialData(false, "", "", 0, 0);

        [DataMember]
        private string _titleBrowserWindow = string.Empty;

        #endregion Fields

        #region Constructors

        public BibleNoteReader(string path, string iso2DigitLangCode, bool isIsoEncoding, string titleBrowserWindow)
            : base(path, iso2DigitLangCode, isIsoEncoding)
        {
            Serial2.CloneFrom(Serial);
            _titleBrowserWindow = titleBrowserWindow;
        }

        #endregion Constructors

        #region Properties

        public override bool IsHearable
        {
            get { return false; }
        }

        public override bool IsLocalChangeDuringLink
        {
            get { return false; }
        }

        public override bool IsSearchable
        {
            get { return false; }
        }

        public override bool IsTranslateable
        {
            get { return true; }
        }

        #endregion Properties

        #region Methods

        public override void GetInfo(out int bookNum, out int absoluteChaptNum, out int relChaptNum, out int verseNum,
            out string fullName, out string title)
        {
            verseNum = Serial.PosVerseNum;
            absoluteChaptNum = Serial.PosChaptNum;
            GetInfo(Serial.PosChaptNum, Serial.PosVerseNum, out bookNum, out relChaptNum, out fullName, out title);
            title = _titleBrowserWindow + " " + fullName + ":" + (relChaptNum + 1);
        }

        public override string GetVerseTextOnly(DisplaySettings displaySettings, int chapterNumber, int verseNumber)
        {
            //give them the notes if you can.
            var chapterBuffer = GetChapterBytes(chapterNumber);
            var verse = Chapters[chapterNumber].Verses[verseNumber];
            int noteMarker = 'a';
            return ParseOsisText(displaySettings,
                                 "",
                                 "",
                                 chapterBuffer,
                                 (int) verse.StartPos,
                                 verse.Length,
                                 Serial.IsIsoEncoding,
                                 true,
                                 true,
                                 ref noteMarker);
        }

        public override void Resume()
        {
            Serial.CloneFrom(Serial2);
            base.Resume();
        }

        public override void SerialSave()
        {
            Serial2.CloneFrom(Serial);
        }

        protected override string GetChapterHtml(DisplaySettings displaySettings, string htmlBackgroundColor,
            string htmlForegroundColor, string htmlPhoneAccentColor,
            double htmlFontSize, bool isNotesOnly, bool addStartFinishHtml = true)
        {
            return GetChapterHtml(displaySettings, Serial.PosChaptNum, htmlBackgroundColor, htmlForegroundColor,
                                  htmlPhoneAccentColor, htmlFontSize, true, addStartFinishHtml);
        }

        #endregion Methods
    }

    [DataContract(Name = "DisplaySettings")]
    public class DisplaySettings
    {
        #region Fields

        [DataMember]
        public string CustomBibleDownloadLinks = 
            @"www.chaniel.se,/crossconnect/bibles/raw,/crossconnect/bibles/biblelist";
        [DataMember]
        public bool EachVerseNewLine;
        [DataMember]
        public string GreekDictionaryLink = 
            @"http://www.eliyah.com/cgi-bin/strongs.cgi?file=greeklexicon&isindex={0}";
        [DataMember]
        public string HebrewDictionaryLink = 
            @"http://www.eliyah.com/cgi-bin/strongs.cgi?file=hebrewlexicon&isindex={0}";
        [DataMember]
        public bool HighlightMarkings;
        [DataMember]
        public bool ShowAddedNotesByChapter;
        [DataMember]
        public bool ShowBookName;
        [DataMember]
        public bool ShowChapterNumber;
        [DataMember]
        public bool ShowHeadings = true;
        [DataMember]
        public bool ShowMorphology;
        [DataMember]
        public bool ShowNotePositions;
        [DataMember]
        public bool ShowStrongsNumbers;
        [DataMember]
        public bool ShowVerseNumber = true;
        [DataMember]
        public bool SmallVerseNumbers = true;
        [DataMember]
        public string SoundLink = 
            @"http://www.chaniel.se/crossconnect/bibles/talking/getabsolutechapter.php?chapternum={0}&language={1}";
        [DataMember]
        public bool UseInternetGreekHebrewDict;
        [DataMember]
        public string UserUniqueGuuid = "";
        [DataMember]
        public bool WordsOfChristRed;

        #endregion Fields

        #region Methods

        public void CheckForNullAndFix()
        {
            var fixer = new DisplaySettings();
            if (SoundLink == null)
            {
                SoundLink = fixer.SoundLink;
            }
            if (GreekDictionaryLink == null)
            {
                GreekDictionaryLink = fixer.GreekDictionaryLink;
            }
            if (HebrewDictionaryLink == null)
            {
                HebrewDictionaryLink = fixer.HebrewDictionaryLink;
            }
            if (CustomBibleDownloadLinks == null)
            {
                CustomBibleDownloadLinks = fixer.CustomBibleDownloadLinks;
            }
            if (UserUniqueGuuid == null)
            {
                UserUniqueGuuid = Guid.NewGuid().ToString();
            }
        }

        public DisplaySettings Clone()
        {
            var cloned = new DisplaySettings
                             {
                                 CustomBibleDownloadLinks = CustomBibleDownloadLinks,
                                 EachVerseNewLine = EachVerseNewLine,
                                 GreekDictionaryLink = GreekDictionaryLink,
                                 HebrewDictionaryLink = HebrewDictionaryLink,
                                 HighlightMarkings = HighlightMarkings,
                                 ShowAddedNotesByChapter = ShowAddedNotesByChapter,
                                 ShowBookName = ShowBookName,
                                 ShowChapterNumber = ShowChapterNumber,
                                 ShowHeadings = ShowHeadings,
                                 ShowMorphology = ShowMorphology,
                                 ShowNotePositions = ShowNotePositions,
                                 ShowStrongsNumbers = ShowStrongsNumbers,
                                 ShowVerseNumber = ShowVerseNumber,
                                 SmallVerseNumbers = SmallVerseNumbers,
                                 SoundLink = SoundLink,
                                 WordsOfChristRed = WordsOfChristRed,
                                 UserUniqueGuuid = UserUniqueGuuid,
                                 UseInternetGreekHebrewDict = UseInternetGreekHebrewDict
                             };
            return cloned;
        }

        #endregion Methods
    }
}