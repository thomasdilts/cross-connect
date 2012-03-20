#region Header

// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TranslatorReader.cs" company="">
//
// </copyright>
// <summary>
//   Load from a file all the book and verse pointers to the bzz file so that
//   we can later read the bzz file quickly and efficiently.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
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

    using Sword.reader;

    /// <summary>
    /// Load from a file all the book and verse pointers to the bzz file so that
    ///   we can later read the bzz file quickly and efficiently.
    /// </summary>
    [DataContract(Name = "TranslatorReader")]
    [KnownType(typeof(ChapterPos))]
    [KnownType(typeof(BookPos))]
    [KnownType(typeof(VersePos))]
    public class TranslatorReader : BibleZtextReader
    {
        #region Fields

        /// <summary>
        /// The display text.
        /// </summary>
        [DataMember]
        public string DisplayText = string.Empty;

        /// <summary>
        /// The _is translateable.
        /// </summary>
        private bool[] _isTranslateable;

        /// <summary>
        /// The _to translate.
        /// </summary>
        private string[] _toTranslate;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="TranslatorReader"/> class.
        /// </summary>
        /// <param name="path">
        /// The path.
        /// </param>
        /// <param name="iso2DigitLangCode">
        /// The iso 2 digit lang code.
        /// </param>
        /// <param name="isIsoEncoding">
        /// The is iso encoding.
        /// </param>
        public TranslatorReader(string path, string iso2DigitLangCode, bool isIsoEncoding)
            : base(path, iso2DigitLangCode, isIsoEncoding)
        {
        }

        #endregion Constructors

        #region Delegates

        /// <summary>
        /// The show progress.
        /// </summary>
        /// <param name="percent">
        /// The percent.
        /// </param>
        /// <param name="totalFound">
        /// The total found.
        /// </param>
        /// <param name="isAbort">
        /// The is abort.
        /// </param>
        /// <param name="isFinished">
        /// The is finished.
        /// </param>
        public delegate void ShowProgress(double percent, int totalFound, bool isAbort, bool isFinished);

        #endregion Delegates

        #region Properties

        /// <summary>
        /// Gets a value indicating whether IsHearable.
        /// </summary>
        public override bool IsHearable
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets a value indicating whether IsPageable.
        /// </summary>
        public override bool IsPageable
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets a value indicating whether IsSearchable.
        /// </summary>
        public override bool IsSearchable
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets a value indicating whether IsSynchronizeable.
        /// </summary>
        public override bool IsSynchronizeable
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets a value indicating whether IsTranslateable.
        /// </summary>
        public override bool IsTranslateable
        {
            get
            {
                return false;
            }
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// The get info.
        /// </summary>
        /// <param name="bookNum">
        /// The book num.
        /// </param>
        /// <param name="absouteChaptNum">
        /// The absoute chapt num.
        /// </param>
        /// <param name="relChaptNum">
        /// The rel chapt num.
        /// </param>
        /// <param name="verseNum">
        /// The verse num.
        /// </param>
        /// <param name="fullName">
        /// The full name.
        /// </param>
        /// <param name="title">
        /// The title.
        /// </param>
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

        /// <summary>
        /// The translate 
        /// </summary>
        /// <param name="toTranslate">
        /// The to translate.
        /// </param>
        /// <param name="isTranslateable">
        /// The is translateable.
        /// </param>
        /// <param name="fromLanguage">
        /// The from language.
        /// </param>
        public void TranslateThis(string[] toTranslate, bool[] isTranslateable, string fromLanguage)
        {
            _toTranslate = toTranslate;
            _isTranslateable = isTranslateable;
            var ggl = new TranslateByGoogle();
            for (int i = 0; i < isTranslateable.Length; i++)
            {
                if (!isTranslateable[i])
                {
                    continue;
                }

                ggl.GetGoogleTranslationAsync(toTranslate[i], fromLanguage, TextTranslatedByGoogle);
                break;
            }
        }

        /// <summary>
        /// The get chapter html.
        /// </summary>
        /// <param name="displaySettings">
        /// The display settings.
        /// </param>
        /// <param name="htmlBackgroundColor">
        /// The html background color.
        /// </param>
        /// <param name="htmlForegroundColor">
        /// The html foreground color.
        /// </param>
        /// <param name="htmlPhoneAccentColor">
        /// The html phone accent color.
        /// </param>
        /// <param name="htmlFontSize">
        /// The html font size.
        /// </param>
        /// <param name="fontFamily">
        /// The font family.
        /// </param>
        /// <param name="isNotesOnly">
        /// The is notes only.
        /// </param>
        /// <param name="addStartFinishHtml">
        /// The add start finish html.
        /// </param>
        /// <param name="forceReload">
        /// The force reload.
        /// </param>
        /// <returns>
        /// The get chapter html.
        /// </returns>
        protected override string GetChapterHtml(
            DisplaySettings displaySettings,
            string htmlBackgroundColor,
            string htmlForegroundColor,
            string htmlPhoneAccentColor,
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
                fontFamily) + DisplayText + "</body></html>";
        }

        /// <summary>
        /// The text translated by google.
        /// </summary>
        /// <param name="translation">
        /// The translation.
        /// </param>
        /// <param name="isError">
        /// The is error.
        /// </param>
        private void TextTranslatedByGoogle(string translation, bool isError)
        {
            DisplayText = string.Empty;
            if (isError)
            {
                DisplayText = translation;
            }
            else
            {
                for (int i = 0; i < _isTranslateable.Length; i++)
                {
                    if (_isTranslateable[i])
                    {
                        DisplayText += translation;
                    }
                    else
                    {
                        DisplayText += _toTranslate[i];
                    }
                }
            }

            RaiseSourceChangedEvent();
        }

        #endregion Methods
    }
}