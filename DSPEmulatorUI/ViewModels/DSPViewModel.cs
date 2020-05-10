﻿using Caliburn.Micro;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Text;
using DSPEmulatorLibrary;
using Newtonsoft.Json;
using System.Runtime.Serialization;
using Newtonsoft.Json.Linq;
using System.Linq;

namespace DSPEmulatorUI.ViewModels
{
    [Serializable()]
    public class DSPViewModel : Conductor<IScreen>.Collection.AllActive, IEffectProvider, ISerializable
    {
        public string ImagePath { get; } = "/Views/Icons/dsp_icon.png";
        public DSPViewModel()
        {
            DisplayName = "DSP";

            Items.Add(new DelayEffectViewModel());
            Items.Add(new EqualizerEffectViewModel());
        }

        public ISampleProvider SampleProvider(ISampleProvider sourceProvider)
        {
            ISampleProvider output = sourceProvider;
            foreach(IEffectProvider effect in Items)
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
                _ => throw new Exception("Unknown effect type."),
            };
            return effectObj;
        }
    }
}
