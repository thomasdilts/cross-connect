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
// Email: thomas@cross-connect.se
// </summary>
// <author>Thomas Dilts</author>

namespace CrossConnect
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Linq;
    using CrossConnect.Common;
    using Sword.reader;
    using Windows.Foundation;
    using Windows.Storage;
    using Windows.Storage.Streams;
    using Windows.System;
    using Windows.UI.ApplicationSettings;
    using Windows.UI.Core;
    using Windows.UI.Popups;
    using Windows.UI.ViewManagement;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Automation;
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml.Media;
    using Windows.UI.Xaml.Media.Imaging;

    /// <summary>
    ///     A page that displays a group title, a list of items within the group, and details for the
    ///     currently selected item.
    /// </summary>
    public sealed partial class MainPageSplit : LayoutAwarePage
    {
        #region Constants

        private const int StartScreenWidth = 444;

        #endregion

        #region Fields

        private readonly List<Button> _buttons = new List<Button>();

        private readonly List<Grid> _windows = new List<Grid>();

        private bool isEventRegistered;

        private double _screenWidth;

        private BrowserTitledWindow[] views = new BrowserTitledWindow[6];

        #endregion

        #region Constructors and Destructors

        public MainPageSplit()
        {
            this.InitializeComponent();
        }

        #endregion

        #region Public Methods and Operators

        public async void ReDrawWindows(bool forceCompleteRedraw = false)
        {
            int numColumns = 1;
            foreach (ITiledWindow win in App.OpenWindows)
            {
                if ((win.State.Window + 1) > numColumns)
                {
                    numColumns = win.State.Window + 1;
                }
            }

            if (this._windows.Count() != numColumns || forceCompleteRedraw)
            {
                //LayoutMainRoot.Width = _screenWidth * numColumns;
                foreach (Grid window in this._windows)
                {
                    window.Children.Clear();
                }

                this._windows.Clear();
                this._buttons.Clear();

                this.WindowGrid.Children.Clear();
                this.WindowGrid.RowDefinitions.Clear();
                this.WindowGrid.ColumnDefinitions.Clear();

                // WindowGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(_screenHeight-45) });
                // SetScreenWidthVariable();

                object columnwidth;
                Rect bounds = Window.Current.Bounds;
                this._screenWidth = bounds.Width * 4 / 10;
                if (ApplicationData.Current.LocalSettings.Values.TryGetValue("ColumnWidth", out columnwidth))
                {
                    this._screenWidth = ((int)columnwidth);
                }

                for (int i = 0; i < numColumns; i++)
                {
                    var gd = new Grid();
                    Grid.SetColumn(gd, i);
                    Grid.SetRow(gd, 0);
                    this.WindowGrid.Children.Add(gd);
                    this.WindowGrid.ColumnDefinitions.Add(
                        new ColumnDefinition { Width = new GridLength(this._screenWidth) });
                    this._windows.Add(gd);
                }
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
                // todo fix these
                //var border = new SolidColorBrush(App.Themes.BorderColor);
                //var fore = new SolidColorBrush(App.Themes.MainFontColor);
                //var back = new SolidColorBrush(App.Themes.MainBackColor);

                // show just a quick menu to add window or bibles
                var text = new TextBlock { Text = "Cross Connect", FontSize = 40, /* todo Foreground = fore*/ };

                Grid.SetRow(text, 0);
                this._windows[0].Children.Add(text);
                var but = new Button { /* todo Background = back, Foreground = fore, BorderBrush = border */ };
                Grid.SetRow(but, 1);
                if ((!App.InstalledBibles.InstalledBibles.Any() && !App.InstalledBibles.InstalledCommentaries.Any() && !App.InstalledBibles.InstalledGeneralBooks.Any() && !App.InstalledBibles.InstalledDictionaries.Any()))
                {
                    but.Content = Translations.Translate("Download bible");
                    but.Click += this.MenuDownloadBibleClick;
                }
                else
                {
                    but.Content = Translations.Translate("Add a new window");
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
                    if (App.OpenWindows[i].State.Window >= numColumns)
                    {
                        App.OpenWindows.RemoveAt(i);
                        i--;
                        continue;
                    }
                    App.OpenWindows[i].ForceReload = true;

                    // make sure we are not doubled up on the events.
                    App.OpenWindows[i].HitButtonBigger -= this.HitButtonBigger;
                    App.OpenWindows[i].HitButtonSmaller -= this.HitButtonSmaller;
                    App.OpenWindows[i].HitButtonClose -= this.HitButtonClose;

                    // then add
                    App.OpenWindows[i].HitButtonBigger += this.HitButtonBigger;
                    App.OpenWindows[i].HitButtonSmaller += this.HitButtonSmaller;
                    App.OpenWindows[i].HitButtonClose += this.HitButtonClose;

                    App.OpenWindows[i].State.CurIndex = i;
                    Grid windowHolder = this._windows[App.OpenWindows[i].State.Window];
                    for (int j = 0; j < App.OpenWindows[i].State.NumRowsIown; j++)
                    {
                        var row = new RowDefinition();
                        windowHolder.RowDefinitions.Add(row);
                    }

                    Grid.SetRow((FrameworkElement)App.OpenWindows[i], rowCount[App.OpenWindows[i].State.Window]);
                    Grid.SetRowSpan((FrameworkElement)App.OpenWindows[i], App.OpenWindows[i].State.NumRowsIown);
                    Grid.SetColumn((FrameworkElement)App.OpenWindows[i], 0);
                    try
                    {
                        var ui = (UIElement)App.OpenWindows[i];
                        windowHolder.Children.Add(ui);
                    }
                    catch (Exception eeee)
                    {
                    }
                    rowCount[App.OpenWindows[i].State.Window] += App.OpenWindows[i].State.NumRowsIown;
                    App.OpenWindows[i].ShowSizeButtons();

                    App.OpenWindows[i].DelayUpdateBrowser();
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
                try
                {
                    StorageFolder localFolder = ApplicationData.Current.LocalFolder;
                    StorageFile file =
                        await localFolder.GetFileAsync(App.WebDirIsolated + "/images/" + App.Themes.MainBackImage);
                    IRandomAccessStreamWithContentType fstream = await file.OpenReadAsync();
                    var bitImage = new BitmapImage();
                    bitImage.SetSourceAsync(fstream);
                    var imageBrush = new ImageBrush { ImageSource = bitImage, };
                    this.LayoutMainRoot.Background = imageBrush;
                }
                catch (Exception ee)
                {
                    Debug.WriteLine(ee);
                    // todo fix these
                    //LayoutMainRoot.Background = new SolidColorBrush(App.Themes.MainBackColor);
                }
            }
            else
            {
                // todo fix these
                // LayoutMainRoot.Background = new SolidColorBrush(App.Themes.MainBackColor);
            }

            this.UpdateLayout();
            //foreach (var win in App.OpenWindows)
            //{
            //    win.RecalculateLayout();
            //}
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Invoked when the page's back button is pressed.
        /// </summary>
        /// <param name="sender">The back button instance.</param>
        /// <param name="e">Event data that describes how the back button was clicked.</param>
        protected override void GoBack(object sender, RoutedEventArgs e)
        {
        }

        /// <summary>
        ///     Populates the page with content passed during navigation.  Any saved state is also
        ///     provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="navigationParameter">
        ///     The parameter value passed to
        ///     <see cref="Frame.Navigate(Type, Object)" /> when this page was initially requested.
        /// </param>
        /// <param name="pageState">
        ///     A dictionary of state preserved by this page during an earlier
        ///     session.  This will be null the first time a page is visited.
        /// </param>
        protected override void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        {
            if (pageState == null)
            {
            }
        }

        /// <summary>
        ///     Preserves state associated with this page in case the application is suspended or the
        ///     page is discarded from the navigation cache.  Values must conform to the serialization
        ///     requirements of <see cref="SuspensionManager.SessionState" />.
        /// </summary>
        /// <param name="pageState">An empty dictionary to be populated with serializable state.</param>
        protected override void SaveState(Dictionary<String, Object> pageState)
        {
            //if (this.itemsViewSource.View != null)
            //{
            //    var selectedItem = (SampleDataItem)this.itemsViewSource.View.CurrentItem;
            //    if (selectedItem != null) pageState["SelectedItem"] = selectedItem.UniqueId;
            //}
        }

        // Visual state management typically reflects the four application view states directly
        // (full screen landscape and portrait plus snapped and filled views.)  The split page is
        // designed so that the snapped and portrait view states each have two distinct sub-states:
        // either the item list or the details are displayed, but not both at the same time.
        //
        // This is all implemented with a single physical page that can represent two logical
        // pages.  The code below achieves this goal without making the user aware of the
        // distinction.

        private void AutoRotatePageBackKeyPress(object sender, CancelEventArgs e)
        {
            Debug.WriteLine("Backed out of the program.");
        }

        private void BottomAppBar1_Closed_1(object sender, object e)
        {
            App.ShowUserInterface(true);
            
        }

        private void BottomAppBar1_Opened_1(object sender, object e)
        {
            this.scrollViewerBottomAppBar1.MaxWidth = Window.Current.Bounds.Width;
            this.scrollViewerTopAppBar1.MaxWidth = Window.Current.Bounds.Width;
            this.Dispatcher.RunAsync(CoreDispatcherPriority.Low, () => App.ShowUserInterface(false));
            //App.ShowUserInterface(false);
            BottomAppBar.IsOpen = true;
        }

        private void ButAddBookmarkClick(object sender, RoutedEventArgs e)
        {
            App.AddBookmark();
        }

        private void ButGoToPlanClick(object sender, RoutedEventArgs e)
        {
            App.AddWindow(string.Empty, string.Empty, WindowType.WindowDailyPlan, 10, string.Empty, null, 0);
        }

        private void ButHelpClick(object sender, RoutedEventArgs e)
        {
            Launcher.LaunchUriAsync(
                new Uri(@"http://www.cross-connect.se/help-metro?version=" + App.Version, UriKind.Absolute));
        }

        private void ColumnsSmaller_OnClick(object sender, RoutedEventArgs e)
        {
            Rect windowBounds = Window.Current.Bounds;
            object columnwidth;
            var columnWidthInteger = (int)(windowBounds.Width * 4 / 10);
            if (ApplicationData.Current.LocalSettings.Values.TryGetValue("ColumnWidth", out columnwidth))
            {
                columnWidthInteger = (int)columnwidth;
            }
            columnWidthInteger -= (int)(windowBounds.Width / 20);
            if (columnWidthInteger > windowBounds.Width)
            {
                columnWidthInteger = (int)windowBounds.Width;
            }
            if (columnWidthInteger < windowBounds.Width / 8)
            {
                columnWidthInteger = (int)(windowBounds.Width / 8);
            }
            ApplicationData.Current.LocalSettings.Values["ColumnWidth"] = columnWidthInteger;
            App.ShowUserInterface(true);
            this.ReDrawWindows(true);
            this.Dispatcher.RunAsync(CoreDispatcherPriority.Low, () => App.ShowUserInterface(false));
        }

        private void ColumnsWider_OnClick(object sender, RoutedEventArgs e)
        {
            Rect windowBounds = Window.Current.Bounds;
            object columnwidth;
            var columnWidthInteger = (int)(windowBounds.Width * 4 / 10);
            if (ApplicationData.Current.LocalSettings.Values.TryGetValue("ColumnWidth", out columnwidth))
            {
                columnWidthInteger = (int)columnwidth;
            }
            columnWidthInteger += (int)(windowBounds.Width / 20);
            if (columnWidthInteger > windowBounds.Width)
            {
                columnWidthInteger = (int)windowBounds.Width;
            }
            if (columnWidthInteger < windowBounds.Width / 8)
            {
                columnWidthInteger = (int)(windowBounds.Width / 8);
            }
            ApplicationData.Current.LocalSettings.Values["ColumnWidth"] = columnWidthInteger;
            App.ShowUserInterface(true);
            this.ReDrawWindows(true);
            this.Dispatcher.RunAsync(CoreDispatcherPriority.Low, () => App.ShowUserInterface(false));
        }

        private void HitButtonBigger(object sender, EventArgs e)
        {
            this.ReDrawWindows();
        }

        private void HitButtonClose(object sender, EventArgs e)
        {
            App.OpenWindows.RemoveAt(((ITiledWindow)sender).State.CurIndex);
            this.ReDrawWindows();
        }

        private void HitButtonSmaller(object sender, EventArgs e)
        {
            this.ReDrawWindows();
        }

        /// <summary>
        ///     Invoked when an item within the list is selected.
        /// </summary>
        /// <param name="sender">
        ///     The GridView (or ListView when the application is Snapped)
        ///     displaying the selected item.
        /// </param>
        /// <param name="e">Event data that describes how the selection was changed.</param>
        private void ItemListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Invalidate the view state when logical page navigation is in effect, as a change
            // in selection may cause a corresponding change in the current logical page.  When
            // an item is selected this has the effect of changing from displaying the item list
            // to showing the selected item's details.  When the selection is cleared this has the
            // opposite effect.
            if (this.UsingLogicalPageNavigation())
            {
                this.InvalidateVisualState();
            }
        }

        /// <summary>
        /// This event is generated when the user opens the settings pane. During this event, append your
        /// SettingsCommand objects to the available ApplicationCommands vector to make them available to the
        /// SettingsPange UI.
        /// </summary>
        /// <param name="settingsPane">Instance that triggered the event.</param>
        /// <param name="eventArgs">Event data describing the conditions that led to the event.</param>
        void onCommandsRequested(SettingsPane settingsPane, SettingsPaneCommandsRequestedEventArgs eventArgs)
        {
            eventArgs.Request.ApplicationCommands.Add(new SettingsCommand("SettingsId", Translations.Translate("Settings"), new UICommandInvokedHandler(onSettingsSettingsCommand)));
            eventArgs.Request.ApplicationCommands.Add(new SettingsCommand("AddWindowId", Translations.Translate("Add a new window"), new UICommandInvokedHandler(onSettingsAddNewWindowCommand)));
            eventArgs.Request.ApplicationCommands.Add(new SettingsCommand("DownBibleId", Translations.Translate("Download bible"), new UICommandInvokedHandler(onSettingsDownloadBiblesCommand)));
            eventArgs.Request.ApplicationCommands.Add(new SettingsCommand("ThemesId", Translations.Translate("Themes"), new UICommandInvokedHandler(onSettingsThemesCommand)));
            eventArgs.Request.ApplicationCommands.Add(new SettingsCommand("LangId", Translations.Translate("Select the language"), new UICommandInvokedHandler(onSettingsSelectLanguageCommand)));
            eventArgs.Request.ApplicationCommands.Add(new SettingsCommand("PrivacyId", Translations.Translate("Privacy policy"), new UICommandInvokedHandler(onSettingsPrivacyPolicyCommand)));
            eventArgs.Request.ApplicationCommands.Add(new SettingsCommand("HelpId", Translations.Translate("Help"), new UICommandInvokedHandler(onSettingsHelpCommand)));
        }
        void onSettingsAddNewWindowCommand(IUICommand command)
        {
            ButAddWindowClick(null,null);
        }
        void onSettingsSettingsCommand(IUICommand command)
        {
            MenuSettingsClick(null,null);
        }
        void onSettingsDownloadBiblesCommand(IUICommand command)
        {
            MenuDownloadBibleClick(null,null);
        }
        void onSettingsThemesCommand(IUICommand command)
        {
            MenuThemesClick(null,null);
        }
        void onSettingsHelpCommand(IUICommand command)
        {
            ButHelpClick(null,null);
        }
        void onSettingsClearHistoryCommand(IUICommand command)
        {
            MenuClearHistoryClick(null,null);
        }        
        void onSettingsSelectLanguageCommand(IUICommand command)
        {
            MenuLanguageClick(null,null);
        }
        void onSettingsDeleteBibleCommand(IUICommand command)
        {
            MenuDeleteBibleClick(null,null);
        }
        void onSettingsDeleteBookmarksCommand(IUICommand command)
        {
            MenuDeleteBookmarkClick(null,null);
        }
        
        void onSettingsPrivacyPolicyCommand(IUICommand command)
        {
             Launcher.LaunchUriAsync(
                new Uri(@"http://www.cross-connect.se/help-metro/privacy-policy?version=" + App.Version, UriKind.Absolute));
        }

        // Load data for the ViewModel Items
        private void MainPageLoaded(object sender, RoutedEventArgs e)
        {
            RedrawMainScreen(false);
            sliderTextSize.ValueChanged += SliderTextSizeValueChanged;
        }

        public void RecolorScreen()
        {
            for (int i = App.OpenWindows.Count - 1; i >= 0; i--)
            {
                App.OpenWindows[i].ForceReload = true;
                App.OpenWindows[i].UpdateBrowser(false);
            }
            this.LayoutMainRoot.Background = new SolidColorBrush(App.Themes.FrameColor);
            this.LeftSpacer.Background = new SolidColorBrush(App.Themes.FrameColor);
            this.RightSpacer.Background = new SolidColorBrush(App.Themes.FrameColor);
            this.LayoutMainRoot.UpdateLayout();
        }

        public void RedrawMainScreen(bool isForceRedraw)
        {
            App.MainWindow = this;
            if (!this.isEventRegistered)
            {
                // Listening for this event lets the app initialize the settings commands and pause its UI until the user closes the pane.
                // To ensure your settings are available at all times in your app, place your CommandsRequested handler in the overridden
                // OnWindowCreated of App.xaml.cs
                SettingsPane.GetForCurrentView().CommandsRequested += onCommandsRequested;
                this.isEventRegistered = true;
            }

            this.SetLanguageDependentTexts();

            if (!App.OpenWindows.Any() || (!App.InstalledBibles.InstalledBibles.Any() && !App.InstalledBibles.InstalledCommentaries.Any() && !App.InstalledBibles.InstalledGeneralBooks.Any() && !App.InstalledBibles.InstalledDictionaries.Any()))
            {
                if (!App.InstalledBibles.InstalledBibles.Any() && !App.InstalledBibles.InstalledCommentaries.Any() && !App.InstalledBibles.InstalledGeneralBooks.Any() && !App.InstalledBibles.InstalledDictionaries.Any())
                {
                    if (App.IsFirstTimeInMainPageSplit == 0)
                    {
                        // cant have any open windows if there are no books!
                        App.OpenWindows.Clear();

                        // get some books.
                        this.MenuDownloadBibleClick(null, null);
                        App.IsFirstTimeInMainPageSplit = 1;
                    }
                }
                else
                {
                    if (App.IsFirstTimeInMainPageSplit <= 1)
                    {
                        this.ButAddWindowClick(null, null);
                        App.IsFirstTimeInMainPageSplit = 2;
                    }
                }
            }

            object columnwidth;
            this._screenWidth = StartScreenWidth;
            if (ApplicationData.Current.LocalSettings.Values.TryGetValue("ColumnWidth", out columnwidth))
            {
                this._screenWidth = ((int)columnwidth);
            }

            this.ReDrawWindows(isForceRedraw);

            // figure out if this is a light color
            // var color = (Color) Application.Current.Resources["PhoneBackgroundColor"];
            // int lightColorCount = (color.R > 0x80 ? 1 : 0) + (color.G > 0x80 ? 1 : 0) + (color.B > 0x80 ? 1 : 0);
            // string colorDir = lightColorCount >= 2 ? "light" : "dark";
            this.LayoutMainRoot.Background = new SolidColorBrush(App.Themes.FrameColor);
            this.LeftSpacer.Background = new SolidColorBrush(App.Themes.FrameColor);
            this.RightSpacer.Background = new SolidColorBrush(App.Themes.FrameColor);
        }

        //private void PhoneApplicationPageOrientationChanged(object sender, OrientationChangedEventArgs e)
        //{
        //    SetScreenWidthVariable();
        //    LayoutMainRoot.Width = _screenWidth * numColumns;
        //    foreach (ColumnDefinition colDef in WindowSelectGrid.ColumnDefinitions)
        //    {
        //        colDef.Width = new GridLength(_screenWidth / _windows.Count());
        //    }

        //    foreach (ColumnDefinition colDef in WindowGrid.ColumnDefinitions)
        //    {
        //        colDef.Width = new GridLength(_screenWidth);
        //    }

        //    WindowGrid.Margin = new Thickness(-_currentScreen * _screenWidth, 0, 0, 0);

        //    // redraw the browsers
        //    for (int i = 0; i < App.OpenWindows.Count(); i++)
        //    {
        //        App.OpenWindows[i].CalculateTitleTextWidth();
        //        App.OpenWindows[i].ForceReload = true;
        //        App.OpenWindows[i].UpdateBrowser(true);
        //    }
        //}

        private void MainPageSplitUnloaded(object sender, RoutedEventArgs e)
        {
            foreach (ITiledWindow nextWindow in App.OpenWindows)
            {
                nextWindow.HitButtonBigger -= this.HitButtonBigger;
                nextWindow.HitButtonSmaller -= this.HitButtonSmaller;
                nextWindow.HitButtonClose -= this.HitButtonClose;
            }

            foreach (Grid window in this._windows)
            {
                window.Children.Clear();
            }

            this._windows.Clear();
        }

        private void MenuClearHistoryClick(object sender, RoutedEventArgs e)
        {
            App.PlaceMarkers.History = new List<BiblePlaceMarker>();
            App.RaiseHistoryChangeEvent();
            this.TopAppBar1.IsOpen = false;
            this.BottomAppBar.IsOpen = false;
            App.SavePersistantMarkers();
        }

        private void SetLanguageDependentTexts()
        {
            this.AddNewWindow.SetValue(AutomationProperties.NameProperty, Translations.Translate("Add a new window"));
            //AddToBookMarks.SetValue(AutomationProperties.NameProperty, Translations.Translate("Add to bookmarks"));
            this.Settings.SetValue(AutomationProperties.NameProperty, Translations.Translate("Settings"));
            this.DownloadBibles.SetValue(AutomationProperties.NameProperty, Translations.Translate("Download bible"));
            this.Themes.SetValue(AutomationProperties.NameProperty, Translations.Translate("Themes"));

            this.ColumnsWider.SetValue(AutomationProperties.NameProperty, Translations.Translate("Columns wider"));
            this.ColumnsSmaller.SetValue(AutomationProperties.NameProperty, Translations.Translate("Columns narrower"));

            this.Help.SetValue(AutomationProperties.NameProperty, Translations.Translate("Help"));
            //AddANote.SetValue(AutomationProperties.NameProperty, Translations.Translate("Add a note"));
            this.ClearHistory.SetValue(AutomationProperties.NameProperty, Translations.Translate("Clear history"));
            this.SelectLanguage.SetValue(
                AutomationProperties.NameProperty, Translations.Translate("Select the language"));
            this.DeleteBible.SetValue(
                AutomationProperties.NameProperty, Translations.Translate("Select bible to delete"));
            this.DeleteBookmarks.SetValue(
                AutomationProperties.NameProperty, Translations.Translate("Select a bookmark to delete"));
            this.OneDriveBackupRestore.SetValue(
                AutomationProperties.NameProperty, Translations.Translate("OneDrive backup / restore"));
        }

        /// <summary>
        ///     Invoked to determine whether the page should act as one logical page or two.
        /// </summary>
        /// <param name="viewState">
        ///     The view state for which the question is being posed, or null
        ///     for the current view state.  This parameter is optional with null as the default
        ///     value.
        /// </param>
        /// <returns>
        ///     True when the view state in question is portrait or snapped, false
        ///     otherwise.
        /// </returns>
        private bool UsingLogicalPageNavigation(ApplicationViewState? viewState = null)
        {
            if (viewState == null)
            {
                viewState = ApplicationView.Value;
            }
            return viewState == ApplicationViewState.FullScreenPortrait || viewState == ApplicationViewState.Snapped;
        }

        #endregion

    }
}