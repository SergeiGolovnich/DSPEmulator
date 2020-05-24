using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Text;

namespace DSPEmulatorLibrary.SampleProviders
{
    public class SignalDefinitionSampleProvider : ISampleProvider
    {
        private readonly ISampleProvider sourceProvider;
        private readonly int channels;
        private int definitionBits;

        public int DefinitionBits
        {
            get => definitionBits;
            set
            {
                if (value < 1)
                {
                    definitionBits = 1;
                }
                else if (value > 16)
                {
                    definitionBits = 16;
                }
                else
                {
                    definitionBits = value;
                }

            }
        }
        public WaveFormat WaveFormat => sourceProvider.WaveFormat;
        public SignalDefinitionSampleProvider(ISampleProvider source)
        {
            sourceProvider = source;
            channels = sourceProvider.WaveFormat.Channels;
        }
        public int Read(float[] buffer, int offset, int count)
        {
            int samplesRead = sourceProvider.Read(buffer, offset, count);

            for (int n = 0; n < samplesRead; n++)
            {
                float float32 = buffer[offset + n];

                // clip check
                if (float32 > 1.0f)
                    float32 = 1.0f;
                if (float32 < -1.0f)
                    float32 = -1.0f;

                short int16 = (short)(float32 * 32767);

                int16 >>= 16 - definitionBits;

                int16 <<= 16 - definitionBits;

                float32 = int16 / 32768f;

                buffer[offset + n] = float32;
            }

            return samplesRead;
        }
    }
}
