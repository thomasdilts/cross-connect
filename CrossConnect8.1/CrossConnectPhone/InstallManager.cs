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
    using System.Diagnostics;
    using System.IO;
    using System.IO.IsolatedStorage;
    using System.Net;
    using System.Text;
    using System.Windows;
    using System.Xml;
    using System.Linq;

    using ICSharpCode.SharpZipLib.GZip;
    using ICSharpCode.SharpZipLib.Tar;
    using ICSharpCode.SharpZipLib.Zip;

    using Sword;
    using Sword.javaprops;
    using Sword.reader;
    using Microsoft.Phone.Storage;
    using System.Threading.Tasks;
    using Windows.Storage;

    

    public class InstallManager
    {
        #region Fields

        private BibleListReturned _callbackListRetrieved;
        private WebClient _client = new WebClient();
        private Dictionary<string, IWebInstaller> _installers = new Dictionary<string, IWebInstaller>();

        #endregion Fields

        #region Constructors

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
                _installers[name] = new WebInstaller(host, packageDirectory, catalogDirectory);
            }

            AddCustomDownloadLink(_installers);
        }

        #endregion Constructors

        #region Delegates

        public delegate void BibleListReturned(Dictionary<string, IWebInstaller> installers, string message);

        #endregion Delegates

        #region Properties

        public Dictionary<string, IWebInstaller> Installers
        {
            get
            {
                return _installers;
            }
        }

        #endregion Properties

        #region Methods

        public void GetBibleDownloadList(BibleListReturned callbackListRetrieved)
        {
            _callbackListRetrieved = callbackListRetrieved;
            string url =
                string.Format(
                    "http://www.cross-connect.se/bibles/getserverlist.php?uuid={0}&language={1}&version={2}",
                    App.DisplaySettings.UserUniqueGuuid,
                    Translations.IsoLanguageCode,
                    App.Version);
            try
            {
                var source = new Uri(url);

                _client = new WebClient();
                _client.OpenReadCompleted += ClientOpenReadCompleted;
                Logger.Debug("download start");
                _client.OpenReadAsync(source);
                Logger.Debug("DownloadStringAsync returned");
            }
            catch (Exception eee)
            {
                _callbackListRetrieved(_installers, string.Empty);
                Logger.Fail(eee.ToString());
            }
        }

        private async Task AddCustomDownloadLink(Dictionary<string, IWebInstaller> installers)
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
            if(await SdCardInstaller.HasSdCardFiles())
            {
                installers[Translations.Translate("Memory card")] =
                        new SdCardInstaller();
            }
        }

        private async void ClientOpenReadCompleted(object sender, OpenReadCompletedEventArgs e)
        {
            var installers = new Dictionary<string, IWebInstaller>();
            string msgFromServer;
            try
            {
                // for debug
                // byte[] buffer=new byte[e.Result.Length];
                // e.Result.Read(buffer, 0, (int)e.Result.Length);
                // System.Diagnostics.Debug.WriteLine("RawFile: " + System.Text.UTF8Encoding.UTF8.GetString(buffer, 0, (int)e.Result.Length));

                using (XmlReader reader = XmlReader.Create(e.Result))
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
                Logger.Fail(e.ToString());
                Logger.Fail(exp.ToString());
                _callbackListRetrieved(_installers, string.Empty);
                return;
            }

            await AddCustomDownloadLink(installers);
            _installers = installers;
            _callbackListRetrieved(installers, msgFromServer);
        }

        #endregion Methods
    }

    public static class Logger
    {
        #region Methods

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

        #endregion Methods
    }

    public class SwordBook
    {
        #region Fields

        public readonly bool IsLoaded;
        public readonly SwordBookMetaData Sbmd;

        public const string ZipSuffix = ".zip";

        private WebClient _client = new WebClient();

        #endregion Fields

        #region Constructors

        public SwordBook(string internalName)
        {
            IsLoaded = false;
            try
            {
                IsolatedStorageFile root = IsolatedStorageFile.GetUserStoreForApplication();
                string filepath = BibleZtextReader.DirConf + '/' + internalName.ToLower()
                                  + BibleZtextReader.ExtensionConf;
                IsolatedStorageFileStream stream =
                    root.OpenFile(
                        filepath,
                        FileMode.Open);
                Sbmd = new SwordBookMetaData(stream, internalName);
                IsLoaded = true;
            }
            catch (Exception ee)
            {
                Debug.WriteLine(ee.StackTrace);
            }
        }

        public SwordBook(Stream stream, string bookName)
        {
            Sbmd = new SwordBookMetaData(stream, bookName);
            IsLoaded = true;
        }
        public SwordBook(byte[] buffer, string bookName)
        {
            Sbmd = new SwordBookMetaData(buffer, bookName);
            IsLoaded = true;
        }

        #endregion Constructors

        #region Events


        #endregion Events

        #region Properties

        public string Name
        {
            get
            {
                return Sbmd.Name;
            }
        }

        #endregion Properties

        /// <summary>
        /// This may be a totally bogus file or a file with various errors.
        /// Therefore there is maximal testing for a valid Sword format.
        /// </summary>
        /// <returns>Error text or empty string if no errors.</returns>
        public static async Task<string[]> TryInstallBibleFromZipFile(StorageFile zipFile)
        {
            string ModFilePath = string.Empty;
            var allowedDrivers = new HashSet<string> { "ZCOM", "RAWCOM", "RAWGENBOOK", "ZTEXT", "RAWTEXT", "ZLD", "RAWLD", "RAWLD4" };
            try
            {
                ZipInputStream zipStream;
                Stream stream;
                try
                {
                    stream = await zipFile.OpenStreamForReadAsync();
                    //Stream stream = file.AsStreamForRead();
                    zipStream = new ZipInputStream(stream);
                }
                catch (Exception ee)
                {
                    return new string[] { "Cannot open the file as a zip stream:" + ee.Message };
                }

                //get all config files and list of all files.
                //ReadOnlyCollection<ZipEntry> entries = zipStream..Entries;
                var foundPathToFiles = string.Empty;
                var allNonConfigFiles = new List<string>();
                ZipEntry entry;
                while ((entry = zipStream.GetNextEntry())!=null)
                {
                    if (entry.Name == "")
                    {
                        // Probably an empty Folder
                    }
                    else if (entry.Name.EndsWith(".conf") && entry.Name.StartsWith("mods.d/"))
                    {
                        // File

                        var buff = new byte[10000]; //it is impossible that this file is over 10000 bytes

                        var bytes = zipStream.Read(buff, 0, buff.Length);
                        Array.Resize(ref buff, bytes);
                        try
                        {
                            var book = new SwordBook(buff, entry.Name);
                            var driver = ((string)book.Sbmd.GetProperty(ConfigEntryType.ModDrv)).ToUpper();
                            if (!allowedDrivers.Contains(driver))
                            {
                                return new string[] { string.Format("Not an allowed type of sword file='{0}' ; Allowed types = {1}", driver, allowedDrivers.Aggregate((i, j) => i + ", " + j)) };
                            }
                            foundPathToFiles = book.Sbmd.GetCetProperty(ConfigEntryType.ADataPath).ToString().Substring(2);
                            if (!foundPathToFiles.StartsWith("modules/"))
                            {
                                return new string[] { string.Format("Something wrong in config file with the path to the files='{0}' ; should begin with './modules/'", foundPathToFiles) };
                            }
                            ModFilePath = Path.GetFileNameWithoutExtension(entry.Name);
                        }
                        catch (Exception eee)
                        {

                            return new string[] { string.Format("Cannot read config file='{0}': error = {1}", entry.Name, eee.Message) };
                        }
                        
                    }
                    else
                    {
                        allNonConfigFiles.Add(entry.Name);
                    }
                }

                if (string.IsNullOrEmpty(ModFilePath))
                {
                    return new string[] { "Cannot find a config file in the expected directory=mods.d" };
                }

                if (!allNonConfigFiles.Any(p => p.StartsWith(foundPathToFiles)))
                {
                    return new string[] { "Cannot find any files in the expected path=" + foundPathToFiles };
                }

                // looks good. Lets put it in our database.
                stream = await zipFile.OpenStreamForReadAsync();
                zipStream = new ZipInputStream(stream);

                IsolatedStorageFile isolatedStorageRoot = IsolatedStorageFile.GetUserStoreForApplication();
                while (true)
                {
                    entry = zipStream.GetNextEntry();
                    if (entry == null)
                    {
                        break;
                    }

                    string entrypath = entry.Name;
                    if (entry.IsDirectory)
                    {
                        if (entry.Name.StartsWith("/"))
                        {
                            entrypath = entry.Name.Substring(1);
                        }

                        if (entry.Name.EndsWith("/"))
                        {
                            entrypath = entry.Name.Substring(0, entry.Name.Length - 1);
                        }

                        isolatedStorageRoot.CreateDirectory(entrypath);
                    }
                    else
                    {
                        WebInstaller.MakeSurePathExists(isolatedStorageRoot, entry.Name);
                        IsolatedStorageFileStream fStream = isolatedStorageRoot.CreateFile(entry.Name);
                        var buffer = new byte[10000];
                        int len;
                        while ((len = zipStream.Read(buffer, 0, buffer.GetUpperBound(0))) > 0)
                        {
                            fStream.Write(buffer, 0, len);
                        }

                        fStream.Close();
                    }
                }
                zipStream.Dispose();

            }
            catch (Exception exp)
            {
                return new string[] { "Unexpected error while examining the zip file=" + exp.Message };
            }

            return new string[] { string.Empty, ModFilePath };
        }


    }

    public interface IWebInstaller
    {
        Dictionary<string, SwordBook> Entries { get;}
        Task<string> DownloadBookNow(SwordBookMetaData sbmd, DownloadProgressChangedEventHandler ProgressChanged, OpenReadCompletedEventHandler ProgressCompleted);
        Task<string> ReloadBookList(DownloadProgressChangedEventHandler ProgressChanged, OpenReadCompletedEventHandler ProgressCompleted);
        void UnzipBookList(DownloadProgressChangedEventHandler ProgressChanged, OpenReadCompletedEventHandler ProgressCompleted);
        bool IsLoaded{get;}
    }

    public class SdCardInstaller : IWebInstaller
    {
        public static async Task<bool> HasSdCardFiles()
        {
            ExternalStorageDevice _sdCard = (await ExternalStorage.GetExternalStorageDevicesAsync()).FirstOrDefault();

            // If the SD card is present, add GPX files to the Routes collection.
            if (_sdCard != null)
            {
                try
                {
                    // Look for a folder on the SD card named Routes.
                    ExternalStorageFolder routesFolder = await _sdCard.GetFolderAsync(CROSSCONNECT_INSTALL_SD_CARD_DIR);

                    // Get all files from the Routes folder.
                    IEnumerable<ExternalStorageFile> routeFiles = await routesFolder.GetFilesAsync();

                    // Add each GPX file to the Routes collection.
                    foreach (ExternalStorageFile esf in routeFiles)
                    {
                        if (esf.Path.EndsWith(".conf"))
                        {
                            return true;
                        }
                    }
                }
                catch (FileNotFoundException)
                {
                    // No Routes folder is present.
                    return false;
                }
            }

            return false;
        }
        public readonly Dictionary<string, SwordBook> _Entries = new Dictionary<string, SwordBook>();
        public Dictionary<string, SwordBook> Entries { get { return _Entries; } }
        public async Task<string> DownloadBookNow(SwordBookMetaData sbmd, DownloadProgressChangedEventHandler ProgressChanged, OpenReadCompletedEventHandler ProgressCompleted)
        {            // Connect to the current SD card.
            ExternalStorageDevice _sdCard = (await ExternalStorage.GetExternalStorageDevicesAsync()).FirstOrDefault();

            // If the SD card is present, add GPX files to the Routes collection.
            if (_sdCard != null)
            {
                try
                {
                    // Look for a folder on the SD card named Routes.
                    ExternalStorageFolder routesFolder = await _sdCard.GetFolderAsync(CROSSCONNECT_INSTALL_SD_CARD_DIR);

                    // Get all files from the Routes folder.
                    IEnumerable<ExternalStorageFile> routeFiles = await routesFolder.GetFilesAsync();

                    // Add each GPX file to the Routes collection.
                    foreach (ExternalStorageFile esf in routeFiles)
                    {
                        var filename = sbmd.Initials + ".czip";
                        if (esf.Name.ToLower().Equals(filename.ToLower()))
                        {
                            string winRtPath = "D:\\" + esf.Path;
                            FileStream s = new System.IO.FileStream(winRtPath, FileMode.Open, FileAccess.Read);
                            var tempWebInst = new WebInstaller(null, null, null);
                            tempWebInst.ClientDownloadBookCompleted(s, null);
                            Deployment.Current.Dispatcher.BeginInvoke(() => ProgressCompleted(null, null));
                            return null;
                        }
                    }
                }
                catch (FileNotFoundException)
                {
                    // No Routes folder is present.
                    Deployment.Current.Dispatcher.BeginInvoke(() => ProgressCompleted(null, null));
                    return "Directory missing on memory card" + " " + CROSSCONNECT_INSTALL_SD_CARD_DIR;
                }
            }
            Deployment.Current.Dispatcher.BeginInvoke(() => ProgressCompleted(null, null));
            return "couldn't find " + sbmd.Initials + ".czip";
        }
        private const string CROSSCONNECT_INSTALL_SD_CARD_DIR = "CrossConnectInstall";
        public async Task<string> ReloadBookList(DownloadProgressChangedEventHandler ProgressChanged, OpenReadCompletedEventHandler ProgressCompleted)
        {
            Entries.Clear();
            _loaded = false;
            // Connect to the current SD card.
            ExternalStorageDevice _sdCard = (await ExternalStorage.GetExternalStorageDevicesAsync()).FirstOrDefault();

            // If the SD card is present, add GPX files to the Routes collection.
            if (_sdCard != null)
            {
                try
                {
                    // Look for a folder on the SD card named Routes.
                    ExternalStorageFolder routesFolder = await _sdCard.GetFolderAsync(CROSSCONNECT_INSTALL_SD_CARD_DIR);

                    // Get all files from the Routes folder.
                    IEnumerable<ExternalStorageFile> routeFiles = await routesFolder.GetFilesAsync();

                    // Add each GPX file to the Routes collection.
                    foreach (ExternalStorageFile esf in routeFiles)
                    {
                        if (esf.Path.EndsWith(".conf"))
                        {
                            string winRtPath = "D:\\" + esf.Path;
                            FileStream s = new System.IO.FileStream(winRtPath, FileMode.Open, FileAccess.Read);
                            var book = new SwordBook(s, Path.GetFileNameWithoutExtension(esf.Name));
                            Entries[book.Name] = book;
                        }
                    }

                    _loaded = true;
                }
                catch (FileNotFoundException)
                {
                    ProgressCompleted(null, null);
                    // No Routes folder is present.
                    return "Directory missing on memory card" + " " + CROSSCONNECT_INSTALL_SD_CARD_DIR;
                }
            }

            ProgressCompleted(null, null);
            return null;
        }
        public void UnzipBookList(DownloadProgressChangedEventHandler ProgressChanged, OpenReadCompletedEventHandler ProgressCompleted)
        {
            Deployment.Current.Dispatcher.BeginInvoke(() => ProgressCompleted(null, null));
        }
        private bool _loaded = false;
        public bool IsLoaded { get { return _loaded; } }
    }
    
    public class WebInstaller:IWebInstaller
    {
        #region Fields

        /// <summary>
        ///   * A map of the books in this download area
        /// </summary>
        public readonly Dictionary<string, SwordBook> _Entries = new Dictionary<string, SwordBook>();
        public Dictionary<string, SwordBook> Entries { get { return _Entries; } }

        /// <summary>
        ///   * The remote hostname.
        /// </summary>
        public readonly string Host;

        /// <summary>
        ///   * The directory containing zipped books on the <code>host</code>.
        /// </summary>
        protected internal readonly string PackageDirectory = string.Empty;

        /// <summary>
        ///   * The sword index file
        /// </summary>
        private const string FileListGz = "mods.d.tar.gz";

        /// <summary>
        /// The _books.
        /// </summary>
        private readonly Dictionary<string, SwordBook> _books = new Dictionary<string, SwordBook>();

        /// <summary>
        ///   * The directory containing the catalog of all books on the
        ///   <code>host</code>.
        /// </summary>
        private readonly string _catalogDirectory = string.Empty;

        /// <summary>
        /// The _client.
        /// </summary>
        private WebClient _client = new WebClient();

        /// <summary>
        ///   * Do we need to reload the index file
        /// </summary>
        private bool _loaded;

        /// <summary>
        /// The _result stream.
        /// </summary>
        private Stream _resultStream;

        #endregion Fields

        #region Constructors

        public WebInstaller(string host, string packageDirectory, string catalogDirectory)
        {
            Host = host;
            PackageDirectory = packageDirectory;
            _catalogDirectory = catalogDirectory;
        }

        #endregion Constructors

        #region Events

        public event OpenReadCompletedEventHandler ProgressCompleted;

        public event DownloadProgressChangedEventHandler ProgressUpdate;

        #endregion Events

        #region Properties

        public bool IsLoaded
        {
            get
            {
                return _loaded;
            }
        }

        #endregion Properties

        #region Methods

        public async Task<string> ReloadBookList(DownloadProgressChangedEventHandler ProgressChanged, OpenReadCompletedEventHandler ProgressCompleted)
        {
            this.ProgressCompleted = ProgressCompleted;
            this.ProgressUpdate = ProgressChanged;

            _books.Clear();
            var uri = new Uri("http://" + Host + "/" + _catalogDirectory + "/" + FileListGz);
            string errMsg = Download(uri);
            if (errMsg != null)
            {
                _loaded = false;
                return errMsg;
            }

            _loaded = true;
            return null;
        }

        public void UnzipBookList(DownloadProgressChangedEventHandler ProgressChanged, OpenReadCompletedEventHandler ProgressCompleted)
        {
            this.ProgressCompleted = ProgressCompleted;
            this.ProgressUpdate = ProgressChanged;

            GZipInputStream gzip;
            try
            {
                gzip = new GZipInputStream(_resultStream);
            }
            catch (Exception)
            {
                if (ProgressCompleted != null)
                {
                    ProgressCompleted(null, null);
                }

                _loaded = false;
                return;
            }

            var tin = new TarInputStream(gzip);

            // long streamLength = e.Result.Length;
            // long streamCounter = 0;
            Entries.Clear();
            while (true)
            {
                TarEntry entry = tin.GetNextEntry();
                if (entry == null)
                {
                    break;
                }

                Deployment.Current.Dispatcher.BeginInvoke(
                    () =>
                    ProgressUpdate(50.0 + (_resultStream.Position * 50.0 / _resultStream.Length), null));
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
                        Entries[book.Name] = book;
                    }
                    catch (Exception exp)
                    {
                        Logger.Fail("Failed to load config for entry: " + @internal);
                        _loaded = true;
                        if (ProgressCompleted != null)
                        {
                            Deployment.Current.Dispatcher.BeginInvoke(() => ProgressCompleted(exp.Message, null));
                        }

                        return;
                    }
                }
            }

            _loaded = true;
            if (ProgressCompleted != null)
            {
                Deployment.Current.Dispatcher.BeginInvoke(() => ProgressCompleted(null, null));
            }

            _resultStream.Close();
            _resultStream.Dispose();
        }

        private void ClientDownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            if (ProgressUpdate != null)
            {
                ProgressUpdate(e.ProgressPercentage / 2.0, e);
            }
        }

        private void ClientOpenReadCompleted(object sender, OpenReadCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                if (ProgressCompleted != null)
                {
                    ProgressCompleted(e.Error.Message, null);
                }

                return;
            }

            _resultStream = e.Result;
            if (ProgressCompleted != null)
            {
                ProgressCompleted(null, null);
            }
        }

        private string Download(Uri source)
        {
            try
            {
                _client = new WebClient { Encoding = Encoding.BigEndianUnicode };
                _client.DownloadProgressChanged += ClientDownloadProgressChanged;
                _client.OpenReadCompleted += ClientOpenReadCompleted;
                Logger.Debug("download start");

                // client.DownloadStringAsync(source);
                _client.OpenReadAsync(source);
                Logger.Debug("DownloadStringAsync returned");
                return null;
            }
            catch (Exception e)
            {
                Logger.Fail(e.ToString());
                return e.Message;
            }
        }

        public async Task<string> DownloadBookNow(SwordBookMetaData sbmd, DownloadProgressChangedEventHandler ProgressChanged, OpenReadCompletedEventHandler ProgressCompleted)
        {
            try
            {
                string pathToHost = "http://" + this.Host + this.PackageDirectory + "/" + sbmd.Initials
                                    + SwordBook.ZipSuffix;
                var source = new Uri(pathToHost);
                this.ProgressUpdate = ProgressChanged;
                this.ProgressCompleted = ProgressCompleted;

                _client = new WebClient();
                _client.DownloadProgressChanged += ClientDownloadBookProgressChanged;
                _client.OpenReadCompleted += ClientDownloadBookCompleted;
                Logger.Debug("download start");
                _client.OpenReadAsync(source);
                Logger.Debug("DownloadStringAsync returned");
                return null;
            }
            catch (Exception e)
            {
                Logger.Fail(e.ToString());
                return e.Message;
            }
        }
        public static void MakeSurePathExists(IsolatedStorageFile isolatedStorageRoot, string path)
        {
            string[] directories = path.Split("/".ToCharArray());
            string totalTestPath = string.Empty;
            if (directories.Length > 1)
            {
                for (int i = 0; i < (directories.Length - 1); i++)
                {
                    if (totalTestPath.Length > 0)
                    {
                        totalTestPath += "/";
                    }

                    totalTestPath += directories[i];
                    if (!isolatedStorageRoot.DirectoryExists(totalTestPath))
                    {
                        isolatedStorageRoot.CreateDirectory(totalTestPath);
                    }
                }
            }
        }

        private void ClientDownloadBookProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            if (ProgressUpdate != null)
            {
                ProgressUpdate(sender, e);
            }
        }

        public void ClientDownloadBookCompleted(object sender, OpenReadCompletedEventArgs e)
        {
            try
            {
                ZipInputStream zipStream;
                try
                {
                    zipStream = new ZipInputStream(sender is Stream?(Stream)sender:e.Result);
                }
                catch (Exception)
                {
                    if (ProgressCompleted != null)
                    {
                        ProgressCompleted(e.Error.Message, e);
                    }

                    return;
                }

                IsolatedStorageFile isolatedStorageRoot = IsolatedStorageFile.GetUserStoreForApplication();
                while (true)
                {
                    ZipEntry entry = zipStream.GetNextEntry();
                    if (entry == null)
                    {
                        break;
                    }

                    string entrypath = entry.Name;
                    if (entry.IsDirectory)
                    {
                        if (entry.Name.StartsWith("/"))
                        {
                            entrypath = entry.Name.Substring(1);
                        }

                        if (entry.Name.EndsWith("/"))
                        {
                            entrypath = entry.Name.Substring(0, entry.Name.Length - 1);
                        }

                        isolatedStorageRoot.CreateDirectory(entrypath);
                    }
                    else
                    {
                        MakeSurePathExists(isolatedStorageRoot, entry.Name);
                        IsolatedStorageFileStream fStream = isolatedStorageRoot.CreateFile(entry.Name);
                        var buffer = new byte[10000];
                        int len;
                        while ((len = zipStream.Read(buffer, 0, buffer.GetUpperBound(0))) > 0)
                        {
                            fStream.Write(buffer, 0, len);
                        }

                        fStream.Close();
                    }
                }

                if (ProgressCompleted != null)
                {
                    ProgressCompleted(null, e);
                }
            }
            catch (Exception exp)
            {
                if (ProgressCompleted != null)
                {
                    ProgressCompleted(exp.Message, e);
                }
            }
        }


        #endregion Methods
    }
}