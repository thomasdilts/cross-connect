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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hoot
{
    using System.IO;

    using Windows.Storage;

    public class File
    {
        public static async Task<bool> Exists(string path)
        {
            try
            {
                await ApplicationData.Current.LocalFolder.GetItemAsync(path);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public static async Task<bool> Exists(StorageFolder folder, string path)
        {
            try
            {
                await folder.GetItemAsync(path);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public static async Task Move(string fromFullFileName, string toDir, string toName)
        {
            var mover = await ApplicationData.Current.LocalFolder.GetFileAsync(fromFullFileName);
            var newDir = await ApplicationData.Current.LocalFolder.GetFolderAsync(toDir);
            await mover.MoveAsync(newDir, toName);
        }

        public static async Task<string[]> GetFiles(string folderPath, StorageFolder containingFolder = null)
        {
            if (containingFolder == null)
            {
                containingFolder = ApplicationData.Current.LocalFolder;
            }
            //IsolatedStorageFile root = IsolatedStorageFile.GetUserStoreForApplication();
            try
            {
                var newDir = await containingFolder.GetFolderAsync(folderPath);
                var storageFiles = await newDir.GetFilesAsync();
                var files = new string[storageFiles.Count()];
                for (int i = 0; i < storageFiles.Count(); i++)
                {
                    files[i] = storageFiles[i].Name;
                }
                return files;
            }
            catch (Exception e)
            {
                return new string[0];
            }
        }
        public static async Task<string[]> GetFolders(string folderPath, StorageFolder containingFolder)
        {
            //IsolatedStorageFile root = IsolatedStorageFile.GetUserStoreForApplication();
            try
            {
                var newDir = await containingFolder.GetFolderAsync(folderPath);
                var storageFiles = await newDir.GetFoldersAsync();
                var files = new string[storageFiles.Count()];
                for (int i = 0; i < storageFiles.Count(); i++)
                {
                    files[i] = storageFiles[i].Name;
                }
                return files;
            }
            catch (Exception e)
            {
                return new string[0];
            }
        }
        public static async Task<byte[]> ReadAllBytes(string path)
        {
            var stream = await ApplicationData.Current.LocalFolder.OpenStreamForReadAsync(path);
            var mstream = new MemoryStream();
            var buf = new byte[10000];
            int i;
            while ((i = await stream.ReadAsync(buf, 0, 10000)) > 0)
            {
                mstream.Write(buf,0,i);
                if (i < 10000) break;
            }

            stream.Dispose();
            return mstream.ToArray();
        }

        public static async Task WriteAllBytes(string path, byte[] buf)
        {
            var stream = await ApplicationData.Current.LocalFolder.OpenStreamForWriteAsync(path, CreationCollisionOption.ReplaceExisting);
            await stream.WriteAsync(buf, 0, buf.Length);
            await stream.FlushAsync();
            stream.Dispose();
        }

        public static async Task<bool> Delete(string filePath, StorageFolder containingFolder = null)
        {
            if (containingFolder == null)
            {
                containingFolder = ApplicationData.Current.LocalFolder;
            }
            try
            {
                StorageFile file = await containingFolder.GetFileAsync(filePath);
                await file.DeleteAsync();
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
        public static async Task<Directory> CreateDirectory(string path)
        {
            try
            {
                await ApplicationData.Current.LocalFolder.CreateFolderAsync(path);
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
