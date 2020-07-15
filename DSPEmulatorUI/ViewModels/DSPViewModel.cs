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
        private int selectedAddEffectIndex = 0;
        private IScreen selectedEffect;

        public string ImagePath { get; } = "/Views/Icons/dsp_icon.png";
        public IScreen SelectedEffect { get => selectedEffect;
            set
            {
                selectedEffect = value;
                NotifyOfPropertyChange(() => SelectedEffect);
                NotifyOfPropertyChange(() => CanMoveEffectDown);
                NotifyOfPropertyChange(() => CanMoveEffectUp);
            } 
        }
        public int SelectedAddEffectIndex
        {
            get => selectedAddEffectIndex;
            set
            {
                selectedAddEffectIndex = value;
                NotifyOfPropertyChange(() => SelectedAddEffectIndex);
            }
        }
        public List<string> EffectsToAdd { get; set; } = new List<string>
        {
            "Add Effect",
            "Channels Delay",
            "Equalizer",
            "Channels Volume",
            "Signal Quality Reducer",
            "Phase Switch",
            "Pass Filters"
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
                nameof(SignalQualityReducerEffectViewModel) => new SignalQualityReducerEffectViewModel(jsonToken),
                nameof(PhaseSwitchEffectViewModel) => new PhaseSwitchEffectViewModel(jsonToken),
                nameof(PassFiltersEffectViewModel) => new PassFiltersEffectViewModel(jsonToken),
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
                "Signal Quality Reducer" => new SignalQualityReducerEffectViewModel(),
                "Phase Switch" => new PhaseSwitchEffectViewModel(),
                "Pass Filters" => new PassFiltersEffectViewModel(),
                _ => null
            };

            if (effect != null)
            {
                Items.Add(effect);
            }

            SelectedAddEffectIndex = 0;
        }

        public void MoveEffectUp()
        {
            if(SelectedEffect != null && Items.IndexOf(SelectedEffect) != 0)
            {
                IScreen currentEffect = SelectedEffect;
                int currentEffectIndex = Items.IndexOf(SelectedEffect);

                Items.RemoveAt(currentEffectIndex);

                Items.Insert(currentEffectIndex - 1, currentEffect);

                SelectedEffect = currentEffect;
            }
        }
        public bool CanMoveEffectUp
        {
            get
            {
                if(selectedEffect == null)
                {
                    return false;
                }
                else if(Items.IndexOf(SelectedEffect) == 0)
                {
                    return false;
                }

                return true;
            }
        }
        public void MoveEffectDown()
        {
            if (SelectedEffect != null && Items.IndexOf(SelectedEffect) != (Items.Count - 1))
            {
                IScreen currentEffect = SelectedEffect;
                int currentEffectIndex = Items.IndexOf(SelectedEffect);

                Items.RemoveAt(currentEffectIndex);

                Items.Insert(currentEffectIndex + 1, currentEffect);

                SelectedEffect = currentEffect;
            }
        }
        public bool CanMoveEffectDown
        { 
            get
            {
                if (SelectedEffect == null)
                {
                    return false;
                }
                else if(Items.IndexOf(SelectedEffect) == (Items.Count - 1))
                {
                    return false;
                }

                return true;
            } 
        }
    }
}
