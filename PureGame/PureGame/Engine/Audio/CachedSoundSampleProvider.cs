using System;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;

namespace PureGame.Engine.Audio
{
    /// <summary>
    /// ISampleProvider, który odtwarza CachedSound (one-shot). Jednorazowo „przepycha” bufor do miksu.
    /// </summary>
    public sealed class CachedSoundSampleProvider : ISampleProvider
    {
        private readonly CachedSound _cached;
        private long _position;
        public WaveFormat WaveFormat => _cached.WaveFormat;

        public float Volume { get; set; } = 1.0f;

        public CachedSoundSampleProvider(CachedSound cached) => _cached = cached;

        public int Read(float[] buffer, int offset, int count)
        {
            var availableSamples = _cached.AudioData.Length - _position;
            if (availableSamples <= 0) return 0;

            var samplesToCopy = (int)Math.Min(availableSamples, count);
            if (Volume == 1.0f)
            {
                Array.Copy(_cached.AudioData, _position, buffer, offset, samplesToCopy);
            }
            else
            {
                // zastosuj głośność
                for (int n = 0; n < samplesToCopy; n++)
                    buffer[offset + n] = _cached.AudioData[_position + n] * Volume;
            }

            _position += samplesToCopy;
            return samplesToCopy;
        }
    }
}