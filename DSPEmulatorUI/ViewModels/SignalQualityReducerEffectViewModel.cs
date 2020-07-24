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
    class SignalQualityReducerEffectViewModel : Screen, IEffect
    {
        [JsonProperty()]
        public string EffectType { get; set; } = typeof(SignalQualityReducerEffectViewModel).Name;
        public string EffectDisplayName => "Signal Quality Reducer";
        private SignalQualityReducerSampleProvider signalQualityReducerSampleProvider;
        private int bitDepth = 16;
        private int sampleRate = 44100;
        [JsonProperty()]
        public int BitDepth
        {
            get { return bitDepth; }
            set { 
                bitDepth = value;
                if(signalQualityReducerSampleProvider != null)
                {
                    signalQualityReducerSampleProvider.BitDepth = BitDepth;
                }
            }
        }
        [JsonProperty()]
        public int SampleRate
        {
            get { return sampleRate; }
            set
            {
                sampleRate = value;
                if (signalQualityReducerSampleProvider != null)
                {
                    signalQualityReducerSampleProvider.SampleRate = SampleRate;
                }
            }
        }
        public SignalQualityReducerEffectViewModel() { }
        public SignalQualityReducerEffectViewModel(JToken jToken) 
        {
            BitDepth = jToken[nameof(BitDepth)].Value<int>();
            SampleRate = jToken[nameof(SampleRate)].Value<int>();
        }
        public ISampleProvider SampleProvider(ISampleProvider sourceProvider)
        {
            return signalQualityReducerSampleProvider = new SignalQualityReducerSampleProvider(sourceProvider) 
            { 
                BitDepth = BitDepth,
                SampleRate = SampleRate
            };
        }
    }
}
