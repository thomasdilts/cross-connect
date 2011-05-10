using System;
using System.Net;
using System.IO;
using System.Reflection;
using System.Globalization;
using System.IO.IsolatedStorage;
using System.Xml;
using System.Collections.Generic;
///
/// <summary> Distribution License:
/// JSword is free software; you can redistribute it and/or modify it under
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
///
/// Copyright: 2011
///     The copyright to this program is held by Thomas Dilts
///  
namespace CrossConnect
{
    public class Translations
    {
        private static Dictionary<string,string> translations=null;

        public static string translate(string key)
        {
            if(translations==null)
            {
                translations = new Dictionary<string, string>();
                readTranslationsFromFile();
            }

            string translation="";
            if(!translations.TryGetValue(key,out translation))
            {
                translation=key;
            }
            return translation;
        }

        private static void readTranslationsFromFile()
        {
            Assembly assem = Assembly.GetExecutingAssembly();
            string isocode = CultureInfo.CurrentCulture.TwoLetterISOLanguageName.ToLower();
            string name = CultureInfo.CurrentCulture.Name.Replace('-', '_').ToLower();
            Stream stream = null;
            stream = assem.GetManifestResourceStream("CrossConnect.Properties.crossc_" + name + ".xml");

            if (stream==null)
            {
                stream = assem.GetManifestResourceStream("CrossConnect.Properties.crossc_" + isocode + ".xml");
                if (stream == null)
                {
                    stream = assem.GetManifestResourceStream("CrossConnect.Properties.crossc_en.xml");
                }
            }

            using (XmlReader reader = XmlReader.Create(stream))
            {
                string key = "";
                string translation = "";
                // Parse the file and get each of the nodes.
                while (reader.Read())
                {
                    switch(reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            if (reader.Name.ToLower().Equals("string") && reader.HasAttributes)
                            {
                                translation = "";
                                key = "";
                                reader.MoveToFirstAttribute();
                                do
                                {
                                    switch (reader.Name.ToLower())
                                    {
                                        case "key":
                                            key = reader.Value;
                                            break;
                                    }
                                } while (reader.MoveToNextAttribute());
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
