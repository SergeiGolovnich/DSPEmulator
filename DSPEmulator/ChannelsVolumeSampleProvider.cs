using System;
using System.Collections.Generic;
using System.Text;
using NAudio.Wave;
using static NAudio.Utils.Decibels;

namespace DSPEmulator
{
    public class ChannelsVolumeSampleProvider : ISampleProvider
    {
        private readonly ISampleProvider source;
        private readonly int channels;

        private float leftVolume, rightVolume;

        public ChannelsVolumeSampleProvider(ISampleProvider source)
        {
            if (source.WaveFormat.Channels != 2) throw new InvalidOperationException("Not a stereo audio file.");

            this.source = source;
            channels = source.WaveFormat.Channels;

            leftVolume = 1f;
            rightVolume = 1f;
        }

        public WaveFormat WaveFormat => source.WaveFormat;

        public int Read(float[] buffer, int offset, int sampleCount)
        {
            int samplesRead = source.Read(buffer, offset, sampleCount);

            for (int n = 0; n < sampleCount; n++)
            {
                int ch = n % channels;

                if (ch == 0)
                {
                    buffer[offset + n] *= leftVolume;
                }
                else
                {
                    buffer[offset + n] *= rightVolume;
                }
            }

            return samplesRead;
        }

        public float LeftChannelVolumeInDB
        {
            get => (float)LinearToDecibels(leftVolume);
            set => leftVolume = (float)DecibelsToLinear(value);
        }
        public float RightChannelVolumeInDB
        {
            get => (float)LinearToDecibels(rightVolume);
            set => rightVolume = (float)DecibelsToLinear(value);
        }
    }
}
