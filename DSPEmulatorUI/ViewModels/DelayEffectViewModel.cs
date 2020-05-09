using System;
using System.Collections.Generic;
using System.Text;
using Caliburn.Micro;
using DSPEmulatorUI;
using DSPEmulatorLibrary.SampleProviders;
using DSPEmulatorLibrary;
using NAudio.Wave;

namespace DSPEmulatorUI.ViewModels
{
    public class DelayEffectViewModel : Screen, IEffectProvider
    {
        public double LeftDelay { get; set; }
        public double RightDelay { get; set; }

        public DelayEffectViewModel()
        {

        }
        public ISampleProvider SampleProvider(ISampleProvider sourceProvider)
        {
            if(LeftDelay == 0 && RightDelay == 0)
            {
                return sourceProvider;
            }

            return new ChannelsDelaySampleProvider(sourceProvider) { LeftDelayMillisec = LeftDelay, RightDelayMillisec = RightDelay };
        }
    }
}
