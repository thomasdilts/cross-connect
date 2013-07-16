// <copyright file="BrowserStartAudio.cs" company="Thomas Dilts">
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
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;

    using CrossConnect.readers;

    using Windows.UI.Popups;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;

    public sealed partial class BrowserTitledWindow
    {
        #region Fields

        private int _chapter;

        private WebClient _client = new WebClient();

        private bool _isInSelectionChanged;

        private string _language;

        private string _titleBar = string.Empty;

        #endregion

        #region Public Methods and Operators

        public static void AddMediaWindow(AudioPlayer.MediaInfo info)
        {
            // only one media window allowed
            for (int i = 0; i < App.OpenWindows.Count(); i++)
            {
                if (App.OpenWindows[i].State.WindowType == WindowType.WindowMediaPlayer)
                {
                    // change the windows view to this one
                    ((MediaPlayerWindow)App.OpenWindows[i]).RestartToThisMedia(info);
                    //App.MainWindow.OverRideCurrentlyShowingScreen(App.OpenWindows[i].State.Window);
                    return;
                }
            }

            var state = new SerializableWindowState
                            {
                                WindowType = WindowType.WindowMediaPlayer,
                                Source = new MediaReader(info)
                            };
            var nextWindow = new MediaPlayerWindow { State = state };
            nextWindow.State.CurIndex = App.OpenWindows.Count();
            nextWindow.State.HtmlFontSize = 20;
            App.OpenWindows.Add(nextWindow);

            if (App.MainWindow != null)
            {
                App.MainWindow.ReDrawWindows();
            }
        }

        #endregion

        #region Methods

        private static void ClientDownloadProgressChanged(byte bb)
        {
            // nothing to do here
        }

        private async void ClientOpenReadCompleted(string errorMessage)
        {
            if (string.IsNullOrEmpty(errorMessage))
            {
                string msg = string.Empty;
                Debug.WriteLine(await this._client.ReadStorageFileAsyncString());

                List<AudioPlayer.MediaInfo> mediaList =
                    await AudioPlayer.ReadMediaSourcesFile(this._client.downloadedFile);
                this._client.RemoveTempFile();
                this.MsgFromServer.Text = msg;

                foreach (
                    TextBlock block in
                        mediaList.Select(
                            mediaInfo =>
                            new TextBlock
                                {
                                    Text = mediaInfo.Name,
                                    Style = this.PageTitle.Style,
                                    Tag = mediaInfo,
                                    Margin = new Thickness(0, 0, 0, 10),
                                    TextWrapping = TextWrapping.Wrap
                                }))
                {
                    this.ListStartAudio.Items.Add(block);
                }
            }
            else
            {
                var dialog =
                    new MessageDialog(
                        Translations.Translate("An error occurred trying to connect to the network. Try again later.")
                        + "; " + errorMessage);
                await dialog.ShowAsync();
                this.StartAudioPopup.IsOpen = false;
            }

            this.MsgFromServer.Visibility = string.IsNullOrEmpty(this.MsgFromServer.Text)
                                                ? Visibility.Collapsed
                                                : Visibility.Visible;
            this.WaitingForDownload.Visibility = Visibility.Collapsed;
        }

        private void ListStartAudio_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this._isInSelectionChanged)
            {
                return;
            }

            this._isInSelectionChanged = true;

            // clear the selection because we might come here again after the media player
            this.ListStartAudio.SelectedIndex = -1;

            var info = (AudioPlayer.MediaInfo)((TextBlock)e.AddedItems[0]).Tag;

            AddMediaWindow(info);
            this._isInSelectionChanged = false;
            this.StartAudioPopup.IsOpen = false;
        }

        private void StartAudioPopup_OnClosed(object sender, object e)
        {
            App.ShowUserInterface(true);
        }

        private void StartAudioPopup_OnOpened(object sender, object e)
        {
            App.ShowUserInterface(false);
        }

        private async void StartAudio_OnClick()
        {
            MainPageSplit.SideBarShowPopup(
                this.StartAudioPopup, this.MainPaneStartAudioPopup, this.scrollViewerStartAudio);

            this.StartMediaTitle.Text = Translations.Translate("Select what you want to hear");
            this.WaitingForDownload.Visibility = Visibility.Visible;
            this.MsgFromServer.Visibility = Visibility.Collapsed;
            this.MsgFromServer.Text = string.Empty;
            this.ListStartAudio.Items.Clear();

            // do a download.
            int bookNum;
            int absoluteChaptNum;
            int relChaptNum;
            int verseNum;
            string fullName;
            string titleText;
            this._state.Source.GetInfo(
                out bookNum, out absoluteChaptNum, out relChaptNum, out verseNum, out fullName, out titleText);
            object language = this._state.Source.GetLanguage();
            ;
            string titleBar = titleText;

            this._chapter = absoluteChaptNum;
            this._language = (string)language;
            this._titleBar = titleBar;
            string url = string.Format(App.DisplaySettings.SoundLink, absoluteChaptNum, language);
            try
            {
                this._client = new WebClient();
                this._client.StartDownload(url, this.ClientOpenReadCompleted, ClientDownloadProgressChanged);
            }
            catch (Exception eee)
            {
                Logger.Fail(eee.ToString());
                var dialog =
                    new MessageDialog(
                        Translations.Translate("An error occurred trying to connect to the network. Try again later.")
                        + "; " + eee.Message);
                dialog.ShowAsync();
                this.StartAudioPopup.IsOpen = false;
            }
        }

        #endregion
    }
}