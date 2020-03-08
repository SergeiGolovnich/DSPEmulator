using System;
using System.IO;
using System.Collections.Generic;
using NAudio;
using NAudio.Wave;
using NAudio.MediaFoundation;
using NAudio.Wave.SampleProviders;

namespace DSPEmulator
{
    class Program
    {
        private static string outputDir = "OUTPUT";
        static void Main(string[] args)
        {
            MediaFoundationApi.Startup();
            Directory.CreateDirectory(Path.Combine(Environment.CurrentDirectory, outputDir));

            foreach (var arg in args)
            {
                try
                {
                    Console.WriteLine($"Loading file {Path.GetFileName(arg)}.");
                    var loadedAudio = LoadFile(arg);

                    var stereo = loadedAudio;
                    if (loadedAudio.WaveFormat.Channels != 2)
                    {
                        stereo = new MonoToStereoSampleProvider(loadedAudio);
                    }

                    ISampleProvider resampled = stereo;
                    if(loadedAudio.WaveFormat.SampleRate != 44100)
                    {
                        Console.WriteLine($"Resampling to 44100...");
                        resampled = new WdlResamplingSampleProvider(loadedAudio, 44100);
                    }

                    Console.WriteLine("Processing...");
                    var processedAudio = ChannelsDelay(resampled, 1000, 0);

                    Console.WriteLine("Saving to mp3...");
                    saveToMp3(Path.Combine(Environment.CurrentDirectory, outputDir, $"{Path.GetFileNameWithoutExtension(arg)}.mp3"),
                        processedAudio);
                }
                catch(Exception ex)
                {
                    Console.WriteLine($"Error on file: {Path.GetFileName(arg)}. {ex.Message}");
                }
            }
            Console.WriteLine("Done.");
            Console.ReadLine();
        }


        private static ISampleProvider LoadFile(string arg)
        {
            var audioFile = new AudioFileReader(arg);

            Console.WriteLine($"{audioFile.WaveFormat.SampleRate} {audioFile.WaveFormat.BitsPerSample} {audioFile.WaveFormat.Channels}");

            return audioFile;


        }

        private static ISampleProvider ChannelsDelay(ISampleProvider audio, double leftChannelDelayMillisec, double rightChannelDelayMillisec)
        {
            return new ChannelsDelaySampleProvider(audio)
            {
                LeftDelay = TimeSpan.FromMilliseconds(leftChannelDelayMillisec),
                RightDelay = TimeSpan.FromMilliseconds(rightChannelDelayMillisec)
            };
        }

        private static void saveToMp3(string fileName, ISampleProvider audioToSave)
        {
            MediaFoundationEncoder.EncodeToMp3(audioToSave.ToWaveProvider(), fileName, 320000);
        }
    }
}
