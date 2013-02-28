// <copyright file="BrowserCopy.cs" company="Thomas Dilts">
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
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Sword.reader;

    using Windows.ApplicationModel.DataTransfer;
    using Windows.System;
    using Windows.UI.Xaml;

    public sealed partial class BrowserTitledWindow
    {
        #region Public Methods and Operators

        public async Task<string[]> GetLast3SecondsChosenVerses()
        {
            string textsWithTitles = string.Empty;
            string titlesOnly = string.Empty;

            DateTime? firstFound = null;
            var foundVerses = new List<BiblePlaceMarker>();
            for (int j = App.PlaceMarkers.History.Count - 1; j >= 0; j--)
            {
                BiblePlaceMarker place = App.PlaceMarkers.History[j];
                if (firstFound == null)
                {
                    firstFound = place.When;
                    foundVerses.Add(place);
                }
                else if (firstFound.Value.AddSeconds(-3).CompareTo(place.When) < 0)
                {
                    foundVerses.Add(place);
                }
                else
                {
                    // we found all the verses, get out.
                    break;
                }
            }

            // they are in reverse order again,
            for (int j = foundVerses.Count - 1; j >= 0; j--)
            {
                BiblePlaceMarker place = foundVerses[j];
                int bookNum;
                int relChaptNum;
                string fullName;
                string titleText;
                ((BibleZtextReader)this._state.Source).GetInfo(
                    place.ChapterNum, place.VerseNum, out bookNum, out relChaptNum, out fullName, out titleText);
                string title = fullName + " " + (relChaptNum + 1) + ":" + (place.VerseNum + 1) + " - "
                               + this._state.BibleToLoad;
                string verseText =
                    await this._state.Source.GetVerseTextOnly(App.DisplaySettings, place.ChapterNum, place.VerseNum);

                if (!string.IsNullOrEmpty(titlesOnly))
                {
                    textsWithTitles += "\n";
                    titlesOnly += ", ";
                }

                titlesOnly += title;
                textsWithTitles +=
                    verseText.Replace("<p>", string.Empty)
                             .Replace("</p>", string.Empty)
                             .Replace("<br />", string.Empty)
                             .Replace("\n", " ") + "\n-" + title;
                if (App.DailyPlan.PersonalNotes.ContainsKey(place.ChapterNum)
                    && App.DailyPlan.PersonalNotes[place.ChapterNum].ContainsKey(place.VerseNum))
                {
                    textsWithTitles += "\n" + Translations.Translate("Added notes") + "\n"
                                       + App.DailyPlan.PersonalNotes[place.ChapterNum][place.VerseNum].Note;
                }
            }
            return new[] { textsWithTitles, titlesOnly };
        }

        #endregion

        #region Methods

        private void CopyPopup_OnClosed(object sender, object e)
        {
            App.ShowUserInterface(true);
        }

        private void CopyPopup_OnOpened(object sender, object e)
        {
            App.ShowUserInterface(false);
        }

        private void CopySelection_OnClick(object sender, RoutedEventArgs e)
        {
            if (this.SelectText.SelectionLength <= 0)
            {
                return;
            }

            string selectedText = this.SelectText.SelectedText;
            // Set the content to DataPackage as (plain) text format  
            var dataPackage = new DataPackage();
            dataPackage.SetText(selectedText);
            try
            {
                // Set the DataPackage to clipboard. 
                Clipboard.SetContent(dataPackage);
            }
            catch (Exception ex)
            {
                // Copying data to Clipboard can potentially fail - for example, if another application is holding Clipboard open 
            }
        }

        private async void Copy_OnClick()
        {
            this.CopyPopup.IsOpen = true;
            MainPageSplit.SideBarShowPopup(this.CopyPopup, this.MainPaneCopyPopup);

            string textsWithTitles;
            string titlesOnly;
            string[] reply = await this.GetLast3SecondsChosenVerses();
            textsWithTitles = reply[0];
            titlesOnly = reply[1];
            this.SelectText.Text = textsWithTitles;
            this.SelectText.Focus(FocusState.Programmatic);
        }

        private async void MenuMailClick()
        {
            string textsWithTitles;
            string titlesOnly;
            string[] reply = await this.GetLast3SecondsChosenVerses();
            textsWithTitles = reply[0];
            titlesOnly = reply[1];
            var mailto = new Uri("mailto:?subject=" + titlesOnly + "&body=" + textsWithTitles);
            await Launcher.LaunchUriAsync(mailto);
        }

        private void SelectAll_OnClick(object sender, RoutedEventArgs e)
        {
            this.SelectText.Focus(FocusState.Programmatic);
            this.SelectText.SelectAll();
        }

        #endregion
    }
}