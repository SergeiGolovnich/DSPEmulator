﻿using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Input;

namespace DSPEmulatorUI.ViewModels
{
    public class FilesViewModel : Screen
    {
        public string ImagePath { get; } = "/Views/files_icon.png";
        public BindableCollection<string> Files { get; set; } = new BindableCollection<string>();
        public string OutputFolder { get; set; } = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic);
        public event EventHandler StartProcessEvent;
        public FilesViewModel()
        {
            DisplayName = "Audio Files";
        }

        public void AddFiles()
        {
            var dialog = new Microsoft.Win32.OpenFileDialog();

            dialog.Filter = "Audio Files| *.flac;*.mp3";
            dialog.Title = "Select Audio Files";
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

        public void SetOutputFolder()
        {
            // Create a "Save As" dialog for selecting a directory (HACK)
            var dialog = new Microsoft.Win32.SaveFileDialog();

            dialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic); // Use current value for initial dir
            dialog.Title = "Select a Directory"; // instead of default "Save As"
            dialog.Filter = "Directory|*.this.directory"; // Prevents displaying files
            dialog.FileName = "select"; // Filename will then be "select.this.directory"
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
                foreach(var item in items)
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
    }
}