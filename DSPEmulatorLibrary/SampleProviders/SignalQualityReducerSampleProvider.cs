using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Text;

namespace DSPEmulatorLibrary.SampleProviders
{
    public class SignalQualityReducerSampleProvider : ISampleProvider
    {
        private readonly ISampleProvider sourceProvider;
        private readonly int channels;
        private int bitDepth;
        private int sampleRate;

        public int BitDepth
        {
            get => bitDepth;
            set
            {
                if (value < 1)
                {
                    bitDepth = 1;
                }
                else if (value > 16)
                {
                    bitDepth = 16;
                }
                else
                {
                    bitDepth = value;
                }

            }
        }
        public int SampleRate { get => sampleRate;
            set 
            {
                if (value < 1)
                {
                    sampleRate = 1;
                }
                else if (value > 44100)
                {
                    sampleRate = 44100;
                }
                else
                {
                    sampleRate = value;
                }
            }
        }
        public WaveFormat WaveFormat => sourceProvider.WaveFormat;
        public SignalQualityReducerSampleProvider(ISampleProvider source)
        {
            sourceProvider = source;
            channels = sourceProvider.WaveFormat.Channels;
        }
        public int Read(float[] buffer, int offset, int count)
        {
            int samplesRead = sourceProvider.Read(buffer, offset, count);

            int shiftBits = 16 - bitDepth;

            int rateDecrease = WaveFormat.SampleRate / SampleRate;
            float prevLeftSample = 0, prevRightSample = 0;
            int leftCurrSampleIndex = 0, rightCurrSampleIndex = 0;

            for (int n = 0; n < samplesRead; n++)
            {
                #region depth reduction
                float float32 = buffer[offset + n];

                // clip check
                if (float32 > 1.0f)
                    float32 = 1.0f;
                if (float32 < -1.0f)
                    float32 = -1.0f;

                short int16 = (short)(float32 * 32767);

                int16 >>= shiftBits;

                int16 <<= shiftBits;

                float32 = int16 / 32768f;

                buffer[offset + n] = float32;
                #endregion

                #region sample rate reduction
                if(n % channels == 0)
                {
                    if(leftCurrSampleIndex % rateDecrease == 0)
                    {
                        prevLeftSample = buffer[offset + n];
                    }
                    else
                    {
                        buffer[offset + n] = prevLeftSample;
                    }
                    leftCurrSampleIndex++;
                }else if (n % channels == 1)
                {
                    if (rightCurrSampleIndex % rateDecrease == 0)
                    {
                        prevRightSample = buffer[offset + n];
                    }
                    else
                    {
                        buffer[offset + n] = prevRightSample;
                    }
                    rightCurrSampleIndex++;
                }
                #endregion
            }

            return samplesRead;
        }
    }
}
