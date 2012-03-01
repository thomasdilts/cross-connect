// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Themes.xaml.cs" company="">
//   
// </copyright>
// <summary>
//   The themes.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#region Header

// <copyright file="Themes.xaml.cs" company="Thomas Dilts">
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
    using System;
    using System.ComponentModel;
    using System.IO.IsolatedStorage;
    using System.Linq;
    using System.Net;
    using System.Windows;
    using System.Windows.Controls;

    using ICSharpCode.SharpZipLib.Zip;

    using Microsoft.Phone.Shell;

    /// <summary>
    /// The themes.
    /// </summary>
    public partial class Themes
    {
        #region Constants and Fields

        /// <summary>
        /// The _client.
        /// </summary>
        private WebClient _client;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Themes"/> class.
        /// </summary>
        public Themes()
        {
            this.InitializeComponent();
        }

        #endregion

        #region Methods

        /// <summary>
        /// The auto rotate page back key press.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void AutoRotatePageBackKeyPress(object sender, CancelEventArgs e)
        {
            // save the current selection
            var uniqId = (Guid)((TextBlock)this.SelectList.SelectedItem).Tag;
            App.Themes.CurrentTheme = uniqId;

            // all the windows must be redrawn
            for (int i = App.OpenWindows.Count - 1; i >= 0; i--)
            {
                App.OpenWindows[i].ForceReload = true;
            }
        }

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
            this.PageTitle.Text = Translations.Translate("Select the theme");

            ((ApplicationBarIconButton)this.ApplicationBar.Buttons[0]).Text = Translations.Translate("Download");
            ((ApplicationBarIconButton)this.ApplicationBar.Buttons[1]).Text = Translations.Translate("New");
            ((ApplicationBarIconButton)this.ApplicationBar.Buttons[2]).Text = Translations.Translate("Edit");

            ((ApplicationBarMenuItem)this.ApplicationBar.MenuItems[0]).Text = Translations.Translate("Download");
            ((ApplicationBarMenuItem)this.ApplicationBar.MenuItems[1]).Text = Translations.Translate("New");
            ((ApplicationBarMenuItem)this.ApplicationBar.MenuItems[2]).Text = Translations.Translate("Edit");
            ((ApplicationBarMenuItem)this.ApplicationBar.MenuItems[3]).Text = Translations.Translate("Delete");

            this.SelectList.Items.Clear();

            // add the default
            var defaultTheme = new TextBlock
                {
                    Text = Translations.Translate("Phone default theme"), 
                    Tag = App.Themes.GetUniqueGuidKey(), 
                    TextWrapping = TextWrapping.Wrap
                };
            this.SelectList.Items.Add(defaultTheme);
            this.SelectList.SelectedIndex = 0;

            // add the rest
            foreach (var tema in App.Themes.Themes.OrderBy(p => p.Value.Name))
            {
                this.SelectList.Items.Add(
                    new TextBlock { Text = tema.Value.Name, Tag = tema.Value.UniqId, TextWrapping = TextWrapping.Wrap });
                if (App.Themes.CurrentTheme.Equals(tema.Value.UniqId))
                {
                    this.SelectList.SelectedIndex = this.SelectList.Items.Count() - 1;
                }
            }
        }

        /// <summary>
        /// The but change theme click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void ButChangeThemeClick(object sender, EventArgs e)
        {
            this.MenuChangeThemeClick(sender, e);
        }

        /// <summary>
        /// The but create new theme click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void ButCreateNewThemeClick(object sender, EventArgs e)
        {
            this.MenuCreateNewThemeClick(sender, e);
        }

        /// <summary>
        /// The but download themes click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void ButDownloadThemesClick(object sender, EventArgs e)
        {
            this.MenuDownloadThemesClick(sender, e);
        }

        /// <summary>
        /// The client download progress changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void ClientDownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            this.progressBarDownloadThemes.Value = e.ProgressPercentage;
        }

        /// <summary>
        /// The client open read completed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void ClientOpenReadCompleted(object sender, OpenReadCompletedEventArgs e)
        {
            var themes = new Theme();
            this.SelectList.Visibility = Visibility.Visible;
            this.progressBarDownloadThemes.Visibility = Visibility.Collapsed;
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
                        Translations.Translate("An error occurred trying to connect to the network. Try again later.")
                        + "; " + e2.Message);
                    return;
                }

                using (IsolatedStorageFile isolatedStorageRoot = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    while (true)
                    {
                        ZipEntry entry = zipStream.GetNextEntry();
                        if (entry == null)
                        {
                            break;
                        }

                        string entrypath = entry.Name.Replace("/", string.Empty);
                        if (entrypath.Equals("themes.xml"))
                        {
                            themes.FromStream(zipStream, true);
                        }
                        else
                        {
                            if (!isolatedStorageRoot.DirectoryExists(App.WebDirIsolated + "/images"))
                            {
                                isolatedStorageRoot.CreateDirectory(App.WebDirIsolated + "/images");
                            }

                            if (isolatedStorageRoot.DirectoryExists(App.WebDirIsolated + "/images/" + entrypath))
                            {
                                isolatedStorageRoot.DeleteFile(App.WebDirIsolated + "/images/" + entrypath);
                            }

                            IsolatedStorageFileStream fStream =
                                isolatedStorageRoot.CreateFile(App.WebDirIsolated + "/images/" + entrypath);
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
                    Translations.Translate("An error occurred trying to connect to the network. Try again later.")
                    + "; " + exp.Message);
                return;
            }

            App.Themes.Merge(themes);
            this.AutoRotatePageLoaded(null, null);
        }

        /// <summary>
        /// The menu change theme click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void MenuChangeThemeClick(object sender, EventArgs e)
        {
            // save the current selection
            var uniqId = (Guid)((TextBlock)this.SelectList.SelectedItem).Tag;
            App.Themes.CurrentTheme = uniqId;

            // call the editing page
            PhoneApplicationService.Current.State["ThemeColorsThemeToChange"] = uniqId;
            PhoneApplicationService.Current.State.Remove("WebFontSelectWindowSelection");
            this.NavigationService.Navigate(new Uri("/ThemeColors.xaml", UriKind.Relative));
        }

        /// <summary>
        /// The menu create new theme click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void MenuCreateNewThemeClick(object sender, EventArgs e)
        {
            var theme = new Theme { UniqId = App.Themes.GetUniqueGuidKey() };
            App.Themes.Themes[theme.UniqId] = theme;
            theme.Name = Translations.Translate("New") + " " + (int)(new Random().NextDouble() * 100);
            PhoneApplicationService.Current.State["ThemeColorsThemeToChange"] = theme.UniqId;
            PhoneApplicationService.Current.State.Remove("WebFontSelectWindowSelection");
            this.NavigationService.Navigate(new Uri("/ThemeColors.xaml", UriKind.Relative));
        }

        /// <summary>
        /// The menu delete theme click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void MenuDeleteThemeClick(object sender, EventArgs e)
        {
            var uniqId = (Guid)((TextBlock)this.SelectList.SelectedItem).Tag;
            this.SelectList.Items.RemoveAt(this.SelectList.SelectedIndex);
            this.SelectList.SelectedIndex = 0;
            if (uniqId.Equals(App.Themes.CurrentTheme))
            {
                var defaultId = (Guid)((TextBlock)this.SelectList.Items[0]).Tag;
                App.Themes.CurrentTheme = defaultId;
            }

            App.Themes.Themes.Remove(uniqId);
        }

        /// <summary>
        /// The menu download themes click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void MenuDownloadThemesClick(object sender, EventArgs e)
        {
            const string url = "http://www.cross-connect.se/bibles/themes/themes.zip";
            try
            {
                var source = new Uri(url);
                this.SelectList.Visibility = Visibility.Collapsed;
                this.progressBarDownloadThemes.Visibility = Visibility.Visible;
                this.progressBarDownloadThemes.Value = 5;
                this._client = new WebClient();
                this._client.DownloadProgressChanged += this.ClientDownloadProgressChanged;
                this._client.OpenReadCompleted += this.ClientOpenReadCompleted;
                Logger.Debug("download start");
                this._client.OpenReadAsync(source);
                Logger.Debug("DownloadStringAsync returned");
            }
            catch (Exception eee)
            {
                this.SelectList.Visibility = Visibility.Visible;
                this.progressBarDownloadThemes.Visibility = Visibility.Collapsed;
                Logger.Fail(eee.ToString());
                MessageBox.Show(
                    Translations.Translate("An error occurred trying to connect to the network. Try again later.")
                    + "; " + eee.Message);
            }
        }

        /// <summary>
        /// The select list selection changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void SelectListSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ((ApplicationBarIconButton)this.ApplicationBar.Buttons[2]).IsEnabled = this.SelectList.SelectedIndex != 0;
            ((ApplicationBarMenuItem)this.ApplicationBar.MenuItems[2]).IsEnabled = this.SelectList.SelectedIndex != 0;
            ((ApplicationBarMenuItem)this.ApplicationBar.MenuItems[3]).IsEnabled = this.SelectList.SelectedIndex != 0;
        }

        #endregion
    }
}