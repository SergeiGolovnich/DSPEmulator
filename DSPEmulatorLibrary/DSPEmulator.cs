using System;
using System.Collections.Generic;
using System.Text;
using NAudio.Wave;
using NAudio.MediaFoundation;
using NAudio.Wave.SampleProviders;
using System.IO;

namespace DSPEmulatorLibrary
{
    public static class DSPEmulator
    {
        public static void ProcessFile(string inputFile, IEffectProvider effects, string outputFolder)
        {
            var audioFile = new AudioFileReader(inputFile);

            var stereoFile = convertToStereo(audioFile);

            var processedAudio = effects.EffectProvider(stereoFile);

            saveToMp3(Path.Combine(outputFolder, $"{Path.GetFileNameWithoutExtension(inputFile)}.mp3"),
                            processedAudio);
        }

        private static ISampleProvider convertToStereo(ISampleProvider audioFile)
        {
            if (audioFile.WaveFormat.Channels != 2)
            {
               return new MonoToStereoSampleProvider(audioFile);
            }

            return audioFile;
        }

        private static void saveToMp3(string fileName, ISampleProvider audioToSave)
        {
            MediaFoundationEncoder.EncodeToMp3(audioToSave.ToWaveProvider(), fileName, 320000);
        }
    }
}
