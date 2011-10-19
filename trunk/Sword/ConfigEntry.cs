#region Header

// <copyright file="ConfigEntry.cs" company="Thomas Dilts">
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
    using System.Text;
    using System.Text.RegularExpressions;

    ///<summary>
    ///  A ConfigEntry holds the value(s) for an entry of ConfigEntryType.
    ///</summary>
    ///<seealso cref =  gnu.lgpl.License for license details.<br>
    ///                                                        The copyright to this program is held by it's authors. </seealso>
    ///<seealso cref =  gnu.lgpl.License
    /// @author DM Smith [ dmsmith555 at yahoo dot com] </seealso>
    public class ConfigEntry
    {
        #region Fields

        /// <summary>
        ///   * A pattern of allowable RTF in a SWORD conf. These are: \pard, \pae, \par,
        ///   \qc \b, \i and embedded Unicode
        /// </summary>
        private static readonly Regex RTF_PATTERN = new Regex("\\\\pard|\\\\pa[er]|\\\\qc|\\\\[bi]|\\\\u-?[0-9]{4,6}");

        private readonly string name;

        /// <summary>
        ///   * A histogram for debugging.
        /// </summary>
        // private static Histogram histogram = new Histogram();
        private readonly ConfigEntryType type;

        private string @internal;
        private object value;
        private IList<string> values;

        #endregion Fields

        #region Constructors

        /// <summary>
        ///   * Create a ConfigEntry whose type is not certain and whose value is not
        ///   known.
        /// </summary>
        /// <param name = "bookName">
        ///   the internal name of the book </param>
        /// <param name = "aName">
        ///   the name of the ConfigEntry. </param>
        public ConfigEntry(string bookName, string aName)
        {
            @internal = bookName;
            name = aName;
            type = ConfigEntryType.fromString(aName);
        }

        /// <summary>
        ///   * Create a ConfigEntry directly with an initial value.
        /// </summary>
        /// <param name = "bookName">
        ///   the internal name of the book </param>
        /// <param name = "aType">
        ///   the kind of ConfigEntry </param>
        /// <param name = "aValue">
        ///   the initial value for the ConfigEntry </param>
        public ConfigEntry(string bookName, ConfigEntryType aType, string aValue)
        {
            @internal = bookName;
            name = aType.Name;
            type = aType;
            addValue(aValue);
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        ///   * Determine whether this config entry is supported.
        /// </summary>
        /// <returns> true if this ConfigEntry has a type. </returns>
        public bool isSupported
        {
            get { return type != null; }
        }

        /// <summary>
        ///   * Get the key of this ConfigEntry
        /// </summary>
        public string Name
        {
            get
            {
                if (type != null)
                {
                    return type.Name;
                }
                return name;
            }
        }

        /// <summary>
        ///   * Get the type of this ConfigEntry
        /// </summary>
        public ConfigEntryType Type
        {
            get { return type; }
        }

        /// <summary>
        ///   * Get the value(s) of this ConfigEntry. If mayRepeat() == true then it
        ///   returns a List. Otherwise it returns a string.
        /// </summary>
        /// <returns> a list, value or null. </returns>
        public object Value
        {
            get
            {
                if (value != null)
                {
                    return value;
                }
                if (values != null)
                {
                    return values;
                }
                return type.Default;
            }
        }

        #endregion Properties

        #region Methods

        /// <summary>
        ///   * Add a value to the list of values for this ConfigEntry
        /// </summary>
        public void addValue(string val)
        {
            string aValue = val;
            string confEntryName = Name;
            // Filter known types of entries
            if (type != null)
            {
                aValue = type.filter(aValue);
            }

            // Report on fields that shouldn't have RTF but do
            if (!allowsRTF() && RTF_PATTERN.IsMatch(aValue))
            {
                //Logger.Debug(report("Ignoring unexpected RTF for", Name, aValue));
            }

            if (mayRepeat())
            {
                if (values == null)
                {
                    // histogram.increment(confEntryName);
                    values = new List<string>();
                }
                if (reportDetails())
                {
                    // histogram.increment(confEntryName + '.' + aValue);
                }
                if (!isAllowed(aValue))
                {
                    //Logger.Debug(report("Ignoring unknown config value for", confEntryName, aValue));
                    return;
                }
                values.Add(aValue);
            }
            else
            {
                if (value != null)
                {
                    //Logger.Debug(report("Ignoring unexpected additional entry for", confEntryName, aValue));
                }
                else
                {
                    // histogram.increment(confEntryName);
                    if (type.hasChoices())
                    {
                        // histogram.increment(confEntryName + '.' + aValue);
                    }
                    if (!isAllowed(aValue))
                    {
                        //Logger.Debug(report("Ignoring unknown config value for", confEntryName, aValue));
                        return;
                    }
                    value = type.convert(aValue);
                }
            }
        }

        /// <summary>
        ///   * While most fields are single line or single value, some allow
        ///   continuation. A continuation mark is a backslash at the end of a line. It
        ///   is not to be followed by whitespace.
        /// </summary>
        /// <returns> true if continuation is allowed </returns>
        public bool allowsContinuation()
        {
            if (type != null)
            {
                return type.allowsContinuation();
            }
            return true;
        }

        /// <summary>
        ///   * RTF is allowed in a few config entries.
        /// </summary>
        /// <returns> true if RTF is allowed </returns>
        public bool allowsRTF()
        {
            if (type != null)
            {
                return type.allowsRTF();
            }
            return true;
        }

        /*
        public Element toOSIS()
        {
            OSISUtil.OSISFactory factory = OSISUtil.factory();

            Element rowEle = factory.createRow();

            Element nameEle = factory.createCell();
            Element hiEle = factory.createHI();
            hiEle.SetAttribute(OSISUtil.OSIS_ATTR_TYPE, OSISUtil.HI_BOLD);
            nameEle.addContent(hiEle);
            Element valueElement = factory.createCell();
            rowEle.addContent(nameEle);
            rowEle.addContent(valueElement);

            // I18N(DMS): use name to lookup translation.
            hiEle.addContent(Name);

            if (value != null)
            {
                string text = value.ToString();
                text = XMLUtil.escape(text);
                if (allowsRTF())
                {
                    valueElement.addContent(OSISUtil.rtfToOsis(text));
                }
                else if (allowsContinuation())
                {
                    valueElement.addContent(processLines(factory, text));
                }
                else
                {
                    valueElement.addContent(text);
                }
            }

            if (values != null)
            {
                Element listEle = factory.createLG();
                valueElement.addContent(listEle);

                foreach (string str in values)
                {
                    string text = XMLUtil.escape(str);
                    Element itemEle = factory.createL();
                    listEle.addContent(itemEle);
                    if (allowsRTF())
                    {
                        itemEle.addContent(OSISUtil.rtfToOsis(text));
                    }
                    else
                    {
                        itemEle.addContent(text);
                    }
                }
            }
            return rowEle;
        }

        public static void resetStatistics()
        {
            histogram.clear();
        }

        public static void dumpStatistics()
        {
            // Uncomment the following line to produce statistics
            // System.out.println(histogram.toString());
        }
        */
        /*
         * (non-Javadoc)
         *
         * @see java.lang.Object#equals(java.lang.Object)
         */
        public override bool Equals(object obj)
        {
            // Since this can not be null
            if (obj == null)
            {
                return false;
            }

            // Check that that is the same as this
            // Don't use instanceOf since that breaks inheritance
            if (!obj.GetType().Equals(GetType()))
            {
                return false;
            }

            var that = (ConfigEntry) obj;
            return that.Name.Equals(Name);
        }

        /*
         * (non-Javadoc)
         *
         * @see java.lang.Object#hashCode()
         */
        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }

        /// <summary>
        ///   * Determines whether the string is allowed. For some config entries, the
        ///   value is expected to be one of a group, for others the format is defined.
        /// </summary>
        /// <param name = "aValue"> </param>
        /// <returns> true if the string is allowed </returns>
        public bool isAllowed(string aValue)
        {
            if (type != null)
            {
                return type.isAllowed(aValue);
            }
            return true;
        }

        /// <summary>
        ///   * Determine whether this Config entry matches the value.
        /// </summary>
        /// <param name = "search">
        ///   the value to match against </param>
        /// <returns> true if this ConfigEntry matches the value </returns>
        public bool match(object search)
        {
            if (value != null)
            {
                return value.Equals(search);
            }
            if (values != null)
            {
                return values.Contains(search.ToString());
            }
            var def = type.Default;
            return def != null && def.Equals(search);
        }

        /// <summary>
        ///   * Some keys can repeat. When this happens each is a single value pick from
        ///   a list of choices.
        /// </summary>
        /// <returns> true if this ConfigEntryType can occur more than once </returns>
        public bool mayRepeat()
        {
            if (type != null)
            {
                return type.mayRepeat();
            }
            return true;
        }

        /// <summary>
        ///   *
        /// </summary>
        public bool reportDetails()
        {
            if (type != null)
            {
                return type.reportDetails();
            }
            return true;
        }

        /// <summary>
        ///   * Build's a SWORD conf file as a string. The result is not identical to the
        ///   original, cleaning up problems in the original and re-arranging the
        ///   entries into a predictable order.
        /// </summary>
        /// <returns> the well-formed conf. </returns>
        public string toConf()
        {
            var buf = new StringBuilder();

            if (value != null)
            {
                buf.Append(Name);
                buf.Append('=');
                string text = getConfValue(value);
                if (allowsContinuation())
                {
                    // With continuation each line is ended with a '\', except the
                    // last.
                    text = text.Replace("\n", "\\\\\n");
                }
                buf.Append(text);
                buf.Append('\n');
            }
            else if (type.Equals(ConfigEntryType.CIPHER_KEY))
            {
                // CipherKey is empty to indicate that it is encrypted and locked.
                buf.Append(Name);
                buf.Append('=');
            }

            if (values != null)
            {
                // History values begin with the history value, e.g. 1.2
                // followed by a space.
                // These are to joined to the key.
                if (type.Equals(ConfigEntryType.HISTORY))
                {
                    foreach (string text in values)
                    {
                        buf.Append(Name);
                        buf.Append('_');
                        if (text.IndexOf(' ') >= 0)
                        {
                            buf.Append(text.Substring(0, text.IndexOf(' ') - 1) + "=" +
                                       text.Substring(text.IndexOf(' ') + 1));
                        }
                        buf.Append('\n');
                    }
                }
                else
                {
                    foreach (string text in values)
                    {
                        buf.Append(Name);
                        buf.Append('=');
                        buf.Append(getConfValue(text));
                        buf.Append('\n');
                    }
                }
            }
            return buf.ToString();
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

        /// <summary>
        ///   * The conf value is the internal representation of the string. For
        ///   Language, this is the code, not the localized name. Add others as needed.
        /// </summary>
        /// <param name = "aValue">
        ///   either value or values[i] </param>
        /// <returns> the conf value. </returns>
        private string getConfValue(object aValue)
        {
            if (aValue != null)
            {
                if (aValue is Language)
                {
                    return ((Language) value).Code;
                }
                return aValue.ToString();
            }
            return null;
        }

        #endregion Methods
    }
}