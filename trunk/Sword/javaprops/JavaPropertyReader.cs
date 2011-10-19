#region Header

// <copyright file="JavaPropertyReader.cs" company="Thomas Dilts">
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

namespace Sword.javaprops
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;

    /// <summary>
    ///   This class reads Java style properties from an input stream.
    /// </summary>
    public class JavaPropertyReader
    {
        #region Fields

        private const int ActionAddToKey = 1;
        private const int ActionAddToValue = 2;
        private const int ActionEscape = 4;
        private const int ActionIgnore = 5;
        private const int ActionStoreProperty = 3;
        private const int MatchAny = 4;
        private const int MatchEndOfInput = 1;
        private const int MatchTerminator = 2;
        private const int MatchWhitespace = 3;
        private const int StateAfterSeparator = 6;
        private const int StateBeforeSeparator = 5;
        private const int StateComment = 1;
        private const int StateFinish = 10;
        private const int StateKey = 2;
        private const int StateKeyEscape = 3;
        private const int StateKeyWs = 4;
        private const int StateStart = 0;
        private const int StateValue = 7;
        private const int StateValueEscape = 8;
        private const int StateValueWs = 9;

        private static readonly int[][] States = new[]
                                                     {
                                                         new[]
                                                             {
        //STATE_start
                                                                 MatchEndOfInput, StateFinish, ActionIgnore,
                                                                 MatchTerminator, StateStart, ActionIgnore,
                                                                 '#', StateComment, ActionIgnore,
                                                                 '!', StateComment, ActionIgnore,
                                                                 MatchWhitespace, StateStart, ActionIgnore,
                                                                 '\\', StateKeyEscape, ActionEscape,
                                                                 ':', StateAfterSeparator, ActionIgnore,
                                                                 '=', StateAfterSeparator, ActionIgnore,
                                                                 MatchAny, StateKey, ActionAddToKey
                                                             },
                                                         new[]
                                                             {
        //STATE_comment
                                                                 MatchEndOfInput, StateFinish, ActionIgnore,
                                                                 MatchTerminator, StateStart, ActionIgnore,
                                                                 MatchAny, StateComment, ActionIgnore
                                                             },
                                                         new[]
                                                             {
        //STATE_key
                                                                 MatchEndOfInput, StateFinish, ActionStoreProperty
                                                                 ,
                                                                 MatchTerminator, StateStart, ActionStoreProperty,
                                                                 MatchWhitespace, StateBeforeSeparator, ActionIgnore
                                                                 ,
                                                                 '\\', StateKeyEscape, ActionEscape,
                                                                 ':', StateAfterSeparator, ActionIgnore,
                                                                 '=', StateAfterSeparator, ActionIgnore,
                                                                 MatchAny, StateKey, ActionAddToKey
                                                             },
                                                         new[]
                                                             {
        //STATE_key_escape
                                                                 MatchTerminator, StateKeyWs, ActionIgnore,
                                                                 MatchAny, StateKey, ActionAddToKey
                                                             },
                                                         new[]
                                                             {
        //STATE_key_ws
                                                                 MatchEndOfInput, StateFinish, ActionStoreProperty
                                                                 ,
                                                                 MatchTerminator, StateStart, ActionStoreProperty,
                                                                 MatchWhitespace, StateKeyWs, ActionIgnore,
                                                                 '\\', StateKeyEscape, ActionEscape,
                                                                 ':', StateAfterSeparator, ActionIgnore,
                                                                 '=', StateAfterSeparator, ActionIgnore,
                                                                 MatchAny, StateKey, ActionAddToKey
                                                             },
                                                         new[]
                                                             {
        //STATE_before_separator
                                                                 MatchEndOfInput, StateFinish, ActionStoreProperty
                                                                 ,
                                                                 MatchTerminator, StateStart, ActionStoreProperty,
                                                                 MatchWhitespace, StateBeforeSeparator, ActionIgnore
                                                                 ,
                                                                 '\\', StateValueEscape, ActionEscape,
                                                                 ':', StateAfterSeparator, ActionIgnore,
                                                                 '=', StateAfterSeparator, ActionIgnore,
                                                                 MatchAny, StateValue, ActionAddToValue
                                                             },
                                                         new[]
                                                             {
        //STATE_after_separator
                                                                 MatchEndOfInput, StateFinish, ActionStoreProperty
                                                                 ,
                                                                 MatchTerminator, StateStart, ActionStoreProperty,
                                                                 MatchWhitespace, StateAfterSeparator, ActionIgnore,
                                                                 '\\', StateValueEscape, ActionEscape,
                                                                 MatchAny, StateValue, ActionAddToValue
                                                             },
                                                         new[]
                                                             {
        //STATE_value
                                                                 MatchEndOfInput, StateFinish, ActionStoreProperty
                                                                 ,
                                                                 MatchTerminator, StateStart, ActionStoreProperty,
                                                                 '\\', StateValueEscape, ActionEscape,
                                                                 MatchAny, StateValue, ActionAddToValue
                                                             },
                                                         new[]
                                                             {
        //STATE_value_escape
                                                                 MatchTerminator, StateValueWs, ActionIgnore,
                                                                 MatchAny, StateValue, ActionAddToValue
                                                             },
                                                         new[]
                                                             {
        //STATE_value_ws
                                                                 MatchEndOfInput, StateFinish, ActionStoreProperty
                                                                 ,
                                                                 MatchTerminator, StateStart, ActionStoreProperty,
                                                                 MatchWhitespace, StateValueWs, ActionIgnore,
                                                                 '\\', StateValueEscape, ActionEscape,
                                                                 MatchAny, StateValue, ActionAddToValue
                                                             }
                                                     };

        private readonly Dictionary<string, string> _dictionary;
        private readonly StringBuilder _keyBuilder = new StringBuilder();
        private readonly StringBuilder _valueBuilder = new StringBuilder();

        private bool _escaped;
        private StreamReader _reader;

        #endregion Fields

        #region Constructors

        /// <summary>
        ///   Construct a reader passing a reference to a Dictionary (or JavaProperties) instance
        ///                                                           where the keys are to be stored.
        /// </summary>
        /// <param name = "dictionary">A reference to a Dictionary where the key-value pairs can be stored.</param>
        public JavaPropertyReader(Dictionary<string, string> dictionary)
        {
            _dictionary = dictionary;
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        ///   <para>Load key value pairs (properties) from an input Stream expected to have ISO-8859-1 encoding (code page 28592).  
        ///     The input stream (usually reading from a ".properties" file) consists of a series of lines (terminated 
        ///     by \r, \n or \r\n) each a key value pair, a comment or a blank line.</para>
        /// 
        ///   <para>Leading whitespace (spaces, tabs, formfeeds) are ignored at the start of any line - and a line that is empty or 
        ///     contains only whitespace is blank and ignored.</para>
        /// 
        ///   <para>A line with the first non-whitespace character is a '#' or '!' is a comment line and the rest of the line is 
        ///     ignored.</para>
        /// 
        ///   <para>If the first non-whitespace character is not '#' or '!' then it is the start of a key.  A key is all the
        ///     characters up to the first whitespace or a key/value separator - '=' or ':'.</para>
        /// 
        ///   <para>The separator is optional.  Any whitespace after the key or after the separator (if present) is ignored.</para>
        /// 
        ///   <para>The first non-whitespace character after the separator (or after the key if no separator) begins the value.  
        ///     The value may include whitespace, separators, or comment characters.</para>
        /// 
        ///   <para>Any unicode character may be included in either key or value by using escapes preceded by the escape 
        ///     character '\'.</para>
        /// 
        ///   <para>The following special cases are defined:</para>
        ///   <code>
        ///     '\t' - horizontal tab.
        ///     '\f' - form feed.
        ///     '\r' - return
        ///     '\n' - new line
        ///     '\\' - add escape character.
        /// 
        ///     '\ ' - add space in a key or at the start of a value.
        ///     '\!', '\#' - add comment markers at the start of a key.
        ///     '\=', '\:' - add a separator in a key.
        ///   </code>
        /// 
        ///   <para>Any unicode character using the following escape:</para>
        ///   <code>
        ///     '\uXXXX' - where XXXX represents the unicode character code as 4 hexadecimal digits.
        ///   </code>
        /// 
        ///   <para>Finally, longer lines can be broken by putting an escape at the very end of the line.  Any leading space
        ///     (unless escaped) is skipped at the beginning of the following line.</para>
        /// 
        ///   Examples
        ///   <code>
        ///     a-key = a-value
        ///     a-key : a-value
        ///     a-key=a-value
        ///     a-key a-value
        ///   </code>
        /// 
        ///   <para>All the above will result in the same key/value pair - key "a-key" and value "a-value".</para>
        ///   <code>
        ///     ! comment...
        ///     # another comment...
        ///   </code>
        /// 
        ///   <para>The above are two examples of comments.</para>
        ///   <code>
        ///     Honk\ Kong = Near China
        ///   </code>
        /// 
        ///   <para>The above shows how to embed a space in a key - key is "Hong Kong", value is "Near China".</para>
        ///   <code>
        ///     a-longer-key-example = a really long value that is \
        ///     split over two lines.
        ///   </code>
        /// 
        ///   <para>An example of a long line split into two.</para>
        /// </summary>
        /// <param name = "stream">The input stream that the properties are read from.</param>
        public void Parse(Stream stream)
        {
            _reader = new StreamReader(stream);

            int state = StateStart;
            do
            {
                int ch = NextChar();

                bool matched = false;

                for (int s = 0; s < States[state].Length; s += 3)
                {
                    if (Matches(States[state][s], ch))
                    {
                        // Debug.WriteLine( stateNames[ state ] + ", " + (s/3) + ", " + ch + (ch>20?" (" + (char) ch + ")" : "") );
                        matched = true;
                        DoAction(States[state][s + 2], ch);

                        state = States[state][s + 1];
                        break;
                    }
                }

                if (!matched)
                {
                    throw new Exception("Unexpected character at " + 1 + ": <<<" + ch + ">>>");
                }
            } while (state != StateFinish);
        }

        private void DoAction(int action, int ch)
        {
            switch (action)
            {
                case ActionAddToKey:
                    _keyBuilder.Append(EscapedChar(ch));
                    _escaped = false;
                    break;

                case ActionAddToValue:
                    _valueBuilder.Append(EscapedChar(ch));
                    _escaped = false;
                    break;

                case ActionStoreProperty:
                    // Debug.WriteLine( keyBuilder.ToString() + "=" + valueBuilder.ToString() );
                    _dictionary[_keyBuilder.ToString()] = _valueBuilder.ToString();
                    _keyBuilder.Length = 0;
                    _valueBuilder.Length = 0;
                    _escaped = false;
                    break;

                case ActionEscape:
                    _escaped = true;
                    break;

                    // case ACTION_ignore:
                default:
                    _escaped = false;
                    break;
            }
        }

        private char EscapedChar(int ch)
        {
            if (_escaped)
            {
                switch (ch)
                {
                    case 't':
                        return '\t';
                    case 'r':
                        return '\r';
                    case 'n':
                        return '\n';
                    case 'f':
                        return '\f';
                    case 'u':
                        int uch = 0;
                        for (int i = 0; i < 4; i++)
                        {
                            ch = NextChar();
                            if (ch >= '0' && ch <= '9')
                            {
                                uch = (uch << 4) + ch - '0';
                            }
                            else if (ch >= 'a' && ch <= 'z')
                            {
                                uch = (uch << 4) + ch - 'a' + 10;
                            }
                            else if (ch >= 'A' && ch <= 'Z')
                            {
                                uch = (uch << 4) + ch - 'A' + 10;
                            }
                            else
                            {
                                throw new Exception("Invalid Unicode character.");
                            }
                        }
                        return (char) uch;
                }
            }

            return (char) ch;
        }

        private bool Matches(int match, int ch)
        {
            switch (match)
            {
                case MatchEndOfInput:
                    return ch == -1;

                case MatchTerminator:
                    if (ch == '\r')
                    {
                        if (PeekChar() == '\n')
                        {
                            NextChar(); // consume the character
                        }
                        return true;
                    }
                    if (ch == '\n')
                    {
                        return true;
                    }
                    return false;

                case MatchWhitespace:
                    return ch == ' ' || ch == '\t' || ch == '\f';

                case MatchAny:
                    return true;

                default:
                    return ch == match;
            }
        }

        private int NextChar()
        {
            var nextC = new char[1];
            if (_reader.Read(nextC, 0, 1) == 0)
            {
                return -1;
            }
            return nextC[0];
        }

        private int PeekChar()
        {
            return _reader.Peek();
        }

        #endregion Methods
    }
}