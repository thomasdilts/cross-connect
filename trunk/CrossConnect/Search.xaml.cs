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
using Microsoft.Phone.Controls;
using System.IO.IsolatedStorage;
using System.IO;
using SwordBackend;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace CrossConnect
{
    public partial class Search : PhoneApplicationPage
    {
        public Search()
        {
            InitializeComponent();
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            var state = App.openWindows[App.windowSettings.openWindowIndex].state;
            int dummy1;
            int dummy2;
            string Name;
            string text;
            App.openWindows[App.windowSettings.openWindowIndex].state.source.getInfo(state.chapterNum, state.verseNum, out dummy1, out dummy2, out Name, out text);
            Chapter.Content = Name;
        }

        private void PhoneApplicationPage_BackKeyPress(object sender, System.ComponentModel.CancelEventArgs e)
        {

        }
        private int numFoundVerses = 0;
        public    List<int> chapters = new List<int>();
        public    int searchTypeIndex = 0;
        public    int searchChapter = 0;
        public bool ignoreCase=false;
        public static Search searchingObject = null;
        public string searchText = "";
        public bool isSearchFinished = false;
        public bool isSearchFinishedReported = false;
        public SearchReader sourceSearch = null;
        public bool isAbort = false;
        private void butSearch_Click(object sender, RoutedEventArgs e)
        {
            ShowControls(false);
            searchingObject = this;
            ignoreCase = (bool)IgnoreCase.IsChecked;
            searchText = SearchText.Text;
            string[] parts = searchText.Split(" ,".ToArray());
            List<string> goodParts = new List<string>();
            for (int j = 0; j < parts.Count(); j++)
            {
                if (!string.IsNullOrEmpty(parts[j]))
                {
                    goodParts.Add(parts[j]);
                }
            }
            if (goodParts.Count() > 1)
            {
                if ((bool)OneOrMoreWords.IsChecked)
                {
                    searchText = goodParts[0];
                    for (int j = 1; j < goodParts.Count(); j++)
                    {
                        searchText = searchText + "|" + goodParts[j];
                    }
                }
                //else if ((bool)AllWords.IsChecked)
                //{
                //    switch (goodParts.Count())
                //    {
                //        case 2:
                //            searchText = "" + goodParts[0] + ".*?" + goodParts[1] + "|" + goodParts[1] + ".*?" + goodParts[0] + "";
                //            break;
                //        case 3:
                //            searchText =
                //                "" + goodParts[0] + ".*?" + goodParts[1] + ".*?" + goodParts[2] + "|" +
                //                "" + goodParts[0] + ".*?" + goodParts[2] + ".*?" + goodParts[1] + "|" +
                //                "" + goodParts[1] + ".*?" + goodParts[2] + ".*?" + goodParts[0] + "|" +
                //                "" + goodParts[1] + ".*?" + goodParts[0] + ".*?" + goodParts[2] + "|" +
                //                "" + goodParts[2] + ".*?" + goodParts[1] + ".*?" + goodParts[0] + "|" +
                //                "" + goodParts[2] + ".*?" + goodParts[0] + ".*?" + goodParts[1] + "";
                //                //"(" + goodParts[0] + ".*?" + goodParts[1] + ".*?" + goodParts[2] + ")|" +
                //                //"(" + goodParts[0] + ".*?" + goodParts[2] + ".*?" + goodParts[1] + ")|" +
                //                //"(" + goodParts[1] + ".*?" + goodParts[2] + ".*?" + goodParts[0] + ")|" +
                //                //"(" + goodParts[1] + ".*?" + goodParts[0] + ".*?" + goodParts[2] + ")|" +
                //                //"(" + goodParts[2] + ".*?" + goodParts[1] + ".*?" + goodParts[0] + ")|" +
                //                //"(" + goodParts[2] + ".*?" + goodParts[0] + ".*?" + goodParts[1] + ")";
                //            break;
                //    }
                //}
            }

            isSearchFinished = false;
            isSearchFinishedReported = false;

            if ((bool)wholeBible.IsChecked)
            {
                for (int i = 0; i < BibleZtextReader.CHAPTERS_IN_BIBLE; i++)
                {
                    chapters.Add(i);
                }
                searchTypeIndex = 0;
            }
            else if ((bool)oldTestement.IsChecked)
            {
                for (int i = 0; i < BibleZtextReader.CHAPTERS_IN_OT; i++)
                {
                    chapters.Add(i);
                }
                searchTypeIndex = 1;
            }
            else if ((bool)newTEstement.IsChecked)
            {
                for (int i = BibleZtextReader.CHAPTERS_IN_OT; i < BibleZtextReader.CHAPTERS_IN_BIBLE; i++)
                {
                    chapters.Add(i);
                }
                searchTypeIndex = 2;
            }
            else
            {
                chapters.Add(App.openWindows[App.windowSettings.openWindowIndex].state.chapterNum);
                searchTypeIndex = 3;
                searchChapter = App.openWindows[App.windowSettings.openWindowIndex].state.chapterNum;
            }
            BibleZtextReader source = (BibleZtextReader)App.openWindows[App.windowSettings.openWindowIndex].state.source;
            sourceSearch = new SearchReader(
                source.path,
                source.iso2DigitLangCode,
                source.isIsoEncoding);

            System.Threading.Timer tmr = new System.Threading.Timer(new System.Threading.TimerCallback(OnTimerTick));
            tmr.Change(300, System.Threading.Timeout.Infinite);

        }
        public void ShowControls(bool isShow)
        {
            System.Windows.Visibility isVis = (isShow ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed);
            SearchText.Visibility = isVis;
            SearchWhereText.Visibility = isVis;
            wholeBible.Visibility = isVis;
            oldTestement.Visibility = isVis;
            newTEstement.Visibility = isVis;
            Chapter.Visibility = isVis;
            IgnoreCase.Visibility = isVis;
            progressBar1.Value = 0;
            butSearch.Visibility = isVis;
            butHelp.Visibility = isVis;
            PageTitle.Text = "Search";
            SearchByText.Visibility = isVis;
            OneOrMoreWords.Visibility = isVis;
            //AllWords.Visibility = isVis;
            ExactMatch.Visibility = isVis;
        }
        public void UpdateProgressBar()
        {
            if (Search.searchingObject.isSearchFinishedReported)
            {
                //this is a delayed reporting that must be ignored.
                return;
            }
            Search.searchingObject.progressBar1.Value = percent;
            PageTitle.Text = "Search; Found; " + numFoundVerses;
            if (isSearchFinished)
            {
                Search.searchingObject.isSearchFinishedReported = true;
                if (numFoundVerses == 0)
                {
                    MessageBox.Show("No matches found");
                    ShowControls(true);
                }
                else
                {
                    if (isAbort)
                    {
                        MessageBox.Show("More then 200 items found. Search stopped");
                    } 
                    
                    App.AddWindow(App.openWindows[App.windowSettings.openWindowIndex].state.bibleToLoad, 0, 0, WINDOW_TYPE.WINDOW_SEARCH, sourceSearch);
                    App.windowSettings.skipWindowSettings = true;
                    NavigationService.GoBack();
                }
            }
        
        }
        private double percent = 1;
        public void ShowProgress(double percent, int totalFound, bool isAbort, bool isFinished)
        {
            Search.searchingObject.isSearchFinished = isFinished;
            Search.searchingObject.percent = percent;
            Search.searchingObject.isAbort = isAbort;

            Dispatcher.BeginInvoke(UpdateProgressBar);

            numFoundVerses = totalFound;

        }
        void OnTimerTick(object state)
        //void OnTimerTick(object sender, EventArgs e)
        {
            ((System.Threading.Timer)state).Dispose();
  
            sourceSearch.doSearch(
                Search.searchingObject.searchChapter,
                Search.searchingObject.searchTypeIndex,
                Search.searchingObject.searchText,
                Search.searchingObject.ignoreCase,
                Search.searchingObject.chapters,
                ShowProgress);

        }

        private void butHelp_Click(object sender, RoutedEventArgs e)
        {
            App.helpstart.title="";
            App.helpstart.embeddedFilePath="CrossConnect.Properties.regex.html";
            NavigationService.Navigate(new Uri("/Help.xaml", UriKind.Relative));
        }
    }
    
}