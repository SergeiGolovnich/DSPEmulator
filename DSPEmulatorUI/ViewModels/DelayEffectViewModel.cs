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
        public double leftDelay { get; set; }
        public double rightDelay { get; set; }

        public DelayEffectViewModel()
        {

        }
        public ISampleProvider SampleProvider(ISampleProvider sourceProvider)
        {
            if(leftDelay == 0 && rightDelay == 0)
            {
                return sourceProvider;
            }

            return new ChannelsDelaySampleProvider(sourceProvider) { LeftDelayMillisec = leftDelay, RightDelayMillisec = rightDelay };
        }
    }
}
