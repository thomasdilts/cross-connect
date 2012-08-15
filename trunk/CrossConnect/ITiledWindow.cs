namespace CrossConnect
{
    using System;
    using System.Runtime.Serialization;

    using CrossConnect.readers;

    using Sword.reader;

    public interface ITiledWindow
    {
        #region Events

        event EventHandler HitButtonBigger;

        event EventHandler HitButtonClose;

        event EventHandler HitButtonSmaller;

        #endregion Events

        #region Properties

        bool ForceReload
        {
            get; set;
        }

        SerializableWindowState State
        {
            get;
        }

        #endregion Properties

        #region Methods

        void CalculateTitleTextWidth();

        void ShowSizeButtons(bool isShow = true);

        void SynchronizeWindow(int chapterNum, int verseNum);

        void UpdateBrowser(bool isOrientationChangeOnly);

        #endregion Methods
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
        public int Window;
        [DataMember(Name = "windowType")]
        public WindowType WindowType = WindowType.WindowBible;
        [DataMember]
        public int VSchrollPosition = 0;

        #endregion Fields
    }
}