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
/// <copyright file="THIS_FILE.cs" company="Thomas Dilts">
///     Thomas Dilts. All rights reserved.
/// </copyright>
/// <author>Thomas Dilts</author>
namespace CrossConnect
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using System.Windows.Shapes;

    using Microsoft.Phone.Controls;

    using SwordBackend;

    public partial class DownloadBooks : PhoneApplicationPage
    {
        #region Fields

        InstallManager imanager = new InstallManager();
        SwordBook sb = null;
        SwordBackend.WebInstaller webInst = null;

        #endregion Fields

        #region Constructors

        public DownloadBooks()
        {
            InitializeComponent();
        }

        #endregion Constructors

        #region Methods

        private void butDownloadBook_Click(object sender, RoutedEventArgs e)
        {
            // not much to do, but get the book!!!!!
            butDownloadBook.Visibility = System.Windows.Visibility.Collapsed;
            sb = webInst.entries[selectBook.SelectedItem.ToString()];
            sb.progress_completed+=new OpenReadCompletedEventHandler(sb_progress_completed);
            sb.progress_update+=new DownloadProgressChangedEventHandler(sb_progress_update);
            progressBarGetBook.Visibility = System.Windows.Visibility.Visible;
            string errMsg=sb.downloadBookNow(webInst);
            if (errMsg != null)
            {
                MessageBox.Show(Translations.translate("An error occurred trying to connect to the network. Try again later.") + "; " + errMsg);
                PhoneApplicationPage_Loaded(null, null);
            }
        }

        private void butDownload_Click(object sender, RoutedEventArgs e)
        {
            butDownload.Visibility = System.Windows.Visibility.Collapsed;
            // Ask the Install Manager for a map of all known module sites
            IDictionary<string, SwordBackend.WebInstaller> installers = imanager.Installers;
            webInst=installers[selectServer.SelectedItem.ToString()];
            progressBarGetBookList.Visibility = System.Windows.Visibility.Visible;
            progressBarGetBookList.Maximum = 100;
            progressBarGetBookList.Minimum = 0;
            progressBarGetBookList.Value = 1;
            webInst.progress_update += webInst_progress_update;
            webInst.progress_completed += webInst_progress_completed;

            string errMsg=webInst.reloadBookList();
            if (errMsg != null)
            {
                MessageBox.Show(Translations.translate("An error occurred trying to connect to the network. Try again later.") + "; " + errMsg);
                PhoneApplicationPage_Loaded(null, null);
            }
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            PageTitle.Text=Translations.translate("Download bibles");
            selectServer.Header=Translations.translate("Select the server");
            butDownload.Content = Translations.translate("Download bible list");
            selectLangauge.Header=Translations.translate("Select the language");
            selectBook.Header=Translations.translate("Select the bible");
            butDownloadBook.Content = Translations.translate("Download bible");

            butDownload.Visibility = System.Windows.Visibility.Visible;
            butDownloadBook.Visibility = System.Windows.Visibility.Collapsed;
            selectBook.Visibility = System.Windows.Visibility.Collapsed;
            progressBarGetBookList.Visibility = System.Windows.Visibility.Collapsed;
            progressBarGetBook.Visibility = System.Windows.Visibility.Collapsed;
            selectLangauge.Visibility = System.Windows.Visibility.Collapsed;
            selectServer.Items.Clear();

            // Ask the Install Manager for a map of all known module sites
            IDictionary<string, SwordBackend.WebInstaller> installers = imanager.Installers;

            foreach (KeyValuePair<string, SwordBackend.WebInstaller> mapEntry in installers)
            {
                selectServer.Items.Add(mapEntry.Key.ToString());
            }
            selectServer.SelectedIndex = 0;
        }

        private void sb_progress_completed(object sender, OpenReadCompletedEventArgs e)
        {
            if (sender != null)
            {
                MessageBox.Show(Translations.translate("An error occurred trying to connect to the network. Try again later.") + "; " + (string)sender);
                PhoneApplicationPage_Loaded(null, null);
                return;
            }
            App.installedBibles.AddBook(sb.sbmd.internalName);
            sb = null;
            if (NavigationService.CanGoBack)
            {
                NavigationService.GoBack();
            }
        }

        private void sb_progress_update(object sender, DownloadProgressChangedEventArgs e)
        {
            progressBarGetBook.Value = e.ProgressPercentage;
        }

        private void selectLangauge_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (selectLangauge!=null && selectLangauge.SelectedItem != null)
            {

                selectBook.Items.Clear();
                // put in the books
                Dictionary<string, string> allBooks = new Dictionary<string, string>();
                foreach (var book in webInst.entries)
                {
                    Language lang = (Language)book.Value.sbmd.getProperty(ConfigEntryType.LANG);
                    if (lang.Name.Equals(selectLangauge.SelectedItem) && ((string)book.Value.sbmd.getProperty(ConfigEntryType.MOD_DRV)).ToUpper().Equals("ZTEXT"))
                    {
                        allBooks[book.Value.sbmd.Name] = book.Value.sbmd.Name;
                    }
                }
                var list = allBooks.OrderBy(t => t.Key).ToList();
                foreach (var x in list)
                {
                    selectBook.Items.Add(x.Key);
                }
                selectBook.Visibility = System.Windows.Visibility.Visible;
                butDownloadBook.Visibility = System.Windows.Visibility.Visible;
            }
        }

        private void selectServer_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (selectServer != null && selectServer.Items.Count > 1)
            {
                // hide everything again.  Make them go through the proceedure
                butDownload.Visibility = System.Windows.Visibility.Visible;
                butDownloadBook.Visibility = System.Windows.Visibility.Collapsed;
                selectBook.Visibility = System.Windows.Visibility.Collapsed;
                progressBarGetBookList.Visibility = System.Windows.Visibility.Collapsed;
                progressBarGetBook.Visibility = System.Windows.Visibility.Collapsed;
                selectLangauge.Visibility = System.Windows.Visibility.Collapsed;
            }
        }

        private void webInst_progress_completed(object sender, OpenReadCompletedEventArgs e)
        {
            if (sender != null)
            {
                MessageBox.Show(Translations.translate("An error occurred trying to connect to the network. Try again later.") + "; " + (string)sender);
                PhoneApplicationPage_Loaded(null, null);
                return;
            }
            progressBarGetBookList.Visibility = System.Windows.Visibility.Collapsed;
            // need to load the book selection with all the books.
            selectLangauge.Items.Clear();
            int bookcount = 0;
            if (webInst.isLoaded)
            {
                Dictionary<string, Language> allLanguages = new Dictionary<string, Language>();
                foreach (var book in webInst.entries)
                {
                    if (((string)book.Value.sbmd.getProperty(ConfigEntryType.MOD_DRV)).ToUpper().Equals("ZTEXT"))
                    {
                        bookcount++;
                        Language lang = (Language)book.Value.sbmd.getProperty(ConfigEntryType.LANG);
                        allLanguages[lang.Name] = lang;
                    }

                }
                var list = allLanguages.OrderBy(t => t.Key).ToList();
                foreach (var x in list)
                {
                    selectLangauge.Items.Add(x.Key);
                }
                selectLangauge.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                MessageBox.Show(Translations.translate("An error occurred trying to connect to the network. Try again later.") + "; " + e.Error.Message);
                PhoneApplicationPage_Loaded(null, null);
            }
        }

        private void webInst_progress_update(object sender, DownloadProgressChangedEventArgs e)
        {
            progressBarGetBookList.Value = e.ProgressPercentage;
        }

        #endregion Methods
    }
}