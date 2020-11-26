using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DSPEmulatorUI.ViewModels
{
    public class PlayerViewModel : Screen
    {
        private AudioPlayer audioPlayer;
        private double positionPercent;
        private string title;

        public event EventHandler<string> NextAudioFileEvent;
        public event EventHandler<string> PreviousAudioFileEvent;

        public string Title { get => title; 
            set
            { 
                title = value;
                NotifyOfPropertyChange(() => Title);
            }
        }
        public double PositionPercent
        {
            get => positionPercent;
            set
            {
                positionPercent = value;

                NotifyOfPropertyChange(() => PositionPercent);
            }
        }

        public PlayerViewModel(AudioPlayer audioPlayer)
        {
            AudioPlayer = audioPlayer;
        }
        public AudioPlayer AudioPlayer
        {
            get => audioPlayer;
            set
            {
                audioPlayer = value;

                audioPlayer.PlaybackPositionPercentChanged += audioPlayer_PlaybackPositionPercentChanged;
                audioPlayer.AudioFileChanged += AudioPlayer_AudioFileChanged;
            }
        }

        private void AudioPlayer_AudioFileChanged(object sender, string e)
        {
            Title = Path.GetFileName(e);
        }

        private void audioPlayer_PlaybackPositionPercentChanged(object sender, double positionPercent)
        {
            PositionPercent = positionPercent;
        }

        public void PlayPauseButton()
        {
            if (audioPlayer.IsPlaying)
            {
                audioPlayer.Pause();
            }
            else
            {
                audioPlayer.Play();
            }
        }

        public void PreviousButton()
        {
            PreviousAudioFileEvent?.Invoke(this, audioPlayer.FilePath);
        }
        public void StopButton()
        {
            audioPlayer.Stop();

            PositionPercent = 0;
        }
        public void NextButton()
        {
            NextAudioFileEvent?.Invoke(this, audioPlayer.FilePath);
        }
    }
}
