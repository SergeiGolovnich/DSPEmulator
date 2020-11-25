using Caliburn.Micro;
using NAudio.Wave;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace DSPEmulatorUI.ViewModels
{
    [JsonObject(MemberSerialization.OptIn)]
    public class FilesViewModel : Screen
    {
        private AudioFileInfo _selectedFile;
        public PlayerViewModel PlayerViewModel { get; set; }

        public string ImagePath { get; } = "/Views/Icons/files_icon.png";
        [JsonProperty]
        public BindableCollection<AudioFileInfo> Files { get; set; } = new BindableCollection<AudioFileInfo>();
        public AudioFileInfo SelectedFile { get => _selectedFile; 
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
                    var files = AudioFileFinder.GetAllAudioFilesRecursively(filePath);
                    AddFiles(files);
                }
            }
        }

        private void AddFiles(List<AudioFileInfo> files)
        {
            Files.AddRange(files);
        }

        public void SetOutputFolder()
        {
            // Create a "Save As" dialog for selecting a directory (HACK)
            var dialog = new Microsoft.Win32.SaveFileDialog
            {
                InitialDirectory = OutputFolder, // Use current value for initial dir
                Title = "Select Output Directory", // instead of default "Save As"
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
                var items_copy = new List<AudioFileInfo>();

                foreach (var item in items)
                {
                    items_copy.Add((AudioFileInfo)item);
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

                if (SelectedFile != null)
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
                AudioFileInfo fileInfo = file.ToObject<AudioFileInfo>();
                Files.Add(fileInfo);
            }
        }

        public void Files_Drop(DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                var dropData = e.Data.GetData(DataFormats.FileDrop) as string[];

                foreach(string file in dropData)
                {
                    var files = AudioFileFinder.GetAllAudioFilesRecursively(file);

                    AddFiles(files);
                }
            }
        }
        public void Files_MouseDoubleClick(MouseButtonEventArgs e)
        {
            if(CanPreviewBtn)
            {
                PreviewBtn();
            }
        }

        public int IndexOfFilePath(string filePath)
        {
            for(int i = 0; i < Files.Count; i++)
            {
                if(filePath == Files[i].FullPath)
                {
                    return i;
                }
            }

            return -1;
        }
    }
}
