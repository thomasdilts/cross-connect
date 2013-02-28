#region Header

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
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using System.Xml;

    using Windows.UI;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Media;

    public class Theme
    {
        #region Static Fields

        public static readonly Dictionary<string, string> FontFamilies = new Dictionary<string, string>
                                                                             {
                                                                                 {
                                                                                     "Andale Mono"
                                                                                     ,
                                                                                     "font-family: 'andale mono','monotype.com',monaco,'courier new',courier,monospace;"
                                                                                 },
                                                                                 {
                                                                                     "Arial Black"
                                                                                     ,
                                                                                     "font-family: 'Arial Black',helvetica,sans-serif;"
                                                                                 },
                                                                                 {
                                                                                     "Arial",
                                                                                     "font-family: arial,helvetica,sans-serif;"
                                                                                 },
                                                                                 {
                                                                                     "Avant Garde Gothic"
                                                                                     ,
                                                                                     "font-family: 'Century Gothic','Avant Garde Gothic','Avant Garde','URW Gothic L',helvetica,sans-serif;"
                                                                                 },
                                                                                 {
                                                                                     "Bookman Old Style"
                                                                                     ,
                                                                                     "font-family: 'Bookman Old Style','URW Bookman L','itc bookman',times,serif;"
                                                                                 },
                                                                                 {
                                                                                     "Century Schoolbook"
                                                                                     ,
                                                                                     "font-family: 'Century Schoolbook',Century,'new century schoolbook','Century Schoolbook L',times,serif;"
                                                                                 },
                                                                                 {
                                                                                     "Comic Sans MS"
                                                                                     ,
                                                                                     "font-family: 'Comic Sans MS',arial,helvetica,sans-serif;"
                                                                                 },
                                                                                 {
                                                                                     "Courier New"
                                                                                     ,
                                                                                     "font-family: 'courier new',courier,monospace;"
                                                                                 },
                                                                                 {
                                                                                     "Courier",
                                                                                     "font-family: courier,monospace;"
                                                                                 },
                                                                                 {
                                                                                     "Garamond"
                                                                                     ,
                                                                                     "font-family: Garamond,Garamond,'Garamond Antiqua',times,serif;"
                                                                                 },
                                                                                 {
                                                                                     "Georgia",
                                                                                     "font-family: georgia,times,serif;"
                                                                                 },
                                                                                 {
                                                                                     "Helvetica"
                                                                                     ,
                                                                                     "font-family: helvetica,sans-serif;"
                                                                                 },
                                                                                 {
                                                                                     "Impact",
                                                                                     "font-family: impact,helvetica,sans-serif;"
                                                                                 },
                                                                                 {
                                                                                     "Palatino Linotype"
                                                                                     ,
                                                                                     "font-family: 'Palatino Linotype','URW Palladio L','palladio l',palatino,'book antiqua',times,serif;"
                                                                                 },
                                                                                 {
                                                                                     "Segoe WP"
                                                                                     ,
                                                                                     "font-family: Segoe WP;"
                                                                                 },
                                                                                 {
                                                                                     "Segoe UI"
                                                                                     ,
                                                                                     "font-family: Segoe UI;"
                                                                                 },
                                                                                 {
                                                                                     "Tahoma",
                                                                                     "font-family: tahoma,arial,helvetica,sans-serif;"
                                                                                 },
                                                                                 {
                                                                                     "Times New Roman"
                                                                                     ,
                                                                                     "font-family: 'Times New Roman','Times Roman',TimesNR,times,serif;"
                                                                                 },
                                                                                 {
                                                                                     "Times Roman"
                                                                                     ,
                                                                                     "font-family: 'Times Roman',times,serif;"
                                                                                 },
                                                                                 {
                                                                                     "Trebuchet MS"
                                                                                     ,
                                                                                     "font-family: 'Trebuchet MS',arial,helvetica,sans-serif;"
                                                                                 },
                                                                                 {
                                                                                     "Verdana",
                                                                                     "font-family: verdana,arial,helvetica,sans-serif;"
                                                                                 }
                                                                             };

        #endregion

        #region Fields

        public readonly Dictionary<Guid, Theme> Themes = new Dictionary<Guid, Theme>();

        public Color AccentColor = Colors.Purple;

        public Color BorderColor;

        public string FontFamily = "Segoe WP";

        public Color FrameColor = Colors.White;

        public bool IsButtonColorDark;

        public bool IsMainBackImage;

        public Color MainBackColor = Colors.White;

        public string MainBackImage = string.Empty;

        public Color MainFontColor = Colors.Black;

        public string Name = string.Empty;

        public Color TitleBackColor;

        public Color TitleFontColor;

        public Guid UniqId;

        private Guid _currentTheme = Guid.NewGuid();

        #endregion

        #region Public Properties

        public Guid CurrentTheme
        {
            get
            {
                return this._currentTheme;
            }

            set
            {
                this._currentTheme = value;
                if (this.Themes.ContainsKey(this._currentTheme))
                {
                    this.Clone(this.Themes[this._currentTheme]);
                }
                else
                {
                    // set the default values.
                    this.FontFamily =
                        ((FontFamily)Application.Current.Resources["ContentControlThemeFontFamily"]).Source;
                    this.AccentColor = StringToColor("FFE90FF1");
                    this.BorderColor =
                        ((SolidColorBrush)Application.Current.Resources["FocusVisualBlackStrokeThemeBrush"]).Color;
                    this.MainFontColor = this.BorderColor;
                    this.TitleFontColor = this.BorderColor;
                    this.MainBackColor =
                        ((SolidColorBrush)Application.Current.Resources["AppBarItemBackgroundThemeBrush"]).Color;
                    this.TitleBackColor = this.MainBackColor;
                    this.IsButtonColorDark = false;
                    this.IsMainBackImage = false;
                    this.MainBackImage = string.Empty;
                }
            }
        }

        #endregion

        #region Public Methods and Operators

        public static string OneThemeToString(Theme theme)
        {
            return "<theme bordercolor=\"" + ColorToString(theme.BorderColor) + "\" " + "titlebackcolor=\""
                   + ColorToString(theme.TitleBackColor) + "\" " + "accentcolor=\"" + ColorToString(theme.AccentColor)
                   + "\" " + "titlefontcolor=\"" + ColorToString(theme.TitleFontColor) + "\" " + "font=\""
                   + theme.FontFamily + "\" " + "ismainbackimage=\"" + theme.IsMainBackImage + "\" "
                   + "mainbackcolor=\"" + ColorToString(theme.MainBackColor) + "\" " + "mainbackimage=\""
                   + theme.MainBackImage + "\" " + "mainfontcolor=\"" + ColorToString(theme.MainFontColor) + "\" "
                   + "framecolor=\"" + ColorToString(theme.FrameColor) + "\" " + "isbuttoncolordark=\""
                   + theme.IsButtonColorDark + "\" " + "uniqid=\"" + theme.UniqId + "\" " + ">" + theme.Name
                   + "</theme>\n";
        }

        public void Clone(Theme from)
        {
            this.BorderColor = from.BorderColor;
            this.FontFamily = from.FontFamily;
            this.AccentColor = from.AccentColor;
            this.IsButtonColorDark = from.IsButtonColorDark;
            this.IsMainBackImage = from.IsMainBackImage;
            this.MainBackColor = from.MainBackColor;
            this.MainBackImage = from.MainBackImage;
            this.MainFontColor = from.MainFontColor;
            this.TitleBackColor = from.TitleBackColor;
            this.TitleFontColor = from.TitleFontColor;
            this.FrameColor = from.FrameColor;
            this.Name = from.Name;
            this.UniqId = from.UniqId;
        }

        public void FromStream(Stream stream, bool isTranslateNames = false)
        {
            Guid currentTheme = Guid.NewGuid();
            Guid bogustheme = currentTheme;

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
                                        case "framecolor":
                                            foundTheme.FrameColor = StringToColor(reader.Value);
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

                                this.Themes[foundTheme.UniqId] = foundTheme;
                            }

                            break;
                        case XmlNodeType.EndElement:
                            if (reader.Name.ToLower().Equals("theme"))
                            {
                                if (isTranslateNames)
                                {
                                    if (foundTheme != null)
                                    {
                                        //foundTheme.Name = Translations.Translate(foundTheme.Name);
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

            this.CurrentTheme = currentTheme;
            if ((bogustheme.Equals(currentTheme) && this.Themes.Any()) || !this.Themes.ContainsKey(currentTheme))
            {
                this.CurrentTheme = this.Themes.First().Value.UniqId;
            }
        }

        public void FromString(string buffer)
        {
            this.FromByteArray(Encoding.UTF8.GetBytes(buffer));
        }

        public Guid GetUniqueGuidKey()
        {
            Guid key = Guid.NewGuid();
            if (this.Themes == null)
            {
                return key;
            }
            do
            {
                key = Guid.NewGuid();
            }
            while (this.Themes.ContainsKey(key));
            return key;
        }

        public void InitializeFromResources()
        {
            Assembly assem = Assembly.Load(new AssemblyName("CrossConnect"));
            Stream stream = assem.GetManifestResourceStream("CrossConnect.Properties.themes.xml");

            if (stream != null)
            {
                this.FromStream(stream, true);
                stream.Dispose();
            }
        }

        public void Merge(Theme themes)
        {
            // try to integrate this into the already existing themes.
            foreach (var tema in themes.Themes)
            {
                if (this.Themes.ContainsKey(tema.Value.UniqId))
                {
                    if (tema.Value.Equals(this.Themes[tema.Value.UniqId]))
                    {
                        continue;
                    }

                    Guid oldKey = tema.Value.UniqId;
                    tema.Value.UniqId = this.GetUniqueGuidKey();
                    if (oldKey.Equals(this._currentTheme))
                    {
                        this._currentTheme = tema.Value.UniqId;
                    }
                }

                this.Themes[tema.Value.UniqId] = tema.Value;
            }
        }

        public new string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("<?xml version=\"1.0\" encoding=\"UTF-8\"?>\n");
            sb.Append("<ccthemes version=\"1.0\" currenttheme=\"" + this._currentTheme + "\">\n");
            foreach (var theme in this.Themes)
            {
                sb.Append(OneThemeToString(theme.Value));
            }

            sb.Append("</ccthemes>");
            return sb.ToString();
        }

        #endregion

        #region Methods

        private static string ColorToString(Color color)
        {
            return color.A.ToString("X2") + color.R.ToString("X2") + color.G.ToString("X2") + color.B.ToString("X2");
        }

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

        private bool Equals(Theme from)
        {
            return this.BorderColor.Equals(from.BorderColor) && this.FontFamily.Equals(from.FontFamily)
                   && this.AccentColor.Equals(from.AccentColor) && this.IsButtonColorDark.Equals(from.IsButtonColorDark)
                   && this.IsMainBackImage.Equals(from.IsMainBackImage) && this.MainBackColor.Equals(from.MainBackColor)
                   && this.MainBackImage.Equals(from.MainBackImage) && this.MainFontColor.Equals(from.MainFontColor)
                   && this.TitleBackColor.Equals(from.TitleBackColor) && this.TitleFontColor.Equals(from.TitleFontColor)
                   && this.FrameColor.Equals(from.FrameColor) && this.Name.Equals(from.Name)
                   && this.UniqId.Equals(from.UniqId);
        }

        private void FromByteArray(byte[] buffer)
        {
            var stream = new MemoryStream(buffer);
            this.FromStream(stream);
            stream.Dispose();
        }

        #endregion
    }
}