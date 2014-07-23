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

                                                    isReferenceLinked = true;
                                                    string textId = bookShortName + "_" + chaptNumLoc + "_" + verseNumLoc;
                                                    plainText.Append(
                                                            "</a><a class=\"normalcolor\" id=\"ID_" + textId
                                                       + "\"  href=\"#\" onclick=\"window.external.notify('" + textId
                                                        + "'); event.returnValue=false; return false;\" >");
                                                }
                                            }
                                        }

                                        plainText.Append("  [");
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
                                            if (chapterNumber.Length > 0 && !isChaptNumGiven)
                                            {
                                                plainText.Append(chapterNumber);
                                                isChaptNumGiven = true;
                                            }

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
                                            if (chapterNumber.Length > 0 && !isChaptNumGiven)
                                            {
                                                plainText.Append(chapterNumber);
                                                isChaptNumGiven = true;
                                            }

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
                                            if (isNotesOnly)
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
                                                            if (lemma.StartsWith("strong:") || lemma.StartsWith("s:"))
                                                            {
                                                                if (!string.IsNullOrEmpty(lemmaText))
                                                                {
                                                                    lemmaText += ",";
                                                                }
                                                                var lemmaSplit = lemma.Split(':');
                                                                lemmaText +=
                                                                    "<a class=\"strongsmorph\" href=\"#\" onclick=\"window.external.notify('STRONG_"
                                                                    + lemmaSplit[1]
                                                                    + "'); event.returnValue=false; return false;\" >"
                                                                    + lemmaSplit[1].Substring(1) + "</a>";
                                                            }
                                                        }
                                                    }
                                                    else if (displaySettings.ShowMorphology
                                                             && reader.Name.Equals("morph"))
                                                    {
                                                        string[] morphs = reader.Value.Split(' ');
                                                        foreach (string morph in morphs)
                                                        {
                                                            if (morph.StartsWith("robinson:") || morph.StartsWith("m:") || morph.StartsWith("r:"))
                                                            {
                                                                var morphSplit = morph.Split(':');
                                                                string subMorph = morphSplit[1];
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
                                        plainText.Append("] ");
                                        if (isReferenceLinked)
                                        {
                                            plainText.Append("</a>" + restartText);
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