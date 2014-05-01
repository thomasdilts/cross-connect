// <copyright file="MainPageThemes.cs" company="Thomas Dilts">
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
// Email: thomas@cross-connect.se
// </summary>
// <author>Thomas Dilts</author>

namespace CrossConnect
{
    using System;
    using System.Linq;

    using Windows.ApplicationModel.DataTransfer;
    using Windows.UI.Core;
    using Windows.UI.Popups;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml.Media;

    public sealed partial class MainPageSplit
    {
        #region Fields

        private bool _IsAddingToThemesList;

        #endregion

        #region Public Methods and Operators

        public void FillWindowWithValues(Guid guuid)
        {
            Theme theme;
            if (App.Themes.Themes != null && App.Themes.Themes.TryGetValue(guuid, out theme))
            {
                this.ThemeName.Text = Translations.Translate(theme.Name);
                this.BorderColor.ColorPicked = theme.BorderColor;
                this.AccentColor.ColorPicked = theme.AccentColor;
                this.TitleFontColor.ColorPicked = theme.TitleFontColor;
                this.TitleBackgroundColor.ColorPicked = theme.TitleBackColor;
                this.MainBackColor.ColorPicked = theme.MainBackColor;
                this.MainFontColor.ColorPicked = theme.MainFontColor;
                this.WordsOfChristRed.ColorPicked = theme.WordsOfChristRed;
                this.Highlight1.ColorPicked = theme.ColorHighligt[0];
                this.Highlight2.ColorPicked = theme.ColorHighligt[1];
                this.Highlight3.ColorPicked = theme.ColorHighligt[2];
                this.Highlight4.ColorPicked = theme.ColorHighligt[3];
                this.Highlight5.ColorPicked = theme.ColorHighligt[4];
                this.Highlight6.ColorPicked = theme.ColorHighligt[5];
                this.FrameColor.ColorPicked = theme.FrameColor;
                ItemCollection itemCollection = this.FontComboBox.Items;
                if (itemCollection != null)
                {
                    for (int i = 0; i < itemCollection.Count; i++)
                    {
                        if (((TextBlock)itemCollection[i]).Text.Equals(theme.FontFamily))
                        {
                            this.FontComboBox.SelectedIndex = i;
                            break;
                        }
                    }
                }
            }
        }

        #endregion

        #region Methods

        private void Copy_OnClick(object o, RoutedEventArgs e)
        {
            Theme theme = this.GetUpdatedTheme();
            // Set the content to DataPackage as (plain) text format  
            var dataPackage = new DataPackage();
            dataPackage.SetText(Theme.OneThemeToString(theme));
            try
            {
                // Set the DataPackage to clipboard. 
                Clipboard.SetContent(dataPackage);
            }
            catch (Exception ex)
            {
                // Copying data to Clipboard can potentially fail - for example, if another application is holding Clipboard open 
            }
        }

        private Theme GetUpdatedTheme()
        {
            var selectedThemeGuid = (Guid)((TextBlock)this.ThemesComboBox.Items[this.ThemesComboBox.SelectedIndex]).Tag;
            Theme themeOld;
            if (App.Themes.Themes.TryGetValue(selectedThemeGuid, out themeOld))
            {
                var theme = new Theme();
                theme.Clone(themeOld);
                if (!Translations.Translate(App.Themes.Themes[selectedThemeGuid].Name).Equals(this.ThemeName.Text))
                {
                    theme.Name = this.ThemeName.Text;
                }

                theme.BorderColor = this.BorderColor.ColorPicked;
                theme.AccentColor = this.AccentColor.ColorPicked;
                theme.TitleFontColor = this.TitleFontColor.ColorPicked;
                theme.TitleBackColor = this.TitleBackgroundColor.ColorPicked;
                theme.WordsOfChristRed = this.WordsOfChristRed.ColorPicked;
                theme.ColorHighligt[0] = this.Highlight1.ColorPicked;
                theme.ColorHighligt[1] = this.Highlight2.ColorPicked;
                theme.ColorHighligt[2] = this.Highlight3.ColorPicked;
                theme.ColorHighligt[3] = this.Highlight4.ColorPicked;
                theme.ColorHighligt[4] = this.Highlight5.ColorPicked;
                theme.ColorHighligt[5] = this.Highlight6.ColorPicked;
                theme.MainBackColor = this.MainBackColor.ColorPicked;
                theme.MainFontColor = this.MainFontColor.ColorPicked;
                theme.FrameColor = this.FrameColor.ColorPicked;
                theme.FontFamily = ((TextBlock)this.FontComboBox.Items[this.FontComboBox.SelectedIndex]).Text;
                return theme;
            }

            return App.Themes.Themes.First().Value;
        }

        private void MenuThemesClick(object sender, RoutedEventArgs e)
        {
            this.SetupThemePage();
            SideBarShowPopup(
                this.ThemePopup, this.MainPaneThemePopup, this.scrollViewerThemes, this.TopAppBar1, this.BottomAppBar);
            this.ThemePopup.IsOpen = true;
        }

        private async void Paste_OnClick(object sender, RoutedEventArgs e)
        {
            DataPackageView dataPackageView = Clipboard.GetContent();
            if (dataPackageView.Contains(StandardDataFormats.Text))
            {
                try
                {
                    string text = await dataPackageView.GetTextAsync();
                    var theme = new Theme();
                    theme.FromString(text);
                    App.Themes.Themes[theme.UniqId] = theme;
                    App.Themes.CurrentTheme = theme.UniqId;
                    this.FillWindowWithValues(App.Themes.CurrentTheme);
                }
                catch (Exception ex)
                {
                }
            }
        }

        private void SetupThemePage()
        {
            this._IsAddingToThemesList = true;
            this.ThemeTitle.Text = Translations.Translate("Themes");
            this.SelectThemeText.Text = Translations.Translate("Select a theme");
            this.ThemeNameText.Text = Translations.Translate("Name");
            this.BorderColor.TextCaption = Translations.Translate("Border color");
            this.AccentColor.TextCaption = Translations.Translate("Accent color");
            this.TitleFontColor.TextCaption = Translations.Translate("Title font color");
            this.TitleBackgroundColor.TextCaption = Translations.Translate("Title background color");
            this.MainBackColor.TextCaption = Translations.Translate("Main background color");
            this.MainFontColor.TextCaption = Translations.Translate("Main font color");
            this.WordsOfChristRed.TextCaption = Translations.Translate("Show the words of Jesus in red");
            this.Highlight1.TextCaption = Translations.Translate("Highlight color") + " 1 => " + App.DisplaySettings.HighlightName1;
            this.Highlight2.TextCaption = Translations.Translate("Highlight color") + " 2 => " + App.DisplaySettings.HighlightName2;
            this.Highlight3.TextCaption = Translations.Translate("Highlight color") + " 3 => " + App.DisplaySettings.HighlightName3;
            this.Highlight4.TextCaption = Translations.Translate("Highlight color") + " 4 => " + App.DisplaySettings.HighlightName4;
            this.Highlight5.TextCaption = Translations.Translate("Highlight color") + " 5 => " + App.DisplaySettings.HighlightName5;
            this.Highlight6.TextCaption = Translations.Translate("Highlight color") + " 6 => " + App.DisplaySettings.HighlightName6;
            this.ThemeFont.Text = Translations.Translate("Font");
            this.FrameColor.TextCaption = Translations.Translate("Frame color");
            this.ThemeSave.Content = Translations.Translate("Save as new theme");
            this.ThemeCopy.Content = Translations.Translate("Theme Copy");
            this.ThemePaste.Content = Translations.Translate("Theme Paste");
            this.ThemeReset.Content = Translations.Translate("Factory reset themes");

            ItemCollection itemCollection = this.FontComboBox.Items;
            if (itemCollection != null)
            {
                itemCollection.Clear();
                foreach (var font in Theme.FontFamilies)
                {
                    itemCollection.Add(
                        new TextBlock
                            {
                                FontSize = 28,
                                Text = font.Key,
                                FontFamily = new FontFamily(font.Value),
                                Tag = font.Value
                            });
                }
            }

            ItemCollection themeCollection = this.ThemesComboBox.Items;
            if (themeCollection != null)
            {
                themeCollection.Clear();
                int i = 0;
                foreach (var tema in App.Themes.Themes)
                {
                    themeCollection.Add(
                        new TextBlock { FontSize = 20, Text = Translations.Translate(tema.Value.Name), Tag = tema.Key });
                    if (tema.Value.UniqId.Equals(App.Themes.CurrentTheme))
                    {
                        this.ThemesComboBox.SelectedIndex = i;
                    }
                    i++;
                }
            }

            if (App.Themes.Themes.Any())
            {
                this.FillWindowWithValues(App.Themes.CurrentTheme);
            }
            this._IsAddingToThemesList = false;
        }

        private void ThemePopup_OnClosed(object sender, object e)
        {
            if (this.ThemesComboBox.SelectedIndex >= 0)
            {
                var selectedThemeGuid = (Guid)((TextBlock)this.ThemesComboBox.Items[this.ThemesComboBox.SelectedIndex]).Tag;
                Theme theme = this.GetUpdatedTheme();
                App.Themes.Themes[selectedThemeGuid] = theme;
                App.Themes.CurrentTheme = selectedThemeGuid;
                App.ShowUserInterface(true);
                this.RecolorScreen();
                App.SavePersistantThemes();
            }
        }

        private void ThemePopup_OnOpened(object sender, object e)
        {
            App.ShowUserInterface(false);
        }

        private async void ThemeReset_OnClick(object sender, RoutedEventArgs e)
        {
            var dialog =
                new MessageDialog(
                    Translations.Translate("Factory reset themes") + "\n\n" + Translations.Translate("Are you sure?"));
            dialog.Commands.Add(
                new UICommand(
                    Translations.Translate("Yes"),
                    (UICommandInvokedHandler) =>
                        {
                            App.Themes.InitializeFromResources();
                            App.ShowUserInterface(true);
                            this.RedrawMainScreen(true);
                        }));
            await dialog.ShowAsync();
        }

        private void ThemeSave_OnClick(object sender, RoutedEventArgs e)
        {
            var selectedThemeGuid = (Guid)((TextBlock)this.ThemesComboBox.Items[this.ThemesComboBox.SelectedIndex]).Tag;
            if (!Translations.Translate(App.Themes.Themes[selectedThemeGuid].Name).Equals(this.ThemeName.Text))
            {
                Theme theme = this.GetUpdatedTheme();
                theme.UniqId = Guid.NewGuid();
                App.Themes.Themes[theme.UniqId] = theme;
                ItemCollection themeCollection = this.ThemesComboBox.Items;
                themeCollection.Add(new TextBlock { FontSize = 20, Text = theme.Name, Tag = theme.UniqId });
                this.ThemesComboBox.SelectedIndex = themeCollection.Count - 1;
            }
        }

        private void ThemesComboBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.ThemesComboBox.SelectedIndex >= 0 && !this._IsAddingToThemesList)
            {
                this.FillWindowWithValues(
                    (Guid)((TextBlock)this.ThemesComboBox.Items[this.ThemesComboBox.SelectedIndex]).Tag);
                var selectedThemeGuid =
                    (Guid)((TextBlock)this.ThemesComboBox.Items[this.ThemesComboBox.SelectedIndex]).Tag;
                Theme theme = this.GetUpdatedTheme();
                App.Themes.Themes[selectedThemeGuid] = theme;
                App.Themes.CurrentTheme = selectedThemeGuid;
                App.ShowUserInterface(true);
                this.RecolorScreen();
                this.Dispatcher.RunAsync(CoreDispatcherPriority.Low, () => App.ShowUserInterface(false));
            }
        }

        #endregion
    }
}