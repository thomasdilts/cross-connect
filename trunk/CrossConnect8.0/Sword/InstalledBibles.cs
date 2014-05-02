#region Header

// <copyright file="InstalledBibles.cs" company="Thomas Dilts">
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

namespace Sword
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.IO;
    //using System.IO.Compression;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Xml;

    using Sword.reader;
    using Windows.Storage;
    using Windows.Storage.Streams;
    using Sword;

    public class InstalledBiblesAndCommentaries
    {
        #region Fields

        public Dictionary<string, SwordBookMetaData> InstalledBibles = new Dictionary<string, SwordBookMetaData>();

        public Dictionary<string, SwordBookMetaData> InstalledCommentaries = new Dictionary<string, SwordBookMetaData>();

        public Dictionary<string, SwordBookMetaData> InstalledGeneralBooks = new Dictionary<string, SwordBookMetaData>();

        #endregion

        #region Constructors and Destructors

        public InstalledBiblesAndCommentaries()
        {
        }

        public async Task Initialize()
        {
            try
            {
                StorageFolder configFolder =
                    await ApplicationData.Current.LocalFolder.GetFolderAsync(BibleZtextReader.DirConf);
                IReadOnlyList<StorageFile> configFiles = await configFolder.GetFilesAsync();

                foreach (var file in configFiles)
                {
                    var splitFileName = file.Name.Split(new char[] { '.' });
                    if (splitFileName.Count() == 2 && ("." + splitFileName[splitFileName.Count() - 1].ToLower()).Equals(BibleZtextReader.ExtensionConf))
                    {
                        await AddGenericBook(splitFileName[splitFileName.Count() - 2].ToLower());
                    }
                }
            }
            catch (Exception e)
            {
                // can come here if there are no files in the directory.
            }

        }

        #endregion

        #region Public Methods and Operators

        public async Task<bool> AddGenericBook(string modPath)
        {
            var book = await this.DoLoading(modPath);
            if (book == null)
            {
                return false;
            }
            if (((string)book.GetProperty(ConfigEntryType.ModDrv)).ToUpper().Equals("ZTEXT"))
            {
                this.InstalledBibles[modPath] = book;
            }
            else if (((string)book.GetProperty(ConfigEntryType.ModDrv)).ToUpper().Equals("ZCOM"))
            {
                this.InstalledCommentaries[modPath] = book;
            }
            else if (((string)book.GetProperty(ConfigEntryType.ModDrv)).ToUpper().Equals("RAWGENBOOK"))
            {
                this.InstalledGeneralBooks[modPath] = book;
            }

            return true;
        }

        public async Task<SwordBookMetaData> DoLoading(string internalName)
        {
            SwordBookMetaData Sbmd = null;
            if (string.IsNullOrEmpty(internalName))
            {
                return null;
            }
            try
            {
                StorageFolder folder = ApplicationData.Current.LocalFolder;
                string filepath = BibleZtextReader.DirConf + '/' + internalName.ToLower()
                                  + BibleZtextReader.ExtensionConf;
                StorageFile file = await folder.GetFileAsync(filepath.Replace("/", "\\"));
                IRandomAccessStream istream = await file.OpenAsync(FileAccessMode.Read);
                Stream stream = istream.AsStreamForRead();
                Sbmd = new SwordBookMetaData(stream, internalName);
            }
            catch (Exception ee)
            {
                Debug.WriteLine(ee.StackTrace);
            }

            return Sbmd;
        }
        public static async void RemoveBible(SwordBookMetaData Sbmd)
        {
            try
            {
                string modFile = BibleZtextReader.DirConf + '\\' + Sbmd.InternalName.ToLower()
                                 + BibleZtextReader.ExtensionConf;
                string bookPath = Sbmd.GetCetProperty(ConfigEntryType.ADataPath).ToString().Substring(2);
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

                if (Sbmd.GetCetProperty(ConfigEntryType.ModDrv).Equals("RawGenBook"))
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
    }
}