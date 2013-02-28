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
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Windows.Storage;

    public class InstalledBiblesAndCommentaries
    {
        #region Fields

        public Dictionary<string, SwordBook> InstalledBibles = new Dictionary<string, SwordBook>();

        public Dictionary<string, SwordBook> InstalledCommentaries = new Dictionary<string, SwordBook>();

        public Dictionary<string, SwordBook> InstalledGeneralBooks = new Dictionary<string, SwordBook>();

        #endregion

        #region Constructors and Destructors

        public InstalledBiblesAndCommentaries()
        {
        }

        public async Task Initialize()
        {
            object sBooks;
            if (ApplicationData.Current.LocalSettings.Values.TryGetValue("installedBibles", out sBooks))
            {
                string[] books = ((string)sBooks).Split("¤".ToCharArray());
                foreach (string book in books)
                {
                    await this.AddBook(book, false);
                }
            }

            if (ApplicationData.Current.LocalSettings.Values.TryGetValue("installedCommentaries", out sBooks))
            {
                string[] books = ((string)sBooks).Split("¤".ToCharArray());
                foreach (string book in books)
                {
                    await this.AddCommentary(book, false);
                }
            }

            if (ApplicationData.Current.LocalSettings.Values.TryGetValue("installedGeneralBooks", out sBooks))
            {
                string[] books = ((string)sBooks).Split("¤".ToCharArray());
                foreach (string book in books)
                {
                    await this.AddGeneralBook(book, false);
                }
            }
        }

        #endregion

        #region Public Methods and Operators

        public async Task<bool> AddBook(string modPath, bool doSave = true)
        {
            this.InstalledBibles[modPath] = new SwordBook(modPath);
            await this.InstalledBibles[modPath].DoLoading(modPath);
            if (!this.InstalledBibles[modPath].IsLoaded)
            {
                this.InstalledBibles.Remove(modPath);
            }

            if (doSave)
            {
                this.Save();
                //Windows.Storage.ApplicationData.Current.LocalSettings.Values.Save();
            }

            return true;
        }

        public async Task<bool> AddCommentary(string modPath, bool doSave = true)
        {
            this.InstalledCommentaries[modPath] = new SwordBook(modPath);
            await this.InstalledCommentaries[modPath].DoLoading(modPath);
            if (!this.InstalledCommentaries[modPath].IsLoaded)
            {
                this.InstalledCommentaries.Remove(modPath);
            }

            if (doSave)
            {
                this.Save();
                //Windows.Storage.ApplicationData.Current.LocalSettings.Values.Save();
            }

            return true;
        }

        public async Task<bool> AddGeneralBook(string modPath, bool doSave = true)
        {
            this.InstalledGeneralBooks[modPath] = new SwordBook(modPath);
            await this.InstalledGeneralBooks[modPath].DoLoading(modPath);
            if (!this.InstalledGeneralBooks[modPath].IsLoaded)
            {
                this.InstalledGeneralBooks.Remove(modPath);
            }

            if (doSave)
            {
                this.Save();
                //Windows.Storage.ApplicationData.Current.LocalSettings.Values.Save();
            }

            return true;
        }
        public void Save()
        {
            string allBooks = string.Empty;
            foreach (var book in this.InstalledBibles)
            {
                if (allBooks.Length > 0)
                {
                    allBooks += "¤";
                }

                allBooks += book.Key;
            }

            ApplicationData.Current.LocalSettings.Values["installedBibles"] = allBooks;

            allBooks = string.Empty;
            foreach (var book in this.InstalledCommentaries)
            {
                if (allBooks.Length > 0)
                {
                    allBooks += "¤";
                }

                allBooks += book.Key;
            }

            ApplicationData.Current.LocalSettings.Values["installedCommentaries"] = allBooks;

            allBooks = string.Empty;
            foreach (var book in this.InstalledGeneralBooks)
            {
                if (allBooks.Length > 0)
                {
                    allBooks += "¤";
                }

                allBooks += book.Key;
            }

            ApplicationData.Current.LocalSettings.Values["installedGeneralBooks"] = allBooks;
        }

        #endregion
    }
}