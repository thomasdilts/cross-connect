#region Header

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
    using System.Collections.Generic;

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
        #region Properties

        bool ExistsShortNames
        {
            get;
        }

        bool IsExternalLink
        {
            get;
        }

        bool IsHearable
        {
            get;
        }

        bool IsLocalChangeDuringLink
        {
            get;
        }

        bool IsPageable
        {
            get;
        }

        bool IsSearchable
        {
            get;
        }

        bool IsSynchronizeable
        {
            get;
        }

        bool IsTranslateable
        {
            get;
        }

        #endregion Properties

        #region Methods

        ButtonWindowSpecs GetButtonWindowSpecs(int stage, int lastSelectedButton);

        string GetExternalLink(DisplaySettings displaySettings);

        void GetInfo(out int bookNum, out int absoluteChaptNum, out int relChaptNum, out int verseNum,
            out string fullName, out string title);

        string GetLanguage();

        void GetTranslateableTexts(DisplaySettings displaySettings, string bibleToLoad, out string[] toTranslate,
            out bool[] isTranslateable);

        string GetVerseTextOnly(DisplaySettings displaySettings, int chapterNumber, int verseNum);

        List<string> MakeListDisplayText(DisplaySettings displaySettings, List<BiblePlaceMarker> listToDisplay);

        void MoveChapterVerse(int chapter, int verse, bool isLocalLinkChange);

        void MoveNext();

        void MovePrevious();

        string PutHtmlTofile(DisplaySettings displaySettings, string htmlBackgroundColor, string htmlForegroundColor,
            string htmlPhoneAccentColor, double htmlFontSize, string fontFamily, string fileErase, string filePath,
            bool forceReload);

        void RegisterUpdateEvent(WindowSourceChanged sourceChangedMethod, bool isRegister = true);

        void Resume();

        void SerialSave();

        #endregion Methods
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

        #endregion Fields

        #region Constructors

        public ButtonWindowSpecs(
            int stage,
            string title,
            int numButtons,
            int[] colors,
            string[] text,
            int[] value,
            ButtonSize butSize)
        {
            Stage = stage;
            Title = title;
            NumButtons = numButtons;
            Colors = colors;
            Text = text;
            Value = value;
            ButSize = butSize;
        }

        #endregion Constructors
    }
}