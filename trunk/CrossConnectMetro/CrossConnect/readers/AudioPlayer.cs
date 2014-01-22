// <copyright file="AudioPlayer.cs" company="Thomas Dilts">
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

namespace CrossConnect.readers
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Threading.Tasks;
    using System.Xml;

    using Sword;
    using Sword.reader;
    using Sword.versification;

    using Windows.Storage;
    using Windows.Storage.Streams;

    public class AudioPlayer
    {
        #region Static Fields

        private static volatile bool _classInitialized;

        #endregion

        #region Constructors and Destructors

        /// <remarks>
        ///     AudioPlayer instances can share the same process.
        ///     Static fields can be used to share state between AudioPlayer instances
        ///     or to communicate with the Audio Streaming agent.
        /// </remarks>
        public AudioPlayer()
        {
            if (!_classInitialized)
            {
                _classInitialized = true;

                // Subscribe to the managed exception handler
                //Deployment.Current.Dispatcher.BeginInvoke(delegate
                //{
                //    Application.Current.UnhandledException += this.AudioPlayer_UnhandledException;
                //});
            }
        }

        #endregion

        #region Public Methods and Operators

        public static string GetTitle(MediaInfo info)
        {
            int bookNum;
            int relChapterNum;
            string source;
            GetBookAndChapterFromAbsoluteChapter(
                info.Chapter, string.Empty, string.Empty, out bookNum, out relChapterNum, out source);
            var canon = CanonManager.GetCanon("KJV");
            CanonBookDef book = null;
            //Chapters 
            if (bookNum >= canon.OldTestBooks.Count())
            {
                book = canon.NewTestBooks[bookNum - canon.OldTestBooks.Count()];
            }
            else
            {
                book = canon.OldTestBooks[bookNum];
            }
            var bookNames = new BibleNames(info.Language);
            return bookNames.GetFullName(book.ShortName1,book.FullName) + " : " + (relChapterNum + 1).ToString(CultureInfo.InvariantCulture);
        }

        public static async Task<List<MediaInfo>> ReadMediaSourcesFile(StorageFile downloadedFile)
        {
            string message;
            // for debug
            //byte[] buffer = new byte[e.Result.Length];
            //e.Result.Read(buffer, 0, (int)e.Result.Length);
            //System.Diagnostics.Debug.WriteLine("RawFile: " + System.Text.UTF8Encoding.UTF8.GetString(buffer, 0, (int)e.Result.Length));
            message = string.Empty;
            var mediaList = new List<MediaInfo>();
            string biblePattern = string.Empty;
            IRandomAccessStream file = await downloadedFile.OpenAsync(FileAccessMode.Read);
            Stream stream = file.AsStreamForRead();
            using (XmlReader reader = XmlReader.Create(stream, new XmlReaderSettings { CheckCharacters = false }))
            {
                string name = string.Empty;
                MediaInfo foundMedia = null;

                // Parse the file and get each of the nodes.
                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            name = string.Empty;
                            if (reader.Name.ToLower().Equals("source") && reader.HasAttributes)
                            {
                                foundMedia = new MediaInfo();
                                name = string.Empty;
                                reader.MoveToFirstAttribute();
                                do
                                {
                                    switch (reader.Name.ToLower())
                                    {
                                        case "onlynt":
                                            foundMedia.IsNtOnly =
                                                bool.TrueString.ToUpper().Equals(reader.Value.ToUpper());
                                            break;
                                        case "abschapter":
                                            int.TryParse(reader.Value, out foundMedia.Chapter);
                                            break;
                                        case "language":
                                            foundMedia.Language = reader.Value;
                                            break;
                                        case "code":
                                            foundMedia.Code = reader.Value;
                                            break;
                                        case "src":
                                            foundMedia.Src = reader.Value;
                                            break;
                                        case "icon":
                                            foundMedia.Icon = reader.Value;
                                            break;
                                        case "iconlink":
                                            foundMedia.IconLink = reader.Value;
                                            break;
                                        case "viewer":
                                            break;
                                        case "player":
                                            if (reader.Value.Equals("explorer"))
                                            {
                                            }

                                            break;
                                    }
                                }
                                while (reader.MoveToNextAttribute());
                            }
                            else if (reader.Name.ToLower().Equals("message"))
                            {
                                name = string.Empty;
                            }

                            break;
                        case XmlNodeType.EndElement:
                            if (reader.Name.ToLower().Equals("source") && foundMedia != null
                                && !string.IsNullOrEmpty(foundMedia.Src) && !string.IsNullOrEmpty(name))
                            {
                                foundMedia.Name = name;
                                mediaList.Add(foundMedia);
                                name = string.Empty;
                            }
                            else if (reader.Name.ToLower().Equals("message"))
                            {
                                message = name;
                                name = string.Empty;
                            }
                            else if (reader.Name.ToLower().Equals("biblepattern"))
                            {
                                biblePattern = name;
                                name = string.Empty;
                            }

                            break;
                        case XmlNodeType.Text:
                            name += reader.Value;
                            break;
                    }
                }
            }

            foreach (MediaInfo media in mediaList)
            {
                media.Pattern = biblePattern;
            }

            return mediaList;
        }

        public static void SetRelativeChapter(int relativePostion, MediaInfo currentInfo)
        {
            if (currentInfo != null)
            {
                // update the _currentInfo
                currentInfo.Chapter = AddChapter(currentInfo, relativePostion);
                int bookNum;
                int relChapterNum;
                string source;
                GetBookAndChapterFromAbsoluteChapter(
                    currentInfo.Chapter,
                    currentInfo.Pattern,
                    currentInfo.Code,
                    out bookNum,
                    out relChapterNum,
                    out source);
                currentInfo.Src = source;
                Debug.WriteLine("starting new track = " + currentInfo.Src);
            }
        }

        #endregion

        #region Methods

        private static int AddChapter(MediaInfo info, int valToAdd)
        {
            int adjustedChapter = info.Chapter + valToAdd;
            var canonKjv = CanonManager.GetCanon("KJV");
            var lastBook = canonKjv.NewTestBooks[canonKjv.NewTestBooks.Count() - 1];
            var lastOtBook = canonKjv.OldTestBooks[canonKjv.OldTestBooks.Count() - 1];
            var chaptersInBible = lastBook.NumberOfChapters + lastBook.VersesInChapterStartIndex;
            var chaptersInOldTestement = lastOtBook.NumberOfChapters + lastOtBook.VersesInChapterStartIndex;
            if (adjustedChapter >= chaptersInBible)
            {
                adjustedChapter = info.IsNtOnly ? chaptersInOldTestement : 0;
            }
            else if (adjustedChapter < 0 || (info.IsNtOnly && adjustedChapter < chaptersInOldTestement))
            {
                adjustedChapter = chaptersInBible - 1;
            }

            return adjustedChapter;
        }

        private static void GetBookAndChapterFromAbsoluteChapter(
            int absolutChapter, string pattern, string code, out int bookNum, out int relChapter, out string source)
        {
            const int BooksInBible = 66;
            source = string.Empty;
            var booksStartAbsoluteChapter = new[]
                                                {
                                                    0, 50, 90, 117, 153, 187, 211, 232, 236, 267, 291, 313, 338, 367, 403,
                                                    413, 426, 436, 478, 628, 659, 671, 679, 745, 797, 802, 850, 862, 876,
                                                    879, 888, 889, 893, 900, 903, 906, 909, 911, 925, 929, 957, 973, 997,
                                                    1018, 1046, 1062, 1078, 1091, 1097, 1103, 1107, 1111, 1116, 1119, 1125
                                                    , 1129, 1132, 1133, 1146, 1151, 1156, 1159, 1164, 1165, 1166, 1167,
                                                    1189
                                                };
            bookNum = 1;
            relChapter = absolutChapter + 1;
            for (int i = 1; i <= BooksInBible; i++)
            {
                if (absolutChapter < booksStartAbsoluteChapter[i])
                {
                    bookNum = i;
                    relChapter = absolutChapter - booksStartAbsoluteChapter[i - 1] + 1;
                    break;
                }
            }

            if (!string.IsNullOrEmpty(pattern) && !string.IsNullOrEmpty(code))
            {
                // http://www.cross-connect.se/bibles/talking/{key}/Bible_{key}_{booknum2d}_{chapternum3d}.mp3
                source =
                    pattern.Replace("{key}", code)
                           .Replace("{booknum2d}", bookNum.ToString("D2"))
                           .Replace("{chapternum3d}", relChapter.ToString("D3"));
            }

            // convert all to zero based
            bookNum--;
            relChapter--;
        }

        private static string ReadableToXmlClean(string toBeCleaned)
        {
            var specialChar = new[] { "&", "\"", "'", "<", ">" };
            var encoded = new[] { "&amp;", "&quot;", "&apos;", "&lt;", "&gt;" };
            for (int i = 0; i < specialChar.Count(); i++)
            {
                toBeCleaned = toBeCleaned.Replace(specialChar[i], encoded[i]);
            }

            return toBeCleaned;
        }

        private static string WriteMediaInfoToXml(MediaInfo info, string sourceLink)
        {
            const string FileContentCoded =
                @"<?xml version=""1.0"" encoding=""UTF-8""?><cross.connect.talking.bible version=""1.0"" book=""1""><message>{0}</message><source language=""{1}"" abschapter=""{2}"" onlynt=""{3}"" code=""{4}"" src=""{5}"" icon=""{6}"" iconlink=""{7}"">{8}</source><biblepattern>{9}</biblepattern></cross.connect.talking.bible>";
            return string.Format(
                FileContentCoded,
                ReadableToXmlClean(sourceLink),
                info.Language,
                info.Chapter,
                info.IsNtOnly.ToString(),
                info.Code,
                info.Src,
                info.Icon,
                info.IconLink,
                info.Name,
                info.Pattern);
        }

        #endregion

        [DataContract]
        public class MediaInfo
        {
            #region Fields

            [DataMember]
            public int Chapter;

            [DataMember]
            public string Code = string.Empty;

            [DataMember]
            public string Icon = string.Empty;

            [DataMember]
            public string IconLink = string.Empty;

            [DataMember]
            public bool IsNtOnly;

            [DataMember]
            public string Language = string.Empty;

            [DataMember]
            public string Name = string.Empty;

            [DataMember]
            public string Pattern = string.Empty;

            [DataMember]
            public string Src = string.Empty;

            #endregion
        }
    }
}