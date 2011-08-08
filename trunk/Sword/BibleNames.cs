/// <summary>
/// Distribution License:
/// CrossConnect is free software; you can redistribute it and/or modify it under
/// the terms of the GNU General Public License, version 3 as published by
/// the Free Software Foundation. This program is distributed in the hope
/// that it will be useful, but WITHOUT ANY WARRANTY; without even the
/// implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
/// See the GNU General Public License for more details.
///
/// The License is available on the internet at:
///       http://www.gnu.org/copyleft/gpl.html
/// or by writing to:
///      Free Software Foundation, Inc.
///      59 Temple Place - Suite 330
///      Boston, MA 02111-1307, USA
/// </summary>
/// <copyright file="BibleNames.cs" company="Thomas Dilts">
///     Thomas Dilts. All rights reserved.
/// </copyright>
/// <author>Thomas Dilts</author>
namespace SwordBackend
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.IO.IsolatedStorage;
    using System.Net;
    using System.Reflection;
    using System.Xml;

    public class BibleNames
    {
        #region Fields

        private string[] fullNames = new string[BibleZtextReader.BOOKS_IN_BIBLE];
        private bool isShortNamesExisting = true;
        private string[] shortNames = new string[BibleZtextReader.BOOKS_IN_BIBLE];

        #endregion Fields

        #region Constructors

        public BibleNames(string isoLang2digitCode)
        {
            Assembly assem = Assembly.GetExecutingAssembly();
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

            if(stream==null)
            {
                stream = assem.GetManifestResourceStream("Sword.Properties.BibleNames_" + isoLang2digitCode + ".xml");
                if(stream==null)
                {
                    stream = assem.GetManifestResourceStream("Sword.Properties.BibleNames_en.xml");
                }
            }
            bool isRTL = isoLang2digitCode.Equals("he") || isoLang2digitCode.Equals("ar") || isoLang2digitCode.Equals("ar_EG") || isoLang2digitCode.Equals("fa");
            int numberBadShortNames = 0;
            using (XmlReader reader = XmlReader.Create(stream))
            {
                // Parse the file and get each of the nodes.
                while (reader.Read())
                {
                    if (reader.NodeType==XmlNodeType.Element)
                    {
                        if (reader.Name.ToLower().Equals("book") && reader.HasAttributes)
                        {
                            string shortName="";
                            string fullName="";
                            int keyNumber=0;
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
                            shortNames[keyNumber]=shortName;
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
            get
            {
                return isShortNamesExisting;
            }
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

        #endregion Methods
    }
}