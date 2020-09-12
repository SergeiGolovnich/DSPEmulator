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

            var processedAudio = effects.SampleProvider(stereoFile);

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
            var resampled = resampleTo44100(audioToSave);

            MediaFoundationEncoder.EncodeToMp3(resampled, fileName, 320000);
        }

        private static IWaveProvider resampleTo44100(ISampleProvider audioFile)
        {
            if (audioFile.WaveFormat.SampleRate != 44100)
            {
                return new MediaFoundationResampler(audioFile.ToWaveProvider(), 44100);
            }

            return audioFile.ToWaveProvider();
        }
    }
}
