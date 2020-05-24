using Caliburn.Micro;
using DSPEmulatorLibrary;
using DSPEmulatorLibrary.SampleProviders;
using NAudio.Wave;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace DSPEmulatorUI.ViewModels
{
    [JsonObject(MemberSerialization.OptIn)]
    class ChannelsVolumeEffectViewModel : Screen, IEffectProvider
    {
        [JsonProperty()]
        public string EffectType { get; set; } = typeof(ChannelsVolumeEffectViewModel).Name;
        private ChannelsVolumeSampleProvider volumeSampleProvider;
        private float leftGain;
        private float rightGain;

        [JsonProperty()]
        public float LeftGain { get => leftGain; set
            {
                leftGain = value;
                if (volumeSampleProvider != null)
                {
                    volumeSampleProvider.LeftChannelVolumeInDB = LeftGain;
                }
            } 
        }
        [JsonProperty()]
        public float RightGain { get => rightGain; set
            {
                rightGain = value;
                if(volumeSampleProvider != null)
                {
                    volumeSampleProvider.RightChannelVolumeInDB = RightGain;
                }
            } }
        public ChannelsVolumeEffectViewModel() { }
        public ChannelsVolumeEffectViewModel(JToken jToken)
        {
            LeftGain = jToken[nameof(LeftGain)].Value<float>();
            RightGain = jToken[nameof(RightGain)].Value<float>();
        }
        public ISampleProvider SampleProvider(ISampleProvider sourceProvider)
        {
            return volumeSampleProvider = new ChannelsVolumeSampleProvider(sourceProvider)
            {
                LeftChannelVolumeInDB = LeftGain,
                RightChannelVolumeInDB = RightGain
            };
        }
    }
}
