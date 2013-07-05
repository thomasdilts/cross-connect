﻿#region Header

// <copyright file="IBrowserTextSource.cs" company="Thomas Dilts">
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
// Email: thomas@cross-connect.se
// </summary>
// <author>Thomas Dilts</author>

#endregion Header

namespace Sword.reader
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Threading.Tasks;

    #region Enumerations

    public enum ButtonSize
    {
        Small,

        Medium,

        Large
    }

    #endregion Enumerations

    #region Delegates

    public delegate void WindowSourceChanged();

    #endregion Delegates

    public interface IBrowserTextSource
    {
        #region Public Properties

        bool ExistsShortNames { get; }

        bool IsExternalLink { get; }

        bool IsHearable { get; }

        bool IsLocalChangeDuringLink { get; }

        bool IsPageable { get; }

        bool IsSearchable { get; }

        bool IsSynchronizeable { get; }

        bool IsTranslateable { get; }

        bool IsLocked { get; }

        #endregion

        #region Public Methods and Operators

        ButtonWindowSpecs GetButtonWindowSpecs(int stage, int lastSelectedButton);

        Task<string> GetChapterHtml(
            DisplaySettings displaySettings,
            HtmlColorRgba htmlBackgroundColor,
            HtmlColorRgba htmlForegroundColor,
            HtmlColorRgba htmlPhoneAccentColor,
            double htmlFontSize,
            string fontFamily,
            bool isNotesOnly,
            bool addStartFinishHtml,
            bool forceReload);

        string GetExternalLink(DisplaySettings displaySettings);

        void GetInfo(
            out int bookNum,
            out int absoluteChaptNum,
            out int relChaptNum,
            out int verseNum,
            out string fullName,
            out string title);

        string GetLanguage();

        Task<object[]> GetTranslateableTexts(DisplaySettings displaySettings, string bibleToLoad);

        Task<string> GetVerseTextOnly(DisplaySettings displaySettings, int chapterNumber, int verseNum);

        Task<List<string>> MakeListDisplayText(DisplaySettings displaySettings, List<BiblePlaceMarker> listToDisplay);

        void MoveChapterVerse(int chapter, int verse, bool isLocalLinkChange, IBrowserTextSource source);

        void MoveNext();

        void MovePrevious();

        Task<string> PutHtmlTofile(
            DisplaySettings displaySettings,
            HtmlColorRgba htmlBackgroundColor,
            HtmlColorRgba htmlForegroundColor,
            HtmlColorRgba htmlPhoneAccentColor,
            double htmlFontSize,
            string fontFamily,
            string fileErase,
            string filePath,
            bool forceReload);

        void RegisterUpdateEvent(WindowSourceChanged sourceChangedMethod, bool isRegister = true);

        Task Resume();

        void SerialSave();

        #endregion
    }

    public class ButtonWindowSpecs
    {
        #region Fields

        public ButtonSize ButSize;

        public int[] Colors;

        public int NumButtons;

        public int Stage;

        public string[] Text;

        public string Title;

        public int[] Value;

        #endregion

        #region Constructors and Destructors

        public ButtonWindowSpecs(
            int stage, string title, int numButtons, int[] colors, string[] text, int[] value, ButtonSize butSize)
        {
            this.Stage = stage;
            this.Title = title;
            this.NumButtons = numButtons;
            this.Colors = colors;
            this.Text = text;
            this.Value = value;
            this.ButSize = butSize;
        }

        #endregion
    }
    public class HtmlColorRgba
    {
        #region Fields

        public byte B;

        public byte G;

        public byte R;

        public double alpha;

        #endregion

        #region Public Methods and Operators

        public static HtmlColorRgba CreateWithHtmlRgb(string rgb)
        {
            if (string.IsNullOrEmpty(rgb) || rgb.Length < 6)
            {
                return new HtmlColorRgba();
            }
            return new HtmlColorRgba
            {
                alpha = 1,
                R = byte.Parse(rgb.Substring(0, 2), NumberStyles.HexNumber),
                G = byte.Parse(rgb.Substring(2, 2), NumberStyles.HexNumber),
                B = byte.Parse(rgb.Substring(4, 2), NumberStyles.HexNumber)
            };
        }

        public string GetHtmlRgba()
        {
            return "rgba(" + this.R + "," + this.G + "," + this.B + ","
                   + Math.Round(this.alpha, 4).ToString(CultureInfo.InvariantCulture) + ")";
        }

        #endregion
    }

}