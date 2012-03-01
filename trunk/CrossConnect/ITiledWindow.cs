// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ITiledWindow.cs" company="">
//   
// </copyright>
// <summary>
//   The i tiled window.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace CrossConnect
{
    using System;
    using System.Runtime.Serialization;

    using CrossConnect.readers;

    using Sword.reader;

    /// <summary>
    /// The i tiled window.
    /// </summary>
    public interface ITiledWindow
    {
        #region Public Events

        /// <summary>
        /// The hit button bigger.
        /// </summary>
        event EventHandler HitButtonBigger;

        /// <summary>
        /// The hit button close.
        /// </summary>
        event EventHandler HitButtonClose;

        /// <summary>
        /// The hit button smaller.
        /// </summary>
        event EventHandler HitButtonSmaller;

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets a value indicating whether ForceReload.
        /// </summary>
        bool ForceReload { get; set; }

        /// <summary>
        /// Gets State.
        /// </summary>
        SerializableWindowState State { get; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The calculate title text width.
        /// </summary>
        void CalculateTitleTextWidth();

        /// <summary>
        /// The show size buttons.
        /// </summary>
        /// <param name="isShow">
        /// The is show.
        /// </param>
        void ShowSizeButtons(bool isShow = true);

        /// <summary>
        /// The synchronize window.
        /// </summary>
        /// <param name="chapterNum">
        /// The chapter num.
        /// </param>
        /// <param name="verseNum">
        /// The verse num.
        /// </param>
        void SynchronizeWindow(int chapterNum, int verseNum);

        /// <summary>
        /// The update browser.
        /// </summary>
        /// <param name="isOrientationChangeOnly">
        /// The is orientation change only.
        /// </param>
        void UpdateBrowser(bool isOrientationChangeOnly);

        #endregion
    }

    /// <summary>
    /// I was forced to make this class just for serialization because a "UserControl" 
    ///   cannot be serialized.
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
    [KnownType(typeof(MediaReader))]
    public class SerializableWindowState
    {
        #region Constants and Fields

        /// <summary>
        /// The bible description.
        /// </summary>
        [DataMember(Name = "bibleDescription")]
        public string BibleDescription = string.Empty;

        /// <summary>
        /// The bible to load.
        /// </summary>
        [DataMember(Name = "bibleToLoad")]
        public string BibleToLoad = string.Empty;

        /// <summary>
        /// The cur index.
        /// </summary>
        [DataMember(Name = "curIndex")]
        public int CurIndex;

        /// <summary>
        /// The html font size.
        /// </summary>
        [DataMember(Name = "htmlFontSize")]
        public double HtmlFontSize = 10;

        /// <summary>
        /// The is resume.
        /// </summary>
        public bool IsResume;

        /// <summary>
        /// The is synchronized.
        /// </summary>
        [DataMember(Name = "isSynchronized")]
        public bool IsSynchronized = true;

        /// <summary>
        /// The num rows iown.
        /// </summary>
        [DataMember(Name = "numRowsIown")]
        public int NumRowsIown = 1;

        /// <summary>
        /// The source.
        /// </summary>
        [DataMember(Name = "source")]
        public IBrowserTextSource Source;

        /// <summary>
        /// The window.
        /// </summary>
        [DataMember]
        public int Window;

        /// <summary>
        /// The window type.
        /// </summary>
        [DataMember(Name = "windowType")]
        public WindowType WindowType = WindowType.WindowBible;

        #endregion
    }
}