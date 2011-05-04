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
/// ID: $Id: BookType.java 2099 2011-03-07 17:13:00Z dmsmith $
/// 
/// Converted from Java to C# by Thomas Dilts with the help of a program from www.tangiblesoftwaresolutions.com
/// called 'Java to VB & C# Converter' on 2011-04-12 </summary>
/// 
namespace SwordBackend
{
    /*
    using JSOtherMsg = org.crosswire.jsword.JSOtherMsg;
    using Book = org.crosswire.jsword.book.Book;
    using BookCategory = org.crosswire.jsword.book.BookCategory;
    using BookException = org.crosswire.jsword.book.BookException;
    using KeyType = org.crosswire.jsword.book.KeyType; */
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;

    ///
    /// <summary> * Data about book types.
    /// *  </summary>
    /// * <seealso cref= gnu.lgpl.License for license details.<br>
    /// *      The copyright to this program is held by it's authors.
    /// * @author Joe Walker [joe at eireneh dot com]
    /// * @author DM Smith [dmsmith555 at yahoo dot com] </seealso>
    /// 
    public class BookType
    {
        
        /// <summary>
        /// This is the used dictionary.
        /// </summary>
        private static readonly Dictionary<string, BookType> values = new Dictionary<string, BookType>()
        {
            {"RAWTEXT",new BookType_RAW_TEXT()},
            {"ZTEXT",new BookType_Z_TEXT()},
            {"RAWCOM",new BookType_RAW_COM()},
            {"RAWCOM4",new BookType_RAW_COM4()},
            {"ZCOM",new BookType_Z_COM()},
            {"HREFCOM",new BookType_HREF_COM()},
            {"RAWFILES",new BookType_RAW_FILES()},
            {"RAWLD",new BookType_RAW_LD()},
            {"RAWLD4",new BookType_RAW_LD4()},
            {"ZLD",new BookType_Z_LD()},
            {"RAWGENBOOK",new BookType_RAW_GEN_BOOK()},
        };
        
        ///    
        ///     <summary> * Simple ctor </summary>
        ///     
        protected BookType(string name, BookCategory category, KeyType type)
        {
            this.name = name;
            this.category = category;
            this.keyType = type;
        }

        ///    
        ///     <summary> * Find a BookType from a name.
        ///     *  </summary>
        ///     * <param name="name">
        ///     *            The name of the BookType to look up </param>
        ///     * <returns> The found BookType or null if the name is not found </returns>
        ///     
        public static BookType getBookType(string name)
        {
            BookType booktype = null;
            values.TryGetValue(name.ToUpper(), out booktype);
            Debug.Assert(booktype != null);
            if (booktype != null)
            {
                return booktype;
            }
            else
            {
                throw new System.ArgumentException(JSOtherMsg.lookupText("BookType {0} is not defined!", name));
            }
        }

        ///    
        ///     <summary> * The category of this book </summary>
        ///     
        public virtual BookCategory BookCategory
        {
            get
            {
                return category;
            }
        }

        ///    
        ///     <summary> * Get the way this type of Book organizes it's keys.
        ///     *  </summary>
        ///     * <returns> the organization of keys for this book </returns>
        ///     
        public virtual KeyType KeyType
        {
            get
            {
                return keyType;
            }
        }

        ///    
        ///     <summary> * Given a SwordBookMetaData determine whether this BookType will work for
        ///     * it.
        ///     *  </summary>
        ///     * <param name="sbmd">
        ///     *            the BookMetaData that this BookType works upon </param>
        ///     * <returns> true if this is a usable BookType </returns>
        ///     
        public virtual bool isSupported(SwordBookMetaData sbmd)
        {
            return category != null && sbmd != null;
        }

        ///    
        ///     <summary> * Create a Book appropriate for the BookMetaData
        ///     *  </summary>
        ///     * <exception cref="BookException"> </exception>
        ///     
        //JAVA TO VB & C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
        //ORIGINAL LINE: public org.crosswire.jsword.book.Book createBook(SwordBookMetaData sbmd) throws org.crosswire.jsword.book.BookException
        //public virtual Book createBook(SwordBookMetaData sbmd)
        //{
        //    return getBook(sbmd, getBackend(sbmd));
        //}

        ///    
        ///     <summary> * Create a Book with the given backend </summary>
        ///     
        //protected internal abstract Book getBook(SwordBookMetaData sbmd, AbstractBackend backend);

        ///    
        ///     <summary> * Create a the appropriate backend for this type of book </summary>
        ///     
        //JAVA TO VB & C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
        //ORIGINAL LINE: protected abstract AbstractBackend getBackend(SwordBookMetaData sbmd) throws org.crosswire.jsword.book.BookException;
        //protected internal abstract AbstractBackend getBackend(SwordBookMetaData sbmd);

        ///    
        ///     <summary> * The name of the BookType </summary>
        ///     
        private string name;

        ///    
        ///     <summary> * What category is this book </summary>
        ///     
        private BookCategory category;

        ///    
        ///     <summary> * What category is this book </summary>
        ///     
        private KeyType keyType;

        ///    
        ///     <summary> * Lookup method to convert from a String </summary>
        ///     
        public static BookType fromString(string name)
        {
            return getBookType(name);
        }

        /* (non-Javadoc)
         * @see java.lang.Enum#toString()
         */
        public override string ToString()
        {
            return name;
        }
        

        ///    
        ///     <summary> * Uncompressed Bibles </summary>
        ///     
        public class BookType_RAW_TEXT : BookType
        {
            public BookType_RAW_TEXT() : base("RawText", BookCategory.BIBLE, KeyType.VERSE) { }
            /*
            protected internal override Book getBook(SwordBookMetaData sbmd, AbstractBackend backend)
            {
                return new SwordBook(sbmd, backend);
            }

            protected internal override AbstractBackend getBackend(SwordBookMetaData sbmd)
            {
                return new RawBackend(sbmd, 2);
            }*/
        }
        public class BookType_Z_TEXT : BookType
        ///    
        ///     <summary> * Compressed Bibles </summary>
        ///     
        {
            public BookType_Z_TEXT() : base("zText", BookCategory.BIBLE, KeyType.VERSE) { }
            /*
            protected internal override Book getBook(SwordBookMetaData sbmd, AbstractBackend backend)
            {
                return new SwordBook(sbmd, backend);
            }

            protected internal override AbstractBackend getBackend(SwordBookMetaData sbmd)
            {
                BlockType blockType = BlockType.fromString((string)sbmd.getProperty(ConfigEntryType.BLOCK_TYPE));
                return new ZVerseBackend(sbmd, blockType);
            }*/
        }
        public class BookType_RAW_COM : BookType
        ///    
        ///     <summary> * Uncompressed Commentaries </summary>
        ///     
        {
            public BookType_RAW_COM() : base("RawCom", BookCategory.COMMENTARY, KeyType.VERSE) { }
            /*
            protected internal override Book getBook(SwordBookMetaData sbmd, AbstractBackend backend)
            {
                return new SwordBook(sbmd, backend);
            }

            protected internal override AbstractBackend getBackend(SwordBookMetaData sbmd)
            {
                return new RawBackend(sbmd, 2);
            }*/
        }
        public class BookType_RAW_COM4 : BookType
        {
            public BookType_RAW_COM4() : base("RawCom4", BookCategory.COMMENTARY, KeyType.VERSE) { }
            /*
            protected internal override Book getBook(SwordBookMetaData sbmd, AbstractBackend backend)
            {
                return new SwordBook(sbmd, backend);
            }

            protected internal override AbstractBackend getBackend(SwordBookMetaData sbmd)
            {
                return new RawBackend(sbmd, 4);
            }*/
        }
        public class BookType_Z_COM : BookType
        ///    
        ///     <summary> * Compressed Commentaries </summary>
        ///     
        {
            public BookType_Z_COM() : base("zCom", BookCategory.COMMENTARY, KeyType.VERSE) { }
            /*
            protected internal override Book getBook(SwordBookMetaData sbmd, AbstractBackend backend)
            {
                return new SwordBook(sbmd, backend);
            }

            protected internal override AbstractBackend getBackend(SwordBookMetaData sbmd)
            {
                BlockType blockType = BlockType.fromString((string)sbmd.getProperty(ConfigEntryType.BLOCK_TYPE));
                return new ZVerseBackend(sbmd, blockType);
            }*/
        }
        public class BookType_HREF_COM : BookType
        ///    
        ///     <summary> * Uncompresses HREF Commentaries </summary>
        ///     
        {
            public BookType_HREF_COM() : base("HREFCom", BookCategory.COMMENTARY, KeyType.VERSE) { }
            /*
            protected internal override Book getBook(SwordBookMetaData sbmd, AbstractBackend backend)
            {
                return new SwordBook(sbmd, backend);
            }

             protected internal override AbstractBackend getBackend(SwordBookMetaData sbmd)
            {
                return new RawBackend(sbmd, 2);
            }
            */
        }
        public class BookType_RAW_FILES : BookType
        ///    
        ///     <summary> * Uncompressed Commentaries </summary>
        ///     
        {
            public BookType_RAW_FILES() : base("RawFiles", BookCategory.COMMENTARY, KeyType.VERSE) { }
            /*
            protected internal override Book getBook(SwordBookMetaData sbmd, AbstractBackend backend)
            {
                return new SwordBook(sbmd, backend);
            }

            protected internal override AbstractBackend getBackend(SwordBookMetaData sbmd)
            {
                return new RawFileBackend(sbmd, 2);
            }*/
        }
        public class BookType_RAW_LD : BookType
        ///    
        ///     <summary> * 2-Byte Index Uncompressed Dictionaries </summary>
        ///     
        {
            public BookType_RAW_LD() : base("RawLD", BookCategory.DICTIONARY, KeyType.LIST) { }
            /*
            protected internal override Book getBook(SwordBookMetaData sbmd, AbstractBackend backend)
            {
                if (sbmd.BookCategory.Equals(BookCategory.DAILY_DEVOTIONS))
                {
                    return new SwordDailyDevotion(sbmd, backend);
                }
                return new SwordDictionary(sbmd, backend);
            }

             protected internal override AbstractBackend getBackend(SwordBookMetaData sbmd)
            {
                return new RawLDBackend(sbmd, 2);
            }*/
        }
        public class BookType_RAW_LD4 : BookType
        ///    
        ///     <summary> * 4-Byte Index Uncompressed Dictionaries </summary>
        ///     
        {
            public BookType_RAW_LD4() : base("RawLD4", BookCategory.DICTIONARY, KeyType.LIST) { }
            /*
            protected internal override Book getBook(SwordBookMetaData sbmd, AbstractBackend backend)
            {
                if (sbmd.BookCategory.Equals(BookCategory.DAILY_DEVOTIONS))
                {
                    return new SwordDailyDevotion(sbmd, backend);
                }
                return new SwordDictionary(sbmd, backend);
            }

            protected internal override AbstractBackend getBackend(SwordBookMetaData sbmd)
            {
                return new RawLDBackend(sbmd, 4);
            }*/
        }
        public class BookType_Z_LD : BookType
        ///    
        ///     <summary> * Compressed Dictionaries </summary>
        ///     
        {
            public BookType_Z_LD() : base("zLD", BookCategory.DICTIONARY, KeyType.LIST) { }
            /*
            protected internal override Book getBook(SwordBookMetaData sbmd, AbstractBackend backend)
            {
                if (sbmd.BookCategory.Equals(BookCategory.DAILY_DEVOTIONS))
                {
                    return new SwordDailyDevotion(sbmd, backend);
                }
                return new SwordDictionary(sbmd, backend);
            }

             protected internal override AbstractBackend getBackend(SwordBookMetaData sbmd)
            {
                return new ZLDBackend(sbmd);
            }*/
        }
        public class BookType_RAW_GEN_BOOK : BookType
        ///    
        ///     <summary> * Generic Books </summary>
        ///     
        {
            public BookType_RAW_GEN_BOOK() : base("RawGenBook", BookCategory.GENERAL_BOOK, KeyType.TREE) { }
            /*
            protected internal override Book getBook(SwordBookMetaData sbmd, AbstractBackend backend)
            {
                return new SwordGenBook(sbmd, backend);
            }

            protected internal override AbstractBackend getBackend(SwordBookMetaData sbmd)
            {
                return new GenBookBackend(sbmd);
            }*/
        }
       
    }
}

