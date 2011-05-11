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
/// ID: $Id: ConfigEntryType.java 2106 2011-03-07 21:14:31Z dmsmith $
/// 
/// Converted from Java to C# by Thomas Dilts with the help of a program from www.tangiblesoftwaresolutions.com
/// called 'Java to VB & C# Converter' on 2011-04-12 </summary>
/// 
namespace SwordBackend
{
    /*
    using Language = org.crosswire.common.util.Language;
    using BookCategory = org.crosswire.jsword.book.BookCategory;
    using System.Runtime.Serialization;*/
    using System;
    using System.Text.RegularExpressions;

    ///
    /// <summary> Constants for the keys in a Sword Config file. Taken from
    /// http://sword.sourceforge.net/cgi-bin/twiki/view/Swordapi/ConfFileLayout<br/>
    /// now located at
    /// http://www.crosswire.org/ucgi-bin/twiki/view/Swordapi/ConfFileLayout<br/>
    /// now located at http://www.crosswire.org/wiki/index.php/DevTools:Modules<br/>
    /// now located at http://www.crosswire.org/wiki/DevTools:confFiles<br/>
    /// <p>
    /// Note: This file is organized the same as the latest wiki documentation.
    ///  </summary>
    /// <seealso cref= gnu.lgpl.License for license details.<br>
    ///      The copyright to this program is held by it's authors.
    /// @author Joe Walker [joe at eireneh dot com]
    /// @author DM Smith [dmsmith555 at yahoo dot com] </seealso>
    /// 
    public class ConfigEntryType
    {
        #region Fields

        public const string DIRECTION_BIDI = "bidi";

        ///    
        /// <summary>* Constants for direction </summary>
        ///     
        public const string DIRECTION_LTOR = "LtoR";
        public const string DIRECTION_RTOL = "RtoL";

        ///    
        /// <summary>* The short name of this book. </summary>
        ///     
        public static readonly ConfigEntryType ABBREVIATION = new ConfigEntryType("Abbreviation");

        ///    
        ///     <summary> * Contains rtf that describes the book. </summary>
        ///     
        public static readonly ConfigEntryType_ABOUT ABOUT = new ConfigEntryType_ABOUT();
        public static readonly string[] A_BLOCK_TYPE_PICKS = new string[] { "BOOK", "CHAPTER", "VERSE" };
        public static readonly string[] A_COMPRESS_TYPE_PICKS = new string[] { "LZSS", "ZIP" };
        public static readonly ConfigEntryType_DATA_PATH A_DATA_PATH = new ConfigEntryType_DATA_PATH();
        public static readonly string[] A_DIRECTION_PICKS = new string[] { DIRECTION_LTOR, DIRECTION_RTOL, DIRECTION_BIDI };
        public static readonly string[] A_ENCODING_PICKS = new string[] { "Latin-1", "UTF-8" };
        public static readonly string[] A_FEATURE_PICKS = new string[] { "StrongsNumbers", "GreekDef", "HebrewDef", "GreekParse", "HebrewParse", "DailyDevotion", "Glossary", "Images" };
        public static readonly string[] A_GLOBAL_OPTION_FILTER_PICKS = new string[] { "GBFStrongs", "GBFFootnotes", "GBFScripref", "GBFMorph", "GBFHeadings", "GBFRedLetterWords", "ThMLStrongs", "ThMLFootnotes", "ThMLScripref", "ThMLMorph", "ThMLHeadings", "ThMLVariants", "ThMLLemma", "UTF8Cantillation", "UTF8GreekAccents", "UTF8HebrewPoints", "OSISStrongs", "OSISFootnotes", "OSISScripref", "OSISMorph", "OSISHeadings", "OSISRedLetterWords", "OSISLemma", "OSISRuby" };
        public static readonly string[] A_KEY_TYPE_PICKS = new string[] { "TreeKey", "VerseKey" };
        public static readonly string[] A_LICENSE_PICKS = new string[] { "Public Domain", "Copyrighted", "Copyrighted; Free non-commercial distribution", "Copyrighted; Permission to distribute granted to CrossWire", "Copyrighted; Freely distributable", "Copyrighted; Permission granted to distribute non-commercially in Sword format", "GFDL", "GPL", "Creative Commons: by-nc-nd", "Creative Commons: by-nc-sa", "Creative Commons: by-nc", "Creative Commons: by-nd", "Creative Commons: by-sa", "Creative Commons: by" };
        public static readonly string[] A_MOD_DRV_PICKS = new string[] { "RawText", "zText", "RawCom", "RawCom4", "zCom", "HREFCom", "RawFiles", "RawLD", "RawLD4", "zLD", "RawGenBook" };
        public static readonly string[] A_SOURCE_TYPE_PICKS = new string[] { "Plaintext", "GBF", "ThML", "OSIS", "TEI", "OSIS", "TEI" };
        public static readonly string[] A_VERSIFICATION_PICKS = new string[] { "KJV", "KJVA", "NRSV", "NRSVA", "Leningrad", "MT" };

        ///    
        ///     <summary> * single value integer, unknown use, some indications that we ought to be
        ///     * using it </summary>
        ///     
        public static readonly ConfigEntryType_BLOCK_COUNT BLOCK_COUNT = new ConfigEntryType_BLOCK_COUNT();

        ///    
        /// <summary>* The level at which compression is applied, BOOK, CHAPTER, or VERSE </summary>
        ///     
        public static readonly ConfigEntryType BLOCK_TYPE = new ConfigEntryPickType("BlockType", A_BLOCK_TYPE_PICKS, A_BLOCK_TYPE_PICKS[0]);
        public static readonly string[] BOOLEAN_PICKS = new string[] { "true", "false" };
        public static readonly string[] CATEGORY_PICKS = new string[] { "Daily Devotional", "Glossaries", "Cults / Unorthodox / Questionable Material", "Essays", "Maps", "Images", "Biblical Texts", "Commentaries", "Lexicons / Dictionaries", "Generic Books" };

        ///    
        /// <summary>* If this exists in the conf, then the book is encrypted. The value is used
        /// to unlock the book. The encryption algorithm is Sapphire. </summary>
        ///     
        public static readonly ConfigEntryType CIPHER_KEY = new ConfigEntryType("CipherKey");

        ///    
        /// <summary>* The type of compression in use. JSword does not support LZSS. While it is
        /// the default, it is not used. At least so far. </summary>
        ///     
        public static readonly ConfigEntryType COMPRESS_TYPE = new ConfigEntryPickType("CompressType", A_COMPRESS_TYPE_PICKS, A_COMPRESS_TYPE_PICKS[0]);

        ///    
        ///     <summary> * Informational copyright notice. </summary>
        ///     
        public static readonly ConfigEntryType_COPYRIGHT COPYRIGHT = new ConfigEntryType_COPYRIGHT();

        ///    
        ///     <summary> * Copyright info. Informational only. </summary>
        ///
        public static readonly ConfigEntryType_COPYRIGHT_CONTACT_ADDRESS COPYRIGHT_CONTACT_ADDRESS = new ConfigEntryType_COPYRIGHT_CONTACT_ADDRESS();

        ///    
        /// <summary>* Copyright info. Informational only. </summary>
        ///     
        public static readonly ConfigEntryType COPYRIGHT_CONTACT_EMAIL = new ConfigEntryType("CopyrightContactEmail");

        ///    
        ///     <summary> * Copyright info. Informational only. </summary>
        ///
        public static readonly ConfigEntryType_COPYRIGHT_CONTACT_NAME COPYRIGHT_CONTACT_NAME = new ConfigEntryType_COPYRIGHT_CONTACT_NAME();

        ///    
        ///     <summary> * Copyright info. Informational only. </summary>
        ///
        public static readonly ConfigEntryType_COPYRIGHT_CONTACT_NOTES COPYRIGHT_CONTACT_NOTES = new ConfigEntryType_COPYRIGHT_CONTACT_NOTES();

        ///    
        ///     <summary> * Copyright info. Informational only. This is a year, a year range or a
        ///     * comma separated list of these. </summary>
        ///     
        public static readonly ConfigEntryType_COPYRIGHT_DATE COPYRIGHT_DATE = new ConfigEntryType_COPYRIGHT_DATE();

        ///    
        /// <summary>* single value string, unknown use </summary>
        ///     
        public static readonly ConfigEntryType COPYRIGHT_HOLDER = new ConfigEntryType("CopyrightHolder");

        ///    
        ///     <summary> * Copyright info. Informational only. </summary>
        ///     
        public static readonly ConfigEntryType_COPYRIGHT_NOTES COPYRIGHT_NOTES = new ConfigEntryType_COPYRIGHT_NOTES();

        ///    
        /// <summary>* The full name of this book </summary>
        ///     
        public static readonly ConfigEntryType DESCRIPTION = new ConfigEntryType("Description");

        ///    
        /// <summary>* The layout direction of the text in the book. Hebrew, Arabic and Farsi
        /// RtoL. Most are 'LtoR'. Some are 'bidi', bi-directional. E.g.
        /// hebrew-english glossary. </summary>
        ///     
        public static readonly ConfigEntryType DIRECTION = new ConfigEntryPickType("Direction", A_DIRECTION_PICKS, A_DIRECTION_PICKS[0]);
        public static readonly ConfigEntryType_DISPLAY_LEVEL DISPLAY_LEVEL = new ConfigEntryType_DISPLAY_LEVEL();

        ///    
        /// <summary>* Copyright info. Informational only. </summary>
        ///     
        public static readonly ConfigEntryType DISTRIBUTION_LICENSE = new ConfigEntryPickType("DistributionLicense", A_LICENSE_PICKS, A_LICENSE_PICKS[0]);

        ///    
        ///     <summary> * Copyright info. Informational only. </summary>
        ///
        public static readonly ConfigEntryType_DISTRIBUTION_NOTES DISTRIBUTION_NOTES = new ConfigEntryType_DISTRIBUTION_NOTES();

        ///    
        ///     <summary> * Similar to DataPath. It gives where on the CrossWire server the book can
        ///     * be found. Informational only. </summary>
        ///
        public static readonly ConfigEntryType_DISTRIBUTION_SOURCE DISTRIBUTION_SOURCE = new ConfigEntryType_DISTRIBUTION_SOURCE();

        ///    
        /// <summary>* The character encoding. Only Latin-1 and UTF-8 are supported. </summary>
        ///     
        public static readonly ConfigEntryType ENCODING = new ConfigEntryPickType("Encoding", A_ENCODING_PICKS, A_ENCODING_PICKS[0]);

        ///    
        ///     <summary> * A Feature describes a characteristic of the Book. </summary>
        ///     
        public static readonly ConfigEntryType_FEATURE FEATURE = new ConfigEntryType_FEATURE();

        ///    
        /// <summary>* A recommended font to use for the book. </summary>
        ///     
        public static readonly ConfigEntryType FONT = new ConfigEntryType("Font");

        ///    
        ///     <summary> * Global Option Filters are the names of routines in Sword that can be used
        ///     * to display the data. These are not used by JSword. </summary>
        ///     
        public static readonly ConfigEntryType_GLOBAL_OPTION_FILTER GLOBAL_OPTION_FILTER = new ConfigEntryType_GLOBAL_OPTION_FILTER();

        ///    
        ///     <summary> * Books with a Feature of Glossary are used to map words FROM one language
        ///     * TO another. </summary>
        ///     
        public static readonly ConfigEntryType_GLOSSARY_FROM GLOSSARY_FROM = new ConfigEntryType_GLOSSARY_FROM();

        ///    
        ///     <summary> * Books with a Feature of Glossary are used to map words FROM one language
        ///     * TO another. </summary>
        ///     
        public static readonly ConfigEntryType_GLOSSARY_TO GLOSSARY_TO = new ConfigEntryType_GLOSSARY_TO();

        ///    
        ///     <summary> * multiple values starting with History, some sort of change-log. In the
        ///     * conf these are of the form History_x.y. We strip off the x.y and prefix
        ///     * the value with it. The x.y corresponds to a current or prior Version
        ///     * value. </summary>
        ///     
        public static readonly ConfigEntryType_HISTORY HISTORY = new ConfigEntryType_HISTORY();

        ///    
        /// <summary>* The abbreviated name by which this book is known. This is in the [] on
        /// the first non-blank line of the conf. JSword uses this for display and
        /// access purposes. </summary>
        ///     
        public static readonly ConfigEntryType INITIALS = new ConfigEntrySyntheticType("Initials");

        ///    
        ///     <summary> * The installed size of the book in bytes. This is not the size of the zip
        ///     * that is downloaded. </summary>
        ///
        public static readonly ConfigEntryType_INSTALL_SIZE INSTALL_SIZE = new ConfigEntryType_INSTALL_SIZE();

        ///    
        /// <summary>* The kind of key that a Generic Book uses. </summary>
        ///     
        public static readonly ConfigEntryType KEY_TYPE = new ConfigEntryPickType("KeyType", A_KEY_TYPE_PICKS, A_KEY_TYPE_PICKS[0]);

        ///    
        ///     <summary> * single value string, defaults to en, the language of the book </summary>
        ///     
        public static readonly ConfigEntryType_LANG LANG = new ConfigEntryType_LANG();

        ///    
        /// <summary>* Library of Congress Subject Heading. Typically this is of the form
        /// BookCategory Scope Language, where scope is typically O.T., N.T. </summary>
        ///     
        public static readonly ConfigEntryType LCSH = new ConfigEntryType("LCSH");

        ///    
        /// <summary>* The location of a collection of modules. JSword uses this to install and
        /// delete a module. </summary>
        ///     
        public static readonly ConfigEntryType LIBRARY_URL = new ConfigEntrySyntheticType("LibraryURL");

        ///    
        /// <summary>* The location of the module. JSword uses this to access a module. </summary>
        ///     
        public static readonly ConfigEntryType LOCATION_URL = new ConfigEntrySyntheticType("LocationURL");

        ///    
        /// <summary>* single value version number, lowest sword c++ version that can read this
        /// book JSword does not use this value. </summary>
        ///     
        public static readonly ConfigEntryType MINIMUM_VERSION = new ConfigEntryType("MinimumVersion", "1.5.1a");

        ///    
        /// <summary>* This indicates how the book was stored. </summary>
        ///     
        public static readonly ConfigEntryType MOD_DRV = new ConfigEntryPickType("ModDrv", A_MOD_DRV_PICKS);

        ///    
        ///     <summary> * A list of prior "initials" for the current book.
        ///     * TODO(dms): when a user installs a book with an obsoletes that matches
        ///     * an installed book, offer the user the opportunity to delete the old book. </summary>
        ///     
        public static readonly ConfigEntryType_OBSOLETES OBSOLETES = new ConfigEntryType_OBSOLETES();

        ///    
        ///     <summary> * When false do not show quotation marks for OSIS text that has <q>
        ///     * elements. </summary>
        ///     
        public static readonly ConfigEntryType_OSIS_Q_TO_TICK OSIS_Q_TO_TICK = new ConfigEntryType_OSIS_Q_TO_TICK();

        ///    
        /// <summary>* single value version number, lowest sword c++ version that can read this
        /// book JSword does not use this value. </summary>
        ///     
        public static readonly ConfigEntryType OSIS_VERSION = new ConfigEntryType("OSISVersion", "2.0");

        ///    
        /// <summary>* A one line copyright statement, required by Lockman for NASB </summary>
        ///     
        public static readonly ConfigEntryType SHORT_COPYRIGHT = new ConfigEntryType("ShortCopyright");

        ///    
        /// <summary>* A one line promo statement, required by Lockman for NASB </summary>
        ///     
        public static readonly ConfigEntryType SHORT_PROMO = new ConfigEntryType("ShortPromo");

        ///    
        /// <summary>* This indicates the kind of markup used for the book. </summary>
        ///     
        public static readonly ConfigEntryType SOURCE_TYPE = new ConfigEntryPickType("SourceType", A_SOURCE_TYPE_PICKS, A_SOURCE_TYPE_PICKS[0]);

        ///    
        ///     <summary> * The date that this version of the book was last updated. Informational
        ///     * only. </summary>
        ///
        public static readonly ConfigEntryType_SWORD_VERSION_DATE SWORD_VERSION_DATE = new ConfigEntryType_SWORD_VERSION_DATE();

        ///    
        ///     <summary> * Information on where the book's text was obtained. </summary>
        ///
        public static readonly ConfigEntryType_TEXT_SOURCE TEXT_SOURCE = new ConfigEntryType_TEXT_SOURCE();

        ///    
        /// <summary>* This indicates the versification of the book, with KJV being the default. </summary>
        ///     
        public static readonly ConfigEntryType VERSIFICATION = new ConfigEntryPickType("Versification", A_VERSIFICATION_PICKS);

        ///    
        ///     <summary> * An informational string indicating the current version of the book. </summary>
        ///     
        public static readonly ConfigEntryType_VERSION VERSION = new ConfigEntryType_VERSION();

        private static readonly ConfigEntryType[] VALUES = { INITIALS, A_DATA_PATH, DESCRIPTION, MOD_DRV, COMPRESS_TYPE, BLOCK_TYPE, BLOCK_COUNT, KEY_TYPE, CIPHER_KEY, VERSIFICATION, GLOBAL_OPTION_FILTER, DIRECTION, SOURCE_TYPE, ENCODING, DISPLAY_LEVEL, FONT, OSIS_Q_TO_TICK, FEATURE, GLOSSARY_FROM, GLOSSARY_TO, ABBREVIATION, ABOUT, VERSION, HISTORY, MINIMUM_VERSION, LCSH, LANG, INSTALL_SIZE, SWORD_VERSION_DATE, OBSOLETES, COPYRIGHT, COPYRIGHT_HOLDER, COPYRIGHT_DATE, COPYRIGHT_NOTES, COPYRIGHT_CONTACT_NAME, COPYRIGHT_CONTACT_NOTES, COPYRIGHT_CONTACT_ADDRESS, COPYRIGHT_CONTACT_EMAIL, SHORT_PROMO, SHORT_COPYRIGHT, DISTRIBUTION_LICENSE, DISTRIBUTION_NOTES, TEXT_SOURCE, DISTRIBUTION_SOURCE, OSIS_VERSION, LIBRARY_URL, LOCATION_URL };

        private readonly int obj = nextObj++;

        // Support for serialization
        private static int nextObj;

        ///    
        /// <summary>* The default for the ConfigEntryType </summary>
        ///     
        private object defaultValue;

        ///    
        /// <summary>* The name of the ConfigEntryType </summary>
        ///     
        private string name;

        #endregion Fields

        #region Constructors

        ///    
        /// <summary>* Simple ctor </summary>
        ///     
        protected internal ConfigEntryType(string name)
            : this(name, null)
        {
        }

        ///    
        /// <summary>* Simple ctor </summary>
        ///     
        protected internal ConfigEntryType(string name, object defaultValue)
        {
            this.name = name;
            this.defaultValue = defaultValue;
        }

        #endregion Constructors

        #region Properties

        ///    
        /// <summary>* Some ConfigEntryTypes have defaults.
        ///  </summary>
        /// <returns> the default, if there is one, null otherwise </returns>
        ///     
        public virtual object Default
        {
            get
            {
                return defaultValue;
            }
        }

        ///    
        /// <summary>* Synthetic keys are those that are not in the Sword Book's conf, but are
        /// needed by the program. Typically, these are derived by the program from
        /// the other entries.
        ///  </summary>
        /// <returns> true if this is synthetic </returns>
        ///     
        public virtual bool isSynthetic
        {
            get
            {
                return false;
            }
        }

        ///    
        /// <summary>* Returns the normalized name of this ConfigEntry.
        ///  </summary>
        /// <returns> the name </returns>
        ///     
        public virtual string Name
        {
            get
            {
                return name;
            }
        }

        #endregion Properties

        #region Methods

        ///    
        /// <summary>* Lookup method to convert from an integer </summary>
        ///     
        public static ConfigEntryType fromInteger(int i)
        {
            return VALUES[i];
        }

        ///    
        /// <summary>* Lookup method to convert from a string </summary>
        ///     
        public static ConfigEntryType fromString(string name)
        {
            if (name != null)
            {
                // special case
                if (name.StartsWith(ConfigEntryType.HISTORY.ToString()))
                {
                    return ConfigEntryType.HISTORY;
                }

                for (int i = 0; i < VALUES.Length; i++)
                {
                    ConfigEntryType o = VALUES[i];
                    if (name.Equals(o.name))
                    {
                        return o;
                    }
                }
            }

            // should not get here.
            // But there are typos in the keys in the book conf files
            // And this allows for the addition of new fields in
            // advance of changing JSword
            return null;
        }

        ///    
        /// <summary>* While most fields are single line or single value, some allow
        /// continuation. A continuation mark is a backslash at the end of a line. It
        /// is not to be followed by whitespace.
        ///  </summary>
        /// <returns> true if continuation is allowed </returns>
        ///     
        public virtual bool allowsContinuation()
        {
            return false;
        }

        ///    
        /// <summary>* RTF is allowed in a few config entries.
        ///  </summary>
        /// <returns> true if rtf is allowed </returns>
        ///     
        public virtual bool allowsRTF()
        {
            return false;
        }

        ///    
        /// <summary>* Convert the string value from the conf into the representation of this
        /// ConfigEntryType.
        ///  </summary>
        /// <returns> the converted object </returns>
        ///     
        public virtual object convert(string input)
        {
            return input;
        }

        ///    
        /// <summary>* Prevent subclasses from overriding canonical identity based Object
        /// methods
        ///  </summary>
        /// <seealso cref= java.lang.Object#equals(java.lang.Object) </seealso>
        ///     
        public override bool Equals(object o)
        {
            return base.Equals(o);
        }

        ///    
        /// <summary>* Modify the value if necessary.
        ///  </summary>
        /// <param name="value">
        ///            the input </param>
        /// <returns> either value or a modified version of it. </returns>
        ///     
        public virtual string filter(string value)
        {
            return value;
        }

        ///    
        /// <summary>* Prevent subclasses from overriding canonical identity based Object
        /// methods
        ///  </summary>
        /// <seealso cref= java.lang.Object#hashCode() </seealso>
        ///     
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        ///    
        /// <summary>* Determines whether the string is allowed. For some config entries, the
        /// value is expected to be one of a group, for others the format is defined.
        ///  </summary>
        /// <param name="value">
        ///            the string to be checked </param>
        /// <returns> true if the string is allowed </returns>
        ///     
        public virtual bool isAllowed(string value)
        {
            return value != null;
        }

        ///    
        /// <summary>* Some keys can repeat. When this happens each is a single value pick from
        /// a list of choices.
        ///  </summary>
        /// <returns> true if this ConfigEntryType can occur more than once </returns>
        ///     
        public virtual bool mayRepeat()
        {
            return false;
        }

        ///    
        /// <summary>* Determines the level of detail stored in the histogram.
        ///  </summary>
        /// <returns> true if the ConfigEntry should report histogram for repetitions </returns>
        ///     
        public virtual bool reportDetails()
        {
            return true;
        }

        /*
         * (non-Javadoc)
         *
         * @see java.lang.Object#toString()
         */
        public override string ToString()
        {
            return name;
        }

        ///    
        /// <summary>* Some keys can repeat. When this happens each is a single value pick from
        /// a list of choices.
        ///  </summary>
        /// <returns> true if this ConfigEntryType can occur more than once </returns>
        ///     
        protected internal virtual bool hasChoices()
        {
            return false;
        }

        internal virtual object readResolve()
        {
            return VALUES[obj];
        }

        #endregion Methods

        #region Nested Types

        ///    
        /// <summary>* A ConfigEntryPickType is a ConfigEntryType that allows values from a pick
        /// list. Matching is expected to be case-sensitive, but data problems
        /// dictate a more flexible approach.
        ///  </summary>
        ///     
        public class ConfigEntryPickType : ConfigEntryType
        {
            #region Fields

            ///        
            ///         <summary> * The array of choices. </summary>
            ///         
            private readonly string[] choiceArray;

            #endregion Fields

            #region Constructors

            ///        
            ///         <summary> * Simple ctor </summary>
            ///         
            public ConfigEntryPickType(string name, string[] picks)
                : this(name, picks, null)
            {
            }

            ///        
            ///         <summary> * Simple ctor </summary>
            ///         
            public ConfigEntryPickType(string name, string[] picks, object defaultPick)
                : base(name, defaultPick)
            {
                choiceArray = (string[])picks.Clone();
            }

            #endregion Constructors

            #region Methods

            /*
             * (non-Javadoc)
             *
             * @see
             * org.crosswire.jsword.book.sword.ConfigEntryType#filter(java.lang.
             * string)
             */
            public override string filter(string value)
            {
                // Do we have an exact match?
                for (int i = 0; i < choiceArray.Length; i++)
                {
                    if (choiceArray[i].Equals(value))
                    {
                        return value;
                    }
                }

                // Do we have a case insensitive match?
                for (int i = 0; i < choiceArray.Length; i++)
                {
                    if (choiceArray[i].ToUpper() == value.ToUpper())
                    {
                        return choiceArray[i];
                    }
                }

                // No match at all!
                return value;
            }

            /*
             * (non-Javadoc)
             *
             * @see
             * org.crosswire.jsword.book.sword.ConfigEntryType#isAllowed(java.lang
             * .String)
             */
            public override bool isAllowed(string value)
            {
                for (int i = 0; i < choiceArray.Length; i++)
                {
                    if (choiceArray[i].ToUpper() == value.ToUpper())
                    {
                        return true;
                    }
                }

                return false;
            }

            /*
             * (non-Javadoc)
             *
             * @see org.crosswire.jsword.book.sword.ConfigEntryType#hasChoices()
             */
            protected internal override bool hasChoices()
            {
                return true;
            }

            #endregion Methods
        }

        ///    
        /// <summary>* Represents a ConfigEntryType that is not actually represented by the
        /// Sword Config file.
        ///  </summary>
        ///     
        public class ConfigEntrySyntheticType : ConfigEntryType
        {
            #region Constructors

            ///        
            ///         <summary> * Simple ctor </summary>
            ///         
            // JAVA TO VB & C# CONVERTER TODO TASK: C# doesn't allow accessing outer class instance members within a nested class:
            public ConfigEntrySyntheticType(string name)
                : base(name)
            {
            }

            #endregion Constructors

            #region Properties

            /*
             * (non-Javadoc)
             *
             * @see org.crosswire.jsword.book.sword.ConfigEntryType#isSynthetic()
             */
            public override bool isSynthetic
            {
                get
                {
                    return true;
                }
            }

            #endregion Properties
        }

        ///    
        ///     <summary> * Contains rtf that describes the book. </summary>
        ///     
        public class ConfigEntryType_ABOUT : ConfigEntryType
        {
            #region Constructors

            public ConfigEntryType_ABOUT()
                : base("About")
            {
            }

            #endregion Constructors

            #region Methods

            /*
             * (non-Javadoc)
             *
             * @see
             * org.crosswire.jsword.book.sword.ConfigEntryType#allowsContinuation()
             */
            public override bool allowsContinuation()
            {
                return true;
            }

            /*
             * (non-Javadoc)
             *
             * @see org.crosswire.jsword.book.sword.ConfigEntryType#allowsRTF()
             */
            public override bool allowsRTF()
            {
                return true;
            }

            #endregion Methods
        }

        ///    
        ///     <summary> * single value integer, unknown use, some indications that we ought to be
        ///     * using it </summary>
        ///     
        public class ConfigEntryType_BLOCK_COUNT : ConfigEntryType
        {
            #region Constructors

            public ConfigEntryType_BLOCK_COUNT()
                : base("BlockCount", 200)
            {
            }

            #endregion Constructors

            #region Methods

            /*
             * (non-Javadoc)
             *
             * @see
             * org.crosswire.jsword.book.sword.ConfigEntryType#convert(java.lang
             * .String)
             */
            public override object convert(string input)
            {
                try
                {
                    return Convert.ToInt32(input);
                }
                catch (Exception)
                {
                    return Default;
                }
            }

            /*
             * (non-Javadoc)
             *
             * @see
             * org.crosswire.jsword.book.sword.ConfigEntryType#isAllowed(java.lang
             * .String)
             */
            public override bool isAllowed(string aValue)
            {
                try
                {
                    Convert.ToInt32(aValue);
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }

            #endregion Methods
        }

        ///    
        ///     <summary> * Informational copyright notice. </summary>
        ///     
        public class ConfigEntryType_COPYRIGHT : ConfigEntryType
        {
            #region Constructors

            public ConfigEntryType_COPYRIGHT()
                : base("Copyright")
            {
            }

            #endregion Constructors

            #region Methods

            public override bool allowsContinuation()
            {
                return true;
            }

            #endregion Methods
        }

        ///    
        ///     <summary> * Copyright info. Informational only. </summary>
        ///     
        public class ConfigEntryType_COPYRIGHT_CONTACT_ADDRESS : ConfigEntryType
        {
            #region Constructors

            public ConfigEntryType_COPYRIGHT_CONTACT_ADDRESS()
                : base("CopyrightContactAddress")
            {
            }

            #endregion Constructors

            #region Methods

            public override bool allowsContinuation()
            {
                return true;
            }

            public override bool allowsRTF()
            {
                return true;
            }

            #endregion Methods
        }

        ///    
        ///     <summary> * Copyright info. Informational only. </summary>
        ///     
        public class ConfigEntryType_COPYRIGHT_CONTACT_NAME : ConfigEntryType
        {
            #region Constructors

            public ConfigEntryType_COPYRIGHT_CONTACT_NAME()
                : base("CopyrightContactName")
            {
            }

            #endregion Constructors

            #region Methods

            public override bool allowsContinuation()
            {
                return true;
            }

            public override bool allowsRTF()
            {
                return true;
            }

            #endregion Methods
        }

        ///    
        ///     <summary> * Copyright info. Informational only. </summary>
        ///     
        public class ConfigEntryType_COPYRIGHT_CONTACT_NOTES : ConfigEntryType
        {
            #region Constructors

            public ConfigEntryType_COPYRIGHT_CONTACT_NOTES()
                : base("CopyrightContactNotes")
            {
            }

            #endregion Constructors

            #region Methods

            public override bool allowsContinuation()
            {
                return true;
            }

            public override bool allowsRTF()
            {
                return true;
            }

            #endregion Methods
        }

        ///    
        ///     <summary> * Copyright info. Informational only. This is a year, a year range or a
        ///     * comma separated list of these. </summary>
        ///     
        public class ConfigEntryType_COPYRIGHT_DATE : ConfigEntryType
        {
            #region Fields

            private string validDatePattern = "\\d{4}(\\s*-\\s*\\d{4})?(\\s*,\\s*\\d{4}(\\s*-\\s*\\d{4})?)*";

            #endregion Fields

            #region Constructors

            public ConfigEntryType_COPYRIGHT_DATE()
                : base("CopyrightDate")
            {
            }

            #endregion Constructors

            #region Methods

            public override bool isAllowed(string value)
            {
                Regex regx = new Regex(validDatePattern);
                return regx.IsMatch(value);
            }

            #endregion Methods
        }

        ///    
        ///     <summary> * Copyright info. Informational only. </summary>
        ///     
        public class ConfigEntryType_COPYRIGHT_NOTES : ConfigEntryType
        {
            #region Constructors

            public ConfigEntryType_COPYRIGHT_NOTES()
                : base("CopyrightNotes")
            {
            }

            #endregion Constructors

            #region Methods

            public override bool allowsContinuation()
            {
                return true;
            }

            public override bool allowsRTF()
            {
                return true;
            }

            #endregion Methods
        }

        ///    
        /// <summary>* Relative path to the data files, some issues with this </summary>
        ///     
        public class ConfigEntryType_DATA_PATH : ConfigEntryType
        {
            #region Constructors

            public ConfigEntryType_DATA_PATH()
                : base("DataPath")
            {
            }

            #endregion Constructors

            #region Methods

            public override bool isAllowed(string value)
            {
                return true;
            }

            #endregion Methods
        }

        ///    
        ///     <summary> * Display level is used by GenBooks to do auto expansion in the tree. A
        ///     * level of 2 indicates that the first two levels should be shown. </summary>
        ///     
        public class ConfigEntryType_DISPLAY_LEVEL : ConfigEntryType
        {
            #region Constructors

            public ConfigEntryType_DISPLAY_LEVEL()
                : base("DisplayLevel")
            {
            }

            #endregion Constructors

            #region Methods

            /*
             * (non-Javadoc)
             *
             * @see
             * org.crosswire.jsword.book.sword.ConfigEntryType#convert(java.lang
             * .String)
             */
            public override object convert(string input)
            {
                try
                {
                    return Convert.ToInt32(input);
                }
                catch (Exception)
                {
                    return null;
                }
            }

            /*
             * (non-Javadoc)
             *
             * @see
             * org.crosswire.jsword.book.sword.ConfigEntryType#isAllowed(java.lang
             * .String)
             */
            public override bool isAllowed(string value)
            {
                try
                {
                    Convert.ToInt32(value);
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }

            #endregion Methods
        }

        ///    
        ///     <summary> * Copyright info. Informational only. </summary>
        ///     
        public class ConfigEntryType_DISTRIBUTION_NOTES : ConfigEntryType
        {
            #region Constructors

            public ConfigEntryType_DISTRIBUTION_NOTES()
                : base("DistributionNotes")
            {
            }

            #endregion Constructors

            #region Methods

            public override bool allowsContinuation()
            {
                return true;
            }

            #endregion Methods
        }

        ///    
        ///     <summary> * Similar to DataPath. It gives where on the CrossWire server the book can
        ///     * be found. Informational only. </summary>
        ///     
        public class ConfigEntryType_DISTRIBUTION_SOURCE : ConfigEntryType
        {
            #region Constructors

            public ConfigEntryType_DISTRIBUTION_SOURCE()
                : base("DistributionSource")
            {
            }

            #endregion Constructors

            #region Methods

            public override bool allowsContinuation()
            {
                return true;
            }

            #endregion Methods
        }

        ///    
        ///     <summary> * A Feature describes a characteristic of the Book. </summary>
        ///     
        public class ConfigEntryType_FEATURE : ConfigEntryPickType
        {
            #region Constructors

            public ConfigEntryType_FEATURE()
                : base("Feature", A_FEATURE_PICKS)
            {
            }

            #endregion Constructors

            #region Methods

            /*
             * (non-Javadoc)
             *
             * @see org.crosswire.jsword.book.sword.ConfigEntryType#mayRepeat()
             */
            public override bool mayRepeat()
            {
                return true;
            }

            #endregion Methods
        }

        ///    
        ///     <summary> * Global Option Filters are the names of routines in Sword that can be used
        ///     * to display the data. These are not used by JSword. </summary>
        ///     
        public class ConfigEntryType_GLOBAL_OPTION_FILTER : ConfigEntryPickType
        {
            #region Constructors

            public ConfigEntryType_GLOBAL_OPTION_FILTER()
                : base("GlobalOptionFilter", A_GLOBAL_OPTION_FILTER_PICKS)
            {
            }

            #endregion Constructors

            #region Methods

            /*
             * (non-Javadoc)
             *
             * @see org.crosswire.jsword.book.sword.ConfigEntryType#mayRepeat()
             */
            public override bool mayRepeat()
            {
                return true;
            }

            #endregion Methods
        }

        ///    
        ///     <summary> * Books with a Feature of Glossary are used to map words FROM one language
        ///     * TO another. </summary>
        ///     
        public class ConfigEntryType_GLOSSARY_FROM : ConfigEntryType
        {
            #region Constructors

            public ConfigEntryType_GLOSSARY_FROM()
                : base("GlossaryFrom")
            {
            }

            #endregion Constructors

            #region Methods

            /*
             * (non-Javadoc)
             *
             * @see
             * org.crosswire.jsword.book.sword.ConfigEntryType#convert(java.lang
             * .String)
             */
            public override object convert(string input)
            {
                return new Language(input);
            }

            #endregion Methods
        }

        ///    
        ///     <summary> * Books with a Feature of Glossary are used to map words FROM one language
        ///     * TO another. </summary>
        ///     
        public class ConfigEntryType_GLOSSARY_TO : ConfigEntryType
        {
            #region Constructors

            public ConfigEntryType_GLOSSARY_TO()
                : base("GlossaryTo")
            {
            }

            #endregion Constructors

            #region Methods

            /*
             * (non-Javadoc)
             *
             * @see
             * org.crosswire.jsword.book.sword.ConfigEntryType#convert(java.lang
             * .String)
             */
            public override object convert(string input)
            {
                return new Language(input);
            }

            #endregion Methods
        }

        ///    
        ///     <summary> * multiple values starting with History, some sort of change-log. In the
        ///     * conf these are of the form History_x.y. We strip off the x.y and prefix
        ///     * the value with it. The x.y corresponds to a current or prior Version
        ///     * value. </summary>
        ///     
        public class ConfigEntryType_HISTORY : ConfigEntryType
        {
            #region Constructors

            public ConfigEntryType_HISTORY()
                : base("History")
            {
            }

            #endregion Constructors

            #region Methods

            /*
             * (non-Javadoc)
             *
             * @see org.crosswire.jsword.book.sword.ConfigEntryType#mayRepeat()
             */
            public override bool mayRepeat()
            {
                return true;
            }

            /*
             * (non-Javadoc)
             *
             * @see org.crosswire.jsword.book.sword.ConfigEntryType#reportDetails()
             */
            public override bool reportDetails()
            {
                return false;
            }

            #endregion Methods
        }

        ///    
        ///     <summary> * The installed size of the book in bytes. This is not the size of the zip
        ///     * that is downloaded. </summary>
        ///     
        public class ConfigEntryType_INSTALL_SIZE : ConfigEntryType
        {
            #region Constructors

            public ConfigEntryType_INSTALL_SIZE()
                : base("InstallSize")
            {
            }

            #endregion Constructors

            #region Methods

            public override object convert(string input)
            {
                try
                {
                    return Convert.ToInt32(input);
                }
                catch (Exception)
                {
                    return null;
                }
            }

            public override bool isAllowed(string value)
            {
                try
                {
                    Convert.ToInt32(value);
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }

            #endregion Methods
        }

        ///    
        ///     <summary> * single value string, defaults to en, the language of the book </summary>
        ///     
        public class ConfigEntryType_LANG : ConfigEntryType
        {
            #region Constructors

            public ConfigEntryType_LANG()
                : base("Lang", new Language(null))
            {
            }

            #endregion Constructors

            #region Methods

            public override object convert(string input)
            {
                return new Language(input);
            }

            #endregion Methods
        }

        ///    
        ///     <summary> * A list of prior "initials" for the current book.
        ///     * TODO(dms): when a user installs a book with an obsoletes that matches
        ///     * an installed book, offer the user the opportunity to delete the old book. </summary>
        ///     
        public class ConfigEntryType_OBSOLETES : ConfigEntryType
        {
            #region Constructors

            public ConfigEntryType_OBSOLETES()
                : base("Obsoletes")
            {
            }

            #endregion Constructors

            #region Methods

            public override bool mayRepeat()
            {
                return true;
            }

            public override bool reportDetails()
            {
                return false;
            }

            #endregion Methods
        }

        ///    
        ///     <summary> * When false do not show quotation marks for OSIS text that has <q>
        ///     * elements. </summary>
        ///     
        public class ConfigEntryType_OSIS_Q_TO_TICK : ConfigEntryPickType
        {
            #region Constructors

            public ConfigEntryType_OSIS_Q_TO_TICK()
                : base("OSISqToTick", BOOLEAN_PICKS, true)
            {
            }

            #endregion Constructors

            #region Methods

            /*
             * (non-Javadoc)
             *
             * @see
             * org.crosswire.jsword.book.sword.ConfigEntryType#convert(java.lang
             * .String)
             */
            public override object convert(string input)
            {
                return Convert.ToBoolean(input);
            }

            #endregion Methods
        }

        ///    
        ///     <summary> * The date that this version of the book was last updated. Informational
        ///     * only. </summary>
        ///     
        public class ConfigEntryType_SWORD_VERSION_DATE : ConfigEntryType
        {
            #region Fields

            private string validDatePattern = "\\d{4}-\\d{2}-\\d{2}";

            #endregion Fields

            #region Constructors

            public ConfigEntryType_SWORD_VERSION_DATE()
                : base("SwordVersionDate")
            {
            }

            #endregion Constructors

            #region Methods

            public override bool isAllowed(string value)
            {
                Regex regx = new Regex(validDatePattern);
                return regx.IsMatch(value);
            }

            #endregion Methods
        }

        ///    
        ///     <summary> * Information on where the book's text was obtained. </summary>
        ///     
        public class ConfigEntryType_TEXT_SOURCE : ConfigEntryType
        {
            #region Constructors

            public ConfigEntryType_TEXT_SOURCE()
                : base("TextSource")
            {
            }

            #endregion Constructors

            #region Methods

            public override bool allowsContinuation()
            {
                return true;
            }

            #endregion Methods
        }

        ///    
        ///     <summary> * An informational string indicating the current version of the book. </summary>
        ///     
        public class ConfigEntryType_VERSION : ConfigEntryType
        {
            #region Constructors

            public ConfigEntryType_VERSION()
                : base("Version", "1.0")
            {
            }

            #endregion Constructors

            #region Methods

            public override bool isAllowed(string aValue)
            {
                try
                {
                    Convert.ToSingle(aValue);
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }

            #endregion Methods
        }

        #endregion Nested Types
    }
}