using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using DSPEmulatorLibrary;
using NAudio.Wave;
using System.Windows;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;

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

        public void SaveSession()
        {
            var dialog = new Microsoft.Win32.SaveFileDialog
            {
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                Title = "Save Session",
                Filter = "DSP Session|*.dspsession",
                FileName = $"{DateTime.Now:Hmm_ddMMyy}"
            };

            if (dialog.ShowDialog() == true)
            {
                Newtonsoft.Json.Linq.JToken jsonFilesView = JToken.FromObject((FilesViewModel)FilesView);
                Newtonsoft.Json.Linq.JToken jsonDSPView = JToken.FromObject(((DSPViewModel)DSPView));

                JObject session = new JObject();
                session.Add("FilesView", jsonFilesView);
                session.Add("DSPView", jsonDSPView);

                string output = JsonConvert.SerializeObject(session);
                File.WriteAllText(dialog.FileName, output);
            }
        }

        public void LoadSession()
        {
            var dialog = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "DSP Session|*.dspsession",
                Title = "Load Session",
                CheckFileExists = true,
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                Multiselect = false
            };

            if (dialog.ShowDialog() == true)
            {
                if (IsPlaying)
                {
                    MainWindowViewModel_PreviewPlayEvent(null, null);
                }

                string input = File.ReadAllText(dialog.FileName);

                JObject jObject = JObject.Parse(input);

                ((FilesViewModel)FilesView).Deserialize(jObject["FilesView"]);
                ((DSPViewModel)DSPView).Deserialize(jObject["DSPView"]);
            }
        }
    }
}
