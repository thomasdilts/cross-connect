#region Header

// <copyright file="EditBookmarks.xaml.cs" company="Thomas Dilts">
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
    using System.Windows;
    using System.Windows.Controls;

    public partial class EditBookmarks
    {
        #region Fields

        private bool _isInSelectionChanged;

        #endregion Fields

        #region Constructors

        public EditBookmarks()
        {
            InitializeComponent();
        }

        #endregion Constructors

        #region Methods

        private void LoadList()
        {
            SelectList.Items.Clear();
            var allBookmarks = App.OpenWindows[0].State.Source.MakeListDisplayText(App.DisplaySettings,
                                                                                   App.PlaceMarkers.Bookmarks);
            // the list is a reversed list from the original list. So we must mark it with the correct reversed index.
            int j = allBookmarks.Count - 1;
            foreach (string t in allBookmarks)
            {
                var block = new TextBlock
                                {
                                    Text = t.Replace("<p>", "").Replace("</p>", ""),
                                    Tag = j--,
                                    TextWrapping = TextWrapping.Wrap
                                };
                SelectList.Items.Add(block);
            }
        }

        private void PhoneApplicationPageLoaded(object sender, RoutedEventArgs e)
        {
            PageTitle.Text = Translations.Translate("Select bookmark to delete");

            LoadList();
        }

        private void SelectListSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_isInSelectionChanged)
            {
                return;
            }
            _isInSelectionChanged = true;
            var result = MessageBox.Show(Translations.Translate("Delete?"), "", MessageBoxButton.OKCancel);
            if (result.Equals(MessageBoxResult.OK))
            {
                var index = (int) ((TextBlock) e.AddedItems[0]).Tag;
                App.PlaceMarkers.Bookmarks.RemoveAt(index);
                LoadList();
                App.RaiseBookmarkChangeEvent();
            }
            else
            {
                SelectList.SelectedItem = null;
            }
            _isInSelectionChanged = false;
        }

        #endregion Methods
    }
}