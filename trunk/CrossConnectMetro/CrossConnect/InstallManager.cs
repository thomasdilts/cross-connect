#region Header

// <copyright file="InstallManager.cs" company="Thomas Dilts">
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
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.IO;
    using System.IO.Compression;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Xml;

    using ICSharpCode.SharpZipLib.Tar;

    using Sword;
    using Sword.javaprops;
    using Sword.reader;

    using Windows.Storage;
    using Windows.Storage.Streams;

    public class InstallManager
    {
        #region Fields

        private BibleListReturned _callbackListRetrieved;

        private WebClient _client = new WebClient();

        private Dictionary<string, WebInstaller> _installers = new Dictionary<string, WebInstaller>();

        #endregion

        #region Constructors and Destructors

        public InstallManager()
        {
            var sitemap = new JavaProperties("InstallManager.plugin", false);

            foreach (string site in sitemap.Values)
            {
                string[] parts = site.Split(",".ToCharArray());
                string name = parts[1];
                string host = parts[2];
                string packageDirectory = parts[3];
                string catalogDirectory = parts[4];
                this._installers[name] = new WebInstaller(host, packageDirectory, catalogDirectory);
            }

            this.AddCustomDownloadLink(this._installers);
        }

        #endregion

        #region Delegates

        public delegate void BibleListReturned(Dictionary<string, WebInstaller> installers, string message);

        #endregion

        #region Public Properties

        public Dictionary<string, WebInstaller> Installers
        {
            get
            {
                return this._installers;
            }
        }

        #endregion

        #region Public Methods and Operators

        public void GetBibleDownloadList(BibleListReturned callbackListRetrieved)
        {
            this._callbackListRetrieved = callbackListRetrieved;
            string url =
                string.Format(
                    "http://www.cross-connect.se/bibles/getserverlist.php?uuid={0}&language={1}&version={2}",
                    App.DisplaySettings.UserUniqueGuuid,
                    Translations.IsoLanguageCode,
                    App.Version);
            try
            {
                this._client = new WebClient();
                Logger.Debug("download start");
                this._client.StartDownload(url, this.ClientOpenReadCompleted, null);
                Logger.Debug("DownloadStringAsync returned");
            }
            catch (Exception eee)
            {
                this._callbackListRetrieved(this._installers, string.Empty);
                Logger.Fail(eee.ToString());
            }
        }

        #endregion

        #region Methods

        private void AddCustomDownloadLink(Dictionary<string, WebInstaller> installers)
        {
            if (!string.IsNullOrEmpty(App.DisplaySettings.CustomBibleDownloadLinks))
            {
                string[] parts = App.DisplaySettings.CustomBibleDownloadLinks.Split(new[] { ',' });
                if (parts.Length == 3 && Uri.IsWellFormedUriString("http://" + parts[0].Trim(), UriKind.Absolute)
                    && Uri.IsWellFormedUriString("http://" + parts[0].Trim() + parts[1].Trim(), UriKind.Absolute)
                    && Uri.IsWellFormedUriString("http://" + parts[0].Trim() + parts[2].Trim(), UriKind.Absolute))
                {
                    installers[Translations.Translate("Custom bible download addresses")] =
                        new WebInstaller(parts[0].Trim(), parts[1].Trim(), parts[2].Trim());
                }
            }
        }

        private async void ClientOpenReadCompleted(string error)
        {
            var installers = new Dictionary<string, WebInstaller>();

            if (!string.IsNullOrEmpty(error))
            {
                Logger.Fail(error);
                this._callbackListRetrieved(this._installers, string.Empty);
                return;
            }

            string msgFromServer;
            try
            {
                // for debug
                // byte[] buffer=new byte[e.Result.Length];
                // e.Result.Read(buffer, 0, (int)e.Result.Length);
                // System.Diagnostics.Debug.WriteLine("RawFile: " + System.Text.UTF8Encoding.UTF8.GetString(buffer, 0, (int)e.Result.Length));
                IRandomAccessStream file = await this._client.downloadedFile.OpenAsync(FileAccessMode.Read);
                Stream stream = file.AsStreamForRead();
                using (XmlReader reader = XmlReader.Create(stream))
                {
                    string name = string.Empty;
                    msgFromServer = string.Empty;
                    string tmpConfig = string.Empty;
                    string tmpRaw = string.Empty;
                    string tmpHost = string.Empty;
                    // Parse the file and get each of the nodes.
                    while (reader.Read())
                    {
                        switch (reader.NodeType)
                        {
                            case XmlNodeType.Element:
                                if (reader.Name.ToLower().Equals("source") && reader.HasAttributes)
                                {
                                    tmpConfig = string.Empty;
                                    tmpRaw = string.Empty;
                                    tmpHost = string.Empty;
                                    name = string.Empty;
                                    reader.MoveToFirstAttribute();
                                    do
                                    {
                                        switch (reader.Name.ToLower())
                                        {
                                            case "server":
                                                tmpHost = reader.Value;
                                                break;
                                            case "config":
                                                tmpConfig = reader.Value;
                                                break;
                                            case "raw":
                                                tmpRaw = reader.Value;
                                                break;
                                        }
                                    }
                                    while (reader.MoveToNextAttribute());
                                }
                                else if (reader.Name.ToLower().Equals("message"))
                                {
                                    name = string.Empty;
                                }

                                break;
                            case XmlNodeType.EndElement:
                                if (reader.Name.ToLower().Equals("source"))
                                {
                                    installers[name] = new WebInstaller(tmpHost, tmpConfig, tmpRaw);
                                    name = string.Empty;
                                }
                                else if (reader.Name.ToLower().Equals("message"))
                                {
                                    msgFromServer = name;
                                    name = string.Empty;
                                }

                                break;
                            case XmlNodeType.Text:
                                name += reader.Value;
                                break;
                        }
                    }
                }
            }
            catch (Exception exp)
            {
                Logger.Fail(exp.ToString());
                this._callbackListRetrieved(this._installers, string.Empty);
                this._client.RemoveTempFile();
                return;
            }
            this._client.RemoveTempFile();
            this.AddCustomDownloadLink(installers);
            this._installers = installers;
            this._callbackListRetrieved(installers, msgFromServer);
        }

        #endregion
    }

    public static class Logger
    {
        #region Public Methods and Operators

        public static void Debug(string message)
        {
            System.Diagnostics.Debug.WriteLine(DateTime.Now + " " + message);
        }

        public static void Fail(string message)
        {
            System.Diagnostics.Debug.WriteLine(DateTime.Now + " " + message);
        }

        public static void Warn(string message)
        {
            System.Diagnostics.Debug.WriteLine(DateTime.Now + " " + message);
        }

        #endregion
    }

    public class SwordBook
    {
        #region Constants

        private const string ZipSuffix = ".zip";

        #endregion

        #region Fields

        public bool IsLoaded;

        public SwordBookMetaData Sbmd;

        private WebClient _client = new WebClient();

        #endregion

        #region Constructors and Destructors

        public SwordBook(string internalName)
        {
            this.IsLoaded = false;
        }

        public SwordBook(byte[] buffer, string bookName)
        {
            this.Sbmd = new SwordBookMetaData(buffer, bookName);
            this.IsLoaded = true;
        }

        #endregion

        #region Public Events

        public event WebInstaller.OpenReadCompletedEventHandler ProgressCompleted;

        public event WebInstaller.DownloadProgressChangedEventHandler ProgressUpdate;

        #endregion

        #region Public Properties

        public string Name
        {
            get
            {
                return this.Sbmd.Name;
            }
        }

        #endregion

        #region Public Methods and Operators

        public static async Task<bool> MakeSurePathExists(string path)
        {
            StorageFolder folder = ApplicationData.Current.LocalFolder;
            string[] directories = path.Split("/\\".ToCharArray());
            if (directories.Length > 1)
            {
                for (int i = 0; i < (directories.Length - 1); i++)
                {
                    IReadOnlyList<StorageFolder> folds = await folder.CreateFolderQuery().GetFoldersAsync();
                    if (!folds.Any(p => p.Name.Equals(directories[i])))
                    {
                        folder = await folder.CreateFolderAsync(directories[i]);
                    }
                    else
                    {
                        folder = await folder.GetFolderAsync(directories[i]);
                    }
                }
            }

            return true;
        }

        public async Task<bool> DoLoading(string internalName)
        {
            this.IsLoaded = false;
            if (string.IsNullOrEmpty(internalName))
            {
                return false;
            }
            try
            {
                StorageFolder folder = ApplicationData.Current.LocalFolder;
                string filepath = BibleZtextReader.DirConf + '/' + internalName.ToLower()
                                  + BibleZtextReader.ExtensionConf;
                StorageFile file = await folder.GetFileAsync(filepath.Replace("/", "\\"));
                IRandomAccessStream istream = await file.OpenAsync(FileAccessMode.Read);
                Stream stream = istream.AsStreamForRead();
                this.Sbmd = new SwordBookMetaData(stream, internalName);
                this.IsLoaded = true;
            }
            catch (Exception ee)
            {
                Debug.WriteLine(ee.StackTrace);
            }

            return true;
        }

        public string DownloadBookNow(WebInstaller iManager)
        {
            try
            {
                string pathToHost = "http://" + iManager.Host + iManager.PackageDirectory + "/" + this.Sbmd.Initials
                                    + ZipSuffix;
                this._client = new WebClient();
                this._client.StartDownload(pathToHost, this.ClientOpenReadCompleted, this.ClientDownloadProgressChanged);
                Logger.Debug("download start");
                Logger.Debug("DownloadStringAsync returned");
                return null;
            }
            catch (Exception e)
            {
                Logger.Fail(e.ToString());
                return e.Message;
            }
        }

        public async void RemoveBible()
        {
            try
            {
                string modFile = BibleZtextReader.DirConf + '/' + this.Sbmd.InternalName.ToLower()
                                 + BibleZtextReader.ExtensionConf;
                string bookPath = this.Sbmd.GetCetProperty(ConfigEntryType.ADataPath).ToString().Substring(2);
                StorageFolder bookFolder =
                    await ApplicationData.Current.LocalFolder.GetFolderAsync(bookPath.Replace("/", "\\"));
                IReadOnlyList<StorageFile> bookFiles = await bookFolder.GetFilesAsync();

                try
                {
                    StorageFile file = await ApplicationData.Current.LocalFolder.GetFileAsync(modFile);
                    await file.DeleteAsync();
                }
                catch (Exception)
                {
                }

                foreach (var file in bookFiles)
                {
                        try
                        {
                            StorageFile deletefile = await bookFolder.GetFileAsync(file.Name);
                            await deletefile.DeleteAsync();
                        }
                        catch (Exception)
                        {
                        }
                }

                if (this.Sbmd.GetCetProperty(ConfigEntryType.ModDrv).Equals("RawGenBook"))
                {
                    // In a book, the main files are one searchway down.
                    var mainDir = Path.GetDirectoryName(bookPath.Replace("/", "\\") + ".idx");
                    bookFolder =
                        await
                        ApplicationData.Current.LocalFolder.GetFolderAsync(mainDir);
                    bookFiles = await bookFolder.GetFilesAsync();
                    foreach (var file in bookFiles)
                    {
                        try
                        {
                            StorageFile deletefile = await bookFolder.GetFileAsync(file.Name);
                            await deletefile.DeleteAsync();
                        }
                        catch (Exception)
                        {
                        }
                    }
                }

                try
                {
                    StorageFolder file =
                        await ApplicationData.Current.LocalFolder.GetFolderAsync(bookPath.Replace("/", "\\"));
                    await file.DeleteAsync();
                }
                catch (Exception)
                {
                }
            }
            catch (Exception e3)
            {
                // many things can go wrong here. It is no danger to leave the bible in the rare case that this does not work.
                Debug.WriteLine(e3);
            }
        }

        #endregion

        #region Methods

        private void ClientDownloadProgressChanged(byte percent)
        {
            if (this.ProgressUpdate != null)
            {
                this.ProgressUpdate(percent);
            }
        }

        private async void ClientOpenReadCompleted(string error)
        {
            if (!string.IsNullOrEmpty(error))
            {
                if (this.ProgressCompleted != null)
                {
                    this.ProgressCompleted(error);
                }
            }

            try
            {
                ZipArchive zipStream;
                try
                {
                    IRandomAccessStream file = await this._client.downloadedFile.OpenAsync(FileAccessMode.Read);
                    Stream stream = file.AsStreamForRead();
                    zipStream = new ZipArchive(stream);
                }
                catch (Exception ee)
                {
                    if (this.ProgressCompleted != null)
                    {
                        this.ProgressCompleted(ee.Message);
                    }

                    return;
                }

                StorageFolder isolatedStorageRoot = ApplicationData.Current.LocalFolder;
                ReadOnlyCollection<ZipArchiveEntry> entries = zipStream.Entries;
                foreach (ZipArchiveEntry zipArchiveEntry in entries)
                {
                    // make sure it is not just a directory
                    if (!zipArchiveEntry.FullName.EndsWith("\\") && !zipArchiveEntry.FullName.EndsWith("/") && zipArchiveEntry.Length!=0)
                    {
                        bool dummy = await MakeSurePathExists(zipArchiveEntry.FullName);

                        StorageFile file =
                            await
                            isolatedStorageRoot.CreateFileAsync(
                                zipArchiveEntry.FullName.Replace("/", "\\"), CreationCollisionOption.ReplaceExisting);
                        Stream stream = await file.OpenStreamForWriteAsync();
                        var buffer = new byte[10000];
                        int len;
                        Stream zstream = zipArchiveEntry.Open();

                        while ((len = zstream.Read(buffer, 0, buffer.GetUpperBound(0))) > 0)
                        {
                            stream.Write(buffer, 0, len);
                        }

                        zstream.Dispose();
                        stream.Dispose();
                    }
                }

                if (this.ProgressCompleted != null)
                {
                    this.ProgressCompleted(string.Empty);
                }
            }
            catch (Exception exp)
            {
                if (this.ProgressCompleted != null)
                {
                    this.ProgressCompleted(exp.Message);
                }
            }

            this._client.RemoveTempFile();
        }

        #endregion
    }

    public class WebInstaller
    {
        #region Constants

        /// <summary>
        ///     * The sword index file
        /// </summary>
        private const string FileListGz = "mods.d.tar.gz";

        #endregion

        #region Fields

        /// <summary>
        ///     * A map of the books in this download area
        /// </summary>
        public readonly Dictionary<string, SwordBook> Entries = new Dictionary<string, SwordBook>();

        /// <summary>
        ///     * The remote hostname.
        /// </summary>
        public readonly string Host;

        /// <summary>
        ///     * The directory containing zipped books on the <code>host</code>.
        /// </summary>
        protected internal readonly string PackageDirectory = string.Empty;

        /// <summary>
        ///     The _books.
        /// </summary>
        private readonly Dictionary<string, SwordBook> _books = new Dictionary<string, SwordBook>();

        /// <summary>
        ///     * The directory containing the catalog of all books on the
        ///     <code>host</code>.
        /// </summary>
        private readonly string _catalogDirectory = string.Empty;

        /// <summary>
        ///     The _client.
        /// </summary>
        private WebClient _client = new WebClient();

        /// <summary>
        ///     * Do we need to reload the index file
        /// </summary>
        private bool _loaded;

        /// <summary>
        ///     The _result stream.
        /// </summary>
        private WebClient _resultStream;

        #endregion

        #region Constructors and Destructors

        public WebInstaller(string host, string packageDirectory, string catalogDirectory)
        {
            this.Host = host;
            this.PackageDirectory = packageDirectory;
            this._catalogDirectory = catalogDirectory;
        }

        #endregion

        #region Delegates

        public delegate void DownloadProgressChangedEventHandler(byte percent);

        public delegate void OpenReadCompletedEventHandler(string error);

        #endregion

        #region Public Events

        public event OpenReadCompletedEventHandler ProgressCompleted;

        public event DownloadProgressChangedEventHandler ProgressUpdate;

        #endregion

        #region Public Properties

        public bool IsLoaded
        {
            get
            {
                return this._loaded;
            }
        }

        #endregion

        #region Public Methods and Operators

        public string ReloadBookList()
        {
            this._books.Clear();
            string uri = "http://" + this.Host + "/" + this._catalogDirectory + "/" + FileListGz;
            string errMsg = this.Download(uri);
            if (errMsg != null)
            {
                this._loaded = false;
                return errMsg;
            }

            this._loaded = true;
            return null;
        }

        public async void UnzipBookList()
        {
            GZipStream gzip;
            Stream stream;

            try
            {
                IRandomAccessStream file = await this._resultStream.downloadedFile.OpenAsync(FileAccessMode.Read);
                stream = file.AsStreamForRead();
                gzip = new GZipStream(stream, CompressionMode.Decompress);
            }
            catch (Exception ee)
            {
                if (this.ProgressCompleted != null)
                {
                    this.ProgressCompleted("Error = " + ee.Message);
                }
                if (this._resultStream != null)
                {
                    this._resultStream.RemoveTempFile();
                }
                this._loaded = false;
                return;
            }

            var tin = new TarInputStream(gzip);

            // long streamLength = e.Result.Length;
            // long streamCounter = 0;
            this.Entries.Clear();
            while (true)
            {
                TarEntry entry = tin.GetNextEntry();
                if (entry == null)
                {
                    break;
                }

                this.ProgressUpdate((byte)(50.0 + (stream.Position * 50.0 / stream.Length)));
                string @internal = entry.Name;
                if (!entry.IsDirectory)
                {
                    try
                    {
                        var size = (int)entry.Size;

                        // Every now and then an empty entry sneaks in
                        if (size == 0)
                        {
                            Logger.Fail("Empty entry: " + @internal);
                            continue;
                        }

                        var buffer = new byte[size];
                        var ms = new MemoryStream(buffer) { Position = 0 };
                        tin.CopyEntryContents(ms);
                        if (ms.Position != size)
                        {
                            // This should not happen, but if it does then skip
                            // it.
                            Logger.Fail("Did not read all that was expected " + @internal);
                            continue;
                        }

                        if (@internal.EndsWith(BibleZtextReader.ExtensionConf))
                        {
                            @internal = @internal.Substring(0, @internal.Length - 5);
                        }
                        else
                        {
                            Logger.Fail("Not a SWORD config file: " + @internal);
                            continue;
                        }

                        if (@internal.StartsWith(BibleZtextReader.DirConf + '/'))
                        {
                            @internal = @internal.Substring(7);
                        }

                        // sbmd.Driver = fake;
                        var book = new SwordBook(buffer, @internal);
                        this.Entries.Add(book.Name, book);
                    }
                    catch (Exception exp)
                    {
                        Logger.Fail("Failed to load config for entry: " + @internal);
                        this._loaded = true;
                        if (this.ProgressCompleted != null)
                        {
                            this.ProgressCompleted("Error = " + exp.Message);
                        }

                        stream.Dispose();
                        this._resultStream.RemoveTempFile();

                        return;
                    }
                }
            }

            this._loaded = true;
            if (this.ProgressCompleted != null)
            {
                this.ProgressCompleted(string.Empty);
            }

            stream.Dispose();
            this._resultStream.RemoveTempFile();
        }

        #endregion

        #region Methods

        private void ClientDownloadProgressChanged(byte percent)
        {
            if (this.ProgressUpdate != null)
            {
                this.ProgressUpdate((byte)(percent / 2.0));
            }
        }

        private void ClientOpenReadCompleted(string error)
        {
            if (!string.IsNullOrEmpty(error))
            {
                if (this.ProgressCompleted != null)
                {
                    this.ProgressCompleted(error);
                }

                return;
            }

            this._resultStream = this._client;
            if (this.ProgressCompleted != null)
            {
                this.ProgressCompleted(string.Empty);
            }
        }

        private string Download(string source)
        {
            try
            {
                this._client = new WebClient();
                Logger.Debug("download start");

                // client.DownloadStringAsync(source);
                this._client.StartDownload(source, this.ClientOpenReadCompleted, this.ClientDownloadProgressChanged);
                Logger.Debug("DownloadStringAsync returned");
                return null;
            }
            catch (Exception e)
            {
                Logger.Fail(e.ToString());
                return e.Message;
            }
        }

        #endregion
    }
}