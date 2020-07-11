using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Windows;
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
        public void AddBand(EqualizerBand eqBand)
        {
            var band = new EqualizerBandViewModel();
            band.EqualizerBand = eqBand;

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

        public void ImportFromREW()
        {
            var dialog = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "Text Files|*.txt",
                Title = "Select Equalizer Settings File",
                CheckFileExists = true,
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                Multiselect = false
            };

            if (dialog.ShowDialog() == true)
            {
                string[] inputLines = File.ReadAllLines(dialog.FileName);

                List<EqualizerBand> parsedBands = new List<EqualizerBand>();

                foreach(var line in inputLines)
                {
                    try
                    {
                        parsedBands.Add(
                            parseEqBand(line.Split(' ').Where(x => !string.IsNullOrWhiteSpace(x)).ToArray())
                            );
                    }
                    catch { }
                }

                if(parsedBands.Count < 1)
                {
                    MessageBox.Show("Failed to import equalizer settings.", "Error");

                    return;
                }

                Items.Clear();
                foreach(var band in parsedBands)
                {
                    AddBand(band);
                }
            }

            EqualizerBand parseEqBand(string[] splittedLine)
            {
                if(splittedLine.Length < 12)
                {
                    throw new Exception("Can't parse eq band");
                }

                if(splittedLine[0].Contains("Filter") && splittedLine[2].Contains("ON") && splittedLine[3].Contains("PK"))
                {
                    float freq;
                    float.TryParse(splittedLine[5].Replace('.',','), out freq);

                    float gain;
                    float.TryParse(splittedLine[8].Replace('.', ','), out gain);

                    float bandwidth;
                    float.TryParse(splittedLine[11].Replace('.', ','), out bandwidth);

                    return new EqualizerBand
                    {
                        Frequency = freq,
                        Gain = gain,
                        Bandwidth = bandwidth
                    };
                }
                else
                {
                    throw new Exception("Can't parse eq band");
                }
            }
        }
    }
}
