#region Header

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

#endregion Header

namespace CrossConnect
{
    using System;
    using System.Runtime.Serialization;


    using CrossConnect.readers;

    using Sword.reader;
    using System.Threading.Tasks;

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

        void SynchronizeWindow(string bookNameShort, int chapterNum, int verseNum, IBrowserTextSource source);

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
    [KnownType(typeof(BiblePlaceMarkReader))]
    [KnownType(typeof(TranslatorReader))]
    [KnownType(typeof(SearchReader))]
    [KnownType(typeof(BibleNoteReader))]
    [KnownType(typeof(BibleZtextReader))]
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
        [DataMember]
        public string VoiceName = string.Empty;

        [DataMember]
        public bool IsNtOnly = false;

        [DataMember]
        public string Pattern = string.Empty;

        [DataMember]
        public string Src = string.Empty;

        [DataMember]
        public string code = string.Empty;
        [DataMember]
        public string IconLink = string.Empty;
        [DataMember]
        public string Name = string.Empty;
        [DataMember]
        public string Language = string.Empty;
        [DataMember]
        public string Icon = string.Empty;

        [DataMember]
        public string Font = string.Empty;

        #endregion Fields
        public async Task<SerializableWindowState> Clone()
        {
            return new SerializableWindowState
            {
                code = this.code,
                Src = this.Src,
                Pattern = this.Pattern,
                IsNtOnly = this.IsNtOnly,
                VoiceName = this.VoiceName,
                BibleDescription = this.BibleDescription,
                BibleToLoad = this.BibleToLoad,
                CurIndex = this.CurIndex,
                HtmlFontSize = this.HtmlFontSize,
                IsResume = this.IsResume,
                IsSynchronized = this.IsSynchronized,
                NumRowsIown = this.NumRowsIown,
                Source = await this.Source.Clone(),
                VSchrollPosition = this.VSchrollPosition,
                Window = this.Window,
                WindowType = this.WindowType,
                IconLink = this.IconLink,
                Language = this.Language,
                Name = this.Name,
                Icon = this.Icon,
                Font = this.Font
            };
        }
    }
}