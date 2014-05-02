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
    using System.Linq;

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
            if (App.PlaceMarkers.History.Any())
            {
                BiblePlaceMarker place = App.PlaceMarkers.History[App.PlaceMarkers.History.Count - 1];
                Highlighter.AddBiblePlaceMarker(place.BookShortName, place.ChapterNum, place.VerseNum, this.TextToAdd.Text.Replace("\r\n","\n").Replace("\r","\n"), App.DailyPlan.PersonalNotesVersified);
                App.RaisePersonalNotesChangeEvent();
            }
        }

        private async void AutoRotatePageLoaded(object sender, RoutedEventArgs e)
        {
            PageTitle.Text = Translations.Translate("Add a note");

            // load the verse
            object openWindowIndex;
            if (!PhoneApplicationService.Current.State.TryGetValue("openWindowIndex", out openWindowIndex))
            {
                openWindowIndex = 0;
            }

            SerializableWindowState state = App.OpenWindows[(int)openWindowIndex].State;

            // they are in reverse order again,
            BiblePlaceMarker place = App.PlaceMarkers.History[App.PlaceMarkers.History.Count - 1];
            string fullName;
            string titleText;
            ((BibleZtextReader)state.Source).GetInfo(
                Translations.IsoLanguageCode,
                place.BookShortName,
                place.ChapterNum, place.VerseNum, out fullName, out titleText);
            string title = fullName + " " + (place.ChapterNum + 1) + ":" + (place.VerseNum + 1) + " - " + state.BibleToLoad;
            string verseText = await state.Source.GetVerseTextOnly(App.DisplaySettings, place.BookShortName, place.ChapterNum, place.VerseNum);

            verse.Text =
                verseText.Replace("<p>", string.Empty).Replace("</p>", string.Empty).Replace("<br />", string.Empty).
                    Replace("\n", " ") + "\n-" + title;

            TextToAdd.Text = string.Empty;
            if (App.DailyPlan.PersonalNotesVersified.ContainsKey(place.BookShortName)
                && App.DailyPlan.PersonalNotesVersified[place.BookShortName].ContainsKey(place.ChapterNum)
                && App.DailyPlan.PersonalNotesVersified[place.BookShortName][place.ChapterNum].ContainsKey(place.VerseNum))
            {
                TextToAdd.Text = App.DailyPlan.PersonalNotesVersified[place.BookShortName][place.ChapterNum][place.VerseNum].Note;
            }
        }

        private void TextToAddKeyUp(object sender, KeyEventArgs e)
        {
            PhoneApplicationService.Current.State["NoteToAddSaved"] = TextToAdd.Text;
        }

        #endregion Methods
    }
}