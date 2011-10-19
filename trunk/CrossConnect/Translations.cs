#region Header

// <copyright file="Translations.cs" company="Thomas Dilts">
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

namespace CrossConnect
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.IO.IsolatedStorage;
    using System.Reflection;
    using System.Xml;

    public class Translations
    {
        #region Fields

        private static string _isoLanguageCode = "";
        private static Dictionary<string, string> _translations;

        #endregion Fields

        #region Properties

        public static string IsoLanguageCode
        {
            get
            {
                string name;
                if (string.IsNullOrEmpty(_isoLanguageCode))
                {
                    var assem = Assembly.GetExecutingAssembly();
                    string isocode = CultureInfo.CurrentCulture.TwoLetterISOLanguageName.ToLower();
                    if (IsolatedStorageSettings.ApplicationSettings.TryGetValue("LanguageIsoCode", out name))
                    {
                        if (string.IsNullOrEmpty(name))
                        {
                            name = CultureInfo.CurrentCulture.Name.Replace('-', '_').ToLower();
                        }
                    }
                    Stream stream = assem.GetManifestResourceStream("CrossConnect.Properties.crossc_" + name + ".xml");

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
                _isoLanguageCode = "";
                IsolatedStorageSettings.ApplicationSettings["LanguageIsoCode"] = value;
                IsolatedStorageSettings.ApplicationSettings.Save();
                _translations = new Dictionary<string, string>();
                ReadTranslationsFromFile();
            }
        }

        #endregion Properties

        #region Methods

        public static string Translate(string key)
        {
            if (_translations == null)
            {
                _translations = new Dictionary<string, string>();
                ReadTranslationsFromFile();
            }

            string translation;
            if (!_translations.TryGetValue(key, out translation))
            {
                Debug.WriteLine("<string key=\"" + key + "\">" + key + "</string>");
                translation = key;
                _translations[key] = key;
            }

            return translation;
        }

        private static void ReadTranslationsFromFile()
        {
            var assem = Assembly.GetExecutingAssembly();
            var stream = assem.GetManifestResourceStream("CrossConnect.Properties.crossc_" + IsoLanguageCode + ".xml");

            if (stream != null)
            {
                using (var reader = XmlReader.Create(stream))
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
                                    } while (reader.MoveToNextAttribute());
                                }
                                break;
                            case XmlNodeType.EndElement:
                                if (reader.Name.ToLower().Equals("string") && !string.IsNullOrEmpty(key) &&
                                    !string.IsNullOrEmpty(translation))
                                {
                                    _translations[key] = translation;
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

        #endregion Methods

        #region Other

        /*  This next function must be run in .NET 4.0.  NOT IN SILVERLIGHT or WP7
        private static void zCreateAllTranslationFilesFromGoogle()
        {
            string googleapikey = "!!!Get your own key from google!!!";
            string SERVICE_URL = "https://www.googleapis.com/language/translate/v2?key={0}&source={1}&target={2}&q={3}";
            string[] allLanguages = new string[] { "es", "fr", "it", "de", "af", "ar", "az", "be", "bg", "cs", "da", "el", "et", "fa", "fi", "hi", "hr", "hu", "hy", "id", "is", "iw", "ja", "ko", "lt", "lv", "mk", "ms", "mt", "nl", "no", "pl", "pt", "ro", "ru", "sk", "sl", "sq", "sr", "sv", "sw", "th", "tl", "tr", "uk", "ur", "vi", "zh_CN", "zh_TW" };
            string workingDirectory = @"C:\Users\tds.ES\Desktop\CrossConnect\CrossConnect\Properties\";

            Dictionary<string, string> translations = new Dictionary<string, string>();
            StreamReader stream = new StreamReader(workingDirectory + "crossc_en.xml");
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

            foreach (string language in allLanguages)
            {
                string isoLang = language.Substring(0, 2);
                string isolocale = language.Length > 2 ? language.Substring(3) : "";
                string fileExt = language.Equals("zh_TW") ? "zh" : language;
                StreamWriter sw = new StreamWriter(workingDirectory + "crossc_" + fileExt + ".xml", false, Encoding.UTF8);
                sw.WriteLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
                sw.WriteLine("<translations isolang=\"" + isoLang + "\" isolocale=\"" + isolocale + "\" version=\"1.0\">");
                foreach (var entry in translations)
                {
                    try
                    {
                        string uri = string.Format(SERVICE_URL, googleapikey, "en", language.Replace("_", "-"), Uri.EscapeDataString(entry.Value));
                        WebRequest request = WebRequest.Create(new Uri(uri));
                        HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                        Stream dataStream = response.GetResponseStream();
                        StreamReader reader = new StreamReader(dataStream);
                        string responseFromServer = Regex.Match(reader.ReadToEnd(), "\"translatedText\" *: *\"(.*?)\"").Groups[1].Value.Replace("\\\\", "\\").Replace("\\\"", "\"").Replace("\\/", "/");
                        responseFromServer = HttpUtility.HtmlDecode(responseFromServer);

                        sw.WriteLine("<string key=\"" + entry.Key + "\">" + responseFromServer + "</string>");
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine("BAD LANG = " + language + " " + entry.Key);
                        break;
                    }
                }
                Debug.WriteLine(language);
                sw.WriteLine("</translations>");
                sw.Close();
            }
        }*/

        #endregion Other
    }
}