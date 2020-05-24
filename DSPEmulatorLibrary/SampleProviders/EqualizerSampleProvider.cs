using NAudio.Wave;
using NAudio.Dsp;
using DSPEmulatorLibrary.SampleProviders.Utils;

namespace DSPEmulatorLibrary.SampleProviders
{
    public class EqualizerSampleProvider : ISampleProvider
    {
        private readonly ISampleProvider sourceProvider;
        private EqualizerBand[] leftBands, rightBands;
        private BiQuadFilter[] leftFilters, rightFilters;
        private readonly int channels;
        private bool updated;

        public EqualizerSampleProvider(ISampleProvider sourceProvider, EqualizerParams eqParams) : 
            this(sourceProvider, eqParams.LeftChannel.ToArray(), eqParams.RightChannel.ToArray())
        {}
        public EqualizerSampleProvider(ISampleProvider sourceProvider, EqualizerBand[] leftBands, EqualizerBand[] rightBands)
        {
            this.sourceProvider = sourceProvider;
            channels = sourceProvider.WaveFormat.Channels;

            this.rightBands = rightBands;
            this.leftBands = leftBands;

            CreateFilters();
        }

        private void CreateFilters()
        {
            leftFilters = new BiQuadFilter[leftBands.Length];
            rightFilters = new BiQuadFilter[rightBands.Length];

            for (int bandIndex = 0; bandIndex < leftBands.Length; bandIndex++)
            {
                var band = leftBands[bandIndex];
                
                leftFilters[bandIndex] = BiQuadFilter.PeakingEQ(sourceProvider.WaveFormat.SampleRate, band.Frequency, band.Bandwidth, band.Gain);
            }

            for (int bandIndex = 0; bandIndex < rightBands.Length; bandIndex++)
            {
                var band = rightBands[bandIndex];
                
                rightFilters[bandIndex] = BiQuadFilter.PeakingEQ(sourceProvider.WaveFormat.SampleRate, band.Frequency, band.Bandwidth, band.Gain);
            }
        }

        public void Update(EqualizerParams eqParams)
        {
            updated = true;

            leftBands = eqParams.LeftChannel.ToArray();
            rightBands = eqParams.RightChannel.ToArray();
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
                    for (int band = 0; band < leftFilters.Length; band++)
                    {
                        buffer[offset + n] = leftFilters[band].Transform(buffer[offset + n]);
                    }
                }
                else
                {
                    for (int band = 0; band < rightFilters.Length; band++)
                    {
                        buffer[offset + n] = rightFilters[band].Transform(buffer[offset + n]);
                    }
                }
                
            }
            return samplesRead;
        }
    }
}
