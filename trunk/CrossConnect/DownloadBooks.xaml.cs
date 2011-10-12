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
/// <copyright file="DownloadBooks.xaml.cs" company="Thomas Dilts">
///     Thomas Dilts. All rights reserved.
/// </copyright>
/// <author>Thomas Dilts</author>
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

    using SwordBackend;

    public partial class DownloadBooks : AutoRotatePage
    {
        #region Fields

        InstallManager imanager = new InstallManager();
        SwordBook sb = null;
        WebInstaller webInst = null;

        #endregion Fields

        #region Constructors

        public DownloadBooks()
        {
            InitializeComponent();
        }

        #endregion Constructors

        #region Methods

        public void CallbackFromUpdate(IAsyncResult ar)
        {
        }

        public void Do(Action action)
        {
            ThreadPool.QueueUserWorkItem(_ =>
            {
                try
                {
                    action();
                    //Deployment.Current.Dispatcher.BeginInvoke(() => callback(null));
                }
                catch (Exception ee)
                {
                    Debug.WriteLine("Do (webunziplist) Failed download books; " + ee.Message);
                    //Deployment.Current.Dispatcher.BeginInvoke(() => callback(null));
                    return;
                }
            });
        }

        private void butDownloadBook_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // not much to do, but get the book!!!!!
                if (webInst.entries != null && webInst.entries.ContainsKey(selectBook.SelectedItem.ToString()))
                {
                    butDownloadBook.Visibility = System.Windows.Visibility.Collapsed;
                    sb = webInst.entries[selectBook.SelectedItem.ToString()];
                    sb.progress_completed += new OpenReadCompletedEventHandler(sb_progress_completed);
                    sb.progress_update += new DownloadProgressChangedEventHandler(sb_progress_update);
                    progressBarGetBook.Visibility = System.Windows.Visibility.Visible;

                    string errMsg = sb.downloadBookNow(webInst);
                    if (errMsg != null)
                    {
                        MessageBox.Show(Translations.translate("An error occurred trying to connect to the network. Try again later.") + "; " + errMsg);
                        PhoneApplicationPage_Loaded(null, null);
                    }
                }
                else
                {
                    MessageBox.Show(Translations.translate("An error occurred trying to connect to the network. Try again later."));
                    PhoneApplicationPage_Loaded(null, null);
                }
            }
            catch (Exception eee)
            {
                MessageBox.Show(Translations.translate("An error occurred trying to connect to the network. Try again later.") + "; " + eee.Message);
                PhoneApplicationPage_Loaded(null, null);
            }
        }

        private void butDownload_Click(object sender, RoutedEventArgs e)
        {
            butDownload.Visibility = System.Windows.Visibility.Collapsed;
            // Ask the Install Manager for a map of all known module sites
            IDictionary<string, WebInstaller> installers = imanager.Installers;
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
                return;
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
            selectType.Header = Translations.translate("Download type");
            selectType.Items.Clear();
            selectType.Items.Add(Translations.translate("Bible"));
            selectType.Items.Add(Translations.translate("Commentaries"));

            selectType.Visibility = System.Windows.Visibility.Collapsed;
            butDownload.Visibility = System.Windows.Visibility.Visible;
            butDownloadBook.Visibility = System.Windows.Visibility.Collapsed;
            selectBook.Visibility = System.Windows.Visibility.Collapsed;
            progressBarGetBookList.Visibility = System.Windows.Visibility.Collapsed;
            progressBarGetBook.Visibility = System.Windows.Visibility.Collapsed;
            selectLangauge.Visibility = System.Windows.Visibility.Collapsed;
            selectServer.Items.Clear();

            // Ask the Install Manager for a map of all known module sites
            IDictionary<string, WebInstaller> installers = imanager.Installers;

            foreach (KeyValuePair<string, WebInstaller> mapEntry in installers)
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
            if (selectType.SelectedItem.Equals(Translations.translate("Commentaries")))
            {
                App.installedBibles.AddCommentary(sb.sbmd.internalName);
            }
            else
            {
                App.installedBibles.AddBook(sb.sbmd.internalName);
            }
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
            if (selectLangauge != null && selectLangauge.SelectedItem != null && webInst != null && webInst.entries!=null)
            {

                selectBook.Items.Clear();
                // put in the books
                Dictionary<string, string> allBooks = new Dictionary<string, string>();
                bool isCommentarySelected = selectType.SelectedItem.Equals(Translations.translate("Commentaries"));
                bool isBibleSelected = selectType.SelectedItem.Equals(Translations.translate("Bible"));
                foreach (var book in webInst.entries)
                {
                    Language lang = (Language)book.Value.sbmd.getProperty(ConfigEntryType.LANG);
                    if (lang.Name.Equals(selectLangauge.SelectedItem) && (
                        (isBibleSelected && ((string)book.Value.sbmd.getProperty(ConfigEntryType.MOD_DRV)).ToUpper().Equals("ZTEXT"))
                        || (isCommentarySelected && ((string)book.Value.sbmd.getProperty(ConfigEntryType.MOD_DRV)).ToUpper().Equals("ZCOM"))))
                    {
                        allBooks[book.Value.sbmd.Name] = book.Value.sbmd.Name;
                    }
                }
                var list = allBooks.OrderBy(t => t.Key).ToList();
                foreach (var x in list)
                {
                    selectBook.Items.Add(x.Key);
                }
                selectType.Visibility = System.Windows.Visibility.Visible;
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

        private void selectType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //måste reload bible list.
            webInst_progress_completed_unzipped(null, null);
            selectLangauge_SelectionChanged(sender, e);
        }

        private void webInst_progress_completed(object sender, OpenReadCompletedEventArgs e)
        {
            if (sender != null)
            {
                MessageBox.Show(Translations.translate("An error occurred trying to connect to the network. Try again later.") + "; " + (string)sender);
                PhoneApplicationPage_Loaded(null, null);
                return;
            }
            webInst.progress_completed -= webInst_progress_completed;
            webInst.progress_completed += webInst_progress_completed_unzipped;
            Do(() =>
            {
                webInst.unzipBookList();
            });
        }

        private void webInst_progress_completed_unzipped(object sender, OpenReadCompletedEventArgs e)
        {
            if (sender != null)
            {
                MessageBox.Show(Translations.translate("An error occurred trying to connect to the network. Try again later.") + "; " + (string)sender);
                PhoneApplicationPage_Loaded(null, null);
                return;
            }
            if (progressBarGetBookList != null)
            {
                progressBarGetBookList.Visibility = System.Windows.Visibility.Collapsed;
            }
            // need to load the book selection with all the books.
            if (selectLangauge != null)
            {
                selectLangauge.Items.Clear();
            }
            int bookcount = 0;
            bool isCommentarySelected = false;
            if (selectType != null && selectType.SelectedItem!=null)
            {
                isCommentarySelected=selectType.SelectedItem.Equals(Translations.translate("Commentaries"));
            }
            if (webInst!=null && webInst.isLoaded)
            {
                Dictionary<string, Language> allLanguages = new Dictionary<string, Language>();
                foreach (var book in webInst.entries)
                {
                    if (isCommentarySelected && ((string)book.Value.sbmd.getProperty(ConfigEntryType.MOD_DRV)).ToUpper().Equals("ZCOM"))
                    {
                        bookcount++;
                        Language lang = (Language)book.Value.sbmd.getProperty(ConfigEntryType.LANG);
                        allLanguages[lang.Name] = lang;

                    }
                    else if (!isCommentarySelected && ((string)book.Value.sbmd.getProperty(ConfigEntryType.MOD_DRV)).ToUpper().Equals("ZTEXT"))
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
            else if (webInst!=null )
            {
                MessageBox.Show(Translations.translate("An error occurred trying to connect to the network. Try again later.") + "; " + e.Error.Message);
                PhoneApplicationPage_Loaded(null, null);
            }
        }

        private void webInst_progress_update(object sender, DownloadProgressChangedEventArgs e)
        {
            progressBarGetBookList.Value = (double)sender;
        }

        #endregion Methods
    }
}