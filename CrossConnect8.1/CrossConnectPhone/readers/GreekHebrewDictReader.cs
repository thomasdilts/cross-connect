#region Header

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
    using System.Threading.Tasks;
    using System.Xml;

    using Sword.reader;
    using ICSharpCode.SharpZipLib.GZip;
        //using ICSharpCode.SharpZipLib.GZip;

    /// <summary>
    ///     Load from a file all the book and verse pointers to the bzz file so that
    ///     we can later read the bzz file quickly and efficiently.
    /// </summary>
    [DataContract(Name = "GreekHebrewDictReader")]
    [KnownType(typeof(ChapterPos))]
    [KnownType(typeof(BookPos))]
    [KnownType(typeof(VersePos))]
    public class GreekHebrewDictReader : BibleZtextReader
    {
        #region Static Fields

        private static readonly LexiconFromXmlFile GreekDict = new LexiconFromXmlFile("strongsgreek.xml.gz");

        private static readonly LexiconFromXmlFile HebrewDict = new LexiconFromXmlFile("strongshebrew.xml.gz");

        #endregion

        #region Fields

        [DataMember]
        public string Link = string.Empty;

        #endregion

        #region Constructors and Destructors

        public GreekHebrewDictReader(string path, string iso2DigitLangCode, bool isIsoEncoding)
            : base(path, iso2DigitLangCode, isIsoEncoding, string.Empty, string.Empty, string.Empty)
        {
        }

        #endregion

        #region Public Properties

        public override bool IsExternalLink
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

        public override bool IsTTChearable
        {
            get
            {
                return false;
            }
        }

        public override bool IsPageable
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

        public override bool IsSynchronizeable
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
            HtmlColorRgba[] htmlHighlightColor,
            double htmlFontSize,
            string fontFamily,
            bool isNotesOnly,
            bool addStartFinishHtml,
            bool forceReload)
        {
            string displayText = string.Empty;
            int number;
            if (int.TryParse(this.Link.Substring(1), out number))
            {
            }

            LexiconEntry lexiconEntry = null;
            if (this.Link.StartsWith("G") && GreekDict.Dict.TryGetValue(number, out lexiconEntry))
            {
            }
            else if (this.Link.StartsWith("H") && HebrewDict.Dict.TryGetValue(number, out lexiconEntry))
            {
            }

            if (lexiconEntry != null)
            {
                displayText = "<p>" + lexiconEntry.Untranslateable + "</p>";
                displayText += lexiconEntry.Value;
                displayText = this.ShowReferences(displayText, lexiconEntry, true);
            }

            // Debug.WriteLine("SearchReader GetChapterHtml.text=" + displayText);
            return HtmlHeader(
                displaySettings,
                htmlBackgroundColor,
                htmlForegroundColor,
                htmlPhoneAccentColor,
                htmlWordsOfChristColor,
                htmlFontSize,
                fontFamily) + displayText + "</body></html>";
        }

        public override void GetInfo(
            string isoLangCode,
            out string bookShortName,
            out int relChaptNum,
            out int verseNum,
            out string fullName,
            out string title)
        {
            verseNum = 0;
            bookShortName = string.Empty;
            relChaptNum = 0;
            fullName = this.Link;

            // <string key="Greek dictionary internet link">Greek dictionary internet link</string>
            // <string key="Hebrew dictionary internet link">Hebrew dictionary internet link</string>
            title = "Dictionary - " + (this.Link.StartsWith("G") ? "Greek " : "Hebrew ") + this.Link.Substring(1);
        }

        public override string GetLanguage()
        {
            return "en";
        }

        public override async Task<object[]> GetTranslateableTexts(string isoLangCode, DisplaySettings displaySettings, string bibleToLoad)
        {
            var toTranslate = new string[2];
            var isTranslateable = new bool[2];

            int number;
            if (int.TryParse(this.Link.Substring(1), out number))
            {
            }

            LexiconEntry lexiconEntry = null;
            if (this.Link.StartsWith("G") && GreekDict.Dict.TryGetValue(number, out lexiconEntry))
            {
            }
            else if (this.Link.StartsWith("H") && HebrewDict.Dict.TryGetValue(number, out lexiconEntry))
            {
            }

            if (lexiconEntry != null)
            {
                toTranslate[0] = "<p>" + lexiconEntry.Untranslateable + "</p>";
                isTranslateable[0] = false;
                toTranslate[1] = lexiconEntry.Value;
                toTranslate[1] = this.ShowReferences(toTranslate[1], lexiconEntry, true);
                isTranslateable[1] = true;
            }
            return new object[] { toTranslate, isTranslateable };
        }

        public void ShowLink(string link)
        {
            this.Link = link;
        }

        #endregion

        #region Methods

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
                        displayText = this.ShowReferences(displayText, foundEntry, false);
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
                        displayText = this.ShowReferences(displayText, foundEntry, false);
                    }
                }
            }

            return displayText;
        }

        #endregion

        public class LexiconEntry
        {
            #region Fields

            public List<int> GreekRelatedKeys = new List<int>();

            public List<int> HebrewRelatedKeys = new List<int>();

            public int Key;

            public string Untranslateable;

            public string Value;

            #endregion

            #region Constructors and Destructors

            public LexiconEntry(int key)
            {
                this.Key = key;
            }

            public LexiconEntry()
            {
            }

            #endregion
        }

        public class LexiconFromXmlFile
        {
            #region Fields

            public Dictionary<int, LexiconEntry> Dict = new Dictionary<int, LexiconEntry>();

            #endregion

            #region Constructors and Destructors

            public LexiconFromXmlFile(string filepath)
            {
                Assembly assem = Assembly.Load(new AssemblyName("CrossConnect"));
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
                                        this.Dict[lexEntry.Key] = lexEntry;
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

            #endregion
        }

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
    }
}