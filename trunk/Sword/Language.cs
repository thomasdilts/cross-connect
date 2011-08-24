///
/// <summary> Distribution License:
/// CrossConnect is free software; you can redistribute it and/or modify it under
/// the terms of the GNU Lesser General Public License, version 2.1 as published by
/// the Free Software Foundation. This program is distributed in the hope
/// that it will be useful, but WITHOUT ANY WARRANTY; without even the
/// implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
/// See the GNU Lesser General Public License for more details.
///
/// The License is available on the internet at:
///       http://www.gnu.org/copyleft/lgpl.html
/// or by writing to:
///      Free Software Foundation, Inc.
///      59 Temple Place - Suite 330
///      Boston, MA 02111-1307, USA
///
/// Copyright: 2007
///     The copyright to this program is held by it's authors.
///
/// ID: $Id: Languages.java 1462 2007-07-02 02:32:23Z dmsmith $
/// 
/// Converted from Java to C# by Thomas Dilts with the help of a program from www.tangiblesoftwaresolutions.com
/// called 'Java to VB & C# Converter' on 2011-04-12 </summary>
/// 
namespace SwordBackend
{
    //: IComparable<Language>
    ///
    /// <summary> A single language, paring an ISO-639 code to a localized representation of
    /// the language.
    ///  </summary>
    /// <seealso cref= gnu.lgpl.License for license details.<br>
    ///      The copyright to this program is held by it's authors.
    /// @author DM Smith [dmsmith555 at yahoo dot com] </seealso>
    /// 
    public class Language
    {
        #region Fields

        public static readonly Language DEFAULT_LANG = new Language(null);

        private string code;
        private bool knowsDirection;
        private bool ltor;
        private string name;

        #endregion Fields

        #region Constructors

        ///    
        /// <summary>* A single language defined by an ISO-639 code. If the code is null or
        /// empty then it is considered to be DEFAULT_LANG_CODE (that is, English).
        ///  </summary>
        /// <param name="iso639Code">
        ///            the particular language </param>
        ///     
        public Language(string iso639Code)
        {
            this.code = Languages.getLanguageCode(iso639Code);
        }

        #endregion Constructors

        #region Properties

        ///    
        /// <summary>* Get the language code.
        ///  </summary>
        /// <returns> the code for the language </returns>
        ///     
        public virtual string Code
        {
            get
            {
                return code;
            }
        }

        ///    
        /// <summary>* Determine whether this language is a Left-to-Right or a Right-to-Left
        /// language. Note: This is problematic. Languages do not have direction.
        /// Scripts do. Further, there are over 7000 living languages, many of which
        /// are written in Right-to-Left scripts and are not listed here.
        ///  </summary>
        /// <returns> true if the language is Left-to-Right. </returns>
        ///     
        public virtual bool isLeftToRight
        {
            get
            {
                if (!knowsDirection)
                {
                    // TODO(DMS): Improve this.
                    ltor = !("he".Equals(code) || "ar".Equals(code) || "fa".Equals(code) || "ur".Equals(code) || "uig".Equals(code) || "syr".Equals(code) || "iw".Equals(code)); // Java's notion of Hebrew -  Syriac -  Uighur, too -  Uighur -  Farsi/Persian -  Arabic -  Hebrew

                    knowsDirection = true;
                }

                return ltor;
            }
        }

        ///    
        /// <summary>* Determine whether this language is valid. The code is valid if it is in
        /// iso639.properties.
        ///  </summary>
        /// <returns> true if the language is valid. </returns>
        ///     
        public virtual bool isValidLanguage
        {
            get
            {
                return Languages.isValidLanguage(code);
            }
        }

        ///    
        /// <summary>* Get the language name.
        ///  </summary>
        /// <returns> the name of the language </returns>
        ///     
        public virtual string Name
        {
            get
            {
                if (name == null)
                {
                    name = Languages.getLanguageName(code);
                }
                return name;
            }
        }

        #endregion Properties

        #region Methods

        /*
         * (non-Javadoc)
         *
         * @see java.lang.Comparable#compareTo(java.lang.Object)
         */
        public virtual int compareTo(Language o)
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

            if (obj == null || this.GetType() != obj.GetType())
            {
                return false;
            }

            // JAVA TO VB & C# CONVERTER WARNING: The original Java variable was marked 'final':
            // ORIGINAL LINE: final Language other = (Language) obj;
            Language other = (Language) obj;

            return code.Equals(other.code);
        }

        /*
         * (non-Javadoc)
         *
         * @see java.lang.Object#hashCode()
         */
        public override int GetHashCode()
        {
            return code.GetHashCode();
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