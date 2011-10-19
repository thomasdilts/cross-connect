#region Header

// <copyright file="WindowSettings.xaml.cs" company="Thomas Dilts">
//
// CrossConnect Bible and Bible Commentary Reader for CrossWire.org
// Copyright (C) 2011 Thomas Dilts
//
// This program is free software: you can redistribute it and/or modify
// it under the +terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see http://www.gnu.org/licenses/.
// </copyright>
// <summary>
// Email: thomas@chaniel.se
// </summary>
// <author>Thomas Dilts</author>

#endregion Header

namespace CrossConnect
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Windows;
    using System.Windows.Controls;

    using Microsoft.Phone.Shell;

    using Sword;
    using Sword.reader;

    public partial class WindowSettings
    {
        #region Constructors

        public WindowSettings()
        {
            InitializeComponent();
        }

        #endregion Constructors

        #region Methods

        private void ButSearchClick(object sender, RoutedEventArgs e)
        {
            SetBookChoosen();
            NavigationService.Navigate(new Uri("/Search.xaml", UriKind.Relative));
        }

        private void ButSelectChapterClick(object sender, RoutedEventArgs e)
        {
            SetBookChoosen();
            NavigationService.Navigate(new Uri("/SelectBibleBook.xaml", UriKind.Relative));
        }

        private void GetSelectedData(out WindowType selectedType, out SwordBook bookSelected)
        {
            bookSelected = null;
            selectedType = WindowType.WindowBible;
            switch (selectDocumentType.SelectedIndex)
            {
                case 0:
                    selectedType = WindowType.WindowBible;
                    break;
                case 1:
                    selectedType = WindowType.WindowBibleNotes;
                    break;
                case 2:
                    selectedType = WindowType.WindowHistory;
                    break;
                case 3:
                    selectedType = WindowType.WindowBookmarks;
                    break;
                case 4:
                    selectedType = WindowType.WindowDailyPlan;
                    break;
                case 5:
                    selectedType = WindowType.WindowAddedNotes;
                    break;
                case 6:
                    selectedType = WindowType.WindowCommentary;
                    break;
            }
            if (selectDocument.SelectedItem != null)
            {
                if (selectedType == WindowType.WindowCommentary)
                {
                    // did the book choice change?
                    foreach (var book in App.InstalledBibles.InstalledCommentaries)
                    {
                        if (selectDocument.SelectedItem.Equals(book.Value.Sbmd.Name))
                        {
                            bookSelected = book.Value;
                            break;
                        }
                    }
                }
                else
                {
                    foreach (var book in App.InstalledBibles.InstalledBibles)
                    {
                        if (selectDocument.SelectedItem.Equals(book.Value.Sbmd.Name))
                        {
                            bookSelected = book.Value;
                            break;
                        }
                    }
                }
            }
        }

        private void PhoneApplicationPageBackKeyPress(object sender, CancelEventArgs e)
        {
            try
            {
                if (selectDocumentType.SelectedIndex == 4)
                {
                    if (planStartDate.Value != null)
                    {
                        App.DailyPlan.PlanStartDate = (DateTime) planStartDate.Value;
                    }
                    App.DailyPlan.PlanNumber = selectPlanType.SelectedIndex;
                }

                object isAddNewWindowOnly;
                if (!PhoneApplicationService.Current.State.TryGetValue("isAddNewWindowOnly", out isAddNewWindowOnly))
                {
                    isAddNewWindowOnly = false;
                }
                if ((bool) isAddNewWindowOnly)
                {
                    WindowType selectedType;
                    SwordBook bookSelected;
                    GetSelectedData(out selectedType, out bookSelected);

                    App.AddWindow(bookSelected.Sbmd.InternalName, bookSelected.Sbmd.Name, selectedType,
                                  sliderTextSize.Value);
                    //if (NavigationService.CanGoBack)
                    //{
                    //    NavigationService.GoBack();
                    //}
                }
                else
                {
                    object openWindowIndex;
                    if (!PhoneApplicationService.Current.State.TryGetValue("openWindowIndex", out openWindowIndex))
                    {
                        openWindowIndex = 0;
                    }

                    if (App.OpenWindows[(int) openWindowIndex].State.WindowType == WindowType.WindowSearch
                        || App.OpenWindows[(int) openWindowIndex].State.WindowType == WindowType.WindowLexiconLink
                        || App.OpenWindows[(int) openWindowIndex].State.WindowType == WindowType.WindowTranslator)
                    {
                        App.OpenWindows[(int) openWindowIndex].State.HtmlFontSize = sliderTextSize.Value;
                    }
                    else
                    {
                        SetBookChoosen();
                    }
                    //if (NavigationService.CanGoBack)
                    //{
                    //    NavigationService.GoBack();
                    //}
                }
            }
            catch (Exception ee)
            {
                Debug.WriteLine("crashed in a strange place. err=" + ee.StackTrace);
            }
        }

        private void PhoneApplicationPageLoaded(object sender, RoutedEventArgs e)
        {
            object skipWindowSettings;
            if (PhoneApplicationService.Current.State.TryGetValue("skipWindowSettings", out skipWindowSettings))
            {
                if ((bool) skipWindowSettings)
                {
                    PhoneApplicationService.Current.State["skipWindowSettings"] = false;
                    // request to skip this window.
                    if (NavigationService.CanGoBack)
                    {
                        Debug.WriteLine("WindowSettings AutoBackout");
                        NavigationService.GoBack();
                    }
                }
            }

            object initializeWindow;
            if (!PhoneApplicationService.Current.State.TryGetValue("InitializeWindowSettings", out initializeWindow))
            {
                initializeWindow = false;
            }

            if ((bool) initializeWindow)
            {
                SetupEntirePage();
                PhoneApplicationService.Current.State["InitializeWindowSettings"] = false;
            }
        }

        private void SelectDocumentTypeSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            object isAddNewWindowOnly;
            object openWindowIndex;
            if (!PhoneApplicationService.Current.State.TryGetValue("isAddNewWindowOnly", out isAddNewWindowOnly))
            {
                isAddNewWindowOnly = false;
            }
            if (!PhoneApplicationService.Current.State.TryGetValue("openWindowIndex", out openWindowIndex))
            {
                openWindowIndex = 0;
            }
            if (selectDocumentType.SelectedIndex == 4)
            {
                //prefill and show the next 2 fields.
                selectPlanType.Items.Clear();
                for (int i = 0; i <= DailyPlans.ZzAllPlansNames.GetUpperBound(0); i++)
                {
                    selectPlanType.Items.Add(Translations.Translate(DailyPlans.ZzAllPlansNames[i][0]) + "; " +
                                             DailyPlans.ZzAllPlansNames[i][1] + " " + Translations.Translate("Days") +
                                             "; " + DailyPlans.ZzAllPlansNames[i][2] + " " +
                                             Translations.Translate("Minutes/Day"));
                }
                selectPlanType.SelectedIndex = App.DailyPlan.PlanNumber;
                planStartDate.Value = App.DailyPlan.PlanStartDate;
            }
            else if (selectDocumentType.SelectedIndex == 6)
            {
                selectDocument.Items.Clear();
                foreach (var book in App.InstalledBibles.InstalledCommentaries)
                {
                    selectDocument.Items.Add(book.Value.Name);
                    if ((bool) isAddNewWindowOnly == false && App.OpenWindows.Count > 0 &&
                        App.OpenWindows[(int) openWindowIndex].State.BibleToLoad.Equals(book.Value.Sbmd.InternalName))
                    {
                        selectDocument.SelectedIndex = selectDocument.Items.Count - 1;
                    }
                }
            }
            else
            {
                selectDocument.Items.Clear();
                foreach (var book in App.InstalledBibles.InstalledBibles)
                {
                    selectDocument.Items.Add(book.Value.Name);
                    if ((bool) isAddNewWindowOnly == false && App.OpenWindows.Count > 0 &&
                        App.OpenWindows[(int) openWindowIndex].State.BibleToLoad.Equals(book.Value.Sbmd.InternalName))
                    {
                        selectDocument.SelectedIndex = selectDocument.Items.Count - 1;
                    }
                }
            }
            bool isPageable = false;
            bool isSearchable = false;
            var windowType = WindowType.WindowBible;
            if (App.OpenWindows.Count > 0 && App.OpenWindows[(int) openWindowIndex].State != null)
            {
                var state = App.OpenWindows[(int) openWindowIndex].State;
                isPageable = state.Source.IsPageable;
                isSearchable = state.Source.IsSearchable;
                windowType = state.WindowType;
            }
            var visibility = (selectDocumentType.SelectedIndex + 1) == (int) windowType && !(bool) isAddNewWindowOnly
                                 ? Visibility.Visible
                                 : Visibility.Collapsed;
            butSelectChapter.Visibility = (isPageable ? visibility : Visibility.Collapsed);
            butSearch.Visibility = (isSearchable ? visibility : Visibility.Collapsed);

            visibility = selectDocumentType.SelectedIndex == 4 ? Visibility.Visible : Visibility.Collapsed;
            DateSelectPanel.Visibility = visibility;
            selectPlanType.Visibility = visibility;
        }

        private void SetBookChoosen()
        {
            WindowType selectedType;
            SwordBook bookSelected;
            GetSelectedData(out selectedType, out bookSelected);
            object openWindowIndex;
            if (!PhoneApplicationService.Current.State.TryGetValue("openWindowIndex", out openWindowIndex))
            {
                openWindowIndex = 0;
            }
            var state = App.OpenWindows[(int) openWindowIndex].State;
            if (!state.BibleToLoad.Equals(bookSelected.Sbmd.InternalName) || state.WindowType != selectedType)
            {
                state.WindowType = selectedType;
                state.BibleToLoad = bookSelected.Sbmd.InternalName;
                state.BibleDescription = bookSelected.Sbmd.Name;
                state.HtmlFontSize = sliderTextSize.Value;
                App.OpenWindows[(int) openWindowIndex].Initialize(state.BibleToLoad, state.BibleDescription,
                                                                  state.WindowType);
            }
            else
            {
                state.HtmlFontSize = sliderTextSize.Value;
            }
        }

        private void SetupEntirePage()
        {
            PageTitle.Text = Translations.Translate("Settings");
            selectDocumentType.Header = Translations.Translate("Select the window type");
            selectDocument.Header = Translations.Translate("Select the bible");
            butSelectChapter.Content = Translations.Translate("Select book and chapter");
            butSearch.Content = Translations.Translate("Search");
            planStartDateCaption.Text = Translations.Translate("Select the daily plan start date");
            selectPlanType.Header = Translations.Translate("Select the daily plan");

            selectDocumentType.Items.Clear();
            selectDocumentType.Items.Add(Translations.Translate("Bible"));
            selectDocumentType.Items.Add(Translations.Translate("Notes"));
            selectDocumentType.Items.Add(Translations.Translate("History"));
            selectDocumentType.Items.Add(Translations.Translate("Bookmarks"));
            selectDocumentType.Items.Add(Translations.Translate("Daily plan"));
            selectDocumentType.Items.Add(Translations.Translate("Added notes"));
            if (App.InstalledBibles.InstalledCommentaries.Count > 0)
            {
                selectDocumentType.Items.Add(Translations.Translate("Commentaries"));
            }

            object isAddNewWindowOnly;
            object openWindowIndex;
            if (!PhoneApplicationService.Current.State.TryGetValue("isAddNewWindowOnly", out isAddNewWindowOnly))
            {
                isAddNewWindowOnly = false;
            }
            if (!PhoneApplicationService.Current.State.TryGetValue("openWindowIndex", out openWindowIndex))
            {
                openWindowIndex = 0;
            }

            selectDocument.Items.Clear();

            foreach (var book in App.InstalledBibles.InstalledBibles)
            {
                selectDocument.Items.Add(book.Value.Name);
                if ((bool) isAddNewWindowOnly == false && App.OpenWindows.Count > 0 &&
                    App.OpenWindows[(int) openWindowIndex].State.BibleToLoad.Equals(book.Value.Sbmd.InternalName))
                {
                    selectDocument.SelectedIndex = selectDocument.Items.Count - 1;
                }
            }
            var visibility = Visibility.Visible;
            if (App.OpenWindows.Count == 0 || (bool) isAddNewWindowOnly)
            {
                visibility = Visibility.Collapsed;
            }
            butSelectChapter.Visibility = visibility;
            butSearch.Visibility = visibility;

            DateSelectPanel.Visibility = Visibility.Collapsed;
            selectPlanType.Visibility = Visibility.Collapsed;

            sliderTextSize.Value = (double) Application.Current.Resources["PhoneFontSizeNormal"]*5/8;
            // must show the current window selections
            if (App.OpenWindows.Count > 0 && !(bool) isAddNewWindowOnly)
            {
                var state = App.OpenWindows[(int) openWindowIndex].State;
                int bookNum;
                int relChaptNum;
                int absoluteChaptNum;
                int verseNum;
                string fullName;
                string titleText;
                state.Source.GetInfo(out bookNum, out absoluteChaptNum, out relChaptNum, out verseNum, out fullName,
                                     out titleText);

                butSelectChapter.Visibility = (state.Source.IsPageable ? visibility : Visibility.Collapsed);
                butSearch.Visibility = (state.Source.IsSearchable ? visibility : Visibility.Collapsed);
                switch (state.WindowType)
                {
                    case WindowType.WindowBible:
                        selectDocumentType.SelectedIndex = 0;
                        break;
                    case WindowType.WindowBibleNotes:
                        selectDocumentType.SelectedIndex = 1;
                        break;
                    case WindowType.WindowHistory:
                        selectDocumentType.SelectedIndex = 2;
                        break;
                    case WindowType.WindowBookmarks:
                        selectDocumentType.SelectedIndex = 3;
                        break;
                    case WindowType.WindowDailyPlan:
                        selectDocumentType.SelectedIndex = 4;
                        selectPlanType.SelectedIndex = App.DailyPlan.PlanNumber;
                        planStartDate.Value = App.DailyPlan.PlanStartDate;
                        break;
                    case WindowType.WindowAddedNotes:
                        selectDocumentType.SelectedIndex = 5;
                        break;
                    case WindowType.WindowCommentary:
                        selectDocument.Items.Clear();
                        foreach (var book in App.InstalledBibles.InstalledCommentaries)
                        {
                            selectDocument.Items.Add(book.Value.Name);
                            if ((bool) isAddNewWindowOnly == false && App.OpenWindows.Count > 0 &&
                                App.OpenWindows[(int) openWindowIndex].State.BibleToLoad.Equals(
                                    book.Value.Sbmd.InternalName))
                            {
                                selectDocument.SelectedIndex = selectDocument.Items.Count - 1;
                            }
                        }
                        selectDocumentType.SelectedIndex = 6;
                        break;
                    case WindowType.WindowInternetLink:
                        selectDocumentType.Visibility = Visibility.Collapsed;
                        selectDocument.Visibility = Visibility.Collapsed;
                        break;
                    case WindowType.WindowLexiconLink:
                        selectDocumentType.Visibility = Visibility.Collapsed;
                        selectDocument.Visibility = Visibility.Collapsed;
                        break;
                    case WindowType.WindowSearch:
                        selectDocumentType.Visibility = Visibility.Collapsed;
                        selectDocument.Visibility = Visibility.Collapsed;
                        break;
                }
                sliderTextSize.Value = state.HtmlFontSize;
            }
        }

        private void SliderTextSizeValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (webBrowser1 != null)
            {
                webBrowser1.FontSize = e.NewValue;
                try
                {
                    webBrowser1.NavigateToString(
                        BibleZtextReader.HtmlHeader(
                            App.DisplaySettings,
                            BrowserTitledWindow.GetBrowserColor("PhoneBackgroundColor"),
                            BrowserTitledWindow.GetBrowserColor("PhoneForegroundColor"),
                            BrowserTitledWindow.GetBrowserColor("PhoneAccentColor"),
                            e.NewValue) +
                        "<a class=\"normalcolor\" href=\"#\">" + Translations.Translate("Text size") + "</a>" +
                        "</body></html>");
                }
                catch (Exception ee)
                {
                    Debug.WriteLine("sliderTextSize_ValueChanged webBrowser1.NavigateToString; " + ee.Message);
                }
            }
        }

        #endregion Methods
    }
}