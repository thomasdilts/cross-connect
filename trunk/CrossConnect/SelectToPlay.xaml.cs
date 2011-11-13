#region Header

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
// Email: thomas@chaniel.se
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

    using CrossConnect.readers;

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

        #region Enumerations

        public enum MediaType
        {
            MediaPlayer,
            MediaExplorer
        }

        #endregion Enumerations

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
                                                foundMedia.Viewer = reader.Value;
                                                break;
                                            case "player":
                                                if (reader.Value.Equals("explorer"))
                                                {
                                                    foundMedia.Player = MediaType.MediaExplorer;
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
            }
        }

        private void PhoneApplicationPageLoaded(object sender, RoutedEventArgs e)
        {
            PageTitle.Text = Translations.Translate("Select what you want to hear");
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
            var info = (MediaInfo) ((TextBlock) e.AddedItems[0]).Tag;

            App.AddMediaWindow(info.Src, _titleBar, info.Icon);
            _isInSelectionChanged = false;
            if (NavigationService.CanGoBack)
            {
                NavigationService.GoBack();
            }
        }

        #endregion Methods

        #region Nested Types

        public class MediaInfo
        {
            #region Fields

            public string Icon = string.Empty;
            public MediaType Player = MediaType.MediaPlayer;
            public string Src = string.Empty;
            public string Viewer = string.Empty;

            #endregion Fields
        }

        #endregion Nested Types
    }
}