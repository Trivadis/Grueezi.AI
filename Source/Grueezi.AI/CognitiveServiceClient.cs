﻿

////
//// Copyright (c) Microsoft. All rights reserved.
//// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
////
//// <code>
//using System;
//using System.Text;
//using Windows.UI.Xaml;
//using Windows.UI.Xaml.Controls;
//using Windows.UI.Xaml.Media;
//using Microsoft.CognitiveServices.Speech;
//using System.Threading.Tasks;
//using Windows.Media.Playback;
//using Windows.Storage;
//using System.IO;
//using Windows.Media.Core;
//using Windows.UI.Core;

//namespace Grueezi.AI
//{
//    internal class CognitiveServiceClient
//    {
//        private CoreDispatcher dispatcher;
//        private MediaPlayer mediaPlayer;
//        SpeechConfig config = SpeechConfig.FromSubscription("ef6c8703033e47caae8a250ffe4ffac5", "switzerlandnorth");

//        public CognitiveServiceClient(CoreDispatcher dispatcher , MediaPlayer mediaPlayer)
//        {
//            this.dispatcher = dispatcher;
//            this.mediaPlayer = mediaPlayer;
//        }

//        internal async void Play(string lastText)
//        {
//            config.SpeechSynthesisLanguage = "de-CH";
//            config.SpeechSynthesisVoiceName = "de-CH-LeniNeural";


//            try
//            {
//                // Creates a speech synthesizer.
//                using (var synthesizer = new SpeechSynthesizer(config, null))
//                {
//                    // Receive a text from TextForSynthesis text box and synthesize it to speaker.
//                    using (var result = await synthesizer.SpeakTextAsync(lastText).ConfigureAwait(false))
//                    {
//                        // Checks result.
//                        if (result.Reason == ResultReason.SynthesizingAudioCompleted)
//                        {
//                            NotifyUser($"Speech Synthesis Succeeded.", NotifyType.StatusMessage);

//                            // Since native playback is not yet supported on UWP (currently only supported on Windows/Linux Desktop),
//                            // use the WinRT API to play audio here as a short term solution.
//                            using (var audioStream = AudioDataStream.FromResult(result))
//                            {
//                                // Save synthesized audio data as a wave file and use MediaPlayer to play it
//                                var filePath = Path.Combine(ApplicationData.Current.LocalFolder.Path, "outputaudio.wav");
//                                await audioStream.SaveToWaveFileAsync(filePath);
//                                mediaPlayer.Source = MediaSource.CreateFromStorageFile(await StorageFile.GetFileFromPathAsync(filePath));
//                                mediaPlayer.Play();
//                            }
//                        }
//                        else if (result.Reason == ResultReason.Canceled)
//                        {
//                            var cancellation = SpeechSynthesisCancellationDetails.FromResult(result);

//                            StringBuilder sb = new StringBuilder();
//                            sb.AppendLine($"CANCELED: Reason={cancellation.Reason}");
//                            sb.AppendLine($"CANCELED: ErrorCode={cancellation.ErrorCode}");
//                            sb.AppendLine($"CANCELED: ErrorDetails=[{cancellation.ErrorDetails}]");

//                            NotifyUser(sb.ToString(), NotifyType.ErrorMessage);
//                        }
//                    }
//                }
//            }
//            catch (Exception ex)
//            {
//                NotifyUser($"{ex.ToString()}", NotifyType.ErrorMessage);
//            }

//        }

//        private enum NotifyType
//        {
//            StatusMessage,
//            ErrorMessage
//        };

//        private void NotifyUser(string strMessage, NotifyType type)
//        {
//            // If called from the UI thread, then update immediately.
//            // Otherwise, schedule a task on the UI thread to perform the update.
//            if (Dispatcher.HasThreadAccess)
//            {
//                UpdateStatus(strMessage, type);
//            }
//            else
//            {
//                var task = Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () => UpdateStatus(strMessage, type));
//            }
//        }

//        private void UpdateStatus(string strMessage, NotifyType type)
//        {
//            switch (type)
//            {
//                case NotifyType.StatusMessage:
//                    StatusBorder.Background = new SolidColorBrush(Windows.UI.Colors.Gray);
//                    break;
//                case NotifyType.ErrorMessage:
//                    StatusBorder.Background = new SolidColorBrush(Windows.UI.Colors.Red);
//                    break;
//            }
//            StatusBlock.Text += string.IsNullOrEmpty(StatusBlock.Text) ? strMessage : "\n" + strMessage;

//            // Collapse the StatusBlock if it has no text to conserve real estate.
//            StatusBorder.Visibility = !string.IsNullOrEmpty(StatusBlock.Text) ? Visibility.Visible : Visibility.Collapsed;
//            if (!string.IsNullOrEmpty(StatusBlock.Text))
//            {
//                StatusBorder.Visibility = Visibility.Visible;
//                StatusPanel.Visibility = Visibility.Visible;
//            }
//            else
//            {
//                StatusBorder.Visibility = Visibility.Collapsed;
//                StatusPanel.Visibility = Visibility.Collapsed;
//            }
//            // Raise an event if necessary to enable a screen reader to announce the status update.
//            var peer = Windows.UI.Xaml.Automation.Peers.FrameworkElementAutomationPeer.FromElement(StatusBlock);
//            if (peer != null)
//            {
//                peer.RaiseAutomationEvent(Windows.UI.Xaml.Automation.Peers.AutomationEvents.LiveRegionChanged);
//            }
//        }


//    }
//}