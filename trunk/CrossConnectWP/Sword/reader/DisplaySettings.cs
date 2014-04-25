using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.IO;
using System.Xml;

namespace Sword.reader
{

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

        [DataMember(Name = "Show2titleRows")]
        public bool Show2titleRows;
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

        [DataMember]
        public string HighlightName1 = "Highlight 1";

        [DataMember]
        public string HighlightName2 = "Highlight 2";

        [DataMember]
        public string HighlightName3 = "Highlight 3";

        [DataMember]
        public string HighlightName4 = "Highlight 4";

        [DataMember]
        public string HighlightName5 = "Highlight 5";

        [DataMember]
        public string HighlightName6 = "Highlight 6";

        [DataMember]
        public bool UseHighlights = true;

        [DataMember]
        public bool SyncMediaVerses = true;

        public Highlighter highlighter = new Highlighter();
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
            if (string.IsNullOrEmpty(this.HighlightName1))
            {
                this.HighlightName1 = "Highlight 1";
            }
            if (string.IsNullOrEmpty(this.HighlightName2))
            {
                this.HighlightName2 = "Highlight 2";
            }
            if (string.IsNullOrEmpty(this.HighlightName3))
            {
                this.HighlightName3 = "Highlight 3";
            }
            if (string.IsNullOrEmpty(this.HighlightName4))
            {
                this.HighlightName4 = "Highlight 4";
            }
            if (string.IsNullOrEmpty(this.HighlightName5))
            {
                this.HighlightName5 = "Highlight 5";
            }
            if (string.IsNullOrEmpty(this.HighlightName6))
            {
                UseHighlights = true;
                this.HighlightName6 = "Highlight 6";
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
                Show2titleRows = this.Show2titleRows,
                NumberOfScreens = this.NumberOfScreens,
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
                UseInternetGreekHebrewDict = this.UseInternetGreekHebrewDict,
                HighlightName1 = this.HighlightName1,
                HighlightName2 = this.HighlightName2,
                HighlightName3 = this.HighlightName3,
                HighlightName4 = this.HighlightName4,
                HighlightName5 = this.HighlightName5,
                HighlightName6 = this.HighlightName6,
                UseHighlights = this.UseHighlights,
                SyncMediaVerses = this.SyncMediaVerses
            };
            cloned.highlighter = this.highlighter;
            return cloned;
        }

        #endregion
    }
    public class Highlighter
    {
        public enum Highlight
        {
            COLOR_1 = 0,
            COLOR_2 = 1,
            COLOR_3 = 2,
            COLOR_4 = 3,
            COLOR_5 = 4,
            COLOR_6 = 5,
            COLOR_NONE = 6
        }

        private Dictionary<string, Dictionary<int, Dictionary<int, BiblePlaceMarker>>> HighlightedVerses =
            new Dictionary<string, Dictionary<int, Dictionary<int, BiblePlaceMarker>>>();

        public Highlight GetHighlightForVerse(string bookShortName, int chapterNumber, int verseNumber)
        {
            Dictionary<int, Dictionary<int, BiblePlaceMarker>> chapters;
            Dictionary<int, BiblePlaceMarker> verses;
            BiblePlaceMarker verse;
            Highlight highlight;
            if (HighlightedVerses.TryGetValue(bookShortName, out chapters)
                && chapters.TryGetValue(chapterNumber, out verses)
                && verses.TryGetValue(verseNumber, out verse)
                && Enum.TryParse<Highlight>(verse.Note, out highlight))
            {
                return highlight;
            }

            return Highlight.COLOR_NONE;
        }

        public void AddHighlight(string bookShortName, int chapter, int verse, Highlight color)
        {
            AddBiblePlaceMarker(bookShortName, chapter, verse, color == Highlight.COLOR_NONE ? string.Empty : color.ToString(), HighlightedVerses);
        }

        public static void AddBiblePlaceMarker(string bookShortName, int chapter, int verse, string note,
            Dictionary<string, Dictionary<int, Dictionary<int, BiblePlaceMarker>>> markerList)
        {
            var place = new BiblePlaceMarker(bookShortName, chapter, verse, DateTime.Now) { Note = note };
            // erase the old first
            RemoveBiblePlaceMarker(place, markerList);
            // add the new
            if (!string.IsNullOrEmpty(note))
            {
                if (!markerList.ContainsKey(place.BookShortName))
                {
                    markerList.Add(place.BookShortName, new Dictionary<int, Dictionary<int, BiblePlaceMarker>>());
                }

                if (!markerList[place.BookShortName].ContainsKey(place.ChapterNum))
                {
                    markerList[place.BookShortName].Add(place.ChapterNum, new Dictionary<int, BiblePlaceMarker>());
                }

                markerList[place.BookShortName][place.ChapterNum][place.VerseNum] = place;
            }
        }

        public static void RemoveBiblePlaceMarker(BiblePlaceMarker place, Dictionary<string, Dictionary<int, Dictionary<int, BiblePlaceMarker>>> markerList)
        {
            if (markerList.ContainsKey(place.BookShortName)
                && markerList[place.BookShortName].ContainsKey(place.ChapterNum)
                && markerList[place.BookShortName][place.ChapterNum].ContainsKey(place.VerseNum))
            {
                markerList[place.BookShortName][place.ChapterNum].Remove(place.VerseNum);
                if (!markerList[place.BookShortName][place.ChapterNum].Any())
                {
                    markerList[place.BookShortName].Remove(place.ChapterNum);
                    if (!markerList[place.BookShortName].Any())
                    {
                        markerList.Remove(place.BookShortName);
                    }
                }
            }
        }

        public void RemoveHighlight(BiblePlaceMarker place)
        {
            RemoveBiblePlaceMarker(place, HighlightedVerses);
        }

        public string ToString()
        {
            return "<highlightedverses>\n" + ExportMarkersDictionary("highlight", HighlightedVerses) + "</highlightedverses>";
        }
        public string ToStringNoRoot()
        {
            return ExportMarkersDictionary("highlight", HighlightedVerses);
        }

        public static string ExportMarkersDictionary(string xmlName, Dictionary<string, Dictionary<int, Dictionary<int, BiblePlaceMarker>>> markerList)
        {
            var returnList = new List<BiblePlaceMarker>();
            foreach (var book in markerList)
            {
                foreach (var chapter in book.Value)
                {
                    foreach (var verse in chapter.Value)
                    {
                        returnList.Add(verse.Value);
                    }
                }
            }

            returnList.Sort(SortByBibleChapterVerse);
            return ExportMarkersList(xmlName, returnList);
        }
        private static Dictionary<string, int> _BooksByNumber = null;
        public static Dictionary<string, int> BooksByNumber
        {
            get
            {
                if (_BooksByNumber == null)
                {
                    _BooksByNumber = new Dictionary<string, int>();
                    var booksInOrder = BibleZtextReader.ChapterCategories.Keys.ToArray();
                    int i = 0;
                    foreach (var item in booksInOrder)
                    {
                        _BooksByNumber[item] = i;
                        i++;
                    }
                }
                return _BooksByNumber;
            }
        }

        public static int SortByBibleChapterVerse(BiblePlaceMarker name1, BiblePlaceMarker name2)
        {
            int nameNum1;
            int nameNum2;
            if (!BooksByNumber.TryGetValue(name1.BookShortName, out nameNum1))
            {
                nameNum1 = 999;
            }
            if (!BooksByNumber.TryGetValue(name2.BookShortName, out nameNum2))
            {
                nameNum2 = 999;
            }

            var nameCompare = nameNum1.CompareTo(nameNum2);
            var chaptCompare = name1.ChapterNum.CompareTo(name2.ChapterNum);
            var verseCompare = name1.VerseNum.CompareTo(name2.VerseNum);
            return nameCompare != 0 ? nameCompare : (chaptCompare != 0 ? chaptCompare : verseCompare);
        }
        public static string HtmlUnescape(string self)
        {
            if (!string.IsNullOrEmpty(self))
            {
                return self
                    .Replace("&quot;", "\"")
                    .Replace("&#39;", "'")
                    .Replace("&lt;", "<")
                    .Replace("&gt;", ">")
                    .Replace("&amp;", "&");
            }
            return self;

        }

        public static string HtmlEscape(string self)
        {
            if (!string.IsNullOrEmpty(self))
            {
                return self
                    .Replace("&", "&amp;")
                    .Replace(">", "&gt;")
                    .Replace("<", "&lt;")
                    .Replace("'", "&#39;")
                    .Replace("\"", "&quot;");
            }

            return self;
        }

        public static string ExportMarkersList(string xmlName, List<BiblePlaceMarker> markerList)
        {
            var returnList = new StringBuilder();
            foreach (var verse in markerList)
            {
                returnList.Append(string.Format("<{0} book=\"{1}\" chapter=\"{2}\" verse=\"{3}\"{4}\n", xmlName, verse.BookShortName, verse.ChapterNum + 1, verse.VerseNum + 1, string.IsNullOrEmpty(verse.Note) ? "/>" : ">" + HtmlEscape(verse.Note) + "</" + xmlName + ">"));
            }
            return returnList.ToString();
        }

        public static void EvaluateXmlRow(string book,int chapter, int verse, string note,bool invalidEntry,
            bool noteRequired, List<BiblePlaceMarker> markerList,
            Dictionary<string, Dictionary<int, Dictionary<int, BiblePlaceMarker>>> markerDictionary)
        {
            if (!invalidEntry
                && !string.IsNullOrEmpty(book)
                && BooksByNumber.ContainsKey(book)
                && chapter >= 0
                && verse >= 0
                && (!noteRequired || !string.IsNullOrEmpty(note)))
            {
                if (markerDictionary != null)
                {
                    AddBiblePlaceMarker(book, chapter, verse, note, markerDictionary);
                }
                if (markerList != null)
                {
                    var alreadyExisting = markerList.FirstOrDefault(p => p.BookShortName.Equals(book) && p.ChapterNum == chapter && p.VerseNum == verse);
                    if (alreadyExisting != null)
                    {
                        // just change it
                        alreadyExisting.Note = note;
                    }
                    else
                    {
                        markerList.Add(new BiblePlaceMarker(book, chapter, verse, DateTime.Now) { Note = note });
                    }
                }
            }
        }

        public static void FromStream(Stream stream, string xmlName, bool noteRequired, List<BiblePlaceMarker> markerList,
            Dictionary<string, Dictionary<int, Dictionary<int, BiblePlaceMarker>>> markerDictionary)
        {
            using (XmlReader reader = XmlReader.Create(stream, new XmlReaderSettings { IgnoreWhitespace = true }))
            {
                string book = string.Empty;
                int chapter = -1;
                int verse = -1;
                string note = string.Empty;
                bool invalidEntry = false;
                // Parse the file and get each of the nodes.
                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            if (reader.Name.ToLower().Equals(xmlName) && reader.HasAttributes)
                            {
                                bool isEmptyElement = reader.IsEmptyElement;
                                reader.MoveToFirstAttribute();
                                do
                                {
                                    switch (reader.Name.ToLower())
                                    {
                                        case "book":
                                            book = reader.Value;
                                            break;
                                        case "chapter":
                                            invalidEntry = !int.TryParse(reader.Value, out chapter);
                                            chapter--;//correction from a 1 based system to zero based system
                                            break;
                                        case "verse":
                                            invalidEntry = !int.TryParse(reader.Value, out verse);
                                            verse--;//correction from a 1 based system to zero based system
                                            break;
                                        case "note":
                                            note = HtmlUnescape(reader.Value);
                                            break;
                                    }
                                }
                                while (reader.MoveToNextAttribute() && !invalidEntry);
                                if (isEmptyElement)
                                {
                                    EvaluateXmlRow(book, chapter, verse, note, invalidEntry, noteRequired, markerList, markerDictionary);
                                    book = string.Empty;
                                    chapter = -1;
                                    verse = -1;
                                    note = string.Empty;
                                    invalidEntry = false;
                                }

                            }
                            break;
                        case XmlNodeType.Text:
                            note = reader.Value;
                            EvaluateXmlRow(book, chapter, verse, note, invalidEntry, noteRequired, markerList, markerDictionary);
                            book = string.Empty;
                            chapter = -1;
                            verse = -1;
                            note = string.Empty;
                            invalidEntry = false;
                            break;
                    }
                }
            }
        }

        public static void FromString(string buffer, string xmlName, bool noteRequired, List<BiblePlaceMarker> markerList,
            Dictionary<string, Dictionary<int, Dictionary<int, BiblePlaceMarker>>> markerDictionary)
        {
            FromByteArray(Encoding.UTF8.GetBytes(buffer), xmlName, noteRequired, markerList, markerDictionary);
        }

        public void FromString(string buffer)
        {
            FromByteArray(Encoding.UTF8.GetBytes(buffer), "highlight", true, null, this.HighlightedVerses);
        }

        private static void FromByteArray(byte[] buffer, string xmlName, bool noteRequired, List<BiblePlaceMarker> markerList,
            Dictionary<string, Dictionary<int, Dictionary<int, BiblePlaceMarker>>> markerDictionary)
        {
            var stream = new MemoryStream(buffer);
            FromStream(stream, xmlName, noteRequired, markerList, markerDictionary);
            stream.Dispose();
        }
    }
}
