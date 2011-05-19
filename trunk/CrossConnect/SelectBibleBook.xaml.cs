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

    public partial class SelectBibleBook : AutoRotatePage
    {
        #region Fields

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
            usingNowSpec = buttonWindow;
            ScrollContentGrid.Children.Clear();
            ScrollContentGrid.ColumnDefinitions.Clear();
            ScrollContentGrid.RowDefinitions.Clear();
            double screenWidth=0;
            double screenHeight=0;
            PageOrientation orient=(App.Current.RootVisual as PhoneApplicationFrame).Orientation;
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

            int numCols = (int)screenWidth / buttonWindow.ButtonWidth;
            if (numCols == 0)
            {
                numCols = 1;
            }
            int numRows = buttonWindow.NumButtons / numCols;
            for (int i = 0; i <= numRows; i++)
            {
                RowDefinition row = new RowDefinition();
                ScrollContentGrid.RowDefinitions.Add(row);
                row.Height = GridLength.Auto;

                for (int j = 0; j < numCols && ((i*numCols)+j)<buttonWindow.NumButtons; j++)
                {
                    ColumnDefinition col = new ColumnDefinition();
                    col.Width = GridLength.Auto;
                    ScrollContentGrid.ColumnDefinitions.Add(col);
                    Button but = new Button();
                    but.Content = buttonWindow.text[(i*numCols)+j];
                    but.Margin = new Thickness(-9,-9,-9,-9);
                    but.MaxHeight = buttonWindow.ButtonHeight;
                    but.MinHeight = buttonWindow.ButtonHeight;
                    but.MaxWidth = buttonWindow.ButtonWidth;
                    but.MinWidth = buttonWindow.ButtonWidth;
                    but.FontSize = (int)((double)buttonWindow.ButtonHeight / 3.5);
                    but.Visibility = System.Windows.Visibility.Visible;
                    if (buttonWindow.colors != null)
                    {
                        but.Foreground = new SolidColorBrush(buttonWindow.colors[(i*numCols)+j]);
                    }

                    Grid.SetRow(but, i);
                    Grid.SetColumn(but, j);
                    ScrollContentGrid.Children.Add(but);
                    but.Click += new RoutedEventHandler(buttonWindow.but_Click);
                    but.Tag = ((i*numCols)+j);
                }
                LayoutRoot.UpdateLayout();
                scrollViewer1.UpdateLayout();
                ScrollContentGrid.UpdateLayout();
                LayoutRoot.UpdateLayout();
                scrollViewer1.UpdateLayout();
                ScrollContentGrid.UpdateLayout();
            }
        }

        private void First_Click(object sender, RoutedEventArgs e)
        {
            PageTitle.Text = Translations.translate("Select chapter");
            // set up the array for the chapter selection
            int bookNum = (int)((Button)sender).Tag;
            App.openWindows[App.windowSettings.openWindowIndex].state.bookNum = bookNum;
            int chapters = SwordBackend.BibleZtextReader.CHAPTERS_IN_BOOK[bookNum];
            int butWidth = 96;
            int butHeight = 70;
            if (chapters < 50)
            {
                double sideLength = System.Math.Sqrt(((double)(App.Current.Host.Content.ActualWidth * (double)App.Current.Host.Content.ActualHeight) /  chapters))/1.15;
                if (sideLength * 2 > (App.Current.Host.Content.ActualWidth > App.Current.Host.Content.ActualHeight ? App.Current.Host.Content.ActualHeight : App.Current.Host.Content.ActualWidth))
                {
                    sideLength = sideLength / 2;
                }
                butWidth = (int)sideLength;
                butHeight = (int)sideLength;
            }
            if (chapters == 1)
            {
                App.openWindows[App.windowSettings.openWindowIndex].state.chapterNum = 0;
                Button but=new Button();
                but.Tag=0;
                Second_Click(but,new RoutedEventArgs());
                return;
            }
            Color butColor=(Color)Application.Current.Resources["PhoneForegroundColor"];
            Color[] butColors = new Color[chapters];
            string[] butText = new string[chapters];
            for (int i = 0; i < chapters; i++)
            {
                butColors[i] = butColor;
                butText[i] = (i+1).ToString();
            }

            // do a nice transition

            reloadWindow(new ButtonWindowSpecs(
                butWidth,
                butHeight,
                chapters,
                butColors,
                butText,
                Second_Click));
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            PageTitle.Text = Translations.translate("Select book");
            reloadWindow(new ButtonWindowSpecs(
                App.openWindows[App.windowSettings.openWindowIndex].state.source.existsShortNames?96:230,
                70,
                66,
                new Color[] { Colors.Blue, Colors.Blue, Colors.Blue, Colors.Blue, Colors.Blue,
                Colors.Brown,Colors.Brown,Colors.Brown,Colors.Brown,Colors.Brown,Colors.Brown,Colors.Brown,Colors.Brown,Colors.Brown,Colors.Brown,Colors.Brown,Colors.Brown,
                Colors.Cyan,Colors.Cyan,Colors.Cyan,Colors.Cyan,Colors.Cyan,
                Colors.Red,Colors.Red,Colors.Red,Colors.Red,Colors.Red,
                Colors.Green,Colors.Green,Colors.Green,Colors.Green,Colors.Green,Colors.Green,Colors.Green,Colors.Green,Colors.Green,Colors.Green,Colors.Green,Colors.Green,
                Colors.Magenta,Colors.Magenta,Colors.Magenta,Colors.Magenta,
                Colors.Orange,
                Colors.Purple,Colors.Purple,Colors.Purple,Colors.Purple,Colors.Purple,Colors.Purple,Colors.Purple,Colors.Purple,Colors.Purple,Colors.Purple,Colors.Purple,Colors.Purple,Colors.Purple,
                Colors.Red,Colors.Red,Colors.Red,Colors.Red,Colors.Red,Colors.Red,Colors.Red,Colors.Red,
                Colors.Yellow},
                App.openWindows[App.windowSettings.openWindowIndex].state.source.getAllShortNames(),
                First_Click));
        }

        private void PhoneApplicationPage_OrientationChanged(object sender, OrientationChangedEventArgs e)
        {
            reloadWindow(usingNowSpec);
        }

        private void Second_Click(object sender, RoutedEventArgs e)
        {
            int chapter=0;
            for (int i = 0; i < App.openWindows[App.windowSettings.openWindowIndex].state.bookNum; i++)
            {
                chapter+=SwordBackend.BibleZtextReader.CHAPTERS_IN_BOOK[i];
            }
            App.openWindows[App.windowSettings.openWindowIndex].state.chapterNum = (int)((Button)sender).Tag + chapter;
            App.openWindows[App.windowSettings.openWindowIndex].state.verseNum = 0;
            App.windowSettings.skipWindowSettings = true;
            if (NavigationService.CanGoBack)
            {
                NavigationService.GoBack();
            }
        }

        #endregion Methods

        #region Nested Types

        public class ButtonWindowSpecs
        {
            #region Fields

            public int ButtonHeight;
            public int ButtonWidth;
            public RoutedEventHandler but_Click;
            public Color[] colors;
            public int NumButtons;
            public string[] text;

            #endregion Fields

            #region Constructors

            public ButtonWindowSpecs(
                int ButtonWidth,
                int ButtonHeight,
                int NumButtons,
                Color[] colors,
                string[] text,
                RoutedEventHandler but_Click)
            {
                this.ButtonWidth = ButtonWidth;
                this.ButtonHeight = ButtonHeight;
                this.NumButtons = NumButtons;
                this.colors = colors;
                this.text = text;
                this.but_Click = but_Click;
            }

            #endregion Constructors
        }

        #endregion Nested Types
    }
}