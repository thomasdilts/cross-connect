#region Header

// <copyright file="AddNote.xaml.cs" company="Thomas Dilts">
//
// CrossConnect Bible and Bible Commentary Reader for CrossWire.org
// Copyright (C) 2011 Thomas Dilts
//
// This program is free software: you can redistribute it and/or modify
// it under the +terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see http://www.gnu.org/licenses/.
// </copyright>
// <summary>
// Email: thomas@cross-connect.se
// </summary>
// <author>Thomas Dilts</author>

#endregion Header

namespace CrossConnect
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Windows;
    using System.Windows.Input;

    using Microsoft.Phone.Shell;

    using Sword.reader;

    public partial class AddNote
    {
        #region Constructors

        public AddNote()
        {
            InitializeComponent();
        }

        #endregion Constructors

        #region Methods

        private void AutoRotatePageBackKeyPress(object sender, CancelEventArgs e)
        {
            //save if there is something there. otherwise erase an old version if there is one
            var place = App.PlaceMarkers.History[App.PlaceMarkers.History.Count - 1];
            //erase the old first
            if (App.DailyPlan.PersonalNotes.ContainsKey(place.ChapterNum) &&
                App.DailyPlan.PersonalNotes[place.ChapterNum].ContainsKey(place.VerseNum))
            {
                App.DailyPlan.PersonalNotes[place.ChapterNum].Remove(place.VerseNum);
                if (App.DailyPlan.PersonalNotes[place.ChapterNum].Count == 0)
                {
                    App.DailyPlan.PersonalNotes.Remove(place.ChapterNum);
                }
            }
            // add the new
            if (TextToAdd.Text.Length > 0)
            {
                var note = BiblePlaceMarker.Clone(place);
                if (!App.DailyPlan.PersonalNotes.ContainsKey(place.ChapterNum))
                {
                    App.DailyPlan.PersonalNotes.Add(place.ChapterNum, new Dictionary<int, BiblePlaceMarker>());
                }
                App.DailyPlan.PersonalNotes[place.ChapterNum][place.VerseNum] = note;
                note.Note = TextToAdd.Text;
            }
            App.RaisePersonalNotesChangeEvent();
        }

        private void AutoRotatePageLoaded(object sender, RoutedEventArgs e)
        {
            PageTitle.Text = Translations.Translate("Add a note");

            //load the verse
            object openWindowIndex;
            if (!PhoneApplicationService.Current.State.TryGetValue("openWindowIndex", out openWindowIndex))
            {
                openWindowIndex = 0;
            }
            var state = App.OpenWindows[(int) openWindowIndex].State;
            //they are in reverse order again,

            var place = App.PlaceMarkers.History[App.PlaceMarkers.History.Count - 1];
            int bookNum;
            int relChaptNum;
            string fullName;
            string titleText;
            ((BibleZtextReader) state.Source).GetInfo(place.ChapterNum, place.VerseNum, out bookNum, out relChaptNum,
                                                      out fullName, out titleText);
            string title = fullName + " " + (relChaptNum + 1) + ":" + (place.VerseNum + 1) + " - " + state.BibleToLoad;
            string verseText = state.Source.GetVerseTextOnly(App.DisplaySettings, place.ChapterNum, place.VerseNum);

            verse.Text = verseText.Replace("<p>", "").Replace("</p>", "").Replace("<br />", "").Replace("\n", " ") +
                         "\n-" + title;

            TextToAdd.Text = "";
            object noteToAddSaved;
            if (PhoneApplicationService.Current.State.TryGetValue("NoteToAddSaved", out noteToAddSaved))
            {
                TextToAdd.Text = (string) noteToAddSaved;
            }
            else if (App.DailyPlan.PersonalNotes.ContainsKey(place.ChapterNum) &&
                     App.DailyPlan.PersonalNotes[place.ChapterNum].ContainsKey(place.VerseNum))
            {
                TextToAdd.Text = App.DailyPlan.PersonalNotes[place.ChapterNum][place.VerseNum].Note;
            }
        }

        private void TextToAddKeyUp(object sender, KeyEventArgs e)
        {
            PhoneApplicationService.Current.State["NoteToAddSaved"] = TextToAdd.Text;
        }

        #endregion Methods
    }
}