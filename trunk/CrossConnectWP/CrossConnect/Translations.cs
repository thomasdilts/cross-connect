#region Header

// <copyright file="Translations.cs" company="Thomas Dilts">
// CrossConnect Bible and Bible Commentary Reader for CrossWire.org
// Copyright (C) 2011 Thomas Dilts
// This program is free software: you can redistribute it and/or modify
// it under the +terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see http://www.gnu.org/licenses/.
// </copyright>
// <summary>
// Email: thomas@cross-connect.se
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

    /// <summary>
    /// The translations.
    /// </summary>
    public class Translations
    {
        #region Fields

        /// <summary>
        /// The _iso language code.
        /// </summary>
        private static string _isoLanguageCode = string.Empty;

        /// <summary>
        /// The _translations.
        /// </summary>
        private static Dictionary<string, string> _translations;

        #endregion Fields

        #region Properties

        /// <summary>
        /// Gets or sets IsoLanguageCode.
        /// </summary>
        public static string IsoLanguageCode
        {
            get
            {
                string name;
                if (string.IsNullOrEmpty(_isoLanguageCode))
                {
                    Assembly assem = Assembly.GetExecutingAssembly();
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
                _isoLanguageCode = string.Empty;
                IsolatedStorageSettings.ApplicationSettings["LanguageIsoCode"] = value;
                IsolatedStorageSettings.ApplicationSettings.Save();
                _translations = new Dictionary<string, string>();
                ReadTranslationsFromFile();
            }
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// The translate.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <returns>
        /// The translate.
        /// </returns>
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

        /// <summary>
        /// The read translations from file.
        /// </summary>
        private static void ReadTranslationsFromFile()
        {
            Assembly assem = Assembly.GetExecutingAssembly();
            Stream stream = assem.GetManifestResourceStream(
                "CrossConnect.Properties.crossc_" + IsoLanguageCode + ".xml");

            if (stream != null)
            {
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
                                if (reader.Name.ToLower().Equals("string") && !string.IsNullOrEmpty(key)
                                    && !string.IsNullOrEmpty(translation))
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
        }
        private static void AddTranslationsToFilesFromGoogle(string[] newTransToAdd, string[] newKeysToAdd)
        {
            string googleapikey = "!!!Get your own key from google!!!";
            string SERVICE_URL = "https://www.googleapis.com/language/translate/v2?key={0}&source={1}&target={2}&q={3}";
            string[] allLanguages = new string[] { "es", "fr", "it", "de", "af", "ar", "az", "be", "bg", "cs", "da", "el", "et", "fa", "fi", "hi", "hr", "hu", "hy", "id", "is", "iw", "ja", "ko", "lt", "lv", "mk", "ms", "mt", "nl", "no", "pl", "pt", "ro", "ru", "sk", "sl", "sq", "sr", "sv", "sw", "th", "tl", "tr", "uk", "ur", "vi", "zh_CN", "zh_TW" };
            string workingDirectory = @"C:\Users\tds\Documents\Visual Studio 2012\Projects\ConsoleApplication3\properties\";

            

            foreach (string language in allLanguages)
            {
                string isoLang = language.Substring(0, 2);
                string isolocale = language.Length > 2 ? language.Substring(3) : "";
                string fileExt = language.Equals("zh_TW") ? "zh" : language;
                Dictionary<string, string> translations = ReadXmlFile(workingDirectory + "crossc_" + fileExt + ".xml");
                StreamWriter sw = new StreamWriter(workingDirectory + "crossc_" + fileExt + ".xml", false, Encoding.UTF8);
                sw.WriteLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
                sw.WriteLine("<translations isolang=\"" + isoLang + "\" isolocale=\"" + isolocale + "\" version=\"1.0\">");
                foreach (var entry in translations)
                {
                    sw.WriteLine("<string key=\"" + entry.Key + "\">" + entry.Value + "</string>");
                }
                for (int i = 0; i < newTransToAdd.Count(); i++)
                {

                    try
                    {
                        string uri = string.Format(SERVICE_URL, googleapikey, "en", language.Replace("_", "-"), Uri.EscapeDataString(newTransToAdd[i]));
                        WebRequest request = WebRequest.Create(new Uri(uri));
                        HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                        Stream dataStream = response.GetResponseStream();
                        StreamReader reader = new StreamReader(dataStream);
                        string responseFromServer = Regex.Match(reader.ReadToEnd(), "\"translatedText\" *: *\"(.*?)\"").Groups[1].Value.Replace("\\\\", "\\").Replace("\\\"", "\"").Replace("\\/", "/");
                        responseFromServer = HttpUtility.HtmlDecode(responseFromServer);

                        sw.WriteLine("<string key=\"" + newKeysToAdd[i] + "\">" + responseFromServer + "</string>");
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine("BAD LANG = " + language + " " + newTransToAdd[i]);
                        break;
                    }
                }

                Debug.WriteLine(language);
                sw.WriteLine("</translations>");
                sw.Close();
            }
        }

        private static Dictionary<string, string> ReadXmlFile(string filename)
        {
            Dictionary<string, string> translations = new Dictionary<string, string>();
            StreamReader stream = new StreamReader(filename);
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
            return translations;
        }

        public static string HexDump(byte[] bytes, int bytesPerLine = 16)
        {
            if (bytes == null) return "<null>";
            int bytesLength = bytes.Length;

            char[] HexChars = "0123456789ABCDEF".ToCharArray();

            int firstHexColumn =
                  8                   // 8 characters for the address
                + 3;                  // 3 spaces

            int firstCharColumn = firstHexColumn
                + bytesPerLine * 3       // - 2 digit for the hexadecimal value and 1 space
                + (bytesPerLine - 1) / 8 // - 1 extra space every 8 characters from the 9th
                + 2;                  // 2 spaces 

            int lineLength = firstCharColumn
                + bytesPerLine           // - characters to show the ascii value
                + Environment.NewLine.Length; // Carriage return and line feed (should normally be 2)

            char[] line = (new String(' ', lineLength - 2) + Environment.NewLine).ToCharArray();
            int expectedLines = (bytesLength + bytesPerLine - 1) / bytesPerLine;
            StringBuilder result = new StringBuilder(expectedLines * lineLength);

            for (int i = 0; i < bytesLength; i += bytesPerLine)
            {
                line[0] = HexChars[(i >> 28) & 0xF];
                line[1] = HexChars[(i >> 24) & 0xF];
                line[2] = HexChars[(i >> 20) & 0xF];
                line[3] = HexChars[(i >> 16) & 0xF];
                line[4] = HexChars[(i >> 12) & 0xF];
                line[5] = HexChars[(i >> 8) & 0xF];
                line[6] = HexChars[(i >> 4) & 0xF];
                line[7] = HexChars[(i >> 0) & 0xF];

                int hexColumn = firstHexColumn;
                int charColumn = firstCharColumn;

                for (int j = 0; j < bytesPerLine; j++)
                {
                    if (j > 0 && (j & 7) == 0) hexColumn++;
                    if (i + j >= bytesLength)
                    {
                        line[hexColumn] = ' ';
                        line[hexColumn + 1] = ' ';
                        line[charColumn] = ' ';
                    }
                    else
                    {
                        byte b = bytes[i + j];
                        line[hexColumn] = HexChars[(b >> 4) & 0xF];
                        line[hexColumn + 1] = HexChars[b & 0xF];
                        line[charColumn] = (b < 32 ? '·' : (char)b);
                    }
                    hexColumn += 3;
                    charColumn++;
                }
                result.Append(line);
            }
            return result.ToString();
        }

    }
    public static class HttpUtility
    {
        #region Static Fields

        private static readonly object Lock = new object();

        private static Dictionary<string, char> _entities;

        #endregion

        #region Properties

        private static Dictionary<string, char> Entities
        {
            get
            {
                lock (Lock)
                {
                    if (_entities == null)
                    {
                        InitEntities();
                    }

                    return _entities;
                }
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Decodes an HTML-encoded string and returns the decoded string.
        /// </summary>
        /// <param name="s">
        ///     The HTML string to decode.
        /// </param>
        /// <returns>
        ///     The decoded text.
        /// </returns>
        public static string HtmlDecode(string s)
        {
            if (s == null)
            {
                throw new ArgumentNullException("s");
            }

            // fix unicode escape sequences
            var rx = new Regex(@"\\[uU]([0-9A-Fa-f]{4})");
            s = rx.Replace(
                s, match => ((char)int.Parse(match.Value.Substring(2), NumberStyles.HexNumber)).ToString());

            // fix html escape sequences.
            if (s.IndexOf('&') == -1)
            {
                return s;
            }

            var entity = new StringBuilder();
            var output = new StringBuilder();
            int len = s.Length;

            // 0 -> nothing,
            // 1 -> right after '&'
            // 2 -> between '&' and ';' but no '#'
            // 3 -> '#' found after '&' and getting numbers
            int state = 0;
            int number = 0;
            bool haveTrailingDigits = false;

            for (int i = 0; i < len; i++)
            {
                char c = s[i];
                if (state == 0)
                {
                    if (c == '&')
                    {
                        entity.Append(c);
                        state = 1;
                    }
                    else
                    {
                        output.Append(c);
                    }

                    continue;
                }

                if (c == '&')
                {
                    state = 1;
                    if (haveTrailingDigits)
                    {
                        entity.Append(number.ToString(CultureInfo.InvariantCulture));
                        haveTrailingDigits = false;
                    }

                    output.Append(entity);
                    entity.Length = 0;
                    entity.Append('&');
                    continue;
                }

                if (state == 1)
                {
                    if (c == ';')
                    {
                        state = 0;
                        output.Append(entity);
                        output.Append(c);
                        entity.Length = 0;
                    }
                    else
                    {
                        number = 0;
                        state = c != '#' ? 2 : 3;
                        entity.Append(c);
                    }
                }
                else if (state == 2)
                {
                    entity.Append(c);
                    if (c == ';')
                    {
                        string key = entity.ToString();
                        if (key.Length > 1 && Entities.ContainsKey(key.Substring(1, key.Length - 2)))
                        {
                            key = Entities[key.Substring(1, key.Length - 2)].ToString();
                        }

                        output.Append(key);
                        state = 0;
                        entity.Length = 0;
                    }
                }
                else if (state == 3)
                {
                    if (c == ';')
                    {
                        if (number > 65535)
                        {
                            output.Append("&#");
                            output.Append(number.ToString(CultureInfo.InvariantCulture));
                            output.Append(";");
                        }
                        else
                        {
                            output.Append((char)number);
                        }

                        state = 0;
                        entity.Length = 0;
                        haveTrailingDigits = false;
                    }
                    else if (char.IsDigit(c))
                    {
                        number = number * 10 + (c - '0');
                        haveTrailingDigits = true;
                    }
                    else
                    {
                        state = 2;
                        if (haveTrailingDigits)
                        {
                            entity.Append(number.ToString(CultureInfo.InvariantCulture));
                            haveTrailingDigits = false;
                        }

                        entity.Append(c);
                    }
                }
            }

            if (entity.Length > 0)
            {
                output.Append(entity);
            }
            else if (haveTrailingDigits)
            {
                output.Append(number.ToString(CultureInfo.InvariantCulture));
            }

            return output.ToString();
        }
         
         
         
         */

        #endregion Other
    }
}