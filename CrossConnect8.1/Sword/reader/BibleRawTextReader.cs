#region Header

// <copyright file="BibleRawComReader.cs" company="Thomas Dilts">
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
    [DataContract(Name = "BibleRawComReader")]
    [KnownType(typeof(ChapterPos))]
    [KnownType(typeof(BookPos))]
    [KnownType(typeof(VersePos))]
    public class BibleRawTextReader : BibleZtextReader
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Load from a file all the book and verse pointers to the bzz file so that
        ///     we can later read the bzz file quickly and efficiently.
        /// </summary>
        /// <param name="path">The path to where the ot.bzs,ot.bzv and ot.bzz and nt files are</param>
        /// <param name="iso2DigitLangCode"></param>
        /// <param name="isIsoEncoding"></param>
        public BibleRawTextReader(string path, string iso2DigitLangCode, bool isIsoEncoding, string cipherKey, string configPath, string versification)
            : base(path, iso2DigitLangCode, isIsoEncoding, cipherKey, configPath, versification)
        {
        }

        #endregion

        public static async Task<bool> ReloadOneIndexRaw(string filename, int startBook, int endBook, CanonBookDef[] booksInTestement, BibleZtextReader thisZtextReader)
        {
            if (string.IsNullOrEmpty(thisZtextReader.Serial.Path))
            {
                return false;
            }
            try
            {
                var bookPositions = new List<BookPos>();
                var fs =
                    await
                    ApplicationData.Current.LocalFolder.OpenStreamForReadAsync(
                        thisZtextReader.Serial.Path.Replace("/", "\\") + filename + "vss");
                bool isEnd;
                // dump the first 2 posts
                for (int i = 0; i < 2; i++)
                {
                    thisZtextReader.GetintFromStream(fs, out isEnd);
                    thisZtextReader.GetShortIntFromStream(fs, out isEnd);
                }

                // now start getting each chapter in each book
                for (int i = startBook; i < endBook; i++)
                {
                    // dump the book record
                    thisZtextReader.GetintFromStream(fs, out isEnd);
                    thisZtextReader.GetShortIntFromStream(fs, out isEnd);

                    var bookDef = booksInTestement[i - startBook];
                    long bookStartPos = 0;
                    var booknumCalculated = i - startBook + 1;
                    if (booknumCalculated < bookPositions.Count)
                    {
                        bookStartPos = bookPositions[booknumCalculated].StartPos;
                    }

                    for (int j = 0; j < bookDef.NumberOfChapters; j++)
                    {
                        // dump the chapter record
                        thisZtextReader.GetintFromStream(fs, out isEnd);
                        thisZtextReader.GetShortIntFromStream(fs, out isEnd);

                        ChapterPos chapt = new ChapterPos(-1, i, j, bookStartPos);
                        long lastNonZeroStartPos = 0;
                        long lastNonZeroLength = 0;
                        int length = 0;

                        for (int k = 0; k < thisZtextReader.canon.VersesInChapter[bookDef.VersesInChapterStartIndex + j]; k++)
                        {
                            long startPos = thisZtextReader.GetintFromStream(fs, out isEnd);
                            if (startPos != 0)
                            {
                                lastNonZeroStartPos = startPos;
                            }

                            length = thisZtextReader.GetShortIntFromStream(fs, out isEnd);

                            if (length != 0)
                            {
                                lastNonZeroLength = length;
                                chapt.IsEmpty = false;

                                if (chapt.StartPos == -1)
                                {
                                    chapt.StartPos = startPos;
                                    //chapt.BookStartPos = bookPositions[booknumCalculated].StartPos;
                                }
                                chapt.Verses.Add(new VersePos(startPos - chapt.StartPos, length, i));
                            }
                            else
                            {
                                chapt.Verses.Add(new VersePos(0, 0, i));
                            }
                        }

                        // update the chapter length now that we know it
                        if (chapt != null)
                        {
                            chapt.Length = (int)(lastNonZeroStartPos - chapt.StartPos) + lastNonZeroLength;
                            thisZtextReader.Chapters.Add(chapt);
                        }
                    }
                }

                fs.Dispose();
            }
            catch (Exception)
            {
                // failed to load old testement.  Maybe it does not exist.
                // fill with fake data
                for (int i = startBook; i < endBook; i++)
                {
                    var bookDef = booksInTestement[i - startBook];
                    for (int j = 0; j < bookDef.NumberOfChapters; j++)
                    {
                        var chapt = new ChapterPos(0, i, j, 0);
                        chapt.IsEmpty = true;
                        for (int k = 0; k < thisZtextReader.canon.VersesInChapter[bookDef.VersesInChapterStartIndex + j]; k++)
                        {
                            chapt.Verses.Add(new VersePos(0, 0, i));
                        }

                        // update the chapter length now that we know it
                        chapt.Length = 0;
                        thisZtextReader.Chapters.Add(chapt);
                    }
                }
            }
            return true;
        }

        protected override async Task<bool> ReloadOneIndex(string filename, int startBook, int endBook, CanonBookDef[] booksInTestement)
        {
            return await ReloadOneIndexRaw(filename, startBook,  endBook, booksInTestement, this);
        }

        protected override async Task<byte[]> GetChapterBytes(int chapterNumber)
        {
            return await GetChapterBytesRaw(chapterNumber, this);
        }

        public static async Task<byte[]> GetChapterBytesRaw(int chapterNumber, BibleZtextReader thisZtextReader)
        {
            //Debug.WriteLine("getChapterBytes start");
            int numberOfChapters = thisZtextReader.Chapters.Count;
            if (numberOfChapters == 0)
            {
                return Encoding.UTF8.GetBytes("Does not exist");
            }
            if (chapterNumber >= numberOfChapters)
            {
                chapterNumber = numberOfChapters - 1;
            }

            if (chapterNumber < 0)
            {
                chapterNumber = 0;
            }

            ChapterPos versesForChapterPositions = thisZtextReader.Chapters[chapterNumber];

            long bookStartPos = versesForChapterPositions.BookStartPos;
            long blockStartPos = versesForChapterPositions.StartPos;
            long blockLen = versesForChapterPositions.Length;
            Stream fs;
            var book = thisZtextReader.canon.OldTestBooks[thisZtextReader.canon.OldTestBooks.Count() - 1];
            string fileName = (chapterNumber < (book.VersesInChapterStartIndex + book.NumberOfChapters)) ? "ot" : "nt";
            try
            {
                //Windows.Storage.ApplicationData appData = Windows.Storage.ApplicationData.Current;
                //var folder = await appData.LocalFolder.GetFolderAsync(Serial.Path.Replace("/", "\\"));
                string filenameComplete = thisZtextReader.Serial.Path + fileName ;
                fs =
                    await
                    ApplicationData.Current.LocalFolder.OpenStreamForReadAsync(filenameComplete.Replace("/", "\\"));
                //fs = await file.OpenStreamForReadAsync();
            }
            catch (Exception ee)
            {
                // does not exist
                return Encoding.UTF8.GetBytes("Does not exist");
            }

            // adjust the start postion of the stream to where this book begins.
            // we must read the entire book up to the chapter we want even though we just want one chapter.
            fs.Position = versesForChapterPositions.StartPos;

            var chapterBuffer = new byte[versesForChapterPositions.Length];
            await fs.ReadAsync(chapterBuffer, 0, (int)versesForChapterPositions.Length);
            fs.Dispose();

            return chapterBuffer;
        }
    }
}