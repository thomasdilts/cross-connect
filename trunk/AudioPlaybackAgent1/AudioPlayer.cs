namespace AudioPlaybackAgent1
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.IO.IsolatedStorage;
    using System.Linq;
    using System.Net;
    using System.Reflection;
    using System.Runtime.Serialization;
    using System.Windows;
    using System.Windows.Media.Imaging;
    using System.Windows.Resources;
    using System.Xml;

    using Microsoft.Devices;
    using Microsoft.Phone.BackgroundAudio;
    using Microsoft.Phone.Logging;

    using Sword;
    using Sword.reader;

    public class AudioPlayer : AudioPlayerAgent
    {
        #region Fields

        private static string _soundLink = string.Empty;

        private static volatile bool _classInitialized;
        private static volatile MediaInfo _currentInfo;
        private static volatile WebClient _nextClient;
        private static volatile MediaInfo _nextInfo;
        private static volatile WebClient _previousClient;
        private static volatile MediaInfo _previousInfo;
        private static volatile bool _waitingForNext;
        private static volatile bool _waitingForPrevious;

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

        public static void GetBookAndChapterFromAbsoluteChapter(int absolutChapter, out int bookNum, out int relChapter)
        {
            const int BooksInBible = 66;
            var booksStartAbsoluteChapter = new int[]
                {
                    0, 50, 90, 117, 153, 187, 211, 232, 236, 267, 291, 313, 338, 367, 403, 413, 426, 436, 478, 628, 659,
                    671, 679, 745, 797, 802, 850, 862, 876, 879, 888, 889, 893, 900, 903, 906, 909, 911, 925, 929, 957,
                    973, 997, 1018, 1046, 1062, 1078, 1091, 1097, 1103, 1107, 1111, 1116, 1119, 1125, 1129, 1132, 1133,
                    1146, 1151, 1156, 1159, 1164, 1165, 1166, 1167, 1189
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

            // convert all to zero based
            bookNum--;
            relChapter--;
        }

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
            catch(Exception ee)
            {
                Debug.WriteLine("crashed ReadMediaInfoFromFile ;" + ee);
            }

            return info;
        }

        public static List<MediaInfo> ReadMediaSourcesFile(Stream file, out string sourceLink)
        {
            // for debug
            // byte[] buffer=new byte[e.Result.Length];
            // e.Result.Read(buffer, 0, (int)e.Result.Length);
            // System.Diagnostics.Debug.WriteLine("RawFile: " + System.Text.UTF8Encoding.UTF8.GetString(buffer, 0, (int)e.Result.Length));
            sourceLink = String.Empty;
            var mediaList = new List<MediaInfo>();
            using (XmlReader reader = XmlReader.Create(file))
            {
                string name = String.Empty;
                MediaInfo foundMedia = null;

                // Parse the file and get each of the nodes.
                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            if (reader.Name.ToLower().Equals("source") && reader.HasAttributes)
                            {
                                foundMedia = new MediaInfo();
                                name = String.Empty;
                                reader.MoveToFirstAttribute();
                                do
                                {
                                    switch (reader.Name.ToLower())
                                    {
                                        case "onlynt":
                                            foundMedia.IsNtOnly =
                                                Boolean.TrueString.ToUpper().Equals(reader.Value.ToUpper());
                                            break;
                                        case "abschapter":
                                            Int32.TryParse(reader.Value, out foundMedia.Chapter);
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
                                name = String.Empty;
                            }

                            break;
                        case XmlNodeType.EndElement:
                            if (reader.Name.ToLower().Equals("source") && foundMedia != null
                                && !String.IsNullOrEmpty(foundMedia.Src) && !String.IsNullOrEmpty(name))
                            {
                                foundMedia.Name = name;
                                mediaList.Add(foundMedia);
                            }
                            else if (reader.Name.ToLower().Equals("message"))
                            {
                                sourceLink = name;
                                name = String.Empty;
                            }

                            break;
                        case XmlNodeType.Text:
                            name += reader.Value;
                            break;
                    }
                }
            }

            return mediaList;
        }

        public static void StartNewTrack(MediaInfo info, string soundLink)
        {
            // I must get some more information to the AudioPlayer thread.
            // I have to do in in the Album and By fields.
            BackgroundAudioPlayer.Instance.Track = new AudioTrack(
                new Uri(info.Src),
                GetTitle(info),
                string.Empty,
                string.Empty,
                string.IsNullOrEmpty(info.Icon) ? null : new Uri(info.Icon),
                WriteMediaInfoToXml(info, soundLink),
                EnabledPlayerControls.All);
        }

        /// <summary>
        /// Called when the agent request is getting cancelled
        /// </summary>
        /// <remarks>
        /// Once the request is Cancelled, the agent gets 5 seconds to finish its work,
        /// by calling NotifyComplete()/Abort().
        /// </remarks>
        protected override void OnCancel()
        {
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
        protected override void OnError(BackgroundAudioPlayer player, AudioTrack track, Exception error, bool isFatal)
        {
            if (isFatal)
            {
                this.Abort();
            }
            else
            {
                this.NotifyComplete();
            }
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
        protected override void OnPlayStateChanged(BackgroundAudioPlayer player, AudioTrack track, PlayState playState)
        {
            switch (playState)
            {
                case PlayState.TrackEnded:
                    player.Track = GetNextTrack();
                    break;
                case PlayState.TrackReady:

                    // recreate from the tag
                    _currentInfo = ReadMediaInfoFromXml(track.Tag, out _soundLink);

                    if (_nextClient != null)
                    {
                        _nextClient.CancelAsync();
                        _nextClient = null;
                    }

                    _waitingForNext = false;
                    StartNextTrackSourceDownload();

                    if (_previousClient != null)
                    {
                        _previousClient.CancelAsync();
                        _previousClient = null;
                    }

                    _waitingForPrevious = false;
                    StartPrevTrackSourceDownload();
                    player.Play();
                    try
                    {
                        MediaHistoryItem mediaHistoryItem = new MediaHistoryItem();

                        StreamResourceInfo sri =
                            Application.GetResourceStream(new Uri("ApplicationIcon.png", UriKind.Relative));
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

                    break;
                case PlayState.Shutdown:
                    // TODO: Handle the shutdown state here (e.g. save state)
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

            this.NotifyComplete();
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
        protected override void OnUserAction(BackgroundAudioPlayer player, AudioTrack track, UserAction action, object param)
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
                    player.Track = GetNextTrack();
                    break;
                case UserAction.SkipPrevious:
                    AudioTrack previousTrack = GetPreviousTrack();
                    if (previousTrack != null)
                    {
                        player.Track = previousTrack;
                    }
                    break;
            }

            this.NotifyComplete();
        }

        private static int AddChapter(int valToAdd)
        {
            const int BooksInBible = 66;
            int adjustedChapter = _currentInfo.Chapter + valToAdd;
            if (adjustedChapter >= BibleZtextReader.ChaptersInBible)
            {
                adjustedChapter = _currentInfo.IsNtOnly ? BibleZtextReader.ChaptersInOt : 0;
            }
            else if (adjustedChapter < 0 || (_currentInfo.IsNtOnly && adjustedChapter < BibleZtextReader.ChaptersInOt))
            {
                adjustedChapter = BibleZtextReader.ChaptersInBible - 1;
            }

            return adjustedChapter;
        }

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
        private static AudioTrack GetNextTrack()
        {
            AudioTrack track = null;
            if (_nextClient == null && _nextInfo != null && _currentInfo != null)
            {
                track = new AudioTrack(
                    new Uri(_nextInfo.Src),
                    GetTitle(_nextInfo),
                    string.Empty,
                    string.Empty,
                    string.IsNullOrEmpty(_nextInfo.Icon) ? null : new Uri(_nextInfo.Icon),
                    WriteMediaInfoToXml(_nextInfo, _soundLink),
                    EnabledPlayerControls.All);

                // Adjust the media infos
                _previousInfo = _currentInfo;
                _currentInfo = _nextInfo;
                StartNextTrackSourceDownload();
            }
            else
            {
                _waitingForNext = _nextClient != null || _nextInfo == null;
            }
            return track;
        }

        private static void GetNextTrackFromInternetCompleted(object sender, OpenReadCompletedEventArgs e)
        {
            try
            {
                string msg;
                var mediaList = AudioPlayer.ReadMediaSourcesFile(e.Result, out msg);
                if (mediaList.Count() == 1)
                {
                    _nextInfo = mediaList[0];
                    if (_waitingForNext)
                    {
                        var track = GetNextTrack();
                        if (track != null)
                        {
                            BackgroundAudioPlayer.Instance.Track = track;
                        }

                        _waitingForNext = false;
                    }
                }
                _nextClient.CancelAsync();
                _nextClient = null;
            }
            catch (Exception exp)
            {
                Debug.WriteLine("GetNextTrackFromInternetCompleted failed;" + exp);
                if (_nextClient != null)
                {
                    _nextClient.CancelAsync();
                }

                _nextClient = null;

                // try again..
                StartNextTrackSourceDownload();
            }
        }

        /// <summary>
        /// Implements the logic to get the previous AudioTrack instance.
        /// </summary>
        /// <remarks>
        /// The AudioTrack URI determines the source, which can be:
        /// (a) Isolated-storage file (Relative URI, represents path in the isolated storage)
        /// (b) HTTP URL (absolute URI)
        /// (c) MediaStreamSource (null)
        /// </remarks>
        /// <returns>an instance of AudioTrack, or null if previous track is not allowed</returns>
        private static AudioTrack GetPreviousTrack()
        {
            AudioTrack track = null;
            if (_previousClient == null && _previousInfo != null && _currentInfo != null)
            {
                track = new AudioTrack(
                    new Uri(_previousInfo.Src),
                    GetTitle(_previousInfo),
                    string.Empty,
                    string.Empty,
                    string.IsNullOrEmpty(_previousInfo.Icon) ? null : new Uri(_previousInfo.Icon),
                    WriteMediaInfoToXml(_previousInfo, _soundLink),
                    EnabledPlayerControls.All);

                // Adjust the media infos
                _nextInfo = _currentInfo;
                _currentInfo = _previousInfo;
                StartPrevTrackSourceDownload();
            }

            return track;
        }

        private static void GetPreviousTrackFromInternetCompleted(object sender, OpenReadCompletedEventArgs e)
        {
            try
            {
                string msg;
                var mediaList = AudioPlayer.ReadMediaSourcesFile(e.Result, out msg);
                if (mediaList.Count() == 1)
                {
                    _previousInfo = mediaList[0];
                    if (_waitingForPrevious)
                    {
                        var track = GetPreviousTrack();
                        if (track != null)
                        {
                            BackgroundAudioPlayer.Instance.Track = track;
                        }

                        _waitingForPrevious = false;
                    }
                }

                _previousClient.CancelAsync();
                _previousClient = null;
            }
            catch (Exception exp)
            {
                Debug.WriteLine("GetPreviousTrackFromInternetCompleted failed;" + exp);
                if (_previousClient != null)
                {
                    _previousClient.CancelAsync();
                }

                _previousClient = null;

                // try again..
                StartNextTrackSourceDownload();
            }
        }

        private static string GetTitle(MediaInfo info)
        {
            int bookNum;
            int relChapterNum;
            GetBookAndChapterFromAbsoluteChapter(
                info.Chapter, out bookNum, out relChapterNum);

            var bookNames = new BibleNames(info.Language);
            return bookNames.GetFullName(bookNum) + " : " + (relChapterNum + 1).ToString(CultureInfo.InvariantCulture);
        }

        private static string GetTrackName(MediaInfo info)
        {
            int bookNum;
            int relChapterNum;
            GetBookAndChapterFromAbsoluteChapter(
                info.Chapter, out bookNum, out relChapterNum);

            var bookNames = new BibleNames(info.Language);

            return bookNames.GetFullName(bookNum);
        }

        private static string ReadableToXmlClean(string toBeCleaned)
        {
            var specialChar = new string[] { "&", "\"", "'", "<", ">" };
            var encoded = new string[] { "&amp;", "&quot;", "&apos;", "&lt;", "&gt;" };
            for (int i = 0; i < specialChar.Count(); i++)
            {
                toBeCleaned = toBeCleaned.Replace(specialChar[i], encoded[i]);
            }

            return toBeCleaned;
        }

        private static void StartNextTrackSourceDownload()
        {
            _nextInfo = null;
            string url = string.Format(
                _soundLink, AddChapter(1), _currentInfo.Language);
            url += "&code=" + _currentInfo.Code;
            try
            {
                var source = new Uri(url);

                _nextClient = new WebClient();
                _nextClient.OpenReadCompleted += GetNextTrackFromInternetCompleted;
                Debug.WriteLine("download start");
                _nextClient.OpenReadAsync(source);
                Debug.WriteLine("DownloadStringAsync returned");
            }
            catch (Exception eee)
            {
                Debug.WriteLine("StartNextTrackSourceDownload failed;" + eee);
            }
        }

        private static void StartPrevTrackSourceDownload()
        {
            _previousInfo = null;
            string url = string.Format(
                _soundLink, AddChapter(-1), _currentInfo.Language);
            url += "&code=" + _currentInfo.Code;
            try
            {
                var source = new Uri(url);

                _previousClient = new WebClient();
                _previousClient.OpenReadCompleted += GetPreviousTrackFromInternetCompleted;
                Debug.WriteLine("download start");
                _previousClient.OpenReadAsync(source);
                Debug.WriteLine("DownloadStringAsync returned");
            }
            catch (Exception eee)
            {
                Debug.WriteLine("StartPrevTrackSourceDownload failed;" + eee);
            }
        }

        private static string WriteMediaInfoToXml(MediaInfo info, string sourceLink)
        {
            const string FileContentCoded = @"<?xml version=""1.0"" encoding=""UTF-8""?><cross.connect.talking.bible version=""1.0"" book=""1""><message>{0}</message><source language=""{1}"" abschapter=""{2}"" onlynt=""{3}"" code=""{4}"" src=""{5}"" icon=""{6}"">{7}</source></cross.connect.talking.bible>";
            return string.Format(
                FileContentCoded, ReadableToXmlClean(sourceLink), info.Language, info.Chapter, info.IsNtOnly.ToString(CultureInfo.InvariantCulture), info.Code, info.Src, info.Icon, info.Name);
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

        /// <summary>
        /// The media info.
        /// </summary>
        [DataContract]
        public class MediaInfo
        {
            #region Fields

            /// <summary>
            /// The src.
            /// </summary>
            [DataMember]
            public int Chapter;

            /// <summary>
            /// The src.
            /// </summary>
            [DataMember]
            public string Code = String.Empty;

            /// <summary>
            /// The icon.
            /// </summary>
            [DataMember]
            public string Icon = String.Empty;

            /// <summary>
            /// The src.
            /// </summary>
            [DataMember]
            public bool IsNtOnly;

            /// <summary>
            /// The src.
            /// </summary>
            [DataMember]
            public string Language = String.Empty;

            /// <summary>
            /// The src.
            /// </summary>
            [DataMember]
            public string Name = String.Empty;

            /// <summary>
            /// The src.
            /// </summary>
            [DataMember]
            public string Src = String.Empty;

            #endregion Fields
        }

        #endregion Nested Types
    }
}