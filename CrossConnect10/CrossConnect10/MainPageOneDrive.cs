﻿// <copyright file="MainPageThemes.cs" company="Thomas Dilts">
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
    using Windows.Storage;
    using Windows.Storage.Pickers;
    using Windows.System;
    using Windows.UI.Core;
    using Windows.UI.Popups;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml.Media;

    public sealed partial class MainPageSplit
    {
        private BackupRestore backupRestoreObj = new BackupRestore();
        bool _isTransfering = false;
        #region Methods

        private void UpdateUi()
        {
            oneDriveContentPanel.Visibility = !_isTransfering ? Windows.UI.Xaml.Visibility.Visible : Windows.UI.Xaml.Visibility.Collapsed;
            //oneDriveConnectPanel.Visibility = backupRestoreObj.IsConnected || _isTransfering ? Windows.UI.Xaml.Visibility.Collapsed : Windows.UI.Xaml.Visibility.Visible;
            //oneDriveButConnect.Visibility = backupRestoreObj.IsConnected ? Windows.UI.Xaml.Visibility.Collapsed : Windows.UI.Xaml.Visibility.Visible;
            //oneDriveProgressBar.Visibility = backupRestoreObj.IsConnected ? Windows.UI.Xaml.Visibility.Visible : Windows.UI.Xaml.Visibility.Collapsed;
            oneDriveConnectPanelTransfer.Visibility = _isTransfering ? Windows.UI.Xaml.Visibility.Visible : Windows.UI.Xaml.Visibility.Collapsed;
        }
        private void SetupOneDrivePage()
        {
            backupRestoreObj.IsCanceled = false;
            UpdateUi();

            OneDriveTitle.Text = Translations.Translate("Backup / restore");
            oneDriveInformationText.Text = Translations.Translate("Select the items you want to backup / restore. Then hit the button at the bottom");
            oneDriveButExport.Content = Translations.Translate("Backup");
            oneDriveButImport.Content = Translations.Translate("Restore");
            oneDriveBibles.Header = Translations.Translate("Bible");
            oneDriveSettings.Header = Translations.Translate("Settings");
            oneDriveHighlighting.Header = Translations.Translate("Highlight");
            oneDriveBookmarks.Header = Translations.Translate("Bookmarks and custom notes");
            oneDriveThemes.Header = Translations.Translate("Themes");
            oneDriveWindowSetup.Header = Translations.Translate("Window setup");
            oneDriveButCancel.Content = Translations.Translate("Cancel");
            oneDriveButHelp.Content = Translations.Translate("Help");
            this.ImportOneSwordModule.Content = Translations.Translate("Install one 'Sword' module");

        }
        private void MenuOneDriveBackupRestoreClick(object sender, RoutedEventArgs e)
        {
            this.SetupOneDrivePage();
            SideBarShowPopup(
                this.OneDrivePopup, this.MainPaneOneDrivePopup, this.scrollViewerOneDrive, this.TopAppBar1, this.BottomAppBar);
            this.OneDrivePopup.IsOpen = true;
        }
        
        private void oneDriveButCancelClick(object sender, RoutedEventArgs e)
        {
            backupRestoreObj.IsCanceled = true;
            this.OneDrivePopup.IsOpen = false;
            _isTransfering = false;
            UpdateUi();
        }
        private async void BackupRestoreProgress(double percentTotal, double percentPartial, bool IsFinal, string Message, string MessageTranslateable1, string MessageTranslateable2)
        {
            this.oneDriveProgressBarTotal.Value = percentTotal;
            this.oneDriveProgressBarPartial.Value = percentPartial;
            bool isError = false;
            if (IsFinal && !string.IsNullOrEmpty(Message))
            {
                var dialog = new MessageDialog(Translations.Translate("An error occurred trying to connect to the network. Try again later.") + "; " + Message);
                await dialog.ShowAsync();
                _isTransfering = false;
                isError = true;
                UpdateUi();
            }
            else if (IsFinal && !string.IsNullOrEmpty(MessageTranslateable1))
            {
                var dialog = new MessageDialog(Translations.Translate(MessageTranslateable1) + (!string.IsNullOrEmpty(MessageTranslateable2) ? " - " + Translations.Translate(MessageTranslateable2) : string.Empty));
                await dialog.ShowAsync();
                _isTransfering = false;
                isError = true;
                UpdateUi();
            }
            if (IsFinal && !isError)
            {
                MainPageSplitUnloaded(null, null);
                await App.LoadPersistantObjects();
                _isTransfering = false;
                UpdateUi();
                RecolorScreen();
                App.MainWindow.ReDrawWindows();

                App.SavePersistantThemes();
                App.SavePersistantDisplaySettings();
            }
        }
        private void ReturnToWindowCallback()
        {
            this.OneDrivePopup.IsOpen = true;
        }
        private async void ButExportClick(object sender, RoutedEventArgs e)
        {
            _isTransfering = true;
            UpdateUi();
            this.oneDriveProgressBarTotal.Minimum = 0;
            this.oneDriveProgressBarTotal.Maximum = 100;
            this.oneDriveProgressBarTotal.Value = 3;

            await App.SaveAllPersistantObjects();
            await backupRestoreObj.DoExport(
                new BackupRestore.BackupManifest
                {
                    bibles = (bool)this.oneDriveBibles.IsOn,
                    settings = (bool)this.oneDriveSettings.IsOn,
                    bookmarks = (bool)this.oneDriveBookmarks.IsOn,
                    themes = (bool)this.oneDriveThemes.IsOn,
                    highlighting = (bool)this.oneDriveHighlighting.IsOn,
                    windowSetup = (bool)this.oneDriveWindowSetup.IsOn,
                    IsWindowsPhone = false
                }, 
                string.Empty, 
                BackupRestoreProgress,
                ReturnToWindowCallback,
                Translations.Translate("Backup") + DateTime.Now.ToString("yyyy-MM-dd.HH.mm"));
        }

        private void butImport_Click(object sender, RoutedEventArgs e)
        {
            _isTransfering = true;
            UpdateUi();
            this.oneDriveProgressBarTotal.Minimum = 0;
            this.oneDriveProgressBarTotal.Maximum = 100;
            this.oneDriveProgressBarTotal.Value = 3;
            backupRestoreObj.DoImport(
                new BackupRestore.BackupManifest
                {
                    bibles = (bool)this.oneDriveBibles.IsOn,
                    settings = (bool)this.oneDriveSettings.IsOn,
                    bookmarks = (bool)this.oneDriveBookmarks.IsOn,
                    themes = (bool)this.oneDriveThemes.IsOn,
                    highlighting = (bool)this.oneDriveHighlighting.IsOn,
                    windowSetup = (bool)this.oneDriveWindowSetup.IsOn,
                    IsWindowsPhone = true
                }, string.Empty, BackupRestoreProgress, ReturnToWindowCallback);
        }
        /*
        private void oneDriveButLogout_Click(object sender, RoutedEventArgs e)
        {
            backupRestoreObj.LogOut();
            _isTransfering = false;
            UpdateUi();
            OneDrivePopup.IsOpen = false;
        }*/
        private void OneDrivePopup_OnClosed(object sender, object e)
        {
            App.ShowUserInterface(true);
        }

        private void OneDrivePopup_OnOpened(object sender, object e)
        {
            App.ShowUserInterface(false);
        }

        private async void ImportOneSwordModule_Click(object sender, RoutedEventArgs e)
        {

            FileOpenPicker openPicker = new FileOpenPicker();
            openPicker.ViewMode = PickerViewMode.Thumbnail;
            openPicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
            openPicker.FileTypeFilter.Add(".zip");

            StorageFile zipFile = await openPicker.PickSingleFileAsync();
            if (zipFile == null)
            {
                return;
            }

            var error = await SwordBook.TryInstallBibleFromZipFile(zipFile);
            if (!string.IsNullOrEmpty(error[0]))
            {
                var dialog2 = new MessageDialog(error[0]);
                await dialog2.ShowAsync();
            }
            else
            {
                await App.InstalledBibles.AddGenericBook(error[1]);
            }
        }

        private async void oneDriveButHelp_Click(object sender, RoutedEventArgs e)
        {
            await Launcher.LaunchUriAsync(
                new Uri(@"http://www.cross-connect.se/help-metro/#backuprestore", UriKind.Absolute));
        }

        #endregion
    }
}