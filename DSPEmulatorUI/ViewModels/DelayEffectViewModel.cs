using System;
using System.Collections.Generic;
using System.Text;
using Caliburn.Micro;
using DSPEmulatorUI;
using DSPEmulatorLibrary.SampleProviders;
using DSPEmulatorLibrary;
using NAudio.Wave;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DSPEmulatorUI.ViewModels
{
    [JsonObject(MemberSerialization.OptIn)]
    public class DelayEffectViewModel : Screen, IEffectProvider
    {
        [JsonProperty()]
        public double LeftDelay { get; set; }
        [JsonProperty()]
        public double RightDelay { get; set; }
        [JsonProperty()]
        public string EffectType { get; set; } = typeof(DelayEffectViewModel).Name;

        public DelayEffectViewModel()
        {
        }

        public DelayEffectViewModel(JToken jToken)
        {
            LeftDelay = jToken[nameof(LeftDelay)].Value<double>();
            RightDelay = jToken[nameof(RightDelay)].Value<double>();
        }
        public ISampleProvider SampleProvider(ISampleProvider sourceProvider)
        {
            if(LeftDelay == 0 && RightDelay == 0)
            {
                return sourceProvider;
            }

            return new ChannelsDelaySampleProvider(sourceProvider) { LeftDelayMillisec = LeftDelay, RightDelayMillisec = RightDelay };
        }
    }
}
