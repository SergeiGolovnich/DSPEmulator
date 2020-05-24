using Caliburn.Micro;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Text;
using DSPEmulatorLibrary;
using Newtonsoft.Json;
using System.Runtime.Serialization;
using Newtonsoft.Json.Linq;
using System.Linq;
using System.Windows.Input;
using DSPEmulatorUI.ViewModels;

namespace DSPEmulatorUI.ViewModels
{
    [Serializable()]
    public class DSPViewModel : Conductor<IScreen>.Collection.AllActive, IEffectProvider, ISerializable
    {
        private int selectedEffectIndex = 0;

        public string ImagePath { get; } = "/Views/Icons/dsp_icon.png";
        public int SelectedEffectIndex { get => selectedEffectIndex; 
            set { 
                selectedEffectIndex = value;
                NotifyOfPropertyChange(() => SelectedEffectIndex);
            } }
        public List<string> EffectsToAdd { get; set; } = new List<string>
        {
            "Add Effect",
            "Channels Delay",
            "Equalizer",
            "Channels Volume",
            "Signal Definition"
        };
        public DSPViewModel()
        {
            DisplayName = "DSP";

            Items.Add(new DelayEffectViewModel());
            Items.Add(new EqualizerEffectViewModel());
            Items.Add(new ChannelsVolumeEffectViewModel());
        }

        public ISampleProvider SampleProvider(ISampleProvider sourceProvider)
        {
            ISampleProvider output = sourceProvider;
            foreach (IEffectProvider effect in Items)
            {
                output = effect.SampleProvider(output);
            }
            return output;
        }

        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Items", Items);
        }

        public void Deserialize(JToken jsonToken)
        {
            Items.Clear();

            List<JToken> Effects = jsonToken["Items"].Children().ToList();

            foreach (JToken effect in Effects)
            {
                Items.Add((IScreen)DeserializeEffect(effect));
            }
        }

        private object DeserializeEffect(JToken jsonToken)
        {
            object effectObj = null;
            string type = jsonToken["EffectType"].Value<string>();

            effectObj = type switch
            {
                nameof(DelayEffectViewModel) => new DelayEffectViewModel(jsonToken),
                nameof(EqualizerEffectViewModel) => new EqualizerEffectViewModel(jsonToken),
                nameof(ChannelsVolumeEffectViewModel) => new ChannelsVolumeEffectViewModel(jsonToken),
                nameof(SignalDefinitionEffectViewModel) => new SignalDefinitionEffectViewModel(jsonToken),
                _ => throw new Exception("Unknown effect type."),
            };
            return effectObj;
        }

        public void RemoveSelectedEffect(IScreen item, KeyEventArgs e)
        {
            if (e.Key == Key.Delete && item != null)
            {
                Items.Remove(item);
            }
        }
        public void AddEffect(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return;
            }

            IScreen effect = name switch
            {
                "Channels Delay" => new DelayEffectViewModel(),
                "Equalizer" => new EqualizerEffectViewModel(),
                "Channels Volume" => new ChannelsVolumeEffectViewModel(),
                "Signal Definition" => new SignalDefinitionEffectViewModel(),
                _ => null
            };

            if (effect != null)
            {
                Items.Add(effect);
            }

            SelectedEffectIndex = 0;
        }
    }
}
