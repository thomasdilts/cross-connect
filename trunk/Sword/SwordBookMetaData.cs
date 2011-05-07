using System.Collections.Generic;
using System.Text;
using System;
using System.IO;

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
/// ID: $Id: SwordBookMetaData.java 2054 2010-12-10 22:12:09Z dmsmith $
/// 
/// Converted from Java to C# by Thomas Dilts with the help of a program from www.tangiblesoftwaresolutions.com
/// called 'Java to VB & C# Converter' on 2011-04-12 </summary>
/// 
namespace SwordBackend
{

    /*
	using Language = org.crosswire.common.util.Language;
	using NetUtil = org.crosswire.common.util.NetUtil;
	using PropertyMap = org.crosswire.common.util.PropertyMap;
	using BookCategory = org.crosswire.jsword.book.BookCategory;
	using FeatureType = org.crosswire.jsword.book.FeatureType;
	using KeyType = org.crosswire.jsword.book.KeyType;
	using AbstractBookMetaData = org.crosswire.jsword.book.basic.AbstractBookMetaData;
	using Filter = org.crosswire.jsword.book.filter.Filter;
	using FilterFactory = org.crosswire.jsword.book.filter.FilterFactory;
	using Document = System.Xml.XmlDocument;
    */
	///
	/// <summary> A utility class for loading and representing Sword book configs.
	/// 
	/// <p>
	/// Config file format. See also: <a href=
	/// "http://sword.sourceforge.net/cgi-bin/twiki/view/Swordapi/ConfFileLayout">
	/// http://sword.sourceforge.net/cgi-bin/twiki/view/Swordapi/ConfFileLayout</a>
	/// 
	/// <p>
	/// The contents of the About field are in rtf.
	/// <p>
	/// \ is used as a continuation line.
	///  </summary>
	/// <seealso cref= gnu.lgpl.License for license details.<br>
	///      The copyright to this program is held by it's authors.
	/// @author Mark Goodwin [mark at thorubio dot org]
	/// @author Joe Walker [joe at eireneh dot com]
	/// @author Jacky Cheung
	/// @author DM Smith [dmsmith555 at yahoo dot com] </seealso>
	/// 
	public class SwordBookMetaData
	{

	///    
	/// <summary>* Loads a sword config from a buffer.
	///  </summary>
	/// <param name="buffer"> </param>
	/// <param name="internal"> </param>
	/// <exception cref="IOException"> </exception>
	///     
        public SwordBookMetaData(byte[] buffer, string bookName)
        {
            cet = new ConfigEntryTable(bookName);
            cet.load(buffer);
            buildProperties();
        }
        public SwordBookMetaData(Stream stream, string bookName)
        {
            cet = new ConfigEntryTable(bookName);
            cet.load(stream);
            buildProperties();
        }
        public bool isLoaded = false;
		/*
		 * (non-Javadoc)
		 * 
		 * @see org.crosswire.jsword.book.BookMetaData#isQuestionable()
		 */
		public  bool isQuestionable
		{
			get
			{
				return cet.isQuestionable;
			}
		}

		/*
		 * (non-Javadoc)
		 * 
		 * @see org.crosswire.jsword.book.BookMetaData#isEnciphered()
		 */
		public  bool isEnciphered
		{
			get
			{
				return cet.isEnciphered;
			}
		}

		/*
		 * (non-Javadoc)
		 * 
		 * @see org.crosswire.jsword.book.BookMetaData#isLocked()
		 */
		public  bool isLocked
		{
			get
			{
				return cet.isLocked;
			}
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
		public  string UnlockKey
		{
			get
			{
				return cet.UnlockKey;
			}
		}

		/*
		 * (non-Javadoc)
		 * 
		 * @see org.crosswire.jsword.book.BookMetaData#getName()
		 */
		public string Name
		{
			get
			{
				return (string) getProperty(ConfigEntryType.DESCRIPTION);
			}
		}


	///    
	/// <summary>* Returns the sourceType. </summary>
	///     
		/*public Filter Filter
		{
			get
			{
				string sourcetype = (string) getProperty(ConfigEntryType.SOURCE_TYPE);
				return FilterFactory.getFilter(sourcetype);
			}
		}*/

	///    
	/// <returns> Returns the relative path of the book's conf. </returns>
	///     
		public string ConfPath
		{
			get
			{
				return BibleZtextReader.DIR_CONF + '/' + cet.internalName.ToLower() + BibleZtextReader.EXTENSION_CONF;
			}
		}
        public string internalName
        {
            get
            {
                return cet.internalName;
            }
        }

		public string Initials
		{
			get
			{
				return (string) getProperty(ConfigEntryType.INITIALS);
			}
		}

	///    
	/// <summary>* Get the string value for the property or null if it is not defined. It is
	/// assumed that all properties gotten with this method are single line.
	///  </summary>
	/// <param name="entry">
	///            the ConfigEntryType </param>
	/// <returns> the property or null </returns>
	///     
		public object getProperty(ConfigEntryType entry)
		{
			return cet.getValue(entry);
		}

		/*
		 * (non-Javadoc)
		 * 
		 * @see org.crosswire.jsword.book.BookMetaData#isLeftToRight()
		 */
		public bool isLeftToRight
		{
			get
			{
				// This should return the dominate direction of the text, if it is BiDi,
				// then we have to guess.
				string dir = (string) getProperty(ConfigEntryType.DIRECTION);
				if (ConfigEntryType.DIRECTION_BIDI.Equals(dir))
				{
					// When BiDi, return the dominate direction based upon the Book's
					// Language not Direction
					Language lang = (Language) getProperty(ConfigEntryType.LANG);
					return lang.isLeftToRight;
				}
    
				return ConfigEntryType.DIRECTION_LTOR.Equals(dir);
			}
		}

		private void buildProperties()
		{
			// merge entries into properties file
			foreach (ConfigEntryType key in cet.Keys)
			{
				object value = cet.getValue(key);
				// value is null if the config entry was rejected.
				if (value == null)
				{
					continue;
				}
//JAVA TO VB & C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: if (value instanceof java.util.List<?>)
                if (value is IList<string>)
				{
					IList<string> list = (IList<string>) value;
                    StringBuilder combined = new StringBuilder();
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

				putProperty(key.ToString(), value);
			}
			// Element ele = cet.toOSIS();
			// SAXEventProvider sep = new JDOMSAXEventProvider(new Document(ele));
			// try
			// {
			// System.out.println(XMLUtil.writeToString(sep));
			// }
			// catch(Exception e)
			// {
			// }
		}

	///    
	/// <summary>* Sword only recognizes two encodings for its modules: UTF-8 and LATIN1
	/// Sword uses MS Windows cp1252 for Latin 1 not the standard. Arrgh! The
	/// language strings need to be converted to Java charsets </summary>
	///    
    /*
		private static readonly PropertyMap ENCODING_JAVA = new PropertyMap();
		static SwordBookMetaData()
		{
			ENCODING_JAVA.put("Latin-1", "WINDOWS-1252");
			ENCODING_JAVA.put("UTF-8", "UTF-8");
		}
        */

        ///    
        /// <param name="key">
        ///            the key of the property. </param>
        /// <returns> the value of the property </returns>
        ///     
        object getProperty(string key)
        {
            if (prop.ContainsKey(key))
            {
                return prop[key];
            }
            else
            {
                return null;
            }
        }

        ///    
        /// <param name="key">
        ///            the key of the property to set </param>
        /// <param name="value">
        ///            the value of the property </param>
        ///     
        void putProperty(string key, object value)
        {
            prop[key] = value;
        }

        public object getCetProperty(ConfigEntryType confType)
        {
            return cet.getValue(confType);
        }

        private Dictionary<string, object> prop = new Dictionary<string, object>();
        private ConfigEntryTable cet;
	}

}