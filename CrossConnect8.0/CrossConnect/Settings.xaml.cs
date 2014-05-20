#region Header

// <copyright file="Settings.xaml.cs" company="Thomas Dilts">
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
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Windows;

    using Sword.reader;
    using Windows.ApplicationModel.DataTransfer;

    /// <summary>
    /// The settings.
    /// </summary>
    public partial class Settings
    {
        private bool _isInThisWindow = false;
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Settings"/> class.
        /// </summary>
        public Settings()
        {
            InitializeComponent();
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// The auto rotate page back key press.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void AutoRotatePageBackKeyPress(object sender, CancelEventArgs e)
        {
            App.DisplaySettings.NumberOfScreens = int.Parse(NumberOfScreens.SelectedItem.ToString());
            if (show2titleRows.IsChecked != null)
            {
                App.DisplaySettings.Show2titleRows = (bool)show2titleRows.IsChecked;
            }

            if (highlightMarkings.IsChecked != null)
            {
                App.DisplaySettings.HighlightMarkings = (bool)highlightMarkings.IsChecked;
            }

            if (wordsOfChristRed.IsChecked != null)
            {
                App.DisplaySettings.WordsOfChristRed = (bool)wordsOfChristRed.IsChecked;
            }

            if (smallVerseNumbers.IsChecked != null)
            {
                App.DisplaySettings.SmallVerseNumbers = (bool)smallVerseNumbers.IsChecked;
            }

            if (showNotePositions.IsChecked != null)
            {
                App.DisplaySettings.ShowNotePositions = (bool)showNotePositions.IsChecked;
            }

            if (showBookName.IsChecked != null)
            {
                App.DisplaySettings.ShowBookName = (bool)showBookName.IsChecked;
            }

            if (showChapterNumber.IsChecked != null)
            {
                App.DisplaySettings.ShowChapterNumber = (bool)showChapterNumber.IsChecked;
            }

            if (showVerseNumber.IsChecked != null)
            {
                App.DisplaySettings.ShowVerseNumber = (bool)showVerseNumber.IsChecked;
            }

            if (showStrongsNumbers.IsChecked != null)
            {
                App.DisplaySettings.ShowStrongsNumbers = (bool)showStrongsNumbers.IsChecked;
            }

            if (showMorphology.IsChecked != null)
            {
                App.DisplaySettings.ShowMorphology = (bool)showMorphology.IsChecked;
            }

            if (showHeadings.IsChecked != null)
            {
                App.DisplaySettings.ShowHeadings = (bool)showHeadings.IsChecked;
            }

            if (eachVerseNewLine.IsChecked != null)
            {
                App.DisplaySettings.EachVerseNewLine = (bool)eachVerseNewLine.IsChecked;
            }

            if (showAddedNotesByChapter.IsChecked != null)
            {
                App.DisplaySettings.ShowAddedNotesByChapter = (bool)showAddedNotesByChapter.IsChecked;
            }

            if (this.SyncVerses.IsChecked != null)
            {
                App.DisplaySettings.SyncMediaVerses = (bool)SyncVerses.IsChecked;
            }

            if (this.useHighlighting.IsChecked != null)
            {
                App.DisplaySettings.UseHighlights = (bool)useHighlighting.IsChecked;
            }
            App.DisplaySettings.HighlightName1 = this.highlightName1.Text;
            App.DisplaySettings.HighlightName2 = this.highlightName2.Text;
            App.DisplaySettings.HighlightName3 = this.highlightName3.Text;
            App.DisplaySettings.HighlightName4 = this.highlightName4.Text;
            App.DisplaySettings.HighlightName5 = this.highlightName5.Text;
            App.DisplaySettings.HighlightName6 = this.highlightName6.Text;

            App.DisplaySettings.HebrewDictionaryLink = hebrewDictionaryLink.Text;
            App.DisplaySettings.GreekDictionaryLink = greekDictionaryLink.Text;
            App.DisplaySettings.CustomBibleDownloadLinks = customBibleDownloadLink.Text;
            App.DisplaySettings.SoundLink = soundLink.Text;
            if (useInternetGreekHebrewDict.IsChecked != null)
            {
                App.DisplaySettings.UseInternetGreekHebrewDict = (bool)useInternetGreekHebrewDict.IsChecked;
            }
            if (AddLineBetweenNotes.IsChecked != null)
            {
                App.DisplaySettings.AddLineBetweenNotes = (bool)AddLineBetweenNotes.IsChecked;
            }
            
            App.RaiseBookmarkChangeEvent();
            App.RaiseHistoryChangeEvent();
            App.RaisePersonalNotesChangeEvent();

            // all the windows must be redrawn
            for (int i = App.OpenWindows.Count - 1; i >= 0; i--)
            {
                App.OpenWindows[i].ForceReload = true;
            }
            App.SavePersistantDisplaySettings();
            _isInThisWindow = false;
        }

        /// <summary>
        /// The auto rotate page loaded.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void AutoRotatePageLoaded(object sender, RoutedEventArgs e)
        {
            if (_isInThisWindow)
            {
                return;
            }

            _isInThisWindow = true;

            NumberOfScreens.Header = Translations.Translate("Number of screens");
            PageTitle.Text = Translations.Translate("Settings");
            butSetDefault.Content = Translations.Translate("Default settings");
            highlightMarkings.Header = Translations.Translate("Highlight text markings");
            show2titleRows.Header = Translations.Translate("Show two title rows in each window");
            wordsOfChristRed.Header = Translations.Translate("Show the words of Jesus in red");
            smallVerseNumbers.Header = Translations.Translate("Use small verse numbers");
            showNotePositions.Header = Translations.Translate("Show note positions");
            showBookName.Header = Translations.Translate("Show the book name on each verse");
            showChapterNumber.Header = Translations.Translate("Show the chapter number on each verse");
            showVerseNumber.Header = Translations.Translate("Show the verse number on each verse");
            showStrongsNumbers.Header = Translations.Translate("Show Strong's numbers");
            showMorphology.Header = Translations.Translate("Show word morphology");
            showHeadings.Header = Translations.Translate("Show the headings");
            eachVerseNewLine.Header = Translations.Translate("Start each verse on a new line");
            showAddedNotesByChapter.Header = Translations.Translate("Show added notes by chapter");
            captionHebrewDictionaryLink.Text = Translations.Translate("Hebrew dictionary internet link");
            captionGreekDictionaryLink.Text = Translations.Translate("Greek dictionary internet link");
            captionCustomBibleDownloadLink.Text = Translations.Translate("Custom bible download addresses");
            captionSoundLink.Text = Translations.Translate("Talking bible internet link");
            useInternetGreekHebrewDict.Header = Translations.Translate("Use internet dictionaries");
            this.captionHightlight1.Text = Translations.Translate("Highlight name") + " 1";
            this.captionHightlight2.Text = Translations.Translate("Highlight name") + " 2";
            this.captionHightlight3.Text = Translations.Translate("Highlight name") + " 3";
            this.captionHightlight4.Text = Translations.Translate("Highlight name") + " 4";
            this.captionHightlight5.Text = Translations.Translate("Highlight name") + " 5";
            this.captionHightlight6.Text = Translations.Translate("Highlight name") + " 6";
            this.useHighlighting.Header = Translations.Translate("Use highlighting");
            this.SyncVerses.Header = Translations.Translate("Synchronize to every verse");
            this.AddLineBetweenNotes.Header = Translations.Translate("Add a new line between notes");
            
            butExportBookmarksHighlightsAndNotes.Content = Translations.Translate("Copy bookmarks, highlights and notes to the clipboard");
            butImportBookmarksHighlightsAndNotes.Content = Translations.Translate("Import bookmarks, highlights and notes after you paste the clipboard into the box below");

            bool successfulInitialize = false;
            while (!successfulInitialize)
            {
                try
                {
                    NumberOfScreens.SelectedIndex = App.DisplaySettings.NumberOfScreens - 1;
                    show2titleRows.IsChecked = App.DisplaySettings.Show2titleRows;
                    highlightMarkings.IsChecked = App.DisplaySettings.HighlightMarkings;
                    wordsOfChristRed.IsChecked = App.DisplaySettings.WordsOfChristRed;
                    smallVerseNumbers.IsChecked = App.DisplaySettings.SmallVerseNumbers;
                    showNotePositions.IsChecked = App.DisplaySettings.ShowNotePositions;
                    showBookName.IsChecked = App.DisplaySettings.ShowBookName;
                    showChapterNumber.IsChecked = App.DisplaySettings.ShowChapterNumber;
                    showVerseNumber.IsChecked = App.DisplaySettings.ShowVerseNumber;
                    showStrongsNumbers.IsChecked = App.DisplaySettings.ShowStrongsNumbers;
                    showMorphology.IsChecked = App.DisplaySettings.ShowMorphology;
                    showHeadings.IsChecked = App.DisplaySettings.ShowHeadings;
                    eachVerseNewLine.IsChecked = App.DisplaySettings.EachVerseNewLine;
                    showAddedNotesByChapter.IsChecked = App.DisplaySettings.ShowAddedNotesByChapter;
                    hebrewDictionaryLink.Text = App.DisplaySettings.HebrewDictionaryLink;
                    greekDictionaryLink.Text = App.DisplaySettings.GreekDictionaryLink;
                    customBibleDownloadLink.Text = App.DisplaySettings.CustomBibleDownloadLinks;
                    soundLink.Text = App.DisplaySettings.SoundLink;
                    useInternetGreekHebrewDict.IsChecked = App.DisplaySettings.UseInternetGreekHebrewDict;
                    this.highlightName1.Text = App.DisplaySettings.HighlightName1;
                    this.highlightName2.Text = App.DisplaySettings.HighlightName2;
                    this.highlightName3.Text = App.DisplaySettings.HighlightName3;
                    this.highlightName4.Text = App.DisplaySettings.HighlightName4;
                    this.highlightName5.Text = App.DisplaySettings.HighlightName5;
                    this.highlightName6.Text = App.DisplaySettings.HighlightName6;
                    this.useHighlighting.IsChecked = App.DisplaySettings.UseHighlights;
                    this.SyncVerses.IsChecked = App.DisplaySettings.SyncMediaVerses;
                    this.AddLineBetweenNotes.IsChecked = App.DisplaySettings.AddLineBetweenNotes;
                    
                    successfulInitialize = true;
                }
                catch (Exception eee)
                {
                    Debug.WriteLine("null in probably: " + eee.Message + "; " + eee.StackTrace);

                    // we got some null value in that crashes everything.  Lets just reset it all.
                    // we could try to repair it but lets take no chances.
                    App.DisplaySettings = new DisplaySettings();
                }
            }
        }

        /// <summary>
        /// The but set default click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void ButSetDefaultClick(object sender, RoutedEventArgs e)
        {
            App.DisplaySettings = new DisplaySettings();
            AutoRotatePageLoaded(null, null);
        }

        private void butImportBookmarksHighlightsAndNotes_Click(object sender, RoutedEventArgs e)
        {
            var text = this.ImportTextBox.Text;
            if (!string.IsNullOrEmpty(text))
            {
                string message = Translations.Translate("Successful import");
                try
                {
                    Highlighter.FromString(text, "note", true, null, App.DailyPlan.PersonalNotesVersified);
                    Highlighter.FromString(text, "bookmark", false, App.PlaceMarkers.Bookmarks, null);
                    App.DisplaySettings.highlighter.FromString(text);
                }
                catch (Exception ex)
                {
                    message = Translations.Translate("Unsuccessful import") + "\n" + ex.Message;
                }
                MessageBox.Show(message);
            }
            App.RaiseBookmarkChangeEvent();
            App.RaisePersonalNotesChangeEvent();
            App.SavePersistantMarkers();
            App.SavePersistantHighlighting();
            App.StartTimerForSavingWindows();

        }

        private void butExportBookmarksHighlightsAndNotes_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Set the DataPackage to clipboard. 
                Clipboard.SetText("<crossconnectbookmarksnoteshighlights>\n" +
                    App.DisplaySettings.highlighter.ToStringNoRoot() +
                    Highlighter.ExportMarkersDictionary("note", App.DailyPlan.PersonalNotesVersified) +
                    Highlighter.ExportMarkersList("bookmark", App.PlaceMarkers.Bookmarks) +
                    "</crossconnectbookmarksnoteshighlights>");
            }
            catch (Exception ex)
            {
                // Copying data to Clipboard can potentially fail - for example, if another application is holding Clipboard open 
            }
        }


        #endregion Methods

    }
}