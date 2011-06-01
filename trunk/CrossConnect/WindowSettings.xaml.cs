///
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
    using Microsoft.Phone.Shell;
    using Microsoft.Phone.Tasks;

    using SwordBackend;

    public partial class WindowSettings : AutoRotatePage
    {
        #region Constructors

        public WindowSettings()
        {
            InitializeComponent();
        }

        #endregion Constructors

        #region Methods

        private void butCancel_Click(object sender, RoutedEventArgs e)
        {
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
                case 4:
                    selectedType = WINDOW_TYPE.WINDOW_DAILY_PLAN;
                    break;
            }
        }

        private void PhoneApplicationPage_BackKeyPress(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (selectDocumentType.SelectedIndex == 4)
            {
                if (this.planStartDate.Value != null)
                {
                    App.dailyPlan.planStartDate = (DateTime)this.planStartDate.Value;
                }
                App.dailyPlan.planNumber = this.selectPlanType.SelectedIndex;
            } 
            
            object isAddNewWindowOnly = null;
            if (!PhoneApplicationService.Current.State.TryGetValue("isAddNewWindowOnly", out isAddNewWindowOnly))
            {
                isAddNewWindowOnly = false;
            }
            if ((bool)isAddNewWindowOnly)
            {
                WINDOW_TYPE selectedType;
                SwordBackend.SwordBook bookSelected;
                GetSelectedData(out selectedType, out bookSelected);

                App.AddWindow(bookSelected.sbmd.internalName,selectedType, (double)sliderTextSize.Value);
                if (NavigationService.CanGoBack)
                {
                    NavigationService.GoBack();
                }
            }
            else
            {
                object openWindowIndex = null;
                if (!PhoneApplicationService.Current.State.TryGetValue("openWindowIndex", out openWindowIndex))
                {
                    openWindowIndex = (int)0;
                }

                if (App.openWindows[(int)openWindowIndex].state.windowType == WINDOW_TYPE.WINDOW_SEARCH)
                {
                    App.openWindows[(int)openWindowIndex].state.htmlFontSize = this.sliderTextSize.Value;
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
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            object skipWindowSettings = null;
            if (PhoneApplicationService.Current.State.TryGetValue("skipWindowSettings", out skipWindowSettings))
            {
                if ((bool)skipWindowSettings)
                {
                    PhoneApplicationService.Current.State["skipWindowSettings"] = false;
                    // request to skip this window.
                    if (NavigationService.CanGoBack)
                    {
                        NavigationService.GoBack();
                    }
                }
            }

            object InitializeWindow = null;
            if (!PhoneApplicationService.Current.State.TryGetValue("InitializeWindowSettings", out InitializeWindow))
            {
                InitializeWindow = false;
            }

            if ((bool)InitializeWindow)
            {
                SetupEntirePage();
                PhoneApplicationService.Current.State["InitializeWindowSettings"] = false;
            }
        }

        private void selectDocumentType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (selectDocumentType.SelectedIndex == 4)
            {
                //prefill and show the next 2 fields.
                selectPlanType.Items.Clear();
                for (int i = 0; i < DailyPlans.zzAllPlansNames.Length; i++)
                {
                    selectPlanType.Items.Add(Translations.translate(DailyPlans.zzAllPlansNames[i]));
                }
                selectPlanType.SelectedIndex = App.dailyPlan.planNumber;
                planStartDate.Value = App.dailyPlan.planStartDate;
            }
            object isAddNewWindowOnly = null;
            if (!PhoneApplicationService.Current.State.TryGetValue("isAddNewWindowOnly", out isAddNewWindowOnly))
            {
                isAddNewWindowOnly = false;
            }
            object openWindowIndex = null;
            if (!PhoneApplicationService.Current.State.TryGetValue("openWindowIndex", out openWindowIndex))
            {
                openWindowIndex = 0;
            }
            bool IsPageable = false;
            bool IsSearchable = false;
            WINDOW_TYPE windowType = WINDOW_TYPE.WINDOW_BIBLE;
            if (App.openWindows.Count>0 && App.openWindows[(int)openWindowIndex].state != null)
            {
                var state = App.openWindows[(int)openWindowIndex].state;
                IsPageable = state.source.IsPageable;
                IsSearchable = state.source.IsSearchable;
                windowType = state.windowType;
            }
            System.Windows.Visibility visibility = (selectDocumentType.SelectedIndex + 1) == (int)windowType && !(bool)isAddNewWindowOnly ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
            butSelectChapter.Visibility = (IsPageable ? visibility : System.Windows.Visibility.Collapsed);
            butSearch.Visibility = (IsSearchable ? visibility : System.Windows.Visibility.Collapsed);

            visibility = selectDocumentType.SelectedIndex == 4 ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
            DateSelectPanel.Visibility = visibility;
            selectPlanType.Visibility = visibility;
        }

        private void SetBookChoosen()
        {
            WINDOW_TYPE selectedType;
            SwordBackend.SwordBook bookSelected;
            GetSelectedData(out selectedType, out bookSelected);
            object openWindowIndex = null;
            if (!PhoneApplicationService.Current.State.TryGetValue("openWindowIndex", out openWindowIndex))
            {
                openWindowIndex = (int)0;
            }
            var state = App.openWindows[(int)openWindowIndex].state;
            if (!state.bibleToLoad.Equals(bookSelected.sbmd.internalName) || state.windowType != selectedType)
            {
                state.windowType = selectedType;
                state.bibleToLoad = bookSelected.sbmd.internalName;
                state.htmlFontSize = this.sliderTextSize.Value;
                App.openWindows[(int)openWindowIndex].Initialize(state.bibleToLoad, state.windowType);
            }
            else
            {
                state.htmlFontSize = this.sliderTextSize.Value;
            }
        }

        private void SetupEntirePage()
        {
            PageTitle.Text = Translations.translate("Settings");
            selectDocumentType.Header = Translations.translate("Select the window type");
            selectDocument.Header = Translations.translate("Select the bible");
            butSelectChapter.Content = Translations.translate("Select book and chapter");
            butSearch.Content = Translations.translate("Search");
            planStartDateCaption.Text= Translations.translate("Select the daily plan start date");
            selectPlanType.Header= Translations.translate("Select the daily plan");

            selectDocumentType.Items.Clear();
            selectDocumentType.Items.Add(Translations.translate("Bible"));
            selectDocumentType.Items.Add(Translations.translate("Notes"));
            selectDocumentType.Items.Add(Translations.translate("History"));
            selectDocumentType.Items.Add(Translations.translate("Bookmarks"));
            selectDocumentType.Items.Add(Translations.translate("Daily plan"));

            object isAddNewWindowOnly = null;
            object openWindowIndex = null;
            if (!PhoneApplicationService.Current.State.TryGetValue("isAddNewWindowOnly", out isAddNewWindowOnly))
            {
                isAddNewWindowOnly = false;
            }
            if (!PhoneApplicationService.Current.State.TryGetValue("openWindowIndex", out openWindowIndex))
            {
                openWindowIndex = 0;
            }

            selectDocument.Items.Clear();

            foreach (var book in App.installedBibles.installedBibles)
            {
                selectDocument.Items.Add(book.Value.Name);
                if ((bool)isAddNewWindowOnly == false && App.openWindows.Count > 0 && App.openWindows[(int)openWindowIndex].state.bibleToLoad.Equals(book.Value.sbmd.internalName))
                {
                    selectDocument.SelectedIndex = selectDocument.Items.Count - 1;
                }
            }
            System.Windows.Visibility visibility = System.Windows.Visibility.Visible;
            if (App.openWindows.Count == 0 || (bool)isAddNewWindowOnly)
            {
                visibility = System.Windows.Visibility.Collapsed;
            }
            butSelectChapter.Visibility = visibility;
            butSearch.Visibility = visibility;

            DateSelectPanel.Visibility = System.Windows.Visibility.Collapsed;
            selectPlanType.Visibility = System.Windows.Visibility.Collapsed;

            sliderTextSize.Value = (double)Application.Current.Resources["PhoneFontSizeNormal"] * 5 / 8;
            // must show the current window selections
            if (App.openWindows.Count > 0 && !(bool)isAddNewWindowOnly)
            {
                var state= App.openWindows[(int)openWindowIndex].state;
                int bookNum;
                int relChaptNum;
                int absoluteChaptNum;
                int verseNum;
                string fullName;
                string titleText;
                state.source.GetInfo(out bookNum,out absoluteChaptNum, out relChaptNum,out verseNum, out fullName, out titleText);
                string title = titleText + " - " + state.bibleToLoad;

                butSelectChapter.Visibility = (state.source.IsPageable ? visibility : System.Windows.Visibility.Collapsed);
                butSearch.Visibility = (state.source.IsSearchable ? visibility : System.Windows.Visibility.Collapsed);
                switch (state.windowType)
                {
                    case WINDOW_TYPE.WINDOW_BIBLE:
                        this.selectDocumentType.SelectedIndex = 0;
                        break;
                    case WINDOW_TYPE.WINDOW_BIBLE_NOTES:
                        this.selectDocumentType.SelectedIndex = 1;
                        break;
                    case WINDOW_TYPE.WINDOW_HISTORY:
                        this.selectDocumentType.SelectedIndex = 2;
                        break;
                    case WINDOW_TYPE.WINDOW_BOOKMARKS:
                        this.selectDocumentType.SelectedIndex = 3;
                        break;
                    case WINDOW_TYPE.WINDOW_DAILY_PLAN:
                        this.selectDocumentType.SelectedIndex = 4;
                        selectPlanType.SelectedIndex = App.dailyPlan.planNumber;
                        planStartDate.Value = App.dailyPlan.planStartDate;
                        break;
                    case WINDOW_TYPE.WINDOW_SEARCH:
                        this.selectDocumentType.Visibility = System.Windows.Visibility.Collapsed;
                        this.selectDocument.Visibility = System.Windows.Visibility.Collapsed;
                        break;
                }
                sliderTextSize.Value = state.htmlFontSize;
            }
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
                    "<a href=\"#\">" + Translations.translate("Text size") + "</a>" +
                    "</body></html>");
            }
        }

        #endregion Methods
    }
}