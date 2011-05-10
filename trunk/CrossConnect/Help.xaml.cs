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
using System.IO;
using SwordBackend;
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
    public partial class Help : PhoneApplicationPage
    {
        public Help()
        {
            InitializeComponent();
        }

        private void WebBrowser_Loaded(object sender, RoutedEventArgs e)
        {
            PageTitle.Text = App.helpstart.title;
            Stream st=this.GetType().Assembly.GetManifestResourceStream(App.helpstart.embeddedFilePath);
            StreamReader sr = new StreamReader(st);
            var state = App.openWindows[App.windowSettings.openWindowIndex].state;
            webBrowser1.NavigateToString(BibleZtextReader.HtmlHeader(
                BrowserTitledWindow.GetBrowserColor("PhoneBackgroundColor"),
                BrowserTitledWindow.GetBrowserColor("PhoneForegroundColor"),
                BrowserTitledWindow.GetBrowserColor("PhoneAccentColor"),
                state.htmlFontSize) + sr.ReadToEnd() + "</body></html>");
            sr.Close();
            st.Close();
        }
    }
}