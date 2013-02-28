﻿#region Header

// <copyright file="Theme.cs" company="Thomas Dilts">
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
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Reflection;
    using System.Text;
    using System.Windows;
    using System.Windows.Media;
    using System.Xml;

    /// <summary>
    /// The theme.
    /// </summary>
    public class Theme
    {
        #region Fields

        /// <summary>
        /// The font families.
        /// </summary>
        public static readonly Dictionary<string, string> FontFamilies = new Dictionary<string, string> {
                { "Andale Mono", "font-family: 'andale mono','monotype.com',monaco,'courier new',courier,monospace;" },
                { "Arial Black", "font-family: 'Arial Black',helvetica,sans-serif;" },
                { "Arial", "font-family: arial,helvetica,sans-serif;" },
                {
                    "Avant Garde Gothic",
                    "font-family: 'Century Gothic','Avant Garde Gothic','Avant Garde','URW Gothic L',helvetica,sans-serif;"
                    },
                { "Bookman Old Style", "font-family: 'Bookman Old Style','URW Bookman L','itc bookman',times,serif;" },
                {
                    "Century Schoolbook",
                    "font-family: 'Century Schoolbook',Century,'new century schoolbook','Century Schoolbook L',times,serif;"
                    },
                { "Comic Sans MS", "font-family: 'Comic Sans MS',arial,helvetica,sans-serif;" },
                { "Courier New", "font-family: 'courier new',courier,monospace;" },
                { "Courier", "font-family: courier,monospace;" },
                { "Garamond", "font-family: Garamond,Garamond,'Garamond Antiqua',times,serif;" },
                { "Georgia", "font-family: georgia,times,serif;" },
                { "Helvetica", "font-family: helvetica,sans-serif;" },
                { "Impact", "font-family: impact,helvetica,sans-serif;" },
                {
                    "Palatino Linotype",
                    "font-family: 'Palatino Linotype','URW Palladio L','palladio l',palatino,'book antiqua',times,serif;"
                    },
                { "Segoe WP", "font-family: Segoe WP;" },
                { "Tahoma", "font-family: tahoma,arial,helvetica,sans-serif;" },
                { "Times New Roman", "font-family: 'Times New Roman','Times Roman',TimesNR,times,serif;" },
                { "Times Roman", "font-family: 'Times Roman',times,serif;" },
                { "Trebuchet MS", "font-family: 'Trebuchet MS',arial,helvetica,sans-serif;" },
                { "Verdana", "font-family: verdana,arial,helvetica,sans-serif;" }
            };

        /// <summary>
        /// The themes.
        /// </summary>
        public readonly Dictionary<Guid, Theme> Themes = new Dictionary<Guid, Theme>();

        /// <summary>
        /// The accent color.
        /// </summary>
        public Color AccentColor;

        /// <summary>
        /// The border color.
        /// </summary>
        public Color BorderColor;

        /// <summary>
        /// The font family.
        /// </summary>
        public string FontFamily = "Segoe WP";

        /// <summary>
        /// The is button color dark.
        /// </summary>
        public bool IsButtonColorDark;

        /// <summary>
        /// The is main back image.
        /// </summary>
        public bool IsMainBackImage;

        /// <summary>
        /// The main back color.
        /// </summary>
        public Color MainBackColor;

        /// <summary>
        /// The main back image.
        /// </summary>
        public string MainBackImage = string.Empty;

        /// <summary>
        /// The main font color.
        /// </summary>
        public Color MainFontColor;

        /// <summary>
        /// The name.
        /// </summary>
        public string Name = string.Empty;

        /// <summary>
        /// The title back color.
        /// </summary>
        public Color TitleBackColor;

        /// <summary>
        /// The title font color.
        /// </summary>
        public Color TitleFontColor;

        /// <summary>
        /// The uniq id.
        /// </summary>
        public Guid UniqId;

        /// <summary>
        /// The _current theme.
        /// </summary>
        private Guid _currentTheme = Guid.NewGuid();

        #endregion Fields

        #region Properties

        /// <summary>
        /// Gets or sets CurrentTheme.
        /// </summary>
        public Guid CurrentTheme
        {
            get
            {
                return _currentTheme;
            }

            set
            {
                _currentTheme = value;
                if (Themes.ContainsKey(_currentTheme))
                {
                    Clone(Themes[_currentTheme]);
                }
                else
                {
                    // set the default values.
                    FontFamily = Application.Current.Resources["PhoneFontFamilyNormal"].ToString();
                    AccentColor = (Color)Application.Current.Resources["PhoneAccentColor"];
                    BorderColor = (Color)Application.Current.Resources["PhoneForegroundColor"];
                    MainFontColor = BorderColor;
                    TitleFontColor = BorderColor;
                    MainBackColor = (Color)Application.Current.Resources["PhoneBackgroundColor"];
                    TitleBackColor = MainBackColor;
                    IsButtonColorDark = (Visibility)Application.Current.Resources["PhoneDarkThemeVisibility"]
                                             == Visibility.Collapsed;
                    IsMainBackImage = false;
                    MainBackImage = string.Empty;
                }
            }
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// The clone.
        /// </summary>
        /// <param name="from">
        /// The from.
        /// </param>
        public void Clone(Theme from)
        {
            BorderColor = from.BorderColor;
            FontFamily = from.FontFamily;
            AccentColor = from.AccentColor;
            IsButtonColorDark = from.IsButtonColorDark;
            IsMainBackImage = from.IsMainBackImage;
            MainBackColor = from.MainBackColor;
            MainBackImage = from.MainBackImage;
            MainFontColor = from.MainFontColor;
            TitleBackColor = from.TitleBackColor;
            TitleFontColor = from.TitleFontColor;
            Name = from.Name;
            UniqId = from.UniqId;
        }

        /// <summary>
        /// The from stream.
        /// </summary>
        /// <param name="stream">
        /// The stream.
        /// </param>
        /// <param name="isTranslateNames">
        /// The is translate names.
        /// </param>
        public void FromStream(Stream stream, bool isTranslateNames = false)
        {
            Guid currentTheme = Guid.NewGuid();

            using (XmlReader reader = XmlReader.Create(stream, new XmlReaderSettings { IgnoreWhitespace = true }))
            {
                Theme foundTheme = null;

                // Parse the file and get each of the nodes.
                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            if (reader.Name.ToLower().Equals("ccthemes") && reader.HasAttributes)
                            {
                                reader.MoveToFirstAttribute();
                                do
                                {
                                    switch (reader.Name.ToLower())
                                    {
                                        case "currenttheme":
                                            Guid.TryParse(reader.Value, out currentTheme);
                                            break;
                                    }
                                }
                                while (reader.MoveToNextAttribute());
                            }
                            else if (reader.Name.ToLower().Equals("theme") && reader.HasAttributes)
                            {
                                foundTheme = new Theme();

                                reader.MoveToFirstAttribute();
                                do
                                {
                                    switch (reader.Name.ToLower())
                                    {
                                        case "accentcolor":
                                            foundTheme.AccentColor = StringToColor(reader.Value);
                                            break;
                                        case "bordercolor":
                                            foundTheme.BorderColor = StringToColor(reader.Value);
                                            break;
                                        case "titlebackcolor":
                                            foundTheme.TitleBackColor = StringToColor(reader.Value);
                                            break;
                                        case "titlefontcolor":
                                            foundTheme.TitleFontColor = StringToColor(reader.Value);
                                            break;
                                        case "font":
                                            foundTheme.FontFamily = reader.Value;
                                            break;
                                        case "ismainbackimage":
                                            foundTheme.IsMainBackImage = reader.Value.ToLower().Equals("true");
                                            break;
                                        case "mainbackcolor":
                                            foundTheme.MainBackColor = StringToColor(reader.Value);
                                            break;
                                        case "mainbackimage":
                                            foundTheme.MainBackImage = reader.Value;
                                            break;
                                        case "mainfontcolor":
                                            foundTheme.MainFontColor = StringToColor(reader.Value);
                                            break;
                                        case "isbuttoncolordark":
                                            foundTheme.IsButtonColorDark = reader.Value.ToLower().Equals("true");
                                            break;
                                        case "uniqid":
                                            Guid.TryParse(reader.Value, out foundTheme.UniqId);
                                            break;
                                    }
                                }
                                while (reader.MoveToNextAttribute());

                                Themes[foundTheme.UniqId] = foundTheme;
                            }

                            break;
                        case XmlNodeType.EndElement:
                            if (reader.Name.ToLower().Equals("theme"))
                            {
                                if (isTranslateNames)
                                {
                                    if (foundTheme != null)
                                    {
                                        foundTheme.Name = Translations.Translate(foundTheme.Name);
                                    }
                                }

                                foundTheme = null;
                            }

                            break;
                        case XmlNodeType.Text:
                            if (foundTheme != null)
                            {
                                foundTheme.Name += reader.Value.Trim();
                            }

                            break;
                    }
                }
            }

            CurrentTheme = currentTheme;
        }

        /// <summary>
        /// The from string.
        /// </summary>
        /// <param name="buffer">
        /// The buffer.
        /// </param>
        public void FromString(string buffer)
        {
            FromByteArray(Encoding.UTF8.GetBytes(buffer));
        }

        /// <summary>
        /// The get unique guid key.
        /// </summary>
        /// <returns>
        /// </returns>
        public Guid GetUniqueGuidKey()
        {
            Guid key = Guid.NewGuid();
            if (Themes == null)
            {
                return key;
            }
            do
            {
                key = Guid.NewGuid();
            }
            while (Themes.ContainsKey(key));
            return key;
        }

        /// <summary>
        /// The initialize from resources.
        /// </summary>
        public void InitializeFromResources()
        {
            Assembly assem = Assembly.GetExecutingAssembly();
            Stream stream = assem.GetManifestResourceStream("CrossConnect.Properties.themes.xml");

            if (stream != null)
            {
                FromStream(stream, true);
                stream.Close();
            }
        }

        /// <summary>
        /// The merge.
        /// </summary>
        /// <param name="themes">
        /// The themes.
        /// </param>
        public void Merge(Theme themes)
        {
            // try to integrate this into the already existing themes.
            foreach (var tema in themes.Themes)
            {
                if (Themes.ContainsKey(tema.Value.UniqId))
                {
                    if (tema.Value.Equals(Themes[tema.Value.UniqId]))
                    {
                        continue;
                    }

                    Guid oldKey = tema.Value.UniqId;
                    tema.Value.UniqId = GetUniqueGuidKey();
                    if (oldKey.Equals(_currentTheme))
                    {
                        _currentTheme = tema.Value.UniqId;
                    }
                }

                Themes[tema.Value.UniqId] = tema.Value;
            }
        }

        /// <summary>
        /// The one theme to string.
        /// </summary>
        /// <param name="theme">
        /// The theme.
        /// </param>
        /// <returns>
        /// The one theme to string.
        /// </returns>
        public string OneThemeToString(Theme theme)
        {
            return "<theme bordercolor=\"" + ColorToString(theme.BorderColor) + "\" " + "titlebackcolor=\""
                   + ColorToString(theme.TitleBackColor) + "\" " + "accentcolor=\"" + ColorToString(theme.AccentColor)
                   + "\" " + "titlefontcolor=\"" + ColorToString(theme.TitleFontColor) + "\" " + "font=\""
                   + theme.FontFamily + "\" " + "ismainbackimage=\"" + theme.IsMainBackImage + "\" "
                   + "mainbackcolor=\"" + ColorToString(theme.MainBackColor) + "\" " + "mainbackimage=\""
                   + theme.MainBackImage + "\" " + "mainfontcolor=\"" + ColorToString(theme.MainFontColor) + "\" "
                   + "isbuttoncolordark=\"" + theme.IsButtonColorDark + "\" " + "uniqid=\"" + theme.UniqId + "\" " + ">"
                   + theme.Name + "</theme>\n";
        }

        /// <summary>
        /// The to string.
        /// </summary>
        /// <returns>
        /// The to string.
        /// </returns>
        public new string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("<?xml version=\"1.0\" encoding=\"UTF-8\"?>\n");
            sb.Append("<ccthemes version=\"1.0\" currenttheme=\"" + _currentTheme + "\">\n");
            foreach (var theme in Themes)
            {
                sb.Append(OneThemeToString(theme.Value));
            }

            sb.Append("</ccthemes>");
            return sb.ToString();
        }

        /// <summary>
        /// The color to string.
        /// </summary>
        /// <param name="color">
        /// The color.
        /// </param>
        /// <returns>
        /// The color to string.
        /// </returns>
        private static string ColorToString(Color color)
        {
            return color.A.ToString("X2") + color.R.ToString("X2") + color.G.ToString("X2") + color.B.ToString("X2");
        }

        /// <summary>
        /// The string to color.
        /// </summary>
        /// <param name="color">
        /// The color.
        /// </param>
        /// <returns>
        /// </returns>
        private static Color StringToColor(string color)
        {
            var returnColor = new Color();
            if (string.IsNullOrEmpty(color) || color.Length < 8)
            {
                return returnColor;
            }

            byte red;
            byte green;
            byte blue;
            byte alpha;
            byte.TryParse(color.Substring(0, 2), NumberStyles.HexNumber, null, out alpha);
            byte.TryParse(color.Substring(2, 2), NumberStyles.HexNumber, null, out red);
            byte.TryParse(color.Substring(4, 2), NumberStyles.HexNumber, null, out green);
            byte.TryParse(color.Substring(6, 2), NumberStyles.HexNumber, null, out blue);
            returnColor.A = alpha;
            returnColor.R = red;
            returnColor.G = green;
            returnColor.B = blue;
            return returnColor;
        }

        /// <summary>
        /// The equals.
        /// </summary>
        /// <param name="from">
        /// The from.
        /// </param>
        /// <returns>
        /// The equals.
        /// </returns>
        private bool Equals(Theme from)
        {
            return BorderColor.Equals(from.BorderColor) && FontFamily.Equals(from.FontFamily)
                   && AccentColor.Equals(from.AccentColor) && IsButtonColorDark.Equals(from.IsButtonColorDark)
                   && IsMainBackImage.Equals(from.IsMainBackImage) && MainBackColor.Equals(from.MainBackColor)
                   && MainBackImage.Equals(from.MainBackImage) && MainFontColor.Equals(from.MainFontColor)
                   && TitleBackColor.Equals(from.TitleBackColor) && TitleFontColor.Equals(from.TitleFontColor)
                   && Name.Equals(from.Name) && UniqId.Equals(from.UniqId);
        }

        /// <summary>
        /// The from byte array.
        /// </summary>
        /// <param name="buffer">
        /// The buffer.
        /// </param>
        private void FromByteArray(byte[] buffer)
        {
            var stream = new MemoryStream(buffer);
            FromStream(stream);
            stream.Close();
        }

        #endregion Methods
    }
}