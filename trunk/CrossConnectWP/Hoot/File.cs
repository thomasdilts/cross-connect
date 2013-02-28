#region Header

// <copyright file="File.cs" company="Thomas Dilts">
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
// Email: thomas@cross-connect.se
// </summary>
// <author>Thomas Dilts</author>

#endregion Header
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Hoot
{
    using System.IO;
    using System.IO.IsolatedStorage;

    public class File
    {
        public static bool Exists(string path)
        {
            IsolatedStorageFile root = IsolatedStorageFile.GetUserStoreForApplication();
            try
            {
                return root.FileExists(path);
                //StorageFile file = ApplicationData.Current.LocalFolder.GetFileAsync(path);
                return true;
            }
            catch (Exception)
            {
                return false;
            }          
        }

        public static void Move(string fromFullFileName, string toDir, string toName)
        {
            System.IO.File.Move(fromFullFileName,Path.Combine(toDir,toName));
            //var mover = ApplicationData.Current.LocalFolder.GetFileAsync(fromFullFileName);
            //var newDir = ApplicationData.Current.LocalFolder.GetFolderAsync(toDir);
            //mover.MoveAsync(newDir, toName);
        }

        public static byte[] ReadAllBytes(string path)
        {
            var stream = Hoot.File.OpenStreamForReadAsync(path);
            var mstream = new MemoryStream();
            var buf = new byte[10000];
            int i;
            while ((i = stream.Read(buf, 0, 10000)) > 0)
            {
                mstream.Write(buf,0,i);
                if (i < 10000) break;
            }
            stream.Close();
            stream.Dispose();
            return mstream.ToArray();
        }

        public enum CreationCollisionOption
        {
            ReplaceExisting,
            OpenIfExists

        }

        public static Stream OpenStreamForWriteAsync(string path, CreationCollisionOption option)
        {
            IsolatedStorageFile root = IsolatedStorageFile.GetUserStoreForApplication();
            if (option == CreationCollisionOption.ReplaceExisting)
            {
                try
                {
                    var stream =root.OpenFile(
                        path,
                        FileMode.CreateNew, 
                        FileAccess.Write);
                    return stream;
                }
                catch (Exception)
                {

                    var stream = root.OpenFile(path, FileMode.OpenOrCreate, FileAccess.Write);
                    stream.Seek(0, SeekOrigin.Begin);
                    stream.SetLength(0);
                    return stream;
                }
            }

            var stream2= root.OpenFile(path, FileMode.OpenOrCreate, FileAccess.Write);
            stream2.Seek(0, SeekOrigin.Begin);
            stream2.SetLength(0);
            return stream2;
        }

        public static void CloseStream(Stream stream)
        {
            stream.Close();
            stream.Dispose();
        }

        public static Stream OpenStreamForReadAsync(string path)
        {
            IsolatedStorageFile root = IsolatedStorageFile.GetUserStoreForApplication();
            return root.OpenFile(
                    path,
                    FileMode.Open,
                    FileAccess.Read);
        }

        public static string[] GetFiles(string folderPath)
        {
            IsolatedStorageFile root = IsolatedStorageFile.GetUserStoreForApplication();
            try
            {
                return root.GetFileNames(folderPath);
            }
            catch (Exception e)
            {
                return new string[0];
            }
        }

        public static void WriteAllBytes(string path, byte[] buf)
        {
            Stream stream = Hoot.File.OpenStreamForWriteAsync(path, CreationCollisionOption.ReplaceExisting);

            stream.Write(buf, 0, buf.Length);
            stream.Flush();
            stream.Close();
            stream.Dispose();

        }

        public static bool Delete(string filePath)
        {
            IsolatedStorageFile root = IsolatedStorageFile.GetUserStoreForApplication();
            try
            {
                root.DeleteFile(filePath);
                //System.IO.File.Delete(filePath);
                //StorageFile file = ApplicationData.Current.LocalFolder.GetFileAsync(filePath);
                //file.DeleteAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }

    public class FileInfo
    {
        public string FullName;
        public DateTime LastWriteTime = new DateTime();
        public int Length = 0;
        public FileInfo(string path)
        {
            FullName = path;
        }
    }

    public class Directory
    {
        public string FullName;
        public static Directory CreateDirectory(string path)
        {
            IsolatedStorageFile root = IsolatedStorageFile.GetUserStoreForApplication();
            try
            {
                root.CreateDirectory(path);
            }
            catch (Exception)
            {
            }      
            
            return new Directory { FullName = path };
        }
    }

    public static class PathHelper
    {
        public static string DirectorySeparatorChar
        {
            get
            {
                return "\\";
            }
        }
    }
}
