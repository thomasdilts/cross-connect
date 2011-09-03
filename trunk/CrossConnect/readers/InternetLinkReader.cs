/// <summary>
/// Distribution License:
/// CrossConnect is free software; you can redistribute it and/or modify it under
/// the terms of the GNU General Public License, version 3 as published by
/// the Free Software Foundation. This program is distributed in the hope
/// that it will be useful, but WITHOUT ANY WARRANTY; without even the
/// implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
/// See the GNU General Public License for more details.
///
/// The License is available on the internet at:
///       http://www.gnu.org/copyleft/gpl.html
/// or by writing to:
///      Free Software Foundation, Inc.
///      59 Temple Place - Suite 330
///      Boston, MA 02111-1307, USA
/// </summary>
/// <copyright file="InternetLinkReader.cs" company="Thomas Dilts">
///     Thomas Dilts. All rights reserved.
/// </copyright>
/// <author>Thomas Dilts</author>
namespace CrossConnect.readers
{
    using System.Runtime.Serialization;

    using SwordBackend;

    /// <summary>
    /// Load from a file all the book and verse pointers to the bzz file so that
    /// we can later read the bzz file quickly and efficiently.
    /// </summary>
    /// <param name="path">The path to where the ot.bzs,ot.bzv and ot.bzz and nt files are</param>
    [DataContract(Name = "InternetLinkReader")]
    [KnownType(typeof(ChapterPos))]
    [KnownType(typeof(BookPos))]
    [KnownType(typeof(VersePos))]
    public class InternetLinkReader : BibleZtextReader
    {
        #region Fields

        [DataMember]
        public string link = string.Empty;

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

        #endregion Properties

        #region Methods

        public override string getExternalLink(DisplaySettings displaySettings)
        {
            int number;
            string strongNumber = "";
            if(int.TryParse(link.Substring(1), out number))
            {
                strongNumber = number.ToString();
            }
            if (link.StartsWith("G"))
            {
                return string.Format(displaySettings.greekDictionaryLink, strongNumber);
            }
            else if (link.StartsWith("H"))
            {
                return string.Format(displaySettings.hebrewDictionaryLink, strongNumber);
            }
            return "";
        }

        public override void GetInfo(out int bookNum, out int absouteChaptNum, out int relChaptNum, out int verseNum, out string fullName, out string title)
        {
            verseNum = 0;
            absouteChaptNum = 0;
            bookNum = 0;
            relChaptNum = 0;
            fullName = "";
            title = link;
        }

        public void ShowLink(string link)
        {
            this.link = link;
        }

        #endregion Methods
    }
}