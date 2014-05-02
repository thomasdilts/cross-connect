﻿// <copyright file="MainPageAddWindow.cs" company="Thomas Dilts">
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
// Email: thomas@cross-connect.se
// </summary>
// <author>Thomas Dilts</author>

namespace CrossConnect
{
    using System;
    using System.Diagnostics;
    using System.Linq;

    using Sword;
    using Sword.reader;

    using Windows.Storage;
    using Windows.UI.Core;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml.Controls.Primitives;

    public sealed partial class MainPageSplit
    {
        #region Fields

        private RadioButton[] columns;

        #endregion

        #region Public Methods and Operators

        public void ShowFontImage()
        {
            this.webBrowser1.Visibility = Visibility.Visible;
            this.webBrowser1.UpdateLayout();
            this.Dispatcher.RunAsync(
                CoreDispatcherPriority.Low,
                () =>
                    {
                        var b = new WebViewBrush();
                        b.SourceName = "webBrowser1";
                        b.Redraw();
                        this.UserInterfaceBlocker.Fill = b;
                        this.webBrowser1.Visibility = Visibility.Collapsed;
                    });
        }

        #endregion

        #region Methods

        private async void AddWindowPopup_OnClosed(object sender, object e)
        {
            try
            {
                WindowType selectedType;
                SwordBookMetaData bookSelected;
                this.GetSelectedData(out selectedType, out bookSelected); 
                if (this.selectDocumentType.SelectedIndex == 4)
                {
                    var startDate = new DateTime(
                        int.Parse(this.planStartDateYear.SelectedItem.ToString()),
                        int.Parse(this.planStartDateMonth.SelectedItem.ToString()),
                        int.Parse(this.planStartDateDay.SelectedItem.ToString()));
                    var actualDate  = new DateTime(
                        int.Parse(this.planActualDateYear.SelectedItem.ToString()),
                        int.Parse(this.planActualDateMonth.SelectedItem.ToString()),
                        int.Parse(this.planActualDateDay.SelectedItem.ToString()));
                    App.DailyPlan.PlanStartDate = startDate;
                    App.DailyPlan.PlanNumber = this.selectPlanType.SelectedIndex;
                    App.DailyPlan.PlanTextSize = this.sliderTextSize.Value;
                    int dayOfPlan = (int)actualDate.Subtract(startDate).TotalDays;
                    var schedule = DailyPlans.ZAllPlans(App.DailyPlan.PlanNumber);
                    if (dayOfPlan < 0 || dayOfPlan > schedule.GetUpperBound(0))
                    {
                        dayOfPlan = 0;
                    }

                    App.DailyPlan.PlanDayNumber = dayOfPlan;
                    App.DailyPlan.PlanBible = bookSelected.InternalName;
                    App.DailyPlan.PlanBibleDescription = bookSelected.Name;
                }

                int column = 0;
                for (int i = 0; i < this.columns.Count(); i++)
                {
                    if ((bool)this.columns[i].IsChecked)
                    {
                        column = i;
                        break;
                    }
                }

                await App.AddWindow(
                    bookSelected.InternalName,
                    bookSelected.Name,
                    selectedType,
                    this.sliderTextSize.Value,
                    column,
                    null,
                    false);
            }
            catch (Exception ee)
            {
                Debug.WriteLine("crashed in a strange place. err=" + ee.StackTrace);
            }

            App.ShowUserInterface(true);

            App.MainWindow.ReDrawWindows(false);
            App.StartTimerForSavingWindows();
        }

        private void AddWindowPopup_OnOpened(object sender, object e)
        {
            this.columns = new[]
                               {
                                   this.Col1, this.Col2, this.Col3, this.Col4, this.Col5, this.Col6, this.Col7, this.Col8,
                                   this.Col9
                               };
            this.SetupEntirePage();
            App.ShowUserInterface(false);
            this.Dispatcher.RunAsync(CoreDispatcherPriority.Low, () => this.ShowFontImage());
        }

        private void ButAddWindowClick(object sender, RoutedEventArgs e)
        {
            SideBarShowPopup(
                this.AddWindowPopup,
                this.MainPaneAddWindowPopup,
                this.scrollViewerAddWindow,
                this.TopAppBar1,
                this.BottomAppBar);
            this.AddWindowPopup.IsOpen = true;
        }

        private void GetSelectedData(out WindowType selectedType, out SwordBookMetaData bookSelected)
        {
            bookSelected = null;
            selectedType = WindowType.WindowBible;
            if (this.selectDocumentType.SelectedItem == null)
            {
                return;
            }

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
                default:
                    if (this.selectDocumentType.SelectedItem.Equals(Translations.Translate("Commentaries")))
                    {
                        selectedType = WindowType.WindowCommentary;
                    }
                    else if (this.selectDocumentType.SelectedItem.Equals(Translations.Translate("Books")))
                    {
                        selectedType = WindowType.WindowBook;
                    }
                    break;

            }

            if (this.selectDocument.SelectedItem != null)
            {
                if (selectedType == WindowType.WindowCommentary)
                {
                    // did the book choice change?
                    foreach (var book in App.InstalledBibles.InstalledCommentaries)
                    {
                        if (this.selectDocument.SelectedItem.Equals(book.Value.Name))
                        {
                            bookSelected = book.Value;
                            break;
                        }
                    }
                }
                else if (selectedType == WindowType.WindowBook)
                {
                    // did the book choice change?
                    foreach (var book in App.InstalledBibles.InstalledGeneralBooks)
                    {
                        if (this.selectDocument.SelectedItem.Equals(book.Value.Name))
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
                        if (this.selectDocument.SelectedItem.Equals(book.Value.Name))
                        {
                            bookSelected = book.Value;
                            break;
                        }
                    }
                }
            }
        }

        private void SelectDocumentTypeSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.selectDocumentType.SelectedItem == null)
            {
                return;
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

            }
            else if (this.selectDocumentType.SelectedItem.Equals(Translations.Translate("Commentaries")))
            {
                this.selectDocument.Items.Clear();
                foreach (var book in App.InstalledBibles.InstalledCommentaries)
                {
                    this.selectDocument.Items.Add(book.Value.Name);
                }

                if (this.selectDocument.Items.Count() > 0)
                {
                    this.selectDocument.SelectedIndex = 0;
                }
            }
            else if (this.selectDocumentType.SelectedItem.Equals(Translations.Translate("Books")))
            {
                this.selectDocument.Items.Clear();
                foreach (var book in App.InstalledBibles.InstalledGeneralBooks)
                {
                    this.selectDocument.Items.Add(book.Value.Name);
                }

                if (this.selectDocument.Items.Count() > 0)
                {
                    this.selectDocument.SelectedIndex = 0;
                }
            }
            else
            {
                this.selectDocument.Items.Clear();
                foreach (var book in App.InstalledBibles.InstalledBibles)
                {
                    this.selectDocument.Items.Add(book.Value.Name);
                }

                if (this.selectDocument.Items.Any())
                {
                    this.selectDocument.SelectedIndex = 0;
                }
            }

            bool isPageable = false;
            bool isSearchable = false;
            var windowType = WindowType.WindowBible;
            if (App.OpenWindows.Any() && App.OpenWindows[0].State != null)
            {
                SerializableWindowState state = App.OpenWindows[0].State;
                isPageable = state.Source.IsPageable;
                isSearchable = state.Source.IsSearchable;
                windowType = state.WindowType;
            }

            Visibility visibility = this.selectDocumentType.SelectedIndex == 4 ? Visibility.Visible : Visibility.Collapsed;
            this.DateSelectPanel.Visibility = visibility;
            this.DateActualSelectPanel.Visibility = visibility;
            this.planStartDateCaption.Visibility = visibility;
            this.planActualDateCaption.Visibility = visibility;
            this.selectPlanType.Visibility = visibility;
            if (this.selectDocumentType.SelectedIndex == 4)
            {
                FillDateSelection(planStartDateYear, planStartDateMonth, planStartDateDay, App.DailyPlan.PlanStartDate);
                DateTime actualDate = App.DailyPlan.PlanStartDate.AddDays(App.DailyPlan.PlanDayNumber);
                FillDateSelection(planActualDateYear, planActualDateMonth, planActualDateDay, actualDate);
                //select the same bible if possible
                for (int i = 0; i < this.selectDocument.Items.Count(); i++)
                {
                    if (this.selectDocument.Items[i].Equals(App.DailyPlan.PlanBibleDescription))
                    {
                        this.selectDocument.SelectedIndex = i;
                    }
                }
            }
        }
        private void ButSetDateTodayClick(object sender, RoutedEventArgs e)
        {
            DateTime now = DateTime.Now;

            FillDateSelection(planStartDateYear, planStartDateMonth, planStartDateDay, now);
            FillDateSelection(planActualDateYear, planActualDateMonth, planActualDateDay, now);
        }
        private static void FillDateSelection(ComboBox dateYear, ComboBox dateMonth, ComboBox dateDay, DateTime showDate)
        {
            dateYear.Items.Clear();
            dateYear.Items.Add(showDate.AddYears(-1).Year.ToString());
            dateYear.Items.Add(showDate.AddYears(0).Year.ToString());
            dateYear.Items.Add(showDate.AddYears(1).Year.ToString());
            dateYear.SelectedIndex = 1;

            dateMonth.Items.Clear();
            for (int i = 1; i <= 12; i++)
            {
                dateMonth.Items.Add(i.ToString());
            }
            dateMonth.SelectedIndex = (showDate.Month - 1);

            dateDay.Items.Clear();
            for (int i = 1; i <= 31; i++)
            {
                dateDay.Items.Add(i.ToString());
            }
            dateDay.SelectedIndex = (showDate.Day - 1);
        }

        private void SetupEntirePage()
        {
            this.AddWindowTitle.Text = Translations.Translate("Add a new window");
            this.selectDocumentTypeHeader.Text = Translations.Translate("Select the window type");
            this.selectDocumentHeader.Text = Translations.Translate("Select the bible");
            this.planStartDateCaption.Text = Translations.Translate("Select the daily plan start date");
            this.planActualDateCaption.Text = Translations.Translate("Select the daily plan current date");
            this.selectPlanTypeHeader.Text = Translations.Translate("Select the daily plan");
            this.SetDateToday.Content = Translations.Translate("Today");

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
            if (App.InstalledBibles.InstalledGeneralBooks.Count > 0)
            {
                this.selectDocumentType.Items.Add(Translations.Translate("Books"));
            }

            this.selectDocument.Items.Clear();

            foreach (var book in App.InstalledBibles.InstalledBibles)
            {
                this.selectDocument.Items.Add(book.Value.Name);
            }
            if (this.selectDocument.Items.Any())
            {
                this.selectDocument.SelectedIndex = 0;
            }

            this.selectPlanTypeHeader.Visibility = Visibility.Collapsed;
            this.DateSelectPanel.Visibility = Visibility.Collapsed;
            this.planStartDateCaption.Visibility = Visibility.Collapsed;
            this.DateActualSelectPanel.Visibility = Visibility.Collapsed;
            this.planActualDateCaption.Visibility = Visibility.Collapsed;
            this.selectPlanType.Visibility = Visibility.Collapsed;

            this.sliderTextSize.Value = 20;
            this.selectDocumentType.SelectedIndex = 0;
            if (App.OpenWindows.Any() && App.OpenWindows[0].State != null)
            {
                this.sliderTextSize.Value = App.OpenWindows[0].State.HtmlFontSize;
            }

            this.columns[0].IsChecked = true;
        }

        private void SliderTextSizeValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            if (this.webBrowser1 != null)
            {
                //webBrowser1.FontSize = e.NewValue;
                try
                {
                    this.webBrowser1.NavigateToString(
                        BibleZtextReader.HtmlHeader(
                            App.DisplaySettings,
                            BrowserTitledWindow.GetBrowserColor("PhoneBackgroundColor"),
                            BrowserTitledWindow.GetBrowserColor("PhoneForegroundColor"),
                            BrowserTitledWindow.GetBrowserColor("PhoneAccentColor"),
                            BrowserTitledWindow.GetBrowserColor("PhoneWordsOfChristColor"),
                            e.NewValue,
                            Theme.FontFamilies[App.Themes.FontFamily]) + "<a class=\"normalcolor\" href=\"#\">"
                        + Translations.Translate("Text size") + "</a>" + "</body></html>");
                }
                catch (Exception ee)
                {
                    Debug.WriteLine("sliderTextSize_ValueChanged webBrowser1.NavigateToString; " + ee.Message);
                }
                this.Dispatcher.RunAsync(CoreDispatcherPriority.Low, () => this.ShowFontImage());
            }
        }

        #endregion
    }
}