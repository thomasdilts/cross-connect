// <copyright file="ITiledWindow.cs" company="Thomas Dilts">
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

namespace CrossConnect
{
    using System;
    using System.Runtime.Serialization;

    using CrossConnect.readers;

    using Sword.reader;

    public interface ITiledWindow
    {
        #region Public Events

        event EventHandler HitButtonBigger;

        event EventHandler HitButtonClose;

        event EventHandler HitButtonSmaller;

        #endregion

        #region Public Properties

        bool ForceReload { get; set; }

        SerializableWindowState State { get; }

        #endregion

        #region Public Methods and Operators

        void DelayUpdateBrowser();

        void ShowSizeButtons(bool isShow = true);

        void ShowUserInterface(bool isShow);

        void SynchronizeWindow(string bookShortName, int chapterNum, int verseNum, IBrowserTextSource source);

        void UpdateBrowser(bool isOrientationChangeOnly);

        #endregion
    }

    public enum WindowType
    {
        WindowBible,

        WindowBibleNotes,

        WindowBook,

        WindowSearch,

        WindowHistory,

        WindowBookmarks,

        WindowDailyPlan,

        WindowAddedNotes,

        WindowCommentary,

        WindowTranslator,

        WindowInternetLink,

        WindowLexiconLink,

        WindowMediaPlayer
    }

    /// <summary>
    ///     I was forced to make this class just for serialization because a "UserControl"
    ///     cannot be serialized.
    /// </summary>
    [DataContract]
    [KnownType(typeof(DailyPlanReader))]
    [KnownType(typeof(CommentZtextReader))]
    [KnownType(typeof(BookMarkReader))]
    [KnownType(typeof(TranslatorReader))]
    [KnownType(typeof(HistoryReader))]
    [KnownType(typeof(SearchReader))]
    [KnownType(typeof(BibleNoteReader))]
    [KnownType(typeof(BibleZtextReader))]
    //[KnownType(typeof(MediaReader))]
    public class SerializableWindowState
    {
        #region Fields

        [DataMember(Name = "bibleDescription")]
        public string BibleDescription = string.Empty;

        [DataMember(Name = "bibleToLoad")]
        public string BibleToLoad = string.Empty;

        [DataMember(Name = "curIndex")]
        public int CurIndex;

        [DataMember(Name = "htmlFontSize")]
        public double HtmlFontSize = 10;

        public bool IsResume;

        [DataMember(Name = "isSynchronized")]
        public bool IsSynchronized = true;

        [DataMember(Name = "numRowsIown")]
        public int NumRowsIown = 1;

        [DataMember(Name = "source")]
        public IBrowserTextSource Source;

        [DataMember]
        public int VSchrollPosition = 0;

        [DataMember]
        public int Window;

        [DataMember(Name = "windowType")]
        public WindowType WindowType = WindowType.WindowBible;

        #endregion
    }
}