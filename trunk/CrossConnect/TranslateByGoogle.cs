#region Header

// <copyright file="TranslateByGoogle.cs" company="Thomas Dilts">
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
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Net;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Xml;

    public class TranslateByGoogle
    {
        #region Fields

        // Developer application ID. Obtain a free key from https://code.google.com/apis/console/.
        // CrossConnect REST service URL.
        private const string CrossconnectUrl = 
            "http://www.chaniel.se/crossconnect/translate/gettranslatekey.php?uuid={0}&language={1}";

        // Google Translate REST service URL.
        private const string ServiceUrl = 
            "https://www.googleapis.com/language/translate/v2?key={0}&source={1}&target={2}&q={3}";

        private GoogleTranslatedTextReturnEvent _endUserFinalTranslationEvent;
        private string _googleTranslateFromLanguage;

        //private const string ServicePostUrl = "https://www.googleapis.com/language/translate/v2";
        //private const string ServicePostData = "?key={0}&source={1}&target={2}&q={3}";
        //private const string ServicePostDataPartial =
        //    "https://www.googleapis.com/language/translate/v2?key={0}&source={1}&target={2}";
        private WebClient _proxy;
        private string _textToGooleTranslate;

        #endregion Fields

        #region Delegates

        public delegate void GoogleTranslatedTextReturnEvent(string translatedText, bool isError);

        #endregion Delegates

        #region Methods

        public void GetGoogleTranslationAsync(string textToGooleTranslate, string googleTranslateFromLanguage,
            GoogleTranslatedTextReturnEvent endUserFinalTranslationEvent)
        {
            _endUserFinalTranslationEvent += endUserFinalTranslationEvent;
            _textToGooleTranslate = textToGooleTranslate;
            _googleTranslateFromLanguage = googleTranslateFromLanguage;
            try
            {
                _proxy = new WebClient();
                _proxy.OpenReadCompleted += OpenReadFromCrossConnectCompleted;

                string crossconnectUrl = string.Format(CrossconnectUrl, App.DisplaySettings.UserUniqueGuuid,
                                                       Translations.IsoLanguageCode);
                _proxy.OpenReadAsync(new Uri(crossconnectUrl));
            }
            catch (Exception eee)
            {
                ReplyToTranslateRequestor(
                    Translations.Translate("An error occurred trying to connect to the network. Try again later.") +
                    "; " + eee.Message, true);
            }
        }

        //private void UploadStringFromGoogleCompleted(object sender, UploadStringCompletedEventArgs e)
        //{
        //    try
        //    {
        //        //must un-jsonize and un-htmlize
        //        string unEscapedText =
        //            HttpUtility.HtmlDecode(
        //                jsonUnescape(Regex.Match(e.Result, "\"translatedText\" *: *\"(.*?)\"").Groups[1].Value));
        //        replyToTranslateRequestor(unEscapedText, false);
        //    }
        //    catch (Exception ee2)
        //    {
        //        replyToTranslateRequestor(
        //            Translations.Translate("An error occurred trying to connect to the network. Try again later.") +
        //            "; " + ee2.Message, true);
        //    }
        //}
        private void DownloadStringFromGoogleCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            try
            {
                //must un-jsonize and un-htmlize
                string unEscapedText =
                    HttpUtility.HtmlDecode(
                        jsonUnescape(Regex.Match(e.Result, "\"translatedText\" *: *\"(.*?)\"").Groups[1].Value));
                ReplyToTranslateRequestor(unEscapedText, false);
            }
            catch (Exception ee2)
            {
                ReplyToTranslateRequestor(
                    Translations.Translate("An error occurred trying to connect to the network. Try again later.") +
                    "; " + ee2.Message, true);
            }
        }

        private string jsonUnescape(string toUnJson)
        {
            return toUnJson.Replace("\\\\", "\\").Replace("\\\"", "\"").Replace("\\/", "/");
        }

        private void OpenReadFromCrossConnectCompleted(object sender, OpenReadCompletedEventArgs e)
        {
            try
            {
                // for debug
                //byte[] buffer = new byte[e.Result.Length];
                //e.Result.Read(buffer, 0, (int)e.Result.Length);
                //System.Diagnostics.Debug.WriteLine("RawFile: " + System.Text.UTF8Encoding.UTF8.GetString(buffer, 0, (int)e.Result.Length));
                string msgFromServer = "";
                string googleApiKey = "";
                int maxtranssize = 0;
                using (var reader = XmlReader.Create(e.Result))
                {
                    string xmltxt = "";
                    // Parse the file and get each of the nodes.
                    while (reader.Read())
                    {
                        switch (reader.NodeType)
                        {
                            case XmlNodeType.Element:
                                if (reader.Name.ToLower().Equals("googleapi") && reader.HasAttributes)
                                {
                                    reader.MoveToFirstAttribute();
                                    do
                                    {
                                        switch (reader.Name.ToLower())
                                        {
                                            case "key":
                                                googleApiKey = reader.Value;
                                                break;
                                            case "maxtranssize":
                                                int.TryParse(reader.Value, out maxtranssize);
                                                break;
                                        }
                                    } while (reader.MoveToNextAttribute());
                                }
                                else if (reader.Name.ToLower().Equals("message"))
                                {
                                    xmltxt = string.Empty;
                                }
                                break;
                            case XmlNodeType.EndElement:
                                if (reader.Name.ToLower().Equals("message"))
                                {
                                    msgFromServer = xmltxt;
                                    xmltxt = string.Empty;
                                }
                                break;
                            case XmlNodeType.Text:
                                xmltxt += reader.Value;
                                break;
                        }
                    }
                }
                if (string.IsNullOrEmpty(googleApiKey) || maxtranssize == 0)
                {
                    if (string.IsNullOrEmpty(msgFromServer))
                    {
                        ReplyToTranslateRequestor(
                            Translations.Translate(
                                "An error occurred trying to connect to the network. Try again later."), true);
                    }
                    else
                    {
                        ReplyToTranslateRequestor(msgFromServer, true);
                    }
                }
                else
                {
                    _proxy = new WebClient();
                    _proxy.DownloadStringCompleted += DownloadStringFromGoogleCompleted;
                    string toTranslate = Uri.EscapeDataString(_textToGooleTranslate);
                    string googleTranslateUrl = string.Format(ServiceUrl, googleApiKey, _googleTranslateFromLanguage,
                                                              Translations.IsoLanguageCode, toTranslate);
                    googleTranslateUrl = googleTranslateUrl.Substring(0,
                                                                      googleTranslateUrl.Length > maxtranssize
                                                                          ? maxtranssize
                                                                          : googleTranslateUrl.Length);
                    _proxy.DownloadStringAsync(new Uri(googleTranslateUrl));
                    // The following were attempts to do a POST instead of GET which would make the maxlimit go to 5K
                    /*  THIS WORKS
                    _proxy.Headers["X-HTTP-Method-Override"]="GET";
                    _proxy.UploadStringCompleted += new UploadStringCompletedEventHandler(UploadStringFromGoogleCompleted);
                    _proxy.UploadStringAsync(new Uri(googleTranslateUrl), "POST", string.Format(SERVICE_POST_DATA, googleApiKey, googleTranslateFromLanguage, Translations.isoLanguageCode, toTranslate.Substring(0, toTranslate.Length > maxtranssize ? maxtranssize : toTranslate.Length)));
                     */
                    /*  does not work
                    _proxy.Headers["X-HTTP-Method-Override"] = "GET";
                    _proxy.UploadStringCompleted += new UploadStringCompletedEventHandler(UploadStringFromGoogleCompleted);
                    _proxy.UploadStringAsync(new Uri(string.Format(SERVICE_POST_DATA_PARTIAL,googleApiKey, googleTranslateFromLanguage, Translations.isoLanguageCode)), "POST", string.Format(SERVICE_POST_DATA, googleApiKey, googleTranslateFromLanguage, Translations.isoLanguageCode, toTranslate.Substring(0, toTranslate.Length > maxtranssize ? maxtranssize : toTranslate.Length)));
                    */
                    /* does not work either
                    _proxy.Headers["X-HTTP-Method-Override"] = "GET";
                    _proxy.Headers["Content-Type"] = "application/x-www-form-urlencoded";
                    _proxy.UploadStringCompleted += new UploadStringCompletedEventHandler(UploadStringFromGoogleCompleted);
                    _proxy.UploadStringAsync(new Uri(SERVICE_POST_URL), "POST", string.Format(SERVICE_POST_DATA, googleApiKey, googleTranslateFromLanguage, Translations.isoLanguageCode, toTranslate.Substring(0, toTranslate.Length > maxtranssize ? maxtranssize : toTranslate.Length)));
                    */
                }
            }
            catch (Exception exp)
            {
                Logger.Fail(e.ToString());
                ReplyToTranslateRequestor(
                    Translations.Translate("An error occurred trying to connect to the network. Try again later.") +
                    "; " + exp.Message, true);
            }
        }

        private void ReplyToTranslateRequestor(string msg, bool isFail)
        {
            if (_endUserFinalTranslationEvent != null)
            {
                _endUserFinalTranslationEvent(msg, isFail);
            }
        }

        #endregion Methods

        #region Nested Types

        /// <summary>
        ///   This is only used for the DecodeHtml function.
        /// </summary>
        private static class HttpUtility
        {
            #region Fields

            private static readonly object Lock = new object();

            private static Dictionary<string, char> _entities;

            #endregion Fields

            #region Properties

            private static Dictionary<string, char> Entities
            {
                get
                {
                    lock (Lock)
                    {
                        if (_entities == null)
                            InitEntities();

                        return _entities;
                    }
                }
            }

            #endregion Properties

            #region Methods

            /// <summary>
            ///   Decodes an HTML-encoded string and returns the decoded string.
            /// </summary>
            /// <param name = "s">The HTML string to decode. </param>
            /// <returns>The decoded text.</returns>
            public static string HtmlDecode(string s)
            {
                if (s == null)
                    throw new ArgumentNullException("s");

                //fix unicode escape sequences
                var rx = new Regex(@"\\[uU]([0-9A-Fa-f]{4})");
                s = rx.Replace(s,
                               match => ((char) Int32.Parse(match.Value.Substring(2), NumberStyles.HexNumber)).
                                            ToString());

                //fix html escape sequences.
                if (s.IndexOf('&') == -1)
                    return s;

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

                        output.Append(entity.ToString());
                        entity.Length = 0;
                        entity.Append('&');
                        continue;
                    }

                    if (state == 1)
                    {
                        if (c == ';')
                        {
                            state = 0;
                            output.Append(entity.ToString());
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
                                key = Entities[key.Substring(1, key.Length - 2)].ToString();

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
                                output.Append((char) number);
                            }
                            state = 0;
                            entity.Length = 0;
                            haveTrailingDigits = false;
                        }
                        else if (Char.IsDigit(c))
                        {
                            number = number*10 + (c - '0');
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
                    output.Append(entity.ToString());
                }
                else if (haveTrailingDigits)
                {
                    output.Append(number.ToString(CultureInfo.InvariantCulture));
                }
                return output.ToString();
            }

            private static void InitEntities()
            {
                // Build the hash table of HTML entity references.  This list comes
                // from the HTML 4.01 W3C recommendation.
                _entities = new Dictionary<string, char>
                               {
                                   {"nbsp", '\u00A0'},
                                   {"iexcl", '\u00A1'},
                                   {"cent", '\u00A2'},
                                   {"pound", '\u00A3'},
                                   {"curren", '\u00A4'},
                                   {"yen", '\u00A5'},
                                   {"brvbar", '\u00A6'},
                                   {"sect", '\u00A7'},
                                   {"uml", '\u00A8'},
                                   {"copy", '\u00A9'},
                                   {"ordf", '\u00AA'},
                                   {"laquo", '\u00AB'},
                                   {"not", '\u00AC'},
                                   {"shy", '\u00AD'},
                                   {"reg", '\u00AE'},
                                   {"macr", '\u00AF'},
                                   {"deg", '\u00B0'},
                                   {"plusmn", '\u00B1'},
                                   {"sup2", '\u00B2'},
                                   {"sup3", '\u00B3'},
                                   {"acute", '\u00B4'},
                                   {"micro", '\u00B5'},
                                   {"para", '\u00B6'},
                                   {"middot", '\u00B7'},
                                   {"cedil", '\u00B8'},
                                   {"sup1", '\u00B9'},
                                   {"ordm", '\u00BA'},
                                   {"raquo", '\u00BB'},
                                   {"frac14", '\u00BC'},
                                   {"frac12", '\u00BD'},
                                   {"frac34", '\u00BE'},
                                   {"iquest", '\u00BF'},
                                   {"Agrave", '\u00C0'},
                                   {"Aacute", '\u00C1'},
                                   {"Acirc", '\u00C2'},
                                   {"Atilde", '\u00C3'},
                                   {"Auml", '\u00C4'},
                                   {"Aring", '\u00C5'},
                                   {"AElig", '\u00C6'},
                                   {"Ccedil", '\u00C7'},
                                   {"Egrave", '\u00C8'},
                                   {"Eacute", '\u00C9'},
                                   {"Ecirc", '\u00CA'},
                                   {"Euml", '\u00CB'},
                                   {"Igrave", '\u00CC'},
                                   {"Iacute", '\u00CD'},
                                   {"Icirc", '\u00CE'},
                                   {"Iuml", '\u00CF'},
                                   {"ETH", '\u00D0'},
                                   {"Ntilde", '\u00D1'},
                                   {"Ograve", '\u00D2'},
                                   {"Oacute", '\u00D3'},
                                   {"Ocirc", '\u00D4'},
                                   {"Otilde", '\u00D5'},
                                   {"Ouml", '\u00D6'},
                                   {"times", '\u00D7'},
                                   {"Oslash", '\u00D8'},
                                   {"Ugrave", '\u00D9'},
                                   {"Uacute", '\u00DA'},
                                   {"Ucirc", '\u00DB'},
                                   {"Uuml", '\u00DC'},
                                   {"Yacute", '\u00DD'},
                                   {"THORN", '\u00DE'},
                                   {"szlig", '\u00DF'},
                                   {"agrave", '\u00E0'},
                                   {"aacute", '\u00E1'},
                                   {"acirc", '\u00E2'},
                                   {"atilde", '\u00E3'},
                                   {"auml", '\u00E4'},
                                   {"aring", '\u00E5'},
                                   {"aelig", '\u00E6'},
                                   {"ccedil", '\u00E7'},
                                   {"egrave", '\u00E8'},
                                   {"eacute", '\u00E9'},
                                   {"ecirc", '\u00EA'},
                                   {"euml", '\u00EB'},
                                   {"igrave", '\u00EC'},
                                   {"iacute", '\u00ED'},
                                   {"icirc", '\u00EE'},
                                   {"iuml", '\u00EF'},
                                   {"eth", '\u00F0'},
                                   {"ntilde", '\u00F1'},
                                   {"ograve", '\u00F2'},
                                   {"oacute", '\u00F3'},
                                   {"ocirc", '\u00F4'},
                                   {"otilde", '\u00F5'},
                                   {"ouml", '\u00F6'},
                                   {"divide", '\u00F7'},
                                   {"oslash", '\u00F8'},
                                   {"ugrave", '\u00F9'},
                                   {"uacute", '\u00FA'},
                                   {"ucirc", '\u00FB'},
                                   {"uuml", '\u00FC'},
                                   {"yacute", '\u00FD'},
                                   {"thorn", '\u00FE'},
                                   {"yuml", '\u00FF'},
                                   {"fnof", '\u0192'},
                                   {"Alpha", '\u0391'},
                                   {"Beta", '\u0392'},
                                   {"Gamma", '\u0393'},
                                   {"Delta", '\u0394'},
                                   {"Epsilon", '\u0395'},
                                   {"Zeta", '\u0396'},
                                   {"Eta", '\u0397'},
                                   {"Theta", '\u0398'},
                                   {"Iota", '\u0399'},
                                   {"Kappa", '\u039A'},
                                   {"Lambda", '\u039B'},
                                   {"Mu", '\u039C'},
                                   {"Nu", '\u039D'},
                                   {"Xi", '\u039E'},
                                   {"Omicron", '\u039F'},
                                   {"Pi", '\u03A0'},
                                   {"Rho", '\u03A1'},
                                   {"Sigma", '\u03A3'},
                                   {"Tau", '\u03A4'},
                                   {"Upsilon", '\u03A5'},
                                   {"Phi", '\u03A6'},
                                   {"Chi", '\u03A7'},
                                   {"Psi", '\u03A8'},
                                   {"Omega", '\u03A9'},
                                   {"alpha", '\u03B1'},
                                   {"beta", '\u03B2'},
                                   {"gamma", '\u03B3'},
                                   {"delta", '\u03B4'},
                                   {"epsilon", '\u03B5'},
                                   {"zeta", '\u03B6'},
                                   {"eta", '\u03B7'},
                                   {"theta", '\u03B8'},
                                   {"iota", '\u03B9'},
                                   {"kappa", '\u03BA'},
                                   {"lambda", '\u03BB'},
                                   {"mu", '\u03BC'},
                                   {"nu", '\u03BD'},
                                   {"xi", '\u03BE'},
                                   {"omicron", '\u03BF'},
                                   {"pi", '\u03C0'},
                                   {"rho", '\u03C1'},
                                   {"sigmaf", '\u03C2'},
                                   {"sigma", '\u03C3'},
                                   {"tau", '\u03C4'},
                                   {"upsilon", '\u03C5'},
                                   {"phi", '\u03C6'},
                                   {"chi", '\u03C7'},
                                   {"psi", '\u03C8'},
                                   {"omega", '\u03C9'},
                                   {"thetasym", '\u03D1'},
                                   {"upsih", '\u03D2'},
                                   {"piv", '\u03D6'},
                                   {"bull", '\u2022'},
                                   {"hellip", '\u2026'},
                                   {"prime", '\u2032'},
                                   {"Prime", '\u2033'},
                                   {"oline", '\u203E'},
                                   {"frasl", '\u2044'},
                                   {"weierp", '\u2118'},
                                   {"image", '\u2111'},
                                   {"real", '\u211C'},
                                   {"trade", '\u2122'},
                                   {"alefsym", '\u2135'},
                                   {"larr", '\u2190'},
                                   {"uarr", '\u2191'},
                                   {"rarr", '\u2192'},
                                   {"darr", '\u2193'},
                                   {"harr", '\u2194'},
                                   {"crarr", '\u21B5'},
                                   {"lArr", '\u21D0'},
                                   {"uArr", '\u21D1'},
                                   {"rArr", '\u21D2'},
                                   {"dArr", '\u21D3'},
                                   {"hArr", '\u21D4'},
                                   {"forall", '\u2200'},
                                   {"part", '\u2202'},
                                   {"exist", '\u2203'},
                                   {"empty", '\u2205'},
                                   {"nabla", '\u2207'},
                                   {"isin", '\u2208'},
                                   {"notin", '\u2209'},
                                   {"ni", '\u220B'},
                                   {"prod", '\u220F'},
                                   {"sum", '\u2211'},
                                   {"minus", '\u2212'},
                                   {"lowast", '\u2217'},
                                   {"radic", '\u221A'},
                                   {"prop", '\u221D'},
                                   {"infin", '\u221E'},
                                   {"ang", '\u2220'},
                                   {"and", '\u2227'},
                                   {"or", '\u2228'},
                                   {"cap", '\u2229'},
                                   {"cup", '\u222A'},
                                   {"int", '\u222B'},
                                   {"there4", '\u2234'},
                                   {"sim", '\u223C'},
                                   {"cong", '\u2245'},
                                   {"asymp", '\u2248'},
                                   {"ne", '\u2260'},
                                   {"equiv", '\u2261'},
                                   {"le", '\u2264'},
                                   {"ge", '\u2265'},
                                   {"sub", '\u2282'},
                                   {"sup", '\u2283'},
                                   {"nsub", '\u2284'},
                                   {"sube", '\u2286'},
                                   {"supe", '\u2287'},
                                   {"oplus", '\u2295'},
                                   {"otimes", '\u2297'},
                                   {"perp", '\u22A5'},
                                   {"sdot", '\u22C5'},
                                   {"lceil", '\u2308'},
                                   {"rceil", '\u2309'},
                                   {"lfloor", '\u230A'},
                                   {"rfloor", '\u230B'},
                                   {"lang", '\u2329'},
                                   {"rang", '\u232A'},
                                   {"loz", '\u25CA'},
                                   {"spades", '\u2660'},
                                   {"clubs", '\u2663'},
                                   {"hearts", '\u2665'},
                                   {"diams", '\u2666'},
                                   {"quot", '\u0022'},
                                   {"amp", '\u0026'},
                                   {"lt", '\u003C'},
                                   {"gt", '\u003E'},
                                   {"OElig", '\u0152'},
                                   {"oelig", '\u0153'},
                                   {"Scaron", '\u0160'},
                                   {"scaron", '\u0161'},
                                   {"Yuml", '\u0178'},
                                   {"circ", '\u02C6'},
                                   {"tilde", '\u02DC'},
                                   {"ensp", '\u2002'},
                                   {"emsp", '\u2003'},
                                   {"thinsp", '\u2009'},
                                   {"zwnj", '\u200C'},
                                   {"zwj", '\u200D'},
                                   {"lrm", '\u200E'},
                                   {"rlm", '\u200F'},
                                   {"ndash", '\u2013'},
                                   {"mdash", '\u2014'},
                                   {"lsquo", '\u2018'},
                                   {"rsquo", '\u2019'},
                                   {"sbquo", '\u201A'},
                                   {"ldquo", '\u201C'},
                                   {"rdquo", '\u201D'},
                                   {"bdquo", '\u201E'},
                                   {"dagger", '\u2020'},
                                   {"Dagger", '\u2021'},
                                   {"permil", '\u2030'},
                                   {"lsaquo", '\u2039'},
                                   {"rsaquo", '\u203A'},
                                   {"euro", '\u20AC'}
                               };
            }

            #endregion Methods
        }

        #endregion Nested Types

        #region Other

        // Http request / response manager.

        #endregion Other
    }
}