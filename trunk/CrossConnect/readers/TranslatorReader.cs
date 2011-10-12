/// <summary>
/// Distribution License:
/// CrossConnect is free software; you can redistribute it and/or modify it under
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
/// </summary>
/// <copyright file="TranslatorReader.cs" company="Thomas Dilts">
///     Thomas Dilts. All rights reserved.
/// </copyright>
/// <author>Thomas Dilts</author>
namespace CrossConnect.readers
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.Serialization;
    using System.Text;
    using System.Text.RegularExpressions;

    using SwordBackend;

    /// <summary>
    /// Load from a file all the book and verse pointers to the bzz file so that
    /// we can later read the bzz file quickly and efficiently.
    /// </summary>
    /// <param name="path">The path to where the ot.bzs,ot.bzv and ot.bzz and nt files are</param>
    [DataContract(Name = "TranslatorReader")]
    [KnownType(typeof(ChapterPos))]
    [KnownType(typeof(BookPos))]
    [KnownType(typeof(VersePos))]
    public class TranslatorReader : BibleZtextReader
    {
        #region Fields

        [DataMember]
        public string displayText = string.Empty;

        #endregion Fields

        #region Constructors

        public TranslatorReader(string path,
            string iso2DigitLangCode,
            bool isIsoEncoding)
            : base(path, iso2DigitLangCode, isIsoEncoding)
        {
        }
        #endregion Constructors

        #region Delegates

        public delegate void ShowProgress(double percent, int totalFound, bool isAbort, bool isFinished);

        #endregion Delegates

        #region Properties

        public override bool IsHearable
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

        #endregion Properties

        #region Methods
        private string[] toTranslate;
        private bool[] isTranslateable;
        public void TranslateThis(string[] toTranslate, bool[] isTranslateable, string fromLanguage)
        {
            this.toTranslate=toTranslate;
            this.isTranslateable=isTranslateable;
            TranslateByGoogle ggl=new TranslateByGoogle();
            for (int i = 0; i < isTranslateable.Length; i++)
			{
			    if(isTranslateable[i])
                {
                    ggl.GetGoogleTranslationAsync(toTranslate[i],fromLanguage,new TranslateByGoogle.GoogleTranslatedTextReturnEvent(TextTranslatedByGoogle));
                    break;
                }
			}
        }
        private void TextTranslatedByGoogle(string translation, bool isError)
        {
            displayText="";
            if(isError)
            {
                displayText=translation;
            }
            else
            {
                for (int i = 0; i < isTranslateable.Length; i++)
			    {
			        if(isTranslateable[i])
                    {
                        displayText+=translation;
                    }
                    else
                    {
                        displayText+=toTranslate[i];
                    }
			    }
            }
            raiseSourceChangedEvent();
        }
        public override void GetInfo(out int bookNum, out int absouteChaptNum, out int relChaptNum, out int verseNum, out string fullName, out string title)
        {
            verseNum = 0;
            absouteChaptNum = 0;
            bookNum = 0;
            relChaptNum = 0;
            fullName = "";
            title = Translations.translate("Translation");
        }

        protected override string GetChapterHtml(DisplaySettings displaySettings, string htmlBackgroundColor, string htmlForegroundColor, string htmlPhoneAccentColor, double htmlFontSize, bool isNotesOnly, bool addStartFinishHtml = true)
        {
            //Debug.WriteLine("SearchReader GetChapterHtml.text=" + displayText);
            return HtmlHeader(displaySettings, htmlBackgroundColor, htmlForegroundColor, htmlPhoneAccentColor, htmlFontSize)
                + displayText + "</body></html>";
        }

        #endregion Methods
    }
}