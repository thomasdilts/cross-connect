// <copyright file="MainPageAddWindow.cs" company="Thomas Dilts">
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
    using Windows.UI.Xaml.Media;

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
                    App.DailyPlan.PlanTextSize = this.sliderTextSize.SliderValue;
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
                var fontFamily = WindowFontComboBox.SelectedItem!=null?((TextBlock)WindowFontComboBox.SelectedItem).Text:string.Empty;
                if (WindowToChange == null)
                {
                    await App.AddWindow(
                        bookSelected.InternalName,
                        bookSelected.Name,
                        selectedType,
                        this.sliderTextSize.SliderValue,
                        fontFamily,
                        SelectVerses.GetPlaceMarkers(),
                        column,
                        null,
                        false);
                }
                else
                {
                    string relbookShortName;
                    int relChaptNum;
                    int relverseNum;
                    string fullName;
                    string title;
                    WindowToChange.State.Source.GetInfo(
                        Translations.IsoLanguageCode,
                        out relbookShortName,
                        out relChaptNum,
                        out relverseNum,
                        out fullName,
                        out title);
                    WindowToChange.State.Font = fontFamily;
                    WindowToChange.State.HtmlFontSize = this.sliderTextSize.SliderValue;
                    WindowToChange.State.WindowType = selectedType;
                    await WindowToChange.Initialize(bookSelected.InternalName,
                        bookSelected.Name, selectedType, SelectVerses.GetPlaceMarkers());
                    WindowToChange.State.Source.MoveChapterVerse(relbookShortName, relChaptNum, relverseNum, false, WindowToChange.State.Source);
                    WindowToChange.ForceReload = true;
                    WindowToChange.DelayUpdateBrowser();
                    App.StartTimerForNotifications();
                }
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

        private BrowserTitledWindow WindowToChange = null;
        public void ButAddWindowClick(object sender, RoutedEventArgs e)
        {
            WindowToChange = sender is BrowserTitledWindow ? (BrowserTitledWindow)sender : null;

            SideBarShowPopup(
                this.AddWindowPopup,
                this.MainPaneAddWindowPopup,
                this.scrollViewerAddWindow,
                this.TopAppBar1,
                this.BottomAppBar);
            this.AddWindowPopup.IsOpen = true;
        }

        private void SetSelectedData(WindowType selectedType, string bookSelected)
        {
            switch (selectedType)
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
                    break;
                case WindowType.WindowAddedNotes:
                    this.selectDocumentType.SelectedIndex = 5;
                    break;
                case WindowType.WindowSelectedVerses:
                    this.selectDocumentType.SelectedIndex = 6;
                    break;
                case WindowType.WindowCommentary:
                    this.selectDocumentType.SelectedIndex = 7;
                    break;
                case WindowType.WindowBook:
                    this.selectDocumentType.SelectedIndex = App.InstalledBibles.InstalledCommentaries.Count > 0?8:7;
                    break;
            }
            foreach (var item in  this.selectDocument.Items)
	        {
                if (item.Equals(bookSelected))
                {
                    this.selectDocument.SelectedItem = item;
                    break;
                }
	        }
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
                case 6:
                    selectedType = WindowType.WindowSelectedVerses;
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
            
            if (this.selectDocumentType.SelectedItem.Equals(Translations.Translate("Commentaries")))
            {
                this.selectDocument.Items.Clear();
                foreach (var book in App.InstalledBibles.InstalledCommentaries)
                {
                    this.selectDocument.Items.Add(book.Value.Name);
                }
            }
            else if (this.selectDocumentType.SelectedItem.Equals(Translations.Translate("Books")))
            {
                this.selectDocument.Items.Clear();
                foreach (var book in App.InstalledBibles.InstalledGeneralBooks)
                {
                    this.selectDocument.Items.Add(book.Value.Name);
                }
            }
            else
            {
                this.selectDocument.Items.Clear();
                foreach (var book in App.InstalledBibles.InstalledBibles)
                {
                    this.selectDocument.Items.Add(book.Value.Name);
                }
            }

            if (WindowToChange != null)
            {
                foreach (var item in this.selectDocument.Items)
                {
                    if (item.Equals(WindowToChange.State.BibleDescription))
                    {
                        this.selectDocument.SelectedItem = item;
                        break;
                    }
                }
            }
            if (this.selectDocument.Items.Any() && this.selectDocument.SelectedIndex < 0)
            {
                this.selectDocument.SelectedIndex = 0;
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

            this.SelectVerses.Visibility = this.selectDocumentType.SelectedIndex == 6 ? Visibility.Visible : Visibility.Collapsed; 


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
            this.AddWindowTitle.Text = Translations.Translate(WindowToChange==null?"Add a new window":"Settings");
            this.selectDocumentTypeHeader.Text = Translations.Translate("Select the window type");
            this.selectDocumentHeader.Text = Translations.Translate("Select the bible");
            this.planStartDateCaption.Text = Translations.Translate("Select the daily plan start date");
            this.planActualDateCaption.Text = Translations.Translate("Select the daily plan current date");
            this.selectPlanTypeHeader.Text = Translations.Translate("Select the daily plan");
            this.SetDateToday.Content = Translations.Translate("Today");
            this.WindowFontComboBoxHeader.Text = Translations.Translate("Font");
            this.selectColumn.Text = Translations.Translate("Column");

            this.selectDocumentType.Items.Clear();
            this.selectDocumentType.Items.Add(Translations.Translate("Bible"));
            this.selectDocumentType.Items.Add(Translations.Translate("Notes"));
            this.selectDocumentType.Items.Add(Translations.Translate("History"));
            this.selectDocumentType.Items.Add(Translations.Translate("Bookmarks"));
            this.selectDocumentType.Items.Add(Translations.Translate("Daily plan"));
            this.selectDocumentType.Items.Add(Translations.Translate("Added notes"));
            this.selectDocumentType.Items.Add(Translations.Translate("Selected verses"));

            if (App.InstalledBibles.InstalledCommentaries.Count > 0)
            {
                this.selectDocumentType.Items.Add(Translations.Translate("Commentaries"));
            }
            if (App.InstalledBibles.InstalledGeneralBooks.Count > 0)
            {
                this.selectDocumentType.Items.Add(Translations.Translate("Books"));
            }

            //this.selectDocument.Items.Clear();

            //foreach (var book in App.InstalledBibles.InstalledBibles)
            //{
            //    this.selectDocument.Items.Add(book.Value.Name);
            //}
            this.stackPanelSelectColumn.Visibility = WindowToChange == null ? Visibility.Visible : Visibility.Collapsed;
            this.selectColumn.Visibility = WindowToChange == null ? Visibility.Visible : Visibility.Collapsed;
            this.selectPlanTypeHeader.Visibility = Visibility.Collapsed;
            this.DateSelectPanel.Visibility = Visibility.Collapsed;
            this.planStartDateCaption.Visibility = Visibility.Collapsed;
            this.DateActualSelectPanel.Visibility = Visibility.Collapsed;
            this.planActualDateCaption.Visibility = Visibility.Collapsed;
            this.selectPlanType.Visibility = Visibility.Collapsed;

            this.sliderTextSize.SliderValue = 20;
            if (App.OpenWindows.Any() && App.OpenWindows[0].State != null)
            {
                this.sliderTextSize.SliderValue = App.OpenWindows[0].State.HtmlFontSize;
            }

            this.columns[0].IsChecked = true;
            ItemCollection itemCollection = this.WindowFontComboBox.Items;
            if (itemCollection != null)
            {
                itemCollection.Clear();
                foreach (var font in Theme.FontFamilies)
                {
                    itemCollection.Add(
                        new TextBlock
                        {
                            FontSize = 28,
                            Text = font.Key,
                            FontFamily = new FontFamily(font.Value),
                            Tag = font.Value
                        }
                    );
                }
            }
            //selectDocumentSelectionChanged(null, null);
            if (WindowToChange != null)
            {
                SetSelectedData(WindowToChange.State.WindowType, WindowToChange.State.BibleDescription);
                this.sliderTextSize.SliderValue = WindowToChange.State.HtmlFontSize;
                foreach (var item in WindowFontComboBox.Items)
                {
                    if (((TextBlock)item).Text.Equals(WindowToChange.State.Font))
                    {
                        WindowFontComboBox.SelectedItem = item;
                        break;
                    }
                }
            }

            if (this.selectDocumentType.Items.Any() && this.selectDocumentType.SelectedIndex < 0)
            {
                this.selectDocumentType.SelectedIndex = 0;
            }
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

        private void WindowFontComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                this.webBrowser1.NavigateToString(
                    BibleZtextReader.HtmlHeader(
                        App.DisplaySettings,
                        BrowserTitledWindow.GetBrowserColor("PhoneBackgroundColor"),
                        BrowserTitledWindow.GetBrowserColor("PhoneForegroundColor"),
                        BrowserTitledWindow.GetBrowserColor("PhoneAccentColor"),
                        BrowserTitledWindow.GetBrowserColor("PhoneWordsOfChristColor"),
                        this.sliderTextSize.SliderValue,
                        Theme.FontFamilies[((TextBlock)WindowFontComboBox.SelectedItem).Text]) + "<a class=\"normalcolor\" href=\"#\">"
                    + Translations.Translate("Text size") + "</a>" + "</body></html>");
            }
            catch (Exception ee)
            {
                Debug.WriteLine("sliderTextSize_ValueChanged webBrowser1.NavigateToString; " + ee.Message);
            }
            this.Dispatcher.RunAsync(CoreDispatcherPriority.Low, () => this.ShowFontImage());
        }

        private bool IsInSelectDocChanged = false;
        private async void selectDocumentSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (selectDocument.SelectedIndex<0)
            {
                return;
            }

            if(IsInSelectDocChanged)
            {
                return;
            }
            IsInSelectDocChanged = true;
            WindowType selectedType;
            SwordBookMetaData bookSelected;
            this.WindowFontComboBox.SelectedIndex = -1;
            this.GetSelectedData(out selectedType, out bookSelected);
            if (bookSelected != null)
            {
                var fontFromBible = (string)bookSelected.GetProperty(ConfigEntryType.Font);
                if (fontFromBible != null)
                {
                    string fontFamily;
                    if (Theme.FontFamilies.TryGetValue(fontFromBible, out fontFamily))
                    {
                        ItemCollection itemCollection = this.WindowFontComboBox.Items;
                        if (itemCollection != null)
                        {
                            for (int i = 0; i < itemCollection.Count; i++)
                            {
                                if (((TextBlock)itemCollection[i]).Text.Equals(fontFromBible))
                                {
                                    this.WindowFontComboBox.SelectedIndex = i;
                                    break;
                                }
                            }
                        }
                    }
                }

                if (selectedType == WindowType.WindowSelectedVerses)
                {
                    string bookPath =
                        bookSelected.GetCetProperty(ConfigEntryType.ADataPath).ToString().Substring(2);
                    bool isIsoEncoding = !bookSelected.GetCetProperty(ConfigEntryType.Encoding).Equals("UTF-8");

                    this.SelectVerses.source = new BibleZtextReader(
                                        bookPath,
                                        ((Language)bookSelected.GetCetProperty(ConfigEntryType.Lang)).Code,
                                        isIsoEncoding,
                                        (string)bookSelected.GetCetProperty(ConfigEntryType.CipherKey),
                                        bookSelected.ConfPath,
                                        (string)bookSelected.GetCetProperty(ConfigEntryType.Versification));
                    await SelectVerses.source.Initialize();
                    var markers = new System.Collections.Generic.List<BiblePlaceMarker>();
                    if (this.WindowToChange != null && this.WindowToChange.State.Source is BiblePlaceMarkReader && ((BiblePlaceMarkReader)this.WindowToChange.State.Source)._title.Equals(Translations.Translate("Selected verses")))
                    {
                        markers = BiblePlaceMarker.Clone( ((BiblePlaceMarkReader)this.WindowToChange.State.Source).BookMarksToShow);
                    }
                    this.SelectVerses.SetPlaceMarkers(markers);
                }
            }

            IsInSelectDocChanged = false;

        }
        #endregion
    }
}