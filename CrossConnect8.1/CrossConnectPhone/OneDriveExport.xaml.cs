#region Header

// <copyright file="Settings.xaml.cs" company="Thomas Dilts">
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
    using System.Dynamic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Windows;
    using System.Collections.Generic;
    using Sword.reader;
    using Windows.ApplicationModel.DataTransfer;
    using System.Threading.Tasks;
    using Microsoft.Live;
    using Windows.Storage;
    using System.Runtime.Serialization;
    using System.IO;
    using System.Xml;
    using System.Text;
    using System.Linq;
    using System.Threading;
    using Microsoft.Phone.Storage;
    using Windows.System;
    using Windows.Storage.Pickers;
    using Windows.ApplicationModel.Activation;
    using System.Windows.Navigation;

    /// <summary>
    /// The settings.
    /// </summary>
    public partial class OneDriveExport
    {
        private bool _isInThisWindow = false;
        private IBackupRestore backupRestoreObj = null;
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Settings"/> class.
        /// </summary>
        public OneDriveExport()
        {
            InitializeComponent();
        }

        #endregion Constructors

        #region Methods

        private void AutoRotatePageBackKeyPress(object sender, CancelEventArgs e)
        {
            _isInThisWindow = false;
            //backupRestoreObj.IsCanceled = true;
            
        }

        bool _isTransfering = false;

        private void UpdateUi()
        {
            var isConnected = backupRestoreObj == null ? false : backupRestoreObj.IsConnected;
            oneDriveButConnectSdCard.Visibility = isConnected || !_existsSdCard ? Visibility.Collapsed : Visibility.Visible;

            oneDriveContentPanel.Visibility = isConnected && !_isTransfering ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
            oneDriveConnectPanel.Visibility = isConnected || _isTransfering ? System.Windows.Visibility.Collapsed : System.Windows.Visibility.Visible;
            oneDriveButConnect.Visibility = isConnected ? System.Windows.Visibility.Collapsed : System.Windows.Visibility.Visible;
            oneDriveProgressBar.Visibility = isConnected ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
            oneDriveConnectPanelTransfer.Visibility = _isTransfering ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
        }
        private async void ButConnectClick(object sender, RoutedEventArgs e)
        {
            oneDriveButConnect.Visibility = System.Windows.Visibility.Collapsed;
            oneDriveProgressBar.Visibility = System.Windows.Visibility.Visible;
            oneDriveButConnectSdCard.Visibility = System.Windows.Visibility.Collapsed;
            backupRestoreObj = new BackupRestoreOneDrive();
            backupRestoreObj.IsCanceled = false;
            var message = await backupRestoreObj.AuthenticateUser();
            if (!string.IsNullOrEmpty(message))
            {
                MessageBox.Show(message);
            }
            UpdateUi();
        }
        private void ButConnectSdCardClick(object sender, RoutedEventArgs e)
        {
            oneDriveButConnect.Visibility = System.Windows.Visibility.Collapsed;
            oneDriveProgressBar.Visibility = System.Windows.Visibility.Collapsed;
            oneDriveButConnectSdCard.Visibility = System.Windows.Visibility.Collapsed;
            backupRestoreObj = new BackupRestoreSdCard();
            backupRestoreObj.IsCanceled = false;
      
            UpdateUi();
        }

        private bool _existsSdCard = false;
        private void AutoRotatePageLoaded(object sender, RoutedEventArgs e)
        {
            if (_isInThisWindow)
            {
                return;
            }
            _isInThisWindow = true;

            PageTitle.Text = Translations.Translate("OneDrive backup / restore");
            oneDriveInformationText.Text = Translations.Translate("Select the items you want to backup / restore. Then hit the button at the bottom");
            oneDriveButExport.Content = Translations.Translate("Backup");
            oneDriveButImport.Content = Translations.Translate("Restore");
            oneDriveBibles.Header = Translations.Translate("Bible");
            oneDriveSettings.Header = Translations.Translate("Settings");
            oneDriveHighlighting.Header = Translations.Translate("Highlight");
            oneDriveBookmarks.Header = Translations.Translate("Bookmarks and custom notes");
            oneDriveThemes.Header = Translations.Translate("Themes");
            oneDriveWindowSetup.Header = Translations.Translate("Window setup");
            this.oneDriveCaptionPutInFolder.Text = Translations.Translate("Folder on OneDrive");
            this.oneDriveButConnect.Content = Translations.Translate("Connect to OneDrive");
            this.oneDriveButLogout.Content = Translations.Translate("Logout from OneDrive");
            this.oneDriveButConnectSdCard.Content = Translations.Translate("Connect to memory card");

            bool successfulInitialize = false;
            while (!successfulInitialize)
            {
                try
                {
                    oneDrivePutInFolder.Text = App.DisplaySettings.OneDriveFolder;

                    successfulInitialize = true;
                }
                catch (Exception eee)
                {
                    Debug.WriteLine("null in probably: " + eee.Message + "; " + eee.StackTrace);
                }
            }

            new Thread(() =>
            {
                _existsSdCard = ExistsSdCard().Result;
                Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    UpdateUi();
                });
            }).Start();

        }

        private async Task<bool> ExistsSdCard()
        {
            return false;
            //try
            //{
            //    // Get the logical root folder for all external storage devices.
            //    //var sdCard = (await Microsoft.Phone.Storage.ExternalStorage.GetExternalStorageDevicesAsync()).FirstOrDefault();
            //    StorageFolder externalDevices = Windows.Storage.KnownFolders.PicturesLibrary;
            //    //StorageFolder sdCard = (await KnownFolders.RemovableDevices.GetFoldersAsync()).FirstOrDefault();
            //    // Get the first child folder, which represents the SD card.
            //    StorageFolder sdCard = (await externalDevices.GetFoldersAsync()).FirstOrDefault();
            //    /*
            //    if (sdCard != null)
            //    {
            //        // Get the root folder on the SD card.
            //        ExternalStorageFolder sdrootFolder = sdCard.RootFolder;
            //        if (sdrootFolder != null)
            //        {
            //            // List all the files on the root folder.
            //            var files = await sdrootFolder.GetFilesAsync();
            //            if (files != null)
            //            { 
            //            }
            //            var folders = await sdrootFolder.GetFoldersAsync();
            //            if (folders != null)
            //            {
            //            }
            //        }
            //    }*/
            //    return sdCard != null;
            //}
            //catch (Exception e)
            //{
            //    return false;
            //}

        }

        #endregion Methods
        private async void BackupRestoreProgress(double percentTotal, double percentPartial, bool IsFinal, string debug, string Message, string MessageTranslateable1, string MessageTranslateable2)
        {
            if (!string.IsNullOrEmpty(debug))
            {
                oneDriveDebugText.Text = debug;
                return;
            }
            this.oneDriveProgressBarTotal.Value = percentTotal;
            this.oneDriveProgressBarPartial.Value = percentPartial;
            if(IsFinal && !string.IsNullOrEmpty(Message))
            {
                MessageBox.Show(Translations.Translate(
                                "An error occurred trying to connect to the network. Try again later.") + "; " + Message);
                _isTransfering = false;
                UpdateUi();
            }
            else if(IsFinal && !string.IsNullOrEmpty(MessageTranslateable1))
            {
                MessageBox.Show(Translations.Translate(MessageTranslateable1) +  (!string.IsNullOrEmpty(MessageTranslateable2)? " - " + Translations.Translate(MessageTranslateable2):string.Empty));
                _isTransfering = false;
                UpdateUi();
            }
            if(IsFinal)
            {
                await App.LoadPersistantObjects();
                _isTransfering = false;
                UpdateUi();
                Deployment.Current.Dispatcher.BeginInvoke(() => Deployment.Current.Dispatcher.BeginInvoke(() => { App.SavePersistantDisplaySettings(); App.SavePersistantThemes(); }));

            }
        }
        private async void ButExportClick(object sender, RoutedEventArgs e)
        {
            _isTransfering = true;
            UpdateUi();
            this.oneDriveProgressBarTotal.Minimum = 0;
            this.oneDriveProgressBarTotal.Maximum = 100;
            this.oneDriveProgressBarTotal.Value = 3;
            this.oneDrivePutInFolder.Text = this.oneDrivePutInFolder.Text.Replace("\\", "").Replace("/", "").Trim();
            if (string.IsNullOrEmpty(this.oneDrivePutInFolder.Text))
            {
                this.oneDrivePutInFolder.Text = "CrossConnectBackup";
            }
            App.DisplaySettings.OneDriveFolder = this.oneDrivePutInFolder.Text;
            await App.SaveAllPersistantObjects();
            await backupRestoreObj.DoExport(
                new BackupRestore.BackupManifest 
                {  
                    bibles=(bool)this.oneDriveBibles.IsChecked,
                    settings = (bool)this.oneDriveSettings.IsChecked,
                    bookmarks = (bool)this.oneDriveBookmarks.IsChecked,
                    themes = (bool)this.oneDriveThemes.IsChecked,
                    highlighting = (bool)this.oneDriveHighlighting.IsChecked,
                    windowSetup = (bool)this.oneDriveWindowSetup.IsChecked,
                    IsWindowsPhone = true
                }, this.oneDrivePutInFolder.Text, BackupRestoreProgress);
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
                    bibles = (bool)this.oneDriveBibles.IsChecked,
                    settings = (bool)this.oneDriveSettings.IsChecked,
                    bookmarks = (bool)this.oneDriveBookmarks.IsChecked,
                    themes = (bool)this.oneDriveThemes.IsChecked,
                    highlighting = (bool)this.oneDriveHighlighting.IsChecked,
                    windowSetup = (bool)this.oneDriveWindowSetup.IsChecked,
                    IsWindowsPhone = true
                }, this.oneDrivePutInFolder.Text, BackupRestoreProgress);
        }

        private void oneDriveButLogout_Click(object sender, RoutedEventArgs e)
        {
            backupRestoreObj.LogOut();
            _isTransfering = false;
            UpdateUi();
        }

        private void oneDriveButHelp_Click(object sender, RoutedEventArgs e)
        {
            Launcher.LaunchUriAsync(
                new Uri(@"http://www.cross-connect.se/help-metro/#backuprestore", UriKind.Absolute));
        }

        private async void oneDriveButSwordImport_Click(object sender, RoutedEventArgs e)
        {
            FileOpenPicker openPicker = new FileOpenPicker();
            openPicker.ViewMode = PickerViewMode.Thumbnail;
            openPicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
            openPicker.FileTypeFilter.Add(".zip");
            openPicker.PickSingleFileAndContinue();

        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            var app = App.Current as App;
            if (app.FilePickerContinuationArgs != null)
            {
                this.ContinueFileOpenPicker(app.FilePickerContinuationArgs);
            }
        }
        /// <summary>
        /// Handle the returned files from file picker
        /// This method is triggered by ContinuationManager based on ActivationKind
        /// </summary>
        /// <param name="args">File open picker continuation activation argment. It cantains the list of files user selected with file open picker </param>
        public async void ContinueFileOpenPicker(FileOpenPickerContinuationEventArgs args)
        {
            if (args.Files.Count > 0)
            {
                var error = await SwordBook.TryInstallBibleFromZipFile(args.Files.First());
                if (!string.IsNullOrEmpty(error[0]))
                {
                    MessageBox.Show(error[0]);
                }
                else
                {
                    await App.InstalledBibles.AddGenericBook(error[1]);
                }
            }

        }
        public async void ContinueFileSavePicker(FileSavePickerContinuationEventArgs args)
        {

        }
    }
}