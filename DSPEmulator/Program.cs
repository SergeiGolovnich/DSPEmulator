using System;
using System.IO;
using System.Collections.Generic;
using NAudio;
using NAudio.Wave;
using NAudio.MediaFoundation;
using NAudio.Wave.SampleProviders;
using Newtonsoft.Json;

namespace DSPEmulator
{
    class Program
    {
        private static string outputDir = "OUTPUT";
        private static string equalizerFilename = "equalizer.json";
        public static EqualizerParams eqParams = null;
        private static double leftDelay = 0, rightDelay = 0;
        private static float volumeAdjustDB = 0;
        private static float leftVolumeDB = 0, rightVolumeDB = 0;
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("There is nothing to process...");
                Console.ReadLine();
            }
            else if(args.Length == 1 && (args[0] == "--create-equalizer"))
            {
                createEqualizerJson(equalizerFilename);

                Console.WriteLine("Done.");
                Console.ReadLine();
            }
            else
            {

                MediaFoundationApi.Startup();
                Directory.CreateDirectory(Path.Combine(Environment.CurrentDirectory, outputDir));

                readEqParams(equalizerFilename);
                calcAdjustVolumeFromEq(eqParams);
                getDelayValues();
                getChannelsVolume();

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

                        if (volumeAdjustDB < 0)
                        {
                            stereo = new ChannelsVolumeSampleProvider(stereo)
                            {
                                LeftChannelVolumeInDB = volumeAdjustDB,
                                RightChannelVolumeInDB = volumeAdjustDB
                            };
                        }

                        var resampled = stereo;
                        if (stereo.WaveFormat.SampleRate != 44100)
                        {
                            resampled = new WdlResamplingSampleProvider(stereo, 44100);
                        }

                        var processedAudio = ChannelsDelay(resampled, leftDelay, rightDelay);

                        processedAudio = eqParams == null ? processedAudio : EqualizeChannels(processedAudio);

                        processedAudio = leftVolumeDB == 0 && rightVolumeDB == 0 ? processedAudio : AdjustChannelsVolume(processedAudio);

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

        private static ChannelsVolumeSampleProvider AdjustChannelsVolume(ISampleProvider processedAudio)
        {
            return new ChannelsVolumeSampleProvider(processedAudio)
            {
                LeftChannelVolumeInDB = leftVolumeDB,
                RightChannelVolumeInDB = rightVolumeDB
            };
        }

        private static void getChannelsVolume()
        {
            try
            {
                Console.Write("Enter left channel volume in Decibels: ");
                leftVolumeDB = (float)Convert.ToDouble(Console.ReadLine().Replace('.', ','));

                Console.Write("Enter right channel volume in Decibels: ");
                rightVolumeDB = (float)Convert.ToDouble(Console.ReadLine().Replace('.', ','));

                if (leftVolumeDB > 12 || rightVolumeDB > 12)
                    throw new ArgumentOutOfRangeException("Too much gain! Maximum +12 decibels.");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine("Wrong input. Try again...");
                getChannelsVolume();
            }
        }

        private static void calcAdjustVolumeFromEq(EqualizerParams eqParams)
        {
            if (eqParams == null)
                return;

            float maxEqGain = 0;
            foreach(EqualizerBand eb in eqParams.LeftChannel)
            {
                if (eb.Gain > 0) maxEqGain = Math.Max(eb.Gain, maxEqGain);
            }
            foreach (EqualizerBand eb in eqParams.RightChannel)
            {
                if (eb.Gain > 0) maxEqGain = Math.Max(eb.Gain, maxEqGain);
            }

            volumeAdjustDB = -maxEqGain;
        }

        private static ISampleProvider EqualizeChannels(ISampleProvider audio)
        {
            return new Equalizer(audio, eqParams);
        }

        private static void readEqParams(string equalizerFilename)
        {
            if(File.Exists(Path.Combine(Environment.CurrentDirectory, equalizerFilename)))
            {
                try
                {
                    string jsonString = File.ReadAllText(Path.Combine(Environment.CurrentDirectory, equalizerFilename));
                    eqParams = JsonConvert.DeserializeObject<EqualizerParams>(jsonString);
                    Console.WriteLine("Equalizer params loaded.");
                }
                catch(Exception ex)
                {
                    Console.WriteLine("Can not load equalizer params. " + ex.Message);
                    eqParams = null;
                }
            }
            else
            {
                Console.WriteLine("Can not find equalizer params file.");
            }
            
        }

        private static void createEqualizerJson(string equalizerFilename)
        {
            var eqParams = new EqualizerParams();
            eqParams.LeftChannel.Add(new EqualizerBand
            {
                Bandwidth = 1.0f,
                Frequency = 1000f,
                Gain = 6f
            });
            eqParams.LeftChannel.Add(new EqualizerBand
            {
                Bandwidth = 1.0f,
                Frequency = 10000f,
                Gain = 6f
            });

            eqParams.RightChannel.Add(new EqualizerBand
            {
                Bandwidth = 1.0f,
                Frequency = 100f,
                Gain = 6f
            });
            eqParams.RightChannel.Add(new EqualizerBand
            {
                Bandwidth = 1.0f,
                Frequency = 6000f,
                Gain = 6f
            });

            string jsonString = JsonConvert.SerializeObject(eqParams);
            File.WriteAllText(Path.Combine(Environment.CurrentDirectory, equalizerFilename), jsonString);
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
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
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
