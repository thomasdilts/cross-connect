#region Header

// <copyright file="CommentRawComReader.cs" company="Thomas Dilts">
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

namespace Sword.reader
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Runtime.Serialization;
    using System.Text;
    using System.Threading.Tasks;
    using System.Xml;
    using System.Linq;
    using Windows.Storage;
    using Sword.versification;

    /// <summary>
    ///     Load from a file all the book and verse pointers to the bzz file so that
    ///     we can later read the bzz file quickly and efficiently.
    /// </summary>
    [DataContract(Name = "CommentRawComReader")]
    [KnownType(typeof(ChapterPos))]
    [KnownType(typeof(BookPos))]
    [KnownType(typeof(VersePos))]
    public class CommentRawComReader : CommentZtextReader
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Load from a file all the book and verse pointers to the bzz file so that
        ///     we can later read the bzz file quickly and efficiently.
        /// </summary>
        /// <param name="path">The path to where the ot.bzs,ot.bzv and ot.bzz and nt files are</param>
        /// <param name="iso2DigitLangCode"></param>
        /// <param name="isIsoEncoding"></param>
        public CommentRawComReader(string path, string iso2DigitLangCode, bool isIsoEncoding, string cipherKey, string configPath, string versification)
            : base(path, iso2DigitLangCode, isIsoEncoding, cipherKey, configPath, versification)
        {
        }

        #endregion
        protected override async Task<byte[]> GetChapterBytes(int chapterNumber)
        {
            return await BibleRawTextReader.GetChapterBytesRaw(chapterNumber, this);
        }
        protected override async Task<bool> ReloadOneIndex(string filename, int startBook, int endBook, CanonBookDef[] booksInTestement)
        {
            return await BibleRawTextReader.ReloadOneIndexRaw(filename, startBook, endBook, booksInTestement, this);
        }
    }
}