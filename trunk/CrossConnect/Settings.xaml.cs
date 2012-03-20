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

    /// <summary>
    /// The settings.
    /// </summary>
    public partial class Settings
    {
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

            App.DisplaySettings.HebrewDictionaryLink = hebrewDictionaryLink.Text;
            App.DisplaySettings.GreekDictionaryLink = greekDictionaryLink.Text;
            App.DisplaySettings.CustomBibleDownloadLinks = customBibleDownloadLink.Text;
            App.DisplaySettings.SoundLink = soundLink.Text;
            if (useInternetGreekHebrewDict.IsChecked != null)
            {
                App.DisplaySettings.UseInternetGreekHebrewDict = (bool)useInternetGreekHebrewDict.IsChecked;
            }

            App.RaiseBookmarkChangeEvent();
            App.RaiseHistoryChangeEvent();
            App.RaisePersonalNotesChangeEvent();

            // all the windows must be redrawn
            for (int i = App.OpenWindows.Count - 1; i >= 0; i--)
            {
                App.OpenWindows[i].ForceReload = true;
            }
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
            NumberOfScreens.Header = Translations.Translate("Number of screens");
            PageTitle.Text = Translations.Translate("Settings");
            butSetDefault.Content = Translations.Translate("Default settings");
            highlightMarkings.Header = Translations.Translate("Highlight text markings");
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

            bool successfulInitialize = false;
            while (!successfulInitialize)
            {
                try
                {
                    NumberOfScreens.SelectedIndex = App.DisplaySettings.NumberOfScreens - 1;
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

        #endregion Methods
    }
}