﻿#region Header

// <copyright file="BrowserTitledWindow.xaml.cs" company="Thomas Dilts">
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
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO.IsolatedStorage;
    using System.Linq;
    using System.Threading;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using System.Windows.Threading;

    using CrossConnect.readers;

    using Microsoft.Phone.Controls;
    using Microsoft.Phone.Shell;

    using Sword;
    using Sword.reader;

    public partial class BrowserTitledWindow : ITiledWindow
    {
        #region Fields

        private bool _isInGetHtmlAsynchronously;
        private string _lastFileName = string.Empty;
        private DateTime _lastManipulationKillTime = DateTime.Now;
        private DispatcherTimer _manipulationTimer;
        private ManipulationCompletedEventArgs _manipulationToProcess;
        private SerializableWindowState _state = new SerializableWindowState();

        #endregion Fields

        #region Constructors

        public BrowserTitledWindow()
        {
            InitializeComponent();
        }

        #endregion Constructors

        #region Events

        public event EventHandler HitButtonBigger;

        public event EventHandler HitButtonClose;

        public event EventHandler HitButtonSmaller;

        #endregion Events

        #region Properties

        public bool ForceReload
        {
            get; set;
        }

        public SerializableWindowState State
        {
            get
            {
                return _state;
            }

            set
            {
                _state = value;
            }
        }

        #endregion Properties

        #region Methods

        public static string GetBrowserColor(string sourceResource)
        {
            var color = (Color)Application.Current.Resources[sourceResource];
            switch (sourceResource)
            {
                case "PhoneBackgroundColor":
                    color = App.Themes.MainBackColor;
                    break;
                case "PhoneForegroundColor":
                    color = App.Themes.MainFontColor;
                    break;
                case "PhoneAccentColor":
                    color = App.Themes.AccentColor;
                    break;
            }

            return "#" + color.ToString().Substring(3, 6);
        }

        public void CalculateTitleTextWidth()
        {
            int numButtonsShowing = 0;
            if (butPrevious != null && butPrevious.Visibility == Visibility.Visible)
            {
                numButtonsShowing++;
            }

            if (butNext != null && butNext.Visibility == Visibility.Visible)
            {
                numButtonsShowing++;
            }

            if (butMenu != null && butMenu.Visibility == Visibility.Visible)
            {
                numButtonsShowing++;
            }

            if (butLink != null && butLink.Visibility == Visibility.Visible)
            {
                numButtonsShowing++;
            }

            if (butLarger != null && butLarger.Visibility == Visibility.Visible)
            {
                numButtonsShowing++;
            }

            if (butSmaller != null && butSmaller.Visibility == Visibility.Visible)
            {
                numButtonsShowing++;
            }

            if (butClose != null && butClose.Visibility == Visibility.Visible)
            {
                numButtonsShowing++;
            }

            if (butHear != null && butHear.Visibility == Visibility.Visible)
            {
                numButtonsShowing++;
            }

            if (butTranslate != null && butTranslate.Visibility == Visibility.Visible)
            {
                numButtonsShowing++;
            }

            bool isLandscape = false;
            try
            {
                // this can crash..
                var parent = (MainPageSplit)((Grid)((Grid)((Grid)Parent).Parent).Parent).Parent;
                isLandscape = parent.Orientation == PageOrientation.Landscape
                              || parent.Orientation == PageOrientation.LandscapeLeft
                              || parent.Orientation == PageOrientation.LandscapeRight;

            }
            catch (Exception)
            {

            }
            if (isLandscape)
            {
                title.Width = Application.Current.Host.Content.ActualHeight
                                   - butClose.Width * numButtonsShowing - 15 - 70;
                title.MaxWidth = title.Width;
            }
            else
            {
                title.Width = Application.Current.Host.Content.ActualWidth
                                   - butClose.Width * numButtonsShowing - 15;
                title.MaxWidth = title.Width;
            }
        }

        public void CallbackFromUpdate(string createdFileName)
        {
            Debug.WriteLine("CallbackFromUpdate start");
            _isInGetHtmlAsynchronously = false;
            _lastFileName = createdFileName;

            // webBrowser1.FontSize = state.htmlFontSize;
            webBrowser1.Base = App.WebDirIsolated;

            var source = new Uri(_lastFileName, UriKind.Relative);
            if (_state.Source.IsSynchronizeable || _state.Source.IsLocalChangeDuringLink)
            {
                int bookNum;
                int absoluteChaptNum;
                int relChaptNum;
                int verseNum;
                string fullName;
                string titleText;
                _state.Source.GetInfo(
                    out bookNum, out absoluteChaptNum, out relChaptNum, out verseNum, out fullName, out titleText);
                source = new Uri(
                    _lastFileName + "#CHAP_" + absoluteChaptNum + "_VERS_" + verseNum, UriKind.Relative);
            }

            try
            {
                // this will often crash because the window no longer exists OR has not had the chance to create itself yet.
                webBrowser1.Navigate(source);
            }
            catch (Exception e)
            {
                Debug.WriteLine("CallbackFromUpdate webBrowser1.Navigate crash; " + e.Message);
                return;
            }

            WriteTitle();

            // update the sync button image
            _state.IsSynchronized = !_state.IsSynchronized;
            ButLinkClick(null, null);

            if (_state.Source.IsSynchronizeable || _state.Source.IsLocalChangeDuringLink)
            {
                // The window wont show the correct verse if we dont wait a few seconds before showing it.
                var tmr = new DispatcherTimer { Interval = TimeSpan.FromSeconds(_state.IsResume ? 2.5 : 1.5) };
                _state.IsResume = false;
                tmr.Tick += OnTimerTick;
                tmr.Start();
            }

            Debug.WriteLine("CallbackFromUpdate end");
        }

        public void Initialize(
            string bibleToLoad, string bibleDescription, WindowType windowType, IBrowserTextSource source = null)
        {
            if (string.IsNullOrEmpty(bibleToLoad) && App.InstalledBibles.InstalledBibles.Count() > 0)
            {
                SwordBook book = App.InstalledBibles.InstalledBibles.FirstOrDefault().Value;
                bibleToLoad = book.Sbmd.InternalName;
                bibleDescription = book.Sbmd.Name;
                _state.HtmlFontSize = 10;
            }

            if (windowType == WindowType.WindowDailyPlan && !string.IsNullOrEmpty(App.DailyPlan.PlanBible))
            {
                bibleToLoad = App.DailyPlan.PlanBible;
                bibleDescription = App.DailyPlan.PlanBibleDescription;
                _state.HtmlFontSize = App.DailyPlan.PlanTextSize;
            }

            _state.BibleToLoad = bibleToLoad;
            _state.BibleDescription = bibleDescription;
            _state.WindowType = windowType;
            if (source != null)
            {
                _state.Source = source;
            }
            else
            {
                Dictionary<string, SwordBook> books = windowType == WindowType.WindowCommentary
                                                          ? App.InstalledBibles.InstalledCommentaries
                                                          : App.InstalledBibles.InstalledBibles;
                foreach (var book in books)
                {
                    if (book.Value.Sbmd.InternalName.Equals(bibleToLoad))
                    {
                        string bookPath =
                            book.Value.Sbmd.GetCetProperty(ConfigEntryType.ADataPath).ToString().Substring(2);
                        bool isIsoEncoding = !book.Value.Sbmd.GetCetProperty(ConfigEntryType.Encoding).Equals("UTF-8");
                        try
                        {
                            switch (windowType)
                            {
                                case WindowType.WindowBible:
                                    _state.Source = new BibleZtextReader(
                                        bookPath,
                                        ((Language)book.Value.Sbmd.GetCetProperty(ConfigEntryType.Lang)).Code,
                                        isIsoEncoding);
                                    return;
                                case WindowType.WindowBibleNotes:
                                    _state.Source = new BibleNoteReader(
                                        bookPath,
                                        ((Language)book.Value.Sbmd.GetCetProperty(ConfigEntryType.Lang)).Code,
                                        isIsoEncoding,
                                        Translations.Translate("Notes"));
                                    break;
                                case WindowType.WindowBookmarks:
                                    _state.Source = new BookMarkReader(
                                        bookPath,
                                        ((Language)book.Value.Sbmd.GetCetProperty(ConfigEntryType.Lang)).Code,
                                        isIsoEncoding);
                                    return;
                                case WindowType.WindowHistory:
                                    _state.Source = new HistoryReader(
                                        bookPath,
                                        ((Language)book.Value.Sbmd.GetCetProperty(ConfigEntryType.Lang)).Code,
                                        isIsoEncoding);
                                    return;
                                case WindowType.WindowDailyPlan:
                                    App.DailyPlan.PlanBible = bibleToLoad;
                                    App.DailyPlan.PlanBibleDescription = bibleDescription;
                                    _state.Source = new DailyPlanReader(
                                        bookPath,
                                        ((Language)book.Value.Sbmd.GetCetProperty(ConfigEntryType.Lang)).Code,
                                        isIsoEncoding);
                                    return;
                                case WindowType.WindowCommentary:
                                    _state.Source = new CommentZtextReader(
                                        bookPath,
                                        ((Language)book.Value.Sbmd.GetCetProperty(ConfigEntryType.Lang)).Code,
                                        isIsoEncoding);
                                    return;
                                case WindowType.WindowAddedNotes:
                                    _state.Source = new PersonalNotesReader(
                                        bookPath,
                                        ((Language)book.Value.Sbmd.GetCetProperty(ConfigEntryType.Lang)).Code,
                                        isIsoEncoding);
                                    return;
                                case WindowType.WindowTranslator:
                                    _state.Source = new TranslatorReader(
                                        bookPath,
                                        ((Language)book.Value.Sbmd.GetCetProperty(ConfigEntryType.Lang)).Code,
                                        isIsoEncoding);
                                    return;
                            }
                        }
                        catch (Exception e3)
                        {
                            // should never be an exception here.
                            Debug.WriteLine("crashed. " + e3.StackTrace);
                        }

                        break;
                    }
                }
            }

            if (windowType == WindowType.WindowDailyPlan && App.InstalledBibles.InstalledBibles.Count() > 0)
            {
                KeyValuePair<string, SwordBook> book = App.InstalledBibles.InstalledBibles.FirstOrDefault();
                bibleToLoad = book.Value.Sbmd.InternalName;
                bibleDescription = book.Value.Sbmd.Name;
                App.DailyPlan.PlanBible = bibleToLoad;
                App.DailyPlan.PlanBibleDescription = bibleDescription;
                _state.BibleToLoad = bibleToLoad;
                _state.BibleDescription = bibleDescription;
                _state.HtmlFontSize = App.DailyPlan.PlanTextSize;
                string bookPath = book.Value.Sbmd.GetCetProperty(ConfigEntryType.ADataPath).ToString().Substring(2);
                bool isIsoEncoding = !book.Value.Sbmd.GetCetProperty(ConfigEntryType.Encoding).Equals("UTF-8");
                _state.Source = new DailyPlanReader(
                    bookPath, ((Language)book.Value.Sbmd.GetCetProperty(ConfigEntryType.Lang)).Code, isIsoEncoding);
            }
        }

        public void ShowSizeButtons(bool isShow = true)
        {
            if (!isShow)
            {
                SetButtonVisibility(butLarger, false, string.Empty, string.Empty);
                SetButtonVisibility(butSmaller, false, string.Empty, string.Empty);
                SetButtonVisibility(butClose, false, string.Empty, string.Empty);
            }
            else
            {
                // figure out if this is a light color
                // var color = (Color) Application.Current.Resources["PhoneBackgroundColor"];
                // int lightColorCount = (color.R > 0x80 ? 1 : 0) + (color.G > 0x80 ? 1 : 0) + (color.B > 0x80 ? 1 : 0);
                // string colorDir = lightColorCount >= 2 ? "light" : "dark";
                string colorDir = App.Themes.IsButtonColorDark ? "light" : "dark";

                SetButtonVisibility(
                    butSmaller,
                    _state.NumRowsIown > 1,
                    "/Images/" + colorDir + "/appbar.minus.rest.png",
                    "/Images/" + colorDir + "/appbar.minus.rest.pressed.png");
                SetButtonVisibility(
                    butLarger,
                    true,
                    "/Images/" + colorDir + "/appbar.feature.search.rest.png",
                    "/Images/" + colorDir + "/appbar.feature.search.rest.pressed.png");
                SetButtonVisibility(
                    butClose,
                    true,
                    "/Images/" + colorDir + "/appbar.cancel.rest.png",
                    "/Images/" + colorDir + "/appbar.cancel.rest.pressed.png");
            }

            CalculateTitleTextWidth();
        }

        public void SynchronizeWindow(int chapterNum, int verseNum)
        {
            if (_state.IsSynchronized && _state.Source.IsSynchronizeable)
            {
                _state.Source.MoveChapterVerse(chapterNum, verseNum, false);
                UpdateBrowser(false);
            }
        }

        public void UpdateBrowser(bool isOrientationChangeOnly)
        {
            if (isOrientationChangeOnly)
            {
                return;
            }

            Debug.WriteLine("UpdateBrowser start");
            if (_state.Source != null && Parent != null)
            {
                if (_state.Source.IsExternalLink)
                {
                    try
                    {
                        var source = new Uri(_state.Source.GetExternalLink(App.DisplaySettings));
                        webBrowser1.Base = string.Empty;
                        webBrowser1.Navigate(source);
                        WriteTitle();
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine("UpdateBrowser webBrowser1.Navigate ; " + e.Message);
                    }
                }
                else
                {
                    double fontSizeMultiplier = 1;
                    if (Parent != null && ((Grid)Parent).Parent != null)
                    {
                        var parent = (MainPageSplit)((Grid)((Grid)((Grid)Parent).Parent).Parent).Parent;
                        if (parent.Orientation == PageOrientation.Landscape
                            || parent.Orientation == PageOrientation.LandscapeLeft
                            || parent.Orientation == PageOrientation.LandscapeRight)
                        {
                            // we must adjust the font size for the new orientation. otherwise the font is too big.
                            // fontSizeMultiplier = parent.ActualHeight/parent.ActualWidth;
                        }
                    }

                    string backcolor = GetBrowserColor("PhoneBackgroundColor");
                    string forecolor = GetBrowserColor("PhoneForegroundColor");
                    string accentcolor = GetBrowserColor("PhoneAccentColor");
                    string fontFamily = Theme.FontFamilies[App.Themes.FontFamily];
                    if (App.Themes.IsMainBackImage && !string.IsNullOrEmpty(App.Themes.MainBackImage))
                    {
                        fontFamily += "background-image:url('/images/" + App.Themes.MainBackImage + "');";
                    }

                    GetHtmlAsynchronously(
                        App.DisplaySettings.Clone(),
                        backcolor,
                        forecolor,
                        accentcolor,
                        _state.HtmlFontSize * fontSizeMultiplier,
                        fontFamily,
                        App.WebDirIsolated + "/" + _lastFileName);
                }
            }

            Debug.WriteLine("UpdateBrowser end");
        }

        private static ImageSource GetImage(string path)
        {
            var uri = new Uri(path, UriKind.Relative);
            return new BitmapImage(uri);
        }

        private static void SetButtonVisibility(ImageButton but, bool isVisible, string image, string pressedImage)
        {
            if (isVisible)
            {
                but.Visibility = Visibility.Visible;
                but.Image = GetImage(image);
                but.PressedImage = GetImage(pressedImage);
            }
            else
            {
                but.Visibility = Visibility.Collapsed;
                but.Image = null;
                but.PressedImage = null;
            }
        }

        private static SlideTransition SlideTransitionElement(string mode)
        {
            var slideTransitionMode = (SlideTransitionMode)Enum.Parse(typeof(SlideTransitionMode), mode, false);
            return new SlideTransition { Mode = slideTransitionMode };
        }

        private void Border1ManipulationCompleted(object sender, ManipulationCompletedEventArgs e)
        {
            DoManipulation(e);
        }

        private void ButCloseClick(object sender, RoutedEventArgs e)
        {
            KillManipulation();
            IsolatedStorageFile root = IsolatedStorageFile.GetUserStoreForApplication();

            if (root.FileExists(App.WebDirIsolated + "/" + _lastFileName))
            {
                try
                {
                    // This can easily fail because the background thread is still processing this file!!!
                    root.DeleteFile(App.WebDirIsolated + "/" + _lastFileName);
                }
                catch (Exception ee)
                {
                    Debug.WriteLine("Failed delete file ; " + ee.Message);
                }
            }

            if (HitButtonClose != null)
            {
                HitButtonClose(this, e);
            }
        }

        private void ButCloseManipulationCompleted(object sender, ManipulationCompletedEventArgs e)
        {
            DoManipulation(e);
        }

        private void ButHearClick(object sender, RoutedEventArgs e)
        {
            int bookNum;
            int absoluteChaptNum;
            int relChaptNum;
            int verseNum;
            string fullName;
            string titleText;
            _state.Source.GetInfo(
                out bookNum, out absoluteChaptNum, out relChaptNum, out verseNum, out fullName, out titleText);
            PhoneApplicationService.Current.State["ChapterToHear"] = absoluteChaptNum;
            PhoneApplicationService.Current.State["ChapterToHearLanguage"] = _state.Source.GetLanguage();
            PhoneApplicationService.Current.State["titleBar"] = titleText;
            var parent = (MainPageSplit)((Grid)((Grid)((Grid)Parent).Parent).Parent).Parent;
            parent.NavigationService.Navigate(new Uri("/SelectToPlay.xaml", UriKind.Relative));
        }

        private void ButHearManipulationCompleted(object sender, ManipulationCompletedEventArgs e)
        {
            DoManipulation(e);
        }

        private void ButLargerClick(object sender, RoutedEventArgs e)
        {
            KillManipulation();
            _state.NumRowsIown++;
            ShowSizeButtons();
            if (HitButtonBigger != null)
            {
                HitButtonBigger(this, e);
            }
        }

        private void ButLargerManipulationCompleted(object sender, ManipulationCompletedEventArgs e)
        {
            DoManipulation(e);
        }

        private void ButLinkClick(object sender, RoutedEventArgs e)
        {
            KillManipulation();
            if (_state.Source.IsSynchronizeable)
            {
                // get all the right images
                // figure out if this is a light color
                // var color = (Color) Application.Current.Resources["PhoneBackgroundColor"];
                // int lightColorCount = (color.R > 0x80 ? 1 : 0) + (color.G > 0x80 ? 1 : 0) + (color.B > 0x80 ? 1 : 0);
                // string colorDir = lightColorCount >= 2 ? "light" : "dark";
                string colorDir = App.Themes.IsButtonColorDark ? "light" : "dark";
                _state.IsSynchronized = !_state.IsSynchronized;
                if (_state.IsSynchronized)
                {
                    SetButtonVisibility(
                        butLink,
                        true,
                        "/Images/" + colorDir + "/appbar.linkto.rest.pressed.png",
                        "/Images/" + colorDir + "/appbar.linkto.rest.png");
                }
                else
                {
                    SetButtonVisibility(
                        butLink,
                        true,
                        "/Images/" + colorDir + "/appbar.linkto.rest.png",
                        "/Images/" + colorDir + "/appbar.linkto.rest.pressed.png");
                }
            }
            else
            {
                SetButtonVisibility(butLink, false, string.Empty, string.Empty);
            }
        }

        private void ButLinkManipulationCompleted(object sender, ManipulationCompletedEventArgs e)
        {
            DoManipulation(e);
        }

        private void ButMenuClick(object sender, RoutedEventArgs e)
        {
            KillManipulation();
            ForceReload = true;
            PhoneApplicationService.Current.State["isAddNewWindowOnly"] = false;
            PhoneApplicationService.Current.State["skipWindowSettings"] = false;
            PhoneApplicationService.Current.State["openWindowIndex"] = _state.CurIndex;
            PhoneApplicationService.Current.State["InitializeWindowSettings"] = true;

            var parent = (MainPageSplit)((Grid)((Grid)((Grid)Parent).Parent).Parent).Parent;
            parent.NavigationService.Navigate(new Uri("/WindowSettings.xaml", UriKind.Relative));
        }

        private void ButMenuManipulationCompleted(object sender, ManipulationCompletedEventArgs e)
        {
            DoManipulation(e);
        }

        private void ButNextClick(object sender, RoutedEventArgs e)
        {
            KillManipulation();
            _state.Source.MoveNext();

            string mode = "SlideLeftFadeOut";
            TransitionElement transitionElement = SlideTransitionElement(mode);

            ITransition transition = transitionElement.GetTransition(this);
            transition.Completed += (sender1, e1) =>
                {
                    UpdateBrowser(false);
                    transition.Stop();
                    mode = "SlideLeftFadeIn";
                    transitionElement = null;
                    transitionElement = SlideTransitionElement(mode);
                    transition = transitionElement.GetTransition(this);
                    transition.Completed += delegate { transition.Stop(); };
                    transition.Begin();
                };
            transition.Begin();
        }

        private void ButPreviousClick(object sender, RoutedEventArgs e)
        {
            KillManipulation();
            _state.Source.MovePrevious();

            string mode = "SlideRightFadeOut";
            TransitionElement transitionElement = SlideTransitionElement(mode);

            ITransition transition = transitionElement.GetTransition(this);

            transition.Completed += (sender1, e1) =>
                {
                    UpdateBrowser(false);
                    transition.Stop();
                    mode = "SlideRightFadeIn";
                    transitionElement = null;
                    transitionElement = SlideTransitionElement(mode);
                    transition = transitionElement.GetTransition(this);
                    transition.Completed += delegate { transition.Stop(); };
                    transition.Begin();
                };
            transition.Begin();
        }

        private void ButSmallerClick(object sender, RoutedEventArgs e)
        {
            KillManipulation();
            _state.NumRowsIown--;
            ShowSizeButtons();
            if (HitButtonSmaller != null)
            {
                HitButtonSmaller(this, e);
            }
        }

        private void ButSmallerManipulationCompleted(object sender, ManipulationCompletedEventArgs e)
        {
            DoManipulation(e);
        }

        private void ButTranslateClick(object sender, RoutedEventArgs e)
        {
            KillManipulation();

            string[] toTranslate;
            bool[] isTranslateable;
            _state.Source.GetTranslateableTexts(
                App.DisplaySettings, _state.BibleToLoad, out toTranslate, out isTranslateable);
            var transReader2 = new TranslatorReader(string.Empty, string.Empty, false);
            App.AddWindow(
                _state.BibleToLoad,
                _state.BibleDescription,
                WindowType.WindowTranslator,
                _state.HtmlFontSize,
                transReader2);
            transReader2.TranslateThis(toTranslate, isTranslateable, _state.Source.GetLanguage());
        }

        private void ButTranslateManipulationCompleted(object sender, ManipulationCompletedEventArgs e)
        {
            DoManipulation(e);
        }

        private void DoManipulation(ManipulationCompletedEventArgs e)
        {
            if (_manipulationTimer == null
                && _lastManipulationKillTime.AddMilliseconds(400).CompareTo(DateTime.Now) < 0)
            {
                _manipulationToProcess = e;

                // start timer
                _manipulationTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(200) };
                _manipulationTimer.Tick += DoManipulationTimerTick;
                _manipulationTimer.Start();
            }
        }

        private void DoManipulationTimerTick(object sender, EventArgs e)
        {
            // we must delay updating of this webbrowser...
            KillManipulation();

            Point pt = _manipulationToProcess.FinalVelocities.LinearVelocity;
            if (pt.X > 700)
            {
                // previous
                ButPreviousClick(null, null);
            }
            else if (pt.X < -700)
            {
                // next
                ButNextClick(null, null);
            }
        }

        private void GetHtmlAsynchronously(
            DisplaySettings dispSet,
            string htmlBackgroundColor,
            string htmlForegroundColor,
            string htmlPhoneAccentColor,
            double htmlFontSize,
            string fontFamily,
            string fileErase)
        {
            if (_isInGetHtmlAsynchronously)
            {
                Debug.WriteLine("GetHtmlAsynchronously MULTIPLE ENTRY");
                return;
            }

            _isInGetHtmlAsynchronously = true;
            Debug.WriteLine("GetHtmlAsynchronously");
            ThreadPool.QueueUserWorkItem(
                _ =>
                    {
                        try
                        {
                            string createdFileName = _state.Source.PutHtmlTofile(
                                dispSet,
                                htmlBackgroundColor,
                                htmlForegroundColor,
                                htmlPhoneAccentColor,
                                htmlFontSize,
                                fontFamily,
                                fileErase,
                                App.WebDirIsolated,
                                ForceReload);
                            ForceReload = false;
                            Deployment.Current.Dispatcher.BeginInvoke(() => CallbackFromUpdate(createdFileName));
                        }
                        catch (Exception e)
                        {
                            Debug.WriteLine("GetHtmlAsynchronously Failed; " + e.Message);
                            Deployment.Current.Dispatcher.BeginInvoke(() => CallbackFromUpdate(string.Empty));
                            return;
                        }
                    });
        }

        private void Grid1ManipulationCompleted(object sender, ManipulationCompletedEventArgs e)
        {
            DoManipulation(e);
        }

        private void KillManipulation()
        {
            if (_manipulationTimer != null)
            {
                _manipulationTimer.Stop();
                _manipulationTimer = null;
            }

            _lastManipulationKillTime = DateTime.Now;
        }

        private void OnTimerTick(object sender, EventArgs e)
        {
            Debug.WriteLine("OnTimerTick start");

            // we must delay updating of this webbrowser...
            ((DispatcherTimer)sender).Stop();
            try
            {
                int bookNum;
                int absoluteChaptNum;
                int relChaptNum;
                int verseNum;
                string fullName;
                string titleText;
                _state.Source.GetInfo(
                    out bookNum, out absoluteChaptNum, out relChaptNum, out verseNum, out fullName, out titleText);
                var source = new Uri(
                    _lastFileName + "#CHAP_" + absoluteChaptNum + "_VERS_" + verseNum, UriKind.Relative);
                webBrowser1.Navigate(source);
            }
            catch (Exception ee)
            {
                Debug.WriteLine("OnTimerTick Failed; " + ee.Message);
            }
        }

        private void SourceChanged()
        {
            ForceReload = true;
            UpdateBrowser(false);
        }

        private void TitleManipulationCompleted(object sender, ManipulationCompletedEventArgs e)
        {
            DoManipulation(e);
        }

        private void WebBrowser1Loaded(object sender, RoutedEventArgs e)
        {
            UpdateBrowser(false);

            // get all the right images
            // figure out if this is a light color
            // var color = (Color) Application.Current.Resources["PhoneBackgroundColor"];
            // int lightColorCount = (color.R > 0x80 ? 1 : 0) + (color.G > 0x80 ? 1 : 0) + (color.B > 0x80 ? 1 : 0);
            // string colorDir = lightColorCount >= 2 ? "light" : "dark";
            string colorDir = App.Themes.IsButtonColorDark ? "light" : "dark";

            bool isPrevNext = _state != null && _state.Source != null && _state.Source.IsPageable;
            SetButtonVisibility(
                butPrevious,
                isPrevNext,
                "/Images/" + colorDir + "/appbar.prev.rest.png",
                "/Images/" + colorDir + "/appbar.prev.rest.press.png");
            SetButtonVisibility(
                butNext,
                isPrevNext,
                "/Images/" + colorDir + "/appbar.next.rest.png",
                "/Images/" + colorDir + "/appbar.next.rest.press.png");
            bool isHearable = _state != null && _state.Source != null && _state.Source.IsHearable;
            SetButtonVisibility(
                butHear,
                isHearable,
                "/Images/" + colorDir + "/appbar.speaker.png",
                "/Images/" + colorDir + "/appbar.speaker.pressed.png");
            bool isTranslate = _state != null && _state.Source != null && _state.Source.IsTranslateable
                               && !_state.Source.GetLanguage().Equals(Translations.IsoLanguageCode);
            SetButtonVisibility(
                butTranslate,
                isTranslate,
                "/Images/" + colorDir + "/appbar.translate.png",
                "/Images/" + colorDir + "/appbar.translate.pressed.png");

            butMenu.Image = GetImage("/Images/" + colorDir + "/appbar.menu.rest.png");
            butMenu.PressedImage = GetImage("/Images/" + colorDir + "/appbar.menu.rest.pressed.png");

            butLarger.Image = GetImage("/Images/" + colorDir + "/appbar.feature.search.rest.png");
            butLarger.PressedImage = GetImage("/Images/" + colorDir + "/appbar.feature.search.rest.pressed.png");

            butSmaller.Image = GetImage("/Images/" + colorDir + "/appbar.minus.rest.png");
            butSmaller.PressedImage = GetImage("/Images/" + colorDir + "/appbar.minus.rest.pressed.png");

            butClose.Image = GetImage("/Images/" + colorDir + "/appbar.cancel.rest.png");
            butClose.PressedImage = GetImage("/Images/" + colorDir + "/appbar.cancel.rest.pressed.png");

            if (_state != null && _state.Source != null && !_state.Source.IsSynchronizeable)
            {
                SetButtonVisibility(butLink, false, string.Empty, string.Empty);
            }

            if (_state != null && _state.Source != null)
            {
                _state.Source.RegisterUpdateEvent(SourceChanged);
            }

            border1.BorderBrush = new SolidColorBrush(App.Themes.BorderColor);
            WebBrowserBorder.BorderBrush = border1.BorderBrush;
            grid1.Background = new SolidColorBrush(App.Themes.TitleBackColor);

            title.Foreground = new SolidColorBrush(App.Themes.TitleFontColor);

            CalculateTitleTextWidth();
        }

        private void WebBrowser1ScriptNotify(object sender, NotifyEventArgs e)
        {
            string[] chapterVerse = e.Value.Split("_".ToArray());
            int chapterNum = -1;
            int verseNum = -1;
            for (int i = 0; i < chapterVerse.Length; i += 2)
            {
                switch (chapterVerse[i])
                {
                    case "CHAP":
                        int.TryParse(chapterVerse[i + 1], out chapterNum);
                        break;
                    case "VERS":
                        int.TryParse(chapterVerse[i + 1], out verseNum);
                        break;
                    case "STRONG":
                        if (App.DisplaySettings.UseInternetGreekHebrewDict)
                        {
                            var win = new InternetLinkReader(string.Empty, string.Empty, false);
                            win.ShowLink(chapterVerse[i + 1], chapterVerse[i + 1]);
                            App.AddWindow(
                                _state.BibleToLoad,
                                _state.BibleDescription,
                                WindowType.WindowInternetLink,
                                _state.HtmlFontSize,
                                win);
                        }
                        else
                        {
                            var win = new GreekHebrewDictReader(string.Empty, string.Empty, false);
                            win.ShowLink(chapterVerse[i + 1]);
                            App.AddWindow(
                                _state.BibleToLoad,
                                _state.BibleDescription,
                                WindowType.WindowLexiconLink,
                                _state.HtmlFontSize,
                                win);
                        }

                        return;
                    case "MORPH":
                        string morphology = MorphologyTranslator.ParseRobinson(chapterVerse[i + 1]);
                        MessageBox.Show(morphology);
                        return;
                }
            }

            if (chapterNum >= 0 && verseNum >= 0)
            {
                if (_state.Source.IsLocalChangeDuringLink)
                {
                    _state.Source.MoveChapterVerse(chapterNum, verseNum, true);
                    WriteTitle();
                }

                App.SynchronizeAllWindows(chapterNum, verseNum, _state.CurIndex);

                App.AddHistory(chapterNum, verseNum);
            }
        }

        private void WebBrowser1Unloaded(object sender, RoutedEventArgs e)
        {
            if (_state != null && _state.Source != null)
            {
                _state.Source.RegisterUpdateEvent(SourceChanged, false);
            }
        }

        private void WriteTitle()
        {
            int bookNum;
            int absoluteChaptNum;
            int relChaptNum;
            int verseNum;
            string fullName;
            string titleText;
            _state.Source.GetInfo(
                out bookNum, out absoluteChaptNum, out relChaptNum, out verseNum, out fullName, out titleText);
            title.Text = titleText + " - "
                              +
                              (string.IsNullOrEmpty(_state.BibleDescription)
                                   ? _state.BibleToLoad
                                   : _state.BibleDescription)
                              + "                                                               ";
        }

        #endregion Methods

        #region Other

        // An event that clients can use to be notified whenever the
        // elements of the list change.

        #endregion Other
    }
}