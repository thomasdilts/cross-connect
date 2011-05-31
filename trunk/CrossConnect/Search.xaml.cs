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
    using System.Diagnostics;
    using System.IO;
    using System.IO.IsolatedStorage;
    using System.Linq;
    using System.Net;
    using System.Text.RegularExpressions;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using System.Windows.Shapes;

    using Microsoft.Phone.Controls;
    using Microsoft.Phone.Shell;

    using SwordBackend;

    public partial class Search : AutoRotatePage
    {
        #region Fields

        public static Search searchingObject = null;

        public List<int> chapters = new List<int>();
        public bool ignoreCase = false;
        public bool isAbort = false;
        public bool isSearchFinished = false;
        public bool isSearchFinishedReported = false;
        public string searchText = string.Empty;
        public int searchTypeIndex = 0;
        public SearchReader sourceSearch = null;

        private int currentBookNum;
        private int numFoundVerses = 0;
        private double percent = 1;

        #endregion Fields

        #region Constructors

        public Search()
        {
            InitializeComponent();
        }

        #endregion Constructors

        #region Methods

        public void ShowControls(bool isShow)
        {
            System.Windows.Visibility isVis = (isShow ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed);
            SearchText.Visibility = isVis;
            SearchWhereText.Visibility = isVis;
            wholeBible.Visibility = isVis;
            oldTestement.Visibility = isVis;
            newTEstement.Visibility = isVis;
            Chapter.Visibility = isVis;
            IgnoreCase.Visibility = isVis;
            progressBar1.Value = 0;
            butSearch.Visibility = isVis;
            butHelp.Visibility = isVis;
            PageTitle.Text = Translations.translate("Search");
            SearchByText.Visibility = isVis;
            OneOrMoreWords.Visibility = isVis;
            AllWords.Visibility = isVis;
            ExactMatch.Visibility = isVis;
        }

        public void ShowProgress(double percent, int totalFound, bool isAbort, bool isFinished)
        {
            Search.searchingObject.isSearchFinished = isFinished;
            Search.searchingObject.percent = percent;
            Search.searchingObject.isAbort = isAbort;

            Dispatcher.BeginInvoke(UpdateProgressBar);

            numFoundVerses = totalFound;
        }

        public void UpdateProgressBar()
        {
            if (Search.searchingObject.isSearchFinishedReported)
            {
                // this is a delayed reporting that must be ignored.
                return;
            }
            Search.searchingObject.progressBar1.Value = percent;
            PageTitle.Text = Translations.translate("Search") + "; " + Translations.translate("Found") + "; " + numFoundVerses;
            if (isSearchFinished)
            {
                Search.searchingObject.isSearchFinishedReported = true;
                if (numFoundVerses == 0)
                {
                    MessageBox.Show(Translations.translate("Nothing found"));
                    ShowControls(true);
                }
                else
                {
                    if (isAbort)
                    {
                        MessageBox.Show(Translations.translate("Too many found. Search stopped"));
                    }
                    object openWindowIndex = null;
                    if (PhoneApplicationService.Current.State.TryGetValue("openWindowIndex", out openWindowIndex))
                    {
                        App.AddWindow(
                            App.openWindows[(int)openWindowIndex].state.bibleToLoad,
                            WINDOW_TYPE.WINDOW_SEARCH,
                            App.openWindows[(int)openWindowIndex].state.htmlFontSize,
                            sourceSearch);
                    }
                    PhoneApplicationService.Current.State["skipWindowSettings"] = true;
                    if (NavigationService.CanGoBack)
                    {
                        NavigationService.GoBack();
                    }
                }
            }
        }

        private void butHelp_Click(object sender, RoutedEventArgs e)
        {
            PhoneApplicationService.Current.State["HelpWindowFileToLoad"] = "CrossConnect.Properties.regex.html";
            PhoneApplicationService.Current.State["HelpWindowTitle"]= Translations.translate("Help") + "(Regular Expressions)";

            NavigationService.Navigate(new Uri("/Help.xaml", UriKind.Relative));
        }

        private void butSearch_Click(object sender, RoutedEventArgs e)
        {
            chapters.Clear();
            ShowControls(false);
            searchingObject = this;
            ignoreCase = (bool)IgnoreCase.IsChecked;
            searchText = SearchText.Text;
            string[] parts = searchText.Split(" ,".ToArray());
            List<string> goodParts = new List<string>();
            for (int j = 0; j < parts.Count(); j++)
            {
                if (!string.IsNullOrEmpty(parts[j]))
                {
                    goodParts.Add(parts[j]);
                }
            }
            if (goodParts.Count() > 1)
            {
                if ((bool)OneOrMoreWords.IsChecked)
                {
                    searchText = goodParts[0];
                    for (int j = 1; j < goodParts.Count(); j++)
                    {
                        searchText = searchText + "|" + goodParts[j];
                    }
                }
                else if ((bool)AllWords.IsChecked)
                {
                    switch (goodParts.Count())
                    {
                        case 2:
                            searchText = "(" + goodParts[0] + ".*?" + goodParts[1] + ")|(" + goodParts[1] + ".*?" + goodParts[0] + ")";
                            break;
                        case 3:
                            searchText =
                            "(" + goodParts[0] + ".*?" + goodParts[1] + ".*?" + goodParts[2] + ")|" +
                            "(" + goodParts[0] + ".*?" + goodParts[2] + ".*?" + goodParts[1] + ")|" +
                            "(" + goodParts[1] + ".*?" + goodParts[2] + ".*?" + goodParts[0] + ")|" +
                            "(" + goodParts[1] + ".*?" + goodParts[0] + ".*?" + goodParts[2] + ")|" +
                            "(" + goodParts[2] + ".*?" + goodParts[1] + ".*?" + goodParts[0] + ")|" +
                            "(" + goodParts[2] + ".*?" + goodParts[0] + ".*?" + goodParts[1] + ")";
                            break;
                    }
                }
            }

            isSearchFinished = false;
            isSearchFinishedReported = false;
            object openWindowIndex = null;
            if (!PhoneApplicationService.Current.State.TryGetValue("openWindowIndex", out openWindowIndex))
            {
                openWindowIndex = (int)0;
            }

            if ((bool)wholeBible.IsChecked)
            {
                for (int i = 0; i < BibleZtextReader.CHAPTERS_IN_BIBLE; i++)
                {
                    chapters.Add(i);
                }
                searchTypeIndex = 0;
            }
            else if ((bool)oldTestement.IsChecked)
            {
                for (int i = 0; i < BibleZtextReader.CHAPTERS_IN_OT; i++)
                {
                    chapters.Add(i);
                }
                searchTypeIndex = 1;
            }
            else if ((bool)newTEstement.IsChecked)
            {
                for (int i = BibleZtextReader.CHAPTERS_IN_OT; i < BibleZtextReader.CHAPTERS_IN_BIBLE; i++)
                {
                    chapters.Add(i);
                }
                searchTypeIndex = 2;
            }
            else
            {
                // we must find the first chapter in the current book.
                int chapter = 0;
                for (int i = 0; i < currentBookNum; i++)
                {
                    chapter += SwordBackend.BibleZtextReader.CHAPTERS_IN_BOOK[i];
                }
                // add all the chapters up to the last chapter in the book.
                int lastChapterInBook = chapter + SwordBackend.BibleZtextReader.CHAPTERS_IN_BOOK[currentBookNum];
                for (int i = chapter; i < lastChapterInBook; i++)
                {
                    chapters.Add(i);
                }

                searchTypeIndex = 3;
            }
            BibleZtextReader source = (BibleZtextReader)App.openWindows[(int)openWindowIndex].state.source;
            sourceSearch = new SearchReader(
                source.serial.path,
                source.serial.iso2DigitLangCode,
                source.serial.isIsoEncoding);

            System.Threading.Timer tmr = new System.Threading.Timer(new System.Threading.TimerCallback(OnTimerTick));
            tmr.Change(300, System.Threading.Timeout.Infinite);
        }

        // void OnTimerTick(object sender, EventArgs e)
        void OnTimerTick(object state)
        {
            ((System.Threading.Timer)state).Dispose();

            sourceSearch.doSearch(
                Search.searchingObject.searchTypeIndex,
                Search.searchingObject.searchText,
                Search.searchingObject.ignoreCase,
                Search.searchingObject.chapters,
                ShowProgress);
        }

        private void PhoneApplicationPage_BackKeyPress(object sender, System.ComponentModel.CancelEventArgs e)
        {
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            object openWindowIndex = null;
            if (!PhoneApplicationService.Current.State.TryGetValue("openWindowIndex", out openWindowIndex))
            {
                openWindowIndex = (int)0;
            }
            var state = App.openWindows[(int)openWindowIndex].state;

            int dummy2;
            string Name;
            string text;
            int verseNum;
            int absoluteChaptNum;
            App.openWindows[(int)openWindowIndex].state.source.GetInfo(out currentBookNum,out absoluteChaptNum, out dummy2, out verseNum, out Name, out text);
            Chapter.Content = Name;

            PageTitle.Text = Translations.translate("Search");
            butSearch.Content = Translations.translate("Search");
            SearchWhereText.Text = Translations.translate("Search where");
            wholeBible.Content = Translations.translate("Whole bible");
            oldTestement.Content = Translations.translate("Old Testement");
            newTEstement.Content = Translations.translate("New Testement");
            SearchByText.Text = Translations.translate("Search conditions");
            OneOrMoreWords.Content = Translations.translate("One or more words");
            AllWords.Content = Translations.translate("All words (maximum 3 words)");
            ExactMatch.Content = Translations.translate("Exact match") + " (Regular Expressions)";
            IgnoreCase.Header = Translations.translate("Case insensitive");
            butHelp.Content = Translations.translate("Help") + " (Regular Expressions)";
        }

        #endregion Methods
    }
}