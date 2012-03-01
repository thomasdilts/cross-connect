// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainPageSplit.xaml.cs" company="">
//   
// </copyright>
// <summary>
//   The main page split.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#region Header

// <copyright file="MainPageSplit.xaml.cs" company="Thomas Dilts">
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
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.IO.IsolatedStorage;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using System.Windows.Threading;

    using Microsoft.Phone.Controls;
    using Microsoft.Phone.Shell;
    using Microsoft.Phone.Tasks;

    using Sword.reader;

    /// <summary>
    /// The main page split.
    /// </summary>
    public partial class MainPageSplit
    {
        #region Constants and Fields

        /// <summary>
        /// The _buttons.
        /// </summary>
        private readonly List<Button> _buttons = new List<Button>();

        /// <summary>
        /// The _windows.
        /// </summary>
        private readonly List<Grid> _windows = new List<Grid>();

        /// <summary>
        /// The _current screen.
        /// </summary>
        private int _currentScreen;

        /// <summary>
        /// The _is in screen moving.
        /// </summary>
        private bool _isInScreenMoving;

        /// <summary>
        /// The _move multi screen timer.
        /// </summary>
        private DispatcherTimer _moveMultiScreenTimer;

        /// <summary>
        /// The _screen pos increment.
        /// </summary>
        private int _screenPosIncrement;

        /// <summary>
        /// The _screen width.
        /// </summary>
        private double _screenWidth;

        #endregion

        // Constructor
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MainPageSplit"/> class.
        /// </summary>
        public MainPageSplit()
        {
            this.InitializeComponent();
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The get last 3 seconds chosen verses.
        /// </summary>
        /// <param name="textsWithTitles">
        /// The texts with titles.
        /// </param>
        /// <param name="titlesOnly">
        /// The titles only.
        /// </param>
        public static void GetLast3SecondsChosenVerses(out string textsWithTitles, out string titlesOnly)
        {
            textsWithTitles = string.Empty;
            titlesOnly = string.Empty;
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
                    // we found all the verses, get out.
                    break;
                }
            }

            object openWindowIndex;
            if (!PhoneApplicationService.Current.State.TryGetValue("openWindowIndex", out openWindowIndex))
            {
                openWindowIndex = 0;
            }

            SerializableWindowState state = App.OpenWindows[(int)openWindowIndex].State;

            // they are in reverse order again,
            for (int j = foundVerses.Count - 1; j >= 0; j--)
            {
                BiblePlaceMarker place = foundVerses[j];
                int bookNum;
                int relChaptNum;
                string fullName;
                string titleText;
                ((BibleZtextReader)state.Source).GetInfo(
                    place.ChapterNum, place.VerseNum, out bookNum, out relChaptNum, out fullName, out titleText);
                string title = fullName + " " + (relChaptNum + 1) + ":" + (place.VerseNum + 1) + " - "
                               + state.BibleToLoad;
                string verseText = state.Source.GetVerseTextOnly(App.DisplaySettings, place.ChapterNum, place.VerseNum);

                if (!string.IsNullOrEmpty(titlesOnly))
                {
                    textsWithTitles += "\n";
                    titlesOnly += ", ";
                }

                titlesOnly += title;
                textsWithTitles +=
                    verseText.Replace("<p>", string.Empty).Replace("</p>", string.Empty).Replace("<br />", string.Empty)
                        .Replace("\n", " ") + "\n-" + title;
                if (App.DailyPlan.PersonalNotes.ContainsKey(place.ChapterNum)
                    && App.DailyPlan.PersonalNotes[place.ChapterNum].ContainsKey(place.VerseNum))
                {
                    textsWithTitles += "\n" + Translations.Translate("Added notes") + "\n"
                                       + App.DailyPlan.PersonalNotes[place.ChapterNum][place.VerseNum].Note;
                }
            }
        }

        /// <summary>
        /// The re draw windows.
        /// </summary>
        public void ReDrawWindows()
        {
            if (this._windows.Count() != App.DisplaySettings.NumberOfScreens)
            {
                if (this._currentScreen >= App.DisplaySettings.NumberOfScreens)
                {
                    this._currentScreen = 0;
                    PhoneApplicationService.Current.State["CurrentScreen"] = this._currentScreen;
                    this.WindowGrid.Margin = new Thickness(-this._currentScreen * this._screenWidth, 0, 0, 0);
                }

                this.LayoutMainRoot.Width = this._screenWidth * App.DisplaySettings.NumberOfScreens;
                foreach (Grid window in this._windows)
                {
                    window.Children.Clear();
                }

                this._windows.Clear();
                this._buttons.Clear();
                this.WindowSelectGrid.Children.Clear();
                this.WindowSelectGrid.RowDefinitions.Clear();
                this.WindowSelectGrid.ColumnDefinitions.Clear();
                this.WindowSelectGrid.RowDefinitions.Add(
                    new RowDefinition { Height = new GridLength(App.DisplaySettings.NumberOfScreens == 1 ? 0 : 45) });
                this.WindowGrid.Children.Clear();
                this.WindowGrid.RowDefinitions.Clear();
                this.WindowGrid.ColumnDefinitions.Clear();

                // WindowGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(_screenHeight-45) });
                this.SetScreenWidthVariable();
                for (int i = 0; i < App.DisplaySettings.NumberOfScreens; i++)
                {
                    if (App.DisplaySettings.NumberOfScreens > 1)
                    {
                        var but = new Button { Content = (i + 1).ToString(CultureInfo.InvariantCulture), Margin = new Thickness(-5, -10, -5, -10) };
                        but.Click += this.ScreenSelectionClick;
                        Grid.SetColumn(but, i);
                        Grid.SetRow(but, 0);
                        this.WindowSelectGrid.Children.Add(but);
                        this.WindowSelectGrid.ColumnDefinitions.Add(
                            new ColumnDefinition
                                {
                                   Width = new GridLength(this._screenWidth / App.DisplaySettings.NumberOfScreens) 
                                });
                        this._buttons.Add(but);
                    }

                    var gd = new Grid();
                    Grid.SetColumn(gd, i);
                    Grid.SetRow(gd, 0);
                    this.WindowGrid.Children.Add(gd);
                    this.WindowGrid.ColumnDefinitions.Add(
                        new ColumnDefinition { Width = new GridLength(this._screenWidth) });
                    this._windows.Add(gd);
                }

                this.DrawWindowSelectionButtons();
            }

            foreach (Grid grid in this._windows)
            {
                grid.Children.Clear();
                grid.ColumnDefinitions.Clear();
                grid.RowDefinitions.Clear();
            }

            if (!App.OpenWindows.Any())
            {
                var row = new RowDefinition { Height = GridLength.Auto };
                this._windows[0].RowDefinitions.Add(row);
                row = new RowDefinition { Height = GridLength.Auto };
                this._windows[0].RowDefinitions.Add(row);
                row = new RowDefinition { Height = new GridLength(0, GridUnitType.Star) };
                this._windows[0].RowDefinitions.Add(row);
                var border = new SolidColorBrush(App.Themes.BorderColor);
                var fore = new SolidColorBrush(App.Themes.MainFontColor);
                var back = new SolidColorBrush(App.Themes.MainBackColor);

                // show just a quick menu to add window or bibles
                var text = new TextBlock { Text = "Cross Connect", FontSize = 40, Foreground = fore };

                Grid.SetRow(text, 0);
                this._windows[0].Children.Add(text);
                var but = new Button { Background = back, Foreground = fore, BorderBrush = border };
                Grid.SetRow(but, 1);
                if (!App.InstalledBibles.InstalledBibles.Any())
                {
                    but.Content = Translations.Translate("Download bibles");
                    but.Click += this.MenuDownloadBibleClick;
                }
                else
                {
                    but.Content = Translations.Translate("Add new window");
                    but.Click += this.ButAddWindowClick;
                }

                this._windows[0].Children.Add(but);
            }
            else
            {
                // var removeWindows = new List<int>();
                var rowCount = new int[this._windows.Count];
                for (int i = 0; i < App.OpenWindows.Count(); i++)
                {
                    if (App.OpenWindows[i].State.Window >= App.DisplaySettings.NumberOfScreens)
                    {
                        App.OpenWindows.RemoveAt(i);
                        i--;
                        continue;
                    }

                    // make sure we are not doubled up on the events.
                    App.OpenWindows[i].HitButtonBigger -= this.HitButtonBigger;
                    App.OpenWindows[i].HitButtonSmaller -= this.HitButtonSmaller;
                    App.OpenWindows[i].HitButtonClose -= this.HitButtonClose;

                    // then add
                    App.OpenWindows[i].HitButtonBigger += this.HitButtonBigger;
                    App.OpenWindows[i].HitButtonSmaller += this.HitButtonSmaller;
                    App.OpenWindows[i].HitButtonClose += this.HitButtonClose;

                    App.OpenWindows[i].State.CurIndex = i;
                    for (int j = 0; j < App.OpenWindows[i].State.NumRowsIown; j++)
                    {
                        var row = new RowDefinition();
                        this._windows[App.OpenWindows[i].State.Window].RowDefinitions.Add(row);
                    }

                    Grid.SetRow((FrameworkElement)App.OpenWindows[i], rowCount[App.OpenWindows[i].State.Window]);
                    Grid.SetRowSpan((FrameworkElement)App.OpenWindows[i], App.OpenWindows[i].State.NumRowsIown);
                    Grid.SetColumn((FrameworkElement)App.OpenWindows[i], 0);
                    this._windows[App.OpenWindows[i].State.Window].Children.Add((UIElement)App.OpenWindows[i]);
                    rowCount[App.OpenWindows[i].State.Window] += App.OpenWindows[i].State.NumRowsIown;
                    App.OpenWindows[i].ShowSizeButtons();
                }

                // foreach (var i in removeWindows)
                // {
                // App.OpenWindows.RemoveAt(i);
                // }
                if (App.OpenWindows.Count() == 1)
                {
                    App.OpenWindows[0].ShowSizeButtons(false);
                }
            }

            if (App.Themes.IsMainBackImage && !string.IsNullOrEmpty(App.Themes.MainBackImage))
            {
                // read from isolated storage.
                using (IsolatedStorageFile isolatedStorageRoot = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    if (isolatedStorageRoot.FileExists(App.WebDirIsolated + "/images/" + App.Themes.MainBackImage))
                    {
                        try
                        {
                            using (
                                var fStream =
                                    isolatedStorageRoot.OpenFile(
                                        App.WebDirIsolated + "/images/" + App.Themes.MainBackImage, FileMode.Open))
                            {
                                var buffer = new byte[10000];
                                int len;
                                var ms = new MemoryStream();
                                while ((len = fStream.Read(buffer, 0, buffer.GetUpperBound(0))) > 0)
                                {
                                    ms.Write(buffer, 0, len);
                                }

                                fStream.Close();
                                ms.Position = 0;
                                var bitImage = new BitmapImage();
                                bitImage.SetSource(ms);
                                var imageBrush = new ImageBrush { ImageSource = bitImage, };
                                this.LayoutMainRoot.Background = imageBrush;
                            }
                        }
                        catch (Exception ee)
                        {
                            Debug.WriteLine(ee);
                            this.LayoutMainRoot.Background = new SolidColorBrush(App.Themes.MainBackColor);
                        }
                    }
                }
            }
            else
            {
                this.LayoutMainRoot.Background = new SolidColorBrush(App.Themes.MainBackColor);
            }
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
            Debug.WriteLine("Backed out of the program.");
        }

        /// <summary>
        /// The but add bookmark click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void ButAddBookmarkClick(object sender, EventArgs e)
        {
            App.AddBookmark();
        }

        /// <summary>
        /// The but add window click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void ButAddWindowClick(object sender, EventArgs e)
        {
            PhoneApplicationService.Current.State["isAddNewWindowOnly"] = true;
            PhoneApplicationService.Current.State["skipWindowSettings"] = false;
            PhoneApplicationService.Current.State["openWindowIndex"] = 0;
            PhoneApplicationService.Current.State["InitializeWindowSettings"] = true;
            this.NavigationService.Navigate(new Uri("/WindowSettings.xaml", UriKind.Relative));
        }

        /// <summary>
        /// The but go to plan click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void ButGoToPlanClick(object sender, EventArgs e)
        {
            App.AddWindow(string.Empty, string.Empty, WindowType.WindowDailyPlan, 10);
        }

        /// <summary>
        /// The but help click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void ButHelpClick(object sender, EventArgs e)
        {
            var webBrowserTask = new WebBrowserTask
                {
                   Uri = new Uri(@"http://www.cross-connect.se/help?version=" + App.Version) 
                };
            webBrowserTask.Show();
        }

        /// <summary>
        /// The do move multi screen timer tick.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void DoMoveMultiScreenTimerTick(object sender, EventArgs e)
        {
            var leftMargin = (int)this.WindowGrid.Margin.Left;
            leftMargin += this._screenPosIncrement;
            this.WindowGrid.Margin = new Thickness(leftMargin, 0, 0, 0);
            if (Math.Abs(leftMargin + (this._currentScreen * this._screenWidth)) < 71)
            {
                // make sure the position is correct
                this.WindowGrid.Margin = new Thickness(-this._currentScreen * this._screenWidth, 0, 0, 0);
                this._moveMultiScreenTimer.Stop();
                this._isInScreenMoving = false;
            }
        }

        /// <summary>
        /// The draw window selection buttons.
        /// </summary>
        private void DrawWindowSelectionButtons()
        {
            var border = new SolidColorBrush(App.Themes.BorderColor);
            var fore = new SolidColorBrush(App.Themes.MainFontColor);
            var back = new SolidColorBrush(App.Themes.MainBackColor);
            this.WindowSelectGrid.Background = back;
            for (int i = 0; i < this._buttons.Count(); i++)
            {
                this._buttons[i].Background = this._currentScreen == i ? fore : back;
                this._buttons[i].Foreground = this._currentScreen == i ? back : fore;
                this._buttons[i].BorderBrush = border;
            }
        }

        /// <summary>
        /// The hit button bigger.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void HitButtonBigger(object sender, EventArgs e)
        {
            this.ReDrawWindows();
        }

        /// <summary>
        /// The hit button close.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void HitButtonClose(object sender, EventArgs e)
        {
            App.OpenWindows.RemoveAt(((ITiledWindow)sender).State.CurIndex);
            this.ReDrawWindows();
        }

        /// <summary>
        /// The hit button smaller.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void HitButtonSmaller(object sender, EventArgs e)
        {
            this.ReDrawWindows();
        }

        // Load data for the ViewModel Items
        private void MainPageLoaded(object sender, RoutedEventArgs e)
        {
            App.MainWindow = this;

            ((ApplicationBarIconButton)this.ApplicationBar.Buttons[0]).Text = Translations.Translate("Add new window");
            ((ApplicationBarIconButton)this.ApplicationBar.Buttons[1]).Text = Translations.Translate("Add to bookmarks");
            ((ApplicationBarIconButton)this.ApplicationBar.Buttons[2]).Text = Translations.Translate("Daily plan");
            ((ApplicationBarIconButton)this.ApplicationBar.Buttons[3]).Text = Translations.Translate("Help");

            ((ApplicationBarMenuItem)this.ApplicationBar.MenuItems[0]).Text = Translations.Translate(
                "Rate this program");
            ((ApplicationBarMenuItem)this.ApplicationBar.MenuItems[1]).Text = Translations.Translate("Copy");
            ((ApplicationBarMenuItem)this.ApplicationBar.MenuItems[2]).Text = Translations.Translate("Themes");
            ((ApplicationBarMenuItem)this.ApplicationBar.MenuItems[3]).Text = Translations.Translate("Download bibles");
            ((ApplicationBarMenuItem)this.ApplicationBar.MenuItems[4]).Text = Translations.Translate("Add a note");
            ((ApplicationBarMenuItem)this.ApplicationBar.MenuItems[5]).Text =
                Translations.Translate("Select bible to delete");
            ((ApplicationBarMenuItem)this.ApplicationBar.MenuItems[6]).Text =
                Translations.Translate("Select bookmark to delete");
            ((ApplicationBarMenuItem)this.ApplicationBar.MenuItems[7]).Text = Translations.Translate("Clear history");
            ((ApplicationBarMenuItem)this.ApplicationBar.MenuItems[8]).Text = Translations.Translate("Send message");
            ((ApplicationBarMenuItem)this.ApplicationBar.MenuItems[9]).Text = Translations.Translate("Send mail");
            ((ApplicationBarMenuItem)this.ApplicationBar.MenuItems[10]).Text = Translations.Translate("Add new window");
            ((ApplicationBarMenuItem)this.ApplicationBar.MenuItems[11]).Text = Translations.Translate(
                "Add to bookmarks");
            ((ApplicationBarMenuItem)this.ApplicationBar.MenuItems[12]).Text = Translations.Translate("Daily plan");
            ((ApplicationBarMenuItem)this.ApplicationBar.MenuItems[13]).Text = Translations.Translate("Settings");
            ((ApplicationBarMenuItem)this.ApplicationBar.MenuItems[14]).Text =
                Translations.Translate("Select the language") + " (language)";
            ((ApplicationBarMenuItem)this.ApplicationBar.MenuItems[15]).Text = Translations.Translate("Help");

            if (!App.OpenWindows.Any() || !App.InstalledBibles.InstalledBibles.Any())
            {
                if (!App.InstalledBibles.InstalledBibles.Any())
                {
                    if (App.IsFirstTimeInMainPageSplit == 0)
                    {
                        // cant have any open windows if there are no books!
                        App.OpenWindows.Clear();

                        // get some books.
                        this.NavigationService.Navigate(new Uri("/DownloadBooks.xaml", UriKind.Relative));
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
                        this.NavigationService.Navigate(new Uri("/WindowSettings.xaml", UriKind.Relative));
                        App.IsFirstTimeInMainPageSplit = 2;
                    }
                }
            }

            this.SetScreenWidthVariable();
            object objCurrrentScreen;
            if (PhoneApplicationService.Current.State.TryGetValue("CurrentScreen", out objCurrrentScreen))
            {
                this._currentScreen = (int)objCurrrentScreen;
                if (this._currentScreen >= App.DisplaySettings.NumberOfScreens)
                {
                    this._currentScreen = 0;
                    PhoneApplicationService.Current.State["CurrentScreen"] = this._currentScreen;
                }

                this.WindowGrid.Margin = new Thickness(-this._currentScreen * this._screenWidth, 0, 0, 0);
                this.DrawWindowSelectionButtons();
            }
            else
            {
                this.DrawWindowSelectionButtons();
            }

            this.ReDrawWindows();

            // figure out if this is a light color
            // var color = (Color) Application.Current.Resources["PhoneBackgroundColor"];
            // int lightColorCount = (color.R > 0x80 ? 1 : 0) + (color.G > 0x80 ? 1 : 0) + (color.B > 0x80 ? 1 : 0);
            // string colorDir = lightColorCount >= 2 ? "light" : "dark";
        }

        /// <summary>
        /// The menu add note click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void MenuAddNoteClick(object sender, EventArgs e)
        {
            if (App.PlaceMarkers.History.Count > 0)
            {
                PhoneApplicationService.Current.State.Remove("NoteToAddSaved");
                this.NavigationService.Navigate(new Uri("/AddNote.xaml", UriKind.Relative));
            }
            else
            {
                MessageBox.Show(Translations.Translate("You must first select a verse"));
            }
        }

        /// <summary>
        /// The menu clear history click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void MenuClearHistoryClick(object sender, EventArgs e)
        {
            App.PlaceMarkers.History = new List<BiblePlaceMarker>();
            App.RaiseHistoryChangeEvent();
        }

        /// <summary>
        /// The menu copy click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void MenuCopyClick(object sender, EventArgs e)
        {
            this.NavigationService.Navigate(new Uri("/CopyTexts.xaml", UriKind.Relative));
        }

        /// <summary>
        /// The menu delete bible click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void MenuDeleteBibleClick(object sender, EventArgs e)
        {
            this.NavigationService.Navigate(new Uri("/RemoveBibles.xaml", UriKind.Relative));
        }

        /// <summary>
        /// The menu delete bookmark click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void MenuDeleteBookmarkClick(object sender, EventArgs e)
        {
            this.NavigationService.Navigate(new Uri("/EditBookmarks.xaml", UriKind.Relative));
        }

        /// <summary>
        /// The menu download bible click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void MenuDownloadBibleClick(object sender, EventArgs e)
        {
            this.NavigationService.Navigate(new Uri("/DownloadBooks.xaml", UriKind.Relative));
        }

        /// <summary>
        /// The menu language click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void MenuLanguageClick(object sender, EventArgs e)
        {
            this.NavigationService.Navigate(new Uri("/SelectLanguage.xaml", UriKind.Relative));
        }

        /// <summary>
        /// The menu mail click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void MenuMailClick(object sender, EventArgs e)
        {
            string textsWithTitles;
            string titlesOnly;
            GetLast3SecondsChosenVerses(out textsWithTitles, out titlesOnly);
            var emailComposeTask = new EmailComposeTask { Body = textsWithTitles, Subject = titlesOnly };

            // emailComposeTask.To = "user@example.com";
            // emailComposeTask.Cc = "user2@example.com";
            emailComposeTask.Show();
        }

        /// <summary>
        /// The menu message click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void MenuMessageClick(object sender, EventArgs e)
        {
            string textsWithTitles;
            string titlesOnly;
            GetLast3SecondsChosenVerses(out textsWithTitles, out titlesOnly);
            var smsComposeTask = new SmsComposeTask { Body = textsWithTitles };

            // smsComposeTask.To = "5555555555";
            smsComposeTask.Show();
        }

        /// <summary>
        /// The menu rate this app click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void MenuRateThisAppClick(object sender, EventArgs e)
        {
            var task = new MarketplaceReviewTask();
            task.Show();
        }

        /// <summary>
        /// The menu settings click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void MenuSettingsClick(object sender, EventArgs e)
        {
            this.NavigationService.Navigate(new Uri("/Settings.xaml", UriKind.Relative));
        }

        /// <summary>
        /// The menu themes click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void MenuThemesClick(object sender, EventArgs e)
        {
            this.NavigationService.Navigate(new Uri("/Themes.xaml", UriKind.Relative));
        }

        /// <summary>
        /// The phone application page orientation changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void PhoneApplicationPageOrientationChanged(object sender, OrientationChangedEventArgs e)
        {
            this.SetScreenWidthVariable();
            this.LayoutMainRoot.Width = this._screenWidth * App.DisplaySettings.NumberOfScreens;
            foreach (ColumnDefinition colDef in this.WindowSelectGrid.ColumnDefinitions)
            {
                colDef.Width = new GridLength(this._screenWidth / this._windows.Count());
            }

            foreach (ColumnDefinition colDef in this.WindowGrid.ColumnDefinitions)
            {
                colDef.Width = new GridLength(this._screenWidth);
            }

            this.WindowGrid.Margin = new Thickness(-this._currentScreen * this._screenWidth, 0, 0, 0);

            // redraw the browsers
            for (int i = 0; i < App.OpenWindows.Count(); i++)
            {
                App.OpenWindows[i].CalculateTitleTextWidth();
                App.OpenWindows[i].ForceReload = true;
                App.OpenWindows[i].UpdateBrowser(true);
            }
        }

        /// <summary>
        /// The phone application page unloaded.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void PhoneApplicationPageUnloaded(object sender, RoutedEventArgs e)
        {
            foreach (ITiledWindow nextWindow in App.OpenWindows)
            {
                nextWindow.HitButtonBigger -= this.HitButtonBigger;
                nextWindow.HitButtonSmaller -= this.HitButtonSmaller;
                nextWindow.HitButtonClose -= this.HitButtonClose;
            }
        }

        /// <summary>
        /// The screen selection click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void ScreenSelectionClick(object sender, RoutedEventArgs e)
        {
            this.ShowScreen(int.Parse(((Button)sender).Content.ToString()) - 1);
        }

        /// <summary>
        /// The set screen width variable.
        /// </summary>
        private void SetScreenWidthVariable()
        {
            if (this.Orientation == PageOrientation.Landscape || this.Orientation == PageOrientation.LandscapeLeft
                || this.Orientation == PageOrientation.LandscapeRight)
            {
                this._screenWidth = Application.Current.Host.Content.ActualHeight - 70;
            }
            else
            {
                this._screenWidth = Application.Current.Host.Content.ActualWidth;
            }
        }

        /// <summary>
        /// The show screen.
        /// </summary>
        /// <param name="screenNum">
        /// The screen num.
        /// </param>
        private void ShowScreen(int screenNum)
        {
            if (this._isInScreenMoving)
            {
                return;
            }

            if (screenNum == this._currentScreen)
            {
                return;
            }

            this._isInScreenMoving = true;
            if (Math.Abs(this._screenWidth) < 0.1)
            {
                this.SetScreenWidthVariable();
            }

            this._screenPosIncrement = (this._currentScreen - screenNum) / Math.Abs(this._currentScreen - screenNum)
                                       * 80;
            this._currentScreen = screenNum;
            PhoneApplicationService.Current.State["CurrentScreen"] = this._currentScreen;
            this.DrawWindowSelectionButtons();

            // give a kick start to the animation
            var leftMargin = (int)this.WindowGrid.Margin.Left;
            leftMargin += this._screenPosIncrement * 3;
            this.WindowGrid.Margin = new Thickness(leftMargin, 0, 0, 0);

            // animate
            this._moveMultiScreenTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(5) };
            this._moveMultiScreenTimer.Tick += this.DoMoveMultiScreenTimerTick;
            this._moveMultiScreenTimer.Start();
        }

        #endregion
    }
}