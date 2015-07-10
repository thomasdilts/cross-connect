// <copyright file="AudioModel.cs" company="Thomas Dilts">
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Windows.Media.Core;

namespace BackgroundAudioShared
{
    /// <summary>
    /// Simple representation for songs in a playlist that can be used both for
    /// data model (across processes) and view model (foreground UI)
    /// </summary>
    [DataContract]
    public class AudioModel
    {
        public const string nChapter = "Chapter";
        public const string nBook = "Book";
        public const string nVoiceName = "VoiceName";
        public const string nVerse = "Verse";
        public const string nCode = "Code";
        public const string nIcon = "Icon";
        public const string nIconLink = "IconLink";
        public const string nIsNtOnly = "IsNtOnly";
        public const string nLanguage = "Language";
        public const string nName = "Name";
        public const string nPattern = "Pattern";
        public const string nSrc = "Src";

        [DataMember]
        public int Chapter;
        [DataMember]
        public string Book;
        [DataMember]
        public string VoiceName;
        [DataMember]
        public int Verse;
        [DataMember]
        public string Code = string.Empty;
        [DataMember]
        public string Icon = string.Empty;
        [DataMember]
        public string IconLink = string.Empty;
        [DataMember]
        public bool IsNtOnly;
        [DataMember]
        public string Language = string.Empty;
        [DataMember]
        public string Name = string.Empty;
        [DataMember]
        public string Pattern = string.Empty;
        [DataMember]
        public string Src = string.Empty;

        public MediaSource AudioModelToMediaSource()
        {
            var source = MediaSource.CreateFromUri(new Uri(this.Src));
            source.CustomProperties[AudioModel.nChapter] = this.Chapter;
            source.CustomProperties[AudioModel.nBook] = this.Book;
            source.CustomProperties[AudioModel.nVoiceName] = this.VoiceName;
            source.CustomProperties[AudioModel.nVerse] = this.Verse;
            source.CustomProperties[AudioModel.nCode] = this.Code;
            source.CustomProperties[AudioModel.nIcon] = this.Icon;
            source.CustomProperties[AudioModel.nIconLink] = this.IconLink;
            source.CustomProperties[AudioModel.nIsNtOnly] = this.IsNtOnly;
            source.CustomProperties[AudioModel.nLanguage] = this.Language;
            source.CustomProperties[AudioModel.nName] = this.Name;
            source.CustomProperties[AudioModel.nPattern] = this.Pattern;
            source.CustomProperties[AudioModel.nSrc] = this.Src;
            return source;
        }

        public static AudioModel CreateFromMediaSource(MediaSource source)
        {
            return new AudioModel()
            {
                Chapter = (int)source.CustomProperties[AudioModel.nChapter],
                Book = (string)source.CustomProperties[AudioModel.nBook],
                VoiceName = (string)source.CustomProperties[AudioModel.nVoiceName],
                Verse = (int)source.CustomProperties[AudioModel.nVerse],
                Code = (string)source.CustomProperties[AudioModel.nCode],
                Icon = (string)source.CustomProperties[AudioModel.nIcon],
                IconLink = (string)source.CustomProperties[AudioModel.nIconLink],
                IsNtOnly = (bool)source.CustomProperties[AudioModel.nIsNtOnly],
                Language = (string)source.CustomProperties[AudioModel.nLanguage],
                Name = (string)source.CustomProperties[AudioModel.nName],
                Pattern = (string)source.CustomProperties[AudioModel.nPattern],
                Src = (string)source.CustomProperties[AudioModel.nSrc]
            };
        }


        public AudioModel Clone()
        {
            return new AudioModel()
            {
                Chapter = this.Chapter,
                Book = this.Book,
                VoiceName = this.VoiceName,
                Verse = this.Verse,
                Code = this.Code,
                Icon = this.Icon,
                IconLink = this.IconLink,
                IsNtOnly = this.IsNtOnly,
                Language = this.Language,
                Name = this.Name,
                Pattern = this.Pattern,
                Src = this.Src
            };
        }

        public void SaveToSettings()
        {
            ApplicationSettingsHelper.SaveSettingsValue(AudioModel.nChapter, this.Chapter);
            ApplicationSettingsHelper.SaveSettingsValue(AudioModel.nBook, this.Book);
            ApplicationSettingsHelper.SaveSettingsValue(AudioModel.nVoiceName, this.VoiceName);
            ApplicationSettingsHelper.SaveSettingsValue(AudioModel.nVerse, this.Verse);
            ApplicationSettingsHelper.SaveSettingsValue(AudioModel.nCode, this.Code);
            ApplicationSettingsHelper.SaveSettingsValue(AudioModel.nIcon, this.Icon);
            ApplicationSettingsHelper.SaveSettingsValue(AudioModel.nIconLink, this.IconLink);
            ApplicationSettingsHelper.SaveSettingsValue(AudioModel.nIsNtOnly, this.IsNtOnly);
            ApplicationSettingsHelper.SaveSettingsValue(AudioModel.nLanguage, this.Language);
            ApplicationSettingsHelper.SaveSettingsValue(AudioModel.nName, this.Name);
            ApplicationSettingsHelper.SaveSettingsValue(AudioModel.nPattern, this.Pattern);
            ApplicationSettingsHelper.SaveSettingsValue(AudioModel.nSrc, this.Src);
        }

        public static AudioModel CreateFromSettings()
        {
            return new AudioModel()
            {
                Chapter = (int)ApplicationSettingsHelper.ReadResetSettingsValue(AudioModel.nChapter),
                Book = (string)ApplicationSettingsHelper.ReadResetSettingsValue(AudioModel.nBook),
                VoiceName = (string)ApplicationSettingsHelper.ReadResetSettingsValue(AudioModel.nVoiceName),
                Verse = (int)ApplicationSettingsHelper.ReadResetSettingsValue(AudioModel.nVerse),
                Code = (string)ApplicationSettingsHelper.ReadResetSettingsValue(AudioModel.nCode),
                Icon = (string)ApplicationSettingsHelper.ReadResetSettingsValue(AudioModel.nIcon),
                IconLink = (string)ApplicationSettingsHelper.ReadResetSettingsValue(AudioModel.nIconLink),
                IsNtOnly = (bool)ApplicationSettingsHelper.ReadResetSettingsValue(AudioModel.nIsNtOnly),
                Language = (string)ApplicationSettingsHelper.ReadResetSettingsValue(AudioModel.nLanguage),
                Name = (string)ApplicationSettingsHelper.ReadResetSettingsValue(AudioModel.nName),
                Pattern = (string)ApplicationSettingsHelper.ReadResetSettingsValue(AudioModel.nPattern),
                Src = (string)ApplicationSettingsHelper.ReadResetSettingsValue(AudioModel.nSrc)
            };
        }
    }
}
