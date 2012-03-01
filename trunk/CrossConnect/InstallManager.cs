// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InstallManager.cs" company="">
//   
// </copyright>
// <summary>
//   Main entry into the world of Sword for downloading books
// </summary>
// --------------------------------------------------------------------------------------------------------------------

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

    /// <summary>
    /// Main entry into the world of Sword for downloading books
    /// </summary>
    public class InstallManager
    {
        #region Constants and Fields

        /// <summary>
        /// The _callback list retrieved.
        /// </summary>
        private BibleListReturned _callbackListRetrieved;

        /// <summary>
        /// The _client.
        /// </summary>
        private WebClient _client = new WebClient();

        /// <summary>
        /// The _installers.
        /// </summary>
        private Dictionary<string, WebInstaller> _installers = new Dictionary<string, WebInstaller>();

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="InstallManager"/> class. 
        ///   * Simple ctor
        /// </summary>
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

        /// <summary>
        /// The bible list returned.
        /// </summary>
        /// <param name="installers">
        /// The installers.
        /// </param>
        /// <param name="message">
        /// The message.
        /// </param>
        public delegate void BibleListReturned(Dictionary<string, WebInstaller> installers, string message);

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets Installers.
        /// </summary>
        public Dictionary<string, WebInstaller> Installers
        {
            get
            {
                return this._installers;
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The get bible download list.
        /// </summary>
        /// <param name="callbackListRetrieved">
        /// The callback list retrieved.
        /// </param>
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
                var source = new Uri(url);

                this._client = new WebClient();
                this._client.OpenReadCompleted += this.ClientOpenReadCompleted;
                Logger.Debug("download start");
                this._client.OpenReadAsync(source);
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

        /// <summary>
        /// The add custom download link.
        /// </summary>
        /// <param name="installers">
        /// The installers.
        /// </param>
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

        /// <summary>
        /// The client open read completed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
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
                this._callbackListRetrieved(this._installers, string.Empty);
                return;
            }

            this.AddCustomDownloadLink(installers);
            this._installers = installers;
            this._callbackListRetrieved(installers, msgFromServer);
        }

        #endregion
    }

    /// <summary>
    /// The logger.
    /// </summary>
    public static class Logger
    {
        #region Public Methods and Operators

        /// <summary>
        /// The debug.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        public static void Debug(string message)
        {
            System.Diagnostics.Debug.WriteLine(DateTime.Now + " " + message);
        }

        /// <summary>
        /// The fail.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        public static void Fail(string message)
        {
            System.Diagnostics.Debug.WriteLine(DateTime.Now + " " + message);
        }

        /// <summary>
        /// The warn.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        public static void Warn(string message)
        {
            System.Diagnostics.Debug.WriteLine(DateTime.Now + " " + message);
        }

        #endregion
    }

    /// <summary>
    /// The sword book.
    /// </summary>
    public class SwordBook
    {
        #region Constants and Fields

        /// <summary>
        /// The is loaded.
        /// </summary>
        public readonly bool IsLoaded;

        /// <summary>
        /// The sbmd.
        /// </summary>
        public readonly SwordBookMetaData Sbmd;

        /// <summary>
        /// The zip suffix.
        /// </summary>
        private const string ZipSuffix = ".zip";

        /// <summary>
        /// The _client.
        /// </summary>
        private WebClient _client = new WebClient();

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SwordBook"/> class.
        /// </summary>
        /// <param name="internalName">
        /// The internal name.
        /// </param>
        public SwordBook(string internalName)
        {
            this.IsLoaded = false;
            try
            {
                IsolatedStorageFile root = IsolatedStorageFile.GetUserStoreForApplication();
                IsolatedStorageFileStream stream =
                    root.OpenFile(
                        BibleZtextReader.DirConf + '/' + internalName.ToLower() + BibleZtextReader.ExtensionConf, 
                        FileMode.Open);
                this.Sbmd = new SwordBookMetaData(stream, internalName);
                this.IsLoaded = true;
            }
            catch (Exception ee)
            {
                Debug.WriteLine(ee.StackTrace);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SwordBook"/> class.
        /// </summary>
        /// <param name="buffer">
        /// The buffer.
        /// </param>
        /// <param name="bookName">
        /// The book name.
        /// </param>
        public SwordBook(byte[] buffer, string bookName)
        {
            this.Sbmd = new SwordBookMetaData(buffer, bookName);
            this.IsLoaded = true;
        }

        #endregion

        #region Public Events

        /// <summary>
        /// The progress completed.
        /// </summary>
        public event OpenReadCompletedEventHandler ProgressCompleted;

        /// <summary>
        /// The progress update.
        /// </summary>
        public event DownloadProgressChangedEventHandler ProgressUpdate;

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets Name.
        /// </summary>
        public string Name
        {
            get
            {
                return this.Sbmd.Name;
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The download book now.
        /// </summary>
        /// <param name="iManager">
        /// The i manager.
        /// </param>
        /// <returns>
        /// The download book now.
        /// </returns>
        public string DownloadBookNow(WebInstaller iManager)
        {
            try
            {
                string pathToHost = "http://" + iManager.Host + iManager.PackageDirectory + "/" + this.Sbmd.Initials
                                    + ZipSuffix;
                var source = new Uri(pathToHost);

                this._client = new WebClient();
                this._client.DownloadProgressChanged += this.ClientDownloadProgressChanged;
                this._client.OpenReadCompleted += this.ClientOpenReadCompleted;
                Logger.Debug("download start");
                this._client.OpenReadAsync(source);
                Logger.Debug("DownloadStringAsync returned");
                return null;
            }
            catch (Exception e)
            {
                Logger.Fail(e.ToString());
                return e.Message;
            }
        }

        /// <summary>
        /// The remove bible.
        /// </summary>
        public void RemoveBible()
        {
            try
            {
                IsolatedStorageFile root = IsolatedStorageFile.GetUserStoreForApplication();
                string modFile = BibleZtextReader.DirConf + '/' + this.Sbmd.InternalName.ToLower()
                                 + BibleZtextReader.ExtensionConf;
                string bookPath = this.Sbmd.GetCetProperty(ConfigEntryType.ADataPath).ToString().Substring(2);
                var filesToDelete = new[]
                    {
                        modFile, bookPath + "ot.bzs", bookPath + "ot.bzv", bookPath + "ot.bzz", bookPath + "nt.bzs", 
                        bookPath + "nt.bzv", bookPath + "nt.bzz" + bookPath + "ot.czs", bookPath + "ot.czv", 
                        bookPath + "ot.czz", bookPath + "nt.czs", bookPath + "nt.czv", bookPath + "nt.czz"
                    };
                foreach (string file in filesToDelete)
                {
                    if (root.FileExists(file))
                    {
                        root.DeleteFile(file);
                    }
                }

                if (root.DirectoryExists(bookPath))
                {
                    root.DeleteDirectory(bookPath.Substring(0, bookPath.Length - 1));
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

        /// <summary>
        /// Create all the directories necesary to make the given path valid.
        /// </summary>
        /// <param name="isolatedStorageRoot">
        /// </param>
        /// <param name="path">
        /// </param>
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

        /// <summary>
        /// The client download progress changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void ClientDownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            if (this.ProgressUpdate != null)
            {
                this.ProgressUpdate(sender, e);
            }
        }

        /// <summary>
        /// The client open read completed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
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
                    if (this.ProgressCompleted != null)
                    {
                        this.ProgressCompleted(e.Error.Message, e);
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

                if (this.ProgressCompleted != null)
                {
                    this.ProgressCompleted(null, e);
                }
            }
            catch (Exception exp)
            {
                if (this.ProgressCompleted != null)
                {
                    this.ProgressCompleted(exp.Message, e);
                }
            }
        }

        #endregion
    }

    /// <summary>
    /// The web installer.
    /// </summary>
    public class WebInstaller
    {
        #region Constants and Fields

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

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="WebInstaller"/> class.
        /// </summary>
        /// <param name="host">
        /// The host.
        /// </param>
        /// <param name="packageDirectory">
        /// The package directory.
        /// </param>
        /// <param name="catalogDirectory">
        /// The catalog directory.
        /// </param>
        public WebInstaller(string host, string packageDirectory, string catalogDirectory)
        {
            this.Host = host;
            this.PackageDirectory = packageDirectory;
            this._catalogDirectory = catalogDirectory;
        }

        #endregion

        #region Public Events

        /// <summary>
        /// The progress completed.
        /// </summary>
        public event OpenReadCompletedEventHandler ProgressCompleted;

        /// <summary>
        /// The progress update.
        /// </summary>
        public event DownloadProgressChangedEventHandler ProgressUpdate;

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets a value indicating whether IsLoaded.
        /// </summary>
        public bool IsLoaded
        {
            get
            {
                return this._loaded;
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The reload book list.
        /// </summary>
        /// <returns>
        /// The reload book list.
        /// </returns>
        public string ReloadBookList()
        {
            this._books.Clear();
            var uri = new Uri("http://" + this.Host + "/" + this._catalogDirectory + "/" + FileListGz);
            string errMsg = this.Download(uri);
            if (errMsg != null)
            {
                this._loaded = false;
                return errMsg;
            }

            this._loaded = true;
            return null;
        }

        /// <summary>
        /// The unzip book list.
        /// </summary>
        public void UnzipBookList()
        {
            GZipInputStream gzip;
            try
            {
                gzip = new GZipInputStream(this._resultStream);
            }
            catch (Exception)
            {
                if (this.ProgressCompleted != null)
                {
                    this.ProgressCompleted(null, null);
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

                Deployment.Current.Dispatcher.BeginInvoke(
                    () =>
                    this.ProgressUpdate(50.0 + (this._resultStream.Position * 50.0 / this._resultStream.Length), null));
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
                            Deployment.Current.Dispatcher.BeginInvoke(() => this.ProgressCompleted(exp.Message, null));
                        }

                        return;
                    }
                }
            }

            this._loaded = true;
            if (this.ProgressCompleted != null)
            {
                Deployment.Current.Dispatcher.BeginInvoke(() => this.ProgressCompleted(null, null));
            }

            this._resultStream.Close();
            this._resultStream.Dispose();
        }

        #endregion

        #region Methods

        /// <summary>
        /// The client download progress changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void ClientDownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            if (this.ProgressUpdate != null)
            {
                this.ProgressUpdate(e.ProgressPercentage / 2.0, e);
            }
        }

        /// <summary>
        /// The client open read completed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void ClientOpenReadCompleted(object sender, OpenReadCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                if (this.ProgressCompleted != null)
                {
                    this.ProgressCompleted(e.Error.Message, null);
                }

                return;
            }

            this._resultStream = e.Result;
            if (this.ProgressCompleted != null)
            {
                this.ProgressCompleted(null, null);
            }
        }

        /// <summary>
        /// The download.
        /// </summary>
        /// <param name="source">
        /// The source.
        /// </param>
        /// <returns>
        /// The download.
        /// </returns>
        private string Download(Uri source)
        {
            try
            {
                this._client = new WebClient { Encoding = Encoding.BigEndianUnicode };
                this._client.DownloadProgressChanged += this.ClientDownloadProgressChanged;
                this._client.OpenReadCompleted += this.ClientOpenReadCompleted;
                Logger.Debug("download start");

                // client.DownloadStringAsync(source);
                this._client.OpenReadAsync(source);
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