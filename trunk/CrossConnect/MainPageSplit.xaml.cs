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

            foreach (var nextWindow in App.openWindows)
            {
                nextWindow.HitButtonBigger += HitButtonBigger;
                nextWindow.HitButtonSmaller += HitButtonSmaller;
                nextWindow.HitButtonClose += HitButtonClose;
            }

            canvas1.Margin = new Thickness(0, this.ActualHeight, 0, 0);
            if (App.openWindows.Count() == 0 || App.installedBooks.installedBooks.Count() == 0)
            {
                if (App.installedBooks.installedBooks.Count() == 0)
                {
                    //cant have any open windows if there are no books!
                    App.openWindows.Clear();
                    //get some books.
                    this.NavigationService.Navigate(new Uri("/DownloadBooks.xaml", UriKind.Relative));
                }
                else
                {
                    App.windowSettings.skipWindowSettings = false;
                    this.NavigationService.Navigate(new Uri("/WindowSettings.xaml", UriKind.Relative));
                }
            }
            else
            {
                ReDrawWindows();
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
        private void ReDrawWindows()
        {
            LayoutRoot.Children.Clear();
            LayoutRoot.ColumnDefinitions.Clear();
            LayoutRoot.RowDefinitions.Clear();
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
        System.Windows.Threading.DispatcherTimer menuDownAnimation = null;
        System.Windows.Threading.DispatcherTimer menuUpAnimation = null;
        private void ShowMenu_Click(object sender, RoutedEventArgs e)
        {
            AppMenu.Items.Clear();
            AppMenu.Items.Add("Add new window");
            AppMenu.Items.Add("Download bibles");
            AppMenu.Items.Add("Remove bibles");
            AppMenu.Items.Add("Help");
            AppMenu.Items.Add("Cancel");
            menuUpAnimation = new System.Windows.Threading.DispatcherTimer();
            menuUpAnimation.Interval = new TimeSpan(0, 0, 0, 0, 15);
            menuUpAnimation.Tick += new EventHandler(menuUpAnimation_Tick);
            menuUpAnimation.Start();
            AppMenu.Width = this.ActualWidth;
            canvas1.Width = this.ActualWidth;
            ShowMenu.Visibility = System.Windows.Visibility.Collapsed;
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
                switch ((string)e.AddedItems[0])
                {
                    case "Add new window":
                        App.windowSettings.isAddNewWindowOnly = true;
                        App.windowSettings.skipWindowSettings = false;
                        this.NavigationService.Navigate(new Uri("/WindowSettings.xaml", UriKind.Relative));
                        break;
                    case "Download bibles":
                        this.NavigationService.Navigate(new Uri("/DownloadBooks.xaml", UriKind.Relative));
                        break;
                    case "Remove bibles":
                        break;
                    case "Help":
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
            }
        }
    }
}