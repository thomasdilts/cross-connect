﻿#region Header

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

#endregion Header

namespace AudioPlaybackAgent1
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Windows;
    using System.Xml;


    using Microsoft.Devices;
    using Microsoft.Phone.BackgroundAudio;

    using Sword;
    using Sword.reader;
    using Sword.versification;

    public class AudioPlayer //: AudioPlayerAgent
    {
        #region Fields

        private static volatile bool _classInitialized;
        private static volatile MediaInfo _currentInfo;

        #endregion Fields

        #region Constructors

        /// <remarks>
        /// AudioPlayer instances can share the same process. 
        /// Static fields can be used to share state between AudioPlayer instances
        /// or to communicate with the Audio Streaming agent.
        /// </remarks>
        public AudioPlayer()
        {
            if (!_classInitialized)
            {
                _classInitialized = true;

                // Subscribe to the managed exception handler
                Deployment.Current.Dispatcher.BeginInvoke(delegate
                {
                    Application.Current.UnhandledException += this.AudioPlayer_UnhandledException;
                });
            }
        }

        #endregion Constructors

        #region Methods

        public static MediaInfo ReadMediaInfoFromXml(string xml, out string soundLink)
        {
            MediaInfo info = null;
            soundLink = string.Empty;
            try
            {
                var bytes = System.Text.Encoding.UTF8.GetBytes(xml);
                var ms = new MemoryStream(bytes);
                ms.Seek(0, SeekOrigin.Begin);
                var list = ReadMediaSourcesFile(ms, out soundLink);
                if (list.Count == 1)
                {
                    info = list[0];
                }
            }
            catch (Exception ee)
            {
                Debug.WriteLine("crashed ReadMediaInfoFromFile ;" + ee);
            }

            return info;
        }

        public static List<MediaInfo> ReadMediaSourcesFile(Stream file, out string message)
        {
            // for debug
            // byte[] buffer=new byte[e.Result.Length];
            // e.Result.Read(buffer, 0, (int)e.Result.Length);
            // System.Diagnostics.Debug.WriteLine("RawFile: " + System.Text.UTF8Encoding.UTF8.GetString(buffer, 0, (int)e.Result.Length));
            message = string.Empty;
            var mediaList = new List<MediaInfo>();
            string biblePattern = string.Empty;
            using (XmlReader reader = XmlReader.Create(file))
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
                                        case "book":
                                            foundMedia.Book = reader.Value;
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
                                && ((!string.IsNullOrEmpty(foundMedia.Src) && !string.IsNullOrEmpty(name)) || !string.IsNullOrEmpty(foundMedia.VoiceName)))
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

            foreach (var media in mediaList)
            {
                media.Pattern = biblePattern;
            }

            return mediaList;
        }

        public static void StartNewTrack(MediaInfo info)
        {
            // I must get some more information to the AudioPlayer thread.
            // I have to do in in the Album and By fields.
            BackgroundAudioPlayer.Instance.Track = new AudioTrack(
                new Uri(info.Src),
                GetTitle(info),
                string.Empty,
                string.Empty,
                string.IsNullOrEmpty(info.Icon) ? null : new Uri(info.Icon),
                WriteMediaInfoToXml(info, string.Empty),
                EnabledPlayerControls.All);
        }


        /// <summary>
        /// Called whenever there is an error with playback, such as an AudioTrack not downloading correctly
        /// </summary>
        /// <param name="player">The BackgroundAudioPlayer</param>
        /// <param name="track">The track that had the error</param>
        /// <param name="error">The error that occured</param>
        /// <param name="isFatal">If true, playback cannot continue and playback of the track will stop</param>
        /// <remarks>
        /// This method is not guaranteed to be called in all cases. For example, if the background agent 
        /// itself has an unhandled exception, it won't get called back to handle its own errors.
        /// </remarks>
        protected void OnError(BackgroundAudioPlayer player, AudioTrack track, Exception error, bool isFatal)
        {
            //if (isFatal)
            //{
            //    this.Abort();
            //}
            //else
            //{
            //    this.NotifyComplete();
            //}
        }

        /// <summary>
        /// Called when the playstate changes, except for the Error state (see OnError)
        /// </summary>
        /// <param name="player">The BackgroundAudioPlayer</param>
        /// <param name="track">The track playing at the time the playstate changed</param>
        /// <param name="playState">The new playstate of the player</param>
        /// <remarks>
        /// Play State changes cannot be cancelled. They are raised even if the application
        /// caused the state change itself, assuming the application has opted-in to the callback.
        /// 
        /// Notable playstate events: 
        /// (a) TrackEnded: invoked when the player has no current track. The agent can set the next track.
        /// (b) TrackReady: an audio track has been set and it is now ready for playack.
        /// 
        /// Call NotifyComplete() only once, after the agent request has been completed, including async callbacks.
        /// </remarks>
        protected void OnPlayStateChanged(BackgroundAudioPlayer player, AudioTrack track, PlayState playState)
        {
            switch (playState)
            {
                case PlayState.TrackEnded:
                    player.Track = GetRelativeTrack(1);
                    break;
                case PlayState.TrackReady:
                    if (!string.IsNullOrEmpty(track.Tag))
                    {
                        // recreate from the tag
                        string msg;
                        _currentInfo = ReadMediaInfoFromXml(track.Tag, out msg);

                        player.Play();
                        try
                        {
                            var mediaHistoryItem = new MediaHistoryItem();

                            var sri = Application.GetResourceStream(new Uri("Images/square173.png", UriKind.Relative));
                            mediaHistoryItem.ImageStream = sri.Stream;

                            mediaHistoryItem.Source = track.Source.ToString();
                            mediaHistoryItem.Title = track.Title;
                            mediaHistoryItem.PlayerContext.Add(track.Title, track.Title);
                            MediaHistory.Instance.NowPlaying = mediaHistoryItem;
                        }
                        catch (Exception ee)
                        {
                            Debug.WriteLine("crash  ; " + ee);
                        }
                    }

                    break;
                case PlayState.Shutdown:
                    break;
                case PlayState.Unknown:
                    break;
                case PlayState.Stopped:
                    break;
                case PlayState.Paused:
                    break;
                case PlayState.Playing:
                    break;
                case PlayState.BufferingStarted:
                    break;
                case PlayState.BufferingStopped:
                    break;
                case PlayState.Rewinding:
                    break;
                case PlayState.FastForwarding:
                    break;
            }

            //this.NotifyComplete();
        }

        /// <summary>
        /// Called when the user requests an action using application/system provided UI
        /// </summary>
        /// <param name="player">The BackgroundAudioPlayer</param>
        /// <param name="track">The track playing at the time of the user action</param>
        /// <param name="action">The action the user has requested</param>
        /// <param name="param">The data associated with the requested action.
        /// In the current version this parameter is only for use with the Seek action,
        /// to indicate the requested position of an audio track</param>
        /// <remarks>
        /// User actions do not automatically make any changes in system state; the agent is responsible
        /// for carrying out the user actions if they are supported.
        /// 
        /// Call NotifyComplete() only once, after the agent request has been completed, including async callbacks.
        /// </remarks>
        protected void OnUserAction(BackgroundAudioPlayer player, AudioTrack track, UserAction action, object param)
        {
            switch (action)
            {
                case UserAction.Play:
                    if (player.PlayerState != PlayState.Playing)
                    {
                        player.Play();
                    }

                    break;
                case UserAction.Stop:
                    player.Stop();
                    break;
                case UserAction.Pause:
                    player.Pause();
                    break;
                case UserAction.FastForward:
                    player.FastForward();
                    break;
                case UserAction.Rewind:
                    player.Rewind();
                    break;
                case UserAction.Seek:
                    player.Position = (TimeSpan)param;
                    break;
                case UserAction.SkipNext:
                    player.Track = GetRelativeTrack(1);
                    break;
                case UserAction.SkipPrevious:
                    player.Track = GetRelativeTrack(-1);

                    break;
            }

            //this.NotifyComplete();
        }

        public static CanonBookDef AddChapter(MediaInfo info, int valToAdd)
        {
            var canonKjv = CanonManager.GetCanon("KJV");
            var book = canonKjv.BookByShortName[info.Book];
            int adjustedChapter = book.VersesInChapterStartIndex + info.Chapter + valToAdd;
            var lastBook = canonKjv.NewTestBooks[canonKjv.NewTestBooks.Count() - 1];
            var lastOtBook = canonKjv.OldTestBooks[canonKjv.OldTestBooks.Count() - 1];
            var chaptersInOldTestement = lastOtBook.NumberOfChapters + lastOtBook.VersesInChapterStartIndex;
            var chaptersInBible = lastBook.NumberOfChapters + lastBook.VersesInChapterStartIndex;
            if (adjustedChapter >= chaptersInBible)
            {
                adjustedChapter = info.IsNtOnly ? chaptersInOldTestement : 0;
            }
            else if (adjustedChapter < 0 || (info.IsNtOnly && adjustedChapter < chaptersInOldTestement))
            {
                adjustedChapter = chaptersInBible - 1;
            }

            var adjustedBook = canonKjv.GetBookFromAbsoluteChapter(adjustedChapter);
            info.Book = adjustedBook.ShortName1;
            info.Chapter = adjustedChapter - adjustedBook.VersesInChapterStartIndex;

            return adjustedBook;
        }

        //private static void GetBookAndChapterFromAbsoluteChapter(int absolutChapter, string pattern, string code, out string bookNameshort, out string bookFullName, out int relChapter, out string source)
        //{
        //    var canon = CanonManager.GetCanon("KJV");
        //    source = string.Empty;
        //    var book = canon.GetBookFromAbsoluteChapter(absolutChapter);
        //    relChapter = absolutChapter - book.VersesInChapterStartIndex;
        //    bookNameshort = book.ShortName1;
        //    bookFullName = book.FullName;
        //    if (!string.IsNullOrEmpty(pattern) && !string.IsNullOrEmpty(code))
        //    {
        //        // http://www.cross-connect.se/bibles/talking/{key}/Bible_{key}_{booknum2d}_{chapternum3d}.mp3
        //        source =
        //            pattern.Replace("{key}", code)
        //                   .Replace("{booknum2d}", (book.BookNum + 1).ToString("D2"))
        //                   .Replace("{chapternum3d}", (relChapter + 1).ToString("D3"));
        //    }
        //}

        /// <summary>
        /// Implements the logic to get the next AudioTrack instance.
        /// In a playlist, the source can be from a file, a web request, etc.
        /// </summary>
        /// <remarks>
        /// The AudioTrack URI determines the source, which can be:
        /// (a) Isolated-storage file (Relative URI, represents path in the isolated storage)
        /// (b) HTTP URL (absolute URI)
        /// (c) MediaStreamSource (null)
        /// </remarks>
        /// <returns>an instance of AudioTrack, or null if the playback is completed</returns>
        private static AudioTrack GetRelativeTrack(int relativePostion)
        {
            AudioTrack track = null;
            if (_currentInfo == null && BackgroundAudioPlayer.Instance.Track != null && !string.IsNullOrEmpty(BackgroundAudioPlayer.Instance.Track.Tag))
            {
                string msg;
                _currentInfo = ReadMediaInfoFromXml(BackgroundAudioPlayer.Instance.Track.Tag, out msg);
            }

            if (_currentInfo != null)
            {
                // update the _currentInfo
                var book = AddChapter(_currentInfo, relativePostion);
                //string bookName;
                //string bookFullName;
                //int relChapterNum;
                //string source;
                //GetBookAndChapterFromAbsoluteChapter(
                //    _currentInfo.Chapter, _currentInfo.Pattern, _currentInfo.Code, out bookName, out bookFullName, out relChapterNum, out source);
                //_currentInfo.Src = source;

                if (!string.IsNullOrEmpty(_currentInfo.Pattern) && !string.IsNullOrEmpty(_currentInfo.Code))
                {
                    // http://www.cross-connect.se/bibles/talking/{key}/Bible_{key}_{booknum2d}_{chapternum3d}.mp3
                    _currentInfo.Src =
                        _currentInfo.Pattern.Replace("{key}", _currentInfo.Code)
                               .Replace("{booknum2d}", (book.BookNum + 1).ToString("D2"))
                               .Replace("{chapternum3d}", (_currentInfo.Chapter + 1).ToString("D3"));
                }





                Debug.WriteLine("starting new track = " + _currentInfo.Src);

                // Create the track
                track = new AudioTrack(
                    new Uri(_currentInfo.Src),
                    GetTitle(_currentInfo),
                    string.Empty,
                    string.Empty,
                    string.IsNullOrEmpty(_currentInfo.Icon) ? null : new Uri(_currentInfo.Icon),
                    WriteMediaInfoToXml(_currentInfo, string.Empty),
                    EnabledPlayerControls.All);
            }

            return track;
        }
        //public static void SetRelativeChapter(int relativePostion, MediaInfo currentInfo, IBrowserTextSource bibleSource)
        //{
        //    if (currentInfo != null)
        //    {
        //        // update the _currentInfo
        //        currentInfo.Chapter = AddChapter(currentInfo, relativePostion);
        //        string bookName;
        //        string bookFullName;
        //        int relChapterNum;
        //        string source;
        //        GetBookAndChapterFromAbsoluteChapter(
        //            currentInfo.Chapter,
        //            currentInfo.Pattern,
        //            currentInfo.Code,
        //            out bookName,
        //            out bookFullName,
        //            out relChapterNum,
        //            out source);
        //        currentInfo.Src = source;
        //        Debug.WriteLine("starting new track = " + currentInfo.Src);
        //        bibleSource.MoveChapterVerse(bookName, relChapterNum, 0, false, bibleSource);
        //    }
        //}
        public static string GetTitle(MediaInfo info)
        {
            //string bookShortName;
            //string bookFullName;
            //int relChapterNum;
            //string source;
            //GetBookAndChapterFromAbsoluteChapter(
            //    info.Chapter, string.Empty, string.Empty, out bookShortName, out bookFullName, out relChapterNum, out source);

            var bookNames = new BibleNames(info.Language, string.Empty);
            return bookNames.GetFullName(info.Book, info.Book) + " : " + (info.Chapter + 1).ToString(CultureInfo.InvariantCulture);
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
            const string FileContentCoded = @"<?xml version=""1.0"" encoding=""UTF-8""?><cross.connect.talking.bible version=""1.0"" book=""1""><message>{0}</message><source language=""{1}"" abschapter=""{2}"" onlynt=""{3}"" code=""{4}"" src=""{5}"" icon=""{6}"" iconlink=""{7}"" voicename=""{8}"" book=""{9}"">{10}</source><biblepattern>{11}</biblepattern></cross.connect.talking.bible>";
            return string.Format(
                FileContentCoded, ReadableToXmlClean(sourceLink), info.Language, info.Chapter, info.IsNtOnly.ToString(CultureInfo.InvariantCulture), info.Code, info.Src, info.Icon, info.IconLink, info.VoiceName, info.Book, info.Name, info.Pattern);
        }

        /// Code to execute on Unhandled Exceptions
        private void AudioPlayer_UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
        {
            if (Debugger.IsAttached)
            {
                // An unhandled exception has occurred; break into the debugger
                Debugger.Break();
            }
        }

        #endregion Methods

        #region Nested Types

        [DataContract]
        public class MediaInfo
        {
            #region Fields

            [DataMember]
            public int Chapter;
            [DataMember]
            public string Book;
            [DataMember]
            public string VoiceName;
            [DataMember]
            public int Verse;
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

            #endregion Fields
        }

        #endregion Nested Types
    }
}