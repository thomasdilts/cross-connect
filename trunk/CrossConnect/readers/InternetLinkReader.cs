﻿#region Header

// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InternetLinkReader.cs" company="">
//
// </copyright>
// <summary>
//   Load from a file all the book and verse pointers to the bzz file so that
//   we can later read the bzz file quickly and efficiently.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InternetLinkReader.cs" company="Thomas Dilts">
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
    using System.Runtime.Serialization;

    using Sword.reader;

    /// <summary>
    /// Load from a file all the book and verse pointers to the bzz file so that
    ///   we can later read the bzz file quickly and efficiently.
    /// </summary>
    [DataContract(Name = "InternetLinkReader")]
    [KnownType(typeof(ChapterPos))]
    [KnownType(typeof(BookPos))]
    [KnownType(typeof(VersePos))]
    public class InternetLinkReader : BibleZtextReader
    {
        #region Fields

        [DataMember]
        public string Link = string.Empty;

        [DataMember]
        public string TitleBar = string.Empty;

        #endregion Fields

        #region Constructors

        public InternetLinkReader(string path, string iso2DigitLangCode, bool isIsoEncoding)
            : base(path, iso2DigitLangCode, isIsoEncoding)
        {
        }

        #endregion Constructors

        #region Properties

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

        #endregion Properties

        #region Methods

        public override string GetExternalLink(DisplaySettings displaySettings)
        {
            int number;
            string strongNumber = string.Empty;
            if (int.TryParse(Link.Substring(1), out number))
            {
                strongNumber = number.ToString();
            }

            if (Link.StartsWith("G"))
            {
                return string.Format(displaySettings.GreekDictionaryLink, strongNumber);
            }

            if (Link.StartsWith("H"))
            {
                return string.Format(displaySettings.HebrewDictionaryLink, strongNumber);
            }

            return Link;
        }

        public override void GetInfo(
            out int bookNum,
            out int absouteChaptNum,
            out int relChaptNum,
            out int verseNum,
            out string fullName,
            out string title)
        {
            verseNum = 0;
            absouteChaptNum = 0;
            bookNum = 0;
            relChaptNum = 0;
            fullName = string.Empty;
            title = TitleBar;
        }

        public void ShowLink(string link, string titleBar)
        {
            Link = link;
            TitleBar = titleBar;
        }

        #endregion Methods
    }
}