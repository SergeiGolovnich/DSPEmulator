using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Text;

namespace DSPEmulatorLibrary.SampleProviders
{
    public class PhaseSwitchSampleProvider : ISampleProvider
    {
        private readonly ISampleProvider source;
        public WaveFormat WaveFormat => source.WaveFormat;
        public bool LeftChannelPhaseSwitched { get; set; } = false;
        public bool RightChannelPhaseSwitched { get; set; } = false;

        public PhaseSwitchSampleProvider(ISampleProvider source)
        {
            if (source.WaveFormat.Channels != 2) throw new InvalidOperationException("Not a stereo audio file.");

            this.source = source;
        }

        public int Read(float[] buffer, int offset, int sampleCount)
        {
            int samplesRead = source.Read(buffer, offset, sampleCount);

            if (LeftChannelPhaseSwitched)
            {
                for (int n = 0; n < sampleCount; n += 2)
                {
                    buffer[offset + n] *= -1f;
                }
            }

            if (RightChannelPhaseSwitched)
            {
                for (int n = 1; n < sampleCount; n += 2)
                {
                    buffer[offset + n] *= -1f;
                }
            }

            return samplesRead;
        }
    }
}
