// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InstalledBibles.cs" company="">
//   
// </copyright>
// <summary>
//   The installed bibles and commentaries.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

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
    using System.IO.IsolatedStorage;

    /// <summary>
    /// The installed bibles and commentaries.
    /// </summary>
    public class InstalledBiblesAndCommentaries
    {
        #region Constants and Fields

        /// <summary>
        /// The installed bibles.
        /// </summary>
        public Dictionary<string, SwordBook> InstalledBibles = new Dictionary<string, SwordBook>();

        /// <summary>
        /// The installed commentaries.
        /// </summary>
        public Dictionary<string, SwordBook> InstalledCommentaries = new Dictionary<string, SwordBook>();

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="InstalledBiblesAndCommentaries"/> class.
        /// </summary>
        public InstalledBiblesAndCommentaries()
        {
            string sBooks;
            if (IsolatedStorageSettings.ApplicationSettings.TryGetValue("installedBibles", out sBooks))
            {
                string[] books = sBooks.Split("¤".ToCharArray());
                foreach (string book in books)
                {
                    this.AddBook(book, false);
                }
            }

            if (IsolatedStorageSettings.ApplicationSettings.TryGetValue("installedCommentaries", out sBooks))
            {
                string[] books = sBooks.Split("¤".ToCharArray());
                foreach (string book in books)
                {
                    this.AddCommentary(book, false);
                }
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The add book.
        /// </summary>
        /// <param name="modPath">
        /// The mod path.
        /// </param>
        /// <param name="doSave">
        /// The do save.
        /// </param>
        public void AddBook(string modPath, bool doSave = true)
        {
            this.InstalledBibles[modPath] = new SwordBook(modPath);
            if (!this.InstalledBibles[modPath].IsLoaded)
            {
                this.InstalledBibles.Remove(modPath);
            }

            if (doSave)
            {
                this.Save();
                IsolatedStorageSettings.ApplicationSettings.Save();
            }
        }

        /// <summary>
        /// The add commentary.
        /// </summary>
        /// <param name="modPath">
        /// The mod path.
        /// </param>
        /// <param name="doSave">
        /// The do save.
        /// </param>
        public void AddCommentary(string modPath, bool doSave = true)
        {
            this.InstalledCommentaries[modPath] = new SwordBook(modPath);
            if (!this.InstalledCommentaries[modPath].IsLoaded)
            {
                this.InstalledCommentaries.Remove(modPath);
            }

            if (doSave)
            {
                this.Save();
                IsolatedStorageSettings.ApplicationSettings.Save();
            }
        }

        /// <summary>
        /// The save.
        /// </summary>
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

            IsolatedStorageSettings.ApplicationSettings["installedBibles"] = allBooks;

            allBooks = string.Empty;
            foreach (var book in this.InstalledCommentaries)
            {
                if (allBooks.Length > 0)
                {
                    allBooks += "¤";
                }

                allBooks += book.Key;
            }

            IsolatedStorageSettings.ApplicationSettings["installedCommentaries"] = allBooks;
        }

        #endregion
    }
}