using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows.Input;
using Caliburn.Micro;
using DSPEmulatorLibrary.SampleProviders.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DSPEmulatorUI.ViewModels
{
    [JsonObject(MemberSerialization.OptIn)]
    public class EqualizerBandViewModel : Screen
    {
        private int freq = 1000;
        private float gain;
        private float q = 1.0f;
        private int freqChangeStep = 1;
        private float qChangeStep = 0.1f;

        [JsonProperty()]
        public int Freq
        {
            get => freq; 
            set
            {
                if(value < 20)
                {
                    freq = 20;
                }else if(value > 20000)
                {
                    freq = 20000;
                }
                else 
                {
                    freq = value;
                }
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
                if (value < 0)
                {
                    q = 0;
                }
                else
                {
                    q = value;
                }
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
            Freq = jToken[nameof(Freq)].Value<int>();
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

        public void FreqChanged(KeyEventArgs e)
        {
            if (e.Key == Key.Down)
            {
                Freq -= freqChangeStep;
            }
            else if (e.Key == Key.Up)
            {
                Freq += freqChangeStep;
            }
        }
        public void QChanged(KeyEventArgs e)
        {
            if (e.Key == Key.Down)
            {
                Q -= qChangeStep;
            }
            else if (e.Key == Key.Up)
            {
                Q += qChangeStep;
            }
        }
    }
}
