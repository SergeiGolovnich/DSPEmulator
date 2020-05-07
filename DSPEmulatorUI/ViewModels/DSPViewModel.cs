using Caliburn.Micro;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Text;
using DSPEmulatorLibrary;

namespace DSPEmulatorUI.ViewModels
{
    public class DSPViewModel : Conductor<IScreen>.Collection.AllActive, IEffectProvider
    {
        public string ImagePath { get; } = "/Views/dsp_icon.png";
        public DSPViewModel()
        {
            DisplayName = "DSP";

            Items.Add(new DelayEffectViewModel());
            Items.Add(new EqualizerEffectViewModel());
        }

        public ISampleProvider EffectProvider(ISampleProvider sourceProvider)
        {
            ISampleProvider output = sourceProvider;
            foreach(IEffectProvider effect in Items)
            {
                output = effect.EffectProvider(output);
            }
            return output;
        }
    }
}
