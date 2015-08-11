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
        public enum FILE_PICKER_WAIT
        {
            WAIT_SWORD_IMPORT,
            WAIT_EXPORT,
            WAIT_IMPORT,
            NONE
        }
        private FILE_PICKER_WAIT currentlyWaitingFor= FILE_PICKER_WAIT.NONE;
        private bool _isInThisWindow = false;
        private BackupRestore backupRestoreObj = new BackupRestore();
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
            currentlyWaitingFor = FILE_PICKER_WAIT.NONE;
            _isTransfering = false;
        }

        bool _isTransfering = false;

        private void UpdateUi()
        {
            oneDriveContentPanel.Visibility = !_isTransfering ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
            oneDriveConnectPanelTransfer.Visibility = _isTransfering ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
        }

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

            bool successfulInitialize = false;
            while (!successfulInitialize)
            {
                try
                {

                    successfulInitialize = true;
                }
                catch (Exception eee)
                {
                    Debug.WriteLine("null in probably: " + eee.Message + "; " + eee.StackTrace);
                }
            }

            new Thread(() =>
            {
                Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    UpdateUi();
                });
            }).Start();

        }

        #endregion Methods
        private async void BackupRestoreProgress(double percentTotal, double percentPartial, bool IsFinal, string Message, string MessageTranslateable1, string MessageTranslateable2)
        {
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
                this.oneDriveProgressBarPartial.Value = 75;
                await App.LoadPersistantObjects();
                _isTransfering = false;
                this.oneDriveProgressBarPartial.Value = 100;
                UpdateUi();
                Deployment.Current.Dispatcher.BeginInvoke(() => Deployment.Current.Dispatcher.BeginInvoke(() => { App.SavePersistantDisplaySettings(); App.SavePersistantThemes(); }));
                currentlyWaitingFor = FILE_PICKER_WAIT.NONE;
            }
        }
        private async void ButExportClick(object sender, RoutedEventArgs e)
        {
            _isTransfering = true;
            UpdateUi();
            currentlyWaitingFor = FILE_PICKER_WAIT.WAIT_EXPORT;
            this.oneDriveProgressBarTotal.Minimum = 0;
            this.oneDriveProgressBarTotal.Maximum = 100;
            this.oneDriveProgressBarTotal.Value = 3;

            await App.SaveAllPersistantObjects();
            backupRestoreObj.DoExport(
                new BackupRestore.BackupManifest 
                {  
                    bibles=(bool)this.oneDriveBibles.IsChecked,
                    settings = (bool)this.oneDriveSettings.IsChecked,
                    bookmarks = (bool)this.oneDriveBookmarks.IsChecked,
                    themes = (bool)this.oneDriveThemes.IsChecked,
                    highlighting = (bool)this.oneDriveHighlighting.IsChecked,
                    windowSetup = (bool)this.oneDriveWindowSetup.IsChecked,
                    IsWindowsPhone = true
                }, 
                BackupRestoreProgress,
                Translations.Translate("Backup") + DateTime.Now.ToString("yyyy-MM-dd.HH.mm"));
        }
        private void ReturnToWindowCallback()
        {

        }
        private void butImport_Click(object sender, RoutedEventArgs e)
        {
            currentlyWaitingFor = FILE_PICKER_WAIT.WAIT_IMPORT;
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
                }, BackupRestoreProgress);
        }

        private void oneDriveButHelp_Click(object sender, RoutedEventArgs e)
        {
            Launcher.LaunchUriAsync(
                new Uri(@"http://www.cross-connect.se/help-metro/#backuprestore", UriKind.Absolute));
        }

        private async void oneDriveButSwordImport_Click(object sender, RoutedEventArgs e)
        {
            currentlyWaitingFor = FILE_PICKER_WAIT.WAIT_SWORD_IMPORT;
            FileOpenPicker openPicker = new FileOpenPicker();
            openPicker.ViewMode = PickerViewMode.Thumbnail;
            openPicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
            openPicker.FileTypeFilter.Add(".zip");
            openPicker.PickSingleFileAndContinue();

        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            var app = App.Current as App;
            if (app.FilePickerContinuationArgs != null )
            {
                switch(currentlyWaitingFor)
                {
                    case FILE_PICKER_WAIT.WAIT_SWORD_IMPORT:
                        this.ContinueFileOpenPickerSwordImport(app.FilePickerContinuationArgs as FileOpenPickerContinuationEventArgs);
                        break;
                    case FILE_PICKER_WAIT.WAIT_IMPORT:
                        this.backupRestoreObj.DoImportContinued(app.FilePickerContinuationArgs as FileOpenPickerContinuationEventArgs);
                        break;
                    case FILE_PICKER_WAIT.WAIT_EXPORT:
                        this.backupRestoreObj.DoExportContinued(app.FilePickerContinuationArgs as FileSavePickerContinuationEventArgs);
                        break;
                }
            }
        }
        /// <summary>
        /// Handle the returned files from file picker
        /// This method is triggered by ContinuationManager based on ActivationKind
        /// </summary>
        /// <param name="args">File open picker continuation activation argment. It cantains the list of files user selected with file open picker </param>
        public async void ContinueFileOpenPickerSwordImport(FileOpenPickerContinuationEventArgs args)
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
        public async void ContinueFileSavePickerForExport(FileSavePickerContinuationEventArgs args)
        {

        }
    }
}