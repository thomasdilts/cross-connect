#region Header

// <copyright file="SwordBookMetaData.cs" company="Thomas Dilts">
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
    using System.Collections.Generic;
    using System.IO;
    using System.Text;

    using reader;

    ///  A utility class for loading and representing Sword book configs.
    ///                                                        The copyright to this program is held by it's authors.
    ///                                                        @author Mark Goodwin [mark at thorubio dot org]
    ///                                                        @author Joe Walker [joe at eireneh dot com]
    ///                                                        @author Jacky Cheung
    ///                                                        @author DM Smith [dmsmith555 at yahoo dot com]
    public class SwordBookMetaData
    {
        #region Fields

        public bool IsLoaded;

        private readonly ConfigEntryTable _cet;
        private readonly Dictionary<string, object> _prop = new Dictionary<string, object>();

        #endregion Fields

        #region Constructors

        /// <summary>
        ///   * Loads a sword config from a buffer.
        /// </summary>
        /// <param name = "buffer"> </param>
        /// <param name="bookName"></param>
        /// <exception cref = "IOException"> </exception>
        public SwordBookMetaData(byte[] buffer, string bookName)
        {
            _cet = new ConfigEntryTable(bookName);
            _cet.Load(buffer);
            BuildProperties();
        }

        public SwordBookMetaData(Stream stream, string bookName)
        {
            _cet = new ConfigEntryTable(bookName);
            _cet.Load(stream);
            BuildProperties();
        }

        #endregion Constructors

        #region Properties

        /*public Filter Filter
        {
            get
            {
                string sourcetype = (string) getProperty(ConfigEntryType.SOURCE_TYPE);
                return FilterFactory.getFilter(sourcetype);
            }
        }*/
        /// <summary>
        ///   * Returns the sourceType.
        /// </summary>
        /// <returns> Returns the relative path of the book's conf. </returns>
        public string ConfPath
        {
            get { return BibleZtextReader.DirConf + '/' + _cet.InternalName.ToLower() + BibleZtextReader.ExtensionConf; }
        }

        public string Initials
        {
            get { return (string) GetProperty(ConfigEntryType.Initials); }
        }

        public string InternalName
        {
            get { return _cet.InternalName; }
        }

        /*
         * (non-Javadoc)
         *
         * @see org.crosswire.jsword.book.BookMetaData#isEnciphered()
         */
        public bool IsEnciphered
        {
            get { return _cet.IsEnciphered; }
        }

        /*
         * (non-Javadoc)
         *
         * @see org.crosswire.jsword.book.BookMetaData#isLeftToRight()
         */
        public bool IsLeftToRight
        {
            get
            {
                // This should return the dominate direction of the text, if it is BiDi,
                // then we have to guess.
                var dir = (string) GetProperty(ConfigEntryType.Direction);
                if (ConfigEntryType.DirectionBidi.Equals(dir))
                {
                    // When BiDi, return the dominate direction based upon the Book's
                    // Language not Direction
                    var lang = (Language) GetProperty(ConfigEntryType.Lang);
                    return lang.IsLeftToRight;
                }

                return ConfigEntryType.DirectionLtor.Equals(dir);
            }
        }

        /*
         * (non-Javadoc)
         *
         * @see org.crosswire.jsword.book.BookMetaData#isLocked()
         */
        public bool IsLocked
        {
            get { return _cet.IsLocked; }
        }

        /*
         * (non-Javadoc)
         *
         * @see org.crosswire.jsword.book.BookMetaData#isQuestionable()
         */
        public bool IsQuestionable
        {
            get { return _cet.IsQuestionable; }
        }

        /*
         * (non-Javadoc)
         *
         * @see org.crosswire.jsword.book.BookMetaData#getName()
         */
        public string Name
        {
            get { return (string) GetProperty(ConfigEntryType.Description); }
        }

        /*
         * (non-Javadoc)
         *
         * @see org.crosswire.jsword.book.BookMetaData#unlock(String)
         */
        /*
        public  bool unlock(string unlockKey)
        {
            return cet.unlock(unlockKey);
        }*/
        /*
         * (non-Javadoc)
         *
         * @see org.crosswire.jsword.book.BookMetaData#getUnlockKey()
         */
        public string UnlockKey
        {
            get { return _cet.UnlockKey; }
        }

        #endregion Properties

        #region Methods

        public object GetCetProperty(ConfigEntryType confType)
        {
            return _cet.GetValue(confType);
        }

        /// <summary>
        ///   * Get the string value for the property or null if it is not defined. It is
        ///   assumed that all properties gotten with this method are single line.
        /// </summary>
        /// <param name = "entry">
        ///   the ConfigEntryType </param>
        /// <returns> the property or null </returns>
        public object GetProperty(ConfigEntryType entry)
        {
            return _cet.GetValue(entry);
        }

        private void BuildProperties()
        {
            // merge entries into properties file
            foreach (var key in _cet.Keys)
            {
                var value = _cet.GetValue(key);
                // value is null if the config entry was rejected.
                if (value == null)
                {
                    continue;
                }
                // JAVA TO VB & C# CONVERTER
                // ORIGINAL LINE: if (value instanceof java.util.List<?>)
                if (value is IList<string>)
                {
                    var list = (IList<string>) value;
                    var combined = new StringBuilder();
                    bool appendSeparator = false;
                    foreach (string element in list)
                    {
                        if (appendSeparator)
                        {
                            combined.Append('\n');
                        }
                        combined.Append(element);
                        appendSeparator = true;
                    }

                    value = combined.ToString();
                }

                PutProperty(key.ToString(), value);
            }
        }

        /// <param name = "key">
        ///   the key of the property to set </param>
        /// <param name = "value">
        ///   the value of the property </param>
        private void PutProperty(string key, object value)
        {
            _prop[key] = value;
        }

        #endregion Methods
    }
}