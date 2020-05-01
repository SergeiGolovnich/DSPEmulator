using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Text;

namespace DSPEmulatorUI.ViewModels
{
    public class FilesViewModel : Screen
    {
        public string ImagePath { get; } = "/Views/files_icon.png";
        public FilesViewModel()
        {
            DisplayName = "Audio Files";
        }
        public BindableCollection<string> Files { get; set; } = new BindableCollection<string>();
        public string OutputFolder { get; set; } = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic);

        public void AddFiles()
        {
            var dialog = new Microsoft.Win32.OpenFileDialog();

            dialog.Filter = "Audio Files| *.flac, *.mp3";
            dialog.Title = "Select audio files";
            dialog.CheckFileExists = true;
            dialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic);
            dialog.Multiselect = true;

            var result = dialog.ShowDialog();
            if (result == true && dialog.FileNames.Length > 0)
            {
                foreach(string filePath in dialog.FileNames)
                {
                    if (!Files.Contains(filePath)){
                        Files.Add(filePath);
                    }
                }
            }
        }
    }
}
