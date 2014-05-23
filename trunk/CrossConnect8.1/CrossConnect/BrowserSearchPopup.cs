// <copyright file="BrowserSearchPopup.cs" company="Thomas Dilts">
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
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Text;

    using CrossConnect.readers;

    using Sword.reader;

    using Windows.Storage;
    using Windows.Storage.Streams;
    using Windows.UI.Popups;
    using Windows.UI.Xaml;

    public sealed partial class BrowserTitledWindow
    {
        #region Static Fields

        public static BrowserTitledWindow SearchingObject;

        #endregion

        #region Fields

        public List<int> Chapters = new List<int>();

        public bool IsAbort;

        public bool IsIgnoreCase;

        public bool IsSearchFinished;

        public bool IsSearchFinishedReported;

        public int SearchTypeIndex;

        public bool IsFastSearch = true;

        public IBrowserTextSource SourceSearch;

        public string TextToSearch = string.Empty;

        private string _currentBookName;

        private int _numFoundVerses;

        private double _percent = 1;

        #endregion

        #region Public Methods and Operators

        public void ShowControls(bool isShow)
        {
            if (!isShow)
            {
                this.RealSearchPopup.Width = this.RealSearchPopup.ActualWidth;
                this.RealSearchPopup.Height = this.RealSearchPopup.ActualHeight;
                this.RealSearchPopup.MinWidth = this.RealSearchPopup.ActualWidth;
                this.RealSearchPopup.MinHeight = this.RealSearchPopup.ActualHeight;
                this.ProgressBar1.MinWidth = this.RealSearchPopup.ActualWidth;
            }
            Visibility isVis = isShow ? Visibility.Visible : Visibility.Collapsed;
            this.IsFastSearch = (this.FastSearch.IsChecked != null && (bool)this.FastSearch.IsChecked);
            this.SearchText.Visibility = isVis;
            this.FastSearch.Visibility = isVis;
            this.butFastHelp.Visibility = !this.IsFastSearch ? Visibility.Collapsed : isVis;
            this.SlowSearch.Visibility = isVis;
            this.SearchWhereText.Visibility = (this._state.Source is RawGenTextReader) ? Visibility.Collapsed : isVis;
            this.WholeBible.Visibility = (this._state.Source is RawGenTextReader) ? Visibility.Collapsed : isVis;
            this.OldTestement.Visibility = (this._state.Source is RawGenTextReader) ? Visibility.Collapsed : isVis;
            this.NewTEstement.Visibility = (this._state.Source is RawGenTextReader) ? Visibility.Collapsed : isVis;
            this.Chapter.Visibility = (this._state.Source is RawGenTextReader) ? Visibility.Collapsed : isVis;
            this.IgnoreCase.Visibility = this.IsFastSearch ? Visibility.Collapsed : isVis;
            this.ProgressBar1.Value = 0;
            this.ProgressBar1.Visibility = isShow ? Visibility.Collapsed : Visibility.Visible;
            this.butSearch.Visibility = isVis;
            this.ButHelp.Visibility = this.IsFastSearch ? Visibility.Collapsed : isVis;
            this.PageTitle.Text = Translations.Translate("Search");
            this.SearchByText.Visibility = this.IsFastSearch ? Visibility.Collapsed : isVis;
            this.OneOrMoreWords.Visibility = this.IsFastSearch ? Visibility.Collapsed : isVis;
            this.AllWords.Visibility = this.IsFastSearch ? Visibility.Collapsed : isVis;
            this.ExactMatch.Visibility = this.IsFastSearch ? Visibility.Collapsed : isVis;
            this.LayoutRoot.UpdateLayout();
        }

        public void ShowProgress(double percent, int totalFound, bool isAbort, bool isFinished)
        {
            SearchingObject.IsSearchFinished = isFinished;
            SearchingObject._percent = percent;
            SearchingObject.IsAbort = isAbort;

            this.UpdateProgressBar();

            this._numFoundVerses = totalFound;
        }

        public async void UpdateProgressBar()
        {
            if (SearchingObject.IsSearchFinishedReported)
            {
                // this is a delayed reporting that must be ignored.
                return;
            }

            SearchingObject.ProgressBar1.Value = this._percent;
            SearchingObject.ProgressBar1.UpdateLayout();
            this.PageTitle.Text = Translations.Translate("Search") + "; " + Translations.Translate("Found") + "; "
                                  + this._numFoundVerses;
            if (this.IsSearchFinished)
            {
                SearchingObject.IsSearchFinishedReported = true;
                if (this._numFoundVerses == 0)
                {
                    var dialog = new MessageDialog(Translations.Translate("Nothing found"));
                    await dialog.ShowAsync();
                    this.ShowControls(true);
                }
                else
                {
                    if (this.IsAbort)
                    {
                        var dialog = new MessageDialog(Translations.Translate("Too many found. Search stopped"));
                        await dialog.ShowAsync();
                    }

                    App.AddWindow(
                        this._state.BibleToLoad,
                        this._state.BibleDescription,
                        WindowType.WindowSearch,
                        this._state.HtmlFontSize,
                        this._state.Font,
                        _state.Window,
                        this.SourceSearch);
                    this.RealSearchPopup.IsOpen = false;
                }
            }
        }

        #endregion

        #region Methods

        private async void ButHelpClick(object sender, RoutedEventArgs e)
        {
            Assembly assem = Assembly.Load(new AssemblyName("CrossConnect"));
            Stream stream = assem.GetManifestResourceStream(
                "CrossConnect.Properties.regex.html");
            var buffer = new byte[10000];
            var bytesRead = stream.Read(buffer, 0, 10000);
            Array.Resize(ref buffer,bytesRead);
            webBrowserHelp.NavigateToString(Encoding.UTF8.GetString(buffer, 0, buffer.Count()));
            webBrowserHelp.UpdateLayout();
            MakeSurePopupIsOnScreen(
                this.webBrowserHelp.ActualHeight,
                this.webBrowserHelp.ActualWidth,
                this.ButSearch,
                this,
                this.HelpPopup);
            HelpPopup.IsOpen = true;
            //PhoneApplicationService.Current.State["HelpWindowFileToLoad"] = "CrossConnect.Properties.regex.html";
            //PhoneApplicationService.Current.State["HelpWindowTitle"] = Translations.Translate("Help")
            //                                                           + "(Regular Expressions)";

            //NavigationService.Navigate(new Uri("/Help.xaml", UriKind.Relative));
        }

        private async void ButSearchClick(object sender, RoutedEventArgs e)
        {
            this.Chapters.Clear();
            this.ShowControls(false);
            SearchingObject = this;
            this.IsIgnoreCase = this.IgnoreCase.IsOn;

            this.TextToSearch = this.SearchText.Text;
            string[] parts = this.TextToSearch.Split(" ,".ToCharArray());
            var goodParts = new List<string>();
            for (int j = 0; j < parts.Count(); j++)
            {
                if (!string.IsNullOrEmpty(parts[j]))
                {
                    goodParts.Add(parts[j]);
                }
            }

            if (goodParts.Count() > 1 && !this.IsFastSearch)
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
            this.IsFastSearch = (this.FastSearch.IsChecked != null && (bool)this.FastSearch.IsChecked);
            if (this._state.Source is BibleZtextReader)
            {
                var source = (BibleZtextReader)this._state.Source;
                if (this.WholeBible.IsChecked != null && (bool)this.WholeBible.IsChecked)
                {
                    for (int i = 0; i < source.canon.GetNumChaptersInBible(); i++)
                    {
                        this.Chapters.Add(i);
                    }

                    this.SearchTypeIndex = 0;
                }
                else if (this.OldTestement.IsChecked != null && (bool)this.OldTestement.IsChecked)
                {
                    for (int i = 0; i < source.canon.GetNumChaptersInOldTestement(); i++)
                    {
                        this.Chapters.Add(i);
                    }

                    this.SearchTypeIndex = 1;
                }
                else if (this.NewTEstement.IsChecked != null && (bool)this.NewTEstement.IsChecked)
                {
                    for (int i = source.canon.GetNumChaptersInOldTestement(); i < source.canon.GetNumChaptersInBible(); i++)
                    {
                        this.Chapters.Add(i);
                    }

                    this.SearchTypeIndex = 2;
                }
                else
                {
                    // we must find the first chapter in the current book.
                    var book = source.canon.BookByShortName[this._currentBookName];

                    // add all the chapters up to the last chapter in the book.
                    for (int i = book.VersesInChapterStartIndex; i < book.VersesInChapterStartIndex + book.NumberOfChapters; i++)
                    {
                        this.Chapters.Add(i);
                    }

                    this.SearchTypeIndex = 3;
                }

                this.SourceSearch = new SearchReader(
                    source.Serial.Path, source.Serial.Iso2DigitLangCode, source.Serial.IsIsoEncoding, source.Serial.CipherKey, source.Serial.ConfigPath, source.Serial.Versification);
                await ((BibleZtextReader)this.SourceSearch).Initialize();
            } 
            else if (this._state.Source is RawGenTextReader)
            {
                this.Chapters = new List<int>();
                for (int i = 0; i < ((RawGenTextReader)this._state.Source).Chapters.Count; i++)
                {
                    this.Chapters.Add(i);
                }

                this.SearchTypeIndex = 0; 
                var source = (RawGenTextReader)this._state.Source;
                this.SourceSearch = new RawGenSearchReader(
                    source.Serial.Path, source.Serial.Iso2DigitLangCode, source.Serial.IsIsoEncoding);
                await ((RawGenTextReader)this.SourceSearch).Initialize();
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
                    if (!await Hoot.File.Exists(Path.Combine(path, index)))
                    {
                        existsIndexes = false;
                        break;
                    }
                }
                if (!existsIndexes)
                {
                    var dialog = new MessageDialog(Translations.Translate("The first time you do a fast search it takes a long time. Afterwards searching is very quick. Do you want to continue?"));
                    dialog.Commands.Add(
                        new UICommand(
                            Translations.Translate("Yes"),
                            (UICommandInvokedHandler) =>
                            {
                                var tmr = new DispatcherTimer();
                                tmr.Tick += this.OnSearchTimerTick;
                                tmr.Interval = TimeSpan.FromMilliseconds(300);
                                tmr.Start();
                            }));
                    dialog.Commands.Add(new UICommand(Translations.Translate("Cancel")));
                    await dialog.ShowAsync();
                }
                else
                {
                    var tmr = new DispatcherTimer();
                    tmr.Tick += this.OnSearchTimerTick;
                    tmr.Interval = TimeSpan.FromMilliseconds(300);
                    tmr.Start();
                }
            }
            else
            {
                var tmr = new DispatcherTimer();
                tmr.Tick += this.OnSearchTimerTick;
                tmr.Interval = TimeSpan.FromMilliseconds(300);
                tmr.Start();
            }
        }

        private void OnSearchTimerTick(object sender, object e)
        {
            ((DispatcherTimer)sender).Stop();
            RealSearchPopup.IsOpen = true;
            ShowControls(false);
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

        private void RealSearchPopupLoaded(object sender, RoutedEventArgs e)
        {
            string fullName;
            string text;
            int verseNum;
            int chaptNum;
            this._state.Source.GetInfo(Translations.IsoLanguageCode, 
                out this._currentBookName, out chaptNum, out verseNum, out fullName, out text);
            this.Chapter.Content = fullName;

            this.PageTitle.Text = Translations.Translate("Search");
            this.butSearch.Content = Translations.Translate("Search");
            this.SearchWhereText.Text = Translations.Translate("Search where");
            this.butFastHelp.Content = Translations.Translate("Help");
            this.SlowSearch.Content = Translations.Translate("Slow search");
            this.FastSearch.Content = Translations.Translate("Fast search");
            this.WholeBible.Content = Translations.Translate("Whole bible");
            this.OldTestement.Content = Translations.Translate("The Old Testement");
            this.NewTEstement.Content = Translations.Translate("The New Testement");
            this.SearchByText.Text = Translations.Translate("Search conditions");
            this.OneOrMoreWords.Content = Translations.Translate("One or more words");
            this.AllWords.Content = Translations.Translate("All words (maximum 3 words)");
            this.ExactMatch.Content = Translations.Translate("Exact match") + " (Regular Expressions)";
            this.IgnoreCase.Header = Translations.Translate("Case insensitive");
            this.IgnoreCase.IsOn = true;
            this.ButHelp.Content = Translations.Translate("Help") + " (Regular Expressions)";
        }

        #endregion

        private void ButFastHelp_OnClick(object sender, RoutedEventArgs e)
        {
            //var dialog = new MessageDialog(Translations.Translate("Fast search help text"));
            //dialog.ShowAsync();

            webBrowserHelp.NavigateToString(Translations.Translate("Fast search help text"));
            webBrowserHelp.UpdateLayout();
            MakeSurePopupIsOnScreen(
                this.webBrowserHelp.ActualHeight,
                this.webBrowserHelp.ActualWidth,
                this.ButSearch,
                this,
                this.HelpPopup);
            HelpPopup.IsOpen = true;
        }

        private void FastSearch_OnClick(object sender, RoutedEventArgs e)
        {
            ShowControls(true);
        }
    }
}