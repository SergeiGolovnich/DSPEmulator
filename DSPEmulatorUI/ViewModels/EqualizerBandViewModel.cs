using System;
using System.Collections.Generic;
using System.Text;
using Caliburn.Micro;
using DSPEmulatorLibrary.SampleProviders.Utils;

namespace DSPEmulatorUI.ViewModels
{
    public class EqualizerBandViewModel : Screen
    {
        public uint Freq { get; set; } = 1000;

        public float Gain { get; set; }

        public float Q { get; set; } = 1.0f;

        public event EventHandler RemoveBandEvent;
        public void RemoveBand()
        {
            RemoveBandEvent?.Invoke(this, null);
        }

        public EqualizerBand EqualizerBand { get
            {
                return new EqualizerBand() { Frequency = Freq, Gain = Gain, Bandwidth = Q };
            } }
    }
}
