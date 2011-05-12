﻿///
/// <summary> Distribution License:
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
/// Copyright: 2011
///     The copyright to this program is held by Thomas Dilts
///
namespace CrossConnect
{
    using System;
    using System.Collections.Generic;
    using System.IO.IsolatedStorage;
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

    public partial class WindowSettings : PhoneApplicationPage
    {
        #region Constructors

        public WindowSettings()
        {
            InitializeComponent();
        }

        #endregion Constructors

        #region Methods

        private void butAddBookmarks_Click(object sender, RoutedEventArgs e)
        {
            App.AddBookmark(App.openWindows[App.windowSettings.openWindowIndex].state.chapterNum,
                App.openWindows[App.windowSettings.openWindowIndex].state.verseNum);
            if (NavigationService.CanGoBack)
            {
                NavigationService.GoBack();
            }
        }

        private void butAddNew_Click(object sender, RoutedEventArgs e)
        {
            WINDOW_TYPE selectedType;
            SwordBackend.SwordBook bookSelected;
            GetSelectedData(out selectedType, out bookSelected);
            App.AddWindow(bookSelected.sbmd.internalName, 0, 0, selectedType,(double)sliderTextSize.Value);
            if (NavigationService.CanGoBack)
            {
                NavigationService.GoBack();
            }
        }

        private void butCancel_Click(object sender, RoutedEventArgs e)
        {
            if (NavigationService.CanGoBack)
            {
                NavigationService.GoBack();
            }
        }

        private void butSave_Click(object sender, RoutedEventArgs e)
        {
            if (App.openWindows[App.windowSettings.openWindowIndex].state.windowType == WINDOW_TYPE.WINDOW_SEARCH)
            {
                App.openWindows[App.windowSettings.openWindowIndex].state.htmlFontSize = this.sliderTextSize.Value;
            }
            else
            {
                SetBookChoosen();
            }
            if (NavigationService.CanGoBack)
            {
                NavigationService.GoBack();
            }
        }

        private void butSearch_Click(object sender, RoutedEventArgs e)
        {
            SetBookChoosen();
            this.NavigationService.Navigate(new Uri("/Search.xaml", UriKind.Relative));
        }

        private void butSelectChapter_Click(object sender, RoutedEventArgs e)
        {
            SetBookChoosen();
            this.NavigationService.Navigate(new Uri("/SelectBibleBook.xaml", UriKind.Relative));
        }

        private void GetSelectedData(out WINDOW_TYPE selectedType, out SwordBackend.SwordBook bookSelected)
        {
            bookSelected = null;
            if (selectDocument.SelectedItem != null)
            {
                // did the book choice change?
                foreach (var book in App.installedBibles.installedBibles)
                {
                    if (selectDocument.SelectedItem.Equals(book.Value.sbmd.Name))
                    {
                        bookSelected = book.Value;
                        break;
                    }
                }
            }
            selectedType = WINDOW_TYPE.WINDOW_BIBLE;
            switch (this.selectDocumentType.SelectedIndex)
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

        private void PhoneApplicationPage_BackKeyPress(object sender, System.ComponentModel.CancelEventArgs e)
        {
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            PageTitle.Text = Translations.translate("Settings");
            selectDocumentType.Header = Translations.translate("Select the window type");
            selectDocument.Header = Translations.translate("Select the bible");
            butSelectChapter.Content = Translations.translate("Select book and chapter");
            butSearch.Content = Translations.translate("Search");
            butAddNew.Content = Translations.translate("Add new window");
            butAddBookmarks.Content = Translations.translate("Add to bookmarks");
            butSave.Content = Translations.translate("Save");
            butCancel.Content = Translations.translate("Cancel");

            selectDocumentType.Items.Clear();
            selectDocumentType.Items.Add(Translations.translate("Bible"));
            selectDocumentType.Items.Add(Translations.translate("Notes"));
            selectDocumentType.Items.Add(Translations.translate("History"));
            selectDocumentType.Items.Add(Translations.translate("Bookmarks"));

            if (App.windowSettings.skipWindowSettings)
            {
                // request to skip this window.
                if (NavigationService.CanGoBack)
                {
                    NavigationService.GoBack();
                }

            }

            selectDocument.Items.Clear();

            foreach (var book in App.installedBibles.installedBibles)
            {
                selectDocument.Items.Add(book.Value.Name);
                if (App.windowSettings.isAddNewWindowOnly == false && App.openWindows.Count > 0 && App.openWindows[App.windowSettings.openWindowIndex].state.bibleToLoad.Equals(book.Value.sbmd.internalName))
                {
                    selectDocument.SelectedIndex = selectDocument.Items.Count - 1;
                }
            }
            System.Windows.Visibility visibility = System.Windows.Visibility.Visible;
            if (App.openWindows.Count == 0 || App.windowSettings.isAddNewWindowOnly)
            {
                visibility = System.Windows.Visibility.Collapsed;
            }
            butSelectChapter.Visibility = visibility;
            butSearch.Visibility = visibility;
            butSave.Visibility = visibility;
            butCancel.Visibility = visibility;
            butAddBookmarks.Visibility = visibility;
            sliderTextSize.Value = (double)Application.Current.Resources["PhoneFontSizeNormal"] * 5 / 8;
            // must show the current window selections
            if (App.openWindows.Count > 0)
            {
                butSelectChapter.Visibility = (App.openWindows[App.windowSettings.openWindowIndex].state.source.IsPageable ? visibility : System.Windows.Visibility.Collapsed);
                butSearch.Visibility = (App.openWindows[App.windowSettings.openWindowIndex].state.source.IsSearchable ? visibility : System.Windows.Visibility.Collapsed);
                butAddBookmarks.Visibility = (App.openWindows[App.windowSettings.openWindowIndex].state.source.IsBookmarkable ? visibility : System.Windows.Visibility.Collapsed);
                switch (App.openWindows[App.windowSettings.openWindowIndex].state.windowType)
                {
                    case WINDOW_TYPE.WINDOW_BIBLE:
                        this.selectDocumentType.SelectedIndex = 0;
                        break;
                    case WINDOW_TYPE.WINDOW_BIBLE_NOTES:
                        this.selectDocumentType.SelectedIndex = 1;
                        break;
                    case WINDOW_TYPE.WINDOW_BOOKMARKS:
                        this.selectDocumentType.SelectedIndex = 2;
                        break;
                    case WINDOW_TYPE.WINDOW_HISTORY:
                        this.selectDocumentType.SelectedIndex = 3;
                        break;
                    case WINDOW_TYPE.WINDOW_SEARCH:
                        this.selectDocumentType.Visibility = System.Windows.Visibility.Collapsed;
                        this.selectDocument.Visibility = System.Windows.Visibility.Collapsed;
                        butAddNew.Visibility = System.Windows.Visibility.Collapsed;
                        break;
                }
                sliderTextSize.Value = App.openWindows[App.windowSettings.openWindowIndex].state.htmlFontSize;
            }
        }

        private void SetBookChoosen()
        {
            WINDOW_TYPE selectedType;
            SwordBackend.SwordBook bookSelected;
            GetSelectedData(out selectedType, out bookSelected);
            var state = App.openWindows[App.windowSettings.openWindowIndex].state;
            state.windowType = selectedType;
            state.bibleToLoad = bookSelected.sbmd.internalName;
            state.htmlFontSize = this.sliderTextSize.Value;
            App.openWindows[App.windowSettings.openWindowIndex].Initialize(state.bibleToLoad, state.bookNum, state.chapterNum, state.windowType);
        }

        private void sliderTextSize_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (webBrowser1 != null)
            {
                webBrowser1.FontSize = e.NewValue;
                webBrowser1.NavigateToString(
                    SwordBackend.BibleZtextReader.HtmlHeader(
                    BrowserTitledWindow.GetBrowserColor("PhoneBackgroundColor"),
                    BrowserTitledWindow.GetBrowserColor("PhoneForegroundColor"),
                    BrowserTitledWindow.GetBrowserColor("PhoneAccentColor"),
                    e.NewValue) +
                    Translations.translate("Text size") +
                    "</body></html>");
            }
        }

        #endregion Methods
    }
}