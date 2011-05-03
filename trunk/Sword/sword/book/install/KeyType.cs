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
/// ID: $Id: CaseType.java 1890 2008-07-09 12:15:15Z dmsmith $
/// 
/// Converted from Java to C# by Thomas Dilts with the help of a program from www.tangiblesoftwaresolutions.com
/// called 'Java to VB & C# Converter' on 2011-04-12 </summary>
/// 
using System.Diagnostics;
using System.Collections.Generic;
namespace book.install
{

    ///
    /// <summary> * Types of Key that a Book uses, either verse, list, or tree.
    /// *  </summary>
    /// * <seealso cref= gnu.lgpl.License for license details.<br>
    /// *      The copyright to this program is held by it's authors.
    /// * @author DM Smith [dmsmith555 at yahoo dot com] </seealso>
    /// 
    public class KeyType
    {
        ///    
        ///     <summary> * Book contains verses and can be understood as book, chapter and verse. </summary>
        ///     
        public static readonly KeyType VERSE = new KeyType("VERSE", 0);

        ///    
        ///     <summary> * Book organizes its entries in a list, as in a dictionary. </summary>
        ///     
        public static readonly KeyType LIST = new KeyType("LIST", 1);

        ///    
        ///     <summary> * Book organizes its entries in a tree, as in a general book. </summary>
        ///     
        public static readonly KeyType TREE = new KeyType("TREE", 2);

        private static Dictionary<string, KeyType> values = new Dictionary<string, KeyType>()
        {
            {"VERSE", VERSE},
            {"LIST", LIST},
            {"TREE", TREE},
        };

        ///    
        ///     <summary> * Get an integer representation for this CaseType </summary>
        ///     
        public virtual int toInteger()
        {
            return index;
        }
        public KeyType(string name, int index)
        {
            this.name = name;
            this.index = index;
        }
        private string name;
        private int index;
        ///    
        ///     <summary> * Lookup method to convert from a String </summary>
        ///     
        public static KeyType fromString(string name)
        {
            KeyType keytype = null;
            values.TryGetValue(name.ToUpper(), out keytype);
            Debug.Assert(keytype != null);
            return keytype;
        }
    }
}