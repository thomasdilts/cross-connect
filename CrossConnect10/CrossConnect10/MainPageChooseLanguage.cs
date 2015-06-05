// <copyright file="MainPageChooseLanguage.cs" company="Thomas Dilts">
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
    using Sword.reader;
    using Windows.Storage;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;

    public sealed partial class MainPageSplit
    {
        #region Static Fields

        private static readonly string[] SupportedLanguages =
            {
                "default", "af", "ar", "az", "be", "bg", "bn", "cs", "cy", "da",
                "de", "el", "en", "eo", "es", "et", "eu", "fa", "fi", "fr", "ga", "gl", "gu", "he", "hi",
                "hr", "ht", "hu", "hy", "id", "is", "it", "ja", "ka", "km", "kn", "ko",
                "lt", "lv", "mi", "mk", "mn", "mr", "ms", "mt", "nl", "no", "pa", "pl", "pt",
                "ro", "ru", "sk", "sl", "so", "sq", "sr", "sw", "sv", "ta", "te", "th",
                "tl", "tr", "uk", "ur", "vi", "zh", "zh_cn", "zu"
            };

        #endregion

        #region Fields

        private bool _isInUpdateList;

        #endregion

        #region Methods

        private void ChooseLanguagePopup_OnClosed(object sender, object e)
        {
            App.ShowUserInterface(true);
        }

        private void ChooseLanguagePopup_OnOpened(object sender, object e)
        {
            App.ShowUserInterface(false);
        }

        private void MenuLanguageClick(object sender, RoutedEventArgs e)
        {
            this._isInUpdateList = true;

            this.SelectLanguageTitle.Text = Translations.Translate("Select the language");
            this.SelectList.Items.RemoveAt(0);
            this.SelectList.Items.Insert(0, Translations.Translate("Default system language"));

            var isoLanguageCode = (string)ApplicationData.Current.LocalSettings.Values["LanguageIsoCode"];
            int i;
            for (i = 0; i < SupportedLanguages.Length; i++)
            {
                //SelectList.Items.Add(new ListBoxItem{Content = ""});
                if (SupportedLanguages[i].Equals(isoLanguageCode))
                {
                    this.SelectList.SelectedIndex = i;
                    break;
                }
            }

            if (i == SupportedLanguages.Length)
            {
                this.SelectList.SelectedIndex = 0;
            }
            SideBarShowPopup(
                this.ChooseLanguagePopup,
                this.MainPaneChooseLanguagePopup,
                this.scrollViewerLanguages,
                this.TopAppBar1,
                this.BottomAppBar);

            this._isInUpdateList = false;
        }

        private void SelectListSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!this._isInUpdateList && this.SelectList.SelectedIndex >= 0)
            {
                Translations.IsoLanguageCode = SupportedLanguages[this.SelectList.SelectedIndex];
                ApplicationData.Current.LocalSettings.Values["LanguageIsoCode"] =
                    SupportedLanguages[this.SelectList.SelectedIndex];
                foreach (var window in App.OpenWindows)
                {
                    if (window.State.Source is BibleZtextReader)
                    {
                        ((BibleZtextReader)window.State.Source).BookNames(Translations.IsoLanguageCode, true);
                    }
                }

                App.MainWindow.ReDrawWindows();
                this.ChooseLanguagePopup.IsOpen = false;
                this.SetLanguageDependentTexts();
            }
        }

        #endregion
    }
}