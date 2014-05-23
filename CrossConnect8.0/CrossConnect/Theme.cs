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
    using System.Windows;
    using System.Windows.Media;
    using System.Xml;

    public class Theme
    {
        #region Static Fields

        public static readonly Dictionary<string, string> FontFamilies = new Dictionary<string, string>
        {
            { "Andale Mono", "font-family: 'andale mono','monotype.com',monaco,'courier new',courier,monospace;" },
            { "Arial Black", "font-family: 'Arial Black', 'Arial Bold', Gadget, sans-serif;" },
            { "Arial Narrow", "font-family: 'Arial Narrow', Arial, sans-serif;" },
            { "Arial Rounded", "font-family: 'Arial Rounded MT Bold', 'Helvetica Rounded', Arial, sans-serif;" },
            { "Arial", "font-family: Arial, 'Helvetica Neue', Helvetica, sans-serif;" },
            { "Avant Garde", "font-family: 'Avant Garde', Avantgarde, 'Century Gothic', CenturyGothic, 'Apple Gothic', sans-serif;" },
            { "Baskerville", "font-family: Baskerville, 'Baskerville old face', 'Hoefler Text', Garamond, 'Times New Roman', serif;" },
            { "Batang 바탕", "font-family: Batang, 바탕, sans-serif;" },
            { "Big Caslon", "font-family: 'Big Caslon', 'Book Antiqua', 'Palatino Linotype', Georgia, serif;" },
            { "Bodoni MT", "font-family: 'Bodoni MT', Didot, 'Didot LT STD', 'Hoefler Text', Garamond, 'Times New Roman', serif;" },
            { "Book Antiqua", "font-family: 'Book Antiqua', Palatino, 'Palatino Linotype', 'Palatino LT STD', Georgia, serif;" },
            { "Bookman Old Style", "font-family: 'Bookman Old Style','URW Bookman L','itc bookman',times,serif;" },
            { "Brush Script MT", "font-family: 'Brush Script MT', cursive;" },
            { "Calibri", "font-family: Calibri, Candara, Segoe, 'Segoe UI', Optima, Arial, sans-serif;" },
            { "Calisto MT", "font-family: 'Calisto MT', 'Bookman Old Style', Bookman, 'Goudy Old Style', Garamond, 'Hoefler Text', 'Bitstream Charter', Georgia, serif;" },
            { "Cambria", "font-family: Cambria, Georgia, serif;" },
            { "Candara", "font-family: Candara, Calibri, Segoe, 'Segoe UI', Optima, Arial, sans-serif;" },
            { "Century Gothic", "font-family: 'Century Gothic', CenturyGothic, 'Apple Gothic', sans-serif;" },
            { "Century Schoolbook", "font-family: 'Century Schoolbook',Century,'new century schoolbook','Century Schoolbook L',times,serif;" },
            { "Comic Sans MS", "font-family: 'Comic Sans MS',arial,helvetica,sans-serif;" },
            { "Consolas", "font-family: consolas, monaco, 'Lucida Console', monospace;" },
            { "Copperplate", "font-family: Copperplate, 'Copperplate Gothic Light', fantasy;" },
            { "Courier New", "font-family: 'Courier New', Courier, 'Lucida Sans Typewriter', 'Lucida Typewriter', monospace;" },
            { "Courier", "font-family: courier,monospace;" },
            { "Didot", "font-family: Didot, 'Didot LT STD', 'Hoefler Text', Garamond, 'Times New Roman', serif;" },
            { "Dotum", "font-family: Dotum, sans-serif;" },
            { "FangSong 仿宋", "font-family: FangSong,仿宋,sans-serif;" },
            { "FangSong_GB2312 仿宋_GB2312", "font-family: FangSong_GB2312,仿宋_GB2312,sans-serif;" },
            { "Franklin Gothic Medium", "font-family: 'Franklin Gothic Medium', Arial, sans-serif;" },
            { "Frutiger", "font-family: Frutiger, 'Frutiger Linotype', Univers, Calibri, 'Gill Sans', 'Gill Sans MT', 'Myriad Pro', Myriad, 'DejaVu Sans Condensed', 'Liberation Sans', 'Nimbus Sans L', Tahoma, Geneva, 'Helvetica Neue', Helvetica, Arial, sans-serif;" },
            { "Futura", "font-family: Futura, 'Trebuchet MS', Arial, sans-serif;" },
            { "Garamond", "font-family: Garamond, Baskerville, 'Baskerville Old Face', 'Hoefler Text', 'Times New Roman', serif;" },
            { "Geneva", "font-family: Geneva, Tahoma, Verdana, sans-serif;" },
            { "Georgia", "font-family: Georgia, Times, 'Times New Romanâ', serif;" },
            { "Gill Sans", "font-family: 'Gill Sans', 'Gill Sans MT', Calibri, sans-serif;" },
            { "Goudy Old Style", "font-family: 'Goudy Old Style', Garamond, 'Big Caslon', 'Times New Roman', serif;" },
            { "Gulim 굴림", "font-family: Gulim, 굴림, sans-serif;" },
            { "Helvetica Neue", "font-family: 'Helvetica Neue', Helvetica, Arial, sans-serif;" },
            { "Helvetica", "font-family: helvetica,sans-serif;" },
            { "Hoefler Text", "font-family: 'Hoefler Text', 'Baskerville old face', Garamond, 'Times New Roman', serif;" },
            { "Impact", "font-family: Impact, Haettenschweiler, 'Franklin Gothic Bold', Charcoal, 'Helvetica Inserat', 'Bitstream Vera Sans Bold', 'Arial Black', sans serif;" },
            { "KaiTi 楷体", "font-family: KaiTi,楷体,sans-serif;" },
            { "KaiTi_GB2312 楷体_GB2312", "font-family: KaiTi_GB2312,楷体_GB2312,sans-serif;" },
            { "Lucida Bright", "font-family: 'Lucida Bright', Georgia, serif;" },
            { "Lucida Console", "font-family: 'Lucida Console', 'Lucida Sans Typewriter', Monaco, 'Bitstream Vera Sans Mono', monospace;" },
            { "Lucida Grande", "font-family: 'Lucida Grande', 'Lucida Sans Unicode', 'Lucida Sans', Geneva, Verdana, sans-serif;" },
            { "Lucida Sans Typewriter", "font-family: 'Lucida Sans Typewriter', 'Lucida Console', Monaco, 'Bitstream Vera Sans Mono', monospace;" },
            { "MS Mincho 明朝", "font-family: 'MS Mincho', 'MS 明朝', 'MS PMincho', 'MS ゴシック',sans-serif;" },
            { "Microsoft YaHei 微软雅黑体", "font-family: 'Microsoft YaHei',微软雅黑体,sans-serif;" },
            { "Monaco", "font-family: monaco, consolas, 'Lucida Console', monospace;" },
            { "Museo-500", "font-family: 'Museo-500', Rockwell, Courier, 'Courier New', serif;" },
            { "NSimSun 新宋体", "font-family: NSimSun,新宋体,sans-serif;" },
            { "Optima", "font-family: Optima, Segoe, 'Segoe UI', Candara, Calibri, Arial, sans-serif;" },
            { "PMingLiu 新细明體", "font-family: PMingLiu,新细明體, sans-serif;" },            
            { "Palatino Linotype", "font-family: 'Palatino Linotype','URW Palladio L','palladio l',palatino,'book antiqua',times,serif;" },
            { "Palatino", "font-family: Palatino, 'Palatino Linotype', 'Palatino LT STD', 'Book Antiqua', Georgia, serif;" },
            { "Papyrus", "font-family: Papyrus, fantasy;" },
            { "Perpetua", "font-family: Perpetua, Baskerville, 'Big Caslon', 'Palatino Linotype', Palatino, 'URW Palladio L', 'Nimbus Roman No9 L', serif;" },
            { "Rockwell Extra Bold", "font-family: 'Rockwell Extra Bold', 'Rockwell Bold', monospace;" },
            { "Rockwell", "font-family: Rockwell, 'Courier Bold', Courier, Georgia, Times, 'Times New Roman', serif;" },
            { "Segoe UI", "font-family: Segoe UI, Segoe, 'Helvetica Neue', Arial, sans-serif;" },
            { "Segoe WP", "font-family: Segoe WP, Segoe, 'Helvetica Neue', Arial, sans-serif;" },
            { "SimHei 黑体", "font-family: SimHei,黑体,sans-serif;" },
            { "SimSun 宋体", "font-family: SimSun,宋体,sans-serif;" },
            { "Tahoma", "font-family: Tahoma, Verdana, Segoe, sans-serif;" },
            { "Times New Roman", "font-family: 'Times New Roman','Times Roman',TimesNR,times,serif;" },
            { "Times Roman", "font-family: 'Times Roman',times,serif;" },
            { "Trebuchet MS", "font-family: 'Trebuchet MS', 'Lucida Grande', 'Lucida Sans Unicode', 'Lucida Sans', Tahoma, sans-serif;" },
            { "Verdana", "font-family: verdana,arial,helvetica,sans-serif;" }
        };

        #endregion

        #region Fields

        public readonly Dictionary<Guid, Theme> Themes = new Dictionary<Guid, Theme>();

        public Color AccentColor;

        public Color BorderColor;

        public string FontFamily = "Segoe WP";

        public Color FrameColor;

        public bool IsButtonColorDark;

        public bool IsMainBackImage;

        public Color MainBackColor;

        public string MainBackImage = string.Empty;

        public Color MainFontColor;

        public string Name = string.Empty;

        public Color TitleBackColor;

        public Color TitleFontColor;

        public Color WordsOfChristRed;

        public Color[] ColorHighligt = new Color[6];

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
                    FontFamily = Application.Current.Resources["PhoneFontFamilyNormal"].ToString();
                    AccentColor = (Color)Application.Current.Resources["PhoneAccentColor"];
                    BorderColor = (Color)Application.Current.Resources["PhoneForegroundColor"];
                    MainBackColor = (Color)Application.Current.Resources["PhoneBackgroundColor"];
                    IsButtonColorDark = (Visibility)Application.Current.Resources["PhoneDarkThemeVisibility"]
                                             == Visibility.Collapsed;

                    // set the default values.
                    this.MainFontColor = this.BorderColor;
                    this.TitleFontColor = this.BorderColor;
                    this.TitleBackColor = this.MainBackColor;
                    this.IsMainBackImage = false;
                    this.MainBackImage = string.Empty;
                    this.WordsOfChristRed = StringToColor("FFFF1439");
                    this.ColorHighligt[0] = StringToColor("88E6E600");
                    this.ColorHighligt[1] = StringToColor("88E86C19");
                    this.ColorHighligt[2] = StringToColor("8819D119");
                    this.ColorHighligt[3] = StringToColor("8825B8B8");
                    this.ColorHighligt[4] = StringToColor("88FF66FF");
                    this.ColorHighligt[5] = StringToColor("88AD5C33");
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
                   + "framecolor=\"" + ColorToString(theme.FrameColor) + "\" " + "wordsofchrist=\"" + ColorToString(theme.WordsOfChristRed) + "\" " + "isbuttoncolordark=\""
                   + theme.IsButtonColorDark + "\" " + "uniqid=\"" + theme.UniqId + "\" " + "highlight1=\"" + ColorToString(theme.ColorHighligt[0]) + "\" "
                   + "highlight2=\"" + ColorToString(theme.ColorHighligt[1]) + "\" " + "highlight3=\"" + ColorToString(theme.ColorHighligt[2]) + "\" "
                   + "highlight4=\"" + ColorToString(theme.ColorHighligt[3]) + "\" " + "highlight5=\"" + ColorToString(theme.ColorHighligt[4]) + "\" "
                   + "highlight6=\"" + ColorToString(theme.ColorHighligt[5]) + "\" " + ">" + theme.Name
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
            this.WordsOfChristRed = from.WordsOfChristRed;
            this.Name = from.Name;
            this.UniqId = from.UniqId;
            for (int i = 0; i < from.ColorHighligt.Length; i++)
            {
                this.ColorHighligt[i] = from.ColorHighligt[i];
            }
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
                                foundTheme.WordsOfChristRed = StringToColor("FFFF1439");
                                foundTheme.ColorHighligt[0] = StringToColor("88E6E600");
                                foundTheme.ColorHighligt[1] = StringToColor("88E86C19");
                                foundTheme.ColorHighligt[2] = StringToColor("8819D119");
                                foundTheme.ColorHighligt[3] = StringToColor("8825B8B8");
                                foundTheme.ColorHighligt[4] = StringToColor("88FF66FF");
                                foundTheme.ColorHighligt[5] = StringToColor("88AD5C33");
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
                                        case "wordsofchrist":
                                            foundTheme.WordsOfChristRed = StringToColor(reader.Value);
                                            break;
                                        case "isbuttoncolordark":
                                            foundTheme.IsButtonColorDark = reader.Value.ToLower().Equals("true");
                                            break;
                                        case "uniqid":
                                            Guid.TryParse(reader.Value, out foundTheme.UniqId);
                                            break;
                                        case "highlight1":
                                            foundTheme.ColorHighligt[0] = StringToColor(reader.Value);
                                            break;
                                        case "highlight2":
                                            foundTheme.ColorHighligt[1] = StringToColor(reader.Value);
                                            break;
                                        case "highlight3":
                                            foundTheme.ColorHighligt[2] = StringToColor(reader.Value);
                                            break;
                                        case "highlight4":
                                            foundTheme.ColorHighligt[3] = StringToColor(reader.Value);
                                            break;
                                        case "highlight5":
                                            foundTheme.ColorHighligt[4] = StringToColor(reader.Value);
                                            break;
                                        case "highlight6":
                                            foundTheme.ColorHighligt[5] = StringToColor(reader.Value);
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
                   && this.FrameColor.Equals(from.FrameColor) && this.WordsOfChristRed.Equals(from.WordsOfChristRed) && this.Name.Equals(from.Name)
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