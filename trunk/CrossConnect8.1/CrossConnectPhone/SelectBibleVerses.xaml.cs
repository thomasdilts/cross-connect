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

namespace CrossConnect
{
    public partial class SelectBibleVerses : UserControl
    {
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
                this.row20
            };
        }
        public void SetPlaceMarkers(List<BiblePlaceMarker> placeMarkers)
        {
            var lastbook = string.Empty;
            var lastChapter = -1;
            var lastVerse = -1;
            var rowcounter = -1;
            placeMarkers.Reverse();

            foreach (var verse in placeMarkers)
            {
                if (!verse.BookShortName.Equals(lastbook) || verse.ChapterNum != lastChapter || verse.VerseNum != (lastVerse + 1))
                {
                    rowcounter++;
                    if (rowcounter==allRows.Count())
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
                var toVerse = allRows[i].toVerse;
                allRows[i].SelectedBookChapterVerseEvent(allRows[i].bookName,allRows[i].chapter,allRows[i].verse);
                allRows[i].SelectedToVerseEvent(toVerse);
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
        private void ChangePlaces(int index1,int index2)
        {
            //if(string.IsNullOrEmpty(allRows[index1].bookName) || string.IsNullOrEmpty(allRows[index2].bookName))
            //{
            //    return;
            //}
            var bookName = allRows[index1].bookName;
            var chapter = allRows[index1].chapter;
            var verse = allRows[index1].verse;
            var toVerse = allRows[index1].toVerse;

            allRows[index1].SelectedBookChapterVerseEvent( allRows[index2].bookName, allRows[index2].chapter, allRows[index2].verse);
            allRows[index1].SelectedToVerseEvent(allRows[index2].toVerse);

            allRows[index2].SelectedBookChapterVerseEvent(bookName, chapter, verse);
            allRows[index2].SelectedToVerseEvent(toVerse);
        }

        public void MoveChildUp(int index)
        {
            if (index==0)
            {
                return;
            }
            ChangePlaces(index - 1, index);

        }
        public void MoveChildDown(int index)
        {
            if (index == (allRows.Count()-1))
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
