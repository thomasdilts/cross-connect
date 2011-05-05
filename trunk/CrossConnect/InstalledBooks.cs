using System;
using System.Net;

using System.IO.IsolatedStorage;
using System.Collections.Generic;
using SwordBackend;

namespace CrossConnect
{
    public class InstalledBibles
    {
        public Dictionary<string, SwordBook> installedBibles = new Dictionary<string, SwordBook>();
        public InstalledBibles()
        {
            string sBooks = "";
            if (IsolatedStorageSettings.ApplicationSettings.TryGetValue<string>("installedBibles", out sBooks))
            {
                string[] books = sBooks.Split("¤".ToCharArray());
                foreach (string book in books)
                {
                    AddBook(book,false);
                }
            }

        }
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
            string allBooks = "";
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
    }
}
