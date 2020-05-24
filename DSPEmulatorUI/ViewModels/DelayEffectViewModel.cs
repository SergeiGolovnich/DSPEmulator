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
        private ChannelsDelaySampleProvider delaySampleProvider;
        private double leftDelay;
        private double rightDelay;

        [JsonProperty()]
        public double LeftDelay { get => leftDelay; 
            set { 
                leftDelay = value;
                if(delaySampleProvider != null)
                {
                    delaySampleProvider.LeftDelayMillisec = leftDelay;
                }
            } }
        [JsonProperty()]
        public double RightDelay { get => rightDelay; 
            set { 
                rightDelay = value;
                if (delaySampleProvider != null)
                {
                    delaySampleProvider.RightDelayMillisec = rightDelay;
                }
            } }
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
            return delaySampleProvider = new ChannelsDelaySampleProvider(sourceProvider) { LeftDelayMillisec = LeftDelay, RightDelayMillisec = RightDelay };
        }
    }
}
