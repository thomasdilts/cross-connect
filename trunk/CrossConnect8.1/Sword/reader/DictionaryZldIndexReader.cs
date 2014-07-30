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

    //using ComponentAce.Compression.Libs.zlib;

    using Windows.Storage;

    [DataContract]
    public class DictionaryZldIndexReader : DictionaryRawIndexReader
    {
        public DictionaryZldIndexReader(string path, string iso2DigitLangCode, bool isIsoEncoding, string WindowMatchingKey)
            : base(path, iso2DigitLangCode, isIsoEncoding, WindowMatchingKey)
        {
            this.Serial.Iso2DigitLangCode = iso2DigitLangCode;
            this.Serial.Path = path;
            this.Serial.IsIsoEncoding = isIsoEncoding;
        }

        protected override async Task<bool> ReloadSettingsFile()
        {
            this.DictionaryEntries = new List<DictionaryEntry>();
            if (string.IsNullOrEmpty(this.Serial.Path))
            {
                return false;
            }
            try
            {
                Stream fs =
                    await
                    ApplicationData.Current.LocalFolder.OpenStreamForReadAsync(
                        this.Serial.Path.Replace("/", "\\") + ".idx");
                bool isEnd;
                long index;

                while ((index = GetintFromStream(fs, out isEnd)) > -100 && !isEnd)
                {
                    var entry = new DictionaryEntry();
                    entry.PositionInDat = index;
                    entry.Length = GetintFromStream(fs, out isEnd);
                    this.DictionaryEntries.Add(entry);
                }
                fs.Dispose();

                fs =
                    await
                    ApplicationData.Current.LocalFolder.OpenStreamForReadAsync(
                        this.Serial.Path.Replace("/", "\\") + ".dat");
                var stopChars = new char[] { '\r', '\n', (char)0, '\\' };
                for (int i = 0; i < this.DictionaryEntries.Count; i++)
                {

                    fs.Seek(this.DictionaryEntries[i].PositionInDat, SeekOrigin.Begin);
                    //get the title and the section postions
                    long section;
                    long posInSection;
                    this.DictionaryEntries[i].Title = ReadStringToCharAndSection(fs, this.DictionaryEntries[i].Length, out section, out posInSection);
                    this.DictionaryEntries[i].PositionInDat = section;
                    this.DictionaryEntries[i].Length = posInSection;
                }
                fs.Dispose();
            }
            catch (Exception ee)
            {

            }
            return true;
        }
        protected string ReadStringToCharAndSection(Stream fs, long length, out long section, out long posInSection)
        {
            section = 0;
            posInSection = 0;
            var totalBuf = new List<byte>();

            var textLength = length - 10;
            var buf = new byte[textLength];
            if (fs.Read(buf, 0, (int)textLength) == (int)textLength)
            {
                // dump 2 bytes
                var buf2 = new byte[2];
                if (fs.Read(buf2, 0, 2) == 2)
                {
                    bool isEnd;
                    section = GetintFromStream(fs, out isEnd);
                    if (!isEnd)
                    {
                        posInSection = GetintFromStream(fs, out isEnd);
                    }
                }
                return Encoding.UTF8.GetString(buf, 0, (int)textLength);
            }

            return string.Empty;
        }
    }

}