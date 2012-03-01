// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WindowSettings.xaml.cs" company="">
//   
// </copyright>
// <summary>
//   The window settings.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#region Header

// <copyright file="WindowSettings.xaml.cs" company="Thomas Dilts">
// CrossConnect Bible and Bible Commentary Reader for CrossWire.org
// Copyright (C) 2011 Thomas Dilts
// This program is free software: you can redistribute it and/or modify
// it under the +terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see http://www.gnu.org/licenses/.
// </copyright>
// <summary>
// Email: thomas@cross-connect.se
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

    /// <summary>
    /// The window settings.
    /// </summary>
    public partial class WindowSettings
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="WindowSettings"/> class.
        /// </summary>
        public WindowSettings()
        {
            this.InitializeComponent();
        }

        #endregion

        #region Methods

        /// <summary>
        /// The but search click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void ButSearchClick(object sender, RoutedEventArgs e)
        {
            this.SetBookChoosen();
            this.NavigationService.Navigate(new Uri("/Search.xaml", UriKind.Relative));
        }

        /// <summary>
        /// The but select chapter click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void ButSelectChapterClick(object sender, RoutedEventArgs e)
        {
            this.SetBookChoosen();
            this.NavigationService.Navigate(new Uri("/SelectBibleBook.xaml", UriKind.Relative));
        }

        /// <summary>
        /// The get selected data.
        /// </summary>
        /// <param name="selectedType">
        /// The selected type.
        /// </param>
        /// <param name="bookSelected">
        /// The book selected.
        /// </param>
        private void GetSelectedData(out WindowType selectedType, out SwordBook bookSelected)
        {
            bookSelected = null;
            selectedType = WindowType.WindowBible;
            switch (this.selectDocumentType.SelectedIndex)
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

            if (this.selectDocument.SelectedItem != null)
            {
                if (selectedType == WindowType.WindowCommentary)
                {
                    // did the book choice change?
                    foreach (var book in App.InstalledBibles.InstalledCommentaries)
                    {
                        if (this.selectDocument.SelectedItem.Equals(book.Value.Sbmd.Name))
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
                        if (this.selectDocument.SelectedItem.Equals(book.Value.Sbmd.Name))
                        {
                            bookSelected = book.Value;
                            break;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// The phone application page back key press.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void PhoneApplicationPageBackKeyPress(object sender, CancelEventArgs e)
        {
            try
            {
                if (this.selectDocumentType.SelectedIndex == 4)
                {
                    if (this.planStartDate.Value != null)
                    {
                        App.DailyPlan.PlanStartDate = (DateTime)this.planStartDate.Value;
                    }

                    App.DailyPlan.PlanNumber = this.selectPlanType.SelectedIndex;
                }

                object isAddNewWindowOnly;
                if (!PhoneApplicationService.Current.State.TryGetValue("isAddNewWindowOnly", out isAddNewWindowOnly))
                {
                    isAddNewWindowOnly = false;
                }

                if ((bool)isAddNewWindowOnly)
                {
                    WindowType selectedType;
                    SwordBook bookSelected;
                    this.GetSelectedData(out selectedType, out bookSelected);

                    App.AddWindow(
                        bookSelected.Sbmd.InternalName, bookSelected.Sbmd.Name, selectedType, this.sliderTextSize.Value);

                    // if (NavigationService.CanGoBack)
                    // {
                    // NavigationService.GoBack();
                    // }
                }
                else
                {
                    object openWindowIndex;
                    if (!PhoneApplicationService.Current.State.TryGetValue("openWindowIndex", out openWindowIndex))
                    {
                        openWindowIndex = 0;
                    }

                    if (App.OpenWindows[(int)openWindowIndex].State.WindowType == WindowType.WindowSearch
                        || App.OpenWindows[(int)openWindowIndex].State.WindowType == WindowType.WindowLexiconLink
                        || App.OpenWindows[(int)openWindowIndex].State.WindowType == WindowType.WindowTranslator)
                    {
                        App.OpenWindows[(int)openWindowIndex].State.HtmlFontSize = this.sliderTextSize.Value;
                    }
                    else
                    {
                        this.SetBookChoosen();
                    }

                    // if (NavigationService.CanGoBack)
                    // {
                    // NavigationService.GoBack();
                    // }
                }
            }
            catch (Exception ee)
            {
                Debug.WriteLine("crashed in a strange place. err=" + ee.StackTrace);
            }
        }

        /// <summary>
        /// The phone application page loaded.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void PhoneApplicationPageLoaded(object sender, RoutedEventArgs e)
        {
            object skipWindowSettings;
            if (PhoneApplicationService.Current.State.TryGetValue("skipWindowSettings", out skipWindowSettings))
            {
                if ((bool)skipWindowSettings)
                {
                    PhoneApplicationService.Current.State["skipWindowSettings"] = false;

                    // request to skip this window.
                    if (this.NavigationService.CanGoBack)
                    {
                        Debug.WriteLine("WindowSettings AutoBackout");
                        this.NavigationService.GoBack();
                    }
                }
            }

            object initializeWindow;
            if (!PhoneApplicationService.Current.State.TryGetValue("InitializeWindowSettings", out initializeWindow))
            {
                initializeWindow = false;
            }

            if ((bool)initializeWindow)
            {
                this.SetupEntirePage();
                PhoneApplicationService.Current.State["InitializeWindowSettings"] = false;
            }
        }

        /// <summary>
        /// The select document type selection changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
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

            if (this.selectDocumentType.SelectedIndex == 4)
            {
                // prefill and show the next 2 fields.
                this.selectPlanType.Items.Clear();
                for (int i = 0; i <= DailyPlans.ZzAllPlansNames.GetUpperBound(0); i++)
                {
                    this.selectPlanType.Items.Add(
                        Translations.Translate(DailyPlans.ZzAllPlansNames[i][0]) + "; "
                        + DailyPlans.ZzAllPlansNames[i][1] + " " + Translations.Translate("Days") + "; "
                        + DailyPlans.ZzAllPlansNames[i][2] + " " + Translations.Translate("Minutes/Day"));
                }

                this.selectPlanType.SelectedIndex = App.DailyPlan.PlanNumber;
                this.planStartDate.Value = App.DailyPlan.PlanStartDate > DateTime.Now.AddYears(-100)
                                               ? App.DailyPlan.PlanStartDate
                                               : DateTime.Now;
            }
            else if (this.selectDocumentType.SelectedIndex == 6)
            {
                this.selectDocument.Items.Clear();
                foreach (var book in App.InstalledBibles.InstalledCommentaries)
                {
                    this.selectDocument.Items.Add(book.Value.Name);
                    if ((bool)isAddNewWindowOnly == false && App.OpenWindows.Count > 0
                        && App.OpenWindows[(int)openWindowIndex].State.BibleToLoad.Equals(book.Value.Sbmd.InternalName))
                    {
                        this.selectDocument.SelectedIndex = this.selectDocument.Items.Count - 1;
                    }
                }
            }
            else
            {
                this.selectDocument.Items.Clear();
                foreach (var book in App.InstalledBibles.InstalledBibles)
                {
                    this.selectDocument.Items.Add(book.Value.Name);
                    if ((bool)isAddNewWindowOnly == false && App.OpenWindows.Count > 0
                        && App.OpenWindows[(int)openWindowIndex].State.BibleToLoad.Equals(book.Value.Sbmd.InternalName))
                    {
                        this.selectDocument.SelectedIndex = this.selectDocument.Items.Count - 1;
                    }
                }
            }

            bool isPageable = false;
            bool isSearchable = false;
            WindowType windowType = WindowType.WindowBible;
            if (App.OpenWindows.Count > 0 && App.OpenWindows[(int)openWindowIndex].State != null)
            {
                SerializableWindowState state = App.OpenWindows[(int)openWindowIndex].State;
                isPageable = state.Source.IsPageable;
                isSearchable = state.Source.IsSearchable;
                windowType = state.WindowType;
            }

            Visibility visibility = (this.selectDocumentType.SelectedIndex + 1) == (int)windowType
                                    && !(bool)isAddNewWindowOnly
                                        ? Visibility.Visible
                                        : Visibility.Collapsed;
            this.butSelectChapter.Visibility = isPageable ? visibility : Visibility.Collapsed;
            this.butSearch.Visibility = isSearchable ? visibility : Visibility.Collapsed;

            visibility = this.selectDocumentType.SelectedIndex == 4 ? Visibility.Visible : Visibility.Collapsed;
            this.DateSelectPanel.Visibility = visibility;
            this.selectPlanType.Visibility = visibility;
        }

        /// <summary>
        /// The set book choosen.
        /// </summary>
        private void SetBookChoosen()
        {
            WindowType selectedType;
            SwordBook bookSelected;
            this.GetSelectedData(out selectedType, out bookSelected);
            object openWindowIndex;
            if (!PhoneApplicationService.Current.State.TryGetValue("openWindowIndex", out openWindowIndex))
            {
                openWindowIndex = 0;
            }

            SerializableWindowState state = App.OpenWindows[(int)openWindowIndex].State;
            if (!state.BibleToLoad.Equals(bookSelected.Sbmd.InternalName) || state.WindowType != selectedType)
            {
                state.WindowType = selectedType;
                state.BibleToLoad = bookSelected.Sbmd.InternalName;
                state.BibleDescription = bookSelected.Sbmd.Name;

                if (state.WindowType == WindowType.WindowDailyPlan)
                {
                    App.DailyPlan.PlanBible = state.BibleToLoad;
                    App.DailyPlan.PlanBibleDescription = state.BibleDescription;
                    App.DailyPlan.PlanTextSize = this.sliderTextSize.Value;
                }

                state.HtmlFontSize = this.sliderTextSize.Value;
                ((BrowserTitledWindow)App.OpenWindows[(int)openWindowIndex]).Initialize(
                    state.BibleToLoad, state.BibleDescription, state.WindowType);
            }
            else
            {
                if (state.WindowType == WindowType.WindowDailyPlan)
                {
                    App.DailyPlan.PlanTextSize = this.sliderTextSize.Value;
                }

                state.HtmlFontSize = this.sliderTextSize.Value;
            }
        }

        /// <summary>
        /// The setup entire page.
        /// </summary>
        private void SetupEntirePage()
        {
            this.PageTitle.Text = Translations.Translate("Settings");
            this.selectDocumentType.Header = Translations.Translate("Select the window type");
            this.selectDocument.Header = Translations.Translate("Select the bible");
            this.butSelectChapter.Content = Translations.Translate("Select book and chapter");
            this.butSearch.Content = Translations.Translate("Search");
            this.planStartDateCaption.Text = Translations.Translate("Select the daily plan start date");
            this.selectPlanType.Header = Translations.Translate("Select the daily plan");

            this.selectDocumentType.Items.Clear();
            this.selectDocumentType.Items.Add(Translations.Translate("Bible"));
            this.selectDocumentType.Items.Add(Translations.Translate("Notes"));
            this.selectDocumentType.Items.Add(Translations.Translate("History"));
            this.selectDocumentType.Items.Add(Translations.Translate("Bookmarks"));
            this.selectDocumentType.Items.Add(Translations.Translate("Daily plan"));
            this.selectDocumentType.Items.Add(Translations.Translate("Added notes"));
            if (App.InstalledBibles.InstalledCommentaries.Count > 0)
            {
                this.selectDocumentType.Items.Add(Translations.Translate("Commentaries"));
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

            this.selectDocument.Items.Clear();

            foreach (var book in App.InstalledBibles.InstalledBibles)
            {
                this.selectDocument.Items.Add(book.Value.Name);
                if ((bool)isAddNewWindowOnly == false && App.OpenWindows.Count > 0
                    && App.OpenWindows[(int)openWindowIndex].State.BibleToLoad.Equals(book.Value.Sbmd.InternalName))
                {
                    this.selectDocument.SelectedIndex = this.selectDocument.Items.Count - 1;
                }
            }

            Visibility visibility = Visibility.Visible;
            if (App.OpenWindows.Count == 0 || (bool)isAddNewWindowOnly)
            {
                visibility = Visibility.Collapsed;
            }

            this.butSelectChapter.Visibility = visibility;
            this.butSearch.Visibility = visibility;

            this.DateSelectPanel.Visibility = Visibility.Collapsed;
            this.selectPlanType.Visibility = Visibility.Collapsed;

            this.sliderTextSize.Value = (double)Application.Current.Resources["PhoneFontSizeNormal"] * 5 / 8;

            // must show the current window selections
            if (App.OpenWindows.Count > 0 && !(bool)isAddNewWindowOnly)
            {
                SerializableWindowState state = App.OpenWindows[(int)openWindowIndex].State;
                int bookNum;
                int relChaptNum;
                int absoluteChaptNum;
                int verseNum;
                string fullName;
                string titleText;
                state.Source.GetInfo(
                    out bookNum, out absoluteChaptNum, out relChaptNum, out verseNum, out fullName, out titleText);

                this.butSelectChapter.Visibility = state.Source.IsPageable ? visibility : Visibility.Collapsed;
                this.butSearch.Visibility = state.Source.IsSearchable ? visibility : Visibility.Collapsed;
                switch (state.WindowType)
                {
                    case WindowType.WindowBible:
                        this.selectDocumentType.SelectedIndex = 0;
                        break;
                    case WindowType.WindowBibleNotes:
                        this.selectDocumentType.SelectedIndex = 1;
                        break;
                    case WindowType.WindowHistory:
                        this.selectDocumentType.SelectedIndex = 2;
                        break;
                    case WindowType.WindowBookmarks:
                        this.selectDocumentType.SelectedIndex = 3;
                        break;
                    case WindowType.WindowDailyPlan:
                        this.selectDocumentType.SelectedIndex = 4;
                        this.selectPlanType.SelectedIndex = App.DailyPlan.PlanNumber;
                        this.planStartDate.Value = App.DailyPlan.PlanStartDate > DateTime.Now.AddYears(-100)
                                                       ? App.DailyPlan.PlanStartDate
                                                       : DateTime.Now;
                        break;
                    case WindowType.WindowAddedNotes:
                        this.selectDocumentType.SelectedIndex = 5;
                        break;
                    case WindowType.WindowCommentary:
                        this.selectDocument.Items.Clear();
                        foreach (var book in App.InstalledBibles.InstalledCommentaries)
                        {
                            this.selectDocument.Items.Add(book.Value.Name);
                            if ((bool)isAddNewWindowOnly == false && App.OpenWindows.Count > 0
                                &&
                                App.OpenWindows[(int)openWindowIndex].State.BibleToLoad.Equals(
                                    book.Value.Sbmd.InternalName))
                            {
                                this.selectDocument.SelectedIndex = this.selectDocument.Items.Count - 1;
                            }
                        }

                        this.selectDocumentType.SelectedIndex = 6;
                        break;
                    case WindowType.WindowInternetLink:
                        this.selectDocumentType.Visibility = Visibility.Collapsed;
                        this.selectDocument.Visibility = Visibility.Collapsed;
                        break;
                    case WindowType.WindowLexiconLink:
                        this.selectDocumentType.Visibility = Visibility.Collapsed;
                        this.selectDocument.Visibility = Visibility.Collapsed;
                        break;
                    case WindowType.WindowSearch:
                        this.selectDocumentType.Visibility = Visibility.Collapsed;
                        this.selectDocument.Visibility = Visibility.Collapsed;
                        break;
                }

                this.sliderTextSize.Value = state.HtmlFontSize;
            }
        }

        /// <summary>
        /// The slider text size value changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void SliderTextSizeValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (this.webBrowser1 != null)
            {
                this.webBrowser1.FontSize = e.NewValue;
                try
                {
                    this.webBrowser1.NavigateToString(
                        BibleZtextReader.HtmlHeader(
                            App.DisplaySettings, 
                            BrowserTitledWindow.GetBrowserColor("PhoneBackgroundColor"), 
                            BrowserTitledWindow.GetBrowserColor("PhoneForegroundColor"), 
                            BrowserTitledWindow.GetBrowserColor("PhoneAccentColor"), 
                            e.NewValue, 
                            Theme.FontFamilies[App.Themes.FontFamily]) + "<a class=\"normalcolor\" href=\"#\">"
                        + Translations.Translate("Text size") + "</a>" + "</body></html>");
                }
                catch (Exception ee)
                {
                    Debug.WriteLine("sliderTextSize_ValueChanged webBrowser1.NavigateToString; " + ee.Message);
                }
            }
        }

        #endregion
    }
}