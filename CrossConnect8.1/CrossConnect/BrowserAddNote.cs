// <copyright file="BrowserAddNote.cs" company="Thomas Dilts">
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

namespace CrossConnect
{
    using System.Collections.Generic;
    using System.Linq;
    using Sword.reader;

    public sealed partial class BrowserTitledWindow
    {
        #region Methods

        private async void AddANote_OnClick()
        {
            this.AddNoteTitle.Text = Translations.Translate("Add a note");

            // load the verse

            // they are in reverse order again,
            BiblePlaceMarker place = App.PlaceMarkers.History[App.PlaceMarkers.History.Count - 1];
            string fullName;
            string titleText;
            ((BibleZtextReader)this._state.Source).GetInfo(Translations.IsoLanguageCode, place.BookShortName,
                place.ChapterNum, place.VerseNum, out fullName, out titleText);
            string title = fullName + " " + (place.ChapterNum + 1) + ":" + (place.VerseNum + 1) + " - "
                           + this._state.BibleToLoad;
            string verseText =
                await this._state.Source.GetVerseTextOnly(App.DisplaySettings,place.BookShortName, place.ChapterNum, place.VerseNum);

            this.verse.Text =
                verseText.Replace("<p>", string.Empty)
                         .Replace("</p>", string.Empty)
                         .Replace("<br />", string.Empty)
                         .Replace("\n", " ") + "\n-" + title;

            this.TextToAdd.Text = string.Empty;
            if (App.DailyPlan.PersonalNotesVersified.ContainsKey(place.BookShortName)
                && App.DailyPlan.PersonalNotesVersified[place.BookShortName].ContainsKey(place.ChapterNum)
                && App.DailyPlan.PersonalNotesVersified[place.BookShortName][place.ChapterNum].ContainsKey(place.VerseNum))
            {
                this.TextToAdd.Text = App.DailyPlan.PersonalNotesVersified[place.BookShortName][place.ChapterNum][place.VerseNum].Note;
            }
            MainPageSplit.SideBarShowPopup(this.AddNotePopup, this.MainPaneAddNotePopup);
        }

        private void AddNotePopup_OnClosed(object sender, object e)
        {
            App.ShowUserInterface(true);
            // save if there is something there. otherwise erase an old version if there is one
            if (App.PlaceMarkers.History.Any())
            {
                BiblePlaceMarker place = App.PlaceMarkers.History[App.PlaceMarkers.History.Count - 1];
                Highlighter.AddBiblePlaceMarker(place.BookShortName, place.ChapterNum, place.VerseNum, this.TextToAdd.Text, App.DailyPlan.PersonalNotesVersified);
                App.RaisePersonalNotesChangeEvent();
            }
        }

        private void AddNotePopup_OnOpened(object sender, object e)
        {
            App.ShowUserInterface(false);
        }

        #endregion
    }
}