using DSPEmulatorLibrary;
using System;
using System.Collections.Generic;
using System.Text;

namespace DSPEmulatorUI
{
    interface IEffect : IEffectProvider
    {
        public string EffectType { get; }
        public string EffectDisplayName { get; }
    }
}
