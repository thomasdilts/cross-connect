// <copyright file="MainPageDeleteBookmark.cs" company="Thomas Dilts">
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

    using Windows.UI.Popups;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;

    public sealed partial class MainPageSplit
    {
        #region Fields

        private bool _isInSelectionBookmarkChanged;

        #endregion

        #region Methods

        private void DeleteBookmarkPopup_OnClosed(object sender, object e)
        {
            App.ShowUserInterface(true);
        }

        private void DeleteBookmarkPopup_OnOpened(object sender, object e)
        {
            App.ShowUserInterface(false);
        }

        private void DeleteBookmarkSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this._isInSelectionBookmarkChanged)
            {
                return;
            }

            this._isInSelectionBookmarkChanged = true;

            var dialog = new MessageDialog(Translations.Translate("Delete?"));
            dialog.Commands.Add(
                new UICommand(
                    Translations.Translate("Yes"),
                    (UICommandInvokedHandler) =>
                        {
                            this._isInSelectionBookmarkChanged = true;
                            var index = (int)((TextBlock)e.AddedItems[0]).Tag;
                            App.PlaceMarkers.Bookmarks.RemoveAt(index);
                            this.LoadBookmarkList();
                            App.RaiseBookmarkChangeEvent();
                            this._isInSelectionBookmarkChanged = false;
                            App.SavePersistantMarkers();
                        }));
            dialog.Commands.Add(new UICommand(Translations.Translate("Cancel")));
            dialog.ShowAsync();

            this._isInSelectionBookmarkChanged = false;
        }

        private async void LoadBookmarkList()
        {
            this.SelectListBookmarks.Items.Clear();
            List<string> allBookmarks =
                await
                App.OpenWindows[0].State.Source.MakeListDisplayText(App.DisplaySettings, App.PlaceMarkers.Bookmarks);

            // the list is a reversed list from the original list. So we must mark it with the correct reversed index.
            int j = allBookmarks.Count - 1;
            foreach (string t in allBookmarks)
            {
                var block = new TextBlock
                                {
                                    Text = t.Replace("<p>", string.Empty).Replace("</p>", string.Empty),
                                    Tag = j--,
                                    TextWrapping = TextWrapping.Wrap
                                };
                this.SelectListBookmarks.Items.Add(block);
            }
        }

        private void MenuDeleteBookmarkClick(object sender, RoutedEventArgs e)
        {
            this.SelectBookmarkTitle.Text = Translations.Translate("Select a bookmark to delete");

            this.LoadBookmarkList();
            SideBarShowPopup(
                this.DeleteBookmarkPopup, this.MainPaneDeleteBookmarkPopup, null, this.TopAppBar1, this.BottomAppBar);
        }

        #endregion
    }
}