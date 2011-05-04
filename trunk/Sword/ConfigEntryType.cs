///
/// <summary> Distribution License:
/// JSword is free software; you can redistribute it and/or modify it under
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
        ///    
        /// <summary>* Constants for direction </summary>
        ///     
        public const string DIRECTION_LTOR = "LtoR";
        public const string DIRECTION_RTOL = "RtoL";
        public const string DIRECTION_BIDI = "bidi";

        private static readonly string[] BLOCK_TYPE_PICKS = new string[] { "BOOK", "CHAPTER", "VERSE" };

        private static readonly string[] BOOLEAN_PICKS = new string[] { "true", "false" };

        private static readonly string[] CATEGORY_PICKS = new string[] { "Daily Devotional", "Glossaries", "Cults / Unorthodox / Questionable Material", "Essays", "Maps", "Images", "Biblical Texts", "Commentaries", "Lexicons / Dictionaries", "Generic Books" };

        private static readonly string[] COMPRESS_TYPE_PICKS = new string[] { "LZSS", "ZIP" };

        private static readonly string[] DIRECTION_PICKS = new string[] { DIRECTION_LTOR, DIRECTION_RTOL, DIRECTION_BIDI };

        private static readonly string[] KEY_TYPE_PICKS = new string[] { "TreeKey", "VerseKey" };

        private static readonly string[] FEATURE_PICKS = new string[] { "StrongsNumbers", "GreekDef", "HebrewDef", "GreekParse", "HebrewParse", "DailyDevotion", "Glossary", "Images" };

        private static readonly string[] GLOBAL_OPTION_FILTER_PICKS = new string[] { "GBFStrongs", "GBFFootnotes", "GBFScripref", "GBFMorph", "GBFHeadings", "GBFRedLetterWords", "ThMLStrongs", "ThMLFootnotes", "ThMLScripref", "ThMLMorph", "ThMLHeadings", "ThMLVariants", "ThMLLemma", "UTF8Cantillation", "UTF8GreekAccents", "UTF8HebrewPoints", "OSISStrongs", "OSISFootnotes", "OSISScripref", "OSISMorph", "OSISHeadings", "OSISRedLetterWords", "OSISLemma", "OSISRuby" };

        private static readonly string[] LICENSE_PICKS = new string[] { "Public Domain", "Copyrighted", "Copyrighted; Free non-commercial distribution", "Copyrighted; Permission to distribute granted to CrossWire", "Copyrighted; Freely distributable", "Copyrighted; Permission granted to distribute non-commercially in Sword format", "GFDL", "GPL", "Creative Commons: by-nc-nd", "Creative Commons: by-nc-sa", "Creative Commons: by-nc", "Creative Commons: by-nd", "Creative Commons: by-sa", "Creative Commons: by" };

        private static readonly string[] ENCODING_PICKS = new string[] { "Latin-1", "UTF-8" };

        private static readonly string[] MOD_DRV_PICKS = new string[] { "RawText", "zText", "RawCom", "RawCom4", "zCom", "HREFCom", "RawFiles", "RawLD", "RawLD4", "zLD", "RawGenBook" };

        private static readonly string[] SOURCE_TYPE_PICKS = new string[] { "Plaintext", "GBF", "ThML", "OSIS", "TEI", "OSIS", "TEI" };

        private static readonly string[] VERSIFICATION_PICKS = new string[] { "KJV", "KJVA", "NRSV", "NRSVA", "Leningrad", "MT" };

        ///    
        /// <summary>* A ConfigEntryPickType is a ConfigEntryType that allows values from a pick
        /// list. Matching is expected to be case-sensitive, but data problems
        /// dictate a more flexible approach.
        ///  </summary>
        ///     
        public class ConfigEntryPickType : ConfigEntryType
        {
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

            /*
             * (non-Javadoc)
             * 
             * @see org.crosswire.jsword.book.sword.ConfigEntryType#hasChoices()
             */
            protected internal override bool hasChoices()
            {
                return true;
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

            ///        
            ///         <summary> * The array of choices. </summary>
            ///         
            private readonly string[] choiceArray;

            ///        
            ///         <summary> * Serialization ID </summary>
            ///         
            private const long serialVersionUID = 5642668733730291463L;
        }

        ///    
        /// <summary>* Represents a ConfigEntryType that is not actually represented by the
        /// Sword Config file.
        ///  </summary>
        ///     
        public class ConfigEntrySyntheticType : ConfigEntryType
        {
            ///        
            ///         <summary> * Simple ctor </summary>
            ///         
            //JAVA TO VB & C# CONVERTER TODO TASK: C# doesn't allow accessing outer class instance members within a nested class:
            public ConfigEntrySyntheticType(string name)
                : base(name)
            {
            }

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

            ///        
            ///         <summary> * Serialization ID </summary>
            ///         
            private const long serialVersionUID = -2468890875139856087L;
        }

        ///    
        /// <summary>* The abbreviated name by which this book is known. This is in the [] on
        /// the first non-blank line of the conf. JSword uses this for display and
        /// access purposes. </summary>
        ///     
        public static readonly ConfigEntryType INITIALS = new ConfigEntrySyntheticType("Initials");

        ///    
        /// <summary>* Relative path to the data files, some issues with this </summary>
        ///     
        public class ConfigEntryType_DATA_PATH : ConfigEntryType
        {
            public ConfigEntryType_DATA_PATH()
                : base("DataPath")
            {
            }
            public override bool isAllowed(string value)
            {
                return true;
            }

            ///        
            ///         <summary> * Serialization ID </summary>
            ///         
            private const long serialVersionUID = 3546926870244309296L;
        };
        public static readonly ConfigEntryType_DATA_PATH DATA_PATH = new ConfigEntryType_DATA_PATH();

        ///    
        /// <summary>* The full name of this book </summary>
        ///     
        public static readonly ConfigEntryType DESCRIPTION = new ConfigEntryType("Description");

        ///    
        /// <summary>* This indicates how the book was stored. </summary>
        ///     
        public static readonly ConfigEntryType MOD_DRV = new ConfigEntryPickType("ModDrv", MOD_DRV_PICKS);

        ///    
        /// <summary>* The type of compression in use. JSword does not support LZSS. While it is
        /// the default, it is not used. At least so far. </summary>
        ///     
        public static readonly ConfigEntryType COMPRESS_TYPE = new ConfigEntryPickType("CompressType", COMPRESS_TYPE_PICKS, COMPRESS_TYPE_PICKS[0]);

        ///    
        /// <summary>* The level at which compression is applied, BOOK, CHAPTER, or VERSE </summary>
        ///     
        public static readonly ConfigEntryType BLOCK_TYPE = new ConfigEntryPickType("BlockType", BLOCK_TYPE_PICKS, BLOCK_TYPE_PICKS[0]);


        ///    
        ///     <summary> * single value integer, unknown use, some indications that we ought to be
        ///     * using it </summary>
        ///     
        public class ConfigEntryType_BLOCK_COUNT : ConfigEntryType
        {
            public ConfigEntryType_BLOCK_COUNT()
                : base("BlockCount", 200)
            {
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

            ///        
            ///         <summary> * Comment for <code>serialVersionUID</code> </summary>
            ///         
            private const long serialVersionUID = 3978711675019212341L;
        }

        ///    
        ///     <summary> * single value integer, unknown use, some indications that we ought to be
        ///     * using it </summary>
        ///     
        public static readonly ConfigEntryType_BLOCK_COUNT BLOCK_COUNT = new ConfigEntryType_BLOCK_COUNT();



        ///    
        /// <summary>* The kind of key that a Generic Book uses. </summary>
        ///     
        public static readonly ConfigEntryType KEY_TYPE = new ConfigEntryPickType("KeyType", KEY_TYPE_PICKS, KEY_TYPE_PICKS[0]);

        ///    
        /// <summary>* If this exists in the conf, then the book is encrypted. The value is used
        /// to unlock the book. The encryption algorithm is Sapphire. </summary>
        ///     
        public static readonly ConfigEntryType CIPHER_KEY = new ConfigEntryType("CipherKey");

        ///    
        /// <summary>* This indicates the versification of the book, with KJV being the default. </summary>
        ///     
        public static readonly ConfigEntryType VERSIFICATION = new ConfigEntryPickType("Versification", VERSIFICATION_PICKS);


        ///    
        ///     <summary> * Global Option Filters are the names of routines in Sword that can be used
        ///     * to display the data. These are not used by JSword. </summary>
        ///     
        public class ConfigEntryType_GLOBAL_OPTION_FILTER : ConfigEntryPickType
        {
            public ConfigEntryType_GLOBAL_OPTION_FILTER()
                : base("GlobalOptionFilter", GLOBAL_OPTION_FILTER_PICKS)
            {
            }

            /*
             * (non-Javadoc)
             * 
             * @see org.crosswire.jsword.book.sword.ConfigEntryType#mayRepeat()
             */
            public override bool mayRepeat()
            {
                return true;
            }

            ///        
            ///         <summary> * Serialization ID </summary>
            ///         
            private const long serialVersionUID = 3258417209599931960L;
        }

        ///    
        ///     <summary> * Global Option Filters are the names of routines in Sword that can be used
        ///     * to display the data. These are not used by JSword. </summary>
        ///     
        public static readonly ConfigEntryType_GLOBAL_OPTION_FILTER GLOBAL_OPTION_FILTER = new ConfigEntryType_GLOBAL_OPTION_FILTER();


        ///    
        /// <summary>* The layout direction of the text in the book. Hebrew, Arabic and Farsi
        /// RtoL. Most are 'LtoR'. Some are 'bidi', bi-directional. E.g.
        /// hebrew-english glossary. </summary>
        ///     
        public static readonly ConfigEntryType DIRECTION = new ConfigEntryPickType("Direction", DIRECTION_PICKS, DIRECTION_PICKS[0]);

        ///    
        /// <summary>* This indicates the kind of markup used for the book. </summary>
        ///     
        public static readonly ConfigEntryType SOURCE_TYPE = new ConfigEntryPickType("SourceType", SOURCE_TYPE_PICKS, SOURCE_TYPE_PICKS[0]);

        ///    
        /// <summary>* The character encoding. Only Latin-1 and UTF-8 are supported. </summary>
        ///     
        public static readonly ConfigEntryType ENCODING = new ConfigEntryPickType("Encoding", ENCODING_PICKS, ENCODING_PICKS[0]);

        ///    
        ///     <summary> * Display level is used by GenBooks to do auto expansion in the tree. A
        ///     * level of 2 indicates that the first two levels should be shown. </summary>
        ///     
        public class ConfigEntryType_DISPLAY_LEVEL : ConfigEntryType
        {
            public ConfigEntryType_DISPLAY_LEVEL()
                : base("DisplayLevel")
            {
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

            ///        
            ///         <summary> * Serialization ID </summary>
            ///         
            private const long serialVersionUID = 3979274654953451830L;
        }

        public static readonly ConfigEntryType_DISPLAY_LEVEL DISPLAY_LEVEL = new ConfigEntryType_DISPLAY_LEVEL();

        ///    
        /// <summary>* A recommended font to use for the book. </summary>
        ///     
        public static readonly ConfigEntryType FONT = new ConfigEntryType("Font");

        ///    
        ///     <summary> * When false do not show quotation marks for OSIS text that has <q>
        ///     * elements. </summary>
        ///     
        public class ConfigEntryType_OSIS_Q_TO_TICK : ConfigEntryPickType
        {
            public ConfigEntryType_OSIS_Q_TO_TICK()
                : base("OSISqToTick", BOOLEAN_PICKS, true)
            {
            }

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

            ///        
            ///         <summary> * Serialization ID </summary>
            ///         
            private const long serialVersionUID = 3258412850174373936L;
        }

        ///    
        ///     <summary> * When false do not show quotation marks for OSIS text that has <q>
        ///     * elements. </summary>
        ///     
        public static readonly ConfigEntryType_OSIS_Q_TO_TICK OSIS_Q_TO_TICK = new ConfigEntryType_OSIS_Q_TO_TICK();

        ///    
        ///     <summary> * A Feature describes a characteristic of the Book. </summary>
        ///     
        public class ConfigEntryType_FEATURE : ConfigEntryPickType
        {
            public ConfigEntryType_FEATURE()
                : base("Feature", FEATURE_PICKS)
            {
            }

            /*
             * (non-Javadoc)
             * 
             * @see org.crosswire.jsword.book.sword.ConfigEntryType#mayRepeat()
             */
            public override bool mayRepeat()
            {
                return true;
            }

            ///        
            ///         <summary> * Serialization ID </summary>
            ///         
            private const long serialVersionUID = 3833181424051172401L;
        }

        ///    
        ///     <summary> * A Feature describes a characteristic of the Book. </summary>
        ///     
        public static readonly ConfigEntryType_FEATURE FEATURE = new ConfigEntryType_FEATURE();

        ///    
        ///     <summary> * Books with a Feature of Glossary are used to map words FROM one language
        ///     * TO another. </summary>
        ///     
        public class ConfigEntryType_GLOSSARY_FROM : ConfigEntryType
        {
            public ConfigEntryType_GLOSSARY_FROM()
                : base("GlossaryFrom")
            {
            }

            ///        
            ///         <summary> * Serialization ID </summary>
            ///         
            private const long serialVersionUID = 6619179970516935818L;

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

        }
        ///    
        ///     <summary> * Books with a Feature of Glossary are used to map words FROM one language
        ///     * TO another. </summary>
        ///     
        public static readonly ConfigEntryType_GLOSSARY_FROM GLOSSARY_FROM = new ConfigEntryType_GLOSSARY_FROM();

        ///    
        ///     <summary> * Books with a Feature of Glossary are used to map words FROM one language
        ///     * TO another. </summary>
        ///     
        public class ConfigEntryType_GLOSSARY_TO : ConfigEntryType
        {
            public ConfigEntryType_GLOSSARY_TO()
                : base("GlossaryTo")
            {
            }

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

            ///        
            ///         <summary> * Serialization ID </summary>
            ///         
            private const long serialVersionUID = 3273532519245386866L;
        }

        ///    
        ///     <summary> * Books with a Feature of Glossary are used to map words FROM one language
        ///     * TO another. </summary>
        ///     
        public static readonly ConfigEntryType_GLOSSARY_TO GLOSSARY_TO = new ConfigEntryType_GLOSSARY_TO();


        ///    
        /// <summary>* The short name of this book. </summary>
        ///     
        public static readonly ConfigEntryType ABBREVIATION = new ConfigEntryType("Abbreviation");

        ///    
        ///     <summary> * Contains rtf that describes the book. </summary>
        ///     
        public class ConfigEntryType_ABOUT : ConfigEntryType
        {
            public ConfigEntryType_ABOUT()
                : base("About")
            {
            }

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

            ///        
            ///         <summary> * Serialization ID </summary>
            ///         
            private const long serialVersionUID = 3258416110121334073L;
        }
        ///    
        ///     <summary> * Contains rtf that describes the book. </summary>
        ///     
        public static readonly ConfigEntryType_ABOUT ABOUT = new ConfigEntryType_ABOUT();

        ///    
        ///     <summary> * An informational string indicating the current version of the book. </summary>
        ///     
        public class ConfigEntryType_VERSION : ConfigEntryType
        {
            public ConfigEntryType_VERSION()
                : base("Version", "1.0")
            {
            }

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

            ///        
            ///         <summary> * Serialization ID </summary>
            ///         
            private const long serialVersionUID = 3256443616242055221L;
        }
        ///    
        ///     <summary> * An informational string indicating the current version of the book. </summary>
        ///     
        public static readonly ConfigEntryType_VERSION VERSION = new ConfigEntryType_VERSION();

        ///    
        ///     <summary> * multiple values starting with History, some sort of change-log. In the
        ///     * conf these are of the form History_x.y. We strip off the x.y and prefix
        ///     * the value with it. The x.y corresponds to a current or prior Version
        ///     * value. </summary>
        ///     
        public class ConfigEntryType_HISTORY : ConfigEntryType
        {
            public ConfigEntryType_HISTORY()
                : base("History")
            {
            }

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

            ///        
            ///         <summary> * Serialization ID </summary>
            ///         
            private const long serialVersionUID = 3979272443195830835L;
        }
        ///    
        ///     <summary> * multiple values starting with History, some sort of change-log. In the
        ///     * conf these are of the form History_x.y. We strip off the x.y and prefix
        ///     * the value with it. The x.y corresponds to a current or prior Version
        ///     * value. </summary>
        ///     
        public static readonly ConfigEntryType_HISTORY HISTORY = new ConfigEntryType_HISTORY();

        ///    
        /// <summary>* single value version number, lowest sword c++ version that can read this
        /// book JSword does not use this value. </summary>
        ///     
        public static readonly ConfigEntryType MINIMUM_VERSION = new ConfigEntryType("MinimumVersion", "1.5.1a");


        ///    
        ///     <summary> * The Category of the book. Used on the web to classify books into a tree. </summary>
        ///     
        public class ConfigEntryType_CATEGORY : ConfigEntryType
        {
            public ConfigEntryType_CATEGORY()
                : base("Category"/*, CATEGORY_PICKS*/, BookCategory.OTHER)
            {
            }


            public override object convert(string input)
            {
                return BookCategory.fromString(input);
            }

            ///        
            ///         <summary> * Serialization ID </summary>
            ///         
            private const long serialVersionUID = 3258412850174571569L;
        }

        ///    
        ///     <summary> * The Category of the book. Used on the web to classify books into a tree. </summary>
        ///     
        public static readonly ConfigEntryType_CATEGORY CATEGORY = new ConfigEntryType_CATEGORY();

        ///    
        /// <summary>* Library of Congress Subject Heading. Typically this is of the form
        /// BookCategory Scope Language, where scope is typically O.T., N.T. </summary>
        ///     
        public static readonly ConfigEntryType LCSH = new ConfigEntryType("LCSH");

        ///    
        ///     <summary> * single value string, defaults to en, the language of the book </summary>
        ///     
        public class ConfigEntryType_LANG : ConfigEntryType
        {
            public ConfigEntryType_LANG()
                : base("Lang", new Language(null))
            {
            }


            public override object convert(string input)
            {
                return new Language(input);
            }

            ///        
            ///         <summary> * Serialization ID </summary>
            ///         
            private const long serialVersionUID = 3257008752317379897L;
        }

        ///    
        ///     <summary> * single value string, defaults to en, the language of the book </summary>
        ///     
        public static readonly ConfigEntryType_LANG LANG = new ConfigEntryType_LANG();

        ///    
        ///     <summary> * The installed size of the book in bytes. This is not the size of the zip
        ///     * that is downloaded. </summary>
        ///     
        public class ConfigEntryType_INSTALL_SIZE : ConfigEntryType
        {
            public ConfigEntryType_INSTALL_SIZE()
                : base("InstallSize")
            {
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

            ///        
            ///         <summary> * Serialization ID </summary>
            ///         
            private const long serialVersionUID = 3256720680388408370L;
        }

        ///    
        ///     <summary> * The installed size of the book in bytes. This is not the size of the zip
        ///     * that is downloaded. </summary>
        ///
        public static readonly ConfigEntryType_INSTALL_SIZE INSTALL_SIZE = new ConfigEntryType_INSTALL_SIZE();

        ///    
        ///     <summary> * The date that this version of the book was last updated. Informational
        ///     * only. </summary>
        ///     
        public class ConfigEntryType_SWORD_VERSION_DATE : ConfigEntryType
        {
            public ConfigEntryType_SWORD_VERSION_DATE()
                : base("SwordVersionDate")
            {
            }

            public override bool isAllowed(string value)
            {
                Regex regx = new Regex(validDatePattern); 
                return regx.IsMatch(value);
            }

            private string validDatePattern = "\\d{4}-\\d{2}-\\d{2}";

            ///        
            ///         <summary> * Serialization ID </summary>
            ///         
            private const long serialVersionUID = 3618697504682948150L;
        }

        ///    
        ///     <summary> * The date that this version of the book was last updated. Informational
        ///     * only. </summary>
        ///
        public static readonly ConfigEntryType_SWORD_VERSION_DATE SWORD_VERSION_DATE = new ConfigEntryType_SWORD_VERSION_DATE();

        ///    
        ///     <summary> * A list of prior "initials" for the current book.
        ///     * TODO(dms): when a user installs a book with an obsoletes that matches
        ///     * an installed book, offer the user the opportunity to delete the old book. </summary>
        ///     
        public class ConfigEntryType_OBSOLETES : ConfigEntryType
        {
            public ConfigEntryType_OBSOLETES()
                : base("Obsoletes")
            {
            }

            public override bool mayRepeat()
            {
                return true;
            }

            public override bool reportDetails()
            {
                return false;
            }

            ///        
            ///         <summary> * Serialization ID </summary>
            ///         
            private const long serialVersionUID = 3258412850157400372L;
        }

        ///    
        ///     <summary> * A list of prior "initials" for the current book.
        ///     * TODO(dms): when a user installs a book with an obsoletes that matches
        ///     * an installed book, offer the user the opportunity to delete the old book. </summary>
        ///     
        public static readonly ConfigEntryType_OBSOLETES OBSOLETES = new ConfigEntryType_OBSOLETES();

        ///    
        ///     <summary> * Informational copyright notice. </summary>
        ///     
        public class ConfigEntryType_COPYRIGHT : ConfigEntryType
        {
            public ConfigEntryType_COPYRIGHT()
                : base("Copyright")
            {
            }


            public override bool allowsContinuation()
            {
                return true;
            }

            ///        
            ///         <summary> * Serialization ID </summary>
            ///         
            private const long serialVersionUID = 3256441412957517110L;
        }

        ///    
        ///     <summary> * Informational copyright notice. </summary>
        ///     
        public static readonly ConfigEntryType_COPYRIGHT COPYRIGHT = new ConfigEntryType_COPYRIGHT();


        ///    
        /// <summary>* single value string, unknown use </summary>
        ///     
        public static readonly ConfigEntryType COPYRIGHT_HOLDER = new ConfigEntryType("CopyrightHolder");

        ///    
        ///     <summary> * Copyright info. Informational only. This is a year, a year range or a
        ///     * comma separated list of these. </summary>
        ///     
        public class ConfigEntryType_COPYRIGHT_DATE : ConfigEntryType
        {
            public ConfigEntryType_COPYRIGHT_DATE()
                : base("CopyrightDate")
            {
            }


            public override bool isAllowed(string value)
            {
                Regex regx = new Regex(validDatePattern);
                return regx.IsMatch(value);
            }

            private string validDatePattern = "\\d{4}(\\s*-\\s*\\d{4})?(\\s*,\\s*\\d{4}(\\s*-\\s*\\d{4})?)*";

            ///        
            ///         <summary> * Serialization ID </summary>
            ///         
            private const long serialVersionUID = 3258126977217935671L;
        }

        ///    
        ///     <summary> * Copyright info. Informational only. This is a year, a year range or a
        ///     * comma separated list of these. </summary>
        ///     
        public static readonly ConfigEntryType_COPYRIGHT_DATE COPYRIGHT_DATE = new ConfigEntryType_COPYRIGHT_DATE();

        ///    
        ///     <summary> * Copyright info. Informational only. </summary>
        ///     
        public class ConfigEntryType_COPYRIGHT_NOTES : ConfigEntryType
        {
            public ConfigEntryType_COPYRIGHT_NOTES()
                : base("CopyrightNotes")
            {
            }


            public override bool allowsContinuation()
            {
                return true;
            }

            public override bool allowsRTF()
            {
                return true;
            }

            ///        
            ///         <summary> * Serialization ID </summary>
            ///         
            private const long serialVersionUID = 3906926794258199608L;
        }

        ///    
        ///     <summary> * Copyright info. Informational only. </summary>
        ///     
        public static readonly ConfigEntryType_COPYRIGHT_NOTES COPYRIGHT_NOTES = new ConfigEntryType_COPYRIGHT_NOTES();

        ///    
        ///     <summary> * Copyright info. Informational only. </summary>
        ///     
        public class ConfigEntryType_COPYRIGHT_CONTACT_NAME : ConfigEntryType
        {
            public ConfigEntryType_COPYRIGHT_CONTACT_NAME()
                : base("CopyrightContactName")
            {
            }


            public override bool allowsContinuation()
            {
                return true;
            }

            public override bool allowsRTF()
            {
                return true;
            }

            ///        
            ///         <summary> * Serialization ID </summary>
            ///         
            private const long serialVersionUID = 3257001060181620787L;
        }

        ///    
        ///     <summary> * Copyright info. Informational only. </summary>
        ///
        public static readonly ConfigEntryType_COPYRIGHT_CONTACT_NAME COPYRIGHT_CONTACT_NAME = new ConfigEntryType_COPYRIGHT_CONTACT_NAME();

        ///    
        ///     <summary> * Copyright info. Informational only. </summary>
        ///     
        public class ConfigEntryType_COPYRIGHT_CONTACT_NOTES : ConfigEntryType
        {
            public ConfigEntryType_COPYRIGHT_CONTACT_NOTES()
                : base("CopyrightContactNotes")
            {
            }


            public override bool allowsContinuation()
            {
                return true;
            }

            public override bool allowsRTF()
            {
                return true;
            }

            ///        
            ///         <summary> * Serialization ID </summary>
            ///         
            private const long serialVersionUID = 3257001060181620787L;
        }

        ///    
        ///     <summary> * Copyright info. Informational only. </summary>
        ///
        public static readonly ConfigEntryType_COPYRIGHT_CONTACT_NOTES COPYRIGHT_CONTACT_NOTES = new ConfigEntryType_COPYRIGHT_CONTACT_NOTES();

        ///    
        ///     <summary> * Copyright info. Informational only. </summary>
        ///     
        public class ConfigEntryType_COPYRIGHT_CONTACT_ADDRESS : ConfigEntryType
        {
            public ConfigEntryType_COPYRIGHT_CONTACT_ADDRESS()
                : base("CopyrightContactAddress")
            {
            }

            public override bool allowsContinuation()
            {
                return true;
            }

            public override bool allowsRTF()
            {
                return true;
            }

            ///        
            ///         <summary> * Serialization ID </summary>
            ///         
            private const long serialVersionUID = 3256721784077365556L;
        }

        ///    
        ///     <summary> * Copyright info. Informational only. </summary>
        ///
        public static readonly ConfigEntryType_COPYRIGHT_CONTACT_ADDRESS COPYRIGHT_CONTACT_ADDRESS = new ConfigEntryType_COPYRIGHT_CONTACT_ADDRESS();


        ///    
        /// <summary>* Copyright info. Informational only. </summary>
        ///     
        public static readonly ConfigEntryType COPYRIGHT_CONTACT_EMAIL = new ConfigEntryType("CopyrightContactEmail");

        ///    
        /// <summary>* A one line promo statement, required by Lockman for NASB </summary>
        ///     
        public static readonly ConfigEntryType SHORT_PROMO = new ConfigEntryType("ShortPromo");

        ///    
        /// <summary>* A one line copyright statement, required by Lockman for NASB </summary>
        ///     
        public static readonly ConfigEntryType SHORT_COPYRIGHT = new ConfigEntryType("ShortCopyright");

        ///    
        /// <summary>* Copyright info. Informational only. </summary>
        ///     
        public static readonly ConfigEntryType DISTRIBUTION_LICENSE = new ConfigEntryPickType("DistributionLicense", LICENSE_PICKS, LICENSE_PICKS[0]);

        ///    
        ///     <summary> * Copyright info. Informational only. </summary>
        ///     
        public class ConfigEntryType_DISTRIBUTION_NOTES : ConfigEntryType
        {
            public ConfigEntryType_DISTRIBUTION_NOTES()
                : base("DistributionNotes")
            {
            }


            public override bool allowsContinuation()
            {
                return true;
            }

            ///        
            ///         <summary> * Serialization ID </summary>
            ///         
            private const long serialVersionUID = 3257005453916518196L;
        }

        ///    
        ///     <summary> * Copyright info. Informational only. </summary>
        ///
        public static readonly ConfigEntryType_DISTRIBUTION_NOTES DISTRIBUTION_NOTES = new ConfigEntryType_DISTRIBUTION_NOTES();

        ///    
        ///     <summary> * Information on where the book's text was obtained. </summary>
        ///     
        public class ConfigEntryType_TEXT_SOURCE : ConfigEntryType
        {
            public ConfigEntryType_TEXT_SOURCE()
                : base("TextSource")
            {
            }

            public override bool allowsContinuation()
            {
                return true;
            }

            ///        
            ///         <summary> * Serialization ID </summary>
            ///         
            private const long serialVersionUID = 3258126968594772272L;
        }

        ///    
        ///     <summary> * Information on where the book's text was obtained. </summary>
        ///
        public static readonly ConfigEntryType_TEXT_SOURCE TEXT_SOURCE = new ConfigEntryType_TEXT_SOURCE();

        ///    
        ///     <summary> * Similar to DataPath. It gives where on the CrossWire server the book can
        ///     * be found. Informational only. </summary>
        ///     
        public class ConfigEntryType_DISTRIBUTION_SOURCE : ConfigEntryType
        {
            public ConfigEntryType_DISTRIBUTION_SOURCE()
                : base("DistributionSource")
            {
            }

            public override bool allowsContinuation()
            {
                return true;
            }

            ///        
            ///         <summary> * Serialization ID </summary>
            ///         
            private const long serialVersionUID = 3763093051127904307L;
        }

        ///    
        ///     <summary> * Similar to DataPath. It gives where on the CrossWire server the book can
        ///     * be found. Informational only. </summary>
        ///
        public static readonly ConfigEntryType_DISTRIBUTION_SOURCE DISTRIBUTION_SOURCE = new ConfigEntryType_DISTRIBUTION_SOURCE();


        ///    
        /// <summary>* single value version number, lowest sword c++ version that can read this
        /// book JSword does not use this value. </summary>
        ///     
        public static readonly ConfigEntryType OSIS_VERSION = new ConfigEntryType("OSISVersion", "2.0");

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
        /// <summary>* RTF is allowed in a few config entries.
        ///  </summary>
        /// <returns> true if rtf is allowed </returns>
        ///     
        public virtual bool allowsRTF()
        {
            return false;
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
        /// <summary>* Lookup method to convert from an integer </summary>
        ///     
        public static ConfigEntryType fromInteger(int i)
        {
            return VALUES[i];
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
        /// <summary>* Prevent subclasses from overriding canonical identity based Object
        /// methods
        ///  </summary>
        /// <seealso cref= java.lang.Object#hashCode() </seealso>
        ///     
        public override int GetHashCode()
        {
            return base.GetHashCode();
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
        /// <summary>* The name of the ConfigEntryType </summary>
        ///     
        private string name;

        ///    
        /// <summary>* The default for the ConfigEntryType </summary>
        ///     
        private object defaultValue;

        ///    
        /// <summary>* Serialization ID </summary>
        ///     
        private const long serialVersionUID = 3258125873411273014L;

        // Support for serialization
        private static int nextObj;
        private readonly int obj = nextObj++;

        internal virtual object readResolve()
        {
            return VALUES[obj];
        }

        private static readonly ConfigEntryType[] VALUES = { INITIALS, DATA_PATH, DESCRIPTION, MOD_DRV, COMPRESS_TYPE, BLOCK_TYPE, BLOCK_COUNT, KEY_TYPE, CIPHER_KEY, VERSIFICATION, GLOBAL_OPTION_FILTER, DIRECTION, SOURCE_TYPE, ENCODING, DISPLAY_LEVEL, FONT, OSIS_Q_TO_TICK, FEATURE, GLOSSARY_FROM, GLOSSARY_TO, ABBREVIATION, ABOUT, VERSION, HISTORY, MINIMUM_VERSION, CATEGORY, LCSH, LANG, INSTALL_SIZE, SWORD_VERSION_DATE, OBSOLETES, COPYRIGHT, COPYRIGHT_HOLDER, COPYRIGHT_DATE, COPYRIGHT_NOTES, COPYRIGHT_CONTACT_NAME, COPYRIGHT_CONTACT_NOTES, COPYRIGHT_CONTACT_ADDRESS, COPYRIGHT_CONTACT_EMAIL, SHORT_PROMO, SHORT_COPYRIGHT, DISTRIBUTION_LICENSE, DISTRIBUTION_NOTES, TEXT_SOURCE, DISTRIBUTION_SOURCE, OSIS_VERSION, LIBRARY_URL, LOCATION_URL };
    }

}