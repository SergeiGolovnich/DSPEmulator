using System;
using System.Collections.Generic;
using System.Text;

namespace DSPEmulatorLibrary.SampleProviders.TubeSaturators
{
    public class StaticWaveshaping : ITubeSaturator
    {
        public float Saturate(float sample)
        {
            return 3.0f * sample * 0.5f * (1 - sample * sample / 3.0f);
        }
    }
}
