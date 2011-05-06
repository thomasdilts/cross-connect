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
using System.Windows.Shapes;
using System.Text;
using System.Threading;
using Microsoft.Phone.Controls;
using SwordBackend;
using System.Runtime.Serialization;
using System.Windows.Navigation;
using System.IO.IsolatedStorage;
using System.IO;
using System.Windows.Media.Imaging;
using System.Diagnostics;

namespace CrossConnect
{
    
    public partial class BrowserTitledWindow : UserControl
    {
        // An event that clients can use to be notified whenever the
        // elements of the list change.
        public event EventHandler HitButtonBigger;
        public event EventHandler HitButtonSmaller;
        public event EventHandler HitButtonClose;

        /// <summary>
        /// I was forced to make this class just for serialization because a "UserControl" 
        /// cannot be serialized.
        /// </summary>
        [DataContract]
        public class SerializableWindowState
        {
            [DataMember]
            public IBrowserTextSource source = null;
            [DataMember]
            public int curIndex = 0;
            [DataMember]
            public int numRowsIown = 1;
            [DataMember]
            public string bibleToLoad = "";
            [DataMember]
            public int bookNum = 1;
            [DataMember]
            public int chapterNum = 1;
            [DataMember]
            public int verseNum = 1;
            [DataMember]
            public WINDOW_TYPE windowType = WINDOW_TYPE.WINDOW_BIBLE;
            [DataMember]
            public bool isSynchronized=true;
            [DataMember]
            public double htmlFontSize = 10;
        }
        public SerializableWindowState state = new SerializableWindowState();

        private string lastFileName="";
        public BrowserTitledWindow()
        {
            InitializeComponent();
        }
        public void SynchronizeWindow(int chapterNum, int verseNum)
        {
            if (state.isSynchronized)
            {
                this.state.chapterNum = chapterNum;
                this.state.verseNum = verseNum;
                UpdateBrowser();
            }
        }
        public void Initialize(string bibleToLoad, int bookNum, int chapterNum, WINDOW_TYPE windowType)
        {
            state.bibleToLoad = bibleToLoad;
            state.bookNum = bookNum;
            state.chapterNum = chapterNum;
            state.windowType = windowType;
            foreach (var book in App.installedBibles.installedBibles)
            {
                if (book.Value.sbmd.internalName.Equals(bibleToLoad))
                {
                    string bookPath = book.Value.sbmd.getCetProperty(ConfigEntryType.DATA_PATH).ToString().Substring(2);
                    bool isIsoEncoding = !book.Value.sbmd.getCetProperty(ConfigEntryType.ENCODING).Equals("UTF-8");
                    switch (windowType)
                    {
                        case WINDOW_TYPE.WINDOW_BIBLE:
                            try
                            {
                                state.source = new BibleZtextReader(bookPath, ((Language)book.Value.sbmd.getCetProperty(ConfigEntryType.LANG)).Code, isIsoEncoding);
                            }
                            catch (Exception)
                            {
                            }
                            break;
                        case WINDOW_TYPE.WINDOW_BIBLE_NOTES:
                            try
                            {
                                state.source = new BibleNoteReader(bookPath, ((Language)book.Value.sbmd.getCetProperty(ConfigEntryType.LANG)).Code, isIsoEncoding);
                            }
                            catch (Exception)
                            {
                            }
                            break;
                        case WINDOW_TYPE.WINDOW_BOOKMARKS:
                            try
                            {
                                state.source = new BookMarkReader(bookPath, ((Language)book.Value.sbmd.getCetProperty(ConfigEntryType.LANG)).Code, isIsoEncoding);
                            }
                            catch (Exception)
                            {
                            }
                            break;
                        case WINDOW_TYPE.WINDOW_HISTORY:
                            try
                            {
                                state.source = new HistoryReader(bookPath, ((Language)book.Value.sbmd.getCetProperty(ConfigEntryType.LANG)).Code, isIsoEncoding);
                            }
                            catch (Exception)
                            {
                            }
                            break;
                    }
                    
                    break;
                }
            } 
        }
        private void webBrowser1_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateBrowser();

            //get all the right images
            //figure out if this is a light color
            var color = (Color)Application.Current.Resources["PhoneBackgroundColor"];
            int lightColorCount = (color.R > 0x80 ? 1 : 0) + (color.G > 0x80 ? 1 : 0) + (color.B > 0x80 ? 1 : 0);
            string colorDir = lightColorCount >= 2 ? "light" : "dark";

            if (state.source.isPageable)
            {
                butPrevious.Image = getImage("/Images/" + colorDir + "/appbar.people.2.rest.png");
                butPrevious.PressedImage = getImage("/Images/" + colorDir + "/appbar.people.2.pressed.rest.png");

                butNext.Image = getImage("/Images/" + colorDir + "/appbar.people.1.rest.png");
                butNext.PressedImage = getImage("/Images/" + colorDir + "/appbar.people.1.rest.pressed.png");
            }
            else
            {
                butPrevious.Image = null;
                butPrevious.PressedImage = null;

                butNext.Image = null;
                butNext.PressedImage = null;
            }

            butMenu.Image=getImage("/Images/" + colorDir + "/appbar.menu.rest.png");
            butMenu.PressedImage=getImage("/Images/" + colorDir + "/appbar.menu.rest.pressed.png");

            butLarger.Image=getImage("/Images/" + colorDir + "/appbar.feature.search.rest.png");
            butLarger.PressedImage=getImage("/Images/" + colorDir + "/appbar.feature.search.rest.pressed.png");

            butSmaller.Image=getImage("/Images/" + colorDir + "/appbar.minus.rest.png");
            butSmaller.PressedImage=getImage("/Images/" + colorDir + "/appbar.minus.rest.pressed.png");

            butClose.Image=getImage("/Images/" + colorDir + "/appbar.cancel.rest.png");
            butClose.PressedImage=getImage("/Images/" + colorDir + "/appbar.cancel.rest.pressed.png");

            if (!state.source.isSynchronizeable)
            {
                butLink.Image = null;
                butLink.PressedImage = null;
            }

            if (state != null && state.source != null)
            {
                state.source.registerUpdateEvent(source_Changed, true);
            }

        }
        private void webBrowser1_Unloaded(object sender, RoutedEventArgs e)
        {
            if (state != null && state.source != null)
            {
                state.source.registerUpdateEvent(source_Changed, false);
            }
        }
        private void source_Changed()
        {
            UpdateBrowser();
        }
        private ImageSource getImage(string path)
        {
            Uri uri = new Uri(path, UriKind.Relative);
            return (ImageSource)new BitmapImage(uri);
        }
        private void UpdateBrowser()
        {
            
            if (state.source != null)
            {
                var root=IsolatedStorageFile.GetUserStoreForApplication();
                //Must change the file name, otherwise the browser may or may not update.
                string file = "web" + (int)(new Random().NextDouble() * 10000) + ".html";
                if (root.FileExists(App.WEB_DIR_ISOLATED + "/" + lastFileName))
                {
                    root.DeleteFile(App.WEB_DIR_ISOLATED + "/" + lastFileName);
                }
                lastFileName = file;
                IsolatedStorageFileStream fs = root.CreateFile(App.WEB_DIR_ISOLATED + "/" + lastFileName);
                System.IO.StreamWriter tw = new System.IO.StreamWriter(fs);
                tw.Write(state.source.GetChapterHtml(
                    state.chapterNum,
                    GetBrowserColor("PhoneBackgroundColor"),
                    GetBrowserColor("PhoneForegroundColor"),
                    GetBrowserColor("PhoneAccentColor"),
                    state.htmlFontSize));
                tw.Close();
                fs.Close();
                webBrowser1.FontSize = state.htmlFontSize;
                webBrowser1.Base = App.WEB_DIR_ISOLATED;

                Uri source = new Uri(file + "#CHAP_" + state.chapterNum + "_VERS_" + state.verseNum, UriKind.Relative);
                webBrowser1.Navigate(source);

                writeTitle();
                //update the sync button image
                state.isSynchronized = !state.isSynchronized;
                butLink_Click(null,null);

                System.Windows.Threading.DispatcherTimer tmr = new System.Windows.Threading.DispatcherTimer();
                tmr.Interval = TimeSpan.FromSeconds(1);
                tmr.Tick += OnTimerTick;
                tmr.Start(); 
            }
        }
        public static string GetBrowserColor(string sourceResource)
        {
            var color = (Color)Application.Current.Resources[sourceResource];
            return "#" + color.ToString().Substring(3, 6);
        }


        private void webBrowser1_ScriptNotify(object sender, Microsoft.Phone.Controls.NotifyEventArgs e)
        {
            string[] chapterVerse = e.Value.ToString().Split("_".ToArray());
            int chapterNum= -1;
            int verseNum = -1;
            for(int i=0;i<chapterVerse.Length;i+=2)
            {
                switch(chapterVerse[i])
                {
                    case "CHAP":
                        int.TryParse(chapterVerse[i + 1], out chapterNum);
                        break;
                    case "VERS":
                        int.TryParse(chapterVerse[i + 1], out verseNum);
                        break;
                }
            }
            if (chapterNum >= 0 && verseNum >= 0)
            {
                if (state.source.isLocalChangeDuringLink)
                {
                    state.chapterNum = chapterNum;
                    state.verseNum = verseNum;
                    writeTitle();
                }
                App.SynchronizeAllWindows(chapterNum, verseNum, state.curIndex);

                App.AddHistory(chapterNum, verseNum);
            }
        }
        private void writeTitle()
        {
            int bookNum;
            int relChaptNum;
            string fullName;
            string titleText;
            state.source.getInfo(state.chapterNum, state.verseNum, out bookNum, out relChaptNum, out fullName, out titleText);
            title.Text = titleText;
        }
        private SlideTransition SlideTransitionElement(string mode)
        {
            SlideTransitionMode slideTransitionMode = (SlideTransitionMode)Enum.Parse(typeof(SlideTransitionMode), mode, false);
            return new SlideTransition { Mode = slideTransitionMode };
            //SlideTransitionMode slideTransitionMode = (SlideTransitionMode)Enum.Parse(typeof(SlideTransitionMode), mode, false);
            //return new SlideTransition { Mode = slideTransitionMode };
        }

        private void butPrevious_Click(object sender, RoutedEventArgs e)
        {
            state.verseNum = 0;
            state.chapterNum--;
            if (state.chapterNum < 0)
            {
                state.chapterNum = BibleZtextReader.CHAPTERS_IN_BIBLE-1;
            }
            string mode = "SlideRightFadeOut";
            TransitionElement transitionElement = null;

            transitionElement = SlideTransitionElement(mode);

            ITransition transition = transitionElement.GetTransition(this);

            transition.Completed += delegate
            {
                UpdateBrowser();
                transition.Stop();
                mode = "SlideRightFadeIn";
                transitionElement = null;

                transitionElement = SlideTransitionElement(mode);

                transition = transitionElement.GetTransition(this);
                transition.Completed += delegate
                {
                    transition.Stop();
                };
                transition.Begin();
            };
            transition.Begin();
        }

        private void butNext_Click(object sender, RoutedEventArgs e)
        {
            state.chapterNum++;
            state.verseNum = 0;
            if (state.chapterNum >= (BibleZtextReader.CHAPTERS_IN_BIBLE-1))
            {
                state.chapterNum = 0;
            } 
            string mode = "SlideLeftFadeOut";
            TransitionElement transitionElement = null;

            transitionElement = SlideTransitionElement(mode);

            PhoneApplicationPage phoneApplicationPage = (PhoneApplicationPage)(((PhoneApplicationFrame)Application.Current.RootVisual)).Content;
            ITransition transition = transitionElement.GetTransition(this);
            transition.Completed += delegate
            {
                UpdateBrowser();
                transition.Stop();
                mode = "SlideLeftFadeIn";
                transitionElement = null;

                transitionElement = SlideTransitionElement(mode);

                transition = transitionElement.GetTransition(this);
                transition.Completed += delegate
                {
                    transition.Stop();
                };
                transition.Begin();
            };
            transition.Begin();
        }

        private void butMenu_Click(object sender, RoutedEventArgs e)
        {
            App.windowSettings.openWindowIndex = state.curIndex;
            App.windowSettings.isAddNewWindowOnly = false;
            MainPageSplit parent = (MainPageSplit)((Grid)((Grid)this.Parent).Parent).Parent;
            App.windowSettings.skipWindowSettings = false;
            parent.NavigationService.Navigate(new Uri("/WindowSettings.xaml", UriKind.Relative));
        }

        private void butLink_Click(object sender, RoutedEventArgs e)
        {
            if (state.source.isSynchronizeable)
            {
                //get all the right images
                //figure out if this is a light color
                var color = (Color)Application.Current.Resources["PhoneBackgroundColor"];
                int lightColorCount = (color.R > 0x80 ? 1 : 0) + (color.G > 0x80 ? 1 : 0) + (color.B > 0x80 ? 1 : 0);
                string colorDir = lightColorCount >= 2 ? "light" : "dark";

                state.isSynchronized = !state.isSynchronized;
                if (state.isSynchronized)
                {
                    butLink.PressedImage = getImage("/Images/" + colorDir + "/appbar.linkto.rest.png");
                    butLink.Image = getImage("/Images/" + colorDir + "/appbar.linkto.rest.pressed.png");

                }
                else
                {
                    butLink.Image = getImage("/Images/" + colorDir + "/appbar.linkto.rest.png");
                    butLink.PressedImage = getImage("/Images/" + colorDir + "/appbar.linkto.rest.pressed.png");
                }
            }
        }
        private void butLarger_Click(object sender, RoutedEventArgs e)
        {
            state.numRowsIown++;
            ShowSizeButtons();
            if (HitButtonBigger != null)
                HitButtonBigger(this, e);
        }

        private void butSmaller_Click(object sender, RoutedEventArgs e)
        {
            state.numRowsIown--;
            ShowSizeButtons();
            if (HitButtonSmaller != null)
                HitButtonSmaller(this, e);
        }

        private void butClose_Click(object sender, RoutedEventArgs e)
        {
            var root = IsolatedStorageFile.GetUserStoreForApplication();
            if (root.FileExists(App.WEB_DIR_ISOLATED + "/" + lastFileName))
            {
                root.DeleteFile(App.WEB_DIR_ISOLATED + "/" + lastFileName);
            }
            if (HitButtonClose != null)
                HitButtonClose(this, e);
        }
        public void ShowSizeButtons(bool isShow=true)
        {
            if (!isShow)
            {
                butLarger.Visibility = System.Windows.Visibility.Collapsed;
                butSmaller.Visibility = System.Windows.Visibility.Collapsed;
                butClose.Visibility = System.Windows.Visibility.Collapsed;
            }
            else
            {
                if (state.numRowsIown > 1)
                {
                    butSmaller.Visibility = System.Windows.Visibility.Visible;
                }
                else
                {
                    butSmaller.Visibility = System.Windows.Visibility.Collapsed;
                }
                butLarger.Visibility = System.Windows.Visibility.Visible;
                butClose.Visibility = System.Windows.Visibility.Visible;
            }
        }

        void OnTimerTick(object sender, EventArgs e)
        {
            //we must delay updating of this webbrowser...
            ((System.Windows.Threading.DispatcherTimer)sender).Stop();
            try
            {
                Uri source = new Uri(lastFileName + "#CHAP_" + state.chapterNum + "_VERS_" + state.verseNum, UriKind.Relative);
                webBrowser1.Navigate(source);
            }
            catch (Exception)
            { }
        }
    }
}
