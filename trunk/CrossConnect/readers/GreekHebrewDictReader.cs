#region Header

// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GreekHebrewDictReader.cs" company="">
//
// </copyright>
// <summary>
//   Load from a file all the book and verse pointers to the bzz file so that
//   we can later read the bzz file quickly and efficiently.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GreekHebrewDictReader.cs" company="Thomas Dilts">
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

namespace CrossConnect.readers
{
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;
    using System.Runtime.Serialization;
    using System.Xml;

    using ICSharpCode.SharpZipLib.GZip;

    using Sword.reader;

    /// <summary>
    /// Load from a file all the book and verse pointers to the bzz file so that
    ///   we can later read the bzz file quickly and efficiently.
    /// </summary>
    [DataContract(Name = "GreekHebrewDictReader")]
    [KnownType(typeof(ChapterPos))]
    [KnownType(typeof(BookPos))]
    [KnownType(typeof(VersePos))]
    public class GreekHebrewDictReader : BibleZtextReader
    {
        #region Fields

        /// <summary>
        /// The link.
        /// </summary>
        [DataMember]
        public string Link = string.Empty;

        /// <summary>
        /// The greek dict.
        /// </summary>
        private static readonly LexiconFromXmlFile GreekDict = new LexiconFromXmlFile("strongsgreek.xml.gz");

        /// <summary>
        /// The hebrew dict.
        /// </summary>
        private static readonly LexiconFromXmlFile HebrewDict = new LexiconFromXmlFile("strongshebrew.xml.gz");

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="GreekHebrewDictReader"/> class.
        /// </summary>
        /// <param name="path">
        /// The path.
        /// </param>
        /// <param name="iso2DigitLangCode">
        /// The iso 2 digit lang code.
        /// </param>
        /// <param name="isIsoEncoding">
        /// The is iso encoding.
        /// </param>
        public GreekHebrewDictReader(string path, string iso2DigitLangCode, bool isIsoEncoding)
            : base(path, iso2DigitLangCode, isIsoEncoding)
        {
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets a value indicating whether IsExternalLink.
        /// </summary>
        public override bool IsExternalLink
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets a value indicating whether IsHearable.
        /// </summary>
        public override bool IsHearable
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets a value indicating whether IsPageable.
        /// </summary>
        public override bool IsPageable
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets a value indicating whether IsSearchable.
        /// </summary>
        public override bool IsSearchable
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets a value indicating whether IsSynchronizeable.
        /// </summary>
        public override bool IsSynchronizeable
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets a value indicating whether IsTranslateable.
        /// </summary>
        public override bool IsTranslateable
        {
            get
            {
                return true;
            }
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// The get info.
        /// </summary>
        /// <param name="bookNum">
        /// The book num.
        /// </param>
        /// <param name="absouteChaptNum">
        /// The absoute chapt num.
        /// </param>
        /// <param name="relChaptNum">
        /// The rel chapt num.
        /// </param>
        /// <param name="verseNum">
        /// The verse num.
        /// </param>
        /// <param name="fullName">
        /// The full name.
        /// </param>
        /// <param name="title">
        /// The title.
        /// </param>
        public override void GetInfo(
            out int bookNum,
            out int absouteChaptNum,
            out int relChaptNum,
            out int verseNum,
            out string fullName,
            out string title)
        {
            verseNum = 0;
            absouteChaptNum = 0;
            bookNum = 0;
            relChaptNum = 0;
            fullName = Link;

            // <string key="Greek dictionary internet link">Greek dictionary internet link</string>
            // <string key="Hebrew dictionary internet link">Hebrew dictionary internet link</string>
            title = "Dictionary - " + (Link.StartsWith("G") ? "Greek " : "Hebrew ") + Link.Substring(1);
        }

        /// <summary>
        /// The get language.
        /// </summary>
        /// <returns>
        /// The get language.
        /// </returns>
        public override string GetLanguage()
        {
            return "en";
        }

        /// <summary>
        /// The get translateable texts.
        /// </summary>
        /// <param name="displaySettings">
        /// The display settings.
        /// </param>
        /// <param name="bibleToLoad">
        /// The bible to load.
        /// </param>
        /// <param name="toTranslate">
        /// The to translate.
        /// </param>
        /// <param name="isTranslateable">
        /// The is translateable.
        /// </param>
        public override void GetTranslateableTexts(
            DisplaySettings displaySettings, string bibleToLoad, out string[] toTranslate, out bool[] isTranslateable)
        {
            toTranslate = new string[2];
            isTranslateable = new bool[2];

            int number;
            if (int.TryParse(Link.Substring(1), out number))
            {
            }

            LexiconEntry lexiconEntry = null;
            if (Link.StartsWith("G") && GreekDict.Dict.TryGetValue(number, out lexiconEntry))
            {
            }
            else if (Link.StartsWith("H") && HebrewDict.Dict.TryGetValue(number, out lexiconEntry))
            {
            }

            if (lexiconEntry != null)
            {
                toTranslate[0] = "<p>" + lexiconEntry.Untranslateable + "</p>";
                isTranslateable[0] = false;
                toTranslate[1] = lexiconEntry.Value;
                toTranslate[1] = ShowReferences(toTranslate[1], lexiconEntry, true);
                isTranslateable[1] = true;
            }
        }

        /// <summary>
        /// The show link.
        /// </summary>
        /// <param name="link">
        /// The link.
        /// </param>
        public void ShowLink(string link)
        {
            Link = link;
        }

        /// <summary>
        /// The get chapter html.
        /// </summary>
        /// <param name="displaySettings">
        /// The display settings.
        /// </param>
        /// <param name="htmlBackgroundColor">
        /// The html background color.
        /// </param>
        /// <param name="htmlForegroundColor">
        /// The html foreground color.
        /// </param>
        /// <param name="htmlPhoneAccentColor">
        /// The html phone accent color.
        /// </param>
        /// <param name="htmlFontSize">
        /// The html font size.
        /// </param>
        /// <param name="fontFamily">
        /// The font family.
        /// </param>
        /// <param name="isNotesOnly">
        /// The is notes only.
        /// </param>
        /// <param name="addStartFinishHtml">
        /// The add start finish html.
        /// </param>
        /// <param name="forceReload">
        /// The force reload.
        /// </param>
        /// <returns>
        /// The get chapter html.
        /// </returns>
        protected override string GetChapterHtml(
            DisplaySettings displaySettings,
            string htmlBackgroundColor,
            string htmlForegroundColor,
            string htmlPhoneAccentColor,
            double htmlFontSize,
            string fontFamily,
            bool isNotesOnly,
            bool addStartFinishHtml,
            bool forceReload)
        {
            string displayText = string.Empty;
            int number;
            if (int.TryParse(Link.Substring(1), out number))
            {
            }

            LexiconEntry lexiconEntry = null;
            if (Link.StartsWith("G") && GreekDict.Dict.TryGetValue(number, out lexiconEntry))
            {
            }
            else if (Link.StartsWith("H") && HebrewDict.Dict.TryGetValue(number, out lexiconEntry))
            {
            }

            if (lexiconEntry != null)
            {
                displayText = "<p>" + lexiconEntry.Untranslateable + "</p>";
                displayText += lexiconEntry.Value;
                displayText = ShowReferences(displayText, lexiconEntry, true);
            }

            // Debug.WriteLine("SearchReader GetChapterHtml.text=" + displayText);
            return HtmlHeader(
                displaySettings,
                htmlBackgroundColor,
                htmlForegroundColor,
                htmlPhoneAccentColor,
                htmlFontSize,
                fontFamily) + displayText + "</body></html>";
        }

        /// <summary>
        /// The show references.
        /// </summary>
        /// <param name="displayText">
        /// The display text.
        /// </param>
        /// <param name="lexiconEntry">
        /// The lexicon entry.
        /// </param>
        /// <param name="showRecursively">
        /// The show recursively.
        /// </param>
        /// <returns>
        /// The show references.
        /// </returns>
        private string ShowReferences(string displayText, LexiconEntry lexiconEntry, bool showRecursively)
        {
            LexiconEntry foundEntry;

            // write all the references
            foreach (int item in lexiconEntry.GreekRelatedKeys)
            {
                if (GreekDict.Dict.TryGetValue(item, out foundEntry))
                {
                    displayText += "<hr /><p>See also Greek " + foundEntry.Untranslateable + "</p>" + foundEntry.Value;
                    if (showRecursively)
                    {
                        displayText = ShowReferences(displayText, foundEntry, false);
                    }
                }
            }

            foreach (int item in lexiconEntry.HebrewRelatedKeys)
            {
                if (HebrewDict.Dict.TryGetValue(item, out foundEntry))
                {
                    displayText += "<hr /><p>See also Hebrew " + foundEntry.Untranslateable + "</p>" + foundEntry.Value;
                    if (showRecursively)
                    {
                        displayText = ShowReferences(displayText, foundEntry, false);
                    }
                }
            }

            return displayText;
        }

        #endregion Methods

        #region Nested Types

        /// <summary>
        /// The lexicon entry.
        /// </summary>
        public class LexiconEntry
        {
            #region Fields

            /// <summary>
            /// The greek related keys.
            /// </summary>
            public List<int> GreekRelatedKeys = new List<int>();

            /// <summary>
            /// The hebrew related keys.
            /// </summary>
            public List<int> HebrewRelatedKeys = new List<int>();

            /// <summary>
            /// The key.
            /// </summary>
            public int Key;

            /// <summary>
            /// The untranslateable.
            /// </summary>
            public string Untranslateable;

            /// <summary>
            /// The value.
            /// </summary>
            public string Value;

            #endregion Fields

            #region Constructors

            /// <summary>
            /// Initializes a new instance of the <see cref="LexiconEntry"/> class.
            /// </summary>
            /// <param name="key">
            /// The key.
            /// </param>
            public LexiconEntry(int key)
            {
                Key = key;
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="LexiconEntry"/> class.
            /// </summary>
            public LexiconEntry()
            {
            }

            #endregion Constructors
        }

        /// <summary>
        /// The lexicon from xml file.
        /// </summary>
        public class LexiconFromXmlFile
        {
            #region Fields

            /// <summary>
            /// The dict.
            /// </summary>
            public Dictionary<int, LexiconEntry> Dict = new Dictionary<int, LexiconEntry>();

            #endregion Fields

            #region Constructors

            /// <summary>
            /// Initializes a new instance of the <see cref="LexiconFromXmlFile"/> class.
            /// </summary>
            /// <param name="filepath">
            /// The filepath.
            /// </param>
            public LexiconFromXmlFile(string filepath)
            {
                Assembly assem = Assembly.GetExecutingAssembly();
                Stream stream = assem.GetManifestResourceStream("CrossConnect.Properties." + filepath);
                var gzip = new GZipInputStream(stream);

                string entry = string.Empty;
                LexiconEntry lexEntry = null;
                using (XmlReader reader = XmlReader.Create(gzip))
                {
                    // Parse the file and get each of the nodes.
                    while (reader.Read())
                    {
                        switch (reader.NodeType)
                        {
                            case XmlNodeType.Element:

                                switch (reader.Name)
                                {
                                    case "i":
                                        lexEntry = new LexiconEntry();
                                        reader.MoveToFirstAttribute();
                                        do
                                        {
                                            if (reader.Name.Equals("k"))
                                            {
                                                lexEntry.Key = int.Parse(reader.Value);
                                            }
                                            else if (reader.Name.Equals("u"))
                                            {
                                                lexEntry.Untranslateable = reader.Value;
                                            }
                                        }
                                        while (reader.MoveToNextAttribute());
                                        Dict[lexEntry.Key] = lexEntry;
                                        break;
                                    case "h":
                                        reader.MoveToFirstAttribute();
                                        if (lexEntry != null)
                                        {
                                            lexEntry.HebrewRelatedKeys.Add(int.Parse(reader.Value));
                                        }

                                        break;
                                    case "g":
                                        reader.MoveToFirstAttribute();
                                        if (lexEntry != null)
                                        {
                                            lexEntry.GreekRelatedKeys.Add(int.Parse(reader.Value));
                                        }

                                        break;
                                }

                                break;
                            case XmlNodeType.Text:
                                entry += reader.Value;
                                break;
                            case XmlNodeType.EndElement:
                                switch (reader.Name)
                                {
                                    case "i":
                                        if (lexEntry != null)
                                        {
                                            lexEntry.Value = entry;
                                        }

                                        entry = string.Empty;
                                        break;
                                }

                                break;
                        }
                    }
                }
            }

            #endregion Constructors
        }

        #endregion Nested Types

        #region Other

        /*
        void readFromDatFile()
        {
            StreamReader sr = new StreamReader(@"C:\Users\tds.ES\Desktop\strongsgreek.dat");
            XmlWriter xmlw = XmlWriter.Create(@"C:\Users\tds.ES\Desktop\strongsgreek.xml");
            xmlw.WriteStartElement("lexicon");
            Dictionary<int, string> dict = new Dictionary<int, string>();
            string entry = "";
            int entryKey = 0;
            string row;
            try
            {
                while (!sr.EndOfStream)
                {
                    row = sr.ReadLine();
                    if (row.StartsWith("$$T"))
                    {
                        int newKey = int.Parse(row.Substring(3));
                        if (!string.IsNullOrEmpty(entry))
                        {
                            xmlw.WriteValue(entry);
                            xmlw.WriteEndElement();

                            dict[entryKey] = entry;
                            entry = "";
                        }
                        entryKey = newKey;
                        xmlw.WriteStartElement("i");
                        xmlw.WriteAttributeString("k", entryKey.ToString());

                        //dump a row
                        sr.ReadLine();
                        row = sr.ReadLine();
                    }

                    if (row.StartsWith(" see HEBREW for "))
                    {
                        try
                        {
                            int key = int.Parse(row.Substring(" see HEBREW for ".Length).Replace("l", "1"));
                            xmlw.WriteStartElement("h");
                            xmlw.WriteAttributeString("k", key.ToString());
                            xmlw.WriteEndElement();
                        }
                        catch (Exception)
                        {
                        }
                    }
                    else if (row.StartsWith(" see GREEK for "))
                    {
                        try
                        {
                            int key = int.Parse(row.Substring(" see GREEK for ".Length).Replace("l", "1"));
                            xmlw.WriteStartElement("g");
                            xmlw.WriteAttributeString("k", key.ToString());
                            xmlw.WriteEndElement();
                        }
                        catch (Exception)
                        {
                        }
                    }
                    else
                    {
                        entry += row;
                    }
                }
            }
            catch (Exception eee)
            {
            }
            xmlw.WriteValue(entry);
            xmlw.WriteEndElement();
            sr.Close();

            xmlw.WriteEndElement();
            xmlw.Close();

        }*/

        #endregion Other
    }
}