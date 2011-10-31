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
    using System;
    using System.Collections.Generic;
    using System.IO.IsolatedStorage;
    using System.Linq;
    using System.Net;
    using System.Windows;
    using System.Windows.Controls;
    using System.Xml;

    using ICSharpCode.SharpZipLib.Zip;

    using Microsoft.Phone.Shell;

    public partial class Themes
    {
        #region Fields

        private WebClient _client;

        #endregion Fields

        #region Constructors

        public Themes()
        {
            InitializeComponent();
        }

        #endregion Constructors

        #region Methods

        private void AutoRotatePageBackKeyPress(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //save the current selection
            var uniqId = (Guid)((TextBlock)SelectList.SelectedItem).Tag;
            App.Themes.CurrentTheme = uniqId;

            //all the windows must be redrawn
            for (int i = App.OpenWindows.Count - 1; i >= 0; i--)
            {
                App.OpenWindows[i].ForceReload = true;
            }
        }

        private void AutoRotatePageLoaded(object sender, RoutedEventArgs e)
        {
            PageTitle.Text = Translations.Translate("Select the theme");

            ((ApplicationBarIconButton)ApplicationBar.Buttons[0]).Text = Translations.Translate("Download");
            ((ApplicationBarIconButton)ApplicationBar.Buttons[1]).Text = Translations.Translate("New");
            ((ApplicationBarIconButton)ApplicationBar.Buttons[2]).Text = Translations.Translate("Edit");

            ((ApplicationBarMenuItem)ApplicationBar.MenuItems[0]).Text = Translations.Translate("Download");
            ((ApplicationBarMenuItem)ApplicationBar.MenuItems[1]).Text = Translations.Translate("New");
            ((ApplicationBarMenuItem)ApplicationBar.MenuItems[2]).Text = Translations.Translate("Edit");
            ((ApplicationBarMenuItem)ApplicationBar.MenuItems[3]).Text = Translations.Translate("Delete");

            SelectList.Items.Clear();
            //add the default
            var defaultTheme = new TextBlock
                                   {
                                       Text = Translations.Translate("Phone default theme"),
                                       Tag = App.Themes.GetUniqueGuidKey(),
                                       TextWrapping = TextWrapping.Wrap
                                   };
            SelectList.Items.Add(defaultTheme);
            SelectList.SelectedIndex = 0;
            //add the rest
            foreach (var tema in App.Themes.Themes.OrderBy(p => p.Value.Name))
            {
                SelectList.Items.Add(new TextBlock
                {
                    Text = tema.Value.Name,
                    Tag = tema.Value.UniqId,
                    TextWrapping = TextWrapping.Wrap
                });
                if (App.Themes.CurrentTheme.Equals(tema.Value.UniqId))
                {
                    SelectList.SelectedIndex = SelectList.Items.Count() - 1;
                }
            }
        }

        private void ButChangeThemeClick(object sender, EventArgs e)
        {
            MenuChangeThemeClick(sender, e);
        }

        private void ButCreateNewThemeClick(object sender, EventArgs e)
        {
            MenuCreateNewThemeClick(sender, e);
        }

        private void ButDownloadThemesClick(object sender, EventArgs e)
        {
            MenuDownloadThemesClick(sender, e);
        }

        private void ClientDownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            progressBarDownloadThemes.Value = e.ProgressPercentage;
        }

        private void ClientOpenReadCompleted(object sender, OpenReadCompletedEventArgs e)
        {
            var themes = new Theme();
            SelectList.Visibility = Visibility.Visible;
            progressBarDownloadThemes.Visibility = Visibility.Collapsed;
            try
            {
                ZipInputStream zipStream;
                try
                {
                    zipStream = new ZipInputStream(e.Result);
                }
                catch (Exception e2)
                {
                    MessageBox.Show(
                        Translations.Translate("An error occurred trying to connect to the network. Try again later.") +
                        "; " + e2.Message);
                    return;
                }
                using (var isolatedStorageRoot = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    while (true)
                    {
                        ZipEntry entry = zipStream.GetNextEntry();
                        if (entry == null)
                        {
                            break;
                        }
                        string entrypath = entry.Name.Replace("/","");
                        if (entrypath.Equals("themes.xml"))
                        {
                            themes.FromStream(zipStream,true);
                        }
                        else
                        {
                            if (!isolatedStorageRoot.DirectoryExists(App.WebDirIsolated + "/images"))
                                isolatedStorageRoot.CreateDirectory(App.WebDirIsolated + "/images");
                            if (isolatedStorageRoot.DirectoryExists(App.WebDirIsolated + "/images/" + entrypath))
                                isolatedStorageRoot.DeleteFile(App.WebDirIsolated + "/images/" + entrypath);
                            IsolatedStorageFileStream fStream = isolatedStorageRoot.CreateFile(App.WebDirIsolated + "/images/" + entrypath);
                            var buffer = new byte[10000];
                            int len;
                            while ((len = zipStream.Read(buffer, 0, buffer.GetUpperBound(0))) > 0)
                            {
                                fStream.Write(buffer, 0, len);
                            }
                            fStream.Close();
                        }
                    }
                }
            }
            catch (Exception exp)
            {
                Logger.Fail(exp.ToString());
                MessageBox.Show(
                    Translations.Translate("An error occurred trying to connect to the network. Try again later.") +
                    "; " + exp.Message);
                return;
            }
            App.Themes.Merge(themes);
            AutoRotatePageLoaded(null, null);
        }

        private void MenuChangeThemeClick(object sender, EventArgs e)
        {
            //save the current selection
            var uniqId = (Guid)((TextBlock)SelectList.SelectedItem).Tag;
            App.Themes.CurrentTheme = uniqId;
            //call the editing page
            PhoneApplicationService.Current.State["ThemeColorsThemeToChange"] = uniqId;
            PhoneApplicationService.Current.State.Remove("WebFontSelectWindowSelection");
            NavigationService.Navigate(new Uri("/ThemeColors.xaml", UriKind.Relative));
        }

        private void MenuCreateNewThemeClick(object sender, EventArgs e)
        {
            var theme = new Theme {UniqId = App.Themes.GetUniqueGuidKey()};
            App.Themes.Themes[theme.UniqId] =theme;
            theme.Name = Translations.Translate("New") + " " + (int)(new Random().NextDouble() * 100);
            PhoneApplicationService.Current.State["ThemeColorsThemeToChange"] = theme.UniqId;
            PhoneApplicationService.Current.State.Remove("WebFontSelectWindowSelection");
            NavigationService.Navigate(new Uri("/ThemeColors.xaml", UriKind.Relative));
        }

        private void MenuDeleteThemeClick(object sender, EventArgs e)
        {
            var uniqId = (Guid)((TextBlock)SelectList.SelectedItem).Tag;
            SelectList.Items.RemoveAt(SelectList.SelectedIndex);
            SelectList.SelectedIndex = 0;
            if(uniqId.Equals(App.Themes.CurrentTheme))
            {
                var defaultId = (Guid)((TextBlock)SelectList.Items[0]).Tag;
                App.Themes.CurrentTheme = defaultId;
            }
            App.Themes.Themes.Remove(uniqId);
        }

        private void MenuDownloadThemesClick(object sender, EventArgs e)
        {
            const string url = "http://www.chaniel.se/crossconnect/bibles/themes/themes.zip";
            try
            {
                var source = new Uri(url);
                SelectList.Visibility= Visibility.Collapsed;
                progressBarDownloadThemes.Visibility = Visibility.Visible;
                progressBarDownloadThemes.Value = 5;
                _client = new WebClient();
                _client.DownloadProgressChanged += ClientDownloadProgressChanged;
                _client.OpenReadCompleted += ClientOpenReadCompleted;
                Logger.Debug("download start");
                _client.OpenReadAsync(source);
                Logger.Debug("DownloadStringAsync returned");
            }
            catch (Exception eee)
            {
                SelectList.Visibility = Visibility.Visible;
                progressBarDownloadThemes.Visibility = Visibility.Collapsed;
                Logger.Fail(eee.ToString());
                MessageBox.Show(
                    Translations.Translate("An error occurred trying to connect to the network. Try again later.") +
                    "; " + eee.Message);
            }
        }

        private void SelectListSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ((ApplicationBarIconButton)ApplicationBar.Buttons[2]).IsEnabled = SelectList.SelectedIndex != 0;
            ((ApplicationBarMenuItem)ApplicationBar.MenuItems[2]).IsEnabled = SelectList.SelectedIndex != 0;
            ((ApplicationBarMenuItem)ApplicationBar.MenuItems[3]).IsEnabled = SelectList.SelectedIndex != 0;
        }

        #endregion Methods
    }
}