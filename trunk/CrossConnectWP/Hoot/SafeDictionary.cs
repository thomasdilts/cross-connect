#region Header

// <copyright file="SaftDictionary.cs" company="Thomas Dilts">
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
using System.Runtime.InteropServices;
using System.Text;
using System.Linq;

namespace RaptorDB.Common
{
    internal class SafeDictionary<TKey, TValue>
    {
        private readonly object _Padlock = new object();
        private readonly Dictionary<TKey, TValue> _Dictionary = null;

        public SafeDictionary(int capacity)
        {
            _Dictionary = new Dictionary<TKey, TValue>(capacity);
        }

        public SafeDictionary()
        {
            _Dictionary = new Dictionary<TKey, TValue>();
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            lock (_Padlock)
                return _Dictionary.TryGetValue(key, out value);
        }

        public TValue this[TKey key]
        {
            get
            {
                lock (_Padlock)
                    return _Dictionary[key];
            }
            set
            {
                lock (_Padlock)
                    _Dictionary[key] = value;
            }
        }

        public int Count
        {
            get { lock (_Padlock) return _Dictionary.Count; }
        }

        public ICollection<KeyValuePair<TKey, TValue>> GetList()
        {
            return (ICollection<KeyValuePair<TKey, TValue>>)_Dictionary;
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return ((ICollection<KeyValuePair<TKey, TValue>>)_Dictionary).GetEnumerator();
        }

        public void Add(TKey key, TValue value)
        {
            lock (_Padlock)
            {
                if (_Dictionary.ContainsKey(key) == false)
                    _Dictionary.Add(key, value);
            }
        }

        public TKey[] Keys()
        {
            lock (_Padlock)
            {
                TKey[] keys = new TKey[_Dictionary.Keys.Count];
                _Dictionary.Keys.CopyTo(keys, 0);
                return keys;
            }
        }

        public bool Remove(TKey key)
        {
            lock (_Padlock)
                return _Dictionary.Remove(key);
        }
    }

    internal class SafeSortedList<T, V>
    {
        private object _padlock = new object();
        Dictionary<T, V> _list = new Dictionary<T, V>();

        public int Count
        {
            get { lock (_padlock) return _list.Count; }
        }

        public void Add(T key, V val)
        {
            lock (_padlock)
                _list.Add(key, val);
        }

        public void Remove(T key)
        {
            if (key == null)
                return;
            lock (_padlock)
                _list.Remove(key);
        }

        public T GetKey(int index)
        {

            lock (_padlock) return _list.Keys.ToList()[index];
        }

        public V GetValue(int index)
        {
            lock (_padlock) return _list.Values.ToList()[index];
        }
    }

    //------------------------------------------------------------------------------------------------------------------

    internal static class FastDateTime
    {
        public static TimeSpan LocalUtcOffset;

        public static DateTime Now
        {
            get { return DateTime.UtcNow + LocalUtcOffset; }
        }

        static FastDateTime()
        {
            LocalUtcOffset = TimeZoneInfo.Utc.GetUtcOffset(DateTime.Now);
        }
    }

    //------------------------------------------------------------------------------------------------------------------

    internal static class Helper
    {
        public static MurmurHash2Unsafe MurMur = new MurmurHash2Unsafe();
        //public static int CompareMemCmp(byte[] left, byte[] right)
        //{
        //    int c = left.Length;
        //    if (c > right.Length)
        //        c = right.Length;
        //    return memcmp(left, right, c);
        //}

        //[DllImport("msvcrt.dll", CallingConvention = CallingConvention.Cdecl)]
        //private static extern int memcmp(byte[] arr1, byte[] arr2, int cnt);

        //internal static int ToInt32(byte[] value, int startIndex, bool reverse)
        //{
        //    if (reverse)
        //    {
        //        byte[] b = new byte[4];
        //        Buffer.BlockCopy(value, startIndex, b, 0, 4);
        //        Array.Reverse(b);
        //        return ToInt32(b, 0);
        //    }

        //    return ToInt32(value, startIndex);
        //}

        internal static /*unsafe*/ int ToInt32(byte[] value, int startIndex)
        {
            byte[] bytes = new byte[4];
            Array.Copy(value,startIndex,bytes,0,4);
            // If the system architecture is little-endian (that is, little end first), 
            // reverse the byte array. 
            //if (BitConverter.IsLittleEndian)
            //    Array.Reverse(bytes);

            return BitConverter.ToInt32(bytes, 0);

            //fixed (byte* numRef = &(value[startIndex]))
            //{
            //    return *((int*)numRef);
            //}
        }

        //internal static unsafe long ToInt64(byte[] value, int startIndex, bool reverse)
        //{
        //    if (reverse)
        //    {
        //        byte[] b = new byte[8];
        //        Buffer.BlockCopy(value, startIndex, b, 0, 8);
        //        Array.Reverse(b);
        //        return ToInt64(b, 0);
        //    }
        //    return ToInt64(value, startIndex);
        //}

        internal static /*unsafe*/ long ToInt64(byte[] value, int startIndex)
        {
            byte[] bytes = new byte[8];
            Array.Copy(value, startIndex, bytes, 0, 8);
            // If the system architecture is little-endian (that is, little end first), 
            // reverse the byte array. 
            //if (BitConverter.IsLittleEndian)
            //    Array.Reverse(bytes);

            return BitConverter.ToInt64(bytes, 0); 
            
            //fixed (byte* numRef = &(value[startIndex]))
            //{
            //    return *(((long*)numRef));
            //}
        }

        //internal static unsafe short ToInt16(byte[] value, int startIndex, bool reverse)
        //{
        //    if (reverse)
        //    {
        //        byte[] b = new byte[2];
        //        Buffer.BlockCopy(value, startIndex, b, 0, 2);
        //        Array.Reverse(b);
        //        return ToInt16(b, 0);
        //    }
        //    return ToInt16(value, startIndex);
        //}

        internal static /*unsafe*/ short ToInt16(byte[] value, int startIndex)
        {
            byte[] bytes = new byte[2];
            Array.Copy(value, startIndex, bytes, 0, 2);
            // If the system architecture is little-endian (that is, little end first), 
            // reverse the byte array. 
            //if (BitConverter.IsLittleEndian)
            //    Array.Reverse(bytes);

            return BitConverter.ToInt16(bytes, 0);
            //fixed (byte* numRef = &(value[startIndex]))
            //{
            //    return *(((short*)numRef));
            //}
        }

        internal static /*unsafe*/ byte[] GetBytes(long num, bool reverse)
        {
            byte[] buffer = BitConverter.GetBytes(num);
            if(reverse)
                Array.Reverse(buffer);
            return buffer;
            //byte[] buffer = new byte[8];
            //fixed (byte* numRef = buffer)
            //{
            //    *((long*)numRef) = num;
            //}
            //if (reverse)
            //    Array.Reverse(buffer);
            //return buffer;
        }

        public static /*unsafe*/ byte[] GetBytes(int num, bool reverse)
        {
            byte[] buffer = BitConverter.GetBytes(num);
            if (reverse)
                Array.Reverse(buffer);
            return buffer;
            //byte[] buffer = new byte[4];
            //fixed (byte* numRef = buffer)
            //{
            //    *((int*)numRef) = num;
            //}
            //if (reverse)
            //    Array.Reverse(buffer);
            //return buffer;
        }

        public static /*unsafe*/ byte[] GetBytes(short num, bool reverse)
        {
            byte[] buffer = BitConverter.GetBytes(num);
            if (reverse)
                Array.Reverse(buffer);
            return buffer; 
            //byte[] buffer = new byte[2];
            //fixed (byte* numRef = buffer)
            //{
            //    *((short*)numRef) = num;
            //}
            //if (reverse)
            //    Array.Reverse(buffer);
            //return buffer;
        }

        public static byte[] GetBytes(string s)
        {
            return Encoding.UTF8.GetBytes(s);
        }

        internal static string GetString(byte[] buffer, int index, short keylength)
        {
            return Encoding.UTF8.GetString(buffer, index, keylength);
        }
    }
}
