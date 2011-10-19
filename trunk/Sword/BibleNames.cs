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
// Email: thomas@chaniel.se
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

    public class BibleNames
    {
        #region Fields

        private readonly string[] fullNames = new string[BibleZtextReader.BooksInBible];
        private readonly bool isShortNamesExisting = true;
        private readonly string[] shortNames = new string[BibleZtextReader.BooksInBible];

        #endregion Fields

        #region Constructors

        public BibleNames(string isoLang2digitCode)
        {
            var assem = Assembly.GetExecutingAssembly();
            string isocode = CultureInfo.CurrentCulture.TwoLetterISOLanguageName.ToLower();
            string name = CultureInfo.CurrentCulture.Name.Replace('-', '_');
            Stream stream = null;

            if (isocode.Equals(isoLang2digitCode))
            {
                // here we have the same language as the phone standard langauge
                //
                // Because none of the bibles have a culture code, but bible names have culturecodes, we
                // will attempt to get the culture code of the phones standard language.
                // try to get the culture code if it exists
                stream = assem.GetManifestResourceStream("Sword.Properties.BibleNames_" + name + ".xml");
            }

            if (stream == null)
            {
                stream = assem.GetManifestResourceStream("Sword.Properties.BibleNames_" + isoLang2digitCode + ".xml");
                if (stream == null)
                {
                    stream = assem.GetManifestResourceStream("Sword.Properties.BibleNames_en.xml");
                }
            }
            bool isRTL = isoLang2digitCode.Equals("iw") || isoLang2digitCode.Equals("he") ||
                         isoLang2digitCode.Equals("ar") || isoLang2digitCode.Equals("ar_EG") ||
                         isoLang2digitCode.Equals("fa");
            int numberBadShortNames = 0;
            using (var reader = XmlReader.Create(stream))
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
                            int keyNumber = 0;
                            reader.MoveToFirstAttribute();
                            do
                            {
                                switch (reader.Name.ToLower())
                                {
                                    case "keynumber":
                                        keyNumber = int.Parse(reader.Value);
                                        break;
                                    case "short":
                                        shortName = isRTL ? Reverse(reader.Value) : reader.Value;
                                        break;
                                    case "full":
                                        fullName = isRTL ? Reverse(reader.Value) : reader.Value;
                                        break;
                                }
                            } while (reader.MoveToNextAttribute());
                            if (shortName.Length > 4)
                            {
                                numberBadShortNames++;
                            }
                            shortNames[keyNumber] = shortName;
                            fullNames[keyNumber] = fullName;
                        }
                    }
                }
            }
            stream.Close();
            isShortNamesExisting = numberBadShortNames < 10;
        }

        #endregion Constructors

        #region Properties

        public bool existsShortNames
        {
            get { return isShortNamesExisting; }
        }

        #endregion Properties

        #region Methods

        public string[] getAllFullNames()
        {
            return fullNames;
        }

        public string[] getAllShortNames()
        {
            return shortNames;
        }

        public string getFullName(int bookNum)
        {
            return fullNames[bookNum];
        }

        public string getShortName(int bookNum)
        {
            return shortNames[bookNum];
        }

        public string Reverse(string str)
        {
            var charArray = str.ToCharArray();
            int len = str.Length - 1;

            for (int i = 0; i < len; i++, len--)
            {
                charArray[i] ^= charArray[len];
                charArray[len] ^= charArray[i];
                charArray[i] ^= charArray[len];
            }
            return new string(charArray);
        }

        #endregion Methods
    }
}