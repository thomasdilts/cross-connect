using Sword.reader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

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

        public List<BiblePlaceMarker> GetPlaceMarkers()
        {
            var list = new List<BiblePlaceMarker>();
            if (!string.IsNullOrEmpty(this.bookName))
            {
                this.toVerse = GetToVerse();
                for (int i = verse; i <= toVerse; i++)
                {
                    list.Add(new BiblePlaceMarker(this.bookName, this.chapter, i, DateTime.Now));
                }
            }
            return list;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            butMoveUp.Visibility = string.IsNullOrEmpty(this.bookName) ? Visibility.Collapsed : Visibility.Visible;
            butMoveDown.Visibility = string.IsNullOrEmpty(this.bookName) ? Visibility.Collapsed : Visibility.Visible;
            butClose.Visibility = string.IsNullOrEmpty(this.bookName) ? Visibility.Collapsed : Visibility.Visible;

            if (string.IsNullOrEmpty(this.bookName))
            {
                this.SelectBibleList.Items.Clear();
                this.SelectBibleList.Items.Add(Translations.Translate("Book"));
                this.SelectBibleList.SelectedIndex = 0;
                SelectBibleList.IsEnabled = true;

                this.SelectChapterList.Items.Clear();
                this.SelectChapterList.Items.Add(Translations.Translate("Chapter"));
                this.SelectChapterList.SelectedIndex = 0;
                SelectChapterList.IsEnabled = false;

                this.SelectVerseList.Items.Clear();
                this.SelectVerseList.Items.Add(Translations.Translate("Verse"));
                this.SelectVerseList.SelectedIndex = 0;
                SelectVerseList.IsEnabled = false;

                this.SelectToVerseList.Items.Clear();
                this.SelectToVerseList.Items.Add(Translations.Translate("To verse"));
                this.SelectToVerseList.SelectedIndex = 0;
                SelectToVerseList.IsEnabled = false;
            }
            else
            {
                var tempChapter = this.chapter;
                var tempVerse = this.verse;
                var tempToVerse = this.toVerse;
                var tempBookName = this.bookName;
                LoadBibleList(true);
                var book = parent.source.canon.BookByShortName[tempBookName];
                foreach (var item in SelectBibleList.Items)
                {
                    if (((int)((TextBlock)item).Tag) == book.BookNum)
                    {
                        SelectBibleList.SelectedItem = item;
                        break;
                    }
                }
                foreach (var item in SelectChapterList.Items)
                {
                    if (!(item is string) && (int)((TextBlock)item).Tag == tempChapter)
                    {
                        SelectChapterList.SelectedItem = item;
                        break;
                    }
                }

                foreach (var item in SelectVerseList.Items)
                {
                    if (!(item is string) && ((int)((TextBlock)item).Tag) == tempVerse)
                    {
                        SelectVerseList.SelectedItem = item;
                        break;
                    }
                }

                foreach (var item in SelectToVerseList.Items)
                {
                    if (!(item is string) && ((int)((TextBlock)item).Tag) == tempToVerse)
                    {
                        SelectToVerseList.SelectedItem = item;
                        break;
                    }
                }
            }
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
            UserControl_Loaded(null, null);
            this.chapter = 0;
            this.verse = 0;
            this.toVerse = 0;
        }

        private void SelectBibleList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (parent.source == null || SelectBibleList.SelectedItem == null || SelectBibleList.SelectedItem is string)
            {
                return;
            }
            var book = parent.source.canon.GetBookFromBookNumber((int)((TextBlock)SelectBibleList.SelectedItem).Tag);
            this.bookName = book.ShortName1;

            SelectChapterList.Items.Clear();
            int numOfChapters = book.NumberOfChapters;

            if (numOfChapters <= 1)
            {
                this.chapter = 0;
                SelectChapterList.Items.Add(new TextBlock { Text = "1", Tag = 0 });
                SelectChapterList.SelectedIndex = 0;
                SelectChapterList.IsEnabled = false;
                return;
            }
            else
            {
                for (int i = 0; i < numOfChapters; i++)
                {
                    if (!parent.source.Chapters[book.VersesInChapterStartIndex + i].IsEmpty)
                    {
                        SelectChapterList.Items.Add(new TextBlock { Text = (i + 1).ToString(), Tag = i });
                    }
                }
            }
            if (SelectChapterList.Items.Any())
            {
                SelectChapterList.SelectedIndex = 0;
                SelectChapterList.IsEnabled = true;
            }
        }

        public void ReloadControl()
        {
            UserControl_Loaded(null, null);
        }

        private void SelectBibleList_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            LoadBibleList();
            //SelectBibleList.IsDropDownOpen = true;

            this.butClose.Visibility = Windows.UI.Xaml.Visibility.Visible;
            this.butMoveDown.Visibility = Windows.UI.Xaml.Visibility.Visible;
            this.butMoveUp.Visibility = Windows.UI.Xaml.Visibility.Visible;
        }

        private void LoadBibleList(bool force=false)
        {
            if (!force && this.SelectBibleList.Items.Count()>1)
            {
                return;
            }
            this.SelectBibleList.Items.Clear();
            // fill the list
            var butspecs = parent.source.GetButtonWindowSpecs(0, 0, Translations.IsoLanguageCode);
            for (int i = 0; i < butspecs.Text.Length; i++)
            {
                this.SelectBibleList.Items.Add(new TextBlock { Text = butspecs.Text[i], Foreground = new SolidColorBrush(BrowserTitledWindow.ColorConversionScheme[butspecs.Colors[i]]), Tag = butspecs.Value[i] });
            }
        }

        private void SelectChapterList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SelectChapterList.SelectedItem == null || SelectChapterList.SelectedItem is string)
            {
                return;
            }
            this.chapter = (int)((TextBlock)SelectChapterList.SelectedItem).Tag;

            // load the verse list
            SelectVerseList.IsEnabled = true;
            SelectVerseList.Items.Clear();
            var book = parent.source.canon.BookByShortName[this.bookName];
            var numVerses = parent.source.canon.VersesInChapter[book.VersesInChapterStartIndex + this.chapter];
            for (int i = 0; i < numVerses; i++)
            {
                SelectVerseList.Items.Add(new TextBlock { Text = (i + 1).ToString(), Tag = i });
            }
            SelectVerseList.SelectedIndex = 0;
        }

        private void SelectVerseList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SelectVerseList.SelectedItem == null || SelectVerseList.SelectedItem is string)
            {
                return;
            }
            this.verse = (int)((TextBlock)SelectVerseList.SelectedItem).Tag;

            // load the to verse list
            SelectToVerseList.IsEnabled = true;
            SelectToVerseList.Items.Clear();
            var book = parent.source.canon.BookByShortName[this.bookName];
            var numVerses = parent.source.canon.VersesInChapter[book.VersesInChapterStartIndex + this.chapter];
            for (int i = this.verse; i < numVerses; i++)
            {
                SelectToVerseList.Items.Add(new TextBlock { Text = (i + 1).ToString(), Tag = i });
            }
            SelectToVerseList.SelectedIndex = 0;
        }

        public int GetToVerse()
        {
            if (SelectToVerseList.SelectedItem == null || SelectToVerseList.SelectedItem is string)
            {
                return this.toVerse;
            }
            return (int)((TextBlock)SelectToVerseList.SelectedItem).Tag;
        }
    }
}