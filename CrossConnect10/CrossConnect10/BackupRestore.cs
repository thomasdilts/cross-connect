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
using System.IO.Compression;
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
        Progress progressCallback;
        double oneDriveProgressBarTotal = 0;
        public async Task DoExport(BackupManifest manifest, string crossConnectFolder, Progress progressCallback, string suggestedName)
        {
            this.progressCallback = progressCallback;
            try
            {
                FileSavePicker savePicker = new FileSavePicker();
                savePicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
                // Dropdown of file types the user can save the file as
                savePicker.FileTypeChoices.Add("Zip archive", new List<string>() { ".zip" });
                // Default file name if the user does not type one in or select a file to replace
                savePicker.SuggestedFileName = suggestedName + ".zip";
                StorageFile zipFile = await savePicker.PickSaveFileAsync();
                if (zipFile == null)
                {
                    progressCallback(100, 100, true, null, null, null);
                    return;
                }
                
                // Prevent updates to the remote version of the file until we finish making changes and call CompleteUpdatesAsync.
                CachedFileManager.DeferUpdates(zipFile);

                var tempSharedTransfers = await GetTransferFolder();

                if (IsCanceled) { progressCallback(100, 100, true, null, null, null); return; };
                progressCallback(5, 0, false, null, null, null);

                var settings = new bool[] { manifest.settings, manifest.themes, manifest.windowSetup, manifest.bookmarks, manifest.highlighting };
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
                if (manifest.bibles)
                {
                    await PutFolderSimulation(tempSharedTransfers, ApplicationData.Current.LocalFolder, "mods.d", "", fileTransferList);
                    if (IsCanceled) { progressCallback(100, 100, true, null, null, null); return; };
                    await PutFolderSimulation(tempSharedTransfers, ApplicationData.Current.LocalFolder, "modules", "", fileTransferList);
                    if (IsCanceled) { progressCallback(100, 100, true, null, null, null); return; };
                }

                this.oneDriveProgressBarTotal = 20;
                progressCallback(this.oneDriveProgressBarTotal, 0, false, null, null, null);
                _progressIncrement = 75.0 / (double)fileTransferList.Count();


                using (MemoryStream zipMemoryStream = new MemoryStream())
                {
                    // Create zip archive
                    using (ZipArchive zipArchive = new ZipArchive(zipMemoryStream, ZipArchiveMode.Create))
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
                                ZipArchiveEntry entry = zipArchive.CreateEntry(Path.Combine(fileToCompress.folderPath, fileToCompress.filename));
                                // And write the contents to it
                                using (Stream entryStream = entry.Open())
                                {
                                    await entryStream.WriteAsync(buffer, 0, buffer.Length);
                                }
                                this.oneDriveProgressBarTotal += _progressIncrement;
                                progressCallback(this.oneDriveProgressBarTotal, 0, false, null, null, null);
                                if (IsCanceled) { progressCallback(100, 100, true, null, null, null); return; };
                            }
                        }
                        ZipArchiveEntry entryManifest = zipArchive.CreateEntry(BackupManifestFileName);
                        // And write the contents to it
                        using (Stream entryStream = entryManifest.Open())
                        {
                            var buff = Encoding.UTF8.GetBytes(Serialize(manifest));
                            await entryStream.WriteAsync(buff, 0, buff.Length);
                        }
                    }

                    this.oneDriveProgressBarTotal = 95;
                    progressCallback(this.oneDriveProgressBarTotal, 0, false, null, null, null);

                    using (IRandomAccessStream zipStream = await zipFile.OpenAsync(FileAccessMode.ReadWrite))
                    {
                        // Write compressed data from memory to file
                        using (Stream outstream = zipStream.AsStreamForWrite())
                        {
                            byte[] buffer = zipMemoryStream.ToArray();
                            outstream.Write(buffer, 0, buffer.Length);
                            outstream.Flush();
                        }
                    }
                }

                // Let Windows know that we're finished changing the file so the other app can update the remote version of the file.
                // Completing updates may require Windows to ask for user input.
                FileUpdateStatus status = await CachedFileManager.CompleteUpdatesAsync(zipFile);
                if (status == FileUpdateStatus.Complete)
                {
                    //OutputTextBlock.Text = "File " + file.Name + " was saved.";
                }
                else
                {
                    //OutputTextBlock.Text = "File " + file.Name + " couldn't be saved.";
                }
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
                fileTransferList.Add(new FileTransferItem(storageFolder, file, Path.Combine(path,folder)));
            }
            var containedFolders = await Hoot.File.GetFolders(folder, rootFolder);
            foreach (var containedFolder in containedFolders)
            {
                await PutFolderSimulation(tempSharedTransfers, storageFolder, containedFolder, Path.Combine(path, folder), fileTransferList);
            }
        }

        private async Task ExtractFile(StorageFolder folder, ZipArchiveEntry entry)
        {
            var steps = entry.FullName.Split('/','\\').ToList();

            steps.RemoveAt(steps.Count() - 1);

            foreach (var i in steps)
            {
                folder = await folder.GetFolderAsync(i);
            }

            using (Stream fileData = entry.Open())
            {
                StorageFile outputFile = await folder.CreateFileAsync(entry.Name, CreationCollisionOption.ReplaceExisting);

                using (Stream outputFileStream = await outputFile.OpenStreamForWriteAsync())
                {
                    byte[] buffer = new byte[entry.Length];
                    await fileData.ReadAsync(buffer, 0, buffer.Length);
                    await outputFileStream.WriteAsync(buffer, 0, (int)entry.Length);
                    await outputFileStream.FlushAsync();
                    //int bytesRead;
                    //try
                    //{
                    //    while (fileData.CanRead && (bytesRead = await fileData.ReadAsync(buffer, 0, buffer.Length)) != 0)
                    //    {
                    //        await outputFileStream.WriteAsync(buffer, 0, bytesRead);
                    //        if (buffer.Length > bytesRead)
                    //        {
                    //            break;
                    //        }
                    //    }
                    //}
                    //catch (Exception e)
                    //{

                    //    throw;
                    //}

                    //await outputFileStream.FlushAsync();
                }
            }
        }

        private async Task CreateRecursiveFolder(StorageFolder folder, ZipArchiveEntry entry)
        {
            var steps = entry.FullName.Split('/').ToList();

            steps.RemoveAt(steps.Count() - 1);

            foreach (var i in steps)
            {
                await folder.CreateFolderAsync(i, CreationCollisionOption.OpenIfExists);

                folder = await folder.GetFolderAsync(i);
            }
        }

        public async void DoImport(BackupManifest manifestSelected, string crossConnectFolder, Progress progressCallback)
        {
            this.progressCallback = progressCallback;
            try
            {

                FileOpenPicker openPicker = new FileOpenPicker();
                openPicker.ViewMode = PickerViewMode.Thumbnail;
                openPicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
                openPicker.FileTypeFilter.Add(".zip");

                StorageFile zipFile = await openPicker.PickSingleFileAsync();
                if (zipFile == null)
                {
                    progressCallback(100, 100, true, null, null, null);
                    return;
                }

                using (var zipStream = await zipFile.OpenStreamForReadAsync())
                {
                    using (MemoryStream zipMemoryStream = new MemoryStream((int)zipStream.Length))
                    {
                        await zipStream.CopyToAsync(zipMemoryStream);

                        using (var archive = new ZipArchive(zipMemoryStream, ZipArchiveMode.Read))
                        {
                            var manifestEntry = archive.Entries.FirstOrDefault(p => p.Name.Equals(BackupManifestFileName));
                            if(manifestEntry==null)
                            {
                                progressCallback(100, 0, true, null, null, "Not a valid backup. No manifest file");
                                return;
                            }

                            var buff = new byte[5000]; //it is impossible that this file is over 5000 bytes
                            using (var stream = manifestEntry.Open())
                            {
                                var bytes = stream.Read(buff, 0, buff.Length);
                                Array.Resize(ref buff, bytes);
                            }
                            BackupRestore.BackupManifest manifestFound = (BackupRestore.BackupManifest)Deserialize(buff, typeof(BackupRestore.BackupManifest));
                            Dictionary<string, ManifestCheck> manifestCheck = new Dictionary<string, ManifestCheck>
                            {
                                { string.Empty, new ManifestCheck(manifestFound.bibles, manifestSelected.bibles, "Bibles",null) },
                                { PersistantObjectsDisplaySettingsFileName, new ManifestCheck(manifestFound.settings, manifestSelected.settings, "Settings",PersistantObjectsDisplaySettingsFileName) },
                                { PersistantObjectsHighlightFileName, new ManifestCheck(manifestFound.highlighting, manifestSelected.highlighting, "Highlighting",PersistantObjectsHighlightFileName) },
                                { PersistantObjectsMarkersFileName, new ManifestCheck(manifestFound.bookmarks, manifestSelected.bookmarks, "Bookmarks and custom notes",PersistantObjectsMarkersFileName) },
                                { PersistantObjectsThemesFileName, new ManifestCheck(manifestFound.themes, manifestSelected.themes, "Themes",PersistantObjectsThemesFileName) },
                                { PersistantObjectsWindowsFileName, new ManifestCheck(manifestFound.windowSetup, manifestSelected.windowSetup, "Window setup", PersistantObjectsWindowsFileName) }
                            };

                            foreach (var check in manifestCheck)
                            {
                                if (check.Value.RequestValue && !check.Value.ManifestValue)
                                {
                                    progressCallback(100, 0, true, null, check.Value.Message, "Doesn't exist in archive. Remove it and try again");
                                    return;
                                }
                            }

                            foreach (ZipArchiveEntry entry in archive.Entries)
                            {
                                ManifestCheck foundCheck = null;
                                if (!manifestCheck.TryGetValue(entry.Name, out foundCheck) || foundCheck.RequestValue)
                                {
                                    
                                    if (entry.Name == "" && manifestSelected.bibles)
                                    {
                                        // Folder
                                        await CreateRecursiveFolder(ApplicationData.Current.LocalFolder, entry);
                                    }
                                    else if (entry.Name != "" && !entry.Name.Equals(BackupManifestFileName) && (foundCheck!=null || manifestSelected.bibles))
                                    {
                                        // File
                                        await ExtractFile(ApplicationData.Current.LocalFolder, entry);
                                    }
                                }
                            }
                        }
                    }
                }
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
