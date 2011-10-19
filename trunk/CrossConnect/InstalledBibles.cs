#region Header

// <copyright file="InstalledBibles.cs" company="Thomas Dilts">
//
// CrossConnect Bible and Bible Commentary Reader for CrossWire.org
// Copyright (C) 2011 Thomas Dilts
//
// This program is free software: you can redistribute it and/or modify
// it under the +terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see http://www.gnu.org/licenses/.
// </copyright>
// <summary>
// Email: thomas@chaniel.se
// </summary>
// <author>Thomas Dilts</author>

#endregion Header

namespace CrossConnect
{
    using System.Collections.Generic;
    using System.IO.IsolatedStorage;

    public class InstalledBiblesAndCommentaries
    {
        #region Fields

        public Dictionary<string, SwordBook> InstalledBibles = new Dictionary<string, SwordBook>();
        public Dictionary<string, SwordBook> InstalledCommentaries = new Dictionary<string, SwordBook>();

        #endregion Fields

        #region Constructors

        public InstalledBiblesAndCommentaries()
        {
            string sBooks;
            if (IsolatedStorageSettings.ApplicationSettings.TryGetValue("installedBibles", out sBooks))
            {
                var books = sBooks.Split("¤".ToCharArray());
                foreach (string book in books)
                {
                    AddBook(book, false);
                }
            }
            if (IsolatedStorageSettings.ApplicationSettings.TryGetValue("installedCommentaries", out sBooks))
            {
                var books = sBooks.Split("¤".ToCharArray());
                foreach (string book in books)
                {
                    AddCommentary(book, false);
                }
            }
        }

        #endregion Constructors

        #region Methods

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
        }

        #endregion Methods
    }
}