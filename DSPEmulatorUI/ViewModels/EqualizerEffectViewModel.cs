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
        private EqualizerSampleProvider equalizerSampleProvider;
        private ChannelsVolumeSampleProvider adjustedSampleProvider;
        public EqualizerEffectViewModel()
        {
            Items.Add(new EqualizerChannelViewModel("Left Channel"));
            Items.Add(new EqualizerChannelViewModel("Right Channel"));

            Items[0].PropertyChanged += EqualizerChannelViewModel_PropertyChanged;
            Items[1].PropertyChanged += EqualizerChannelViewModel_PropertyChanged;
        }

        public EqualizerEffectViewModel(JToken jToken)
        {
            foreach(var channel in jToken["Items"].Children().ToList())
            {
                EqualizerChannelViewModel item = new EqualizerChannelViewModel(channel);
                item.PropertyChanged += EqualizerChannelViewModel_PropertyChanged;

                Items.Add(item);
            }
        }
        private void EqualizerChannelViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            EqualizerParams eqParams;
            try
            {
                eqParams = GetEqParams();
            }
            catch
            {
                return;
            }

            float adjustVolume = CalcAdjustVolumeFromEq(eqParams);
            if(adjustedSampleProvider != null)
            {
                adjustedSampleProvider.LeftChannelVolumeInDB = adjustVolume;
                adjustedSampleProvider.RightChannelVolumeInDB = adjustVolume;
            }

            equalizerSampleProvider?.Update(eqParams);
        }
        public ISampleProvider SampleProvider(ISampleProvider sourceProvider)
        {
            EqualizerParams eqParams = GetEqParams();

            float adjustVolume = CalcAdjustVolumeFromEq(eqParams);
            adjustedSampleProvider = new ChannelsVolumeSampleProvider(sourceProvider)
            {
                LeftChannelVolumeInDB = adjustVolume,
                RightChannelVolumeInDB = adjustVolume
            };

            return equalizerSampleProvider = new EqualizerSampleProvider(adjustedSampleProvider, eqParams);
        }

        private EqualizerParams GetEqParams()
        {
            if(Items.Count != 2)
            {
                throw new Exception("Must be 2 eq channels.");
            }

            return new EqualizerParams()
            {
                LeftChannel = ((EqualizerChannelViewModel)Items[0]).EqualizerBands,
                RightChannel = ((EqualizerChannelViewModel)Items[1]).EqualizerBands
            };
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
