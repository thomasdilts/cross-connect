using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using Sword.reader;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace CrossConnect
{
    public partial class SelectBibleVerses : UserControl
    {
        public BibleZtextReader source;
        private List<SelectBibleVerse> allRows;
        public SelectBibleVerses()
        {
            InitializeComponent();
            allRows = new List<SelectBibleVerse>
            {
                this.row1,
                this.row2,
                this.row3,
                this.row4,
                this.row5,
                this.row6,
                this.row7,
                this.row8,
                this.row9,
                this.row10,
                this.row11,
                this.row12,
                this.row13,
                this.row14,
                this.row15,
                this.row16,
                this.row17,
                this.row18,
                this.row19,
                this.row20,
                this.row21,
                this.row22,
                this.row23,
                this.row24,
                this.row25,
                this.row26,
                this.row27,
                this.row28,
                this.row29,
                this.row30,
                this.row31,
                this.row32,
                this.row33,
                this.row34,
                this.row35,
                this.row36,
                this.row37,
                this.row38,
                this.row39,
                this.row40
            };
        }
        public void SetPlaceMarkers(List<BiblePlaceMarker> placeMarkers)
        {
            var lastbook = string.Empty;
            var lastChapter = -1;
            var lastVerse = -1;
            var rowcounter = -1;
            if (placeMarkers==null)
            {
                placeMarkers = new List<BiblePlaceMarker>();
            }
            placeMarkers.Reverse();
            foreach (var row in allRows)
            {
                row.parent = this;
                row.bookName = string.Empty;
                row.ReloadControl();
            }

            foreach (var verse in placeMarkers)
            {
                if (!verse.BookShortName.Equals(lastbook) || verse.ChapterNum != lastChapter || verse.VerseNum != (lastVerse + 1))
                {
                    rowcounter++;
                    if (rowcounter == allRows.Count())
                    {
                        break;
                    }

                    allRows[rowcounter].bookName = verse.BookShortName;
                    allRows[rowcounter].chapter = verse.ChapterNum;
                    allRows[rowcounter].verse = verse.VerseNum;
                    allRows[rowcounter].toVerse = verse.VerseNum;
                }
                else
                {
                    allRows[rowcounter].toVerse = verse.VerseNum;
                }

                lastbook = verse.BookShortName;
                lastChapter = verse.ChapterNum;
                lastVerse = verse.VerseNum;
            }
            for (int i = 0; i <= rowcounter; i++)
            {
                allRows[i].parent = this;
                allRows[i].ReloadControl();
            }
        }

        public List<BiblePlaceMarker> GetPlaceMarkers()
        {

            var list = new List<BiblePlaceMarker>();

            foreach (var row in allRows)
            {
                list.AddRange(row.GetPlaceMarkers());
            }
            list.Reverse();

            return list;
        }
        private void ChangePlaces(int index1, int index2)
        {
            //if(string.IsNullOrEmpty(allRows[index1].bookName) || string.IsNullOrEmpty(allRows[index2].bookName))
            //{
            //    return;
            //}
            var bookName = allRows[index1].bookName;
            var chapter = allRows[index1].chapter;
            var verse = allRows[index1].verse;
            var toVerse = allRows[index1].GetToVerse();

            SetSelectedVerse(allRows[index1], allRows[index2].bookName, allRows[index2].chapter, allRows[index2].verse, allRows[index2].GetToVerse());

            SetSelectedVerse(allRows[index2], bookName, chapter, verse, toVerse);
        }

        private void SetSelectedVerse(SelectBibleVerse row, string bookName, int chapter, int verse, int toverse)
        {
            row.bookName = bookName;
            row.chapter = chapter;
            row.verse = verse;
            row.toVerse = toverse;
            row.ReloadControl();
        }

        public void MoveChildUp(int index)
        {
            if (index == 0)
            {
                return;
            }
            ChangePlaces(index - 1, index);

        }
        public void MoveChildDown(int index)
        {
            if (index == (allRows.Count() - 1))
            {
                return;
            }
            ChangePlaces(index + 1, index);
        }
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            title.Text = Translations.Translate("Selected verses");
            int index = 0;
            foreach (var row in allRows)
            {
                row.parent = this;
                row.index = index;
                index++;
            }
        }
    }
}
