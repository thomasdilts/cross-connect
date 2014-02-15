// <copyright file="BitmapIndex.cs" company="Thomas Dilts">
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
using System.Text;
using System.IO;
using RaptorDB.Common;
using System.Threading;

namespace RaptorDB
{
    using System.Threading.Tasks;

    using Hoot;

    using Windows.Storage;

    internal class BitmapIndex
    {
        public async Task Initialize(string path, string filename)
        {
            _FileName = Path.GetFileNameWithoutExtension(filename);
            _Path = path;
            if (_Path.EndsWith(PathHelper.DirectorySeparatorChar.ToString()) == false)
                _Path += PathHelper.DirectorySeparatorChar.ToString();

            await Initialize();
        }

        private string _recExt = ".mgbmr";
        private string _bmpExt = ".mgbmp";
        private string _FileName = "";
        private string _Path = "";
        //private Stream _bitmapFileWriteOrg;
        //private Stream _bitmapFileWrite;
        //private Stream _bitmapFileRead;
        //private Stream _recordFileRead;
        //private Stream _recordFileWriteOrg;
        //private Stream _recordFileWrite;
        private long _lastBitmapOffset = 0;
        private int _lastRecordNumber = 0;
        private object _lock = new object();
        private SafeDictionary<int, WAHBitArray> _cache = new SafeDictionary<int, WAHBitArray>();
        private SafeDictionary<int, long> _offsetCache = new SafeDictionary<int, long>();
        //private ILog log = LogManager.GetLogger(typeof(BitmapIndex));
        //private bool _optimizing = false;
        //private int _threadSleepTime = 10;
        private bool _shutdownDone = false;

        #region [  P U B L I C  ]
        public async Task Shutdown()
        {
            //while (_optimizing) Thread.Sleep(_threadSleepTime);

            //log.Debug("Shutdown BitmapIndex");

            await InternalShutdown();
        }

        public int GetFreeRecordNumber()
        {
            //while (_optimizing) Thread.Sleep(_threadSleepTime);

            int i = _lastRecordNumber++;

            _cache.Add(i, new WAHBitArray());
            return i;
        }

        public async Task Commit(bool freeMemory)
        {
            //while (_optimizing) Thread.Sleep(_threadSleepTime);

            int[] keys = _cache.Keys();
            Array.Sort(keys);
            var _bmpFileWrite = await ApplicationData.Current.LocalFolder.OpenStreamForWriteAsync(_Path + _FileName + _bmpExt, CreationCollisionOption.OpenIfExists);
            var _recFileWrite = await ApplicationData.Current.LocalFolder.OpenStreamForWriteAsync(_Path + _FileName + _recExt, CreationCollisionOption.ReplaceExisting);
            //_recFileWrite.Seek(0L, SeekOrigin.End);
            _bmpFileWrite.Seek(0L, SeekOrigin.End);
            foreach (int k in keys)
            {
                var bmp = _cache[k];
                if (bmp.isDirty)
                {
                    SaveBitmap(k, bmp, _recFileWrite, _bmpFileWrite);
                    bmp.FreeMemory();
                    bmp.isDirty = false;
                }
            }
            _recFileWrite.Flush();
            _recFileWrite.Dispose();
            _bmpFileWrite.Flush();
            _bmpFileWrite.Dispose();
            if (freeMemory)
            {
                _cache = new SafeDictionary<int, WAHBitArray>();
            }
        }

        public async Task SetDuplicate(int bitmaprecno, int record)
        {
            //while (_optimizing) Thread.Sleep(_threadSleepTime);

            WAHBitArray ba = null;

            ba = await GetBitmap(bitmaprecno);

            ba.Set(record, true);
        }

        public async Task<WAHBitArray> GetBitmap(int recno)
        {
            //while (_optimizing) Thread.Sleep(_threadSleepTime);

            return await internalGetBitmap(recno);
        }

        private object _oplock = new object();
        public async Task Optimize()
        {
            //lock (_oplock)
            //    lock (_readlock)
            //        lock (_writelock)
            //        {
                        //_optimizing = true;
                        Flush();

                        //if (File.Exists(_Path + _FileName + "$" + _bmpExt))
                            await File.Delete(_Path + _FileName + "$" + _bmpExt);

                        //if (File.Exists(_Path + _FileName + "$" + _recExt))
                            await File.Delete(_Path + _FileName + "$" + _recExt);
                            Stream _newrec = await ApplicationData.Current.LocalFolder.OpenStreamForWriteAsync(_Path + _FileName + "$" + _recExt, CreationCollisionOption.OpenIfExists);
                            Stream _newbmp = await ApplicationData.Current.LocalFolder.OpenStreamForWriteAsync(_Path + _FileName + "$" + _bmpExt, CreationCollisionOption.OpenIfExists);

                        long newoffset = 0;
                        var readstream = await ApplicationData.Current.LocalFolder.OpenStreamForReadAsync(_Path + _FileName + _recExt);
                        var readbmpstream = await ApplicationData.Current.LocalFolder.OpenStreamForReadAsync(_Path + _FileName + _bmpExt);
                        int c = (int)(readstream.Length / 8);
                        for (int i = 0; i < c; i++)
                        {
                            long offset = ReadRecordOffset(i, readstream);

                            byte[] b = ReadBMPData(offset, readbmpstream);
                            if (b == null)
                            {
                                //_optimizing = false;
                                throw new Exception("bitmap index file is corrupted");
                            }

                            _newrec.Write(Helper.GetBytes(newoffset, false), 0, 8); 
                            newoffset += b.Length;
                            _newbmp.Write(b, 0, b.Length);

                        }
                        readstream.Dispose();
                        readbmpstream.Dispose();
                        _newbmp.Flush();
                        _newbmp.Dispose();
                        _newrec.Flush();
                        _newrec.Dispose();

                        await InternalShutdown();

                        await File.Delete(_Path + _FileName + _bmpExt);
                        await File.Delete(_Path + _FileName + _recExt);

                        await File.Move(_Path + _FileName + "$" + _bmpExt, _Path , _FileName + _bmpExt);
                        await File.Move(_Path + _FileName + "$" + _recExt, _Path , _FileName + _recExt);

                        await Initialize();
                        //_optimizing = false;
                    //}
        }
        #endregion


        #region [  P R I V A T E  ]
        private byte[] ReadBMPData(long offset, Stream readstream)
        {
            readstream.Seek(offset, SeekOrigin.Begin);

            byte[] b = new byte[8];

            readstream.Read(b, 0, 8);
            if (b[0] == (byte)'B' && b[1] == (byte)'M' && b[7] == 0)
            {
                int c = Helper.ToInt32(b, 2) * 4 + 8;
                byte[] data = new byte[c];
                readstream.Seek(offset, SeekOrigin.Begin);
                readstream.Read(data, 0, c);
                return data;
            }
            return null;
        }

        private long ReadRecordOffset(int recnum, Stream readstream)
        {
            byte[] b = new byte[8];
            long off = ((long)recnum) * 8;
            readstream.Seek(off, SeekOrigin.Begin);
            readstream.Read(b, 0, 8);
            return Helper.ToInt64(b, 0);
        }

        private async Task Initialize()
        {
            Stream _recordFileCreate;
            if (await File.Exists(_Path + _FileName + _recExt))
            {
                _recordFileCreate =
                    await
                    ApplicationData.Current.LocalFolder.OpenStreamForReadAsync(
                        _Path + _FileName + _recExt);
            }
            else
            {
                _recordFileCreate =
                    await
                    ApplicationData.Current.LocalFolder.OpenStreamForWriteAsync(
                        _Path + _FileName + _recExt, CreationCollisionOption.OpenIfExists);
            }

            Stream _bitmapFileWriteOrg;
            if (await File.Exists(_Path + _FileName + _bmpExt))
            {
                _bitmapFileWriteOrg = await ApplicationData.Current.LocalFolder.OpenStreamForReadAsync(_Path + _FileName + _bmpExt);
            }
            else
            {
                _bitmapFileWriteOrg = await ApplicationData.Current.LocalFolder.OpenStreamForWriteAsync(_Path + _FileName + _bmpExt, CreationCollisionOption.OpenIfExists);
            }


            _bitmapFileWriteOrg.Seek(0L, SeekOrigin.End);
            _lastBitmapOffset = _bitmapFileWriteOrg.Length;

            _bitmapFileWriteOrg.Dispose();
            _lastRecordNumber = (int)(_recordFileCreate.Length / 8);
            _recordFileCreate.Dispose();
            _shutdownDone = false;
        }

        private async Task InternalShutdown()
        {
            bool d1 = false;
            bool d2 = false;
            Flush();
            if (_shutdownDone == false)
            {
                var _recordFileWrite = await ApplicationData.Current.LocalFolder.OpenStreamForWriteAsync(_Path + _FileName + _recExt, CreationCollisionOption.OpenIfExists);
                var _bitmapFileWrite = await ApplicationData.Current.LocalFolder.OpenStreamForWriteAsync(_Path + _FileName + _bmpExt, CreationCollisionOption.OpenIfExists);
                _bitmapFileWrite.Seek(0L, SeekOrigin.End);
                _recordFileWrite.Seek(0L, SeekOrigin.End);
                if (_recordFileWrite.Length == 0) d1 = true;
                if (_bitmapFileWrite.Length == 0) d2 = true;
                _bitmapFileWrite.Dispose();
                _recordFileWrite.Dispose();
                if (d1)
                    await File.Delete(_Path + _FileName + _recExt);
                if (d2)
                    await File.Delete(_Path + _FileName + _bmpExt);
                _shutdownDone = true;
            }
        }

        private void Flush()
        {
        }

        private object _readlock = new object();
        private async Task< WAHBitArray> internalGetBitmap(int recno)
        {
            //lock (_readlock)
            //{
                WAHBitArray ba = new WAHBitArray();
                if (recno == -1)
                    return ba;

                if (_cache.TryGetValue(recno, out ba))
                {
                    return ba;
                }
                else
                {
                    long offset = 0;
                    if (_offsetCache.TryGetValue(recno, out offset) == false)
                    {
                        var readstream = await ApplicationData.Current.LocalFolder.OpenStreamForReadAsync(_Path + _FileName + _recExt);
                        offset = ReadRecordOffset(recno, readstream);
                        readstream.Dispose();
                        _offsetCache.Add(recno, offset);
                    }
                    ba = await LoadBitmap(offset);

                    _cache.Add(recno, ba);

                    return ba;
                }
           // }
        }

        private object _writelock = new object();
        private void SaveBitmap(int recno, WAHBitArray bmp, Stream writeStreamRec, Stream writeStreamBmp)
        {
            //lock (_writelock)
            //{
            long offset = SaveBitmapToFile(bmp, writeStreamBmp);
                long v;
                if (_offsetCache.TryGetValue(recno, out v))
                    _offsetCache[recno] = offset;
                else
                    _offsetCache.Add(recno, offset);

                long pointer = ((long)recno) * 8;
                //writeStreamRec.Seek(pointer, SeekOrigin.Begin);
                byte[] b = new byte[8];
                b = Helper.GetBytes(offset, false);
                writeStreamRec.Write(b, 0, 8);
            //}
        }

        //-----------------------------------------------------------------
        // BITMAP FILE FORMAT
        //    0  'B','M'
        //    2  uint count = 4 bytes
        //    6  Bitmap type :
        //                0 = int record list   
        //                1 = uint bitmap
        //                2 = rec# indexes
        //    7  '0'
        //    8  uint data
        //-----------------------------------------------------------------
        private long SaveBitmapToFile(WAHBitArray bmp, Stream _bitmapFileWrite)
        {
            long off = _lastBitmapOffset;
            WAHBitArray.TYPE t;
            uint[] bits = bmp.GetCompressed(out t);

            byte[] b = new byte[bits.Length * 4 + 8];
            // write header data
            b[0] = ((byte)'B');
            b[1] = ((byte)'M');
            Buffer.BlockCopy(Helper.GetBytes(bits.Length, false), 0, b, 2, 4);

            b[6] = (byte)t;
            b[7] = (byte)(0);

            for (int i = 0; i < bits.Length; i++)
            {
                byte[] u = Helper.GetBytes((int)bits[i], false);
                Buffer.BlockCopy(u, 0, b, i * 4 + 8, 4);
            }
            _bitmapFileWrite.Write(b, 0, b.Length);
            _lastBitmapOffset += b.Length;
            return off;
        }

        private async Task<WAHBitArray> LoadBitmap(long offset)
        {
            WAHBitArray bc = new WAHBitArray();
            if (offset == -1)
                return bc;

            List<uint> ar = new List<uint>();
            WAHBitArray.TYPE type = WAHBitArray.TYPE.WAH;
            Stream bmp = await ApplicationData.Current.LocalFolder.OpenStreamForReadAsync(_Path + _FileName + _bmpExt); 
            {
                bmp.Seek(offset, SeekOrigin.Begin);

                byte[] b = new byte[8];

                bmp.Read(b, 0, 8);
                if (b[0] == (byte)'B' && b[1] == (byte)'M' && b[7] == 0)
                {
                    type = (WAHBitArray.TYPE)Enum.ToObject(typeof(WAHBitArray.TYPE), b[6]);
                    int c = Helper.ToInt32(b, 2);
                    byte[] buf = new byte[c * 4];
                    bmp.Read(buf, 0, c * 4);
                    for (int i = 0; i < c; i++)
                    {
                        ar.Add((uint)Helper.ToInt32(buf, i * 4));
                    }
                }
            }
            bmp.Dispose();
            bc = new WAHBitArray(type, ar.ToArray());

            return bc;
        }
        #endregion
    }
}
