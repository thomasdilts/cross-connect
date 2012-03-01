// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MediaReader.cs" company="">
//   
// </copyright>
// <summary>
//   Load from a file all the book and verse pointers to the bzz file so that
//   we can later read the bzz file quickly and efficiently.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

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
    using System.Runtime.Serialization;

    using Sword.reader;

    /// <summary>
    /// Load from a file all the book and verse pointers to the bzz file so that
    ///   we can later read the bzz file quickly and efficiently.
    /// </summary>
    [DataContract(Name = "MediaReader")]
    [KnownType(typeof(ChapterPos))]
    [KnownType(typeof(BookPos))]
    [KnownType(typeof(VersePos))]
    public class MediaReader : BibleZtextReader
    {
        #region Constants and Fields

        /// <summary>
        /// The icon.
        /// </summary>
        [DataMember]
        public string Icon = string.Empty;

        /// <summary>
        /// The link.
        /// </summary>
        [DataMember]
        public string Link = string.Empty;

        /// <summary>
        /// The title bar.
        /// </summary>
        [DataMember]
        public string TitleBar = string.Empty;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MediaReader"/> class.
        /// </summary>
        /// <param name="link">
        /// The link.
        /// </param>
        /// <param name="titleBar">
        /// The title bar.
        /// </param>
        /// <param name="icon">
        /// The icon.
        /// </param>
        public MediaReader(string link, string titleBar, string icon)
            : base(string.Empty, "en", false)
        {
            this.Link = link;
            this.TitleBar = titleBar;
            this.Icon = icon;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets a value indicating whether IsExternalLink.
        /// </summary>
        public override bool IsExternalLink
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Gets a value indicating whether IsHearable.
        /// </summary>
        public override bool IsHearable
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets a value indicating whether IsPageable.
        /// </summary>
        public override bool IsPageable
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets a value indicating whether IsSearchable.
        /// </summary>
        public override bool IsSearchable
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets a value indicating whether IsSynchronizeable.
        /// </summary>
        public override bool IsSynchronizeable
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets a value indicating whether IsTranslateable.
        /// </summary>
        public override bool IsTranslateable
        {
            get
            {
                return false;
            }
        }

        #endregion
    }
}