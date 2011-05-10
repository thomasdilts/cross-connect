using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
///
/// <summary> Distribution License:
/// JSword is free software; you can redistribute it and/or modify it under
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
///
/// Copyright: 2011
///     The copyright to this program is held by Thomas Dilts
///  
namespace CrossConnect
{
    public partial class EditBookmarks : PhoneApplicationPage
    {
        public EditBookmarks()
        {
            InitializeComponent();
        }
        private bool isInSelectionChanged=false;
        private void SelectList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (isInSelectionChanged)
            {
                return;
            }
            isInSelectionChanged = true;
            MessageBoxResult result= MessageBox.Show(Translations.translate("Delete?"),"", MessageBoxButton.OKCancel);
            if (result.Equals(MessageBoxResult.OK))
            {
                int index = (int)((TextBlock)e.AddedItems[0]).Tag;
                App.placeMarkers.bookmarks.RemoveAt(index);
                LoadList();
                App.RaiseBookmarkChangeEvent();
            }
            else
            {
                SelectList.SelectedItem = null;
            }
            isInSelectionChanged = false;
        }

        private void LoadList()
        {
            SelectList.Items.Clear();
            List<string> allBookmarks = App.openWindows[0].state.source.makeListDisplayText(App.placeMarkers.bookmarks);
            //the list is a reversed list from the original list. So we must mark it with the correct reversed index.
            int j = allBookmarks.Count - 1;
            for (int i = 0; i < allBookmarks.Count; i++)
            {
                TextBlock block = new TextBlock();
                block.Text = allBookmarks[i].Replace("<p>", "").Replace("</p>", "");
                block.Tag = j--;
                block.TextWrapping = TextWrapping.Wrap;
                SelectList.Items.Add(block);
            }
        }
        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            PageTitle.Text = Translations.translate("Select bookmark to delete");    
        

            LoadList();
        }
    }
}