using System;
using System.Net;
using System.Runtime.Serialization;


namespace SwordBackend
{
    public interface IBrowserTextSource
    {
        /// <summary>
        /// Return the entire chapter
        /// </summary>
        /// <param name="bookNumber">Chaptern number beginning with zero for the first chapter and 1188 for the last chapter in the bible.</param>
        /// <returns>Entire Chapter</returns>
        string GetChapterRaw(int chapterNumber);
        string getShortName(int bookNum);

        string getFullName(int bookNum);
        void getInfo(int chaptNum, out int bookNum, out int relChaptNum, out string fullName);
        string[] getAllShortNames();
        void ReloadSettingsFile();
        bool existsShortNames{get;}
        string GetChapterHtml(int chapterNumber, string htmlBackgroundColor, string htmlForegroundColor, string htmlPhoneAccentColor, double htmlFontSize);
        string GetVerseRaw(int chapterNumber, int verseNumber);
        
    }
}
