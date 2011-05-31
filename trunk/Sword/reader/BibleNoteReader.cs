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
/// <copyright file="THIS_FILE.cs" company="Thomas Dilts">
///     Thomas Dilts. All rights reserved.
/// </copyright>
/// <author>Thomas Dilts</author>
namespace SwordBackend
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.IO.IsolatedStorage;
    using System.Net;
    using System.Runtime.Serialization;
    using System.Text;

    using ComponentAce.Compression.Libs.zlib;

    /// <summary>
    /// Load from a file all the book and verse pointers to the bzz file so that
    /// we can later read the bzz file quickly and efficiently.
    /// </summary>
    /// <param name="path">The path to where the ot.bzs,ot.bzv and ot.bzz and nt files are</param>
    [DataContract(Name = "BibleNoteReader")]
    [KnownType(typeof(ChapterPos))]
    [KnownType(typeof(BookPos))]
    [KnownType(typeof(VersePos))]
    public class BibleNoteReader : BibleZtextReader
    {
        #region Fields

        [DataMember]
        public BibleZtextReaderSerialData serial2 = new BibleZtextReaderSerialData(false,"","",0,0);

        private string titleBrowserWindow = string.Empty;

        #endregion Fields

        #region Constructors

        public BibleNoteReader(string path, string iso2DigitLangCode, bool isIsoEncoding, string titleBrowserWindow)
            : base(path, iso2DigitLangCode, isIsoEncoding)
        {
            this.serial2.cloneFrom(base.serial);
            this.titleBrowserWindow = titleBrowserWindow;
        }

        #endregion Constructors

        #region Properties

        public override bool IsBookmarkable
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

        #endregion Properties

        #region Methods

        public override void GetInfo(out int bookNum, out int absoluteChaptNum, out int relChaptNum, out int verseNum, out string fullName, out string title)
        {
            verseNum=this.serial.posVerseNum;
            absoluteChaptNum = this.serial.posChaptNum;
            base.GetInfo(this.serial.posChaptNum, this.serial.posVerseNum, out bookNum, out relChaptNum, out fullName, out title);
            title = titleBrowserWindow + " " + fullName + ":" + (relChaptNum + 1);
        }

        public override string GetVerseTextOnly(int chapterNumber, int verseNumber)
        {
            //give them the notes if you can.
            byte[] chapterBuffer = getChapterBytes(chapterNumber);
            string chapter = System.Text.UTF8Encoding.UTF8.GetString(chapterBuffer, 0, chapterBuffer.Length);
            BibleZtextReader.VersePos verse = chapters[chapterNumber].verses[verseNumber];
            return parseOsisText(
                "",
                "",
                chapterBuffer,
                (int)verse.startPos,
                verse.length,
                serial.isIsoEncoding,
                true,
                true);
        }

        public override void Resume()
        {
            base.serial.cloneFrom(this.serial2);
            base.Resume();
        }

        public override void SerialSave()
        {
            this.serial2.cloneFrom(base.serial);
        }

        protected override string GetChapterHtml(string htmlBackgroundColor, string htmlForegroundColor, string htmlPhoneAccentColor, double htmlFontSize, bool isNotesOnly, bool addStartFinishHtml = true)
        {
            return GetChapterHtml(serial.posChaptNum, htmlBackgroundColor, htmlForegroundColor, htmlPhoneAccentColor, htmlFontSize,true, addStartFinishHtml);
        }

        #endregion Methods
    }
}