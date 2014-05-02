#region Header

// <copyright file="Languages.cs" company="Thomas Dilts">
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

namespace Sword
{
    using System;
    using System.Diagnostics;

    using Sword.javaprops;

    /// <summary>
    ///     A utility class that converts ISO-639 codes or locales to their "friendly"
    ///     language name.
    /// </summary>
    /// The copyright to this program is held by it's authors.
    /// @author DM Smith [dmsmith555 at yahoo dot com]
    public class Languages
    {
        #region Constants

        public const string DefaultLangCode = "en";

        private const string UnknownLangCode = "und";

        #endregion

        #region Static Fields

        private static readonly JavaProperties JavaLanguages; // final

        #endregion

        #region Constructors and Destructors

        static Languages()
        {
            try
            {
                JavaLanguages = new JavaProperties("iso639", true);
            }
            catch (Exception)
            {
                Debug.Assert(false);
            }
        }

        /// <summary>
        ///     * Make the class a true utility class by having a private constructor.
        /// </summary>
        private Languages()
        {
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     * Get the language code from the input. If the code is null or empty then
        ///     it is considered to be DEFAULT_LANG_CODE (that is, English). If a locale
        ///     is used for the iso639Code, it will use the part before the '_'. Thus,
        ///     this code does not support dialects, except as found in the iso639. If it
        ///     is known to be unknown then return unknown. Otherwise, return the 2 or 3
        ///     letter code.
        /// </summary>
        /// <param name="input"> </param>
        /// <returns> the code for the language </returns>
        public static string GetLanguageCode(string input)
        {
            string lookup = input!=null?input.ToLower().Replace("-", "_"):null;
            if (string.IsNullOrEmpty(lookup))
            {
                return DefaultLangCode;
            }
            switch (lookup)
            {
                case "zh_hant":
                case "zh_tw":
                case "zh_hk":
                case "zh_mo":
                    return "zh-Hant"; //traditional chinese
                case "zh_hans":
                case "zh_cn":
                case "zh_sg":
                case "zh_my":
                    return "zh-Hans"; // simplified chinese
            }
            if (lookup.IndexOf('_') != -1)
            {
                string[] locale = lookup.Split('_');
                // We need to check what stands before the _, it might be empty or
                // unknown.
                return GetLanguageCode(locale[0]);
            }

            // These are not uncommon. Looking for them prevents exceptions
            // and provides the same result.
            if (lookup.StartsWith("x-") || lookup.StartsWith("X-") || lookup.Length > 3)
            {
                return UnknownLangCode;
            }

            return lookup;
        }

        /// <summary>
        ///     * Get the language name from the language code. If the code is null or
        ///     empty then it is considered to be DEFAULT_LANG_CODE (that is, English).
        ///     If it starts with x- or is too long then it will return unknown. If the
        ///     code's name cannot be found, it will return the code. If a locale is used
        ///     for the iso639Code, it will use the part before the '_'. Thus, this code
        ///     does not support dialects, except as found in the iso639.
        /// </summary>
        /// <param name="iso639Code"> </param>
        /// <returns> the name of the language </returns>
        public static string GetLanguageName(string iso639Code)
        {
            string code = GetLanguageCode(iso639Code);
            try
            {
                return JavaLanguages[code];
            }
            catch (Exception)
            {
                return code;
            }
        }

        /// <summary>
        ///     * Determine whether the language code is valid. The code is valid if it is
        ///     null or empty. The code is valid if it is in iso639.properties. If a
        ///     locale is used for the iso639Code, it will use the part before the '_'.
        ///     Thus, this code does not support dialects, except as found in the iso639.
        /// </summary>
        /// <param name="iso639Code"> </param>
        /// <returns> true if the language is valid. </returns>
        public static bool IsValidLanguage(string iso639Code)
        {
            try
            {
                string code = GetLanguageCode(iso639Code);
                if (DefaultLangCode.Equals(code) || UnknownLangCode.Equals(code))
                {
                    return true;
                }

                return JavaLanguages.ContainsKey(code);
            }
            catch (Exception)
            {
                return false;
            }
        }

        #endregion
    }
}