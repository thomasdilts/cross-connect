// <copyright file="BrowserTitledWindow.xaml.cs" company="Thomas Dilts">
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
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Globalization;
    using System.Linq;
    using System.Threading.Tasks;

    using CrossConnect.readers;

    using Sword;
    using Sword.reader;

    using Windows.Devices.Input;
    using Windows.Storage;
    using Windows.UI;
    using Windows.UI.Core;
    using Windows.UI.Popups;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml.Media;
    using Windows.UI.Xaml.Media.Imaging;
    using Windows.UI.Xaml.Navigation;

    public sealed partial class BrowserTitledWindow : ITiledWindow
    {
        #region Fields

        private bool _isInGetHtmlAsynchronously;

        private bool _isUserInterfaceShowing = true;

        private string _lastFileName = string.Empty;

        private string _lastSelectedItem;

        private int _nextVSchroll;

        private int _selectBibleBookSecondSelection;

        private SerializableWindowState _state = new SerializableWindowState();

        #endregion

        #region Constructors and Destructors

        public BrowserTitledWindow()
        {
            this.InitializeComponent();
        }

        #endregion

        #region Public Events

        public event EventHandler HitButtonBigger;

        public event EventHandler HitButtonClose;

        public event EventHandler HitButtonSmaller;

        #endregion

        #region Public Properties

        public bool ForceReload { get; set; }

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

        public static HtmlColorRgba ConvertColorToHtmlRgba(Color color)
        {
            return new LocalHtmlColorRgba { R = color.R, G = color.G, B = color.B, alpha = color.A / 255.0 };
        }
        public class LocalHtmlColorRgba : HtmlColorRgba
        {
            public override string GetHtmlRgba()
            {
                return "rgba(" + this.R + "," + this.G + "," + this.B + ","
                       + Math.Round(this.alpha, 4).ToString(CultureInfo.InvariantCulture) + ")";
            }
        }

        public static HtmlColorRgba GetBrowserColor(string sourceResource)
        {
            switch (sourceResource)
            {
                case "PhoneBackgroundColor":
                    return ConvertColorToHtmlRgba(App.Themes.MainBackColor);
                case "PhoneForegroundColor":
                    return ConvertColorToHtmlRgba(App.Themes.MainFontColor);
                case "PhoneAccentColor":
                    return ConvertColorToHtmlRgba(App.Themes.AccentColor);
                case "PhoneWordsOfChristColor":
                    return ConvertColorToHtmlRgba(App.Themes.WordsOfChristRed);
            }

            return null;
        }
        
        public void CallbackFromUpdate(string createdFile)
        {
            Debug.WriteLine("CallbackFromUpdate start");
            this._isInGetHtmlAsynchronously = false;
            this._lastFileName = createdFile;

            int relChaptNum;
            int verseNum = 0;
            if (this._state.Source.IsSynchronizeable || this._state.Source.IsLocalChangeDuringLink)
            {
                string bookShortName;
                string fullName;
                string titleText;
                this._state.Source.GetInfo(Translations.IsoLanguageCode, 
                    out bookShortName, out relChaptNum, out verseNum, out fullName, out titleText);
            }

            try
            {
                // this will often crash because the window no longer exists OR has not had the chance to create itself yet.
                this.webBrowser1.NavigateToString(createdFile);

/*                if (!_isNextOrPrevious && (this._state.Source.IsSynchronizeable || this._state.Source.IsLocalChangeDuringLink))
                {
                    // The window wont show the correct verse if we dont wait a few seconds before showing it.
//                    var tmr = new DispatcherTimer { Interval = TimeSpan.FromSeconds(_state.IsResume ? 2.5 : 1.5) };
                    var tmr = new DispatcherTimer { Interval = TimeSpan.FromSeconds(_state.IsResume ? 1.5 : 0.5) };
                    _state.IsResume = false;
                    tmr.Tick += OnTimerTick;
                    tmr.Start();
                    //this.Dispatcher.RunAsync(
                    //    CoreDispatcherPriority.Low,
                    //    () => this.Dispatcher.RunAsync(CoreDispatcherPriority.Low, () => this.OnTimerTick(null, null)));
                }*/
            }
            catch (Exception e)
            {
                this._isNextOrPrevious = false;
                Debug.WriteLine("CallbackFromUpdate webBrowser1.Navigate crash; " + e.Message);
                return;
            }

            this.WriteTitle();

            // update the sync button image
            this._state.IsSynchronized = !this._state.IsSynchronized;
            this.ButLinkClick(null, null);

            //if (_state.Source.IsSynchronizeable || _state.Source.IsLocalChangeDuringLink)
            //{
            //    // The window wont show the correct verse if we dont wait a few seconds before showing it.
            //    var tmr = new DispatcherTimer { Interval = TimeSpan.FromSeconds(_state.IsResume ? 2.5 : 1.5) };
            //    _state.IsResume = false;
            //    tmr.Tick += OnTimerTick;
            //    tmr.Start();
            //}
            _isNextOrPrevious = false;
            Debug.WriteLine("CallbackFromUpdate end");
        }
        

        private bool _isNextOrPrevious;

        public void DelayUpdateBrowser()
        {
            var tmr = new DispatcherTimer();
            tmr.Tick += this.OnDelayUpdateTimerTick;
            tmr.Interval = TimeSpan.FromSeconds(1);
            tmr.Start();
        }

        public async Task<int> GetVScroll()
        {
            try
            {
                object pos = await this.webBrowser1.InvokeScriptAsync("getVerticalScrollPosition", new string[0]);
                return int.Parse(pos.ToString());
            }
            catch (Exception exn)
            {
                return 0;
            }
        }

        public async Task Initialize(
            string bibleToLoad, string bibleDescription, WindowType windowType, IBrowserTextSource source = null)
        {
            if (string.IsNullOrEmpty(bibleToLoad) && App.InstalledBibles.InstalledBibles.Count() > 0)
            {
                SwordBookMetaData book = App.InstalledBibles.InstalledBibles.FirstOrDefault().Value;
                bibleToLoad = book.InternalName;
                bibleDescription = book.Name;
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
                Dictionary<string, SwordBookMetaData> books;
                switch (windowType)
                {
                    case WindowType.WindowCommentary:
                        books = App.InstalledBibles.InstalledCommentaries;
                        break;
                    case WindowType.WindowBook:
                        books = App.InstalledBibles.InstalledGeneralBooks;
                        break;
                    default:
                        books = App.InstalledBibles.InstalledBibles;
                        break;
                }
                foreach (var book in books)
                {
                    if (book.Value != null && book.Value.InternalName.Equals(bibleToLoad))
                    {
                        string bookPath =
                            book.Value.GetCetProperty(ConfigEntryType.ADataPath).ToString().Substring(2);
                        bool isIsoEncoding = !book.Value.GetCetProperty(ConfigEntryType.Encoding).Equals("UTF-8");
                        try
                        {
                            switch (windowType)
                            {
                                case WindowType.WindowBible:
                                    this._state.Source = new BibleZtextReader(
                                        bookPath,
                                        ((Language)book.Value.GetCetProperty(ConfigEntryType.Lang)).Code,
                                        isIsoEncoding,
                                        (string)book.Value.GetCetProperty(ConfigEntryType.CipherKey),
                                        book.Value.ConfPath,
                                        (string)book.Value.GetCetProperty(ConfigEntryType.Versification));
                                    await ((BibleZtextReader)this._state.Source).Initialize();
                                    return;
                                case WindowType.WindowBibleNotes:
                                    this._state.Source = new BibleNoteReader(
                                        bookPath,
                                        ((Language)book.Value.GetCetProperty(ConfigEntryType.Lang)).Code,
                                        isIsoEncoding,
                                        Translations.Translate("Notes"),
                                        (string)book.Value.GetCetProperty(ConfigEntryType.CipherKey),
                                        book.Value.ConfPath,
                                        (string)book.Value.GetCetProperty(ConfigEntryType.Versification));
                                    await ((BibleZtextReader)this._state.Source).Initialize();
                                    break;
                                case WindowType.WindowBookmarks:
                                    this._state.Source = new BookMarkReader(
                                        bookPath,
                                        ((Language)book.Value.GetCetProperty(ConfigEntryType.Lang)).Code,
                                        isIsoEncoding,
                                        (string)book.Value.GetCetProperty(ConfigEntryType.CipherKey),
                                        book.Value.ConfPath,
                                        (string)book.Value.GetCetProperty(ConfigEntryType.Versification));
                                    await ((BibleZtextReader)this._state.Source).Initialize();
                                    return;
                                case WindowType.WindowHistory:
                                    this._state.Source = new HistoryReader(
                                        bookPath,
                                        ((Language)book.Value.GetCetProperty(ConfigEntryType.Lang)).Code,
                                        isIsoEncoding,
                                        (string)book.Value.GetCetProperty(ConfigEntryType.CipherKey),
                                        book.Value.ConfPath,
                                        (string)book.Value.GetCetProperty(ConfigEntryType.Versification));
                                    await ((BibleZtextReader)this._state.Source).Initialize();
                                    return;
                                case WindowType.WindowDailyPlan:
                                    App.DailyPlan.PlanBible = bibleToLoad;
                                    App.DailyPlan.PlanBibleDescription = bibleDescription;
                                    this._state.Source = new DailyPlanReader(
                                        bookPath,
                                        ((Language)book.Value.GetCetProperty(ConfigEntryType.Lang)).Code,
                                        isIsoEncoding,
                                        (string)book.Value.GetCetProperty(ConfigEntryType.CipherKey),
                                        book.Value.ConfPath,
                                        (string)book.Value.GetCetProperty(ConfigEntryType.Versification));
                                    await ((BibleZtextReader)this._state.Source).Initialize();
                                    return;
                                case WindowType.WindowCommentary:
                                    this._state.Source = new CommentZtextReader(
                                        bookPath,
                                        ((Language)book.Value.GetCetProperty(ConfigEntryType.Lang)).Code,
                                        isIsoEncoding,
                                        (string)book.Value.GetCetProperty(ConfigEntryType.CipherKey),
                                        book.Value.ConfPath,
                                        (string)book.Value.GetCetProperty(ConfigEntryType.Versification));
                                    await ((BibleZtextReader)this._state.Source).Initialize();
                                    return;
                                case WindowType.WindowBook:
                                    this._state.Source = new RawGenTextReader(
                                        bookPath,
                                        ((Language)book.Value.GetCetProperty(ConfigEntryType.Lang)).Code,
                                        isIsoEncoding);
                                    await ((RawGenTextReader)this._state.Source).Initialize();
                                    return;
                                case WindowType.WindowAddedNotes:
                                    this._state.Source = new PersonalNotesReader(
                                        bookPath,
                                        ((Language)book.Value.GetCetProperty(ConfigEntryType.Lang)).Code,
                                        isIsoEncoding,
                                        (string)book.Value.GetCetProperty(ConfigEntryType.CipherKey),
                                        book.Value.ConfPath,
                                        (string)book.Value.GetCetProperty(ConfigEntryType.Versification));
                                    await ((BibleZtextReader)this._state.Source).Initialize();
                                    return;
                                case WindowType.WindowTranslator:
                                    this._state.Source = new TranslatorReader(
                                        bookPath,
                                        ((Language)book.Value.GetCetProperty(ConfigEntryType.Lang)).Code,
                                        isIsoEncoding,
                                        (string)book.Value.GetCetProperty(ConfigEntryType.CipherKey),
                                        book.Value.ConfPath,
                                        (string)book.Value.GetCetProperty(ConfigEntryType.Versification));
                                    await ((BibleZtextReader)this._state.Source).Initialize();
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
                KeyValuePair<string, SwordBookMetaData> book = App.InstalledBibles.InstalledBibles.FirstOrDefault();
                bibleToLoad = book.Value.InternalName;
                bibleDescription = book.Value.Name;
                App.DailyPlan.PlanBible = bibleToLoad;
                App.DailyPlan.PlanBibleDescription = bibleDescription;
                this._state.BibleToLoad = bibleToLoad;
                this._state.BibleDescription = bibleDescription;
                this._state.HtmlFontSize = App.DailyPlan.PlanTextSize;
                string bookPath = book.Value.GetCetProperty(ConfigEntryType.ADataPath).ToString().Substring(2);
                bool isIsoEncoding = !book.Value.GetCetProperty(ConfigEntryType.Encoding).Equals("UTF-8");
                this._state.Source = new DailyPlanReader(
                    bookPath, ((Language)book.Value.GetCetProperty(ConfigEntryType.Lang)).Code, isIsoEncoding,
                                        (string)book.Value.GetCetProperty(ConfigEntryType.CipherKey), book.Value.ConfPath,
                                        (string)book.Value.GetCetProperty(ConfigEntryType.Versification));
                await ((BibleZtextReader)this._state.Source).Initialize();
            }
        }

        public void SetVScroll(int newVSchrollValue)
        {
            this._nextVSchroll = newVSchrollValue;
        }

        public void ShowSizeButtons(bool isShow = true)
        {
        }

        public void ShowUserInterface(bool isShow)
        {
            this._isUserInterfaceShowing = isShow;
            if (isShow)
            {
                this.webBrowser1.Visibility = Visibility.Visible;
            }
            else
            {
                var b = new WebViewBrush();
                b.SourceName = "webBrowser1";
                b.Redraw();
                this.UserInterfaceBlocker.Fill = b;
                this.webBrowser1.Visibility = Visibility.Collapsed;
            }
        }

        public void SynchronizeWindow(string bookShortName, int chapterNum, int verseNum, IBrowserTextSource source)
        {
            if (!string.IsNullOrEmpty(bookShortName) &&  this._state.IsSynchronized && this._state.Source.IsSynchronizeable)
            {
                string relbookShortName;
                int relChaptNum;
                int relverseNum;
                string fullName;
                string title;
                _state.Source.GetInfo(
                    Translations.IsoLanguageCode, 
                    out relbookShortName,
                    out relChaptNum,
                    out relverseNum,
                    out fullName,
                    out title);
                bool isBookAndChapterTheSame = bookShortName.Equals(relbookShortName) && relChaptNum.Equals(chapterNum);
                this._state.Source.MoveChapterVerse(bookShortName, chapterNum, verseNum, false, source);
                if (!isBookAndChapterTheSame)
                {
                    this.UpdateBrowser(false);
                }
                else 
                {
                    ScrollToThisVerse(bookShortName, chapterNum, verseNum);
                }
            }
        }

        public async void UpdateBrowser(bool isOrientationChangeOnly)
        {
            App.StartTimerForSavingWindows();
            if (isOrientationChangeOnly)
            {
                return;
            }
            this.border1.BorderBrush = new SolidColorBrush(App.Themes.BorderColor);
            this.WebBrowserBorder.BorderBrush = this.border1.BorderBrush;
            this.grid1.Background = new SolidColorBrush(App.Themes.TitleBackColor);
            this.title.Foreground = new SolidColorBrush(App.Themes.TitleFontColor);
            Debug.WriteLine("UpdateBrowser start");
            if (this._state.Source != null && this.Parent != null)
            {
                if (this._state.Source.IsExternalLink)
                {
                    try
                    {
                        var source = new Uri(this._state.Source.GetExternalLink(App.DisplaySettings));
                        //webBrowser1.Base = string.Empty;
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
                        //var parent = (MainPageSplit)((Grid)((Grid)((Grid)Parent).Parent).Parent).Parent;
                        //if (parent.Orientation == PageOrientation.Landscape
                        //    || parent.Orientation == PageOrientation.LandscapeLeft
                        //    || parent.Orientation == PageOrientation.LandscapeRight)
                        //{
                        //    // we must adjust the font size for the new orientation. otherwise the font is too big.
                        //    // fontSizeMultiplier = parent.ActualHeight/parent.ActualWidth;
                        //}
                    }

                    HtmlColorRgba backcolor = GetBrowserColor("PhoneBackgroundColor");
                    HtmlColorRgba forecolor = GetBrowserColor("PhoneForegroundColor");
                    HtmlColorRgba accentcolor = GetBrowserColor("PhoneAccentColor");
                    HtmlColorRgba htmlWordsOfChristColor = GetBrowserColor("PhoneWordsOfChristColor");
                    HtmlColorRgba[] htmlHighlights = new HtmlColorRgba[6];
                    for (int i = 0; i < 6; i++)
                    {
                        htmlHighlights[i] = ConvertColorToHtmlRgba(App.Themes.ColorHighligt[i]);
                    }

                    string fontFamily;
                    if (!Theme.FontFamilies.TryGetValue(App.Themes.FontFamily, out fontFamily))
                    {
                        fontFamily = Theme.FontFamilies.First().Key;
                    }

                    if (App.Themes.IsMainBackImage && !string.IsNullOrEmpty(App.Themes.MainBackImage))
                    {
                        fontFamily += "background-image:url('/images/" + App.Themes.MainBackImage + "');";
                    }

                    try
                    {
                        string createdFileName =
                            await
                            this._state.Source.GetChapterHtml(
                                Translations.IsoLanguageCode,
                                App.DisplaySettings.Clone(),
                                backcolor,
                                forecolor,
                                accentcolor,
                                htmlWordsOfChristColor,
                                htmlHighlights,
                                this._state.HtmlFontSize * fontSizeMultiplier,
                                fontFamily,
                                false,
                                true,
                                this.ForceReload);
                        this.ForceReload = false;
                        this.CallbackFromUpdate(createdFileName);
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine("GetHtmlAsynchronously Failed; " + e.Message);
                        this.CallbackFromUpdate(string.Empty);
                        return;
                    }

                }
            }

            Debug.WriteLine("UpdateBrowser end");

        }
        /*
        public void UpdateBrowser(bool isOrientationChangeOnly)
        {
            App.StartTimerForSavingWindows();
            if (isOrientationChangeOnly)
            {
                return;
            }
            this.border1.BorderBrush = new SolidColorBrush(App.Themes.BorderColor);
            this.WebBrowserBorder.BorderBrush = this.border1.BorderBrush;
            this.grid1.Background = new SolidColorBrush(App.Themes.TitleBackColor);
            this.title.Foreground = new SolidColorBrush(App.Themes.TitleFontColor);
            Debug.WriteLine("UpdateBrowser start");
            if (this._state.Source != null && this.Parent != null)
            {
                if (this._state.Source.IsExternalLink)
                {
                    try
                    {
                        var source = new Uri(this._state.Source.GetExternalLink(App.DisplaySettings));
                        //webBrowser1.Base = string.Empty;
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
                        //var parent = (MainPageSplit)((Grid)((Grid)((Grid)Parent).Parent).Parent).Parent;
                        //if (parent.Orientation == PageOrientation.Landscape
                        //    || parent.Orientation == PageOrientation.LandscapeLeft
                        //    || parent.Orientation == PageOrientation.LandscapeRight)
                        //{
                        //    // we must adjust the font size for the new orientation. otherwise the font is too big.
                        //    // fontSizeMultiplier = parent.ActualHeight/parent.ActualWidth;
                        //}
                    }

                    HtmlColorRgba backcolor = GetBrowserColor("PhoneBackgroundColor");
                    HtmlColorRgba forecolor = GetBrowserColor("PhoneForegroundColor");
                    HtmlColorRgba accentcolor = GetBrowserColor("PhoneAccentColor");
                    HtmlColorRgba htmlWordsOfChristColor = GetBrowserColor("PhoneWordsOfChristColor");
                    HtmlColorRgba[] htmlHighlights = new HtmlColorRgba[6];
                    for (int i = 0; i < 6; i++)
                    {
                        htmlHighlights[i] = ConvertColorToHtmlRgba(App.Themes.ColorHighligt[i]);
                    }

                    string fontFamily;
                    if (!Theme.FontFamilies.TryGetValue(App.Themes.FontFamily, out fontFamily))
                    {
                        fontFamily = Theme.FontFamilies.First().Key;
                    }

                    if (App.Themes.IsMainBackImage && !string.IsNullOrEmpty(App.Themes.MainBackImage))
                    {
                        fontFamily += "background-image:url('/images/" + App.Themes.MainBackImage + "');";
                    }

                    this.GetHtmlAsynchronously(
                        App.DisplaySettings.Clone(),
                        backcolor,
                        forecolor,
                        accentcolor,
                        htmlWordsOfChristColor,
                        htmlHighlights,
                        this._state.HtmlFontSize * fontSizeMultiplier,
                        fontFamily,
                        App.WebDirIsolated + "/" + this._lastFileName);
                }
            }

            Debug.WriteLine("UpdateBrowser end");
        }
        */

        #endregion

        #region Methods

        private static ImageSource GetImage(string path)
        {
            var uri = new Uri(path, UriKind.Relative);
            return new BitmapImage(uri);
        }

        //private static SlideTransition SlideTransitionElement(string mode)
        //{
        //    var slideTransitionMode = (SlideTransitionMode)Enum.Parse(typeof(SlideTransitionMode), mode, false);
        //    return new SlideTransition { Mode = slideTransitionMode };
        //}

        private async void ButCloseClick(object sender, RoutedEventArgs e)
        {
            try
            {
                // This can easily fail because the background thread is still processing this file!!!
                StorageFile file =
                    await
                    ApplicationData.Current.LocalFolder.GetFileAsync(App.WebDirIsolated + "\\" + this._lastFileName);
                await file.DeleteAsync();
            }
            catch (Exception ee)
            {
                Debug.WriteLine("Failed delete file ; " + ee.Message);
            }

            if (this.HitButtonClose != null)
            {
                this.HitButtonClose(this, null);
            }
        }

        private void ButLinkClick(object sender, RoutedEventArgs e)
        {
            this._state.IsSynchronized = !this._state.IsSynchronized;
            bool isShowLink = this._state != null && this._state.Source != null && this._state.Source.IsSynchronizeable;
            this.ButUnLink.Visibility = (this._state != null && !this._state.IsSynchronized && isShowLink)
                                            ? Visibility.Visible
                                            : Visibility.Collapsed;
            this.ButLink.Visibility = (this._state != null && this._state.IsSynchronized && isShowLink)
                                          ? Visibility.Visible
                                          : Visibility.Collapsed;
        }

        private void ButNextClick(object sender, RoutedEventArgs e)
        {
            this._isNextOrPrevious = true;
            this._state.Source.MoveNext(false);
            this.UpdateBrowser(false);
            App.StartTimerForSavingWindows();
        }

        private void ButPreviousClick(object sender, RoutedEventArgs e)
        {
            this._isNextOrPrevious = true;
            this._state.Source.MovePrevious(false);
            this.UpdateBrowser(false);
            App.StartTimerForSavingWindows();
        }

        private void ButSmallerClick(object sender, RoutedEventArgs e)
        {
            this._state.NumRowsIown--;
            this.ShowSizeButtons();
            if (this.HitButtonSmaller != null)
            {
                this.HitButtonSmaller(this, null);
            }
        }

        private async void GetHtmlAsynchronously(
            DisplaySettings dispSet,
            HtmlColorRgba htmlBackgroundColor,
            HtmlColorRgba htmlForegroundColor,
            HtmlColorRgba htmlPhoneAccentColor,
            HtmlColorRgba htmlWordsOfChristColor,
            HtmlColorRgba[] htmlHighlightColor,
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

            try
            {
                string createdFileName =
                    await
                    this._state.Source.GetChapterHtml(
                        Translations.IsoLanguageCode,
                        dispSet,
                        htmlBackgroundColor,
                        htmlForegroundColor,
                        htmlPhoneAccentColor,
                        htmlWordsOfChristColor,
                        htmlHighlightColor,
                        htmlFontSize,
                        fontFamily,
                        false,
                        true,
                        this.ForceReload);
                this.ForceReload = false;
                this.CallbackFromUpdate(createdFileName);
            }
            catch (Exception e)
            {
                Debug.WriteLine("GetHtmlAsynchronously Failed; " + e.Message);
                this.CallbackFromUpdate(string.Empty);
                return;
            }
        }

        private void GetTouchProperties()
        {
            var touchCapabilities = new TouchCapabilities();
            //TouchPresent.Text = touchCapabilities.TouchPresent != 0 ? "Yes" : "No";
            //Contacts.Text = touchCapabilities.Contacts.ToString();
        }

        private void OnDelayUpdateTimerTick(object sender, object e)
        {
            ((DispatcherTimer)sender).Stop();

            this.UpdateBrowser(false);
        }
        /*
        private void OnTimerTick(object sender, object e)
        {
            if (sender != null)
            {
                ((DispatcherTimer)sender).Stop();
            }

            if (this._nextVSchroll > 0)
            {
                this.webBrowser1.InvokeScriptAsync(
                    "setVerticalScrollPosition", new[] { this._nextVSchroll.ToString(CultureInfo.InvariantCulture) });
                this._nextVSchroll = 0;
            }
            else
            {
                string bookShortName;
                int relChaptNum;
                int verseNum;
                string fullName;
                string titleText;
                this._state.Source.GetInfo(
                    out bookShortName, out relChaptNum, out verseNum, out fullName, out titleText);
                try
                {
                    this._lastSelectedItem = bookShortName + "_" + relChaptNum + "_" + verseNum;
                    this.webBrowser1.InvokeScriptAsync(
                        "ScrollToAnchor",
                        new[] { this._lastSelectedItem, ConvertColorToHtmlRgba(App.Themes.AccentColor).GetHtmlRgba() });
                }
                catch (Exception ee)
                {
                    Debug.WriteLine("crashed: " + ee.Message);
                }
            }
        }
        */
        private void SourceChanged()
        {
            this.ForceReload = true;
            this.UpdateBrowser(false);
        }

        private void WebBrowser1Loaded(object sender, RoutedEventArgs e)
        {
            //this.webBrowser1.AllowedScriptNotifyUris = WebView.AnyScriptNotifyUri;

            this.UpdateBrowser(false);

            bool isPrevNext = this._state != null && this._state.Source != null && this._state.Source.IsPageable;
            this.ButPrevious.Visibility = isPrevNext ? Visibility.Visible : Visibility.Collapsed;
            this.ButNext.Visibility = isPrevNext ? Visibility.Visible : Visibility.Collapsed;

            bool isShowLink = this._state != null && this._state.Source != null && this._state.Source.IsSynchronizeable;

            if (this._state != null && this._state.Source != null)
            {
                this._state.Source.RegisterUpdateEvent(this.SourceChanged);
            }

            this.ButUnLink.Visibility = (this._state != null && !this._state.IsSynchronized && isShowLink)
                                            ? Visibility.Visible
                                            : Visibility.Collapsed;
            this.ButLink.Visibility = (this._state != null && this._state.IsSynchronized && isShowLink)
                                          ? Visibility.Visible
                                          : Visibility.Collapsed;
            this.ButSearch.Visibility = (this._state != null && this._state.Source != null
                                         && this.State.Source.IsSearchable)
                                            ? Visibility.Visible
                                            : Visibility.Collapsed;
            this.ButChapters.Visibility = (this._state != null && this._state.Source != null
                                         && this.State.Source.IsPageable)
                                            ? Visibility.Visible
                                            : Visibility.Collapsed;

            this.border1.BorderBrush = new SolidColorBrush(App.Themes.BorderColor);
            this.WebBrowserBorder.BorderBrush = this.border1.BorderBrush;
            this.grid1.Background = new SolidColorBrush(App.Themes.TitleBackColor);

            this.title.Foreground = new SolidColorBrush(App.Themes.TitleFontColor);
        }

        private async void WebBrowser1ScriptNotify(object sender, NotifyEventArgs e)
        {
            string[] chapterVerse = e.Value.Split('_');
            if (e.Value.Contains("STRONG"))
            {
                if (App.DisplaySettings.UseInternetGreekHebrewDict)
                {
                    ITiledWindow foundWin = null;
                    foreach (ITiledWindow win in App.OpenWindows)
                    {
                        if (win.State.WindowType == WindowType.WindowInternetLink)
                        {
                            foundWin = win;
                        }
                    }
                    if (foundWin == null)
                    {
                        var win = new InternetLinkReader(string.Empty, string.Empty, false);
                        win.ShowLink(chapterVerse[1], chapterVerse[1]);
                        await App.AddWindow(
                            this._state.BibleToLoad,
                            this._state.BibleDescription,
                            WindowType.WindowInternetLink,
                            this._state.HtmlFontSize,
                            this.State.Window,
                            win);
                    }
                    else
                    {
                        ((InternetLinkReader)foundWin.State.Source).ShowLink(
                            chapterVerse[1], chapterVerse[1]);
                        foundWin.UpdateBrowser(false);
                    }
                }
                else
                {
                    ITiledWindow foundWin = null;
                    foreach (ITiledWindow win in App.OpenWindows)
                    {
                        if (win.State.WindowType == WindowType.WindowLexiconLink)
                        {
                            foundWin = win;
                        }
                    }
                    if (foundWin == null)
                    {
                        var win = new GreekHebrewDictReader(string.Empty, string.Empty, false);
                        win.ShowLink(chapterVerse[1]);
                        await App.AddWindow(
                            this._state.BibleToLoad,
                            this._state.BibleDescription,
                            WindowType.WindowLexiconLink,
                            this._state.HtmlFontSize,
                            this.State.Window,
                            win);
                    }
                    else
                    {
                        ((GreekHebrewDictReader)foundWin.State.Source).ShowLink(chapterVerse[1]);
                        foundWin.UpdateBrowser(false);
                    }
                }
            }
            else if (e.Value.Contains("MORPH"))
            {
                // todo complete the morphology.
                string morphology = MorphologyTranslator.ParseRobinson(chapterVerse[1]);
                var dialog = new MessageDialog(morphology);
                dialog.ShowAsync();
            }
            else if (chapterVerse.Count()>=3)
            {
                string bookShortName = chapterVerse[0];
                int chapterNum = -1;
                int verseNum = -1;
                int.TryParse(chapterVerse[1], out chapterNum);
                int.TryParse(chapterVerse[2], out verseNum);
                if (!string.IsNullOrEmpty(bookShortName) && chapterNum >= 0 && verseNum >= 0)
                {
                    if (this._state.Source.IsLocalChangeDuringLink)
                    {
                        this._state.Source.MoveChapterVerse(bookShortName, chapterNum, verseNum, true, this._state.Source);
                        this.WriteTitle();
                    }
                    try
                    {
                        string id = bookShortName + "_" + chapterNum + "_" + verseNum;
                        await this.webBrowser1.InvokeScriptAsync(
                            "SetFontColorForElement",
                            new[] { id, ConvertColorToHtmlRgba(App.Themes.AccentColor).GetHtmlRgba() });
                        if (!string.IsNullOrEmpty(this._lastSelectedItem) && !this._lastSelectedItem.Equals(id))
                        {
                            try
                            {
                                await this.webBrowser1.InvokeScriptAsync(
                                    "SetFontColorForElement",
                                    new[]
                                    {
                                        this._lastSelectedItem,
                                        ConvertColorToHtmlRgba(App.Themes.MainFontColor).GetHtmlRgba()
                                    });
                            }
                            catch (Exception)
                            {
                            }
                        }
                        this._lastSelectedItem = id;
                    }
                    catch (Exception)
                    {
                    }
                    App.SynchronizeAllWindows(bookShortName, chapterNum, verseNum, this._state.CurIndex, this._state.Source);

                    App.AddHistory(bookShortName, chapterNum, verseNum);
                }
            }
        }
        private async void ScrollToThisVerse(string bookShortName, int chapterNum, int verseNum)
        {
            try
            {
                string id = bookShortName + "_" + chapterNum + "_" + verseNum;
                await this.webBrowser1.InvokeScriptAsync(
                    "SetFontColorForElement",
                    new[] { id, ConvertColorToHtmlRgba(App.Themes.AccentColor).GetHtmlRgba() });
                if (!string.IsNullOrEmpty(this._lastSelectedItem) && !this._lastSelectedItem.Equals(id))
                {
                    try
                    {
                        await this.webBrowser1.InvokeScriptAsync(
                            "SetFontColorForElement",
                            new[]
                            {
                                this._lastSelectedItem,
                                ConvertColorToHtmlRgba(App.Themes.MainFontColor).GetHtmlRgba()
                            });
                    }
                    catch (Exception)
                    {
                    }
                }
                this._lastSelectedItem = id;
                await this.webBrowser1.InvokeScriptAsync(
                     "ShowNode",
                    new[] { this._lastSelectedItem });
                this.WriteTitle();
            }
            catch (Exception)
            {
            }

        }

        private void WebBrowser1Unloaded(object sender, RoutedEventArgs e)
        {
            if (this._state != null && this._state.Source != null)
            {
                this._state.Source.RegisterUpdateEvent(this.SourceChanged, false);
            }
        }

        private void WebBrowser1_OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            this.gridPanelButtons1.Width = this.webBrowser1.ActualWidth;
        }

        private void WriteTitle()
        {
            string bookNum;
            int relChaptNum;
            int verseNum;
            string fullName;
            string titleText;
            this._state.Source.GetInfo(Translations.IsoLanguageCode, 
                out bookNum, out relChaptNum, out verseNum, out fullName, out titleText);
            this.title.Text = titleText + " - "
                              + (string.IsNullOrEmpty(this._state.BibleDescription)
                                     ? this._state.BibleToLoad
                                     : this._state.BibleDescription)
                              + "                                                               ";
        }

        private void webBrowser1_NavigationFailed_1(object sender, WebViewNavigationFailedEventArgs e)
        {
            Debug.WriteLine("Failed Navigation");
        }

        #endregion

        private void WebBrowser1_OnLoadCompleted(object sender, NavigationEventArgs e)
        {
            Debug.WriteLine("WebBrowser1_OnLoadCompleted " + e.SourcePageType);
        }

        private void webBrowser1_NavigationCompleted(WebView sender, WebViewNavigationCompletedEventArgs args)
        {
            string bookShortName;
            int relChaptNum;
            int verseNum;
            string fullName;
            string titleText;
            this._state.Source.GetInfo(Translations.IsoLanguageCode, 
                out bookShortName, out relChaptNum, out verseNum, out fullName, out titleText);
            ScrollToThisVerse(bookShortName, relChaptNum, verseNum);
        }
    }
}