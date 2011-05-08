using System;
using System.Net;
using System.Runtime.Serialization;
using System.Collections.Generic;


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
