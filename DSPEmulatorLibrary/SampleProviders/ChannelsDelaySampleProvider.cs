using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Text;


namespace DSPEmulatorLibrary.SampleProviders
{
    public class ChannelsDelaySampleProvider : ISampleProvider
    {
        private readonly ISampleProvider sourceProvider;
        private float[] sourceBuffer;
        private List<float> leftChannel = new List<float>(),
            rightChannel = new List<float>();
        private List<float> leftChannelPreviousSamples = new List<float>(),
            rightChannelPreviousSamples = new List<float>();
        private int leftDelayBySamples;
        private int rightDelayBySamples;

        public ChannelsDelaySampleProvider(ISampleProvider sourceProvider)
        {
            if (sourceProvider.WaveFormat.Channels != 2) throw new InvalidOperationException("Not a stereo audio file.");

            this.sourceProvider = sourceProvider;
        }
        public double LeftDelayMillisec
        {
            get { return SamplesToDoubleMillisec(leftDelayBySamples); }
            set { leftDelayBySamples = MillisecToSamples(value); }
        }

        public double RightDelayMillisec
        {
            get { return SamplesToDoubleMillisec(rightDelayBySamples); }
            set { rightDelayBySamples = MillisecToSamples(value); }
        }

        public int LeftDelayBySamples
        {
            get { return leftDelayBySamples; }
            set { leftDelayBySamples = value; }
        }

        public int RightDelayBySamples
        {
            get { return rightDelayBySamples; }
            set { rightDelayBySamples = value; }
        }

        private int MillisecToSamples(double time)
        {
            var samples = (int)(time / 1000 * WaveFormat.SampleRate);
            return samples;
        }

        private double SamplesToDoubleMillisec(int samples)
        {
            return (samples / (double)WaveFormat.SampleRate) * 1000;
        }

        public WaveFormat WaveFormat => sourceProvider.WaveFormat;

        public int Read(float[] buffer, int offset, int count)
        {
            EnsureSourceBuffer(count);
            var samlesReadedFromSource = sourceProvider.Read(sourceBuffer, offset, count);

            if (isEndOfSource(samlesReadedFromSource)) return 0;

            channelize(sourceBuffer, samlesReadedFromSource);

            delayChannel(leftChannel, leftDelayBySamples);
            delayChannel(rightChannel, rightDelayBySamples);

            insertFromPrevious(leftChannel, leftChannelPreviousSamples);
            insertFromPrevious(rightChannel, rightChannelPreviousSamples);

            if (samlesReadedFromSource != 0)
            {
                fillPreviousSamples(leftChannel, leftChannelPreviousSamples, leftDelayBySamples);
                fillPreviousSamples(rightChannel, rightChannelPreviousSamples, rightDelayBySamples);

                leftChannel.RemoveRange(leftChannel.Count - leftDelayBySamples, leftDelayBySamples);
                rightChannel.RemoveRange(rightChannel.Count - rightDelayBySamples, rightDelayBySamples);
            }

            var stereoBuffer = channelsToBuffer(leftChannel, rightChannel);

            for (int i = 0; i < stereoBuffer.Length; ++i)
            {
                buffer[offset + i] = stereoBuffer[offset + i];
            }

            return stereoBuffer.Length;
        }

        private bool isEndOfSource(int samlesReadedFromSource)
        {
            return Math.Max(leftChannelPreviousSamples.Count, rightChannelPreviousSamples.Count) == 0 && samlesReadedFromSource == 0;
        }

        private void EnsureSourceBuffer(int count)
        {
            if (sourceBuffer == null || sourceBuffer.Length < count)
            {
                sourceBuffer = new float[count];
            }
        }

        private float[] channelsToBuffer(List<float> leftChannel, List<float> rightChannel)
        {
            var resultBuffer = new List<float>(Math.Max(leftChannel.Count, rightChannel.Count) * 2);

            for(int i = 0; i < Math.Max(leftChannel.Count, rightChannel.Count); ++i)
            {
                if(i < leftChannel.Count)
                resultBuffer.Add(leftChannel[i]);
                else resultBuffer.Add(0);

                if(i < rightChannel.Count)
                resultBuffer.Add(rightChannel[i]);
                else resultBuffer.Add(0);
            }

            return resultBuffer.ToArray();
        }

        private void fillPreviousSamples(List<float> list, List<float> previousList, int endSamples)
        {
            for (int i = list.Count - endSamples; i < list.Count; ++i)
            {
                previousList.Add(list[i]);
            }
        }

        private void insertFromPrevious(List<float> list, List<float> previousList)
        {
            for (int i = 0; i < previousList.Count; ++i)
            {
                list[i] = previousList[i];
            }
            previousList.Clear();
        }

        private void delayChannel(List<float> list, int samples)
        {
           for(int i = 0; i < samples; ++i)
            {
                list.Insert(0, 0);
            }
        }

        private void channelize(float[] sourceBuffer, int length)
        {
            leftChannel.Clear();
            rightChannel.Clear();

            for(int i = 0; i < length; i+=2)
            {
                leftChannel.Add(sourceBuffer[i]);
                rightChannel.Add(sourceBuffer[i + 1]);
            }
        }

        
    }
}
