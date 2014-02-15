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
    using System.Threading.Tasks;

    public partial class MainPageSplit
    {
        #region Fields

        private readonly List<Button> _buttons = new List<Button>();
        private readonly List<Grid> _windows = new List<Grid>();

        private int _currentScreen;
        private bool _isInScreenMoving;
        private DispatcherTimer _moveMultiScreenTimer;
        private int _overrideCurrentScreen = -1;
        private DispatcherTimer _overrideShowingScreenTimer;
        private int _screenPosIncrement;
        private double _screenWidth;

        #endregion Fields

        #region Constructors

        public MainPageSplit()
        {
            InitializeComponent();
        }

        #endregion Constructors

        #region Methods

        public void OverRideCurrentlyShowingScreen(int screenNum)
        {
            if (_currentScreen != screenNum)
            {
                _overrideCurrentScreen = screenNum;
                _overrideShowingScreenTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(1000) };
                _overrideShowingScreenTimer.Tick += OverRideCurrentlyShowingScreenTimerTick;
                _overrideShowingScreenTimer.Start();
            }
        }

        public void ReDrawWindows()
        {
            if (_windows.Count() != App.DisplaySettings.NumberOfScreens)
            {
                if (_currentScreen >= App.DisplaySettings.NumberOfScreens)
                {
                    _currentScreen = 0;
                    PhoneApplicationService.Current.State["CurrentScreen"] = _currentScreen;
                    WindowGrid.Margin = new Thickness(-_currentScreen * _screenWidth, 0, 0, 0);
                }

                LayoutMainRoot.Width = _screenWidth * App.DisplaySettings.NumberOfScreens;
                foreach (Grid window in _windows)
                {
                    window.Children.Clear();
                }

                _windows.Clear();
                _buttons.Clear();
                WindowSelectGrid.Children.Clear();
                WindowSelectGrid.RowDefinitions.Clear();
                WindowSelectGrid.ColumnDefinitions.Clear();
                WindowSelectGrid.RowDefinitions.Add(
                    new RowDefinition { Height = new GridLength(App.DisplaySettings.NumberOfScreens == 1 ? 0 : 45) });
                WindowGrid.Children.Clear();
                WindowGrid.RowDefinitions.Clear();
                WindowGrid.ColumnDefinitions.Clear();

                // WindowGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(_screenHeight-45) });
                SetScreenWidthVariable();
                for (int i = 0; i < App.DisplaySettings.NumberOfScreens; i++)
                {
                    if (App.DisplaySettings.NumberOfScreens > 1)
                    {
                        var but = new Button { Content = (i + 1).ToString(CultureInfo.InvariantCulture), Margin = new Thickness(-5, -10, -5, -10) };
                        but.Click += ScreenSelectionClick;
                        Grid.SetColumn(but, i);
                        Grid.SetRow(but, 0);
                        WindowSelectGrid.Children.Add(but);
                        WindowSelectGrid.ColumnDefinitions.Add(
                            new ColumnDefinition
                                {
                                   Width = new GridLength(_screenWidth / App.DisplaySettings.NumberOfScreens)
                                });
                        _buttons.Add(but);
                    }

                    var gd = new Grid();
                    Grid.SetColumn(gd, i);
                    Grid.SetRow(gd, 0);
                    WindowGrid.Children.Add(gd);
                    WindowGrid.ColumnDefinitions.Add(
                        new ColumnDefinition { Width = new GridLength(_screenWidth) });
                    _windows.Add(gd);
                }

                DrawWindowSelectionButtons();
            }

            foreach (Grid grid in _windows)
            {
                grid.Children.Clear();
                grid.ColumnDefinitions.Clear();
                grid.RowDefinitions.Clear();
            }

            if (!App.OpenWindows.Any())
            {
                var row = new RowDefinition { Height = GridLength.Auto };
                _windows[0].RowDefinitions.Add(row);
                row = new RowDefinition { Height = GridLength.Auto };
                _windows[0].RowDefinitions.Add(row);
                row = new RowDefinition { Height = new GridLength(0, GridUnitType.Star) };
                _windows[0].RowDefinitions.Add(row);
                var border = new SolidColorBrush(App.Themes.BorderColor);
                var fore = new SolidColorBrush(App.Themes.MainFontColor);
                var back = new SolidColorBrush(App.Themes.MainBackColor);

                // show just a quick menu to add window or bibles
                var text = new TextBlock { Text = "Cross Connect", FontSize = 40, Foreground = fore };

                Grid.SetRow(text, 0);
                _windows[0].Children.Add(text);
                var but = new Button { Background = back, Foreground = fore, BorderBrush = border };
                Grid.SetRow(but, 1);
                if (!App.InstalledBibles.InstalledBibles.Any())
                {
                    but.Content = Translations.Translate("Download bibles");
                    but.Click += MenuDownloadBibleClick;
                }
                else
                {
                    but.Content = Translations.Translate("Add new window");
                    but.Click += ButAddWindowClick;
                }

                _windows[0].Children.Add(but);
            }
            else
            {
                // var removeWindows = new List<int>();
                var rowCount = new int[_windows.Count];
                for (int i = 0; i < App.OpenWindows.Count(); i++)
                {
                    if (App.OpenWindows[i].State.Window >= App.DisplaySettings.NumberOfScreens)
                    {
                        App.OpenWindows.RemoveAt(i);
                        i--;
                        continue;
                    }

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
                        _windows[App.OpenWindows[i].State.Window].RowDefinitions.Add(row);
                    }

                    Grid.SetRow((FrameworkElement)App.OpenWindows[i], rowCount[App.OpenWindows[i].State.Window]);
                    Grid.SetRowSpan((FrameworkElement)App.OpenWindows[i], App.OpenWindows[i].State.NumRowsIown);
                    Grid.SetColumn((FrameworkElement)App.OpenWindows[i], 0);
                    _windows[App.OpenWindows[i].State.Window].Children.Add((UIElement)App.OpenWindows[i]);
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
                                LayoutMainRoot.Background = imageBrush;
                            }
                        }
                        catch (Exception ee)
                        {
                            Debug.WriteLine(ee);
                            LayoutMainRoot.Background = new SolidColorBrush(App.Themes.MainBackColor);
                        }
                    }
                }
            }
            else
            {
                LayoutMainRoot.Background = new SolidColorBrush(App.Themes.MainBackColor);
            }
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
            App.AddWindow(string.Empty, string.Empty, WindowType.WindowDailyPlan, 10);
        }

        private void ButHelpClick(object sender, EventArgs e)
        {
            var webBrowserTask = new WebBrowserTask
                {
                   Uri = new Uri(@"http://www.cross-connect.se/help?version=" + App.Version)
                };
            webBrowserTask.Show();
        }

        private void DoMoveMultiScreenTimerTick(object sender, EventArgs e)
        {
            var leftMargin = (int)WindowGrid.Margin.Left;
            leftMargin += _screenPosIncrement;
            WindowGrid.Margin = new Thickness(leftMargin, 0, 0, 0);
            if (Math.Abs(leftMargin + (_currentScreen * _screenWidth)) < 71)
            {
                // make sure the position is correct
                WindowGrid.Margin = new Thickness(-_currentScreen * _screenWidth, 0, 0, 0);
                _moveMultiScreenTimer.Stop();
                _isInScreenMoving = false;
            }
        }

        private void DrawWindowSelectionButtons()
        {
            var border = new SolidColorBrush(App.Themes.BorderColor);
            var fore = new SolidColorBrush(App.Themes.MainFontColor);
            var back = new SolidColorBrush(App.Themes.MainBackColor);
            WindowSelectGrid.Background = back;
            for (int i = 0; i < _buttons.Count(); i++)
            {
                _buttons[i].Background = _currentScreen == i ? fore : back;
                _buttons[i].Foreground = _currentScreen == i ? back : fore;
                _buttons[i].BorderBrush = border;
            }
        }

        private void HitButtonBigger(object sender, EventArgs e)
        {
            ReDrawWindows();
        }

        private void HitButtonClose(object sender, EventArgs e)
        {
            App.OpenWindows.RemoveAt(((ITiledWindow)sender).State.CurIndex);
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
            if (App.IsPersitanceLoaded)
            {
                DoLoading();
            }
        }

        public void DoLoading()
        {
            ((ApplicationBarIconButton)ApplicationBar.Buttons[0]).Text = Translations.Translate("Add new window");
            ((ApplicationBarIconButton)ApplicationBar.Buttons[1]).Text = Translations.Translate("Add to bookmarks");
            ((ApplicationBarIconButton)ApplicationBar.Buttons[2]).Text = Translations.Translate("Daily plan");
            ((ApplicationBarIconButton)ApplicationBar.Buttons[3]).Text = Translations.Translate("Help");

            ((ApplicationBarMenuItem)ApplicationBar.MenuItems[0]).Text = Translations.Translate(
                "Rate this program");
            ((ApplicationBarMenuItem)ApplicationBar.MenuItems[1]).Text = Translations.Translate("Highlight");
            ((ApplicationBarMenuItem)ApplicationBar.MenuItems[2]).Text = Translations.Translate("Copy");
            ((ApplicationBarMenuItem)ApplicationBar.MenuItems[3]).Text = Translations.Translate("Themes");
            ((ApplicationBarMenuItem)ApplicationBar.MenuItems[4]).Text = Translations.Translate("Download bibles");
            ((ApplicationBarMenuItem)ApplicationBar.MenuItems[5]).Text = Translations.Translate("Add a note");
            ((ApplicationBarMenuItem)ApplicationBar.MenuItems[6]).Text =
                Translations.Translate("Select bible to delete");
            ((ApplicationBarMenuItem)ApplicationBar.MenuItems[7]).Text =
                Translations.Translate("Select bookmark to delete");
            ((ApplicationBarMenuItem)ApplicationBar.MenuItems[8]).Text = Translations.Translate("Clear history");
            ((ApplicationBarMenuItem)ApplicationBar.MenuItems[9]).Text = Translations.Translate("Send message");
            ((ApplicationBarMenuItem)ApplicationBar.MenuItems[10]).Text = Translations.Translate("Send mail");
            ((ApplicationBarMenuItem)ApplicationBar.MenuItems[11]).Text = Translations.Translate("Add new window");
            ((ApplicationBarMenuItem)ApplicationBar.MenuItems[12]).Text = Translations.Translate(
                "Add to bookmarks");
            ((ApplicationBarMenuItem)ApplicationBar.MenuItems[13]).Text = Translations.Translate("Daily plan");
            ((ApplicationBarMenuItem)ApplicationBar.MenuItems[14]).Text = Translations.Translate("Settings");
            ((ApplicationBarMenuItem)ApplicationBar.MenuItems[15]).Text =
                Translations.Translate("Select the language") + " (language)";
            ((ApplicationBarMenuItem)ApplicationBar.MenuItems[16]).Text = Translations.Translate("Help");

            if (!App.OpenWindows.Any() || !App.InstalledBibles.InstalledBibles.Any())
            {
                if (!App.InstalledBibles.InstalledBibles.Any())
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

            SetScreenWidthVariable();
            object objCurrrentScreen;
            if (PhoneApplicationService.Current.State.TryGetValue("CurrentScreen", out objCurrrentScreen))
            {
                _currentScreen = (int)objCurrrentScreen;
                if (_currentScreen >= App.DisplaySettings.NumberOfScreens)
                {
                    _currentScreen = 0;
                    PhoneApplicationService.Current.State["CurrentScreen"] = _currentScreen;
                }

                WindowGrid.Margin = new Thickness(-_currentScreen * _screenWidth, 0, 0, 0);
                DrawWindowSelectionButtons();
            }
            else
            {
                DrawWindowSelectionButtons();
            }

            ReDrawWindows();

            // figure out if this is a light color
            // var color = (Color) Application.Current.Resources["PhoneBackgroundColor"];
            // int lightColorCount = (color.R > 0x80 ? 1 : 0) + (color.G > 0x80 ? 1 : 0) + (color.B > 0x80 ? 1 : 0);
            // string colorDir = lightColorCount >= 2 ? "light" : "dark";
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

        private void MenuCopyClick(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/CopyTexts.xaml", UriKind.Relative));
        }
        private void MenuHighlightClick(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/SelectHighlight.xaml", UriKind.Relative));
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

        private async void MenuMailClick(object sender, EventArgs e)
        {
            object openWindowIndex;
            if (!PhoneApplicationService.Current.State.TryGetValue("openWindowIndex", out openWindowIndex))
            {
                openWindowIndex = 0;
            }
            if (App.OpenWindows[(int)openWindowIndex] is BrowserTitledWindow)
            {
                var obj = await ((BrowserTitledWindow)App.OpenWindows[(int)openWindowIndex]).GetLast3SecondsChosenVerses();
                var textsWithTitles = (string)obj[0];
                var titlesOnly = (string)obj[1];
                var emailComposeTask = new EmailComposeTask { Body = textsWithTitles, Subject = titlesOnly };

                // emailComposeTask.To = "user@example.com";
                // emailComposeTask.Cc = "user2@example.com";
                emailComposeTask.Show();
            }
        }

        private async void MenuMessageClick(object sender, EventArgs e)
        {
            object openWindowIndex;
            if (!PhoneApplicationService.Current.State.TryGetValue("openWindowIndex", out openWindowIndex))
            {
                openWindowIndex = 0;
            }
            if (App.OpenWindows[(int)openWindowIndex] is BrowserTitledWindow)
            {
                var obj = await ((BrowserTitledWindow)App.OpenWindows[(int)openWindowIndex]).GetLast3SecondsChosenVerses();
                string textsWithTitles = (string)obj[0];
                string titlesOnly = (string)obj[1];
                var smsComposeTask = new SmsComposeTask { Body = textsWithTitles };

                // smsComposeTask.To = "5555555555";
                smsComposeTask.Show();
            }
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

        private void MenuThemesClick(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/Themes.xaml", UriKind.Relative));
        }

        private void OverRideCurrentlyShowingScreenTimerTick(object sender, EventArgs e)
        {
            _overrideShowingScreenTimer.Stop();
            this.ShowScreen(_overrideCurrentScreen);
        }

        private void PhoneApplicationPageOrientationChanged(object sender, OrientationChangedEventArgs e)
        {
            SetScreenWidthVariable();
            LayoutMainRoot.Width = _screenWidth * App.DisplaySettings.NumberOfScreens;
            foreach (ColumnDefinition colDef in WindowSelectGrid.ColumnDefinitions)
            {
                colDef.Width = new GridLength(_screenWidth / _windows.Count());
            }

            foreach (ColumnDefinition colDef in WindowGrid.ColumnDefinitions)
            {
                colDef.Width = new GridLength(_screenWidth);
            }

            WindowGrid.Margin = new Thickness(-_currentScreen * _screenWidth, 0, 0, 0);

            // redraw the browsers
            for (int i = 0; i < App.OpenWindows.Count(); i++)
            {
                App.OpenWindows[i].CalculateTitleTextWidth();
                App.OpenWindows[i].ForceReload = true;
                App.OpenWindows[i].UpdateBrowser(true);
            }
        }

        private void PhoneApplicationPageUnloaded(object sender, RoutedEventArgs e)
        {
            foreach (ITiledWindow nextWindow in App.OpenWindows)
            {
                nextWindow.HitButtonBigger -= HitButtonBigger;
                nextWindow.HitButtonSmaller -= HitButtonSmaller;
                nextWindow.HitButtonClose -= HitButtonClose;
            }
        }

        private void ScreenSelectionClick(object sender, RoutedEventArgs e)
        {
            ShowScreen(int.Parse(((Button)sender).Content.ToString()) - 1);
        }

        private void SetScreenWidthVariable()
        {
            if (Orientation == PageOrientation.Landscape || Orientation == PageOrientation.LandscapeLeft
                || Orientation == PageOrientation.LandscapeRight)
            {
                _screenWidth = Application.Current.Host.Content.ActualHeight - 70;
            }
            else
            {
                _screenWidth = Application.Current.Host.Content.ActualWidth;
            }
        }

        private void ShowScreen(int screenNum)
        {
            if (_isInScreenMoving)
            {
                return;
            }

            if (screenNum == _currentScreen)
            {
                return;
            }

            _isInScreenMoving = true;
            if (Math.Abs(_screenWidth) < 0.1)
            {
                SetScreenWidthVariable();
            }

            _screenPosIncrement = (_currentScreen - screenNum) / Math.Abs(_currentScreen - screenNum)
                                       * 80;
            _currentScreen = screenNum;
            PhoneApplicationService.Current.State["CurrentScreen"] = _currentScreen;
            DrawWindowSelectionButtons();

            // give a kick start to the animation
            var leftMargin = (int)WindowGrid.Margin.Left;
            leftMargin += _screenPosIncrement * 3;
            WindowGrid.Margin = new Thickness(leftMargin, 0, 0, 0);

            // animate
            _moveMultiScreenTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(5) };
            _moveMultiScreenTimer.Tick += DoMoveMultiScreenTimerTick;
            _moveMultiScreenTimer.Start();
        }

        #endregion Methods

    }
}