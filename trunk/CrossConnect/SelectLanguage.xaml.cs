#region Header

// <copyright file="SelectLanguage.xaml.cs" company="Thomas Dilts">
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
    using System.IO.IsolatedStorage;
    using System.Windows;
    using System.Windows.Controls;

    /// <summary>
    /// The select language.
    /// </summary>
    public partial class SelectLanguage
    {
        #region Fields

        /// <summary>
        /// The supported languages.
        /// </summary>
        private static readonly string[] SupportedLanguages = {
                                                                  "default", "af", "ar", "az", "be", "bg", "cs", "da",
                                                                  "de", "el", "en", "es", "et", "fa", "fi", "fr", "hi",
                                                                  "hr", "hu", "hy", "id", "is", "it", "iw", "ja", "ko",
                                                                  "lt", "lv", "mk", "ms", "mt", "nl", "no", "pl", "pt",
                                                                  "ro", "ru", "sk", "sl", "sq", "sr", "sw", "sv", "th",
                                                                  "tl", "tr", "uk", "ur", "vi", "zh", "zh_cn"
                                                              };

        /// <summary>
        /// The _is in update list.
        /// </summary>
        private bool _isInUpdateList;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SelectLanguage"/> class.
        /// </summary>
        public SelectLanguage()
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
            SelectList.Items.RemoveAt(0);
            SelectList.Items.Insert(0, Translations.Translate("Default system language"));

            _isInUpdateList = true;
            var isoLanguageCode = (string)IsolatedStorageSettings.ApplicationSettings["LanguageIsoCode"];
            int i;
            for (i = 0; i < SupportedLanguages.Length; i++)
            {
                if (SupportedLanguages[i].Equals(isoLanguageCode))
                {
                    SelectList.SelectedIndex = i;
                    break;
                }
            }

            if (i == SupportedLanguages.Length)
            {
                SelectList.SelectedIndex = 0;
            }

            _isInUpdateList = false;
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
            if (!_isInUpdateList)
            {
                Translations.IsoLanguageCode = SupportedLanguages[SelectList.SelectedIndex];
                if (NavigationService.CanGoBack)
                {
                    NavigationService.GoBack();
                }
            }
        }

        #endregion Methods
    }
}