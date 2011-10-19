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
// Email: thomas@chaniel.se
// </summary>
// <author>Thomas Dilts</author>

#endregion Header

namespace Sword
{
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using System.Text.RegularExpressions;

    ///<summary>
    ///  A utility class for loading the entries in a Sword book's conf file. Since
    ///  the conf files are manually maintained, there can be all sorts of errors in
    ///  them. This class does robust checking and reporting.
    /// 
    ///  <p>
    ///    Config file format. See also: <a href = "http://sword.sourceforge.net/cgi-bin/twiki/view/Swordapi/ConfFileLayout">
    ///                                    http://sword.sourceforge.net/cgi-bin/twiki/view/Swordapi/ConfFileLayout</a>
    /// 
    ///    <p>
    ///      The contents of the About field are in rtf.
    ///      <p>
    ///        \ is used as a continuation line.
    ///</summary>
    ///<seealso cref =  gnu.lgpl.License for license details.<br>
    ///                                                        The copyright to this program is held by it's authors.
    ///                                                        @author Mark Goodwin [mark at thorubio dot org]
    ///                                                        @author Joe Walker [joe at eireneh dot com]
    ///                                                        @author Jacky Cheung
    ///                                                        @author DM Smith [dmsmith555 at yahoo dot com] </seealso>
    public class ConfigEntryTable
    {
        #region Fields

        private const string ENCODING_LATIN1 = "WINDOWS-1252";

        /// <summary>
        ///   * Sword only recognizes two encodings for its modules: UTF-8 and LATIN1
        ///   Sword uses MS Windows cp1252 for Latin 1 not the standard. Arrgh!
        /// </summary>
        private const string ENCODING_UTF8 = "UTF-8";

        private static readonly ConfigEntryType[] COPYRIGHT_INFO = {
                                                                       ConfigEntryType.About, ConfigEntryType.SHORT_PROMO,
                                                                       ConfigEntryType.DISTRIBUTION_LICENSE,
                                                                       ConfigEntryType.DISTRIBUTION_NOTES,
                                                                       ConfigEntryType.DISTRIBUTION_SOURCE,
                                                                       ConfigEntryType.SHORT_COPYRIGHT,
                                                                       ConfigEntryType.COPYRIGHT,
                                                                       ConfigEntryType.COPYRIGHT_DATE,
                                                                       ConfigEntryType.COPYRIGHT_HOLDER,
                                                                       ConfigEntryType.COPYRIGHT_CONTACT_NAME,
                                                                       ConfigEntryType.COPYRIGHT_CONTACT_ADDRESS,
                                                                       ConfigEntryType.COPYRIGHT_CONTACT_EMAIL,
                                                                       ConfigEntryType.COPYRIGHT_CONTACT_NOTES,
                                                                       ConfigEntryType.COPYRIGHT_NOTES,
                                                                       ConfigEntryType.TEXT_SOURCE
                                                                   };
        private static readonly ConfigEntryType[] FEATURE_INFO = {
                                                                     ConfigEntryType.FEATURE,
                                                                     ConfigEntryType.GLOBAL_OPTION_FILTER,
                                                                     ConfigEntryType.FONT
                                                                 };
        private static readonly ConfigEntryType[] HIDDEN = {ConfigEntryType.CIPHER_KEY};

        // private File configFile;
        /// <summary>
        ///   * If the module's config is tied to a file remember it so that it can be
        ///   updated.
        /// </summary>
        /// <summary>
        ///   * Pattern that matches a key=value. The key can contain ascii letters,
        ///   numbers, underscore and period. The key must begin at the beginning of
        ///   the line. The = sign following the key may be surrounded by whitespace.
        ///   The value may contain anything, including an = sign.
        /// </summary>
        private static readonly Regex KEY_VALUE_PATTERN = new Regex("^([A-Za-z0-9_.]+)\\s*=\\s*(.*)$");

        /// <summary>
        ///   * These are the elements that JSword requires. They are a superset of those
        ///   that Sword requires.
        /// </summary>
        /*
         * For documentation purposes at this time. private static final
         * ConfigEntryType[] REQUIRED = { ConfigEntryType.INITIALS,
         * ConfigEntryType.DESCRIPTION, ConfigEntryType.CATEGORY, // may not be
         * present in conf ConfigEntryType.DATA_PATH, ConfigEntryType.MOD_DRV, };
         */
        private static readonly ConfigEntryType[] LANG_INFO = {
                                                                  ConfigEntryType.LANG, ConfigEntryType.GLOSSARY_FROM,
                                                                  ConfigEntryType.GLOSSARY_TO
                                                              };
        private static readonly ConfigEntryType[] SYSTEM_INFO = {
                                                                    ConfigEntryType.ADataPath, ConfigEntryType.MOD_DRV,
                                                                    ConfigEntryType.SOURCE_TYPE, ConfigEntryType.BLOCK_TYPE
                                                                    , ConfigEntryType.BLOCK_COUNT,
                                                                    ConfigEntryType.COMPRESS_TYPE, ConfigEntryType.ENCODING
                                                                    , ConfigEntryType.MINIMUM_VERSION,
                                                                    ConfigEntryType.OSIS_VERSION,
                                                                    ConfigEntryType.OSIS_Q_TO_TICK,
                                                                    ConfigEntryType.DIRECTION, ConfigEntryType.KEY_TYPE,
                                                                    ConfigEntryType.DISPLAY_LEVEL
                                                                };

        /// <summary>
        ///   * The original name of this config file from mods.d. This is only used for
        ///   managing warnings and errors
        /// </summary>
        private readonly string @internal;

        /// <summary>
        ///   * A map of lists of unknown config entries.
        /// </summary>
        private readonly Dictionary<string, ConfigEntry> extra;

        /// <summary>
        ///   * A map of lists of known config entries.
        /// </summary>
        private readonly Dictionary<ConfigEntryType, ConfigEntry> table;

        /// <summary>
        ///   * True if this book is considered questionable.
        /// </summary>
        private bool questionable;

        /// <summary>
        ///   * A helper for the reading of the conf file.
        /// </summary>
        private string readahead;

        /// <summary>
        ///   * True if this book's config type can be used by JSword.
        /// </summary>
        private bool supported;

        #endregion Fields

        #region Constructors

        /// <summary>
        ///   * Create an empty Sword config for the named book.
        /// </summary>
        /// <param name = "bookName">
        ///   the name of the book </param>
        public ConfigEntryTable(string bookName)
        {
            table = new Dictionary<ConfigEntryType, ConfigEntry>();
            extra = new Dictionary<string, ConfigEntry>();
            @internal = bookName;
            supported = true;
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        ///   * Returns an Enumeration of all the unknown keys found in the config file.
        /// </summary>
        public Dictionary<string, ConfigEntry>.KeyCollection ExtraKeys
        {
            get { return extra.Keys; }
        }

        public string internalName
        {
            get { return @internal; }
        }

        /// <summary>
        ///   * Determines whether the Sword Book is enciphered.
        /// </summary>
        /// <returns> true if enciphered </returns>
        public bool isEnciphered
        {
            get
            {
                string cipher = (string) getValue(ConfigEntryType.CIPHER_KEY);
                return cipher != null;
            }
        }

        /// <summary>
        ///   * Determines whether the Sword Book is enciphered and without a key.
        /// </summary>
        /// <returns> true if enciphered </returns>
        public bool isLocked
        {
            get
            {
                string cipher = (string) getValue(ConfigEntryType.CIPHER_KEY);
                return cipher != null && cipher.Length == 0;
            }
        }

        /// <summary>
        ///   * Determines whether the Sword Book's conf is supported by JSword.
        /// </summary>
        public bool isQuestionable
        {
            get { return questionable; }
        }

        /// <summary>
        ///   * Determines whether the Sword Book's conf is supported by JSword.
        /// </summary>
        public bool isSupported
        {
            get { return supported; }
        }

        /// <summary>
        ///   * Returns an Enumeration of all the known keys found in the config file.
        /// </summary>
        public Dictionary<ConfigEntryType, ConfigEntry>.KeyCollection Keys
        {
            get { return table.Keys; }
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
        ///   * Gets the unlock key for the module.
        /// </summary>
        /// <returns> the unlock key, if any, null otherwise. </returns>
        public string UnlockKey
        {
            get { return (string) getValue(ConfigEntryType.CIPHER_KEY); }
        }

        #endregion Properties

        #region Methods

        /// <summary>
        ///   * A helper to create/replace a value for a given type.
        /// </summary>
        /// <param name = "type"> </param>
        /// <param name = "aValue"> </param>
        public void add(ConfigEntryType type, string aValue)
        {
            if (!table.ContainsKey(type))
            {
                var ce = new ConfigEntry(@internal, type, aValue);
                table.Add(type, ce);
            }
        }

        /// <summary>
        ///   * Gets a particular ConfigEntry's value by its type
        /// </summary>
        /// <param name = "type">
        ///   of the ConfigEntry </param>
        /// <returns> the requested value, the default (if there is no entry) or null
        ///   (if there is no default) </returns>
        public object getValue(ConfigEntryType type)
        {
            ConfigEntry ce = null;
            table.TryGetValue(type, out ce);
            if (ce != null)
            {
                return ce.Value;
            }
            return type.Default;
        }

        public void load(Stream streamInput)
        {
            StreamReader @in = null;
            try
            {
                // Quiet Android from complaining about using the default BufferReader buffer size.
                // The actual buffer size is undocumented. So this is a good idea any way.
                @in = new StreamReader(streamInput);
                loadInitials(@in);
                loadContents(@in);
                @in.Close();
                @in = null;
                if (getValue(ConfigEntryType.ENCODING).Equals(ENCODING_LATIN1))
                {
                    supported = true;
                    questionable = false;
                    readahead = null;
                    table.Clear();
                    extra.Clear();
                    @in = new StreamReader(streamInput);
                    loadInitials(@in);
                    loadContents(@in);
                    @in.Close();
                    @in = null;
                }
                adjustDataPath();
                adjustLanguage();
                adjustName();
                validate();
            }
            finally
            {
                if (@in != null)
                {
                    @in.Close();
                }
            }
        }

        /// <summary>
        ///   * Load the conf from a buffer. This is used to load conf entries from the
        ///   mods.d.tar.gz file.
        /// </summary>
        /// <param name = "buffer">
        ///   the buffer to load </param>
        /// <exception cref = "IOException"> </exception>
        public void load(byte[] buffer)
        {
            load(new MemoryStream(buffer));
        }

        /// <summary>
        ///   * Determine whether this ConfigEntryTable has the ConfigEntry and it
        ///   matches the value.
        /// </summary>
        /// <param name = "type">
        ///   The kind of ConfigEntry to look for </param>
        /// <param name = "search">
        ///   the value to match against </param>
        /// <returns> true if there is a matching ConfigEntry matching the value </returns>
        public bool match(ConfigEntryType type, object search)
        {
            var ce = table[type];
            return ce != null && ce.match(search);
        }

        private void adjustDataPath()
        {
            string datapath = (string) getValue(ConfigEntryType.ADataPath);
            if (datapath == null)
            {
                datapath = string.Empty;
            }
            if (datapath.StartsWith("./"))
            {
                datapath = datapath.Substring(2);
            }
            add(ConfigEntryType.ADataPath, datapath);
        }

        private void adjustLanguage()
        {
            var lang = (Language) getValue(ConfigEntryType.LANG);
            if (lang == null)
            {
                lang = Language.DefaultLang;
                add(ConfigEntryType.LANG, lang.ToString());
            }
            testLanguage(@internal, lang);

            var langFrom = (Language) getValue(ConfigEntryType.GLOSSARY_FROM);
            var langTo = (Language) getValue(ConfigEntryType.GLOSSARY_TO);

            // If we have either langFrom or langTo, we are dealing with a glossary
            if (langFrom != null || langTo != null)
            {
                if (langFrom == null)
                {
                    //Logger.Warn("Missing data for " + @internal + ". Assuming " + ConfigEntryType.GLOSSARY_FROM.Name + '=' + Languages.DEFAULT_LANG_CODE);
                    langFrom = Language.DefaultLang;
                    add(ConfigEntryType.GLOSSARY_FROM, lang.Code);
                }
                testLanguage(@internal, langFrom);

                if (langTo == null)
                {
                    //Logger.Warn("Missing data for " + @internal + ". Assuming " + ConfigEntryType.GLOSSARY_TO.Name + '=' + Languages.DEFAULT_LANG_CODE);
                    langTo = Language.DefaultLang;
                    add(ConfigEntryType.GLOSSARY_TO, lang.Code);
                }
                testLanguage(@internal, langTo);

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
                    add(ConfigEntryType.LANG, lang.Code);
                }
            }
        }

        private void adjustName()
        {
            // If there is no name then use the internal name
            if (table[ConfigEntryType.DESCRIPTION] == null)
            {
                //Logger.Fail("Malformed conf file for " + @internal + " no " + ConfigEntryType.DESCRIPTION.Name + " found. Using internal of " + @internal);
                add(ConfigEntryType.DESCRIPTION, @internal);
            }
        }

        /// <summary>
        ///   * Get the next line from the input
        /// </summary>
        /// <param name = "bin">
        ///   The reader to get data from </param>
        /// <returns> the next line </returns>
        /// <exception cref = "IOException"> </exception>
        private string advance(StreamReader bin)
        {
            // Was something put back? If so, return it.
            if (readahead != null)
            {
                string line = readahead;
                readahead = null;
                return line;
            }

            // Get the next non-blank, non-comment line
            string trimmed = null;
            for (string line = bin.ReadLine(); line != null; line = bin.ReadLine())
            {
                // Remove trailing whitespace
                trimmed = line.Trim();

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
        ///   * Read too far ahead and need to return a line.
        /// </summary>
        private void backup(string oops)
        {
            if (oops.Length > 0)
            {
                readahead = oops;
            }
            else
            {
                // should never happen
                //Logger.Fail("Backup an empty string for " + @internal);
            }
        }

        /// <summary>
        ///   * Get continuation lines, if any.
        /// </summary>
        private void getContinuation(ConfigEntry configEntry, StreamReader bin, StringBuilder buf)
        {
            for (string line = advance(bin); line != null; line = advance(bin))
            {
                int length = buf.Length;

                // Look for bad data as this condition did exist
                bool continuation_expected = length > 0 && buf[length - 1] == '\\';

                if (continuation_expected)
                {
                    // delete the continuation character
                    buf.Remove(length - 1, 1);
                }

                if (isKeyLine(line))
                {
                    if (continuation_expected)
                    {
                        //Logger.Warn(report("Continuation followed by key for", configEntry.Name, line));
                    }

                    backup(line);
                    break;
                }
                else if (!continuation_expected)
                {
                    //Logger.Warn(report("Line without previous continuation for", configEntry.Name, line));
                }

                if (!configEntry.allowsContinuation())
                {
                    //Logger.Warn(report("Ignoring unexpected additional line for", configEntry.Name, line));
                }
                else
                {
                    if (continuation_expected)
                    {
                        buf.Append('\n');
                    }
                    buf.Append(line);
                }
            }
        }

        /// <summary>
        ///   * Does this line of text represent a key/value pair?
        /// </summary>
        private bool isKeyLine(string line)
        {
            return KEY_VALUE_PATTERN.IsMatch(line);
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
        ///   * Build's a SWORD conf file as a string. The result is not identical to the
        ///   original, cleaning up problems in the original and re-arranging the
        ///   entries into a predictable order.
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
                        writer.close();
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
        private void loadContents(StreamReader @in)
        {
            var buf = new StringBuilder();
            while (true)
            {
                // Empty out the buffer
                buf.Length = 0;

                string line = advance(@in);
                if (line == null)
                {
                    break;
                }

                // skip blank lines
                if (line.Length == 0)
                {
                    continue;
                }

                var matcher = KEY_VALUE_PATTERN.Match(line);
                if (matcher == null)
                {
                    //Logger.Fail("Expected to see '=' in " + @internal + ": " + line);
                    continue;
                }

                string key = matcher.Groups[1].Value.Trim();
                string value = matcher.Groups[2].Value.Trim();
                // Only CIPHER_KEYS that are empty are not ignored
                if (value.Length == 0 && !ConfigEntryType.CIPHER_KEY.Name.Equals(key))
                {
                    //Logger.Warn("Ignoring empty entry in " + @internal + ": " + line);
                    continue;
                }

                // Create a configEntry so that the name is normalized.
                var configEntry = new ConfigEntry(@internal, key);

                var type = configEntry.Type;

                ConfigEntry e = null;
                if (type != null)
                {
                    table.TryGetValue(type, out e);
                }
                if (e == null)
                {
                    if (type == null)
                    {
                        //Logger.Warn("Extra entry in " + @internal + " of " + configEntry.Name);
                        extra.Add(key, configEntry);
                    }
                    else if (type.isSynthetic)
                    {
                        //Logger.Warn("Ignoring unexpected entry in " + @internal + " of " + configEntry.Name);
                    }
                    else
                    {
                        table.Add(type, configEntry);
                    }
                }
                else
                {
                    configEntry = e;
                }

                buf.Append(value);
                getContinuation(configEntry, @in, buf);

                // History is a special case it is of the form History_x.x
                // The config entry is History without the x.x.
                // We want to put x.x at the beginning of the string
                value = buf.ToString();
                if (ConfigEntryType.HISTORY.Equals(type))
                {
                    int pos = key.IndexOf('_');
                    value = key.Substring(pos + 1) + ' ' + value;
                }

                configEntry.addValue(value);
            }
        }

        private void loadInitials(StreamReader @in)
        {
            string initials = null;
            while (true)
            {
                string line = advance(@in);
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
                initials = @internal;
            }
            add(ConfigEntryType.INITIALS, initials);
        }

        /*
        private void toOSIS(OSISUtil.OSISFactory factory, Element ele, string aTitle, ConfigEntryType[] category)
        {
            Element title = null;
            for (int i = 0; i < category.Length; i++)
            {
                ConfigEntry entry = table[category[i]];
                Element configElement = null;

                if (entry != null)
                {
                    configElement = entry.toOSIS();
                }

                if (title == null && configElement != null)
                {
                    // I18N(DMS): use aTitle to lookup translation.
                    title = factory.createHeader();
                    title.addContent(aTitle);
                    ele.addContent(title);
                }

                if (configElement != null)
                {
                    ele.addContent(configElement);
                }
            }
        }

        private void toConf(StringBuilder buf, ConfigEntryType[] category)
        {
            for (int i = 0; i < category.Length; i++)
            {

                ConfigEntry entry = table[category[i]];

                if (entry != null && !entry.Type.Synthetic)
                {
                    string text = entry.toConf();
                    if (text != null && text.Length > 0)
                    {
                        buf.Append(entry.toConf());
                    }
                }
            }
        }
        */
        /// <summary>
        ///   * Build an ordered map so that it displays in a consistent order.
        /// </summary>
        /// <summary>
        ///   * Build an ordered map so that it displays in a consistent order.
        /// </summary>
        /*
        private void toOSIS(OSISUtil.OSISFactory factory, Element ele, string aTitle, IDictionary<string, ConfigEntry> map)
        {
            Element title = null;
        // JAVA TO VB & C# CONVERTER TODO TASK: There is no .NET Dictionary equivalent to the Java 'entrySet' method:
            foreach (KeyValuePair<string, ConfigEntry> mapEntry in map.entrySet())
            {
                ConfigEntry entry = mapEntry.Value;
                Element configElement = null;

                if (entry != null)
                {
                    configElement = entry.toOSIS();
                }

                if (title == null && configElement != null)
                {
                    // I18N(DMS): use aTitle to lookup translation.
                    title = factory.createHeader();
                    title.addContent(aTitle);
                    ele.addContent(title);
                }

                if (configElement != null)
                {
                    ele.addContent(configElement);
                }
            }
        }
        */
        /*
        private void toConf(StringBuilder buf, IDictionary<string, ConfigEntry> map)
        {
        // JAVA TO VB & C# CONVERTER TODO TASK: There is no .NET Dictionary equivalent to the Java 'entrySet' method:
            foreach (KeyValuePair<string, ConfigEntry> mapEntry in map.entrySet())
            {
                ConfigEntry entry = mapEntry.Value;
                string text = entry.toConf();
                if (text != null && text.Length > 0)
                {
                    buf.Append(text);
                }
            }
        }*/
        private string report(string issue, string confEntryName, string line)
        {
            var buf = new StringBuilder(100);
            buf.Append(issue);
            buf.Append(' ');
            buf.Append(confEntryName);
            buf.Append(" in ");
            buf.Append(@internal);
            buf.Append(": ");
            buf.Append(line);

            return buf.ToString();
        }

        private void testLanguage(string initials, Language lang)
        {
            if (!lang.IsValidLanguage)
            {
                //Logger.Warn("Unknown language " + lang.Code + " in book " + initials);
            }
        }

        /// <summary>
        ///   * Determine which books are not supported. Also, report on problems.
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

        #endregion Methods
    }
}