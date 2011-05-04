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
/// Copyright: 2007
///     The copyright to this program is held by it's authors.
///
/// ID: $Id: JSMsg.java 2099 2011-03-07 17:13:00Z dmsmith $
/// 
/// Converted from Java to C# by Thomas Dilts with the help of a program from www.tangiblesoftwaresolutions.com
/// called 'Java to VB & C# Converter' on 2011-04-12 </summary>
/// 
using javaprops;
namespace SwordBackend
{

	///
	/// <summary> Compile safe Msg resource settings.
	///  </summary>
	/// <seealso cref= gnu.lgpl.License for license details.<br>
	///      The copyright to this program is held by it's authors.
	/// @author DM Smith [dmsmith555 at yahoo dot com] </seealso>
	/// 
	public class JSOtherMsg// : org.crosswire.common.util.MsgBase
	{
	///    
	/// <summary>* Get the internationalized text, but return key if key is unknown.
	/// The text requires one or more parameters to be passed.
	///  </summary>
	/// <param name="key"> </param>
	/// <param name="params"> </param>
	/// <returns> the formatted, internationalized text </returns>
	///     
        public static string lookupText(string key, params object[] paramsIn)
		{
            string msg;
            if (!messages.TryGetValue(key, out msg))
            {
                msg = key;
            }
            return string.Format(msg, paramsIn);
		}

        private static JavaProperties messages = new JavaProperties("JSOtherMsg",true);

	}

}