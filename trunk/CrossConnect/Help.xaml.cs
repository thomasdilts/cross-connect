/// <summary>
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
/// <copyright file="Help.xaml.cs" company="Thomas Dilts">
///     Thomas Dilts. All rights reserved.
/// </copyright>
/// <author>Thomas Dilts</author>
namespace CrossConnect
{
    using System;
    using System.Collections.Generic;
    using System.IO;
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
    using Microsoft.Phone.Shell;

    using SwordBackend;

    public partial class Help : AutoRotatePage
    {
        #region Constructors

        public Help()
        {
            InitializeComponent();
        }

        #endregion Constructors

        #region Methods

        private void WebBrowser_Loaded(object sender, RoutedEventArgs e)
        {
            object filePath = null;
            object title = null;
            object openWindowIndex = null;
            PhoneApplicationService.Current.State.TryGetValue("HelpWindowFileToLoad", out filePath);
            PhoneApplicationService.Current.State.TryGetValue("HelpWindowTitle", out title);
            PhoneApplicationService.Current.State.TryGetValue("openWindowIndex", out openWindowIndex);

            if (string.IsNullOrEmpty((string)filePath) || string.IsNullOrEmpty((string)title) || openWindowIndex==null)
            {
                if (this.NavigationService.CanGoBack)
                {
                    this.NavigationService.GoBack();
                }
                return;
            }
            PageTitle.Text = (string)title;
            Stream st = this.GetType().Assembly.GetManifestResourceStream((string)filePath);
            StreamReader sr = new StreamReader(st);

            var state = App.openWindows[(int)openWindowIndex].state;
            webBrowser1.NavigateToString(BibleZtextReader.HtmlHeader(
                BrowserTitledWindow.GetBrowserColor("PhoneBackgroundColor"),
                BrowserTitledWindow.GetBrowserColor("PhoneForegroundColor"),
                BrowserTitledWindow.GetBrowserColor("PhoneAccentColor"),
                state.htmlFontSize) + sr.ReadToEnd() + "</body></html>");
            sr.Close();
            st.Close();
        }

        #endregion Methods
    }
}