#region Header

// <copyright file="CommentZtextReader.cs" company="Thomas Dilts">
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
    using System.Diagnostics;
    using System.IO;
    using System.Runtime.Serialization;
    using System.Text;
    using System.Threading.Tasks;
    using System.Xml;
    using System.Linq;
    using Windows.Storage;

    /// <summary>
    ///     Load from a file all the book and verse pointers to the bzz file so that
    ///     we can later read the bzz file quickly and efficiently.
    /// </summary>
    [DataContract(Name = "CommentZtextReader")]
    [KnownType(typeof(ChapterPos))]
    [KnownType(typeof(BookPos))]
    [KnownType(typeof(VersePos))]
    public class CommentZtextReader : BibleZtextReader
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Load from a file all the book and verse pointers to the bzz file so that
        ///     we can later read the bzz file quickly and efficiently.
        /// </summary>
        /// <param name="path">The path to where the ot.bzs,ot.bzv and ot.bzz and nt files are</param>
        /// <param name="iso2DigitLangCode"></param>
        /// <param name="isIsoEncoding"></param>
        public CommentZtextReader(string path, string iso2DigitLangCode, bool isIsoEncoding, string cipherKey, string configPath, string versification)
            : base(path, iso2DigitLangCode, isIsoEncoding, cipherKey, configPath, versification)
        {
        }

        #endregion

        #region Public Properties
        public override bool IsTTChearable
        {
            get
            {
                return true;
            }
        }
        public override bool IsHearable
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
        /*
        protected override string ParseOsisText(
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
                return chapterNumber + System.Text.UTF8Encoding.UTF8.GetString(xmlbytes, 0, xmlbytes.Length) + "</a>";
            }
            catch (Exception ee)
            {
                Debug.WriteLine("crashed in a strange place. err=" + ee.StackTrace);
            }
            return string.Empty;
        }*/


        protected override async Task<string> GetChapterHtml(
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
            bool forceReload,
            bool isSmallScreen)
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
                    fontFamily,
                    isSmallScreen,
                    this.Serial.Iso2DigitLangCode);
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
                        var texts = await ParseOsisText(
                            displaySettings,
                            startText,
                            restartText,
                            chapterBuffer,
                            (int)verse.StartPos,
                            verse.Length,
                            this.Serial.IsIsoEncoding,
                            isNotesOnly,
                            false,
                            noteIdentifier,
                            isInPoetry);
                        verseTxt = texts[0];
                        isInPoetry = bool.Parse(texts[1]);
                        noteIdentifier = int.Parse(texts[2]);
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
                     "<p>" + chapterStartHtml + verseTxt
                    + (verseTxt.Length > 0 ? "</a></p>" : string.Empty));
                chapterStartHtml = string.Empty;
            }

            htmlChapter.Append(chapterEndHtml);
            Debug.WriteLine("GetChapterHtml end");
            return htmlChapter.ToString();
        }

    }
}