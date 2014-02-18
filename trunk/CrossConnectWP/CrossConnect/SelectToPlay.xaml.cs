#region Header

// <copyright file="SelectToPlay.xaml.cs" company="Thomas Dilts">
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
    using System.Linq;
    using System.Net;
    using System.Windows;
    using System.Windows.Controls;
    using readers;
    using Microsoft.Phone.Shell;
    using AudioPlaybackAgent1;
    using Sword.versification;

    /// <summary>
    /// The select to play.
    /// </summary>
    public partial class SelectToPlay
    {
        #region Fields

        /// <summary>
        /// The _title bar.
        /// </summary>
        private int _chapter;
        private string _book;

        /// <summary>
        /// The _client.
        /// </summary>
        private WebClient _client = new WebClient();

        /// <summary>
        /// The _is in selection changed.
        /// </summary>
        private bool _isInSelectionChanged;

        /// <summary>
        /// The _title bar.
        /// </summary>
        private string _language;

        /// <summary>
        /// The _title bar.
        /// </summary>
        private string _titleBar = string.Empty;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SelectToPlay"/> class.
        /// </summary>
        public SelectToPlay()
        {
            InitializeComponent();
        }

        #endregion Constructors

        #region Methods

        private static void ClientDownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            // nothing to do here
        }

        private void ClientOpenReadCompleted(object sender, OpenReadCompletedEventArgs e)
        {
            try
            {
                string msg;
                var mediaList = AudioPlayer.ReadMediaSourcesFile(e.Result, out msg);
                MsgFromServer.Text = msg;

                foreach (var block in mediaList.Select(mediaInfo => new TextBlock
                    {
                        Text = mediaInfo.Name,
                        Style = PageTitle.Style,
                        Tag = mediaInfo,
                        Margin = new Thickness(0, 0, 0, 10),
                        TextWrapping = TextWrapping.Wrap
                    }))
                {
                    SelectList.Items.Add(block);
                }
            }
            catch (Exception exp)
            {
                Logger.Fail(e.ToString());
                MessageBox.Show(
                    Translations.Translate("An error occurred trying to connect to the network. Try again later.")
                    + "; " + exp.Message);
                if (NavigationService.CanGoBack)
                {
                    NavigationService.GoBack();
                }
            }

            MsgFromServer.Visibility = string.IsNullOrEmpty(MsgFromServer.Text)
                                                ? Visibility.Collapsed
                                                : Visibility.Visible;
            WaitingForDownload.Visibility = Visibility.Collapsed;
        }

        private void PhoneApplicationPageLoaded(object sender, RoutedEventArgs e)
        {
            PageTitle.Text = Translations.Translate("Select what you want to hear");
            WaitingForDownload.Visibility = Visibility.Visible;
            MsgFromServer.Visibility = Visibility.Collapsed;
            MsgFromServer.Text = string.Empty;
            SelectList.Items.Clear();

            // do a download.
            object BookToHear;
            object chapterToHear;
            object language;
            if (PhoneApplicationService.Current.State.TryGetValue("ChapterToHear", out chapterToHear)
                && PhoneApplicationService.Current.State.TryGetValue("BookToHear", out BookToHear)
                && PhoneApplicationService.Current.State.TryGetValue("ChapterToHearLanguage", out language))
            {
                object titleBar;
                PhoneApplicationService.Current.State.TryGetValue("titleBar", out titleBar);
                if (titleBar == null)
                {
                    titleBar = string.Empty;
                }

                _book = (string)BookToHear;
                _chapter = (int)chapterToHear;
                var canon = CanonManager.GetCanon("KJV");
                CanonBookDef book;
                if(canon.BookByShortName.TryGetValue(_book,out book))
                {
                    _language = (string)language;
                    _titleBar = (string)titleBar;
                    string url = string.Format(
                        App.DisplaySettings.SoundLink, book.VersesInChapterStartIndex + _chapter, language);
                    try
                    {
                        var source = new Uri(url);

                        _client = new WebClient();
                        _client.DownloadProgressChanged += ClientDownloadProgressChanged;
                        _client.OpenReadCompleted += ClientOpenReadCompleted;
                        Logger.Debug("download start");
                        _client.OpenReadAsync(source);
                        Logger.Debug("DownloadStringAsync returned");
                    }
                    catch (Exception eee)
                    {
                        Logger.Fail(eee.ToString());
                        MessageBox.Show(
                            Translations.Translate("An error occurred trying to connect to the network. Try again later.")
                            + "; " + eee.Message);
                        if (NavigationService.CanGoBack)
                        {
                            NavigationService.GoBack();
                        }
                    }
                }
            }
        }

        private void SelectListSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_isInSelectionChanged)
            {
                return;
            }

            _isInSelectionChanged = true;

            // clear the selection because we might come here again after the media player
            SelectList.SelectedIndex = -1;

            var info = (AudioPlayer.MediaInfo)((TextBlock)e.AddedItems[0]).Tag;
            object BookToHear;
            object chapterToHear;
            object language;
            if (PhoneApplicationService.Current.State.TryGetValue("ChapterToHear", out chapterToHear)
                && PhoneApplicationService.Current.State.TryGetValue("BookToHear", out BookToHear)
                && PhoneApplicationService.Current.State.TryGetValue("ChapterToHearLanguage", out language))
            {
                info.Book = (string)BookToHear;
                info.Chapter = (int)chapterToHear;
            }
            if (NavigationService.CanGoBack)
            {
                NavigationService.GoBack();
            }

            App.AddMediaWindow(info);
            _isInSelectionChanged = false;
        }

        #endregion Methods
    }
}