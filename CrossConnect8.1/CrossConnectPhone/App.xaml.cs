#region Header

// <copyright file="App.xaml.cs" company="Thomas Dilts">
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
    using System.IO;
    using System.IO.IsolatedStorage;
    using System.Linq;
    using System.Runtime.Serialization;

    using System.Windows;
    using System.Windows.Navigation;
    using System.Xml;

    using CrossConnect.readers;

    using Microsoft.Phone.Controls;
    using Microsoft.Phone.Shell;

    using Sword.reader;
    using System.Windows.Threading;
    using System.Threading.Tasks;
    using AudioPlaybackAgent1;
    using Windows.Storage;
    using Sword.versification;
    using Sword;
    using Windows.UI.Notifications;
    //using NotificationsExtensions.TileContent;
    using Windows.Data.Xml.Dom;
    using System.Xml.Linq;
    using Windows.ApplicationModel.Activation;

    #region Enumerations

    public enum WindowType
    {
        WindowBible,
        WindowBibleNotes,
        WindowSearch,
        WindowHistory,
        WindowBookmarks,
        WindowDailyPlan,
        WindowSelectedVerses,
        WindowAddedNotes,
        WindowCommentary,
        WindowTranslator,
        WindowInternetLink,
        WindowLexiconLink,
        WindowMediaPlayer,
        WindowBook,
        WindowDictionary
    }

    #endregion Enumerations

    public partial class App
    {
        #region Fields

        public const string Version = "2.0.8.7";
        public const string WebDirIsolated = "webtemporary";
        // the newer file names divided into sections.

        public static SerializableDailyPlan DailyPlan = new SerializableDailyPlan();
        public static DisplaySettings DisplaySettings = new DisplaySettings();
        public static InstalledBiblesAndCommentaries InstalledBibles = new InstalledBiblesAndCommentaries();
        public static int IsFirstTimeInMainPageSplit;
        public static MainPageSplit MainWindow;
        public static List<ITiledWindow> OpenWindows = new List<ITiledWindow>();
        public static BiblePlaceMarkers PlaceMarkers = new BiblePlaceMarkers();
        public static Theme Themes = new Theme();
        public static bool IsPersitanceLoaded = false;
        public static bool IsClosingDown = false;

        private const int MaxNumWindows = 30;

        private bool _phoneApplicationInitialized;
        public IActivatedEventArgs FilePickerContinuationArgs { get; set; }

        #endregion Fields

        #region Constructors

        public App()
        {
            // Global handler for uncaught exceptions.
            UnhandledException += Application_UnhandledException;

            // Show graphics profiling information while debugging.
            if (Debugger.IsAttached)
            {
                // Display the current frame rate counters.
                Current.Host.Settings.EnableFrameRateCounter = true;

                // Show the areas of the app that are being redrawn in each frame.
                // Application.Current.Host.Settings.EnableRedrawRegions = true;

                // Enable non-production analysis visualization mode,
                // which shows areas of a page that are being GPU accelerated with a colored overlay.
                // Application.Current.Host.Settings.EnableCacheVisualization = true;
            }

            // Standard Silverlight initialization
            InitializeComponent();

            // Phone-specific initialization
            InitializePhoneApplication();
        }

        #endregion Constructors

        #region Events

        public static event Sword.reader.BiblePlaceMarkReader.BiblePlaceMarkChangedDelegate BookMarksChanged;

        public static event Sword.reader.BiblePlaceMarkReader.BiblePlaceMarkChangedDelegate HistoryChanged;

        public static event Sword.reader.PersonalNotesReader.NotesChangedDelegate PersonalNotesChanged;

        #endregion Events

        #region Properties

        /// <summary>
        ///   Provides easy access to the root frame of the Phone Application.
        /// </summary>
        /// <returns>The root frame of the Phone Application.</returns>
        private PhoneApplicationFrame RootFrame
        {
            get;
            set;
        }

        #endregion Properties

        #region Methods

        public static void AddBookmark()
        {
            // we take from the history and add to the bookmark
            if (PlaceMarkers.History.Count > 0)
            {
                // stop repeats
                BiblePlaceMarker last = PlaceMarkers.History[PlaceMarkers.History.Count - 1];
                if (PlaceMarkers.Bookmarks.Count > 0)
                {
                    BiblePlaceMarker lastBookmark = PlaceMarkers.Bookmarks[PlaceMarkers.Bookmarks.Count - 1];
                    if (last.BookShortName.Equals(lastBookmark.BookShortName) && last.ChapterNum == lastBookmark.ChapterNum && last.VerseNum == lastBookmark.VerseNum)
                    {
                        PlaceMarkers.Bookmarks.RemoveAt(PlaceMarkers.Bookmarks.Count - 1);
                    }
                }

                PlaceMarkers.Bookmarks.Add(new BiblePlaceMarker(last.BookShortName, last.ChapterNum, last.VerseNum, DateTime.Now));

                // don't let this get more then a 100
                if (PlaceMarkers.Bookmarks.Count > 100)
                {
                    for (int i = 0; i < PlaceMarkers.Bookmarks.Count - 100; i++)
                    {
                        PlaceMarkers.Bookmarks.RemoveAt(0);
                    }
                }

                RaiseBookmarkChangeEvent();
            }

            SavePersistantMarkers();
        }

        public static void AddHistory(string bookShortName, int chapterNum, int verseNum)
        {
            // stop repeats
            if (PlaceMarkers.History.Count > 0)
            {
                BiblePlaceMarker last = PlaceMarkers.History[PlaceMarkers.History.Count - 1];
                if (last.BookShortName.Equals(bookShortName) && last.ChapterNum == chapterNum && last.VerseNum == verseNum)
                {
                    PlaceMarkers.History.RemoveAt(PlaceMarkers.History.Count - 1);
                }
            }

            PlaceMarkers.History.Add(new BiblePlaceMarker(bookShortName, chapterNum, verseNum, DateTime.Now));

            // don't let this get more then a 100
            if (PlaceMarkers.History.Count > 100)
            {
                for (int i = 0; i < PlaceMarkers.History.Count - 100; i++)
                {
                    PlaceMarkers.History.RemoveAt(0);
                }
            }

            RaiseHistoryChangeEvent();
            SavePersistantMarkers();
        }

        private static DispatcherTimer TimerForNotifications = null;
        public static void StartTimerForNotifications()
        {
            if (TimerForNotifications != null)
            {
                TimerForNotifications.Stop();
                TimerForNotifications = null;
            }
            TimerForNotifications = new DispatcherTimer();
            TimerForNotifications.Tick += OnStartNotificationsTimerTick;
            TimerForNotifications.Interval = TimeSpan.FromSeconds(15);
            TimerForNotifications.Start();
        }
        private static async void OnStartNotificationsTimerTick(object sender, object e)
        {
            if (TimerForNotifications != null)
            {
                TimerForNotifications.Stop();
                TimerForNotifications = null;
            }

            if (IsClosingDown) return;
            // find the window
            if (!OpenWindows.Any())
            {
                return;
            }

            ITiledWindow foundWindowToUse = null;
            ITiledWindow firstBibleWindowToUse = null;
            try
            {
                foreach (var window in OpenWindows)
                {
                    if (window.State.WindowType == WindowType.WindowBible)
                    {
                        if (firstBibleWindowToUse == null)
                        {
                            firstBibleWindowToUse = window;
                        }

                        if (window.State.Source.GetLanguage().Substring(0, 2).ToLower().Equals(Translations.IsoLanguageCode.Substring(0, 2).ToLower()))
                        {
                            foundWindowToUse = window;
                            break;
                        }
                    }
                }
            }
            catch (Exception)
            {

            }

            if (IsClosingDown) return;
            if (foundWindowToUse == null)
            {
                foundWindowToUse = firstBibleWindowToUse;
                if (foundWindowToUse == null)
                {
                    return;
                }
            }

            if (IsClosingDown) return;

            var random = new Random();
            string verseText = string.Empty;
            string title = string.Empty;
            for (int i = 0; i < 40 && string.IsNullOrEmpty(verseText); i++)
            {
                var index = random.Next(0, StaticBibleVerses.Markers.Count - 1);
                var place = StaticBibleVerses.Markers[index];
                try
                {
                    verseText = await foundWindowToUse.State.Source.GetVerseTextOnly(App.DisplaySettings, place.BookShortName, place.ChapterNum, place.VerseNum);
                    title = ((BibleZtextReader)foundWindowToUse.State.Source).BookNames(Translations.IsoLanguageCode).GetShortName(place.BookShortName) + " " + (place.ChapterNum + 1) + ":" + (place.VerseNum + 1) + " - "
                        + foundWindowToUse.State.BibleDescription;
                }
                catch (Exception)
                {
                    //this text is not in that bible.
                    //continue;
                }
            }

            if (IsClosingDown) return;
            if (string.IsNullOrEmpty(verseText))
            {
                return;
            }
            
            foreach (var item in ShellTile.ActiveTiles)
            {
                FlipTileData TileData = new FlipTileData()
                {
                    Title = "CrossConnect",
                    BackTitle = title,
                    BackContent = verseText,
                    WideBackContent = verseText,
                    Count = 0,
                    SmallBackgroundImage = new Uri("Background.png", UriKind.Relative),
                    BackgroundImage = new Uri("Background.png", UriKind.Relative),
                    BackBackgroundImage = new Uri("Backgroundlight.png", UriKind.Relative),
                    WideBackgroundImage = new Uri("crossbigbut691x336.png", UriKind.Relative),
                    WideBackBackgroundImage = new Uri("sidecrossbigbut691x336.png", UriKind.Relative),
                };
                item.Update(TileData);
            }

        }
        private static XmlDocument ToXmlDocument(XDocument xDocument)
        {
            var xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(xDocument.ToString());
            return xmlDocument;
            /*
            using (var reader = xDocument.CreateReader())
            {
                xmlDocument.Load(reader);
            }
            var xDeclaration = xDocument.Declaration;
            if (xDeclaration != null)
            {
                var xmlDeclaration = xmlDocument.CreateXmlDeclaration(
                    xDeclaration.Version,
                    xDeclaration.Encoding,
                    xDeclaration.Standalone);
                xmlDocument.InsertBefore(xmlDeclaration, xmlDocument.FirstChild);
            }
            return xmlDocument;*/
        }
        public static async void AddMediaWindow(AudioPlayer.MediaInfo info)
        {
            object openWindowIndex;
            if (!PhoneApplicationService.Current.State.TryGetValue("openWindowIndex", out openWindowIndex))
            {
                openWindowIndex = 0;
            }
            SerializableWindowState state = await OpenWindows[(int)openWindowIndex].State.Clone();
            state.WindowType = WindowType.WindowMediaPlayer;
            state.IconLink = info.IconLink;
            state.Src = info.Src;
            state.Pattern = info.Pattern;
            state.IsNtOnly = info.IsNtOnly;
            state.code = info.Code;
            state.VoiceName = info.VoiceName;
            state.Language = info.Language;
            state.Name = info.Name;
            state.Icon = info.Icon;
            state.Source.MoveChapterVerse(info.Book, info.Chapter, info.Verse, false, state.Source);

            // only one media window allowed
            for (int i = 0; i < OpenWindows.Count(); i++)
            {
                if (OpenWindows[i].State.WindowType == WindowType.WindowMediaPlayer)
                {
                    // change the windows view to this one
                    ((MediaPlayerWindow)OpenWindows[i]).SetMediaInfo(state, info);
                    MainWindow.OverRideCurrentlyShowingScreen(OpenWindows[i].State.Window);
                    return;
                }
            }

            var nextWindow = new MediaPlayerWindow { State = state };
            nextWindow.State.CurIndex = OpenWindows.Count();
            nextWindow.State.HtmlFontSize = 10;
            nextWindow.SetMediaInfo(state, info);
            OpenWindows.Add(nextWindow);
            object objCurrrentScreen;
            nextWindow.State.Window = 0;
            if (PhoneApplicationService.Current.State.TryGetValue("CurrentScreen", out objCurrrentScreen))
            {
                nextWindow.State.Window = (int)objCurrrentScreen;
            }

            if (MainWindow != null)
            {
                MainWindow.ReDrawWindows();
            }
        }

        public static async void AddWindow(
            string bibleToLoad,
            string bibleDescription,
            WindowType typeOfWindow,
            double textSize,
            string font,
            object initialData,
            IBrowserTextSource source = null)
        {
            object objCurrrentScreen;
            int column = 0;
            if (PhoneApplicationService.Current.State.TryGetValue("CurrentScreen", out objCurrrentScreen))
            {
                column = (int)objCurrrentScreen;
            }

            var nextWindow = new BrowserTitledWindow { State = { HtmlFontSize = textSize, Font = font } };
            await nextWindow.Initialize(bibleToLoad, bibleDescription, typeOfWindow, initialData, source);
            nextWindow.State.CurIndex = OpenWindows.Count();
            nextWindow.State.Window = column;
            OpenWindows.Add(nextWindow);
            if (typeOfWindow == WindowType.WindowDictionary)
            {
                var nextWindow2 = new BrowserTitledWindow { State = { HtmlFontSize = textSize, Font = font } };
                if (nextWindow.State.Source is DictionaryZldIndexReader)
                {
                    var indexerSource = ((DictionaryZldIndexReader)nextWindow.State.Source);
                    var dataSource = new DictionaryZldDefReader(indexerSource.Serial.Path, indexerSource.Serial.Iso2DigitLangCode, indexerSource.Serial.IsIsoEncoding, indexerSource.WindowMatchingKey);
                    await dataSource.Initialize();
                    await nextWindow2.Initialize(bibleToLoad, bibleDescription, typeOfWindow, initialData, dataSource);
                }
                else if (nextWindow.State.Source is DictionaryRaw4IndexReader)
                {
                    var indexerSource = ((DictionaryRaw4IndexReader)nextWindow.State.Source);
                    var dataSource = new DictionaryRawDefReader(indexerSource.Serial.Path, indexerSource.Serial.Iso2DigitLangCode, indexerSource.Serial.IsIsoEncoding, indexerSource.WindowMatchingKey);
                    await dataSource.Initialize();
                    await nextWindow2.Initialize(bibleToLoad, bibleDescription, typeOfWindow, initialData, dataSource);
                }
                else
                {
                    var indexerSource = ((DictionaryRawIndexReader)nextWindow.State.Source);
                    var dataSource = new DictionaryRawDefReader(indexerSource.Serial.Path, indexerSource.Serial.Iso2DigitLangCode, indexerSource.Serial.IsIsoEncoding, indexerSource.WindowMatchingKey);
                    await dataSource.Initialize();
                    await nextWindow2.Initialize(bibleToLoad, bibleDescription, typeOfWindow, initialData, dataSource);
                }
                nextWindow2.State.CurIndex = OpenWindows.Count();
                nextWindow2.State.Window = column;
                OpenWindows.Add(nextWindow2);
            }

            if (MainWindow != null)
            {
                MainWindow.ReDrawWindows();
            }
            App.StartTimerForNotifications();
        }

        public static void RaiseBookmarkChangeEvent()
        {
            if (BookMarksChanged != null)
            {
                BookMarksChanged(App.PlaceMarkers.Bookmarks, App.DisplaySettings);
            }
            SavePersistantMarkers();
        }

        public static void RaiseHistoryChangeEvent()
        {
            if (HistoryChanged != null)
            {
                HistoryChanged(App.PlaceMarkers.History, App.DisplaySettings);
            }
            SavePersistantMarkers();
        }

        public static void RaisePersonalNotesChangeEvent()
        {
            if (PersonalNotesChanged != null)
            {
                PersonalNotesChanged(App.DailyPlan.PersonalNotesVersified, App.DisplaySettings);
            }
            SavePersistantMarkers();
        }

        public static void SynchronizeAllWindows(string bookShortName, int chapterNum, int verseNum, int curIndex, IBrowserTextSource source)
        {
            for (int i = 0; i < OpenWindows.Count(); i++)
            {
                if (i != curIndex)
                {
                    OpenWindows[i].SynchronizeWindow(bookShortName, chapterNum, verseNum, source);
                }
            }

            App.StartTimerForSavingWindows();
        }

        private async void ApplicationActivated(object sender, ActivatedEventArgs e)
        {
            
            IsPersitanceLoaded = false;
            if (!e.IsApplicationInstancePreserved)
            {
                await LoadPersistantObjects();
            }
        }

        // Code to execute when the application is closing (eg, user hit Back)
        // This code will not execute when the application is deactivated
        private void ApplicationClosing(object sender, ClosingEventArgs e)
        {
            // stop any attempts to save data
            IsClosingDown = true;
            if (TimerForSavingWindows != null)
            {
                TimerForSavingWindows.Stop();
                TimerForSavingWindows = null;
            }
            if (TimerForNotifications != null)
            {
                TimerForNotifications.Stop();
                TimerForNotifications = null;
            }

            //SavePersistantObjects();
        }

        // Code to execute when the application is deactivated (sent to background)
        // This code will not execute when the application is closing
        private void ApplicationDeactivated(object sender, DeactivatedEventArgs e)
        {
            //SavePersistantObjects();
        }

        // Code to execute when the application is launching (eg, from Start)
        // This code will not execute when the application is reactivated
        private void ApplicationLaunching(object sender, LaunchingEventArgs e)
        {
            IsClosingDown = false;
            IsPersitanceLoaded = false;
            LoadPersistantObjects();

            // this is just to force the library to load¨. It is not used.
            var library = new Microsoft.Xna.Framework.Media.MediaLibrary();
        }

        // Code to execute on Unhandled Exceptions
        private void Application_UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
        {
            if (Debugger.IsAttached)
            {
                // An unhandled exception has occurred; break into the debugger
                Debugger.Break();
            }

            // MessageBoxResult result = MessageBox.Show(Translations.translate("An error occured. Do you want to completely erase the memory for this program?"), string.Empty, MessageBoxButton.OKCancel);
            // if (result == MessageBoxResult.OK)
            // {
            // IsolatedStorageFile root = IsolatedStorageFile.GetUserStoreForApplication();
            // root.Remove();
            // System.IO.IsolatedStorage.IsolatedStorageSettings.ApplicationSettings.Clear();
            // }
        }

        // Do not add any additional code to this method
        private void CompleteInitializePhoneApplication(object sender, NavigationEventArgs e)
        {
            // Set the root visual to allow the application to render
            RootVisual = RootFrame;

            // Remove this handler since it is no longer needed
            RootFrame.Navigated -= CompleteInitializePhoneApplication;
        }

        // Do not add any additional code to this method
        private void InitializePhoneApplication()
        {
            if (_phoneApplicationInitialized)
            {
                return;
            }

            // Create the frame but don't set it as RootVisual yet; this allows the splash
            // screen to remain active until the application is ready to render.
            RootFrame = new TransitionFrame();
            RootFrame.Navigated += CompleteInitializePhoneApplication;

            // Handle navigation failures
            RootFrame.NavigationFailed += RootFrame_NavigationFailed;
            // Ensure we don't initialize again
            _phoneApplicationInitialized = true;
            PhoneApplicationService.Current.ContractActivated += Application_ContractActivated;

        }
        // Code to execute when the application is activated (brought to foreground)
        // This code will not execute when the application is first launched
        private void Application_ContractActivated(object sender, IActivatedEventArgs e)
        {
            this.FilePickerContinuationArgs = e;
        }
        private static async Task LoadPersistantWindows(Dictionary<String, Object> objectsToLoad)
        {
            try
            {
                OpenWindows.Clear();
                DailyPlan = new SerializableDailyPlan();

                // get the daily plan first
                object dailyPlanXmlData;
                if (objectsToLoad.TryGetValue("DailyPlan", out dailyPlanXmlData))
                {
                    using (var sr = new StringReader((string)dailyPlanXmlData))
                    {
                        var settings = new XmlReaderSettings();
                        using (XmlReader reader = XmlReader.Create(sr, settings))
                        {
                            var types = new[] { typeof(SerializableDailyPlan) };
                            var ser = new DataContractSerializer(typeof(SerializableDailyPlan), types);
                            DailyPlan = (SerializableDailyPlan)ser.ReadObject(reader);
                        }
                    }
                }

                if (DailyPlan == null)
                {
                    DailyPlan = new SerializableDailyPlan();
                }

                if (DailyPlan.PlanBible == null)
                {
                    DailyPlan.PlanBible = string.Empty;
                    DailyPlan.PlanBibleDescription = string.Empty;
                    DailyPlan.PlanTextSize = 15;
                }
            }
            catch (Exception e)
            {

            }
            OpenWindows.Clear();

            // get all windows
            for (int i = 0; i < MaxNumWindows; i++)
            {
                try
                {
                    object windowsXmlData;
                    if (objectsToLoad.TryGetValue("Windows" + i, out windowsXmlData))
                    {
                        using (var sr = new StringReader((string)windowsXmlData))
                        {
                            var settings = new XmlReaderSettings();
                            using (XmlReader reader = XmlReader.Create(sr, settings))
                            {
                                var types = new[]
                                                {
                                                    typeof(SerializableWindowState), typeof(BibleZtextReader.VersePos),
                                                    typeof(BibleZtextReader.ChapterPos), typeof(BibleZtextReader.BookPos),
                                                    typeof(BibleZtextReader), typeof(BibleNoteReader),typeof(BibleRawTextReader),
                                                    typeof(BibleZtextReaderSerialData), typeof(CommentZtextReader),typeof(CommentRawComReader),
                                                    typeof(TranslatorReader), typeof(BiblePlaceMarkReader),
                                                    typeof(SearchReader), typeof(DailyPlanReader),
                                                    typeof(PersonalNotesReader), typeof(InternetLinkReader),
                                                    typeof(GreekHebrewDictReader), typeof(RawGenSearchReader),
                                                    typeof(DictionaryRawDefReader),typeof(DictionaryRawIndexReader),typeof(DictionaryRaw4IndexReader),typeof(DictionaryZldDefReader),typeof(DictionaryZldIndexReader),
                                                    typeof(AudioPlayer.MediaInfo), typeof(RawGenTextReader), typeof(RawGenTextPlaceMarker)
                                                };
                                var ser = new DataContractSerializer(typeof(SerializableWindowState), types);
                                var state = (SerializableWindowState)ser.ReadObject(reader);
                                if ((state.Window + 1) > App.DisplaySettings.NumberOfScreens)
                                {
                                    App.DisplaySettings.NumberOfScreens = state.Window + 1;
                                }
                                ITiledWindow nextWindow;
                                if (state.WindowType.Equals(WindowType.WindowMediaPlayer))
                                {
                                    nextWindow = new MediaPlayerWindow { State = state };
                                    await nextWindow.State.Source.Resume();
                                    nextWindow.State.IsResume = true;
                                    string bookShortName;
                                    int relChaptNum;
                                    int verseNum;
                                    string fullName;
                                    string title;
                                    nextWindow.State.Source.GetInfo(Translations.IsoLanguageCode, out bookShortName,
                                        out relChaptNum,
                                        out verseNum,
                                        out fullName,
                                        out title);
                                    var canon = CanonManager.GetCanon("KJV");
                                    var book = canon.BookByShortName[bookShortName];
                                    var info = new AudioPlayer.MediaInfo() { Book = bookShortName, Chapter = book.VersesInChapterStartIndex + relChaptNum, Verse = verseNum, VoiceName = nextWindow.State.VoiceName, IsNtOnly = nextWindow.State.IsNtOnly, Pattern = nextWindow.State.Pattern, Src = nextWindow.State.Src, Code = nextWindow.State.code };
                                    ((MediaPlayerWindow)nextWindow).SetMediaInfo(nextWindow.State, info);
                                }
                                else
                                {
                                    nextWindow = new BrowserTitledWindow { State = state };
                                    ((BrowserTitledWindow)nextWindow).SetVScroll(state.VSchrollPosition);
                                    await nextWindow.State.Source.Resume();
                                    nextWindow.State.IsResume = true;
                                }

                                switch (state.WindowType)
                                {
                                    case WindowType.WindowHistory:
                                        App.HistoryChanged += ((BiblePlaceMarkReader)state.Source).BiblePlaceMarkSourceChanged;
                                        break;
                                    case WindowType.WindowBookmarks:
                                        App.BookMarksChanged += ((BiblePlaceMarkReader)state.Source).BiblePlaceMarkSourceChanged;
                                        break;
                                    case WindowType.WindowAddedNotes:
                                        App.PersonalNotesChanged += ((PersonalNotesReader)state.Source).NotesSourceChanged;
                                        break;
                                }

                                OpenWindows.Add(nextWindow);

                                // nextWindow.Initialize(nextWindow.state.bibleToLoad, nextWindow.state.windowType);
                            }
                        }
                    }
                    else
                    {
                        // no more windows to load.
                        break;
                    }
                }
                catch (Exception e)
                {

                }
            }

            object objCurrrentScreen;
            if (IsolatedStorageSettings.ApplicationSettings.TryGetValue("CurrentScreen", out objCurrrentScreen))
            {
                PhoneApplicationService.Current.State["CurrentScreen"] = (int)objCurrrentScreen;
            }

            //if (OpenWindows.Any())
            //{
            //    StartTimerForNotifications();
            //}
        }

        private static async Task LoadPersistantThemes(Dictionary<String, Object> objectsToLoad)
        {
            object markerXmlData;
            if (objectsToLoad.TryGetValue("Themes", out markerXmlData))
            {
                Themes.FromString((string)markerXmlData);
            }
            else
            {
                Themes.InitializeFromResources();
            }
        }

        private static async Task LoadPersistantDisplaySettings(Dictionary<String, Object> objectsToLoad)
        {
            object markerXmlData;
            DisplaySettings = new DisplaySettings();
            if (!IsolatedStorageSettings.ApplicationSettings.Contains("LanguageIsoCode"))
            {
                IsolatedStorageSettings.ApplicationSettings["LanguageIsoCode"] = "default";
            }

            Translations.IsoLanguageCode = (string)IsolatedStorageSettings.ApplicationSettings["LanguageIsoCode"];

            if (objectsToLoad.TryGetValue("DisplaySettings", out markerXmlData))
            {
                using (var sr = new StringReader((string)markerXmlData))
                {
                    var settings = new XmlReaderSettings();
                    using (XmlReader reader = XmlReader.Create(sr, settings))
                    {
                        var types = new[] { typeof(DisplaySettings) };
                        var ser = new DataContractSerializer(typeof(DisplaySettings), types);
                        DisplaySettings = (DisplaySettings)ser.ReadObject(reader);
                    }
                }
            }

            if (DisplaySettings == null)
            {
                DisplaySettings = new DisplaySettings();
            }

            DisplaySettings.CheckForNullAndFix();
            DisplaySettings.MarginInsideTextWindow = 4;
        }

        private static async Task LoadPersistantHighlighting(Dictionary<String, Object> objectsToLoad)
        {
            object markerXmlData;
            DisplaySettings.highlighter = new Highlighter();
            if (objectsToLoad.TryGetValue("Highlights", out markerXmlData))
            {
                DisplaySettings.highlighter.FromString((string)markerXmlData);                //using (var sr = new StringReader((string)markerXmlData))
                //{

                //    var settings = new XmlReaderSettings();
                //    using (XmlReader reader = XmlReader.Create(sr, settings))
                //    {
                //        var types = new[] { typeof(Highlighter), typeof(BiblePlaceMarker) };
                //        var ser = new DataContractSerializer(typeof(Highlighter), types);
                //        DisplaySettings.highlighter.FromString((string)ser.ReadObject(reader));
                //    }
                //}
            }
        }

        private static async Task LoadPersistantMarkers(Dictionary<String, Object> objectsToLoad)
        {
            PlaceMarkers = new BiblePlaceMarkers();
            object markerXmlData;
            if (objectsToLoad.TryGetValue("BiblePlaceMarkers", out markerXmlData))
            {
                using (var sr = new StringReader((string)markerXmlData))
                {
                    var settings = new XmlReaderSettings();
                    using (XmlReader reader = XmlReader.Create(sr, settings))
                    {
                        var types = new[] { typeof(BiblePlaceMarkers), typeof(BiblePlaceMarker) };
                        var ser = new DataContractSerializer(typeof(BiblePlaceMarkers), types);
                        PlaceMarkers = (BiblePlaceMarkers)ser.ReadObject(reader);
                        reader.Close();
                    }
                    sr.Close();
                }
            }

            if (PlaceMarkers == null)
            {
                PlaceMarkers = new BiblePlaceMarkers();
            }
            RaiseHistoryChangeEvent();
            RaiseBookmarkChangeEvent();
        }

        public delegate Task LoadPersObjDelegate(Dictionary<String, Object> objectsToLoad);

        public static async Task<bool> LoadPersistantObjects()
        {
            // just a test to see if the old system is there
            try
            {
                if (IsClosingDown == true) return true;

                // make sure some important directories exist.
                IsolatedStorageFile root = IsolatedStorageFile.GetUserStoreForApplication();
                if (!root.DirectoryExists(WebDirIsolated))
                {
                    root.CreateDirectory(WebDirIsolated);
                }

                // clear web directory
                string[] filenames = root.GetFileNames(WebDirIsolated + "/*.*");
                foreach (string file in filenames)
                {
                    root.DeleteFile(WebDirIsolated + "/" + file);
                }

                await InstalledBibles.Initialize();
                var fileNames = new List<string> { 
                    BackupRestore.PersistantObjectsDisplaySettingsFileName,
                    BackupRestore.PersistantObjectsThemesFileName,
                    BackupRestore.PersistantObjectsHighlightFileName,
                    BackupRestore.PersistantObjectsMarkersFileName,
                    BackupRestore.PersistantObjectsWindowsFileName};
                var loadFunctions = new List<LoadPersObjDelegate>{
                    LoadPersistantDisplaySettings,
                    LoadPersistantThemes,
                    LoadPersistantHighlighting,
                    LoadPersistantMarkers,
                    LoadPersistantWindows};
                for (int i = 0; i < loadFunctions.Count(); i++)
                {
                    if (IsClosingDown == true) return true;
                    try
                    {
                        await LoadPersistantObjectsFromFile(fileNames[i], loadFunctions[i], true);
                    }
                    catch (Exception e)
                    {

                    }
                }
            }
            catch (Exception ee)
            {
                Debug.WriteLine("crashed in a strange place. err=" + ee.StackTrace);
            }
            IsPersitanceLoaded = true;
            if (MainWindow != null)
            {
                MainWindow.DoLoading();
            }
            if (OpenWindows.Any())
            {
                StartTimerForNotifications();
            }
            return true;
        }
        private static async Task<Dictionary<String, Object>> LoadPersistantObjectsFromFile(string filename, LoadPersObjDelegate loadFunction, bool alwaysLocal = false)
        {
            var objectsToLoad = new Dictionary<String, Object>();
            bool isLoaded = false;
            if (await Hoot.File.Exists(filename))
            {
                Stream stream = null;
                try
                {
                    stream = await ApplicationData.Current.LocalFolder.OpenStreamForReadAsync(filename);

                    // Deserialize the Session State
                    var serializer = new DataContractSerializer(
                        typeof(Dictionary<string, object>), new[] { typeof(string) });
                    objectsToLoad = (Dictionary<string, object>)serializer.ReadObject(stream);
                    stream.Close();
                    stream = null;

                    await loadFunction(objectsToLoad);
                    isLoaded = true;


                }
                catch (Exception e)
                {
                    // debugging start
                    //if(stream!=null)
                    //{
                    //    byte[] buf = new byte[20000];
                    //    var mstream = new MemoryStream();

                    //    int i;
                    //    if ((i = stream.Read(buf, 0, buf.Count())) > 0)
                    //    {
                    //        string all = System.Text.UTF8Encoding.UTF8.GetString(buf, 0, i);
                    //        Debug.WriteLine(all);
                    //    }
                    //}
                    // debugging stop

                    if (stream != null)
                    {
                        try
                        {
                            stream.Close();
                        }
                        catch (Exception)
                        {

                        }
                    }


                    // we can't use the file. Just kill it.
                    Hoot.File.Delete(filename);
                    objectsToLoad = new Dictionary<String, Object>();
                    isLoaded = false;
                }
            }

            if (!isLoaded)
            {
                await loadFunction(objectsToLoad);
            }

            return objectsToLoad;
        }

        // Code to execute if a navigation fails
        private void RootFrame_NavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            if (Debugger.IsAttached)
            {
                // A navigation has failed; break into the debugger
                Debugger.Break();
            }
        }

        private static DispatcherTimer TimerForSavingWindows = null;

        public static void StartTimerForSavingWindows()
        {
            if (IsClosingDown) return;
            if (TimerForSavingWindows != null)
            {
                TimerForSavingWindows.Stop();
            }
            else
            {
                OnTimerForSavingWindowsTick(null, null);
            }

            TimerForSavingWindows = new DispatcherTimer();
            TimerForSavingWindows.Tick += OnTimerForSavingWindowsTick;
            TimerForSavingWindows.Interval = TimeSpan.FromMilliseconds(1500);
            TimerForSavingWindows.Start();
        }

        private static async void OnTimerForSavingWindowsTick(object sender, object e)
        {
            if (TimerForSavingWindows != null)
            {
                TimerForSavingWindows.Stop();
            }
            TimerForSavingWindows = null;
            await SavePersistantWindows();
        }

        public static async Task SavePersistantWindows()
        {
            var objectsToSave = new Dictionary<string, object>();
            var windowCounter = 0;
            for (int i = 0; i < OpenWindows.Count(); i++)
            {
                try
                {
                    var types = new[]
                                {
                                    typeof(SerializableWindowState), typeof(BibleZtextReader.VersePos),
                                    typeof(BibleZtextReader.ChapterPos), typeof(BibleZtextReader.BookPos),
                                    typeof(BibleZtextReader), typeof(BibleNoteReader), typeof(BibleZtextReaderSerialData),
                                    typeof(CommentZtextReader), typeof(TranslatorReader), typeof(BiblePlaceMarkReader),
                                    typeof(SearchReader), typeof(DailyPlanReader),typeof(BibleRawTextReader),typeof(CommentRawComReader),
                                    typeof(PersonalNotesReader), typeof(InternetLinkReader),
                                    typeof(GreekHebrewDictReader), typeof(AudioPlayer.MediaInfo), typeof(RawGenTextReader), 
                                    typeof(DictionaryRawDefReader),typeof(DictionaryRawIndexReader),typeof(DictionaryRaw4IndexReader),typeof(DictionaryZldDefReader),typeof(DictionaryZldIndexReader),
                                    typeof(RawGenTextPlaceMarker), typeof(RawGenSearchReader)
                                };
                    var ser = new DataContractSerializer(typeof(SerializableWindowState), types);
                    using (var sw = new StringWriter())
                    {
                        var settings = new XmlWriterSettings
                        {
                            OmitXmlDeclaration = true,
                            Indent = true,
                            NamespaceHandling = NamespaceHandling.OmitDuplicates
                        };
                        using (XmlWriter writer = XmlWriter.Create(sw, settings))
                        {
                            OpenWindows[i].State.Source.SerialSave();
                            //if (OpenWindows[i].State.WindowType != WindowType.WindowMediaPlayer)
                            //{
                            //    // change the windows view to this one
                            //    OpenWindows[i].State.VSchrollPosition = await ((BrowserTitledWindow)OpenWindows[i]).GetVScroll();
                            //}

                            ser.WriteObject(writer, OpenWindows[i].State);
                        }

                        objectsToSave.Add("Windows" + windowCounter, sw.ToString());
                        windowCounter++;
                    }
                }
                catch (Exception e)
                {

                }
            }

            try
            {
                var types3 = new[] { typeof(SerializableDailyPlan) };

                var ser3 = new DataContractSerializer(typeof(SerializableDailyPlan), types3);
                using (var sw = new StringWriter())
                {
                    var settings = new XmlWriterSettings
                    {
                        OmitXmlDeclaration = true,
                        Indent = true,
                        NamespaceHandling = NamespaceHandling.OmitDuplicates
                    };
                    using (XmlWriter writer = XmlWriter.Create(sw, settings))
                    {
                        ser3.WriteObject(writer, DailyPlan);
                    }

                    objectsToSave["DailyPlan"] = sw.ToString();
                }
                // this particular state must be saved
                object objCurrrentScreen;
                int currentScreen = 0;
                if (PhoneApplicationService.Current.State.TryGetValue("CurrentScreen", out objCurrrentScreen))
                {
                    currentScreen = (int)objCurrrentScreen;
                }

                IsolatedStorageSettings.ApplicationSettings["CurrentScreen"] = currentScreen;
            }
            catch (Exception e)
            {

            }
            await SavePersistantObjects(objectsToSave, BackupRestore.PersistantObjectsWindowsFileName);
        }
        public static async Task SaveAllPersistantObjects()
        {
            if (IsClosingDown) return;
            await SavePersistantThemes();
            if (IsClosingDown) return;
            await SavePersistantDisplaySettings();
            if (IsClosingDown) return;
            await SavePersistantHighlighting();
            if (IsClosingDown) return;
            await SavePersistantMarkers();
            if (TimerForSavingWindows != null)
            {
                TimerForSavingWindows.Stop();
            }
            if (IsClosingDown) return;
            await SavePersistantWindows();
        }
        public static async Task SavePersistantThemes()
        {
            var objectsToSave = new Dictionary<string, object>();
            if (Themes.Themes.Count() > 0)
            {
                string text = Themes.ToString();
                objectsToSave["Themes"] = text;
            }
            else
            {
                objectsToSave.Remove("Themes");
            }

            await SavePersistantObjects(objectsToSave, BackupRestore.PersistantObjectsThemesFileName);
        }

        public static async Task SavePersistantDisplaySettings()
        {
            var objectsToSave = new Dictionary<string, object>();
            var types2 = new[] { typeof(DisplaySettings) };
            var ser4 = new DataContractSerializer(typeof(DisplaySettings), types2);
            using (var sw = new StringWriter())
            {
                var settings = new XmlWriterSettings
                {
                    OmitXmlDeclaration = true,
                    Indent = true,
                    NamespaceHandling = NamespaceHandling.OmitDuplicates
                };
                using (XmlWriter writer = XmlWriter.Create(sw, settings))
                {
                    ser4.WriteObject(writer, DisplaySettings);
                }

                objectsToSave["DisplaySettings"] = sw.ToString();
            }
            // UseRemoteStorage is always local
            await SavePersistantObjects(objectsToSave, BackupRestore.PersistantObjectsDisplaySettingsFileName, true);
        }

        public static async Task SavePersistantHighlighting()
        {
            var objectsToSave = new Dictionary<string, object>();
            objectsToSave["Highlights"] = DisplaySettings.highlighter.ToString();
            await SavePersistantObjects(objectsToSave, BackupRestore.PersistantObjectsHighlightFileName);
        }

        public static async Task SavePersistantMarkers()
        {
            var objectsToSave = new Dictionary<string, object>();
            // add new settings.
            var types2 = new[] { typeof(BiblePlaceMarkers), typeof(BiblePlaceMarker) };

            var ser2 = new DataContractSerializer(typeof(BiblePlaceMarkers), types2);
            using (var sw = new StringWriter())
            {
                var settings = new XmlWriterSettings
                {
                    OmitXmlDeclaration = true,
                    Indent = true,
                    NamespaceHandling = NamespaceHandling.OmitDuplicates
                };
                using (XmlWriter writer = XmlWriter.Create(sw, settings))
                {
                    ser2.WriteObject(writer, PlaceMarkers);
                }

                objectsToSave["BiblePlaceMarkers"] = sw.ToString();
            }

            await SavePersistantObjects(objectsToSave, BackupRestore.PersistantObjectsMarkersFileName);
        }

        public static async Task SavePersistantObjects(Dictionary<string, object> objectsToSave, string filename, bool alwaysLocal = false)
        {
            try
            {
                if (IsClosingDown) return;
                var sessionData = new MemoryStream();
                var serializer = new DataContractSerializer(
                    typeof(Dictionary<string, object>), new[] { typeof(string) });
                serializer.WriteObject(sessionData, objectsToSave);
                if (IsClosingDown) return;
                sessionData.Seek(0, SeekOrigin.Begin);
                byte[] buffer = new byte[sessionData.Length];
                sessionData.Read(buffer, 0, (int)sessionData.Length);
                if (IsClosingDown) return;
                await Hoot.File.WriteAllBytes(filename, buffer);
            }
            catch (Exception e)
            {
                //throw new SuspensionManagerException(e);
            }
        }

        #endregion Methods

        #region Nested Types

        [DataContract(IsReference = true)]
        [KnownType(typeof(BiblePlaceMarker))]
        public class BiblePlaceMarkers
        {
            #region Fields

            [DataMember(Name = "bookmarks")]
            public List<BiblePlaceMarker> Bookmarks = new List<BiblePlaceMarker>();

            [DataMember(Name = "history")]
            public List<BiblePlaceMarker> History = new List<BiblePlaceMarker>();

            #endregion

            public static void FixOldStyleMarkers(List<BiblePlaceMarker> maybeOldStyleMarkers)
            {
                if (maybeOldStyleMarkers.Count() > 0 && string.IsNullOrEmpty(maybeOldStyleMarkers[0].BookShortName))
                {
                    var canon = CanonManager.GetCanon("KJV");
                    foreach (var mark in maybeOldStyleMarkers)
                    {
                        var book = canon.GetBookFromAbsoluteChapter(mark.ChapterNum);
                        mark.ChapterNum = mark.ChapterNum - book.VersesInChapterStartIndex;
                        mark.BookShortName = book.ShortName1;
                    }
                }
            }
        }


        [DataContract]
        public class SerializableDailyPlan
        {
            #region Fields

            [DataMember(Name = "currentChapterNumber")]
            public int CurrentChapterNumber;
            [DataMember(Name = "currentVerseNumber")]
            public int CurrentVerseNumber;
            [DataMember(Name = "personalNotes")]
            public Dictionary<int, Dictionary<int, BiblePlaceMarker>> PersonalNotes =
                new Dictionary<int, Dictionary<int, BiblePlaceMarker>>();
            [DataMember(Name = "PersonalNotesVersified")]
            public Dictionary<string, Dictionary<int, Dictionary<int, BiblePlaceMarker>>> PersonalNotesVersified =
                new Dictionary<string, Dictionary<int, Dictionary<int, BiblePlaceMarker>>>();

            [DataMember]
            public string PlanBible = string.Empty;
            [DataMember]
            public string PlanBibleDescription = string.Empty;
            [DataMember(Name = "planDayNumber")]
            public int PlanDayNumber;
            [DataMember(Name = "planNumber")]
            public int PlanNumber;
            [DataMember(Name = "planStartDate")]
            public DateTime PlanStartDate = DateTime.Now;
            [DataMember]
            public double PlanTextSize = 5;

            #endregion Fields
        }

        #endregion Nested Types

        #region Other

        // Code to execute when the application is activated (brought to foreground)
        // This code will not execute when the application is first launched

        #endregion Other
    }
}