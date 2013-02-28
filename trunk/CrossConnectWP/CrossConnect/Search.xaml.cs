#region Header

// <copyright file="Search.xaml.cs" company="Thomas Dilts">
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
    using System.IO;
    using System.Linq;
    using System.Threading;
    using System.Windows;

    using CrossConnect.readers;

    using Microsoft.Phone.Shell;

    using Sword.reader;

    /// <summary>
    /// The search.
    /// </summary>
    public partial class Search
    {
        #region Fields

        /// <summary>
        /// The searching object.
        /// </summary>
        public static Search SearchingObject;

        /// <summary>
        /// The chapters.
        /// </summary>
        public List<int> Chapters = new List<int>();

        /// <summary>
        /// The is abort.
        /// </summary>
        public bool IsAbort;

        /// <summary>
        /// The is ignore case.
        /// </summary>
        public bool IsIgnoreCase;

        /// <summary>
        /// The is search finished.
        /// </summary>
        public bool IsSearchFinished;

        /// <summary>
        /// The is search finished reported.
        /// </summary>
        public bool IsSearchFinishedReported;
        public bool IsFastSearch = true;
        /// <summary>
        /// The search type index.
        /// </summary>
        public int SearchTypeIndex;

        /// <summary>
        /// The source search.
        /// </summary>
        public IBrowserTextSource SourceSearch;

        /// <summary>
        /// The text to search.
        /// </summary>
        public string TextToSearch = string.Empty;

        /// <summary>
        /// The _current book num.
        /// </summary>
        private int _currentBookNum;

        /// <summary>
        /// The _num found verses.
        /// </summary>
        private int _numFoundVerses;

        /// <summary>
        /// The _percent.
        /// </summary>
        private double _percent = 1;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Search"/> class.
        /// </summary>
        public Search()
        {
            InitializeComponent();
        }

        #endregion Constructors

        #region Methods

        public void ShowControls(bool isShow)
        {
            object openWindowIndex;
            if (!PhoneApplicationService.Current.State.TryGetValue("openWindowIndex", out openWindowIndex))
            {
                openWindowIndex = 0;
            } 
            
            var source = App.OpenWindows[(int)openWindowIndex].State.Source;
            this.IsFastSearch = (this.FastSearch.IsChecked != null && (bool)this.FastSearch.IsChecked);
            Visibility isVis = isShow ? Visibility.Visible : Visibility.Collapsed;
            SearchText.Visibility = isVis;
            this.FastSearch.Visibility = isVis;
            this.butFastHelp.Visibility = !this.IsFastSearch ? Visibility.Collapsed : isVis;
            this.SlowSearch.Visibility = isVis;
            SearchWhereText.Visibility = (source is RawGenTextReader) ? Visibility.Collapsed : isVis;
            wholeBible.Visibility = (source is RawGenTextReader) ? Visibility.Collapsed : isVis;
            oldTestement.Visibility = (source is RawGenTextReader) ? Visibility.Collapsed : isVis;
            newTEstement.Visibility = (source is RawGenTextReader) ? Visibility.Collapsed : isVis;
            Chapter.Visibility = (source is RawGenTextReader) ? Visibility.Collapsed : isVis;
            IgnoreCase.Visibility = this.IsFastSearch ? Visibility.Collapsed : isVis;
            progressBar1.Value = 0;
            butSearch.Visibility = isVis;
            butHelp.Visibility = this.IsFastSearch ? Visibility.Collapsed : isVis;
            PageTitle.Text = Translations.Translate("Search");
            SearchByText.Visibility = this.IsFastSearch ? Visibility.Collapsed : isVis;
            OneOrMoreWords.Visibility = this.IsFastSearch ? Visibility.Collapsed : isVis;
            AllWords.Visibility = this.IsFastSearch ? Visibility.Collapsed : isVis;
            ExactMatch.Visibility = this.IsFastSearch ? Visibility.Collapsed : isVis;
        }

        public void ShowProgress(double percent, int totalFound, bool isAbort, bool isFinished)
        {
            SearchingObject.IsSearchFinished = isFinished;
            SearchingObject._percent = percent;
            SearchingObject.IsAbort = isAbort;

            Dispatcher.BeginInvoke(UpdateProgressBar);

            _numFoundVerses = totalFound;
        }

        public void UpdateProgressBar()
        {
            if (SearchingObject.IsSearchFinishedReported)
            {
                // this is a delayed reporting that must be ignored.
                return;
            }

            SearchingObject.progressBar1.Value = _percent;
            PageTitle.Text = Translations.Translate("Search") + "; " + Translations.Translate("Found") + "; "
                                  + _numFoundVerses;
            if (IsSearchFinished)
            {
                SearchingObject.IsSearchFinishedReported = true;
                if (_numFoundVerses == 0)
                {
                    MessageBox.Show(Translations.Translate("Nothing found"));
                    ShowControls(true);
                }
                else
                {
                    if (IsAbort)
                    {
                        MessageBox.Show(Translations.Translate("Too many found. Search stopped"));
                    }

                    object openWindowIndex;
                    if (PhoneApplicationService.Current.State.TryGetValue("openWindowIndex", out openWindowIndex))
                    {
                        App.AddWindow(
                            App.OpenWindows[(int)openWindowIndex].State.BibleToLoad,
                            App.OpenWindows[(int)openWindowIndex].State.BibleDescription,
                            WindowType.WindowSearch,
                            App.OpenWindows[(int)openWindowIndex].State.HtmlFontSize,
                            SourceSearch);
                    }

                    PhoneApplicationService.Current.State["skipWindowSettings"] = true;
                    if (NavigationService.CanGoBack)
                    {
                        Debug.WriteLine("Now returning from search");
                        NavigationService.GoBack();
                    }
                }
            }
        }

        /// <summary>
        /// The but help click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void ButHelpClick(object sender, RoutedEventArgs e)
        {
            PhoneApplicationService.Current.State["HelpWindowFileToLoad"] = "CrossConnect.Properties.regex.html";
            PhoneApplicationService.Current.State["HelpWindowTitle"] = Translations.Translate("Help")
                                                                       + "(Regular Expressions)";

            NavigationService.Navigate(new Uri("/Help.xaml", UriKind.Relative));
        }

        /// <summary>
        /// The but search click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void ButSearchClick(object sender, RoutedEventArgs e)
        {
            Chapters.Clear();
            ShowControls(false);
            SearchingObject = this;
            if (IgnoreCase.IsChecked != null)
            {
                IsIgnoreCase = (bool)IgnoreCase.IsChecked;
            }

            TextToSearch = SearchText.Text;
            string[] parts = TextToSearch.Split(" ,".ToArray());
            var goodParts = new List<string>();
            for (int j = 0; j < parts.Count(); j++)
            {
                if (!string.IsNullOrEmpty(parts[j]))
                {
                    goodParts.Add(parts[j]);
                }
            }

            if (goodParts.Count() > 1)
            {
                if (OneOrMoreWords.IsChecked != null && (bool)OneOrMoreWords.IsChecked && !this.IsFastSearch)
                {
                    TextToSearch = goodParts[0];
                    for (int j = 1; j < goodParts.Count(); j++)
                    {
                        TextToSearch = TextToSearch + "|" + goodParts[j];
                    }
                }
                else if (AllWords.IsChecked != null && (bool)AllWords.IsChecked && !this.IsFastSearch)
                {
                    switch (goodParts.Count())
                    {
                        case 2:
                            TextToSearch = "(" + goodParts[0] + ".*?" + goodParts[1] + ")|(" + goodParts[1] + ".*?"
                                                + goodParts[0] + ")";
                            break;
                        case 3:
                            TextToSearch = "(" + goodParts[0] + ".*?" + goodParts[1] + ".*?" + goodParts[2] + ")|"
                                                + "(" + goodParts[0] + ".*?" + goodParts[2] + ".*?" + goodParts[1]
                                                + ")|" + "(" + goodParts[1] + ".*?" + goodParts[2] + ".*?"
                                                + goodParts[0] + ")|" + "(" + goodParts[1] + ".*?" + goodParts[0]
                                                + ".*?" + goodParts[2] + ")|" + "(" + goodParts[2] + ".*?"
                                                + goodParts[1] + ".*?" + goodParts[0] + ")|" + "(" + goodParts[2]
                                                + ".*?" + goodParts[0] + ".*?" + goodParts[1] + ")";
                            break;
                    }
                }
            }

            IsSearchFinished = false;
            IsSearchFinishedReported = false;
            object openWindowIndex;
            if (!PhoneApplicationService.Current.State.TryGetValue("openWindowIndex", out openWindowIndex))
            {
                openWindowIndex = 0;
            }

            if (wholeBible.IsChecked != null && (bool)wholeBible.IsChecked)
            {
                for (int i = 0; i < BibleZtextReader.ChaptersInBible; i++)
                {
                    Chapters.Add(i);
                }

                SearchTypeIndex = 0;
            }
            else if (oldTestement.IsChecked != null && (bool)oldTestement.IsChecked)
            {
                for (int i = 0; i < BibleZtextReader.ChaptersInOt; i++)
                {
                    Chapters.Add(i);
                }

                SearchTypeIndex = 1;
            }
            else if (newTEstement.IsChecked != null && (bool)newTEstement.IsChecked)
            {
                for (int i = BibleZtextReader.ChaptersInOt; i < BibleZtextReader.ChaptersInBible; i++)
                {
                    Chapters.Add(i);
                }

                SearchTypeIndex = 2;
            }
            else
            {
                // we must find the first chapter in the current book.
                int chapter = 0;
                for (int i = 0; i < _currentBookNum; i++)
                {
                    chapter += BibleZtextReader.ChaptersInBook[i];
                }

                // add all the chapters up to the last chapter in the book.
                int lastChapterInBook = chapter + BibleZtextReader.ChaptersInBook[_currentBookNum];
                for (int i = chapter; i < lastChapterInBook; i++)
                {
                    Chapters.Add(i);
                }

                SearchTypeIndex = 3;
            }

            var sourceStart = App.OpenWindows[(int)openWindowIndex].State.Source;
            if (sourceStart is RawGenTextReader)
            {
                this.Chapters = new List<int>();
                for (int i = 0; i < ((RawGenTextReader)sourceStart).Chapters.Count; i++)
                {
                    this.Chapters.Add(i);
                }

                this.SearchTypeIndex = 0;
                var source = (RawGenTextReader)sourceStart;
                this.SourceSearch = new RawGenSearchReader(
                    source.Serial.Path, source.Serial.Iso2DigitLangCode, source.Serial.IsIsoEncoding);
                ((RawGenTextReader)this.SourceSearch).Initialize();
            }
            else
            {
                var source = (BibleZtextReader)sourceStart;
                this.SourceSearch = new SearchReader(
                    source.Serial.Path, source.Serial.Iso2DigitLangCode, source.Serial.IsIsoEncoding);
                ((BibleZtextReader)this.SourceSearch).Initialize();
            }
            if (this.IsFastSearch)
            {
                var indexes = new string[] { "_deleted.idx", "index.mgbmp", "index.mgbmr", "index.words" };
                bool existsIndexes = true;
                var path = this.SourceSearch is SearchReader
                               ? ((SearchReader)this.SourceSearch).Serial.Path.Replace("/", "\\")
                               : ((RawGenSearchReader)this.SourceSearch).Serial.Path.Replace("/", "\\");
                foreach (var index in indexes)
                {
                    if (!Hoot.File.Exists(Path.Combine(path, index)))
                    {
                        existsIndexes = false;
                        break;
                    }
                }
                if (!existsIndexes)
                {
                    MessageBoxResult result =
                        MessageBox.Show(
                            Translations.Translate(
                                "The first time you do a fast search it takes a long time. Afterwards searching is very quick. Do you want to continue?"),
                            string.Empty,
                            MessageBoxButton.OKCancel);
                    if (result != MessageBoxResult.OK)
                    {
                        PhoneApplicationService.Current.State["skipWindowSettings"] = true;
                        if (NavigationService.CanGoBack)
                        {
                            Debug.WriteLine("Now returning from search");
                            NavigationService.GoBack();
                        }

                        return;
                    }
                }
            }
            tmr = new Timer(OnTimerTick);
            tmr.Change(300, Timeout.Infinite);
        }

        private Timer tmr = null;

        // void OnTimerTick(object sender, EventArgs e)
        /// <summary>
        /// The on timer tick.
        /// </summary>
        /// <param name="state">
        /// The state.
        /// </param>
        private void OnTimerTick(object state)
        {
            ((Timer)state).Dispose();


            if (this.SourceSearch is SearchReader)
            {
                ((SearchReader)this.SourceSearch).DoSearch(
                    App.DisplaySettings,
                    SearchingObject.SearchTypeIndex,
                    SearchingObject.TextToSearch,
                    SearchingObject.IsIgnoreCase,
                    SearchingObject.Chapters,
                    this.IsFastSearch,
                    this.ShowProgress);

            }
            else
            {
                ((RawGenSearchReader)this.SourceSearch).DoSearch(
                    App.DisplaySettings,
                    SearchingObject.SearchTypeIndex,
                    SearchingObject.TextToSearch,
                    SearchingObject.IsIgnoreCase,
                    SearchingObject.Chapters,
                    this.IsFastSearch,
                    this.ShowProgress);

            }
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
            object openWindowIndex;
            if (!PhoneApplicationService.Current.State.TryGetValue("openWindowIndex", out openWindowIndex))
            {
                openWindowIndex = 0;
            }

            int dummy2;
            string fullName;
            string text;
            int verseNum;
            int absoluteChaptNum;
            App.OpenWindows[(int)openWindowIndex].State.Source.GetInfo(
                out _currentBookNum, out absoluteChaptNum, out dummy2, out verseNum, out fullName, out text);
            Chapter.Content = fullName;

            PageTitle.Text = Translations.Translate("Search");
            butSearch.Content = Translations.Translate("Search");
            this.butFastHelp.Content = Translations.Translate("Help");
            this.SlowSearch.Content = Translations.Translate("Slow search");
            this.FastSearch.Content = Translations.Translate("Fast search");
            SearchWhereText.Text = Translations.Translate("Search where");
            wholeBible.Content = Translations.Translate("Whole bible");
            oldTestement.Content = Translations.Translate("The Old Testement");
            newTEstement.Content = Translations.Translate("The New Testement");
            SearchByText.Text = Translations.Translate("Search conditions");
            OneOrMoreWords.Content = Translations.Translate("One or more words");
            AllWords.Content = Translations.Translate("All words (maximum 3 words)");
            ExactMatch.Content = Translations.Translate("Exact match") + " (Regular Expressions)";
            IgnoreCase.Header = Translations.Translate("Case insensitive");
            butHelp.Content = Translations.Translate("Help") + " (Regular Expressions)";
            ShowControls(true);
        }

        private void ButFastHelp_OnClick(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(Translations.Translate("Fast search help text"));
        }

        private void FastSearch_OnClick(object sender, RoutedEventArgs e)
        {
            ShowControls(true);
        }

        #endregion Methods
    }
}