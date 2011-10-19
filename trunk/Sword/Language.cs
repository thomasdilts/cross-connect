#region Header

// <copyright file="Language.cs" company="Thomas Dilts">
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
// Email: thomas@chaniel.se
// </summary>
// <author>Thomas Dilts</author>

#endregion Header

namespace Sword
{
    //: IComparable<Language>
    ///<summary>
    ///  A single language, paring an ISO-639 code to a localized representation of
    ///  the language.
    ///</summary>
    ///                                                        The copyright to this program is held by it's authors.
    ///                                                        @author DM Smith [dmsmith555 at yahoo dot com] 
    public class Language
    {
        #region Fields

        public static readonly Language DefaultLang = new Language(null);

        private readonly string _code;

        private bool _knowsDirection;
        private bool _ltor;
        private string _name;

        #endregion Fields

        #region Constructors

        /// <summary>
        ///   * A single language defined by an ISO-639 code. If the code is null or
        ///   empty then it is considered to be DEFAULT_LANG_CODE (that is, English).
        /// </summary>
        /// <param name = "iso639Code">
        ///   the particular language </param>
        public Language(string iso639Code)
        {
            _code = Languages.GetLanguageCode(iso639Code);
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        ///   * Get the language code.
        /// </summary>
        /// <returns> the code for the language </returns>
        public virtual string Code
        {
            get { return _code; }
        }

        /// <summary>
        ///   * Determine whether this language is a Left-to-Right or a Right-to-Left
        ///   language.  This is problematic. Languages do not have direction.
        ///   Scripts do. Further, there are over 7000 living languages, many of which
        ///   are written in Right-to-Left scripts and are not listed here.
        /// </summary>
        /// <returns> true if the language is Left-to-Right. </returns>
        public virtual bool IsLeftToRight
        {
            get
            {
                if (!_knowsDirection)
                {
                    // Improve this.
                    _ltor =
                        !("he".Equals(_code) || "ar".Equals(_code) || "fa".Equals(_code) || "ur".Equals(_code) ||
                          "uig".Equals(_code) || "syr".Equals(_code) || "iw".Equals(_code));
                        // Java's notion of Hebrew -  Syriac -  Uighur, too -  Uighur -  Farsi/Persian -  Arabic -  Hebrew

                    _knowsDirection = true;
                }

                return _ltor;
            }
        }

        /// <summary>
        ///   * Determine whether this language is valid. The code is valid if it is in
        ///   iso639.properties.
        /// </summary>
        /// <returns> true if the language is valid. </returns>
        public virtual bool IsValidLanguage
        {
            get { return Languages.IsValidLanguage(_code); }
        }

        /// <summary>
        ///   * Get the language name.
        /// </summary>
        /// <returns> the name of the language </returns>
        public virtual string Name
        {
            get { return _name ?? (_name = Languages.GetLanguageName(_code)); }
        }

        #endregion Properties

        #region Methods

        /*
         * (non-Javadoc)
         *
         * @see java.lang.Comparable#compareTo(java.lang.Object)
         */
        public virtual int CompareTo(Language o)
        {
            return Name.CompareTo(o.ToString());
        }

        /*
         * (non-Javadoc)
         *
         * @see java.lang.Object#equals(java.lang.Object)
         */
        public override bool Equals(object obj)
        {
            if (this == obj)
            {
                return true;
            }

            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            // JAVA TO VB & C# CONVERTER WARNING: The original Java variable was marked 'final':
            // ORIGINAL LINE: final Language other = (Language) obj;
            var other = (Language) obj;

            return _code.Equals(other._code);
        }

        /*
         * (non-Javadoc)
         *
         * @see java.lang.Object#hashCode()
         */
        public override int GetHashCode()
        {
            return _code.GetHashCode();
        }

        /*
         * (non-Javadoc)
         *
         * @see java.lang.Object#toString()
         */
        public override string ToString()
        {
            return Name;
        }

        #endregion Methods
    }
}