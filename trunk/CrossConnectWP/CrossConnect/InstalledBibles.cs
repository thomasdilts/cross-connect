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

namespace CrossConnect
{
    using Sword;
    using Sword.reader;
    using System.Collections.Generic;
    using System.IO.IsolatedStorage;
    using System.Threading.Tasks;
    using Windows.Storage;

    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO.IsolatedStorage;
    using System.Linq;
    using System.Threading;
    using System.Windows.Threading;




    public class InstalledBiblesAndCommentaries
    {
        #region Fields

        public Dictionary<string, SwordBook> InstalledBibles = new Dictionary<string, SwordBook>();
        public Dictionary<string, SwordBook> InstalledCommentaries = new Dictionary<string, SwordBook>();
        public Dictionary<string, SwordBook> InstalledGeneralBooks = new Dictionary<string, SwordBook>();

        #endregion Fields

        #region Constructors

        public InstalledBiblesAndCommentaries()
        {
        }

        public async Task Initialize()
        {
            try
            {
                //ApplicationData.Current.LocalFolder
                StorageFolder configFolder =
                    await ApplicationData.Current.LocalFolder.GetFolderAsync(BibleZtextReader.DirConf);
                IReadOnlyList<StorageFile> configFiles = await configFolder.GetFilesAsync();

                foreach (var file in configFiles)
                {
                    var splitFileName = file.Name.Split(new char[] { '.' });
                    if (splitFileName.Count() == 2 && ("." + splitFileName[splitFileName.Count() - 1].ToLower()).Equals(BibleZtextReader.ExtensionConf))
                    {
                        AddGenericBook(splitFileName[splitFileName.Count() - 2].ToLower());
                    }
                }
            }
            catch(Exception e)
            {
                // this can crash here in windows 8 if there are no files.  But in wp8 it does not
                // crash here. But just in case we put the try catch here.
            }
        }

        #endregion Constructors

        #region Methods

        public void AddGenericBook(string modPath, bool doSave = true)
        {
            var book = new SwordBook(modPath);
            if (!book.IsLoaded)
            {
                return ;
            }
            if (((string)book.Sbmd.GetProperty(ConfigEntryType.ModDrv)).ToUpper().Equals("ZTEXT"))
            {
                this.InstalledBibles[modPath] = book;
            }
            else if (((string)book.Sbmd.GetProperty(ConfigEntryType.ModDrv)).ToUpper().Equals("ZCOM"))
            {
                this.InstalledCommentaries[modPath] = book;
            }
            else if (((string)book.Sbmd.GetProperty(ConfigEntryType.ModDrv)).ToUpper().Equals("RAWGENBOOK"))
            {
                this.InstalledGeneralBooks[modPath] = book;
            }

            if (doSave)
            {
                this.Save();
                //Windows.Storage.ApplicationData.Current.LocalSettings.Values.Save();
            }
        }

        public void AddBook(string modPath, bool doSave = true)
        {
            InstalledBibles[modPath] = new SwordBook(modPath);
            if (!InstalledBibles[modPath].IsLoaded)
            {
                InstalledBibles.Remove(modPath);
            }

            if (doSave)
            {
                Save();
                IsolatedStorageSettings.ApplicationSettings.Save();
            }
        }

        public void AddCommentary(string modPath, bool doSave = true)
        {
            InstalledCommentaries[modPath] = new SwordBook(modPath);
            if (!InstalledCommentaries[modPath].IsLoaded)
            {
                InstalledCommentaries.Remove(modPath);
            }

            if (doSave)
            {
                Save();
                IsolatedStorageSettings.ApplicationSettings.Save();
            }
        }

        public void AddGeneralBook(string modPath, bool doSave = true)
        {
            InstalledGeneralBooks[modPath] = new SwordBook(modPath);
            if (!InstalledGeneralBooks[modPath].IsLoaded)
            {
                InstalledGeneralBooks.Remove(modPath);
            }

            if (doSave)
            {
                Save();
                IsolatedStorageSettings.ApplicationSettings.Save();
            }
        }

        public void Save()
        {
            string allBooks = string.Empty;
            foreach (var book in InstalledBibles)
            {
                if (allBooks.Length > 0)
                {
                    allBooks += "¤";
                }

                allBooks += book.Key;
            }

            IsolatedStorageSettings.ApplicationSettings["installedBibles"] = allBooks;

            allBooks = string.Empty;
            foreach (var book in InstalledCommentaries)
            {
                if (allBooks.Length > 0)
                {
                    allBooks += "¤";
                }

                allBooks += book.Key;
            }

            IsolatedStorageSettings.ApplicationSettings["installedCommentaries"] = allBooks;
            allBooks = string.Empty;
            foreach (var book in InstalledGeneralBooks)
            {
                if (allBooks.Length > 0)
                {
                    allBooks += "¤";
                }

                allBooks += book.Key;
            }

            IsolatedStorageSettings.ApplicationSettings["installedGeneralBooks"] = allBooks;
        }

        #endregion Methods
    }
}