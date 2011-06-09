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
/// <copyright file="BrowserTitledWindow.xaml.cs" company="Thomas Dilts">
///     Thomas Dilts. All rights reserved.
/// </copyright>
/// <author>Thomas Dilts</author>
namespace CrossConnect
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.IO.IsolatedStorage;
    using System.Linq;
    using System.Net;
    using System.Runtime.Serialization;
    using System.Text;
    using System.Threading;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using System.Windows.Media.Imaging;
    using System.Windows.Navigation;
    using System.Windows.Shapes;

    using Microsoft.Phone.Controls;
    using Microsoft.Phone.Shell;

    using SwordBackend;

    public partial class BrowserTitledWindow : UserControl
    {
        #region Fields

        public SerializableWindowState state = new SerializableWindowState();

        private string lastFileName = string.Empty;
        private DateTime lastManipulationKillTime = DateTime.Now;
        private System.Windows.Threading.DispatcherTimer manipulationTimer = null;
        private ManipulationCompletedEventArgs manipulationToProcess = null;

        #endregion Fields

        #region Constructors

        public BrowserTitledWindow()
        {
            InitializeComponent();
        }

        #endregion Constructors

        #region Events

        // An event that clients can use to be notified whenever the
        // elements of the list change.
        public event EventHandler HitButtonBigger;

        public event EventHandler HitButtonClose;

        public event EventHandler HitButtonSmaller;

        #endregion Events

        #region Methods

        public static string GetBrowserColor(string sourceResource)
        {
            var color = (Color)Application.Current.Resources[sourceResource];
            return "#" + color.ToString().Substring(3, 6);
        }

        public void CallbackFromUpdate(string createdFileName)
        {
            this.lastFileName = createdFileName;
            //webBrowser1.FontSize = this.state.htmlFontSize;
            webBrowser1.Base = App.WEB_DIR_ISOLATED;

            Uri source = new Uri(this.lastFileName, UriKind.Relative);
            if (this.state.source.IsSynchronizeable || this.state.source.IsLocalChangeDuringLink)
            {
                int bookNum;
                int absoluteChaptNum;
                int relChaptNum;
                int verseNum;
                string fullName;
                string titleText;
                this.state.source.GetInfo(out bookNum, out absoluteChaptNum, out relChaptNum,out verseNum, out fullName, out titleText);
                source = new Uri(this.lastFileName + "#CHAP_" + absoluteChaptNum + "_VERS_" + verseNum, UriKind.Relative);
            }

            webBrowser1.Navigate(source);

            this.WriteTitle();

            // update the sync button image
            this.state.isSynchronized = !this.state.isSynchronized;
            this.ButLink_Click(null, null);

            if (this.state.source.IsSynchronizeable || this.state.source.IsLocalChangeDuringLink)
            {
                //The window wont show the correct verse if we dont wait a few seconds before showing it.
                System.Windows.Threading.DispatcherTimer tmr = new System.Windows.Threading.DispatcherTimer();
                tmr.Interval = TimeSpan.FromSeconds(state.isResume ? 2.5 : 1);
                state.isResume = false;
                tmr.Tick += this.OnTimerTick;
                tmr.Start();
            }
        }

        public void Initialize(string bibleToLoad, WINDOW_TYPE windowType, IBrowserTextSource source = null)
        {
            this.state.bibleToLoad = bibleToLoad;
            this.state.windowType = windowType;
            if (source != null)
            {
                this.state.source = source;
            }
            else
            {
                foreach (var book in App.installedBibles.installedBibles)
                {
                    if (book.Value.sbmd.internalName.Equals(bibleToLoad))
                    {
                        string bookPath = book.Value.sbmd.getCetProperty(ConfigEntryType.A_DATA_PATH).ToString().Substring(2);
                        bool isIsoEncoding = !book.Value.sbmd.getCetProperty(ConfigEntryType.ENCODING).Equals("UTF-8");
                        switch (windowType)
                        {
                            case WINDOW_TYPE.WINDOW_BIBLE:
                                try
                                {
                                    this.state.source = new BibleZtextReader(bookPath, ((Language)book.Value.sbmd.getCetProperty(ConfigEntryType.LANG)).Code, isIsoEncoding);
                                }
                                catch (Exception)
                                {
                                }

                                break;
                            case WINDOW_TYPE.WINDOW_BIBLE_NOTES:
                                try
                                {
                                    this.state.source = new BibleNoteReader(bookPath, ((Language)book.Value.sbmd.getCetProperty(ConfigEntryType.LANG)).Code, isIsoEncoding, Translations.translate("Notes"));
                                }
                                catch (Exception)
                                {
                                }

                                break;
                            case WINDOW_TYPE.WINDOW_BOOKMARKS:
                                try
                                {
                                    this.state.source = new BookMarkReader(bookPath, ((Language)book.Value.sbmd.getCetProperty(ConfigEntryType.LANG)).Code, isIsoEncoding);
                                }
                                catch (Exception)
                                {
                                }

                                break;
                            case WINDOW_TYPE.WINDOW_HISTORY:
                                try
                                {
                                    this.state.source = new HistoryReader(bookPath, ((Language)book.Value.sbmd.getCetProperty(ConfigEntryType.LANG)).Code, isIsoEncoding);
                                }
                                catch (Exception)
                                {
                                }

                                break;
                            case WINDOW_TYPE.WINDOW_DAILY_PLAN:
                                try
                                {
                                    this.state.source = new DailyPlanReader(bookPath, ((Language)book.Value.sbmd.getCetProperty(ConfigEntryType.LANG)).Code, isIsoEncoding);
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
        }

        public void ShowSizeButtons(bool isShow = true)
        {
            if (!isShow)
            {
                butLarger.Visibility = System.Windows.Visibility.Collapsed;
                butSmaller.Visibility = System.Windows.Visibility.Collapsed;
                butClose.Visibility = System.Windows.Visibility.Collapsed;
            }
            else
            {
                if (this.state.numRowsIown > 1)
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

        public void SynchronizeWindow(int chapterNum, int verseNum)
        {
            if (this.state.isSynchronized && this.state.source.IsSynchronizeable)
            {
                this.state.source.moveChapterVerse(chapterNum, verseNum,false);
                this.UpdateBrowser();
            }
        }

        public void UpdateBrowser()
        {
            if (this.state.source != null && this.Parent != null)
            {
                double fontSizeMultiplier = 1;
                if (this.Parent != null && ((Grid)(Grid)this.Parent).Parent != null)
                {
                    MainPageSplit parent = (MainPageSplit)((Grid)(Grid)this.Parent).Parent;
                    if (parent.Orientation == PageOrientation.Landscape
                        || parent.Orientation == PageOrientation.LandscapeLeft
                        || parent.Orientation == PageOrientation.LandscapeRight)
                    {
                        //we must adjust the font size for the new orientation. otherwise the font is too big.
                        fontSizeMultiplier = parent.ActualHeight / parent.ActualWidth;
                    }
                }
                var backcolor=GetBrowserColor("PhoneBackgroundColor");
                var forecolor=GetBrowserColor("PhoneForegroundColor");
                var accentcolor = GetBrowserColor("PhoneAccentColor");

                GetHtmlAsynchronously(backcolor,
                            forecolor,
                            accentcolor,
                            this.state.htmlFontSize * fontSizeMultiplier,
                            App.WEB_DIR_ISOLATED + "/" + this.lastFileName);
            }
        }

        private void border1_ManipulationCompleted(object sender, ManipulationCompletedEventArgs e)
        {
            DoManipulation(e);
        }

        private void ButClose_Click(object sender, RoutedEventArgs e)
        {
            killManipulation();
            var root = IsolatedStorageFile.GetUserStoreForApplication();

            if (root.FileExists(App.WEB_DIR_ISOLATED + "/" + this.lastFileName))
            {
                root.DeleteFile(App.WEB_DIR_ISOLATED + "/" + this.lastFileName);
            }

            if (this.HitButtonClose != null)
            {
                this.HitButtonClose(this, e);
            }
        }

        private void butClose_ManipulationCompleted(object sender, ManipulationCompletedEventArgs e)
        {
            DoManipulation(e);
        }

        private void ButLarger_Click(object sender, RoutedEventArgs e)
        {
            killManipulation();
            this.state.numRowsIown++;
            ShowSizeButtons();
            if (this.HitButtonBigger != null)
            {
                this.HitButtonBigger(this, e);
            }
        }

        private void butLarger_ManipulationCompleted(object sender, ManipulationCompletedEventArgs e)
        {
            DoManipulation(e);
        }

        private void ButLink_Click(object sender, RoutedEventArgs e)
        {
            killManipulation();
            if (this.state.source.IsSynchronizeable)
            {
                // get all the right images
                // figure out if this is a light color
                var color = (Color)Application.Current.Resources["PhoneBackgroundColor"];
                int lightColorCount = (color.R > 0x80 ? 1 : 0) + (color.G > 0x80 ? 1 : 0) + (color.B > 0x80 ? 1 : 0);
                string colorDir = lightColorCount >= 2 ? "light" : "dark";

                this.state.isSynchronized = !this.state.isSynchronized;
                if (this.state.isSynchronized)
                {
                    butLink.PressedImage = this.GetImage("/Images/" + colorDir + "/appbar.linkto.rest.png");
                    butLink.Image = this.GetImage("/Images/" + colorDir + "/appbar.linkto.rest.pressed.png");
                }
                else
                {
                    butLink.Image = this.GetImage("/Images/" + colorDir + "/appbar.linkto.rest.png");
                    butLink.PressedImage = this.GetImage("/Images/" + colorDir + "/appbar.linkto.rest.pressed.png");
                }
            }
        }

        private void butLink_ManipulationCompleted(object sender, ManipulationCompletedEventArgs e)
        {
            DoManipulation(e);
        }

        private void ButMenu_Click(object sender, RoutedEventArgs e)
        {
            killManipulation();

            PhoneApplicationService.Current.State["isAddNewWindowOnly"] = false;
            PhoneApplicationService.Current.State["skipWindowSettings"] = false;
            PhoneApplicationService.Current.State["openWindowIndex"] = this.state.curIndex;
            PhoneApplicationService.Current.State["InitializeWindowSettings"] = true;

            MainPageSplit parent = (MainPageSplit)((Grid)(Grid)this.Parent).Parent;
            parent.NavigationService.Navigate(new Uri("/WindowSettings.xaml", UriKind.Relative));
        }

        private void butMenu_ManipulationCompleted(object sender, ManipulationCompletedEventArgs e)
        {
            DoManipulation(e);
        }

        private void ButNext_Click(object sender, RoutedEventArgs e)
        {
            killManipulation();
            this.state.source.moveNext();

            string mode = "SlideLeftFadeOut";
            TransitionElement transitionElement = null;

            transitionElement = this.SlideTransitionElement(mode);

            PhoneApplicationPage phoneApplicationPage = (PhoneApplicationPage)((PhoneApplicationFrame)Application.Current.RootVisual).Content;
            ITransition transition = transitionElement.GetTransition(this);
            transition.Completed += delegate
            {
                this.UpdateBrowser();
                transition.Stop();
                mode = "SlideLeftFadeIn";
                transitionElement = null;

                transitionElement = this.SlideTransitionElement(mode);

                transition = transitionElement.GetTransition(this);
                transition.Completed += delegate
                {
                    transition.Stop();
                };
                transition.Begin();
            };
            transition.Begin();
        }

        private void ButPrevious_Click(object sender, RoutedEventArgs e)
        {
            killManipulation();
            this.state.source.movePrevious();

            string mode = "SlideRightFadeOut";
            TransitionElement transitionElement = null;

            transitionElement = this.SlideTransitionElement(mode);

            ITransition transition = transitionElement.GetTransition(this);

            transition.Completed += delegate
            {
                this.UpdateBrowser();
                transition.Stop();
                mode = "SlideRightFadeIn";
                transitionElement = null;

                transitionElement = this.SlideTransitionElement(mode);

                transition = transitionElement.GetTransition(this);
                transition.Completed += delegate
                {
                    transition.Stop();
                };
                transition.Begin();
            };
            transition.Begin();
        }

        private void ButSmaller_Click(object sender, RoutedEventArgs e)
        {
            killManipulation();
            this.state.numRowsIown--;
            ShowSizeButtons();
            if (this.HitButtonSmaller != null)
            {
                this.HitButtonSmaller(this, e);
            }
        }

        private void butSmaller_ManipulationCompleted(object sender, ManipulationCompletedEventArgs e)
        {
            DoManipulation(e);
        }

        private void DoManipulation(ManipulationCompletedEventArgs e)
        {
            if (manipulationTimer == null && lastManipulationKillTime.AddMilliseconds(400).CompareTo(DateTime.Now) < 0)
            {
                manipulationToProcess = e;
                //start timer
                manipulationTimer = new System.Windows.Threading.DispatcherTimer();
                manipulationTimer.Interval = TimeSpan.FromMilliseconds(200);
                manipulationTimer.Tick += this.DoManipulationTimerTick;
                manipulationTimer.Start();
            }
        }

        private void DoManipulationTimerTick(object sender, EventArgs e)
        {
            // we must delay updating of this webbrowser...
            killManipulation();

            System.Windows.Point pt = manipulationToProcess.FinalVelocities.LinearVelocity;
            if (pt.X > 700)
            {
                //previous
                ButPrevious_Click(null, null);
            }
            else if (pt.X < -700)
            {
                //next
                ButNext_Click(null, null);
            }
        }

        private void GetHtmlAsynchronously( string htmlBackgroundColor, string htmlForegroundColor, string htmlPhoneAccentColor, double htmlFontSize, string fileErase)
        {
            ThreadPool.QueueUserWorkItem(_ =>
            {
                try
                {
                    string createdFileName=this.state.source.putHtmlTofile(
                        htmlBackgroundColor,
                        htmlForegroundColor,
                        htmlPhoneAccentColor,
                        htmlFontSize,
                        fileErase,
                        App.WEB_DIR_ISOLATED);

                    Deployment.Current.Dispatcher.BeginInvoke(() => CallbackFromUpdate(createdFileName));
                }
                catch (Exception)
                {
                    Deployment.Current.Dispatcher.BeginInvoke(() => CallbackFromUpdate(""));
                    return;
                }
            });
        }

        private ImageSource GetImage(string path)
        {
            Uri uri = new Uri(path, UriKind.Relative);
            return (ImageSource)new BitmapImage(uri);
        }

        private void grid1_ManipulationCompleted(object sender, ManipulationCompletedEventArgs e)
        {
            DoManipulation(e);
        }

        private void killManipulation()
        {
            if (manipulationTimer != null)
            {
                manipulationTimer.Stop();
                manipulationTimer = null;
            }
            lastManipulationKillTime = DateTime.Now;
        }

        private void OnTimerTick(object sender, EventArgs e)
        {
            // we must delay updating of this webbrowser...
            ((System.Windows.Threading.DispatcherTimer)sender).Stop();
            try
            {
                int bookNum;
                int absoluteChaptNum;
                int relChaptNum;
                int verseNum;
                string fullName;
                string titleText;
                this.state.source.GetInfo(out bookNum, out absoluteChaptNum, out relChaptNum,out verseNum, out fullName, out titleText);
                Uri source = new Uri(this.lastFileName + "#CHAP_" + absoluteChaptNum + "_VERS_" + verseNum, UriKind.Relative);
                webBrowser1.Navigate(source);
            }
            catch (Exception)
            {
            }
        }

        private SlideTransition SlideTransitionElement(string mode)
        {
            SlideTransitionMode slideTransitionMode = (SlideTransitionMode)Enum.Parse(typeof(SlideTransitionMode), mode, false);
            return new SlideTransition { Mode = slideTransitionMode };
        }

        private void Source_Changed()
        {
            this.UpdateBrowser();
        }

        private void title_ManipulationCompleted(object sender, ManipulationCompletedEventArgs e)
        {
            DoManipulation(e);
        }

        private void WebBrowser1_Loaded(object sender, RoutedEventArgs e)
        {
            this.UpdateBrowser();

            // get all the right images
            // figure out if this is a light color
            var color = (Color)Application.Current.Resources["PhoneBackgroundColor"];
            int lightColorCount = (color.R > 0x80 ? 1 : 0) + (color.G > 0x80 ? 1 : 0) + (color.B > 0x80 ? 1 : 0);
            string colorDir = lightColorCount >= 2 ? "light" : "dark";

            if (this.state.source.IsPageable)
            {
                butPrevious.Image = this.GetImage("/Images/" + colorDir + "/appbar.prev.rest.png");
                butPrevious.PressedImage = this.GetImage("/Images/" + colorDir + "/appbar.prev.rest.press.png");

                butNext.Image = this.GetImage("/Images/" + colorDir + "/appbar.next.rest.png");
                butNext.PressedImage = this.GetImage("/Images/" + colorDir + "/appbar.next.rest.press.png");
            }
            else
            {
                butPrevious.Image = null;
                butPrevious.PressedImage = null;

                butNext.Image = null;
                butNext.PressedImage = null;
            }

            butMenu.Image = this.GetImage("/Images/" + colorDir + "/appbar.menu.rest.png");
            butMenu.PressedImage = this.GetImage("/Images/" + colorDir + "/appbar.menu.rest.pressed.png");

            butLarger.Image = this.GetImage("/Images/" + colorDir + "/appbar.feature.search.rest.png");
            butLarger.PressedImage = this.GetImage("/Images/" + colorDir + "/appbar.feature.search.rest.pressed.png");

            butSmaller.Image = this.GetImage("/Images/" + colorDir + "/appbar.minus.rest.png");
            butSmaller.PressedImage = this.GetImage("/Images/" + colorDir + "/appbar.minus.rest.pressed.png");

            butClose.Image = this.GetImage("/Images/" + colorDir + "/appbar.cancel.rest.png");
            butClose.PressedImage = this.GetImage("/Images/" + colorDir + "/appbar.cancel.rest.pressed.png");

            if (!this.state.source.IsSynchronizeable)
            {
                butLink.Image = null;
                butLink.PressedImage = null;
            }

            if (this.state != null && this.state.source != null)
            {
                this.state.source.RegisterUpdateEvent(this.Source_Changed, true);
            }
        }

        private void WebBrowser1_ScriptNotify(object sender, Microsoft.Phone.Controls.NotifyEventArgs e)
        {
            string[] chapterVerse = e.Value.ToString().Split("_".ToArray());
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
                }
            }

            if (chapterNum >= 0 && verseNum >= 0)
            {
                if (this.state.source.IsLocalChangeDuringLink)
                {
                    this.state.source.moveChapterVerse(chapterNum, verseNum, true);
                    this.WriteTitle();
                }

                App.SynchronizeAllWindows(chapterNum, verseNum, this.state.curIndex);

                App.AddHistory(chapterNum, verseNum);
            }
        }

        private void WebBrowser1_Unloaded(object sender, RoutedEventArgs e)
        {
            if (this.state != null && this.state.source != null)
            {
                this.state.source.RegisterUpdateEvent(this.Source_Changed, false);
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
            this.state.source.GetInfo(out bookNum, out absoluteChaptNum, out relChaptNum, out verseNum, out fullName, out titleText);
            title.Text = titleText + " - " + this.state.bibleToLoad;
        }

        #endregion Methods

        #region Nested Types

        /// <summary>
        /// I was forced to make this class just for serialization because a "UserControl" 
        /// cannot be serialized.
        /// </summary>
        [DataContract]
        [KnownType(typeof(DailyPlanReader))]
        [KnownType(typeof(BookMarkReader))]
        [KnownType(typeof(HistoryReader))]
        [KnownType(typeof(SearchReader))]
        [KnownType(typeof(BibleNoteReader))]
        [KnownType(typeof(BibleZtextReader))]
        public class SerializableWindowState
        {
            #region Fields

            [DataMember]
            public string bibleToLoad = string.Empty;
            [DataMember]
            public int curIndex = 0;
            [DataMember]
            public double htmlFontSize = 10;
            public bool isResume = false;
            [DataMember]
            public bool isSynchronized = true;
            [DataMember]
            public int numRowsIown = 1;
            [DataMember]
            public IBrowserTextSource source = null;
            [DataMember]
            public WINDOW_TYPE windowType = WINDOW_TYPE.WINDOW_BIBLE;

            #endregion Fields
        }

        #endregion Nested Types
    }
}