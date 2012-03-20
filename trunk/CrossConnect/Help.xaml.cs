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

    public partial class Help
    {
        #region Constructors

        public Help()
        {
            InitializeComponent();
        }

        #endregion Constructors

        #region Methods

        private void WebBrowserLoaded(object sender, RoutedEventArgs e)
        {
            object filePath;
            object title;
            object openWindowIndex;
            PhoneApplicationService.Current.State.TryGetValue("HelpWindowFileToLoad", out filePath);
            PhoneApplicationService.Current.State.TryGetValue("HelpWindowTitle", out title);
            PhoneApplicationService.Current.State.TryGetValue("openWindowIndex", out openWindowIndex);

            if (string.IsNullOrEmpty((string)filePath) || string.IsNullOrEmpty((string)title) || openWindowIndex == null)
            {
                if (NavigationService.CanGoBack)
                {
                    NavigationService.GoBack();
                }

                return;
            }

            PageTitle.Text = (string)title;
            Stream st = GetType().Assembly.GetManifestResourceStream((string)filePath);
            if (st != null)
            {
                var sr = new StreamReader(st);

                SerializableWindowState state = App.OpenWindows[(int)openWindowIndex].State;
                webBrowser1.NavigateToString(
                    BibleZtextReader.HtmlHeader(
                        App.DisplaySettings,
                        BrowserTitledWindow.GetBrowserColor("PhoneBackgroundColor"),
                        BrowserTitledWindow.GetBrowserColor("PhoneForegroundColor"),
                        BrowserTitledWindow.GetBrowserColor("PhoneAccentColor"),
                        state.HtmlFontSize,
                        Theme.FontFamilies[App.Themes.FontFamily]) + sr.ReadToEnd() + "</body></html>");
                sr.Close();
            }

            if (st != null)
            {
                st.Close();
            }
        }

        #endregion Methods
    }
}