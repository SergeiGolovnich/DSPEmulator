using Caliburn.Micro;
using DSPEmulatorLibrary;
using DSPEmulatorLibrary.SampleProviders;
using NAudio.Wave;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DSPEmulatorUI.ViewModels
{
    [JsonObject(MemberSerialization.OptIn)]
    class TubeSaturationEffectViewModel : Screen, IEffect
    {
        private TubeSaturationSampleProvider sampleProvider;
        private float wetPercent = 50.0f;

        [JsonProperty()]
        public string EffectType => typeof(TubeSaturationEffectViewModel).Name;
        [JsonProperty()]
        public float WetPercent { get => wetPercent; 
            set { 
                wetPercent = value;

                if(sampleProvider != null)
                {
                    sampleProvider.WetPercent = wetPercent;
                }
            }
        }
        public string EffectDisplayName => "Tube Saturation";

        public TubeSaturationEffectViewModel() { }
        public TubeSaturationEffectViewModel(JToken jToken)
        {
            WetPercent = jToken[nameof(WetPercent)].Value<float>();
        }

        public ISampleProvider SampleProvider(ISampleProvider sourceProvider)
        {
            return sampleProvider = new TubeSaturationSampleProvider(sourceProvider, wetPercent);
        }
    }
}
