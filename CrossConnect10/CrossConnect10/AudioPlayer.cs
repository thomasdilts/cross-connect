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

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Media;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

namespace CrossConnect
{
    internal enum AudioPlayerStates
    {
        Normal,
        Playing,
        Paused,
        Seeking
    }

    [TemplatePart(Name = "mediaElement", Type = typeof(MediaElement))]
    [TemplatePart(Name = "positionBar", Type = typeof(Slider))]
    [TemplatePart(Name = "trackPosition", Type = typeof(TextBlock))]
    [TemplatePart(Name = "trackLength", Type = typeof(TextBlock))]
    [TemplatePart(Name = "volumeBar", Type = typeof(Slider))]
    [TemplatePart(Name = "muteButton", Type = typeof(Button))]
    [TemplatePart(Name = "unMuteButton", Type = typeof(Button))]
    [TemplatePart(Name = "prevTrackButton", Type = typeof(Button))]
    [TemplatePart(Name = "rwButton", Type = typeof(Button))]
    [TemplatePart(Name = "stopButton", Type = typeof(Button))]
    [TemplatePart(Name = "playButton", Type = typeof(Button))]
    [TemplatePart(Name = "pauseButton", Type = typeof(Button))]
    [TemplatePart(Name = "ffButton", Type = typeof(Button))]
    [TemplatePart(Name = "nextTrackButton", Type = typeof(Button))]
    public sealed class AudioPlayer : Control
    {
        private MediaElement mediaElement = null;
        private Slider positionBar = null;
        private TextBlock trackPosition = null;
        private TextBlock trackLength = null;
        private Slider volumeBar = null;
        private Button muteButton = null;
        private Button unMuteButton = null;
        private Button prevTrackButton = null;
        private Button rwButton = null;
        private Button stopButton = null;
        private Button playButton = null;
        private Button pauseButton = null;
        private Button ffButton = null;
        private Button nextTrackButton = null;

        private bool _repeatPlayList = true;
        private bool _updating = false;
        private bool _isRewinding = false;
        private double _playRate = 1.0d;
        private double _ffRate = 10.0d;
        private DispatcherTimer _timer = null;
        private List<Uri> _playList = null;
        private Uri _selectedItem = null;
        private MediaElementState _previousState = MediaElementState.Stopped;

        #region DPs

        // Items
        public static readonly DependencyProperty ItemsProperty =
            DependencyProperty.Register("Items", typeof(object), typeof(AudioPlayer), new PropertyMetadata(null, ItemsPropertyChanged));

        public object Items
        {
            get { return (object)GetValue(ItemsProperty); }
            set { SetValue(ItemsProperty, value); }
        }

        public static void ItemsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as AudioPlayer;
            if(e.NewValue is IEnumerable<Uri>)
                control._playList = (e.NewValue as IEnumerable<Uri>).ToList();
        }

        // Selected Item
        public static readonly DependencyProperty SelectedItemProperty =
            DependencyProperty.Register("SelectedItem", typeof(Uri), typeof(AudioPlayer), new PropertyMetadata(null, SelectedItemChanged));

        public Uri SelectedItem
        {
            get { return (Uri)GetValue(SelectedItemProperty); }
            set { SetValue(SelectedItemProperty, value); }
        }
        
        private static void SelectedItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as AudioPlayer;
            control._selectedItem = e.NewValue as Uri;
            if (control.mediaElement != null)
            {
                control._previousState = control.mediaElement.CurrentState;

                control.mediaElement.Source = new Uri("http://www.cross-connect.se/bibles/talking/fr/Bible_fr_01_004.mp3");//control._selectedItem;
            }
        }

        #endregion

        public AudioPlayer()
        {
            base.DefaultStyleKey = typeof(AudioPlayer);
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            this.mediaElement = base.GetTemplateChild("mediaElement") as MediaElement;
            this.positionBar = base.GetTemplateChild("positionBar") as Slider;
            this.trackPosition = base.GetTemplateChild("trackPosition") as TextBlock;
            this.trackLength = base.GetTemplateChild("trackLength") as TextBlock;
            this.volumeBar = base.GetTemplateChild("volumeBar") as Slider;
            this.muteButton = base.GetTemplateChild("muteButton") as Button;
            this.unMuteButton = base.GetTemplateChild("unMuteButton") as Button;
            this.prevTrackButton = base.GetTemplateChild("prevTrackButton") as Button;
            this.rwButton = base.GetTemplateChild("rwButton") as Button;
            this.stopButton = base.GetTemplateChild("stopButton") as Button;
            this.playButton = base.GetTemplateChild("playButton") as Button;
            this.pauseButton = base.GetTemplateChild("pauseButton") as Button;
            this.ffButton = base.GetTemplateChild("ffButton") as Button;
            this.nextTrackButton = base.GetTemplateChild("nextTrackButton") as Button;

            this.ChangeVisualState(AudioPlayerStates.Normal);

            // Attach MediaControl events
            //MediaControl.FastForwardPressed += MediaControl_FastForwardPressed;
            //MediaControl.NextTrackPressed += MediaControl_NextTrackPressed;
            //MediaControl.PausePressed += MediaControl_PausePressed;
            //MediaControl.PlayPauseTogglePressed += MediaControl_PlayPauseTogglePressed;
            //MediaControl.PlayPressed += MediaControl_PlayPressed;
            //MediaControl.PreviousTrackPressed += MediaControl_PreviousTrackPressed;
            //MediaControl.RewindPressed += MediaControl_RewindPressed;
            //MediaControl.StopPressed += MediaControl_StopPressed;
            //MediaControl.SoundLevelChanged += MediaControl_SoundLevelChanged;

            //// Attach MediaElement events
            //if (this.mediaElement != null)
            //{
            //    this.mediaElement.BufferingProgressChanged += mediaElement_BufferingProgressChanged;
            //    this.mediaElement.CurrentStateChanged += mediaElement_CurrentStateChanged;
            //    this.mediaElement.DownloadProgressChanged += mediaElement_DownloadProgressChanged;
            //    this.mediaElement.MediaEnded += mediaElement_MediaEnded;
            //    this.mediaElement.MediaFailed += mediaElement_MediaFailed;
            //    this.mediaElement.MediaOpened += mediaElement_MediaOpened;
            //    this.mediaElement.SeekCompleted += mediaElement_SeekCompleted;
            //    this.mediaElement.VolumeChanged += mediaElement_VolumeChanged;
            //}

            // Attach button events
            if (this.prevTrackButton != null)
                this.prevTrackButton.Tapped += prevTrackButton_Tapped;

            if (this.rwButton != null)
                this.rwButton.Tapped += rwButton_Tapped;

            if (this.stopButton != null)
                this.stopButton.Tapped += stopButton_Tapped;

            if (this.playButton != null)
                this.playButton.Tapped += playButton_Tapped;

            if (this.pauseButton != null)
                this.pauseButton.Tapped += pauseButton_Tapped;

            if (this.ffButton != null)
                this.ffButton.Tapped += ffButton_Tapped;

            if (this.nextTrackButton != null)
                this.nextTrackButton.Tapped += nextTrackButton_Tapped;

            if (this.muteButton != null)
                this.muteButton.Tapped += muteButton_Tapped;

            if (this.unMuteButton != null)
                this.unMuteButton.Tapped += unMuteButton_Tapped;

            // Attach slider events
            if (this.positionBar != null)
                this.positionBar.ValueChanged += positionBar_ValueChanged;

            if (this.volumeBar != null)
                this.volumeBar.ValueChanged += volumeBar_ValueChanged;

            // Update timer
            this._timer = new DispatcherTimer();
            this._timer.Interval = TimeSpan.FromMilliseconds(500);
            this._timer.Tick += _timer_Tick;
            this._timer.Start();

            // Set volume position
            if (this.volumeBar != null)
                this.volumeBar.Value = this.mediaElement.Volume * 100;
        }

        #region MediaControl Event Handlers

        private async void MediaControl_StopPressed(object sender, object e)
        {
            await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                this.OnStopPressed();
            });
        }

        private async void MediaControl_RewindPressed(object sender, object e)
        {
            await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                this.OnRewindPressed();
            });
        }

        private async void MediaControl_PreviousTrackPressed(object sender, object e)
        {
            await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                this.OnPreviousTrackPressed();
            });
        }

        private async void MediaControl_PlayPressed(object sender, object e)
        {
            await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                this.OnPlayPressed();
            });
        }

        private async void MediaControl_PlayPauseTogglePressed(object sender, object e)
        {
            await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                this.OnPlayPauseTogglePressed();
            });
        }

        private async void MediaControl_PausePressed(object sender, object e)
        {
            await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                this.OnPausePressed();
            });
        }

        private async void MediaControl_NextTrackPressed(object sender, object e)
        {
            await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                this.OnNextTrackPressed();
            });
        }

        private async void MediaControl_FastForwardPressed(object sender, object e)
        {
            await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                this.OnFastForwardPressed();
            });
        }

        private async void MediaControl_SoundLevelChanged(object sender, object e)
        {
            await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                this.OnSoundLevelChanged();
            });
        }

        private async void unMuteButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                this.OnUnMute();
            });
        }

        private async void muteButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                this.OnMute();
            });
        }

        #endregion

        #region MediaElement Event Handlers

        private void mediaElement_VolumeChanged(object sender, RoutedEventArgs e)
        {

        }

        private void mediaElement_SeekCompleted(object sender, RoutedEventArgs e)
        {

        }

        private void mediaElement_MediaOpened(object sender, RoutedEventArgs e)
        {
            // Set playback rates
            this._playRate = this.mediaElement.PlaybackRate;
            this._ffRate = this.mediaElement.PlaybackRate * 10;

            // Set track length
            if (this.mediaElement.NaturalDuration.HasTimeSpan)
            {
                double length = this.mediaElement.NaturalDuration.TimeSpan.TotalSeconds;
                this.positionBar.Maximum = length;

                this.trackLength.Text = string.Format("{0}:{1}:{2}.{3}",
                this.mediaElement.NaturalDuration.TimeSpan.Hours.ToString("00"),
                this.mediaElement.NaturalDuration.TimeSpan.Minutes.ToString("00"),
                this.mediaElement.NaturalDuration.TimeSpan.Seconds.ToString("00"),
                this.mediaElement.NaturalDuration.TimeSpan.Milliseconds.ToString("000"));
            }

            // If track has changed and was playing, play again
            if (this._previousState == MediaElementState.Playing)
                this.OnPlayPressed();
        }

        private void mediaElement_MediaFailed(object sender, ExceptionRoutedEventArgs e)
        {

        }

        private void mediaElement_MediaEnded(object sender, RoutedEventArgs e)
        {
            this.OnNextTrackPressed();
        }

        private void mediaElement_DownloadProgressChanged(object sender, RoutedEventArgs e)
        {

        }

        private void mediaElement_CurrentStateChanged(object sender, RoutedEventArgs e)
        {

        }

        private void mediaElement_BufferingProgressChanged(object sender, RoutedEventArgs e)
        {

        }

        #endregion

        #region Buton Event Handlers

        private void nextTrackButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            this.OnNextTrackPressed();
        }

        private void ffButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            this.OnFastForwardPressed();
        }

        private void pauseButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            this.OnPausePressed();
        }

        private void playButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            this.OnPlayPressed();
        }

        private void stopButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            this.OnStopPressed();
        }

        private void rwButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            this.OnRewindPressed();
        }

        private void prevTrackButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            this.OnPreviousTrackPressed();
        }

        #endregion

        #region Position Bar Event Handlers

        private void positionBar_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            if (!this._updating && this.mediaElement.CanSeek)
                this.mediaElement.Position = TimeSpan.FromSeconds(this.positionBar.Value);
        }

        private void volumeBar_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            this.mediaElement.Volume = (double)(this.volumeBar.Value / 100.0d);
        }

        #endregion

        #region Control Methods

        private void OnStopPressed()
        {
            this.mediaElement.Stop();
            this._isRewinding = false;
            this.ChangeVisualState(AudioPlayerStates.Normal);
            BadgeHelper.UpdateBadge(Glyphs.none);
        }

        private void OnRewindPressed()
        {
            this._isRewinding = true;
            this.ChangeVisualState(AudioPlayerStates.Seeking);
            BadgeHelper.UpdateBadge(Glyphs.activity);
        }

        private void OnPreviousTrackPressed()
        {
            this._isRewinding = false;

            if (this._selectedItem == null || this._playList == null || mediaElement == null)
                return;

            int index = this._playList.IndexOf(this._selectedItem);

            // Previous
            if (index > 0)
                this.SelectedItem = this._playList[index - 1];
            // Repeat last
            else if (index <= 0)
                this.SelectedItem = this._playList[this._playList.Count - 1];
        }

        private void OnPlayPressed()
        {
            this._isRewinding = false;

            this.mediaElement.PlaybackRate = this._playRate;
            this.mediaElement.Play();
            this.ChangeVisualState(AudioPlayerStates.Playing);
            BadgeHelper.UpdateBadge(Glyphs.playing);
        }

        private void OnPlayPauseTogglePressed()
        {
            this._isRewinding = false;

            if (this.mediaElement.CurrentState == MediaElementState.Playing)
                this.OnPausePressed();
            else
                this.OnPlayPressed();
        }

        private void OnPausePressed()
        {
            this._isRewinding = false;

            if (this.mediaElement.CanPause)
            {
                this.mediaElement.Pause();
                this.ChangeVisualState(AudioPlayerStates.Paused);
                BadgeHelper.UpdateBadge(Glyphs.paused);
            }
        }

        private void OnNextTrackPressed()
        {
            this._isRewinding = false;

            if (this._selectedItem == null || this._playList == null || this.mediaElement == null)
                return;

            int index = this._playList.IndexOf(this._selectedItem);

            var state = this.mediaElement.CurrentState;

            // Next
            if (index < this._playList.Count - 1)
                this.SelectedItem = this._playList[index + 1];
            // Repeat first
            else if (index == this._playList.Count - 1 && this._repeatPlayList)
                this.SelectedItem = this._playList[0];


        }

        private void OnFastForwardPressed()
        {
            this._isRewinding = false;
            this.mediaElement.PlaybackRate = this._ffRate;
            this.ChangeVisualState(AudioPlayerStates.Seeking);
            BadgeHelper.UpdateBadge(Glyphs.activity);
        }

        private void OnMute()
        {
            this.mediaElement.IsMuted = true;
            this.muteButton.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            this.unMuteButton.Visibility = Windows.UI.Xaml.Visibility.Visible;
        }

        private void OnUnMute()
        {
            this.mediaElement.IsMuted = false;
            this.muteButton.Visibility = Windows.UI.Xaml.Visibility.Visible;
            this.unMuteButton.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
        }

        private void OnSoundLevelChanged()
        {
            //// Detect mute
            //if (MediaControl.SoundLevel == SoundLevel.Muted)
            //    this.OnMute();
            //else
            //    this.OnUnMute();
        }

        #endregion

        private void _timer_Tick(object sender, object e)
        {
            this._updating = true;

            double pos = this.mediaElement.Position.TotalSeconds;

            // Move back if rewinding
            if (this._isRewinding && this.mediaElement.CanSeek)
            {
                this.mediaElement.Position = TimeSpan.FromSeconds(pos - 10);
            }

            // Update position
            this.positionBar.Value = pos;

            this.trackPosition.Text = string.Format("{0}:{1}:{2}.{3}",
                this.mediaElement.Position.Hours.ToString("00"),
                this.mediaElement.Position.Minutes.ToString("00"),
                this.mediaElement.Position.Seconds.ToString("00"),
                this.mediaElement.Position.Milliseconds.ToString("000"));

            this._updating = false;
        }

        private void ChangeVisualState(AudioPlayerStates state)
        {
            VisualStateManager.GoToState(this, state.ToString(), true);
        }

    }
}
