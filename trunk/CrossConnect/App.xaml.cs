/// <summary>
/// Distribution License:
/// CrossConnect is free software; you can redistribute it and/or modify it under
/// the terms of the GNU General Public License, version 3 as published by
/// the Free Software Foundation. This program is distributed in the hope
/// that it will be useful, but WITHOUT ANY WARRANTY; without even the
/// implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
/// See the GNU General Public License for more details.
///
/// The License is available on the internet at:
///       http://www.gnu.org/copyleft/gpl.html
/// or by writing to:
///      Free Software Foundation, Inc.
///      59 Temple Place - Suite 330
///      Boston, MA 02111-1307, USA
/// </summary>
/// <copyright file="App.xaml.cs" company="Thomas Dilts">
///     Thomas Dilts. All rights reserved.
/// </copyright>
/// <author>Thomas Dilts</author>
namespace CrossConnect
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.IO.IsolatedStorage;
    using System.Linq;
    using System.Net;
    using System.Runtime.Serialization;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using System.Windows.Navigation;
    using System.Windows.Shapes;
    using System.Xml;

    using Microsoft.Phone.Controls;
    using Microsoft.Phone.Shell;

    using SwordBackend;

    #region Enumerations

    public enum WINDOW_TYPE
    {
        WINDOW_BIBLE,
        WINDOW_BIBLE_NOTES,
        WINDOW_SEARCH,
        WINDOW_HISTORY,
        WINDOW_BOOKMARKS,
        WINDOW_DAILY_PLAN,
    }

    #endregion Enumerations

    public partial class App : Application
    {
        #region Fields

        public const int MAX_NUM_WINDOWS = 10;
        public const string WEB_DIR_ISOLATED = "webtemporary";

        public static InstalledBibles installedBibles = new InstalledBibles();
        public static int isFirstTimeInMainPageSplit = 0;
        public static MainPageSplit mainWindow = null;
        public static List<BrowserTitledWindow> openWindows = new List<BrowserTitledWindow>();
        public static BiblePlaceMarkers placeMarkers = new BiblePlaceMarkers();

        /// <summary>
        /// Avoid double-initialization
        /// </summary>
        private bool phoneApplicationInitialized = false;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Constructor for the Application object.
        /// </summary>
        public App()
        {
            // Global handler for uncaught exceptions.
            UnhandledException += this.Application_UnhandledException;

            // Show graphics profiling information while debugging.
            if (System.Diagnostics.Debugger.IsAttached)
            {
                // Display the current frame rate counters.
                Application.Current.Host.Settings.EnableFrameRateCounter = true;

                // Show the areas of the app that are being redrawn in each frame.
                // Application.Current.Host.Settings.EnableRedrawRegions = true;

                // Enable non-production analysis visualization mode,
                // which shows areas of a page that are being GPU accelerated with a colored overlay.
                // Application.Current.Host.Settings.EnableCacheVisualization = true;
            }

            // Standard Silverlight initialization
            InitializeComponent();

            // Phone-specific initialization
            this.InitializePhoneApplication();
        }

        #endregion Constructors

        #region Events

        public static event WindowSourceChanged BookMarksChanged;

        public static event WindowSourceChanged HistoryChanged;

        #endregion Events

        #region Properties

        /// <summary>
        /// Provides easy access to the root frame of the Phone Application.
        /// </summary>
        /// <returns>The root frame of the Phone Application.</returns>
        public PhoneApplicationFrame RootFrame
        {
            get;
            private set;
        }

        #endregion Properties

        #region Methods

        public static void AddBookmark(int chapterNum, int verseNum)
        {
            placeMarkers.bookmarks.Add(new BiblePlaceMarker(chapterNum, verseNum, DateTime.Now));
            if (BookMarksChanged != null)
            {
                BookMarksChanged();
            }
        }

        public static void AddHistory(int chapterNum, int verseNum)
        {
            // stop repeats
            if (placeMarkers.history.Count > 0)
            {
                BiblePlaceMarker last = placeMarkers.history[placeMarkers.history.Count - 1];
                if (last.chapterNum == chapterNum && last.verseNum == verseNum)
                {
                    placeMarkers.history.RemoveAt(placeMarkers.history.Count - 1);
                }
            }

            placeMarkers.history.Add(new BiblePlaceMarker(chapterNum, verseNum, DateTime.Now));

            // don't let this get more then a 200
            if (placeMarkers.history.Count > 200)
            {
                placeMarkers.history.RemoveAt(0);
            }

            RaiseHistoryChangeEvent();
        }

        public static void AddWindow(string bibleToLoad, int bookNum, int chapterNum, WINDOW_TYPE typeOfWindow, double textSize, IBrowserTextSource source = null)
        {
            BrowserTitledWindow nextWindow = new BrowserTitledWindow();
            nextWindow.Initialize(bibleToLoad, bookNum, chapterNum, typeOfWindow, source);
            nextWindow.state.curIndex = App.openWindows.Count();
            nextWindow.state.htmlFontSize = textSize;
            App.openWindows.Add(nextWindow);
            if (mainWindow != null)
            {
                mainWindow.ReDrawWindows();
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

        public static void SynchronizeAllWindows(int chapterNum, int verseNum, int curIndex)
        {
            for (int i = 0; i < openWindows.Count(); i++)
            {
                if (i != curIndex)
                {
                    openWindows[i].SynchronizeWindow(chapterNum, verseNum);
                }
            }
        }

        public void LoadPersistantObjects()
        {
            // make sure some important directories exist.
            var root = IsolatedStorageFile.GetUserStoreForApplication();
            if (!root.DirectoryExists(WEB_DIR_ISOLATED))
            {
                root.CreateDirectory(WEB_DIR_ISOLATED);
            }

            // clear web directory
            string[] filenames = root.GetFileNames(WEB_DIR_ISOLATED + "/*.*");
            foreach (string file in filenames)
            {
                root.DeleteFile(WEB_DIR_ISOLATED + "/" + file);
            }

            //get the daily plan first
            string dailyPlanXmlData;
            if (IsolatedStorageSettings.ApplicationSettings.TryGetValue<string>("DailyPlan", out dailyPlanXmlData))
            {
                using (StringReader sr = new StringReader(dailyPlanXmlData))
                {
                    XmlReaderSettings settings = new XmlReaderSettings();
                    using (XmlReader reader = XmlReader.Create(sr, settings))
                    {
                        Type[] types = new Type[]
                        {
                            typeof(SerializableDailyPlan),
                            };
                        DataContractSerializer ser = new DataContractSerializer(typeof(SerializableDailyPlan), types);
                        dailyPlan = (SerializableDailyPlan)ser.ReadObject(reader);
                    }
                }
            }

            if (dailyPlan == null)
            {
                dailyPlan = new SerializableDailyPlan();
            } 
            
            openWindows.Clear();

            // get all windows
            for (int i = 0; i < MAX_NUM_WINDOWS; i++)
            {
                string windowsXmlData = string.Empty;
                if (IsolatedStorageSettings.ApplicationSettings.TryGetValue<string>("Windows" + i, out windowsXmlData))
                {
                    using (StringReader sr = new StringReader(windowsXmlData))
                    {
                        XmlReaderSettings settings = new XmlReaderSettings();
                        using (XmlReader reader = XmlReader.Create(sr, settings))
                        {
                            Type[] types = new Type[]
                            {
                                typeof(CrossConnect.BrowserTitledWindow.SerializableWindowState),
                                typeof(SwordBackend.BibleZtextReader.VersePos),
                                typeof(SwordBackend.BibleZtextReader.ChapterPos),
                                typeof(SwordBackend.BibleZtextReader.BookPos),
                                typeof(SwordBackend.BibleZtextReader),
                                typeof(SwordBackend.BibleNoteReader),
                                typeof(BookMarkReader),
                                typeof(HistoryReader),
                                typeof(SearchReader),
                                typeof(SwordBackend.DailyPlanReader),
                            };
                            DataContractSerializer ser = new DataContractSerializer(typeof(CrossConnect.BrowserTitledWindow.SerializableWindowState), types);
                            BrowserTitledWindow nextWindow = new BrowserTitledWindow();
                            nextWindow.state = (CrossConnect.BrowserTitledWindow.SerializableWindowState)ser.ReadObject(reader);
                            nextWindow.state.source.ReloadSettingsFile();
                            nextWindow.state.isResume = true;
                            openWindows.Add(nextWindow);
                            nextWindow.Initialize(nextWindow.state.bibleToLoad, nextWindow.state.bookNum, nextWindow.state.chapterNum, nextWindow.state.windowType);
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
            if (IsolatedStorageSettings.ApplicationSettings.TryGetValue<string>("BiblePlaceMarkers", out markerXmlData))
            {
                using (StringReader sr = new StringReader(markerXmlData))
                {
                    XmlReaderSettings settings = new XmlReaderSettings();
                    using (XmlReader reader = XmlReader.Create(sr, settings))
                    {
                        Type[] types = new Type[]
                        {
                            typeof(BiblePlaceMarkers),
                                typeof(BiblePlaceMarker),
                            };
                        DataContractSerializer ser = new DataContractSerializer(typeof(BiblePlaceMarkers), types);
                        placeMarkers = (BiblePlaceMarkers)ser.ReadObject(reader);
                    }
                }
            }

            if (placeMarkers == null)
            {
                placeMarkers = new BiblePlaceMarkers();
            }
        }

        public void SavePersistantObjects()
        {
            // remove all current settings.
            for (int i = 0; i < MAX_NUM_WINDOWS; i++)
            {
                string windowsXmlData = string.Empty;
                if (IsolatedStorageSettings.ApplicationSettings.TryGetValue<string>("Windows" + i, out windowsXmlData))
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
            for (int i = 0; i < openWindows.Count(); i++)
            {
                Type[] types = new Type[]
                {
                    typeof(CrossConnect.BrowserTitledWindow.SerializableWindowState),
                    typeof(SwordBackend.BibleZtextReader.VersePos),
                    typeof(SwordBackend.BibleZtextReader.ChapterPos),
                    typeof(SwordBackend.BibleZtextReader.BookPos),
                    typeof(SwordBackend.BibleZtextReader),
                    typeof(SwordBackend.BibleNoteReader),
                    typeof(BookMarkReader),
                    typeof(HistoryReader),
                    typeof(SearchReader),
                    typeof(SwordBackend.DailyPlanReader),
                };
                DataContractSerializer ser = new DataContractSerializer(typeof(CrossConnect.BrowserTitledWindow.SerializableWindowState), types);
                using (StringWriter sw = new StringWriter())
                {
                    XmlWriterSettings settings = new XmlWriterSettings();
                    settings.OmitXmlDeclaration = true;
                    settings.Indent = true;
                    settings.NamespaceHandling = NamespaceHandling.OmitDuplicates;
                    using (XmlWriter writer = XmlWriter.Create(sw, settings))
                    {
                        ser.WriteObject(writer, openWindows[i].state);
                    }

                    IsolatedStorageSettings.ApplicationSettings.Add("Windows" + i, sw.ToString());
                }
            }

            Type[] types2 = new Type[]
            {
                typeof(BiblePlaceMarkers),
                typeof(BiblePlaceMarker),
            };

            DataContractSerializer ser2 = new DataContractSerializer(typeof(BiblePlaceMarkers), types2);
            using (StringWriter sw = new StringWriter())
            {
                XmlWriterSettings settings = new XmlWriterSettings();
                settings.OmitXmlDeclaration = true;
                settings.Indent = true;
                settings.NamespaceHandling = NamespaceHandling.OmitDuplicates;
                using (XmlWriter writer = XmlWriter.Create(sw, settings))
                {
                    ser2.WriteObject(writer, placeMarkers);
                }

                IsolatedStorageSettings.ApplicationSettings["BiblePlaceMarkers"] = sw.ToString();
            }

            Type[] types3 = new Type[]
            {
                typeof(SerializableDailyPlan),
            };

            DataContractSerializer ser3 = new DataContractSerializer(typeof(SerializableDailyPlan), types3);
            using (StringWriter sw = new StringWriter())
            {
                XmlWriterSettings settings = new XmlWriterSettings();
                settings.OmitXmlDeclaration = true;
                settings.Indent = true;
                settings.NamespaceHandling = NamespaceHandling.OmitDuplicates;
                using (XmlWriter writer = XmlWriter.Create(sw, settings))
                {
                    ser3.WriteObject(writer, dailyPlan);
                }

                IsolatedStorageSettings.ApplicationSettings["DailyPlan"] = sw.ToString();
            }        
        }

        // Code to execute when the application is activated (brought to foreground)
        // This code will not execute when the application is first launched
        private void Application_Activated(object sender, ActivatedEventArgs e)
        {
            this.LoadPersistantObjects();
        }

        // Code to execute when the application is closing (eg, user hit Back)
        // This code will not execute when the application is deactivated
        private void Application_Closing(object sender, ClosingEventArgs e)
        {
            this.SavePersistantObjects();
        }

        // Code to execute when the application is deactivated (sent to background)
        // This code will not execute when the application is closing
        private void Application_Deactivated(object sender, DeactivatedEventArgs e)
        {
            this.SavePersistantObjects();
        }

        // Code to execute when the application is launching (eg, from Start)
        // This code will not execute when the application is reactivated
        private void Application_Launching(object sender, LaunchingEventArgs e)
        {
            this.LoadPersistantObjects();
        }

        // Code to execute on Unhandled Exceptions
        private void Application_UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
        {
            if (System.Diagnostics.Debugger.IsAttached)
            {
                // An unhandled exception has occurred; break into the debugger
                System.Diagnostics.Debugger.Break();
            }

            //MessageBoxResult result = MessageBox.Show(Translations.translate("An error occured. Do you want to completely erase the memory for this program?"), string.Empty, MessageBoxButton.OKCancel);
            //if (result == MessageBoxResult.OK)
            //{
            //    IsolatedStorageFile root = IsolatedStorageFile.GetUserStoreForApplication();
            //    root.Remove();
            //    System.IO.IsolatedStorage.IsolatedStorageSettings.ApplicationSettings.Clear();
            //}
        }

        // Do not add any additional code to this method
        private void CompleteInitializePhoneApplication(object sender, NavigationEventArgs e)
        {
            // Set the root visual to allow the application to render
            if (this.RootVisual != this.RootFrame)
            {
                this.RootVisual = this.RootFrame;
            }

            // Remove this handler since it is no longer needed
            this.RootFrame.Navigated -= this.CompleteInitializePhoneApplication;
        }

        // Do not add any additional code to this method
        private void InitializePhoneApplication()
        {
            if (this.phoneApplicationInitialized)
            {
                return;
            }

            // Create the frame but don't set it as RootVisual yet; this allows the splash
            // screen to remain active until the application is ready to render.
            this.RootFrame = new TransitionFrame();
            this.RootFrame.Navigated += this.CompleteInitializePhoneApplication;

            // Handle navigation failures
            this.RootFrame.NavigationFailed += this.RootFrame_NavigationFailed;

            // Ensure we don't initialize again
            this.phoneApplicationInitialized = true;
        }

        // Code to execute if a navigation fails
        private void RootFrame_NavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            if (System.Diagnostics.Debugger.IsAttached)
            {
                // A navigation has failed; break into the debugger
                System.Diagnostics.Debugger.Break();
            }
        }

        #endregion Methods

        #region Nested Types

        [DataContract(IsReference = true)]
        [KnownType(typeof(BiblePlaceMarker))]
        public class BiblePlaceMarkers
        {
            #region Fields

            [DataMember]
            public List<BiblePlaceMarker> bookmarks = new List<BiblePlaceMarker>();
            [DataMember]
            public List<BiblePlaceMarker> history = new List<BiblePlaceMarker>();

            #endregion Fields
        }

        [DataContract]
        public class SerializableDailyPlan
        {
            #region Fields

            [DataMember]
            public DateTime planStartDate = DateTime.Now;
            [DataMember]
            public int planNumber = 0;
            [DataMember]
            public int planDayNumber = 0;

            #endregion Fields
        }
        #endregion Nested Types

        public static SerializableDailyPlan dailyPlan=new SerializableDailyPlan();

    }
}