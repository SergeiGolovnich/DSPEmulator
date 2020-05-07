using System;
using System.Collections.Generic;
using System.Text;
using Caliburn.Micro;
using DSPEmulatorLibrary;
using NAudio.Wave;
using DSPEmulatorLibrary.SampleProviders.Utils;
using DSPEmulatorLibrary.SampleProviders;

namespace DSPEmulatorUI.ViewModels
{
    public class EqualizerEffectViewModel : Conductor<IScreen>.Collection.AllActive, IEffectProvider
    {
        public EqualizerEffectViewModel() : base(true)
        {
            Items.Add(new EqualizerChannelViewModel("Left Channel"));
            Items.Add(new EqualizerChannelViewModel("Right Channel"));
        }
        public ISampleProvider EffectProvider(ISampleProvider sourceProvider)
        {
            return new EqualizerSampleProvider(sourceProvider,
                ((EqualizerChannelViewModel)Items[0]).EqualizerBands.ToArray(),
                ((EqualizerChannelViewModel)Items[1]).EqualizerBands.ToArray());
        }
    }
}
