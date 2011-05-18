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
/// <copyright file="THIS_FILE.cs" company="Thomas Dilts">
///     Thomas Dilts. All rights reserved.
/// </copyright>
/// <author>Thomas Dilts</author>
namespace SwordBackend
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Runtime.Serialization;

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

        bool IsBookmarkable
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

        string[] getAllShortNames();

        string GetChapterHtml(int chapterNumber, string htmlBackgroundColor, string htmlForegroundColor, string htmlPhoneAccentColor, double htmlFontSize);

        // string getShortName(int bookNum);
        // string getFullName(int bookNum);
        void GetInfo(int chaptNum, int verseNum, out int bookNum, out int relChaptNum, out string fullName, out string title);

        string GetVerseTextOnly(int chapterNumber, int verseNum);

        List<string> MakeListDisplayText(List<BiblePlaceMarker> listToDisplay);

        void RegisterUpdateEvent(WindowSourceChanged sourceChangedMethod, bool isRegister = true);

        void ReloadSettingsFile();

        #endregion Methods
    }
}