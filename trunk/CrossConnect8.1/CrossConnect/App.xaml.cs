// <copyright file="App.xaml.cs" company="Thomas Dilts">
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
    using System.IO;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Threading.Tasks;
    using System.Xml;

    using CrossConnect.Common;
    using CrossConnect.readers;

    using Sword.reader;

    using Windows.ApplicationModel;
    using Windows.ApplicationModel.Activation;
    using Windows.Storage;
    using Windows.Storage.Streams;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Notifications;
    using NotificationsExtensions.TileContent;
    using Sword.versification;
    using Sword;

    /// <summary>
    ///     Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App : Application
    {
        #region Constants

        public const string Version = "2.0.9.39";

        public const string WebDirIsolated = "webtemporary";

        private const int MaxNumWindows = 30;

        // this is actually superceded but needs to be checked for older programs updated to the newer version
        private const string PersistantObjectsFileName = "_PersistantObjects.xml";

        #endregion

        #region Static Fields

        public static SerializableDailyPlan DailyPlan = new SerializableDailyPlan();

        public static DisplaySettings DisplaySettings = new DisplaySettings();

        public static InstalledBiblesAndCommentaries InstalledBibles = new InstalledBiblesAndCommentaries();

        public static int IsFirstTimeInMainPageSplit;

        public static MainPageSplit MainWindow;

        public static List<ITiledWindow> OpenWindows = new List<ITiledWindow>();

        public static BiblePlaceMarkers PlaceMarkers = new BiblePlaceMarkers();

        public static Theme Themes = new Theme();

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes the singleton Application object.  This is the first line of authored code
        ///     executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();
            this.Suspending += this.OnSuspending;
        }

        #endregion

        #region Events

        public static event Sword.reader.BiblePlaceMarkReader.BiblePlaceMarkChangedDelegate BookMarksChanged;

        public static event Sword.reader.BiblePlaceMarkReader.BiblePlaceMarkChangedDelegate HistoryChanged;

        public static event Sword.reader.PersonalNotesReader.NotesChangedDelegate PersonalNotesChanged;

        #endregion Events

        #region Public Methods and Operators

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

        private static DispatcherTimer TimerForSavingWindows=null;

        public static void StartTimerForSavingWindows()
        {
            if(TimerForSavingWindows!=null)
            {
                TimerForSavingWindows.Stop();
            }

            TimerForSavingWindows = new DispatcherTimer();
            TimerForSavingWindows.Tick += OnTimerForSavingWindowsTick;
            TimerForSavingWindows.Interval = TimeSpan.FromSeconds(7);
            TimerForSavingWindows.Start();
        }

        public static async void OnTimerForSavingWindowsTick(object sender, object e)
        {
            if (TimerForSavingWindows != null)
            {
                TimerForSavingWindows.Stop();
            }
            TimerForSavingWindows = null;
            await SavePersistantWindows();
        }

        public static void StartTimerForNotifications()
        {
            var tmr = new DispatcherTimer();
            tmr.Tick += OnStartNotificationsTimerTick;
            tmr.Interval = TimeSpan.FromSeconds(15);
            tmr.Start();
        }

        private static async void OnStartNotificationsTimerTick(object sender, object e)
        {
            ((DispatcherTimer)sender).Stop();

            // find the window
            if (!OpenWindows.Any())
            {
                return;
            }

            ITiledWindow foundWindowToUse = null;
            ITiledWindow firstBibleWindowToUse = null;
            foreach (var window in OpenWindows)
            {
                if (window.State.WindowType == WindowType.WindowBible)
                {
                    if (firstBibleWindowToUse == null)
                    {
                        firstBibleWindowToUse = window;
                    }

                    if (window.State.Source.GetLanguage().Substring(0,2).ToLower().Equals(Translations.IsoLanguageCode.Substring(0,2).ToLower()))
                    {
                        foundWindowToUse = window;
                        break;
                    }
                } 
            }

            if(foundWindowToUse == null)
            {
                foundWindowToUse = firstBibleWindowToUse;
                if(foundWindowToUse == null)
                {
                    return;
                }
            }

            //TileUpdateManager.CreateTileUpdaterForApplication().EnableNotificationQueue(false);
            var updater = TileUpdateManager.CreateTileUpdaterForApplication();
            //clear the queue.  It might be cleared anyway.
            updater.Clear();

            // Enable the notification queue - this only needs to be called once in the lifetime of your app
            updater.EnableNotificationQueue(true);

            //Try to find the best possible bible verses
            var bibleVerses = new Dictionary<string, BiblePlaceMarker>();

            var random = new Random();

            //Try to get one from bookmarks
            if (PlaceMarkers.Bookmarks.Any())
            {
                var count = PlaceMarkers.Bookmarks.Count();
                var index = random.Next(0, count - 1);
                bibleVerses[PlaceMarkers.Bookmarks[index].ToString()] = PlaceMarkers.Bookmarks[index];

                //try to get another one from bookmarks if there is a lot
                if (count > 3)
                {
                    do
                    {
                        index = random.Next(0, count - 1);
                    } while (bibleVerses.ContainsKey(PlaceMarkers.Bookmarks[index].ToString()));
                    bibleVerses[PlaceMarkers.Bookmarks[index].ToString()] = PlaceMarkers.Bookmarks[index];
                }
            }
            else
            {
                //else, try to get one from history
                if (PlaceMarkers.History.Any())
                {
                    var count = PlaceMarkers.History.Count();
                    var index = random.Next(0, count - 1);
                    bibleVerses[PlaceMarkers.History[index].ToString()] = PlaceMarkers.History[index];
                }
            }

            var count2 = bibleVerses.Count();
            //get the rest from the presaved intresting verses.
            for (int i = count2; i < 5; i++)
            {
                var staticcount = StaticBibleVerses.Markers.Count;
                int index;
                do
                {
                    index = random.Next(0, staticcount - 1);
                } while (bibleVerses.ContainsKey(StaticBibleVerses.Markers[index].ToString()));
                bibleVerses[StaticBibleVerses.Markers[index].ToString()] = StaticBibleVerses.Markers[index];
            }

            //
            foreach (var item in bibleVerses)
            {
                string titlesOnly = string.Empty;
                string textsWithTitles = string.Empty;
                BiblePlaceMarker place = item.Value;
                string title = ((BibleZtextReader)foundWindowToUse.State.Source).BookNames(Translations.IsoLanguageCode).GetShortName(place.BookShortName) + " " + (place.ChapterNum + 1) + ":" + (place.VerseNum + 1) + " - "
                               + foundWindowToUse.State.BibleDescription;
                string verseText = string.Empty;
                try
                {
                    verseText = await foundWindowToUse.State.Source.GetVerseTextOnly(App.DisplaySettings, place.BookShortName, place.ChapterNum, place.VerseNum);
                }
                catch(Exception)
                {
                    //this text is not in that bible.
                    continue;
                }
                titlesOnly += title;
                textsWithTitles +=
                    verseText.Replace("<p>", string.Empty)
                             .Replace("</p>", string.Empty)
                             .Replace("<br />", string.Empty)
                             .Replace("\n", " ") + " " + title;

                // create the wide template
                var tileContent = TileContentFactory.CreateTileWidePeekImage04();
                tileContent.TextBodyWrap.Text = textsWithTitles;
                tileContent.Image.Src = "ms-appx:///Assets/splash310x150.png";
                tileContent.Image.Alt = "well what do you know";

                // Users can resize tiles to square or wide.
                // create the square template and attach it to the wide template
                ITileSquarePeekImageAndText04 squareContent = TileContentFactory.CreateTileSquarePeekImageAndText04();
                squareContent.TextBodyWrap.Text = textsWithTitles;
                squareContent.Image.Src = "ms-appx:///Assets/splash150x150.png";
                squareContent.Image.Alt = item.Key + "well what do you know";
                tileContent.SquareContent = squareContent;

                // send the notification
                updater.Update(tileContent.CreateNotification());

            }
        }

        public static async Task AddWindow(
            string bibleToLoad,
            string bibleDescription,
            WindowType typeOfWindow,
            double textSize,
            string font,
            object initialData,
            int column,
            IBrowserTextSource source = null,
            bool isReDraw = true)
        {
            var nextWindow = new BrowserTitledWindow { State = { HtmlFontSize = textSize,Font = font } };
            await nextWindow.Initialize(bibleToLoad, bibleDescription, typeOfWindow, initialData, source);
            nextWindow.State.CurIndex = OpenWindows.Count();
            nextWindow.State.Window = column;
            OpenWindows.Add(nextWindow);

            if (MainWindow != null && isReDraw)
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

        public static void ShowUserInterface(bool isShow)
        {
            for (int i = 0; i < OpenWindows.Count(); i++)
            {
                OpenWindows[i].ShowUserInterface(isShow);
            }
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
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Invoked when the application is launched normally by the end user.  Other entry points
        ///     will be used when the application is launched to open a specific file, to display
        ///     search results, and so forth.
        /// </summary>
        /// <param name="args">Details about the launch request and process.</param>
        protected override async void OnLaunched(LaunchActivatedEventArgs args)
        {
            if (args.PreviousExecutionState != ApplicationExecutionState.Suspended && args.PreviousExecutionState != ApplicationExecutionState.Running)
            {
                await LoadPersistantObjects();
            }

            var rootFrame = Window.Current.Content as Frame;

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active

            if (rootFrame == null)
            {
                // Create a Frame to act as the navigation context and navigate to the first page
                rootFrame = new Frame();
                //Associate the frame with a SuspensionManager key                                
                SuspensionManager.RegisterFrame(rootFrame, "AppFrame");

                if (args.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    // Restore the saved session state only when appropriate
                    try
                    {
                        await SuspensionManager.RestoreAsync();
                    }
                    catch (SuspensionManagerException)
                    {
                        //Something went wrong restoring state.
                        //Assume there is no state and continue
                    }
                }

                // Place the frame in the current Window
                Window.Current.Content = rootFrame;
            }
            if (rootFrame.Content == null)
            {
                // When the navigation stack isn't restored navigate to the first page,
                // configuring the new page by passing required information as a navigation
                // parameter
                if (!rootFrame.Navigate(typeof(MainPageSplit), "AllGroups"))
                {
                    throw new Exception("Failed to create initial page");
                }
            }
            // Ensure the current window is active
            Window.Current.Activate();
        }

        private static async Task<Dictionary<String, Object>> LoadPersistantObjectsFromFile(string filename, LoadPersObjDelegate loadFunction, bool alwaysLocal=false)
        {
            var objectsToLoad = new Dictionary<String, Object>();
            bool isLoaded = false;
            StorageFolder folder = ApplicationData.Current.LocalFolder;
            if (await BibleZtextReader.FileExists(folder, filename))
            {
                try
                {
                    // Get the input stream for the SessionState file
                    StorageFile file = await folder.GetFileAsync(filename);

                    using (IInputStream inStream = await file.OpenSequentialReadAsync())
                    {
                        // debugging start
                        //byte[] buff = new byte[20000];
                        //var xx = inStream.AsStreamForRead().Read(buff, 0, buff.Length);
                        //string all = System.Text.UTF8Encoding.UTF8.GetString(buff, 0, xx);
                        //inStream.AsStreamForRead().Position = 0;
                        // debugging stop

                        // Deserialize the Session State
                        var serializer = new DataContractSerializer(
                            typeof(Dictionary<string, object>), new[] { typeof(string) });
                        objectsToLoad = (Dictionary<string, object>)serializer.ReadObject(inStream.AsStreamForRead());
                    }

                    await loadFunction(objectsToLoad);
                    isLoaded = true;
                }
                catch (Exception e)
                {
                    // we can't use the file. Just kill it.
                    Hoot.File.Delete(filename);
                    objectsToLoad = new Dictionary<String, Object>();
                    isLoaded = false;
                }
            }

            if(!isLoaded)
            {
                await loadFunction(objectsToLoad);
            }

            return objectsToLoad;
        }

        private static async Task LoadPersistantWindows(Dictionary<String, Object> objectsToLoad)
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
            else
            {
                if(DailyPlan.PersonalNotes.Any())
                {
                    //convert to the new note system
                    var canon = CanonManager.GetCanon("KJV");
                    foreach (var chapter in DailyPlan.PersonalNotes)
	                {
                        var book = canon.GetBookFromAbsoluteChapter(chapter.Key);
                        foreach (var verse in chapter.Value)
	                    {
                            BiblePlaceMarker note = BiblePlaceMarker.Clone(verse.Value);
                            note.BookShortName = book.ShortName1;
                            note.VerseNum = note.VerseNum - book.VersesInChapterStartIndex;
                            if (!App.DailyPlan.PersonalNotesVersified.ContainsKey(note.BookShortName))
                            {
                                App.DailyPlan.PersonalNotesVersified.Add(note.BookShortName, new Dictionary<int, Dictionary<int, BiblePlaceMarker>>());
                            }

                            if (!App.DailyPlan.PersonalNotesVersified[note.BookShortName].ContainsKey(note.ChapterNum))
                            {
                                App.DailyPlan.PersonalNotesVersified[note.BookShortName].Add(note.ChapterNum, new Dictionary<int, BiblePlaceMarker>());
                            }

                            App.DailyPlan.PersonalNotesVersified[note.BookShortName][note.ChapterNum][note.VerseNum] = note;
	                    }		 
	                }

                }

                DailyPlan.PersonalNotes = new Dictionary<int,Dictionary<int,BiblePlaceMarker>>();
            }

            if (DailyPlan.PlanBible == null)
            {
                DailyPlan.PlanBible = string.Empty;
                DailyPlan.PlanBibleDescription = string.Empty;
                DailyPlan.PlanTextSize = 15;
            }

            OpenWindows.Clear();

            // get all windows
            for (int i = 0; i < MaxNumWindows; i++)
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
                                typeof(BibleZtextReader), typeof(BibleNoteReader),
                                typeof(BibleZtextReaderSerialData), typeof(CommentZtextReader),
                                typeof(TranslatorReader), typeof(BiblePlaceMarkReader),
                                typeof(SearchReader), typeof(DailyPlanReader),
                                typeof(PersonalNotesReader), typeof(InternetLinkReader),
                                typeof(GreekHebrewDictReader), typeof(RawGenSearchReader),
                                typeof(AudioPlayer.MediaInfo), typeof(RawGenTextReader), typeof(RawGenTextPlaceMarker)
                            };
                            var ser = new DataContractSerializer(typeof(SerializableWindowState), types);
                            var state = (SerializableWindowState)ser.ReadObject(reader);
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
                                var info = new AudioPlayer.MediaInfo() { Book = bookShortName, Chapter = book.VersesInChapterStartIndex + relChaptNum, Verse = verseNum, VoiceName = nextWindow.State.VoiceName, IsNtOnly = nextWindow.State.IsNtOnly, Pattern = nextWindow.State.Pattern, Src = nextWindow.State.Src, Code=nextWindow.State.code };
                                ((MediaPlayerWindow)nextWindow).SetMediaInfo(nextWindow.State, info);
                            }
                            else
                            {
                                nextWindow = new BrowserTitledWindow { State = state };
                                ((BrowserTitledWindow)nextWindow).SetVScroll(state.VSchrollPosition);
                                // find config file and fix the language.
                                // this will update a language in the case that the config file has changed.
                                if (nextWindow.State.Source is BibleZtextReader)
                                {
                                    var path = ((BibleZtextReader)nextWindow.State.Source).Serial.Path;
                                    foreach (var book in App.InstalledBibles.InstalledBibles)
                                    {
                                        if (book.Value != null && book.Value.GetCetProperty(ConfigEntryType.ADataPath).ToString().Substring(2).Equals(path))
                                        {
                                            ((BibleZtextReader)nextWindow.State.Source).Serial.Iso2DigitLangCode = ((Language)book.Value.GetCetProperty(ConfigEntryType.Lang)).Code;
                                            break;
                                        }
                                    }
                                }

                                await nextWindow.State.Source.Resume();
                                nextWindow.State.IsResume = true;
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

            if (OpenWindows.Any())
            {
                StartTimerForNotifications();
            }
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
            
            if (!ApplicationData.Current.LocalSettings.Values.ContainsKey("LanguageIsoCode"))
            {
                ApplicationData.Current.LocalSettings.Values["LanguageIsoCode"] = "default";
            }

            Translations.IsoLanguageCode = (string)ApplicationData.Current.LocalSettings.Values["LanguageIsoCode"];

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
            DisplaySettings.MarginInsideTextWindow = 20;
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
                    }
                }
            }

            if (PlaceMarkers == null)
            {
                PlaceMarkers = new BiblePlaceMarkers();
            }
            else
            {
                BiblePlaceMarkers.FixOldStyleMarkers(PlaceMarkers.Bookmarks);
                BiblePlaceMarkers.FixOldStyleMarkers(PlaceMarkers.History);
            }
        }

        public delegate Task LoadPersObjDelegate(Dictionary<String, Object> objectsToLoad);

        public static async Task<bool> LoadPersistantObjects(bool alwaysLocal = false)
        {
            StorageFolder folder = ApplicationData.Current.LocalFolder;

            try
            {
                // clear temp directory
                IReadOnlyList<StorageFile> webFiles = await ApplicationData.Current.TemporaryFolder.GetFilesAsync();
                foreach (StorageFile file in webFiles)
                {
                    if (file.DisplayName.StartsWith(WebClient.destination))
                    {
                        await file.DeleteAsync();
                    }
                }

                await InstalledBibles.Initialize();
                await LoadPersistantObjectsFromFile(BackupRestore.PersistantObjectsDisplaySettingsFileName, LoadPersistantDisplaySettings, true);
                await LoadPersistantObjectsFromFile(BackupRestore.PersistantObjectsThemesFileName, LoadPersistantThemes, true);
                await LoadPersistantObjectsFromFile(BackupRestore.PersistantObjectsHighlightFileName, LoadPersistantHighlighting, true);
                await LoadPersistantObjectsFromFile(BackupRestore.PersistantObjectsMarkersFileName, LoadPersistantMarkers, true);
                await LoadPersistantObjectsFromFile(BackupRestore.PersistantObjectsWindowsFileName, LoadPersistantWindows, true);
            }
            catch (Exception ee)
            {
                Debug.WriteLine("crashed in a strange place. err=" + ee.StackTrace);
            }

            return true;
        }

        /// <summary>
        ///     Invoked when application execution is being suspended.  Application state is saved
        ///     without knowing whether the application will be terminated or resumed with the contents
        ///     of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        private async void OnSuspending(object sender, SuspendingEventArgs e)
        {
            if (TimerForSavingWindows != null)
            {
                TimerForSavingWindows.Stop();
            }
            SuspendingDeferral deferral = e.SuspendingOperation.GetDeferral();
            await SuspensionManager.SaveAsync();
            deferral.Complete();
        }

        public static async Task SavePersistantWindows()
        {
            var objectsToSave = new Dictionary<string, object>();
            for (int i = 0; i < OpenWindows.Count(); i++)
            {
                var types = new[]
                                {
                                    typeof(SerializableWindowState), typeof(BibleZtextReader.VersePos),
                                    typeof(BibleZtextReader.ChapterPos), typeof(BibleZtextReader.BookPos),
                                    typeof(BibleZtextReader), typeof(BibleNoteReader), typeof(BibleZtextReaderSerialData),
                                    typeof(CommentZtextReader), typeof(TranslatorReader), typeof(BiblePlaceMarkReader),
                                    typeof(SearchReader), typeof(DailyPlanReader),
                                    typeof(PersonalNotesReader), typeof(InternetLinkReader),
                                    typeof(GreekHebrewDictReader), typeof(AudioPlayer.MediaInfo), typeof(RawGenTextReader), 
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
                        if (OpenWindows[i].State.WindowType != WindowType.WindowMediaPlayer)
                        {
                            // change the windows view to this one
                            OpenWindows[i].State.VSchrollPosition = await ((BrowserTitledWindow)OpenWindows[i]).GetVScroll();
                        }

                        ser.WriteObject(writer, OpenWindows[i].State);
                    }

                    objectsToSave.Add("Windows" + i, sw.ToString());
                }
            }

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

            await SavePersistantObjects(objectsToSave, BackupRestore.PersistantObjectsWindowsFileName);
        }
        public static async Task SaveAllPersistantObjects()
        {
            await SavePersistantThemes();
            await SavePersistantDisplaySettings();
            await SavePersistantHighlighting();
            await SavePersistantMarkers();
            if (TimerForSavingWindows != null)
            {
                TimerForSavingWindows.Stop();
            }
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

        private static async Task SavePersistantObjects(Dictionary<string, object> objectsToSave, string filename, bool alwaysLocal=false)
        {
            try
            {
                var sessionData = new MemoryStream();
                var serializer = new DataContractSerializer(
                    typeof(Dictionary<string, object>), new[] { typeof(string) });
                serializer.WriteObject(sessionData, objectsToSave);

                // Get an output stream for the SessionState file and write the state asynchronously
                StorageFolder folder = ApplicationData.Current.LocalFolder;
                StorageFile file = await folder.CreateFileAsync(
                            filename, CreationCollisionOption.ReplaceExisting);

                using (Stream fileStream = await file.OpenStreamForWriteAsync())
                {
                    sessionData.Seek(0, SeekOrigin.Begin);
                    await sessionData.CopyToAsync(fileStream);
                    await fileStream.FlushAsync();
                }
            }
            catch (Exception e)
            {
                //throw new SuspensionManagerException(e);
            }
        }


        #endregion

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
                if(maybeOldStyleMarkers.Count()>0 && string.IsNullOrEmpty(maybeOldStyleMarkers[0].BookShortName))
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
            public double PlanTextSize = 15;

            #endregion
        }
    }
}