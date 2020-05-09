using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using DSPEmulatorLibrary;
using NAudio.Wave;

namespace DSPEmulatorUI.ViewModels
{
    public class MainWindowViewModel : Conductor<object>.Collection.AllActive
    {
        public object FilesView { get; } = new FilesViewModel();
        public object DSPView { get; } = new DSPViewModel();

        public bool IsPlaying { get; set; } = false;
        private readonly IWavePlayer wavePlayer = new WaveOutEvent();

        public MainWindowViewModel()
        {
            Items.Add(FilesView);
            Items.Add(DSPView);

            ((FilesViewModel)FilesView).StartProcessEvent += MainWindowViewModel_StartProcessEvent;

            ((FilesViewModel)FilesView).PreviewPlayEvent += MainWindowViewModel_PreviewPlayEvent;
        }

        private void MainWindowViewModel_PreviewPlayEvent(object sender, EventArgs e)
        {
            if (IsPlaying)
            {
                wavePlayer.Stop();

                IsPlaying = false;
            }
            else
            {
                ISampleProvider audio = new AudioFileReader(((FilesViewModel)FilesView).SelectedFile);
                audio = ((IEffectProvider)DSPView).SampleProvider(audio);

                wavePlayer.Init(audio);
                wavePlayer.Play();

                IsPlaying = true;
            }
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
