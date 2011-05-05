using System;
using System.Net;
using System.Runtime.Serialization;


namespace SwordBackend
{
    public interface IBrowserTextSource
    {
        string getShortName(int bookNum);
        string getFullName(int bookNum);
        void getInfo(int chaptNum, out int bookNum, out int relChaptNum, out string fullName);
        string[] getAllShortNames();
        void ReloadSettingsFile();
        bool existsShortNames{get;}
        string GetChapterHtml(int chapterNumber, string htmlBackgroundColor, string htmlForegroundColor, string htmlPhoneAccentColor, double htmlFontSize);        
    }
}
