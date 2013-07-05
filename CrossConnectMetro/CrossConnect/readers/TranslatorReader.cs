#region Header

// <copyright file="TranslatorReader.cs" company="Thomas Dilts">
// CrossConnect Bible and Bible Commentary Reader for CrossWire.org
// Copyright (C) 2011 Thomas Dilts
// This program is free software: you can redistribute it and/or modify
// it under the +terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see http://www.gnu.org/licenses/.
// </copyright>
// <summary>
// Email: thomas@cross-connect.se
// </summary>
// <author>Thomas Dilts</author>

#endregion Header

namespace CrossConnect.readers
{
    using System.Runtime.Serialization;
    using System.Threading.Tasks;

    using Sword.reader;

    /// <summary>
    ///     Load from a file all the book and verse pointers to the bzz file so that
    ///     we can later read the bzz file quickly and efficiently.
    /// </summary>
    [DataContract(Name = "TranslatorReader")]
    [KnownType(typeof(ChapterPos))]
    [KnownType(typeof(BookPos))]
    [KnownType(typeof(VersePos))]
    public class TranslatorReader : BibleZtextReader
    {
        #region Fields

        [DataMember]
        public string DisplayText = string.Empty;

        private bool[] _isTranslateable;

        private string[] _toTranslate;

        #endregion

        #region Constructors and Destructors

        public TranslatorReader(string path, string iso2DigitLangCode, bool isIsoEncoding, string cipherKey, string configPath)
            : base(path, iso2DigitLangCode, isIsoEncoding, cipherKey, configPath)
        {
        }

        #endregion

        #region Delegates

        public delegate void ShowProgress(double percent, int totalFound, bool isAbort, bool isFinished);

        #endregion

        #region Public Properties

        public override bool IsHearable
        {
            get
            {
                return false;
            }
        }

        public override bool IsPageable
        {
            get
            {
                return false;
            }
        }

        public override bool IsSearchable
        {
            get
            {
                return false;
            }
        }

        public override bool IsSynchronizeable
        {
            get
            {
                return false;
            }
        }

        public override bool IsTranslateable
        {
            get
            {
                return false;
            }
        }

        #endregion

        #region Public Methods and Operators

        public override async Task<string> GetChapterHtml(
            DisplaySettings displaySettings,
            HtmlColorRgba htmlBackgroundColor,
            HtmlColorRgba htmlForegroundColor,
            HtmlColorRgba htmlPhoneAccentColor,
            double htmlFontSize,
            string fontFamily,
            bool isNotesOnly,
            bool addStartFinishHtml,
            bool forceReload)
        {
            // Debug.WriteLine("SearchReader GetChapterHtml.text=" + displayText);
            return HtmlHeader(
                displaySettings,
                htmlBackgroundColor,
                htmlForegroundColor,
                htmlPhoneAccentColor,
                htmlFontSize,
                fontFamily) + this.DisplayText + "</body></html>";
        }

        public override void GetInfo(
            out int bookNum,
            out int absouteChaptNum,
            out int relChaptNum,
            out int verseNum,
            out string fullName,
            out string title)
        {
            verseNum = 0;
            absouteChaptNum = 0;
            bookNum = 0;
            relChaptNum = 0;
            fullName = string.Empty;
            title = Translations.Translate("Translation");
        }

        public void TranslateThis(string[] toTranslate, bool[] isTranslateable, string fromLanguage)
        {
            this._toTranslate = toTranslate;
            this._isTranslateable = isTranslateable;
            var ggl = new TranslateByGoogle();
            for (int i = 0; i < isTranslateable.Length; i++)
            {
                if (!isTranslateable[i])
                {
                    continue;
                }

                ggl.GetGoogleTranslationAsync(toTranslate[i], fromLanguage, this.TextTranslatedByGoogle);
                break;
            }
        }

        #endregion

        #region Methods

        private void TextTranslatedByGoogle(string translation, bool isError)
        {
            this.DisplayText = string.Empty;
            if (isError)
            {
                this.DisplayText = translation;
            }
            else
            {
                for (int i = 0; i < this._isTranslateable.Length; i++)
                {
                    if (this._isTranslateable[i])
                    {
                        this.DisplayText += translation;
                    }
                    else
                    {
                        this.DisplayText += this._toTranslate[i];
                    }
                }
            }

            this.RaiseSourceChangedEvent();
        }

        #endregion
    }
}