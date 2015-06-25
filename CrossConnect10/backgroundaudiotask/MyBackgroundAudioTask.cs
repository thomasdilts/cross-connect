//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// This code is licensed under the MIT License (MIT).
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************

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
        private BibleZtextReader zTextReader = null;
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

        async void MediaEndedEvent(MediaPlayer mp, System.Object obj)
        {
            Debug.WriteLine("MediaEndedEvent ");
            if(string.IsNullOrEmpty(currentlyPlaying.VoiceName))
            {
                SkipToNext();
            }
            else
            {
                MoveToNextVerse();

                this.zTextReader.MoveNext(true);
                var stream = await synthesizer.SynthesizeTextToStreamAsync(await zTextReader.GetTTCtext(true));
                BackgroundMediaPlayer.Current.SetStreamSource(stream);
                UpdateUVCOnNewTrack(true);
            }
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
            Debug.WriteLine("MyBackgroundAudioTask " + sender.Task.TaskId + " Cancel Requested...");
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

                var book = booknames.GetFullName(currentlyPlaying.Book, currentlyPlaying.Book);

                smtc.DisplayUpdater.MusicProperties.Title = book + " " + (currentlyPlaying.Chapter + 1);
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
                AddChapter(currentlyPlaying, relativePostion);
                SetAudioStartPoint(currentlyPlaying);
                Debug.WriteLine("starting new track = " + currentlyPlaying.Src);
            }
        }

        private void MoveToNextVerse()
        {
            var canonKjv = CanonManager.GetCanon("KJV");
            var book = canonKjv.BookByShortName[currentlyPlaying.Book];
            int nextVerse = currentlyPlaying.Verse + 1;
            if (nextVerse >= canonKjv.VersesInChapter[book.VersesInChapterStartIndex + currentlyPlaying.Chapter])
            {
                AddChapter(currentlyPlaying, 1);
            }
            else
            {
                currentlyPlaying.Verse = nextVerse;
            }

        }

        private static CanonBookDef AddChapter(AudioModel info, int valToAdd)
        {
            var canonKjv = CanonManager.GetCanon("KJV");
            var book = canonKjv.BookByShortName[info.Book];
            int adjustedChapter = book.VersesInChapterStartIndex + info.Chapter + valToAdd;
            var lastBook = canonKjv.NewTestBooks[canonKjv.NewTestBooks.Count() - 1];
            var lastOtBook = canonKjv.OldTestBooks[canonKjv.OldTestBooks.Count() - 1];
            var chaptersInOldTestement = lastOtBook.NumberOfChapters + lastOtBook.VersesInChapterStartIndex;
            var chaptersInBible = lastBook.NumberOfChapters + lastBook.VersesInChapterStartIndex;
            if (adjustedChapter >= chaptersInBible)
            {
                adjustedChapter = info.IsNtOnly ? chaptersInOldTestement : 0;
            }
            else if (adjustedChapter < 0 || (info.IsNtOnly && adjustedChapter < chaptersInOldTestement))
            {
                adjustedChapter = chaptersInBible - 1;
            }

            var adjustedBook = canonKjv.GetBookFromAbsoluteChapter(adjustedChapter);
            info.Book = adjustedBook.ShortName1;
            info.Chapter = adjustedChapter - adjustedBook.VersesInChapterStartIndex;

            if (!string.IsNullOrEmpty(info.Pattern) && !string.IsNullOrEmpty(info.Code) && string.IsNullOrEmpty(info.VoiceName))
            {
                // http://www.cross-connect.se/bibles/talking/{key}/Bible_{key}_{booknum2d}_{chapternum3d}.mp3
                info.Src =
                    info.Pattern.Replace("{key}", info.Code)
                           .Replace("{booknum2d}", (adjustedBook.BookNum + 1).ToString("D2"))
                           .Replace("{chapternum3d}", (info.Chapter + 1).ToString("D3"));
            }

            if(!string.IsNullOrEmpty(info.VoiceName))
            {
                info.Verse = 0;
            }

            return adjustedBook;
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
            if(MessageService.TryParseMessage(e.Data, out skipPreviousMessage))
            {
                // User has chosen to skip track from app context.
                Debug.WriteLine("Skipping to previous");
                SkipToPrevious();
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
                SwordBookMetaData bookSelected = bookKeyValue.Value==null? bibles.InstalledBibles.First().Value : bookKeyValue.Value ;

                string bookPath =
                    bookSelected.GetCetProperty(ConfigEntryType.ADataPath).ToString().Substring(2);
                bool isIsoEncoding = !bookSelected.GetCetProperty(ConfigEntryType.Encoding).Equals("UTF-8");

                this.zTextReader = new BibleZtextReader(
                                    bookPath,
                                    ((Language)bookSelected.GetCetProperty(ConfigEntryType.Lang)).Code,
                                    isIsoEncoding,
                                    (string)bookSelected.GetCetProperty(ConfigEntryType.CipherKey),
                                    bookSelected.ConfPath,
                                    (string)bookSelected.GetCetProperty(ConfigEntryType.Versification));
                await this.zTextReader.Initialize();
                this.zTextReader.MoveChapterVerse(startPoint.Book, startPoint.Chapter, startPoint.Verse, false, zTextReader);
                if (synthesizer==null)
                {
                    synthesizer = new SpeechSynthesizer();
                }

                synthesizer.Voice = SpeechSynthesizer.AllVoices.First(p => p.DisplayName.Equals(startPoint.VoiceName));
                var textToSpeak = await zTextReader.GetTTCtext(true);
                var stream = await synthesizer.SynthesizeTextToStreamAsync(textToSpeak);
                BackgroundMediaPlayer.Current.SetStreamSource(stream);
            }
            else
            {
                BackgroundMediaPlayer.Current.SetUriSource(new Uri(startPoint.Src));
            }

            UpdateUVCOnNewTrack(true);
        }
        #endregion
    }
}
