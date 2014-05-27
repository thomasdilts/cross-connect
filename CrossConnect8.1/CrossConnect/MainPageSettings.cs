// <copyright file="MainPageSettings.cs" company="Thomas Dilts">
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
    using System.Diagnostics;

    using Sword.reader;

    using Windows.Foundation;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml.Controls.Primitives;
    using Windows.UI.Xaml.Media;
    using Windows.ApplicationModel.DataTransfer;
    using Windows.UI.Popups;

    public sealed partial class MainPageSplit
    {
        #region Public Methods and Operators

        public static void SideBarShowPopup(
            Popup popup,
            Panel mainpane,
            ScrollViewer scroller = null,
            AppBar TopAppBar1 = null,
            AppBar BottomAppBar = null)
        {
            if (TopAppBar1 != null)
            {
                TopAppBar1.IsOpen = false;
            }

            if (BottomAppBar != null)
            {
                BottomAppBar.IsOpen = false;
            }

            popup.IsOpen = true;
            Rect windowBounds = Window.Current.Bounds;
            double popupWidth = windowBounds.Width / 4;
            const double MinimumDesiredWidth = 350;
            if (popupWidth < MinimumDesiredWidth)
            {
                double minWidth = windowBounds.Width * 4 / 5;
                popupWidth = (minWidth > MinimumDesiredWidth) ? MinimumDesiredWidth : minWidth;
            }
            mainpane.MinHeight = windowBounds.Height;
            mainpane.MinWidth = popupWidth;
            mainpane.MaxWidth = popupWidth;

            UIElement rootVisual = Window.Current.Content;

            GeneralTransform gt = popup.TransformToVisual(rootVisual);

            Point absolutePosition = gt.TransformPoint(new Point(0, 0));

            popup.HorizontalOffset = windowBounds.Width - absolutePosition.X - mainpane.MinWidth;
            popup.VerticalOffset = - absolutePosition.Y;
            if (scroller != null)
            {
                //GeneralTransform gtscroller = scroller.TransformToVisual(rootVisual);
                //Point scrollerPosition = gtscroller.TransformPoint(new Point(0, 0));
                scroller.MaxHeight = windowBounds.Height;// -scrollerPosition.Y * 2;
                scroller.MaxWidth = popupWidth;
            }
        }

        #endregion

        #region Methods

        private void ButSettingsDefault_OnClick(object sender, RoutedEventArgs e)
        {
            App.DisplaySettings = new DisplaySettings();
            this.SettingsPopup_OnOpened(null, null);
        }

        private void MenuSettingsClick(object sender, RoutedEventArgs e)
        {
            SideBarShowPopup(
                this.SettingsPopup,
                this.MainPaneSettingsPopup,
                this.scrollViewerSettings,
                this.TopAppBar1,
                this.BottomAppBar);
        }

        private void SettingsPopup_OnClosed(object sender, object e)
        {
            App.ShowUserInterface(true);
            App.DisplaySettings.HighlightMarkings = this.highlightMarkings.IsOn;
            App.DisplaySettings.WordsOfChristRed = this.wordsOfChristRed.IsOn;
            App.DisplaySettings.SmallVerseNumbers = this.smallVerseNumbers.IsOn;
            App.DisplaySettings.ShowNotePositions = this.showNotePositions.IsOn;
            App.DisplaySettings.ShowBookName = this.showBookName.IsOn;
            App.DisplaySettings.ShowChapterNumber = this.showChapterNumber.IsOn;
            App.DisplaySettings.ShowVerseNumber = this.showVerseNumber.IsOn;
            App.DisplaySettings.ShowStrongsNumbers = this.showStrongsNumbers.IsOn;
            App.DisplaySettings.ShowMorphology = this.showMorphology.IsOn;
            App.DisplaySettings.ShowHeadings = this.showHeadings.IsOn;
            App.DisplaySettings.EachVerseNewLine = this.eachVerseNewLine.IsOn;
            App.DisplaySettings.ShowAddedNotesByChapter = this.showAddedNotesByChapter.IsOn;
            App.DisplaySettings.HebrewDictionaryLink = this.hebrewDictionaryLink.Text;
            App.DisplaySettings.GreekDictionaryLink = this.greekDictionaryLink.Text;
            App.DisplaySettings.CustomBibleDownloadLinks = this.customBibleDownloadLink.Text;
            App.DisplaySettings.SoundLink = this.soundLink.Text;
            App.DisplaySettings.UseInternetGreekHebrewDict = this.useInternetGreekHebrewDict.IsOn;
            App.DisplaySettings.HighlightName1 = this.highlightName1.Text;
            App.DisplaySettings.HighlightName2 = this.highlightName2.Text;
            App.DisplaySettings.HighlightName3 = this.highlightName3.Text;
            App.DisplaySettings.HighlightName4 = this.highlightName4.Text;
            App.DisplaySettings.HighlightName5 = this.highlightName5.Text;
            App.DisplaySettings.HighlightName6 = this.highlightName6.Text;
            App.DisplaySettings.UseHighlights = this.useHighlighting.IsOn;
            App.DisplaySettings.SyncMediaVerses = this.SyncVerses.IsOn;
            App.DisplaySettings.AddLineBetweenNotes = this.AddLineBetweenNotes.IsOn;
            
            App.RaiseBookmarkChangeEvent();
            App.RaiseHistoryChangeEvent();
            App.RaisePersonalNotesChangeEvent();

            App.SavePersistantDisplaySettings();

            // all the windows must be redrawn
            for (int i = App.OpenWindows.Count - 1; i >= 0; i--)
            {
                App.OpenWindows[i].ForceReload = true;
                App.OpenWindows[i].UpdateBrowser(false);
            }
        }

        private void SettingsPopup_OnOpened(object sender, object e)
        {
            App.ShowUserInterface(false);
            this.SettingsTitle.Text = Translations.Translate("Settings");
            this.highlightMarkings.Header = Translations.Translate("Highlight text markings");
            this.wordsOfChristRed.Header = Translations.Translate("Show the words of Jesus in red");
            this.smallVerseNumbers.Header = Translations.Translate("Use small verse numbers");
            this.showNotePositions.Header = Translations.Translate("Show note positions");
            this.showBookName.Header = Translations.Translate("Show the book name on each verse");
            this.showChapterNumber.Header = Translations.Translate("Show the chapter number on each verse");
            this.showVerseNumber.Header = Translations.Translate("Show the verse number on each verse");
            this.showStrongsNumbers.Header = Translations.Translate("Show") + " \"Strong's numbers\"";
            this.showMorphology.Header = Translations.Translate("Show word morphology");
            this.showHeadings.Header = Translations.Translate("Show the headings");
            this.eachVerseNewLine.Header = Translations.Translate("Start each verse on a new line");
            this.showAddedNotesByChapter.Header = Translations.Translate("Show added notes by chapter");
            this.captionHebrewDictionaryLink.Text = Translations.Translate("Hebrew dictionary internet link");
            this.captionGreekDictionaryLink.Text = Translations.Translate("Greek dictionary internet link");
            this.captionCustomBibleDownloadLink.Text = Translations.Translate("Custom bible download addresses");
            this.captionSoundLink.Text = Translations.Translate("Talking bible internet link");
            this.useInternetGreekHebrewDict.Header = Translations.Translate("Use internet dictionaries");
            this.captionHightlight1.Text = Translations.Translate("Highlight name") + " 1";
            this.captionHightlight2.Text = Translations.Translate("Highlight name") + " 2";
            this.captionHightlight3.Text = Translations.Translate("Highlight name") + " 3";
            this.captionHightlight4.Text = Translations.Translate("Highlight name") + " 4";
            this.captionHightlight5.Text = Translations.Translate("Highlight name") + " 5";
            this.captionHightlight6.Text = Translations.Translate("Highlight name") + " 6";
            this.butSettingsDefault.Content = Translations.Translate("Default settings");
            this.useHighlighting.Header = Translations.Translate("Use highlighting");
            this.useRemoteStorage.Header = Translations.Translate("Use remote storage");
            this.SyncVerses.Header = Translations.Translate("Synchronize to every verse");
            this.AddLineBetweenNotes.Header = Translations.Translate("Add a line between notes");
            
            butExportBookmarksHighlightsAndNotes.Content = Translations.Translate("Copy bookmarks, highlights and notes to the clipboard");
            butImportBookmarksHighlightsAndNotes.Content = Translations.Translate("Import bookmarks, highlights and notes from the clipboard");

            bool successfulInitialize = false;
            while (!successfulInitialize)
            {
                try
                {
                    this.highlightMarkings.IsOn = App.DisplaySettings.HighlightMarkings;
                    this.wordsOfChristRed.IsOn = App.DisplaySettings.WordsOfChristRed;
                    this.smallVerseNumbers.IsOn = App.DisplaySettings.SmallVerseNumbers;
                    this.showNotePositions.IsOn = App.DisplaySettings.ShowNotePositions;
                    this.showBookName.IsOn = App.DisplaySettings.ShowBookName;
                    this.showChapterNumber.IsOn = App.DisplaySettings.ShowChapterNumber;
                    this.showVerseNumber.IsOn = App.DisplaySettings.ShowVerseNumber;
                    this.showStrongsNumbers.IsOn = App.DisplaySettings.ShowStrongsNumbers;
                    this.showMorphology.IsOn = App.DisplaySettings.ShowMorphology;
                    this.showHeadings.IsOn = App.DisplaySettings.ShowHeadings;
                    this.eachVerseNewLine.IsOn = App.DisplaySettings.EachVerseNewLine;
                    this.showAddedNotesByChapter.IsOn = App.DisplaySettings.ShowAddedNotesByChapter;
                    this.hebrewDictionaryLink.Text = App.DisplaySettings.HebrewDictionaryLink;
                    this.greekDictionaryLink.Text = App.DisplaySettings.GreekDictionaryLink;
                    this.customBibleDownloadLink.Text = App.DisplaySettings.CustomBibleDownloadLinks;
                    this.soundLink.Text = App.DisplaySettings.SoundLink;
                    this.highlightName1.Text = App.DisplaySettings.HighlightName1;
                    this.highlightName2.Text = App.DisplaySettings.HighlightName2;
                    this.highlightName3.Text = App.DisplaySettings.HighlightName3;
                    this.highlightName4.Text = App.DisplaySettings.HighlightName4;
                    this.highlightName5.Text = App.DisplaySettings.HighlightName5;
                    this.highlightName6.Text = App.DisplaySettings.HighlightName6;
                    this.useInternetGreekHebrewDict.IsOn = App.DisplaySettings.UseInternetGreekHebrewDict;
                    this.useHighlighting.IsOn = App.DisplaySettings.UseHighlights;
                    this.SyncVerses.IsOn = App.DisplaySettings.SyncMediaVerses;
                    this.AddLineBetweenNotes.IsOn = App.DisplaySettings.AddLineBetweenNotes;
                    
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

        private async void butImportBookmarksHighlightsAndNotes_Click(object sender, RoutedEventArgs e)
        {
            DataPackageView dataPackageView = Clipboard.GetContent();
            if (dataPackageView.Contains(StandardDataFormats.Text))
            {
                string message = Translations.Translate("Successful import");
                try
                {
                    string text = await dataPackageView.GetTextAsync();
                    Highlighter.FromString(text, "note", true, null, App.DailyPlan.PersonalNotesVersified);
                    Highlighter.FromString(text, "bookmark", false, App.PlaceMarkers.Bookmarks, null);
                    App.DisplaySettings.highlighter.FromString(text);
                }
                catch (Exception ex)
                {
                    message = Translations.Translate("Unsuccessful import") + "\n" + ex.Message;
                }

                var dialog =
                    new MessageDialog(message);
                dialog.ShowAsync();
            }
            App.RaiseBookmarkChangeEvent();
            App.RaisePersonalNotesChangeEvent();
            App.SavePersistantMarkers();
            App.SavePersistantHighlighting();
            App.StartTimerForSavingWindows();

        }

        private void butExportBookmarksHighlightsAndNotes_Click(object sender, RoutedEventArgs e)
        {
            var dataPackage = new DataPackage();
            dataPackage.SetText(
                    "<crossconnectbookmarksnoteshighlights>\n" +
                    App.DisplaySettings.highlighter.ToStringNoRoot() +
                    Highlighter.ExportMarkersDictionary("note",  App.DailyPlan.PersonalNotesVersified) +
                    Highlighter.ExportMarkersList("bookmark",  App.PlaceMarkers.Bookmarks) +
                    "</crossconnectbookmarksnoteshighlights>" 
                );
            try
            {
                // Set the DataPackage to clipboard. 
                Clipboard.SetContent(dataPackage);
            }
            catch (Exception ex)
            {
                // Copying data to Clipboard can potentially fail - for example, if another application is holding Clipboard open 
            }
        }

        #endregion
    }
}