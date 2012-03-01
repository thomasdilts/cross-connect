// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DownloadBooks.xaml.cs" company="">
//   
// </copyright>
// <summary>
//   The download books.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#region Header

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

    /// <summary>
    /// The download books.
    /// </summary>
    public partial class DownloadBooks
    {
        #region Constants and Fields

        /// <summary>
        /// The _imanager.
        /// </summary>
        private readonly InstallManager _imanager = new InstallManager();

        /// <summary>
        /// The _is in completed unzipped.
        /// </summary>
        private bool _isInCompletedUnzipped;

        /// <summary>
        /// The _sb.
        /// </summary>
        private SwordBook _sb;

        /// <summary>
        /// The _web inst.
        /// </summary>
        private WebInstaller _webInst;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DownloadBooks"/> class.
        /// </summary>
        public DownloadBooks()
        {
            this.InitializeComponent();
        }

        #endregion

        #region Methods

        /// <summary>
        /// The but download book click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void ButDownloadBookClick(object sender, RoutedEventArgs e)
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
                        MessageBox.Show(
                            Translations.Translate(
                                "An error occurred trying to connect to the network. Try again later.") + "; " + errMsg);
                        this.PhoneApplicationPageLoaded(null, null);
                    }
                }
                else
                {
                    MessageBox.Show(
                        Translations.Translate("An error occurred trying to connect to the network. Try again later."));
                    this.PhoneApplicationPageLoaded(null, null);
                }
            }
            catch (Exception eee)
            {
                this.selectType.IsEnabled = true;
                this.selectBook.IsEnabled = true;
                this.selectLangauge.IsEnabled = true;
                this.selectServer.IsEnabled = true;
                MessageBox.Show(
                    Translations.Translate("An error occurred trying to connect to the network. Try again later.")
                    + "; " + eee.Message);
                this.PhoneApplicationPageLoaded(null, null);
            }
        }

        /// <summary>
        /// The but download click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void ButDownloadClick(object sender, RoutedEventArgs e)
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
                MessageBox.Show(
                    Translations.Translate("An error occurred trying to connect to the network. Try again later.")
                    + "; " + errMsg);
                this.selectServer.IsEnabled = true;
                this.PhoneApplicationPageLoaded(null, null);
                return;
            }
        }

        /// <summary>
        /// The do.
        /// </summary>
        /// <param name="action">
        /// The action.
        /// </param>
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

        /// <summary>
        /// The installers retrieved.
        /// </summary>
        /// <param name="installers">
        /// The installers.
        /// </param>
        /// <param name="message">
        /// The message.
        /// </param>
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
            this.WaitingForDownload.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// The phone application page loaded.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void PhoneApplicationPageLoaded(object sender, RoutedEventArgs e)
        {
            this.PageTitle.Text = Translations.Translate("Download bibles");
            this.selectServer.Header = Translations.Translate("Select the server");
            this.butDownload.Content = Translations.Translate("Download bible list");
            this.selectLangauge.Header = Translations.Translate("Select the language");
            this.selectBook.Header = Translations.Translate("Select the bible");
            this.butDownloadBook.Content = Translations.Translate("Download bible");
            this.selectType.Header = Translations.Translate("Download type");
            this.selectType.Items.Clear();
            this.selectType.Items.Add(Translations.Translate("Bible"));
            this.selectType.Items.Add(Translations.Translate("Commentaries"));

            this.WaitingForDownload.Visibility = Visibility.Visible;
            this.selectType.Visibility = Visibility.Collapsed;
            this.butDownload.Visibility = Visibility.Collapsed;
            this.butDownloadBook.Visibility = Visibility.Collapsed;
            this.selectBook.Visibility = Visibility.Collapsed;
            this.progressBarGetBookList.Visibility = Visibility.Collapsed;
            this.progressBarGetBook.Visibility = Visibility.Collapsed;
            this.selectLangauge.Visibility = Visibility.Collapsed;
            this.selectServer.Visibility = Visibility.Collapsed;
            this.ServerMessage.Visibility = Visibility.Collapsed;
            this.ServerMessage.Text = string.Empty;
            this.selectServer.Items.Clear();

            this._imanager.GetBibleDownloadList(this.InstallersRetrieved);
        }

        /// <summary>
        /// The sb progress completed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void SbProgressCompleted(object sender, OpenReadCompletedEventArgs e)
        {
            this.selectType.IsEnabled = true;
            this.selectBook.IsEnabled = true;
            this.selectLangauge.IsEnabled = true;
            this.selectServer.IsEnabled = true;
            if (sender != null)
            {
                MessageBox.Show(
                    Translations.Translate("An error occurred trying to connect to the network. Try again later.")
                    + "; " + (string)sender);
                this.PhoneApplicationPageLoaded(null, null);
                return;
            }

            if (this.selectType.SelectedItem.Equals(Translations.Translate("Commentaries")))
            {
                App.InstalledBibles.AddCommentary(this._sb.Sbmd.InternalName);
            }
            else
            {
                App.InstalledBibles.AddBook(this._sb.Sbmd.InternalName);
            }

            this._sb = null;
            if (this.NavigationService.CanGoBack)
            {
                this.NavigationService.GoBack();
            }
        }

        /// <summary>
        /// The sb progress update.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void SbProgressUpdate(object sender, DownloadProgressChangedEventArgs e)
        {
            this.progressBarGetBook.Value = e.ProgressPercentage;
        }

        /// <summary>
        /// The select langauge selection changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void SelectLangaugeSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.selectLangauge != null && this.selectLangauge.SelectedItem != null && this._webInst != null
                && this._webInst.Entries != null)
            {
                this.selectBook.Items.Clear();

                // put in the books
                var allBooks = new Dictionary<string, string>();
                bool isCommentarySelected = this.selectType.SelectedItem.Equals(Translations.Translate("Commentaries"));
                bool isBibleSelected = this.selectType.SelectedItem.Equals(Translations.Translate("Bible"));
                foreach (var book in this._webInst.Entries)
                {
                    var lang = (Language)book.Value.Sbmd.GetProperty(ConfigEntryType.Lang);
                    if (lang.Name.Equals(this.selectLangauge.SelectedItem)
                        &&
                        ((isBibleSelected
                          && ((string)book.Value.Sbmd.GetProperty(ConfigEntryType.ModDrv)).ToUpper().Equals("ZTEXT"))
                         ||
                         (isCommentarySelected
                          && ((string)book.Value.Sbmd.GetProperty(ConfigEntryType.ModDrv)).ToUpper().Equals("ZCOM"))))
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
                this.selectBook.Visibility = Visibility.Visible;
                this.butDownloadBook.Visibility = Visibility.Visible;
            }
        }

        /// <summary>
        /// The select server selection changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void SelectServerSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.selectServer != null && this.selectServer.Items.Count > 1)
            {
                // hide everything again.  Make them go through the proceedure
                this.butDownload.Visibility = Visibility.Visible;
                this.butDownloadBook.Visibility = Visibility.Collapsed;
                this.selectBook.Visibility = Visibility.Collapsed;
                this.progressBarGetBookList.Visibility = Visibility.Collapsed;
                this.progressBarGetBook.Visibility = Visibility.Collapsed;
                this.selectLangauge.Visibility = Visibility.Collapsed;
            }
        }

        /// <summary>
        /// The select type selection changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void SelectTypeSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // måste reload bible list.
            if (this.selectType == null || this.selectType.Items.Count() == 0)
            {
                return;
            }

            this.WebInstProgressCompletedUnzipped(null, null);
            this.SelectLangaugeSelectionChanged(sender, e);
        }

        /// <summary>
        /// The web inst progress completed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void WebInstProgressCompleted(object sender, OpenReadCompletedEventArgs e)
        {
            if (sender != null)
            {
                MessageBox.Show(
                    Translations.Translate("An error occurred trying to connect to the network. Try again later.")
                    + "; " + (string)sender);
                this.PhoneApplicationPageLoaded(null, null);
                return;
            }

            this._webInst.ProgressCompleted -= this.WebInstProgressCompleted;
            this._webInst.ProgressCompleted += this.WebInstProgressCompletedUnzipped;
            this.Do(() => this._webInst.UnzipBookList());
        }

        /// <summary>
        /// The web inst progress completed unzipped.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void WebInstProgressCompletedUnzipped(object sender, OpenReadCompletedEventArgs e)
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

                if (sender != null)
                {
                    MessageBox.Show(
                        Translations.Translate("An error occurred trying to connect to the network. Try again later.")
                        + "; " + (string)sender);
                    this.PhoneApplicationPageLoaded(null, null);
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

                if (this._webInst != null && this._webInst.IsLoaded && this.selectLangauge != null
                    && this.selectLangauge.Items != null)
                {
                    var allLanguages = new Dictionary<string, Language>();
                    foreach (var book in this._webInst.Entries)
                    {
                        if (isCommentarySelected
                            && ((string)book.Value.Sbmd.GetProperty(ConfigEntryType.ModDrv)).ToUpper().Equals("ZCOM"))
                        {
                            var lang = (Language)book.Value.Sbmd.GetProperty(ConfigEntryType.Lang);
                            allLanguages[lang.Name] = lang;
                        }
                        else if (!isCommentarySelected
                                 &&
                                 ((string)book.Value.Sbmd.GetProperty(ConfigEntryType.ModDrv)).ToUpper().Equals("ZTEXT"))
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
                    }
                }
                else if (this._webInst != null)
                {
                    MessageBox.Show(
                        Translations.Translate("An error occurred trying to connect to the network. Try again later.")
                        + "; " + e.Error.Message);
                    this.PhoneApplicationPageLoaded(null, null);
                }
            }
            catch (Exception e2)
            {
                Debug.WriteLine(e2.StackTrace);
                this.PhoneApplicationPageLoaded(null, null);
            }

            this._isInCompletedUnzipped = false;
        }

        /// <summary>
        /// The web inst progress update.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void WebInstProgressUpdate(object sender, DownloadProgressChangedEventArgs e)
        {
            this.progressBarGetBookList.Value = (double)sender;
        }

        #endregion
    }
}