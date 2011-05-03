using System;
using System.Net;


namespace book.install
{
    public interface IDataStorage
    {
        bool Backup(string token, object value);
        T Restore<T>(string token);
    }
}
