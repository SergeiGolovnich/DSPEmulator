using System;
using System.Collections.Generic;
using System.Text;

namespace DSPEmulator
{
    class EqualizerParams
    {
        public EqualizerParams()
        {
            LeftChannel = new List<EqualizerBand>();
            RightChannel = new List<EqualizerBand>();
        }

        public List<EqualizerBand> LeftChannel { get; set; }
        public List<EqualizerBand> RightChannel { get; set; }
    }
}
