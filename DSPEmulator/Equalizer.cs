using System;
using System.Collections.Generic;
using System.Text;
using NAudio.Wave;
using NAudio.Dsp;

namespace DSPEmulator
{
    class Equalizer : ISampleProvider
    {
        private readonly ISampleProvider sourceProvider;
        private readonly EqualizerBand[] leftBands, rightBands;
        private readonly BiQuadFilter[] leftFilters, rightFilters;
        private readonly int channels;
        private bool updated;

        public Equalizer(ISampleProvider sourceProvider, EqualizerParams eqParams) : 
            this(sourceProvider, eqParams.LeftChannel.ToArray(), eqParams.RightChannel.ToArray())
        {}
        public Equalizer(ISampleProvider sourceProvider, EqualizerBand[] leftBands, EqualizerBand[] rightBands)
        {
            this.sourceProvider = sourceProvider;
            this.rightBands = rightBands;
            this.leftBands = leftBands;

            channels = sourceProvider.WaveFormat.Channels;

            leftFilters = new BiQuadFilter[leftBands.Length];
            rightFilters = new BiQuadFilter[rightBands.Length];
            CreateFilters();
        }

        private void CreateFilters()
        {
            for (int bandIndex = 0; bandIndex < leftBands.Length; bandIndex++)
            {
                var band = leftBands[bandIndex];

                if (leftFilters[bandIndex] == null)
                    leftFilters[bandIndex] = BiQuadFilter.PeakingEQ(sourceProvider.WaveFormat.SampleRate, band.Frequency, band.Bandwidth, band.Gain);
                else
                    leftFilters[bandIndex].SetPeakingEq(sourceProvider.WaveFormat.SampleRate, band.Frequency, band.Bandwidth, band.Gain);
            }

            for (int bandIndex = 0; bandIndex < rightBands.Length; bandIndex++)
            {
                var band = rightBands[bandIndex];

                if (rightFilters[bandIndex] == null)
                    rightFilters[bandIndex] = BiQuadFilter.PeakingEQ(sourceProvider.WaveFormat.SampleRate, band.Frequency, band.Bandwidth, band.Gain);
                else
                    rightFilters[bandIndex].SetPeakingEq(sourceProvider.WaveFormat.SampleRate, band.Frequency, band.Bandwidth, band.Gain);
            }
        }

        public void Update()
        {
            updated = true;
            CreateFilters();
        }

        public WaveFormat WaveFormat => sourceProvider.WaveFormat;

        public int Read(float[] buffer, int offset, int count)
        {
            int samplesRead = sourceProvider.Read(buffer, offset, count);

            if (updated)
            {
                CreateFilters();
                updated = false;
            }

            for (int n = 0; n < samplesRead; n++)
            {
                int ch = n % channels;

                if(ch == 0)
                {
                    for (int band = 0; band < leftBands.Length; band++)
                    {
                        buffer[offset + n] = leftFilters[band].Transform(buffer[offset + n]);
                    }
                }
                else
                {
                    for (int band = 0; band < rightBands.Length; band++)
                    {
                        buffer[offset + n] = rightFilters[band].Transform(buffer[offset + n]);
                    }
                }
                
            }
            return samplesRead;
        }
    }
}
