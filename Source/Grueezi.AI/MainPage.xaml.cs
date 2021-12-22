 //
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
// <code>
using System;
using System.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Microsoft.CognitiveServices.Speech;
using System.Threading.Tasks;
using Windows.Media.Playback;
using Windows.Storage;
using System.IO;
using Windows.Media.Core;
using System.Collections.Generic;
using Windows.UI.ViewManagement;
using Windows.Foundation;

namespace Grueezi.AI
{
    /// <summary>
    /// Startpage with Speech functionality.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        bool isStatusBarPanelVisible = false;
        bool isMicAvailable = false;
        private MediaPlayer mediaPlayer;
        // Creates an instance of a speech config with specified subscription key and service region.
        // Replace with your own subscription key and service region (e.g., "switzerlandnorth").
        SpeechConfig config = SpeechConfig.FromSubscription("YourSubscriptionKey", "YourServiceRegion");
        
        string lastText ="Dies ist ein Beispieltext. Hier können sie ihren Text eingeben und mit einer Schweizerstimme ausgeben lassen. Unten in der Liste sind einige Redewendungen in Deutsch. Sie können sich die schweizer Übersetzung ausgeben lassen.";
        List<Phrase> phrases;
        Voice SelectedVoice;
        public MainPage()
        {
            this.InitializeComponent();
            ApplicationView.PreferredLaunchViewSize = new Size(376.0, 752.0);
            ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.PreferredLaunchViewSize;
            TextboxPlayText.Text = lastText;

            this.mediaPlayer = new MediaPlayer();
            this.phrases = new List<Phrase>
            {
                new Phrase{De = "Rutsch mir den Buckel runter!",                           Ch = "Blas mer doch id Schueh!"               , MeaningDe="Ausruf, wenn man mit etwas nichts zu tun haben möchte. Ausdruck von Überdruss / Ablehnung"},
                new Phrase{De = "Du kannst mich mal!",                                     Ch =" Chasch mer am Ranze hange!"             , MeaningDe="Derbe Redensart für: Vergiss es! Du kannst mich mal! Leck mich doch!"},
                new Phrase{De = "Rutsch mir doch den Buckel runter.",                      Ch = "rutsch mer de Buggel abe"               , MeaningDe="Lass mich in Ruhe. Das interessiert mich nicht."},
                new Phrase{De = "Das Leben ist kein Zuckerschlecken.",                     Ch = "sLäbe isch kein Sugus"                  , MeaningDe="Soll heissen, dass du im Leben nicht immer alles geschenkt bekommst."},
                new Phrase{De = "Wer zuerst kommt, mahlt zuerst.",                         Ch = "s'het solang s'het"                     , MeaningDe="Nur solange Vorrat, wer zuerst kommt, mahlt zuerst."},
                new Phrase{De = "Lass gehen!",                                             Ch = "Hopp de Bäse"                           , MeaningDe="Los jetzt! Mach vorwärts!"},
                new Phrase{De = "Nur solange Vorrat, wer zuerst kommt, mahlt zuerst.",     Ch = "De Schneller isch de Gsch"              , MeaningDe="Nur solange Vorrat, wer zuerst kommt, mahlt zuerst."},
                new Phrase{De = "Es nützt nichts, wenn du alles verkompliziert.",          Ch = "Mach keis Büro u"                       , MeaningDe="Es nützt nichts, wenn du alles verkompliziert."},
                new Phrase{De = "Nützt es nichts, so schadet es auch nicht.",              Ch = "Nützt’s nüt so schadt’s"                , MeaningDe="Wir wissen zwar nicht, ob wir erfolgreich sein werden – probieren es aber trotzdem aus."},
                new Phrase{De = "Das leckt keine Ziege weg.",                              Ch = "Das schläckt kei Geiss"                 , MeaningDe="Das ist einfach so, dagegen ist nichts zu machen."},
                new Phrase{De = "Du kannst nicht den 5-Räppler (Münze) und das Weggli (Brötchen) haben.",             Ch = "Chasch nöd de Füfer und sWeggli ha"     , MeaningDe="Du musst dich jetzt entscheiden, du kannst nicht alles haben."},
                new Phrase{De = "Ich zeige dir, wo Bartli das Geld holt.",                 Ch = "Ich zeig der, wo de Bartli de Most holt", MeaningDe="Ich zeige dir, wo der Hammer hängt, wo es lang geht. Über die Herkunft des Sprichworts haben wir bereits einmal berichtet."},
                new Phrase{De = "Der Schlauere gibt nach, der Esel bleibt stehen.",        Ch = "De Gschider git na, de Esel blibt sta"  , MeaningDe="Es ist besser, von seinem Standpunkt abzurücken, wenn dadurch eine Einigung erzielt werden kann, statt stur daran festzuhalten"},
                new Phrase{De = "Jetzt ist genug Heu unten!",                              Ch = "s'isch gnueg Heu dund"                  , MeaningDe="Jetzt reicht es!"},

            };
            ListViewExamples.ItemsSource = this.phrases;

            //ComboboxPharses.ItemsSource = typeof(Phrase).GetProperties();
            foreach (var item in phrases)
            {
                //  ComboboxPharses.Items.Add(item);
            }

            var voices = new List<Voice>
            {
                new Voice{ VoiceName= "Leni", Image1 ="/Assets/Frau_blau.jpg", Image2 ="/Assets/Flagge.png", AzureKey = "de-CH-LeniNeural"},
                new Voice{ VoiceName= "Jan",  Image1 ="/Assets/Mann_blau.jpg", Image2 ="/Assets/Flagge.png", AzureKey = "de-CH-JanNeural"},
            };
            ComboboxSpeakerVoice.ItemsSource = voices;
            ComboboxSpeakerVoice.SelectedIndex= 0;

            


        }


        private async void EnableMicrophone_ButtonClicked(object sender, RoutedEventArgs e)
        {
            bool isMicAvailable = true;
            try
            {
                var mediaCapture = new Windows.Media.Capture.MediaCapture();
                var settings = new Windows.Media.Capture.MediaCaptureInitializationSettings();
                settings.StreamingCaptureMode = Windows.Media.Capture.StreamingCaptureMode.Audio;
                await mediaCapture.InitializeAsync(settings);
            }
            catch (Exception)
            {
                isMicAvailable = false;
            }
            if (!isMicAvailable)
            {
                await Windows.System.Launcher.LaunchUriAsync(new Uri("ms-settings:privacy-microphone"));
            }
            else
            {
                NotifyUser("Microphone was enabled", NotifyType.StatusMessage);
            }
        }

        private async void SpeechRecognitionFromMicrophone_ButtonClicked(object sender, RoutedEventArgs e)
        {
            if (!isMicAvailable)
            {
                bool isMicAvailable = true;
                try
                {
                    var mediaCapture = new Windows.Media.Capture.MediaCapture();
                    var settings = new Windows.Media.Capture.MediaCaptureInitializationSettings();
                    settings.StreamingCaptureMode = Windows.Media.Capture.StreamingCaptureMode.Audio;
                    await mediaCapture.InitializeAsync(settings);
                }
                catch (Exception)
                {
                    isMicAvailable = false;
                }
                if (!isMicAvailable)
                {
                    await Windows.System.Launcher.LaunchUriAsync(new Uri("ms-settings:privacy-microphone"));
                }
                else
                {
                    NotifyUser("Microphone was enabled", NotifyType.StatusMessage);
                }

            }
            try
            {
  
                // Creates a speech recognizer using microphone as audio input.
                using (var recognizer = new SpeechRecognizer(config, "de-CH"))
                {
                    // Starts speech recognition, and returns after a single utterance is recognized. The end of a
                    // single utterance is determined by listening for silence at the end or until a maximum of 15
                    // seconds of audio is processed.  The task returns the recognition text as result.
                    // Note: Since RecognizeOnceAsync() returns only a single utterance, it is suitable only for single
                    // shot recognition like command or query.
                    // For long-running multi-utterance recognition, use StartContinuousRecognitionAsync() instead.
                    var result = await recognizer.RecognizeOnceAsync().ConfigureAwait(false);

                    // Checks result.
                    StringBuilder sb = new StringBuilder();
                    if (result.Reason == ResultReason.RecognizedSpeech)
                    {
                        sb.AppendLine($"RECOGNIZED: Text={result.Text}");
                        lastText = result.Text;
                    }
                    else if (result.Reason == ResultReason.NoMatch)
                    {
                        sb.AppendLine($"NOMATCH: Speech could not be recognized.");
                        lastText = "No Match";
                    }
                    else if (result.Reason == ResultReason.Canceled)
                    {
                        var cancellation = CancellationDetails.FromResult(result);
                        sb.AppendLine($"CANCELED: Reason={cancellation.Reason}");
                        lastText = "Canceled because {cancellation.Reason}";
                        if (cancellation.Reason == CancellationReason.Error)
                        {
                            sb.AppendLine($"CANCELED: ErrorCode={cancellation.ErrorCode}");
                            sb.AppendLine($"CANCELED: ErrorDetails={cancellation.ErrorDetails}");
                            sb.AppendLine($"CANCELED: Did you update the subscription info?");
                        }
                    }

                    // Update the UI
                    NotifyUser(sb.ToString(), NotifyType.StatusMessage);
                }
            }
            catch(Exception ex)
            {
                NotifyUser($"Enable Microphone First.\n {ex.ToString()}", NotifyType.ErrorMessage);
            }
        }


        private async void TextToSpeechListItem_ButtonClicked(object sender, RoutedEventArgs e)
        {

            var s = (Phrase)ListViewExamples.SelectedItem;
            if (s == null)
            {
                return;
            }
            lastText = s.Ch;
            TextboxPlayText.Text = s.Ch;
            TextToSpeech_ButtonClicked(sender, e);

        }
        private async void TextToSpeech_ButtonClicked(object sender, RoutedEventArgs e)
        {
           
            config.SpeechSynthesisLanguage = "de-CH";
            config.SpeechSynthesisVoiceName = SelectedVoice.AzureKey;


            try
            {
                // Creates a speech synthesizer.
                using (var synthesizer = new SpeechSynthesizer(config, null))
                {
                    // Receive a text from TextForSynthesis text box and synthesize it to speaker.
                    using (var result = await synthesizer.SpeakTextAsync(lastText).ConfigureAwait(false))
                    {
                        // Checks result.
                        if (result.Reason == ResultReason.SynthesizingAudioCompleted)
                        {
                            NotifyUser($"Speech Synthesis Succeeded.", NotifyType.StatusMessage);

                            // Since native playback is not yet supported on UWP (currently only supported on Windows/Linux Desktop),
                            // use the WinRT API to play audio here as a short term solution.
                            using (var audioStream = AudioDataStream.FromResult(result))
                            {
                                // Save synthesized audio data as a wave file and use MediaPlayer to play it
                                var filePath = Path.Combine(ApplicationData.Current.LocalFolder.Path, "outputaudio.wav");
                                await audioStream.SaveToWaveFileAsync(filePath);
                                mediaPlayer.Source = MediaSource.CreateFromStorageFile(await StorageFile.GetFileFromPathAsync(filePath));
                                mediaPlayer.Play();
                            }
                        }
                        else if (result.Reason == ResultReason.Canceled)
                        {
                            var cancellation = SpeechSynthesisCancellationDetails.FromResult(result);

                            StringBuilder sb = new StringBuilder();
                            sb.AppendLine($"CANCELED: Reason={cancellation.Reason}");
                            sb.AppendLine($"CANCELED: ErrorCode={cancellation.ErrorCode}");
                            sb.AppendLine($"CANCELED: ErrorDetails=[{cancellation.ErrorDetails}]");

                            NotifyUser(sb.ToString(), NotifyType.ErrorMessage);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                NotifyUser($"{ex.ToString()}", NotifyType.ErrorMessage);
            }


        }


        private enum NotifyType
        {
            StatusMessage,
            ErrorMessage
        };

        private void NotifyUser(string strMessage, NotifyType type)
        {
            // If called from the UI thread, then update immediately.
            // Otherwise, schedule a task on the UI thread to perform the update.
            if (Dispatcher.HasThreadAccess)
            {
                UpdateStatus(strMessage, type);
            }
            else
            {
                var task = Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () => UpdateStatus(strMessage, type));
            }
        }

        private void UpdateStatus(string strMessage, NotifyType type)
        {
            switch (type)
            {
                case NotifyType.StatusMessage:
                    StatusBorder.Background = new SolidColorBrush(Windows.UI.Colors.Gray);
                    TextBlockRecognizedText.Text = lastText;
                    break;
                case NotifyType.ErrorMessage:
                    StatusBorder.Background = new SolidColorBrush(Windows.UI.Colors.Red);
                    break;
            }
            StatusBlock.Text += string.IsNullOrEmpty(StatusBlock.Text) ? strMessage : "\n" + strMessage;

            // Collapse the StatusBlock if it has no text to conserve real estate.
            StatusBorder.Visibility = !string.IsNullOrEmpty(StatusBlock.Text) ? Visibility.Visible : Visibility.Collapsed;
            if (isStatusBarPanelVisible && !string.IsNullOrEmpty(StatusBlock.Text))
            {
                StatusBorder.Visibility = Visibility.Visible;
                StatusPanel.Visibility = Visibility.Visible;
            }
            else
            {
                StatusBorder.Visibility = Visibility.Collapsed;
                StatusPanel.Visibility = Visibility.Collapsed;
            }

            // Raise an event if necessary to enable a screen reader to announce the status update.
            var peer = Windows.UI.Xaml.Automation.Peers.FrameworkElementAutomationPeer.FromElement(StatusBlock);
            if (peer != null)
            {
                peer.RaiseAutomationEvent(Windows.UI.Xaml.Automation.Peers.AutomationEvents.LiveRegionChanged);
            }
            var peer1 = Windows.UI.Xaml.Automation.Peers.FrameworkElementAutomationPeer.FromElement(TextBlockRecognizedText);
            if (peer1 != null)
            {
                peer.RaiseAutomationEvent(Windows.UI.Xaml.Automation.Peers.AutomationEvents.LiveRegionChanged);
            }
        }

       
        private void TextboxPlayText_TextChanged(object sender, TextChangedEventArgs e)
        {
            lastText = TextboxPlayText.Text;
        }

        private void ComboboxSpeakerVoice_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ComboboxSpeakerVoice.SelectedItem == null)
            {
                return;
            }
            SelectedVoice = (Voice)ComboboxSpeakerVoice.SelectedItem;
        }

        private void Image_DoubleTapped(object sender, Windows.UI.Xaml.Input.DoubleTappedRoutedEventArgs e)
        {
            isStatusBarPanelVisible = !isStatusBarPanelVisible;
            NotifyUser($"Status Panel visible = {isStatusBarPanelVisible}",NotifyType.StatusMessage);
        }
    }

   
}
// </code>
