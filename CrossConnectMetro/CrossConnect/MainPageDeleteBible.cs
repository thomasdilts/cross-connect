// <copyright file="MainPageDeleteBible.cs" company="Thomas Dilts">
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

    using Windows.UI.Popups;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;

    public sealed partial class MainPageSplit
    {
        #region Fields

        private bool _isInSelectionChanged;

        #endregion

        #region Methods

        private void DeleteBiblePopup_OnClosed(object sender, object e)
        {
            App.ShowUserInterface(true);
        }

        private void DeleteBiblePopup_OnOpened(object sender, object e)
        {
            App.ShowUserInterface(false);
        }

        private void DeleteBibleSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this._isInSelectionChanged)
            {
                return;
            }

            this._isInSelectionChanged = true;
            SwordBook foundBook = null;
            string foundKey = string.Empty;
            var index = (string)((TextBlock)e.AddedItems[0]).Tag;
            Dictionary<string, SwordBook> biblesAndCommentaries =
                App.InstalledBibles.InstalledBibles.Concat(App.InstalledBibles.InstalledCommentaries).Concat(App.InstalledBibles.InstalledGeneralBooks)
                   .ToDictionary(x => x.Key, x => x.Value);
            foreach (var book in biblesAndCommentaries)
            {
                if (book.Value.Sbmd.Initials.Equals(index))
                {
                    foundBook = book.Value;
                    foundKey = book.Key;
                    break;
                }
            }

            if (App.InstalledBibles.InstalledBibles.Count == 1
                && App.InstalledBibles.InstalledBibles.ContainsKey(foundKey))
            {
                var dialog = new MessageDialog(Translations.Translate("You must have at least one bible"));
                dialog.ShowAsync();
            }
            else
            {
                var dialog = new MessageDialog(Translations.Translate("Delete?"));
                dialog.Commands.Add(
                    new UICommand(
                        Translations.Translate("Yes"),
                        (UICommandInvokedHandler) =>
                            {
                                this._isInSelectionChanged = true;
                                for (int i = App.OpenWindows.Count() - 1; i >= 0; i--)
                                {
                                    if (foundBook != null
                                        && (App.OpenWindows[i].State.BibleToLoad.Equals(foundBook.Sbmd.InternalName)
                                            && App.MainWindow != null))
                                    {
                                        App.OpenWindows.RemoveAt(i);
                                        App.MainWindow.ReDrawWindows();
                                    }
                                }

                                if (foundBook != null)
                                {
                                    foundBook.RemoveBible();
                                }

                                if (App.InstalledBibles.InstalledBibles.ContainsKey(foundKey))
                                {
                                    App.InstalledBibles.InstalledBibles.Remove(foundKey);
                                }
                                else if(App.InstalledBibles.InstalledGeneralBooks.ContainsKey(foundKey))
                                {
                                    App.InstalledBibles.InstalledGeneralBooks.Remove(foundKey);
                                }
                                else
                                {
                                    App.InstalledBibles.InstalledCommentaries.Remove(foundKey);
                                }

                                App.InstalledBibles.Save();
                                this.LoadList();
                                this._isInSelectionChanged = false;
                            }));
                dialog.Commands.Add(new UICommand(Translations.Translate("Cancel")));
                dialog.ShowAsync();
            }

            this._isInSelectionChanged = false;
        }

        private void LoadList()
        {
            this.SelectListBibleDelete.Items.Clear();
            Dictionary<string, SwordBook> biblesAndCommentaries =
                App.InstalledBibles.InstalledBibles.Concat(App.InstalledBibles.InstalledCommentaries).Concat(App.InstalledBibles.InstalledGeneralBooks)
                   .ToDictionary(x => x.Key, x => x.Value);
            foreach (var book in biblesAndCommentaries)
            {
                var block = new TextBlock
                                {
                                    Text = book.Value.Name,
                                    Tag = book.Value.Sbmd.Initials,
                                    TextWrapping = TextWrapping.Wrap
                                };
                this.SelectListBibleDelete.Items.Add(block);
            }
        }

        private void MenuDeleteBibleClick(object sender, RoutedEventArgs e)
        {
            this.SelectBibleDeleteTitle.Text = Translations.Translate("Select bible to delete");
            this.LoadList();
            SideBarShowPopup(
                this.DeleteBiblePopup,
                this.MainPaneDeleteBiblePopup,
                this.scrollViewerBibleDelete,
                this.TopAppBar1,
                this.BottomAppBar);
        }

        #endregion
    }
}