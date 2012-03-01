// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BrowserTitledWindow.xaml.cs" company="">
//   
// </copyright>
// <summary>
//   The browser titled window.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#region Header

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

    /// <summary>
    /// The browser titled window.
    /// </summary>
    public partial class BrowserTitledWindow : ITiledWindow
    {
        #region Constants and Fields

        /// <summary>
        /// The _is in get html asynchronously.
        /// </summary>
        private bool _isInGetHtmlAsynchronously;

        /// <summary>
        /// The _last file name.
        /// </summary>
        private string _lastFileName = string.Empty;

        /// <summary>
        /// The _last manipulation kill time.
        /// </summary>
        private DateTime _lastManipulationKillTime = DateTime.Now;

        /// <summary>
        /// The _manipulation timer.
        /// </summary>
        private DispatcherTimer _manipulationTimer;

        /// <summary>
        /// The _manipulation to process.
        /// </summary>
        private ManipulationCompletedEventArgs _manipulationToProcess;

        /// <summary>
        /// The _state.
        /// </summary>
        private SerializableWindowState _state = new SerializableWindowState();

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="BrowserTitledWindow"/> class.
        /// </summary>
        public BrowserTitledWindow()
        {
            this.InitializeComponent();
        }

        #endregion

        // An event that clients can use to be notified whenever the
        // elements of the list change.
        #region Public Events

        /// <summary>
        /// The hit button bigger.
        /// </summary>
        public event EventHandler HitButtonBigger;

        /// <summary>
        /// The hit button close.
        /// </summary>
        public event EventHandler HitButtonClose;

        /// <summary>
        /// The hit button smaller.
        /// </summary>
        public event EventHandler HitButtonSmaller;

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets a value indicating whether ForceReload.
        /// </summary>
        public bool ForceReload { get; set; }

        /// <summary>
        /// Gets or sets State.
        /// </summary>
        public SerializableWindowState State
        {
            get
            {
                return this._state;
            }

            set
            {
                this._state = value;
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The get browser color.
        /// </summary>
        /// <param name="sourceResource">
        /// The source resource.
        /// </param>
        /// <returns>
        /// The get browser color.
        /// </returns>
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

        /// <summary>
        /// The calculate title text width.
        /// </summary>
        public void CalculateTitleTextWidth()
        {
            int numButtonsShowing = 0;
            if (this.butPrevious != null && this.butPrevious.Visibility == Visibility.Visible)
            {
                numButtonsShowing++;
            }

            if (this.butNext != null && this.butNext.Visibility == Visibility.Visible)
            {
                numButtonsShowing++;
            }

            if (this.butMenu != null && this.butMenu.Visibility == Visibility.Visible)
            {
                numButtonsShowing++;
            }

            if (this.butLink != null && this.butLink.Visibility == Visibility.Visible)
            {
                numButtonsShowing++;
            }

            if (this.butLarger != null && this.butLarger.Visibility == Visibility.Visible)
            {
                numButtonsShowing++;
            }

            if (this.butSmaller != null && this.butSmaller.Visibility == Visibility.Visible)
            {
                numButtonsShowing++;
            }

            if (this.butClose != null && this.butClose.Visibility == Visibility.Visible)
            {
                numButtonsShowing++;
            }

            if (this.butHear != null && this.butHear.Visibility == Visibility.Visible)
            {
                numButtonsShowing++;
            }

            if (this.butTranslate != null && this.butTranslate.Visibility == Visibility.Visible)
            {
                numButtonsShowing++;
            }

            bool isLandscape = false;
            try
            {
                // this can crash..
                var parent = (MainPageSplit)((Grid)((Grid)((Grid)this.Parent).Parent).Parent).Parent;
                isLandscape = parent.Orientation == PageOrientation.Landscape
                              || parent.Orientation == PageOrientation.LandscapeLeft
                              || parent.Orientation == PageOrientation.LandscapeRight;

            }
            catch (Exception)
            {

            }
            if (isLandscape)
            {
                this.title.Width = Application.Current.Host.Content.ActualHeight
                                   - this.butClose.Width * numButtonsShowing - 15 - 70;
                this.title.MaxWidth = this.title.Width;
            }
            else
            {
                this.title.Width = Application.Current.Host.Content.ActualWidth
                                   - this.butClose.Width * numButtonsShowing - 15;
                this.title.MaxWidth = this.title.Width;
            }
        }

        /// <summary>
        /// The callback from update.
        /// </summary>
        /// <param name="createdFileName">
        /// The created file name.
        /// </param>
        public void CallbackFromUpdate(string createdFileName)
        {
            Debug.WriteLine("CallbackFromUpdate start");
            this._isInGetHtmlAsynchronously = false;
            this._lastFileName = createdFileName;

            // webBrowser1.FontSize = state.htmlFontSize;
            this.webBrowser1.Base = App.WebDirIsolated;

            var source = new Uri(this._lastFileName, UriKind.Relative);
            if (this._state.Source.IsSynchronizeable || this._state.Source.IsLocalChangeDuringLink)
            {
                int bookNum;
                int absoluteChaptNum;
                int relChaptNum;
                int verseNum;
                string fullName;
                string titleText;
                this._state.Source.GetInfo(
                    out bookNum, out absoluteChaptNum, out relChaptNum, out verseNum, out fullName, out titleText);
                source = new Uri(
                    this._lastFileName + "#CHAP_" + absoluteChaptNum + "_VERS_" + verseNum, UriKind.Relative);
            }

            try
            {
                // this will often crash because the window no longer exists OR has not had the chance to create itself yet.
                this.webBrowser1.Navigate(source);
            }
            catch (Exception e)
            {
                Debug.WriteLine("CallbackFromUpdate webBrowser1.Navigate crash; " + e.Message);
                return;
            }

            this.WriteTitle();

            // update the sync button image
            this._state.IsSynchronized = !this._state.IsSynchronized;
            this.ButLinkClick(null, null);

            if (this._state.Source.IsSynchronizeable || this._state.Source.IsLocalChangeDuringLink)
            {
                // The window wont show the correct verse if we dont wait a few seconds before showing it.
                var tmr = new DispatcherTimer { Interval = TimeSpan.FromSeconds(this._state.IsResume ? 2.5 : 1.5) };
                this._state.IsResume = false;
                tmr.Tick += this.OnTimerTick;
                tmr.Start();
            }

            Debug.WriteLine("CallbackFromUpdate end");
        }

        /// <summary>
        /// The initialize.
        /// </summary>
        /// <param name="bibleToLoad">
        /// The bible to load.
        /// </param>
        /// <param name="bibleDescription">
        /// The bible description.
        /// </param>
        /// <param name="windowType">
        /// The window type.
        /// </param>
        /// <param name="source">
        /// The source.
        /// </param>
        public void Initialize(
            string bibleToLoad, string bibleDescription, WindowType windowType, IBrowserTextSource source = null)
        {
            if (string.IsNullOrEmpty(bibleToLoad) && App.InstalledBibles.InstalledBibles.Count() > 0)
            {
                SwordBook book = App.InstalledBibles.InstalledBibles.FirstOrDefault().Value;
                bibleToLoad = book.Sbmd.InternalName;
                bibleDescription = book.Sbmd.Name;
                this._state.HtmlFontSize = 10;
            }

            if (windowType == WindowType.WindowDailyPlan && !string.IsNullOrEmpty(App.DailyPlan.PlanBible))
            {
                bibleToLoad = App.DailyPlan.PlanBible;
                bibleDescription = App.DailyPlan.PlanBibleDescription;
                this._state.HtmlFontSize = App.DailyPlan.PlanTextSize;
            }

            this._state.BibleToLoad = bibleToLoad;
            this._state.BibleDescription = bibleDescription;
            this._state.WindowType = windowType;
            if (source != null)
            {
                this._state.Source = source;
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
                                    this._state.Source = new BibleZtextReader(
                                        bookPath, 
                                        ((Language)book.Value.Sbmd.GetCetProperty(ConfigEntryType.Lang)).Code, 
                                        isIsoEncoding);
                                    return;
                                case WindowType.WindowBibleNotes:
                                    this._state.Source = new BibleNoteReader(
                                        bookPath, 
                                        ((Language)book.Value.Sbmd.GetCetProperty(ConfigEntryType.Lang)).Code, 
                                        isIsoEncoding, 
                                        Translations.Translate("Notes"));
                                    break;
                                case WindowType.WindowBookmarks:
                                    this._state.Source = new BookMarkReader(
                                        bookPath, 
                                        ((Language)book.Value.Sbmd.GetCetProperty(ConfigEntryType.Lang)).Code, 
                                        isIsoEncoding);
                                    return;
                                case WindowType.WindowHistory:
                                    this._state.Source = new HistoryReader(
                                        bookPath, 
                                        ((Language)book.Value.Sbmd.GetCetProperty(ConfigEntryType.Lang)).Code, 
                                        isIsoEncoding);
                                    return;
                                case WindowType.WindowDailyPlan:
                                    App.DailyPlan.PlanBible = bibleToLoad;
                                    App.DailyPlan.PlanBibleDescription = bibleDescription;
                                    this._state.Source = new DailyPlanReader(
                                        bookPath, 
                                        ((Language)book.Value.Sbmd.GetCetProperty(ConfigEntryType.Lang)).Code, 
                                        isIsoEncoding);
                                    return;
                                case WindowType.WindowCommentary:
                                    this._state.Source = new CommentZtextReader(
                                        bookPath, 
                                        ((Language)book.Value.Sbmd.GetCetProperty(ConfigEntryType.Lang)).Code, 
                                        isIsoEncoding);
                                    return;
                                case WindowType.WindowAddedNotes:
                                    this._state.Source = new PersonalNotesReader(
                                        bookPath, 
                                        ((Language)book.Value.Sbmd.GetCetProperty(ConfigEntryType.Lang)).Code, 
                                        isIsoEncoding);
                                    return;
                                case WindowType.WindowTranslator:
                                    this._state.Source = new TranslatorReader(
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
                this._state.BibleToLoad = bibleToLoad;
                this._state.BibleDescription = bibleDescription;
                this._state.HtmlFontSize = App.DailyPlan.PlanTextSize;
                string bookPath = book.Value.Sbmd.GetCetProperty(ConfigEntryType.ADataPath).ToString().Substring(2);
                bool isIsoEncoding = !book.Value.Sbmd.GetCetProperty(ConfigEntryType.Encoding).Equals("UTF-8");
                this._state.Source = new DailyPlanReader(
                    bookPath, ((Language)book.Value.Sbmd.GetCetProperty(ConfigEntryType.Lang)).Code, isIsoEncoding);
            }
        }

        /// <summary>
        /// The show size buttons.
        /// </summary>
        /// <param name="isShow">
        /// The is show.
        /// </param>
        public void ShowSizeButtons(bool isShow = true)
        {
            if (!isShow)
            {
                SetButtonVisibility(this.butLarger, false, string.Empty, string.Empty);
                SetButtonVisibility(this.butSmaller, false, string.Empty, string.Empty);
                SetButtonVisibility(this.butClose, false, string.Empty, string.Empty);
            }
            else
            {
                // figure out if this is a light color
                // var color = (Color) Application.Current.Resources["PhoneBackgroundColor"];
                // int lightColorCount = (color.R > 0x80 ? 1 : 0) + (color.G > 0x80 ? 1 : 0) + (color.B > 0x80 ? 1 : 0);
                // string colorDir = lightColorCount >= 2 ? "light" : "dark";
                string colorDir = App.Themes.IsButtonColorDark ? "light" : "dark";

                SetButtonVisibility(
                    this.butSmaller, 
                    this._state.NumRowsIown > 1, 
                    "/Images/" + colorDir + "/appbar.minus.rest.png", 
                    "/Images/" + colorDir + "/appbar.minus.rest.pressed.png");
                SetButtonVisibility(
                    this.butLarger, 
                    true, 
                    "/Images/" + colorDir + "/appbar.feature.search.rest.png", 
                    "/Images/" + colorDir + "/appbar.feature.search.rest.pressed.png");
                SetButtonVisibility(
                    this.butClose, 
                    true, 
                    "/Images/" + colorDir + "/appbar.cancel.rest.png", 
                    "/Images/" + colorDir + "/appbar.cancel.rest.pressed.png");
            }

            this.CalculateTitleTextWidth();
        }

        /// <summary>
        /// The synchronize window.
        /// </summary>
        /// <param name="chapterNum">
        /// The chapter num.
        /// </param>
        /// <param name="verseNum">
        /// The verse num.
        /// </param>
        public void SynchronizeWindow(int chapterNum, int verseNum)
        {
            if (this._state.IsSynchronized && this._state.Source.IsSynchronizeable)
            {
                this._state.Source.MoveChapterVerse(chapterNum, verseNum, false);
                this.UpdateBrowser(false);
            }
        }

        /// <summary>
        /// The update browser.
        /// </summary>
        /// <param name="isOrientationChangeOnly">
        /// The is orientation change only.
        /// </param>
        public void UpdateBrowser(bool isOrientationChangeOnly)
        {
            if (isOrientationChangeOnly)
            {
                return;
            }

            Debug.WriteLine("UpdateBrowser start");
            if (this._state.Source != null && this.Parent != null)
            {
                if (this._state.Source.IsExternalLink)
                {
                    try
                    {
                        var source = new Uri(this._state.Source.GetExternalLink(App.DisplaySettings));
                        this.webBrowser1.Base = string.Empty;
                        this.webBrowser1.Navigate(source);
                        this.WriteTitle();
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine("UpdateBrowser webBrowser1.Navigate ; " + e.Message);
                    }
                }
                else
                {
                    double fontSizeMultiplier = 1;
                    if (this.Parent != null && ((Grid)this.Parent).Parent != null)
                    {
                        var parent = (MainPageSplit)((Grid)((Grid)((Grid)this.Parent).Parent).Parent).Parent;
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

                    this.GetHtmlAsynchronously(
                        App.DisplaySettings.Clone(), 
                        backcolor, 
                        forecolor, 
                        accentcolor, 
                        this._state.HtmlFontSize * fontSizeMultiplier, 
                        fontFamily, 
                        App.WebDirIsolated + "/" + this._lastFileName);
                }
            }

            Debug.WriteLine("UpdateBrowser end");
        }

        #endregion

        #region Methods

        /// <summary>
        /// The get image.
        /// </summary>
        /// <param name="path">
        /// The path.
        /// </param>
        /// <returns>
        /// </returns>
        private static ImageSource GetImage(string path)
        {
            var uri = new Uri(path, UriKind.Relative);
            return new BitmapImage(uri);
        }

        /// <summary>
        /// The set button visibility.
        /// </summary>
        /// <param name="but">
        /// The but.
        /// </param>
        /// <param name="isVisible">
        /// The is visible.
        /// </param>
        /// <param name="image">
        /// The image.
        /// </param>
        /// <param name="pressedImage">
        /// The pressed image.
        /// </param>
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

        /// <summary>
        /// The slide transition element.
        /// </summary>
        /// <param name="mode">
        /// The mode.
        /// </param>
        /// <returns>
        /// </returns>
        private static SlideTransition SlideTransitionElement(string mode)
        {
            var slideTransitionMode = (SlideTransitionMode)Enum.Parse(typeof(SlideTransitionMode), mode, false);
            return new SlideTransition { Mode = slideTransitionMode };
        }

        /// <summary>
        /// The border 1 manipulation completed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void Border1ManipulationCompleted(object sender, ManipulationCompletedEventArgs e)
        {
            this.DoManipulation(e);
        }

        /// <summary>
        /// The but close click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void ButCloseClick(object sender, RoutedEventArgs e)
        {
            this.KillManipulation();
            IsolatedStorageFile root = IsolatedStorageFile.GetUserStoreForApplication();

            if (root.FileExists(App.WebDirIsolated + "/" + this._lastFileName))
            {
                try
                {
                    // This can easily fail because the background thread is still processing this file!!!
                    root.DeleteFile(App.WebDirIsolated + "/" + this._lastFileName);
                }
                catch (Exception ee)
                {
                    Debug.WriteLine("Failed delete file ; " + ee.Message);
                }
            }

            if (this.HitButtonClose != null)
            {
                this.HitButtonClose(this, e);
            }
        }

        /// <summary>
        /// The but close manipulation completed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void ButCloseManipulationCompleted(object sender, ManipulationCompletedEventArgs e)
        {
            this.DoManipulation(e);
        }

        /// <summary>
        /// The but hear click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void ButHearClick(object sender, RoutedEventArgs e)
        {
            int bookNum;
            int absoluteChaptNum;
            int relChaptNum;
            int verseNum;
            string fullName;
            string titleText;
            this._state.Source.GetInfo(
                out bookNum, out absoluteChaptNum, out relChaptNum, out verseNum, out fullName, out titleText);
            PhoneApplicationService.Current.State["ChapterToHear"] = absoluteChaptNum;
            PhoneApplicationService.Current.State["titleBar"] = titleText;
            var parent = (MainPageSplit)((Grid)((Grid)((Grid)this.Parent).Parent).Parent).Parent;
            parent.NavigationService.Navigate(new Uri("/SelectToPlay.xaml", UriKind.Relative));
        }

        /// <summary>
        /// The but hear manipulation completed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void ButHearManipulationCompleted(object sender, ManipulationCompletedEventArgs e)
        {
            this.DoManipulation(e);
        }

        /// <summary>
        /// The but larger click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void ButLargerClick(object sender, RoutedEventArgs e)
        {
            this.KillManipulation();
            this._state.NumRowsIown++;
            this.ShowSizeButtons();
            if (this.HitButtonBigger != null)
            {
                this.HitButtonBigger(this, e);
            }
        }

        /// <summary>
        /// The but larger manipulation completed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void ButLargerManipulationCompleted(object sender, ManipulationCompletedEventArgs e)
        {
            this.DoManipulation(e);
        }

        /// <summary>
        /// The but link click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void ButLinkClick(object sender, RoutedEventArgs e)
        {
            this.KillManipulation();
            if (this._state.Source.IsSynchronizeable)
            {
                // get all the right images
                // figure out if this is a light color
                // var color = (Color) Application.Current.Resources["PhoneBackgroundColor"];
                // int lightColorCount = (color.R > 0x80 ? 1 : 0) + (color.G > 0x80 ? 1 : 0) + (color.B > 0x80 ? 1 : 0);
                // string colorDir = lightColorCount >= 2 ? "light" : "dark";
                string colorDir = App.Themes.IsButtonColorDark ? "light" : "dark";
                this._state.IsSynchronized = !this._state.IsSynchronized;
                if (this._state.IsSynchronized)
                {
                    SetButtonVisibility(
                        this.butLink, 
                        true, 
                        "/Images/" + colorDir + "/appbar.linkto.rest.pressed.png", 
                        "/Images/" + colorDir + "/appbar.linkto.rest.png");
                }
                else
                {
                    SetButtonVisibility(
                        this.butLink, 
                        true, 
                        "/Images/" + colorDir + "/appbar.linkto.rest.png", 
                        "/Images/" + colorDir + "/appbar.linkto.rest.pressed.png");
                }
            }
            else
            {
                SetButtonVisibility(this.butLink, false, string.Empty, string.Empty);
            }
        }

        /// <summary>
        /// The but link manipulation completed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void ButLinkManipulationCompleted(object sender, ManipulationCompletedEventArgs e)
        {
            this.DoManipulation(e);
        }

        /// <summary>
        /// The but menu click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void ButMenuClick(object sender, RoutedEventArgs e)
        {
            this.KillManipulation();
            this.ForceReload = true;
            PhoneApplicationService.Current.State["isAddNewWindowOnly"] = false;
            PhoneApplicationService.Current.State["skipWindowSettings"] = false;
            PhoneApplicationService.Current.State["openWindowIndex"] = this._state.CurIndex;
            PhoneApplicationService.Current.State["InitializeWindowSettings"] = true;

            var parent = (MainPageSplit)((Grid)((Grid)((Grid)this.Parent).Parent).Parent).Parent;
            parent.NavigationService.Navigate(new Uri("/WindowSettings.xaml", UriKind.Relative));
        }

        /// <summary>
        /// The but menu manipulation completed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void ButMenuManipulationCompleted(object sender, ManipulationCompletedEventArgs e)
        {
            this.DoManipulation(e);
        }

        /// <summary>
        /// The but next click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void ButNextClick(object sender, RoutedEventArgs e)
        {
            this.KillManipulation();
            this._state.Source.MoveNext();

            string mode = "SlideLeftFadeOut";
            TransitionElement transitionElement = SlideTransitionElement(mode);

            ITransition transition = transitionElement.GetTransition(this);
            transition.Completed += (sender1, e1) =>
                {
                    this.UpdateBrowser(false);
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

        /// <summary>
        /// The but previous click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void ButPreviousClick(object sender, RoutedEventArgs e)
        {
            this.KillManipulation();
            this._state.Source.MovePrevious();

            string mode = "SlideRightFadeOut";
            TransitionElement transitionElement = SlideTransitionElement(mode);

            ITransition transition = transitionElement.GetTransition(this);

            transition.Completed += (sender1, e1) =>
                {
                    this.UpdateBrowser(false);
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

        /// <summary>
        /// The but smaller click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void ButSmallerClick(object sender, RoutedEventArgs e)
        {
            this.KillManipulation();
            this._state.NumRowsIown--;
            this.ShowSizeButtons();
            if (this.HitButtonSmaller != null)
            {
                this.HitButtonSmaller(this, e);
            }
        }

        /// <summary>
        /// The but smaller manipulation completed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void ButSmallerManipulationCompleted(object sender, ManipulationCompletedEventArgs e)
        {
            this.DoManipulation(e);
        }

        /// <summary>
        /// The but translate click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void ButTranslateClick(object sender, RoutedEventArgs e)
        {
            this.KillManipulation();

            string[] toTranslate;
            bool[] isTranslateable;
            this._state.Source.GetTranslateableTexts(
                App.DisplaySettings, this._state.BibleToLoad, out toTranslate, out isTranslateable);
            var transReader2 = new TranslatorReader(string.Empty, string.Empty, false);
            App.AddWindow(
                this._state.BibleToLoad, 
                this._state.BibleDescription, 
                WindowType.WindowTranslator, 
                this._state.HtmlFontSize, 
                transReader2);
            transReader2.TranslateThis(toTranslate, isTranslateable, this._state.Source.GetLanguage());
        }

        /// <summary>
        /// The but translate manipulation completed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void ButTranslateManipulationCompleted(object sender, ManipulationCompletedEventArgs e)
        {
            this.DoManipulation(e);
        }

        /// <summary>
        /// The do manipulation.
        /// </summary>
        /// <param name="e">
        /// The e.
        /// </param>
        private void DoManipulation(ManipulationCompletedEventArgs e)
        {
            if (this._manipulationTimer == null
                && this._lastManipulationKillTime.AddMilliseconds(400).CompareTo(DateTime.Now) < 0)
            {
                this._manipulationToProcess = e;

                // start timer
                this._manipulationTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(200) };
                this._manipulationTimer.Tick += this.DoManipulationTimerTick;
                this._manipulationTimer.Start();
            }
        }

        /// <summary>
        /// The do manipulation timer tick.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void DoManipulationTimerTick(object sender, EventArgs e)
        {
            // we must delay updating of this webbrowser...
            this.KillManipulation();

            Point pt = this._manipulationToProcess.FinalVelocities.LinearVelocity;
            if (pt.X > 700)
            {
                // previous
                this.ButPreviousClick(null, null);
            }
            else if (pt.X < -700)
            {
                // next
                this.ButNextClick(null, null);
            }
        }

        /// <summary>
        /// The get html asynchronously.
        /// </summary>
        /// <param name="dispSet">
        /// The disp set.
        /// </param>
        /// <param name="htmlBackgroundColor">
        /// The html background color.
        /// </param>
        /// <param name="htmlForegroundColor">
        /// The html foreground color.
        /// </param>
        /// <param name="htmlPhoneAccentColor">
        /// The html phone accent color.
        /// </param>
        /// <param name="htmlFontSize">
        /// The html font size.
        /// </param>
        /// <param name="fontFamily">
        /// The font family.
        /// </param>
        /// <param name="fileErase">
        /// The file erase.
        /// </param>
        private void GetHtmlAsynchronously(
            DisplaySettings dispSet, 
            string htmlBackgroundColor, 
            string htmlForegroundColor, 
            string htmlPhoneAccentColor, 
            double htmlFontSize, 
            string fontFamily, 
            string fileErase)
        {
            if (this._isInGetHtmlAsynchronously)
            {
                Debug.WriteLine("GetHtmlAsynchronously MULTIPLE ENTRY");
                return;
            }

            this._isInGetHtmlAsynchronously = true;
            Debug.WriteLine("GetHtmlAsynchronously");
            ThreadPool.QueueUserWorkItem(
                _ =>
                    {
                        try
                        {
                            string createdFileName = this._state.Source.PutHtmlTofile(
                                dispSet, 
                                htmlBackgroundColor, 
                                htmlForegroundColor, 
                                htmlPhoneAccentColor, 
                                htmlFontSize, 
                                fontFamily, 
                                fileErase, 
                                App.WebDirIsolated, 
                                this.ForceReload);
                            this.ForceReload = false;
                            Deployment.Current.Dispatcher.BeginInvoke(() => this.CallbackFromUpdate(createdFileName));
                        }
                        catch (Exception e)
                        {
                            Debug.WriteLine("GetHtmlAsynchronously Failed; " + e.Message);
                            Deployment.Current.Dispatcher.BeginInvoke(() => this.CallbackFromUpdate(string.Empty));
                            return;
                        }
                    });
        }

        /// <summary>
        /// The grid 1 manipulation completed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void Grid1ManipulationCompleted(object sender, ManipulationCompletedEventArgs e)
        {
            this.DoManipulation(e);
        }

        /// <summary>
        /// The kill manipulation.
        /// </summary>
        private void KillManipulation()
        {
            if (this._manipulationTimer != null)
            {
                this._manipulationTimer.Stop();
                this._manipulationTimer = null;
            }

            this._lastManipulationKillTime = DateTime.Now;
        }

        /// <summary>
        /// The on timer tick.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
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
                this._state.Source.GetInfo(
                    out bookNum, out absoluteChaptNum, out relChaptNum, out verseNum, out fullName, out titleText);
                var source = new Uri(
                    this._lastFileName + "#CHAP_" + absoluteChaptNum + "_VERS_" + verseNum, UriKind.Relative);
                this.webBrowser1.Navigate(source);
            }
            catch (Exception ee)
            {
                Debug.WriteLine("OnTimerTick Failed; " + ee.Message);
            }
        }

        /// <summary>
        /// The source changed.
        /// </summary>
        private void SourceChanged()
        {
            this.ForceReload = true;
            this.UpdateBrowser(false);
        }

        /// <summary>
        /// The title manipulation completed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void TitleManipulationCompleted(object sender, ManipulationCompletedEventArgs e)
        {
            this.DoManipulation(e);
        }

        /// <summary>
        /// The web browser 1 loaded.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void WebBrowser1Loaded(object sender, RoutedEventArgs e)
        {
            this.UpdateBrowser(false);

            // get all the right images
            // figure out if this is a light color
            // var color = (Color) Application.Current.Resources["PhoneBackgroundColor"];
            // int lightColorCount = (color.R > 0x80 ? 1 : 0) + (color.G > 0x80 ? 1 : 0) + (color.B > 0x80 ? 1 : 0);
            // string colorDir = lightColorCount >= 2 ? "light" : "dark";
            string colorDir = App.Themes.IsButtonColorDark ? "light" : "dark";

            bool isPrevNext = this._state != null && this._state.Source != null && this._state.Source.IsPageable;
            SetButtonVisibility(
                this.butPrevious, 
                isPrevNext, 
                "/Images/" + colorDir + "/appbar.prev.rest.png", 
                "/Images/" + colorDir + "/appbar.prev.rest.press.png");
            SetButtonVisibility(
                this.butNext, 
                isPrevNext, 
                "/Images/" + colorDir + "/appbar.next.rest.png", 
                "/Images/" + colorDir + "/appbar.next.rest.press.png");
            bool isHearable = this._state != null && this._state.Source != null && this._state.Source.IsHearable;
            SetButtonVisibility(
                this.butHear, 
                isHearable, 
                "/Images/" + colorDir + "/appbar.speaker.png", 
                "/Images/" + colorDir + "/appbar.speaker.pressed.png");
            bool isTranslate = this._state != null && this._state.Source != null && this._state.Source.IsTranslateable
                               && !this._state.Source.GetLanguage().Equals(Translations.IsoLanguageCode);
            SetButtonVisibility(
                this.butTranslate, 
                isTranslate, 
                "/Images/" + colorDir + "/appbar.translate.png", 
                "/Images/" + colorDir + "/appbar.translate.pressed.png");

            this.butMenu.Image = GetImage("/Images/" + colorDir + "/appbar.menu.rest.png");
            this.butMenu.PressedImage = GetImage("/Images/" + colorDir + "/appbar.menu.rest.pressed.png");

            this.butLarger.Image = GetImage("/Images/" + colorDir + "/appbar.feature.search.rest.png");
            this.butLarger.PressedImage = GetImage("/Images/" + colorDir + "/appbar.feature.search.rest.pressed.png");

            this.butSmaller.Image = GetImage("/Images/" + colorDir + "/appbar.minus.rest.png");
            this.butSmaller.PressedImage = GetImage("/Images/" + colorDir + "/appbar.minus.rest.pressed.png");

            this.butClose.Image = GetImage("/Images/" + colorDir + "/appbar.cancel.rest.png");
            this.butClose.PressedImage = GetImage("/Images/" + colorDir + "/appbar.cancel.rest.pressed.png");

            if (this._state != null && this._state.Source != null && !this._state.Source.IsSynchronizeable)
            {
                SetButtonVisibility(this.butLink, false, string.Empty, string.Empty);
            }

            if (this._state != null && this._state.Source != null)
            {
                this._state.Source.RegisterUpdateEvent(this.SourceChanged);
            }

            this.border1.BorderBrush = new SolidColorBrush(App.Themes.BorderColor);
            this.WebBrowserBorder.BorderBrush = this.border1.BorderBrush;
            this.grid1.Background = new SolidColorBrush(App.Themes.TitleBackColor);

            this.title.Foreground = new SolidColorBrush(App.Themes.TitleFontColor);

            this.CalculateTitleTextWidth();
        }

        /// <summary>
        /// The web browser 1 script notify.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
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
                                this._state.BibleToLoad, 
                                this._state.BibleDescription, 
                                WindowType.WindowInternetLink, 
                                this._state.HtmlFontSize, 
                                win);
                        }
                        else
                        {
                            var win = new GreekHebrewDictReader(string.Empty, string.Empty, false);
                            win.ShowLink(chapterVerse[i + 1]);
                            App.AddWindow(
                                this._state.BibleToLoad, 
                                this._state.BibleDescription, 
                                WindowType.WindowLexiconLink, 
                                this._state.HtmlFontSize, 
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
                if (this._state.Source.IsLocalChangeDuringLink)
                {
                    this._state.Source.MoveChapterVerse(chapterNum, verseNum, true);
                    this.WriteTitle();
                }

                App.SynchronizeAllWindows(chapterNum, verseNum, this._state.CurIndex);

                App.AddHistory(chapterNum, verseNum);
            }
        }

        /// <summary>
        /// The web browser 1 unloaded.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void WebBrowser1Unloaded(object sender, RoutedEventArgs e)
        {
            if (this._state != null && this._state.Source != null)
            {
                this._state.Source.RegisterUpdateEvent(this.SourceChanged, false);
            }
        }

        /// <summary>
        /// The write title.
        /// </summary>
        private void WriteTitle()
        {
            int bookNum;
            int absoluteChaptNum;
            int relChaptNum;
            int verseNum;
            string fullName;
            string titleText;
            this._state.Source.GetInfo(
                out bookNum, out absoluteChaptNum, out relChaptNum, out verseNum, out fullName, out titleText);
            this.title.Text = titleText + " - "
                              +
                              (string.IsNullOrEmpty(this._state.BibleDescription)
                                   ? this._state.BibleToLoad
                                   : this._state.BibleDescription)
                              + "                                                               ";
        }

        #endregion
    }
}