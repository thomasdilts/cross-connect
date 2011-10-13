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
/// <copyright file="GreekHebrewDictReader.cs" company="Thomas Dilts">
///     Thomas Dilts. All rights reserved.
/// </copyright>
/// <author>Thomas Dilts</author>
namespace CrossConnect.readers
{
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;
    using System.Runtime.Serialization;
    using System.Xml;

    using ICSharpCode.SharpZipLib.GZip;

    using SwordBackend;

    /// <summary>
    /// Load from a file all the book and verse pointers to the bzz file so that
    /// we can later read the bzz file quickly and efficiently.
    /// </summary>
    /// <param name="path">The path to where the ot.bzs,ot.bzv and ot.bzz and nt files are</param>
    [DataContract(Name = "GreekHebrewDictReader")]
    [KnownType(typeof(ChapterPos))]
    [KnownType(typeof(BookPos))]
    [KnownType(typeof(VersePos))]
    public class GreekHebrewDictReader : BibleZtextReader
    {
        #region Fields

        [DataMember]
        public string link = string.Empty;

        private static LexiconFromXmlFile greekDict = new LexiconFromXmlFile("strongsgreek.xml.gz");
        private static LexiconFromXmlFile hebrewDict = new LexiconFromXmlFile("strongshebrew.xml.gz");

        #endregion Fields

        #region Constructors

        public GreekHebrewDictReader(string path, string iso2DigitLangCode, bool isIsoEncoding)
            : base(path, iso2DigitLangCode, isIsoEncoding)
        {
        }

        #endregion Constructors

        #region Properties

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

        #endregion Properties

        #region Methods

        public override void GetInfo(out int bookNum, out int absouteChaptNum, out int relChaptNum, out int verseNum, out string fullName, out string title)
        {
            verseNum = 0;
            absouteChaptNum = 0;
            bookNum = 0;
            relChaptNum = 0;
            fullName = this.link;
            //<string key="Greek dictionary internet link">Greek dictionary internet link</string>
            //<string key="Hebrew dictionary internet link">Hebrew dictionary internet link</string>
            title = "Dictionary - " + (link.StartsWith("G") ? "Greek " : "Hebrew ") + link.Substring(1);
        }

        public override string GetLanguage()
        {
            return "en";
        }

        public override void GetTranslateableTexts(DisplaySettings displaySettings, string bibleToLoad, out string[] toTranslate, out bool[] isTranslateable)
        {
            toTranslate = new string[2];
            isTranslateable = new bool[2];

            string strongNumber = "";
            int number;
            if (int.TryParse(link.Substring(1), out number))
            {
                strongNumber = number.ToString();
            }
            LexiconEntry lexiconEntry = null;
            if (link.StartsWith("G") && greekDict.dict.TryGetValue(number, out lexiconEntry))
            {
            }
            else if (link.StartsWith("H") && hebrewDict.dict.TryGetValue(number, out lexiconEntry))
            {
            }
            if (lexiconEntry != null)
            {
                toTranslate[0] = "<p>" + lexiconEntry.untranslateable + "</p>";
                isTranslateable[0] = false;
                toTranslate[1] = lexiconEntry.value;
                toTranslate[1] = ShowReferences(toTranslate[1], lexiconEntry, true);
                isTranslateable[1] = true;
            }
        }

        public void ShowLink(string link)
        {
            this.link = link;
        }

        protected override string GetChapterHtml(DisplaySettings displaySettings, string htmlBackgroundColor, string htmlForegroundColor, string htmlPhoneAccentColor, double htmlFontSize, bool isNotesOnly, bool addStartFinishHtml = true)
        {
            string displayText = "";
            string strongNumber = "";
            int number;
            if (int.TryParse(link.Substring(1), out number))
            {
                strongNumber = number.ToString();
            }
            LexiconEntry lexiconEntry = null;
            if (link.StartsWith("G") && greekDict.dict.TryGetValue(number, out lexiconEntry))
            {
            }
            else if (link.StartsWith("H") && hebrewDict.dict.TryGetValue(number, out lexiconEntry))
            {
            }
            if (lexiconEntry != null)
            {
                displayText = "<p>" + lexiconEntry.untranslateable + "</p>";
                displayText += lexiconEntry.value;
                displayText = ShowReferences(displayText, lexiconEntry, true);
            }

            //Debug.WriteLine("SearchReader GetChapterHtml.text=" + displayText);
            return HtmlHeader(displaySettings, htmlBackgroundColor, htmlForegroundColor, htmlPhoneAccentColor, htmlFontSize)
                + displayText + "</body></html>";
        }

        private string ShowReferences(string displayText, LexiconEntry lexiconEntry, bool showRecursively)
        {
            LexiconEntry foundEntry = null;
            //write all the references
            foreach (var item in lexiconEntry.GreekRelatedKeys)
            {
                if (greekDict.dict.TryGetValue(item, out foundEntry))
                {
                    displayText += "<hr /><p>See also Greek " + foundEntry.untranslateable + "</p>" + foundEntry.value;
                    if (showRecursively)
                    {
                        displayText = ShowReferences(displayText, foundEntry, false);
                    }
                }
            }
            foreach (var item in lexiconEntry.HebrewRelatedKeys)
            {
                if (hebrewDict.dict.TryGetValue(item, out foundEntry))
                {
                    displayText += "<hr /><p>See also Hebrew " + foundEntry.untranslateable + "</p>" + foundEntry.value;
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

        public class LexiconEntry
        {
            #region Fields

            public List<int> GreekRelatedKeys = new List<int>();
            public List<int> HebrewRelatedKeys = new List<int>();
            public int key;
            public string untranslateable;
            public string value;

            #endregion Fields

            #region Constructors

            public LexiconEntry(int key)
            {
                this.key = key;
            }

            public LexiconEntry()
            {
            }

            #endregion Constructors
        }

        public class LexiconFromXmlFile
        {
            #region Fields

            public Dictionary<int, LexiconEntry> dict = new Dictionary<int, LexiconEntry>();

            #endregion Fields

            #region Constructors

            public LexiconFromXmlFile(string filepath)
            {
                Assembly assem = Assembly.GetExecutingAssembly();
                Stream stream = assem.GetManifestResourceStream("CrossConnect.Properties." + filepath);
                GZipInputStream gzip = new GZipInputStream(stream);

                int key;
                string entry="";
                LexiconEntry lexEntry=null;
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
                                                lexEntry.key = int.Parse(reader.Value);
                                            }
                                            else if (reader.Name.Equals("u"))
                                            {
                                                lexEntry.untranslateable = reader.Value;
                                            }
                                        } while (reader.MoveToNextAttribute());
                                        dict[lexEntry.key] = lexEntry;
                                        break;
                                    case "h":
                                        reader.MoveToFirstAttribute();
                                        lexEntry.HebrewRelatedKeys.Add(int.Parse(reader.Value));
                                        key = int.Parse(reader.Value);
                                        break;
                                    case "g":
                                        reader.MoveToFirstAttribute();
                                        lexEntry.GreekRelatedKeys.Add(int.Parse(reader.Value));
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
                                        lexEntry.value = entry;
                                        entry = "";
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