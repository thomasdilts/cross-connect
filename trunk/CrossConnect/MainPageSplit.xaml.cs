#region Header

// <copyright file="MainPageSplit.xaml.cs" company="Thomas Dilts">
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
// Email: thomas@chaniel.se
// </summary>
// <author>Thomas Dilts</author>

#endregion Header

namespace CrossConnect
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;

    using Microsoft.Phone.Controls;
    using Microsoft.Phone.Shell;
    using Microsoft.Phone.Tasks;

    using Sword.reader;

    public partial class MainPageSplit
    {
        #region Constructors

        // Constructor
        public MainPageSplit()
        {
            InitializeComponent();
        }

        #endregion Constructors

        #region Methods

        public void ReDrawWindows()
        {
            LayoutRoot.Children.Clear();
            LayoutRoot.ColumnDefinitions.Clear();
            LayoutRoot.RowDefinitions.Clear();
            if (App.OpenWindows.Count() == 0)
            {
                var row = new RowDefinition {Height = GridLength.Auto};
                LayoutRoot.RowDefinitions.Add(row);
                row = new RowDefinition {Height = GridLength.Auto};
                LayoutRoot.RowDefinitions.Add(row);
                row = new RowDefinition {Height = new GridLength(0, GridUnitType.Star)};
                LayoutRoot.RowDefinitions.Add(row);

                // show just a quick menu to add window or bibles
                var text = new TextBlock {Text = "Cross Connect", FontSize = 40};
                Grid.SetRow(text, 0);
                LayoutRoot.Children.Add(text);
                var but = new Button();
                Grid.SetRow(but, 1);
                if (App.InstalledBibles.InstalledBibles.Count() == 0)
                {
                    but.Content = Translations.Translate("Download bibles");
                    but.Click += MenuDownloadBibleClick;
                }
                else
                {
                    but.Content = Translations.Translate("Add new window");
                    but.Click += ButAddWindowClick;
                }
                LayoutRoot.Children.Add(but);
            }
            else
            {
                int rowCount = 0;
                for (int i = 0; i < App.OpenWindows.Count(); i++)
                {
                    // make sure we are not doubled up on the events.
                    App.OpenWindows[i].HitButtonBigger -= HitButtonBigger;
                    App.OpenWindows[i].HitButtonSmaller -= HitButtonSmaller;
                    App.OpenWindows[i].HitButtonClose -= HitButtonClose;

                    // then add
                    App.OpenWindows[i].HitButtonBigger += HitButtonBigger;
                    App.OpenWindows[i].HitButtonSmaller += HitButtonSmaller;
                    App.OpenWindows[i].HitButtonClose += HitButtonClose;

                    App.OpenWindows[i].State.CurIndex = i;
                    for (int j = 0; j < App.OpenWindows[i].State.NumRowsIown; j++)
                    {
                        var row = new RowDefinition();
                        LayoutRoot.RowDefinitions.Add(row);
                    }
                    Grid.SetRow(App.OpenWindows[i], rowCount);
                    Grid.SetRowSpan(App.OpenWindows[i], App.OpenWindows[i].State.NumRowsIown);
                    Grid.SetColumn(App.OpenWindows[i], 0);
                    LayoutRoot.Children.Add(App.OpenWindows[i]);
                    rowCount += App.OpenWindows[i].State.NumRowsIown;
                    App.OpenWindows[i].ShowSizeButtons();
                }
                if (App.OpenWindows.Count() == 1)
                {
                    App.OpenWindows[0].ShowSizeButtons(false);
                }
            }
        }

        public void ShowEmptyWindow()
        {
            LayoutRoot.Children.Clear();
            LayoutRoot.ColumnDefinitions.Clear();
            LayoutRoot.RowDefinitions.Clear();
        }

        private void AutoRotatePageBackKeyPress(object sender, CancelEventArgs e)
        {
            Debug.WriteLine("Backed out of the program.");
        }

        private void ButAddBookmarkClick(object sender, EventArgs e)
        {
            App.AddBookmark();
        }

        private void ButAddWindowClick(object sender, EventArgs e)
        {
            PhoneApplicationService.Current.State["isAddNewWindowOnly"] = true;
            PhoneApplicationService.Current.State["skipWindowSettings"] = false;
            PhoneApplicationService.Current.State["openWindowIndex"] = 0;
            PhoneApplicationService.Current.State["InitializeWindowSettings"] = true;
            NavigationService.Navigate(new Uri("/WindowSettings.xaml", UriKind.Relative));
        }

        private void ButGoToPlanClick(object sender, EventArgs e)
        {
            if (App.OpenWindows.Count() > 0)
            {
                //take the present window and show the plan if it exists.
                var state = App.OpenWindows[0].State;
                state.WindowType = WindowType.WindowDailyPlan;
                App.OpenWindows[0].Initialize(state.BibleToLoad, state.BibleDescription, state.WindowType);
                ReDrawWindows();
            }
        }

        private void ButHelpClick(object sender, EventArgs e)
        {
            var webBrowserTask = new WebBrowserTask();
            const string version = "1.0.0.20";
            webBrowserTask.Uri = new Uri(@"http://www.chaniel.se/crossconnect/help?version=" + version);
            webBrowserTask.Show();
        }

        private void GetLast3SecondsChosenVerses(out string textsWithTitles, out string titlesOnly)
        {
            textsWithTitles = "";
            titlesOnly = "";
            DateTime? firstFound = null;
            var foundVerses = new List<BiblePlaceMarker>();
            for (int j = App.PlaceMarkers.History.Count - 1; j >= 0; j--)
            {
                BiblePlaceMarker place = App.PlaceMarkers.History[j];
                if (firstFound == null)
                {
                    firstFound = place.When;
                    foundVerses.Add(place);
                }
                else if (firstFound.Value.AddSeconds(-3).CompareTo(place.When) < 0)
                {
                    foundVerses.Add(place);
                }
                else
                {
                    //we found all the verses, get out.
                    break;
                }
            }
            object openWindowIndex;
            if (!PhoneApplicationService.Current.State.TryGetValue("openWindowIndex", out openWindowIndex))
            {
                openWindowIndex = 0;
            }

            var state = App.OpenWindows[(int) openWindowIndex].State;
            //they are in reverse order again,
            for (int j = foundVerses.Count - 1; j >= 0; j--)
            {
                BiblePlaceMarker place = foundVerses[j];
                int bookNum;
                int relChaptNum;
                string fullName;
                string titleText;
                ((BibleZtextReader) state.Source).GetInfo(place.ChapterNum, place.VerseNum, out bookNum, out relChaptNum,
                                                          out fullName, out titleText);
                string title = fullName + " " + (relChaptNum + 1) + ":" + (place.VerseNum + 1) + " - " +
                               state.BibleToLoad;
                string verseText = state.Source.GetVerseTextOnly(App.DisplaySettings, place.ChapterNum, place.VerseNum);

                if (!string.IsNullOrEmpty(titlesOnly))
                {
                    textsWithTitles += "\n";
                    titlesOnly += ", ";
                }
                titlesOnly += title;
                textsWithTitles +=
                    verseText.Replace("<p>", "").Replace("</p>", "").Replace("<br />", "").Replace("\n", " ") + "\n-" +
                    title;
                if (App.DailyPlan.PersonalNotes.ContainsKey(place.ChapterNum) &&
                    App.DailyPlan.PersonalNotes[place.ChapterNum].ContainsKey(place.VerseNum))
                {
                    textsWithTitles += "\n" + Translations.Translate("Added notes") + "\n" +
                                       App.DailyPlan.PersonalNotes[place.ChapterNum][place.VerseNum].Note;
                }
            }
        }

        private void HitButtonBigger(object sender, EventArgs e)
        {
            ReDrawWindows();
        }

        private void HitButtonClose(object sender, EventArgs e)
        {
            App.OpenWindows.RemoveAt(((BrowserTitledWindow) sender).State.CurIndex);
            ReDrawWindows();
        }

        private void HitButtonSmaller(object sender, EventArgs e)
        {
            ReDrawWindows();
        }

        // Load data for the ViewModel Items
        private void MainPageLoaded(object sender, RoutedEventArgs e)
        {
            App.MainWindow = this;

            ((ApplicationBarIconButton) ApplicationBar.Buttons[0]).Text = Translations.Translate("Add new window");
            ((ApplicationBarIconButton) ApplicationBar.Buttons[1]).Text = Translations.Translate("Add to bookmarks");
            ((ApplicationBarIconButton) ApplicationBar.Buttons[2]).Text = Translations.Translate("Daily plan");
            ((ApplicationBarIconButton) ApplicationBar.Buttons[3]).Text = Translations.Translate("Help");

            ((ApplicationBarMenuItem) ApplicationBar.MenuItems[0]).Text = Translations.Translate("Rate this program");
            ((ApplicationBarMenuItem)ApplicationBar.MenuItems[1]).Text = Translations.Translate("Themes");
            ((ApplicationBarMenuItem)ApplicationBar.MenuItems[2]).Text = Translations.Translate("Download bibles");
            ((ApplicationBarMenuItem)ApplicationBar.MenuItems[3]).Text = Translations.Translate("Add a note");
            ((ApplicationBarMenuItem) ApplicationBar.MenuItems[4]).Text =
                Translations.Translate("Select bible to delete");
            ((ApplicationBarMenuItem) ApplicationBar.MenuItems[5]).Text =
                Translations.Translate("Select bookmark to delete");
            ((ApplicationBarMenuItem) ApplicationBar.MenuItems[6]).Text = Translations.Translate("Clear history");
            ((ApplicationBarMenuItem) ApplicationBar.MenuItems[7]).Text = Translations.Translate("Send message");
            ((ApplicationBarMenuItem) ApplicationBar.MenuItems[8]).Text = Translations.Translate("Send mail");
            ((ApplicationBarMenuItem) ApplicationBar.MenuItems[9]).Text = Translations.Translate("Add new window");
            ((ApplicationBarMenuItem) ApplicationBar.MenuItems[10]).Text = Translations.Translate("Add to bookmarks");
            ((ApplicationBarMenuItem) ApplicationBar.MenuItems[11]).Text = Translations.Translate("Daily plan");
            ((ApplicationBarMenuItem) ApplicationBar.MenuItems[12]).Text = Translations.Translate("Settings");
            ((ApplicationBarMenuItem) ApplicationBar.MenuItems[13]).Text =
                Translations.Translate("Select the language") + " (language)";
            ((ApplicationBarMenuItem) ApplicationBar.MenuItems[14]).Text = Translations.Translate("Help");

            if (App.OpenWindows.Count() == 0 || App.InstalledBibles.InstalledBibles.Count() == 0)
            {
                if (App.InstalledBibles.InstalledBibles.Count() == 0)
                {
                    if (App.IsFirstTimeInMainPageSplit == 0)
                    {
                        // cant have any open windows if there are no books!
                        App.OpenWindows.Clear();
                        // get some books.
                        NavigationService.Navigate(new Uri("/DownloadBooks.xaml", UriKind.Relative));
                        App.IsFirstTimeInMainPageSplit = 1;
                    }
                }
                else
                {
                    if (App.IsFirstTimeInMainPageSplit <= 1)
                    {
                        PhoneApplicationService.Current.State["skipWindowSettings"] = false;
                        PhoneApplicationService.Current.State["isAddNewWindowOnly"] = true;
                        PhoneApplicationService.Current.State["InitializeWindowSettings"] = true;
                        NavigationService.Navigate(new Uri("/WindowSettings.xaml", UriKind.Relative));
                        App.IsFirstTimeInMainPageSplit = 2;
                    }
                }
            }
            ReDrawWindows();
            // figure out if this is a light color
            //var color = (Color) Application.Current.Resources["PhoneBackgroundColor"];
            //int lightColorCount = (color.R > 0x80 ? 1 : 0) + (color.G > 0x80 ? 1 : 0) + (color.B > 0x80 ? 1 : 0);
            //string colorDir = lightColorCount >= 2 ? "light" : "dark";
        }

        private void MenuAddNoteClick(object sender, EventArgs e)
        {
            if (App.PlaceMarkers.History.Count > 0)
            {
                PhoneApplicationService.Current.State.Remove("NoteToAddSaved");
                NavigationService.Navigate(new Uri("/AddNote.xaml", UriKind.Relative));
            }
            else
            {
                MessageBox.Show(Translations.Translate("You must first select a verse"));
            }
        }

        private void MenuClearHistoryClick(object sender, EventArgs e)
        {
            App.PlaceMarkers.History = new List<BiblePlaceMarker>();
            App.RaiseHistoryChangeEvent();
        }

        private void MenuDeleteBibleClick(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/RemoveBibles.xaml", UriKind.Relative));
        }

        private void MenuDeleteBookmarkClick(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/EditBookmarks.xaml", UriKind.Relative));
        }

        private void MenuDownloadBibleClick(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/DownloadBooks.xaml", UriKind.Relative));
        }

        private void MenuLanguageClick(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/SelectLanguage.xaml", UriKind.Relative));
        }

        private void MenuMailClick(object sender, EventArgs e)
        {
            string textsWithTitles;
            string titlesOnly;
            GetLast3SecondsChosenVerses(out textsWithTitles, out titlesOnly);
            var emailComposeTask = new EmailComposeTask {Body = textsWithTitles, Subject = titlesOnly};

            //emailComposeTask.To = "user@example.com";
            //emailComposeTask.Cc = "user2@example.com";
            emailComposeTask.Show();
        }

        private void MenuMessageClick(object sender, EventArgs e)
        {
            string textsWithTitles;
            string titlesOnly;
            GetLast3SecondsChosenVerses(out textsWithTitles, out titlesOnly);
            var smsComposeTask = new SmsComposeTask {Body = textsWithTitles};
            //smsComposeTask.To = "5555555555";
            smsComposeTask.Show();
        }

        private void MenuRateThisAppClick(object sender, EventArgs e)
        {
            var task = new MarketplaceReviewTask();
            task.Show();
        }

        private void MenuSettingsClick(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/Settings.xaml", UriKind.Relative));
        }

        private void PhoneApplicationPageOrientationChanged(object sender, OrientationChangedEventArgs e)
        {
            //redraw the browsers
            for (int i = 0; i < App.OpenWindows.Count(); i++)
            {
                App.OpenWindows[i].CalculateTitleTextWidth();
                App.OpenWindows[i].ForceReload = true;
                App.OpenWindows[i].UpdateBrowser();
            }
        }

        private void PhoneApplicationPageUnloaded(object sender, RoutedEventArgs e)
        {
            foreach (var nextWindow in App.OpenWindows)
            {
                nextWindow.HitButtonBigger -= HitButtonBigger;
                nextWindow.HitButtonSmaller -= HitButtonSmaller;
                nextWindow.HitButtonClose -= HitButtonClose;
            }
        }

        #endregion Methods

        private void MenuThemesClick(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/Themes.xaml", UriKind.Relative));
        }
    }
}