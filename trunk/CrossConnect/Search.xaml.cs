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
            int dummy;
            string Name;
            string text;
            App.openWindows[App.windowSettings.openWindowIndex].state.source.getInfo(state.chapterNum, state.verseNum,out dummy,out dummy,out Name,out text);
            Chapter.Content = Name;
        }

        private void PhoneApplicationPage_BackKeyPress(object sender, System.ComponentModel.CancelEventArgs e)
        {

        }
        private int numFoundVerses = 0;
        private void butSearch_Click(object sender, RoutedEventArgs e)
        {
            List<int> chapters = new List<int>();
            int searchTypeIndex = 0;
            int searchChapter=0;
            if ((bool)wholeBible.IsChecked)
            {
                for(int i=0;i<BibleZtextReader.CHAPTERS_IN_BIBLE;i++)
                {
                    chapters.Add(i);
                }
                searchTypeIndex = 0;
            }
            else if ((bool)oldTestement.IsChecked)
            {
                for(int i=0;i<BibleZtextReader.CHAPTERS_IN_OT;i++)
                {
                    chapters.Add(i);
                }
                searchTypeIndex = 1;
            }
            else if ((bool)newTEstement.IsChecked)
            {
                for(int i=BibleZtextReader.CHAPTERS_IN_OT;i<BibleZtextReader.CHAPTERS_IN_BIBLE;i++)
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

            SearchText.Visibility= System.Windows.Visibility.Collapsed;
            SearchWhereText.Visibility= System.Windows.Visibility.Collapsed;
            wholeBible.Visibility= System.Windows.Visibility.Collapsed;
            oldTestement.Visibility= System.Windows.Visibility.Collapsed;
            newTEstement.Visibility= System.Windows.Visibility.Collapsed;
            Chapter.Visibility= System.Windows.Visibility.Collapsed;
            butSearch.Visibility= System.Windows.Visibility.Collapsed;
            IgnoreCase.Visibility= System.Windows.Visibility.Collapsed;

            BibleZtextReader source = (BibleZtextReader)App.openWindows[App.windowSettings.openWindowIndex].state.source;

            SearchReader sourceSearch = new SearchReader(
                source.path,
                source.iso2DigitLangCode,
                source.isIsoEncoding);
            sourceSearch.doSearch(
                searchChapter,
                searchTypeIndex,
                this.SearchText.Text,
                (bool)this.IgnoreCase.IsChecked,
                chapters,
                ShowProgress);
            if (numFoundVerses == 0)
            {
                MessageBox.Show("No matches found");
            }
            else
            {
                App.AddWindow(App.openWindows[App.windowSettings.openWindowIndex].state.bibleToLoad, 0, 0, WINDOW_TYPE.WINDOW_SEARCH, sourceSearch);
                App.windowSettings.skipWindowSettings = true;
                NavigationService.GoBack();
            }
        }
        public void ShowProgress(double percent, int totalFound, bool isAbort)
        {
            this.progressBar1.Value = percent;
            this.progressBar1.UpdateLayout();
            this.UpdateLayout();
            numFoundVerses = totalFound;
            if (isAbort)
            {
                MessageBox.Show("More then 200 items found. Search stopped");
            }
        }
        public static void DoEvents()
        {

        }
    }
    
}