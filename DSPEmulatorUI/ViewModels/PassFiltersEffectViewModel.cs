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
using System.Windows.Input;
using DSPEmulatorLibrary.SampleProviders.Utils;

namespace DSPEmulatorUI.ViewModels
{
    [JsonObject(MemberSerialization.OptIn)]
    class PassFiltersEffectViewModel : Screen, IEffectProvider
    {
        [JsonProperty()]
        public string EffectType { get; set; } = typeof(PassFiltersEffectViewModel).Name;
        private PassFiltersSampleProvider passFiltersSampleProvider;
        private int hpFreq = 40;
        private int lpFreq = 15000;
        private float hpQ = 0.75f;//gives the smoothest drop in amplitude
        private float lpQ = 0.75f;//gives the smoothest drop in amplitude
        private bool isHpEnabled = true;
        private bool isLpEnabled = true;
        private int freqChangeStep = 1;

        [JsonProperty()]
        public bool IsLpEnabled
        {
            get { return isLpEnabled; }
            set { isLpEnabled = value;
                passFiltersSampleProvider?.Update(HpBand, LpBand);
            }
        }

        [JsonProperty()]
        public bool IsHpEnabled
        {
            get { return isHpEnabled; }
            set { isHpEnabled = value;
                passFiltersSampleProvider?.Update(HpBand, LpBand);
            }
        }


        [JsonProperty()]
        public int HpFreq
        {
            get { return hpFreq; }
            set { 
                if (value < 1)
                {
                    hpFreq = 1;
                }
                else if (value > 20000)
                {
                    hpFreq = 20000;
                }
                else
                {
                    hpFreq = value;
                }
                NotifyOfPropertyChange(() => HpFreq);
                passFiltersSampleProvider?.Update(HpBand, LpBand);
            }
        }
        [JsonProperty()]
        public int LpFreq
        {
            get { return lpFreq; }
            set { 
                if (value < 1)
                {
                    lpFreq = 1;
                }
                else if (value > 20000)
                {
                    lpFreq = 20000;
                }
                else
                {
                    lpFreq = value;
                }
                NotifyOfPropertyChange(() => LpFreq);
                passFiltersSampleProvider?.Update(HpBand, LpBand);
            }
        }
        [JsonProperty()]
        public float HpQ
        {
            get { return hpQ; }
            set { hpQ = value;
                passFiltersSampleProvider?.Update(HpBand, LpBand);
            }
        }
        [JsonProperty()]
        public float LpQ
        {
            get { return lpQ; }
            set { lpQ = value;
                passFiltersSampleProvider?.Update(HpBand, LpBand);
            }
        }


        public PassFiltersEffectViewModel() { }
        public PassFiltersEffectViewModel(JToken jToken)
        {
            //LeftDelay = jToken[nameof(LeftDelay)].Value<double>();
            //RightDelay = jToken[nameof(RightDelay)].Value<double>();
        }

        public ISampleProvider SampleProvider(ISampleProvider sourceProvider)
        {
            return passFiltersSampleProvider = new PassFiltersSampleProvider(sourceProvider, HpBand, LpBand);
        }

        public EqualizerBand HpBand
        {
            get
            {
                if (!IsHpEnabled)
                    return null;

                return new EqualizerBand { Frequency = HpFreq, Bandwidth = HpQ };
            }
        }

        public EqualizerBand LpBand
        {
            get
            {
                if (!IsLpEnabled)
                    return null;

                return new EqualizerBand { Frequency = LpFreq, Bandwidth = LpQ };
            }
        }

        public void HpFreqChanged(KeyEventArgs e)
        {
            if (e.Key == Key.Down)
            {
                HpFreq -= freqChangeStep;
            }
            else if (e.Key == Key.Up)
            {
                HpFreq += freqChangeStep;
            }
        }

        public void LpFreqChanged(KeyEventArgs e)
        {
            if (e.Key == Key.Down)
            {
                LpFreq -= freqChangeStep;
            }
            else if (e.Key == Key.Up)
            {
                LpFreq += freqChangeStep;
            }
        }
    }
}
