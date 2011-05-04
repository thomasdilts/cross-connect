using System;
using System.Runtime.CompilerServices;
using System.Diagnostics;
using System.Collections.Generic;
using System.Text;
using System.IO;

///
/// <summary> Distribution License:
/// JSword is free software; you can redistribute it and/or modify it under
/// the terms of the GNU Lesser General Public License, version 2.1 as published by
/// the Free Software Foundation. This program is distributed in the hope
/// that it will be useful, but WITHOUT ANY WARRANTY; without even the
/// implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
/// See the GNU Lesser General Public License for more details.
///
/// The License is available on the internet at:
///       http://www.gnu.org/copyleft/lgpl.html
/// or by writing to:
///      Free Software Foundation, Inc.
///      59 Temple Place - Suite 330
///      Boston, MA 02111-1307, USA
///
/// Copyright: 2005
///     The copyright to this program is held by it's authors.
///
/// ID: $Id: InstallManager.java 2054 2010-12-10 22:12:09Z dmsmith $
/// 
/// Converted from Java to C# by Thomas Dilts with the help of a program from www.tangiblesoftwaresolutions.com
/// called 'Java to VB & C# Converter' on 2011-04-12 </summary>
/// 
namespace SwordBackend
{

    using javaprops;
    using System.Reflection;
    using System.Net;
    using ICSharpCode.SharpZipLib.Tar;
    using ICSharpCode.SharpZipLib.GZip;
    using ICSharpCode.SharpZipLib.Zip;
    using System.IO.IsolatedStorage;

    //using Ionic.Zlib;

	///
	/// <summary> A manager to abstract out the non-view specific book installation tasks.
	///  </summary>
	/// <seealso cref= gnu.lgpl.License for license details.<br>
	///      The copyright to this program is held by it's authors.
	/// @author Joe Walker [joe at eireneh dot com]
	/// @author DM Smith [dmsmith555 at yahoo dot com] </seealso>
	/// 
	public class InstallManager
	{
        private Dictionary<string, WebInstaller> installers=new Dictionary<string, WebInstaller>();
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
        public Dictionary<string, WebInstaller> Installers
        {
            get { return installers; }
        }

	}
    public class WebInstaller
    {
        private Dictionary<string, SwordBook> books = new Dictionary<string, SwordBook>();
        public bool isLoaded
        {
            get 
            {
                return loaded;
            }
        }
        public WebInstaller(string host, string packageDirectory, string catalogDirectory)
        {
            this.host = host;
            this.packageDirectory = packageDirectory;
            this.catalogDirectory = catalogDirectory;
        }
        private WebClient client = new WebClient();
        public event DownloadProgressChangedEventHandler progress_update;
        public event OpenReadCompletedEventHandler progress_completed;
        private void download(Uri source)
        {
            try
            {
                client = new WebClient();
                client.Encoding = Encoding.BigEndianUnicode;
                client.DownloadProgressChanged+=new DownloadProgressChangedEventHandler(client_DownloadProgressChanged);
                client.OpenReadCompleted+=new OpenReadCompletedEventHandler(client_OpenReadCompleted);
                Logger.Debug("download start");
                //client.DownloadStringAsync(source);
                client.OpenReadAsync(source);
                Logger.Debug("DownloadStringAsync returned");
                
            }
            catch (Exception e)
            {
                Logger.Fail(e.ToString());
            };
        }
        private void client_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            Logger.Debug("client_DownloadProgressChanged;" + e.ProgressPercentage + ";" + e.TotalBytesToReceive);
            if (progress_update != null)
            {
                progress_update(sender,e);
            }
        }
        private void client_OpenReadCompleted(object sender, OpenReadCompletedEventArgs e)
        {
            Logger.Debug("client_DownloadStringCompleted;" + e.Result.Length );

            GZipInputStream gzip = new GZipInputStream(e.Result);
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

                        
                        //sbmd.Driver = fake;
                        SwordBook book = new SwordBook(buffer, @internal);
                        entries.Add(book.Name, book);
                    }
                    catch (IOException ex)
                    {
                        Logger.Fail("Failed to load config for entry: " + @internal);
                    }
                }
            }

            loaded = true;
            if (progress_completed != null)
            {
                progress_completed(sender, e);
            }
        }
        public bool reloadBookList()
        {
            books.Clear();
            try
            {
                Uri uri = new Uri("http://" + host + "/" + catalogDirectory + "/" + FILE_LIST_GZ);
                download(uri);
            }catch(Exception e)
            {
                Logger.Fail(e.Message);
                return loaded;
            }
            
            loaded = true;
            return loaded;
        }


        ///    
        /// <summary>* A map of the books in this download area </summary>
        ///     
        public Dictionary<string, SwordBook> entries = new Dictionary<string, SwordBook>();

        ///    
        /// <summary>* The remote hostname. </summary>
        ///     
        public string host;

        ///    
        /// <summary>* The remote proxy hostname. </summary>
        ///     
        protected internal string proxyHost;

        ///    
        /// <summary>* The remote proxy port. </summary>
        ///     
        protected internal int? proxyPort;

        ///    
        /// <summary>* The directory containing zipped books on the <code>host</code>. </summary>
        ///     
        protected internal string packageDirectory = "";

        ///    
        /// <summary>* The directory containing the catalog of all books on the
        /// <code>host</code>. </summary>
        ///     
        public string catalogDirectory = "";

        ///    
        /// <summary>* The directory containing the catalog of all books on the
        /// <code>host</code>. </summary>
        ///     
        protected internal string indexDirectory = "";

        ///    
        /// <summary>* Do we need to reload the index file </summary>
        ///     
        protected internal bool loaded;

        ///    
        /// <summary>* The sword index file </summary>
        ///     
        protected internal const string FILE_LIST_GZ = "mods.d.tar.gz";

        ///    
        /// <summary>* The suffix of zip books on this server </summary>
        ///     
        protected internal const string ZIP_SUFFIX = ".zip";

        ///    
        /// <summary>* The relative path of the dir holding the search index files </summary>
        ///     
        protected internal const string SEARCH_DIR = "search/jsword/L1";

        ///    
        /// <summary>* When we cache a download index </summary>
        ///     
        protected internal const string DOWNLOAD_PREFIX = "download-";

    }
    public class SwordBook
    {
        private WebClient client = new WebClient();
        public event DownloadProgressChangedEventHandler progress_update;
        public event OpenReadCompletedEventHandler progress_completed;
        public SwordBookMetaData sbmd = null;
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
        public bool isLoaded = false;
        public string Name
        {
            get 
            {
                return sbmd.Name;
            }
        }
        public const string ZIP_SUFFIX = ".zip";
        public void downloadBookNow(WebInstaller iManager)
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
                //client.DownloadStringAsync(source);
                client.OpenReadAsync(source);
                Logger.Debug("DownloadStringAsync returned");

            }
            catch (Exception e)
            {
                Logger.Fail(e.ToString());
            };
        }
        private void client_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            Logger.Debug("client_DownloadProgressChanged;" + e.ProgressPercentage + ";" + e.TotalBytesToReceive);
            if (progress_update != null)
            {
                progress_update(sender,e);
            }
        }
        private void client_OpenReadCompleted(object sender, OpenReadCompletedEventArgs e)
        {
            try
            {
                Logger.Debug("client_DownloadStringCompleted;" + e.Result.Length);

                ZipInputStream zipStream = new ZipInputStream(e.Result);
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
                    progress_completed(sender, e);
                }
            }
            catch (Exception)
            {
                if (progress_completed != null)
                {
                    progress_completed(sender, e);
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
    }

    public class Logger
    {
        public static void Warn(string message)
        {
            System.Diagnostics.Debug.WriteLine(DateTime.Now + " " + message);
        }
        public static void Fail(string message)
        {
            System.Diagnostics.Debug.WriteLine(DateTime.Now + " " + message);
        }
        public static void Debug(string message)
        {
            System.Diagnostics.Debug.WriteLine(DateTime.Now + " " + message);
        }
    }

}