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
        [JsonProperty()]
        public string EffectType => typeof(TubeSaturationEffectViewModel).Name;

        public string EffectDisplayName => "Tube Saturation";

        public TubeSaturationEffectViewModel() { }
        public TubeSaturationEffectViewModel(JToken jToken)
        {

        }

        public ISampleProvider SampleProvider(ISampleProvider sourceProvider)
        {
            throw new System.NotImplementedException();
        }
    }
}
