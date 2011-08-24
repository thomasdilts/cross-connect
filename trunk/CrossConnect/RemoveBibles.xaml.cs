﻿/// <summary>
/// Distribution License:
/// CrossConnect is free software; you can redistribute it and/or modify it under
/// the terms of the GNU General Public License, version 3 as published by
/// the Free Software Foundation. This program is distributed in the hope
/// that it will be useful, but WITHOUT ANY WARRANTY; without even the
/// implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
/// See the GNU General Public License for more details.
///
/// The License is available on the internet at:
///       http://www.gnu.org/copyleft/gpl.html
/// or by writing to:
///      Free Software Foundation, Inc.
///      59 Temple Place - Suite 330
///      Boston, MA 02111-1307, USA
/// </summary>
/// <copyright file="RemoveBibles.cs" company="Thomas Dilts">
///     Thomas Dilts. All rights reserved.
/// </copyright>
/// <author>Thomas Dilts</author>
namespace CrossConnect
{
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;

    public partial class RemoveBibles : AutoRotatePage
    {
        #region Fields

        private bool isInSelectionChanged = false;

        #endregion Fields

        #region Constructors

        public RemoveBibles()
        {
            InitializeComponent();
        }

        #endregion Constructors

        #region Methods

        private void LoadList()
        {
            SelectList.Items.Clear();
            foreach (var book in App.installedBibles.installedBibles)
            {
                TextBlock block = new TextBlock();
                block.Text = book.Value.Name;
                block.Tag = book.Value.sbmd.Initials;
                block.TextWrapping = TextWrapping.Wrap;
                SelectList.Items.Add(block);
            }
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            PageTitle.Text = Translations.translate("Select bible to delete");
            LoadList();
        }

        private void SelectList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (isInSelectionChanged)
            {
                return;
            }
            isInSelectionChanged = true;
            if (App.installedBibles.installedBibles.Count == 1)
            {
                MessageBox.Show(Translations.translate("You must have at least one bible"));
            }
            else
            {
                MessageBoxResult result = MessageBox.Show(Translations.translate("Delete?"), "", MessageBoxButton.OKCancel);
                if (result.Equals(MessageBoxResult.OK))
                {
                    string index = (string)((TextBlock)e.AddedItems[0]).Tag;
                    foreach (var book in App.installedBibles.installedBibles)
                    {
                        if (book.Value.sbmd.Initials.Equals(index))
                        {
                            for (int i = App.openWindows.Count()-1; i >=0 ; i--)
                            {
                                if (App.openWindows[i].state.bibleToLoad.Equals(book.Value.sbmd.internalName) && App.mainWindow!=null)
                                {
                                    App.openWindows.RemoveAt(i);
                                    App.mainWindow.ReDrawWindows();
                                }
                            }
                            book.Value.RemoveBible();
                            App.installedBibles.installedBibles.Remove(book.Key);
                            App.installedBibles.save();
                            break;
                        }
                    }
                    LoadList();
                }
                else
                {
                    SelectList.SelectedItem = null;
                }
            }
            isInSelectionChanged = false;
        }

        #endregion Methods
    }
}