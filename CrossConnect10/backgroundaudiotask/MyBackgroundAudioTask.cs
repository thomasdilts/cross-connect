// <copyright file="MyBackgroundAudioTask.cs" company="Thomas Dilts">
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
using System.Diagnostics;
using System.Threading;
using System.Linq;
using Windows.ApplicationModel.Background;
using Windows.Media;
using Windows.Media.Playback;
using Windows.Media.SpeechSynthesis;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Media.Core;
using System.Collections.Generic;

using BackgroundAudioShared;
using Windows.Foundation;
using BackgroundAudioShared.Messages;
using Windows.Storage.Streams;
using Sword.versification;
using Sword;
using Sword.reader;
using System.Threading.Tasks;

/* This background task will start running the first time the
 * MediaPlayer singleton instance is accessed from foreground. When a new audio 
 * or video app comes into picture the task is expected to recieve the cancelled 
 * event. User can save state and shutdown MediaPlayer at that time. When foreground 
 * app is resumed or restarted check if your music is still playing or continue from
 * previous state.
 * 
 * This task also implements SystemMediaTransportControl APIs for windows phone universal 
 * volume control. Unlike Windows 8.1 where there are different views in phone context, 
 * SystemMediaTransportControl is singleton in nature bound to the process in which it is 
 * initialized. If you want to hook up volume controls for the background task, do not 
 * implement SystemMediaTransportControls in foreground app process.
 */

namespace BackgroundAudioTask
{
    /// <summary>
    /// Impletements IBackgroundTask to provide an entry point for app code to be run in background. 
    /// Also takes care of handling UVC and communication channel with foreground
    /// </summary>
    public sealed class MyBackgroundAudioTask : IBackgroundTask
    {
        #region Private fields, properties

        private SystemMediaTransportControls smtc;
        private AudioModel currentlyPlaying = new AudioModel();
        private BackgroundTaskDeferral deferral; // Used to keep task alive
        private AppState foregroundAppState = AppState.Unknown;
        private ManualResetEvent backgroundTaskStarted = new ManualResetEvent(false);
        private bool playbackStartedPreviously = false;
        private Sword.InstalledBiblesAndCommentaries bibles = new Sword.InstalledBiblesAndCommentaries();
        //private Timer positionReporting;
        private SpeechSynthesizer synthesizer = null;
        private Sword.BibleNames booknames = null;
        private IBrowserTextSource zTextReader = null;
        #endregion

        #region IBackgroundTask and IBackgroundTaskInstance Interface Members and handlers
        /// <summary>
        /// The Run method is the entry point of a background task. 
        /// </summary>
        /// <param name="taskInstance"></param>
        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            //await bibles.Initialize();
            Debug.WriteLine("Background Audio Task " + taskInstance.Task.Name + " starting...");
            try
            {
                // Initialize SystemMediaTransportControls (SMTC) for integration with
                // the Universal Volume Control (UVC).
                //
                // The UI for the UVC must update even when the foreground process has been terminated
                // and therefore the SMTC is configured and updated from the background task.
                smtc = BackgroundMediaPlayer.Current.SystemMediaTransportControls;
                smtc.ButtonPressed += smtc_ButtonPressed;
                smtc.PropertyChanged += smtc_PropertyChanged;
                smtc.IsEnabled = true;
                smtc.IsPauseEnabled = true;
                smtc.IsPlayEnabled = true;
                smtc.IsNextEnabled = true;
                smtc.IsPreviousEnabled = true;

                // Read persisted state of foreground app

                // Add handlers for MediaPlayer
                BackgroundMediaPlayer.Current.CurrentStateChanged += Current_CurrentStateChanged;

                // Initialize message channel 
                BackgroundMediaPlayer.MessageReceivedFromForeground += BackgroundMediaPlayer_MessageReceivedFromForeground;
                BackgroundMediaPlayer.Current.MediaEnded += MediaEndedEvent;
                BackgroundMediaPlayer.Current.MediaFailed += MediaFailedEvent;

                // Send information to foreground that background task has been started if app is active
                MessageService.SendMessageToForeground(new BackgroundAudioTaskStartedMessage());

                deferral = taskInstance.GetDeferral(); // This must be retrieved prior to subscribing to events below which use it

                // Mark the background task as started to unblock SMTC Play operation (see related WaitOne on this signal)
                backgroundTaskStarted.Set();

                // Associate a cancellation and completed handlers with the background task.
                taskInstance.Task.Completed += TaskCompleted;
                taskInstance.Canceled += new BackgroundTaskCanceledEventHandler(OnCanceled); // event may raise immediately before continung thread excecution so must be at the end
            }
            catch (Exception)
            {

                throw;
            }
        }

        void Position_TimerCallback(object state)
        {
            MessageService.SendMessageToForeground(new PositionMessage(BackgroundMediaPlayer.Current.Position.Seconds*100/BackgroundMediaPlayer.Current.NaturalDuration.Seconds));
        }

        void MediaEndedEvent(MediaPlayer mp, System.Object obj)
        {
            Debug.WriteLine("MediaEndedEvent ");
            SkipToNext();
        }
        void MediaFailedEvent(MediaPlayer mp, MediaPlayerFailedEventArgs fail)
        {
            MessageService.SendMessageToForeground(new ErrorMessage(fail.ErrorMessage));
            Debug.WriteLine("MediaFailedEvent " + fail.ErrorMessage);
        }

        /// <summary>
        /// Indicate that the background task is completed.
        /// </summary>       
        void TaskCompleted(BackgroundTaskRegistration sender, BackgroundTaskCompletedEventArgs args)
        {
            Debug.WriteLine("MyBackgroundAudioTask " + sender.TaskId + " Completed...");
            deferral.Complete();
        }

        /// <summary>
        /// Handles background task cancellation. Task cancellation happens due to:
        /// 1. Another Media app comes into foreground and starts playing music 
        /// 2. Resource pressure. Your task is consuming more CPU and memory than allowed.
        /// In either case, save state so that if foreground app resumes it can know where to start.
        /// </summary>
        private void OnCanceled(IBackgroundTaskInstance sender, BackgroundTaskCancellationReason reason)
        {
            // You get some time here to save your state before process and resources are reclaimed
            //Debug.WriteLine("MyBackgroundAudioTask " + sender.Task.TaskId + " Cancel Requested...");
            try
            {
                // immediately set not running
                backgroundTaskStarted.Reset();

                if (this.currentlyPlaying != null && this.currentlyPlaying.Src!=null)
                {
                    // save state
                    currentlyPlaying.SaveToSettings();
                }

                // unsubscribe event handlers
                BackgroundMediaPlayer.MessageReceivedFromForeground -= BackgroundMediaPlayer_MessageReceivedFromForeground;
                BackgroundMediaPlayer.Current.MediaEnded -= MediaEndedEvent;
                BackgroundMediaPlayer.Current.MediaFailed -= MediaFailedEvent;
                smtc.ButtonPressed -= smtc_ButtonPressed;
                smtc.PropertyChanged -= smtc_PropertyChanged;
                
                BackgroundMediaPlayer.Shutdown(); // shutdown media pipeline
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
            deferral.Complete(); // signals task completion. 
            Debug.WriteLine("MyBackgroundAudioTask Cancel complete...");
        }
        #endregion

        #region SysteMediaTransportControls related functions and handlers
        /// <summary>
        /// Update Universal Volume Control (UVC) using SystemMediaTransPortControl APIs
        /// </summary>
        private void UpdateUVCOnNewTrack(bool isPlaying)
        {
            if (!isPlaying)
            {
                try
                {
                    MessageService.SendMessageToForeground(new TrackChangedMessage(currentlyPlaying, string.Empty));
                    smtc.PlaybackStatus = MediaPlaybackStatus.Stopped;
                    smtc.DisplayUpdater.MusicProperties.Title = string.Empty;
                    smtc.DisplayUpdater.Update();
                    return;
                }
                catch (Exception)
                {
                }
            }
            try
            {
                smtc.IsChannelDownEnabled = true;
                smtc.IsChannelUpEnabled = true;
                smtc.IsNextEnabled = true;
                smtc.IsPreviousEnabled = true;
                smtc.PlaybackStatus = MediaPlaybackStatus.Playing;
                smtc.DisplayUpdater.Type = MediaPlaybackType.Music;

                smtc.DisplayUpdater.MusicProperties.Title = GetCurrentTitle();
                MessageService.SendMessageToForeground(new TrackChangedMessage(currentlyPlaying, smtc.DisplayUpdater.MusicProperties.Title));

                if (!string.IsNullOrEmpty(currentlyPlaying.Icon))
                {
                    smtc.DisplayUpdater.Thumbnail = RandomAccessStreamReference.CreateFromUri(new Uri(currentlyPlaying.Icon));
                }
                else
                    smtc.DisplayUpdater.Thumbnail = null;

                smtc.DisplayUpdater.Update();
            }
            catch (Exception)
            {
            }

        }

        private string GetCurrentTitle()
        {
            string bookShortName;
            int relChaptNum;
            int verseNum;
            string fullName;
            string title;
            zTextReader.GetInfo(
                this.currentlyPlaying.Language,
                out bookShortName,
                out relChaptNum,
                out verseNum,
                out fullName,
                out title);
            return title;
        }

        /// <summary>
        /// Fires when any SystemMediaTransportControl property is changed by system or user
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        void smtc_PropertyChanged(SystemMediaTransportControls sender, SystemMediaTransportControlsPropertyChangedEventArgs args)
        {
            // TODO: If soundlevel turns to muted, app can choose to pause the music
        }

        /// <summary>
        /// This function controls the button events from UVC.
        /// This code if not run in background process, will not be able to handle button pressed events when app is suspended.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void smtc_ButtonPressed(SystemMediaTransportControls sender, SystemMediaTransportControlsButtonPressedEventArgs args)
        {
            switch (args.Button)
            {
                case SystemMediaTransportControlsButton.Play:
                    Debug.WriteLine("UVC play button pressed");

                    // When the background task has been suspended and the SMTC
                    // starts it again asynchronously, some time is needed to let
                    // the task startup process in Run() complete.

                    // Wait for task to start. 
                    // Once started, this stays signaled until shutdown so it won't wait
                    // again unless it needs to.
                    bool result = backgroundTaskStarted.WaitOne(5000);
                    if (!result)
                        throw new Exception("Background Task didnt initialize in time");
                    
                    StartPlayback();
                    break;
                case SystemMediaTransportControlsButton.Pause:
                    Debug.WriteLine("UVC pause button pressed");
                    try
                    {
                        BackgroundMediaPlayer.Current.Pause();
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex.ToString());
                    }
                    break;
                case SystemMediaTransportControlsButton.Next:
                    Debug.WriteLine("UVC next button pressed");
                    SkipToNext();
                    break;
                case SystemMediaTransportControlsButton.Previous:
                    Debug.WriteLine("UVC previous button pressed");
                    SkipToPrevious();
                    break;
            }
        }



        #endregion

        #region Playlist management functions and handlers
        /// <summary>
        /// Start playlist and change UVC state
        /// </summary>
        private void StartPlayback()
        {
            try
            {
                // If playback was already started once we can just resume playing.
                if (!playbackStartedPreviously)
                {
                    playbackStartedPreviously = true;

                    // If the task was cancelled we would have saved the current item
                    var source = AudioModel.CreateFromSettings();
                    if (source.Src != null)
                    {
                        SetAudioStartPoint(source);
                    }
                    else
                    {
                        // Begin playing
                        BackgroundMediaPlayer.Current.Play();
                    }
                }
                else
                {
                    // Begin playing
                    BackgroundMediaPlayer.Current.Play();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
        }

        /// <summary>
        /// Skip track and update UVC via SMTC
        /// </summary>
        private void SkipToPrevious()
        {
            smtc.PlaybackStatus = MediaPlaybackStatus.Changing;
            SetRelativeChapter(-1);
        }

        private void SetRelativeChapter(int relativePostion)
        {
            if (currentlyPlaying != null)
            {
                AddChapter(relativePostion);
                Debug.WriteLine("starting new track = " + currentlyPlaying.Src);
            }
        }

        private async void AddChapter(int valToAdd)
        {
            if (valToAdd > 0)
            {
                this.zTextReader.MoveNext(!string.IsNullOrEmpty(currentlyPlaying.VoiceName));
            }
            else
            {
                this.zTextReader.MovePrevious(!string.IsNullOrEmpty(currentlyPlaying.VoiceName));
            }
            string bookShortName;
            int relChaptNum;
            int verseNum;
            string fullName;
            string title;
            zTextReader.GetInfo(
                this.currentlyPlaying.Language,
                out bookShortName,
                out relChaptNum,
                out verseNum,
                out fullName,
                out title);
            currentlyPlaying.Book = bookShortName;
            currentlyPlaying.Chapter = relChaptNum;
            currentlyPlaying.Verse = verseNum;

            if(!string.IsNullOrEmpty(currentlyPlaying.VoiceName))
            {
                string textToSpeak = string.Empty;
                int loopCounter = 0;
                while (loopCounter<50 && string.IsNullOrEmpty((textToSpeak = (await zTextReader.GetTTCtext(true)).Trim())))
                {
                    loopCounter++;
                    if (valToAdd > 0)
                    {
                        this.zTextReader.MoveNext(true);
                    }
                    else
                    {
                        this.zTextReader.MovePrevious(true);
                    }
                    zTextReader.GetInfo(
                        this.currentlyPlaying.Language,
                        out bookShortName,
                        out relChaptNum,
                        out verseNum,
                        out fullName,
                        out title);
                    currentlyPlaying.Book = bookShortName;
                    currentlyPlaying.Chapter = relChaptNum;
                    currentlyPlaying.Verse = verseNum;
                }

                var stream = await synthesizer.SynthesizeTextToStreamAsync(textToSpeak);
                BackgroundMediaPlayer.Current.SetStreamSource(stream);
            }
            else
            {
                if (!string.IsNullOrEmpty(currentlyPlaying.Pattern) && !string.IsNullOrEmpty(currentlyPlaying.Code) && string.IsNullOrEmpty(currentlyPlaying.VoiceName))
                {
                    // we must make sure this follows the KJV canon
                    var canonKjv = CanonManager.GetCanon("KJV");
                    CanonBookDef book;
                    if (!canonKjv.BookByShortName.TryGetValue(currentlyPlaying.Book, out book))
                    {
                        // we can only have KJV in the audio here.
                        book = valToAdd > 0 ? canonKjv.BookByShortName["Matt"]: canonKjv.BookByShortName["Mal"];
                        currentlyPlaying.Book = book.ShortName1;
                        currentlyPlaying.Chapter = valToAdd > 0 ? 0 : book.NumberOfChapters - 1; ;
                        currentlyPlaying.Verse = 0;
                    }
                    else if(book.NumberOfChapters<(currentlyPlaying.Chapter +1))
                    {
                        // versification mess-up here.  move to next/previous book.
                        book = canonKjv.GetBookFromBookNumber(book.BookNum + valToAdd);
                        currentlyPlaying.Book = book.ShortName1;
                        currentlyPlaying.Chapter = valToAdd>0?0:book.NumberOfChapters-1;
                        currentlyPlaying.Verse = 0;
                    }

                    // http://www.cross-connect.se/bibles/talking/{key}/Bible_{key}_{booknum2d}_{chapternum3d}.mp3
                    currentlyPlaying.Src =
                        currentlyPlaying.Pattern.Replace("{key}", currentlyPlaying.Code)
                               .Replace("{booknum2d}", (book.BookNum + 1).ToString("D2"))
                               .Replace("{chapternum3d}", (currentlyPlaying.Chapter + 1).ToString("D3"));
                    BackgroundMediaPlayer.Current.SetUriSource(new Uri(currentlyPlaying.Src));
                }
            }

            UpdateUVCOnNewTrack(true);
        }

        /// <summary>
        /// Skip track and update UVC via SMTC
        /// </summary>
        private void SkipToNext()
        {
            smtc.PlaybackStatus = MediaPlaybackStatus.Changing;
            SetRelativeChapter(1);
        }
        #endregion

        #region Background Media Player Handlers
        void Current_CurrentStateChanged(MediaPlayer sender, object args)
        {
            Debug.WriteLine("Current_CurrentStateChanged");

            if (sender.CurrentState == MediaPlayerState.Playing)
            {
                smtc.PlaybackStatus = MediaPlaybackStatus.Playing;
            }
            else if (sender.CurrentState == MediaPlayerState.Paused)
            {
                smtc.PlaybackStatus = MediaPlaybackStatus.Paused;
            }
            else if (sender.CurrentState == MediaPlayerState.Closed)
            {
                smtc.PlaybackStatus = MediaPlaybackStatus.Closed;
            }
        }

        /// <summary>
        /// Raised when a message is recieved from the foreground app
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void BackgroundMediaPlayer_MessageReceivedFromForeground(object sender, MediaPlayerDataReceivedEventArgs e)
        {
            AppSuspendedMessage appSuspendedMessage;
            if (MessageService.TryParseMessage(e.Data, out appSuspendedMessage))
            {
                Debug.WriteLine("App suspending"); // App is suspended, you can save your task state at this point
                foregroundAppState = AppState.Suspended;
                if (this.currentlyPlaying != null && this.currentlyPlaying.Src != null)
                {
                    currentlyPlaying.SaveToSettings();
                }

                return;
            }

            AppResumedMessage appResumedMessage;
            if(MessageService.TryParseMessage(e.Data, out appResumedMessage))
            {
                Debug.WriteLine("App resuming"); // App is resumed, now subscribe to message channel
                foregroundAppState = AppState.Active;
                UpdateUVCOnNewTrack(smtc.PlaybackStatus == MediaPlaybackStatus.Playing);
                return;
            }

            StartPlaybackMessage startPlaybackMessage;
            if(MessageService.TryParseMessage(e.Data, out startPlaybackMessage))
            {
                //Foreground App process has signalled that it is ready for playback
                Debug.WriteLine("Starting Playback");
                StartPlayback();
                return;
            }

            SkipNextMessage skipNextMessage;
            if(MessageService.TryParseMessage(e.Data, out skipNextMessage))
            {
                // User has chosen to skip track from app context.
                Debug.WriteLine("Skipping to next");
                SkipToNext();
                return;
            }

            SkipPreviousMessage skipPreviousMessage;
            if (MessageService.TryParseMessage(e.Data, out skipPreviousMessage))
            {
                // User has chosen to skip track from app context.
                Debug.WriteLine("Skipping to previous");
                SkipToPrevious();
                return;
            }

            KillMessage killMessage;
            if (MessageService.TryParseMessage(e.Data, out killMessage))
            {
                // User has chosen to skip track from app context.
                OnCanceled(null, new BackgroundTaskCancellationReason());
                return;
            }

            UpdateStartPointMessage updatetartPointsMessage;
            if(MessageService.TryParseMessage(e.Data, out updatetartPointsMessage))
            {
                SetAudioStartPoint(updatetartPointsMessage.audioModel);
                BackgroundMediaPlayer.Current.Play();
                return;
            }
        }

        /// <summary>
        /// Create a playback list from the list of songs received from the foreground app.
        /// </summary>
        /// <param name="songs"></param>
        async void SetAudioStartPoint(AudioModel startPoint)
        {
            currentlyPlaying = startPoint;
            // Assign the list to the player
            playbackStartedPreviously = true;
            BackgroundMediaPlayer.Current.AutoPlay = true;
            booknames = new Sword.BibleNames(currentlyPlaying.Language, string.Empty);

            if (!string.IsNullOrEmpty(startPoint.VoiceName))
            {
                await bibles.Initialize();
                var bookKeyValue = bibles.InstalledBibles.FirstOrDefault(p => p.Value.InternalName.Equals(startPoint.Src));
                if(bookKeyValue.Value == null)
                {
                    bookKeyValue = bibles.InstalledCommentaries.FirstOrDefault(p => p.Value.InternalName.Equals(startPoint.Src));
                    if (bookKeyValue.Value == null)
                    {
                        bookKeyValue = bibles.InstalledGeneralBooks.FirstOrDefault(p => p.Value.InternalName.Equals(startPoint.Src));
                    }
                }
                SwordBookMetaData bookSelected = bookKeyValue.Value==null? bibles.InstalledBibles.First().Value : bookKeyValue.Value ;
                var driver = ((string)bookSelected.GetProperty(ConfigEntryType.ModDrv)).ToUpper();
                string bookPath =
                    bookSelected.GetCetProperty(ConfigEntryType.ADataPath).ToString().Substring(2);
                bool isIsoEncoding = !bookSelected.GetCetProperty(ConfigEntryType.Encoding).Equals("UTF-8");
                var langCode = ((Language)bookSelected.GetCetProperty(ConfigEntryType.Lang)).Code;
                var cipherKey = (string)bookSelected.GetCetProperty(ConfigEntryType.CipherKey);
                var versification = (string)bookSelected.GetCetProperty(ConfigEntryType.Versification);
                switch (driver)
                {
                    case "ZTEXT":
                        this.zTextReader = new BibleZtextReader(
                                            bookPath,
                                            langCode,
                                            isIsoEncoding,
                                            cipherKey,
                                            bookSelected.ConfPath,
                                            versification);
                        await ((BibleZtextReader)this.zTextReader).Initialize();
                        break;
                    case "RAWTEXT":
                        this.zTextReader = new BibleRawTextReader(
                                            bookPath,
                                            langCode,
                                            isIsoEncoding,
                                            cipherKey,
                                            bookSelected.ConfPath,
                                            versification);
                        await ((BibleRawTextReader)this.zTextReader).Initialize();
                        break;
                    case "ZCOM":
                        this.zTextReader = new CommentZtextReader(
                                            bookPath,
                                            langCode,
                                            isIsoEncoding,
                                            cipherKey,
                                            bookSelected.ConfPath,
                                            versification);
                        await ((CommentZtextReader)this.zTextReader).Initialize();
                        break;
                    case "RAWCOM":
                        this.zTextReader = new CommentRawComReader(
                                            bookPath,
                                            langCode,
                                            isIsoEncoding,
                                            cipherKey,
                                            bookSelected.ConfPath,
                                            versification);
                        await ((CommentRawComReader)this.zTextReader).Initialize();
                        break;
                    case "RAWGENBOOK":
                        this.zTextReader = new RawGenTextReader(
                                            bookPath,
                                            langCode,
                                            isIsoEncoding);
                        await ((RawGenTextReader)this.zTextReader).Initialize();
                        break;

                }

                this.zTextReader.MoveChapterVerse(startPoint.Book, startPoint.Chapter, startPoint.Verse, false, zTextReader);
                if (synthesizer==null)
                {
                    synthesizer = new SpeechSynthesizer();
                }

                synthesizer.Voice = SpeechSynthesizer.AllVoices.First(p => p.DisplayName.Equals(startPoint.VoiceName));
                string textToSpeak = string.Empty;
                int loopCounter = 0;
                while (loopCounter < 50 && string.IsNullOrEmpty((textToSpeak = (await zTextReader.GetTTCtext(true)).Trim())))
                {
                    loopCounter++;
                    zTextReader.MoveNext(true);
                    string bookShortName;
                    int relChaptNum;
                    int verseNum;
                    string fullName;
                    string title;
                    zTextReader.GetInfo(
                        this.currentlyPlaying.Language,
                        out bookShortName,
                        out relChaptNum,
                        out verseNum,
                        out fullName,
                        out title);
                    currentlyPlaying.Book = bookShortName;
                    currentlyPlaying.Chapter = relChaptNum;
                    currentlyPlaying.Verse = verseNum;
                }
                var stream = await synthesizer.SynthesizeTextToStreamAsync(textToSpeak);
                BackgroundMediaPlayer.Current.SetStreamSource(stream);
            }
            else
            {
                this.zTextReader = new PsuedoKjvBibleReader(
                    startPoint.Language,
                    startPoint.IsNtOnly);
                this.zTextReader.MoveChapterVerse(startPoint.Book, startPoint.Chapter, startPoint.Verse, false, this.zTextReader);
                BackgroundMediaPlayer.Current.SetUriSource(new Uri(startPoint.Src));
            }


            UpdateUVCOnNewTrack(true);
        }
        #endregion
    }

    class PsuedoKjvBibleReader : IBrowserTextSource
    {
        private string Language = string.Empty;
        private string Book = "Gen";
        private int Chapter = 0;
        private int Verse = 0;
        private Sword.BibleNames booknames = null;
        private Canon canon = null;
        private bool IsNtOnly = false;
        public PsuedoKjvBibleReader(string Language, bool IsNtOnly)
        {
            this.Language = Language;
            booknames = new Sword.BibleNames(Language, string.Empty);
            canon = CanonManager.GetCanon("KJV");
            this.IsNtOnly = IsNtOnly;
        }

        public void GetInfo(string isoLangCode, out string bookShortName, out int relChaptNum, out int verseNum, out string fullName, out string title)
        {
            bookShortName = this.Book;
            relChaptNum = this.Chapter;
            verseNum = this.Verse;
            fullName = string.Empty;
            title = string.Empty;
            try
            {
                fullName = GetFullName(bookShortName, isoLangCode);
                title = fullName + " " + (this.Chapter + 1);// + ":" + (verseNum + 1);
            }
            catch (Exception ee)
            {
                Debug.WriteLine("PsuedoKjvBibleReader.GetInfo; " + ee.Message);
            }
        }
        private string GetFullName(string bookShortName, string appChoosenIsoLangCode)
        {
            var book = canon.BookByShortName[bookShortName];
            return this.booknames.GetFullName(book.ShortName1, book.FullName);
        }

        public bool MoveChapterVerse(string bookShortName, int chapter, int verse, bool isLocalLinkChange, IBrowserTextSource source)
        {
            Book = bookShortName;
            Chapter = chapter;
            Verse = 0;
            return true;
        }

        public void MoveNext(bool isVerse)
        {
            addChapter(1);
        }
        private void addChapter(int valToAdd)
        {
            var canonKjv = CanonManager.GetCanon("KJV");
            var book = canonKjv.BookByShortName[this.Book];
            int adjustedChapter = book.VersesInChapterStartIndex + this.Chapter + valToAdd;
            var lastBook = canonKjv.NewTestBooks[canonKjv.NewTestBooks.Count() - 1];
            var lastOtBook = canonKjv.OldTestBooks[canonKjv.OldTestBooks.Count() - 1];
            var chaptersInOldTestement = lastOtBook.NumberOfChapters + lastOtBook.VersesInChapterStartIndex;
            var chaptersInBible = lastBook.NumberOfChapters + lastBook.VersesInChapterStartIndex;
            if (adjustedChapter >= chaptersInBible)
            {
                adjustedChapter = this.IsNtOnly ? chaptersInOldTestement : 0;
            }
            else if (adjustedChapter < 0 || (this.IsNtOnly && adjustedChapter < chaptersInOldTestement))
            {
                adjustedChapter = chaptersInBible - 1;
            }

            var adjustedBook = canonKjv.GetBookFromAbsoluteChapter(adjustedChapter);
            this.Book = adjustedBook.ShortName1;
            this.Chapter = adjustedChapter - adjustedBook.VersesInChapterStartIndex;
        }

        #region unused required functions

        public void MovePrevious(bool isVerse)
        {
            addChapter(-1);
        }

        public bool IsExternalLink
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public bool IsHearable
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public bool IsLocalChangeDuringLink
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public bool IsLocked
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public bool IsPageable
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public bool IsSearchable
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public bool IsSynchronizeable
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public bool IsTranslateable
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public bool IsTTChearable
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public Task<IBrowserTextSource> Clone()
        {
            throw new NotImplementedException();
        }

        public bool ExistsShortNames(string isoLangCode)
        {
            throw new NotImplementedException();
        }

        public ButtonWindowSpecs GetButtonWindowSpecs(int stage, int lastSelectedButton, string isoLangCode)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetChapterHtml(string isoLangCode, DisplaySettings displaySettings, HtmlColorRgba htmlBackgroundColor, HtmlColorRgba htmlForegroundColor, HtmlColorRgba htmlPhoneAccentColor, HtmlColorRgba htmlWordsOfChristColor, HtmlColorRgba[] htmlHighlightColor, double htmlFontSize, string fontFamily, bool isNotesOnly, bool addStartFinishHtml, bool forceReload, bool isSmallScreen)
        {
            throw new NotImplementedException();
        }

        public string GetExternalLink(DisplaySettings displaySettings)
        {
            throw new NotImplementedException();
        }

        public string GetLanguage()
        {
            throw new NotImplementedException();
        }

        public Task<object[]> GetTranslateableTexts(string isoLangCode, DisplaySettings displaySettings, string bibleToLoad)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetTTCtext(bool isVerseOnly)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetVerseTextOnly(DisplaySettings displaySettings, string bookName, int chapterNumber, int verseNum)
        {
            throw new NotImplementedException();
        }

        public Task<List<string>> MakeListDisplayText(string isoLangCode, DisplaySettings displaySettings, List<BiblePlaceMarker> listToDisplay)
        {
            throw new NotImplementedException();
        }

        public Task<string> PutHtmlTofile(string isoLangCode, DisplaySettings displaySettings, HtmlColorRgba htmlBackgroundColor, HtmlColorRgba htmlForegroundColor, HtmlColorRgba htmlPhoneAccentColor, HtmlColorRgba htmlWordsOfChristColor, HtmlColorRgba[] htmlHighlightColor, double htmlFontSize, string fontFamily, string fileErase, string filePath, bool forceReload)
        {
            throw new NotImplementedException();
        }

        public void RegisterUpdateEvent(WindowSourceChanged sourceChangedMethod, bool isRegister = true)
        {
            throw new NotImplementedException();
        }

        public Task Resume()
        {
            throw new NotImplementedException();
        }

        public void SerialSave()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
