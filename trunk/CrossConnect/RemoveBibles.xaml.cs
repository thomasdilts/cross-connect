#region Header

// <copyright file="RemoveBibles.xaml.cs" company="Thomas Dilts">
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
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;

    /// <summary>
    /// The remove bibles.
    /// </summary>
    public partial class RemoveBibles
    {
        #region Fields

        /// <summary>
        /// The _is in selection changed.
        /// </summary>
        private bool _isInSelectionChanged;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="RemoveBibles"/> class.
        /// </summary>
        public RemoveBibles()
        {
            InitializeComponent();
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// The load list.
        /// </summary>
        private void LoadList()
        {
            SelectList.Items.Clear();
            Dictionary<string, SwordBook> biblesAndCommentaries =
                App.InstalledBibles.InstalledBibles.Concat(App.InstalledBibles.InstalledCommentaries).ToDictionary(
                    x => x.Key, x => x.Value);
            foreach (var book in biblesAndCommentaries)
            {
                var block = new TextBlock
                    {
                       Text = book.Value.Name, Tag = book.Value.Sbmd.Initials, TextWrapping = TextWrapping.Wrap
                    };
                SelectList.Items.Add(block);
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
            PageTitle.Text = Translations.Translate("Select bible to delete");
            LoadList();
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
            if (_isInSelectionChanged)
            {
                return;
            }

            _isInSelectionChanged = true;
            SwordBook foundBook = null;
            string foundKey = string.Empty;
            var index = (string)((TextBlock)e.AddedItems[0]).Tag;
            Dictionary<string, SwordBook> biblesAndCommentaries =
                App.InstalledBibles.InstalledBibles.Concat(App.InstalledBibles.InstalledCommentaries).ToDictionary(
                    x => x.Key, x => x.Value);
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
                MessageBox.Show(Translations.Translate("You must have at least one bible"));
            }
            else
            {
                MessageBoxResult result = MessageBox.Show(
                    Translations.Translate("Delete?"), string.Empty, MessageBoxButton.OKCancel);
                if (result.Equals(MessageBoxResult.OK))
                {
                    for (int i = App.OpenWindows.Count() - 1; i >= 0; i--)
                    {
                        if (foundBook != null
                            &&
                            (App.OpenWindows[i].State.BibleToLoad.Equals(foundBook.Sbmd.InternalName)
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
                    else
                    {
                        App.InstalledBibles.InstalledCommentaries.Remove(foundKey);
                    }

                    App.InstalledBibles.Save();
                    LoadList();
                }
                else
                {
                    SelectList.SelectedItem = null;
                }
            }

            _isInSelectionChanged = false;
        }

        #endregion Methods
    }
}