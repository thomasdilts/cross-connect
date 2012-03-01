// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Search.xaml.cs" company="">
//   
// </copyright>
// <summary>
//   The search.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

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
        #region Constants and Fields

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

        /// <summary>
        /// The search type index.
        /// </summary>
        public int SearchTypeIndex;

        /// <summary>
        /// The source search.
        /// </summary>
        public SearchReader SourceSearch;

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

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Search"/> class.
        /// </summary>
        public Search()
        {
            this.InitializeComponent();
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The show controls.
        /// </summary>
        /// <param name="isShow">
        /// The is show.
        /// </param>
        public void ShowControls(bool isShow)
        {
            Visibility isVis = isShow ? Visibility.Visible : Visibility.Collapsed;
            this.SearchText.Visibility = isVis;
            this.SearchWhereText.Visibility = isVis;
            this.wholeBible.Visibility = isVis;
            this.oldTestement.Visibility = isVis;
            this.newTEstement.Visibility = isVis;
            this.Chapter.Visibility = isVis;
            this.IgnoreCase.Visibility = isVis;
            this.progressBar1.Value = 0;
            this.butSearch.Visibility = isVis;
            this.butHelp.Visibility = isVis;
            this.PageTitle.Text = Translations.Translate("Search");
            this.SearchByText.Visibility = isVis;
            this.OneOrMoreWords.Visibility = isVis;
            this.AllWords.Visibility = isVis;
            this.ExactMatch.Visibility = isVis;
        }

        /// <summary>
        /// The show progress.
        /// </summary>
        /// <param name="percent">
        /// The percent.
        /// </param>
        /// <param name="totalFound">
        /// The total found.
        /// </param>
        /// <param name="isAbort">
        /// The is abort.
        /// </param>
        /// <param name="isFinished">
        /// The is finished.
        /// </param>
        public void ShowProgress(double percent, int totalFound, bool isAbort, bool isFinished)
        {
            SearchingObject.IsSearchFinished = isFinished;
            SearchingObject._percent = percent;
            SearchingObject.IsAbort = isAbort;

            this.Dispatcher.BeginInvoke(this.UpdateProgressBar);

            this._numFoundVerses = totalFound;
        }

        /// <summary>
        /// The update progress bar.
        /// </summary>
        public void UpdateProgressBar()
        {
            if (SearchingObject.IsSearchFinishedReported)
            {
                // this is a delayed reporting that must be ignored.
                return;
            }

            SearchingObject.progressBar1.Value = this._percent;
            this.PageTitle.Text = Translations.Translate("Search") + "; " + Translations.Translate("Found") + "; "
                                  + this._numFoundVerses;
            if (this.IsSearchFinished)
            {
                SearchingObject.IsSearchFinishedReported = true;
                if (this._numFoundVerses == 0)
                {
                    MessageBox.Show(Translations.Translate("Nothing found"));
                    this.ShowControls(true);
                }
                else
                {
                    if (this.IsAbort)
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
                            this.SourceSearch);
                    }

                    PhoneApplicationService.Current.State["skipWindowSettings"] = true;
                    if (this.NavigationService.CanGoBack)
                    {
                        Debug.WriteLine("Now returning from search");
                        this.NavigationService.GoBack();
                    }
                }
            }
        }

        #endregion

        #region Methods

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

            this.NavigationService.Navigate(new Uri("/Help.xaml", UriKind.Relative));
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
            this.Chapters.Clear();
            this.ShowControls(false);
            SearchingObject = this;
            if (this.IgnoreCase.IsChecked != null)
            {
                this.IsIgnoreCase = (bool)this.IgnoreCase.IsChecked;
            }

            this.TextToSearch = this.SearchText.Text;
            string[] parts = this.TextToSearch.Split(" ,".ToArray());
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
                if (this.OneOrMoreWords.IsChecked != null && (bool)this.OneOrMoreWords.IsChecked)
                {
                    this.TextToSearch = goodParts[0];
                    for (int j = 1; j < goodParts.Count(); j++)
                    {
                        this.TextToSearch = this.TextToSearch + "|" + goodParts[j];
                    }
                }
                else if (this.AllWords.IsChecked != null && (bool)this.AllWords.IsChecked)
                {
                    switch (goodParts.Count())
                    {
                        case 2:
                            this.TextToSearch = "(" + goodParts[0] + ".*?" + goodParts[1] + ")|(" + goodParts[1] + ".*?"
                                                + goodParts[0] + ")";
                            break;
                        case 3:
                            this.TextToSearch = "(" + goodParts[0] + ".*?" + goodParts[1] + ".*?" + goodParts[2] + ")|"
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

            this.IsSearchFinished = false;
            this.IsSearchFinishedReported = false;
            object openWindowIndex;
            if (!PhoneApplicationService.Current.State.TryGetValue("openWindowIndex", out openWindowIndex))
            {
                openWindowIndex = 0;
            }

            if (this.wholeBible.IsChecked != null && (bool)this.wholeBible.IsChecked)
            {
                for (int i = 0; i < BibleZtextReader.ChaptersInBible; i++)
                {
                    this.Chapters.Add(i);
                }

                this.SearchTypeIndex = 0;
            }
            else if (this.oldTestement.IsChecked != null && (bool)this.oldTestement.IsChecked)
            {
                for (int i = 0; i < BibleZtextReader.ChaptersInOt; i++)
                {
                    this.Chapters.Add(i);
                }

                this.SearchTypeIndex = 1;
            }
            else if (this.newTEstement.IsChecked != null && (bool)this.newTEstement.IsChecked)
            {
                for (int i = BibleZtextReader.ChaptersInOt; i < BibleZtextReader.ChaptersInBible; i++)
                {
                    this.Chapters.Add(i);
                }

                this.SearchTypeIndex = 2;
            }
            else
            {
                // we must find the first chapter in the current book.
                int chapter = 0;
                for (int i = 0; i < this._currentBookNum; i++)
                {
                    chapter += BibleZtextReader.ChaptersInBook[i];
                }

                // add all the chapters up to the last chapter in the book.
                int lastChapterInBook = chapter + BibleZtextReader.ChaptersInBook[this._currentBookNum];
                for (int i = chapter; i < lastChapterInBook; i++)
                {
                    this.Chapters.Add(i);
                }

                this.SearchTypeIndex = 3;
            }

            var source = (BibleZtextReader)App.OpenWindows[(int)openWindowIndex].State.Source;
            this.SourceSearch = new SearchReader(
                source.Serial.Path, source.Serial.Iso2DigitLangCode, source.Serial.IsIsoEncoding);

            var tmr = new Timer(this.OnTimerTick);
            tmr.Change(300, Timeout.Infinite);
        }

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

            this.SourceSearch.DoSearch(
                App.DisplaySettings, 
                SearchingObject.SearchTypeIndex, 
                SearchingObject.TextToSearch, 
                SearchingObject.IsIgnoreCase, 
                SearchingObject.Chapters, 
                this.ShowProgress);
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
                out this._currentBookNum, out absoluteChaptNum, out dummy2, out verseNum, out fullName, out text);
            this.Chapter.Content = fullName;

            this.PageTitle.Text = Translations.Translate("Search");
            this.butSearch.Content = Translations.Translate("Search");
            this.SearchWhereText.Text = Translations.Translate("Search where");
            this.wholeBible.Content = Translations.Translate("Whole bible");
            this.oldTestement.Content = Translations.Translate("The Old Testement");
            this.newTEstement.Content = Translations.Translate("The New Testement");
            this.SearchByText.Text = Translations.Translate("Search conditions");
            this.OneOrMoreWords.Content = Translations.Translate("One or more words");
            this.AllWords.Content = Translations.Translate("All words (maximum 3 words)");
            this.ExactMatch.Content = Translations.Translate("Exact match") + " (Regular Expressions)";
            this.IgnoreCase.Header = Translations.Translate("Case insensitive");
            this.butHelp.Content = Translations.Translate("Help") + " (Regular Expressions)";
        }

        #endregion
    }
}