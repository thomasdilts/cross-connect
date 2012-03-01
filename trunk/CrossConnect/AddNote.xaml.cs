// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AddNote.xaml.cs" company="">
//   
// </copyright>
// <summary>
//   The add note.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#region Header

// <copyright file="AddNote.xaml.cs" company="Thomas Dilts">
// CrossConnect Bible and Bible Commentary Reader for CrossWire.org
// Copyright (C) 2011 Thomas Dilts
// This program is free software: you can redistribute it and/or modify
// it under the +terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
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

    /// <summary>
    /// The add note.
    /// </summary>
    public partial class AddNote
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="AddNote"/> class.
        /// </summary>
        public AddNote()
        {
            this.InitializeComponent();
        }

        #endregion

        #region Methods

        /// <summary>
        /// The auto rotate page back key press.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void AutoRotatePageBackKeyPress(object sender, CancelEventArgs e)
        {
            // save if there is something there. otherwise erase an old version if there is one
            BiblePlaceMarker place = App.PlaceMarkers.History[App.PlaceMarkers.History.Count - 1];

            // erase the old first
            if (App.DailyPlan.PersonalNotes.ContainsKey(place.ChapterNum)
                && App.DailyPlan.PersonalNotes[place.ChapterNum].ContainsKey(place.VerseNum))
            {
                App.DailyPlan.PersonalNotes[place.ChapterNum].Remove(place.VerseNum);
                if (App.DailyPlan.PersonalNotes[place.ChapterNum].Count == 0)
                {
                    App.DailyPlan.PersonalNotes.Remove(place.ChapterNum);
                }
            }

            // add the new
            if (this.TextToAdd.Text.Length > 0)
            {
                BiblePlaceMarker note = BiblePlaceMarker.Clone(place);
                if (!App.DailyPlan.PersonalNotes.ContainsKey(place.ChapterNum))
                {
                    App.DailyPlan.PersonalNotes.Add(place.ChapterNum, new Dictionary<int, BiblePlaceMarker>());
                }

                App.DailyPlan.PersonalNotes[place.ChapterNum][place.VerseNum] = note;
                note.Note = this.TextToAdd.Text;
            }

            App.RaisePersonalNotesChangeEvent();
        }

        /// <summary>
        /// The auto rotate page loaded.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void AutoRotatePageLoaded(object sender, RoutedEventArgs e)
        {
            this.PageTitle.Text = Translations.Translate("Add a note");

            // load the verse
            object openWindowIndex;
            if (!PhoneApplicationService.Current.State.TryGetValue("openWindowIndex", out openWindowIndex))
            {
                openWindowIndex = 0;
            }

            SerializableWindowState state = App.OpenWindows[(int)openWindowIndex].State;

            // they are in reverse order again,
            BiblePlaceMarker place = App.PlaceMarkers.History[App.PlaceMarkers.History.Count - 1];
            int bookNum;
            int relChaptNum;
            string fullName;
            string titleText;
            ((BibleZtextReader)state.Source).GetInfo(
                place.ChapterNum, place.VerseNum, out bookNum, out relChaptNum, out fullName, out titleText);
            string title = fullName + " " + (relChaptNum + 1) + ":" + (place.VerseNum + 1) + " - " + state.BibleToLoad;
            string verseText = state.Source.GetVerseTextOnly(App.DisplaySettings, place.ChapterNum, place.VerseNum);

            this.verse.Text =
                verseText.Replace("<p>", string.Empty).Replace("</p>", string.Empty).Replace("<br />", string.Empty).
                    Replace("\n", " ") + "\n-" + title;

            this.TextToAdd.Text = string.Empty;
            object noteToAddSaved;
            if (PhoneApplicationService.Current.State.TryGetValue("NoteToAddSaved", out noteToAddSaved))
            {
                this.TextToAdd.Text = (string)noteToAddSaved;
            }
            else if (App.DailyPlan.PersonalNotes.ContainsKey(place.ChapterNum)
                     && App.DailyPlan.PersonalNotes[place.ChapterNum].ContainsKey(place.VerseNum))
            {
                this.TextToAdd.Text = App.DailyPlan.PersonalNotes[place.ChapterNum][place.VerseNum].Note;
            }
        }

        /// <summary>
        /// The text to add key up.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void TextToAddKeyUp(object sender, KeyEventArgs e)
        {
            PhoneApplicationService.Current.State["NoteToAddSaved"] = this.TextToAdd.Text;
        }

        #endregion
    }
}