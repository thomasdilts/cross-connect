#region Header

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
    ///     Load from a file all the book and verse pointers to the bzz file so that
    ///     we can later read the bzz file quickly and efficiently.
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

        #endregion

        #region Constructors and Destructors

        public InternetLinkReader(string path, string iso2DigitLangCode, bool isIsoEncoding)
            : base(path, iso2DigitLangCode, isIsoEncoding, string.Empty, string.Empty, string.Empty)
        {
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

        public override bool IsTTChearable
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

        public override string GetExternalLink(DisplaySettings displaySettings)
        {
            int number;
            string strongNumber = string.Empty;
            if (int.TryParse(this.Link.Substring(1), out number))
            {
                strongNumber = number.ToString();
            }

            if (this.Link.StartsWith("G"))
            {
                return string.Format(displaySettings.GreekDictionaryLink, strongNumber);
            }

            if (this.Link.StartsWith("H"))
            {
                return string.Format(displaySettings.HebrewDictionaryLink, strongNumber);
            }

            return this.Link;
        }

        public override void GetInfo(
            out string bookShortName,
            out int relChaptNum,
            out int verseNum,
            out string fullName,
            out string title)
        {
            verseNum = 0;
            bookShortName = string.Empty;
            relChaptNum = 0;
            fullName = string.Empty;
            title = this.TitleBar;
        }

        public void ShowLink(string link, string titleBar)
        {
            this.Link = link;
            this.TitleBar = titleBar;
        }

        #endregion
    }
}