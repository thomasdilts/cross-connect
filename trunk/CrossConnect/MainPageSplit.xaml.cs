using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using System.Text;
using System.IO.IsolatedStorage;
using System.Runtime.Serialization;
using System.IO;
using System.Xml;
///
/// <summary> Distribution License:
/// JSword is free software; you can redistribute it and/or modify it under
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
///
/// Copyright: 2011
///     The copyright to this program is held by Thomas Dilts
///  
namespace CrossConnect
{
    public partial class MainPageSplit : PhoneApplicationPage
    {
        // Constructor
        public MainPageSplit()
        {
            InitializeComponent();
        }

        // Load data for the ViewModel Items
        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            App.mainWindow = this;
            foreach (var nextWindow in App.openWindows)
            {
                nextWindow.HitButtonBigger += HitButtonBigger;
                nextWindow.HitButtonSmaller += HitButtonSmaller;
                nextWindow.HitButtonClose += HitButtonClose;
            }

            canvas1.Margin = new Thickness(0, this.ActualHeight, 0, 0);
            if (App.openWindows.Count() == 0 || App.installedBibles.installedBibles.Count() == 0)
            {
                if (App.installedBibles.installedBibles.Count() == 0)
                {
                    if (App.isFirstTimeInMainPageSplit==0)
                    {
                        //cant have any open windows if there are no books!
                        App.openWindows.Clear();
                        //get some books.
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
            //figure out if this is a light color
            var color = (Color)Application.Current.Resources["PhoneBackgroundColor"];
            int lightColorCount = (color.R > 0x80 ? 1 : 0) + (color.G > 0x80 ? 1 : 0) + (color.B > 0x80 ? 1 : 0);
            string colorDir = lightColorCount >= 2 ? "light" : "dark";
            ShowMenu.Image = new System.Windows.Media.Imaging.BitmapImage(new Uri("/Images/" + colorDir + "/appbar.menu.rest.png", UriKind.Relative));
            ShowMenu.PressedImage = new System.Windows.Media.Imaging.BitmapImage(new Uri("/Images/" + colorDir + "/appbar.menu.rest.pressed.png", UriKind.Relative));
        }

        private void PhoneApplicationPage_Unloaded(object sender, RoutedEventArgs e)
        {
            canvas1.Visibility = System.Windows.Visibility.Collapsed;
            ShowMenu.Visibility = System.Windows.Visibility.Visible;
            foreach (var nextWindow in App.openWindows)
            {
                nextWindow.HitButtonBigger -= HitButtonBigger;
                nextWindow.HitButtonSmaller -= HitButtonSmaller;
                nextWindow.HitButtonClose -= HitButtonClose;
            }
        }

        private void ApplicationBar_StateChanged(object sender, Microsoft.Phone.Shell.ApplicationBarStateChangedEventArgs e)
        {
            ApplicationBar.Opacity = e.IsMenuVisible ? 1 : 0;
        }

        private TurnstileTransition TurnstileTransitionElement(string mode)
        {
            TurnstileTransitionMode slideTransitionMode = (TurnstileTransitionMode)Enum.Parse(typeof(TurnstileTransitionMode), mode, false);
            return new TurnstileTransition { Mode = slideTransitionMode };
        }

        WindowSettings settings = new WindowSettings();

        private void HitButtonBigger(object sender, EventArgs e)
        {
            ReDrawWindows();
        }
        private void HitButtonSmaller(object sender, EventArgs e)
        {
            ReDrawWindows();
        }
        private void HitButtonClose(object sender, EventArgs e)
        {
            App.openWindows.RemoveAt(((BrowserTitledWindow)sender).state.curIndex);
            ReDrawWindows();
        }
        public void ShowEmptyWindow()
        {
            LayoutRoot.Children.Clear();
            LayoutRoot.ColumnDefinitions.Clear();
            LayoutRoot.RowDefinitions.Clear();

        }
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

                //show just a quick menu to add window or bibles
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
        System.Windows.Threading.DispatcherTimer menuDownAnimation = null;
        System.Windows.Threading.DispatcherTimer menuUpAnimation = null;
        private void ShowMenu_Click(object sender, RoutedEventArgs e)
        {
            AppMenu.Items.Clear();
            AppMenu.Items.Add(createTextBlock(Translations.translate("Add new window"), "Add new window"));
            AppMenu.Items.Add(createTextBlock(Translations.translate("Download bibles"), "Download bibles"));
            AppMenu.Items.Add(createTextBlock(Translations.translate("Select bible to delete"), "Select bible to delete"));
            AppMenu.Items.Add(createTextBlock(Translations.translate("Select bookmark to delete"), "Select bookmark to delete"));
            AppMenu.Items.Add(createTextBlock(Translations.translate("Clear history"), "Clear history"));
            AppMenu.Items.Add(createTextBlock(Translations.translate("Help"), "Help"));
            AppMenu.Items.Add(createTextBlock(Translations.translate("Cancel"), "Cancel"));

            menuUpAnimation = new System.Windows.Threading.DispatcherTimer();
            menuUpAnimation.Interval = new TimeSpan(0, 0, 0, 0, 15);
            menuUpAnimation.Tick += new EventHandler(menuUpAnimation_Tick);
            menuUpAnimation.Start();
            AppMenu.Width = this.ActualWidth;
            canvas1.Width = this.ActualWidth;
            ShowMenu.Visibility = System.Windows.Visibility.Collapsed;
            canvas1.Visibility = System.Windows.Visibility.Visible;
        }
        private TextBlock createTextBlock(string text,string tag)
        {
            TextBlock textBlock = new TextBlock();
            textBlock.Text = text;
            textBlock.Tag = tag;
            return textBlock;
        }

        void menuUpAnimation_Tick(object sender, EventArgs e)
        {
                // Do Stuff here.
            Thickness but = canvas1.Margin;
            canvas1.Margin = new Thickness(0, but.Top - 10, 0, 0);
            //this.UpdateLayout();
            if ((this.ActualHeight-but.Top)>200)
            {
                menuUpAnimation.Stop();
                menuUpAnimation = null;
            }
            
        }

        private void AppMenu_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                switch (((TextBlock)e.AddedItems[0]).Tag.ToString())
                {
                    case "Add new window":
                        App.windowSettings.isAddNewWindowOnly = true;
                        App.windowSettings.skipWindowSettings = false;
                        this.NavigationService.Navigate(new Uri("/WindowSettings.xaml", UriKind.Relative));
                        break;
                    case "Download bibles":
                        this.NavigationService.Navigate(new Uri("/DownloadBooks.xaml", UriKind.Relative));
                        break;
                    case "Select bible to delete":
                        NavigationService.Navigate(new Uri("/RemoveBibles.xaml", UriKind.Relative));
                        break;
                    case "Select bookmark to delete":
                        NavigationService.Navigate(new Uri("/EditBookmarks.xaml", UriKind.Relative));
                        break;
                    case "Clear history":
                        App.placeMarkers.history = new List<SwordBackend.BiblePlaceMarker>();
                        App.RaiseHistoryChangeEvent();
                        break;
                    case "Help":
                        App.helpstart.title=Translations.translate("Help");
                        App.helpstart.embeddedFilePath="CrossConnect.Properties.regex.html";
                        NavigationService.Navigate(new Uri("/Help.xaml", UriKind.Relative));
                        break;
                    case "Cancel":
                        //just do nothing
                        break;
                }
                menuDownAnimation = new System.Windows.Threading.DispatcherTimer();
                menuDownAnimation.Interval = new TimeSpan(0, 0, 0, 0, 15);
                menuDownAnimation.Tick += new EventHandler(menuDownAnimation_Tick);
                menuDownAnimation.Start();
                AppMenu.Width = this.ActualWidth;
                canvas1.Width = this.ActualWidth;
            }
        }
        void menuDownAnimation_Tick(object sender, EventArgs e)
        {
            // Do Stuff here.
            Thickness but = canvas1.Margin;
            canvas1.Margin = new Thickness(0, but.Top + 10, 0, 0);
            //this.UpdateLayout();
            if (this.ActualHeight < but.Top)
            {
                menuDownAnimation.Stop();
                menuDownAnimation = null;
                ShowMenu.Visibility = System.Windows.Visibility.Visible;
                canvas1.Visibility = System.Windows.Visibility.Collapsed;
            }
        }

        private void PhoneApplicationPage_OrientationChanged(object sender, OrientationChangedEventArgs e)
        {
            canvas1.Margin = new Thickness(0, this.ActualHeight, 0, 0);
            canvas1.Visibility = System.Windows.Visibility.Collapsed;
            ShowMenu.Visibility = System.Windows.Visibility.Visible;
        }
    }
}