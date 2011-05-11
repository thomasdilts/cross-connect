using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.IO.IsolatedStorage;
using System.IO;
using System.Xml;
using System.Runtime.Serialization;
using SwordBackend;
///
/// <summary> Distribution License:
/// JSword is free software; you can redistribute it and/or modify it under
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
///
/// Copyright: 2011
///     The copyright to this program is held by Thomas Dilts
///  
namespace CrossConnect
{
    public enum WINDOW_TYPE
    {
        WINDOW_BIBLE,
        WINDOW_BIBLE_NOTES,
        WINDOW_SEARCH,
        WINDOW_HISTORY,
        WINDOW_BOOKMARKS,
    }

    public partial class App : Application
    {
        public struct WindowSettingsSpec
        {
            public int openWindowIndex;
            public bool isAddNewWindowOnly;
            public bool skipWindowSettings;
        }
        public static WindowSettingsSpec windowSettings;

        [DataContract(IsReference = true)]
        [KnownType(typeof(BiblePlaceMarker))]
        public class BiblePlaceMarkers
        {
            [DataMember]
            public List<BiblePlaceMarker> history = new List<BiblePlaceMarker>();
            [DataMember]
            public List<BiblePlaceMarker> bookmarks = new List<BiblePlaceMarker>();
        }
        public static BiblePlaceMarkers placeMarkers = new BiblePlaceMarkers();

        public const int MAX_NUM_WINDOWS = 10;
        public static MainPageSplit mainWindow=null;
        public static InstalledBibles installedBibles = new InstalledBibles();
        public static List<BrowserTitledWindow> openWindows = new List<BrowserTitledWindow>();
        public const string WEB_DIR_ISOLATED = "webtemporary";
        public static event WindowSourceChanged BookMarksChanged;
        public static event WindowSourceChanged HistoryChanged;
        public static int isFirstTimeInMainPageSplit = 0;
        public class HelpStartValues
        {
            public string embeddedFilePath = "";
            public string title = "";
        }
        public static HelpStartValues helpstart=new HelpStartValues();
        
        /// <summary>
        /// Provides easy access to the root frame of the Phone Application.
        /// </summary>
        /// <returns>The root frame of the Phone Application.</returns>
        public PhoneApplicationFrame RootFrame { get; private set; }

        /// <summary>
        /// Constructor for the Application object.
        /// </summary>
        public App()
        {
            // Global handler for uncaught exceptions. 
            UnhandledException += Application_UnhandledException;

            // Show graphics profiling information while debugging.
            if (System.Diagnostics.Debugger.IsAttached)
            {
                // Display the current frame rate counters.
                Application.Current.Host.Settings.EnableFrameRateCounter = true;

                // Show the areas of the app that are being redrawn in each frame.
                //Application.Current.Host.Settings.EnableRedrawRegions = true;

                // Enable non-production analysis visualization mode, 
                // which shows areas of a page that are being GPU accelerated with a colored overlay.
                //Application.Current.Host.Settings.EnableCacheVisualization = true;
            }

            // Standard Silverlight initialization
            InitializeComponent();

            // Phone-specific initialization
            InitializePhoneApplication();
        }
        public static void AddBookmark(int chapterNum, int verseNum)
        {
            placeMarkers.bookmarks.Add(new BiblePlaceMarker(chapterNum, verseNum, DateTime.Now));
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
        public static void RaiseBookmarkChangeEvent()
        {
            if (BookMarksChanged != null)
            {
                BookMarksChanged();
            }
        }
        public static void AddHistory(int chapterNum, int verseNum)
        {
            //stop repeats
            if (placeMarkers.history.Count > 0)
            {
                BiblePlaceMarker last = placeMarkers.history[placeMarkers.history.Count - 1];
                if (last.chapterNum == chapterNum && last.verseNum == verseNum)
                {
                    placeMarkers.history.RemoveAt(placeMarkers.history.Count - 1);
                }
            }
            placeMarkers.history.Add(new BiblePlaceMarker(chapterNum, verseNum, DateTime.Now));
            //don't let this get more then a 200
            if (placeMarkers.history.Count > 200) 
            {
                placeMarkers.history.RemoveAt(0);
            }
            RaiseHistoryChangeEvent();
        }
        public static void AddWindow(string bibleToLoad, int bookNum, int chapterNum, WINDOW_TYPE typeOfWindow, IBrowserTextSource source=null)
        {
            BrowserTitledWindow nextWindow = new BrowserTitledWindow();
            nextWindow.Initialize(bibleToLoad, bookNum, chapterNum, typeOfWindow, source);
            nextWindow.state.curIndex = App.openWindows.Count();
            App.openWindows.Add(nextWindow);
        }
        public void LoadPersistantObjects()
        {
            //make sure some important directories exist.
            var root=IsolatedStorageFile.GetUserStoreForApplication();
            if (!root.DirectoryExists(WEB_DIR_ISOLATED))
            {
                root.CreateDirectory(WEB_DIR_ISOLATED);
            }
            //clear web directory
            string[] filenames = root.GetFileNames(WEB_DIR_ISOLATED+"/*.*");
            foreach (string file in filenames)
            {
                root.DeleteFile(WEB_DIR_ISOLATED+"/"+file);
            }
            openWindows.Clear();
            //get all windows
            for (int i = 0; i < MAX_NUM_WINDOWS; i++)
            {
                string windowsXmlData = "";
                if (IsolatedStorageSettings.ApplicationSettings.TryGetValue<string>("Windows" + i, out windowsXmlData))
                {
                    using (StringReader sr = new StringReader(windowsXmlData))
                    {
                        XmlReaderSettings settings = new XmlReaderSettings();
                        using (XmlReader reader = XmlReader.Create(sr, settings))
                        {
                            Type[] types = new Type[] { typeof(CrossConnect.BrowserTitledWindow.SerializableWindowState), 
                                typeof(SwordBackend.BibleZtextReader.VersePos),
                                typeof(SwordBackend.BibleZtextReader.ChapterPos),
                                typeof(SwordBackend.BibleZtextReader.BookPos),
                                typeof(SwordBackend.BibleZtextReader),
                                typeof(SwordBackend.BibleNoteReader),
                                typeof(BookMarkReader),
                                typeof(HistoryReader),
                                typeof(SearchReader),
                            };
                            DataContractSerializer ser = new DataContractSerializer(typeof(CrossConnect.BrowserTitledWindow.SerializableWindowState), types);
                            BrowserTitledWindow nextWindow = new BrowserTitledWindow();
                            nextWindow.state = (CrossConnect.BrowserTitledWindow.SerializableWindowState)ser.ReadObject(reader);
                            nextWindow.state.source.ReloadSettingsFile();
                            openWindows.Add(nextWindow);
                            nextWindow.Initialize(nextWindow.state.bibleToLoad, nextWindow.state.bookNum, nextWindow.state.chapterNum, nextWindow.state.windowType);
                        }
                    }

                }
                else
                {
                    //no more windows to load.
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
                        Type[] types = new Type[] { typeof(BiblePlaceMarkers), 
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
            //remove all current settings.
            for (int i = 0; i < MAX_NUM_WINDOWS; i++)
            {
                string windowsXmlData = "";
                if (IsolatedStorageSettings.ApplicationSettings.TryGetValue<string>("Windows" + i, out windowsXmlData))
                {
                    IsolatedStorageSettings.ApplicationSettings.Remove("Windows" + i);
                }
                else
                {
                    //no more windows to remove.
                    break;
                }
            }

            //add new settings.
            for (int i = 0; i < openWindows.Count(); i++)
            {
                Type[] types = new Type[] { typeof(CrossConnect.BrowserTitledWindow.SerializableWindowState), 
                    typeof(SwordBackend.BibleZtextReader.VersePos),
                    typeof(SwordBackend.BibleZtextReader.ChapterPos),
                    typeof(SwordBackend.BibleZtextReader.BookPos),
                    typeof(SwordBackend.BibleZtextReader),
                    typeof(SwordBackend.BibleNoteReader),
                    typeof(BookMarkReader),
                    typeof(HistoryReader),
                    typeof(SearchReader),
                };
                DataContractSerializer ser = new DataContractSerializer(typeof(CrossConnect.BrowserTitledWindow.SerializableWindowState), types);
                using (StringWriter sw = new StringWriter())
                {
                    XmlWriterSettings settings = new XmlWriterSettings();
                    settings.OmitXmlDeclaration = true;
                    settings.Indent = true;
                    settings.NamespaceHandling = NamespaceHandling.OmitDuplicates;
                    //settings.NewLineOnAttributes = true;
                    using (XmlWriter writer = XmlWriter.Create(sw, settings))
                        ser.WriteObject(writer, openWindows[i].state);
                    IsolatedStorageSettings.ApplicationSettings.Add("Windows" + i, sw.ToString());
                }
            }
            Type[] types2 = new Type[] { typeof(BiblePlaceMarkers), 
                                typeof(BiblePlaceMarker),
                            };
            DataContractSerializer ser2 = new DataContractSerializer(typeof(BiblePlaceMarkers), types2);
            using (StringWriter sw = new StringWriter())
            {
                XmlWriterSettings settings = new XmlWriterSettings();
                settings.OmitXmlDeclaration = true;
                settings.Indent = true;
                settings.NamespaceHandling = NamespaceHandling.OmitDuplicates;
                //settings.NewLineOnAttributes = true;
                using (XmlWriter writer = XmlWriter.Create(sw, settings))
                    ser2.WriteObject(writer, placeMarkers);
                IsolatedStorageSettings.ApplicationSettings["BiblePlaceMarkers"] = sw.ToString();
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
        // Code to execute when the application is launching (eg, from Start)
        // This code will not execute when the application is reactivated
        private void Application_Launching(object sender, LaunchingEventArgs e)
        {
            LoadPersistantObjects();
        }

        // Code to execute when the application is activated (brought to foreground)
        // This code will not execute when the application is first launched
        private void Application_Activated(object sender, ActivatedEventArgs e)
        {
            LoadPersistantObjects();
        }

        // Code to execute when the application is deactivated (sent to background)
        // This code will not execute when the application is closing
        private void Application_Deactivated(object sender, DeactivatedEventArgs e)
        {
            SavePersistantObjects();
        }

        // Code to execute when the application is closing (eg, user hit Back)
        // This code will not execute when the application is deactivated
        private void Application_Closing(object sender, ClosingEventArgs e)
        {
            SavePersistantObjects();
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

        // Code to execute on Unhandled Exceptions
        private void Application_UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
        {
            if (System.Diagnostics.Debugger.IsAttached)
            {
                // An unhandled exception has occurred; break into the debugger
                System.Diagnostics.Debugger.Break();
            } 
            
            MessageBoxResult result = MessageBox.Show(Translations.translate("An error occured. Do you want to completely erase the memory for this program?"), "", MessageBoxButton.OKCancel);
            if (result == MessageBoxResult.OK)
            {
                IsolatedStorageFile root = IsolatedStorageFile.GetUserStoreForApplication();
                root.Remove();
                System.IO.IsolatedStorage.IsolatedStorageSettings.ApplicationSettings.Clear();
            }

        }

        #region Phone application initialization

        // Avoid double-initialization
        private bool phoneApplicationInitialized = false;

        // Do not add any additional code to this method
        private void InitializePhoneApplication()
        {
            if (phoneApplicationInitialized)
                return;

            // Create the frame but don't set it as RootVisual yet; this allows the splash
            // screen to remain active until the application is ready to render.
            RootFrame = new TransitionFrame();
            RootFrame.Navigated += CompleteInitializePhoneApplication;

            // Handle navigation failures
            RootFrame.NavigationFailed += RootFrame_NavigationFailed;

            // Ensure we don't initialize again
            phoneApplicationInitialized = true;
        }

        // Do not add any additional code to this method
        private void CompleteInitializePhoneApplication(object sender, NavigationEventArgs e)
        {
            // Set the root visual to allow the application to render
            if (RootVisual != RootFrame)
                RootVisual = RootFrame;

            // Remove this handler since it is no longer needed
            RootFrame.Navigated -= CompleteInitializePhoneApplication;
        }

        #endregion
    }
}