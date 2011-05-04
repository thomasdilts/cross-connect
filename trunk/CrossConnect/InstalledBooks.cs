using System;
using System.Net;

using System.IO.IsolatedStorage;
using System.Collections.Generic;
using SwordBackend;

namespace CrossConnect
{
    public class InstalledBooks
    {
        public Dictionary<string, SwordBook> installedBooks = new Dictionary<string, SwordBook>();
        public InstalledBooks()
        {
            string sBooks = "";
            if (IsolatedStorageSettings.ApplicationSettings.TryGetValue<string>("InstalledBooks", out sBooks))
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
            installedBooks[modPath] = new SwordBook(modPath);
            if (!installedBooks[modPath].isLoaded)
            {
                installedBooks.Remove(modPath);
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
            foreach(var book in installedBooks)
            {
                if(allBooks.Length>0)
                {
                    allBooks+="¤";
                }
                allBooks += book.Key;
            }
            IsolatedStorageSettings.ApplicationSettings["InstalledBooks"]= allBooks;
        }
    }
}
