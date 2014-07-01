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
    using System.Globalization;
    using System.IO.IsolatedStorage;
    using System.Linq;
    using System.Threading;

    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using System.Windows.Navigation;
    using System.Windows.Threading;

    using CrossConnect.readers;

    using Microsoft.Phone.Controls;
    using Microsoft.Phone.Shell;

    using Sword;
    using Sword.reader;
    using System.Threading.Tasks;
    using System.IO;
    using Windows.Storage;

    public partial class BrowserTitledWindow : ITiledWindow
    {
        #region Fields

        private string _lastSelectedItem;
        private bool _isInGetHtmlAsynchronously;
        private int _nextVSchroll;
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

        public static HtmlColorRgba ConvertColorToHtmlRgba(Color color)
        {
            return new LocalHtmlColorRgba { R = color.R, G = color.G, B = color.B, alpha = color.A / 255.0 };
        }
        public class LocalHtmlColorRgba : HtmlColorRgba
        {
            public override string GetHtmlRgba()
            {
                return string.Format("#{0:x2}{1:x2}{2:x2}", this.R, this.G, this.B);
            }
        }

        public static HtmlColorRgba GetBrowserColor(string sourceResource)
        {
            //var color = (Color)Application.Current.Resources[sourceResource];
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

        public void SetVScroll(int newVSchrollValue)
        {
            _nextVSchroll = newVSchrollValue;
        }

        public int GetVScroll()
        {
            try
            {
                object pos = this.webBrowser1.InvokeScript("getVerticalScrollPosition");
                return int.Parse(pos.ToString());
            }
            catch (Exception exn)
            {
                return 0;
            }
        }

        public void CalculateTitleTextWidth()
        {
            try
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

                if (butSearch != null && butSearch.Visibility == Visibility.Visible)
                {
                    numButtonsShowing++;
                }

                if (butSelectBook != null && butSelectBook.Visibility == Visibility.Visible)
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
                    title.Width = Application.Current.Host.Content.ActualHeight - butClose.Width * numButtonsShowing
                                  - 15 - 70;
                    title.MaxWidth = title.Width;
                }
                else
                {
                    title.Width = Application.Current.Host.Content.ActualWidth - butClose.Width * numButtonsShowing - 15;
                    title.MaxWidth = title.Width;
                }
            }
            catch(Exception eee)
            {
                title.Width = 100;
                title.MaxWidth = 100;
                Debug.WriteLine(eee);
            }
        }

        public async void CallbackFromUpdate(string createdFile)
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
                if (App.Themes.IsMainBackImage && !string.IsNullOrEmpty(App.Themes.MainBackImage))
                {
                    webBrowser1.Base = App.WebDirIsolated;
                    var filename = await PutToFile(createdFile);
                    var source = new Uri(filename, UriKind.Relative);
                    this.webBrowser1.Navigate(source);
                }
                else
                {
                    // this will often crash because the window no longer exists OR has not had the chance to create itself yet.
                    this.webBrowser1.NavigateToString(createdFile);
                }
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

            _isNextOrPrevious = false;
            Debug.WriteLine("CallbackFromUpdate end");
        }

        private string _fileErase=string.Empty;
        private async Task<string> PutToFile(string fileContent)
        {

            ApplicationData appData = ApplicationData.Current;
            StorageFolder folder = await appData.LocalFolder.GetFolderAsync(App.WebDirIsolated.Replace("/", "\\"));

            // Find a new file name.
            // Must change the file name, otherwise the browser may or may not update.
            string fileCreate = "web" + (int)(new Random().NextDouble() * 10000) + ".html";
            IReadOnlyList<StorageFile> files = null;
            try
            {
                files = await folder.CreateFileQuery().GetFilesAsync();

                // the name must be unique of course
                while (files.Any(p => p.Name.Equals(fileCreate)))
                {
                    fileCreate = "web" + (int)(new Random().NextDouble() * 10000) + ".html";
                }
            }
            catch (Exception)
            {
            }

            // delete the old file
            string fileToErase = Path.GetFileName(_fileErase);
            if (files != null && files.Any(p => p.Name.Equals(fileToErase)))
            {
                try
                {
                    StorageFile fileErasing = await folder.GetFileAsync(fileToErase);
                    await fileErasing.DeleteAsync();
                }
                catch (Exception ee)
                {
                    // should never crash here but I have noticed any file delete is a risky business when you have more then one thread.
                    Debug.WriteLine("BibleZtextReader.putHtmlTofile; " + ee.Message);
                }
            }

            StorageFile file = await folder.CreateFileAsync(fileCreate);
            Stream fs = await file.OpenStreamForWriteAsync();
            var tw = new StreamWriter(fs);

            tw.Write(fileContent);
            tw.Flush();
            tw.Dispose();
            fs.Dispose();

            Debug.WriteLine("putHtmlTofile end");
            _fileErase = fileCreate;
            return fileCreate;
        }

        private bool _isNextOrPrevious;


        private DispatcherTimer updateHighlightTimer;

        public async Task<bool> Initialize(
            string bibleToLoad, string bibleDescription, WindowType windowType, object initialData, IBrowserTextSource source = null)
        {
            if (string.IsNullOrEmpty(bibleToLoad) && App.InstalledBibles.InstalledBibles.Any())
            {
                SwordBookMetaData book = App.InstalledBibles.InstalledBibles.FirstOrDefault().Value;
                bibleToLoad = book.InternalName;
                bibleDescription = book.Name;
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
                    if (book.Value.InternalName.Equals(bibleToLoad))
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
                                    return true;
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
                                    
                                    var bookmarkreader = new BiblePlaceMarkReader(
                                        bookPath,
                                        ((Language)book.Value.GetCetProperty(ConfigEntryType.Lang)).Code,
                                        isIsoEncoding,
                                        (string)book.Value.GetCetProperty(ConfigEntryType.CipherKey),
                                        book.Value.ConfPath,
                                        (string)book.Value.GetCetProperty(ConfigEntryType.Versification),
                                        App.PlaceMarkers.Bookmarks,
                                        Translations.Translate("Bookmarks"),
                                        true);
                                    App.BookMarksChanged += bookmarkreader.BiblePlaceMarkSourceChanged;
                                    this._state.Source = bookmarkreader;
                                    await ((BibleZtextReader)this._state.Source).Initialize();
                                    return true;
                                case WindowType.WindowHistory:
                                    var history = new BiblePlaceMarkReader(
                                        bookPath,
                                        ((Language)book.Value.GetCetProperty(ConfigEntryType.Lang)).Code,
                                        isIsoEncoding,
                                        (string)book.Value.GetCetProperty(ConfigEntryType.CipherKey),
                                        book.Value.ConfPath,
                                        (string)book.Value.GetCetProperty(ConfigEntryType.Versification),
                                        App.PlaceMarkers.History,
                                        Translations.Translate("History"),
                                        true);
                                    App.HistoryChanged += history.BiblePlaceMarkSourceChanged;
                                    this._state.Source = history;
                                    await ((BibleZtextReader)this._state.Source).Initialize();
                                    return true;
                                case WindowType.WindowSelectedVerses:
                                    this._state.Source = new BiblePlaceMarkReader(
                                        bookPath,
                                        ((Language)book.Value.GetCetProperty(ConfigEntryType.Lang)).Code,
                                        isIsoEncoding,
                                        (string)book.Value.GetCetProperty(ConfigEntryType.CipherKey),
                                        book.Value.ConfPath,
                                        (string)book.Value.GetCetProperty(ConfigEntryType.Versification),
                                        (List<BiblePlaceMarker>)initialData,
                                        Translations.Translate("Selected verses"),
                                        false);
                                    await ((BibleZtextReader)this._state.Source).Initialize();
                                    return true;
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
                                    return true;
                                case WindowType.WindowCommentary:
                                    this._state.Source = new CommentZtextReader(
                                        bookPath,
                                        ((Language)book.Value.GetCetProperty(ConfigEntryType.Lang)).Code,
                                        isIsoEncoding,
                                        (string)book.Value.GetCetProperty(ConfigEntryType.CipherKey),
                                        book.Value.ConfPath,
                                        (string)book.Value.GetCetProperty(ConfigEntryType.Versification));
                                    await ((BibleZtextReader)this._state.Source).Initialize();
                                    return true;
                                case WindowType.WindowBook:
                                    this._state.Source = new RawGenTextReader(
                                        bookPath,
                                        ((Language)book.Value.GetCetProperty(ConfigEntryType.Lang)).Code,
                                        isIsoEncoding);
                                    await ((RawGenTextReader)this._state.Source).Initialize();
                                    return true;
                                case WindowType.WindowAddedNotes:
                                    var notes = new PersonalNotesReader(
                                        bookPath,
                                        ((Language)book.Value.GetCetProperty(ConfigEntryType.Lang)).Code,
                                        isIsoEncoding,
                                        (string)book.Value.GetCetProperty(ConfigEntryType.CipherKey),
                                        book.Value.ConfPath,
                                        (string)book.Value.GetCetProperty(ConfigEntryType.Versification),
                                        App.DailyPlan.PersonalNotesVersified,
                                        Translations.Translate("Added notes"),
                                        App.DisplaySettings);
                                    App.PersonalNotesChanged += notes.NotesSourceChanged;
                                    this._state.Source = notes;
                                    await ((BibleZtextReader)this._state.Source).Initialize();
                                    return true;
                                case WindowType.WindowTranslator:
                                    this._state.Source = new TranslatorReader(
                                        bookPath,
                                        ((Language)book.Value.GetCetProperty(ConfigEntryType.Lang)).Code,
                                        isIsoEncoding,
                                        (string)book.Value.GetCetProperty(ConfigEntryType.CipherKey),
                                        book.Value.ConfPath,
                                        (string)book.Value.GetCetProperty(ConfigEntryType.Versification));
                                    await ((BibleZtextReader)this._state.Source).Initialize();
                                    return true;
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
                _state.BibleToLoad = bibleToLoad;
                _state.BibleDescription = bibleDescription;
                _state.HtmlFontSize = App.DailyPlan.PlanTextSize;
                string bookPath = book.Value.GetCetProperty(ConfigEntryType.ADataPath).ToString().Substring(2);
                bool isIsoEncoding = !book.Value.GetCetProperty(ConfigEntryType.Encoding).Equals("UTF-8");
                this._state.Source = new DailyPlanReader(
                    bookPath, ((Language)book.Value.GetCetProperty(ConfigEntryType.Lang)).Code, isIsoEncoding,
                                        (string)book.Value.GetCetProperty(ConfigEntryType.CipherKey), book.Value.ConfPath,
                                        (string)book.Value.GetCetProperty(ConfigEntryType.Versification));
                await ((BibleZtextReader)this._state.Source).Initialize();
            }
            return true;
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
                    "/Images/" + colorDir + "/window.smaller.png",
                    "/Images/" + colorDir + "/window.smaller.pressed.png");
                SetButtonVisibility(
                    butLarger,
                    true,
                    "/Images/" + colorDir + "/window.bigger.png",
                    "/Images/" + colorDir + "/window.bigger.pressed.png");
                SetButtonVisibility(
                    butClose,
                    true,
                    "/Images/" + colorDir + "/appbar.cancel.rest.png",
                    "/Images/" + colorDir + "/appbar.cancel.rest.pressed.png");
            }

            CalculateTitleTextWidth();
        }

        public void SynchronizeWindow(string bookShortName, int chapterNum, int verseNum, IBrowserTextSource source)
        {
            if (!string.IsNullOrEmpty(bookShortName) && this._state.IsSynchronized && this._state.Source.IsSynchronizeable)
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

                    string fontFamily = string.Empty;
                    if (!string.IsNullOrEmpty(this._state.Font))
                    {
                        Theme.FontFamilies.TryGetValue(this._state.Font, out fontFamily);
                    }
                    if (string.IsNullOrEmpty(fontFamily) && !Theme.FontFamilies.TryGetValue(App.Themes.FontFamily, out fontFamily))
                    {
                        fontFamily = Theme.FontFamilies.First().Key;
                    }

                    if (App.Themes.IsMainBackImage && !string.IsNullOrEmpty(App.Themes.MainBackImage))
                    {
                        fontFamily += "background-image:url('images/" + App.Themes.MainBackImage + "');";
                    }

                    try
                    {
                        string createdFileName =
                            await this._state.Source.GetChapterHtml(
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

            App.StartTimerForSavingWindows();
            Debug.WriteLine("UpdateBrowser end");
        }

        public static ImageSource GetImage(string path)
        {
            var uri = new Uri(path, UriKind.Relative);
            return new BitmapImage(uri);
        }

        public static void SetButtonVisibility(ImageButton but, bool isVisible, string image, string pressedImage)
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

            if (!string.IsNullOrEmpty(_fileErase) && root.FileExists(App.WebDirIsolated + "/" + _fileErase))
            {
                try
                {
                    // This can easily fail because the background thread is still processing this file!!!
                    root.DeleteFile(App.WebDirIsolated + "/" + _fileErase);
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
            string bookNameShort;
            int relChaptNum;
            int verseNum;
            string fullName;
            string titleText;
            _state.Source.GetInfo(Translations.IsoLanguageCode,
                out bookNameShort, out relChaptNum, out verseNum, out fullName, out titleText);
            PhoneApplicationService.Current.State["BookToHear"] = bookNameShort;
            PhoneApplicationService.Current.State["ChapterToHear"] = relChaptNum;
            PhoneApplicationService.Current.State["VerseToHear"] = verseNum;
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
            PhoneApplicationService.Current.State["MoveOpenWindow"] = true;

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
            _state.Source.MoveNext(false);

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
            _state.Source.MovePrevious(false);

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

        private async void ButTranslateClick(object sender, RoutedEventArgs e)
        {
            KillManipulation();


            var obj = await _state.Source.GetTranslateableTexts(Translations.IsoLanguageCode,
                App.DisplaySettings, _state.BibleToLoad);
            var toTranslate = (string[])obj[0];
            var isTranslateable = (bool[])obj[1];
            var serial = ((BibleZtextReader)_state.Source).Serial;
            var transReader2 = new TranslatorReader(serial.Path,serial.Iso2DigitLangCode,serial.IsIsoEncoding,serial.CipherKey,serial.ConfigPath,serial.Versification);
            App.AddWindow(
                _state.BibleToLoad,
                _state.BibleDescription,
                WindowType.WindowTranslator,
                _state.HtmlFontSize,
                _state.Font,
                null,
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

        private void SourceChanged()
        {
            ForceReload = true;
            UpdateBrowser(false);
        }

        private void TitleManipulationCompleted(object sender, ManipulationCompletedEventArgs e)
        {
            DoManipulation(e);
        }

        public async Task<string[]> GetLast3SecondsChosenVerses()
        {
            string textsWithTitles = string.Empty;
            string titlesOnly = string.Empty;

            DateTime? firstFound = null;
            var foundVerses = new List<BiblePlaceMarker>();
            for (int j = App.PlaceMarkers.History.Count - 1; j >= 0; j--)
            {
                BiblePlaceMarker place = App.PlaceMarkers.History[j];
                if (firstFound == null)
                {
                    firstFound = place.When;
                    foundVerses.Add(place);
                }
                else if (firstFound.Value.AddSeconds(-3).CompareTo(place.When) < 0)
                {
                    foundVerses.Add(place);
                }
                else
                {
                    // we found all the verses, get out.
                    break;
                }
            }

            // they are in reverse order again,
            for (int j = foundVerses.Count - 1; j >= 0; j--)
            {
                BiblePlaceMarker place = foundVerses[j];
                string fullName;
                string titleText;
                ((BibleZtextReader)this.State.Source).GetInfo(Translations.IsoLanguageCode, place.BookShortName,
                    place.ChapterNum, place.VerseNum, out fullName, out titleText);
                string title = fullName + " " + (place.ChapterNum + 1) + ":" + (place.VerseNum + 1) + " - "
                               + this.State.BibleToLoad;
                string verseText =
                    await this.State.Source.GetVerseTextOnly(App.DisplaySettings, place.BookShortName, place.ChapterNum, place.VerseNum);

                if (!string.IsNullOrEmpty(titlesOnly))
                {
                    textsWithTitles += "\n";
                    titlesOnly += ", ";
                }

                titlesOnly += title;
                textsWithTitles +=
                    verseText.Replace("<p>", string.Empty)
                             .Replace("</p>", string.Empty)
                             .Replace("<br />", string.Empty)
                             .Replace("\n", " ") + "\n-" + title;
                if (App.DailyPlan.PersonalNotesVersified.ContainsKey(place.BookShortName)
                    && App.DailyPlan.PersonalNotesVersified[place.BookShortName].ContainsKey(place.ChapterNum)
                    && App.DailyPlan.PersonalNotesVersified[place.BookShortName][place.ChapterNum].ContainsKey(place.VerseNum))
                {
                    textsWithTitles += "\n" + Translations.Translate("Added notes") + "\n"
                                       + App.DailyPlan.PersonalNotesVersified[place.BookShortName][place.ChapterNum][place.VerseNum].Note;
                }
            }
            return new[] { textsWithTitles, titlesOnly };
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
            bool IsSearchable = _state != null && _state.Source != null && _state.Source.IsSearchable;
            SetButtonVisibility(
                butSearch,
                IsSearchable,
                "/Images/" + colorDir + "/appbar.feature.search.rest.png",
                "/Images/" + colorDir + "/appbar.feature.search.rest.pressed.png");
            bool IsPageable = _state != null && _state.Source != null && _state.Source.IsPageable;
                               
            SetButtonVisibility(
                butSelectBook,
                IsPageable,
                "/Images/" + colorDir + "/open.folder.png",
                "/Images/" + colorDir + "/open.folder.pressed.png");

            butMenu.Image = GetImage("/Images/" + colorDir + "/appbar.menu.rest.png");
            butMenu.PressedImage = GetImage("/Images/" + colorDir + "/appbar.menu.rest.pressed.png");

            butLarger.Image = GetImage("/Images/" + colorDir + "/window.bigger.png");
            butLarger.PressedImage = GetImage("/Images/" + colorDir + "/window.bigger.pressed.png");

            butSmaller.Image = GetImage("/Images/" + colorDir + "/window.smaller.png");
            butSmaller.PressedImage = GetImage("/Images/" + colorDir + "/window.smaller.pressed.png");

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
                        App.AddWindow(
                            this._state.BibleToLoad,
                            this._state.BibleDescription,
                            WindowType.WindowInternetLink,
                            this._state.HtmlFontSize,
                            this._state.Font,
                            null,
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
                        App.AddWindow(
                            this._state.BibleToLoad,
                            this._state.BibleDescription,
                            WindowType.WindowLexiconLink,
                            this._state.HtmlFontSize,
                            this._state.Font,
                            null,
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
                MessageBox.Show(morphology);
                return;
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
                        this.webBrowser1.InvokeScript(
                            "SetFontColorForElement",
                            new[] { id, ConvertColorToHtmlRgba(App.Themes.AccentColor).GetHtmlRgba() });
                        if (!string.IsNullOrEmpty(this._lastSelectedItem) && !this._lastSelectedItem.Equals(id))
                        {
                            try
                            {
                                this.webBrowser1.InvokeScript(
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

        private void WebBrowser1Unloaded(object sender, RoutedEventArgs e)
        {
            if (_state != null && _state.Source != null)
            {
                _state.Source.RegisterUpdateEvent(SourceChanged, false);
            }
        }

        private void WriteTitle()
        {
            string bookNameshort;
            int relChaptNum;
            int verseNum;
            string fullName;
            string titleText;
            _state.Source.GetInfo(Translations.IsoLanguageCode,
                out bookNameshort, out relChaptNum, out verseNum, out fullName, out titleText);
            var entireTitleText = titleText + " - "
                                  +
                                  (string.IsNullOrEmpty(_state.BibleDescription)
                                       ? _state.BibleToLoad
                                       : _state.BibleDescription)
                                  + "                                                               ";
            if (App.DisplaySettings.Show2titleRows)
            {
                title2.Text = entireTitleText;
                title2.Visibility = Visibility.Visible;
                title.Text = "                                                               ";
                grid1.RowDefinitions[1].Height = grid1.RowDefinitions[0].Height;
            }
            else
            {
                title.Text = entireTitleText;
                title2.Visibility = Visibility.Collapsed;
                grid1.RowDefinitions[1].Height = new System.Windows.GridLength(0);
            }
        }

        #endregion Methods

        private void WebBrowser1_OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            //Debug.WriteLine("OnNavigationFailed " + e.Exception.Message);
            e.Handled = true;
            Deployment.Current.Dispatcher.BeginInvoke(() => CallbackFromUpdate(_lastFileName));
        }
        public void SelectedVerseEvent(string bookName, int moveToChapter, int verse)
        {
            this._state.Source.MoveChapterVerse(bookName, moveToChapter, verse, false, this._state.Source);
        }
        private void ButSelectBook_OnClick(object sender, RoutedEventArgs e)
        {
            KillManipulation();
            PhoneApplicationService.Current.State["openWindowIndex"] = _state.CurIndex;
            PhoneApplicationService.Current.State["openWindowSource"] = _state.Source;
            SelectBibleBook.SelectedEvent += SelectedVerseEvent;
            var parent = (MainPageSplit)((Grid)((Grid)((Grid)Parent).Parent).Parent).Parent;
            parent.NavigationService.Navigate(new Uri("/SelectBibleBook.xaml", UriKind.Relative));
        }

        private void ButSearch_OnClick(object sender, RoutedEventArgs e)
        {
            KillManipulation();
            PhoneApplicationService.Current.State["openWindowIndex"] = _state.CurIndex;

            var parent = (MainPageSplit)((Grid)((Grid)((Grid)Parent).Parent).Parent).Parent;
            parent.NavigationService.Navigate(new Uri("/Search.xaml", UriKind.Relative));
        }

        private void ButSearch_OnManipulationCompleted(object sender, ManipulationCompletedEventArgs e)
        {
            DoManipulation(e);
        }

        private void ButSelectBook_OnManipulationCompleted(object sender, ManipulationCompletedEventArgs e)
        {
            DoManipulation(e);
        }
         
        private void webBrowser1_NavigationCompleted(object sender, NavigationEventArgs args)
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

        private void ScrollToThisVerse(string bookShortName, int chapterNum, int verseNum)
        {
            try
            {
                string id = bookShortName + "_" + chapterNum + "_" + verseNum;
                string xx = ConvertColorToHtmlRgba(App.Themes.AccentColor).GetHtmlRgba();
                this.webBrowser1.InvokeScript(
                    "SetFontColorForElement",
                    new[] { id, ConvertColorToHtmlRgba(App.Themes.AccentColor).GetHtmlRgba().ToUpper() });
                if (!string.IsNullOrEmpty(this._lastSelectedItem) && !this._lastSelectedItem.Equals(id))
                {
                    try
                    {
                        this.webBrowser1.InvokeScript(
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
                this.webBrowser1.InvokeScript(
                     "ShowNodePhone",
                    new[] { this._lastSelectedItem });
                this.WriteTitle();
            }
            catch (Exception)
            {
            }
        }
    }
}