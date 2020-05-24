﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Caliburn.Micro;
using DSPEmulatorLibrary.SampleProviders.Utils;
using NAudio.Codecs;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DSPEmulatorUI.ViewModels
{
    [Serializable()]
    public class EqualizerChannelViewModel : Conductor<IScreen>.Collection.AllActive, ISerializable
    {
        [JsonProperty()]
        public string ChannelName { get; set; }
        public EqualizerChannelViewModel() : this("Channel")
        {
            
        }
        public EqualizerChannelViewModel(string name)
        {
            ChannelName = name;
        }
        public EqualizerChannelViewModel(Newtonsoft.Json.Linq.JToken jToken)
        {
            ChannelName = jToken[nameof(ChannelName)].Value<string>();

            foreach (var band in jToken["Items"].Children().ToList())
            {
                AddBand(band);
            }

            foreach(object band in Items)
            {
                ((EqualizerBandViewModel)band).PropertyChanged += EqualizerBandViewModel_PropertyChanged;
            }
        }

        private void EqualizerBandViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            NotifyOfPropertyChange(() => Items);
        }

        private void RemoveBandEvent(object sender, EventArgs e)
        {
            if(sender != null && sender is EqualizerBandViewModel)
            {
                Items.Remove((EqualizerBandViewModel)sender);
                ((EqualizerBandViewModel)sender).PropertyChanged -= EqualizerBandViewModel_PropertyChanged;
            }
        }

        public void AddBand(JToken jToken)
        {
            var band = new EqualizerBandViewModel(jToken);
            band.RemoveBandEvent += RemoveBandEvent;
            band.PropertyChanged += EqualizerBandViewModel_PropertyChanged;

            Items.Add(band);
        }
        public void AddBandBtn()
        {
            var band = new EqualizerBandViewModel();
            band.RemoveBandEvent += RemoveBandEvent;
            band.PropertyChanged += EqualizerBandViewModel_PropertyChanged;

            Items.Add(band);
        }

        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Items", Items);
            info.AddValue(nameof(ChannelName), ChannelName);
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
