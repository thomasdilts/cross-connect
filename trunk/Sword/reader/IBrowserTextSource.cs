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
/// <copyright file="IBrowserTextSource.cs" company="Thomas Dilts">
///     Thomas Dilts. All rights reserved.
/// </copyright>
/// <author>Thomas Dilts</author>
namespace SwordBackend
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Runtime.Serialization;

    #region Enumerations

    public enum ButtonSize
    {
        SMALL,
        MEDIUM,
        LARGE
    }

    #endregion Enumerations

    #region Delegates

    public delegate void WindowSourceChanged();

    #endregion Delegates

    public interface IBrowserTextSource
    {
        #region Properties

        bool existsShortNames
        {
            get;
        }

        bool IsExternalLink
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

        #endregion Properties

        #region Methods

        ButtonWindowSpecs GetButtonWindowSpecs(int stage, int lastSelectedButton);

        string getExternalLink(DisplaySettings displaySettings);

        void GetInfo(out int bookNum,out int absoluteChaptNum, out int relChaptNum, out int verseNum, out string fullName, out string title);

        string GetVerseTextOnly(DisplaySettings displaySettings, int chapterNumber, int verseNum);

        List<string> MakeListDisplayText(DisplaySettings displaySettings, List<BiblePlaceMarker> listToDisplay);

        void moveChapterVerse(int chapter, int verse, bool isLocalLinkChange);

        void moveNext();

        void movePrevious();

        string putHtmlTofile(DisplaySettings displaySettings,  string htmlBackgroundColor, string htmlForegroundColor, string htmlPhoneAccentColor, double htmlFontSize, string fileErase, string filePath);

        void RegisterUpdateEvent(WindowSourceChanged sourceChangedMethod, bool isRegister = true);

        void Resume();

        void SerialSave();

        #endregion Methods
    }

    public class ButtonWindowSpecs
    {
        #region Fields

        public ButtonSize butSize;
        public int[] colors;
        public int NumButtons;
        public int stage;
        public string[] text;
        public string title;
        public int[] value;

        #endregion Fields

        #region Constructors

        public ButtonWindowSpecs(
            int stage,
            string title,
            int NumButtons,
            int[] colors,
            string[] text,
            int[] value,
            ButtonSize butSize)
        {
            this.stage = stage;
            this.title = title;
            this.NumButtons = NumButtons;
            this.colors = colors;
            this.text = text;
            this.value = value;
            this.butSize = butSize;
        }

        #endregion Constructors
    }
}