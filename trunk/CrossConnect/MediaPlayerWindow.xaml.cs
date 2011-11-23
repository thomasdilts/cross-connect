#region Header

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

    using Microsoft.Phone.Controls;

    using readers;

    public partial class MediaPlayerWindow : ITiledWindow
    {
        #region Fields

        private WebClient _client;
        private bool _isFirstProgressGotten;
        private bool _isLoaded;
        private bool _isPlaying = true;
        private SerializableWindowState _state = new SerializableWindowState();

        #endregion Fields

        #region Constructors

        public MediaPlayerWindow()
        {
            InitializeComponent();
        }

        #endregion Constructors

        #region Events

        // An event that clients can use to be notified whenever the
        // elements of the list change.
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
            get { return _state; }
            set { _state = value; }
        }

        #endregion Properties

        #region Methods

        public void CalculateTitleTextWidth()
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
                title.Width = Application.Current.Host.Content.ActualHeight - butClose.Width*numButtonsShowing - 15 -
                              butClose.Width*2;
                title.MaxWidth = title.Width;
            }
            else
            {
                title.Width = Application.Current.Host.Content.ActualWidth - butClose.Width*numButtonsShowing - 15;
                title.MaxWidth = title.Width;
            }
        }

        public void ShowSizeButtons(bool isShow = true)
        {
            if (!isShow)
            {
                SetButtonVisibility(butLarger, false, "", "");
                SetButtonVisibility(butSmaller, false, "", "");
                SetButtonVisibility(butClose, false, "", "");
            }
            else
            {

                string colorDir = App.Themes.IsButtonColorDark ? "light" : "dark";

                SetButtonVisibility(butSmaller, _state.NumRowsIown > 1, "/Images/" + colorDir + "/appbar.minus.rest.png",
                                    "/Images/" + colorDir + "/appbar.minus.rest.pressed.png");
                SetButtonVisibility(butLarger, true, "/Images/" + colorDir + "/appbar.feature.search.rest.png",
                                    "/Images/" + colorDir + "/appbar.feature.search.rest.pressed.png");
                SetButtonVisibility(butClose, true, "/Images/" + colorDir + "/appbar.cancel.rest.png",
                                    "/Images/" + colorDir + "/appbar.cancel.rest.pressed.png");
            }
            CalculateTitleTextWidth();
        }

        public void SynchronizeWindow(int chapterNum, int verseNum)
        {
        }

        public void UpdateBrowser(bool isOrientationChangeOnly)
        {
            border1.BorderBrush = new SolidColorBrush(App.Themes.BorderColor);
            WebBrowserBorder.BorderBrush = border1.BorderBrush;
            grid1.Background = new SolidColorBrush(App.Themes.TitleBackColor);
            title.Foreground = new SolidColorBrush(App.Themes.TitleFontColor);
            if (App.Themes.IsMainBackImage && !string.IsNullOrEmpty(App.Themes.MainBackImage))
            {
                //read from isolated storage.
                using (var isolatedStorageRoot = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    if (isolatedStorageRoot.FileExists(App.WebDirIsolated + "/images/" + App.Themes.MainBackImage))
                    {
                        try
                        {
                            using (IsolatedStorageFileStream fStream =
                                isolatedStorageRoot.OpenFile(App.WebDirIsolated + "/images/" + App.Themes.MainBackImage, FileMode.Open))
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
                                var imageBrush = new ImageBrush
                                {
                                    ImageSource = bitImage,
                                };
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
            if (parent.Orientation == PageOrientation.Landscape
                || parent.Orientation == PageOrientation.LandscapeLeft
                || parent.Orientation == PageOrientation.LandscapeRight)
            {
                GridProgressBars.Width = Application.Current.Host.Content.ActualHeight - txtPosition.Width * 2 - 15-70;
            }
            else
            {
                GridProgressBars.Width = Application.Current.Host.Content.ActualWidth - txtPosition.Width * 2 - 15;
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

        private void ButPlayPauseClick(object sender, RoutedEventArgs e)
        {
            SetPlayPauseButton(!_isPlaying);
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
            }
            catch (Exception ee)
            {
                Debug.WriteLine("IconDownloadingComplete error = " + ee);
            }
        }

        private void MediaPlayerDownloadProgressChanged(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!_isFirstProgressGotten)
                {
                    _isFirstProgressGotten = true;
                    MediaPlayer.Markers.Clear();
                    var duration = (int)MediaPlayer.NaturalDuration.TimeSpan.TotalSeconds;
                    for (int i = 0; i < duration; i += 2)
                    {
                        MediaPlayer.Markers.Add(new TimelineMarker { Time = new TimeSpan(0, 0, 0, i) });
                    }
                    WaitingForDownload.Visibility = Visibility.Collapsed;
                    butPlayPause.Visibility = Visibility.Visible;
                    stackContent.Visibility = Visibility.Visible;
                    SetPlayPauseButton(true);
                }
                ProgressDownload.Value = 100 * MediaPlayer.DownloadProgress;
            }
            catch (Exception)
            {
                if (MediaPlayer != null)
                {
                    MediaPlayer.Stop();
                }
            }
        }

        private void MediaPlayerLoaded(object sender, RoutedEventArgs e)
        {
            UpdateBrowser(false);
            if (_isLoaded) return;
            _isLoaded = true;

            butPlayPause.Visibility = Visibility.Collapsed;
            stackContent.Visibility = Visibility.Collapsed;
            WaitingForDownload.Visibility= Visibility.Visible;
            //try to load the icon.
            if (!string.IsNullOrEmpty(((MediaReader)State.Source).Icon))
            {
                try
                {
                    _client = new WebClient();
                    _client.OpenReadCompleted += IconDownloadingComplete;
                    _client.OpenReadAsync(new Uri(((MediaReader)State.Source).Icon));
                }
                catch (Exception ee)
                {
                    Debug.WriteLine("Starting icon download error = " + ee);
                }
            }
            try
            {
                string url = ((MediaReader)State.Source).Link;
                title.Text = ((MediaReader)State.Source).TitleBar;
                MediaPlayer.Source = new Uri(url);
            }
            catch (Exception ee)
            {
                Debug.WriteLine("MediaPlayer.Source error = " + ee);
                WaitingForDownload.Visibility = Visibility.Collapsed;
            }
        }

        private void MediaPlayerMarkerReached(object sender, TimelineMarkerRoutedEventArgs e)
        {
            try
            {
                ProgressPosition.Value = 100*
                                         (MediaPlayer.Position.TotalMinutes/
                                          MediaPlayer.NaturalDuration.TimeSpan.TotalMinutes);
                txtPosition.Text = MediaPlayer.Position.ToString("c").Substring(3, 5);
            }catch(Exception)
            {
                if(MediaPlayer!=null)
                {
                    MediaPlayer.Stop();
                }
            }
        }

        private void MediaPlayerMediaEnded(object sender, RoutedEventArgs e)
        {
            SetPlayPauseButton(false);
        }

        private void MediaPlayerMediaFailed(object sender, ExceptionRoutedEventArgs e)
        {
            WaitingForDownload.Visibility = Visibility.Collapsed;
            butPlayPause.Visibility = Visibility.Collapsed;
            stackContent.Visibility = Visibility.Collapsed;
            SetPlayPauseButton(false);
        }

        private void MediaPlayerMediaOpened(object sender, RoutedEventArgs e)
        {
            txtDuration.Text = MediaPlayer.NaturalDuration.TimeSpan.ToString("c").Substring(3, 5);
        }

        private void SetPlayPauseButton(bool isPlaying)
        {
            butPlayPause.Visibility= Visibility.Visible;
            if (isPlaying)
            {
                MediaPlayer.Play();
                MediaPlayer.Markers.Clear();
                var duration = (int)MediaPlayer.NaturalDuration.TimeSpan.TotalSeconds;
                for (int i = 0; i < duration; i += 2)
                {
                    MediaPlayer.Markers.Add(new TimelineMarker { Time = new TimeSpan(0, 0, 0, i) });
                }
            }
            else
            {
                MediaPlayer.Pause();
            }
            string colorDir = App.Themes.IsButtonColorDark ? "light" : "dark";
            butPlayPause.Image = GetImage(!isPlaying ? "/Images/" + colorDir + "/appbar.transport.play.rest.png" : "/Images/" + colorDir + "/appbar.transport.pause.rest.png");
            butPlayPause.PressedImage = GetImage(!isPlaying ? "/Images/" + colorDir + "/appbar.transport.play.rest.pressed.png" : "/Images/" + colorDir + "/appbar.transport.pause.rest.pressed.png");
            _isPlaying = isPlaying;
        }

        #endregion Methods
    }
}