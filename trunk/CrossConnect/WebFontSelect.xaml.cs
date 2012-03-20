#region Header

// <copyright file="WebFontSelect.xaml.cs" company="Thomas Dilts">
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
    using System.Text;
    using System.Windows;

    using Microsoft.Phone.Controls;
    using Microsoft.Phone.Shell;

    using Sword.reader;

    /// <summary>
    /// The web font select.
    /// </summary>
    public partial class WebFontSelect
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="WebFontSelect"/> class.
        /// </summary>
        public WebFontSelect()
        {
            InitializeComponent();
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// The auto rotate page loaded.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void AutoRotatePageLoaded(object sender, RoutedEventArgs e)
        {
            PageTitle.Text = Translations.Translate("Select the language");

            var sb = new StringBuilder();
            foreach (var font in Theme.FontFamilies)
            {
                sb.Append(
                    "<p><a style=\"" + font.Value + "color:"
                    + BrowserTitledWindow.GetBrowserColor("PhoneForegroundColor")
                    + ";decoration:none\" href=\"#\" onclick=\"window.external.Notify('" + font.Key
                    + "'); event.returnValue=false; return false;\" >" + font.Key + "</a></p>");
            }

            webBrowser1.NavigateToString(
                BibleZtextReader.HtmlHeader(
                    App.DisplaySettings,
                    BrowserTitledWindow.GetBrowserColor("PhoneBackgroundColor"),
                    BrowserTitledWindow.GetBrowserColor("PhoneForegroundColor"),
                    BrowserTitledWindow.GetBrowserColor("PhoneAccentColor"),
                    20,
                    string.Empty) + sb + "</body></html>");
        }

        /// <summary>
        /// The web browser 1 loaded.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void WebBrowser1Loaded(object sender, RoutedEventArgs e)
        {
        }

        /// <summary>
        /// The web browser 1 script notify.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void WebBrowser1ScriptNotify(object sender, NotifyEventArgs e)
        {
            PhoneApplicationService.Current.State["WebFontSelectWindowSelection"] = e.Value;
            if (NavigationService.CanGoBack)
            {
                NavigationService.GoBack();
            }
        }

        /// <summary>
        /// The web browser 1 unloaded.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void WebBrowser1Unloaded(object sender, RoutedEventArgs e)
        {
        }

        #endregion Methods
    }
}