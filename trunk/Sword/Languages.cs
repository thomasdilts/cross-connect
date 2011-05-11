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
/// Copyright: 2005
///     The copyright to this program is held by it's authors.
///
/// ID: $Id: Languages.java 2012 2010-11-11 23:24:41Z dmsmith $
/// 
/// Converted from Java to C# by Thomas Dilts with the help of a program from www.tangiblesoftwaresolutions.com
/// called 'Java to VB & C# Converter' on 2011-04-12 </summary>
/// 
namespace SwordBackend
{
    using System;
    using System.Diagnostics;

    using javaprops;

    ///
    /// <summary> A utility class that converts ISO-639 codes or locales to their "friendly"
    /// language name.
    ///  </summary>
    /// <seealso cref= gnu.lgpl.License for license details.<br>
    ///      The copyright to this program is held by it's authors.
    /// @author DM Smith [dmsmith555 at yahoo dot com] </seealso>
    /// 
    public class Languages
    {
        #region Fields

        public const string DEFAULT_LANG_CODE = "en";

        private const string UNKNOWN_LANG_CODE = "und";

        private static JavaProperties languages; // final

        #endregion Fields

        #region Constructors

        static Languages()
        {
            try
            {
                languages = new JavaProperties("iso639",true);
            }
            catch (Exception )
            {
                Debug.Assert(false);
            }
        }

        ///    
        /// <summary>* Make the class a true utility class by having a private constructor. </summary>
        ///     
        private Languages()
        {
        }

        #endregion Constructors

        #region Methods

        ///    
        /// <summary>* Get the language code from the input. If the code is null or empty then
        /// it is considered to be DEFAULT_LANG_CODE (that is, English). If a locale
        /// is used for the iso639Code, it will use the part before the '_'. Thus,
        /// this code does not support dialects, except as found in the iso639. If it
        /// is known to be unknown then return unknown. Otherwise, return the 2 or 3
        /// letter code. Note: it might not be valid.
        ///  </summary>
        /// <param name="input"> </param>
        /// <returns> the code for the language </returns>
        ///     
        public static string getLanguageCode(string input)
        {
            string lookup = input;
            if (lookup == null || lookup.Length == 0)
            {
                return DEFAULT_LANG_CODE;
            }

            if (lookup.IndexOf('_') != -1)
            {
                string[] locale = lookup.Split('_');
                // We need to check what stands before the _, it might be empty or
                // unknown.
                return getLanguageCode(locale[0]);
            }

            // These are not uncommon. Looking for them prevents exceptions
            // and provides the same result.
            if (lookup.StartsWith("x-") || lookup.StartsWith("X-") || lookup.Length > 3)
            {
                return UNKNOWN_LANG_CODE;
            }

            return lookup;
        }

        ///    
        /// <summary>* Get the language name from the language code. If the code is null or
        /// empty then it is considered to be DEFAULT_LANG_CODE (that is, English).
        /// If it starts with x- or is too long then it will return unknown. If the
        /// code's name cannot be found, it will return the code. If a locale is used
        /// for the iso639Code, it will use the part before the '_'. Thus, this code
        /// does not support dialects, except as found in the iso639.
        ///  </summary>
        /// <param name="iso639Code"> </param>
        /// <returns> the name of the language </returns>
        ///     
        public static string getLanguageName(string iso639Code)
        {
            string code = getLanguageCode(iso639Code);
            try
            {
                return languages[code];
            }
            catch (Exception )
            {
                return code;
            }
        }

        ///    
        /// <summary>* Determine whether the language code is valid. The code is valid if it is
        /// null or empty. The code is valid if it is in iso639.properties. If a
        /// locale is used for the iso639Code, it will use the part before the '_'.
        /// Thus, this code does not support dialects, except as found in the iso639.
        ///  </summary>
        /// <param name="iso639Code"> </param>
        /// <returns> true if the language is valid. </returns>
        ///     
        public static bool isValidLanguage(string iso639Code)
        {
            try
            {
                string code = getLanguageCode(iso639Code);
                if (DEFAULT_LANG_CODE.Equals(code) || UNKNOWN_LANG_CODE.Equals(code))
                {
                    return true;
                }

                return languages.ContainsKey(code);
            }
            catch (Exception )
            {
                return false;
            }
        }

        #endregion Methods
    }
}