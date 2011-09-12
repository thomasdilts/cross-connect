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
/// <copyright file="Settings.cs" company="Thomas Dilts">
///     Thomas Dilts. All rights reserved.
/// </copyright>
/// <author>Thomas Dilts</author>
namespace CrossConnect
{
    using System.Windows;
    using System;
    using System.Diagnostics;

    public partial class Settings : AutoRotatePage
    {
        #region Constructors

        public Settings()
        {
            InitializeComponent();
        }

        #endregion Constructors

        #region Methods

        private void AutoRotatePage_BackKeyPress(object sender, System.ComponentModel.CancelEventArgs e)
        {
            App.displaySettings.highlightMarkings = (bool)highlightMarkings.IsChecked;
            App.displaySettings.wordsOfChristRed = (bool)wordsOfChristRed.IsChecked;
            App.displaySettings.smallVerseNumbers = (bool)smallVerseNumbers.IsChecked;
            App.displaySettings.showNotePositions = (bool)showNotePositions.IsChecked;
            App.displaySettings.showBookName = (bool)showBookName.IsChecked;
            App.displaySettings.showChapterNumber = (bool)showChapterNumber.IsChecked;
            App.displaySettings.showVerseNumber = (bool)showVerseNumber.IsChecked;
            App.displaySettings.showStrongsNumbers = (bool)showStrongsNumbers.IsChecked;
            App.displaySettings.showMorphology = (bool)showMorphology.IsChecked;
            App.displaySettings.showHeadings = (bool)showHeadings.IsChecked;
            App.displaySettings.eachVerseNewLine = (bool)eachVerseNewLine.IsChecked;
            App.displaySettings.showAddedNotesByChapter = (bool)showAddedNotesByChapter.IsChecked;
            App.displaySettings.hebrewDictionaryLink = hebrewDictionaryLink.Text;
            App.displaySettings.greekDictionaryLink = greekDictionaryLink.Text;
            App.displaySettings.customBibleDownloadLinks = customBibleDownloadLink.Text;
            App.displaySettings.soundLink = soundLink.Text;
            App.RaiseBookmarkChangeEvent();
            App.RaiseHistoryChangeEvent();
            App.RaisePersonalNotesChangeEvent();
        }

        private void AutoRotatePage_Loaded(object sender, RoutedEventArgs e)
        {
            PageTitle.Text = Translations.translate("Settings");
            butSetDefault.Content = Translations.translate("Default settings");
            highlightMarkings.Header = Translations.translate("Highlight text markings");
            wordsOfChristRed.Header = Translations.translate("Show the words of Jesus in red");
            smallVerseNumbers.Header = Translations.translate("Use small verse numbers");
            showNotePositions.Header = Translations.translate("Show note positions");
            showBookName.Header = Translations.translate("Show the book name on each verse");
            showChapterNumber.Header = Translations.translate("Show the chapter number on each verse");
            showVerseNumber.Header = Translations.translate("Show the verse number on each verse");
            showStrongsNumbers.Header = Translations.translate("Show Strong's numbers");
            showMorphology.Header = Translations.translate("Show word morphology");
            showHeadings.Header = Translations.translate("Show the headings");
            eachVerseNewLine.Header = Translations.translate("Start each verse on a new line");
            showAddedNotesByChapter.Header = Translations.translate("Show added notes by chapter");
            captionHebrewDictionaryLink.Text = Translations.translate("Hebrew dictionary internet link");
            captionGreekDictionaryLink.Text = Translations.translate("Greek dictionary internet link");
            captionCustomBibleDownloadLink.Text = Translations.translate("Custom bible download addresses");
            captionSoundLink.Text = Translations.translate("Talking bible internet link");

            bool successfulInitialize = false;
            while (!successfulInitialize)
            {
                try
                {
                    highlightMarkings.IsChecked = App.displaySettings.highlightMarkings;
                    wordsOfChristRed.IsChecked = App.displaySettings.wordsOfChristRed;
                    smallVerseNumbers.IsChecked = App.displaySettings.smallVerseNumbers;
                    showNotePositions.IsChecked = App.displaySettings.showNotePositions;
                    showBookName.IsChecked = App.displaySettings.showBookName;
                    showChapterNumber.IsChecked = App.displaySettings.showChapterNumber;
                    showVerseNumber.IsChecked = App.displaySettings.showVerseNumber;
                    showStrongsNumbers.IsChecked = App.displaySettings.showStrongsNumbers;
                    showMorphology.IsChecked = App.displaySettings.showMorphology;
                    showHeadings.IsChecked = App.displaySettings.showHeadings;
                    eachVerseNewLine.IsChecked = App.displaySettings.eachVerseNewLine;
                    showAddedNotesByChapter.IsChecked = App.displaySettings.showAddedNotesByChapter;
                    hebrewDictionaryLink.Text = App.displaySettings.hebrewDictionaryLink;
                    greekDictionaryLink.Text = App.displaySettings.greekDictionaryLink;
                    customBibleDownloadLink.Text = App.displaySettings.customBibleDownloadLinks;
                    soundLink.Text = App.displaySettings.soundLink;

                    successfulInitialize = true;
                }
                catch (Exception eee)
                {
                    Debug.WriteLine("null in probably: " + eee.Message + "; " + eee.StackTrace);
                    //we got some null value in that crashes everything.  Lets just reset it all.
                    //we could try to repair it but lets take no chances.
                    App.displaySettings = new SwordBackend.DisplaySettings();
                }
            }
        }

        private void butSetDefault_Click(object sender, RoutedEventArgs e)
        {
            App.displaySettings = new SwordBackend.DisplaySettings();
            AutoRotatePage_Loaded(null, null);
        }

        #endregion Methods
    }
}