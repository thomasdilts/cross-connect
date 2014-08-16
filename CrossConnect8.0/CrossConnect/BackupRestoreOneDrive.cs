using Microsoft.Live;
using Microsoft.Phone.BackgroundTransfer;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Windows.Storage;

namespace CrossConnect
{
    public class BackupRestoreOneDrive : IBackupRestore
    {
        #region Methods

        public bool _isCanceled = false;
        public bool IsCanceled
        {
            get
            {
                return _isCanceled;
            }
            set
            {
                _isCanceled = value;
            }
        }
        private bool _isConnected = false;
        public bool IsConnected
        {
            get
            {
                return _isConnected;
            }
        }

        double _progressIncrement = 0;
        private readonly string[] _defaultAuthScopes = new string[] { "wl.signin", /*"wl.basic", "wl.skydrive",*/ "wl.skydrive_update" };
        private LiveConnectClient liveClient = null;
        private LiveAuthClient authClient = null;
        public void LogOut()
        {
            authClient.Logout();
            _isConnected = false;
        }
        public async Task<string> AuthenticateUser()
        {
            string text = null;
            string firstName = string.Empty;
            bool connected = false;
            try
            {
                authClient = new LiveAuthClient("00000000400E8489");
                LiveLoginResult result = await authClient.LoginAsync(_defaultAuthScopes);

                if (result.Status == LiveConnectSessionStatus.Connected)
                {
                    connected = true;
                    this.liveClient = new LiveConnectClient(result.Session);
                    var meResult = await liveClient.GetAsync("me");
                    dynamic meData = meResult.Result;
                    firstName = meData.first_name;
                }
            }
            catch (LiveAuthException ex)
            {
                text = "Error: " + Uri.UnescapeDataString(ex.Message);
            }
            catch (LiveConnectException ex)
            {
                text = "Error: " + Uri.UnescapeDataString(ex.Message);
            }

            _isConnected = connected;

            return text;
        }

        #endregion Methods


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

            return  await sharedFolder.GetFolderAsync("transfers");
        }
        private const string BackupManifestFileName = "_BackupManifest.xml";
        BackupRestoreConstants.Progress progressCallback;
        double oneDriveProgressBarTotal = 0;
        private class FileTransferItem
        {
            public FileTransferItem(StorageFolder folder, string filename, string idFolder)
            {
                this.folder = folder;
                this.filename = filename;
                this.idFolder = idFolder;
            }
            public StorageFolder folder;
            public string filename;
            public string idFolder;
        }

        public async Task DoExport(BackupRestore.BackupManifest manifest, string crossConnectFolder,BackupRestoreConstants.Progress progressCallback)
        {
            this.progressCallback = progressCallback;
            try
            {
                if (_isCanceled) { progressCallback(100, 100, true, null, null, null, null); return; };
                progressCallback(5, 0, false,null, null, null, null);

                var rootContents = await GetRootContents(crossConnectFolder);
                string putInFolderId = rootContents[0][RootCrossConnectId];

                if (_isCanceled) { progressCallback(100, 100, true,null, null, null, null); return; };
                progressCallback(7, 0, false,null, null, null, null);

                var tempSharedTransfers = await GetTransferFolder();

                // remove all files
                foreach (var file in rootContents[0].Where(p => !p.Key.Equals(RootCrossConnectId)))
                {
                    LiveOperationResult operationResult = await liveClient.DeleteAsync(file.Value);
                } 
                foreach (var file in rootContents[1])
                {
                    LiveOperationResult operationResult = await liveClient.DeleteAsync(file.Value);
                }
                progressCallback(10, 0, false,null, null, null, null);
                if (_isCanceled) { progressCallback(100, 100, true,null, null, null, null); return; };

                var settings = new bool[] { manifest.settings, manifest.themes, manifest.windowSetup, manifest.bookmarks, manifest.highlighting };
                var filenames = new string[]
                { 
                    BackupRestoreConstants.PersistantObjectsDisplaySettingsFileName, 
                    BackupRestoreConstants.PersistantObjectsThemesFileName, 
                    BackupRestoreConstants.PersistantObjectsWindowsFileName, 
                    BackupRestoreConstants.PersistantObjectsMarkersFileName, 
                    BackupRestoreConstants.PersistantObjectsHighlightFileName
                };

                var fileTransferList = new List<FileTransferItem>();
                for (int i = 0; i < settings.Length; i++)
                {
                    if (settings[i])
                    {
                        fileTransferList.Add(new FileTransferItem(ApplicationData.Current.LocalFolder, filenames[i], putInFolderId));
                    }
                }

                progressCallback(15, 0, false, null, null, null, null);
                if (manifest.bibles)
                {
                    await PutFolderSimulation(tempSharedTransfers, ApplicationData.Current.LocalFolder, "mods.d", putInFolderId, fileTransferList);
                    if (_isCanceled) { progressCallback(100, 100, true,null, null, null, null); return; };
                    await PutFolderSimulation(tempSharedTransfers, ApplicationData.Current.LocalFolder, "modules", putInFolderId, fileTransferList);
                    if (_isCanceled) { progressCallback(100, 100, true, null, null, null, null); return; };
                }

                this.oneDriveProgressBarTotal = 20;
                progressCallback(this.oneDriveProgressBarTotal, 0, false, null, null, null, null);
                _progressIncrement = 75.0 / (double)fileTransferList.Count();
                foreach (var file in fileTransferList)
                {
                    // try 3 times to get the file.
                    int i = 0;
                    Exception error = null;
                    for (i = 0; i < 3; i++)
                    {
                        try
                        {
                            error = null;
                            await PutFileInFolder(tempSharedTransfers, file.folder, file.filename, file.idFolder);
                            this.oneDriveProgressBarTotal += _progressIncrement;
                            progressCallback(this.oneDriveProgressBarTotal, 0, false, null, null, null, null);
                            if (_isCanceled) { progressCallback(100, 100, true, null, null, null, null); return; };
                            // it went ok. Lets get out
                            break;
                        }
                        catch (Exception e)
                        {
                            // we probably need to wait here for things to cool off.
                            System.Threading.Thread.Sleep(2500);
                            error = e;
                        }
                    }
                    if (i == 3 && error != null)
                    {
                        throw error;
                    }
                }

                this.oneDriveProgressBarTotal = 95;
                progressCallback(this.oneDriveProgressBarTotal, 0, false, null, null, null, null);
                if (_isCanceled) { progressCallback(100, 100, true, null, null, null, null); return; }; 
                await Hoot.File.WriteAllBytes(BackupManifestFileName, Encoding.UTF8.GetBytes(Serialize(manifest)));
                await PutFileInFolder(tempSharedTransfers, ApplicationData.Current.LocalFolder, BackupManifestFileName, putInFolderId);
            }
            catch(Exception e)
            {
                progressCallback(100, 100, true, null, Uri.UnescapeDataString(e.Message), null, null);
                return;
            }
            progressCallback(100, 100, true,null, null, null, null);
        }
        private System.Threading.CancellationToken ctsUpload;

        private async Task<string> CreateFolder(string folder, string idContainingFolder)
        {
            //create the id
            var folderData = new Dictionary<string, object>();
            folderData.Add("name", folder);
            LiveOperationResult operationResult = await liveClient.PostAsync(idContainingFolder, folderData);
            dynamic result = operationResult.Result;
            return result.id;
        }

        private async Task PutFolderSimulation(StorageFolder tempSharedTransfers, StorageFolder rootFolder, string folder, string idContainingFolder, List<FileTransferItem> fileTransferList)
        {
            var idFolder = await CreateFolder(folder, idContainingFolder);
            var storageFolder = await rootFolder.GetFolderAsync(folder);
            var files = await Hoot.File.GetFiles(folder, rootFolder);
            foreach (var file in files)
            {
                fileTransferList.Add(new FileTransferItem(storageFolder, file, idFolder));
            }
            var containedFolders = await Hoot.File.GetFolders(folder, rootFolder);
            foreach (var containedFolder in containedFolders)
            {
                await PutFolderSimulation(tempSharedTransfers, storageFolder, containedFolder, idFolder, fileTransferList);
            }
        }

        //private async Task PutFolder(StorageFolder tempSharedTransfers, StorageFolder rootFolder, string folder, string idContainingFolder)
        //{
        //    var idFolder = await CreateFolder(folder, idContainingFolder);
        //    var storageFolder = await rootFolder.GetFolderAsync(folder);
        //    var files = await Hoot.File.GetFiles(folder, rootFolder);
        //    foreach (var file in files)
        //    {
        //        await PutFileInFolder(tempSharedTransfers, storageFolder, file, idFolder);
        //        this.oneDriveProgressBarTotal += _progressIncrement;
        //        progressCallback(this.oneDriveProgressBarTotal, 0, false,null, null, null, null);
        //    }
        //    var containedFolders = await Hoot.File.GetFolders(folder, rootFolder);
        //    foreach (var containedFolder in containedFolders)
        //    {
        //        await PutFolder(tempSharedTransfers, storageFolder, containedFolder, idFolder);
        //    }
        //}

        private async Task<string> PutFileInFolder(StorageFolder transferFolder, StorageFolder folder, string filename, string idFolder)
        {
            var progressHandler = new Progress<LiveOperationProgress>(
            (progress) =>
            {
                progressCallback(this.oneDriveProgressBarTotal, progress.ProgressPercentage, false, null, null, null, null);
            });
            var ctsDownload = new System.Threading.CancellationToken();
            using (var stream = await folder.OpenStreamForReadAsync(filename))
            {
                var uploadOperationResult = await liveClient.UploadAsync(idFolder, filename, stream,OverwriteOption.Overwrite,ctsDownload, progressHandler);
                dynamic result = uploadOperationResult.Result;
            }
            return string.Empty;
        }

        private async Task<Dictionary<string, string>[]> GetFolderContents(string url)
        {
            var rootFiles = new Dictionary<string, string>();
            var rootFoldersFiles = new Dictionary<string, string>();
            var allFiles = await this.liveClient.GetAsync(url);
            dynamic result = allFiles.Result;
            foreach (dynamic pairKeyValue in result)
            {
                foreach (IDictionary<string, object> dictionary in pairKeyValue.Value)
                {
                    if (((string)dictionary["type"]).Equals("folder"))
                    {
                        rootFoldersFiles[(string)dictionary["name"]] = (string)dictionary["id"];
                    }
                    else
                    {
                        rootFiles[(string)dictionary["name"]] = (string)dictionary["id"];
                    }
                }
            }

            return new Dictionary<string, string>[] { rootFiles, rootFoldersFiles };
        }

        private const string RootCrossConnectId = "RootCrossConnectIdxxyyzz";
        private async Task<Dictionary<string, string>[]> GetRootContents(string crossConnectFolder)
        {
            string putInFolderId = string.Empty;
            var rootFiles = new Dictionary<string, string>();
            var allFiles = await this.liveClient.GetAsync("me/skydrive/files");
            dynamic result = allFiles.Result;
            foreach (dynamic pairKeyValue in result)
            {
                foreach (IDictionary<string, object> dictionary in pairKeyValue.Value)
                {
                    rootFiles[(string)dictionary["name"]] = (string)dictionary["id"];
                }
            }



            if (!rootFiles.TryGetValue(crossConnectFolder, out putInFolderId))
            {
                //create the id
                putInFolderId = await CreateFolder(crossConnectFolder, "me/skydrive");
            }

            // get contents of folder
            var rootContents = await GetFolderContents(putInFolderId + "/files");
            rootContents[0][RootCrossConnectId] = putInFolderId;

            return rootContents;
        }

        System.Threading.CancellationTokenSource ctsDownload;
        private async Task GetFileRemote(string remoteFileId, string filename, StorageFolder localFolder, StorageFolder transferFolder)
        {
            var progressHandler = new Progress<LiveOperationProgress>(
                (progress) =>
                {
                    progressCallback(this.oneDriveProgressBarTotal, progress.ProgressPercentage, false, null, null, null, null);
                });
            var ctsDownload = new System.Threading.CancellationToken();
            var downloadOperationResult = await liveClient.DownloadAsync(remoteFileId + "/Content", ctsDownload, progressHandler);
            using (Stream downloadStream = downloadOperationResult.Stream)
            {
                if (downloadStream != null)
                {
                    await Hoot.File.Delete(filename, localFolder);
                    using (var stream = await localFolder.OpenStreamForWriteAsync(filename, CreationCollisionOption.ReplaceExisting))
                    {
                        downloadStream.CopyTo(stream);
                        downloadStream.Flush();
                        downloadStream.Close();
                    }
                }
            }
        }

        //private async Task GetRemoteFolder(StorageFolder tempSharedTransfers, StorageFolder rootFolder, string folder, string idRemoteFolder)
        //{
        //    var storageFolder = await rootFolder.CreateFolderAsync(folder);
        //    var files = await GetFolderContents(idRemoteFolder + "/files" );
        //    foreach (var file in files[0])
        //    {
        //        await GetFileRemote(file.Value, file.Key, storageFolder, tempSharedTransfers);
        //        this.oneDriveProgressBarTotal += _progressIncrement;
        //        progressCallback(this.oneDriveProgressBarTotal, 0, false,null, null, null, null);
        //    }
        //    foreach (var folderIdInRemote in files[1])
        //    {
        //        await GetRemoteFolder(tempSharedTransfers, storageFolder, folderIdInRemote.Key, folderIdInRemote.Value);
        //    }
        //}


        private async Task GetRemoteFolderSimulation(StorageFolder tempSharedTransfers, StorageFolder rootFolder, string folder, string idRemoteFolder, List<FileTransferItem> fileTransferList)
        {
            var storageFolder = await rootFolder.CreateFolderAsync(folder);
            var files = await GetFolderContents(idRemoteFolder + "/files");
            this.oneDriveProgressBarTotal += _progressIncrement;
            progressCallback(this.oneDriveProgressBarTotal, 0, false, null, null, null, null);
            foreach (var file in files[0])
            {
                fileTransferList.Add(new FileTransferItem(storageFolder, file.Key, file.Value));
            }
            foreach (var folderIdInRemote in files[1])
            {
                await GetRemoteFolderSimulation(tempSharedTransfers, storageFolder, folderIdInRemote.Key, folderIdInRemote.Value, fileTransferList);
            }
        }

        private const string TempImportFolderName = "XyzXXTempImportFolderZZ";
        public async void DoImport(BackupRestore.BackupManifest manifestSelected, string crossConnectFolder, BackupRestoreConstants.Progress progressCallback)
        {
            this.progressCallback = progressCallback;
            try
            {
                var tempSharedTransfers = await GetTransferFolder();
                var rootContents = await GetRootContents(crossConnectFolder);
                progressCallback(6, 0, false,null, null, null, null);
                if (_isCanceled) { progressCallback(100, 100, true,null, null, null, null); return; };
                string remoteCrossConnectFolderId = rootContents[0][RootCrossConnectId];

                string foundManifestFileId;
                if (!rootContents[0].TryGetValue(BackupManifestFileName, out foundManifestFileId))
                {
                    progressCallback(100, 0, true, null, null, "There is no backup to restore", null);
                    return;
                }

                string modsFolderId;
                if(rootContents[1].TryGetValue("mods.d",out modsFolderId))
                {
                    var modsContents = await GetFolderContents(modsFolderId + "/files");
                    _progressIncrement = 80.0 / ((double)modsContents[0].Count() * 7.0 + 5.0);
                }
                progressCallback(8, 0, false,null, null, null, null);
                if (_isCanceled) { progressCallback(100, 100, true,null, null, null, null); return; };
                await GetFileRemote(foundManifestFileId, BackupManifestFileName, ApplicationData.Current.LocalFolder, tempSharedTransfers);
                var manifestBytes = await Hoot.File.ReadAllBytes(BackupManifestFileName);
                BackupRestore.BackupManifest manifest = (BackupRestore.BackupManifest)Deserialize(manifestBytes, typeof(BackupRestore.BackupManifest));
                progressCallback(10, 0, false,null, null, null, null);
                if (_isCanceled) { progressCallback(100, 100, true,null, null, null, null); return; };
                await Hoot.File.Delete(BackupManifestFileName);
                List<ManifestCheck> manifestChecker = new List<ManifestCheck> 
                { 
                    new ManifestCheck(manifest.bibles, manifestSelected.bibles, "Bibles",null),
                    new ManifestCheck(manifest.settings, manifestSelected.settings, "Settings",BackupRestoreConstants.PersistantObjectsDisplaySettingsFileName),
                    new ManifestCheck(manifest.highlighting, manifestSelected.highlighting, "Highlighting",BackupRestoreConstants.PersistantObjectsHighlightFileName),
                    new ManifestCheck(manifest.bookmarks, manifestSelected.bookmarks, "Bookmarks and custom notes",BackupRestoreConstants.PersistantObjectsMarkersFileName),
                    new ManifestCheck(manifest.themes, manifestSelected.themes, "Themes",BackupRestoreConstants.PersistantObjectsThemesFileName),
                    new ManifestCheck(manifest.windowSetup, manifestSelected.windowSetup, "Window setup", BackupRestoreConstants.PersistantObjectsWindowsFileName),
                };

                foreach (var check in manifestChecker)
                {
                    if (check.RequestValue && !check.ManifestValue)
                    {
                        progressCallback(100, 0, true, null, null, check.Message, "Doesn't exist in OneDrive. Remove it and try again");
                        return;
                    }
                }

                progressCallback(12, 0, false,null, null, null, null);
                if (_isCanceled) { progressCallback(100, 100, true,null, null, null, null); return; };

                // Create tempFolder but delete it first.
                if (await Hoot.File.Exists(TempImportFolderName))
                {
                    var deleteFolder = await ApplicationData.Current.LocalFolder.GetFolderAsync(TempImportFolderName);
                    await deleteFolder.DeleteAsync();
                }

                var tempImportFolder = await ApplicationData.Current.LocalFolder.CreateFolderAsync(TempImportFolderName);
                if (_isCanceled) { progressCallback(100, 100, true,null, null, null, null); return; };

                var fileTransferList = new List<FileTransferItem>();

                _progressIncrement = 0.5;
                this.oneDriveProgressBarTotal = 10.0;
                foreach (var file in manifestChecker)
                {
                    if (file.RequestValue)
                    {
                        if (!string.IsNullOrEmpty(file.fileName))
                        {
                            fileTransferList.Add(new FileTransferItem(tempImportFolder, file.fileName, rootContents[0][file.fileName]));
                        }
                        else
                        {
                            await GetRemoteFolderSimulation(tempSharedTransfers, tempImportFolder, "mods.d", rootContents[1]["mods.d"], fileTransferList);
                            if (_isCanceled) { progressCallback(100, 100, true, null, null, null, null); return; };
                            await GetRemoteFolderSimulation(tempSharedTransfers, tempImportFolder, "modules", rootContents[1]["modules"], fileTransferList);
                        }
                        
                        if (_isCanceled) { progressCallback(100, 100, true, null, null, null, null); return; };
                    }
                }

                this.oneDriveProgressBarTotal = 15.0;
                _progressIncrement = 80.0 / (double)fileTransferList.Count();
                foreach (var file in fileTransferList)
                {
                    // try 3 times to get the file.
                    int i = 0;
                    Exception error = null;
                    for (i = 0; i < 3; i++)
                    {
                        try
                        {
                            error = null;
                            await GetFileRemote(file.idFolder, file.filename, file.folder, tempSharedTransfers);
                            this.oneDriveProgressBarTotal += _progressIncrement;
                            progressCallback(this.oneDriveProgressBarTotal, 0, false, null, null, null, null);
                            // it went ok. Lets get out
                            break;
                        }
                        catch (Exception e)
                        {
                            // we probably need to wait here for things to cool off.
                            System.Threading.Thread.Sleep(2500);
                            error = e;
                        }
                    }
                    if (i == 3 && error!=null)
                    {
                        throw error;
                    }
                }

                this.oneDriveProgressBarTotal = 95;
                progressCallback(this.oneDriveProgressBarTotal, 0, false,null, null, null, null);
                if (_isCanceled) { progressCallback(100, 100, true,null, null, null, null); return; };

                // move the temporary folder to the actual folder
                await MoveFolder(tempImportFolder, ApplicationData.Current.LocalFolder);
            }
            catch (Exception ee)
            {
                progressCallback(100, 100, true, null, Uri.UnescapeDataString(ee.Message), null, null);
                return;
            }

            progressCallback(100, 100, true,null, null, null, null);
        }

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
                if(await Hoot.File.Exists(toFolder, folder.Name))
                {
                    await ( await toFolder.GetFolderAsync(folder.Name)).DeleteAsync();
                }

                var childFolder = await toFolder.CreateFolderAsync(folder.Name);
                await MoveFolder(folder, childFolder);
            }
        }

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
                XmlDictionaryReader reader = XmlDictionaryReader.CreateTextReader(memoryStream, new XmlDictionaryReaderQuotas());
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
    }
}
