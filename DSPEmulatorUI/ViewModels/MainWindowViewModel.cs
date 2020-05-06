using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using DSPEmulatorLibrary;

namespace DSPEmulatorUI.ViewModels
{
    public class MainWindowViewModel : Conductor<IScreen>.Collection.AllActive
    {
        public IScreen FilesView { get; } = new FilesViewModel();
        public IScreen DSPView { get; } = new DSPViewModel();
        public MainWindowViewModel(): base(true)
        {
            ((FilesViewModel)FilesView).StartProcessEvent += MainWindowViewModel_StartProcessEvent;
        }

        private void MainWindowViewModel_StartProcessEvent(object sender, EventArgs e)
        {
            foreach(string file in ((FilesViewModel)FilesView).Files)
            {
                DSPEmulator.ProcessFile(file, (DSPViewModel)DSPView, ((FilesViewModel)FilesView).OutputFolder);
            }
        }
    }
}
