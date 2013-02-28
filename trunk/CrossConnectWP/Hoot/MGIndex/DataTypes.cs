﻿#region Header

// <copyright file="DataTypes.cs" company="Thomas Dilts">
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
using RaptorDB.Common;

namespace RaptorDB
{
    internal interface IGetBytes<T>
    {
        byte[] GetBytes(T obj);
        T GetObject(byte[] buffer, int offset, int count);
    }

    internal class RDBDataType<T>
    {
        public static IGetBytes<T> ByteHandler()
        {
            Type type = typeof(T);

            if (type == typeof(int)) return (IGetBytes<T>)new int_handler<T>();
            else if (type == typeof(uint)) return (IGetBytes<T>)new uint_handler<T>();
            else if (type == typeof(long)) return (IGetBytes<T>)new long_handler<T>();
            else if (type == typeof(Guid)) return (IGetBytes<T>)new guid_handler<T>();
            else if (type == typeof(string)) return (IGetBytes<T>)new string_handler<T>();
            else if (type == typeof(DateTime)) return (IGetBytes<T>)new datetime_handler<T>();
            else if (type == typeof(decimal)) return (IGetBytes<T>)new decimal_handler<T>();
            else if (type == typeof(short)) return (IGetBytes<T>)new short_handler<T>();
            else if (type == typeof(float)) return (IGetBytes<T>)new float_handler<T>();
            else if (type == typeof(byte)) return (IGetBytes<T>)new byte_handler<T>();
            else if (type == typeof(double)) return (IGetBytes<T>)new double_handler<T>();

            return null;
        }

        public static byte GetByteSize(byte keysize)
        {
            byte size = 4;
            Type t = typeof(T);

            if (t == typeof(int)) size = 4;
            if (t == typeof(uint)) size = 4;
            if (t == typeof(long)) size = 8;
            if (t == typeof(Guid)) size = 16;
            if (t == typeof(DateTime)) size = 8;
            if (t == typeof(decimal)) size = 16;
            if (t == typeof(float)) size = 4;
            if (t == typeof(short)) size = 2;
            if (t == typeof(string)) size = keysize;
            if (t == typeof(byte)) size = 1;
            if (t == typeof(double)) size = 8;

            return size;
        }

        internal static object GetEmpty()
        {
            Type t = typeof(T);

            if (t == typeof(string))
                return "";

            return default(T);
        }
    }

    #region [  handlers  ]

    internal class double_handler<T> : IGetBytes<double>
    {
        public byte[] GetBytes(double obj)
        {
            return BitConverter.GetBytes(obj);
        }

        public double GetObject(byte[] buffer, int offset, int count)
        {
            return BitConverter.ToDouble(buffer, offset);
        }
    }

    internal class byte_handler<T> : IGetBytes<byte>
    {
        public byte[] GetBytes(byte obj)
        {
            return new byte[1] { obj };
        }

        public byte GetObject(byte[] buffer, int offset, int count)
        {
            return buffer[offset];
        }
    }

    internal class float_handler<T> : IGetBytes<float>
    {
        public byte[] GetBytes(float obj)
        {
            return BitConverter.GetBytes(obj);
        }

        public float GetObject(byte[] buffer, int offset, int count)
        {
            return BitConverter.ToSingle(buffer, offset);
        }
    }

    internal class decimal_handler<T> : IGetBytes<decimal>
    {
        public byte[] GetBytes(decimal obj)
        {
            byte[] b = new byte[16];
            var bb = decimal.GetBits(obj);
            int index = 0;
            foreach (var d in bb)
            {
                byte[] db = Helper.GetBytes(d, false);
                Buffer.BlockCopy(db, 0, b, index, 4);
                index += 4;
            }

            return b;
        }

        public decimal GetObject(byte[] buffer, int offset, int count)
        {
            int[] i = new int[4];
            i[0] = Helper.ToInt32(buffer, offset);
            offset += 4;
            i[1] = Helper.ToInt32(buffer, offset);
            offset += 4;
            i[2] = Helper.ToInt32(buffer, offset);
            offset += 4;
            i[3] = Helper.ToInt32(buffer, offset);
            offset += 4;

            return new decimal(i);
        }
    }

    internal class short_handler<T> : IGetBytes<short>
    {
        public byte[] GetBytes(short obj)
        {
            return Helper.GetBytes(obj, false);
        }

        public short GetObject(byte[] buffer, int offset, int count)
        {
            return Helper.ToInt16(buffer, offset);
        }
    }

    internal class string_handler<T> : IGetBytes<string>
    {
        public byte[] GetBytes(string obj)
        {
            return Helper.GetBytes(obj);
        }

        public string GetObject(byte[] buffer, int offset, int count)
        {
            return Helper.GetString(buffer, offset, (short)count);
        }
    }

    internal class int_handler<T> : IGetBytes<int>
    {
        public byte[] GetBytes(int obj)
        {
            return Helper.GetBytes(obj, false);
        }

        public int GetObject(byte[] buffer, int offset, int count)
        {
            return Helper.ToInt32(buffer, offset);
        }
    }

    internal class uint_handler<T> : IGetBytes<uint>
    {
        public byte[] GetBytes(uint obj)
        {
            return Helper.GetBytes(obj, false);
        }

        public uint GetObject(byte[] buffer, int offset, int count)
        {
            return (uint)Helper.ToInt32(buffer, offset);
        }
    }

    internal class long_handler<T> : IGetBytes<long>
    {
        public byte[] GetBytes(long obj)
        {
            return Helper.GetBytes(obj, false);
        }

        public long GetObject(byte[] buffer, int offset, int count)
        {
            return Helper.ToInt64(buffer, offset);
        }
    }

    internal class guid_handler<T> : IGetBytes<Guid>
    {
        public byte[] GetBytes(Guid obj)
        {
            return obj.ToByteArray();
        }

        public Guid GetObject(byte[] buffer, int offset, int count)
        {
            byte[] b = new byte[16];
            Buffer.BlockCopy(buffer, offset, b, 0, 16);
            return new Guid(b);
        }
    }

    internal class datetime_handler<T> : IGetBytes<DateTime>
    {
        public byte[] GetBytes(DateTime obj)
        {
            return Helper.GetBytes(obj.Ticks, false);
        }

        public DateTime GetObject(byte[] buffer, int offset, int count)
        {
            long ticks = Helper.ToInt64(buffer, offset);

            return new DateTime(ticks);
        }
    }
    #endregion
}