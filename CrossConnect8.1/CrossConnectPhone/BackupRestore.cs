#region Header

// <copyright file="BackupRestore.cs" company="Thomas Dilts">
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

//using Microsoft.Live;
using System;
using System.Collections.Generic;
using System.IO;
using ICSharpCode.SharpZipLib.Zip;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Provider;
using Windows.Storage.Streams;
using Windows.ApplicationModel.Activation;

namespace CrossConnect
{
    public class BackupRestore
    {
        public bool IsCanceled = false;

        double _progressIncrement = 0;
        public const string PersistantObjectsWindowsFileName = "_Windows.xml";
        public const string PersistantObjectsThemesFileName = "_Themes.xml";
        public const string PersistantObjectsDisplaySettingsFileName = "_DisplaySettings.xml";
        public const string PersistantObjectsMarkersFileName = "_Markers.xml";
        public const string PersistantObjectsHighlightFileName = "_Highlights.xml";


        private async Task<StorageFolder> GetTransferFolder()
        {
            if (!await Hoot.File.Exists("shared"))
            {
                await ApplicationData.Current.LocalFolder.CreateFolderAsync("shared");
            }
            var sharedFolder = await ApplicationData.Current.LocalFolder.GetFolderAsync("shared");
            if (!await Hoot.File.Exists(sharedFolder, "transfers"))
            {
                await sharedFolder.CreateFolderAsync("transfers");
            }

            return await sharedFolder.GetFolderAsync("transfers");
        }
        private const string BackupManifestFileName = "_BackupManifest.xml";
        public delegate void Progress(double percentTotal, double percentPartial, bool IsFinal, string Message, string MessageTranslateable1, string MessageTranslateable2);
        public delegate void ReturnToWindow();
        Progress progressCallback;
        double oneDriveProgressBarTotal = 0;
        public void DoExport(BackupManifest manifest, Progress progressCallback, string suggestedName)
        {
            this.progressCallback = progressCallback;
            manifestSelected = manifest;
            try
            {
                FileSavePicker savePicker = new FileSavePicker();
                savePicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
                // Dropdown of file types the user can save the file as
                savePicker.FileTypeChoices.Add("Zip archive", new List<string>() { ".zip" });
                // Default file name if the user does not type one in or select a file to replace
                savePicker.SuggestedFileName = suggestedName + ".zip";
                savePicker.PickSaveFileAndContinue();

            }
            catch (Exception e)
            {
                progressCallback(100, 100, true, null, Uri.UnescapeDataString(e.Message), null);
                return;
            }

        }
        public async void DoExportContinued(FileSavePickerContinuationEventArgs args)
        {

            try
            {
                StorageFile zipFile = args.File;
                if (zipFile == null)
                {
                    progressCallback(100, 100, true, null, null, null);
                    return;
                }

                // Prevent updates to the remote version of the file until we finish making changes and call CompleteUpdatesAsync.
                //CachedFileManager.DeferUpdates(zipFile);

                var tempSharedTransfers = await GetTransferFolder();

                if (IsCanceled) { progressCallback(100, 100, true, null, null, null); return; };
                progressCallback(5, 0, false, null, null, null);

                var settings = new bool[] { manifestSelected.settings, manifestSelected.themes, manifestSelected.windowSetup, manifestSelected.bookmarks, manifestSelected.highlighting };
                var filenames = new string[]
                {
                    PersistantObjectsDisplaySettingsFileName,
                    PersistantObjectsThemesFileName,
                    PersistantObjectsWindowsFileName,
                    PersistantObjectsMarkersFileName,
                    PersistantObjectsHighlightFileName
                };

                var fileTransferList = new List<FileTransferItem>();
                for (int i = 0; i < settings.Length; i++)
                {
                    if (settings[i])
                    {
                        fileTransferList.Add(new FileTransferItem(ApplicationData.Current.LocalFolder, filenames[i], ""));
                    }
                }

                progressCallback(15, 0, false, null, null, null);
                if (manifestSelected.bibles)
                {
                    await PutFolderSimulation(tempSharedTransfers, ApplicationData.Current.LocalFolder, "mods.d", "", fileTransferList);
                    if (IsCanceled) { progressCallback(100, 100, true, null, null, null); return; };
                    await PutFolderSimulation(tempSharedTransfers, ApplicationData.Current.LocalFolder, "modules", "", fileTransferList);
                    if (IsCanceled) { progressCallback(100, 100, true, null, null, null); return; };
                }

                this.oneDriveProgressBarTotal = 20;
                progressCallback(this.oneDriveProgressBarTotal, 0, false, null, null, null);
                _progressIncrement = 75.0 / (double)fileTransferList.Count();

                using (var zipStream = await zipFile.OpenStreamForWriteAsync())
                {
                    // Create zip archive
                    using (var zipArchive = new ZipOutputStream(zipStream))
                    {
                        // For each file to compress...
                        foreach (var fileToCompress in fileTransferList)
                        {
                            if (await Hoot.File.Exists(fileToCompress.folder, fileToCompress.filename))
                            {
                                var fileObj = await fileToCompress.folder.GetFileAsync(fileToCompress.filename);
                                //Read the contents of the file
                                byte[] buffer = WindowsRuntimeBufferExtensions.ToArray(await FileIO.ReadBufferAsync(fileObj));
                                // Create a zip archive entry
                                ZipEntry entry = new ZipEntry(Path.Combine(fileToCompress.folderPath, fileToCompress.filename));
                                // And write the contents to it
                                zipArchive.PutNextEntry(entry);
                                zipArchive.Write(buffer, 0, buffer.Length);

                                this.oneDriveProgressBarTotal += _progressIncrement;
                                progressCallback(this.oneDriveProgressBarTotal, 0, false, null, null, null);
                                if (IsCanceled) { progressCallback(100, 100, true, null, null, null); return; };
                            }
                        }
                        ZipEntry entryManifest = new ZipEntry(BackupManifestFileName);
                        // And write the contents to it
                        zipArchive.PutNextEntry(entryManifest);
                        var buff = Encoding.UTF8.GetBytes(Serialize(manifestSelected));
                        zipArchive.Write(buff, 0, buff.Length);
                    }

                    this.oneDriveProgressBarTotal = 95;
                    progressCallback(this.oneDriveProgressBarTotal, 0, false, null, null, null);

                }

                // Let Windows know that we're finished changing the file so the other app can update the remote version of the file.
                // Completing updates may require Windows to ask for user input.
                //FileUpdateStatus status = await CachedFileManager.CompleteUpdatesAsync(zipFile);
                //if (status == FileUpdateStatus.Complete)
                //{
                //    //OutputTextBlock.Text = "File " + file.Name + " was saved.";
                //}
                //else
                //{
                //    //OutputTextBlock.Text = "File " + file.Name + " couldn't be saved.";
                //}
            }
            catch (Exception e)
            {
                progressCallback(100, 100, true, null, Uri.UnescapeDataString(e.Message), null);
                return;
            }
            progressCallback(100, 100, true, null, null, null);

        }
        private async Task PutFolderSimulation(StorageFolder tempSharedTransfers, StorageFolder rootFolder, string folder, string path, List<FileTransferItem> fileTransferList)
        {
            //var idFolder = await CreateFolder(folder, idContainingFolder);
            var storageFolder = await rootFolder.GetFolderAsync(folder);
            var files = await Hoot.File.GetFiles(folder, rootFolder);
            foreach (var file in files)
            {
                //await PutFileInFolder(tempSharedTransfers, storageFolder, file, idFolder);
                //this.oneDriveProgressBarTotal += _progressIncrement;
                //progressCallback(this.oneDriveProgressBarTotal, 0, false, null, null, null);
                fileTransferList.Add(new FileTransferItem(storageFolder, file, Path.Combine(path, folder)));
            }
            var containedFolders = await Hoot.File.GetFolders(folder, rootFolder);
            foreach (var containedFolder in containedFolders)
            {
                await PutFolderSimulation(tempSharedTransfers, storageFolder, containedFolder, Path.Combine(path, folder), fileTransferList);
            }
        }

        private async Task ExtractFile(StorageFolder folder, ZipEntry entry, ZipInputStream archive)
        {
            var steps = entry.Name.Split('/', '\\').ToList();
            var filename = steps[steps.Count() - 1];
            steps.RemoveAt(steps.Count() - 1);

            foreach (var i in steps)
            {
                folder = await folder.CreateFolderAsync(i, CreationCollisionOption.OpenIfExists);
                //folder = await folder.GetFolderAsync(i);
            }


            StorageFile outputFile = await folder.CreateFileAsync(filename, CreationCollisionOption.ReplaceExisting);

            using (Stream outputFileStream = await outputFile.OpenStreamForWriteAsync())
            {
                int bytesRead;
                byte[] buffer = new byte[10000];
                try
                {
                    // if you use async read and write it doesn't work.
                    while ((bytesRead = archive.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        await outputFileStream.WriteAsync(buffer, 0, bytesRead);
                    }
                }
                catch (Exception e)
                {

                    throw;
                }

                await outputFileStream.FlushAsync();
                outputFileStream.Close();
            }
            
        }

        private async Task CreateRecursiveFolder(StorageFolder folder, ZipEntry entry)
        {
            var steps = entry.Name.Split('/').ToList();

            steps.RemoveAt(steps.Count() - 1);

            foreach (var i in steps)
            {
                await folder.CreateFolderAsync(i, CreationCollisionOption.OpenIfExists);

                folder = await folder.GetFolderAsync(i);
            }
        }

        BackupManifest manifestSelected;

        public void DoImport(BackupManifest manifestSelected, Progress progressCallback)
        {
            this.progressCallback = progressCallback;
            this.manifestSelected = manifestSelected;
            try
            {

                FileOpenPicker openPicker = new FileOpenPicker();
                openPicker.ViewMode = PickerViewMode.Thumbnail;
                openPicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
                openPicker.FileTypeFilter.Add(".zip");
                openPicker.PickSingleFileAndContinue();
            }
            catch (Exception ee)
            {
                progressCallback(100, 100, true, null, Uri.UnescapeDataString(ee.Message), null);
                return;
            }

        }
        public async void DoImportContinued(FileOpenPickerContinuationEventArgs args)
        {
            try
            {
                StorageFile zipFile = args.Files.FirstOrDefault();
                if (zipFile == null)
                {
                    progressCallback(100, 100, true, null, null, null);
                    return;
                }

                var zipStream = await zipFile.OpenStreamForReadAsync();
                var archive = new ZipInputStream(zipStream);
                Dictionary<string, ManifestCheck> manifestCheck=null;
                ZipEntry entry;
                bool foundManifestFile = false;
                var numberOfEntries = 0;
                while ((entry = archive.GetNextEntry()) != null)
                {
                    if (entry.Name.Equals(BackupManifestFileName))
                    {
                        foundManifestFile = true;

                        var buff = new byte[5000]; //it is impossible that this file is over 5000 bytes

                        var bytes = archive.Read(buff, 0, buff.Length);
                        Array.Resize(ref buff, bytes);

                        BackupRestore.BackupManifest manifestFound = (BackupRestore.BackupManifest)Deserialize(buff, typeof(BackupRestore.BackupManifest));
                        manifestCheck = new Dictionary<string, ManifestCheck>
                        {
                            { string.Empty, new ManifestCheck(manifestFound.bibles, manifestSelected.bibles, "Bibles",null) },
                            { PersistantObjectsDisplaySettingsFileName, new ManifestCheck(manifestFound.settings, manifestSelected.settings, "Settings",PersistantObjectsDisplaySettingsFileName) },
                            { PersistantObjectsHighlightFileName, new ManifestCheck(manifestFound.highlighting, manifestSelected.highlighting, "Highlighting",PersistantObjectsHighlightFileName) },
                            { PersistantObjectsMarkersFileName, new ManifestCheck(manifestFound.bookmarks, manifestSelected.bookmarks, "Bookmarks and custom notes",PersistantObjectsMarkersFileName) },
                            { PersistantObjectsThemesFileName, new ManifestCheck(manifestFound.themes, manifestSelected.themes, "Themes",PersistantObjectsThemesFileName) },
                            { PersistantObjectsWindowsFileName, new ManifestCheck(manifestFound.windowSetup, manifestSelected.windowSetup, "Window setup", PersistantObjectsWindowsFileName) }
                        };
                    }

                    numberOfEntries++;
                }

                if (!foundManifestFile)
                {
                    progressCallback(100, 0, true, null, null, "Not a valid backup. No manifest file");
                    return;
                }


                foreach (var check in manifestCheck)
                {
                    if (check.Value.RequestValue && !check.Value.ManifestValue)
                    {
                        progressCallback(100, 0, true, null, check.Value.Message, "Doesn't exist in archive. Remove it and try again");
                        return;
                    }
                }
                this.oneDriveProgressBarTotal = 20;
                progressCallback(this.oneDriveProgressBarTotal, 0, false, null, null, null);
                _progressIncrement = 75.0 / (double)numberOfEntries;
                zipStream.Close();
                archive.Close();
                zipStream = await zipFile.OpenStreamForReadAsync();
                archive = new ZipInputStream(zipStream);
                while ((entry = archive.GetNextEntry()) != null)
                {
                    ManifestCheck foundCheck = null;
                    if (!manifestCheck.TryGetValue(entry.Name, out foundCheck) || foundCheck.RequestValue)
                    {

                        if (entry.Name == "" && manifestSelected.bibles)
                        {
                            // Folder
                            await CreateRecursiveFolder(ApplicationData.Current.LocalFolder, entry);
                        }
                        else if (entry.Name != "" && !entry.Name.Equals(BackupManifestFileName) && (foundCheck != null || manifestSelected.bibles))
                        {
                            // File
                            await ExtractFile(ApplicationData.Current.LocalFolder, entry, archive);
                        }
                    }

                    this.oneDriveProgressBarTotal += _progressIncrement;
                    progressCallback(this.oneDriveProgressBarTotal, 0, false, null, null, null);
                }

                zipStream.Close();
                archive.Close();
            }
            catch (Exception ee)
            {
                progressCallback(100, 100, true, null, Uri.UnescapeDataString(ee.Message), null);
                return;
            }

            progressCallback(100, 100, true, null, null, null);

        }
        /*
        public static async Task MoveFolder(StorageFolder fromFolder, StorageFolder toFolder)
        {
            var files = await fromFolder.GetFilesAsync();
            foreach (var file in files)
            {
                await Hoot.File.Delete(file.Name, toFolder);
                await file.MoveAsync(toFolder);
            }
            var folders = await fromFolder.GetFoldersAsync();
            foreach (var folder in folders)
            {
                if (await Hoot.File.Exists(toFolder, folder.Name))
                {
                    await (await toFolder.GetFolderAsync(folder.Name)).DeleteAsync();
                }

                var childFolder = await toFolder.CreateFolderAsync(folder.Name);
                await MoveFolder(folder, childFolder);
            }
        }*/

        public static string Serialize(object obj)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            using (StreamReader reader = new StreamReader(memoryStream))
            {
                DataContractSerializer serializer = new DataContractSerializer(obj.GetType());
                serializer.WriteObject(memoryStream, obj);
                memoryStream.Position = 0;
                return reader.ReadToEnd();
            }
        }
        public static object Deserialize(Byte[] xml, Type toType)
        {
            using (MemoryStream memoryStream = new MemoryStream(xml))
            {
                XmlDictionaryReader reader = XmlDictionaryReader.CreateTextReader(memoryStream, XmlDictionaryReaderQuotas.Max);
                DataContractSerializer serializer = new DataContractSerializer(toType);
                return serializer.ReadObject(reader);
            }
        }

        private class ManifestCheck
        {
            public bool ManifestValue;
            public bool RequestValue;
            public string fileName;
            public string Message;
            public ManifestCheck(bool ManifestValue, bool RequestValue, string Message, string fileName)
            {
                this.ManifestValue = ManifestValue;
                this.RequestValue = RequestValue;
                this.Message = Message;
                this.fileName = fileName;
            }
        }
        private class FileTransferItem
        {
            public FileTransferItem(StorageFolder folder, string filename, string folderPath)
            {
                this.folder = folder;
                this.filename = filename;
                this.folderPath = folderPath;
            }
            public StorageFolder folder;
            public string filename;
            public string folderPath;
        }
        [DataContract]
        public class BackupManifest
        {
            [DataMember]
            public bool bibles;

            [DataMember]
            public bool settings;

            [DataMember]
            public bool bookmarks;

            [DataMember]
            public bool themes;

            [DataMember]
            public bool highlighting;

            [DataMember]
            public bool windowSetup;

            [DataMember]
            public bool IsWindowsPhone;
        }
    }
}
