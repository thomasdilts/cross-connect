// <copyright file="BrowserHighlight.cs" company="Thomas Dilts">
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
    using System.Threading.Tasks;

    using Sword.reader;

    using Windows.ApplicationModel.DataTransfer;
    using Windows.System;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Media;

    public sealed partial class BrowserTitledWindow
    {

        #region Methods

        private async void Highlight_OnClick()
        {
            this.HighlightPopup.IsOpen = true;
            MainPageSplit.SideBarShowPopup(this.HighlightPopup, this.MainPaneHighlightPopup);
            this.SelectHighlight1.Content = App.DisplaySettings.HighlightName1;
            this.SelectHighlight2.Content = App.DisplaySettings.HighlightName2;
            this.SelectHighlight3.Content = App.DisplaySettings.HighlightName3;
            this.SelectHighlight4.Content = App.DisplaySettings.HighlightName4;
            this.SelectHighlight5.Content = App.DisplaySettings.HighlightName5;
            this.SelectHighlight6.Content = App.DisplaySettings.HighlightName6;
            this.SelectHighlightNone.Content = Translations.Translate("None");

            this.SelectHighlight1.Background = new SolidColorBrush(App.Themes.ColorHighligt[0]);
            this.SelectHighlight2.Background = new SolidColorBrush(App.Themes.ColorHighligt[1]);
            this.SelectHighlight3.Background = new SolidColorBrush(App.Themes.ColorHighligt[2]);
            this.SelectHighlight4.Background = new SolidColorBrush(App.Themes.ColorHighligt[3]);
            this.SelectHighlight5.Background = new SolidColorBrush(App.Themes.ColorHighligt[4]);
            this.SelectHighlight6.Background = new SolidColorBrush(App.Themes.ColorHighligt[5]);
            if (App.PlaceMarkers.History.Count > 0)
            {
                BiblePlaceMarker place = App.PlaceMarkers.History[App.PlaceMarkers.History.Count - 1];
                var style = App.DisplaySettings.highlighter.GetHighlightForVerse(place.BookShortName, place.ChapterNum, place.VerseNum);
                switch(style)
                {
                    case Highlighter.Highlight.COLOR_1:
                        this.SelectHighlight1.IsChecked = true;
                        break;
                    case Highlighter.Highlight.COLOR_2:
                        this.SelectHighlight2.IsChecked = true;
                        break;
                    case Highlighter.Highlight.COLOR_3:
                        this.SelectHighlight3.IsChecked = true;
                        break;
                    case Highlighter.Highlight.COLOR_4:
                        this.SelectHighlight4.IsChecked = true;
                        break;
                    case Highlighter.Highlight.COLOR_5:
                        this.SelectHighlight5.IsChecked = true;
                        break;
                    case Highlighter.Highlight.COLOR_6:
                        this.SelectHighlight6.IsChecked = true;
                        break;
                    case Highlighter.Highlight.COLOR_NONE:
                        this.SelectHighlightNone.IsChecked = true;
                        break;
                }
            }
        }

        private void HighlightPopup_OnClosed(object sender, object e)
        {
            App.ShowUserInterface(true);
            for (int i = App.OpenWindows.Count - 1; i >= 0; i--)
            {
                App.OpenWindows[i].ForceReload = true;
                App.OpenWindows[i].UpdateBrowser(false);
            }
            App.StartTimerForSavingWindows();
        }

        private void HighlightPopup_OnOpened(object sender, object e)
        {
            App.ShowUserInterface(false);
        }

        private void SelectHighlight_OnClick(object sender, RoutedEventArgs e)
        {

            if (App.PlaceMarkers.History.Count > 0)
            {
                BiblePlaceMarker place = App.PlaceMarkers.History[App.PlaceMarkers.History.Count - 1];
                int chapter = place.ChapterNum;
                int verse = place.VerseNum;
                if (this.SelectHighlight1.IsChecked != null && (bool)this.SelectHighlight1.IsChecked)
                {
                    App.DisplaySettings.highlighter.AddHighlight(place.BookShortName, chapter, verse, Highlighter.Highlight.COLOR_1);
                }
                else if (this.SelectHighlight2.IsChecked != null && (bool)this.SelectHighlight2.IsChecked)
                {
                    App.DisplaySettings.highlighter.AddHighlight(place.BookShortName, chapter, verse, Highlighter.Highlight.COLOR_2);
                }
                else if (this.SelectHighlight3.IsChecked != null && (bool)this.SelectHighlight3.IsChecked)
                {
                    App.DisplaySettings.highlighter.AddHighlight(place.BookShortName, chapter, verse, Highlighter.Highlight.COLOR_3);
                }
                else if (this.SelectHighlight4.IsChecked != null && (bool)this.SelectHighlight4.IsChecked)
                {
                    App.DisplaySettings.highlighter.AddHighlight(place.BookShortName, chapter, verse, Highlighter.Highlight.COLOR_4);
                }
                else if (this.SelectHighlight5.IsChecked != null && (bool)this.SelectHighlight5.IsChecked)
                {
                    App.DisplaySettings.highlighter.AddHighlight(place.BookShortName, chapter, verse, Highlighter.Highlight.COLOR_5);
                }
                else if (this.SelectHighlight6.IsChecked != null && (bool)this.SelectHighlight6.IsChecked)
                {
                    App.DisplaySettings.highlighter.AddHighlight(place.BookShortName, chapter, verse, Highlighter.Highlight.COLOR_6);
                }
                else if (this.SelectHighlightNone.IsChecked != null && (bool)this.SelectHighlightNone.IsChecked)
                {
                    App.DisplaySettings.highlighter.RemoveHighlight(place);
                }
                App.SavePersistantHighlighting();
            }
            this.HighlightPopup.IsOpen = false;
        }

        #endregion
    }
}