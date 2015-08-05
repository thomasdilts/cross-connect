// <copyright file="BrowserMenu.cs" company="Thomas Dilts">
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

    using CrossConnect.readers;

    using Windows.Storage;
    using Windows.UI.Core;
    using Windows.UI.Popups;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml.Input;
    using Sword.versification;
    using Windows.Media.SpeechSynthesis;

    public sealed partial class BrowserTitledWindow
    {
        #region Methods

        private async void AddANote_Click_1(object sender, RoutedEventArgs e)
        {
            if (App.PlaceMarkers.History.Count > 0)
            {
                ApplicationData.Current.LocalSettings.Values.Remove("NoteToAddSaved");
                //Frame.Navigate(typeof(AddNote), this);
            }
            else
            {
                var dialog = new MessageDialog(Translations.Translate("You must first select a verse"));
                await dialog.ShowAsync();
            }
        }
        private bool ExistsBibleBookInKjv()
        {
            string bookShortName;
            int relChaptNum;
            int verseNum;
            string fullName;
            string titleText;
            this._state.Source.GetInfo(Translations.IsoLanguageCode, 
                out bookShortName, out relChaptNum, out verseNum, out fullName, out titleText);
            var canonKjv = CanonManager.GetCanon("KJV");
            CanonBookDef book;

            if (string.IsNullOrEmpty(bookShortName) || !canonKjv.BookByShortName.TryGetValue(bookShortName, out book))
            {
                return false;
            }
            return true;
        }

        private void ButMenuClick(object sender, RoutedEventArgs e)
        {
            if (!this.MenuPopup.IsOpen)
            {
                this.MenuPopup.IsOpen = true;
            }
            this.MenuPopup.UpdateLayout();
            this.SubMenuStackPanel.UpdateLayout();

            // Which items are to be in the menu.

            bool isHearable = this._state != null && this._state.Source != null && this._state.Source.IsHearable;
            bool isTTChearable = this._state != null && this._state.Source != null && this._state.Source.IsTTChearable;
            int numberOfWindowsInMyColumn = App.OpenWindows.Count(win => win.State.Window == this._state.Window);
            this.EnterKey.Visibility = this.State.Source.IsLocked ? Visibility.Visible : Visibility.Collapsed;
            this.MoveWindowRight.Visibility = (this.State.Window < 8) ? Visibility.Visible : Visibility.Collapsed;
            this.MoveWindowLeft.Visibility = (this.State.Window > 0) ? Visibility.Visible : Visibility.Collapsed;
            this.WindowSmaller.Visibility = (numberOfWindowsInMyColumn > 1 && this.State.NumRowsIown > 1)
                                                ? Visibility.Visible
                                                : Visibility.Collapsed;
            this.WindowLarger.Visibility = (numberOfWindowsInMyColumn > 1 && this.State.NumRowsIown < 10)
                                               ? Visibility.Visible
                                               : Visibility.Collapsed;
            this.AddANote.Visibility = (this._state.WindowType == WindowType.WindowBible
                                        || this._state.WindowType == WindowType.WindowDailyPlan)
                                           ? Visibility.Visible
                                           : Visibility.Collapsed;
            this.AddToBookMarks.Visibility = this.AddANote.Visibility;
            this.Hear.Visibility = this._state != null && this._state.Source != null && this._state.Source.IsHearable && ExistsBibleBookInKjv()
                                       ? Visibility.Visible
                                       : Visibility.Collapsed;
            this.TTS.Visibility = this._state != null && this._state.Source != null && this._state.Source.IsTTChearable
                                       ? Visibility.Visible
                                       : Visibility.Collapsed;
            this.Translate.Visibility = (this._state.WindowType == WindowType.WindowBible
                                        || this._state.WindowType == WindowType.WindowDailyPlan
                                        || this._state.WindowType == WindowType.WindowLexiconLink)
                                           ? Visibility.Visible
                                           : Visibility.Collapsed;
            this.Copy.Visibility = this.AddANote.Visibility;
            this.Highlight.Visibility = this.AddANote.Visibility;
            this.SendMail.Visibility = this.AddANote.Visibility;
            this.FontSmaller.Visibility = (this.State.HtmlFontSize > 4) ? Visibility.Visible : Visibility.Collapsed;
            this.FontLarger.Visibility = (this.State.HtmlFontSize < 65) ? Visibility.Visible : Visibility.Collapsed;

            this.MoveWindowRight.Content = Translations.Translate("Enter key");
            this.MoveWindowRight.Content = Translations.Translate("Move this window to the right");
            this.MoveWindowLeft.Content = Translations.Translate("Move this window to the left");
            this.WindowSmaller.Content = Translations.Translate("Make this window smaller");
            this.WindowLarger.Content = Translations.Translate("Make this window larger");
            this.AddANote.Content = Translations.Translate("Add a note to a verse");
            this.AddToBookMarks.Content = Translations.Translate("Bookmark this verse");
            this.Hear.Content = Translations.Translate("Listen to this chapter");
            this.TTS.Content = "(TTS) " + Translations.Translate("Listen to this chapter");
            this.Translate.Content = Translations.Translate("Translate to the current language");
            this.Copy.Content = Translations.Translate("Copy the last selected verses");
            this.Highlight.Content = Translations.Translate("Highlight");
            this.SendMail.Content = Translations.Translate("Share the last selected verses");
            this.FontSmaller.Content = Translations.Translate("Make the text size smaller");
            this.FontLarger.Content = Translations.Translate("Make the text size larger");

            MakeSurePopupIsOnScreen(
                this.SubMenuStackPanel.ActualHeight,
                this.SubMenuStackPanel.ActualWidth,
                this.ButMenu,
                this,
                this.MenuPopup);
        }

        private async void SubMenuMenuPopup_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var itemSelected = (ListBoxItem)e.AddedItems.FirstOrDefault();
            if (itemSelected == null)
            {
                return;
            }
            switch (itemSelected.Name)
            {
                case "MoveWindowRight":
                    this.State.Window += 1;
                    if (this.State.Window > 8)
                    {
                        this.State.Window = 8;
                    }
                    this.ShowUserInterface(true);
                    App.MainWindow.ReDrawWindows(false);
                    break;
                case "MoveWindowLeft":
                    this.State.Window -= 1;
                    if (this.State.Window < 0)
                    {
                        this.State.Window = 0;
                    }
                    this.ShowUserInterface(true);
                    App.MainWindow.ReDrawWindows(false);
                    break;
                case "WindowSmaller":
                    this.State.NumRowsIown -= 1;
                    if (this.State.NumRowsIown < 1)
                    {
                        this.State.NumRowsIown = 1;
                    }
                    this.ShowUserInterface(true);
                    App.MainWindow.ReDrawWindows(false);
                    break;
                case "WindowLarger":
                    this.State.NumRowsIown += 1;
                    if (this.State.NumRowsIown > 10)
                    {
                        this.State.NumRowsIown = 10;
                    }
                    this.ShowUserInterface(true);
                    App.MainWindow.ReDrawWindows(false);
                    break;
                case "AddANote":
                    this.MenuPopup.IsOpen = false;
                    this.AddANote_OnClick();
                    this.ShowUserInterface(false);
                    break;
                case "AddToBookMarks":
                    App.AddBookmark();
                    this.MenuPopup.IsOpen = false;
                    break;
                case "Hear":
                    this.MenuPopup.IsOpen = false;
                    this.StartAudio_OnClick();
                    this.ShowUserInterface(false);
                    break;
                case "TTS":
                    this.MenuPopup.IsOpen = false;
                    this.StartTTS_OnClick();
                    this.ShowUserInterface(false);
                    break;
                case "Translate":
                    this.MenuPopup.IsOpen = false;

                    object[] reply =
                        await this._state.Source.GetTranslateableTexts(Translations.IsoLanguageCode, App.DisplaySettings, this._state.BibleToLoad);
                    var toTranslate = (string[])reply[0];
                    var isTranslateable = (bool[])reply[1];
                    var transReader2 = new TranslatorReader(string.Empty, string.Empty, false, null, string.Empty, string.Empty);
                    await App.AddWindow(
                        this._state.BibleToLoad,
                        this._state.BibleDescription,
                        WindowType.WindowTranslator,
                        this._state.HtmlFontSize,
                        this._state.Font,
                        null,
                        this._state.Window,
                        transReader2);
                    transReader2.TranslateThis(toTranslate, isTranslateable, this._state.Source.GetLanguage());
                    break;
                case "FontSmaller":
                    this.State.HtmlFontSize -= 1;
                    if (this.State.HtmlFontSize < 4)
                    {
                        this.State.HtmlFontSize = 4;
                    }
                    this.ShowUserInterface(true);
                    this.UpdateBrowser(false);
                    this.Dispatcher.RunAsync(CoreDispatcherPriority.Low, () => App.ShowUserInterface(false));
                    break;
                case "FontLarger":
                    this.State.HtmlFontSize += 1;
                    if (this.State.HtmlFontSize > 65)
                    {
                        this.State.HtmlFontSize = 65;
                    }
                    this.ShowUserInterface(true);
                    this.UpdateBrowser(false);
                    this.Dispatcher.RunAsync(CoreDispatcherPriority.Low, () => App.ShowUserInterface(false));
                    break;
                case "Copy":
                    this.MenuPopup.IsOpen = false;
                    this.Copy_OnClick();
                    this.ShowUserInterface(false);
                    break;
                case "Highlight":
                    this.MenuPopup.IsOpen = false;
                    this.Highlight_OnClick();
                    this.ShowUserInterface(false);
                    break;
                case "SendMail":
                    this.MenuPopup.IsOpen = false;
                    this.MenuMailClick();
                    this.ShowUserInterface(true);
                    break;
                case "EnterKey":
                    this.MenuPopup.IsOpen = false;
                    this.EnterKey_OnClick();
                    this.ShowUserInterface(false);
                    break;
            }
            this.SubMenuMenuPopup.SelectedItem = null;
        }

        #endregion
    }
}