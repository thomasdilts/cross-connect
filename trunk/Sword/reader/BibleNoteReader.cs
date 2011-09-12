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
/// <copyright file="BibleNoteReader.cs" company="Thomas Dilts">
///     Thomas Dilts. All rights reserved.
/// </copyright>
/// <author>Thomas Dilts</author>
namespace SwordBackend
{
    using System.Runtime.Serialization;

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

        [DataMember]
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

        #endregion Properties

        #region Methods

        public override void GetInfo(out int bookNum, out int absoluteChaptNum, out int relChaptNum, out int verseNum, out string fullName, out string title)
        {
            verseNum=this.serial.posVerseNum;
            absoluteChaptNum = this.serial.posChaptNum;
            base.GetInfo(this.serial.posChaptNum, this.serial.posVerseNum, out bookNum, out relChaptNum, out fullName, out title);
            title = titleBrowserWindow + " " + fullName + ":" + (relChaptNum + 1);
        }

        public override string GetVerseTextOnly(DisplaySettings displaySettings, int chapterNumber, int verseNumber)
        {
            //give them the notes if you can.
            byte[] chapterBuffer = getChapterBytes(chapterNumber);
            BibleZtextReader.VersePos verse = chapters[chapterNumber].verses[verseNumber];
            int noteMarker = 'a';
            return parseOsisText(displaySettings,
                "",
                "",
                chapterBuffer,
                (int)verse.startPos,
                verse.length,
                serial.isIsoEncoding,
                true,
                true,
                ref noteMarker);
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

        protected override string GetChapterHtml(DisplaySettings displaySettings, string htmlBackgroundColor, string htmlForegroundColor, string htmlPhoneAccentColor, double htmlFontSize, bool isNotesOnly, bool addStartFinishHtml = true)
        {
            return GetChapterHtml(displaySettings, serial.posChaptNum, htmlBackgroundColor, htmlForegroundColor, htmlPhoneAccentColor, htmlFontSize,true, addStartFinishHtml);
        }

        #endregion Methods
    }

    [DataContract(Name = "DisplaySettings")]
    public class DisplaySettings
    {
        #region Fields

        [DataMember]
        public string customBibleDownloadLinks = @"www.chaniel.se,/crossconnect/bibles/raw,/crossconnect/bibles/biblelist";
        [DataMember]
        public bool eachVerseNewLine = false;
        [DataMember]
        public string greekDictionaryLink = @"http://www.eliyah.com/cgi-bin/strongs.cgi?file=greeklexicon&isindex={0}";
        [DataMember]
        public string hebrewDictionaryLink = @"http://www.eliyah.com/cgi-bin/strongs.cgi?file=hebrewlexicon&isindex={0}";
        [DataMember]
        public bool highlightMarkings = false;
        [DataMember]
        public bool showAddedNotesByChapter = false;
        [DataMember]
        public bool showBookName = false;
        [DataMember]
        public bool showChapterNumber = false;
        [DataMember]
        public bool showHeadings = true;
        [DataMember]
        public bool showMorphology = false;
        [DataMember]
        public bool showNotePositions = false;
        [DataMember]
        public bool showStrongsNumbers = false;
        [DataMember]
        public bool showVerseNumber = true;
        [DataMember]
        public bool smallVerseNumbers = true;
        [DataMember]
        public string soundLink = @"http://www.chaniel.se/crossconnect/bibles/talking/getabsolutechapter.php?chapternum={0}&language={1}";
        [DataMember]
        public bool wordsOfChristRed = false;

        #endregion Fields

        #region Methods

        public void CheckForNullAndFix()
        {
            DisplaySettings fixer = new DisplaySettings();
            if (soundLink == null)
            {
                soundLink = fixer.soundLink;
            }
            if (greekDictionaryLink == null)
            {
                greekDictionaryLink = fixer.greekDictionaryLink;
            }
            if (hebrewDictionaryLink == null)
            {
                hebrewDictionaryLink = fixer.hebrewDictionaryLink;
            }
            if (customBibleDownloadLinks == null)
            {
                customBibleDownloadLinks = fixer.customBibleDownloadLinks;
            }

        }

        public DisplaySettings clone()
        {
            DisplaySettings cloned = new DisplaySettings();
            cloned.customBibleDownloadLinks = customBibleDownloadLinks;
            cloned.eachVerseNewLine = eachVerseNewLine;
            cloned.greekDictionaryLink = greekDictionaryLink;
            cloned.hebrewDictionaryLink = hebrewDictionaryLink;
            cloned.highlightMarkings = highlightMarkings;
            cloned.showAddedNotesByChapter = showAddedNotesByChapter;
            cloned.showBookName = showBookName;
            cloned.showChapterNumber = showChapterNumber;
            cloned.showHeadings = showHeadings;
            cloned.showMorphology = showMorphology;
            cloned.showNotePositions = showNotePositions;
            cloned.showStrongsNumbers = showStrongsNumbers;
            cloned.showVerseNumber = showVerseNumber;
            cloned.smallVerseNumbers = smallVerseNumbers;
            cloned.soundLink = soundLink;
            cloned.wordsOfChristRed = wordsOfChristRed;
            return cloned;
        }

        #endregion Methods
    }
}