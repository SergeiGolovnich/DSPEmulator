using NAudio.Wave;
using NAudio.Dsp;
using DSPEmulatorLibrary.SampleProviders.Utils;

namespace DSPEmulatorLibrary.SampleProviders
{
    public class PassFiltersSampleProvider : ISampleProvider
    {
        private readonly ISampleProvider sourceProvider;
        private EqualizerBand HighPass, LowPass;
        private BiQuadFilter HighPassFilterLeft, HighPassFilterRight, LowPassFilterLeft, LowPassFilterRight;
        private bool updated;

        public PassFiltersSampleProvider(ISampleProvider sourceProvider, EqualizerBand highPass, EqualizerBand lowPass)
        {
            this.sourceProvider = sourceProvider;

            this.HighPass = highPass;
            this.LowPass = lowPass;

            CreateFilters();
        }

        private void CreateFilters()
        {
            HighPassFilterLeft = HighPass == null ? null : BiQuadFilter.HighPassFilter(sourceProvider.WaveFormat.SampleRate, HighPass.Frequency, HighPass.Bandwidth);
            HighPassFilterRight = HighPass == null ? null : BiQuadFilter.HighPassFilter(sourceProvider.WaveFormat.SampleRate, HighPass.Frequency, HighPass.Bandwidth);

            LowPassFilterLeft = LowPass == null ? null : BiQuadFilter.LowPassFilter(sourceProvider.WaveFormat.SampleRate, LowPass.Frequency, LowPass.Bandwidth);
            LowPassFilterRight = LowPass == null ? null : BiQuadFilter.LowPassFilter(sourceProvider.WaveFormat.SampleRate, LowPass.Frequency, LowPass.Bandwidth);
        }

        public void Update(EqualizerBand highPass, EqualizerBand lowPass)
        {
            updated = true;

            HighPass = highPass;
            LowPass = lowPass;
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

            if(HighPass != null)
            {
                for (int n = 0; n < samplesRead; n += 2)
                {
                    buffer[offset + n] = HighPassFilterLeft.Transform(buffer[offset + n]);

                    buffer[offset + n + 1] = HighPassFilterRight.Transform(buffer[offset + n + 1]);
                }
            }

            if (LowPass != null)
            {
                for (int n = 0; n < samplesRead; n += 2)
                {
                    buffer[offset + n] = LowPassFilterLeft.Transform(buffer[offset + n]);

                    buffer[offset + n + 1] = LowPassFilterRight.Transform(buffer[offset + n + 1]);
                }
            }

            return samplesRead;
        }
    }
}
