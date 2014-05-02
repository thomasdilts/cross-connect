#region Header

// <copyright file="ConfigEntryType.cs" company="Thomas Dilts">
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
    using System.Linq;
    using System.Text.RegularExpressions;

    /// <summary>
    ///     Constants for the keys in a Sword Config file. Taken from
    ///     http://sword.sourceforge.net/cgi-bin/twiki/view/Swordapi/ConfFileLayout
    ///     now located at
    ///     http://www.crosswire.org/ucgi-bin/twiki/view/Swordapi/ConfFileLayout
    ///     now located at http://www.crosswire.org/wiki/index.php/DevTools:Modules
    ///     now located at http://www.crosswire.org/wiki/DevTools:confFiles
    ///     This file is organized the same as the latest wiki documentation.
    /// </summary>
    /// The copyright to this program is held by it's authors.
    /// @author Joe Walker [joe at eireneh dot com]
    /// @author DM Smith [dmsmith555 at yahoo dot com]
    public class ConfigEntryType
    {
        #region Constants

        public const string DirectionBidi = "bidi";

        /// <summary>
        ///     * Constants for direction
        /// </summary>
        public const string DirectionLtor = "LtoR";

        public const string DirectionRtol = "RtoL";

        #endregion

        #region Static Fields

        public static readonly string[] ABlockTypePicks = new[] { "BOOK", "CHAPTER", "VERSE" };

        public static readonly string[] ACompressTypePicks = new[] { "LZSS", "ZIP" };

        public static readonly ConfigEntryTypeDataPath ADataPath = new ConfigEntryTypeDataPath();

        public static readonly string[] ADirectionPicks = new[] { DirectionLtor, DirectionRtol, DirectionBidi };

        public static readonly string[] AEncodingPicks = new[] { "Latin-1", "UTF-8" };

        public static readonly string[] AFeaturePicks = new[]
                                                            {
                                                                "StrongsNumbers", "GreekDef", "HebrewDef", "GreekParse",
                                                                "HebrewParse", "DailyDevotion", "Glossary", "Images"
                                                            };

        public static readonly string[] AGlobalOptionFilterPicks = new[]
                                                                       {
                                                                           "GBFStrongs", "GBFFootnotes", "GBFScripref",
                                                                           "GBFMorph", "GBFHeadings", "GBFRedLetterWords",
                                                                           "ThMLStrongs", "ThMLFootnotes", "ThMLScripref",
                                                                           "ThMLMorph", "ThMLHeadings", "ThMLVariants",
                                                                           "ThMLLemma", "UTF8Cantillation",
                                                                           "UTF8GreekAccents", "UTF8HebrewPoints",
                                                                           "OSISStrongs", "OSISFootnotes", "OSISScripref",
                                                                           "OSISMorph", "OSISHeadings",
                                                                           "OSISRedLetterWords", "OSISLemma", "OSISRuby"
                                                                       };

        public static readonly string[] AKeyTypePicks = new[] { "TreeKey", "VerseKey" };

        public static readonly string[] ALicensePicks = new[]
                                                            {
                                                                "Public Domain", "Copyrighted",
                                                                "Copyrighted; Free non-commercial distribution",
                                                                "Copyrighted; Permission to distribute granted to CrossWire"
                                                                , "Copyrighted; Freely distributable",
                                                                "Copyrighted; Permission granted to distribute non-commercially in Sword format"
                                                                , "GFDL", "GPL", "Creative Commons: by-nc-nd",
                                                                "Creative Commons: by-nc-sa", "Creative Commons: by-nc",
                                                                "Creative Commons: by-nd", "Creative Commons: by-sa",
                                                                "Creative Commons: by"
                                                            };

        public static readonly string[] AModDrvPicks = new[]
                                                           {
                                                               "RawText", "zText", "RawCom", "RawCom4", "zCom", "HREFCom",
                                                               "RawFiles", "RawLD", "RawLD4", "zLD", "RawGenBook"
                                                           };

        public static readonly string[] ASourceTypePicks = new[]
                                                               {
                                                                   "Plaintext", "GBF", "ThML", "OSIS", "TEI", "OSIS",
                                                                   "TEI"
                                                               };

        public static readonly string[] AVersificationPicks = new[] { "KJV", "Leningrad", "MT", "KJVA", "NRSV", "NRSVA", "Synodal", "SynodalProt", "Vulg", "German", "Luther", "Catholic", "Catholic2", "LXX", "Orthodox" };
 
        /// <summary>
        ///     * The short name of this book.
        /// </summary>
        public static readonly ConfigEntryType Abbreviation = new ConfigEntryType("Abbreviation");

        /// <summary>
        ///     * Contains rtf that describes the book.
        /// </summary>
        public static readonly ConfigEntryTypeAbout About = new ConfigEntryTypeAbout();

        /// <summary>
        ///     * single value integer, unknown use, some indications that we ought to be
        ///     * using it
        /// </summary>
        public static readonly ConfigEntryTypeBlockCount BlockCount = new ConfigEntryTypeBlockCount();

        /// <summary>
        ///     * The level at which compression is applied, BOOK, CHAPTER, or VERSE
        /// </summary>
        public static readonly ConfigEntryType BlockType = new ConfigEntryPickType(
            "BlockType", ABlockTypePicks, ABlockTypePicks[0]);

        public static readonly string[] BooleanPicks = new[] { "true", "false" };

        public static readonly string[] CategoryPicks = new[]
                                                            {
                                                                "Daily Devotional", "Glossaries",
                                                                "Cults / Unorthodox / Questionable Material", "Essays",
                                                                "Maps", "Images", "Biblical Texts", "Commentaries",
                                                                "Lexicons / Dictionaries", "Generic Books"
                                                            };

        /// <summary>
        ///     * If this exists in the conf, then the book is encrypted. The value is used
        ///     to unlock the book. The encryption algorithm is Sapphire.
        /// </summary>
        public static readonly ConfigEntryType CipherKey = new ConfigEntryType("CipherKey");

        /// <summary>
        ///     * The type of compression in use. JSword does not support LZSS. While it is
        ///     the default, it is not used. At least so far.
        /// </summary>
        public static readonly ConfigEntryType CompressType = new ConfigEntryPickType(
            "CompressType", ACompressTypePicks, ACompressTypePicks[0]);

        /// <summary>
        ///     * Informational copyright notice.
        /// </summary>
        public static readonly ConfigEntryTypeCopyright Copyright = new ConfigEntryTypeCopyright();

        /// <summary>
        ///     * Copyright info. Informational only.
        /// </summary>
        public static readonly ConfigEntryTypeCopyrightContactAddress CopyrightContactAddress =
            new ConfigEntryTypeCopyrightContactAddress();

        /// <summary>
        ///     * Copyright info. Informational only.
        /// </summary>
        public static readonly ConfigEntryType CopyrightContactEmail = new ConfigEntryType("CopyrightContactEmail");

        /// <summary>
        ///     * Copyright info. Informational only.
        /// </summary>
        public static readonly ConfigEntryTypeCopyrightContactName CopyrightContactName =
            new ConfigEntryTypeCopyrightContactName();

        /// <summary>
        ///     * Copyright info. Informational only.
        /// </summary>
        public static readonly ConfigEntryTypeCopyrightContactNotes CopyrightContactNotes =
            new ConfigEntryTypeCopyrightContactNotes();

        /// <summary>
        ///     * Copyright info. Informational only. This is a year, a year range or a
        ///     * comma separated list of these.
        /// </summary>
        public static readonly ConfigEntryTypeCopyrightDate CopyrightDate = new ConfigEntryTypeCopyrightDate();

        /// <summary>
        ///     * single value string, unknown use
        /// </summary>
        public static readonly ConfigEntryType CopyrightHolder = new ConfigEntryType("CopyrightHolder");

        /// <summary>
        ///     * Copyright info. Informational only.
        /// </summary>
        public static readonly ConfigEntryTypeCopyrightNotes CopyrightNotes = new ConfigEntryTypeCopyrightNotes();

        /// <summary>
        ///     * The full name of this book
        /// </summary>
        public static readonly ConfigEntryType Description = new ConfigEntryType("Description");

        /// <summary>
        ///     * The layout direction of the text in the book. Hebrew, Arabic and Farsi
        ///     RtoL. Most are 'LtoR'. Some are 'bidi', bi-directional. E.g.
        ///     hebrew-english glossary.
        /// </summary>
        public static readonly ConfigEntryType Direction = new ConfigEntryPickType(
            "Direction", ADirectionPicks, ADirectionPicks[0]);

        public static readonly ConfigEntryTypeDisplayLevel DisplayLevel = new ConfigEntryTypeDisplayLevel();

        /// <summary>
        ///     * Copyright info. Informational only.
        /// </summary>
        public static readonly ConfigEntryType DistributionLicense = new ConfigEntryPickType(
            "DistributionLicense", ALicensePicks, ALicensePicks[0]);

        /// <summary>
        ///     * Copyright info. Informational only.
        /// </summary>
        public static readonly ConfigEntryTypeDistributionNotes DistributionNotes =
            new ConfigEntryTypeDistributionNotes();

        /// <summary>
        ///     * Similar to DataPath. It gives where on the CrossWire server the book can
        ///     * be found. Informational only.
        /// </summary>
        public static readonly ConfigEntryTypeDistributionSource DistributionSource =
            new ConfigEntryTypeDistributionSource();

        /// <summary>
        ///     * The character encoding. Only Latin-1 and UTF-8 are supported.
        /// </summary>
        public static readonly ConfigEntryType Encoding = new ConfigEntryPickType(
            "Encoding", AEncodingPicks, AEncodingPicks[0]);

        /// <summary>
        ///     * A Feature describes a characteristic of the Book.
        /// </summary>
        public static readonly ConfigEntryTypeFeature Feature = new ConfigEntryTypeFeature();

        /// <summary>
        ///     * A recommended font to use for the book.
        /// </summary>
        public static readonly ConfigEntryType Font = new ConfigEntryType("Font");

        /// <summary>
        ///     * Global Option Filters are the names of routines in Sword that can be used
        ///     * to display the data. These are not used by JSword.
        /// </summary>
        public static readonly ConfigEntryTypeGlobalOptionFilter GlobalOptionFilter =
            new ConfigEntryTypeGlobalOptionFilter();

        /// <summary>
        ///     * Books with a Feature of Glossary are used to map words FROM one language
        ///     * TO another.
        /// </summary>
        public static readonly ConfigEntryTypeGlossaryFrom GlossaryFrom = new ConfigEntryTypeGlossaryFrom();

        /// <summary>
        ///     * Books with a Feature of Glossary are used to map words FROM one language
        ///     * TO another.
        /// </summary>
        public static readonly ConfigEntryTypeGlossaryTo GlossaryTo = new ConfigEntryTypeGlossaryTo();

        /// <summary>
        ///     * multiple values starting with History, some sort of change-log. In the
        ///     * conf these are of the form History_x.y. We strip off the x.y and prefix
        ///     * the value with it. The x.y corresponds to a current or prior Version
        ///     * value.
        /// </summary>
        public static readonly ConfigEntryTypeHistory History = new ConfigEntryTypeHistory();

        /// <summary>
        ///     * The abbreviated name by which this book is known. This is in the [] on
        ///     the first non-blank line of the conf. JSword uses this for display and
        ///     access purposes.
        /// </summary>
        public static readonly ConfigEntryType Initials = new ConfigEntrySyntheticType("Initials");

        /// <summary>
        ///     * The installed size of the book in bytes. This is not the size of the zip
        ///     * that is downloaded.
        /// </summary>
        public static readonly ConfigEntryTypeInstallSize InstallSize = new ConfigEntryTypeInstallSize();

        /// <summary>
        ///     * The kind of key that a Generic Book uses.
        /// </summary>
        public static readonly ConfigEntryType KeyType = new ConfigEntryPickType(
            "KeyType", AKeyTypePicks, AKeyTypePicks[0]);

        /// <summary>
        ///     * single value string, defaults to en, the language of the book
        /// </summary>
        public static readonly ConfigEntryTypeLang Lang = new ConfigEntryTypeLang();

        /// <summary>
        ///     * Library of Congress Subject Heading. Typically this is of the form
        ///     BookCategory Scope Language, where scope is typically O.T., N.T.
        /// </summary>
        public static readonly ConfigEntryType Lcsh = new ConfigEntryType("LCSH");

        /// <summary>
        ///     * The location of a collection of modules. JSword uses this to install and
        ///     delete a module.
        /// </summary>
        public static readonly ConfigEntryType LibraryUrl = new ConfigEntrySyntheticType("LibraryURL");

        /// <summary>
        ///     * The location of the module. JSword uses this to access a module.
        /// </summary>
        public static readonly ConfigEntryType LocationUrl = new ConfigEntrySyntheticType("LocationURL");

        /// <summary>
        ///     * single value version number, lowest sword c++ version that can read this
        ///     book JSword does not use this value.
        /// </summary>
        public static readonly ConfigEntryType MinimumVersion = new ConfigEntryType("MinimumVersion", "1.5.1a");

        /// <summary>
        ///     * This indicates how the book was stored.
        /// </summary>
        public static readonly ConfigEntryType ModDrv = new ConfigEntryPickType("ModDrv", AModDrvPicks);

        /// <summary>
        ///     * A list of prior "initials" for the current book.
        ///     *  when a user installs a book with an obsoletes that matches
        ///     * an installed book, offer the user the opportunity to delete the old book.
        /// </summary>
        public static readonly ConfigEntryTypeObsoletes Obsoletes = new ConfigEntryTypeObsoletes();

        /// <summary>
        ///     * When false do not show quotation marks for OSIS text that has
        ///     * elements.
        /// </summary>
        public static readonly ConfigEntryTypeOsisQToTick OsisQToTick = new ConfigEntryTypeOsisQToTick();

        /// <summary>
        ///     * single value version number, lowest sword c++ version that can read this
        ///     book JSword does not use this value.
        /// </summary>
        public static readonly ConfigEntryType OsisVersion = new ConfigEntryType("OSISVersion", "2.0");

        /// <summary>
        ///     * A one line copyright statement, required by Lockman for NASB
        /// </summary>
        public static readonly ConfigEntryType ShortCopyright = new ConfigEntryType("ShortCopyright");

        /// <summary>
        ///     * A one line promo statement, required by Lockman for NASB
        /// </summary>
        public static readonly ConfigEntryType ShortPromo = new ConfigEntryType("ShortPromo");

        /// <summary>
        ///     * This indicates the kind of markup used for the book.
        /// </summary>
        public static readonly ConfigEntryType SourceType = new ConfigEntryPickType(
            "SourceType", ASourceTypePicks, ASourceTypePicks[0]);

        /// <summary>
        ///     * The date that this version of the book was last updated. Informational
        ///     * only.
        /// </summary>
        public static readonly ConfigEntryTypeSwordVersionDate SwordVersionDate = new ConfigEntryTypeSwordVersionDate();

        /// <summary>
        ///     * Information on where the book's text was obtained.
        /// </summary>
        public static readonly ConfigEntryTypeTextSource TextSource = new ConfigEntryTypeTextSource();

        /// <summary>
        ///     * This indicates the versification of the book, with KJV being the default.
        /// </summary>
        public static readonly ConfigEntryType Versification = new ConfigEntryPickType(
            "Versification", AVersificationPicks);

        /// <summary>
        ///     * An informational string indicating the current version of the book.
        /// </summary>
        public static readonly ConfigEntryTypeVersion Version = new ConfigEntryTypeVersion();

        private static readonly ConfigEntryType[] Values =
            {
                Initials, ADataPath, Description, ModDrv, CompressType,
                BlockType, BlockCount, KeyType, CipherKey, Versification,
                GlobalOptionFilter, Direction, SourceType, Encoding,
                DisplayLevel, Font, OsisQToTick, Feature, GlossaryFrom,
                GlossaryTo, Abbreviation, About, Version, History,
                MinimumVersion, Lcsh, Lang, InstallSize, SwordVersionDate,
                Obsoletes, Copyright, CopyrightHolder, CopyrightDate,
                CopyrightNotes, CopyrightContactName, CopyrightContactNotes
                , CopyrightContactAddress, CopyrightContactEmail,
                ShortPromo, ShortCopyright, DistributionLicense,
                DistributionNotes, TextSource, DistributionSource,
                OsisVersion, LibraryUrl, LocationUrl
            };

        private static int _nextObj;

        #endregion

        #region Fields

        /// <summary>
        ///     * The default for the ConfigEntryType
        /// </summary>
        private readonly object _defaultValue;

        /// <summary>
        ///     * The name of the ConfigEntryType
        /// </summary>
        private readonly string _name;

        private readonly int _obj = _nextObj++;

        #endregion

        // Support for serialization

        #region Constructors and Destructors

        /// <summary>
        ///     * Simple ctor
        /// </summary>
        protected internal ConfigEntryType(string name)
            : this(name, null)
        {
        }

        /// <summary>
        ///     * Simple ctor
        /// </summary>
        protected internal ConfigEntryType(string name, object defaultValue)
        {
            this._name = name;
            this._defaultValue = defaultValue;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     * Some ConfigEntryTypes have defaults.
        /// </summary>
        /// <returns> the default, if there is one, null otherwise </returns>
        public virtual object Default
        {
            get
            {
                return this._defaultValue;
            }
        }

        /// <summary>
        ///     * Synthetic keys are those that are not in the Sword Book's conf, but are
        ///     needed by the program. Typically, these are derived by the program from
        ///     the other entries.
        /// </summary>
        /// <returns> true if this is synthetic </returns>
        public virtual bool IsSynthetic
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        ///     * Returns the normalized name of this ConfigEntry.
        /// </summary>
        /// <returns> the name </returns>
        public virtual string Name
        {
            get
            {
                return this._name;
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     * Lookup method to convert from an integer
        /// </summary>
        public static ConfigEntryType FromInteger(int i)
        {
            return Values[i];
        }

        /// <summary>
        ///     * Lookup method to convert from a string
        /// </summary>
        public static ConfigEntryType FromString(string name)
        {
            if (name != null)
            {
                // special case
                if (name.StartsWith(History.ToString()))
                {
                    return History;
                }

                return Values.FirstOrDefault(o => name.Equals(o._name));
            }

            // should not get here.
            // But there are typos in the keys in the book conf files
            // And this allows for the addition of new fields in
            // advance of changing JSword
            return null;
        }

        /// <summary>
        ///     * While most fields are single line or single value, some allow
        ///     continuation. A continuation mark is a backslash at the end of a line. It
        ///     is not to be followed by whitespace.
        /// </summary>
        /// <returns> true if continuation is allowed </returns>
        public virtual bool AllowsContinuation()
        {
            return false;
        }

        /// <summary>
        ///     * RTF is allowed in a few config entries.
        /// </summary>
        /// <returns> true if rtf is allowed </returns>
        public virtual bool AllowsRtf()
        {
            return false;
        }

        /// <summary>
        ///     * Convert the string value from the conf into the representation of this
        ///     ConfigEntryType.
        /// </summary>
        /// <returns> the converted object </returns>
        public virtual object Convert(string input)
        {
            return input;
        }

        /// <summary>
        ///     * Modify the value if necessary.
        /// </summary>
        /// <param name="value">
        ///     the input
        /// </param>
        /// <returns> either value or a modified version of it. </returns>
        public virtual string Filter(string value)
        {
            return value;
        }

        /// <summary>
        ///     * Determines whether the string is allowed. For some config entries, the
        ///     value is expected to be one of a group, for others the format is defined.
        /// </summary>
        /// <param name="value">
        ///     the string to be checked
        /// </param>
        /// <returns> true if the string is allowed </returns>
        public virtual bool IsAllowed(string value)
        {
            return value != null;
        }

        /// <summary>
        ///     * Some keys can repeat. When this happens each is a single value pick from
        ///     a list of choices.
        /// </summary>
        /// <returns> true if this ConfigEntryType can occur more than once </returns>
        public virtual bool MayRepeat()
        {
            return false;
        }

        /// <summary>
        ///     * Determines the level of detail stored in the histogram.
        /// </summary>
        /// <returns> true if the ConfigEntry should report histogram for repetitions </returns>
        public virtual bool ReportDetails()
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
            return this._name;
        }

        #endregion

        #region Methods

        internal virtual object ReadResolve()
        {
            return Values[this._obj];
        }

        /// <summary>
        ///     * Some keys can repeat. When this happens each is a single value pick from
        ///     a list of choices.
        /// </summary>
        /// <returns> true if this ConfigEntryType can occur more than once </returns>
        protected internal virtual bool HasChoices()
        {
            return false;
        }

        #endregion

        /// <summary>
        ///     * A ConfigEntryPickType is a ConfigEntryType that allows values from a pick
        ///     list. Matching is expected to be case-sensitive, but data problems
        ///     dictate a more flexible approach.
        /// </summary>
        public class ConfigEntryPickType : ConfigEntryType
        {
            #region Fields

            /// <summary>
            ///     * The array of choices.
            /// </summary>
            private readonly string[] _choiceArray;

            #endregion

            #region Constructors and Destructors

            /// <summary>
            ///     * Simple ctor
            /// </summary>
            public ConfigEntryPickType(string name, string[] picks)
                : this(name, picks, null)
            {
            }

            /// <summary>
            ///     * Simple ctor
            /// </summary>
            public ConfigEntryPickType(string name, string[] picks, object defaultPick)
                : base(name, defaultPick)
            {
                this._choiceArray = (string[])picks.Clone();
            }

            #endregion

            /*
             * (non-Javadoc)
             *
             * @see
             * org.crosswire.jsword.book.sword.ConfigEntryType#filter(java.lang.
             * string)
             */

            #region Public Methods and Operators

            public override string Filter(string value)
            {
                // Do we have an exact match?
                if (this._choiceArray.Any(t => t.Equals(value)))
                {
                    return value;
                }

                // Do we have a case insensitive match?
                foreach (string t in this._choiceArray.Where(t => t.ToUpper() == value.ToUpper()))
                {
                    return t;
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

            public override bool IsAllowed(string value)
            {
                return this._choiceArray.Any(t => t.ToUpper() == value.ToUpper());
            }

            #endregion

            /*
             * (non-Javadoc)
             *
             * @see org.crosswire.jsword.book.sword.ConfigEntryType#hasChoices()
             */

            #region Methods

            protected internal override bool HasChoices()
            {
                return true;
            }

            #endregion
        }

        /// <summary>
        ///     * Represents a ConfigEntryType that is not actually represented by the
        ///     Sword Config file.
        /// </summary>
        public class ConfigEntrySyntheticType : ConfigEntryType
        {
            #region Constructors and Destructors

            /// <summary>
            ///     * Simple ctor
            /// </summary>
            //  C# doesn't allow accessing outer class instance members within a nested class:
            public ConfigEntrySyntheticType(string name)
                : base(name)
            {
            }

            #endregion

            /*
             * (non-Javadoc)
             *
             * @see org.crosswire.jsword.book.sword.ConfigEntryType#isSynthetic()
             */

            #region Public Properties

            public override bool IsSynthetic
            {
                get
                {
                    return true;
                }
            }

            #endregion
        }

        /// <summary>
        ///     * Contains rtf that describes the book.
        /// </summary>
        public class ConfigEntryTypeAbout : ConfigEntryType
        {
            #region Constructors and Destructors

            public ConfigEntryTypeAbout()
                : base("About")
            {
            }

            #endregion

            /*
             * (non-Javadoc)
             *
             * @see
             * org.crosswire.jsword.book.sword.ConfigEntryType#allowsContinuation()
             */

            #region Public Methods and Operators

            public override bool AllowsContinuation()
            {
                return true;
            }

            /*
             * (non-Javadoc)
             *
             * @see org.crosswire.jsword.book.sword.ConfigEntryType#allowsRTF()
             */

            public override bool AllowsRtf()
            {
                return true;
            }

            #endregion
        }

        /// <summary>
        ///     * single value integer, unknown use, some indications that we ought to be
        ///     * using it
        /// </summary>
        public class ConfigEntryTypeBlockCount : ConfigEntryType
        {
            #region Constructors and Destructors

            public ConfigEntryTypeBlockCount()
                : base("BlockCount", 200)
            {
            }

            #endregion

            /*
             * (non-Javadoc)
             *
             * @see
             * org.crosswire.jsword.book.sword.ConfigEntryType#convert(java.lang
             * .String)
             */

            #region Public Methods and Operators

            public override object Convert(string input)
            {
                try
                {
                    return System.Convert.ToInt32(input);
                }
                catch (Exception)
                {
                    return this.Default;
                }
            }

            #endregion
        }

        /// <summary>
        ///     * Informational copyright notice.
        /// </summary>
        public class ConfigEntryTypeCopyright : ConfigEntryType
        {
            #region Constructors and Destructors

            public ConfigEntryTypeCopyright()
                : base("Copyright")
            {
            }

            #endregion

            #region Public Methods and Operators

            public override bool AllowsContinuation()
            {
                return true;
            }

            #endregion
        }

        /// <summary>
        ///     * Copyright info. Informational only.
        /// </summary>
        public class ConfigEntryTypeCopyrightContactAddress : ConfigEntryType
        {
            #region Constructors and Destructors

            public ConfigEntryTypeCopyrightContactAddress()
                : base("CopyrightContactAddress")
            {
            }

            #endregion

            #region Public Methods and Operators

            public override bool AllowsContinuation()
            {
                return true;
            }

            public override bool AllowsRtf()
            {
                return true;
            }

            #endregion
        }

        /// <summary>
        ///     * Copyright info. Informational only.
        /// </summary>
        public class ConfigEntryTypeCopyrightContactName : ConfigEntryType
        {
            #region Constructors and Destructors

            public ConfigEntryTypeCopyrightContactName()
                : base("CopyrightContactName")
            {
            }

            #endregion

            #region Public Methods and Operators

            public override bool AllowsContinuation()
            {
                return true;
            }

            public override bool AllowsRtf()
            {
                return true;
            }

            #endregion
        }

        /// <summary>
        ///     * Copyright info. Informational only.
        /// </summary>
        public class ConfigEntryTypeCopyrightContactNotes : ConfigEntryType
        {
            #region Constructors and Destructors

            public ConfigEntryTypeCopyrightContactNotes()
                : base("CopyrightContactNotes")
            {
            }

            #endregion

            #region Public Methods and Operators

            public override bool AllowsContinuation()
            {
                return true;
            }

            public override bool AllowsRtf()
            {
                return true;
            }

            #endregion
        }

        /// <summary>
        ///     * Copyright info. Informational only. This is a year, a year range or a
        ///     * comma separated list of these.
        /// </summary>
        public class ConfigEntryTypeCopyrightDate : ConfigEntryType
        {
            #region Constants

            private const string ValidDatePattern = "\\d{4}(\\s*-\\s*\\d{4})?(\\s*,\\s*\\d{4}(\\s*-\\s*\\d{4})?)*";

            #endregion

            #region Constructors and Destructors

            public ConfigEntryTypeCopyrightDate()
                : base("CopyrightDate")
            {
            }

            #endregion

            #region Public Methods and Operators

            public override bool IsAllowed(string value)
            {
                var regx = new Regex(ValidDatePattern);
                return regx.IsMatch(value);
            }

            #endregion
        }

        /// <summary>
        ///     * Copyright info. Informational only.
        /// </summary>
        public class ConfigEntryTypeCopyrightNotes : ConfigEntryType
        {
            #region Constructors and Destructors

            public ConfigEntryTypeCopyrightNotes()
                : base("CopyrightNotes")
            {
            }

            #endregion

            #region Public Methods and Operators

            public override bool AllowsContinuation()
            {
                return true;
            }

            public override bool AllowsRtf()
            {
                return true;
            }

            #endregion
        }

        /// <summary>
        ///     * Relative path to the data files, some issues with this
        /// </summary>
        public class ConfigEntryTypeDataPath : ConfigEntryType
        {
            #region Constructors and Destructors

            public ConfigEntryTypeDataPath()
                : base("DataPath")
            {
            }

            #endregion

            #region Public Methods and Operators

            public override bool IsAllowed(string value)
            {
                return true;
            }

            #endregion
        }

        /// <summary>
        ///     * Display level is used by GenBooks to do auto expansion in the tree. A
        ///     * level of 2 indicates that the first two levels should be shown.
        /// </summary>
        public class ConfigEntryTypeDisplayLevel : ConfigEntryType
        {
            #region Constructors and Destructors

            public ConfigEntryTypeDisplayLevel()
                : base("DisplayLevel")
            {
            }

            #endregion

            /*
             * (non-Javadoc)
             *
             * @see
             * org.crosswire.jsword.book.sword.ConfigEntryType#convert(java.lang
             * .String)
             */

            #region Public Methods and Operators

            public override object Convert(string input)
            {
                try
                {
                    return System.Convert.ToInt32(input);
                }
                catch (Exception)
                {
                    return null;
                }
            }

            #endregion
        }

        /// <summary>
        ///     * Copyright info. Informational only.
        /// </summary>
        public class ConfigEntryTypeDistributionNotes : ConfigEntryType
        {
            #region Constructors and Destructors

            public ConfigEntryTypeDistributionNotes()
                : base("DistributionNotes")
            {
            }

            #endregion

            #region Public Methods and Operators

            public override bool AllowsContinuation()
            {
                return true;
            }

            #endregion
        }

        /// <summary>
        ///     * Similar to DataPath. It gives where on the CrossWire server the book can
        ///     * be found. Informational only.
        /// </summary>
        public class ConfigEntryTypeDistributionSource : ConfigEntryType
        {
            #region Constructors and Destructors

            public ConfigEntryTypeDistributionSource()
                : base("DistributionSource")
            {
            }

            #endregion

            #region Public Methods and Operators

            public override bool AllowsContinuation()
            {
                return true;
            }

            #endregion
        }

        /// <summary>
        ///     * A Feature describes a characteristic of the Book.
        /// </summary>
        public class ConfigEntryTypeFeature : ConfigEntryPickType
        {
            #region Constructors and Destructors

            public ConfigEntryTypeFeature()
                : base("Feature", AFeaturePicks)
            {
            }

            #endregion

            /*
             * (non-Javadoc)
             *
             * @see org.crosswire.jsword.book.sword.ConfigEntryType#mayRepeat()
             */

            #region Public Methods and Operators

            public override bool MayRepeat()
            {
                return true;
            }

            #endregion
        }

        /// <summary>
        ///     * Global Option Filters are the names of routines in Sword that can be used
        ///     * to display the data. These are not used by JSword.
        /// </summary>
        public class ConfigEntryTypeGlobalOptionFilter : ConfigEntryPickType
        {
            #region Constructors and Destructors

            public ConfigEntryTypeGlobalOptionFilter()
                : base("GlobalOptionFilter", AGlobalOptionFilterPicks)
            {
            }

            #endregion

            /*
             * (non-Javadoc)
             *
             * @see org.crosswire.jsword.book.sword.ConfigEntryType#mayRepeat()
             */

            #region Public Methods and Operators

            public override bool MayRepeat()
            {
                return true;
            }

            #endregion
        }

        /// <summary>
        ///     * Books with a Feature of Glossary are used to map words FROM one language
        ///     * TO another.
        /// </summary>
        public class ConfigEntryTypeGlossaryFrom : ConfigEntryType
        {
            #region Constructors and Destructors

            public ConfigEntryTypeGlossaryFrom()
                : base("GlossaryFrom")
            {
            }

            #endregion

            /*
             * (non-Javadoc)
             *
             * @see
             * org.crosswire.jsword.book.sword.ConfigEntryType#convert(java.lang
             * .String)
             */

            #region Public Methods and Operators

            public override object Convert(string input)
            {
                return new Language(input);
            }

            #endregion
        }

        /// <summary>
        ///     * Books with a Feature of Glossary are used to map words FROM one language
        ///     * TO another.
        /// </summary>
        public class ConfigEntryTypeGlossaryTo : ConfigEntryType
        {
            #region Constructors and Destructors

            public ConfigEntryTypeGlossaryTo()
                : base("GlossaryTo")
            {
            }

            #endregion

            /*
             * (non-Javadoc)
             *
             * @see
             * org.crosswire.jsword.book.sword.ConfigEntryType#convert(java.lang
             * .String)
             */

            #region Public Methods and Operators

            public override object Convert(string input)
            {
                return new Language(input);
            }

            #endregion
        }

        /// <summary>
        ///     * multiple values starting with History, some sort of change-log. In the
        ///     * conf these are of the form History_x.y. We strip off the x.y and prefix
        ///     * the value with it. The x.y corresponds to a current or prior Version
        ///     * value.
        /// </summary>
        public class ConfigEntryTypeHistory : ConfigEntryType
        {
            #region Constructors and Destructors

            public ConfigEntryTypeHistory()
                : base("History")
            {
            }

            #endregion

            /*
             * (non-Javadoc)
             *
             * @see org.crosswire.jsword.book.sword.ConfigEntryType#mayRepeat()
             */

            #region Public Methods and Operators

            public override bool MayRepeat()
            {
                return true;
            }

            /*
             * (non-Javadoc)
             *
             * @see org.crosswire.jsword.book.sword.ConfigEntryType#reportDetails()
             */

            public override bool ReportDetails()
            {
                return false;
            }

            #endregion
        }

        /// <summary>
        ///     * The installed size of the book in bytes. This is not the size of the zip
        ///     * that is downloaded.
        /// </summary>
        public class ConfigEntryTypeInstallSize : ConfigEntryType
        {
            #region Constructors and Destructors

            public ConfigEntryTypeInstallSize()
                : base("InstallSize")
            {
            }

            #endregion

            #region Public Methods and Operators

            public override object Convert(string input)
            {
                try
                {
                    return System.Convert.ToInt32(input);
                }
                catch (Exception)
                {
                    return null;
                }
            }

            #endregion
        }

        /// <summary>
        ///     * single value string, defaults to en, the language of the book
        /// </summary>
        public class ConfigEntryTypeLang : ConfigEntryType
        {
            #region Constructors and Destructors

            public ConfigEntryTypeLang()
                : base("Lang", new Language(null))
            {
            }

            #endregion

            #region Public Methods and Operators

            public override object Convert(string input)
            {
                return new Language(input);
            }

            #endregion
        }

        /// <summary>
        ///     * A list of prior "initials" for the current book.
        ///     *  when a user installs a book with an obsoletes that matches
        ///     * an installed book, offer the user the opportunity to delete the old book.
        /// </summary>
        public class ConfigEntryTypeObsoletes : ConfigEntryType
        {
            #region Constructors and Destructors

            public ConfigEntryTypeObsoletes()
                : base("Obsoletes")
            {
            }

            #endregion

            #region Public Methods and Operators

            public override bool MayRepeat()
            {
                return true;
            }

            public override bool ReportDetails()
            {
                return false;
            }

            #endregion
        }

        /// <summary>
        ///     * When false do not show quotation marks for OSIS text that has
        ///     * elements.
        /// </summary>
        public class ConfigEntryTypeOsisQToTick : ConfigEntryPickType
        {
            #region Constructors and Destructors

            public ConfigEntryTypeOsisQToTick()
                : base("OSISqToTick", BooleanPicks, true)
            {
            }

            #endregion

            /*
             * (non-Javadoc)
             *
             * @see
             * org.crosswire.jsword.book.sword.ConfigEntryType#convert(java.lang
             * .String)
             */

            #region Public Methods and Operators

            public override object Convert(string input)
            {
                return System.Convert.ToBoolean(input);
            }

            #endregion
        }

        /// <summary>
        ///     * The date that this version of the book was last updated. Informational
        ///     * only.
        /// </summary>
        public class ConfigEntryTypeSwordVersionDate : ConfigEntryType
        {
            #region Constants

            private const string ValidDatePattern = "\\d{4}-\\d{2}-\\d{2}";

            #endregion

            #region Constructors and Destructors

            public ConfigEntryTypeSwordVersionDate()
                : base("SwordVersionDate")
            {
            }

            #endregion

            #region Public Methods and Operators

            public override bool IsAllowed(string value)
            {
                var regx = new Regex(ValidDatePattern);
                return regx.IsMatch(value);
            }

            #endregion
        }

        /// <summary>
        ///     * Information on where the book's text was obtained.
        /// </summary>
        public class ConfigEntryTypeTextSource : ConfigEntryType
        {
            #region Constructors and Destructors

            public ConfigEntryTypeTextSource()
                : base("TextSource")
            {
            }

            #endregion

            #region Public Methods and Operators

            public override bool AllowsContinuation()
            {
                return true;
            }

            #endregion
        }

        /// <summary>
        ///     * An informational string indicating the current version of the book.
        /// </summary>
        public class ConfigEntryTypeVersion : ConfigEntryType
        {
            #region Constructors and Destructors

            public ConfigEntryTypeVersion()
                : base("Version", "1.0")
            {
            }

            #endregion
        }
    }
}