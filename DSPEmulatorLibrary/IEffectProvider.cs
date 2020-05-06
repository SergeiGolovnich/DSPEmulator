using System;
using System.Collections.Generic;
using System.Text;
using DSPEmulatorLibrary.SampleProviders;
using NAudio.Wave;

namespace DSPEmulatorLibrary
{
    public interface IEffectProvider
    {
        ISampleProvider EffectProvider(ISampleProvider sourceProvider);
    }
}
