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
using DSPEmulatorUI.Views;
using System.ComponentModel;

namespace DSPEmulatorUI.ViewModels
{
    public class MainWindowViewModel : Conductor<object>.Collection.AllActive
    {
        public object FilesView { get => Items[0]; set { Items[0] = value; } }
        public object DSPView { get => Items[1]; set { Items[1] = value; } }

        public bool IsPlaying { get; set; } = false;
        private readonly IWavePlayer wavePlayer = new WaveOutEvent();

        private static BackgroundWorker backgroundWorker;

        public MainWindowViewModel()
        {
            Items.Add(new FilesViewModel());
            Items.Add(new DSPViewModel());

            subscribeToEvents();
        }

        private void subscribeToEvents()
        {
            ((FilesViewModel)FilesView).StartProcessEvent += MainWindowViewModel_StartProcessEvent;
            ((FilesViewModel)FilesView).PreviewPlayEvent += MainWindowViewModel_PreviewPlayEvent;
        }

        private void MainWindowViewModel_PreviewPlayEvent(object sender, EventArgs e)
        {
            if (IsPlaying || wavePlayer.PlaybackState == PlaybackState.Playing)
            {
                wavePlayer.Stop();

                IsPlaying = false;
            }
            else
            {
                try
                {
                    ISampleProvider audio = new AudioFileReader(((FilesViewModel)FilesView).SelectedFile);
                    audio = ((IEffectProvider)DSPView).SampleProvider(audio);

                    wavePlayer.Init(audio);
                    wavePlayer.Play();

                    IsPlaying = true;
                }
                catch(Exception ex)
                {
                    MessageBox.Show($"Error On Playing Preview: {ex.Message}", "Error");
                }
                
            }
        }

        private void MainWindowViewModel_StartProcessEvent(object sender, EventArgs e)
        {
            backgroundWorker = new BackgroundWorker
            {
                WorkerReportsProgress = true,
                WorkerSupportsCancellation = true
            };
            backgroundWorker.DoWork += BackgroundWorker_DoWork;

            ProgressView progress = new ProgressView(backgroundWorker);
            progress.ShowDialog();
        }

        private void BackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            int index = 1;
            foreach (string file in ((FilesViewModel)FilesView).Files)
            {
                if (backgroundWorker.CancellationPending)
                {
                    e.Cancel = true;
                    return;
                }

                try
                {
                    DSPEmulator.ProcessFile(file, (DSPViewModel)DSPView, ((FilesViewModel)FilesView).OutputFolder);
                }
                catch { }

                backgroundWorker.ReportProgress(index++ * 100 / ((FilesViewModel)FilesView).Files.Count);
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
                try
                {
                    Newtonsoft.Json.Linq.JToken jsonFilesView = JToken.FromObject((FilesViewModel)FilesView);
                    Newtonsoft.Json.Linq.JToken jsonDSPView = JToken.FromObject(((DSPViewModel)DSPView));

                    JObject session = new JObject();
                    session.Add("FilesView", jsonFilesView);
                    session.Add("DSPView", jsonDSPView);

                    string output = JsonConvert.SerializeObject(session);
                    File.WriteAllText(dialog.FileName, output);
                }catch(Exception ex)
                {
                    MessageBox.Show($"Error On Saving Session: {ex.Message}", "Error");
                }
                
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

                FilesViewModel newFilesView = new FilesViewModel();
                DSPViewModel newDSPView = new DSPViewModel();

                try
                {
                    newFilesView.Deserialize(jObject["FilesView"]);
                    newDSPView.Deserialize(jObject["DSPView"]);
                }
                catch(Exception ex)
                {
                    MessageBox.Show($"Error On Loading Session: {ex.Message}", "Error");
                    return;
                }

                Items.Clear();

                Items.Add(newFilesView);
                Items.Add(newDSPView);

                subscribeToEvents();
            }
        }
    }
}
