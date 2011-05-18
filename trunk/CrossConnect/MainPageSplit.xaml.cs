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
/// <copyright file="THIS_FILE.cs" company="Thomas Dilts">
///     Thomas Dilts. All rights reserved.
/// </copyright>
/// <author>Thomas Dilts</author>
namespace CrossConnect
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.IO.IsolatedStorage;
    using System.Linq;
    using System.Net;
    using System.Runtime.Serialization;
    using System.Text;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using System.Windows.Shapes;
    using System.Xml;

    using Microsoft.Phone.Controls;
    using Microsoft.Phone.Shell;
    using Microsoft.Phone.Tasks;

    public partial class MainPageSplit : PhoneApplicationPage
    {
        #region Fields

        System.Windows.Threading.DispatcherTimer menuDownAnimation = null;
        System.Windows.Threading.DispatcherTimer menuUpAnimation = null;
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
                    but.Click += butDownLoadBibles_Click;
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

        private void butAddWindow_Click(object sender, RoutedEventArgs e)
        {
            App.windowSettings.isAddNewWindowOnly = true;
            App.windowSettings.skipWindowSettings = false;
            this.NavigationService.Navigate(new Uri("/WindowSettings.xaml", UriKind.Relative));
        }

        private void butDownLoadBibles_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new Uri("/DownloadBooks.xaml", UriKind.Relative));
        }

        private void butDownload_Click(object sender, EventArgs e)
        {
            this.NavigationService.Navigate(new Uri("/DownloadBooks.xaml", UriKind.Relative));
        }

        private void butHelp_Click(object sender, EventArgs e)
        {
            WebBrowserTask webBrowserTask = new WebBrowserTask();
            string version = "1.0.0.1";
            webBrowserTask.URL = @"http://www.chaniel.se/crossconnect/help?version=" + version;
            webBrowserTask.Show();
        }

        private void butNewWindow_Click(object sender, EventArgs e)
        {
            App.windowSettings.isAddNewWindowOnly = true;
            App.windowSettings.skipWindowSettings = false;
            this.NavigationService.Navigate(new Uri("/WindowSettings.xaml", UriKind.Relative));
        }

        private TextBlock createTextBlock(string text,string tag)
        {
            TextBlock textBlock = new TextBlock();
            textBlock.Text = text;
            textBlock.Tag = tag;
            return textBlock;
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

            ((ApplicationBarIconButton)this.ApplicationBar.Buttons[0]).Text = Translations.translate("Download bibles");
            ((ApplicationBarIconButton)this.ApplicationBar.Buttons[1]).Text = Translations.translate("Add new window");
            ((ApplicationBarIconButton)this.ApplicationBar.Buttons[2]).Text = Translations.translate("Help");
            ((ApplicationBarMenuItem)this.ApplicationBar.MenuItems[0]).Text = Translations.translate("Select bible to delete");
            ((ApplicationBarMenuItem)this.ApplicationBar.MenuItems[1]).Text = Translations.translate("Select bookmark to delete");
            ((ApplicationBarMenuItem)this.ApplicationBar.MenuItems[2]).Text = Translations.translate("Clear history");

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
                        App.windowSettings.skipWindowSettings = false;
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

        private void PhoneApplicationPage_OrientationChanged(object sender, OrientationChangedEventArgs e)
        {
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