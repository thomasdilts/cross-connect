#region Header

// <copyright file="JavaProperties.cs" company="Thomas Dilts">
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

namespace Sword.javaprops
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Reflection;

    /// <summary>
    ///   Hold Java style properties as key-value pairs and allow them to be loaded from or
    ///   saved to a ".properties" file. The file is stored with character set ISO-8859-1 which extends US-ASCII
    ///   (the characters 0-127 are the same) and forms the first part of the Unicode character set.  Within the
    ///   application <see cref = "string" /> are Unicode - but all values outside the basic US-ASCII set are escaped.
    /// </summary>
    public class JavaProperties : Dictionary<string, string>
    {
        #region Fields

        /// <summary>
        ///   A reference to an optional set of default properties - these values are returned
        ///   if the value has not been loaded from a ".properties" file or set programatically.
        /// </summary>
        protected Dictionary<string, string> Defaults;

        #endregion Fields

        #region Constructors

        /// <summary>
        ///   Load a java property file that is an embedded resource in the Properties directory.
        /// </summary>
        /// <param name = "resourceName">if isLangDependentResource is false, this is the complete name of the resource. 
        ///   Otherwise, this is just the name without the suffixes</param>
        /// <param name = "isLangDependentResource">True if there could be a language dependent file as well to be loaded</param>
        public JavaProperties(string resourceName, bool isLangDependentResource)
        {
            var assem = Assembly.GetExecutingAssembly();
            // this is java style folks.  We load the standard resource
            // file and then load the language specific file and then merge them
            // with the language taking priority.
            string name = "Sword.Properties." + resourceName + ".properties";
            if (!isLangDependentResource)
            {
                name = "Sword.Properties." + resourceName;
            }
            var reader = new JavaPropertyReader(this);

            using (var stream = assem.GetManifestResourceStream(name))
            {
                if (stream != null)
                {
                    reader.Parse(stream);
                }
            }
            if (isLangDependentResource)
            {
                string isocode = CultureInfo.CurrentCulture.TwoLetterISOLanguageName.ToLower();
                using (
                    var stream =
                        assem.GetManifestResourceStream("Sword.Properties." + resourceName + "_" + isocode +
                                                        ".properties"))
                {
                    if (stream != null)
                    {
                        reader.Parse(stream);
                    }
                }
            }
        }

        /// <summary>
        ///   Use this constructor to provide a set of default values.  The default values are kept separate
        ///   to the ones in this instant.
        /// </summary>
        /// <param name = "defaults">A Dictionary that holds a set of defafult key value pairs to
        ///                                        return when the requested key has not been set.</param>
        public JavaProperties(Dictionary<string, string> defaults)
        {
            Defaults = defaults;
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        ///   Get the value for the specified key value.  If the key is not found, then return the
        ///   default value - and if still not found, return null.
        /// </summary>
        /// <param name = "key">The key whose value should be returned.</param>
        /// <returns>The value corresponding to the key - or null if not found.</returns>
        public string GetProperty(string key)
        {
            Object objectValue = this[key];
            if (objectValue != null)
            {
                return AsString(objectValue);
            }
            if (Defaults != null)
            {
                return AsString(Defaults[key]);
            }

            return null;
        }

        /// <summary>
        ///   Get the value for the specified key value.  If the key is not found, then return the
        ///   default value - and if still not found, return <c>defaultValue</c>.
        /// </summary>
        /// <param name = "key">The key whose value should be returned.</param>
        /// <param name = "defaultValue">The default value if the key is not found.</param>
        /// <returns>The value corresponding to the key - or null if not found.</returns>
        public string GetProperty(string key, string defaultValue)
        {
            string val = GetProperty(key);
            return val ?? defaultValue;
        }

        /// <summary>
        ///   Returns an enumerator of all the properties available in this instance - including the
        ///   defaults.
        /// </summary>
        /// <returns>An enumarator for all of the keys including defaults.</returns>
        public IEnumerator PropertyNames()
        {
            Dictionary<string, string> combined;
            if (Defaults != null)
            {
                combined = new Dictionary<string, string>(Defaults);

                for (IEnumerator e = Keys.GetEnumerator(); e.MoveNext();)
                {
                    string key = AsString(e.Current);
                    combined.Add(key, this[key]);
                }
            }
            else
            {
                combined = new Dictionary<string, string>(this);
            }

            return combined.Keys.GetEnumerator();
        }

        /// <summary>
        ///   Set the value for a property key.  The old value is returned - if any.
        /// </summary>
        /// <param name = "key">The key whose value is to be set.</param>
        /// <param name = "newValue">The new value off the key.</param>
        /// <returns>The original value of the key - as a string.</returns>
        public string SetProperty(string key, string newValue)
        {
            string oldValue = AsString(this[key]);
            this[key] = newValue;
            return oldValue;
        }

        /// <summary>
        ///   A utility method to safely convert an <c>Object</c> to a <c>string</c>.
        /// </summary>
        /// <param name = "o">An Object or null to be returned as a string.</param>
        /// <returns>string value of the object - or null.</returns>
        private string AsString(Object o)
        {
            if (o == null)
            {
                return null;
            }

            return o.ToString();
        }

        #endregion Methods
    }
}