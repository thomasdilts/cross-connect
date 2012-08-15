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

    using AudioPlaybackAgent1;

    using CrossConnect.readers;

    using Microsoft.Phone.Controls;
    using Microsoft.Phone.Shell;

    using Sword.reader;

    #region Enumerations

    public enum WindowType
    {
        WindowBible,
        WindowBibleNotes,
        WindowSearch,
        WindowHistory,
        WindowBookmarks,
        WindowDailyPlan,
        WindowAddedNotes,
        WindowCommentary,
        WindowTranslator,
        WindowInternetLink,
        WindowLexiconLink,
        WindowMediaPlayer
    }

    #endregion Enumerations

    public partial class App
    {
        #region Fields

        public const string Version = "1.0.0.28";
        public const string WebDirIsolated = "webtemporary";

        public static SerializableDailyPlan DailyPlan = new SerializableDailyPlan();
        public static DisplaySettings DisplaySettings = new DisplaySettings();
        public static InstalledBiblesAndCommentaries InstalledBibles = new InstalledBiblesAndCommentaries();
        public static int IsFirstTimeInMainPageSplit;
        public static MainPageSplit MainWindow;
        public static List<ITiledWindow> OpenWindows = new List<ITiledWindow>();
        public static BiblePlaceMarkers PlaceMarkers = new BiblePlaceMarkers();
        public static Theme Themes = new Theme();

        private const int MaxNumWindows = 30;

        private bool _phoneApplicationInitialized;

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

        public static event WindowSourceChanged BookMarksChanged;

        public static event WindowSourceChanged HistoryChanged;

        public static event WindowSourceChanged PersonalNotesChanged;

        #endregion Events

        #region Properties

        /// <summary>
        ///   Provides easy access to the root frame of the Phone Application.
        /// </summary>
        /// <returns>The root frame of the Phone Application.</returns>
        private PhoneApplicationFrame RootFrame
        {
            get; set;
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
                    if (last.ChapterNum == lastBookmark.ChapterNum && last.VerseNum == lastBookmark.VerseNum)
                    {
                        PlaceMarkers.Bookmarks.RemoveAt(PlaceMarkers.Bookmarks.Count - 1);
                    }
                }

                PlaceMarkers.Bookmarks.Add(new BiblePlaceMarker(last.ChapterNum, last.VerseNum, DateTime.Now));

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
        }

        public static void AddHistory(int chapterNum, int verseNum)
        {
            // stop repeats
            if (PlaceMarkers.History.Count > 0)
            {
                BiblePlaceMarker last = PlaceMarkers.History[PlaceMarkers.History.Count - 1];
                if (last.ChapterNum == chapterNum && last.VerseNum == verseNum)
                {
                    PlaceMarkers.History.RemoveAt(PlaceMarkers.History.Count - 1);
                }
            }

            PlaceMarkers.History.Add(new BiblePlaceMarker(chapterNum, verseNum, DateTime.Now));

            // don't let this get more then a 100
            if (PlaceMarkers.History.Count > 100)
            {
                for (int i = 0; i < PlaceMarkers.History.Count - 100; i++)
                {
                    PlaceMarkers.History.RemoveAt(0);
                }
            }

            RaiseHistoryChangeEvent();
        }

        public static void AddMediaWindow(AudioPlayer.MediaInfo info)
        {
            // only one media window allowed
            for (int i = 0; i < OpenWindows.Count(); i++)
            {
                if (OpenWindows[i].State.WindowType == WindowType.WindowMediaPlayer)
                {
                    // change the windows view to this one
                    ((MediaPlayerWindow)OpenWindows[i]).RestartToThisMedia(info);
                    MainWindow.OverRideCurrentlyShowingScreen(OpenWindows[i].State.Window);
                    return;
                }
            }

            var state = new SerializableWindowState
                {
                    WindowType = WindowType.WindowMediaPlayer,
                    Source = new MediaReader(info)
                };
            var nextWindow = new MediaPlayerWindow { State = state };
            nextWindow.State.CurIndex = OpenWindows.Count();
            nextWindow.State.HtmlFontSize = 10;
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

        public static void AddWindow(
            string bibleToLoad,
            string bibleDescription,
            WindowType typeOfWindow,
            double textSize,
            IBrowserTextSource source = null)
        {
            var nextWindow = new BrowserTitledWindow { State = { HtmlFontSize = textSize } };
            nextWindow.Initialize(bibleToLoad, bibleDescription, typeOfWindow, source);
            nextWindow.State.CurIndex = OpenWindows.Count();
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

        public static void RaiseBookmarkChangeEvent()
        {
            if (BookMarksChanged != null)
            {
                BookMarksChanged();
            }
        }

        public static void RaiseHistoryChangeEvent()
        {
            if (HistoryChanged != null)
            {
                HistoryChanged();
            }
        }

        public static void RaisePersonalNotesChangeEvent()
        {
            if (PersonalNotesChanged != null)
            {
                PersonalNotesChanged();
            }
        }

        public static void SynchronizeAllWindows(int chapterNum, int verseNum, int curIndex)
        {
            for (int i = 0; i < OpenWindows.Count(); i++)
            {
                if (i != curIndex)
                {
                    OpenWindows[i].SynchronizeWindow(chapterNum, verseNum);
                }
            }
        }

        private void ApplicationActivated(object sender, ActivatedEventArgs e)
        {
            LoadPersistantObjects();
        }

        // Code to execute when the application is closing (eg, user hit Back)
        // This code will not execute when the application is deactivated
        private void ApplicationClosing(object sender, ClosingEventArgs e)
        {
            SavePersistantObjects();
        }

        // Code to execute when the application is deactivated (sent to background)
        // This code will not execute when the application is closing
        private void ApplicationDeactivated(object sender, DeactivatedEventArgs e)
        {
            SavePersistantObjects();
        }

        // Code to execute when the application is launching (eg, from Start)
        // This code will not execute when the application is reactivated
        private void ApplicationLaunching(object sender, LaunchingEventArgs e)
        {
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
        }

        private void LoadPersistantObjects()
        {
            DailyPlan = new SerializableDailyPlan();
            OpenWindows.Clear();
            PlaceMarkers = new BiblePlaceMarkers();
            IsolatedStorageSettings.ApplicationSettings["LanguageIsoCode"] = "default";
            DisplaySettings = new DisplaySettings();
            try
            {
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

                PhoneApplicationService.Current.State["InitializeWindowSettings"] = true;

                // get the daily plan first
                string dailyPlanXmlData;
                if (IsolatedStorageSettings.ApplicationSettings.TryGetValue("DailyPlan", out dailyPlanXmlData))
                {
                    using (var sr = new StringReader(dailyPlanXmlData))
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
                    DailyPlan.PlanTextSize = 5;
                }

                OpenWindows.Clear();

                // get all windows
                for (int i = 0; i < MaxNumWindows; i++)
                {
                    string windowsXmlData;
                    if (IsolatedStorageSettings.ApplicationSettings.TryGetValue("Windows" + i, out windowsXmlData))
                    {
                        using (var sr = new StringReader(windowsXmlData))
                        {
                            var settings = new XmlReaderSettings();
                            using (XmlReader reader = XmlReader.Create(sr, settings))
                            {
                                var types = new[]
                                    {
                                        typeof(SerializableWindowState), typeof(BibleZtextReader.VersePos),
                                        typeof(BibleZtextReader.ChapterPos), typeof(BibleZtextReader.BookPos),
                                        typeof(BibleZtextReader), typeof(BibleNoteReader),
                                        typeof(BibleZtextReaderSerialData), typeof(CommentZtextReader),
                                        typeof(TranslatorReader), typeof(BookMarkReader), typeof(HistoryReader),
                                        typeof(SearchReader), typeof(DailyPlanReader), typeof(PersonalNotesReader),
                                        typeof(InternetLinkReader), typeof(MediaReader), typeof(GreekHebrewDictReader),
                                        typeof(AudioPlayer.MediaInfo)
                                    };
                                var ser = new DataContractSerializer(typeof(SerializableWindowState), types);
                                var state = (SerializableWindowState)ser.ReadObject(reader);
                                ITiledWindow nextWindow;
                                if (state.WindowType.Equals(WindowType.WindowMediaPlayer))
                                {
                                    nextWindow = new MediaPlayerWindow { State = state };
                                }
                                else
                                {
                                    nextWindow = new BrowserTitledWindow { State = state };
                                    ((BrowserTitledWindow)nextWindow).SetVScroll(state.VSchrollPosition);
                                }

                                nextWindow.State.Source.Resume();
                                nextWindow.State.IsResume = true;
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

                string markerXmlData;
                if (IsolatedStorageSettings.ApplicationSettings.TryGetValue("BiblePlaceMarkers", out markerXmlData))
                {
                    using (var sr = new StringReader(markerXmlData))
                    {
                        var settings = new XmlReaderSettings();
                        using (XmlReader reader = XmlReader.Create(sr, settings))
                        {
                            var types = new[] { typeof(BiblePlaceMarkers), typeof(BiblePlaceMarker) };
                            var ser = new DataContractSerializer(typeof(BiblePlaceMarkers), types);
                            PlaceMarkers = (BiblePlaceMarkers)ser.ReadObject(reader);
                        }
                    }
                }

                if (PlaceMarkers == null)
                {
                    PlaceMarkers = new BiblePlaceMarkers();
                }

                if (!IsolatedStorageSettings.ApplicationSettings.Contains("LanguageIsoCode"))
                {
                    IsolatedStorageSettings.ApplicationSettings["LanguageIsoCode"] = "default";
                }

                if (IsolatedStorageSettings.ApplicationSettings.TryGetValue("DisplaySettings", out markerXmlData))
                {
                    using (var sr = new StringReader(markerXmlData))
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

                if (IsolatedStorageSettings.ApplicationSettings.TryGetValue("Themes", out markerXmlData))
                {
                    Themes.FromString(markerXmlData);
                }
                else
                {
                    Themes.InitializeFromResources();
                }

                object objCurrrentScreen;
                if (IsolatedStorageSettings.ApplicationSettings.TryGetValue("CurrentScreen", out objCurrrentScreen))
                {
                    PhoneApplicationService.Current.State["CurrentScreen"] = (int)objCurrrentScreen;
                }
            }
            catch (Exception ee)
            {
                Debug.WriteLine("crashed in a strange place. err=" + ee.StackTrace);
            }
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

        private void SavePersistantObjects()
        {
            // remove all current settings.
            for (int i = 0; i < MaxNumWindows; i++)
            {
                string windowsXmlData;
                if (IsolatedStorageSettings.ApplicationSettings.TryGetValue("Windows" + i, out windowsXmlData))
                {
                    IsolatedStorageSettings.ApplicationSettings.Remove("Windows" + i);
                }
                else
                {
                    // no more windows to remove.
                    break;
                }
            }

            // add new settings.
            for (int i = 0; i < OpenWindows.Count(); i++)
            {
                var types = new[]
                    {
                        typeof(SerializableWindowState), typeof(BibleZtextReader.VersePos),
                        typeof(BibleZtextReader.ChapterPos), typeof(BibleZtextReader.BookPos), typeof(BibleZtextReader),
                        typeof(BibleNoteReader), typeof(BibleZtextReaderSerialData), typeof(CommentZtextReader),
                        typeof(TranslatorReader), typeof(BookMarkReader), typeof(HistoryReader), typeof(SearchReader),
                        typeof(DailyPlanReader), typeof(PersonalNotesReader), typeof(InternetLinkReader),
                        typeof(MediaReader), typeof(GreekHebrewDictReader), typeof(AudioPlayer.MediaInfo)
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
                        if (OpenWindows[i].State.WindowType != WindowType.WindowMediaPlayer)
                        {
                            // change the windows view to this one
                            OpenWindows[i].State.VSchrollPosition = ((BrowserTitledWindow)OpenWindows[i]).GetVScroll();
                        }

                        ser.WriteObject(writer, OpenWindows[i].State);
                    }

                    IsolatedStorageSettings.ApplicationSettings.Add("Windows" + i, sw.ToString());
                }
            }

            var types2 = new[] { typeof(BiblePlaceMarkers), typeof(BiblePlaceMarker) };

            var ser2 = new DataContractSerializer(typeof(BiblePlaceMarkers), types2);
            using (var sw = new StringWriter())
            {
                var settings = new XmlWriterSettings
                    {
                       OmitXmlDeclaration = true, Indent = true, NamespaceHandling = NamespaceHandling.OmitDuplicates
                    };
                using (XmlWriter writer = XmlWriter.Create(sw, settings))
                {
                    ser2.WriteObject(writer, PlaceMarkers);
                }

                IsolatedStorageSettings.ApplicationSettings["BiblePlaceMarkers"] = sw.ToString();
            }

            var ser4 = new DataContractSerializer(typeof(DisplaySettings), types2);
            using (var sw = new StringWriter())
            {
                var settings = new XmlWriterSettings
                    {
                       OmitXmlDeclaration = true, Indent = true, NamespaceHandling = NamespaceHandling.OmitDuplicates
                    };
                using (XmlWriter writer = XmlWriter.Create(sw, settings))
                {
                    ser4.WriteObject(writer, DisplaySettings);
                }

                IsolatedStorageSettings.ApplicationSettings["DisplaySettings"] = sw.ToString();
            }

            var types3 = new[] { typeof(SerializableDailyPlan) };

            var ser3 = new DataContractSerializer(typeof(SerializableDailyPlan), types3);
            using (var sw = new StringWriter())
            {
                var settings = new XmlWriterSettings
                    {
                       OmitXmlDeclaration = true, Indent = true, NamespaceHandling = NamespaceHandling.OmitDuplicates
                    };
                using (XmlWriter writer = XmlWriter.Create(sw, settings))
                {
                    ser3.WriteObject(writer, DailyPlan);
                }

                IsolatedStorageSettings.ApplicationSettings["DailyPlan"] = sw.ToString();
            }

            if (Themes.Themes.Count() > 0)
            {
                string text = Themes.ToString();
                IsolatedStorageSettings.ApplicationSettings["Themes"] = text;
            }
            else
            {
                IsolatedStorageSettings.ApplicationSettings.Remove("Themes");
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

            #endregion Fields
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