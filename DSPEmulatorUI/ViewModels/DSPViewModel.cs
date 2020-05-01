using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Text;

namespace DSPEmulatorUI.ViewModels
{
    class DSPViewModel : Screen
    {
        public string ImagePath { get; } = "/Views/dsp_icon.png";
        public DSPViewModel()
        {
            DisplayName = "DSP";
        }
    }
}
