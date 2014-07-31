#region Header

// <copyright file="RawGenTextReader.cs" company="Thomas Dilts">
// CrossConnect Bible and Bible Commentary Reader for CrossWire.org
// Copyright (C) 2011 Thomas Dilts
// This program is free software: you can redistribute it and/or modify
// it under the +terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see http://www.gnu.org/licenses/.
// </copyright>
// <summary>
// Email: thomas@cross-connect.se
// </summary>
// <author>Thomas Dilts</author>

#endregion Header

namespace Sword.reader
{
    //using Ionic.Zlib;
    using ComponentAce.Compression.Libs.zlib;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Runtime.Serialization;
    using System.Text;
    using System.Threading.Tasks;
    using System.Xml;

    using Windows.Storage;

    [DataContract]
    public class DictionaryZldDefReader : DictionaryRawDefReader
    {
        public class SectionEntry
        {
            public long PositionInDat;
            public long Length;
        }
        public List<SectionEntry> SectionEntries = new List<SectionEntry>();

        public DictionaryZldDefReader(string path, string iso2DigitLangCode, bool isIsoEncoding, string WindowMatchingKey)
            : base(path, iso2DigitLangCode, isIsoEncoding, WindowMatchingKey)
        {
            this.Serial.Iso2DigitLangCode = iso2DigitLangCode;
            this.Serial.Path = path;
            this.Serial.IsIsoEncoding = isIsoEncoding;
        }

        protected override async Task<bool> ReloadSettingsFile()
        {
            this.SectionEntries = new List<SectionEntry>();
            if (string.IsNullOrEmpty(this.Serial.Path))
            {
                return false;
            }
            try
            {
                Stream fs =
                    await
                    ApplicationData.Current.LocalFolder.OpenStreamForReadAsync(
                        this.Serial.Path.Replace("/", "\\") + ".zdx");
                bool isEnd;
                long index;

                while ((index = GetintFromStream(fs, out isEnd)) > -100 && !isEnd)
                {
                    var entry = new SectionEntry();
                    entry.PositionInDat = index;
                    entry.Length = GetintFromStream(fs, out isEnd);
                    this.SectionEntries.Add(entry);
                }
                fs.Dispose();
            }
            catch (Exception ee)
            {

            }
            return true;
        }
        protected override async Task<byte[]> GetChapterBytes()
        {
            var sectionedBuffer = new byte[0];
            try
            {
                string filenameComplete = this.Serial.Path + ".zdt";
                var fs =
                    await
                    ApplicationData.Current.LocalFolder.OpenStreamForReadAsync(filenameComplete.Replace("/", "\\"));
                var entry = this.SectionEntries[(int)this.PositionInDat];


                fs.Seek(entry.PositionInDat, SeekOrigin.Begin);
                var sectionCompressed = new byte[entry.Length];
                fs.Read(sectionCompressed, 0, (int)entry.Length);
                fs.Dispose();
                // create a memorystream from the data
                MemoryStream memoryStream = new MemoryStream(sectionCompressed);

                // unzip the section
                ZInputStream zipStream = new ZInputStream(memoryStream);
                //ZlibStream zipStream = new ZlibStream(memoryStream, CompressionMode.Decompress);


                var expandedBuffer = new byte[0];
                const int BUFFER_SIZE = 10000;
                var buffer = new byte[BUFFER_SIZE];
                var length = 0;
                while ((length = zipStream.read(buffer, 0, BUFFER_SIZE)) > 0)
                //while ((length = zipStream.Read(buffer, 0, BUFFER_SIZE)) > 0)
                {
                    Array.Resize<byte>(ref buffer, length);
                    expandedBuffer = expandedBuffer.Concat(buffer).ToArray();
                    //this must be removed in the 8.0 version
                    //if (length < BUFFER_SIZE)
                    //{
                    //    break;
                    //}
                    buffer = new byte[BUFFER_SIZE];
                }

                // find the actual entry by getting the pointer.
                var posPointer = (this.Length * 8) + 4;
                long pointer = expandedBuffer[posPointer + 3] * 0x100000 + expandedBuffer[posPointer + 2] * 0x10000 + expandedBuffer[posPointer + 1] * 0x100 + expandedBuffer[posPointer];
                var posLength = posPointer + 4;
                long sectionLength = expandedBuffer[posLength + 3] * 0x100000 + expandedBuffer[posLength + 2] * 0x10000 + expandedBuffer[posLength + 1] * 0x100 + expandedBuffer[posLength];

                sectionedBuffer = new byte[sectionLength];
                Array.Copy(expandedBuffer, (int)pointer, sectionedBuffer, 0, (int)sectionLength);
            }
            catch (Exception ee)
            {
                return new byte[0];
                // does not exist
                //return Encoding.UTF8.GetBytes("Does not exist");
            }
            return sectionedBuffer;
        }
    }

}