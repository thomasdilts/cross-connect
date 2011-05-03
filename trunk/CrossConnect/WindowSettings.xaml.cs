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
using System.IO.IsolatedStorage;

namespace CrossConnect
{
    public partial class WindowSettings : PhoneApplicationPage
    {
        public WindowSettings()
        {
            InitializeComponent();
        }
        
        private void PhoneApplicationPage_BackKeyPress(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //we will assume this is the same as a CANCEL unless there are no windows defined.
            if (App.openWindows.Count == 0)
            {
                //default is Add new window
                butAddNew_Click(null, new RoutedEventArgs());
            }
        }
        private void SetBookChoosen()
        {
            WINDOW_TYPE selectedType;
            book.install.SwordBook bookSelected;
            GetSelectedData(out selectedType, out bookSelected);
            var state=App.openWindows[App.windowSettings.openWindowIndex].state;
            state.windowType = selectedType;
            state.bookToLoad = bookSelected.sbmd.Initials;
            App.openWindows[App.windowSettings.openWindowIndex].Initialize(state.bookToLoad, state.bookNum, state.chapterNum, state.windowType);
        }
        private void GetSelectedData(out WINDOW_TYPE selectedType, out book.install.SwordBook bookSelected)
        {
            bookSelected = null;
            if (selectDocument.SelectedItem != null)
            {
                //did the book choice change?
                foreach (var book in App.installedBooks.installedBooks)
                {
                    if (selectDocument.SelectedItem.Equals(book.Value.sbmd.Name))
                    {
                        bookSelected = book.Value;
                        break;
                    }
                }
            }
            selectedType = WINDOW_TYPE.WINDOW_BIBLE;
            switch (selectDocument.SelectedIndex)
            {
                case 0:
                    selectedType = WINDOW_TYPE.WINDOW_BIBLE;
                    break;
                case 1:
                    selectedType = WINDOW_TYPE.WINDOW_BIBLE_NOTES;
                    break;
                case 2:
                    selectedType = WINDOW_TYPE.WINDOW_HISTORY;
                    break;
                case 3:
                    selectedType = WINDOW_TYPE.WINDOW_BOOKMARKS;
                    break;
            }
        }
        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            selectDocument.Items.Clear();

            foreach (var book in App.installedBooks.installedBooks)
            {
                selectDocument.Items.Add(book.Value.Name);
                if (App.windowSettings.isAddNewWindowOnly == false && App.openWindows.Count > 0 && App.openWindows[App.windowSettings.openWindowIndex].state.bookToLoad.Equals(book.Value.sbmd.Initials))
                {
                    selectDocument.SelectedIndex = selectDocument.Items.Count - 1;
                }
            }
            System.Windows.Visibility visibility= System.Windows.Visibility.Visible;
            if(App.openWindows.Count == 0 || App.windowSettings.isAddNewWindowOnly)
            {
                visibility=System.Windows.Visibility.Collapsed;
            }
            butSelectChapter.Visibility=visibility;
            butSearch.Visibility=visibility;
            butSave.Visibility=visibility;
            butCancel.Visibility=visibility;
        }

        private void butSelectChapter_Click(object sender, RoutedEventArgs e)
        {
            SetBookChoosen();
            this.NavigationService.Navigate(new Uri("/SelectBibleBook.xaml", UriKind.Relative));
        }

        private void butSearch_Click(object sender, RoutedEventArgs e)
        {
            SetBookChoosen();
            this.NavigationService.Navigate(new Uri("/SearchSelection.xaml", UriKind.Relative));
        }

        private void butAddNew_Click(object sender, RoutedEventArgs e)
        {
            WINDOW_TYPE selectedType;
            book.install.SwordBook bookSelected;
            GetSelectedData(out selectedType, out bookSelected);
            App.AddWindow(bookSelected.sbmd.Initials, 0, 0, selectedType);
            this.NavigationService.GoBack();
        }

        private void butCancel_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.GoBack();
        }

        private void butSave_Click(object sender, RoutedEventArgs e)
        {
            SetBookChoosen();
            this.NavigationService.GoBack();
        }
    }
}