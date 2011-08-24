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
/// <copyright file="AddNote.xaml.cs" company="Thomas Dilts">
///     Thomas Dilts. All rights reserved.
/// </copyright>
/// <author>Thomas Dilts</author>using System;
namespace CrossConnect
{
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Input;

    using Microsoft.Phone.Shell;

    using SwordBackend;

    public partial class AddNote : AutoRotatePage
    {
        #region Constructors

        public AddNote()
        {
            InitializeComponent();
        }

        #endregion Constructors

        #region Methods

        private void AutoRotatePage_BackKeyPress(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //save if there is something there. otherwise erase an old version if there is one
            BiblePlaceMarker place = App.placeMarkers.history[App.placeMarkers.history.Count - 1];
            //erase the old first
            if (App.dailyPlan.personalNotes.ContainsKey(place.chapterNum) && App.dailyPlan.personalNotes[place.chapterNum].ContainsKey(place.verseNum))
            {
                App.dailyPlan.personalNotes[place.chapterNum].Remove(place.verseNum);
                if (App.dailyPlan.personalNotes[place.chapterNum].Count == 0)
                {
                    App.dailyPlan.personalNotes.Remove(place.chapterNum);
                }
            }
            // add the new
            if (TextToAdd.Text.Length > 0)
            {
                BiblePlaceMarker note = BiblePlaceMarker.clone(place);
                if (!App.dailyPlan.personalNotes.ContainsKey(place.chapterNum))
                {
                    App.dailyPlan.personalNotes.Add(place.chapterNum, new Dictionary<int, BiblePlaceMarker>());
                }
                App.dailyPlan.personalNotes[place.chapterNum][place.verseNum] = note;
                note.note = TextToAdd.Text;
            }
            App.RaisePersonalNotesChangeEvent();
        }

        private void AutoRotatePage_Loaded(object sender, RoutedEventArgs e)
        {
            PageTitle.Text = Translations.translate("Add a note");

            //load the verse
            object openWindowIndex = null;
            if (!PhoneApplicationService.Current.State.TryGetValue("openWindowIndex", out openWindowIndex))
            {
                openWindowIndex = 0;
            }
            var state = App.openWindows[(int)openWindowIndex].state;
            //they are in reverse order again,

            BiblePlaceMarker place = App.placeMarkers.history[App.placeMarkers.history.Count-1];
            int bookNum;
            int relChaptNum;
            string fullName;
            string titleText;
            ((BibleZtextReader)state.source).GetInfo(place.chapterNum, place.verseNum, out bookNum, out relChaptNum, out fullName, out titleText);
            string title = fullName + " " + (relChaptNum + 1) + ":" + (place.verseNum + 1) + " - " + state.bibleToLoad;
            string verseText = state.source.GetVerseTextOnly(App.displaySettings, place.chapterNum, place.verseNum);

            verse.Text = verseText.Replace("<p>", "").Replace("</p>", "").Replace("<br />", "").Replace("\n", " ") + "\n-" + title;

            TextToAdd.Text = "";
            object NoteToAddSaved = null;
            if (PhoneApplicationService.Current.State.TryGetValue("NoteToAddSaved", out NoteToAddSaved))
            {
                TextToAdd.Text = (string)NoteToAddSaved;
            }
            else if (App.dailyPlan.personalNotes.ContainsKey(place.chapterNum) && App.dailyPlan.personalNotes[place.chapterNum].ContainsKey(place.verseNum))
            {
                TextToAdd.Text = App.dailyPlan.personalNotes[place.chapterNum][place.verseNum].note;
            }
        }

        private void TextToAdd_KeyUp(object sender, KeyEventArgs e)
        {
            PhoneApplicationService.Current.State["NoteToAddSaved"] = TextToAdd.Text;
        }

        #endregion Methods
    }
}