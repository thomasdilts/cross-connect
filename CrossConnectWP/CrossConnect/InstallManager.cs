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

    using ICSharpCode.SharpZipLib.GZip;
    using ICSharpCode.SharpZipLib.Tar;
    using ICSharpCode.SharpZipLib.Zip;

    using Sword;
    using Sword.javaprops;
    using Sword.reader;

    

    public class InstallManager
    {
        #region Fields

        private BibleListReturned _callbackListRetrieved;
        private WebClient _client = new WebClient();
        private Dictionary<string, WebInstaller> _installers = new Dictionary<string, WebInstaller>();

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

        public delegate void BibleListReturned(Dictionary<string, WebInstaller> installers, string message);

        #endregion Delegates

        #region Properties

        public Dictionary<string, WebInstaller> Installers
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

        private void ClientOpenReadCompleted(object sender, OpenReadCompletedEventArgs e)
        {
            var installers = new Dictionary<string, WebInstaller>();
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

            AddCustomDownloadLink(installers);
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

        private const string ZipSuffix = ".zip";

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

        public SwordBook(byte[] buffer, string bookName)
        {
            Sbmd = new SwordBookMetaData(buffer, bookName);
            IsLoaded = true;
        }

        #endregion Constructors

        #region Events

        public event OpenReadCompletedEventHandler ProgressCompleted;

        public event DownloadProgressChangedEventHandler ProgressUpdate;

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

        #region Methods

        public string DownloadBookNow(WebInstaller iManager)
        {
            try
            {
                string pathToHost = "http://" + iManager.Host + iManager.PackageDirectory + "/" + Sbmd.Initials
                                    + ZipSuffix;
                var source = new Uri(pathToHost);

                _client = new WebClient();
                _client.DownloadProgressChanged += ClientDownloadProgressChanged;
                _client.OpenReadCompleted += ClientOpenReadCompleted;
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

        public void RemoveBible()
        {
            try
            {
                string modFile = BibleZtextReader.DirConf + '/' + this.Sbmd.InternalName.ToLower()
                                 + BibleZtextReader.ExtensionConf;
                string bookPath = this.Sbmd.GetCetProperty(ConfigEntryType.ADataPath).ToString().Substring(2);

                Hoot.File.Delete(modFile.Replace("/", "\\"));

                var bookFiles = Hoot.File.GetFiles(bookPath.Replace("/", "\\") + "*.*");
                foreach (var file in bookFiles)
                {
                    Hoot.File.Delete(Path.Combine(bookPath.Replace("/", "\\"), file));
                }

                if (this.Sbmd.GetCetProperty(ConfigEntryType.ModDrv).Equals("RawGenBook"))
                {
                    // In a book, the main files are one searchway down.
                    var mainDir = Path.GetDirectoryName(bookPath.Replace("/", "\\") + ".idx");

                    bookFiles = Hoot.File.GetFiles(mainDir.Replace("/", "\\") + "\\*.*");
                    foreach (var file in bookFiles)
                    {
                        Hoot.File.Delete(Path.Combine(bookPath.Replace("/", "\\"), file));
                    }
                }
            }
            catch (Exception e3)
            {
                // many things can go wrong here. It is no danger to leave the bible in the rare case that this does not work.
                Debug.WriteLine(e3);
            }
        }

        private static void MakeSurePathExists(IsolatedStorageFile isolatedStorageRoot, string path)
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

        private void ClientDownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            if (ProgressUpdate != null)
            {
                ProgressUpdate(sender, e);
            }
        }

        private void ClientOpenReadCompleted(object sender, OpenReadCompletedEventArgs e)
        {
            try
            {
                ZipInputStream zipStream;
                try
                {
                    zipStream = new ZipInputStream(e.Result);
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

    public class WebInstaller
    {
        #region Fields

        /// <summary>
        ///   * A map of the books in this download area
        /// </summary>
        public readonly Dictionary<string, SwordBook> Entries = new Dictionary<string, SwordBook>();

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

        public string ReloadBookList()
        {
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

        public void UnzipBookList()
        {
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

        #endregion Methods
    }
}