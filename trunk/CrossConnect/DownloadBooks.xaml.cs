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
using book.install;

namespace CrossConnect
{
    public partial class DownloadBooks : PhoneApplicationPage
    {
        InstallManager imanager = new InstallManager();

        public DownloadBooks()
        {
            InitializeComponent();
        }
        book.install.WebInstaller webInst = null;

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            butDownload.Visibility = System.Windows.Visibility.Visible;
            butDownloadBook.Visibility = System.Windows.Visibility.Collapsed;
            selectBook.Visibility = System.Windows.Visibility.Collapsed;
            progressBarGetBookList.Visibility = System.Windows.Visibility.Collapsed;
            progressBarGetBook.Visibility = System.Windows.Visibility.Collapsed;
            selectLangauge.Visibility = System.Windows.Visibility.Collapsed;
            selectServer.Items.Clear();

            // Ask the Install Manager for a map of all known module sites
            IDictionary<string, book.install.WebInstaller> installers = imanager.Installers;

            foreach (KeyValuePair<string, book.install.WebInstaller> mapEntry in installers)
            {
                selectServer.Items.Add(mapEntry.Key.ToString());
            }
            selectServer.SelectedIndex = 0;
        }

        private void butDownload_Click(object sender, RoutedEventArgs e)
        {
            butDownload.Visibility = System.Windows.Visibility.Collapsed;
            // Ask the Install Manager for a map of all known module sites
            IDictionary<string, book.install.WebInstaller> installers = imanager.Installers;
            webInst=installers[selectServer.SelectedItem.ToString()];
            progressBarGetBookList.Visibility = System.Windows.Visibility.Visible;
            progressBarGetBookList.Maximum = 100;
            progressBarGetBookList.Minimum = 0;
            progressBarGetBookList.Value = 1;
            webInst.progress_update += webInst_progress_update;
            webInst.progress_completed += webInst_progress_completed;

            webInst.reloadBookList();
        }
        private void webInst_progress_update(object sender, DownloadProgressChangedEventArgs e)
        {
            progressBarGetBookList.Value = e.ProgressPercentage;
        }
        private void webInst_progress_completed(object sender, OpenReadCompletedEventArgs e)
        {
            progressBarGetBookList.Visibility = System.Windows.Visibility.Collapsed;
            //need to load the book selection with all the books.
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
                selectLangauge.Items.Add("Touch here to select");
                foreach (var x in list)
                {
                    selectLangauge.Items.Add(x.Key);
                }
            }
            selectLangauge.Visibility = System.Windows.Visibility.Visible;
            
        }

        private void selectLangauge_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (selectLangauge!=null && selectLangauge.SelectedItem != null)
            {
                if (selectLangauge.SelectedItem.Equals("Touch here to select"))
                {
                    //hide books and such
                    butDownloadBook.Visibility = System.Windows.Visibility.Collapsed;
                    selectBook.Visibility = System.Windows.Visibility.Collapsed;
                    progressBarGetBookList.Visibility = System.Windows.Visibility.Collapsed;
                    progressBarGetBook.Visibility = System.Windows.Visibility.Collapsed;
                    return;
                }
                selectBook.Items.Clear();
                //put in the books
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
        SwordBook sb = null;
        private void butDownloadBook_Click(object sender, RoutedEventArgs e)
        {
            //not much to do, but get the book!!!!!
            butDownloadBook.Visibility = System.Windows.Visibility.Collapsed;
            sb = webInst.entries[selectBook.SelectedItem.ToString()];
            sb.progress_completed+=new OpenReadCompletedEventHandler(sb_progress_completed);
            sb.progress_update+=new DownloadProgressChangedEventHandler(sb_progress_update);
            progressBarGetBook.Visibility = System.Windows.Visibility.Visible;
            sb.downloadBookNow(webInst);
        }
        private void sb_progress_update(object sender, DownloadProgressChangedEventArgs e)
        {
            progressBarGetBook.Value = e.ProgressPercentage;
        }
        private void sb_progress_completed(object sender, OpenReadCompletedEventArgs e)
        {
            if (sb.isLoaded)
            {
                App.installedBooks.AddBook(sb.sbmd.Initials);
                sb = null;
            }
            this.NavigationService.GoBack();
        }
        private void selectServer_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (selectServer != null && selectServer.Items.Count > 1)
            {
                //hide everything again.  Make them go through the proceedure
                butDownload.Visibility = System.Windows.Visibility.Visible;
                butDownloadBook.Visibility = System.Windows.Visibility.Collapsed;
                selectBook.Visibility = System.Windows.Visibility.Collapsed;
                progressBarGetBookList.Visibility = System.Windows.Visibility.Collapsed;
                progressBarGetBook.Visibility = System.Windows.Visibility.Collapsed;
                selectLangauge.Visibility = System.Windows.Visibility.Collapsed;                
            }
        }

    }
}