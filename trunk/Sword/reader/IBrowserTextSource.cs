using System;
using System.Net;
using System.Runtime.Serialization;
using System.Collections.Generic;

///
/// <summary> Distribution License:
/// JSword is free software; you can redistribute it and/or modify it under
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
///
/// Copyright: 2011
///     The copyright to this program is held by Thomas Dilts
///  
namespace SwordBackend
{
    public delegate void WindowSourceChanged();

    public interface IBrowserTextSource
    {
        //string getShortName(int bookNum);
        //string getFullName(int bookNum);
        void getInfo(int chaptNum, int verseNum, out int bookNum, out int relChaptNum, out string fullName, out string title);
        string[] getAllShortNames();
        void ReloadSettingsFile();
        bool existsShortNames { get; }
        bool isSynchronizeable { get; }
        bool isSearchable { get; }
        bool isPageable { get; }
        bool isBookmarkable { get; }
        bool isLocalChangeDuringLink { get; }
        string GetChapterHtml(int chapterNumber, string htmlBackgroundColor, string htmlForegroundColor, string htmlPhoneAccentColor, double htmlFontSize);
        void registerUpdateEvent(WindowSourceChanged sourceChangedMethod, bool isRegister = true);
        List<string> makeListDisplayText(List<BiblePlaceMarker> listToDisplay);
    }
}
