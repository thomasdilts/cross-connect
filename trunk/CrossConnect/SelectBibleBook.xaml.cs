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

namespace CrossConnect
{
    public partial class SelectBibleBook : PhoneApplicationPage
    {
        public class ButtonWindowSpecs
        {
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
            public int ButtonWidth;
            public int ButtonHeight;
            public int NumButtons;
            public Color[] colors;
            public string[] text;
            public RoutedEventHandler but_Click;
        }

        public SelectBibleBook()
        {
            InitializeComponent();
        }
        public void reloadWindow(ButtonWindowSpecs buttonWindow)
        {
            ScrollContentGrid.Children.Clear();
            ScrollContentGrid.ColumnDefinitions.Clear();
            ScrollContentGrid.RowDefinitions.Clear();

            int numCols = (int)App.Current.Host.Content.ActualWidth / buttonWindow.ButtonWidth;
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
        private void Second_Click(object sender, RoutedEventArgs e)
        {
            int chapter=0;
            for (int i = 0; i < App.openWindows[App.windowSettings.openWindowIndex].state.bookNum; i++)
            {
                chapter+=book.install.BibleZtextReader.CHAPTERS_IN_BOOK[i];
            }
            App.openWindows[App.windowSettings.openWindowIndex].state.chapterNum = (int)((Button)sender).Tag + chapter;
            NavigationService.GoBack();
        }
        private void First_Click(object sender, RoutedEventArgs e)
        {
            //set up the array for the chapter selection
            int bookNum = (int)((Button)sender).Tag;
            App.openWindows[App.windowSettings.openWindowIndex].state.bookNum = bookNum;
            int chapters = book.install.BibleZtextReader.CHAPTERS_IN_BOOK[bookNum];
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

            //do a nice transition





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
            reloadWindow(new ButtonWindowSpecs(
                96,
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
    }
}