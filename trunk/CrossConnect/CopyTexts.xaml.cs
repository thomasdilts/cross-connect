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
    using System.Windows;

    public partial class CopyTexts
    {
        #region Fields

        private bool _isInSelectionChanged;

        #endregion Fields

        #region Constructors

        public CopyTexts()
        {
            InitializeComponent();
        }

        #endregion Constructors

        #region Methods

        private void PhoneApplicationPageLoaded(object sender, RoutedEventArgs e)
        {
            PageTitle.Text = Translations.Translate("Copy");
            SelectAll.Content = Translations.Translate("Select all");
            string textsWithTitles;
            string titlesOnly;
            MainPageSplit.GetLast3SecondsChosenVerses(out textsWithTitles, out titlesOnly);
            SelectText.Text = textsWithTitles;
            SelectText.Focus();
        }

        private void SelectAllClick(object sender, RoutedEventArgs e)
        {
            SelectText.Focus();
            SelectText.SelectAll();
        }

        #endregion Methods
    }
}