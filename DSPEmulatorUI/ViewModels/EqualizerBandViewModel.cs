using System;
using System.Collections.Generic;
using System.Text;
using Caliburn.Micro;
using DSPEmulatorLibrary.SampleProviders.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DSPEmulatorUI.ViewModels
{
    [JsonObject(MemberSerialization.OptIn)]
    public class EqualizerBandViewModel : Screen
    {
        [JsonProperty()]
        public uint Freq { get; set; } = 1000;
        [JsonProperty()]
        public float Gain { get; set; }
        [JsonProperty()]
        public float Q { get; set; } = 1.0f;

        public event EventHandler RemoveBandEvent;
        public void RemoveBand()
        {
            RemoveBandEvent?.Invoke(this, null);
        }
        public EqualizerBandViewModel()
        {

        }
        public EqualizerBandViewModel(JToken jToken)
        {
            Freq = jToken[nameof(Freq)].Value<uint>();
            Gain = jToken[nameof(Gain)].Value<float>();
            Q = jToken[nameof(Q)].Value<float>();
        }
        public EqualizerBand EqualizerBand { get
            {
                return new EqualizerBand() { Frequency = Freq, Gain = Gain, Bandwidth = Q };
            } }
    }
}
