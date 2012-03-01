// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Settings.xaml.cs" company="">
//   
// </copyright>
// <summary>
//   The settings.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

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
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Settings"/> class.
        /// </summary>
        public Settings()
        {
            this.InitializeComponent();
        }

        #endregion

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
            App.DisplaySettings.NumberOfScreens = int.Parse(this.NumberOfScreens.SelectedItem.ToString());
            if (this.highlightMarkings.IsChecked != null)
            {
                App.DisplaySettings.HighlightMarkings = (bool)this.highlightMarkings.IsChecked;
            }

            if (this.wordsOfChristRed.IsChecked != null)
            {
                App.DisplaySettings.WordsOfChristRed = (bool)this.wordsOfChristRed.IsChecked;
            }

            if (this.smallVerseNumbers.IsChecked != null)
            {
                App.DisplaySettings.SmallVerseNumbers = (bool)this.smallVerseNumbers.IsChecked;
            }

            if (this.showNotePositions.IsChecked != null)
            {
                App.DisplaySettings.ShowNotePositions = (bool)this.showNotePositions.IsChecked;
            }

            if (this.showBookName.IsChecked != null)
            {
                App.DisplaySettings.ShowBookName = (bool)this.showBookName.IsChecked;
            }

            if (this.showChapterNumber.IsChecked != null)
            {
                App.DisplaySettings.ShowChapterNumber = (bool)this.showChapterNumber.IsChecked;
            }

            if (this.showVerseNumber.IsChecked != null)
            {
                App.DisplaySettings.ShowVerseNumber = (bool)this.showVerseNumber.IsChecked;
            }

            if (this.showStrongsNumbers.IsChecked != null)
            {
                App.DisplaySettings.ShowStrongsNumbers = (bool)this.showStrongsNumbers.IsChecked;
            }

            if (this.showMorphology.IsChecked != null)
            {
                App.DisplaySettings.ShowMorphology = (bool)this.showMorphology.IsChecked;
            }

            if (this.showHeadings.IsChecked != null)
            {
                App.DisplaySettings.ShowHeadings = (bool)this.showHeadings.IsChecked;
            }

            if (this.eachVerseNewLine.IsChecked != null)
            {
                App.DisplaySettings.EachVerseNewLine = (bool)this.eachVerseNewLine.IsChecked;
            }

            if (this.showAddedNotesByChapter.IsChecked != null)
            {
                App.DisplaySettings.ShowAddedNotesByChapter = (bool)this.showAddedNotesByChapter.IsChecked;
            }

            App.DisplaySettings.HebrewDictionaryLink = this.hebrewDictionaryLink.Text;
            App.DisplaySettings.GreekDictionaryLink = this.greekDictionaryLink.Text;
            App.DisplaySettings.CustomBibleDownloadLinks = this.customBibleDownloadLink.Text;
            App.DisplaySettings.SoundLink = this.soundLink.Text;
            if (this.useInternetGreekHebrewDict.IsChecked != null)
            {
                App.DisplaySettings.UseInternetGreekHebrewDict = (bool)this.useInternetGreekHebrewDict.IsChecked;
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
            this.NumberOfScreens.Header = Translations.Translate("Number of screens");
            this.PageTitle.Text = Translations.Translate("Settings");
            this.butSetDefault.Content = Translations.Translate("Default settings");
            this.highlightMarkings.Header = Translations.Translate("Highlight text markings");
            this.wordsOfChristRed.Header = Translations.Translate("Show the words of Jesus in red");
            this.smallVerseNumbers.Header = Translations.Translate("Use small verse numbers");
            this.showNotePositions.Header = Translations.Translate("Show note positions");
            this.showBookName.Header = Translations.Translate("Show the book name on each verse");
            this.showChapterNumber.Header = Translations.Translate("Show the chapter number on each verse");
            this.showVerseNumber.Header = Translations.Translate("Show the verse number on each verse");
            this.showStrongsNumbers.Header = Translations.Translate("Show Strong's numbers");
            this.showMorphology.Header = Translations.Translate("Show word morphology");
            this.showHeadings.Header = Translations.Translate("Show the headings");
            this.eachVerseNewLine.Header = Translations.Translate("Start each verse on a new line");
            this.showAddedNotesByChapter.Header = Translations.Translate("Show added notes by chapter");
            this.captionHebrewDictionaryLink.Text = Translations.Translate("Hebrew dictionary internet link");
            this.captionGreekDictionaryLink.Text = Translations.Translate("Greek dictionary internet link");
            this.captionCustomBibleDownloadLink.Text = Translations.Translate("Custom bible download addresses");
            this.captionSoundLink.Text = Translations.Translate("Talking bible internet link");
            this.useInternetGreekHebrewDict.Header = Translations.Translate("Use internet dictionaries");

            bool successfulInitialize = false;
            while (!successfulInitialize)
            {
                try
                {
                    this.NumberOfScreens.SelectedIndex = App.DisplaySettings.NumberOfScreens - 1;
                    this.highlightMarkings.IsChecked = App.DisplaySettings.HighlightMarkings;
                    this.wordsOfChristRed.IsChecked = App.DisplaySettings.WordsOfChristRed;
                    this.smallVerseNumbers.IsChecked = App.DisplaySettings.SmallVerseNumbers;
                    this.showNotePositions.IsChecked = App.DisplaySettings.ShowNotePositions;
                    this.showBookName.IsChecked = App.DisplaySettings.ShowBookName;
                    this.showChapterNumber.IsChecked = App.DisplaySettings.ShowChapterNumber;
                    this.showVerseNumber.IsChecked = App.DisplaySettings.ShowVerseNumber;
                    this.showStrongsNumbers.IsChecked = App.DisplaySettings.ShowStrongsNumbers;
                    this.showMorphology.IsChecked = App.DisplaySettings.ShowMorphology;
                    this.showHeadings.IsChecked = App.DisplaySettings.ShowHeadings;
                    this.eachVerseNewLine.IsChecked = App.DisplaySettings.EachVerseNewLine;
                    this.showAddedNotesByChapter.IsChecked = App.DisplaySettings.ShowAddedNotesByChapter;
                    this.hebrewDictionaryLink.Text = App.DisplaySettings.HebrewDictionaryLink;
                    this.greekDictionaryLink.Text = App.DisplaySettings.GreekDictionaryLink;
                    this.customBibleDownloadLink.Text = App.DisplaySettings.CustomBibleDownloadLinks;
                    this.soundLink.Text = App.DisplaySettings.SoundLink;
                    this.useInternetGreekHebrewDict.IsChecked = App.DisplaySettings.UseInternetGreekHebrewDict;

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
            this.AutoRotatePageLoaded(null, null);
        }

        #endregion
    }
}