using System;

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
/// ID: $Id: BookCategory.java 2106 2011-03-07 21:14:31Z dmsmith $
/// 
/// Converted from Java to C# by Thomas Dilts with the help of a program from www.tangiblesoftwaresolutions.com
/// called 'Java to VB & C# Converter' on 2011-04-12 </summary>
/// 
namespace SwordBackend
{


	//using JSMsg = org.crosswire.jsword.JSMsg;
    using System.Collections.Generic;

	///
	/// <summary> An Enumeration of the possible types of Book.
	/// </summary>
	/// <seealso cref= gnu.lgpl.License for license details.<br>
	///      The copyright to this program is held by it's authors.
	/// @author Joe Walker [joe at eireneh dot com]
	/// @author DM Smith [dmsmith555 at yahoo dot com] </seealso>
	/// 
	public class BookCategory //: IComparable<BookCategory>
	{
        private static readonly BookCategory[] VALUES = { BIBLE, DICTIONARY, COMMENTARY, DAILY_DEVOTIONS, GLOSSARY, QUESTIONABLE, ESSAYS, IMAGES, MAPS, GENERAL_BOOK, OTHER };
        private static Dictionary<string, BookCategory> internalValues = new Dictionary<string, BookCategory>();
        private static Dictionary<string, BookCategory> externalValues = new Dictionary<string, BookCategory>();

        /// <summary> Books that are Bibles  </summary>
		// TRANSLATOR: The name for the book category consisting of Bibles.
		public static readonly BookCategory BIBLE = new BookCategory("Biblical Texts", JSMsg.gettext("Biblical Texts"));

		/// <summary> Books that are Dictionaries  </summary>
		// TRANSLATOR: The name for the book category consisting of Lexicons and Dictionaries.
		public static readonly BookCategory DICTIONARY = new BookCategory("Lexicons / Dictionaries", JSMsg.gettext("Dictionaries"));

		/// <summary> Books that are Commentaries  </summary>
		// TRANSLATOR: The name for the book category consisting of Commentaries.
		public static readonly BookCategory COMMENTARY = new BookCategory("Commentaries", JSMsg.gettext("Commentaries"));

		/// <summary> Books that are indexed by day. AKA, Daily Devotions  </summary>
		// TRANSLATOR: The name for the book category consisting of Daily Devotions, indexed by day of the year.
		public static readonly BookCategory DAILY_DEVOTIONS = new BookCategory("Daily Devotional", JSMsg.gettext("Daily Devotionals"));

		/// <summary> Books that map words from one language to another.  </summary>
		// TRANSLATOR: The name for the book category consisting of Glossaries that map words/phrases from one language into another.
		public static readonly BookCategory GLOSSARY = new BookCategory("Glossaries", JSMsg.gettext("Glossaries"));

		/// <summary> Books that are questionable.  </summary>
		// TRANSLATOR: The name for the book category consisting of books that are considered unorthodox by mainstream Christianity.
		public static readonly BookCategory QUESTIONABLE = new BookCategory("Cults / Unorthodox / Questionable Material", JSMsg.gettext("Cults / Unorthodox / Questionable Materials"));

		/// <summary> Books that are just essays.  </summary>
		// TRANSLATOR: The name for the book category consisting of just essays.
		public static readonly BookCategory ESSAYS = new BookCategory("Essays", JSMsg.gettext("Essays"));

		/// <summary> Books that are predominately images.  </summary>
		// TRANSLATOR: The name for the book category consisting of books containing mostly images.
		public static readonly BookCategory IMAGES = new BookCategory("Images", JSMsg.gettext("Images"));

		/// <summary> Books that are a collection of maps.  </summary>
		// TRANSLATOR: The name for the book category consisting of books containing mostly maps.
		public static readonly BookCategory MAPS = new BookCategory("Maps", JSMsg.gettext("Maps"));

		/// <summary> Books that are just books.  </summary>
		// TRANSLATOR: The name for the book category consisting of general books.
		public static readonly BookCategory GENERAL_BOOK = new BookCategory("Generic Books", JSMsg.gettext("General Books"));

		/// <summary> Books that are not any of the above. This is a catch all for new book categories.  </summary>
		// TRANSLATOR: The name for the book category consisting of books not in any of the other categories.
		public static readonly BookCategory OTHER = new BookCategory("Other", JSMsg.gettext("Other"));

	///    
	/// <param name="name">
	///            The name of the BookCategory </param>
	///     
		private BookCategory(string name, string externalName)
		{
			this.name = name;
			this.externalName = externalName;
            BookCategory.internalValues[name.ToUpper()] = this;
            BookCategory.externalValues[externalName.ToUpper()] = this;
        }

	///    
	/// <summary>* Lookup method to convert from a string </summary>
	///     
		public static BookCategory fromString(string name)
		{
            BookCategory bookCategory = null;
            internalValues.TryGetValue(name.ToUpper(), out bookCategory);
            if (bookCategory != null)
            {
                return bookCategory;
            }
            else
            {
                return OTHER;
            }
		}

	///    
	/// <summary>* Lookup method to convert from a string </summary>
	///     
		public static BookCategory fromExternalString(string name)
		{
            BookCategory bookCategory = null;
            externalValues.TryGetValue(name.ToUpper(), out bookCategory);
            if (bookCategory != null)
            {
                return bookCategory;
            }
            else
            {
                return OTHER;
            } 
		}

	///    
	/// <summary>* Lookup method to convert from an integer </summary>
	///     
		public static BookCategory fromInteger(int i)
		{
			return VALUES[i];
		}

		/*
		 * (non-Javadoc)
		 * 
		 * @see java.lang.Comparable#compareTo(java.lang.Object)
		 */
		public int compareTo(BookCategory that)
		{
			return this.name.CompareTo(that.name);
		}

		/*
		 * (non-Javadoc)
		 * 
		 * @see java.lang.Object#equals(java.lang.Object)
		 */
		public override bool Equals(object o)
		{
			return base.Equals(o);
		}

		/*
		 * (non-Javadoc)
		 * 
		 * @see java.lang.Object#hashCode()
		 */
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

	///    
	/// <returns> the internal name. </returns>
	///     
		public string Name
		{
			get
			{
				return name;
			}
		}

		/*
		 * (non-Javadoc)
		 * 
		 * @see java.lang.Object#toString()
		 */
		public override string ToString()
		{
			return externalName;
		}

	///    
	/// <summary>* The names of the BookCategory </summary>
	///     

		private string name;

		private string externalName;

		// Support for serialization
		private static int nextObj;
		private readonly int obj = nextObj++;

		internal object readResolve()
		{
			return VALUES[obj];
		}

	///    
	/// <summary>* Serialization ID </summary>
	///     
		private const long serialVersionUID = 3256727260177708345L;
	}

}