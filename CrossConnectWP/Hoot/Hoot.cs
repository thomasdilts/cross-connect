#region Header

// <copyright file="Hoot.cs" company="Thomas Dilts">
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
using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.IO;
using System.Xml.Serialization;
using System.Threading;
using System.Text.RegularExpressions;
using RaptorDB.Common;
using RaptorDB;

namespace hOOt
{


    

    using global::Hoot;
    internal enum OPERATION
    {
        AND,
        OR,
        ANDNOT
    }
    public class Hoot
    {
        private static char[] punctuation = new char[]
        {
            ' ', '.', ',', ';', ':', '?', '!', '\'', '-', '_', '(', ')', '[', ']',
            '{', '}', '/', '\\', '\"', '¿', '*', '¡', '«', '»', '=', '@', '¤', '%'
            , '&', '+', '§', '|', '^', '¨', '~', '$','“','”','’','‘'
        };
        public void Initialize(string IndexPath, string FileName)
        {
            _Path = IndexPath;
            _FileName = FileName;
            _docMode = false;
            if (_Path.EndsWith(PathHelper.DirectorySeparatorChar.ToString()) == false) _Path += PathHelper.DirectorySeparatorChar;
            Directory.CreateDirectory(IndexPath);
            //LogManager.Configure(_Path + "log.txt", 200, false);
            //_log.Debug("\r\n\r\n");
            //_log.Debug("Starting hOOt....");
            //_log.Debug("Storage Folder = " + _Path);

            _bitmaps = new BitmapIndex();
            _bitmaps.Initialize(_Path, _FileName + ".mgbmp");

            // read words
            LoadWords();
            // read deleted
            _deleted = new BoolIndex();
            _deleted.Initialize(_Path, "_deleted.idx");
        }

        private SafeDictionary<string, int> _words = new SafeDictionary<string, int>();
        private BitmapIndex _bitmaps;
        private BoolIndex _deleted;
        //private ILog _log = LogManager.GetLogger(typeof(Hoot));
        private int _lastDocNum = 0;
        private string _FileName = "words";
        private string _Path = "";
        private bool _docMode = false;

        public int WordCount
        {
            get { return _words.Count; }
        }

        public int DocumentCount
        {
            get { return _lastDocNum - (int)_deleted.GetBits().CountOnes(); }
        }

        public void Save()
        {
            InternalSave();
        }

        public void Index(int recordnumber, string text)
        {
            AddtoIndex(recordnumber, text);
        }

        public WAHBitArray Query(string filter, int maxsize)
        {
            return ExecutionPlan(filter, maxsize);
        }

        public void OptimizeIndex()
        {
            _bitmaps.Commit(false);
            _bitmaps.Optimize();
        }

        #region [  P R I V A T E   M E T H O D S  ]

        private WAHBitArray ExecutionPlan(string filter, int maxsize)
        {
            //_log.Debug("query : " + filter);
            DateTime dt = FastDateTime.Now;
            // query indexes
            string[] words = filter.Split(' ');
            bool defaulttoand = false;
            if (filter.IndexOfAny(new char[] { '+', '-' }, 0) >= 0)
                defaulttoand = true;

            WAHBitArray bits = null;

            foreach (string s in words)
            {
                int c;
                string word = s;
                if (s == "") continue;

                OPERATION op = OPERATION.OR;
                if (defaulttoand)
                    op = OPERATION.AND;

                if (s.StartsWith("+"))
                {
                    op = OPERATION.AND;
                    word = s.Replace("+", "");
                }

                if (s.StartsWith("-"))
                {
                    op = OPERATION.ANDNOT;
                    word = s.Replace("-", "");
                }

                if (word.Contains("*") || word.Contains("?"))
                {
                    WAHBitArray wildbits = null;
                    // do wildcard search
                    Regex reg = new Regex("^" + word.Replace("*", ".*").Replace("?", "."), RegexOptions.IgnoreCase);
                    foreach (string key in _words.Keys())
                    {
                        if (reg.IsMatch(key))
                        {
                            _words.TryGetValue(key, out c);
                            WAHBitArray ba = _bitmaps.GetBitmap(c);

                            wildbits = DoBitOperation(wildbits, ba, OPERATION.OR, maxsize);
                        }
                    }
                    if (wildbits == null)
                    {
                        wildbits = new WAHBitArray();
                    }

                    bits = DoBitOperation(bits, wildbits, op, maxsize);
                }
                else if (_words.TryGetValue(word.ToLowerInvariant(), out c))
                {
                    // bits logic
                    WAHBitArray ba = _bitmaps.GetBitmap(c);
                    bits = DoBitOperation(bits, ba, op, maxsize);
                }
                else
                {
                    // empty object but important in AND functionality.
                    var ba = new WAHBitArray();
                    bits = DoBitOperation(bits, ba, op, maxsize);
                }
            }
            if (bits == null)
                return new WAHBitArray();

            // remove deleted docs
            WAHBitArray ret ;
            if (_docMode)
                ret = bits.AndNot(_deleted.GetBits());
            else
                ret = bits;
            //_log.Debug("query time (ms) = " + FastDateTime.Now.Subtract(dt).TotalMilliseconds);
            return ret;
        }

        private static WAHBitArray DoBitOperation(WAHBitArray bits, WAHBitArray c, OPERATION op, int maxsize)
        {
            if (op == OPERATION.ANDNOT)
            {
                c = c.Not(maxsize);
            }

            if (bits != null)
            {
                switch (op)
                {
                    case OPERATION.ANDNOT:
                    case OPERATION.AND:
                        bits = c.And(bits);
                        break;
                    case OPERATION.OR:
                        bits = c.Or(bits);
                        break;
                }
            }
            else
                bits = c;
            return bits;
        }

        private object _lock = new object();
        private void InternalSave()
        {
            //lock (_lock)
            //{
                //_log.Debug("saving index...");
                DateTime dt = FastDateTime.Now;
                // save deleted
                _deleted.SaveIndex();

                _bitmaps.Commit(false);

                MemoryStream ms = new MemoryStream();
                BinaryWriter bw = new BinaryWriter(ms, Encoding.UTF8);

                // save words and bitmaps
                Stream words =
                    
                    File.OpenStreamForWriteAsync(
                        _Path + _FileName + ".words", File.CreationCollisionOption.ReplaceExisting);
               
                    foreach (string key in _words.Keys())
                    {
                        bw.Write(key);
                        bw.Write(_words[key]);
                    }
                    byte[] b = ms.ToArray();
                    words.Write(b, 0, b.Length);
                    words.Flush();
                    File.CloseStream(words);
               
                //_log.Debug("save time (ms) = " + FastDateTime.Now.Subtract(dt).TotalMilliseconds);
            //}
        }

        private void LoadWords()
        {
            if (File.Exists(_Path + _FileName + ".words") == false)
                return;
            // load words
            byte[] b = File.ReadAllBytes(_Path + _FileName + ".words");
            MemoryStream ms = new MemoryStream(b);
            BinaryReader br = new BinaryReader(ms, Encoding.UTF8);
            string s = br.ReadString();
            while (s != "")
            {
                int off = br.ReadInt32();
                _words.Add(s, off);
                try
                {
                    s = br.ReadString();
                }
                catch { s = ""; }
            }
            //_log.Debug("Word Count = " + _words.Count);
        }

        private void AddtoIndex(int recnum, string text)
        {
            if (text == "" || text == null)
                return;
            string[] keys;
            if (_docMode)
            {
                //_log.Debug("text size = " + text.Length);
                Dictionary<string, int> wordfreq = GenerateWordFreq(text);
                //_log.Debug("word count = " + wordfreq.Count);
                var kk = wordfreq.Keys;
                keys = new string[kk.Count];
                kk.CopyTo(keys, 0);
            }
            else
            {
                keys = text.Split(punctuation);
            }

            foreach (string key in keys)
            {
                if (key == "")
                    continue;
                var keyLower = key.ToLower();
                int bmp;
                if (_words.TryGetValue(keyLower, out bmp))
                {
                    (_bitmaps.GetBitmap(bmp)).Set(recnum, true);
                }
                else
                {
                    bmp = _bitmaps.GetFreeRecordNumber();
                    _bitmaps.SetDuplicate(bmp, recnum);
                    _words.Add(keyLower, bmp);
                }
            }
        }

        private Dictionary<string, int> GenerateWordFreq(string text)
        {
            Dictionary<string, int> dic = new Dictionary<string, int>(50000);

            char[] chars = text.ToCharArray();
            int index = 0;
            int run = -1;
            int count = chars.Length;
            while (index < count)
            {
                char c = chars[index++];
                if (!char.IsLetter(c))
                {
                    if (run != -1)
                    {
                        ParseString(dic, chars, index, run);
                        run = -1;
                    }
                }
                else
                    if (run == -1)
                        run = index - 1;
            }

            if (run != -1)
            {
                ParseString(dic, chars, index, run);
                run = -1;
            }

            return dic;
        }

        private void ParseString(Dictionary<string, int> dic, char[] chars, int end, int start)
        {
            // check if upper lower case mix -> extract words
            int uppers = 0;
            bool found = false;
            for (int i = start; i < end; i++)
            {
                if (char.IsUpper(chars[i]))
                    uppers++;
            }
            // not all uppercase
            if (uppers != end - start - 1)
            {
                int lastUpper = start;

                string word = "";
                for (int i = start + 1; i < end; i++)
                {
                    char c = chars[i];
                    if (char.IsUpper(c))
                    {
                        found = true;
                        word = new string(chars, lastUpper, i - lastUpper).ToLowerInvariant().Trim();
                        AddDictionary(dic, word);
                        lastUpper = i;
                    }
                }
                if (lastUpper > start)
                {
                    string last = new string(chars, lastUpper, end - lastUpper).ToLowerInvariant().Trim();
                    if (word != last)
                        AddDictionary(dic, last);
                }
            }
            if (found == false)
            {
                string s = new string(chars, start, end - start - 1).ToLowerInvariant().Trim();
                AddDictionary(dic, s);
            }
        }

        private void AddDictionary(Dictionary<string, int> dic, string word)
        {
            int l = word.Length;
            if (l > Global.DefaultStringKeySize)
                return;
            if (l < 2)
                return;
            if (char.IsLetter(word[l - 1]) == false)
                word = new string(word.ToCharArray(), 0, l - 2);
            if (word.Length < 2)
                return;
            int cc = 0;
            if (dic.TryGetValue(word, out cc))
                dic[word] = ++cc;
            else
                dic.Add(word, 1);
        }
        #endregion

        public void Shutdown()
        {
            Save();
            _deleted.Shutdown();
        }
    }
}