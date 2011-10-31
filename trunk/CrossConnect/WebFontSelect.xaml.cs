#region Header

// <copyright file="SelectLanguage.xaml.cs" company="Thomas Dilts">
//
// CrossConnect Bible and Bible Commentary Reader for CrossWire.org
// Copyright (C) 2011 Thomas Dilts
//
// This program is free software: you can redistribute it and/or modify
// it under the +terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see http://www.gnu.org/licenses/.
// </copyright>
// <summary>
// Email: thomas@chaniel.se
// </summary>
// <author>Thomas Dilts</author>

#endregion Header

namespace CrossConnect
{
    using System.Text;
    using System.Windows;
    using Microsoft.Phone.Shell;

    using Sword.reader;

    public partial class WebFontSelect
    {
        #region Constructors

        public WebFontSelect()
        {
            InitializeComponent();
        }

        #endregion Constructors

        #region Methods

        private void AutoRotatePageLoaded(object sender, RoutedEventArgs e)
        {
            PageTitle.Text = Translations.Translate("Select the language");

            var sb=new StringBuilder();
            foreach (var font in Theme.FontFamilies)
            {
                sb.Append("<p><a style=\"" + font.Value + "color:" + BrowserTitledWindow.GetBrowserColor("PhoneForegroundColor") + ";decoration:none\" href=\"#\" onclick=\"window.external.Notify('" +
                                                        font.Key + "'); event.returnValue=false; return false;\" >" + font.Key + "</a></p>");
            }
            webBrowser1.NavigateToString(BibleZtextReader.HtmlHeader(
                App.DisplaySettings,
                BrowserTitledWindow.GetBrowserColor("PhoneBackgroundColor"),
                BrowserTitledWindow.GetBrowserColor("PhoneForegroundColor"),
                BrowserTitledWindow.GetBrowserColor("PhoneAccentColor"),
                20, "") + sb + "</body></html>");
        }

        private void WebBrowser1Loaded(object sender, RoutedEventArgs e)
        {
        }

        private void WebBrowser1ScriptNotify(object sender, Microsoft.Phone.Controls.NotifyEventArgs e)
        {
            PhoneApplicationService.Current.State["WebFontSelectWindowSelection"] = e.Value;
            if (NavigationService.CanGoBack)
            {
                NavigationService.GoBack();
            }
        }

        private void WebBrowser1Unloaded(object sender, RoutedEventArgs e)
        {
        }

        #endregion Methods
    }
}