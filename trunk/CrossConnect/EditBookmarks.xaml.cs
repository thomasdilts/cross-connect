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
            MessageBoxResult result= MessageBox.Show("Delete?","", MessageBoxButton.OKCancel);
            if (result.Equals(MessageBoxResult.OK))
            {
                int index = (int)((TextBlock)e.AddedItems[0]).Tag;
                App.placeMarkers.bookmarks.RemoveAt(index);
                LoadList();
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
            
            LoadList();
        }
    }
}