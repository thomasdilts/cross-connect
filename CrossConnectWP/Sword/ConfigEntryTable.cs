#region Header

// <copyright file="ConfigEntryTable.cs" company="Thomas Dilts">
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
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using System.Text.RegularExpressions;

    /// <summary>
    ///     A utility class for loading the entries in a Sword book's conf file. Since
    ///     the conf files are manually maintained, there can be all sorts of errors in
    ///     them. This class does robust checking and reporting.
    ///     Config file format. See also:  "http://sword.sourceforge.net/cgi-bin/twiki/view/Swordapi/ConfFileLayout"
    ///     http://sword.sourceforge.net/cgi-bin/twiki/view/Swordapi/ConfFileLayout
    ///     The contents of the About field are in rtf.
    ///     \ is used as a continuation line.
    /// </summary>
    /// The copyright to this program is held by it's authors.
    /// @author Mark Goodwin [mark at thorubio dot org]
    /// @author Joe Walker [joe at eireneh dot com]
    /// @author Jacky Cheung
    /// @author DM Smith [dmsmith555 at yahoo dot com]
    public class ConfigEntryTable
    {
        #region Constants

        private const string EncodingLatin1 = "WINDOWS-1252";

        #endregion

        // private File configFile;

        #region Static Fields

        /// <summary>
        ///     * If the module's config is tied to a file remember it so that it can be
        ///     updated.
        /// </summary>
        /// <summary>
        ///     * Pattern that matches a key=value. The key can contain ascii letters,
        ///     numbers, underscore and period. The key must begin at the beginning of
        ///     the line. The = sign following the key may be surrounded by whitespace.
        ///     The value may contain anything, including an = sign.
        /// </summary>
        private static readonly Regex KeyValuePattern = new Regex("^([A-Za-z0-9_.]+)\\s*=\\s*(.*)$");

        #endregion

        #region Fields

        /// <summary>
        ///     * A map of lists of unknown config entries.
        /// </summary>
        private readonly Dictionary<string, ConfigEntry> _extra;

        /// <summary>
        ///     * The original name of this config file from mods.d. This is only used for
        ///     managing warnings and errors
        /// </summary>
        private readonly string _internal;

        /// <summary>
        ///     * A map of lists of known config entries.
        /// </summary>
        private readonly Dictionary<ConfigEntryType, ConfigEntry> _table;

        /// <summary>
        ///     * True if this book is considered questionable.
        /// </summary>
        private bool _questionable;

        /// <summary>
        ///     * A helper for the reading of the conf file.
        /// </summary>
        private string _readahead;

        /// <summary>
        ///     * True if this book's config type can be used by JSword.
        /// </summary>
        private bool _supported;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     * Create an empty Sword config for the named book.
        /// </summary>
        /// <param name="bookName">
        ///     the name of the book
        /// </param>
        public ConfigEntryTable(string bookName)
        {
            this._table = new Dictionary<ConfigEntryType, ConfigEntry>();
            this._extra = new Dictionary<string, ConfigEntry>();
            this._internal = bookName;
            this._supported = true;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     * Returns an Enumeration of all the unknown keys found in the config file.
        /// </summary>
        public Dictionary<string, ConfigEntry>.KeyCollection ExtraKeys
        {
            get
            {
                return this._extra.Keys;
            }
        }

        public string InternalName
        {
            get
            {
                return this._internal;
            }
        }

        /// <summary>
        ///     * Determines whether the Sword Book is enciphered.
        /// </summary>
        /// <returns> true if enciphered </returns>
        public bool IsEnciphered
        {
            get
            {
                var cipher = (string)this.GetValue(ConfigEntryType.CipherKey);
                return cipher != null;
            }
        }

        /// <summary>
        ///     * Determines whether the Sword Book is enciphered and without a key.
        /// </summary>
        /// <returns> true if enciphered </returns>
        public bool IsLocked
        {
            get
            {
                var cipher = (string)this.GetValue(ConfigEntryType.CipherKey);
                return cipher != null && cipher.Length == 0;
            }
        }

        /// <summary>
        ///     * Determines whether the Sword Book's conf is supported by JSword.
        /// </summary>
        public bool IsQuestionable
        {
            get
            {
                return this._questionable;
            }
        }

        /// <summary>
        ///     * Determines whether the Sword Book's conf is supported by JSword.
        /// </summary>
        public bool IsSupported
        {
            get
            {
                return this._supported;
            }
        }

        /// <summary>
        ///     * Returns an Enumeration of all the known keys found in the config file.
        /// </summary>
        public Dictionary<ConfigEntryType, ConfigEntry>.KeyCollection Keys
        {
            get
            {
                return this._table.Keys;
            }
        }

        /*
        ///
        /// <summary>* Unlocks a book with the given key. The key is trimmed of any leading or
        /// trailing whitespace.
        ///  </summary>
        /// <param name="unlockKey">
        ///            the key to try </param>
        /// <returns> true if the unlock key worked. </returns>
        ///
        public bool unlock(string unlockKey)
        {
            string tmpKey = unlockKey;
            if (tmpKey != null)
            {
                tmpKey = tmpKey.Trim();
            }
            add(ConfigEntryType.CIPHER_KEY, tmpKey);
            if (configFile != null)
            {
                try
                {
                    save();
                }
                catch (IOException e)
                {
                    // TRANSLATOR: Common error condition: The user supplied unlock key could not be saved.
                    Reporter.informUser(this, JSMsg.gettext("Unable to save the book's unlock key."));
                }
            }
            return true;
        }
         * */

        /// <summary>
        ///     * Gets the unlock key for the module.
        /// </summary>
        /// <returns> the unlock key, if any, null otherwise. </returns>
        public string UnlockKey
        {
            get
            {
                return (string)this.GetValue(ConfigEntryType.CipherKey);
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     * A helper to create/replace a value for a given type.
        /// </summary>
        /// <param name="type"> </param>
        /// <param name="aValue"> </param>
        public void Add(ConfigEntryType type, string aValue)
        {
            if (!this._table.ContainsKey(type))
            {
                var ce = new ConfigEntry(type, aValue);
                this._table.Add(type, ce);
            }
        }

        /// <summary>
        ///     * Gets a particular ConfigEntry's value by its type
        /// </summary>
        /// <param name="type">
        ///     of the ConfigEntry
        /// </param>
        /// <returns>
        ///     the requested value, the default (if there is no entry) or null
        ///     (if there is no default)
        /// </returns>
        public object GetValue(ConfigEntryType type)
        {
            ConfigEntry ce;
            this._table.TryGetValue(type, out ce);
            if (ce != null)
            {
                return ce.Value;
            }
            return type.Default;
        }

        public void Load(Stream streamInput)
        {
            StreamReader @in = null;
            try
            {
                // Quiet Android from complaining about using the default BufferReader buffer size.
                // The actual buffer size is undocumented. So this is a good idea any way.
                @in = new StreamReader(streamInput);
                this.LoadInitials(@in);
                this.LoadContents(@in);
                @in.Dispose();
                @in = null;
                if (this.GetValue(ConfigEntryType.Encoding).Equals(EncodingLatin1))
                {
                    this._supported = true;
                    this._questionable = false;
                    this._readahead = null;
                    this._table.Clear();
                    this._extra.Clear();
                    @in = new StreamReader(streamInput);
                    this.LoadInitials(@in);
                    this.LoadContents(@in);
                    @in.Dispose();
                    @in = null;
                }
                this.AdjustDataPath();
                this.AdjustLanguage();
                this.AdjustName();
                this.validate();
            }
            finally
            {
                if (@in != null)
                {
                    @in.Dispose();
                }
            }
        }

        /// <summary>
        ///     * Load the conf from a buffer. This is used to load conf entries from the
        ///     mods.d.tar.gz file.
        /// </summary>
        /// <param name="buffer">
        ///     the buffer to load
        /// </param>
        /// <exception cref="IOException"> </exception>
        public void Load(byte[] buffer)
        {
            this.Load(new MemoryStream(buffer));
        }

        /// <summary>
        ///     * Determine whether this ConfigEntryTable has the ConfigEntry and it
        ///     matches the value.
        /// </summary>
        /// <param name="type">
        ///     The kind of ConfigEntry to look for
        /// </param>
        /// <param name="search">
        ///     the value to match against
        /// </param>
        /// <returns> true if there is a matching ConfigEntry matching the value </returns>
        public bool Match(ConfigEntryType type, object search)
        {
            ConfigEntry ce = this._table[type];
            return ce != null && ce.Match(search);
        }

        #endregion

        #region Methods

        private void AdjustDataPath()
        {
            string datapath = (string)this.GetValue(ConfigEntryType.ADataPath) ?? string.Empty;
            if (datapath.StartsWith("./"))
            {
                datapath = datapath.Substring(2);
            }
            this.Add(ConfigEntryType.ADataPath, datapath);
        }

        private void AdjustLanguage()
        {
            var lang = (Language)this.GetValue(ConfigEntryType.Lang);
            if (lang == null)
            {
                lang = Language.DefaultLang;
                this.Add(ConfigEntryType.Lang, lang.ToString());
            }
            this.testLanguage(lang);

            var langFrom = (Language)this.GetValue(ConfigEntryType.GlossaryFrom);
            var langTo = (Language)this.GetValue(ConfigEntryType.GlossaryTo);

            // If we have either langFrom or langTo, we are dealing with a glossary
            if (langFrom != null || langTo != null)
            {
                if (langFrom == null)
                {
                    //Logger.Warn("Missing data for " + @internal + ". Assuming " + ConfigEntryType.GLOSSARY_FROM.Name + '=' + Languages.DEFAULT_LANG_CODE);
                    langFrom = Language.DefaultLang;
                    this.Add(ConfigEntryType.GlossaryFrom, lang.Code);
                }
                this.testLanguage(langFrom);

                if (langTo == null)
                {
                    //Logger.Warn("Missing data for " + @internal + ". Assuming " + ConfigEntryType.GLOSSARY_TO.Name + '=' + Languages.DEFAULT_LANG_CODE);
                    langTo = Language.DefaultLang;
                    this.Add(ConfigEntryType.GlossaryTo, lang.Code);
                }
                this.testLanguage(langTo);

                // At least one of the two languages should match the lang entry
                if (!langFrom.Equals(lang) && !langTo.Equals(lang))
                {
                    //Logger.Fail("Data error in " + @internal + ". Neither " + ConfigEntryType.GLOSSARY_FROM.Name + " or " + ConfigEntryType.GLOSSARY_FROM.Name + " match " + ConfigEntryType.LANG.Name);
                }
                else if (!langFrom.Equals(lang))
                {
                    // The LANG field should match the GLOSSARY_FROM field
                    /*
                     * //Logger.Fail("Data error in " + internal + ". " +
                     * ConfigEntryType.GLOSSARY_FROM.getName() + " ("
                     * + langFrom.getCode() + ") does not match " +
                     * ConfigEntryType.LANG.getName() + " (" + lang.getCode() +
                     * ")");
                     */
                    lang = langFrom;
                    this.Add(ConfigEntryType.Lang, lang.Code);
                }
            }
        }

        private void AdjustName()
        {
            // If there is no name then use the internal name
            if (this._table[ConfigEntryType.Description] == null)
            {
                //Logger.Fail("Malformed conf file for " + @internal + " no " + ConfigEntryType.DESCRIPTION.Name + " found. Using internal of " + @internal);
                this.Add(ConfigEntryType.Description, this._internal);
            }
        }

        /// <summary>
        ///     * Get the next line from the input
        /// </summary>
        /// <param name="bin">
        ///     The reader to get data from
        /// </param>
        /// <returns> the next line </returns>
        /// <exception cref="IOException"> </exception>
        private string Advance(StreamReader bin)
        {
            // Was something put back? If so, return it.
            if (this._readahead != null)
            {
                string line = this._readahead;
                this._readahead = null;
                return line;
            }

            // Get the next non-blank, non-comment line
            for (string line = bin.ReadLine(); line != null; line = bin.ReadLine())
            {
                // Remove trailing whitespace
                string trimmed = line.Trim();

                int length = trimmed.Length;

                // skip blank and comment lines
                if (length != 0 && trimmed[0] != '#')
                {
                    return trimmed;
                }
            }
            return null;
        }

        /// <summary>
        ///     * Read too far ahead and need to return a line.
        /// </summary>
        private void Backup(string oops)
        {
            if (oops.Length > 0)
            {
                this._readahead = oops;
            }
        }

        /// <summary>
        ///     * Get continuation lines, if any.
        /// </summary>
        private void GetContinuation(ConfigEntry configEntry, StreamReader bin, StringBuilder buf)
        {
            for (string line = this.Advance(bin); line != null; line = this.Advance(bin))
            {
                int length = buf.Length;

                // Look for bad data as this condition did exist
                bool continuationExpected = length > 0 && buf[length - 1] == '\\';

                if (continuationExpected)
                {
                    // delete the continuation character
                    buf.Remove(length - 1, 1);
                }

                if (this.isKeyLine(line))
                {
                    if (continuationExpected)
                    {
                        //Logger.Warn(report("Continuation followed by key for", configEntry.Name, line));
                    }

                    this.Backup(line);
                    break;
                }
                if (!continuationExpected)
                {
                    //Logger.Warn(report("Line without previous continuation for", configEntry.Name, line));
                }

                if (!configEntry.AllowsContinuation())
                {
                    //Logger.Warn(report("Ignoring unexpected additional line for", configEntry.Name, line));
                }
                else
                {
                    if (continuationExpected)
                    {
                        buf.Append('\n');
                    }
                    buf.Append(line);
                }
            }
        }

        /*
        ///
        /// <summary>* Sort the keys for a more meaningful presentation order. </summary>
        ///
        public Element toOSIS()
        {
            OSISUtil.OSISFactory factory = OSISUtil.factory();
            Element ele = factory.createTable();
            toOSIS(factory, ele, "BasicInfo", BASIC_INFO);
            toOSIS(factory, ele, "LangInfo", LANG_INFO);
            toOSIS(factory, ele, "LicenseInfo", COPYRIGHT_INFO);
            toOSIS(factory, ele, "FeatureInfo", FEATURE_INFO);
            toOSIS(factory, ele, "SysInfo", SYSTEM_INFO);
            toOSIS(factory, ele, "Extra", extra);
            return ele;
        }*/

        /// <summary>
        ///     * Build's a SWORD conf file as a string. The result is not identical to the
        ///     original, cleaning up problems in the original and re-arranging the
        ///     entries into a predictable order.
        /// </summary>
        /// <returns> the well-formed conf. </returns>
        /*
        public string toConf()
        {
            StringBuilder buf = new StringBuilder();
            buf.Append('[');
            buf.Append(getValue(ConfigEntryType.INITIALS));
            buf.Append("]\n");
            toConf(buf, BASIC_INFO);
            toConf(buf, SYSTEM_INFO);
            toConf(buf, HIDDEN);
            toConf(buf, FEATURE_INFO);
            toConf(buf, LANG_INFO);
            toConf(buf, COPYRIGHT_INFO);
            toConf(buf, extra);
            return buf.ToString();
        }*/
        /*
        public void save()
        {
            if (configFile != null)
            {
                // The encoding of the conf must match the encoding of the module.
                string encoding = ENCODING_LATIN1;
                if (getValue(ConfigEntryType.ENCODING).Equals(ENCODING_UTF8))
                {
                    encoding = ENCODING_UTF8;
                }
                Writer writer = null;
                try
                {
                    writer = new OutputStreamWriter(new FileOutputStream(configFile), encoding);
                    writer.write(toConf());
                }
                finally
                {
                    if (writer != null)
                    {
                        writer.Dispose();
                    }
                }
            }
        }

        public void save(File file)
        {
            this.configFile = file;
            this.save();
        }
        */
        private void LoadContents(StreamReader @in)
        {
            var buf = new StringBuilder();
            while (true)
            {
                // Empty out the buffer
                buf.Length = 0;

                string line = this.Advance(@in);
                if (line == null)
                {
                    break;
                }

                // skip blank lines
                if (line.Length == 0)
                {
                    continue;
                }

                Match matcher = KeyValuePattern.Match(line);

                string key = matcher.Groups[1].Value.Trim();
                string value = matcher.Groups[2].Value.Trim();
                // Only CIPHER_KEYS that are empty are not ignored
                if (value.Length == 0 && !ConfigEntryType.CipherKey.Name.Equals(key))
                {
                    //Logger.Warn("Ignoring empty entry in " + @internal + ": " + line);
                    continue;
                }

                // Create a configEntry so that the name is normalized.
                var configEntry = new ConfigEntry(key);

                ConfigEntryType type = configEntry.Type;

                ConfigEntry e = null;
                if (type != null)
                {
                    this._table.TryGetValue(type, out e);
                }
                if (e == null)
                {
                    if (type == null)
                    {
                        //Logger.Warn("Extra entry in " + @internal + " of " + configEntry.Name);
                        this._extra.Add(key, configEntry);
                    }
                    else if (type.IsSynthetic)
                    {
                        //Logger.Warn("Ignoring unexpected entry in " + @internal + " of " + configEntry.Name);
                    }
                    else
                    {
                        this._table.Add(type, configEntry);
                    }
                }
                else
                {
                    configEntry = e;
                }

                buf.Append(value);
                this.GetContinuation(configEntry, @in, buf);

                // History is a special case it is of the form History_x.x
                // The config entry is History without the x.x.
                // We want to put x.x at the beginning of the string
                value = buf.ToString();
                if (ConfigEntryType.History.Equals(type))
                {
                    int pos = key.IndexOf('_');
                    value = key.Substring(pos + 1) + ' ' + value;
                }

                configEntry.AddValue(value);
            }
        }

        private void LoadInitials(StreamReader @in)
        {
            string initials = null;
            while (true)
            {
                string line = this.Advance(@in);
                if (line == null)
                {
                    break;
                }

                if (line[0] == '[' && line[line.Length - 1] == ']')
                {
                    // The conf file contains a leading line of the form [KJV]
                    // This is the acronym by which Sword refers to it.
                    initials = line.Substring(1, line.Length - 1 - 1);
                    break;
                }
            }
            if (initials == null)
            {
                //Logger.Fail("Malformed conf file for " + @internal + " no initials found. Using internal of " + @internal);
                initials = this._internal;
            }
            this.Add(ConfigEntryType.Initials, initials);
        }

        /// <summary>
        ///     * Does this line of text represent a key/value pair?
        /// </summary>
        private bool isKeyLine(string line)
        {
            return KeyValuePattern.IsMatch(line);
        }

        private void testLanguage(Language lang)
        {
            if (!lang.IsValidLanguage)
            {
                //Logger.Warn("Unknown language " + lang.Code + " in book " + initials);
            }
        }

        /// <summary>
        ///     * Determine which books are not supported. Also, report on problems.
        /// </summary>
        private void validate()
        {
            // if (isEnciphered())
            // {
            //            log.debug("Book not supported: " + internal + " because it is locked and there is no key.");
            // supported = false;
            // return;
            // }
        }

        #endregion
    }
}