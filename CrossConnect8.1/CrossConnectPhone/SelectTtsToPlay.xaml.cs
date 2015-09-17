#region Header

// <copyright file="SelectToPlay.xaml.cs" company="Thomas Dilts">
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

namespace CrossConnect
{
    using System;
    using System.Linq;
    using System.Net;
    using System.Windows;
    using System.Windows.Controls;
    using readers;
    using Microsoft.Phone.Shell;
    using AudioPlaybackAgent1;
    using Windows.Phone.Speech.Synthesis;

    /// <summary>
    /// The select to play.
    /// </summary>
    public partial class SelectTtsToPlay
    {
        #region Fields

        /// <summary>
        /// The _title bar.
        /// </summary>
        private int _chapter;

        /// <summary>
        /// The _is in selection changed.
        /// </summary>
        private bool _isInSelectionChanged;

        /// <summary>
        /// The _title bar.
        /// </summary>
        private string _language;

        /// <summary>
        /// The _title bar.
        /// </summary>
        private string _titleBar = string.Empty;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SelectToPlay"/> class.
        /// </summary>
        public SelectTtsToPlay()
        {
            InitializeComponent();
        }

        #endregion Constructors

        #region Methods

        private void PhoneApplicationPageLoaded(object sender, RoutedEventArgs e)
        {
            PageTitle.Text = Translations.Translate("Select what you want to hear");
            MsgFromServer.Visibility = Visibility.Collapsed;
            MsgFromServer.Text = string.Empty;
            SelectList.Items.Clear();

            // do a download.
            object BookToHear;
            object chapterToHear;
            object language;
            if (PhoneApplicationService.Current.State.TryGetValue("ChapterToHear", out chapterToHear)
                && PhoneApplicationService.Current.State.TryGetValue("BookToHear", out BookToHear)
                && PhoneApplicationService.Current.State.TryGetValue("ChapterToHearLanguage", out language))
            {
                //show the voices available.
                // get all of the installed voices
                var voices = Windows.Phone.Speech.Synthesis.InstalledVoices.All;
                // get the currently selected voice
                this.SelectList.Items.Clear();
                foreach (var voice in voices)
                {
                    var item = new TextBlock
                    {
                        Text = voice.DisplayName + " : " + voice.Language,
                        Style = this.PageTitle.Style,
                        Tag = voice,
                        Name = voice.DisplayName,
                        Margin = new Thickness(0, 0, 0, 10),
                        TextWrapping = TextWrapping.Wrap
                    };
                    this.SelectList.Items.Add(item);
                }
            }
        }

        private void SelectListSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_isInSelectionChanged)
            {
                return;
            }

            _isInSelectionChanged = true;

            // clear the selection because we might come here again after the media player
            SelectList.SelectedIndex = -1;
            object BookToHear;
            object chapterToHear;
            object verseToHear;
            object language;
            if (PhoneApplicationService.Current.State.TryGetValue("ChapterToHear", out chapterToHear)
                && PhoneApplicationService.Current.State.TryGetValue("BookToHear", out BookToHear)
                && PhoneApplicationService.Current.State.TryGetValue("VerseToHear", out verseToHear)
                && PhoneApplicationService.Current.State.TryGetValue("ChapterToHearLanguage", out language))
            {
                var info = new AudioPlayer.MediaInfo { Book = (string)BookToHear, Chapter = (int)chapterToHear, Verse = (int)verseToHear, VoiceName = ((VoiceInformation)((TextBlock)e.AddedItems[0]).Tag).DisplayName };
                info.Language = Translations.IsoLanguageCode;
                App.AddMediaWindow(info);
            }
            if (NavigationService.CanGoBack)
            {
                NavigationService.GoBack();
            }

            
            _isInSelectionChanged = false;
        }

        #endregion Methods
    }
}