// <copyright file="MediaPlayerMenu.cs" company="Thomas Dilts">
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
    using System.Linq;

    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;

    public sealed partial class MediaPlayerWindow
    {
        #region Methods

        private void ButMenuClick(object sender, RoutedEventArgs e)
        {
            if (!this.MenuPopup.IsOpen)
            {
                this.MenuPopup.IsOpen = true;
            }
            this.MenuPopup.UpdateLayout();
            this.SubMenuStackPanel.UpdateLayout();

            // Which items are to be in the menu.
            int numberOfWindowsInMyColumn = App.OpenWindows.Count(win => win.State.Window == this._state.Window);
            this.MoveWindowRight.Visibility = (this.State.Window < 8) ? Visibility.Visible : Visibility.Collapsed;
            this.MoveWindowLeft.Visibility = (this.State.Window > 0) ? Visibility.Visible : Visibility.Collapsed;
            this.WindowSmaller.Visibility = (numberOfWindowsInMyColumn > 1 && this.State.NumRowsIown > 1)
                                                ? Visibility.Visible
                                                : Visibility.Collapsed;
            this.WindowLarger.Visibility = (numberOfWindowsInMyColumn > 1 && this.State.NumRowsIown < 10)
                                               ? Visibility.Visible
                                               : Visibility.Collapsed;

            this.MoveWindowRight.Content = Translations.Translate("Move this window to the right");
            this.MoveWindowLeft.Content = Translations.Translate("Move this window to the left");
            this.WindowSmaller.Content = Translations.Translate("Make this window smaller");
            this.WindowLarger.Content = Translations.Translate("Make this window larger");

            MakeSurePopupIsOnScreen(
                this.SubMenuStackPanel.ActualHeight,
                this.SubMenuStackPanel.ActualWidth,
                this.ButMenu,
                this,
                this.MenuPopup);

        }

        private void SubMenuMenuPopup_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var itemSelected = (ListBoxItem)e.AddedItems.FirstOrDefault();
            if (itemSelected == null)
            {
                return;
            }
            switch (itemSelected.Name)
            {
                case "MoveWindowRight":
                    this.State.Window += 1;
                    if (this.State.Window > 8)
                    {
                        this.State.Window = 8;
                    }
                    this.ShowUserInterface(true);
                    App.MainWindow.ReDrawWindows(false);
                    this.ShowUserInterface(false);
                    break;
                case "MoveWindowLeft":
                    this.State.Window -= 1;
                    if (this.State.Window < 0)
                    {
                        this.State.Window = 0;
                    }
                    this.ShowUserInterface(true);
                    App.MainWindow.ReDrawWindows(false);
                    this.ShowUserInterface(false);
                    break;
                case "WindowSmaller":
                    this.State.NumRowsIown -= 1;
                    if (this.State.NumRowsIown < 1)
                    {
                        this.State.NumRowsIown = 1;
                    }
                    this.ShowUserInterface(true);
                    App.MainWindow.ReDrawWindows(false);
                    this.ShowUserInterface(false);
                    break;
                case "WindowLarger":
                    this.State.NumRowsIown += 1;
                    if (this.State.NumRowsIown > 10)
                    {
                        this.State.NumRowsIown = 10;
                    }
                    this.ShowUserInterface(true);
                    App.MainWindow.ReDrawWindows(false);
                    this.ShowUserInterface(false);
                    break;
            }
            this.SubMenuMenuPopup.SelectedItem = null;
        }

        #endregion
    }
}