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
    class SignalDefinitionEffectViewModel : Screen, IEffectProvider
    {
        [JsonProperty()]
        public string EffectType { get; set; } = typeof(SignalDefinitionEffectViewModel).Name;
        private SignalDefinitionSampleProvider signalDefinitionSampleProvider;
        private int definitionBits = 16;
        [JsonProperty()]
        public int DefinitionBits
        {
            get { return definitionBits; }
            set { 
                definitionBits = value;
                if(signalDefinitionSampleProvider != null)
                {
                    signalDefinitionSampleProvider.DefinitionBits = DefinitionBits;
                }
            }
        }
        public SignalDefinitionEffectViewModel() { }
        public SignalDefinitionEffectViewModel(JToken jToken) 
        {
            DefinitionBits = jToken[nameof(DefinitionBits)].Value<int>();
        }
        public ISampleProvider SampleProvider(ISampleProvider sourceProvider)
        {
            return signalDefinitionSampleProvider = new SignalDefinitionSampleProvider(sourceProvider) { DefinitionBits = DefinitionBits};
        }
    }
}
