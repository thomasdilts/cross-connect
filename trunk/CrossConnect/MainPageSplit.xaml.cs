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
/// <copyright file="MainPageSplit.cs" company="Thomas Dilts">
///     Thomas Dilts. All rights reserved.
/// </copyright>
/// <author>Thomas Dilts</author>
namespace CrossConnect
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;

    using Microsoft.Phone.Controls;
    using Microsoft.Phone.Shell;
    using Microsoft.Phone.Tasks;

    using SwordBackend;

    public partial class MainPageSplit : AutoRotatePage
    {
        #region Fields

        WindowSettings settings = new WindowSettings();

        #endregion Fields

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
            if (App.openWindows.Count() == 0)
            {
                RowDefinition row = new RowDefinition();
                row.Height = GridLength.Auto;
                LayoutRoot.RowDefinitions.Add(row);
                row = new RowDefinition();
                row.Height = GridLength.Auto;
                LayoutRoot.RowDefinitions.Add(row);
                row = new RowDefinition();
                row.Height = new GridLength(0,GridUnitType.Star);
                LayoutRoot.RowDefinitions.Add(row);

                // show just a quick menu to add window or bibles
                TextBlock text = new TextBlock();
                text.Text = "Cross Connect";
                text.FontSize = 40;
                Grid.SetRow(text, 0);
                LayoutRoot.Children.Add(text);
                Button but = new Button();
                Grid.SetRow(but, 1);
                if (App.installedBibles.installedBibles.Count() == 0)
                {
                    but.Content = Translations.translate("Download bibles");
                    but.Click += menuDownloadBible_Click;
                }
                else
                {
                    but.Content = Translations.translate("Add new window");
                    but.Click += butAddWindow_Click;
                }
                LayoutRoot.Children.Add(but);
            }
            else
            {

                int rowCount = 0;
                for (int i = 0; i < App.openWindows.Count(); i++)
                {
                    // make sure we are not doubled up on the events.
                    App.openWindows[i].HitButtonBigger -= HitButtonBigger;
                    App.openWindows[i].HitButtonSmaller -= HitButtonSmaller;
                    App.openWindows[i].HitButtonClose -= HitButtonClose;

                    // then add
                    App.openWindows[i].HitButtonBigger += HitButtonBigger;
                    App.openWindows[i].HitButtonSmaller += HitButtonSmaller;
                    App.openWindows[i].HitButtonClose += HitButtonClose;

                    App.openWindows[i].state.curIndex = i;
                    for (int j = 0; j < App.openWindows[i].state.numRowsIown; j++)
                    {
                        RowDefinition row = new RowDefinition();
                        LayoutRoot.RowDefinitions.Add(row);
                    }
                    Grid.SetRow(App.openWindows[i], rowCount);
                    Grid.SetRowSpan(App.openWindows[i], App.openWindows[i].state.numRowsIown);
                    Grid.SetColumn(App.openWindows[i], 0);
                    LayoutRoot.Children.Add(App.openWindows[i]);
                    rowCount += App.openWindows[i].state.numRowsIown;
                    App.openWindows[i].ShowSizeButtons(true);
                }
                if (App.openWindows.Count() == 1)
                {
                    App.openWindows[0].ShowSizeButtons(false);
                }
            }
        }

        public void ShowEmptyWindow()
        {
            LayoutRoot.Children.Clear();
            LayoutRoot.ColumnDefinitions.Clear();
            LayoutRoot.RowDefinitions.Clear();
        }

        private void AutoRotatePage_BackKeyPress(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Debug.WriteLine("Backed out of the program.");
        }

        private void butAddBookmark_Click(object sender, EventArgs e)
        {
            App.AddBookmark();
        }

        private void butAddWindow_Click(object sender, EventArgs e)
        {
            PhoneApplicationService.Current.State["isAddNewWindowOnly"] = true;
            PhoneApplicationService.Current.State["skipWindowSettings"] = false;
            PhoneApplicationService.Current.State["openWindowIndex"] = 0;
            PhoneApplicationService.Current.State["InitializeWindowSettings"] = true;
            this.NavigationService.Navigate(new Uri("/WindowSettings.xaml", UriKind.Relative));
        }

        private void butDownload_Click(object sender, EventArgs e)
        {
            this.NavigationService.Navigate(new Uri("/DownloadBooks.xaml", UriKind.Relative));
        }

        private void butGoToPlan_Click(object sender, EventArgs e)
        {
            if (App.openWindows.Count() > 0)
            {
                //take the present window and show the plan if it exists.
                var state = App.openWindows[0].state;
                state.windowType = WINDOW_TYPE.WINDOW_DAILY_PLAN;
                App.openWindows[0].Initialize(state.bibleToLoad, state.windowType);
                ReDrawWindows();
            }
        }

        private void butHelp_Click(object sender, EventArgs e)
        {
            WebBrowserTask webBrowserTask = new WebBrowserTask();
            string version = "1.0.0.10";
            webBrowserTask.URL = @"http://www.chaniel.se/crossconnect/help?version=" + version;
            //webBrowserTask.Uri = new Uri(@"http://www.chaniel.se/crossconnect/help?version=" + version);
            webBrowserTask.Show();
        }

        private TextBlock createTextBlock(string text,string tag)
        {
            TextBlock textBlock = new TextBlock();
            textBlock.Text = text;
            textBlock.Tag = tag;
            return textBlock;
        }

        private void GetLast3SecondsChosenVerses(out string textsWithTitles, out string titlesOnly)
        {
            textsWithTitles = "";
            titlesOnly = "";
            DateTime? firstFound = null;
            List<BiblePlaceMarker> foundVerses = new List<BiblePlaceMarker>();
            for (int j = App.placeMarkers.history.Count - 1; j >= 0; j--)
            {
                BiblePlaceMarker place = App.placeMarkers.history[j];
                if (firstFound == null)
                {
                    firstFound = place.when;
                    foundVerses.Add(place);
                }
                else if (firstFound.Value.AddSeconds(-3).CompareTo(place.when) < 0)
                {
                    foundVerses.Add(place);
                }
                else
                {
                    //we found all the verses, get out.
                    break;
                }
            }
            object openWindowIndex = null;
            if (!PhoneApplicationService.Current.State.TryGetValue("openWindowIndex", out openWindowIndex))
            {
                openWindowIndex = 0;
            }

            var state = App.openWindows[(int)openWindowIndex].state;
            //they are in reverse order again,
            for (int j = foundVerses.Count - 1; j >= 0; j--)
            {
                BiblePlaceMarker place = foundVerses[j];
                int bookNum;
                int relChaptNum;
                string fullName;
                string titleText;
                ((BibleZtextReader)state.source).GetInfo(place.chapterNum, place.verseNum, out bookNum, out relChaptNum, out fullName, out titleText);
                string title = fullName + " " + (relChaptNum + 1) + ":" + (place.verseNum + 1) + " - " + state.bibleToLoad;
                string verseText = state.source.GetVerseTextOnly(App.displaySettings, place.chapterNum, place.verseNum);

                if (!string.IsNullOrEmpty(titlesOnly))
                {
                    textsWithTitles += "\n";
                    titlesOnly += ", ";
                }
                titlesOnly += title;
                textsWithTitles += verseText.Replace("<p>", "").Replace("</p>", "").Replace("<br />", "").Replace("\n", " ") + "\n-" + title;
                if (App.dailyPlan.personalNotes.ContainsKey(place.chapterNum) && App.dailyPlan.personalNotes[place.chapterNum].ContainsKey(place.verseNum))
                {
                    textsWithTitles += "\n" + Translations.translate("Added notes") + "\n" + App.dailyPlan.personalNotes[place.chapterNum][place.verseNum].note;
                }
            }
        }

        private void HitButtonBigger(object sender, EventArgs e)
        {
            ReDrawWindows();
        }

        private void HitButtonClose(object sender, EventArgs e)
        {
            App.openWindows.RemoveAt(((BrowserTitledWindow)sender).state.curIndex);
            ReDrawWindows();
        }

        private void HitButtonSmaller(object sender, EventArgs e)
        {
            ReDrawWindows();
        }

        // Load data for the ViewModel Items
        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            App.mainWindow = this;

            ((ApplicationBarIconButton)this.ApplicationBar.Buttons[0]).Text = Translations.translate("Add new window");
            ((ApplicationBarIconButton)this.ApplicationBar.Buttons[1]).Text = Translations.translate("Add to bookmarks");
            ((ApplicationBarIconButton)this.ApplicationBar.Buttons[2]).Text = Translations.translate("Daily plan");
            ((ApplicationBarIconButton)this.ApplicationBar.Buttons[3]).Text = Translations.translate("Help");

            ((ApplicationBarMenuItem)this.ApplicationBar.MenuItems[0]).Text = Translations.translate("Rate this program");
            ((ApplicationBarMenuItem)this.ApplicationBar.MenuItems[1]).Text = Translations.translate("Download bibles");
            ((ApplicationBarMenuItem)this.ApplicationBar.MenuItems[2]).Text = Translations.translate("Add a note");
            ((ApplicationBarMenuItem)this.ApplicationBar.MenuItems[3]).Text = Translations.translate("Select bible to delete");
            ((ApplicationBarMenuItem)this.ApplicationBar.MenuItems[4]).Text = Translations.translate("Select bookmark to delete");
            ((ApplicationBarMenuItem)this.ApplicationBar.MenuItems[5]).Text = Translations.translate("Clear history");
            ((ApplicationBarMenuItem)this.ApplicationBar.MenuItems[6]).Text = Translations.translate("Send message");
            ((ApplicationBarMenuItem)this.ApplicationBar.MenuItems[7]).Text = Translations.translate("Send mail");
            ((ApplicationBarMenuItem)this.ApplicationBar.MenuItems[8]).Text = Translations.translate("Add new window");
            ((ApplicationBarMenuItem)this.ApplicationBar.MenuItems[9]).Text = Translations.translate("Add to bookmarks");
            ((ApplicationBarMenuItem)this.ApplicationBar.MenuItems[10]).Text = Translations.translate("Daily plan");
            ((ApplicationBarMenuItem)this.ApplicationBar.MenuItems[11]).Text = Translations.translate("Settings");
            ((ApplicationBarMenuItem)this.ApplicationBar.MenuItems[12]).Text = Translations.translate("Select the language") + " (language)";
            ((ApplicationBarMenuItem)this.ApplicationBar.MenuItems[13]).Text = Translations.translate("Help");

            if (App.openWindows.Count() == 0 || App.installedBibles.installedBibles.Count() == 0)
            {
                if (App.installedBibles.installedBibles.Count() == 0)
                {
                    if (App.isFirstTimeInMainPageSplit==0)
                    {
                        // cant have any open windows if there are no books!
                        App.openWindows.Clear();
                        // get some books.
                        this.NavigationService.Navigate(new Uri("/DownloadBooks.xaml", UriKind.Relative));
                        App.isFirstTimeInMainPageSplit = 1;
                    }
                }
                else
                {
                    if (App.isFirstTimeInMainPageSplit<=1)
                    {
                        PhoneApplicationService.Current.State["skipWindowSettings"] = false;
                        PhoneApplicationService.Current.State["isAddNewWindowOnly"] = true;
                        PhoneApplicationService.Current.State["InitializeWindowSettings"] = true;
                        this.NavigationService.Navigate(new Uri("/WindowSettings.xaml", UriKind.Relative));
                        App.isFirstTimeInMainPageSplit=2;
                    }
                }
            }
            ReDrawWindows();
            // figure out if this is a light color
            var color = (Color)Application.Current.Resources["PhoneBackgroundColor"];
            int lightColorCount = (color.R > 0x80 ? 1 : 0) + (color.G > 0x80 ? 1 : 0) + (color.B > 0x80 ? 1 : 0);
            string colorDir = lightColorCount >= 2 ? "light" : "dark";
        }

        private void menuAddNote_Click(object sender, EventArgs e)
        {
            if (App.placeMarkers.history.Count > 0)
            {
                PhoneApplicationService.Current.State.Remove("NoteToAddSaved");
                this.NavigationService.Navigate(new Uri("/AddNote.xaml", UriKind.Relative));
            }
            else
            {
                MessageBox.Show(Translations.translate("You must first select a verse"));
            }
        }

        private void menuClearHistory_Click(object sender, EventArgs e)
        {
            App.placeMarkers.history = new List<SwordBackend.BiblePlaceMarker>();
            App.RaiseHistoryChangeEvent();
        }

        private void menuDeleteBible_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/RemoveBibles.xaml", UriKind.Relative));
        }

        private void menuDeleteBookmark_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/EditBookmarks.xaml", UriKind.Relative));
        }

        private void menuDownloadBible_Click(object sender, EventArgs e)
        {
            this.NavigationService.Navigate(new Uri("/DownloadBooks.xaml", UriKind.Relative));
        }

        private void menuLanguage_Click(object sender, EventArgs e)
        {
            this.NavigationService.Navigate(new Uri("/SelectLanguage.xaml", UriKind.Relative));
        }

        private void menuMail_Click(object sender, EventArgs e)
        {
            string textsWithTitles;
            string titlesOnly;
            GetLast3SecondsChosenVerses(out textsWithTitles, out titlesOnly);
            EmailComposeTask emailComposeTask = new EmailComposeTask();
            //emailComposeTask.To = "user@example.com";
            emailComposeTask.Body = textsWithTitles;
            //emailComposeTask.Cc = "user2@example.com";
            emailComposeTask.Subject = titlesOnly;
            emailComposeTask.Show();
        }

        private void menuMessage_Click(object sender, EventArgs e)
        {
            string textsWithTitles;
            string titlesOnly;
            GetLast3SecondsChosenVerses(out textsWithTitles, out titlesOnly);
            SmsComposeTask smsComposeTask = new SmsComposeTask();
            //smsComposeTask.To = "5555555555";
            smsComposeTask.Body = textsWithTitles;
            smsComposeTask.Show();
        }

        private void menuRateThisApp_Click(object sender, EventArgs e)
        {
            MarketplaceReviewTask task = new MarketplaceReviewTask();
            task.Show();
        }

        private void menuSettings_Click(object sender, EventArgs e)
        {
            this.NavigationService.Navigate(new Uri("/Settings.xaml", UriKind.Relative));
        }

        private void PhoneApplicationPage_OrientationChanged(object sender, OrientationChangedEventArgs e)
        {
            //redraw the browsers
            for (int i = 0; i < App.openWindows.Count(); i++)
            {
                App.openWindows[i].UpdateBrowser();
            }
        }

        private void PhoneApplicationPage_Unloaded(object sender, RoutedEventArgs e)
        {
            foreach (var nextWindow in App.openWindows)
            {
                nextWindow.HitButtonBigger -= HitButtonBigger;
                nextWindow.HitButtonSmaller -= HitButtonSmaller;
                nextWindow.HitButtonClose -= HitButtonClose;
            }
        }

        private TurnstileTransition TurnstileTransitionElement(string mode)
        {
            TurnstileTransitionMode slideTransitionMode = (TurnstileTransitionMode)Enum.Parse(typeof(TurnstileTransitionMode), mode, false);
            return new TurnstileTransition { Mode = slideTransitionMode };
        }

        #endregion Methods
    }
}