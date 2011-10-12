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
/// <copyright file="Translations.cs" company="Thomas Dilts">
///     Thomas Dilts. All rights reserved.
/// </copyright>
/// <author>Thomas Dilts</author>
namespace CrossConnect
{
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.IO.IsolatedStorage;
    using System.Reflection;
    using System.Xml;

    public class Translations
    {
        #region Fields

        private static Dictionary<string, string> translations = null;
        private static string _isoLanguageCode = "";

        #endregion Fields

        #region Properties

        public static string isoLanguageCode
        {
            
            get
            {
                string name="";
                if (string.IsNullOrEmpty(_isoLanguageCode))
                {
                    Assembly assem = Assembly.GetExecutingAssembly();
                    string isocode = CultureInfo.CurrentCulture.TwoLetterISOLanguageName.ToLower();
                    name = CultureInfo.CurrentCulture.Name.Replace('-', '_').ToLower();
                    if (IsolatedStorageSettings.ApplicationSettings.TryGetValue<string>("LanguageIsoCode", out name))
                    {
                        if (string.IsNullOrEmpty(name))
                        {
                            name = CultureInfo.CurrentCulture.Name.Replace('-', '_').ToLower();
                        }
                    }
                    Stream stream = null;
                    stream = assem.GetManifestResourceStream("CrossConnect.Properties.crossc_" + name + ".xml");

                    if (stream == null)
                    {
                        name = isocode;
                        stream = assem.GetManifestResourceStream("CrossConnect.Properties.crossc_" + name + ".xml");
                        if (stream == null)
                        {
                            name = "en";
                        }
                    }
                    if (stream != null)
                    {
                        stream.Close();
                    }
                    _isoLanguageCode = name;
                }
                else
                {
                    name = _isoLanguageCode;
                }
                return name;
            }
            set
            {
                _isoLanguageCode = value;
                IsolatedStorageSettings.ApplicationSettings["LanguageIsoCode"] = value;
                IsolatedStorageSettings.ApplicationSettings.Save();
                translations = new Dictionary<string, string>();
                readTranslationsFromFile();
            }
        }

        #endregion Properties


        public static string translate(string key)
        {
            if (translations == null)
            {
                translations = new Dictionary<string, string>();
                readTranslationsFromFile();
            }

            string translation = string.Empty;
            if (!translations.TryGetValue(key, out translation))
            {
                translation = key;
            }

            return translation;
        }

        private static void readTranslationsFromFile()
        {
            Assembly assem = Assembly.GetExecutingAssembly();
            Stream stream = assem.GetManifestResourceStream("CrossConnect.Properties.crossc_" + isoLanguageCode + ".xml");

            using (XmlReader reader = XmlReader.Create(stream))
            {
                string key = string.Empty;
                string translation = string.Empty;
                // Parse the file and get each of the nodes.
                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            if (reader.Name.ToLower().Equals("string") && reader.HasAttributes)
                            {
                                translation = string.Empty;
                                key = string.Empty;
                                reader.MoveToFirstAttribute();
                                do
                                {
                                    switch (reader.Name.ToLower())
                                    {
                                        case "key":
                                            key = reader.Value;
                                            break;
                                    }
                                }
                                while (reader.MoveToNextAttribute());
                            }
                            break;
                        case XmlNodeType.EndElement:
                            if (reader.Name.ToLower().Equals("string") && !string.IsNullOrEmpty(key) && !string.IsNullOrEmpty(translation))
                            {
                                translations[key] = translation;
                            }
                            break;
                        case XmlNodeType.Text:
                            translation += reader.Value;
                            break;
                    }
                }
            }

            stream.Close();
        }

    }
}