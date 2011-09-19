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
/// <copyright file="SelectBibleBook.cs" company="Thomas Dilts">
///     Thomas Dilts. All rights reserved.
/// </copyright>
/// <author>Thomas Dilts</author>
namespace CrossConnect
{
    using System;
    using System.Diagnostics;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;

    using Microsoft.Phone.Controls;
    using Microsoft.Phone.Shell;

    using SwordBackend;

    public partial class SelectBibleBook : AutoRotatePage
    {
        #region Fields

        private static Color[] colorConversionScheme = 
        {
            (Color)Application.Current.Resources["PhoneForegroundColor"],
            Color.FromArgb(255,0xD2,0x69,0x1E), //chochlate
            Color.FromArgb(255,0x00,0xce,0xd1), //dark turqoise
            Color.FromArgb(255,0xff,0x14,0x39), //deep pink
            Color.FromArgb(255,0xa9,0xa9,0xa9), //darkgrey
            Color.FromArgb(255,0xb8,0x86,0x0b), //darkgoldenrod
            Color.FromArgb(255,0x8f,0xbc,0x8f), //dark seagreen
            Color.FromArgb(255,0x8a,0x2b,0xe2), //blueviolet
            Color.FromArgb(255,0xff,0x8c,0x00), //dark orange
            Color.FromArgb(255,0x00,0xbf,0xff), //deep skyblue
            Color.FromArgb(255,0xdc,0x14,0x3c), //crimson

        };

        private ButtonWindowSpecs usingNowSpec = null;

        #endregion Fields

        #region Constructors

        public SelectBibleBook()
        {
            InitializeComponent();
        }

        #endregion Constructors

        #region Methods

        public void reloadWindow(ButtonWindowSpecs buttonWindow)
        {
            try
            {
                if (buttonWindow != null)
                {
                    int butWidth = 96;
                    int butHeight = 70;
                    switch (buttonWindow.butSize)
                    {
                        case ButtonSize.LARGE:
                            butWidth = 200;
                            butHeight = 70;
                            break;
                        case ButtonSize.MEDIUM:
                            butWidth = 96;
                            butHeight = 70;
                            break;
                        case ButtonSize.SMALL:
                            if (buttonWindow.NumButtons < 50)
                            {
                                double sideLength = System.Math.Sqrt(((App.Current.Host.Content.ActualWidth * App.Current.Host.Content.ActualHeight) / buttonWindow.NumButtons)) / 1.15;
                                if (sideLength * 2 > (App.Current.Host.Content.ActualWidth > App.Current.Host.Content.ActualHeight ? App.Current.Host.Content.ActualHeight : App.Current.Host.Content.ActualWidth))
                                {
                                    sideLength = sideLength / 2;
                                }
                                butWidth = (int)sideLength;
                                butHeight = (int)sideLength;
                            }
                            break;
                    }

                    PageTitle.Text = Translations.translate(buttonWindow.title);
                    usingNowSpec = buttonWindow;
                    ScrollContentGrid.Children.Clear();
                    ScrollContentGrid.ColumnDefinitions.Clear();
                    ScrollContentGrid.RowDefinitions.Clear();
                    double screenWidth = 0;
                    double screenHeight = 0;
                    PageOrientation orient = (App.Current.RootVisual as PhoneApplicationFrame).Orientation;
                    if (orient == PageOrientation.Portrait || orient == PageOrientation.PortraitDown || orient == PageOrientation.PortraitUp || orient == PageOrientation.None)
                    {
                        screenWidth = App.Current.Host.Content.ActualWidth;
                        screenHeight = App.Current.Host.Content.ActualHeight;
                    }
                    else
                    {
                        screenWidth = App.Current.Host.Content.ActualHeight;
                        screenHeight = App.Current.Host.Content.ActualWidth;
                    }

                    int numCols = (int)screenWidth / butWidth;
                    if (numCols == 0)
                    {
                        numCols = 1;
                    }
                    int numRows = buttonWindow.NumButtons / numCols;
                    for (int i = 0; i <= numRows; i++)
                    {
                        RowDefinition row = new RowDefinition();
                        row.Height = GridLength.Auto;
                        ScrollContentGrid.RowDefinitions.Add(row);

                        for (int j = 0; j < numCols && ((i * numCols) + j) < buttonWindow.NumButtons; j++)
                        {
                            ColumnDefinition col = new ColumnDefinition();
                            col.Width = GridLength.Auto;
                            ScrollContentGrid.ColumnDefinitions.Add(col);
                            Button but = new Button();
                            but.Content = buttonWindow.text[(i * numCols) + j];
                            but.Margin = new Thickness(-9, -9, -9, -9);
                            but.MaxHeight = butHeight;
                            but.MinHeight = butHeight;
                            but.MaxWidth = butWidth;
                            but.MinWidth = butWidth;
                            but.FontSize = (int)((double)butHeight / 3.5);
                            but.Visibility = System.Windows.Visibility.Visible;
                            if (buttonWindow.colors != null)
                            {
                                but.Foreground = new SolidColorBrush(colorConversionScheme[buttonWindow.colors[(i * numCols) + j]]);
                            }

                            Grid.SetRow(but, i);
                            Grid.SetColumn(but, j);
                            ScrollContentGrid.Children.Add(but);
                            but.Click += new RoutedEventHandler(buttonWindow.stage == 0 ? (RoutedEventHandler)First_Click : (RoutedEventHandler)Second_Click);
                            but.Tag = buttonWindow.value[(i * numCols) + j];
                        }
                    }
                    LayoutRoot.UpdateLayout();
                    scrollViewer1.UpdateLayout();
                    ScrollContentGrid.UpdateLayout();
                }
            }
            catch(Exception eee)
            {
                Debug.WriteLine("strange crash =" + eee.Message + "; " + eee.StackTrace);
            }
        }

        private void First_Click(object sender, RoutedEventArgs e)
        {
            PhoneApplicationService.Current.State["SelectBibleBookFirsSelection"] = 0;
            object openWindowIndex = null;
            if (!PhoneApplicationService.Current.State.TryGetValue("openWindowIndex", out openWindowIndex))
            {
                openWindowIndex = 0;
            }
            ButtonWindowSpecs specs = App.openWindows[(int)openWindowIndex].state.source.GetButtonWindowSpecs(1, (int)((Button)sender).Tag);
            if (specs != null)
            {
                PhoneApplicationService.Current.State["SelectBibleBookFirsSelection"]=(int)((Button)sender).Tag;
                reloadWindow(specs);
            }
            else
            {
                App.openWindows[(int)openWindowIndex].state.source.moveChapterVerse((int)((Button)sender).Tag,0,false);
                PhoneApplicationService.Current.State["skipWindowSettings"] = true;
                if (NavigationService.CanGoBack)
                {
                    NavigationService.GoBack();
                }
            }
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            object openWindowIndex = null;
            if (!PhoneApplicationService.Current.State.TryGetValue("openWindowIndex", out openWindowIndex))
            {
                openWindowIndex = 0;
            }
            reloadWindow(App.openWindows[(int)openWindowIndex].state.source.GetButtonWindowSpecs(0,0));
        }

        private void PhoneApplicationPage_OrientationChanged(object sender, OrientationChangedEventArgs e)
        {
            reloadWindow(usingNowSpec);
        }

        private void Second_Click(object sender, RoutedEventArgs e)
        {
            object openWindowIndex = null;
            if (!PhoneApplicationService.Current.State.TryGetValue("openWindowIndex", out openWindowIndex))
            {
                openWindowIndex = 0;
            }
            object SelectBibleBookFirsSelection = null;
            int selectBibleBookFirsSelection = 0;
            if (PhoneApplicationService.Current.State.TryGetValue("SelectBibleBookFirsSelection", out SelectBibleBookFirsSelection))
            {
                selectBibleBookFirsSelection = (int)PhoneApplicationService.Current.State["SelectBibleBookFirsSelection"];
            }
            App.openWindows[(int)openWindowIndex].state.source.moveChapterVerse((int)((Button)sender).Tag + selectBibleBookFirsSelection, 0,false);
            PhoneApplicationService.Current.State["skipWindowSettings"] = true;
            if (NavigationService.CanGoBack)
            {
                NavigationService.GoBack();
            }
        }

        #endregion Methods
    }
}