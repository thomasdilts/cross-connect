// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EditBookmarks.xaml.cs" company="">
//   
// </copyright>
// <summary>
//   The edit bookmarks.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#region Header

// <copyright file="EditBookmarks.xaml.cs" company="Thomas Dilts">
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
    using System.Windows;
    using System.Windows.Controls;

    /// <summary>
    /// The edit bookmarks.
    /// </summary>
    public partial class EditBookmarks
    {
        #region Constants and Fields

        /// <summary>
        /// The _is in selection changed.
        /// </summary>
        private bool _isInSelectionChanged;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="EditBookmarks"/> class.
        /// </summary>
        public EditBookmarks()
        {
            this.InitializeComponent();
        }

        #endregion

        #region Methods

        /// <summary>
        /// The load list.
        /// </summary>
        private void LoadList()
        {
            this.SelectList.Items.Clear();
            List<string> allBookmarks = App.OpenWindows[0].State.Source.MakeListDisplayText(
                App.DisplaySettings, App.PlaceMarkers.Bookmarks);

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
                this.SelectList.Items.Add(block);
            }
        }

        /// <summary>
        /// The phone application page loaded.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void PhoneApplicationPageLoaded(object sender, RoutedEventArgs e)
        {
            this.PageTitle.Text = Translations.Translate("Select bookmark to delete");

            this.LoadList();
        }

        /// <summary>
        /// The select list selection changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void SelectListSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this._isInSelectionChanged)
            {
                return;
            }

            this._isInSelectionChanged = true;
            MessageBoxResult result = MessageBox.Show(Translations.Translate("Delete?"), string.Empty, MessageBoxButton.OKCancel);
            if (result.Equals(MessageBoxResult.OK))
            {
                var index = (int)((TextBlock)e.AddedItems[0]).Tag;
                App.PlaceMarkers.Bookmarks.RemoveAt(index);
                this.LoadList();
                App.RaiseBookmarkChangeEvent();
            }
            else
            {
                this.SelectList.SelectedItem = null;
            }

            this._isInSelectionChanged = false;
        }

        #endregion
    }
}