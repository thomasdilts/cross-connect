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

    using Sword;
    using Sword.reader;

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

        public MediaReader(AudioPlayer.MediaInfo info)
            : base(string.Empty, info.Language, false, string.Empty, string.Empty)
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

                        string[] buttonNamesStart = this.BookNames.GetAllShortNames();
                        if (!this.BookNames.ExistsShortNames)
                        {
                            buttonNamesStart = this.BookNames.GetAllFullNames();
                        }

                        //always add the old testement.
                        if (!this.Info.IsNtOnly)
                        {
                            for (int i = 0; i < BooksInOt; i++)
                            {
                                colors.Add(ChapterCategories[i]);
                                values.Add(FirstChapternumInBook[i]);
                                buttonNames.Add(buttonNamesStart[i]);
                            }
                        }

                        for (int i = BooksInOt; i < BooksInBible; i++)
                        {
                            colors.Add(ChapterCategories[i]);
                            values.Add(FirstChapternumInBook[i]);
                            buttonNames.Add(buttonNamesStart[i]);
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
                        int booknum = 0;
                        for (int i = 0; i < BooksInBible; i++)
                        {
                            if (lastSelectedButton == FirstChapternumInBook[i])
                            {
                                booknum = i;
                                break;
                            }

                            if (lastSelectedButton < FirstChapternumInBook[i])
                            {
                                booknum = i - 1;
                                break;
                            }
                        }

                        // set up the array for the chapter selection
                        int numOfChapters = ChaptersInBook[booknum];

                        if (numOfChapters <= 1)
                        {
                            return null;
                        }

                        // Color butColor = (Color)Application.Current.Resources["PhoneForegroundColor"];
                        var butColors = new int[numOfChapters];
                        var values = new int[numOfChapters];
                        var butText = new string[numOfChapters];
                        for (int i = 0; i < numOfChapters; i++)
                        {
                            butColors[i] = 0;
                            butText[i] = (i + 1).ToString();
                            values[i] = FirstChapternumInBook[booknum] + i;
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