using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Text;

namespace DSPEmulatorLibrary.SampleProviders
{
    class SampleProviderWrapper : ISampleProvider
    {
        public ISampleProvider SourceProvider { get; set; }
        public WaveFormat WaveFormat => SourceProvider.WaveFormat;
        public SampleProviderWrapper(ISampleProvider sourceProvider)
        {
            SourceProvider = sourceProvider;
        }
        public int Read(float[] buffer, int offset, int count)
        {
            return SourceProvider.Read(buffer, offset, count);
        }
    }
}
