#region Header

// <copyright file="Global.cs" company="Thomas Dilts">
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

    public class Global
    {
        public static int BitmapOffsetSwitchOverCount = 10;

        public static ushort PageItemCount = 10000;

        public static int SaveIndexToDiskTimerSeconds = 3000;

        public static byte DefaultStringKeySize = 60;

        public static bool FlushStorageFileImmetiatley = false;

        public static bool FreeBitmapMemoryOnSave = false;
    }

