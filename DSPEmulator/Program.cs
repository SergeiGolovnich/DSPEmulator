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
        private static double leftDelay = 0, rightDelay = 0;
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("There is nothing to process...");
                Console.ReadLine();
            }
            else
            {

                MediaFoundationApi.Startup();
                Directory.CreateDirectory(Path.Combine(Environment.CurrentDirectory, outputDir));

                getDelayValues();

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

                        var resampled = stereo;
                        if (stereo.WaveFormat.SampleRate != 44100)
                        {
                            resampled = new WdlResamplingSampleProvider(stereo, 44100);
                        }

                        var processedAudio = ChannelsDelay(resampled, leftDelay, rightDelay);

                        saveToMp3(Path.Combine(Environment.CurrentDirectory, outputDir, $"{Path.GetFileNameWithoutExtension(arg)}.mp3"),
                            processedAudio);

                        Console.WriteLine("Success.");
                        Console.WriteLine("");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error on file: {Path.GetFileName(arg)}. {ex.Message}");
                    }
                }
                Console.WriteLine("Done.");
                Console.ReadLine();
            }
        }

        private static void getDelayValues()
        {
            try
            {
                Console.Write("Enter left channel delay in milliseconds: ");
                leftDelay = Convert.ToDouble(Console.ReadLine().Replace('.', ','));

                Console.Write("Enter right channel delay in milliseconds: ");
                rightDelay = Convert.ToDouble(Console.ReadLine().Replace('.', ','));

                if (leftDelay < 0 || rightDelay < 0)
                    throw new ArgumentOutOfRangeException("Delays can't be below zero.");
            }
            catch
            {
                Console.WriteLine("Wrong input. Try again...");
                getDelayValues();
            }
        }

        private static ISampleProvider LoadFile(string arg)
        {
            var audioFile = new AudioFileReader(arg);

            return audioFile;


        }

        private static ISampleProvider ChannelsDelay(ISampleProvider audio, double leftChannelDelayMillisec, double rightChannelDelayMillisec)
        {
            return new ChannelsDelaySampleProvider(audio)
            {
                LeftDelayMillisec = leftChannelDelayMillisec,
                RightDelayMillisec = rightChannelDelayMillisec
            };
        }

        private static void saveToMp3(string fileName, ISampleProvider audioToSave)
        {
            MediaFoundationEncoder.EncodeToMp3(audioToSave.ToWaveProvider(), fileName, 320000);
        }
    }
}
