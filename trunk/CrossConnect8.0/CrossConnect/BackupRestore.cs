using Microsoft.Live;
using Microsoft.Phone.BackgroundTransfer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Windows.Storage;

namespace CrossConnect
{
    public class BackupRestore
    {
        #region Methods

        public bool IsCanceled = false;
        private bool _isConnected = false;
        public bool IsConnected
        {
            get
            {
                return _isConnected;
            }
        }

        double _progressIncrement = 0;
        public const string PersistantObjectsWindowsFileName = "_Windows.xml";
        public const string PersistantObjectsThemesFileName = "_Themes.xml";
        public const string PersistantObjectsDisplaySettingsFileName = "_DisplaySettings.xml";
        public const string PersistantObjectsMarkersFileName = "_Markers.xml";
        public const string PersistantObjectsHighlightFileName = "_Highlights.xml";
        private readonly string[] _defaultAuthScopes = new string[] { "wl.signin", "wl.skydrive", "wl.skydrive_update" };
        private LiveConnectClient liveClient = null;


        public async Task<string> AuthenticateUser()
        {
            string text = null;
            string firstName = string.Empty;
            bool connected = false;
            try
            {
                var authClient = new LiveAuthClient("00000000400E8489");
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
        public delegate void Progress(double percentTotal, double percentPartial, bool IsFinal, string Message, string MessageTranslateable1, string MessageTranslateable2);
        Progress progressCallback;
        double oneDriveProgressBarTotal = 0;
        public async Task DoExport(BackupManifest manifest, string crossConnectFolder, Progress progressCallback)
        {
            this.progressCallback = progressCallback;
            try
            {
                if (IsCanceled) { progressCallback(100, 100, true, null, null, null); return; };
                progressCallback(5, 0, false, null, null, null);
                var files = await Hoot.File.GetFiles("mods.d", ApplicationData.Current.LocalFolder);
                if (files.Length != 0)
                {
                    _progressIncrement = 80.0 / ((double)files.Length * 7.0 + 5.0);
                }

                if (IsCanceled) { progressCallback(100, 100, true, null, null, null); return; };
                progressCallback(7, 0, false, null, null, null);
                var rootContents = await GetRootContents(crossConnectFolder);
                string putInFolderId = rootContents[0][RootCrossConnectId];

                if (IsCanceled) { progressCallback(100, 100, true, null, null, null); return; };
                progressCallback(9, 0, false, null, null, null);

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
                progressCallback(11, 0, false, null, null, null);
                this.oneDriveProgressBarTotal = 11;
                if (IsCanceled) { progressCallback(100, 100, true, null, null, null); return; };

                // put all files
                if (manifest.settings)
                {
                    await PutFileInFolder(tempSharedTransfers, ApplicationData.Current.LocalFolder, PersistantObjectsDisplaySettingsFileName, putInFolderId);
                }
                this.oneDriveProgressBarTotal += _progressIncrement;
                progressCallback(this.oneDriveProgressBarTotal, 0, false, null, null, null);
                if (IsCanceled) { progressCallback(100, 100, true, null, null, null); return; };
                if (manifest.themes)
                {
                    await PutFileInFolder(tempSharedTransfers, ApplicationData.Current.LocalFolder, PersistantObjectsThemesFileName, putInFolderId);
                }
                this.oneDriveProgressBarTotal += _progressIncrement;
                progressCallback(this.oneDriveProgressBarTotal, 0, false, null, null, null);
                if (IsCanceled) { progressCallback(100, 100, true, null, null, null); return; };
                if (manifest.windowSetup)
                {
                    await PutFileInFolder(tempSharedTransfers, ApplicationData.Current.LocalFolder, PersistantObjectsWindowsFileName, putInFolderId);
                }
                this.oneDriveProgressBarTotal += _progressIncrement;
                progressCallback(this.oneDriveProgressBarTotal, 0, false, null, null, null);
                if (IsCanceled) { progressCallback(100, 100, true, null, null, null); return; };
                if (manifest.bookmarks)
                {
                    await PutFileInFolder(tempSharedTransfers, ApplicationData.Current.LocalFolder, PersistantObjectsMarkersFileName, putInFolderId);
                }
                this.oneDriveProgressBarTotal += _progressIncrement;
                progressCallback(this.oneDriveProgressBarTotal, 0, false, null, null, null);
                if (IsCanceled) { progressCallback(100, 100, true, null, null, null); return; };
                if (manifest.highlighting)
                {
                    await PutFileInFolder(tempSharedTransfers, ApplicationData.Current.LocalFolder, PersistantObjectsHighlightFileName, putInFolderId);
                }
                this.oneDriveProgressBarTotal += _progressIncrement;
                progressCallback(this.oneDriveProgressBarTotal, 0, false, null, null, null);
                if (IsCanceled) { progressCallback(100, 100, true, null, null, null); return; };

                if (manifest.bibles)
                {
                    await PutFolder(tempSharedTransfers, ApplicationData.Current.LocalFolder, "mods.d", putInFolderId);
                    if (IsCanceled) { progressCallback(100, 100, true, null, null, null); return; };
                    await PutFolder(tempSharedTransfers, ApplicationData.Current.LocalFolder, "modules", putInFolderId);
                }
                this.oneDriveProgressBarTotal = 95;
                progressCallback(this.oneDriveProgressBarTotal, 0, false, null, null, null);
                if (IsCanceled) { progressCallback(100, 100, true, null, null, null); return; };

                await Hoot.File.WriteAllBytes(BackupManifestFileName, Encoding.UTF8.GetBytes(Serialize(manifest)));
                await PutFileInFolder(tempSharedTransfers, ApplicationData.Current.LocalFolder, BackupManifestFileName, putInFolderId);
            }
            catch(Exception e)
            {
                progressCallback(100, 100, true, Uri.UnescapeDataString(e.Message), null, null);
            }
            progressCallback(100, 100, true, null, null, null);
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

        private async Task PutFolder(StorageFolder tempSharedTransfers, StorageFolder rootFolder, string folder, string idContainingFolder)
        {
            var idFolder = await CreateFolder(folder, idContainingFolder);
            var storageFolder = await rootFolder.GetFolderAsync(folder);
            var files = await Hoot.File.GetFiles(folder, rootFolder);
            foreach (var file in files)
            {
                await PutFileInFolder(tempSharedTransfers, storageFolder, file, idFolder);
                this.oneDriveProgressBarTotal += _progressIncrement;
                progressCallback(this.oneDriveProgressBarTotal, 0, false, null, null, null);
            }
            var containedFolders = await Hoot.File.GetFolders(folder, rootFolder);
            foreach (var containedFolder in containedFolders)
            {
                await PutFolder(tempSharedTransfers, storageFolder, containedFolder, idFolder);
            }
        }

        private async Task<string> PutFileInFolder(StorageFolder transferFolder, StorageFolder folder, string filename, string idFolder)
        {
            // Prepare for download, make sure there are no previous requests
            var reqList = BackgroundTransferService.Requests.ToList();
            foreach (var req in reqList)
            {
                if (req.DownloadLocation.Equals(new Uri(@"\shared\transfers\" + filename, UriKind.Relative)))
                {
                    BackgroundTransferService.Remove(BackgroundTransferService.Find(req.RequestId));
                }
            }
            if (await Hoot.File.Exists(folder, filename))
            {
                var fileObj = await folder.GetFileAsync(filename);
                var fileToDelete = await fileObj.CopyAsync(transferFolder, filename, NameCollisionOption.ReplaceExisting);

                var progressHandler = new Progress<LiveOperationProgress>(
                    (progress) => 
                    {
                        progressCallback(this.oneDriveProgressBarTotal, progress.ProgressPercentage, false, null, null, null);
                    });
                this.ctsUpload = new System.Threading.CancellationToken();
                var operationResult = await liveClient.BackgroundUploadAsync(idFolder, new Uri("/shared/transfers/" + filename, UriKind.Relative), OverwriteOption.Overwrite, this.ctsUpload, progressHandler);
                dynamic result = operationResult.Result;

                await fileToDelete.DeleteAsync();
                return result.id;
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
            // Prepare for download, make sure there are no previous requests
            var reqList = BackgroundTransferService.Requests.ToList();
            foreach (var req in reqList)
            {
                if (req.DownloadLocation.Equals(new Uri(@"\shared\transfers\" + filename, UriKind.Relative)))
                {
                    BackgroundTransferService.Remove(BackgroundTransferService.Find(req.RequestId));
                }
            }

            var progressHandler = new Progress<LiveOperationProgress>(
                (progress) =>
                {
                    progressCallback(this.oneDriveProgressBarTotal, progress.ProgressPercentage, false, null, null, null);
                });
            this.ctsDownload = new System.Threading.CancellationTokenSource();
            await liveClient.BackgroundDownloadAsync(remoteFileId + "/Content", new Uri("/shared/transfers/" + filename, UriKind.Relative), this.ctsDownload.Token, progressHandler);
            await Hoot.File.Delete(filename, localFolder);
            var fileObj = await transferFolder.GetFileAsync(filename);
            await fileObj.MoveAsync(localFolder);
        }

        private async Task GetRemoteFolder(StorageFolder tempSharedTransfers, StorageFolder rootFolder, string folder, string idRemoteFolder)
        {
            var storageFolder = await rootFolder.CreateFolderAsync(folder);
            var files = await GetFolderContents(idRemoteFolder + "/files" );
            foreach (var file in files[0])
            {
                await GetFileRemote(file.Value, file.Key, storageFolder, tempSharedTransfers);
                this.oneDriveProgressBarTotal += _progressIncrement;
                progressCallback(this.oneDriveProgressBarTotal, 0, false, null, null, null);
            }
            foreach (var folderIdInRemote in files[1])
            {
                await GetRemoteFolder(tempSharedTransfers, storageFolder, folderIdInRemote.Key, folderIdInRemote.Value);
            }
        }

        private const string TempImportFolderName = "XyzXXTempImportFolderZZ";
        public async void DoImport(BackupManifest manifestSelected, string crossConnectFolder, Progress progressCallback)
        {
            this.progressCallback = progressCallback;
            try
            {
                var tempSharedTransfers = await GetTransferFolder();
                var rootContents = await GetRootContents(crossConnectFolder);
                progressCallback(6, 0, false, null, null, null);
                if (IsCanceled) { progressCallback(100, 100, true, null, null, null); return; };
                string remoteCrossConnectFolderId = rootContents[0][RootCrossConnectId];

                string foundManifestFileId;
                if (!rootContents[0].TryGetValue(BackupManifestFileName, out foundManifestFileId))
                {
                    progressCallback(100, 0, true, null, "There is no backup to restore", null);
                    return;
                }

                string modsFolderId;
                if(rootContents[1].TryGetValue("mods.d",out modsFolderId))
                {
                    var modsContents = await GetFolderContents(modsFolderId + "/files");
                    _progressIncrement = 80.0 / ((double)modsContents[0].Count() * 7.0 + 5.0);
                }
                progressCallback(8, 0, false, null, null, null);
                if (IsCanceled) { progressCallback(100, 100, true, null, null, null); return; };
                await GetFileRemote(foundManifestFileId, BackupManifestFileName, ApplicationData.Current.LocalFolder, tempSharedTransfers);
                var manifestBytes = await Hoot.File.ReadAllBytes(BackupManifestFileName);
                BackupManifest manifest = (BackupManifest)Deserialize(manifestBytes, typeof(BackupManifest));
                progressCallback(10, 0, false, null, null, null);
                if (IsCanceled) { progressCallback(100, 100, true, null, null, null); return; };
                await Hoot.File.Delete(BackupManifestFileName);
                List<ManifestCheck> manifestChecker = new List<ManifestCheck> 
                { 
                    new ManifestCheck(manifest.bibles, manifestSelected.bibles, "Bibles"),
                    new ManifestCheck(manifest.settings, manifestSelected.settings, "Settings"),
                    new ManifestCheck(manifest.highlighting, manifestSelected.highlighting, "Highlighting"),
                    new ManifestCheck(manifest.bookmarks, manifestSelected.bookmarks, "Bookmarks and custom notes"),
                    new ManifestCheck(manifest.themes, manifestSelected.themes, "Themes"),
                    new ManifestCheck(manifest.windowSetup, manifestSelected.windowSetup, "Window setup"),
                };

                foreach (var check in manifestChecker)
                {
                    if (check.RequestValue && !check.ManifestValue)
                    {
                        progressCallback(100, 0, true, null, check.Message, "Doesn't exist in OneDrive. Remove it and try again");
                        return;
                    }
                }
                this.oneDriveProgressBarTotal = 12;
                progressCallback(this.oneDriveProgressBarTotal, 0, false, null, null, null);
                if (IsCanceled) { progressCallback(100, 100, true, null, null, null); return; };

                // Create tempFolder but delete it first.
                if (await Hoot.File.Exists(TempImportFolderName))
                {
                    var deleteFolder = await ApplicationData.Current.LocalFolder.GetFolderAsync(TempImportFolderName);
                    await deleteFolder.DeleteAsync();
                }

                var tempImportFolder = await ApplicationData.Current.LocalFolder.CreateFolderAsync(TempImportFolderName);
                if (IsCanceled) { progressCallback(100, 100, true, null, null, null); return; };

                // put all files
                if (manifestSelected.settings)
                {
                    await GetFileRemote(rootContents[0][PersistantObjectsDisplaySettingsFileName], PersistantObjectsDisplaySettingsFileName, tempImportFolder, tempSharedTransfers);
                }
                this.oneDriveProgressBarTotal += _progressIncrement;
                progressCallback(this.oneDriveProgressBarTotal, 0, false, null, null, null);
                if (IsCanceled) { progressCallback(100, 100, true, null, null, null); return; };
                if (manifestSelected.themes)
                {
                    await GetFileRemote(rootContents[0][PersistantObjectsThemesFileName], PersistantObjectsThemesFileName, tempImportFolder, tempSharedTransfers);
                }
                this.oneDriveProgressBarTotal += _progressIncrement;
                progressCallback(this.oneDriveProgressBarTotal, 0, false, null, null, null);
                if (IsCanceled) { progressCallback(100, 100, true, null, null, null); return; };
                if (manifestSelected.windowSetup)
                {
                    await GetFileRemote(rootContents[0][PersistantObjectsWindowsFileName], PersistantObjectsWindowsFileName, tempImportFolder, tempSharedTransfers);
                }
                this.oneDriveProgressBarTotal += _progressIncrement;
                progressCallback(this.oneDriveProgressBarTotal, 0, false, null, null, null);
                if (IsCanceled) { progressCallback(100, 100, true, null, null, null); return; };
                if (manifestSelected.bookmarks)
                {
                    await GetFileRemote(rootContents[0][PersistantObjectsMarkersFileName], PersistantObjectsMarkersFileName, tempImportFolder, tempSharedTransfers);
                }
                this.oneDriveProgressBarTotal += _progressIncrement;
                progressCallback(this.oneDriveProgressBarTotal, 0, false, null, null, null);
                if (IsCanceled) { progressCallback(100, 100, true, null, null, null); return; };
                if (manifestSelected.highlighting)
                {
                    await GetFileRemote(rootContents[0][PersistantObjectsHighlightFileName], PersistantObjectsHighlightFileName, tempImportFolder, tempSharedTransfers);
                }
                this.oneDriveProgressBarTotal += _progressIncrement;
                progressCallback(this.oneDriveProgressBarTotal, 0, false, null, null, null);
                if (IsCanceled) { progressCallback(100, 100, true, null, null, null); return; };

                if (manifestSelected.bibles && rootContents[1].ContainsKey("mods.d") && rootContents[1].ContainsKey("modules"))
                {
                    await GetRemoteFolder(tempSharedTransfers, tempImportFolder, "mods.d", rootContents[1]["mods.d"]);
                    if (IsCanceled) { progressCallback(100, 100, true, null, null, null); return; };
                    await GetRemoteFolder(tempSharedTransfers, tempImportFolder, "modules", rootContents[1]["modules"]);
                }
                this.oneDriveProgressBarTotal = 95;
                progressCallback(this.oneDriveProgressBarTotal, 0, false, null, null, null);
                if (IsCanceled) { progressCallback(100, 100, true, null, null, null); return; };

                // move the temporary folder to the actual folder
                await MoveFolder(tempImportFolder, ApplicationData.Current.LocalFolder);

            }
            catch (Exception ee)
            {
                progressCallback(100, 100, true, Uri.UnescapeDataString(ee.Message), null, null);
            }

            progressCallback(100, 100, true, null, null, null);
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
            public string Message;
            public ManifestCheck(bool ManifestValue, bool RequestValue, string Message)
            {
                this.ManifestValue = ManifestValue;
                this.RequestValue = RequestValue;
                this.Message = Message;
            }
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
