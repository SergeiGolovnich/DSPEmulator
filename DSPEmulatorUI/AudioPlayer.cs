using DSPEmulatorLibrary;
using DSPEmulatorLibrary.SampleProviders;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Text;
using System.Timers;
using System.Windows;

namespace DSPEmulatorUI
{
    public class AudioPlayer
    {
        private Timer timer = new Timer(500);
        public bool IsPlaying { get => wavePlayer.PlaybackState == PlaybackState.Playing; }
        public bool IsPaused { get => wavePlayer.PlaybackState == PlaybackState.Paused; }
        private readonly IWavePlayer wavePlayer = new WaveOutEvent();
        private AudioFileReader audioFileReader;
        private SampleProviderWrapper sampleProviderWrapper = new SampleProviderWrapper(null);
        private IEffectProvider effects;
        public IEffectProvider Effects
        {
            get => effects;

            set
            {
                effects = value;

                if (IsPlaying)
                {
                    ISampleProvider audio = audioFileReader;

                    audio = Effects.SampleProvider(audio);

                    sampleProviderWrapper.SourceProvider = audio;
                }
            }
        }
        public string FilePath { get; set; }

        public event EventHandler PlaybackStopped;
        public event EventHandler<double> PlaybackPositionPercentChanged;
        public event EventHandler<string> AudioFileChanged;

        public double PositionPercent
        {
            get
            {
                if (audioFileReader == null)
                    return 0;

                return audioFileReader.Position * 100 / audioFileReader.Length;
            }
            set
            {
                if (audioFileReader == null)
                    return;

                audioFileReader.Position = (long)(value / 100.0f * audioFileReader.Length);
            }
        }
        public AudioPlayer()
        {
            wavePlayer.PlaybackStopped += WavePlayer_PlaybackStopped;
            timer.Elapsed += Timer_Elapsed;
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (IsPlaying)
            {
                PlaybackPositionPercentChanged?.Invoke(this, PositionPercent);
            }
        }

        private void WavePlayer_PlaybackStopped(object sender, StoppedEventArgs e)
        {
            PlaybackStopped?.Invoke(this, EventArgs.Empty);
        }

        public void Play()
        {
            try
            {
                if(wavePlayer.PlaybackState == PlaybackState.Stopped)
                {
                    if (string.IsNullOrWhiteSpace(FilePath)) return;

                    audioFileReader = new AudioFileReader(FilePath);

                    ISampleProvider audio = audioFileReader;

                    audio = Effects.SampleProvider(audio);

                    sampleProviderWrapper = new SampleProviderWrapper(audio);

                    wavePlayer.Init(sampleProviderWrapper);

                    AudioFileChanged?.Invoke(this, FilePath);
                }
                
                wavePlayer.Play();

                timer.Enabled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error On Playing Preview: {ex.Message}", "Error");
            }
        }
        public void Pause()
        {
            wavePlayer.Pause();

            timer.Enabled = false;
        }
        public void Stop()
        {
            wavePlayer.Stop();

            if(audioFileReader != null)
                audioFileReader.Position = 0;

            timer.Enabled = false;
        }
    }
}
