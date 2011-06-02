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
/// <copyright file="InstalledBooks.cs" company="Thomas Dilts">
///     Thomas Dilts. All rights reserved.
/// </copyright>
/// <author>Thomas Dilts</author>
namespace CrossConnect
{
    using System;
    using System.Collections.Generic;
    using System.IO.IsolatedStorage;
    using System.Net;

    using SwordBackend;

    public class InstalledBibles
    {
        #region Fields

        public Dictionary<string, SwordBook> installedBibles = new Dictionary<string, SwordBook>();

        #endregion Fields

        #region Constructors

        public InstalledBibles()
        {
            string sBooks = string.Empty;
            if (IsolatedStorageSettings.ApplicationSettings.TryGetValue<string>("installedBibles", out sBooks))
            {
                string[] books = sBooks.Split("¤".ToCharArray());
                foreach (string book in books)
                {
                    AddBook(book,false);
                }
            }
        }

        #endregion Constructors

        #region Methods

        public void AddBook(string modPath, bool doSave = true)
        {
            installedBibles[modPath] = new SwordBook(modPath);
            if (!installedBibles[modPath].isLoaded)
            {
                installedBibles.Remove(modPath);
            }
            if (doSave)
            {
                save();
                IsolatedStorageSettings.ApplicationSettings.Save();
            }
        }

        public void save()
        {
            string allBooks = string.Empty;
            foreach(var book in installedBibles)
            {
                if(allBooks.Length>0)
                {
                    allBooks+="¤";
                }
                allBooks += book.Key;
            }
            IsolatedStorageSettings.ApplicationSettings["installedBibles"]= allBooks;
        }

        #endregion Methods
    }
}