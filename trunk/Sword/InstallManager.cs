/// <summary>
/// Distribution License:
/// CrossConnect is free software; you can redistribute it and/or modify it under
/// the terms of the GNU General Public License, version 3 as published by
/// the Free Software Foundation. This program is distributed in the hope
/// that it will be useful, but WITHOUT ANY WARRANTY; without even the
/// implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
/// See the GNU General Public License for more details.
///
/// The License is available on the internet at:
///       http://www.gnu.org/copyleft/gpl.html
/// or by writing to:
///      Free Software Foundation, Inc.
///      59 Temple Place - Suite 330
///      Boston, MA 02111-1307, USA
/// </summary>
/// <copyright file="THIS_FILE.cs" company="Thomas Dilts">
///     Thomas Dilts. All rights reserved.
/// </copyright>
/// <author>Thomas Dilts</author>
namespace SwordBackend
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.IO.IsolatedStorage;
    using System.Net;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Text;

    using ICSharpCode.SharpZipLib.GZip;
    using ICSharpCode.SharpZipLib.Tar;
    using ICSharpCode.SharpZipLib.Zip;

    using javaprops;

    /// <summary>
    /// Main entry into the world of Sword for downloading books
    /// </summary>
    public class InstallManager
    {
        #region Fields

        private Dictionary<string, WebInstaller> installers = new Dictionary<string, WebInstaller>();

        #endregion Fields

        #region Constructors

        ///    
        /// <summary>* Simple ctor </summary>
        ///     
        public InstallManager()
        {
            JavaProperties sitemap = new JavaProperties("InstallManager.plugin",false);

            foreach (var site in sitemap.Values)
            {
                string[] parts = site.Split(",".ToCharArray());
                string type = parts[0];
                string name = parts[1];
                string host = parts[2];
                string packageDirectory = parts[3];
                string catalogDirectory = parts[4];
                installers[name] = new WebInstaller(host, packageDirectory, catalogDirectory);
            }
        }

        #endregion Constructors

        #region Properties

        public Dictionary<string, WebInstaller> Installers
        {
            get { return installers; }
        }

        #endregion Properties
    }

    public class Logger
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

        public const string ZIP_SUFFIX = ".zip";

        public bool isLoaded = false;
        public SwordBookMetaData sbmd = null;

        private WebClient client = new WebClient();

        #endregion Fields

        #region Constructors

        public SwordBook(string internalName)
        {
            isLoaded = false;
            try
            {
                IsolatedStorageFile root = System.IO.IsolatedStorage.IsolatedStorageFile.GetUserStoreForApplication();
                IsolatedStorageFileStream stream = root.OpenFile(BibleZtextReader.DIR_CONF + '/' + internalName.ToLower() + BibleZtextReader.EXTENSION_CONF, FileMode.Open);
                if (stream != null)
                {
                    sbmd = new SwordBookMetaData(stream, internalName);
                    isLoaded = true;
                }
            }
            catch (Exception)
            {
            }
        }

        public SwordBook(byte[] buffer, string bookName)
        {
            sbmd = new SwordBookMetaData(buffer, bookName);
            isLoaded = true;
        }

        #endregion Constructors

        #region Events

        public event OpenReadCompletedEventHandler progress_completed;

        public event DownloadProgressChangedEventHandler progress_update;

        #endregion Events

        #region Properties

        public string Name
        {
            get
            {
                return sbmd.Name;
            }
        }

        #endregion Properties

        #region Methods

        public string downloadBookNow(WebInstaller iManager)
        {
            try
            {

                string relativePath = (string)sbmd.getProperty(ConfigEntryType.DATA_PATH);
                string pathToHost = "http://" + iManager.host  + iManager.packageDirectory + "/" + sbmd.Initials + ZIP_SUFFIX;
                Uri source = new Uri(pathToHost);

                client = new WebClient();
                client.DownloadProgressChanged += new DownloadProgressChangedEventHandler(client_DownloadProgressChanged);
                client.OpenReadCompleted += new OpenReadCompletedEventHandler(client_OpenReadCompleted);
                Logger.Debug("download start");
                // client.DownloadStringAsync(source);
                client.OpenReadAsync(source);
                Logger.Debug("DownloadStringAsync returned");
                return null;
            }
            catch (Exception e)
            {
                Logger.Fail(e.ToString());
                return e.Message;
            };
        }

        public void RemoveBible()
        {
            IsolatedStorageFile root = System.IO.IsolatedStorage.IsolatedStorageFile.GetUserStoreForApplication();
            string modFile=BibleZtextReader.DIR_CONF + '/' + sbmd.internalName.ToLower() + BibleZtextReader.EXTENSION_CONF;
            string bookPath = sbmd.getCetProperty(ConfigEntryType.DATA_PATH).ToString().Substring(2);
            string[] filesToDelete=new string[]{modFile,bookPath +"ot.bzs",bookPath +"ot.bzv",bookPath +"ot.bzz",bookPath +"nt.bzs",bookPath +"nt.bzv",bookPath +"nt.bzz" };
            foreach(string file in filesToDelete)
            {
                if (root.FileExists(file))
                {
                    root.DeleteFile(file);
                }
            }
            if (root.DirectoryExists(bookPath))
            {
                root.DeleteDirectory(bookPath.Substring(0,bookPath.Length-1));
            }
        }

        private void client_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            if (progress_update != null)
            {
                progress_update(sender,e);
            }
        }

        private void client_OpenReadCompleted(object sender, OpenReadCompletedEventArgs e)
        {
            try
            {
                ZipInputStream zipStream = null;
                try
                {
                    zipStream = new ZipInputStream(e.Result);
                }
                catch (Exception)
                {
                    if (progress_completed != null)
                    {
                        progress_completed(e.Error.Message, e);
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
                        isolatedStorageRoot.CreateDirectory(entry.Name);
                    }
                    else
                    {
                        makeSurePathExists(isolatedStorageRoot, entry.Name);
                        IsolatedStorageFileStream fStream = isolatedStorageRoot.CreateFile(entry.Name);
                        byte[] buffer = new byte[10000];
                        int len;
                        while ((len = zipStream.Read(buffer, 0, buffer.GetUpperBound(0))) > 0)
                        {
                            fStream.Write(buffer, 0, len);
                        }
                        fStream.Close();
                    }
                }
                if (progress_completed != null)
                {
                    progress_completed(null, e);
                }
            }
            catch (Exception exp)
            {
                if (progress_completed != null)
                {
                    progress_completed(exp.Message, e);
                }
            }
        }

        /// <summary>
        /// Create all the directories necesary to make the given path valid.
        /// </summary>
        /// <param name="isolatedStorageRoot"></param>
        /// <param name="path"></param>
        private void makeSurePathExists(IsolatedStorageFile isolatedStorageRoot,string path)
        {
            string[] directories = path.Split("/".ToCharArray());
            string totalTestPath="";
            if (directories.Length > 1)
            {
                for(int i=0;i<(directories.Length-1);i++)
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

        #endregion Methods
    }

    public class WebInstaller
    {
        #region Fields

        ///    
        /// <summary>* The directory containing the catalog of all books on the
        /// <code>host</code>. </summary>
        ///     
        public string catalogDirectory = string.Empty;

        ///    
        /// <summary>* A map of the books in this download area </summary>
        ///     
        public Dictionary<string, SwordBook> entries = new Dictionary<string, SwordBook>();

        ///    
        /// <summary>* The remote hostname. </summary>
        ///     
        public string host;

        ///    
        /// <summary>* When we cache a download index </summary>
        ///     
        protected internal const string DOWNLOAD_PREFIX = "download-";

        ///    
        /// <summary>* The sword index file </summary>
        ///     
        protected internal const string FILE_LIST_GZ = "mods.d.tar.gz";

        ///    
        /// <summary>* The relative path of the dir holding the search index files </summary>
        ///     
        protected internal const string SEARCH_DIR = "search/jsword/L1";

        ///    
        /// <summary>* The suffix of zip books on this server </summary>
        ///     
        protected internal const string ZIP_SUFFIX = ".zip";

        ///    
        /// <summary>* The directory containing the catalog of all books on the
        /// <code>host</code>. </summary>
        ///     
        protected internal string indexDirectory = string.Empty;

        ///    
        /// <summary>* Do we need to reload the index file </summary>
        ///     
        protected internal bool loaded;

        ///    
        /// <summary>* The directory containing zipped books on the <code>host</code>. </summary>
        ///     
        protected internal string packageDirectory = string.Empty;

        ///    
        /// <summary>* The remote proxy hostname. </summary>
        ///     
        protected internal string proxyHost;

        ///    
        /// <summary>* The remote proxy port. </summary>
        ///     
        protected internal int? proxyPort;

        private Dictionary<string, SwordBook> books = new Dictionary<string, SwordBook>();
        private WebClient client = new WebClient();

        #endregion Fields

        #region Constructors

        public WebInstaller(string host, string packageDirectory, string catalogDirectory)
        {
            this.host = host;
            this.packageDirectory = packageDirectory;
            this.catalogDirectory = catalogDirectory;
        }

        #endregion Constructors

        #region Events

        public event OpenReadCompletedEventHandler progress_completed;

        public event DownloadProgressChangedEventHandler progress_update;

        #endregion Events

        #region Properties

        public bool isLoaded
        {
            get
            {
                return loaded;
            }
        }

        #endregion Properties

        #region Methods

        public string reloadBookList()
        {
            books.Clear();
            Uri uri = new Uri("http://" + host + "/" + catalogDirectory + "/" + FILE_LIST_GZ);
            string errMsg=download(uri);
            if(errMsg!=null)
            {
                loaded = false;
                return errMsg;
            }
            loaded = true;
            return null;
        }

        private void client_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            if (progress_update != null)
            {
                progress_update(sender,e);
            }
        }

        private void client_OpenReadCompleted(object sender, OpenReadCompletedEventArgs e)
        {
            GZipInputStream gzip = null;
            try
            {
                gzip = new GZipInputStream(e.Result);
            }
            catch(Exception)
            {
                if (progress_completed != null)
                {
                    progress_completed(e.Error.Message, e);
                }
                loaded = false;
                return;
            }
            TarInputStream tin = new TarInputStream(gzip);
            entries.Clear();
            while (true)
            {
                TarEntry entry = tin.GetNextEntry();
                if (entry == null)
                {
                    break;
                }

                string @internal = entry.Name;
                if (!entry.IsDirectory)
                {
                    try
                    {
                        int size = (int)entry.Size;

                        // Every now and then an empty entry sneaks in
                        if (size == 0)
                        {
                            Logger.Fail("Empty entry: " + @internal);
                            continue;
                        }

                        byte[] buffer = new byte[size];
                        MemoryStream ms = new MemoryStream(buffer);
                        ms.Position = 0;
                        tin.CopyEntryContents(ms);
                        if (ms.Position != size)
                        {
                            // This should not happen, but if it does then skip
                            // it.
                            Logger.Fail("Did not read all that was expected " + @internal);
                            continue;
                        }

                        if (@internal.EndsWith(BibleZtextReader.EXTENSION_CONF))
                        {
                            @internal = @internal.Substring(0, @internal.Length - 5);
                        }
                        else
                        {
                            Logger.Fail("Not a SWORD config file: " + @internal);
                            continue;
                        }

                        if (@internal.StartsWith(BibleZtextReader.DIR_CONF + '/'))
                        {
                            @internal = @internal.Substring(7);
                        }

                        // sbmd.Driver = fake;
                        SwordBook book = new SwordBook(buffer, @internal);
                        entries.Add(book.Name, book);
                    }
                    catch (Exception exp)
                    {
                        Logger.Fail("Failed to load config for entry: " + @internal);
                        loaded = true;
                        if (progress_completed != null)
                        {
                            progress_completed(exp.Message, e);
                        }
                        return;
                    }
                }
            }

            loaded = true;
            if (progress_completed != null)
            {
                progress_completed(null, e);
            }
        }

        private string download(Uri source)
        {
            try
            {
                client = new WebClient();
                client.Encoding = Encoding.BigEndianUnicode;
                client.DownloadProgressChanged+=new DownloadProgressChangedEventHandler(client_DownloadProgressChanged);
                client.OpenReadCompleted+=new OpenReadCompletedEventHandler(client_OpenReadCompleted);
                Logger.Debug("download start");
                // client.DownloadStringAsync(source);
                client.OpenReadAsync(source);
                Logger.Debug("DownloadStringAsync returned");
                return null;
            }
            catch (Exception e)
            {
                Logger.Fail(e.ToString());
                return e.Message;
            };
        }

        #endregion Methods
    }
}