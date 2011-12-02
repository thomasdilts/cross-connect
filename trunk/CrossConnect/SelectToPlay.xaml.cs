﻿#region Header

// <copyright file="SelectToPlay.xaml.cs" company="Thomas Dilts">
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
    using System.Net;
    using System.Windows;
    using System.Windows.Controls;
    using System.Xml;

    using Microsoft.Phone.Shell;
    using Microsoft.Phone.Tasks;

    public partial class SelectToPlay
    {
        #region Fields

        private WebClient _client = new WebClient();
        private bool _isInSelectionChanged;
        private string _titleBar = "";

        #endregion Fields

        #region Constructors

        public SelectToPlay()
        {
            InitializeComponent();
        }

        #endregion Constructors

        #region Methods

        private static void ClientDownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            //nothing to do here
        }

        private void ClientOpenReadCompleted(object sender, OpenReadCompletedEventArgs e)
        {
            try
            {
                // for debug
                //byte[] buffer=new byte[e.Result.Length];
                //e.Result.Read(buffer, 0, (int)e.Result.Length);
                //System.Diagnostics.Debug.WriteLine("RawFile: " + System.Text.UTF8Encoding.UTF8.GetString(buffer, 0, (int)e.Result.Length));

                using (XmlReader reader = XmlReader.Create(e.Result))
                {
                    string name = string.Empty;
                    MediaInfo foundMedia = null;
                    MsgFromServer.Text = "";
                    // Parse the file and get each of the nodes.
                    while (reader.Read())
                    {
                        switch (reader.NodeType)
                        {
                            case XmlNodeType.Element:
                                if (reader.Name.ToLower().Equals("source") && reader.HasAttributes)
                                {
                                    foundMedia = new MediaInfo();
                                    name = string.Empty;
                                    reader.MoveToFirstAttribute();
                                    do
                                    {
                                        switch (reader.Name.ToLower())
                                        {
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
                                    } while (reader.MoveToNextAttribute());
                                }
                                else if (reader.Name.ToLower().Equals("message"))
                                {
                                    name = string.Empty;
                                }
                                break;
                            case XmlNodeType.EndElement:
                                if (reader.Name.ToLower().Equals("source") && foundMedia != null &&
                                    !string.IsNullOrEmpty(foundMedia.Src) && !string.IsNullOrEmpty(name))
                                {
                                    var block = new TextBlock
                                                    {
                                                        Text = name,
                                                        Style = PageTitle.Style,
                                                        Tag = foundMedia,
                                                        Margin = new Thickness(0, 0, 0, 10),
                                                        TextWrapping = TextWrapping.Wrap
                                                    };
                                    SelectList.Items.Add(block);
                                    foundMedia = null;
                                }
                                else if (reader.Name.ToLower().Equals("message"))
                                {
                                    MsgFromServer.Text = name;
                                    name = string.Empty;
                                }
                                break;
                            case XmlNodeType.Text:
                                name += reader.Value;
                                break;
                        }
                    }
                }
            }
            catch (Exception exp)
            {
                Logger.Fail(e.ToString());
                MessageBox.Show(
                    Translations.Translate("An error occurred trying to connect to the network. Try again later.") +
                    "; " + exp.Message);
                if (NavigationService.CanGoBack)
                {
                    NavigationService.GoBack();
                }
            }
            MsgFromServer.Visibility = string.IsNullOrEmpty(MsgFromServer.Text) ? Visibility.Collapsed : Visibility.Visible;
            WaitingForDownload.Visibility = Visibility.Collapsed;
            useMediaPlayer.Visibility = Visibility.Visible;
        }

        private void PhoneApplicationPageLoaded(object sender, RoutedEventArgs e)
        {
            PageTitle.Text = Translations.Translate("Select what you want to hear");
            WaitingForDownload.Visibility = Visibility.Visible;
            useMediaPlayer.Visibility = Visibility.Collapsed;
            MsgFromServer.Visibility = Visibility.Collapsed;
            MsgFromServer.Text = string.Empty;
            SelectList.Items.Clear();
            //do a download.
            object chapterToHear;
            if (PhoneApplicationService.Current.State.TryGetValue("ChapterToHear", out chapterToHear))
            {
                object titleBar;
                PhoneApplicationService.Current.State.TryGetValue("titleBar", out titleBar);
                if (titleBar == null)
                {
                    titleBar = "";
                }
                _titleBar = (string) titleBar;
                string url = string.Format(App.DisplaySettings.SoundLink, (int) chapterToHear,
                                           Translations.IsoLanguageCode);
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
                        Translations.Translate("An error occurred trying to connect to the network. Try again later.") +
                        "; " + eee.Message);
                    if (NavigationService.CanGoBack)
                    {
                        NavigationService.GoBack();
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
            //clear the selection because we might come here again after the media player
            SelectList.SelectedIndex = -1;
            //get what player was selected
            var info = (MediaInfo) ((TextBlock) e.AddedItems[0]).Tag;
            if(useMediaPlayer.IsChecked != null && (bool) useMediaPlayer.IsChecked)
            {
                var mediaPlayerLauncher = new MediaPlayerLauncher
                    {
                        Media = new Uri(info.Src, UriKind.Absolute)
                    };
                mediaPlayerLauncher.Show();
            }
            else
            {
                App.AddMediaWindow(info.Src, _titleBar, info.Icon);
                if (NavigationService.CanGoBack)
                {
                    NavigationService.GoBack();
                }
            }

            _isInSelectionChanged = false;
        }

        #endregion Methods

        #region Nested Types

        private class MediaInfo
        {
            #region Fields

            public string Icon = string.Empty;
            public string Src = string.Empty;

            #endregion Fields
        }

        #endregion Nested Types
    }
}