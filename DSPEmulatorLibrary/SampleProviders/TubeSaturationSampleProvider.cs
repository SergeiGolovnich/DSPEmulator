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
        private readonly ITubeSaturator saturator = new StaticWaveshaping();
        private float wetCoeff;
        public WaveFormat WaveFormat => source.WaveFormat;

        public float WetPercent { get
            {
                return wetCoeff * 100.0f;
            }
            set
            {
                wetCoeff = value / 100.0f;
            }
        }

        public TubeSaturationSampleProvider(ISampleProvider source, float wetPercent)
        {
            if (source.WaveFormat.Channels != 2) throw new InvalidOperationException("Not a stereo audio file.");

            this.source = source;
            WetPercent = wetPercent;
        }

        public int Read(float[] buffer, int offset, int sampleCount)
        {
            int samplesRead = source.Read(buffer, offset, sampleCount);

            float dryCoeff = 1.0f - wetCoeff;
            float drySample, wetSample;

            for (int n = 0; n < samplesRead; n++)
            {
                drySample = dryCoeff * buffer[offset + n];
                wetSample = wetCoeff * buffer[offset + n];

                wetSample = saturator.Saturate(wetSample);

                buffer[offset + n] = drySample + wetSample;
            }

            return samplesRead;
        }
    }
}
