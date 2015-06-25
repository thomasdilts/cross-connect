// <copyright file="MainPageDownloadBible.cs" company="Thomas Dilts">
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

    using Sword;

    using Windows.Storage;
    using Windows.UI.Core;
    using Windows.UI.Popups;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;

    public sealed partial class MainPageSplit
    {
        #region Fields

        private readonly InstallManager _imanager = new InstallManager();

        private bool _isInCompletedUnzipped;

        private SwordBook _sb;

        private WebInstaller _webInst;

        #endregion

        #region Methods

        private async void ButDownloadBookClick(object sender, RoutedEventArgs e)
        {
            try
            {
                // not much to do, but get the book!!!!!
                if (this._webInst.Entries != null
                    && this._webInst.Entries.ContainsKey(this.selectBook.SelectedItem.ToString()))
                {
                    this.selectType.IsEnabled = false;
                    this.selectBook.IsEnabled = false;
                    this.selectLangauge.IsEnabled = false;
                    this.selectServer.IsEnabled = false;

                    this.butDownloadBook.Visibility = Visibility.Collapsed;
                    this._sb = this._webInst.Entries[this.selectBook.SelectedItem.ToString()];
                    this._sb.ProgressCompleted += this.SbProgressCompleted;
                    this._sb.ProgressUpdate += this.SbProgressUpdate;
                    this.progressBarGetBook.Visibility = Visibility.Visible;
                    this.progressBarGetBook.Value = 5;
                    string errMsg = this._sb.DownloadBookNow(this._webInst);
                    if (errMsg != null)
                    {
                        var dialog =
                            new MessageDialog(
                                Translations.Translate(
                                    "An error occurred trying to connect to the network. Try again later.") + "; "
                                + errMsg);
                        await dialog.ShowAsync();
                        this.MenuDownloadBibleClick(null, null);
                    }
                }
                else
                {
                    var dialog =
                        new MessageDialog(
                            Translations.Translate(
                                "An error occurred trying to connect to the network. Try again later."));
                    await dialog.ShowAsync();
                    this.MenuDownloadBibleClick(null, null);
                }
            }
            catch (Exception eee)
            {
                this.selectType.IsEnabled = true;
                this.selectBook.IsEnabled = true;
                this.selectLangauge.IsEnabled = true;
                this.selectServer.IsEnabled = true;
                var dialog =
                    new MessageDialog(
                        Translations.Translate("An error occurred trying to connect to the network. Try again later.")
                        + "; " + eee.Message);
                dialog.ShowAsync();
                this.MenuDownloadBibleClick(null, null);
            }
        }

        private async void ButDownloadClick(object sender, RoutedEventArgs e)
        {
            this.butDownload.Visibility = Visibility.Collapsed;

            // Ask the Install Manager for a map of all known module sites
            IDictionary<string, WebInstaller> installers = this._imanager.Installers;
            this._webInst = installers[this.selectServer.SelectedItem.ToString()];
            this.progressBarGetBookList.Visibility = Visibility.Visible;
            this.progressBarGetBookList.Maximum = 100;
            this.progressBarGetBookList.Minimum = 0;
            this.progressBarGetBookList.Value = 5;
            this.selectServer.IsEnabled = false;
            this._webInst.ProgressUpdate += this.WebInstProgressUpdate;
            this._webInst.ProgressCompleted += this.WebInstProgressCompleted;

            string errMsg = this._webInst.ReloadBookList();
            if (errMsg != null)
            {
                var dialog =
                    new MessageDialog(
                        Translations.Translate("An error occurred trying to connect to the network. Try again later.")
                        + "; " + errMsg);
                await dialog.ShowAsync();
                this.selectServer.IsEnabled = true;
                this.MenuDownloadBibleClick(null, null);
                return;
            }
        }

        private void DownloadBiblePopup_OnClosed(object sender, object e)
        {
            App.ShowUserInterface(true);
        }

        private void DownloadBiblePopup_OnOpened(object sender, object e)
        {
            App.ShowUserInterface(false);
        }

        private void InstallersRetrieved(Dictionary<string, WebInstaller> installers, string message)
        {
            foreach (var mapEntry in installers)
            {
                this.selectServer.Items.Add(mapEntry.Key);
            }

            this.ServerMessage.Text = message;
            this.selectServer.SelectedIndex = 0;
            this.ServerMessage.Visibility = string.IsNullOrEmpty(message) ? Visibility.Collapsed : Visibility.Visible;
            this.butDownload.Visibility = Visibility.Visible;
            this.selectServer.Visibility = Visibility.Visible;
            this.selectServerHeader.Visibility = Visibility.Visible;
            this.WaitingForDownload.Visibility = Visibility.Collapsed;
        }

        private void MenuDownloadBibleClick(object sender, RoutedEventArgs e)
        {
            SideBarShowPopup(
                this.DownloadBiblePopup,
                this.MainPaneDownloadBiblePopup,
                this.scrollViewerBibleDownload,
                this.TopAppBar1,
                this.BottomAppBar);
            this.SelectBibleDownloadTitle.Text = Translations.Translate("Download bible");
            this.selectServerHeader.Text = Translations.Translate("Select the server");
            this.selectLangaugeHeader.Text = Translations.Translate("Select the language");
            this.selectBookHeader.Text = Translations.Translate("Select the bible");
            this.selectTypeHeader.Text = Translations.Translate("Download type");
            this.butDownload.Content = Translations.Translate("Download bible list");
            this.butDownloadBook.Content = Translations.Translate("Download bible");
            this.selectType.Items.Clear();
            this.selectType.Items.Add(Translations.Translate("Bible"));
            this.selectType.Items.Add(Translations.Translate("Commentaries"));
            this.selectType.Items.Add(Translations.Translate("Books"));
            this.selectType.Items.Add(Translations.Translate("Dictionaries"));
            this.selectType.SelectedIndex = 0;

            this.WaitingForDownload.Visibility = Visibility.Visible;
            this.selectServerHeader.Visibility = Visibility.Collapsed;
            this.selectType.Visibility = Visibility.Collapsed;
            this.selectTypeHeader.Visibility = Visibility.Collapsed;
            this.butDownload.Visibility = Visibility.Collapsed;
            this.butDownloadBook.Visibility = Visibility.Collapsed;
            this.selectBookHeader.Visibility = Visibility.Collapsed;
            this.selectBook.Visibility = Visibility.Collapsed;
            this.progressBarGetBookList.Visibility = Visibility.Collapsed;
            this.progressBarGetBook.Visibility = Visibility.Collapsed;
            this.selectLangaugeHeader.Visibility = Visibility.Collapsed;
            this.selectLangauge.Visibility = Visibility.Collapsed;
            this.selectServer.Visibility = Visibility.Collapsed;
            this.ServerMessage.Visibility = Visibility.Collapsed;
            this.ServerMessage.Text = string.Empty;
            this.selectServer.Items.Clear();
            this.DownloadBiblePopup.IsOpen = true;

            this._imanager.GetBibleDownloadList(this.InstallersRetrieved);
        }

        private async void SbProgressCompleted(string error)
        {
            this.selectType.IsEnabled = true;
            this.selectBook.IsEnabled = true;
            this.selectLangauge.IsEnabled = true;
            this.selectServer.IsEnabled = true;
            if (!string.IsNullOrEmpty(error))
            {
                var dialog =
                    new MessageDialog(
                        Translations.Translate("An error occurred trying to connect to the network. Try again later.")
                        + "; " + error);
                await dialog.ShowAsync();
                this.MenuDownloadBibleClick(null, null);
                return;
            }

            int Window = 0;
            double Font = 20;
            if (App.OpenWindows.Any())
            {
                Window = App.OpenWindows[0].State.Window;
                Font = App.OpenWindows[0].State.HtmlFontSize;
            }
            WindowType typeSelected = WindowType.WindowBible;
            switch (this.selectType.SelectedIndex)
            {
                case 0:
                    typeSelected = WindowType.WindowBible;
                    break;
                case 1:
                    typeSelected = WindowType.WindowCommentary;
                    break;
                case 2:
                    typeSelected = WindowType.WindowBook;
                    break;
                case 3:
                    typeSelected = WindowType.WindowDictionary;
                    break;
            }

            var fontFamily = (string)_sb.Sbmd.GetProperty(ConfigEntryType.Font);
            fontFamily = fontFamily==null?string.Empty:fontFamily;
            await App.InstalledBibles.AddGenericBook(this._sb.Sbmd.InternalName);
            await App.AddWindow(
                this._sb.Sbmd.InternalName,
                this._sb.Sbmd.Name,
                typeSelected,
                Font,
                fontFamily,
                null,
                Window,
                null,
                false); 

            this._sb = null;
            this.DownloadBiblePopup.IsOpen = false;
            this.ReDrawWindows(false);
        }

        private void SbProgressUpdate(byte percent)
        {
            this.progressBarGetBook.Value = percent;
        }

        private void SelectLangaugeSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.selectLangauge != null && this.selectLangauge.SelectedItem != null && this._webInst != null
                && this._webInst.Entries != null)
            {
                this.selectBook.Items.Clear();

                // put in the books
                var allBooks = new Dictionary<string, string>();
                bool isCommentarySelected = this.selectType.SelectedItem != null
                                            && this.selectType.SelectedItem.Equals(
                                                Translations.Translate("Commentaries"));
                bool isGeneralBookSelected = this.selectType.SelectedItem != null
                                            && this.selectType.SelectedItem.Equals(
                                                Translations.Translate("Books")); 
                bool isDictionariesSelected = this.selectType.SelectedItem != null
                                             && this.selectType.SelectedItem.Equals(
                                                 Translations.Translate("Dictionaries"));
                bool isBibleSelected = this.selectType.SelectedItem == null
                                       || this.selectType.SelectedItem.Equals(Translations.Translate("Bible"));
                foreach (var book in this._webInst.Entries)
                {
                    var lang = (Language)book.Value.Sbmd.GetProperty(ConfigEntryType.Lang);
                    var driver = ((string)book.Value.Sbmd.GetProperty(ConfigEntryType.ModDrv)).ToUpper();
                    if (lang.Name.Equals(this.selectLangauge.SelectedItem)
                        && ((isBibleSelected
                             && (driver.Equals("ZTEXT") || driver.Equals("RAWTEXT")))
                            || (isCommentarySelected
                                && (driver.Equals("ZCOM") || driver.Equals("RAWCOM")))
                                || (isGeneralBookSelected
                                && driver.Equals("RAWGENBOOK"))
                                || (isDictionariesSelected
                                && (driver.Equals("RAWLD") || driver.Equals("RAWLD4") || driver.Equals("ZLD")))))
                    {
                        allBooks[book.Value.Sbmd.Name] = book.Value.Sbmd.Name;
                    }
                }

                List<KeyValuePair<string, string>> list = allBooks.OrderBy(t => t.Key).ToList();
                foreach (var x in list)
                {
                    this.selectBook.Items.Add(x.Key);
                }

                this.selectType.Visibility = App.InstalledBibles.InstalledBibles.Count > 0
                                                 ? Visibility.Visible
                                                 : Visibility.Collapsed;
                this.selectTypeHeader.Visibility = this.selectType.Visibility;
                this.selectBook.Visibility = Visibility.Visible;
                this.selectBookHeader.Visibility = Visibility.Visible;
                this.butDownloadBook.Visibility = Visibility.Visible;
                if (this.selectBook.Items.Any())
                {
                    this.selectBook.SelectedIndex = 0;
                }
            }
        }

        private void SelectServerSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.selectServer != null && this.selectServer.Items.Count > 1)
            {
                // hide everything again.  Make them go through the proceedure
                this.butDownload.Visibility = Visibility.Visible;
                this.butDownloadBook.Visibility = Visibility.Collapsed;
                this.selectBook.Visibility = Visibility.Collapsed;
                this.selectBookHeader.Visibility = Visibility.Collapsed;
                this.progressBarGetBookList.Visibility = Visibility.Collapsed;
                this.progressBarGetBook.Visibility = Visibility.Collapsed;
                this.selectLangauge.Visibility = Visibility.Collapsed;
                this.selectLangaugeHeader.Visibility = Visibility.Collapsed;
            }
        }

        private void SelectTypeSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // måste reload bible list.
            if (this.selectType == null || this.selectType.Items.Count() == 0)
            {
                return;
            }

            this.WebInstProgressCompletedUnzipped(string.Empty);
            this.SelectLangaugeSelectionChanged(sender, e);
        }

        private async void WebInstProgressCompleted(string error)
        {
            if (!string.IsNullOrEmpty(error))
            {
                var dialog =
                    new MessageDialog(
                        Translations.Translate("An error occurred trying to connect to the network. Try again later.")
                        + "; " + error);
                dialog.ShowAsync();
                this.MenuDownloadBibleClick(null, null);
                return;
            }

            this._webInst.ProgressCompleted -= this.WebInstProgressCompleted;
            this._webInst.ProgressCompleted += this.WebInstProgressCompletedUnzipped;
            this._webInst.UnzipBookList();
            this.selectType.SelectedItem = 0;
        }

        private async void WebInstProgressCompletedUnzipped(string error)
        {
            if (this._isInCompletedUnzipped)
            {
                return;
            }

            try
            {
                this._isInCompletedUnzipped = true;
                if (this.selectServer != null)
                {
                    this.selectServer.IsEnabled = true;
                }

                if (!string.IsNullOrEmpty(error))
                {
                    var dialog =
                        new MessageDialog(
                            Translations.Translate(
                                "An error occurred trying to connect to the network. Try again later.") + "; " + error);
                    await dialog.ShowAsync();
                    this.MenuDownloadBibleClick(null, null);
                    this._isInCompletedUnzipped = false;
                    return;
                }

                if (this.progressBarGetBookList != null)
                {
                    this.progressBarGetBookList.Visibility = Visibility.Collapsed;
                }

                // need to load the book selection with all the books.
                if (this.selectLangauge != null)
                {
                    this.selectLangauge.IsEnabled = true;
                    this.selectLangauge.Items.Clear();
                }

                bool isCommentarySelected = false;
                if (this.selectType != null && this.selectType.SelectedItem != null)
                {
                    isCommentarySelected = this.selectType.SelectedItem.Equals(Translations.Translate("Commentaries"));
                }
                bool isGeneralBookSelected = false;
                if (this.selectType != null && this.selectType.SelectedItem != null)
                {
                    isGeneralBookSelected = this.selectType.SelectedItem.Equals(Translations.Translate("Books"));
                }
                bool isDictionarySelected = false;
                if (this.selectType != null && this.selectType.SelectedItem != null)
                {
                    isDictionarySelected = this.selectType.SelectedItem.Equals(Translations.Translate("Dictionaries"));
                }
                bool isBibleSelected = !isDictionarySelected && !isGeneralBookSelected && !isCommentarySelected;

                if (this._webInst != null && this._webInst.IsLoaded && this.selectLangauge != null
                    && this.selectLangauge.Items != null)
                {
                    var allLanguages = new Dictionary<string, Language>();
                    foreach (var book in this._webInst.Entries)
                    {
                        var driver = ((string)book.Value.Sbmd.GetProperty(ConfigEntryType.ModDrv)).ToUpper();
                        if ((isCommentarySelected
                            && (driver.Equals("ZCOM") || driver.Equals("RAWCOM")))
                            ||(isGeneralBookSelected
                            && driver.Equals("RAWGENBOOK"))
                            ||(isBibleSelected
                            && (driver.Equals("ZTEXT")|| driver.Equals("RAWTEXT")))
                            ||(isDictionarySelected
                            && (driver.Equals("ZLD") || driver.Equals("RAWLD") || driver.Equals("RAWLD4"))))
                        {
                            var lang = (Language)book.Value.Sbmd.GetProperty(ConfigEntryType.Lang);
                            allLanguages[lang.Name] = lang;
                        }
                    }

                    List<KeyValuePair<string, Language>> list = allLanguages.OrderBy(t => t.Key).ToList();
                    foreach (var x in list)
                    {
                        if (this.selectLangauge != null)
                        {
                            this.selectLangauge.Items.Add(x.Key);
                        }
                    }

                    if (this.selectLangauge != null)
                    {
                        this.selectLangauge.Visibility = Visibility.Visible;
                        this.selectLangaugeHeader.Visibility = Visibility.Visible;
                        this.selectType.Visibility = Visibility.Visible;
                        this.selectTypeHeader.Visibility = Visibility.Visible;
                    }
                }
                else if (this._webInst != null)
                {
                    var dialog =
                        new MessageDialog(
                            Translations.Translate(
                                "An error occurred trying to connect to the network. Try again later."));
                    await dialog.ShowAsync();
                    this.MenuDownloadBibleClick(null, null);
                }
            }
            catch (Exception e2)
            {
                Debug.WriteLine(e2.StackTrace);
                this.MenuDownloadBibleClick(null, null);
            }

            this._isInCompletedUnzipped = false;
        }

        private void WebInstProgressUpdate(byte percent)
        {
            this.progressBarGetBookList.Value = percent;
        }

        #endregion
    }
}