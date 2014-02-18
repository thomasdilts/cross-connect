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
    using System.Text;
    using System.Windows;
    using System.Windows.Controls;
    using System.Linq;

    using CrossConnect.readers;

    using Hoot;

    using Microsoft.Phone.Shell;

    using Sword;
    using Sword.reader;
    using Windows.Phone.Speech.Synthesis;
    using Sword.versification;

    /// <summary>
    /// The window settings.
    /// </summary>
    public partial class WindowSettings
    {
        private bool _isInThisWindow = false;
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="WindowSettings"/> class.
        /// </summary>
        public WindowSettings()
        {
            InitializeComponent();
        }

        #endregion Constructors

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
            _isInThisWindow = false;
            SetBookChoosen();
            NavigationService.Navigate(new Uri("/Search.xaml", UriKind.Relative));
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
            _isInThisWindow = false;
            SetBookChoosen();
            NavigationService.Navigate(new Uri("/SelectBibleBook.xaml", UriKind.Relative));
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
                else if (selectedType == WindowType.WindowBook)
                {
                    // did the book choice change?
                    foreach (var book in App.InstalledBibles.InstalledGeneralBooks)
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

        private bool HasFoundGoodKey = false;

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
            _isInThisWindow = false;

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

            if (App.OpenWindows.Count > 0 && !(bool)isAddNewWindowOnly)
            {
                SerializableWindowState state = App.OpenWindows[(int)openWindowIndex].State;
                if (state.Source is BibleZtextReader)
                {
                    if (HasFoundGoodKey || ((BibleZtextReader)state.Source).IsLocked)
                    {
                        return;
                    }
                }
            }

            try
            {
                if (selectDocumentType.SelectedIndex == 4)
                {
                    if (planStartDate.Value != null)
                    {
                        App.DailyPlan.PlanStartDate = (DateTime)planStartDate.Value;
                    }

                    App.DailyPlan.PlanNumber = selectPlanType.SelectedIndex;
                }

                if ((bool)isAddNewWindowOnly)
                {
                    WindowType selectedType;
                    SwordBook bookSelected;
                    GetSelectedData(out selectedType, out bookSelected);

                    App.AddWindow(
                        bookSelected.Sbmd.InternalName, bookSelected.Sbmd.Name, selectedType, sliderTextSize.Value);

                    // if (NavigationService.CanGoBack)
                    // {
                    // NavigationService.GoBack();
                    // }
                }
                else
                {
                    if (App.OpenWindows[(int)openWindowIndex].State.WindowType == WindowType.WindowSearch
                        || App.OpenWindows[(int)openWindowIndex].State.WindowType == WindowType.WindowLexiconLink
                        || App.OpenWindows[(int)openWindowIndex].State.WindowType == WindowType.WindowTranslator)
                    {
                        App.OpenWindows[(int)openWindowIndex].State.HtmlFontSize = sliderTextSize.Value;
                    }
                    else
                    {
                        SetBookChoosen();
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
            if (_isInThisWindow)
            {
                return;
            }

            _isInThisWindow = true;
            object skipWindowSettings;
            if (PhoneApplicationService.Current.State.TryGetValue("skipWindowSettings", out skipWindowSettings))
            {
                if ((bool)skipWindowSettings)
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

            if ((bool)initializeWindow)
            {
                SetupEntirePage();
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

            if (selectDocumentType.SelectedIndex == 4)
            {
                // prefill and show the next 2 fields.
                selectPlanType.Items.Clear();
                for (int i = 0; i <= DailyPlans.ZzAllPlansNames.GetUpperBound(0); i++)
                {
                    selectPlanType.Items.Add(
                        Translations.Translate(DailyPlans.ZzAllPlansNames[i][0]) + "; "
                        + DailyPlans.ZzAllPlansNames[i][1] + " " + Translations.Translate("Days") + "; "
                        + DailyPlans.ZzAllPlansNames[i][2] + " " + Translations.Translate("Minutes/Day"));
                }

                selectPlanType.SelectedIndex = App.DailyPlan.PlanNumber;
                planStartDate.Value = App.DailyPlan.PlanStartDate > DateTime.Now.AddYears(-100)
                                               ? App.DailyPlan.PlanStartDate
                                               : DateTime.Now;
            }
            else if (this.selectDocumentType.SelectedItem!=null && this.selectDocumentType.SelectedItem.Equals(Translations.Translate("Commentaries")))
            {
                selectDocument.Items.Clear();
                foreach (var book in App.InstalledBibles.InstalledCommentaries)
                {
                    selectDocument.Items.Add(book.Value.Name);
                    if ((bool)isAddNewWindowOnly == false && App.OpenWindows.Count > 0
                        && App.OpenWindows[(int)openWindowIndex].State.BibleToLoad.Equals(book.Value.Sbmd.InternalName))
                    {
                        selectDocument.SelectedIndex = selectDocument.Items.Count - 1;
                    }
                }
            }
            else if (this.selectDocumentType.SelectedItem!=null && this.selectDocumentType.SelectedItem.Equals(Translations.Translate("Books")))
            {
                selectDocument.Items.Clear();
                foreach (var book in App.InstalledBibles.InstalledGeneralBooks)
                {
                    selectDocument.Items.Add(book.Value.Name);
                    if ((bool)isAddNewWindowOnly == false && App.OpenWindows.Count > 0
                        && App.OpenWindows[(int)openWindowIndex].State.BibleToLoad.Equals(book.Value.Sbmd.InternalName))
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
                    if ((bool)isAddNewWindowOnly == false && App.OpenWindows.Count > 0
                        && App.OpenWindows[(int)openWindowIndex].State.BibleToLoad.Equals(book.Value.Sbmd.InternalName))
                    {
                        selectDocument.SelectedIndex = selectDocument.Items.Count - 1;
                    }
                }
            }

            bool isTranslateable = false;
            bool isListenable = false;
            bool isTtsListenable = false;
            WindowType windowType = WindowType.WindowBible;
            if (App.OpenWindows.Count > 0 && App.OpenWindows[(int)openWindowIndex].State != null)
            {
                SerializableWindowState state = App.OpenWindows[(int)openWindowIndex].State;
                string bookNameShort;
                int relChaptNum;
                int verseNum;
                string fullName;
                string titleText;
                state.Source.GetInfo(
                    out bookNameShort, out relChaptNum, out verseNum, out fullName, out titleText);
                isTranslateable = state.Source.IsTranslateable && !state.Source.GetLanguage().Equals(Translations.IsoLanguageCode);
                var canon = CanonManager.GetCanon("KJV");
                isListenable = state.Source.IsHearable && canon.BookByShortName.ContainsKey(bookNameShort);
                isTtsListenable = state.Source.IsTTChearable && InstalledVoices.All.Any();
                windowType = state.WindowType;
            }

            Visibility visibility = (((selectDocumentType.SelectedIndex + 1) == (int)windowType) || this.selectDocumentType.SelectedItem != null && this.selectDocumentType.SelectedItem.Equals(Translations.Translate("Books")))
                                    && !(bool)isAddNewWindowOnly
                                        ? Visibility.Visible
                                        : Visibility.Collapsed;
            butTranslate.Visibility = isTranslateable ? visibility : Visibility.Collapsed;
            butListen.Visibility = isListenable ? visibility : Visibility.Collapsed;
            butListenTts.Visibility = isTtsListenable ? visibility : Visibility.Collapsed;

            visibility = selectDocumentType.SelectedIndex == 4 ? Visibility.Visible : Visibility.Collapsed;
            DateSelectPanel.Visibility = visibility;
            selectPlanType.Visibility = visibility;
        }

        /// <summary>
        /// The set book choosen.
        /// </summary>
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
                    App.DailyPlan.PlanTextSize = sliderTextSize.Value;
                }

                state.HtmlFontSize = sliderTextSize.Value;
                ((BrowserTitledWindow)App.OpenWindows[(int)openWindowIndex]).Initialize(
                    state.BibleToLoad, state.BibleDescription, state.WindowType);
            }
            else
            {
                if (state.WindowType == WindowType.WindowDailyPlan)
                {
                    App.DailyPlan.PlanTextSize = sliderTextSize.Value;
                }

                state.HtmlFontSize = sliderTextSize.Value;
            }
        }

        /// <summary>
        /// The setup entire page.
        /// </summary>
        private void SetupEntirePage()
        {
            PageTitle.Text = Translations.Translate("Settings");
            selectDocumentType.Header = Translations.Translate("Select the window type");
            selectDocument.Header = Translations.Translate("Select the bible");
            butTranslate.Content = Translations.Translate("Translate selected text");
            butListen.Content = Translations.Translate("Listen to this chapter");
            butListenTts.Content = "(TTS) " + Translations.Translate("Listen to this chapter");
            planStartDateCaption.Text = Translations.Translate("Select the daily plan start date");
            selectPlanType.Header = Translations.Translate("Select the daily plan");
            EnterKeyTitle.Text = Translations.Translate("Enter key");
            butEnterKeySave.Content = Translations.Translate("Save");

            selectDocumentType.Items.Clear();
            selectDocumentType.Items.Add(Translations.Translate("Bible"));
            selectDocumentType.Items.Add(Translations.Translate("Notes"));
            selectDocumentType.Items.Add(Translations.Translate("History"));
            selectDocumentType.Items.Add(Translations.Translate("Bookmarks"));
            selectDocumentType.Items.Add(Translations.Translate("Daily plan"));
            selectDocumentType.Items.Add(Translations.Translate("My comments"));
            if (App.InstalledBibles.InstalledCommentaries.Count > 0)
            {
                selectDocumentType.Items.Add(Translations.Translate("Commentaries"));
            }
            if (App.InstalledBibles.InstalledGeneralBooks.Count > 0)
            {
                selectDocumentType.Items.Add(Translations.Translate("Books"));
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
                if ((bool)isAddNewWindowOnly == false && App.OpenWindows.Count > 0
                    && App.OpenWindows[(int)openWindowIndex].State.BibleToLoad.Equals(book.Value.Sbmd.InternalName))
                {
                    selectDocument.SelectedIndex = selectDocument.Items.Count - 1;
                }
            }

            Visibility visibility = Visibility.Visible;
            if (App.OpenWindows.Count == 0 || (bool)isAddNewWindowOnly)
            {
                visibility = Visibility.Collapsed;
            }

            butListen.Visibility = visibility;
            butListenTts.Visibility = visibility;
            butTranslate.Visibility = visibility;

            DateSelectPanel.Visibility = Visibility.Collapsed;
            selectPlanType.Visibility = Visibility.Collapsed;

            EnterKeyTitle.Visibility = Visibility.Collapsed;
            EnterKeyText.Visibility = Visibility.Collapsed;
            butEnterKeySave.Visibility = Visibility.Collapsed;

            sliderTextSize.Value = (double)Application.Current.Resources["PhoneFontSizeNormal"] * 5 / 8;
            bool isLocked = false;
            // must show the current window selections
            if (App.OpenWindows.Count > 0 && !(bool)isAddNewWindowOnly)
            {
                SerializableWindowState state = App.OpenWindows[(int)openWindowIndex].State;
                if (state.Source is BibleZtextReader)
                {
                    isLocked = ((BibleZtextReader)state.Source).IsLocked;
                    if (isLocked)
                    {
                        EnterKeyTitle.Visibility = Visibility.Visible;
                        EnterKeyText.Visibility = Visibility.Visible;
                        butEnterKeySave.Visibility = Visibility.Visible;
                        EnterKeyText.Focus();

                        butListenTts.Visibility = Visibility.Collapsed;
                        butListen.Visibility = Visibility.Collapsed;
                        butTranslate.Visibility = Visibility.Collapsed;
                        DateSelectPanel.Visibility = Visibility.Collapsed;
                        selectPlanType.Visibility = Visibility.Collapsed;
                        selectDocumentType.Visibility = Visibility.Collapsed;
                        selectDocument.Visibility = Visibility.Collapsed;
                        webBrowser1.Visibility=Visibility.Collapsed;
                        sliderTextSize.Visibility=Visibility.Collapsed;
                        
                        planStartDateCaption.Visibility = Visibility.Collapsed;
                        return;

                    }
                }

                string bookNameShort;
                int relChaptNum;
                int verseNum;
                string fullName;
                string titleText;
                state.Source.GetInfo(
                    out bookNameShort, out relChaptNum, out verseNum, out fullName, out titleText);
                var canon = CanonManager.GetCanon("KJV");
                butTranslate.Visibility = state.Source.IsTranslateable && !state.Source.GetLanguage().Equals(Translations.IsoLanguageCode) ? visibility : Visibility.Collapsed;
                butListen.Visibility = state.Source.IsHearable && canon.BookByShortName.ContainsKey(bookNameShort) ? visibility : Visibility.Collapsed;
                butListenTts.Visibility = state.Source.IsTTChearable && InstalledVoices.All.Any() ? visibility : Visibility.Collapsed;
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
                        planStartDate.Value = App.DailyPlan.PlanStartDate > DateTime.Now.AddYears(-100)
                                                       ? App.DailyPlan.PlanStartDate
                                                       : DateTime.Now;
                        break;
                    case WindowType.WindowAddedNotes:
                        selectDocumentType.SelectedIndex = 5;
                        break;
                    case WindowType.WindowBook:
                        selectDocument.Items.Clear();
                        foreach (var book in App.InstalledBibles.InstalledGeneralBooks)
                        {
                            selectDocument.Items.Add(book.Value.Name);
                            if ((bool)isAddNewWindowOnly == false && App.OpenWindows.Count > 0
                                &&
                                App.OpenWindows[(int)openWindowIndex].State.BibleToLoad.Equals(
                                    book.Value.Sbmd.InternalName))
                            {
                                selectDocument.SelectedIndex = selectDocument.Items.Count - 1;
                            }
                        }
                        this.selectDocumentType.SelectedIndex = App.InstalledBibles.InstalledCommentaries.Count > 0 ? 7 : 6;

                        break;
                    case WindowType.WindowCommentary:
                        selectDocument.Items.Clear();
                        foreach (var book in App.InstalledBibles.InstalledCommentaries)
                        {
                            selectDocument.Items.Add(book.Value.Name);
                            if ((bool)isAddNewWindowOnly == false && App.OpenWindows.Count > 0
                                &&
                                App.OpenWindows[(int)openWindowIndex].State.BibleToLoad.Equals(
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
                            BrowserTitledWindow.GetBrowserColor("PhoneWordsOfChristColor"),
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

        #endregion Methods

        private async void ButTranslate_OnClick(object sender, RoutedEventArgs e)
        {
            _isInThisWindow = false;
            SetBookChoosen();
            object openWindowIndex;
            if (!PhoneApplicationService.Current.State.TryGetValue("openWindowIndex", out openWindowIndex))
            {
                openWindowIndex = 0;
            }

            SerializableWindowState state = App.OpenWindows[(int)openWindowIndex].State;
            var obj = await state.Source.GetTranslateableTexts(
                App.DisplaySettings, state.BibleToLoad);
            var toTranslate = (string[])obj[0];
            var isTranslateable = (bool[])obj[1];
            var serial = ((BibleZtextReader)state.Source).Serial;
            var transReader2 = new TranslatorReader(serial.Path, serial.Iso2DigitLangCode, serial.IsIsoEncoding, serial.CipherKey, serial.ConfigPath, serial.Versification);
            App.AddWindow(
                state.BibleToLoad,
                state.BibleDescription,
                WindowType.WindowTranslator,
                state.HtmlFontSize,
                transReader2);
            transReader2.TranslateThis(toTranslate, isTranslateable, state.Source.GetLanguage());

            PhoneApplicationService.Current.State["skipWindowSettings"] = false;

            // request to skip this window.
            if (NavigationService.CanGoBack)
            {
                Debug.WriteLine("WindowSettings AutoBackout");
                NavigationService.GoBack();
            }
        }

        private void ButListen_OnClick(object sender, RoutedEventArgs e)
        {
            _isInThisWindow = false;
            SetBookChoosen();
            object openWindowIndex;
            if (!PhoneApplicationService.Current.State.TryGetValue("openWindowIndex", out openWindowIndex))
            {
                openWindowIndex = 0;
            }

            SerializableWindowState state = App.OpenWindows[(int)openWindowIndex].State;
            string bookNameShort;
            int relChaptNum;
            int verseNum;
            string fullName;
            string titleText;
            state.Source.GetInfo(
                out bookNameShort, out relChaptNum, out verseNum, out fullName, out titleText);
            PhoneApplicationService.Current.State["BookToHear"] = bookNameShort;
            PhoneApplicationService.Current.State["ChapterToHear"] = relChaptNum;
            PhoneApplicationService.Current.State["VerseToHear"] = verseNum;
            PhoneApplicationService.Current.State["ChapterToHearLanguage"] = state.Source.GetLanguage();
            PhoneApplicationService.Current.State["titleBar"] = titleText;
            PhoneApplicationService.Current.State["skipWindowSettings"] = true;
            NavigationService.Navigate(new Uri("/SelectToPlay.xaml", UriKind.Relative));
        }

        private async void ButEnterKeySave_OnClick(object sender, RoutedEventArgs e)
        {
            object openWindowIndex;
            if (!PhoneApplicationService.Current.State.TryGetValue("openWindowIndex", out openWindowIndex))
            {
                openWindowIndex = 0;
            }

            SerializableWindowState state = App.OpenWindows[(int)openWindowIndex].State;
            if (await ((BibleZtextReader)state.Source).IsCipherKeyGood(this.EnterKeyText.Text))
            {
                ((BibleZtextReader)state.Source).Serial.CipherKey = this.EnterKeyText.Text;
                string filenameComplete = ((BibleZtextReader)state.Source).Serial.Path + "CipherKey.txt";
                await Hoot.File.WriteAllBytes(filenameComplete, Encoding.UTF8.GetBytes(this.EnterKeyText.Text));
                App.OpenWindows[(int)openWindowIndex].ForceReload = true;
                App.OpenWindows[(int)openWindowIndex].UpdateBrowser(false);
                HasFoundGoodKey=true;
                if (NavigationService.CanGoBack)
                {
                    NavigationService.GoBack();
                }
            }
            else
            {
                this.EnterKeyTitle.Text = Translations.Translate("Invalid key. Try again"); ;

            }
        }

        private void ButTtsListen_OnClick(object sender, RoutedEventArgs e)
        {
            _isInThisWindow = false;
            SetBookChoosen();
            object openWindowIndex;
            if (!PhoneApplicationService.Current.State.TryGetValue("openWindowIndex", out openWindowIndex))
            {
                openWindowIndex = 0;
            }

            SerializableWindowState state = App.OpenWindows[(int)openWindowIndex].State;
            string bookNameShort;
            int relChaptNum;
            int verseNum;
            string fullName;
            string titleText;
            state.Source.GetInfo(
                out bookNameShort, out relChaptNum, out verseNum, out fullName, out titleText);
            PhoneApplicationService.Current.State["BookToHear"] = bookNameShort;
            PhoneApplicationService.Current.State["ChapterToHear"] = relChaptNum;
            PhoneApplicationService.Current.State["VerseToHear"] = verseNum;
            PhoneApplicationService.Current.State["ChapterToHearLanguage"] = state.Source.GetLanguage();
            PhoneApplicationService.Current.State["titleBar"] = titleText;
            PhoneApplicationService.Current.State["skipWindowSettings"] = true;
            NavigationService.Navigate(new Uri("/SelectTtsToPlay.xaml", UriKind.Relative));
        }
    }
}