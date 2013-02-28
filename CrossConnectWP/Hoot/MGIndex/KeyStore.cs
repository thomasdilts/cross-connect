﻿#region Header

// <copyright file="KeyStore.cs" company="Thomas Dilts">
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
using System.Text;
using System.IO;
using RaptorDB.Common;

namespace RaptorDB
{

    using System.Windows.Threading;

    using Hoot;

    //using Windows.UI.Xaml;
/*
    #region [   KeyStoreGuid   ]
    internal class KeyStoreGuid : IDisposable
    {
        public KeyStoreGuid(string filename)
        {
            _db = KeyStore<int>.Open(filename, true);
        }

        KeyStore<int> _db;

        public void Set(Guid key, string val)
        {
            Set(key, Encoding.Unicode.GetBytes(val));
        }

        public int Set(Guid key, byte[] val)
        {
            byte[] bkey = key.ToByteArray();
            int hc = (int)Helper.MurMur.Hash(bkey);
            MemoryStream ms = new MemoryStream();
            ms.Write(Helper.GetBytes(bkey.Length, false), 0, 4);
            ms.Write(bkey, 0, bkey.Length);
            ms.Write(val, 0, val.Length);

            return _db.Set(hc, ms.ToArray());
        }

        public bool Get(Guid key, out string val)
        {
            val = null;
            byte[] bval;
            bool b = Get(key, out bval);
            if (b)
            {
                val = Encoding.Unicode.GetString(bval,0,bval.Length);
            }
            return b;
        }

        public bool Get(Guid key, out byte[] val)
        {
            val = null;
            byte[] bkey = key.ToByteArray();
            int hc = (int)Helper.MurMur.Hash(bkey);

            if (_db.Get(hc, out val))
            {
                // unpack data
                byte[] g = null;
                if (UnpackData(val, out val, out g))
                {
                    if (Helper.CompareMemCmp(bkey, g) != 0)
                    {
                        // if data not equal check duplicates (hash conflict)
                        List<int> ints = new List<int>(_db.GetDuplicates(hc));
                        ints.Reverse();
                        foreach (int i in ints)
                        {
                            byte[] bb = _db.FetchRecordBytes(i);
                            if (UnpackData(bb, out val, out g))
                            {
                                if (Helper.CompareMemCmp(bkey, g) == 0)
                                    return true;
                            }
                        }
                        return false;
                    }
                    return true;
                }
            }
            return false;
        }

        public void SaveIndex()
        {
            _db.SaveIndex();
        }

        public void Shutdown()
        {
            _db.Shutdown();
        }

        public void Dispose()
        {
            _db.Shutdown();
        }

        public byte[] FetchRecordBytes(int record)
        {
            return _db.FetchRecordBytes(record);
        }

        public int Count()
        {
            return (int)_db.Count();
        }

        public int RecordCount()
        {
            return (int)_db.RecordCount();
        }

        private bool UnpackData(byte[] buffer, out byte[] val, out byte[] key)
        {
            int len = Helper.ToInt32(buffer, 0, false);
            key = new byte[len];
            Buffer.BlockCopy(buffer, 4, key, 0, len);
            val = new byte[buffer.Length - 4 - len];
            Buffer.BlockCopy(buffer, 4 + len, val, 0, buffer.Length - 4 - len);

            return true;
        }

        internal byte[] Get(int recnumber, out Guid docid)
        {
            bool isdeleted = false;
            return Get(recnumber, out docid, out isdeleted);
        }

        public bool RemoveKey(Guid key)
        {
            byte[] bkey = key.ToByteArray();
            int hc = (int)Helper.MurMur.Hash(bkey);
            MemoryStream ms = new MemoryStream();
            ms.Write(Helper.GetBytes(bkey.Length, false), 0, 4);
            ms.Write(bkey, 0, bkey.Length);
            return _db.Delete(hc, ms.ToArray());
        }

        internal byte[] Get(int recnumber, out Guid docid, out bool isdeleted)
        {
            docid = Guid.Empty;
            byte[] buffer = _db.FetchRecordBytes(recnumber, out isdeleted);
            if (buffer == null) return null;
            if (buffer.Length == 0) return null;
            byte[] key;
            byte[] val;
            // unpack data
            UnpackData(buffer, out val, out key);
            docid = new Guid(key);
            return val;
        }

        internal int CopyTo(StorageFile<int> backup, int start)
        {
            return _db.CopyTo(backup, start);
        }
    }
    #endregion
*/
    internal class KeyStore<T> : IDisposable where T : IComparable<T>
    {

        private string _Path = "";
        private string _FileName = "";
        private byte _MaxKeySize;
        private StorageFile<T> _archive;
        private MGIndex<T> _index;
        private string _datExtension = ".mgdat";
        private string _idxExtension = ".mgidx";
        IGetBytes<T> _T = null;
        private DispatcherTimer _saveTimer;
        private BoolIndex _deleted;


        public static KeyStore<T> Open(string Filename, bool AllowDuplicateKeys)
        {
            var keystore = new KeyStore<T>();
            keystore.Initialize(Filename, Global.DefaultStringKeySize, AllowDuplicateKeys);
            return keystore; 
        }

        public static KeyStore<T> Open(string Filename, byte MaxKeySize, bool AllowDuplicateKeys)
        {
            var keystore = new KeyStore<T>();
            keystore.Initialize(Filename, MaxKeySize, AllowDuplicateKeys);
            return keystore;
        }

        object _savelock = new object();
        public void SaveIndex()
        {
            if (_index == null)
                return;
            //lock (_savelock)
            //{
                //log.Debug("saving to disk");
                _index.SaveIndex();
                _deleted.SaveIndex();
                //log.Debug("index saved");
            //}
        }

        public IEnumerable<int> GetDuplicates(T key)
        {
            // get duplicates from index
            return _index.GetDuplicates(key);
        }

        public byte[] FetchRecordBytes(int record)
        {
            return _archive.ReadData(record);
        }

        public bool RemoveKey(T key)
        {
            // remove and store key in storage file
            byte[] bkey = _T.GetBytes(key);
            MemoryStream ms = new MemoryStream();
            ms.Write(Helper.GetBytes(bkey.Length, false), 0, 4);
            ms.Write(bkey, 0, bkey.Length);
            return Delete(key, ms.ToArray());
        }

        public long Count()
        {
            int c = _archive.Count();
            return c - _deleted.GetBits().CountOnes() * 2;
        }

        public bool Get(T key, out string val)
        {
            byte[] b = null;
            val = "";
            bool ret = Get(key, out b);
            if (ret)
                val = Encoding.Unicode.GetString(b,0,b.Length);
            return ret;
        }

        public bool Get(T key, out byte[] val)
        {
            int off;
            val = null;
            T k = key;
            // search index
            if (_index.Get(k, out off))
            {
                val = _archive.ReadData(off);
                return true;
            }
            return false;
        }

        public int Set(T key, string data)
        {
            return Set(key, Encoding.Unicode.GetBytes(data));
        }

        public int Set(T key, byte[] data)
        {
            int recno = -1;
            // save to storage
            recno = _archive.WriteData(key, data, false);
            // save to index
            _index.Set(key, recno);

            return recno;
        }

        private object _shutdownlock = new object();
        public void Shutdown()
        {
            //lock (_shutdownlock)
            //{
                if (_index != null)
                {
                    //log.Debug("Shutting down");
                }
                else return;
                SaveIndex();
                SaveLastRecord();

                if (_deleted != null)
                    _deleted.Shutdown();
                if (_index != null)
                    _index.Shutdown();
                if (_archive != null)
                    _archive.Shutdown();
                _index = null;
                _archive = null;
                _deleted = null;
                //log.Debug("Shutting down log");
                //LogManager.Shutdown();
            //}
        }

        public void Dispose()
        {
            Shutdown();
        }

        #region [            P R I V A T E     M E T H O D S              ]
        private void SaveLastRecord()
        {
            // save the last record number in the index file
            _index.SaveLastRecordNumber(_archive.Count());
        }

        private void Initialize(string filename, byte maxkeysize, bool AllowDuplicateKeys)
        {
            _MaxKeySize = RDBDataType<T>.GetByteSize(maxkeysize);
            _T = RDBDataType<T>.ByteHandler();

            _Path = Path.GetDirectoryName(filename);
            Directory.CreateDirectory(_Path);

            _FileName = Path.GetFileNameWithoutExtension(filename);
            string db = _Path + PathHelper.DirectorySeparatorChar + _FileName + _datExtension;
            string idx = _Path + PathHelper.DirectorySeparatorChar + _FileName + _idxExtension;

            //LogManager.Configure(_Path + Path.DirectorySeparatorChar + _FileName + ".txt", 500, false);

            _index = new MGIndex<T>();
            _index.Initialize(_Path, _FileName + _idxExtension, _MaxKeySize, Global.PageItemCount, AllowDuplicateKeys);
            _archive = new StorageFile<T>();
            _archive.Initialize(db, false);

            _deleted = new BoolIndex();
            _deleted.Initialize(_Path, _FileName + "_deleted.idx");
            _archive.SkipDateTime = true;

            //log.Debug("Current Count = " + RecordCount().ToString("#,0"));

            CheckIndexState();

            //log.Debug("Starting save timer");
            _saveTimer = new DispatcherTimer();
            _saveTimer.Interval = new TimeSpan(0, 0, 0, 500);
            _saveTimer.Tick += _saveTimer_Elapsed;
            _saveTimer.Start();

        }

        private void CheckIndexState()
        {
            //log.Debug("Checking Index state...");
            int last = _index.GetLastIndexedRecordNumber();
            int count = _archive.Count();
            if (last < count)
            {
                //log.Debug("Rebuilding index...");
                //log.Debug("   last index count = " + last);
                //log.Debug("   data items count = " + count);
                // check last index record and archive record
                //       rebuild index if needed
                for (int i = last; i < count; i++)
                {
                    bool deleted = false;
                    T key = _archive.GetKey(i, out deleted);
                    if (deleted == false)
                        _index.Set(key, i);
                    else
                        _index.RemoveKey(key);

                    if (i % 100000 == 0)
                    {
                        //log.Debug("100,000 items re-indexed");
                    }
                }
                //log.Debug("Rebuild index done.");
            }
        }

        void _saveTimer_Elapsed(object sender, object e)
        {
            _saveTimer.Stop();
            SaveIndex();
        }

        #endregion

        internal int RecordCount()
        {
            return _archive.Count();
        }

        internal byte[] FetchRecordBytes(int record, out bool isdeleted)
        {
            return _archive.ReadData(record, out isdeleted);
        }

        internal bool Delete(T id, byte[] data)
        {
            // write a delete record
            int rec = _archive.WriteData(id, data, true);
            _deleted.Set(true, rec);
            return _index.RemoveKey(id);
        }

        internal int CopyTo(StorageFile<int> storagefile, int start)
        {
            return _archive.CopyTo(storagefile, start);
        }
    }
}
