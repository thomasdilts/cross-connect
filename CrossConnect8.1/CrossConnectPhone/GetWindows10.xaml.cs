#region Header

// <copyright file="Help.xaml.cs" company="Thomas Dilts">
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
    using System.IO;
    using System.Windows;

    using Microsoft.Phone.Shell;

    using Sword.reader;
    using System.Threading;
    using System.Globalization;

    public partial class GetWindows10
    {
        #region Constructors

        public GetWindows10()
        {
            InitializeComponent();
        }

        #endregion Constructors
        private void PhoneApplicationPageLoaded(object sender, RoutedEventArgs e)
        {
            PageTitle.Text = Translations.Translate("Download the latest version that replaces this program");
            PageDescription.Text = "Cross Connect";
            DownloadNow.Content = Translations.Translate("Download now");
            Later.Content = Translations.Translate("Not now");
        }


        private void DownloadNow_Click(object sender, RoutedEventArgs e)
        {
            Windows.System.Launcher.LaunchUriAsync(new System.Uri("ms-windows-store:PDP?PFN=57294ThomasDilts.BibleReaderCrossConnect_pyd4hamsr8ypw"));
            NavigationService.GoBack();
        }

        private void Later_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}