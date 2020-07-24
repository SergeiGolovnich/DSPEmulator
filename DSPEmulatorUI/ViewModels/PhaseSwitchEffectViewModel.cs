using Caliburn.Micro;
using DSPEmulatorLibrary;
using DSPEmulatorLibrary.SampleProviders;
using NAudio.Wave;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DSPEmulatorUI.ViewModels
{
    [JsonObject(MemberSerialization.OptIn)]
    class PhaseSwitchEffectViewModel : Screen, IEffect
    {
        [JsonProperty()]
        public string EffectType { get; set; } = typeof(PhaseSwitchEffectViewModel).Name;
        public string EffectDisplayName => "Phase Switch";
        private PhaseSwitchSampleProvider phaseSwitchSampleProvider;
        private bool leftPhaseSwitched;
        private bool rightPhaseSwitched;
        [JsonProperty()]
        public bool LeftPhaseSwitched
        {
            get { return leftPhaseSwitched; }
            set 
            { 
                leftPhaseSwitched = value;

                if (phaseSwitchSampleProvider != null)
                {
                    phaseSwitchSampleProvider.LeftChannelPhaseSwitched = leftPhaseSwitched;
                }
            }
        }
        [JsonProperty()]
        public bool RightPhaseSwitched
        {
            get { return rightPhaseSwitched; }
            set 
            { 
                rightPhaseSwitched = value;

                if (phaseSwitchSampleProvider != null)
                {
                    phaseSwitchSampleProvider.RightChannelPhaseSwitched = rightPhaseSwitched;
                }
            }
        }
        public PhaseSwitchEffectViewModel() { }
        public PhaseSwitchEffectViewModel(JToken jToken)
        {
            LeftPhaseSwitched = jToken[nameof(LeftPhaseSwitched)].Value<bool>();
            RightPhaseSwitched = jToken[nameof(RightPhaseSwitched)].Value<bool>();
        }
        public ISampleProvider SampleProvider(ISampleProvider sourceProvider)
        {
            return phaseSwitchSampleProvider = new PhaseSwitchSampleProvider(sourceProvider) 
            { LeftChannelPhaseSwitched = LeftPhaseSwitched, RightChannelPhaseSwitched = RightPhaseSwitched };
        }
    }
}
