using Caliburn.Micro;
using NAudio.Wave;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace DSPEmulatorUI.ViewModels
{
    [JsonObject(MemberSerialization.OptIn)]
    public class FilesViewModel : Screen
    {
        private string _selectedFile;

        public string ImagePath { get; } = "/Views/Icons/files_icon.png";
        [JsonProperty]
        public BindableCollection<string> Files { get; set; } = new BindableCollection<string>();
        public string SelectedFile { get => _selectedFile; 
            set 
            {
                _selectedFile = value;
                NotifyOfPropertyChange(() => SelectedFile);
                NotifyOfPropertyChange(() => CanPreviewBtn);
            } 
        }
        [JsonProperty]
        public string OutputFolder { get; set; } = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic);

        public event EventHandler StartProcessEvent;
        public event EventHandler PreviewPlayEvent;
        public FilesViewModel()
        {
            DisplayName = "Audio Files";
        }

        public void AddFiles()
        {
            var dialog = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "Audio Files| *.flac;*.mp3",
                Title = "Select Audio Files",
                CheckFileExists = true,
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic),
                Multiselect = true
            };

            var result = dialog.ShowDialog();
            if (result == true && dialog.FileNames.Length > 0)
            {
                foreach (string filePath in dialog.FileNames)
                {
                    if (!Files.Contains(filePath))
                    {
                        Files.Add(filePath);
                    }
                }
            }
        }

        public void SetOutputFolder()
        {
            // Create a "Save As" dialog for selecting a directory (HACK)
            var dialog = new Microsoft.Win32.SaveFileDialog
            {
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic), // Use current value for initial dir
                Title = "Select a Directory", // instead of default "Save As"
                Filter = "Directory|*.this.directory", // Prevents displaying files
                FileName = "select" // Filename will then be "select.this.directory"
            };

            if (dialog.ShowDialog() == true)
            {
                string path = dialog.FileName;
                // Remove fake filename from resulting path
                path = path.Replace("\\select.this.directory", "");
                path = path.Replace(".this.directory", "");
                // If user has changed the filename, create the new directory
                if (!System.IO.Directory.Exists(path))
                {
                    System.IO.Directory.CreateDirectory(path);
                }
                // Our final value is in path
                OutputFolder = path;
                NotifyOfPropertyChange(() => OutputFolder);
            }
        }

        public void RemoveSelectedItems(IEnumerable<object> items, KeyEventArgs e)
        {
            if (e.Key == Key.Delete && items != null)
            {
                List<string> items_copy = new List<string>();
                foreach (var item in items)
                {
                    items_copy.Add((string)item);
                }

                Files.RemoveRange(items_copy);
            }
        }
        public void StartProcess()
        {
            StartProcessEvent?.Invoke(this, null);
        }

        public void PreviewBtn()
        {
            PreviewPlayEvent?.Invoke(this, null);
        }

        public bool CanPreviewBtn { 
            get
            {
                var output = false;

                if (!string.IsNullOrEmpty(SelectedFile))
                {
                    output = true;
                }

                return output;
            }
        }

        public void Deserialize(JToken jsonToken)
        {
            OutputFolder = jsonToken[nameof(OutputFolder)].Value<string>();
            NotifyOfPropertyChange(() => OutputFolder);

            List<JToken> files = jsonToken["Files"].Children().ToList();

            Files.Clear();

            foreach(JToken file in files)
            {
                Files.Add(file.Value<string>());
            }
        }

    }
}
