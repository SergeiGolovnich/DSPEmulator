using DSPEmulatorLibrary.SampleProviders.TubeSaturators;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Text;

namespace DSPEmulatorLibrary.SampleProviders
{
    public class TubeSaturationSampleProvider : ISampleProvider
    {
        private readonly ISampleProvider source;
        private readonly int channels;
        private ITubeSaturator saturator = new StaticWaveshapingType1();
        public WaveFormat WaveFormat => source.WaveFormat;

        public TubeSaturationSampleProvider(ISampleProvider source)
        {
            if (source.WaveFormat.Channels != 2) throw new InvalidOperationException("Not a stereo audio file.");

            this.source = source;
            channels = source.WaveFormat.Channels;
        }

        public int Read(float[] buffer, int offset, int sampleCount)
        {
            int samplesRead = source.Read(buffer, offset, sampleCount);

            for (int n = 0; n < sampleCount; n++)
            {
                int ch = n % channels;

                if (ch == 0)
                {
                    buffer[offset + n] = saturator.Saturate(buffer[offset + n]);
                }
                else
                {
                    buffer[offset + n] = saturator.Saturate(buffer[offset + n]);
                }
            }

            return samplesRead;
        }
    }
}
