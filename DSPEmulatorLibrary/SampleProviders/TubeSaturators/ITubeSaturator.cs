using System;
using System.Collections.Generic;
using System.Text;

namespace DSPEmulatorLibrary.SampleProviders.TubeSaturators
{
    public interface ITubeSaturator
    {
        float Saturate(float sample);
    }
}
