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
        public ISampleProvider SampleProvider(ISampleProvider sourceProvider)
        {
            var eqParams = new EqualizerParams()
            {
                LeftChannel = ((EqualizerChannelViewModel)Items[0]).EqualizerBands,
                RightChannel = ((EqualizerChannelViewModel)Items[1]).EqualizerBands
            };

            if (eqParams.IsEmpty)
            {
                return sourceProvider;
            }

            float adjustVolume = calcAdjustVolumeFromEq(eqParams);
            var adjusted = new ChannelsVolumeSampleProvider(sourceProvider)
            {
                LeftChannelVolumeInDB = adjustVolume,
                RightChannelVolumeInDB = adjustVolume
            };

            return new EqualizerSampleProvider(adjusted, eqParams);
        }

        private float calcAdjustVolumeFromEq(EqualizerParams eqParams)
        {
            float maxEqGain = 0;
            foreach (EqualizerBand eb in eqParams.LeftChannel)
            {
                if (eb.Gain > 0) maxEqGain = Math.Max(eb.Gain, maxEqGain);
            }
            foreach (EqualizerBand eb in eqParams.RightChannel)
            {
                if (eb.Gain > 0) maxEqGain = Math.Max(eb.Gain, maxEqGain);
            }

            return -maxEqGain;
        }
    }
}
