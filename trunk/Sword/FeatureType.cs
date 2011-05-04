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
/// ID: $Id: FeatureType.java 2050 2010-12-09 15:31:45Z dmsmith $
/// 
/// Converted from Java to C# by Thomas Dilts with the help of a program from www.tangiblesoftwaresolutions.com
/// called 'Java to VB & C# Converter' on 2011-04-12 </summary>
/// 
using System.Diagnostics;
using System.Collections.Generic;
namespace SwordBackend
{
    ///
    /// <summary> * An Enumeration of the possible Features a Book may have.
    /// *  </summary>
    /// * <seealso cref= gnu.lgpl.License for license details.<br>
    /// *      The copyright to this program is held by it's authors.
    /// * @author Joe Walker [joe at eireneh dot com] </seealso>
    /// 
    public class FeatureType
    {
        ///    
        ///     <summary> * The book is one of Greek Definitions. AKA, Strong's. </summary>
        ///     
        public static readonly FeatureType GREEK_DEFINITIONS = new FeatureType("GreekDef");

        ///    
        ///     <summary> * The book is one of Greek word parsings. AKA, Robinson. </summary>
        ///     
        public static readonly FeatureType GREEK_PARSE = new FeatureType("GreekParse");

        ///    
        ///     <summary> * The book is one of Hebrew Definitions. AKA, Strong's. </summary>
        ///     
        public static readonly FeatureType HEBREW_DEFINITIONS = new FeatureType("HebrewDef");

        ///    
        ///     <summary> * The book is one of Hebrew word parsings. AKA, ???. </summary>
        ///     
        public static readonly FeatureType HEBREW_PARSE = new FeatureType("HebrewParse");

        ///    
        ///     <summary> * The book is one of Daily Devotions. </summary>
        ///     
        public static readonly FeatureType DAILY_DEVOTIONS = new FeatureType("DailyDevotions");

        ///    
        ///     <summary> * The book is glossary of translations from one language to another. </summary>
        ///     
        public static readonly FeatureType GLOSSARY = new FeatureType("Glossary");

        ///    
        ///     <summary> * The book contains Strong's Numbers </summary>
        ///     
        public static readonly FeatureType STRONGS_NUMBERS = new FeatureType("StrongsNumbers");

        ///    
        ///     <summary> * The book contains footnotes </summary>
        ///     
        public static readonly FeatureType FOOTNOTES = new FeatureType("Footnotes");

        ///    
        ///     <summary> * The book contains Scripture cross references </summary>
        ///     
        public static readonly FeatureType SCRIPTURE_REFERENCES = new FeatureType("Scripref");

        ///    
        ///     <summary> * The book marks the Word's of Christ </summary>
        ///     
        public static readonly FeatureType WORDS_OF_CHRIST = new FeatureType("RedLetterText");

        ///    
        ///     <summary> * The book contains Morphology info </summary>
        ///     
        public static readonly FeatureType MORPHOLOGY = new FeatureType("Morph");

        ///    
        ///     <summary> * The book contains Headings </summary>
        ///     
        public static readonly FeatureType HEADINGS = new FeatureType("Headings");

        ///    
        ///     * <param name="name">
        ///     *            The name of the FeatureType </param>
        ///    

        private static Dictionary<string, FeatureType> values = new Dictionary<string, FeatureType>()
        {
            {"GREEKDEF", GREEK_DEFINITIONS},
            {"GREEKPARSE", GREEK_PARSE},
            {"HEBREWDEF", HEBREW_DEFINITIONS},
            {"HEBREWPARSE", HEBREW_PARSE},
            {"DAILYDEVOTIONS", DAILY_DEVOTIONS},
            {"GLOSSARY", GLOSSARY},
            {"STRONGSNUMBERS", STRONGS_NUMBERS},
            {"FOOTNOTES", FOOTNOTES},
            {"SCRIPREF", SCRIPTURE_REFERENCES},
            {"REDLETTERTEXT", WORDS_OF_CHRIST},
            {"MORPH", MORPHOLOGY},
            {"HEADINGS", HEADINGS},
        };

        private FeatureType(string name)
        {
            this.name = name;
        }

        ///    
        ///     <summary> * Lookup method to convert from a String </summary>
        ///     
        public static FeatureType fromString(string name)
        {
            FeatureType featureType = null;
            values.TryGetValue(name.ToUpper(), out featureType);
            Debug.Assert(featureType != null);
            return featureType;
        }

        /* (non-Javadoc)
         * @see java.lang.Enum#toString()
         */
        public override string ToString()
        {
            return name;
        }

        ///    
        ///     <summary> * The name of the FeatureType </summary>
        ///     
        private string name;
    }
}