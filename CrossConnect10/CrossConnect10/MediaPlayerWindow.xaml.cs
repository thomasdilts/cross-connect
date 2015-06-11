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
    using System.Linq;

    using CrossConnect.readers;

    using Sword.reader;

    using Windows.Media;
    using Windows.System;
    using Windows.UI.Core;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Input;
    using Windows.UI.Xaml.Media;
    using Windows.UI.Xaml.Media.Imaging;
    using Windows.Media.SpeechSynthesis;
    using System.Threading.Tasks;
    using Windows.Media.Playback;
    using System.Threading;
    using BackgroundAudioShared.Messages;
    using Windows.Foundation;
    using BackgroundAudioShared;
    using Windows.UI.Popups;

    public sealed partial class MediaPlayerWindow : ITiledWindow
    {
        #region Fields

        private bool _isLoaded;

        private SerializableWindowState _state = new SerializableWindowState();
        private AutoResetEvent backgroundAudioTaskStarted;
        private bool isMyBackgroundTaskRunning = false;
        private BackgroundAudioShared.AudioModel Info= null;

        #endregion

        #region Constructors and Destructors

        public MediaPlayerWindow()
        {
            this.InitializeComponent();
            // Setup the initialization lock
            backgroundAudioTaskStarted = new AutoResetEvent(false);
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
        /// <summary>
        /// Gets the information about background task is running or not by reading the setting saved by background task.
        /// This is used to determine when to start the task and also when to avoid sending messages.
        /// </summary>
        private bool IsMyBackgroundTaskRunning
        {
            get
            {
                if (isMyBackgroundTaskRunning)
                    return true;

                string value = ApplicationSettingsHelper.ReadResetSettingsValue(ApplicationSettingsConstants.BackgroundTaskState) as string;
                if (value == null)
                {
                    return false;
                }
                else
                {
                    try
                    {
                        isMyBackgroundTaskRunning = EnumHelper.Parse<BackgroundTaskState>(value) == BackgroundTaskState.Running;
                    }
                    catch (ArgumentException)
                    {
                        isMyBackgroundTaskRunning = false;
                    }
                    return isMyBackgroundTaskRunning;
                }
            }
        }
        #endregion

        #region Public Methods and Operators

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

        /// <summary>
        /// Unsubscribes to MediaPlayer events. Should run only on suspend
        /// </summary>
        private void RemoveMediaPlayerEventHandlers()
        {
            BackgroundMediaPlayer.Current.CurrentStateChanged -= this.MediaPlayer_CurrentStateChanged;
            BackgroundMediaPlayer.MessageReceivedFromBackground -= this.BackgroundMediaPlayer_MessageReceivedFromBackground;
        }

        /// <summary>
        /// Subscribes to MediaPlayer events
        /// </summary>
        private void AddMediaPlayerEventHandlers()
        {
            BackgroundMediaPlayer.Current.CurrentStateChanged += this.MediaPlayer_CurrentStateChanged;
            BackgroundMediaPlayer.MessageReceivedFromBackground += this.BackgroundMediaPlayer_MessageReceivedFromBackground;
        }

        private void MediaPlayerWindowUnloaded(object sender, RoutedEventArgs e)
        {
            if (isMyBackgroundTaskRunning)
            {
                RemoveMediaPlayerEventHandlers();
            }
            BackgroundMediaPlayer.Current.Pause();
            ApplicationSettingsHelper.SaveSettingsValue(ApplicationSettingsConstants.BackgroundTaskState, BackgroundTaskState.Running.ToString());
        }

        private void MediaPlayerWindowLoaded(object sender, RoutedEventArgs e)
        {
            Application.Current.Suspending += ForegroundApp_Suspending;
            Application.Current.Resuming += ForegroundApp_Resuming;

            this.UpdateBrowser(false);
            if (this._isLoaded)
            {
                return;
            }

            this._isLoaded = true;

            this.SetButtonVisibility(false);

            switch (BackgroundMediaPlayer.Current.CurrentState)
            {
                case MediaPlayerState.Playing:
                case MediaPlayerState.Paused:
                    if (Info != null)
                    {
                        if (!string.IsNullOrEmpty(Info.Src))
                        {
                            this.ShowTrack(Info);
                        }
                        else
                        {
                            // do a restart
                            this.RestartToThisMedia();
                        }
                    }

                    break;
                case MediaPlayerState.Closed:
                    break;
                default:
                    // lets start it again.
                    this.RestartToThisMedia();
                    break;
            }
        }


        /// <summary>
        /// MediaPlayer state changed event handlers. 
        /// Note that we can subscribe to events even if Media Player is playing media in background
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        async void MediaPlayer_CurrentStateChanged(MediaPlayer sender, object args)
        {
            var currentState = sender.CurrentState; // cache outside of completion or you might get a different value
            await this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                // Update controls
                UpdateTransportControls(currentState);
            });
        }
        private void UpdateTransportControls(MediaPlayerState state)
        {
            if (state == MediaPlayerState.Playing)
            {
                //playButton.Content = "| |";     // Change to pause button
            }
            else
            {
                //playButton.Content = ">";     // Change to play button
            }
        }
        /// <summary>
        /// This event is raised when a message is recieved from BackgroundAudioTask
        /// </summary>
        async void BackgroundMediaPlayer_MessageReceivedFromBackground(object sender, MediaPlayerDataReceivedEventArgs e)
        {
            PositionMessage backgroundPositionMessage;
            if (MessageService.TryParseMessage(e.Data, out backgroundPositionMessage))
            {
                // StartBackgroundAudioTask is waiting for this signal to know when the task is up and running
                // and ready to receive messages
                Debug.WriteLine("position " + backgroundPositionMessage.procent.ToString());
                //this.txtCurrentState.Text = backgroundPositionMessage.procent.ToString();
                return;
            }

            TrackChangedMessage trackChangedMessage;
            if (MessageService.TryParseMessage(e.Data, out trackChangedMessage))
            {
                Info = trackChangedMessage.audioModel;
                // When foreground app is active change track based on background message
                await this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    SetButtonVisibility(true, true);
                    var verseText = string.IsNullOrEmpty(trackChangedMessage.audioModel.VoiceName) ? string.Empty : ":" + (trackChangedMessage.audioModel.Verse + 1).ToString();
                    this.title.Text = trackChangedMessage.title + verseText + "        " + Info.Name;
                    App.SynchronizeAllWindows(trackChangedMessage.audioModel.Book, trackChangedMessage.audioModel.Chapter, trackChangedMessage.audioModel.Verse, this._state.CurIndex, this._state.Source);
                });
                return;
            }

            BackgroundAudioTaskStartedMessage backgroundAudioTaskStartedMessage;
            if (MessageService.TryParseMessage(e.Data, out backgroundAudioTaskStartedMessage))
            {
                // StartBackgroundAudioTask is waiting for this signal to know when the task is up and running
                // and ready to receive messages
                Debug.WriteLine("BackgroundAudioTask started");
                backgroundAudioTaskStarted.Set();
                return;
            }

            ErrorMessage errorMessage;
            if (MessageService.TryParseMessage(e.Data, out errorMessage))
            {
                await this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    var dialog =
                        new MessageDialog(
                            Translations.Translate(
                                "An error occurred trying to connect to the network. Try again later.") + "; " + errorMessage.message);
                    dialog.ShowAsync();
                });
                return;
            }
        }
        /// <summary>
        /// Initialize Background Media Player Handlers and starts playback
        /// </summary>
        private void StartBackgroundAudioTask()
        {

            var startResult = this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                bool result = backgroundAudioTaskStarted.WaitOne(10000);
                //Send message to initiate playback
                if (result == true)
                {
                    MessageService.SendMessageToBackground(new UpdateStartPointMessage(Info));
                    if (string.IsNullOrEmpty(Info.VoiceName))
                    {
                    }
                    else
                    {
                    }
                }
                else
                {
                    throw new Exception("Background Audio Task didn't start in expected time");
                }
            });
            startResult.Completed = new AsyncActionCompletedHandler(BackgroundTaskInitializationCompleted);
        }

        private void BackgroundTaskInitializationCompleted(IAsyncAction action, AsyncStatus status)
        {
            if (status == AsyncStatus.Completed)
            {
                Debug.WriteLine("Background Audio Task initialized");
            }
            else if (status == AsyncStatus.Error)
            {
                Debug.WriteLine("Background Audio Task could not initialized due to an error ::" + action.ErrorCode.ToString());
            }
        }

        private void RestartToThisMedia()
        {
            if (Info == null || (string.IsNullOrEmpty(Info.VoiceName) && string.IsNullOrEmpty(Info.Src)))
            {
                return;
            }
            this.ButPlay_OnClick(null, null);
            App.StartTimerForSavingWindows();
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

            this.grid1.UpdateLayout();
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
            if (Info != null)
            {
                MessageService.SendMessageToBackground(new SkipNextMessage());

                // Prevent the user from repeatedly pressing the button and causing 
                // a backlong of button presses to be handled. This button is re-eneabled 
                // in the TrackReady Playstate handler.
                ButNext.IsEnabled = false;
            }
        }

        private void ButPause_OnClick(object sender, RoutedEventArgs e)
        {
            BackgroundMediaPlayer.Current.Pause();
            SetButtonVisibility(true);
        }

        private void ButPlay_OnClick(object o, RoutedEventArgs e)
        {
            Debug.WriteLine("Play button pressed from App");
            if (IsMyBackgroundTaskRunning)
            {
                if (MediaPlayerState.Playing == BackgroundMediaPlayer.Current.CurrentState)
                {
                    BackgroundMediaPlayer.Current.Pause();
                }
                else if (MediaPlayerState.Paused == BackgroundMediaPlayer.Current.CurrentState)
                {
                    if (o == null)
                    {
                        MessageService.SendMessageToBackground(new UpdateStartPointMessage(Info));
                    }
                    else
                    {
                        BackgroundMediaPlayer.Current.Play();
                    }
                }
                else if (MediaPlayerState.Closed == BackgroundMediaPlayer.Current.CurrentState)
                {
                    StartBackgroundAudioTask();
                }
                SetButtonVisibility(true);
            }
            else
            {
                StartBackgroundAudioTask();
            }
        }

        private void ButPreviousClick(object sender, RoutedEventArgs e)
        {
            if (Info != null)
            {
                MessageService.SendMessageToBackground(new SkipPreviousMessage());

                // Prevent the user from repeatedly pressing the button and causing 
                // a backlong of button presses to be handled. This button is re-eneabled 
                // in the TrackReady Playstate handler.
                ButPrevious.IsEnabled = false;
            }
        }

        private void ImageIcon_OnTapped(object sender, TappedRoutedEventArgs e)
        {
            BackgroundAudioShared.AudioModel info = Info;
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

        public void SetMediaInfo(SerializableWindowState theState, BackgroundAudioShared.AudioModel info)
        {
            this._state = theState;
            this.Info = info;
            theState.VoiceName = info.VoiceName;
            theState.Src = info.Src;
            theState.Pattern = info.Pattern;
            theState.IsNtOnly = info.IsNtOnly;
            theState.code = info.Code;
            if(!string.IsNullOrEmpty(info.VoiceName) && !string.IsNullOrEmpty(theState.BibleToLoad))
            {
                this.Info.Src = theState.BibleToLoad;
            }
            AddMediaPlayerEventHandlers();
            this.RestartToThisMedia();
        }

        private void MediaPlayerWindow_OnLayoutUpdated(object sender, object e)
        {
        }

        private void MediaPlayerWindow_OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            this.gridPanelButtons1.Width = this.grid1.ActualWidth;
        }

        private void MyMedia_OnCurrentStateChanged(object sender, RoutedEventArgs e)
        {
            this.ButPlay.Visibility = (BackgroundMediaPlayer.Current.CurrentState != MediaPlayerState.Playing)
                                          ? Visibility.Visible
                                          : Visibility.Collapsed;
            this.ButPause.Visibility = (this.ButPlay.Visibility == Visibility.Collapsed)
                                           ? Visibility.Visible
                                           : Visibility.Collapsed;
            //MediaControl.IsPlaying = (this.myMedia.CurrentState == MediaElementState.Playing);
            switch (BackgroundMediaPlayer.Current.CurrentState)
            {
                case MediaPlayerState.Buffering:
                    break;
                case MediaPlayerState.Playing:
                    // try to load the icon.
                    BackgroundAudioShared.AudioModel info = Info;
                    this.ShowTrack(info);
                    //title.Text = AudioPlayerReader.GetTitle(info);
                    this.SetButtonVisibility(true);
                    break;

                case MediaPlayerState.Paused:
                    //if (this.myMedia.Position.Seconds.Equals(this.myMedia.NaturalDuration.TimeSpan.Seconds))
                    //{
                    //    this.ButNextClick(null,null);
                    //}
                    //else
                    //{
                    //    if (this._updatePositionTimer != null)
                    //    {
                    //        this._updatePositionTimer.Stop();
                    //    }
                    //}
                    break;
                case MediaPlayerState.Stopped:
                    //if (this._updatePositionTimer != null)
                    //{
                    //    this._updatePositionTimer.Stop();
                    //}
                    break;
            }
        }

        private void OnDelayUpdateTimerTick(object sender, object e)
        {
            ((DispatcherTimer)sender).Stop();

            this.UpdateBrowser(false);
        }

        private void SetButtonVisibility(bool isButtonsVisible, bool overridePlay=false)
        {
            this.ButPlay.Visibility = isButtonsVisible ? Visibility.Visible : Visibility.Collapsed;
            this.ButPause.Visibility = isButtonsVisible ? Visibility.Visible : Visibility.Collapsed;
            this.ButNext.Visibility = isButtonsVisible ? Visibility.Visible : Visibility.Collapsed;
            this.ButPrevious.Visibility = isButtonsVisible ? Visibility.Visible : Visibility.Collapsed;
            this.stackContent.Visibility = isButtonsVisible ? Visibility.Visible : Visibility.Collapsed;
            this.WaitingForDownload.Visibility = !isButtonsVisible ? Visibility.Visible : Visibility.Collapsed;
            this.ButNext.IsEnabled = true;
            this.ButPrevious.IsEnabled = true;
            if (isButtonsVisible)
            {
                this.ButPlay.Visibility = (!overridePlay && BackgroundMediaPlayer.Current.CurrentState != MediaPlayerState.Playing)
                                              ? Visibility.Visible
                                              : Visibility.Collapsed;
                this.ButPause.Visibility = (this.ButPlay.Visibility == Visibility.Collapsed)
                                               ? Visibility.Visible
                                               : Visibility.Collapsed;
            }
        }

        private void ShowTrack(BackgroundAudioShared.AudioModel info)
        {
            Info = info;

            if (!string.IsNullOrEmpty(info.Icon))
            {
                this.ImageIcon.Source = new BitmapImage(new Uri(info.Icon));
            }
            else
            {
                this.ImageIcon.Source = null;
            }
        }

        /// <summary>
        /// Sends message to background informing app has resumed
        /// Subscribe to MediaPlayer events
        /// </summary>
        void ForegroundApp_Resuming(object sender, object e)
        {
            ApplicationSettingsHelper.SaveSettingsValue(ApplicationSettingsConstants.AppState, AppState.Active.ToString());

            // Verify the task is running
            if (IsMyBackgroundTaskRunning)
            {
                // If yes, it's safe to reconnect to media play handlers
                AddMediaPlayerEventHandlers();

                // Send message to background task that app is resumed so it can start sending notifications again
                MessageService.SendMessageToBackground(new AppResumedMessage());

                UpdateTransportControls(BackgroundMediaPlayer.Current.CurrentState);
                //var saved = AudioModel.CreateFromSettings();

                //txtCurrentTrack.Text = saved.Src;
                //txtCurrentState.Text = BackgroundMediaPlayer.Current.CurrentState.ToString();
            }
            else
            {
                //playButton.Content = ">";     // Change to play button
                //txtCurrentTrack.Text = string.Empty;
                //txtCurrentState.Text = "Background Task Not Running";
            }
        }

        /// <summary>
        /// Send message to Background process that app is to be suspended
        /// Stop clock and slider when suspending
        /// Unsubscribe handlers for MediaPlayer events
        /// </summary>
        void ForegroundApp_Suspending(object sender, Windows.ApplicationModel.SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();

            // Only if the background task is already running would we do these, otherwise
            // it would trigger starting up the background task when trying to suspend.
            if (IsMyBackgroundTaskRunning)
            {
                // Stop handling player events immediately
                RemoveMediaPlayerEventHandlers();

                // Tell the background task the foreground is suspended
                MessageService.SendMessageToBackground(new AppSuspendedMessage());
            }

            deferral.Complete();
        }
        #endregion

    }
}