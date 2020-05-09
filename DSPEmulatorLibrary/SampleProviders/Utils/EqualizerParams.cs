using System;
using System.Collections.Generic;
using System.Text;

namespace DSPEmulatorLibrary.SampleProviders.Utils
{
    public class EqualizerParams
    {
        public List<EqualizerBand> LeftChannel { get; set; } = new List<EqualizerBand>();
        public List<EqualizerBand> RightChannel { get; set; } = new List<EqualizerBand>();
        public bool IsEmpty => LeftChannel.Count == 0 && RightChannel.Count == 0;
    }
}
