#region Header

// <copyright file="BibleNames.cs" company="Thomas Dilts">
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
    using System.Globalization;
    using System.IO;
    using System.Reflection;
    using System.Xml;
    using Sword.reader;
    using System.Collections.Generic;
    using System.Linq;

    public class BibleNames
    {
        #region Fields

        private readonly Dictionary<string,string> _fullNames = new Dictionary<string,string>();

        private readonly bool _isShortNamesExisting = true;

        private readonly Dictionary<string, string> _shortNames = new Dictionary<string, string>();

        private string _isoLanguage;
        public string isoLanguage { get { return _isoLanguage; } }

        #endregion

        #region Constructors and Destructors

        public BibleNames(string isoLang2DigitCode, string appChosenIsoLangCode)
        {
            // rules:
            // 1. Bible language code = isoLang2DigitCode unless it is zh, in that case 1.zh=os, os 2.zh=chosen,chosen, else simplified.
            // 2. appChosenIsoLangCode
            // 3. operating system iso lang code.
            // 4. english

            Assembly assem = Assembly.Load(new AssemblyName("Sword"));
            string isocode = CultureInfo.CurrentCulture.TwoLetterISOLanguageName.ToLower();
            string name = CultureInfo.CurrentCulture.Name.Replace('-', '_');
            Stream stream = null;

            if (isoLang2DigitCode.ToLower().StartsWith("zh"))
            {
                switch (isoLang2DigitCode.ToLower().Replace('-', '_'))
                {
                    case "zh_hant":
                    case "zh_tw":
                    case "zh_hk":
                    case "zh_mo":
                        stream = assem.GetManifestResourceStream("Sword.Properties.BibleNames_" + "zh_tw" + ".xml");
                        if(stream!=null)
                        {
                            _isoLanguage = "zh_tw";
                        }
                        break;
                    case "zh_hans":
                    case "zh_cn":
                    case "zh_sg":
                    case "zh_my":
                        stream = assem.GetManifestResourceStream("Sword.Properties.BibleNames_" + "zh_cn" + ".xml");
                        if(stream!=null)
                        {
                            _isoLanguage = "zh_cn";
                        }
                        break;
                    case "zh":
                        break;
                }
                if (stream == null && !string.IsNullOrEmpty(appChosenIsoLangCode) && appChosenIsoLangCode.Substring(0, 2).Equals(isoLang2DigitCode.Substring(0, 2)))
                {
                    stream = assem.GetManifestResourceStream("Sword.Properties.BibleNames_" + (appChosenIsoLangCode.Equals("zh")?"zh_tw":"zh_cn") + ".xml");
                    if(stream!=null)
                    {
                        _isoLanguage = appChosenIsoLangCode.Equals("zh") ? "zh_tw" : "zh_cn";
                    }
                }

                if (stream == null && isocode.Equals(isoLang2DigitCode.Substring(0, 2)))
                {
                    stream = assem.GetManifestResourceStream("Sword.Properties.BibleNames_" + name + ".xml");
                    if (stream != null)
                    {
                        _isoLanguage = name;
                    }
                }

                if (stream == null)
                {
                    stream = assem.GetManifestResourceStream("Sword.Properties.BibleNames_" + "zh_cn" + ".xml");
                    if (stream != null)
                    {
                        _isoLanguage = "zh_cn";
                    }
                }
            }

            if(stream==null)
            {
                stream = assem.GetManifestResourceStream("Sword.Properties.BibleNames_" + isoLang2DigitCode.Substring(0, 2) + ".xml");
                if(stream == null)
                {
                    stream = assem.GetManifestResourceStream("Sword.Properties.BibleNames_" + appChosenIsoLangCode + ".xml");
                    if (stream == null)
                    {
                        stream = assem.GetManifestResourceStream("Sword.Properties.BibleNames_" + isocode + ".xml");
                        if (stream == null)
                        {
                            stream = assem.GetManifestResourceStream("Sword.Properties.BibleNames_" + "en" + ".xml");
                            _isoLanguage = "en";
                        }
                        else
                        {
                            _isoLanguage = isocode;
                        }
                    }
                    else
                    {
                        _isoLanguage = appChosenIsoLangCode;
                    }
                }
                else
                {
                    _isoLanguage = isoLang2DigitCode;
                }
            }

            // This right to left problem was fixed when we went over to xml format. So this code is no longer needed.
            bool isRtl = false; /* isoLang2DigitCode.Equals("iw") || isoLang2DigitCode.Equals("he") ||
                          isoLang2DigitCode.Equals("ar") || isoLang2DigitCode.Equals("ar_EG") ||
                          isoLang2DigitCode.Equals("fa");*/
            int numberBadShortNames = 0;
            if (stream != null)
            {
                using (XmlReader reader = XmlReader.Create(stream))
                {
                    // Parse the file and get each of the nodes.
                    while (reader.Read())
                    {
                        if (reader.NodeType == XmlNodeType.Element)
                        {
                            if (reader.Name.ToLower().Equals("book") && reader.HasAttributes)
                            {
                                string shortName = "";
                                string fullName = "";
                                string key = "";
                                reader.MoveToFirstAttribute();
                                do
                                {
                                    switch (reader.Name.ToLower())
                                    {
                                        case "key":
                                            key = reader.Value;
                                            break;
                                        case "short":
                                            shortName = isRtl ? this.Reverse(reader.Value) : reader.Value;
                                            break;
                                        case "full":
                                            fullName = isRtl ? this.Reverse(reader.Value) : reader.Value;
                                            break;
                                    }
                                }
                                while (reader.MoveToNextAttribute());
                                if (shortName.Length > 5)
                                {
                                    numberBadShortNames++;
                                }
                                this._shortNames[key] = shortName;
                                this._fullNames[key] = fullName;
                            }
                        }
                    }
                }
                stream.Dispose();
            }
            this._isShortNamesExisting = numberBadShortNames < 10 && !_isoLanguage.Substring(0, 2).Equals("zh");
        }

        #endregion

        #region Public Properties

        public bool ExistsShortNames
        {
            get
            {
                return this._isShortNamesExisting;
            }
        }

        #endregion

        #region Public Methods and Operators

        public string[] GetAllFullNames()
        {
            return this._fullNames.Values.ToArray();
        }

        public string[] GetAllShortNames()
        {
            return this._shortNames.Values.ToArray();
        }

        public string GetFullName(string bookShortName, string defaultFullName)
        {
            string fullname;
            if (this._fullNames.TryGetValue(bookShortName, out fullname))
            {
                return fullname;
            }

            return defaultFullName;
        }

        public string GetShortName(string bookShortName)
        {
            string fullname;
            if (this._shortNames.TryGetValue(bookShortName, out fullname))
            {
                return fullname;
            }

            return bookShortName; 
        }

        public string Reverse(string str)
        {
            char[] charArray = str.ToCharArray();
            int len = str.Length - 1;

            for (int i = 0; i < len; i++, len--)
            {
                charArray[i] ^= charArray[len];
                charArray[len] ^= charArray[i];
                charArray[i] ^= charArray[len];
            }
            return new string(charArray);
        }

        #endregion
    }
}