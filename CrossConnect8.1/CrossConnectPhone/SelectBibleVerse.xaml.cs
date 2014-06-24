using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Sword.reader;
using System.Windows.Media;

namespace CrossConnect
{
    public partial class SelectBibleVerse : UserControl
    {
        public SelectBibleVerse()
        {
            InitializeComponent();
        }
        public void SelectedBibleVerseEvent()
        {

        }
        public SelectBibleVerses parent;
        public int index;
        public string bookName;
        public int chapter;
        public int verse;
        public int toVerse;
        public void SelectedBookChapterVerseEvent(string bookName, int chapter, int verse)
        {
            this.bookName = bookName;
            this.chapter = chapter;
            this.verse = verse;
            object openWindowIndex;
            if (string.IsNullOrEmpty(this.bookName))
            {
                UserControl_Loaded(null, null);
                return;
            }
            if (!PhoneApplicationService.Current.State.TryGetValue("openWindowIndex", out openWindowIndex))
            {
                openWindowIndex = 0;
            }
            var source = App.OpenWindows[(int)openWindowIndex].State.Source;
            var name = ((BibleZtextReader)source).GetFullName(this.bookName,Translations.IsoLanguageCode);
            SelectBible.Content = name + " " + (chapter + 1) + ":" + (verse + 1);
            SelectToVerse.Content = (verse + 1);
            butMoveUp.Visibility = Visibility.Visible;
            butMoveDown.Visibility = Visibility.Visible;
            butClose.Visibility = Visibility.Visible;
        }
        public void SelectedToVerseEvent(int toVerse)
        {
            if (string.IsNullOrEmpty(this.bookName))
            {
                UserControl_Loaded(null, null);
                return;
            } 
            this.toVerse = toVerse;

            SelectToVerse.Content = (this.toVerse + 1);
        }
        private void SelectBible_Click(object sender, RoutedEventArgs e)
        {
            SelectBibleBook.SelectedEvent += SelectedBookChapterVerseEvent;
            App.MainWindow.NavigationService.Navigate(new Uri("/SelectBibleBook.xaml", UriKind.Relative));
        }

        private void SelectToVerse_Click(object sender, RoutedEventArgs e)
        {
           
            object openWindowIndex;
            if (!PhoneApplicationService.Current.State.TryGetValue("openWindowIndex", out openWindowIndex))
            {
                openWindowIndex = 0;
            }
            var source = App.OpenWindows[(int)openWindowIndex].State.Source;
            var canon = ((BibleZtextReader)source).canon;
            var book = canon.BookByShortName[this.bookName];
            CrossConnect.SelectToVerse.VerseMax = canon.VersesInChapter[book.VersesInChapterStartIndex + this.chapter];
            CrossConnect.SelectToVerse.VerseMin = verse;
            if (CrossConnect.SelectToVerse.VerseMax - CrossConnect.SelectToVerse.VerseMin<2)
            {
                return;
            }

            CrossConnect.SelectToVerse.SelectedEvent += SelectedToVerseEvent;
            App.MainWindow.NavigationService.Navigate(new Uri("/SelectToVerse.xaml", UriKind.Relative));
        }
        public List<BiblePlaceMarker> GetPlaceMarkers()
        {
            var list = new List<BiblePlaceMarker>();
            if (!string.IsNullOrEmpty(this.bookName))
            {
                for (int i = verse; i <= toVerse; i++)
                {
                    list.Add(new BiblePlaceMarker(this.bookName, this.chapter, i, DateTime.Now));
                }
            }
            return list;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            string colorDir = ((Visibility)Application.Current.Resources["PhoneDarkThemeVisibility"])
                                             == Visibility.Collapsed ? "light" : "dark";
            BrowserTitledWindow.SetButtonVisibility(butMoveUp, true,
                "/Images/" + colorDir + "/appbar.arrowup.rest.png",
                "/Images/" + colorDir + "/appbar.arrowup.rest.press.png");
            BrowserTitledWindow.SetButtonVisibility(butMoveDown, true,
                "/Images/" + colorDir + "/appbar.arrowdown.rest.png",
                "/Images/" + colorDir + "/appbar.arrowdown.rest.press.png");
            BrowserTitledWindow.SetButtonVisibility(butClose, true,
                "/Images/" + colorDir + "/appbar.cancel.rest.png",
                "/Images/" + colorDir + "/appbar.cancel.rest.press.png");
            this.arrowright.Source = BrowserTitledWindow.GetImage("/Images/" + colorDir + "/appbar.arrowto.rest.png");
            butMoveUp.Visibility = string.IsNullOrEmpty(this.bookName)?Visibility.Collapsed:Visibility.Visible;
            butMoveDown.Visibility = string.IsNullOrEmpty(this.bookName)?Visibility.Collapsed:Visibility.Visible;
            butClose.Visibility = string.IsNullOrEmpty(this.bookName) ? Visibility.Collapsed : Visibility.Visible;

            if (!string.IsNullOrEmpty(this.bookName))
            {
                return;
            } 
            
            this.SelectBible.Content = Translations.Translate("Select verse");
            this.SelectToVerse.Content = Translations.Translate("To verse");

        }

        private void ButMoveUpClick(object sender, RoutedEventArgs e)
        {
            parent.MoveChildUp(index);
        }
        private void ButMoveDownClick(object sender, RoutedEventArgs e)
        {
            parent.MoveChildDown(index);
        }
        private void ButCloseClick(object sender, RoutedEventArgs e)
        {
            this.bookName = string.Empty;
            UserControl_Loaded(null,null);
            this.chapter = 0;
            this.verse = 0;
            this.toVerse = 0;
        }
    }
}
