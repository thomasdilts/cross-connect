/// <summary>
/// Distribution License:
/// CrossConnect is free software; you can redistribute it and/or modify it under
/// the terms of the GNU General Public License, version 3 as published by
/// the Free Software Foundation. This program is distributed in the hope
/// that it will be useful, but WITHOUT ANY WARRANTY; without even the
/// implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
/// See the GNU General Public License for more details.
///
/// The License is available on the internet at:
///       http://www.gnu.org/copyleft/gpl.html
/// or by writing to:
///      Free Software Foundation, Inc.
///      59 Temple Place - Suite 330
///      Boston, MA 02111-1307, USA
/// </summary>
/// <copyright file="SelectToPlay.xaml.cs" company="Thomas Dilts">
///     Thomas Dilts. All rights reserved.
/// </copyright>
/// <author>Thomas Dilts</author>
namespace CrossConnect
{
    using System;
    using System.Net;
    using System.Windows;
    using System.Windows.Controls;
    using System.Xml;

    using Microsoft.Phone.Shell;
    using Microsoft.Phone.Tasks;

    public partial class SelectToPlay : AutoRotatePage
    {
        #region Fields

        private WebClient client = new WebClient();
        private bool isInSelectionChanged = false;

        #endregion Fields

        #region Constructors

        public SelectToPlay()
        {
            InitializeComponent();
        }

        #endregion Constructors

        #region Methods

        private void client_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            //nothing to do here
        }

        private void client_OpenReadCompleted(object sender, OpenReadCompletedEventArgs e)
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
                    this.MsgFromServer.Text = "";
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
                                                foundMedia.src = reader.Value;
                                                break;
                                            case "icon":
                                                foundMedia.icon = reader.Value;
                                                break;
                                            case "viewer":
                                                foundMedia.viewer = reader.Value;
                                                break;
                                            case "player":
                                                if (reader.Value.Equals("explorer"))
                                                {
                                                    foundMedia.player = MediaType.MEDIA_EXPLORER;
                                                }
                                                break;
                                        }
                                    }
                                    while (reader.MoveToNextAttribute());
                                }
                                else if (reader.Name.ToLower().Equals("message"))
                                {
                                    name = string.Empty;
                                }
                                break;
                            case XmlNodeType.EndElement:
                                if (reader.Name.ToLower().Equals("source") && foundMedia!=null && !string.IsNullOrEmpty(foundMedia.src) && !string.IsNullOrEmpty(name))
                                {
                                    TextBlock block = new TextBlock();
                                    block.Text = name;
                                    block.Style = PageTitle.Style;
                                    foundMedia.visualText = name;
                                    block.Tag = foundMedia;
                                    block.Margin = new Thickness(0,0,0,10);
                                    block.TextWrapping = TextWrapping.Wrap;
                                    SelectList.Items.Add(block);
                                    foundMedia = null;
                                }
                                else if (reader.Name.ToLower().Equals("message"))
                                {
                                    this.MsgFromServer.Text = name;
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
                MessageBox.Show(Translations.translate("An error occurred trying to connect to the network. Try again later.") + "; " + exp.Message);
            }
        }

        private string titleBar = "";

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            PageTitle.Text = Translations.translate("Select what you want to hear");
            SelectList.Items.Clear();
            //do a download.
            object chapterToHear;
            if (PhoneApplicationService.Current.State.TryGetValue("ChapterToHear", out chapterToHear))
            {
                object titleBar;
                PhoneApplicationService.Current.State.TryGetValue("titleBar", out titleBar);
                if (titleBar==null)
                {
                    titleBar = "";
                }
                this.titleBar = (string)titleBar;
                string url = string.Format(App.displaySettings.soundLink, (int)chapterToHear, Translations.isoLanguageCode);
                try
                {
                    Uri source = new Uri(url);

                    client = new WebClient();
                    client.DownloadProgressChanged += new DownloadProgressChangedEventHandler(client_DownloadProgressChanged);
                    client.OpenReadCompleted += new OpenReadCompletedEventHandler(client_OpenReadCompleted);
                    Logger.Debug("download start");
                    client.OpenReadAsync(source);
                    Logger.Debug("DownloadStringAsync returned");
                }
                catch (Exception eee)
                {
                    Logger.Fail(eee.ToString());
                    MessageBox.Show(Translations.translate("An error occurred trying to connect to the network. Try again later.") + "; " + eee.Message);
                }
            }
            else
            {
            }
        }

        private void SelectList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (isInSelectionChanged)
            {
                return;
            }
            isInSelectionChanged = true;
            MediaInfo info = (MediaInfo)((TextBlock)e.AddedItems[0]).Tag;
            if (info.player == MediaType.MEDIA_EXPLORER)
            {
                string uri = info.viewer + (string.IsNullOrEmpty(info.src) ? "" : "?src=" + Uri.EscapeDataString(info.src)) + (string.IsNullOrEmpty(info.icon) ? "" : "&icon=" + Uri.EscapeDataString(info.icon));
                BrowserTitledWindow.showInternetLinkWindow(uri, this.titleBar);
                if (NavigationService.CanGoBack)
                {
                    NavigationService.GoBack();
                }
            }
            else
            {
                MediaPlayerLauncher mediaPlayerLauncher =
                    new MediaPlayerLauncher()
                    {
                        Media = new Uri(info.src, UriKind.Absolute)
                    };
                mediaPlayerLauncher.Show();
            }
            isInSelectionChanged = false;
        }

        #endregion Methods

        enum MediaType
        {
            MEDIA_PLAYER,
            MEDIA_EXPLORER
        }

        private class MediaInfo
        {
            public string src = string.Empty;
            public string icon = string.Empty;
            public string viewer = string.Empty;
            public MediaType player = MediaType.MEDIA_PLAYER;
            public string visualText = string.Empty;
            public MediaInfo()
            {
            }
        }
    }
}