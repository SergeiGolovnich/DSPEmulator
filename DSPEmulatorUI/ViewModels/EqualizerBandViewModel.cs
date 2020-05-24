using System;
using System.Collections.Generic;
using System.ComponentModel;
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
        private uint freq = 1000;
        private float gain;
        private float q = 1.0f;

        [JsonProperty()]
        public uint Freq
        {
            get => freq; 
            set
            {
                freq = value;
                NotifyOfPropertyChange(() => Freq);
            }
        }
        [JsonProperty()]
        public float Gain { 
            get => gain; 
            set
            {
                gain = value;
                NotifyOfPropertyChange(() => Gain);
            }
        }
        [JsonProperty()]
        public float Q { 
            get => q; 
            set 
            { 
                q = value;
                NotifyOfPropertyChange(() => Q);
            } 
        }
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
        public EqualizerBand EqualizerBand
        {
            get
            {
                return new EqualizerBand() { Frequency = Freq, Gain = Gain, Bandwidth = Q };
            }
        }
    }
}
