#region Header

// <copyright file="ThemeColors.xaml.cs" company="Thomas Dilts">
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
    using System.Diagnostics;
    using System.IO;
    using System.IO.IsolatedStorage;
    using System.Text;
    using System.Windows;
    using System.Windows.Media.Imaging;

    using Microsoft.Phone.Controls;
    using Microsoft.Phone.Shell;
    using Microsoft.Phone.Tasks;

    using Sword.reader;

    /// <summary>
    /// The theme colors.
    /// </summary>
    public partial class ThemeColors
    {
        #region Fields

        /// <summary>
        /// The _photo chooser task.
        /// </summary>
        private readonly PhotoChooserTask _photoChooserTask;

        /// <summary>
        /// The _font family.
        /// </summary>
        private string _fontFamily;

        /// <summary>
        /// The _main back image.
        /// </summary>
        private string _mainBackImage;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ThemeColors"/> class.
        /// </summary>
        public ThemeColors()
        {
            InitializeComponent();
            _photoChooserTask = new PhotoChooserTask();
            _photoChooserTask.Completed += PhotoSelectionCompleted;
        }

        #endregion Constructors

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
            object unitId;
            if (PhoneApplicationService.Current.State.TryGetValue("ThemeColorsThemeToChange", out unitId))
            {
                Theme theme;
                if (App.Themes.Themes.TryGetValue((Guid)unitId, out theme))
                {
                    theme.Name = ThemeName.Text;
                    if (string.IsNullOrEmpty(theme.Name))
                    {
                        theme.Name = Translations.Translate("New") + " " + (int)(new Random().NextDouble() * 100);
                    }

                    if (IsBackgroundImage.IsChecked != null)
                    {
                        theme.IsMainBackImage = (bool)IsBackgroundImage.IsChecked;
                    }

                    if (IsButtonDark.IsChecked != null)
                    {
                        theme.IsButtonColorDark = (bool)IsButtonDark.IsChecked;
                    }

                    theme.MainBackImage = _mainBackImage;
                    theme.FontFamily = _fontFamily;


                    theme.BorderColor = this.BorderColor.Color;
                    theme.AccentColor = this.AccentColor.Color;
                    theme.TitleFontColor = this.TitleFontColor.Color;
                    theme.TitleBackColor = this.TitleBackgroundColor.Color;
                    theme.WordsOfChristRed = this.WordsOfChristRed.Color;
                    theme.ColorHighligt[0] = this.Highlight1.Color;
                    theme.ColorHighligt[1] = this.Highlight2.Color;
                    theme.ColorHighligt[2] = this.Highlight3.Color;
                    theme.ColorHighligt[3] = this.Highlight4.Color;
                    theme.ColorHighligt[4] = this.Highlight5.Color;
                    theme.ColorHighligt[5] = this.Highlight6.Color;
                    theme.MainBackColor = this.MainBackColor.Color;
                    theme.MainFontColor = this.MainFontColor.Color;

                    if (App.Themes.CurrentTheme.Equals(theme.UniqId))
                    {
                        App.Themes.Clone(theme);
                    }
                }
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
            ThemeNameText.Text = Translations.Translate("Name");
            BorderColorText.Text = Translations.Translate("Border color");
            AccentColorText.Text = Translations.Translate("Accent color");
            TitleFontColorText.Text = Translations.Translate("Title font color");
            TitleBackgroundColorText.Text = Translations.Translate("Title background color");
            IsBackgroundImage.Header = Translations.Translate("Use an image for the background");
            MainBackColorText.Text = Translations.Translate("Main background color");
            MainBackPicture.Text = Translations.Translate("Main background image");
            MainFontColorText.Text = Translations.Translate("Main font color");
            ThemeFont.Text = Translations.Translate("Font");
            IsButtonDark.Header = Translations.Translate("Dark buttons");
            ButGetFromCamera.Content = Translations.Translate("From the camera");
            ButGetFromDownloaded.Content = Translations.Translate("From the downloads");
            this.Highlight1Text.Text = Translations.Translate("Highlight color") + " 1 => " + App.DisplaySettings.HighlightName1;
            this.Highlight2Text.Text = Translations.Translate("Highlight color") + " 2 => " + App.DisplaySettings.HighlightName2;
            this.Highlight3Text.Text = Translations.Translate("Highlight color") + " 3 => " + App.DisplaySettings.HighlightName3;
            this.Highlight4Text.Text = Translations.Translate("Highlight color") + " 4 => " + App.DisplaySettings.HighlightName4;
            this.Highlight5Text.Text = Translations.Translate("Highlight color") + " 5 => " + App.DisplaySettings.HighlightName5;
            this.Highlight6Text.Text = Translations.Translate("Highlight color") + " 6 => " + App.DisplaySettings.HighlightName6;

            ((ApplicationBarIconButton)ApplicationBar.Buttons[0]).Text = Translations.Translate("Share this theme");
            ((ApplicationBarMenuItem)ApplicationBar.MenuItems[0]).Text = Translations.Translate("Share this theme");

            object unitId;
            if (PhoneApplicationService.Current.State.TryGetValue("ThemeColorsThemeToChange", out unitId))
            {
                Theme theme;
                if (App.Themes.Themes.TryGetValue((Guid)unitId, out theme))
                {
                    this.BorderColor.Color = theme.BorderColor;
                    this.AccentColor.Color = theme.AccentColor;
                    this.TitleFontColor.Color = theme.TitleFontColor;
                    this.TitleBackgroundColor.Color = theme.TitleBackColor;
                    this.MainBackColor.Color = theme.MainBackColor;
                    this.MainFontColor.Color = theme.MainFontColor;
                    this.WordsOfChristRed.Color = theme.WordsOfChristRed;
                    this.Highlight1.Color = theme.ColorHighligt[0];
                    this.Highlight2.Color = theme.ColorHighligt[1];
                    this.Highlight3.Color = theme.ColorHighligt[2];
                    this.Highlight4.Color = theme.ColorHighligt[3];
                    this.Highlight5.Color = theme.ColorHighligt[4];
                    this.Highlight6.Color = theme.ColorHighligt[5];

                    ThemeName.Text = theme.Name;
                    IsBackgroundImage.IsChecked = theme.IsMainBackImage;
                    IsButtonDark.IsChecked = theme.IsButtonColorDark;
                    _mainBackImage = theme.MainBackImage;
                    _fontFamily = theme.FontFamily;
                    LoadImageToScreen(theme.MainBackImage);
                    SetFontWindow(theme.FontFamily);
                }
            }

            object changedFont;
            if (PhoneApplicationService.Current.State.TryGetValue("WebFontSelectWindowSelection", out changedFont))
            {
                SetFontWindow((string)changedFont);
                _fontFamily = (string)changedFont;
            }

            object selectedImage;
            if (PhoneApplicationService.Current.State.TryGetValue("ImageBrowserSelected", out selectedImage))
            {
                _mainBackImage = (string)selectedImage;
                LoadImageToScreen(_mainBackImage);
                PhoneApplicationService.Current.State.Remove("ImageBrowserSelected");
            }
        }

        /// <summary>
        /// The but get from camera click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void ButGetFromCameraClick(object sender, RoutedEventArgs e)
        {
            try
            {
                _photoChooserTask.Show();
            }
            catch (InvalidOperationException ex)
            {
                MessageBox.Show("An error occurred." + ex);
            }
        }

        /// <summary>
        /// The but get from downloaded click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void ButGetFromDownloadedClick(object sender, RoutedEventArgs e)
        {
            PhoneApplicationService.Current.State.Remove("ImageBrowserSelected");
            NavigationService.Navigate(new Uri("/ImageBrowse.xaml", UriKind.Relative));
        }

        /// <summary>
        /// The but share my theme click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void ButShareMyThemeClick(object sender, EventArgs e)
        {
            MenuShareMyThemeClick(sender, e);
        }

        /// <summary>
        /// The get image stream.
        /// </summary>
        /// <param name="imageName">
        /// The image name.
        /// </param>
        /// <returns>
        /// </returns>
        private MemoryStream GetImageStream(string imageName)
        {
            if (!string.IsNullOrEmpty(imageName))
            {
                // read from isolated storage.
                using (IsolatedStorageFile isolatedStorageRoot = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    if (isolatedStorageRoot.FileExists(App.WebDirIsolated + "/images/" + imageName))
                    {
                        try
                        {
                            using (
                                IsolatedStorageFileStream fStream =
                                    isolatedStorageRoot.OpenFile(
                                        App.WebDirIsolated + "/images/" + imageName, FileMode.Open))
                            {
                                var buffer = new byte[10000];
                                int len;
                                var ms = new MemoryStream();
                                while ((len = fStream.Read(buffer, 0, buffer.GetUpperBound(0))) > 0)
                                {
                                    ms.Write(buffer, 0, len);
                                }

                                fStream.Close();
                                ms.Position = 0;
                                return ms;
                            }
                        }
                        catch (Exception ee)
                        {
                            Debug.WriteLine(ee);
                        }
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// The load image to screen.
        /// </summary>
        /// <param name="imageName">
        /// The image name.
        /// </param>
        private void LoadImageToScreen(string imageName)
        {
            MemoryStream ms = GetImageStream(imageName);
            if (ms != null)
            {
                var bitImage = new BitmapImage();
                bitImage.SetSource(ms);
                MainBackImage.Source = bitImage;
            }
        }

        /// <summary>
        /// The menu share my theme click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void MenuShareMyThemeClick(object sender, EventArgs e)
        {
            var body =
                new StringBuilder(
                    Translations.Translate("Please share this theme with others from your website") + "\n\n");
            body.Append(Translations.Translate("Please attach an image to this email if used in the theme") + "\n\n");
            object unitId;
            if (PhoneApplicationService.Current.State.TryGetValue("ThemeColorsThemeToChange", out unitId))
            {
                Theme theme;
                if (App.Themes.Themes.TryGetValue((Guid)unitId, out theme))
                {
                    body.Append(Translations.Translate("The definition of the theme") + "\n\n");
                    body.Append(Theme.OneThemeToString(theme));

                    // cant add an image. it is too big.
                    // if(IsBackgroundImage.IsChecked!=null && (bool)IsBackgroundImage.IsChecked && !string.IsNullOrEmpty(_mainBackImage))
                    // {
                    // var ms = GetImageStream(_mainBackImage);
                    // if (ms != null)
                    // {
                    // body.Append("\n\n" + "The definition of the image" + "\n\n");
                    // var buff=new byte[ms.Length];
                    // ms.Read(buff,0,buff.Length);
                    // body.Append(Convert.ToBase64String(buff));
                    // }
                    // }
                }
            }

            var emailComposeTask = new EmailComposeTask
                {
                    Body = body.ToString(),
                    Subject = Translations.Translate("Share this theme"),
                    To = "crossconnect@cross-connect.se"
                };
            emailComposeTask.Show();
        }

        /// <summary>
        /// The photo selection completed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void PhotoSelectionCompleted(object sender, PhotoResult e)
        {
            if (e.TaskResult == TaskResult.OK)
            {
                try
                {
                    var bitImage = new BitmapImage();
                    bitImage.SetSource(e.ChosenPhoto);

                    // image is a Image Control in the form
                    MainBackImage.Source = bitImage;
                    e.ChosenPhoto.Position = 0;
                    string name = Path.GetFileName(e.OriginalFileName);
                    using (IsolatedStorageFile isolatedStorageRoot = IsolatedStorageFile.GetUserStoreForApplication())
                    {
                        if (!isolatedStorageRoot.DirectoryExists(App.WebDirIsolated + "/images"))
                        {
                            isolatedStorageRoot.CreateDirectory(App.WebDirIsolated + "/images");
                        }

                        if (isolatedStorageRoot.DirectoryExists(App.WebDirIsolated + "/images/" + name))
                        {
                            isolatedStorageRoot.DeleteFile(App.WebDirIsolated + "/images/" + name);
                        }

                        IsolatedStorageFileStream fStream =
                            isolatedStorageRoot.CreateFile(App.WebDirIsolated + "/images/" + name);
                        var buffer = new byte[10000];
                        int len;
                        while ((len = e.ChosenPhoto.Read(buffer, 0, buffer.GetUpperBound(0))) > 0)
                        {
                            fStream.Write(buffer, 0, len);
                        }

                        fStream.Close();
                    }

                    _mainBackImage = name;
                }
                catch (Exception eee)
                {
                    Debug.WriteLine("A returned error occurred." + eee);
                }
            }
            else
            {
                Debug.WriteLine("A returned error occurred." + e.Error.Message);
            }
        }

        /// <summary>
        /// The set font window.
        /// </summary>
        /// <param name="fontFamily">
        /// The font family.
        /// </param>
        private void SetFontWindow(string fontFamily)
        {
            if (!Theme.FontFamilies.ContainsKey(fontFamily))
            {
                fontFamily = "Segoe WP";
            }

            string body = "<p><a style=\"" + Theme.FontFamilies[fontFamily] + "color:"
                          + BrowserTitledWindow.GetBrowserColor("PhoneForegroundColor")
                          + ";decoration:none\" href=\"#\" onclick=\"window.external.Notify('" + fontFamily
                          + "'); event.returnValue=false; return false;\" >" + fontFamily + "</a></p>";
            webBrowser1.NavigateToString(
                BibleZtextReader.HtmlHeader(
                    App.DisplaySettings,
                    BrowserTitledWindow.GetBrowserColor("PhoneBackgroundColor"),
                    BrowserTitledWindow.GetBrowserColor("PhoneForegroundColor"),
                    BrowserTitledWindow.GetBrowserColor("PhoneAccentColor"),
                    BrowserTitledWindow.GetBrowserColor("PhoneWordsOfChristColor"),
                    20,
                    string.Empty,
                    "en") + body + "</body></html>");
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
            AutoRotatePageBackKeyPress(null, null);
            PhoneApplicationService.Current.State["WebFontSelectWindowSelection"] = _fontFamily;
            NavigationService.Navigate(new Uri("/WebFontSelect.xaml", UriKind.Relative));
        }

        #endregion Methods
    }
}