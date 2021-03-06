﻿#region Header

// <copyright file="DownloadBooks.xaml.cs" company="Thomas Dilts">
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
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Net;
    using System.Threading;
    using System.Windows;
    using System.Windows.Controls;

    using Sword;
    using System.Threading.Tasks;

    public partial class DownloadBooks
    {
        #region Fields

        private readonly InstallManager _imanager = new InstallManager();

        private bool _isInCompletedUnzipped;
        private SwordBook _sb;
        private IWebInstaller _webInst;

        private bool _isInThisWindow = false;

        #endregion Fields

        #region Constructors

        public DownloadBooks()
        {
            InitializeComponent();
        }

        #endregion Constructors

        #region Methods

        private async void ButDownloadBookClick(object sender, RoutedEventArgs e)
        {
            try
            {
                // not much to do, but get the book!!!!!
                if (_webInst.Entries != null
                    && _webInst.Entries.ContainsKey(selectBook.SelectedItem.ToString()))
                {
                    selectType.IsEnabled = false;
                    selectBook.IsEnabled = false;
                    selectLangauge.IsEnabled = false;
                    selectServer.IsEnabled = false;

                    butDownloadBook.Visibility = Visibility.Collapsed;
                    _sb = _webInst.Entries[selectBook.SelectedItem.ToString()];
                    progressBarGetBook.Visibility = Visibility.Visible;
                    progressBarGetBook.Value = 5;
                    string errMsg = await _webInst.DownloadBookNow(_sb.Sbmd, SbProgressUpdate, SbProgressCompleted);
                    if (errMsg != null)
                    {
                        MessageBox.Show(
                            Translations.Translate(
                                "An error occurred trying to connect to the network. Try again later.") + "; " + errMsg);
                        PhoneApplicationPageLoaded(null, null);
                    }
                }
                else
                {
                    MessageBox.Show(
                        Translations.Translate("An error occurred trying to connect to the network. Try again later."));
                    PhoneApplicationPageLoaded(null, null);
                }
            }
            catch (Exception eee)
            {
                selectType.IsEnabled = true;
                selectBook.IsEnabled = true;
                selectLangauge.IsEnabled = true;
                selectServer.IsEnabled = true;
                MessageBox.Show(
                    Translations.Translate("An error occurred trying to connect to the network. Try again later.")
                    + "; " + eee.Message);
                PhoneApplicationPageLoaded(null, null);
            }
        }

        private async void ButDownloadClick(object sender, RoutedEventArgs e)
        {
            butDownload.Visibility = Visibility.Collapsed;

            // Ask the Install Manager for a map of all known module sites
            IDictionary<string, IWebInstaller> installers = _imanager.Installers;
            _webInst = installers[selectServer.SelectedItem.ToString()];
            progressBarGetBookList.Visibility = Visibility.Visible;
            progressBarGetBookList.Maximum = 100;
            progressBarGetBookList.Minimum = 0;
            progressBarGetBookList.Value = 5;
            selectServer.IsEnabled = false;

            string errMsg = await _webInst.ReloadBookList(WebInstProgressUpdate, WebInstProgressCompleted);
            if (errMsg != null)
            {
                MessageBox.Show(
                    Translations.Translate("An error occurred trying to connect to the network. Try again later.")
                    + "; " + errMsg);
                selectServer.IsEnabled = true;
                PhoneApplicationPageLoaded(null, null);
                return;
            }
        }

        private void Do(Action action)
        {
            ThreadPool.QueueUserWorkItem(
                _ =>
                    {
                        try
                        {
                            action();

                            // Deployment.Current.Dispatcher.BeginInvoke(() => callback(null));
                        }
                        catch (Exception ee)
                        {
                            Debug.WriteLine("Do (webunziplist) Failed download books; " + ee.Message);

                            // Deployment.Current.Dispatcher.BeginInvoke(() => callback(null));
                            return;
                        }
                    });
        }

        private void InstallersRetrieved(Dictionary<string, IWebInstaller> installers, string message)
        {
            foreach (var mapEntry in installers)
            {
                selectServer.Items.Add(mapEntry.Key);
            }

            ServerMessage.Text = message;
            selectServer.SelectedIndex = 0;
            ServerMessage.Visibility = string.IsNullOrEmpty(message) ? Visibility.Collapsed : Visibility.Visible;
            butDownload.Visibility = Visibility.Visible;
            selectServer.Visibility = Visibility.Visible;
            WaitingForDownload.Visibility = Visibility.Collapsed;
        }

        private void PhoneApplicationPageLoaded(object sender, RoutedEventArgs e)
        {
            if (_isInThisWindow)
            {
                return;
            }

            _isInThisWindow = true;

           // selectLangauge.ExpansionMode = Microsoft.Phone.Controls.ExpansionMode.FullScreenOnly;
            PageTitle.Text = Translations.Translate("Download bibles");
            selectServer.Header = Translations.Translate("Select the server");
            butDownload.Content = Translations.Translate("Download bible list");
            selectLangauge.Header = Translations.Translate("Select the language");
            selectBook.Header = Translations.Translate("Select the bible");
            butDownloadBook.Content = Translations.Translate("Download bible");
            selectType.Header = Translations.Translate("Download type");
            selectType.Items.Clear();
            selectType.Items.Add(Translations.Translate("Bible"));
            selectType.Items.Add(Translations.Translate("Commentaries"));
            selectType.Items.Add(Translations.Translate("Books"));
            selectType.Items.Add(Translations.Translate("Dictionaries"));

            WaitingForDownload.Visibility = Visibility.Visible;
            selectType.Visibility = Visibility.Collapsed;
            butDownload.Visibility = Visibility.Collapsed;
            butDownloadBook.Visibility = Visibility.Collapsed;
            selectBook.Visibility = Visibility.Collapsed;
            progressBarGetBookList.Visibility = Visibility.Collapsed;
            progressBarGetBook.Visibility = Visibility.Collapsed;
            selectLangauge.Visibility = Visibility.Collapsed;
            selectServer.Visibility = Visibility.Collapsed;
            ServerMessage.Visibility = Visibility.Collapsed;
            ServerMessage.Text = string.Empty;
            selectServer.Items.Clear();

            _imanager.GetBibleDownloadList(InstallersRetrieved);
        }

        private async void SbProgressCompleted(object sender, OpenReadCompletedEventArgs e)
        {
            selectType.IsEnabled = true;
            selectBook.IsEnabled = true;
            selectLangauge.IsEnabled = true;
            selectServer.IsEnabled = true;
            if (sender != null)
            {
                MessageBox.Show(
                    Translations.Translate("An error occurred trying to connect to the network. Try again later.")
                    + "; " + (string)sender);
                PhoneApplicationPageLoaded(null, null);
                return;
            }

            await App.InstalledBibles.AddGenericBook(_sb.Sbmd.InternalName);

            _sb = null;
            _isInThisWindow = false;

            if (NavigationService.CanGoBack)
            {
                NavigationService.GoBack();
            }
        }

        private void SbProgressUpdate(object sender, DownloadProgressChangedEventArgs e)
        {
            progressBarGetBook.Value = e.ProgressPercentage;
        }

        private void SelectLangaugeSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (selectLangauge != null && selectLangauge.SelectedItem != null && _webInst != null
                && _webInst.Entries != null)
            {
                selectBook.Items.Clear();

                // put in the books
                var allBooks = new Dictionary<string, string>();
                bool isCommentarySelected = selectType.SelectedItem.Equals(Translations.Translate("Commentaries"));
                bool isGeneralBookSelected = selectType.SelectedItem.Equals(Translations.Translate("Books"));
                bool isDictionarySelected = selectType.SelectedItem.Equals(Translations.Translate("Dictionaries"));
                bool isBibleSelected = selectType.SelectedItem.Equals(Translations.Translate("Bible"));
                foreach (var book in _webInst.Entries)
                {
                    var lang = (Language)book.Value.Sbmd.GetProperty(ConfigEntryType.Lang);
                    var driver = ((string)book.Value.Sbmd.GetProperty(ConfigEntryType.ModDrv)).ToUpper();
                    if (lang.Name.Equals(selectLangauge.SelectedItem)
                        &&
                        ((isBibleSelected
                          && driver.Equals("ZTEXT"))
                         ||
                         (isCommentarySelected
                          && driver.Equals("ZCOM"))
                        || (isGeneralBookSelected
                                && driver.Equals("RAWGENBOOK"))
                        || (isDictionarySelected
                                && (driver.Equals("RAWLD") || driver.Equals("RAWLD4") || driver.Equals("ZLD")))))
                    {
                        allBooks[book.Value.Sbmd.Name] = book.Value.Sbmd.Name;
                    }
                }

                List<KeyValuePair<string, string>> list = allBooks.OrderBy(t => t.Key).ToList();
                foreach (var x in list)
                {
                    selectBook.Items.Add(x.Key);
                }

                selectType.Visibility = App.InstalledBibles.InstalledBibles.Count > 0
                                                 ? Visibility.Visible
                                                 : Visibility.Collapsed;
                selectBook.Visibility = Visibility.Visible;
                butDownloadBook.Visibility = Visibility.Visible;
            }
        }

        private void SelectServerSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (selectServer != null && selectServer.Items.Count > 1)
            {
                // hide everything again.  Make them go through the proceedure
                butDownload.Visibility = Visibility.Visible;
                butDownloadBook.Visibility = Visibility.Collapsed;
                selectBook.Visibility = Visibility.Collapsed;
                progressBarGetBookList.Visibility = Visibility.Collapsed;
                progressBarGetBook.Visibility = Visibility.Collapsed;
                selectLangauge.Visibility = Visibility.Collapsed;
            }
        }

        private void SelectTypeSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // måste reload bible list.
            if (selectType == null || selectType.Items.Count() == 0)
            {
                return;
            }

            WebInstProgressCompletedUnzipped(null, null);
            SelectLangaugeSelectionChanged(sender, e);
        }

        private void WebInstProgressCompleted(object sender, OpenReadCompletedEventArgs e)
        {
            if (sender != null)
            {
                MessageBox.Show(
                    Translations.Translate("An error occurred trying to connect to the network. Try again later.")
                    + "; " + (string)sender);
                PhoneApplicationPageLoaded(null, null);
                return;
            }

            Do(() => _webInst.UnzipBookList(WebInstProgressUpdate, WebInstProgressCompletedUnzipped));
        }

        private void WebInstProgressCompletedUnzipped(object sender, OpenReadCompletedEventArgs e)
        {
            if (_isInCompletedUnzipped)
            {
                return;
            }

            try
            {
                _isInCompletedUnzipped = true;
                if (selectServer != null)
                {
                    selectServer.IsEnabled = true;
                }

                if (sender != null)
                {
                    MessageBox.Show(
                        Translations.Translate("An error occurred trying to connect to the network. Try again later.")
                        + "; " + (string)sender);
                    PhoneApplicationPageLoaded(null, null);
                    _isInCompletedUnzipped = false;
                    return;
                }

                if (progressBarGetBookList != null)
                {
                    progressBarGetBookList.Visibility = Visibility.Collapsed;
                }

                // need to load the book selection with all the books.
                if (selectLangauge != null)
                {
                    selectLangauge.IsEnabled = true;
                    selectLangauge.Items.Clear();
                }

                bool isCommentarySelected = false;
                bool isGeneralBookSelected = false;
                bool isDictionarySelected = false;
                if (selectType != null && selectType.SelectedItem != null)
                {
                    isCommentarySelected = selectType.SelectedItem.Equals(Translations.Translate("Commentaries"));
                    isGeneralBookSelected = selectType.SelectedItem.Equals(Translations.Translate("Books"));
                    isDictionarySelected = selectType.SelectedItem.Equals(Translations.Translate("Dictionaries"));
                    selectType.Visibility = Visibility.Visible;
                }

                if (_webInst != null && _webInst.IsLoaded && selectLangauge != null
                    && selectLangauge.Items != null)
                {
                    var allLanguages = new Dictionary<string, Language>();
                    foreach (var book in _webInst.Entries)
                    {
                        var driver = ((string)book.Value.Sbmd.GetProperty(ConfigEntryType.ModDrv)).ToUpper();
                        if (isCommentarySelected
                            && driver.Equals("ZCOM"))
                        {
                            var lang = (Language)book.Value.Sbmd.GetProperty(ConfigEntryType.Lang);
                            allLanguages[lang.Name] = lang;
                        }
                        else if (isGeneralBookSelected
                            && driver.Equals("RAWGENBOOK"))
                        {
                            var lang = (Language)book.Value.Sbmd.GetProperty(ConfigEntryType.Lang);
                            allLanguages[lang.Name] = lang;
                        }
                        else if (isDictionarySelected
                            && (driver.Equals("RAWLD") || driver.Equals("RAWLD4") || driver.Equals("ZLD")))
                        {
                            var lang = (Language)book.Value.Sbmd.GetProperty(ConfigEntryType.Lang);
                            allLanguages[lang.Name] = lang;
                        }
                        else if (!isCommentarySelected && !isGeneralBookSelected && !isDictionarySelected
                                 &&
                                 driver.Equals("ZTEXT"))
                        {
                            var lang = (Language)book.Value.Sbmd.GetProperty(ConfigEntryType.Lang);
                            allLanguages[lang.Name] = lang;
                        }
                    }

                    List<KeyValuePair<string, Language>> list = allLanguages.OrderBy(t => t.Key).ToList();
                    foreach (var x in list)
                    {
                        if (selectLangauge != null)
                        {
                            selectLangauge.Items.Add(x.Key);
                        }
                    }

                    if (selectLangauge != null)
                    {
                        selectLangauge.Visibility = Visibility.Visible;
                    }
                }
                else if (_webInst != null)
                {
                    MessageBox.Show(
                        Translations.Translate("An error occurred trying to connect to the network. Try again later.")
                        + "; " + e.Error.Message);
                    PhoneApplicationPageLoaded(null, null);
                }
            }
            catch (Exception e2)
            {
                Debug.WriteLine(e2.StackTrace);
                PhoneApplicationPageLoaded(null, null);
            }

            _isInCompletedUnzipped = false;
        }

        private void WebInstProgressUpdate(object sender, DownloadProgressChangedEventArgs e)
        {
            progressBarGetBookList.Value = (double)sender;
        }

        #endregion Methods
    }
}