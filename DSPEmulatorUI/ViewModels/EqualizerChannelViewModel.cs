using System;
using System.Collections.Generic;
using System.Text;
using Caliburn.Micro;
using DSPEmulatorLibrary.SampleProviders.Utils;
using NAudio.Codecs;

namespace DSPEmulatorUI.ViewModels
{
    public class EqualizerChannelViewModel : Conductor<IScreen>.Collection.AllActive
    {
        public string ChannelName { get; set; }
        public EqualizerChannelViewModel() : this("Channel")
        {
            
        }
        public EqualizerChannelViewModel(string name)
        {
            ChannelName = name;
        }

        private void RemoveBandEvent(object sender, EventArgs e)
        {
            if(sender != null && sender is EqualizerBandViewModel)
            {
                Items.Remove((EqualizerBandViewModel)sender);
            }
        }

        public void AddBand()
        {
            var band = new EqualizerBandViewModel();
            band.RemoveBandEvent += RemoveBandEvent;

            Items.Add(band);
        }
        public List<EqualizerBand> EqualizerBands
        {
            get
            {
                var bands = new List<EqualizerBand>();
                foreach(object bandView in Items)
                {
                    bands.Add(
                        ((EqualizerBandViewModel)bandView).EqualizerBand
                        );
                }

                return bands;
            }
        }
    }
}
