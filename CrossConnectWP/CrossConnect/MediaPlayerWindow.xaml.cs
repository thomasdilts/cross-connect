#region Header

// <copyright file="MediaPlayerWindow.xaml.cs" company="Thomas Dilts">
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

namespace CrossConnect
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.IO.IsolatedStorage;
    using System.Linq;
    using System.Net;

    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using System.Windows.Threading;

    using Microsoft.Phone.BackgroundAudio;
    using Microsoft.Phone.Controls;
    using Microsoft.Phone.Tasks;

    using Sword.reader;

    using readers;
    using AudioPlaybackAgent1;
    using Windows.Phone.Speech.Synthesis;

    public partial class MediaPlayerWindow : ITiledWindow
    {
        #region Fields

        private WebClient _client;
        private bool _isLoaded;
        private SerializableWindowState _state = new SerializableWindowState();
        private DispatcherTimer _updatePositionTimer;

        #endregion Fields

        #region Constructors

        public MediaPlayerWindow()
        {
            InitializeComponent();
            BackgroundAudioPlayer.Instance.PlayStateChanged += InstancePlayStateChanged;
        }

        #endregion Constructors

        #region Events

        public event EventHandler HitButtonBigger;

        public event EventHandler HitButtonClose;

        public event EventHandler HitButtonSmaller;

        #endregion Events

        #region Properties

        public bool ForceReload
        {
            get; set;
        }

        public SerializableWindowState State
        {
            get
            {
                return _state;
            }

            set
            {
                _state = value;
            }
        }

        #endregion Properties

        #region Methods

        public void CalculateTitleTextWidth()
        {
            try
            {
                int numButtonsShowing = 0;
                if (butLarger.Visibility == Visibility.Visible)
                {
                    numButtonsShowing++;
                }

                if (butSmaller.Visibility == Visibility.Visible)
                {
                    numButtonsShowing++;
                }

                if (butClose.Visibility == Visibility.Visible)
                {
                    numButtonsShowing++;
                }

                var parent = (MainPageSplit)((Grid)((Grid)((Grid)Parent).Parent).Parent).Parent;
                if (parent.Orientation == PageOrientation.Landscape
                    || parent.Orientation == PageOrientation.LandscapeLeft
                    || parent.Orientation == PageOrientation.LandscapeRight)
                {
                    title.Width = Application.Current.Host.Content.ActualHeight - (butClose.Width * numButtonsShowing)
                                  - 15 - (butClose.Width * 2);
                    title.MaxWidth = title.Width;
                }
                else
                {
                    title.Width = Application.Current.Host.Content.ActualWidth - (butClose.Width * numButtonsShowing)
                                  - 15;
                    title.MaxWidth = title.Width;
                }
            }
            catch (Exception eee)
            {
                title.Width = 100;
                title.MaxWidth = 100;
                Debug.WriteLine(eee);
            }
        }
        private static SpeechSynthesizer _synth = null;
        private SpeechSynthesizer GetSynthesizer(string voiceName)
        {
            var currentVoice = _synth!=null?_synth.GetVoice():null;
            if (_synth == null || currentVoice != null && !currentVoice.DisplayName.Equals(voiceName))
            {
                _synth = new SpeechSynthesizer();
                VoiceInformation uniqueVoice = InstalledVoices.All.FirstOrDefault(v => v.DisplayName.Equals(voiceName));
                _synth.SetVoice(uniqueVoice);
            }

            return _synth;
        }
        public void RestartToThisMedia(AudioPlayer.MediaInfo Info)
        {

            if (Info == null || (string.IsNullOrEmpty(Info.VoiceName) && string.IsNullOrEmpty(Info.Src)))
            {
                return;
            }
            if (string.IsNullOrEmpty(Info.VoiceName))
            {
                AudioPlayer.StartNewTrack(Info);
                title.Text = AudioPlayer.GetTitle(Info);
                SetButtonVisibility(false);
                return;
            }
            InfinateLoopTextToSpeech(Info, GetSynthesizer(Info.VoiceName));
        }
        private static bool _IsCancel = false;
        private static bool _IsInLoop = false;
        public async void InfinateLoopTextToSpeech(AudioPlayer.MediaInfo Info, SpeechSynthesizer synth)
        {
            if (_IsInLoop)
            {
                SetButtonVisibilityTTS(true);
                return;
            }
            _IsInLoop = true;
            while(true)
            {
                var chapterdata = string.Empty;
                while (string.IsNullOrEmpty(chapterdata))
                {
                    chapterdata = await this._state.Source.GetTTCtext(App.DisplaySettings.SyncMediaVerses);
                    if (!string.IsNullOrEmpty(chapterdata))
                    {
                        break;
                    }
                    else
                    {
                        this._state.Source.MoveNext(App.DisplaySettings.SyncMediaVerses);
                    }
                }


                string bookShortName;
                int relChaptNum;
                int verseNum;
                string fullName;
                string title2;
                this._state.Source.GetInfo(out bookShortName,
                    out relChaptNum,
                    out verseNum,
                    out fullName,
                    out title2);
                if (this.title != null)
                {
                    this.title.Text = title2 + " - "
                                + (string.IsNullOrEmpty(this._state.BibleDescription)
                                        ? this._state.BibleToLoad
                                        : this._state.BibleDescription);
                }
                App.SynchronizeAllWindows(bookShortName, relChaptNum, verseNum, -1, this._state.Source);
                SetButtonVisibilityTTS(true);
                try
                {
                    await synth.SpeakTextAsync(chapterdata);
                }
                catch(Exception e)
                {

                }
                if(_IsCancel)
                {
                    _IsCancel = false;
                    break;
                }
                this._state.Source.MoveNext(App.DisplaySettings.SyncMediaVerses);
            }
            _IsInLoop = false;
        }


        public void ShowSizeButtons(bool isShow = true)
        {
            if (!isShow)
            {
                SetButtonVisibility(butLarger, false, string.Empty, string.Empty);
                SetButtonVisibility(butSmaller, false, string.Empty, string.Empty);
                SetButtonVisibility(butClose, false, string.Empty, string.Empty);
            }
            else
            {
                string colorDir = App.Themes.IsButtonColorDark ? "light" : "dark";

                SetButtonVisibility(
                    butSmaller,
                    _state.NumRowsIown > 1,
                    "/Images/" + colorDir + "/appbar.minus.rest.png",
                    "/Images/" + colorDir + "/appbar.minus.rest.pressed.png");
                SetButtonVisibility(
                    butLarger,
                    true,
                    "/Images/" + colorDir + "/appbar.feature.search.rest.png",
                    "/Images/" + colorDir + "/appbar.feature.search.rest.pressed.png");
                SetButtonVisibility(
                    butClose,
                    true,
                    "/Images/" + colorDir + "/appbar.cancel.rest.png",
                    "/Images/" + colorDir + "/appbar.cancel.rest.pressed.png");
            }

            CalculateTitleTextWidth();
        }

        public void SynchronizeWindow(string shortBookName, int chapterNum, int verseNum, IBrowserTextSource source)
        {
        }

        public void SetMediaInfo(SerializableWindowState theState, AudioPlayer.MediaInfo info)
        {
            this._state = theState;
            theState.VoiceName = info.VoiceName;
            theState.Src = info.Src;
            theState.Pattern = info.Pattern;
            theState.IsNtOnly = info.IsNtOnly;
            theState.code = info.Code;
            this.RestartToThisMedia(info);
        }

        public void UpdateBrowser(bool isOrientationChangeOnly)
        {
            border1.BorderBrush = new SolidColorBrush(App.Themes.BorderColor);
            WebBrowserBorder.BorderBrush = border1.BorderBrush;
            grid1.Background = new SolidColorBrush(App.Themes.TitleBackColor);
            title.Foreground = new SolidColorBrush(App.Themes.TitleFontColor);
            if (App.Themes.IsMainBackImage && !string.IsNullOrEmpty(App.Themes.MainBackImage))
            {
                // read from isolated storage.
                using (IsolatedStorageFile isolatedStorageRoot = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    if (isolatedStorageRoot.FileExists(App.WebDirIsolated + "/images/" + App.Themes.MainBackImage))
                    {
                        try
                        {
                            using (
                                IsolatedStorageFileStream stream =
                                    isolatedStorageRoot.OpenFile(
                                        App.WebDirIsolated + "/images/" + App.Themes.MainBackImage, FileMode.Open))
                            {
                                var buffer = new byte[10000];
                                int len;
                                var ms = new MemoryStream();
                                while ((len = stream.Read(buffer, 0, buffer.GetUpperBound(0))) > 0)
                                {
                                    ms.Write(buffer, 0, len);
                                }

                                stream.Close();
                                ms.Position = 0;
                                var bitImage = new BitmapImage();
                                bitImage.SetSource(ms);
                                var imageBrush = new ImageBrush { ImageSource = bitImage, };
                                AllContent.Background = imageBrush;
                            }
                        }
                        catch (Exception ee)
                        {
                            Debug.WriteLine(ee);
                            AllContent.Background = new SolidColorBrush(App.Themes.MainBackColor);
                        }
                    }
                }
            }
            else
            {
                AllContent.Background = new SolidColorBrush(App.Themes.MainBackColor);
            }

            ProgressPosition.Foreground = new SolidColorBrush(App.Themes.AccentColor);
            WaitingForDownload.Foreground = new SolidColorBrush(App.Themes.AccentColor);
            txtDuration.Foreground = new SolidColorBrush(App.Themes.MainFontColor);
            txtPosition.Foreground = new SolidColorBrush(App.Themes.MainFontColor);
            var parent = (MainPageSplit)((Grid)((Grid)((Grid)Parent).Parent).Parent).Parent;
            if (parent.Orientation == PageOrientation.Landscape || parent.Orientation == PageOrientation.LandscapeLeft
                || parent.Orientation == PageOrientation.LandscapeRight)
            {
                GridProgressBars.Width = Application.Current.Host.Content.ActualHeight - (txtPosition.Width * 2) - 15 - 70;
            }
            else
            {
                GridProgressBars.Width = Application.Current.Host.Content.ActualWidth - (txtPosition.Width * 2) - 15;
            }
        }

        private static ImageSource GetImage(string path)
        {
            var uri = new Uri(path, UriKind.Relative);
            return new BitmapImage(uri);
        }

        private static void SetButtonVisibility(ImageButton but, bool isVisible, string image, string pressedImage)
        {
            if (isVisible)
            {
                but.Visibility = Visibility.Visible;
                but.Image = GetImage(image);
                but.PressedImage = GetImage(pressedImage);
            }
            else
            {
                but.Visibility = Visibility.Collapsed;
                but.Image = null;
                but.PressedImage = null;
            }
        }

        private void ButCloseClick(object sender, RoutedEventArgs e)
        {
            SetButtonVisibility(false);
            if (string.IsNullOrEmpty(_state.VoiceName))
            {
                BackgroundAudioPlayer.Instance.Close();

            }
            else if (_synth!=null)
            {
                _IsCancel = true;
                _synth.CancelAll();
            }
            if (HitButtonClose != null)
            {
                HitButtonClose(this, e);
            }
        }

        private void ButLargerClick(object sender, RoutedEventArgs e)
        {
            _state.NumRowsIown++;
            ShowSizeButtons();
            if (HitButtonBigger != null)
            {
                HitButtonBigger(this, e);
            }
        }

        private async void ButNextClick(object sender, RoutedEventArgs e)
        {
            // Prevent the user from repeatedly pressing the button and causing
            // a backlong of button presses to be handled. This button is re-eneabled
            // in the TrackReady Playstate handler.
            SetButtonVisibility(false);
            if (string.IsNullOrEmpty(_state.VoiceName))
            {
                BackgroundAudioPlayer.Instance.SkipNext();
            }
            else 
            {
                _IsCancel = true;
                _synth.CancelAll();
                _state.Source.MoveNext(App.DisplaySettings.SyncMediaVerses);
                
                System.Threading.ThreadPool.QueueUserWorkItem(obj =>
                {
                    System.Threading.Thread.Sleep(1000);

                    Dispatcher.BeginInvoke(() =>
                    {
                        if (!_IsCancel)
                        {
                            InfinateLoopTextToSpeech(GetMediaInfo(), GetSynthesizer(_state.VoiceName));
                        }
                    });
                });
            }
        }

        private void ButPlayPauseClick(object sender, RoutedEventArgs e)
        {

            if (string.IsNullOrEmpty(_state.VoiceName))
            {
                if (PlayState.Playing == BackgroundAudioPlayer.Instance.PlayerState)
                {
                    BackgroundAudioPlayer.Instance.Pause();
                }
                else
                {
                    BackgroundAudioPlayer.Instance.Play();
                }
            }
            else
            {
                if (_isPlaying)
                {
                    _IsCancel = true;
                    _synth.CancelAll();
                    SetButtonVisibilityTTS(false);
                }
                else
                {
                    SetButtonVisibilityTTS(true);
                    InfinateLoopTextToSpeech(GetMediaInfo(),GetSynthesizer(_state.VoiceName));
                }
            }
        }

        private void ButPreviousClick(object sender, RoutedEventArgs e)
        {
            
            // Prevent the user from repeatedly pressing the button and causing
            // a backlong of button presses to be handled. This button is re-eneabled
            // in the TrackReady Playstate handler.
            SetButtonVisibility(false);
            if (string.IsNullOrEmpty(_state.VoiceName))
            {
                BackgroundAudioPlayer.Instance.SkipPrevious();
            }
            else
            {
                _IsCancel = true;
                _synth.CancelAll();
                _state.Source.MovePrevious(App.DisplaySettings.SyncMediaVerses);
                System.Threading.ThreadPool.QueueUserWorkItem(obj =>
                {
                    System.Threading.Thread.Sleep(1000);

                    Dispatcher.BeginInvoke(() =>
                    {
                        if (!_IsCancel)
                        {
                            InfinateLoopTextToSpeech(GetMediaInfo(), GetSynthesizer(_state.VoiceName));
                        }
                    });
                });
            }
        }

        private void ButSmallerClick(object sender, RoutedEventArgs e)
        {
            _state.NumRowsIown--;
            ShowSizeButtons();
            if (HitButtonSmaller != null)
            {
                HitButtonSmaller(this, e);
            }
        }

        private void IconDownloadingComplete(object sender, OpenReadCompletedEventArgs e)
        {
            try
            {
                var bitImage = new BitmapImage();
                bitImage.SetSource(e.Result);
                ImageIcon.Source = bitImage;
                ImageIcon.Width = bitImage.PixelWidth;
                ImageIcon.Height = bitImage.PixelHeight;
                ImageIcon.Visibility = Visibility.Visible;
            }
            catch (Exception ee)
            {
                Debug.WriteLine("IconDownloadingComplete error = " + ee);
            }
        }

        private void ImageIconTap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (!string.IsNullOrEmpty(_state.IconLink))
            {
                try
                {
                    var webBrowserTask = new WebBrowserTask { Uri = new Uri(_state.IconLink) };
                    webBrowserTask.Show();
                }
                catch (Exception ee)
                {
                    Debug.WriteLine("ImageIconTap crash; " + ee);
                }
            }
        }

        /// <summary>
        /// Updates the UI with the current song data.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void InstancePlayStateChanged(object sender, EventArgs e)
        {
            switch (BackgroundAudioPlayer.Instance.PlayerState)
            {
                case PlayState.BufferingStarted:
                    break;
                case PlayState.Playing:
                    // try to load the icon.
                    string msg;
                    var info = AudioPlayer.ReadMediaInfoFromXml(BackgroundAudioPlayer.Instance.Track.Tag, out msg);
                    ShowTrack(info);
                    title.Text = BackgroundAudioPlayer.Instance.Track.Title;
                    break;

                case PlayState.Paused:
                case PlayState.Stopped:
                    if (_updatePositionTimer != null)
                    {
                        _updatePositionTimer.Stop();
                    }

                    SetPlayPauseButton(false);     // Change to play button
                    break;
            }
        }

        private void SetStateFromMediaInfo(AudioPlayer.MediaInfo info)
        {
            _state.IconLink = info.IconLink;
            _state.Src = info.Src;
            _state.Pattern = info.Pattern;
            _state.IsNtOnly = info.IsNtOnly;
            _state.code = info.Code;
            _state.VoiceName = info.VoiceName;
            _state.Name = info.Name;
            _state.Language = info.Language;
            _state.Icon = info.Icon;
            _state.Source.MoveChapterVerse(info.Book, info.Chapter, info.Verse, false, _state.Source);
        }
        private AudioPlayer.MediaInfo GetMediaInfo()
        {
            string bookShortName;
            int relChaptNum;
            int verseNum;
            string fullName;
            string title;
            _state.Source.GetInfo(out bookShortName,
                out relChaptNum,
                out verseNum,
                out fullName,
                out title);
            return new AudioPlayer.MediaInfo
            {
                Verse = verseNum,
                Chapter = relChaptNum,
                Book = bookShortName,
                VoiceName = _state.VoiceName,
                Code = _state.code,
                IsNtOnly = _state.IsNtOnly,
                Pattern = _state.Pattern,
                Src = _state.Src,
                IconLink = _state.IconLink,
                Name = _state.Name,
                Language = _state.Language,
                Icon = _state.Icon
            };
        }

        private void MediaPlayerWindowLoaded(object sender, RoutedEventArgs e)
        {
            
            if (_isLoaded)
            {
                return;
            }

            _isLoaded = true;
            UpdateBrowser(false);
            string colorDir = App.Themes.IsButtonColorDark ? "light" : "dark";
            SetButtonVisibility(
                butNext,
                true,
                "/Images/" + colorDir + "/appbar.transport.ff.rest.png",
                "/Images/" + colorDir + "/appbar.transport.ff.pressed.png");
            SetButtonVisibility(
                 butPrevious,
                 true,
                 "/Images/" + colorDir + "/appbar.transport.rew.rest.png",
                 "/Images/" + colorDir + "/appbar.transport.rew.pressed.png");
            if (string.IsNullOrEmpty(_state.VoiceName))
            {
                SetButtonVisibility(false); 
                switch (BackgroundAudioPlayer.Instance.PlayerState)
                {
                    case PlayState.Playing:
                    case PlayState.Paused:
                        if (BackgroundAudioPlayer.Instance.Track != null)
                        {
                            string msg;
                            AudioPlayer.MediaInfo info = AudioPlayer.ReadMediaInfoFromXml(BackgroundAudioPlayer.Instance.Track.Tag, out msg);
                            if (!string.IsNullOrEmpty(info.Src))
                            {
                                this.ShowTrack(info);
                            }
                            else
                            {
                                // do a restart
                                AudioPlayer.StartNewTrack(GetMediaInfo());
                                title.Text = AudioPlayer.GetTitle(GetMediaInfo());
                            }
                        }

                        break;
                    default:
                        // lets start it again.
                        AudioPlayer.StartNewTrack(GetMediaInfo());
                        title.Text = AudioPlayer.GetTitle(GetMediaInfo());
                        break;
                }
            }
            else
            {
                // todo start the other player?
                SetButtonVisibilityTTS(_isPlaying);
            }
        }

        private void SetButtonVisibility(bool isButtonsVisible)
        {
            butPlayPause.Visibility = isButtonsVisible ? Visibility.Visible : Visibility.Collapsed;
            butNext.Visibility = isButtonsVisible ? Visibility.Visible : Visibility.Collapsed;
            butPrevious.Visibility = isButtonsVisible ? Visibility.Visible : Visibility.Collapsed;
            stackContent.Visibility = isButtonsVisible ? Visibility.Visible : Visibility.Collapsed;
            WaitingForDownload.Visibility = !isButtonsVisible ? Visibility.Visible : Visibility.Collapsed;
            if (isButtonsVisible && null != BackgroundAudioPlayer.Instance.Track)
            {
                txtDuration.Text = BackgroundAudioPlayer.Instance.Track.Duration.ToString("c").Substring(3, 5);
                title.Text = BackgroundAudioPlayer.Instance.Track.Title;
            }
        }

        private void SetButtonVisibilityTTS(bool isPlaying)
        {
            butPlayPause.Visibility = true ? Visibility.Visible : Visibility.Collapsed;
            butNext.Visibility = isPlaying ? Visibility.Visible : Visibility.Collapsed;
            butPrevious.Visibility = isPlaying ? Visibility.Visible : Visibility.Collapsed;
            stackContent.Visibility = true ? Visibility.Visible : Visibility.Collapsed;
            stackProgress.Visibility = false ? Visibility.Visible : Visibility.Collapsed;
            WaitingForDownload.Visibility = false ? Visibility.Visible : Visibility.Collapsed;

            SetPlayPauseButtonTTS(isPlaying);

        }

        bool _isPlaying = false;
        private void SetPlayPauseButtonTTS(bool isPlaying)
        {
            string colorDir = App.Themes.IsButtonColorDark ? "light" : "dark";
            butPlayPause.Image =
                GetImage(
                    !isPlaying
                        ? "/Images/" + colorDir + "/appbar.transport.play.rest.png"
                        : "/Images/" + colorDir + "/appbar.transport.pause.rest.png");
            butPlayPause.PressedImage =
                GetImage(
                    !isPlaying
                        ? "/Images/" + colorDir + "/appbar.transport.play.rest.pressed.png"
                        : "/Images/" + colorDir + "/appbar.transport.pause.rest.pressed.png");
            _isPlaying = isPlaying;
        }
        private void SetPlayPauseButton(bool isPlaying)
        {
            SetButtonVisibility(true);

            string colorDir = App.Themes.IsButtonColorDark ? "light" : "dark";
            butPlayPause.Image =
                GetImage(
                    !isPlaying
                        ? "/Images/" + colorDir + "/appbar.transport.play.rest.png"
                        : "/Images/" + colorDir + "/appbar.transport.pause.rest.png");
            butPlayPause.PressedImage =
                GetImage(
                    !isPlaying
                        ? "/Images/" + colorDir + "/appbar.transport.play.rest.pressed.png"
                        : "/Images/" + colorDir + "/appbar.transport.pause.rest.pressed.png");
        }
        private void ShowTrack(AudioPlayer.MediaInfo info)
        {
            SetStateFromMediaInfo(info);
            if (BackgroundAudioPlayer.Instance.PlayerState == PlayState.Playing)
            {
                // start timer
                _updatePositionTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(2) };
                _updatePositionTimer.Tick += UpdatePositionTimerTick;
                _updatePositionTimer.Start();
            }
            else
            {
                if (_updatePositionTimer != null)
                {
                    _updatePositionTimer.Stop();
                }
            }

            if (!string.IsNullOrEmpty(info.Icon))
            {
                try
                {
                    _client = new WebClient();
                    _client.OpenReadCompleted += IconDownloadingComplete;
                    _client.OpenReadAsync(new Uri(info.Icon));
                }
                catch (Exception ee)
                {
                    Debug.WriteLine("Starting icon download error = " + ee);
                }
            }

            SetPlayPauseButton(BackgroundAudioPlayer.Instance.PlayerState == PlayState.Playing);
            butPrevious.IsEnabled = true;
            butNext.IsEnabled = true;
        }

        private void UpdatePositionTimerTick(object sender, EventArgs e)
        {
            // we must delay updating of this webbrowser...
            if (BackgroundAudioPlayer.Instance.Track != null)
            {
                try
                {
                    ProgressDownload.Value = BackgroundAudioPlayer.Instance.BufferingProgress * 100.0;
                    if (BackgroundAudioPlayer.Instance.Track.Duration.Seconds != 0)
                    {
                        ProgressPosition.Value = (100.0 * BackgroundAudioPlayer.Instance.Position.TotalSeconds)
                                                  / BackgroundAudioPlayer.Instance.Track.Duration.TotalSeconds;
                        txtPosition.Text = BackgroundAudioPlayer.Instance.Position.ToString("c").Substring(3, 5);
                        txtDuration.Text = BackgroundAudioPlayer.Instance.Track.Duration.ToString("c").Substring(3, 5);
                    }
                    else
                    {
                        ProgressPosition.Value = 0;
                        txtPosition.Text = "00:00";
                    }
                }
                catch (Exception ee)
                {
                    Logger.Debug("crashed UpdatePositionTimerTick; " + ee);
                }
            }
        }

        #endregion Methods
    }
}