// <copyright file="MurMurHash2.cs" company="Thomas Dilts">
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

namespace RaptorDB
{
    internal class MurmurHash2Unsafe
    {
        //public UInt32 Hash(Byte[] data)
        //{
        //    return Hash(data, 0xc58f1a7b);
        //}
        //const UInt32 m = 0x5bd1e995;
        //const Int32 r = 24;

        //public unsafe UInt32 Hash(Byte[] data, UInt32 seed)
        //{
        //    Int32 length = data.Length;
        //    if (length == 0)
        //        return 0;
        //    UInt32 h = seed ^ (UInt32)length;
        //    Int32 remainingBytes = length & 3; // mod 4
        //    Int32 numberOfLoops = length >> 2; // div 4
        //    fixed (byte* firstByte = &(data[0]))
        //    {
        //        UInt32* realData = (UInt32*)firstByte;
        //        while (numberOfLoops != 0)
        //        {
        //            UInt32 k = *realData;
        //            k *= m;
        //            k ^= k >> r;
        //            k *= m;

        //            h *= m;
        //            h ^= k;
        //            numberOfLoops--;
        //            realData++;
        //        }
        //        switch (remainingBytes)
        //        {
        //            case 3:
        //                h ^= (UInt16)(*realData);
        //                h ^= ((UInt32)(*(((Byte*)(realData)) + 2))) << 16;
        //                h *= m;
        //                break;
        //            case 2:
        //                h ^= (UInt16)(*realData);
        //                h *= m;
        //                break;
        //            case 1:
        //                h ^= *((Byte*)realData);
        //                h *= m;
        //                break;
        //            default:
        //                break;
        //        }
        //    }

        //    // Do a few final mixes of the hash to ensure the last few
        //    // bytes are well-incorporated.

        //    h ^= h >> 13;
        //    h *= m;
        //    h ^= h >> 15;

        //    return h;
        //}
    }
}
