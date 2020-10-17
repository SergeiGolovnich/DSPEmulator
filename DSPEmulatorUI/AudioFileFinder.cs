using System;
using System.Collections.Generic;
using System.IO;

namespace DSPEmulatorUI
{
    public static class AudioFileFinder
    {
        static readonly List<string> audioFileExtensions = new List<string>{ ".flac", ".mp3", ".wav", ".aiff", ".wma", ".aac", ".aiff", ".mp4" };
        public static List<AudioFileInfo> GetAllAudioFilesRecursively(string originFolder)
        {
            List<string> filesPaths = new List<string>();

            FindAudioFiles(originFolder, filesPaths);


            originFolder = Directory.GetParent(originFolder).FullName;

            List<AudioFileInfo> fileInfos = new List<AudioFileInfo>();
            foreach(var file in filesPaths)
            {
                fileInfos.Add(new AudioFileInfo { FullPath = file, OriginFolder = originFolder });
            }

            return fileInfos;
        }
        static void FindAudioFiles(string folder, List<string> filesPaths)
        {
            var directory = new DirectoryInfo(folder);

            if(!directory.Exists)
            {
                if (IsAudioFile(folder))
                {
                    filesPaths.Add(folder);
                }
                return;
            }

            foreach(var file in Directory.GetFiles(folder))
            {
                if (IsAudioFile(file))
                {
                    filesPaths.Add(file);
                }
            }

            foreach (var dir in Directory.GetDirectories(folder))
            {
                FindAudioFiles(dir, filesPaths);
            }
        }
        static bool IsAudioFile(string file)
        {
            return audioFileExtensions.Contains(Path.GetExtension(file));
        }
    }
}
