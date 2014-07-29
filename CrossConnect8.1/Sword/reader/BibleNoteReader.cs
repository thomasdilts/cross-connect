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

namespace CrossConnect.readers
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Runtime.Serialization;
    using System.Text;
    using System.Threading.Tasks;
    using System.Xml;
    using System.Linq;
    using Sword.reader;
    using Sword.versification;

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


        public override bool IsTTChearable
        {
            get
            {
                return false;
            }
        }
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
                    true,
                    addStartFinishHtml,
                    forceReload);
        }

        public override void GetInfo(
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
            title = this.TitleBrowserWindow + " " + fullName + ":" + (relChaptNum + 1);
        }

        public override async Task<string> GetVerseTextOnly(
            DisplaySettings displaySettings, string bookShortName, int chapterNumber, int verseNumber)
        {
            //give them the notes if you can.
            var book = canon.BookByShortName[bookShortName];
            Task<byte[]> chapterBuffer = this.GetChapterBytes(chapterNumber + book.VersesInChapterStartIndex);
            VersePos verse = this.Chapters[chapterNumber].Verses[verseNumber];
            int noteMarker = 0;
            bool isInPoetry = false;
            var texts = await this.ParseOsisText(
                displaySettings,
                "",
                "",
                await chapterBuffer,
                (int)verse.StartPos,
                verse.Length,
                this.Serial.IsIsoEncoding,
                true,
                true,
                noteMarker,
                isInPoetry);
            return texts[0];
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

}