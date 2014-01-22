// <copyright file="MediaPlayerWindow.xaml.cs" company="Thomas Dilts">
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
namespace CrossConnect
{
    using System;
    using System.Diagnostics;

    using CrossConnect.readers;

    using Sword.reader;

    using Windows.Media;
    using Windows.System;
    using Windows.UI.Core;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Input;
    using Windows.UI.Xaml.Media;
    using Windows.UI.Xaml.Media.Imaging;

    public sealed partial class MediaPlayerWindow : ITiledWindow
    {
        #region Fields

        private bool _isLoaded;

        private SerializableWindowState _state = new SerializableWindowState();

        private DispatcherTimer _updatePositionTimer;

        #endregion

        #region Constructors and Destructors

        public MediaPlayerWindow()
        {
            this.InitializeComponent();

            MediaControl.PlayPressed += this.MediaControlPlayPressed;
            MediaControl.PausePressed += this.MediaControlPausePressed;
            MediaControl.PlayPauseTogglePressed += this.MediaControlPlayPauseTogglePressed;
            MediaControl.StopPressed += this.MediaControlStopPressed;
            MediaControl.FastForwardPressed += this.MediaControl_FastForwardPressed;
            MediaControl.RewindPressed += this.MediaControl_RewindPressed;
            MediaControl.ChannelDownPressed += this.MediaControl_ChannelDownPressed;
            MediaControl.ChannelUpPressed += this.MediaControl_ChannelUpPressed;
            MediaControl.PreviousTrackPressed += this.MediaControl_ChannelUpPressed;
            MediaControl.NextTrackPressed += this.MediaControl_ChannelDownPressed;
            MediaControl.AlbumArt = new Uri("ms-appx:///assets/splash150x150.png");

            this.myMedia.AudioCategory = AudioCategory.BackgroundCapableMedia;
        }

        #endregion

        #region Public Events

        public event EventHandler HitButtonClose;

        public event EventHandler HitButtonSmaller;

        #endregion

        #region Public Properties

        public bool ForceReload { get; set; }

        public SerializableWindowState State
        {
            get
            {
                return this._state;
            }

            set
            {
                this._state = value;
            }
        }

        #endregion

        #region Public Methods and Operators

        public static void StartNewTrack(AudioPlayer.MediaInfo info)
        {
        }

        public void CalculateTitleTextWidth()
        {
        }

        public void DelayUpdateBrowser()
        {
            this.UpdateLayout();
            var tmr = new DispatcherTimer();
            tmr.Tick += this.OnDelayUpdateTimerTick;
            tmr.Interval = TimeSpan.FromSeconds(5);
            tmr.Start();
        }

        public void RestartToThisMedia(AudioPlayer.MediaInfo info)
        {
            ((MediaReader)this._state.Source).Info = info;
            AudioPlayer.SetRelativeChapter(0, info);
            this.myMedia.Source = new Uri(((MediaReader)this._state.Source).Info.Src);
            if (this.title != null)
            {
                this.title.Text = AudioPlayer.GetTitle(info) + "        " + ((MediaReader)this._state.Source).Info.Name;
                this.SetButtonVisibility(false);
                MediaControl.TrackName = this.title.Text;
                MediaControl.ArtistName = ((MediaReader)this._state.Source).Info.Name;
            }
        }

        public void ShowSizeButtons(bool isShow = true)
        {
            this.CalculateTitleTextWidth();
        }

        public void ShowUserInterface(bool isShow)
        {
        }

        public void SynchronizeWindow(string bookShortName, int chapterNum, int verseNum, IBrowserTextSource source)
        {
        }

        public void UpdateBrowser(bool isOrientationChangeOnly)
        {
            this.border1.BorderBrush = new SolidColorBrush(App.Themes.BorderColor);
            this.WebBrowserBorder.BorderBrush = this.border1.BorderBrush;
            this.grid1.Background = new SolidColorBrush(App.Themes.TitleBackColor);
            this.title.Foreground = new SolidColorBrush(App.Themes.TitleFontColor);
            this.AllContent.Background = new SolidColorBrush(App.Themes.MainBackColor);

            this.ProgressPosition.Foreground = new SolidColorBrush(App.Themes.AccentColor);
            this.txtDuration.Foreground = new SolidColorBrush(App.Themes.MainFontColor);
            this.txtPosition.Foreground = new SolidColorBrush(App.Themes.MainFontColor);
            this.grid1.UpdateLayout();
            double newWidth = this.grid1.ActualWidth - (this.txtPosition.Width * 2) - 15;
            if (newWidth > 0)
            {
                this.GridProgressBars.Width = this.grid1.ActualWidth - (this.txtPosition.Width * 2) - 15;
            }
        }

        #endregion

        #region Methods

        private void ButCloseClick(object sender, RoutedEventArgs e)
        {
            if (this.HitButtonClose != null)
            {
                this.HitButtonClose(this, null);
            }
        }

        private void ButNextClick(object sender, RoutedEventArgs e)
        {
            if (((MediaReader)this._state.Source).Info != null)
            {
                AudioPlayer.SetRelativeChapter(1, ((MediaReader)this._state.Source).Info);
                this.RestartToThisMedia(((MediaReader)this._state.Source).Info);
            }

            // Prevent the user from repeatedly pressing the button and causing
            // a backlong of button presses to be handled. This button is re-eneabled
            // in the TrackReady Playstate handler.
            //SetButtonVisibility(false);
        }

        private void ButPause_OnClick(object sender, RoutedEventArgs e)
        {
            this.myMedia.Pause();
        }

        private void ButPlay_OnClick(object o, RoutedEventArgs e)
        {
            this.myMedia.Play();
        }

        private void ButPreviousClick(object sender, RoutedEventArgs e)
        {
            if (((MediaReader)this._state.Source).Info != null)
            {
                AudioPlayer.SetRelativeChapter(-1, ((MediaReader)this._state.Source).Info);
                this.RestartToThisMedia(((MediaReader)this._state.Source).Info);
            }

            // Prevent the user from repeatedly pressing the button and causing
            // a backlong of button presses to be handled. This button is re-eneabled
            // in the TrackReady Playstate handler.
            //SetButtonVisibility(false);
        }

        private void ImageIcon_OnTapped(object sender, TappedRoutedEventArgs e)
        {
            AudioPlayer.MediaInfo info = ((MediaReader)this._state.Source).Info;
            if (info != null && !string.IsNullOrEmpty(info.IconLink))
            {
                try
                {
                    Launcher.LaunchUriAsync(new Uri(info.IconLink, UriKind.Absolute));
                }
                catch (Exception ee)
                {
                    Debug.WriteLine("ImageIconTap crash; " + ee);
                }
            }
        }

        private void MediaControlPausePressed(object sender, object e)
        {
            this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => this.myMedia.Pause());
        }

        private async void MediaControlPlayPauseTogglePressed(object sender, object e)
        {
            if (MediaControl.IsPlaying)
            {
                await this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => { this.myMedia.Pause(); });
            }
            else
            {
                await this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => { this.myMedia.Play(); });
            }
        }

        private void MediaControlPlayPressed(object sender, object e)
        {
            this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => this.myMedia.Play());
        }

        private void MediaControlStopPressed(object sender, object e)
        {
            this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => this.myMedia.Stop());
        }

        private void MediaControl_ChannelDownPressed(object sender, object e)
        {
            this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => this.ButNextClick(null, null));
        }

        private void MediaControl_ChannelUpPressed(object sender, object e)
        {
            this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => this.ButPreviousClick(null, null));
        }

        private void MediaControl_FastForwardPressed(object sender, object e)
        {
        }

        private void MediaControl_RewindPressed(object sender, object e)
        {
        }

        private void MediaPlayerWindowLoaded(object sender, RoutedEventArgs e)
        {
            this.UpdateBrowser(false);
            if (this._isLoaded)
            {
                return;
            }

            this._isLoaded = true;

            this.SetButtonVisibility(false);

            switch (this.myMedia.CurrentState)
            {
                case MediaElementState.Playing:
                case MediaElementState.Paused:
                    if (((MediaReader)this._state.Source).Info != null)
                    {
                        if (!string.IsNullOrEmpty(((MediaReader)this._state.Source).Info.Src))
                        {
                            this.ShowTrack(((MediaReader)this._state.Source).Info);
                        }
                        else
                        {
                            // do a restart
                            this.RestartToThisMedia(((MediaReader)this._state.Source).Info);
                        }
                    }

                    break;
                default:
                    // lets start it again.
                    this.RestartToThisMedia(((MediaReader)this._state.Source).Info);
                    break;
            }
        }

        private void MediaPlayerWindow_OnLayoutUpdated(object sender, object e)
        {
        }

        private void MediaPlayerWindow_OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            this.gridPanelButtons1.Width = this.grid1.ActualWidth;
        }

        private void MyMedia_OnBufferingProgressChanged(object sender, RoutedEventArgs e)
        {
        }

        private void MyMedia_OnCurrentStateChanged(object sender, RoutedEventArgs e)
        {
            this.ButPlay.Visibility = (this.myMedia.CurrentState != MediaElementState.Playing)
                                          ? Visibility.Visible
                                          : Visibility.Collapsed;
            this.ButPause.Visibility = (this.ButPlay.Visibility == Visibility.Collapsed)
                                           ? Visibility.Visible
                                           : Visibility.Collapsed;
            MediaControl.IsPlaying = (this.myMedia.CurrentState == MediaElementState.Playing);
            switch (this.myMedia.CurrentState)
            {
                case MediaElementState.Buffering:
                    break;
                case MediaElementState.Playing:
                    // try to load the icon.
                    AudioPlayer.MediaInfo info = ((MediaReader)this._state.Source).Info;
                    this.ShowTrack(info);
                    //title.Text = AudioPlayer.GetTitle(info);
                    this.SetButtonVisibility(true);
                    break;

                case MediaElementState.Paused:
                    if (this.myMedia.Position.Equals(this.myMedia.NaturalDuration.TimeSpan))
                    {
                        this.ButNextClick(null,null);
                    }
                    else
                    {
                        if (this._updatePositionTimer != null)
                        {
                            this._updatePositionTimer.Stop();
                        }
                    }
                    break;
                case MediaElementState.Stopped:
                    if (this._updatePositionTimer != null)
                    {
                        this._updatePositionTimer.Stop();
                    }
                    break;
            }
        }

        private void MyMedia_OnDownloadProgressChanged(object sender, RoutedEventArgs e)
        {
        }

        private void MyMedia_OnMarkerReached(object sender, TimelineMarkerRoutedEventArgs e)
        {
        }

        private void MyMedia_OnMediaEnded(object sender, RoutedEventArgs e)
        {
        }

        private void MyMedia_OnMediaFailed(object sender, ExceptionRoutedEventArgs e)
        {
        }

        private void MyMedia_OnMediaOpened(object sender, RoutedEventArgs e)
        {
        }

        private void OnDelayUpdateTimerTick(object sender, object e)
        {
            ((DispatcherTimer)sender).Stop();

            this.UpdateBrowser(false);
        }

        private void SetButtonVisibility(bool isButtonsVisible)
        {
            this.ButPlay.Visibility = isButtonsVisible ? Visibility.Visible : Visibility.Collapsed;
            this.ButPause.Visibility = isButtonsVisible ? Visibility.Visible : Visibility.Collapsed;
            this.ButNext.Visibility = isButtonsVisible ? Visibility.Visible : Visibility.Collapsed;
            this.ButPrevious.Visibility = isButtonsVisible ? Visibility.Visible : Visibility.Collapsed;
            this.stackContent.Visibility = isButtonsVisible ? Visibility.Visible : Visibility.Collapsed;
            this.WaitingForDownload.Visibility = !isButtonsVisible ? Visibility.Visible : Visibility.Collapsed;
            if (isButtonsVisible)
            {
                this.ButPlay.Visibility = (this.myMedia.CurrentState != MediaElementState.Playing)
                                              ? Visibility.Visible
                                              : Visibility.Collapsed;
                this.ButPause.Visibility = (this.ButPlay.Visibility == Visibility.Collapsed)
                                               ? Visibility.Visible
                                               : Visibility.Collapsed;
            }
            MediaControl.IsPlaying = (this.myMedia.CurrentState == MediaElementState.Playing);

            //if (isButtonsVisible && null != BackgroundAudioPlayer.Instance.Track)
            //{
            //    myMedia.
            //    txtDuration.Text = BackgroundAudioPlayer.Instance.Track.Duration.ToString("c").Substring(3, 5);
            //    title.Text = BackgroundAudioPlayer.Instance.Track.Title;
            //}
        }

        private void ShowTrack(AudioPlayer.MediaInfo info)
        {
            ((MediaReader)this._state.Source).Info = info;
            if (this.myMedia.CurrentState == MediaElementState.Playing)
            {
                // start timer
                this._updatePositionTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(2) };
                this._updatePositionTimer.Tick += this.UpdatePositionTimerTick;
                this._updatePositionTimer.Start();
            }
            else
            {
                if (this._updatePositionTimer != null)
                {
                    this._updatePositionTimer.Stop();
                }
            }

            if (!string.IsNullOrEmpty(info.Icon))
            {
                this.ImageIcon.Source = new BitmapImage(new Uri(info.Icon));
            }
            else
            {
                this.ImageIcon.Source = null;
            }
        }

        private void UpdatePositionTimerTick(object sender, object other)
        {
            AudioPlayer.MediaInfo info = ((MediaReader)this._state.Source).Info;
            if (info != null && info.Src != null)
            {
                try
                {
                    this.ProgressDownload.Value = this.myMedia.BufferingProgress * 100.0;
                    if (this.myMedia.NaturalDuration.TimeSpan.Seconds != 0
                        && this.myMedia.Position.TotalSeconds > 0.0001)
                    {
                        this.ProgressPosition.Value = (100.0 * this.myMedia.Position.TotalSeconds)
                                                      / this.myMedia.NaturalDuration.TimeSpan.TotalSeconds;
                        this.txtPosition.Text = this.myMedia.Position.ToString("c").Substring(3, 5);
                        this.txtDuration.Text = this.myMedia.NaturalDuration.TimeSpan.ToString("c").Substring(3, 5);
                    }
                    else
                    {
                        this.ProgressPosition.Value = 0;
                        this.txtPosition.Text = "00:00";
                    }
                }
                catch (Exception ee)
                {
                    Logger.Debug("crashed UpdatePositionTimerTick; " + ee);
                }
            }
        }

        #endregion
    }
}