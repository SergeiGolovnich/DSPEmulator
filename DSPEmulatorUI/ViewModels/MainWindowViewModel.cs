using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace DSPEmulatorUI.ViewModels
{
    public class MainWindowViewModel : Conductor<IScreen>.Collection.AllActive
    {
        public IScreen FilesView { get; } = new FilesViewModel();
        public MainWindowViewModel(): base(true)
        {

        }
         
    }
}
