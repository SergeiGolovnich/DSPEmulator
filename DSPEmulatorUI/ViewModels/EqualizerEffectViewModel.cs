using System;
using System.Collections.Generic;
using System.Text;
using Caliburn.Micro;
using DSPEmulatorLibrary;
using NAudio.Wave;
using DSPEmulatorLibrary.SampleProviders.Utils;
using DSPEmulatorLibrary.SampleProviders;
using Newtonsoft.Json;
using System.Runtime.Serialization;
using Newtonsoft.Json.Linq;
using System.Linq;

namespace DSPEmulatorUI.ViewModels
{
    [Serializable()]
    public class EqualizerEffectViewModel : Conductor<IScreen>.Collection.AllActive, IEffectProvider, ISerializable
    {
        public string EffectType { get; set; } = typeof(EqualizerEffectViewModel).Name;
        public EqualizerEffectViewModel()
        {
            Items.Add(new EqualizerChannelViewModel("Left Channel"));
            Items.Add(new EqualizerChannelViewModel("Right Channel"));
        }

        public EqualizerEffectViewModel(JToken jToken)
        {
            foreach(var channel in jToken["Items"].Children().ToList())
            {
                Items.Add(new EqualizerChannelViewModel(channel));
            }
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

            float adjustVolume = CalcAdjustVolumeFromEq(eqParams);
            var adjusted = new ChannelsVolumeSampleProvider(sourceProvider)
            {
                LeftChannelVolumeInDB = adjustVolume,
                RightChannelVolumeInDB = adjustVolume
            };

            return new EqualizerSampleProvider(adjusted, eqParams);
        }

        private float CalcAdjustVolumeFromEq(EqualizerParams eqParams)
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

        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Items", Items);
            info.AddValue("EffectType", EffectType);
        }
    }
}
