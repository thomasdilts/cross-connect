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