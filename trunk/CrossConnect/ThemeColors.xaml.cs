#region Header

// <copyright file="ThemeColors.xaml.cs" company="Thomas Dilts">
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
    using System.Diagnostics;
    using System.IO;
    using System.IO.IsolatedStorage;
    using System.Text;
    using System.Windows;
    using System.Windows.Media.Imaging;

    using Microsoft.Phone.Shell;
    using Microsoft.Phone.Tasks;

    using Sword.reader;

    public partial class ThemeColors
    {
        #region Fields

        readonly PhotoChooserTask _photoChooserTask;

        private string _fontFamily;
        private string _mainBackImage;

        #endregion Fields

        #region Constructors

        public ThemeColors()
        {
            InitializeComponent();
            _photoChooserTask = new PhotoChooserTask();
            _photoChooserTask.Completed += PhotoSelectionCompleted;
        }

        #endregion Constructors

        #region Methods

        private void AutoRotatePageBackKeyPress(object sender, System.ComponentModel.CancelEventArgs e)
        {
            object unitId;
            if (PhoneApplicationService.Current.State.TryGetValue("ThemeColorsThemeToChange", out unitId))
            {
                Theme theme;
                if (App.Themes.Themes.TryGetValue((Guid)unitId, out theme))
                {
                    theme.Name = ThemeName.Text;
                    if (string.IsNullOrEmpty(theme.Name))
                        theme.Name = Translations.Translate("New")  + " " + (int)(new Random().NextDouble() * 100);
                    theme.BorderColor = BorderColor.Color;
                    theme.AccentColor = AccentColor.Color;
                    theme.TitleFontColor = TitleFontColor.Color;
                    theme.TitleBackColor = TitleBackgroundColor.Color;
                    if (IsBackgroundImage.IsChecked != null) theme.IsMainBackImage = (bool)IsBackgroundImage.IsChecked;
                    theme.MainBackColor = MainBackColor.Color;
                    theme.MainFontColor = MainFontColor.Color;
                    if (IsButtonDark.IsChecked != null) theme.IsButtonColorDark = (bool)IsButtonDark.IsChecked;
                    theme.MainBackImage = _mainBackImage;
                    theme.FontFamily = _fontFamily;
                    if (App.Themes.CurrentTheme.Equals(theme.UniqId))
                        App.Themes.Clone(theme);
                }
            }
        }

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

            ((ApplicationBarIconButton)ApplicationBar.Buttons[0]).Text = Translations.Translate("Share this theme");
            ((ApplicationBarMenuItem)ApplicationBar.MenuItems[0]).Text = Translations.Translate("Share this theme");

            object unitId;
            if(PhoneApplicationService.Current.State.TryGetValue("ThemeColorsThemeToChange", out unitId))
            {
                Theme theme;
                if (App.Themes.Themes.TryGetValue((Guid)unitId, out theme))
                {
                    ThemeName.Text = theme.Name;
                    BorderColor.Color = theme.BorderColor;
                    AccentColor.Color = theme.AccentColor;
                    TitleFontColor.Color = theme.TitleFontColor;
                    TitleBackgroundColor.Color = theme.TitleBackColor;
                    IsBackgroundImage.IsChecked = theme.IsMainBackImage;
                    MainBackColor.Color = theme.MainBackColor;
                    MainFontColor.Color = theme.MainFontColor;
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
                _mainBackImage = (string) selectedImage;
                LoadImageToScreen(_mainBackImage);
                PhoneApplicationService.Current.State.Remove("ImageBrowserSelected");
            }
        }

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

        private void ButGetFromDownloadedClick(object sender, RoutedEventArgs e)
        {
            PhoneApplicationService.Current.State.Remove("ImageBrowserSelected");
            NavigationService.Navigate(new Uri("/ImageBrowse.xaml", UriKind.Relative));
        }

        private void ButShareMyThemeClick(object sender, EventArgs e)
        {
            MenuShareMyThemeClick(sender, e);
        }

        private MemoryStream GetImageStream(string imageName)
        {
            if (!string.IsNullOrEmpty(imageName))
            {
                //read from isolated storage.
                using (var isolatedStorageRoot = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    if (isolatedStorageRoot.FileExists(App.WebDirIsolated + "/images/" + imageName))
                    {
                        try
                        {
                            using (IsolatedStorageFileStream fStream =
                                isolatedStorageRoot.OpenFile(App.WebDirIsolated + "/images/" + imageName, FileMode.Open))
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

        private void LoadImageToScreen(string imageName)
        {
            var ms = GetImageStream(imageName);
            if(ms!=null)
            {
                var bitImage = new BitmapImage();
                bitImage.SetSource(ms);
                MainBackImage.Source = bitImage;
            }
        }

        private void MenuShareMyThemeClick(object sender, EventArgs e)
        {
            var body =new StringBuilder(
                Translations.Translate("Please share this theme with others from your website") + "\n\n");
            body.Append(Translations.Translate("Please attach an image to this email if used in the theme") + "\n\n");
            object unitId;
            if (PhoneApplicationService.Current.State.TryGetValue("ThemeColorsThemeToChange", out unitId))
            {
                Theme theme;
                if (App.Themes.Themes.TryGetValue((Guid) unitId, out theme))
                {
                    body.Append(Translations.Translate("The definition of the theme") + "\n\n");
                    body.Append(App.Themes.OneThemeToString(theme));
                    //cant add an image. it is too big.
                    //if(IsBackgroundImage.IsChecked!=null && (bool)IsBackgroundImage.IsChecked && !string.IsNullOrEmpty(_mainBackImage))
                    //{
                    //    var ms = GetImageStream(_mainBackImage);
                    //    if (ms != null)
                    //    {
                    //        body.Append("\n\n" + "The definition of the image" + "\n\n");
                    //        var buff=new byte[ms.Length];
                    //        ms.Read(buff,0,buff.Length);
                    //        body.Append(Convert.ToBase64String(buff));
                    //    }
                    //}
                }
            }
            var emailComposeTask = new EmailComposeTask { Body = body.ToString(), Subject = Translations.Translate("Share this theme"),To = "crossconnect@chaniel.se"};
            emailComposeTask.Show();
        }

        void PhotoSelectionCompleted(object sender, PhotoResult e)
        {
            if (e.TaskResult == TaskResult.OK)
            {
                try
                {
                    var bitImage = new BitmapImage();
                    bitImage.SetSource(e.ChosenPhoto);

                    //image is a Image Control in the form
                    MainBackImage.Source = bitImage;
                    e.ChosenPhoto.Position = 0;
                    string name = Path.GetFileName(e.OriginalFileName);
                    using (var isolatedStorageRoot = IsolatedStorageFile.GetUserStoreForApplication())
                    {
                        if (!isolatedStorageRoot.DirectoryExists(App.WebDirIsolated + "/images"))
                            isolatedStorageRoot.CreateDirectory(App.WebDirIsolated + "/images");
                        if (isolatedStorageRoot.DirectoryExists(App.WebDirIsolated + "/images/" + name))
                            isolatedStorageRoot.DeleteFile(App.WebDirIsolated + "/images/" + name);
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
                catch(Exception eee)
                {
                    Debug.WriteLine("A returned error occurred." + eee);
                }
            }
            else
            {
                Debug.WriteLine("A returned error occurred." + e.Error.Message);
            }
        }

        private void SetFontWindow(string fontFamily)
        {
            if (!Theme.FontFamilies.ContainsKey(fontFamily))
                fontFamily = "Segoe WP";

            string body = "<p><a style=\"" + Theme.FontFamilies[fontFamily] + "color:" +
                  BrowserTitledWindow.GetBrowserColor("PhoneForegroundColor") +
                  ";decoration:none\" href=\"#\" onclick=\"window.external.Notify('" +
                  fontFamily + "'); event.returnValue=false; return false;\" >" + fontFamily +
                  "</a></p>";
            webBrowser1.NavigateToString(BibleZtextReader.HtmlHeader(
                App.DisplaySettings,
                BrowserTitledWindow.GetBrowserColor("PhoneBackgroundColor"),
                BrowserTitledWindow.GetBrowserColor("PhoneForegroundColor"),
                BrowserTitledWindow.GetBrowserColor("PhoneAccentColor"),
                20, "") + body + "</body></html>");
        }

        private void WebBrowser1ScriptNotify(object sender, Microsoft.Phone.Controls.NotifyEventArgs e)
        {
            AutoRotatePageBackKeyPress(null, null);
            PhoneApplicationService.Current.State["WebFontSelectWindowSelection"] = _fontFamily;
            NavigationService.Navigate(new Uri("/WebFontSelect.xaml", UriKind.Relative));
        }

        #endregion Methods
    }
}