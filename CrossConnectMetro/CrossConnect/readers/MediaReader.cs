#region Header

// <copyright file="MediaReader.cs" company="Thomas Dilts">
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

namespace CrossConnect.readers
{
    using System.Collections.Generic;
    using System.Runtime.Serialization;
    using System.Linq;
    using Sword;
    using Sword.reader;
    using Sword.versification;

    /// <summary>
    ///     Load from a file all the book and verse pointers to the bzz file so that
    ///     we can later read the bzz file quickly and efficiently.
    /// </summary>
    [DataContract(Name = "MediaReader")]
    [KnownType(typeof(ChapterPos))]
    [KnownType(typeof(BookPos))]
    [KnownType(typeof(VersePos))]
    public class MediaReader : BibleZtextReader
    {
        #region Fields

        [DataMember]
        public AudioPlayer.MediaInfo Info;

        #endregion

        #region Constructors and Destructors

        public MediaReader(string path, string iso2DigitLangCode, bool isIsoEncoding, string cipherKey, string configPath, string versification,AudioPlayer.MediaInfo info)
            : base(path, iso2DigitLangCode, isIsoEncoding, cipherKey, configPath, versification)
        {
            this.Info = info;
        }

        #endregion

        #region Public Properties

        public override bool IsExternalLink
        {
            get
            {
                return true;
            }
        }

        public override bool IsHearable
        {
            get
            {
                return false;
            }
        }

        public override bool IsPageable
        {
            get
            {
                return false;
            }
        }

        public override bool IsSearchable
        {
            get
            {
                return false;
            }
        }

        public override bool IsSynchronizeable
        {
            get
            {
                return false;
            }
        }

        public override bool IsTranslateable
        {
            get
            {
                return false;
            }
        }

        #endregion

        #region Public Methods and Operators

        public override void MoveNext()
        {
            if (this.Serial == null || canon == null)
            {
                return;
            }

            if(!App.DisplaySettings.SyncMediaVerses)
            {
                base.MoveNext();
                return;
            }

            int nextVerse = this.Serial.PosVerseNum + 1;
            var book = canon.BookByShortName[this.Serial.PosBookShortName];
            if (nextVerse >= canon.VersesInChapter[book.VersesInChapterStartIndex + this.Serial.PosChaptNum])
            {
                base.MoveNext();
            }
            else
            {
                this.MoveChapterVerse(this.Serial.PosBookShortName, this.Serial.PosChaptNum, nextVerse, false, this);
            }
            App.SynchronizeAllWindows(this.Serial.PosBookShortName, this.Serial.PosChaptNum, nextVerse, -1, this);
        }

        public override void MovePrevious()
        {
            if (this.Serial == null || canon == null)
            {
                return;
            }

            if (!App.DisplaySettings.SyncMediaVerses)
            {
                base.MovePrevious();
                return;
            }

            int nextVerse = this.Serial.PosVerseNum - 1;
            if (nextVerse < 0)
            {
                base.MovePrevious();
            }
            else
            {
                this.MoveChapterVerse(this.Serial.PosBookShortName, this.Serial.PosChaptNum, nextVerse, false, this);
            }
            App.SynchronizeAllWindows(this.Serial.PosBookShortName, this.Serial.PosChaptNum, nextVerse, -1, this);
        }


        public override ButtonWindowSpecs GetButtonWindowSpecs(int stage, int lastSelectedButton)
        {
            switch (stage)
            {
                case 0:
                    {
                        // books
                        var colors = new List<int>();
                        var values = new List<int>();
                        var buttonNames = new List<string>();
                        if (this.BookNames == null)
                        {
                            this.BookNames = new BibleNames(this.Serial.Iso2DigitLangCode);
                        }

                        //string[] buttonNamesStart = this.BookNames.GetAllShortNames();
                        //if (!this.BookNames.ExistsShortNames)
                        //{
                        //    buttonNamesStart = this.BookNames.GetAllFullNames();
                        //}

                        int bookCounter = 0;
                        //always add the old testement.
                        if (!this.Info.IsNtOnly)
                        {
                            foreach (var book in canon.OldTestBooks)
                            {
                                colors.Add(ChapterCategories[book.ShortName1]);
                                values.Add(bookCounter);
                                buttonNames.Add(this.BookNames.GetShortName(book.ShortName1));
                                bookCounter++;
                            }
                        }
                        else
                        {
                            bookCounter = canon.OldTestBooks.Count();
                        }

                        foreach (var book in canon.NewTestBooks)
                        {
                            colors.Add(ChapterCategories[book.ShortName1]);
                            values.Add(bookCounter);
                            buttonNames.Add(this.BookNames.GetShortName(book.ShortName1));
                            bookCounter++;
                        }

                        return new ButtonWindowSpecs(
                            stage,
                            "Select a book to view",
                            colors.Count,
                            colors.ToArray(),
                            buttonNames.ToArray(),
                            values.ToArray(),
                            !this.BookNames.ExistsShortNames ? ButtonSize.Large : ButtonSize.Medium);
                    }
                case 1:
                    {
                        //Chapters 
                        CanonBookDef book = null;
                        //Chapters 
                        if (lastSelectedButton >= canon.OldTestBooks.Count())
                        {
                            book = canon.NewTestBooks[lastSelectedButton - canon.OldTestBooks.Count()];
                        }
                        else
                        {
                            book = canon.OldTestBooks[lastSelectedButton];
                        }

                        // set up the array for the chapter selection
                        int numOfChapters = book.NumberOfChapters;

                        if (numOfChapters <= 1)
                        {
                            return null;
                        }

                        var butColors = new int[numOfChapters];
                        var values = new int[numOfChapters];
                        var butText = new string[numOfChapters];
                        for (int i = 0; i < numOfChapters; i++)
                        {
                            butColors[i] = 0;
                            butText[i] = (i + 1).ToString();
                            values[i] = book.VersesInChapterStartIndex + i;
                        }

                        // do a nice transition
                        return new ButtonWindowSpecs(
                            stage,
                            "Select a chapter to view",
                            numOfChapters,
                            butColors,
                            butText,
                            values,
                            ButtonSize.Small);
                    }
            }
            return null;
        }

        #endregion
    }
}