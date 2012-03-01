// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MediaPlayerWindow.xaml.cs" company="">
//   
// </copyright>
// <summary>
//   The media player window.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

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
    using System.Net;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;

    using CrossConnect.readers;

    using Microsoft.Phone.Controls;

    /// <summary>
    /// The media player window.
    /// </summary>
    public partial class MediaPlayerWindow : ITiledWindow
    {
        #region Constants and Fields

        /// <summary>
        /// The _client.
        /// </summary>
        private WebClient _client;

        /// <summary>
        /// The _is first progress gotten.
        /// </summary>
        private bool _isFirstProgressGotten;

        /// <summary>
        /// The _is loaded.
        /// </summary>
        private bool _isLoaded;

        /// <summary>
        /// The _is playing.
        /// </summary>
        private bool _isPlaying = true;

        /// <summary>
        /// The _state.
        /// </summary>
        private SerializableWindowState _state = new SerializableWindowState();

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MediaPlayerWindow"/> class.
        /// </summary>
        public MediaPlayerWindow()
        {
            this.InitializeComponent();
        }

        #endregion

        // An event that clients can use to be notified whenever the
        // elements of the list change.
        #region Public Events

        /// <summary>
        /// The hit button bigger.
        /// </summary>
        public event EventHandler HitButtonBigger;

        /// <summary>
        /// The hit button close.
        /// </summary>
        public event EventHandler HitButtonClose;

        /// <summary>
        /// The hit button smaller.
        /// </summary>
        public event EventHandler HitButtonSmaller;

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets a value indicating whether ForceReload.
        /// </summary>
        public bool ForceReload { get; set; }

        /// <summary>
        /// Gets or sets State.
        /// </summary>
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

        /// <summary>
        /// The calculate title text width.
        /// </summary>
        public void CalculateTitleTextWidth()
        {
            int numButtonsShowing = 0;
            if (this.butLarger.Visibility == Visibility.Visible)
            {
                numButtonsShowing++;
            }

            if (this.butSmaller.Visibility == Visibility.Visible)
            {
                numButtonsShowing++;
            }

            if (this.butClose.Visibility == Visibility.Visible)
            {
                numButtonsShowing++;
            }

            var parent = (MainPageSplit)((Grid)((Grid)((Grid)this.Parent).Parent).Parent).Parent;
            if (parent.Orientation == PageOrientation.Landscape || parent.Orientation == PageOrientation.LandscapeLeft
                || parent.Orientation == PageOrientation.LandscapeRight)
            {
                this.title.Width = Application.Current.Host.Content.ActualHeight
                                   - this.butClose.Width * numButtonsShowing - 15 - this.butClose.Width * 2;
                this.title.MaxWidth = this.title.Width;
            }
            else
            {
                this.title.Width = Application.Current.Host.Content.ActualWidth
                                   - this.butClose.Width * numButtonsShowing - 15;
                this.title.MaxWidth = this.title.Width;
            }
        }

        /// <summary>
        /// The show size buttons.
        /// </summary>
        /// <param name="isShow">
        /// The is show.
        /// </param>
        public void ShowSizeButtons(bool isShow = true)
        {
            if (!isShow)
            {
                SetButtonVisibility(this.butLarger, false, string.Empty, string.Empty);
                SetButtonVisibility(this.butSmaller, false, string.Empty, string.Empty);
                SetButtonVisibility(this.butClose, false, string.Empty, string.Empty);
            }
            else
            {
                string colorDir = App.Themes.IsButtonColorDark ? "light" : "dark";

                SetButtonVisibility(
                    this.butSmaller, 
                    this._state.NumRowsIown > 1, 
                    "/Images/" + colorDir + "/appbar.minus.rest.png", 
                    "/Images/" + colorDir + "/appbar.minus.rest.pressed.png");
                SetButtonVisibility(
                    this.butLarger, 
                    true, 
                    "/Images/" + colorDir + "/appbar.feature.search.rest.png", 
                    "/Images/" + colorDir + "/appbar.feature.search.rest.pressed.png");
                SetButtonVisibility(
                    this.butClose, 
                    true, 
                    "/Images/" + colorDir + "/appbar.cancel.rest.png", 
                    "/Images/" + colorDir + "/appbar.cancel.rest.pressed.png");
            }

            this.CalculateTitleTextWidth();
        }

        /// <summary>
        /// The synchronize window.
        /// </summary>
        /// <param name="chapterNum">
        /// The chapter num.
        /// </param>
        /// <param name="verseNum">
        /// The verse num.
        /// </param>
        public void SynchronizeWindow(int chapterNum, int verseNum)
        {
        }

        /// <summary>
        /// The update browser.
        /// </summary>
        /// <param name="isOrientationChangeOnly">
        /// The is orientation change only.
        /// </param>
        public void UpdateBrowser(bool isOrientationChangeOnly)
        {
            this.border1.BorderBrush = new SolidColorBrush(App.Themes.BorderColor);
            this.WebBrowserBorder.BorderBrush = this.border1.BorderBrush;
            this.grid1.Background = new SolidColorBrush(App.Themes.TitleBackColor);
            this.title.Foreground = new SolidColorBrush(App.Themes.TitleFontColor);
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
                                IsolatedStorageFileStream fStream =
                                    isolatedStorageRoot.OpenFile(
                                        App.WebDirIsolated + "/images/" + App.Themes.MainBackImage, FileMode.Open))
                            {
                                var buffer = new byte[10000];
                                int len;
                                var ms = new MemoryStream();
                                while ((len = fStream.Read(buffer, 0, buffer.GetUpperBound(0))) > 0)
                                {
                                    ms.Write(buffer, 0, len);
                                }

                                fStream.Close();
                                ms.Position = 0;
                                var bitImage = new BitmapImage();
                                bitImage.SetSource(ms);
                                var imageBrush = new ImageBrush { ImageSource = bitImage, };
                                this.AllContent.Background = imageBrush;
                            }
                        }
                        catch (Exception ee)
                        {
                            Debug.WriteLine(ee);
                            this.AllContent.Background = new SolidColorBrush(App.Themes.MainBackColor);
                        }
                    }
                }
            }
            else
            {
                this.AllContent.Background = new SolidColorBrush(App.Themes.MainBackColor);
            }

            this.ProgressPosition.Foreground = new SolidColorBrush(App.Themes.AccentColor);
            this.WaitingForDownload.Foreground = new SolidColorBrush(App.Themes.AccentColor);
            this.txtDuration.Foreground = new SolidColorBrush(App.Themes.MainFontColor);
            this.txtPosition.Foreground = new SolidColorBrush(App.Themes.MainFontColor);
            var parent = (MainPageSplit)((Grid)((Grid)((Grid)this.Parent).Parent).Parent).Parent;
            if (parent.Orientation == PageOrientation.Landscape || parent.Orientation == PageOrientation.LandscapeLeft
                || parent.Orientation == PageOrientation.LandscapeRight)
            {
                this.GridProgressBars.Width = Application.Current.Host.Content.ActualHeight - this.txtPosition.Width * 2
                                              - 15 - 70;
            }
            else
            {
                this.GridProgressBars.Width = Application.Current.Host.Content.ActualWidth - this.txtPosition.Width * 2
                                              - 15;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// The get image.
        /// </summary>
        /// <param name="path">
        /// The path.
        /// </param>
        /// <returns>
        /// </returns>
        private static ImageSource GetImage(string path)
        {
            var uri = new Uri(path, UriKind.Relative);
            return new BitmapImage(uri);
        }

        /// <summary>
        /// The set button visibility.
        /// </summary>
        /// <param name="but">
        /// The but.
        /// </param>
        /// <param name="isVisible">
        /// The is visible.
        /// </param>
        /// <param name="image">
        /// The image.
        /// </param>
        /// <param name="pressedImage">
        /// The pressed image.
        /// </param>
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

        /// <summary>
        /// The but close click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void ButCloseClick(object sender, RoutedEventArgs e)
        {
            if (this.HitButtonClose != null)
            {
                this.HitButtonClose(this, e);
            }
        }

        /// <summary>
        /// The but larger click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void ButLargerClick(object sender, RoutedEventArgs e)
        {
            this._state.NumRowsIown++;
            this.ShowSizeButtons();
            if (this.HitButtonBigger != null)
            {
                this.HitButtonBigger(this, e);
            }
        }

        /// <summary>
        /// The but play pause click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void ButPlayPauseClick(object sender, RoutedEventArgs e)
        {
            this.SetPlayPauseButton(!this._isPlaying);
        }

        /// <summary>
        /// The but smaller click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void ButSmallerClick(object sender, RoutedEventArgs e)
        {
            this._state.NumRowsIown--;
            this.ShowSizeButtons();
            if (this.HitButtonSmaller != null)
            {
                this.HitButtonSmaller(this, e);
            }
        }

        /// <summary>
        /// The icon downloading complete.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void IconDownloadingComplete(object sender, OpenReadCompletedEventArgs e)
        {
            try
            {
                var bitImage = new BitmapImage();
                bitImage.SetSource(e.Result);
                this.ImageIcon.Source = bitImage;
                this.ImageIcon.Width = bitImage.PixelWidth;
                this.ImageIcon.Height = bitImage.PixelHeight;
            }
            catch (Exception ee)
            {
                Debug.WriteLine("IconDownloadingComplete error = " + ee);
            }
        }

        /// <summary>
        /// The media player download progress changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void MediaPlayerDownloadProgressChanged(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!this._isFirstProgressGotten)
                {
                    this._isFirstProgressGotten = true;
                    this.MediaPlayer.Markers.Clear();
                    var duration = (int)this.MediaPlayer.NaturalDuration.TimeSpan.TotalSeconds;
                    for (int i = 0; i < duration; i += 2)
                    {
                        this.MediaPlayer.Markers.Add(new TimelineMarker { Time = new TimeSpan(0, 0, 0, i) });
                    }

                    this.WaitingForDownload.Visibility = Visibility.Collapsed;
                    this.butPlayPause.Visibility = Visibility.Visible;
                    this.stackContent.Visibility = Visibility.Visible;
                    this.SetPlayPauseButton(true);
                }

                this.ProgressDownload.Value = 100 * this.MediaPlayer.DownloadProgress;
            }
            catch (Exception)
            {
                if (this.MediaPlayer != null)
                {
                    this.MediaPlayer.Stop();
                }
            }
        }

        /// <summary>
        /// The media player loaded.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void MediaPlayerLoaded(object sender, RoutedEventArgs e)
        {
            this.UpdateBrowser(false);
            if (this._isLoaded)
            {
                return;
            }

            this._isLoaded = true;

            this.butPlayPause.Visibility = Visibility.Collapsed;
            this.stackContent.Visibility = Visibility.Collapsed;
            this.WaitingForDownload.Visibility = Visibility.Visible;

            // try to load the icon.
            if (!string.IsNullOrEmpty(((MediaReader)this.State.Source).Icon))
            {
                try
                {
                    this._client = new WebClient();
                    this._client.OpenReadCompleted += this.IconDownloadingComplete;
                    this._client.OpenReadAsync(new Uri(((MediaReader)this.State.Source).Icon));
                }
                catch (Exception ee)
                {
                    Debug.WriteLine("Starting icon download error = " + ee);
                }
            }

            try
            {
                string url = ((MediaReader)this.State.Source).Link;
                this.title.Text = ((MediaReader)this.State.Source).TitleBar;
                this.MediaPlayer.Source = new Uri(url);
            }
            catch (Exception ee)
            {
                Debug.WriteLine("MediaPlayer.Source error = " + ee);
                this.WaitingForDownload.Visibility = Visibility.Collapsed;
            }
        }

        /// <summary>
        /// The media player marker reached.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void MediaPlayerMarkerReached(object sender, TimelineMarkerRoutedEventArgs e)
        {
            try
            {
                this.ProgressPosition.Value = 100
                                              *
                                              (this.MediaPlayer.Position.TotalMinutes
                                               / this.MediaPlayer.NaturalDuration.TimeSpan.TotalMinutes);
                this.txtPosition.Text = this.MediaPlayer.Position.ToString("c").Substring(3, 5);
            }
            catch (Exception)
            {
                if (this.MediaPlayer != null)
                {
                    this.MediaPlayer.Stop();
                }
            }
        }

        /// <summary>
        /// The media player media ended.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void MediaPlayerMediaEnded(object sender, RoutedEventArgs e)
        {
            this.SetPlayPauseButton(false);
        }

        /// <summary>
        /// The media player media failed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void MediaPlayerMediaFailed(object sender, ExceptionRoutedEventArgs e)
        {
            this.WaitingForDownload.Visibility = Visibility.Collapsed;
            this.butPlayPause.Visibility = Visibility.Collapsed;
            this.stackContent.Visibility = Visibility.Collapsed;
            this.SetPlayPauseButton(false);
        }

        /// <summary>
        /// The media player media opened.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void MediaPlayerMediaOpened(object sender, RoutedEventArgs e)
        {
            this.txtDuration.Text = this.MediaPlayer.NaturalDuration.TimeSpan.ToString("c").Substring(3, 5);
        }

        /// <summary>
        /// The set play pause button.
        /// </summary>
        /// <param name="isPlaying">
        /// The is playing.
        /// </param>
        private void SetPlayPauseButton(bool isPlaying)
        {
            this.butPlayPause.Visibility = Visibility.Visible;
            if (isPlaying)
            {
                this.MediaPlayer.Play();
                this.MediaPlayer.Markers.Clear();
                var duration = (int)this.MediaPlayer.NaturalDuration.TimeSpan.TotalSeconds;
                for (int i = 0; i < duration; i += 2)
                {
                    this.MediaPlayer.Markers.Add(new TimelineMarker { Time = new TimeSpan(0, 0, 0, i) });
                }
            }
            else
            {
                this.MediaPlayer.Pause();
            }

            string colorDir = App.Themes.IsButtonColorDark ? "light" : "dark";
            this.butPlayPause.Image =
                GetImage(
                    !isPlaying
                        ? "/Images/" + colorDir + "/appbar.transport.play.rest.png"
                        : "/Images/" + colorDir + "/appbar.transport.pause.rest.png");
            this.butPlayPause.PressedImage =
                GetImage(
                    !isPlaying
                        ? "/Images/" + colorDir + "/appbar.transport.play.rest.pressed.png"
                        : "/Images/" + colorDir + "/appbar.transport.pause.rest.pressed.png");
            this._isPlaying = isPlaying;
        }

        #endregion
    }
}