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
        public SelectBibleBook()
        {
            InitializeComponent();
        }
        public void reloadWindow()
        {
            ScrollContentGrid.Children.Clear();
            ScrollContentGrid.ColumnDefinitions.Clear();
            ScrollContentGrid.RowDefinitions.Clear();
            int rowCount = 0;
            int numCols = (int)this.Width / App.buttonWindow.ButtonWidth;
            int numRows = App.buttonWindow.NumButtons / numCols;
            for (int i = 0; i < rowCount; i++)
            {
                RowDefinition row = new RowDefinition();
                ScrollContentGrid.RowDefinitions.Add(row);

                for (int j = 0; j < numCols; j++)
                {
                    ColumnDefinition col = new ColumnDefinition();
                    ScrollContentGrid.ColumnDefinitions.Add(col);
                    Button but = new Button();
                    but.Content = App.buttonWindow.text[i*j];
                    but.Margin = new Thickness(-7,-7,-7,-7);
                    but.MaxHeight = App.buttonWindow.ButtonHeight;
                    but.MaxWidth = App.buttonWindow.ButtonWidth;
                    but.FontSize = (int)((double)App.buttonWindow.ButtonHeight/3.5);
                    if (App.buttonWindow.colors != null)
                    {
                        but.Foreground = new SolidColorBrush(App.buttonWindow.colors[i * j]);
                    }

                    Grid.SetRow(but, i);
                    Grid.SetColumn(but, j);
                    ScrollContentGrid.Children.Add(but);
                    but.Click+=new RoutedEventHandler(but_Click);
                    but.Tag = (i * j);
                }
            }
        }
        private void but_Click(object sender, RoutedEventArgs e)
        {
            App.buttonWindow.buttonSelected = (int)((Button)sender).Tag;
        }
        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {

        }
    }
}