using System;
using NAudio.Wave;

namespace PureGame.Engine.Audio
{
    /// <summary>
    /// Wczytuje cały plik audio do pamięci jako PCM float (stereo 44.1kHz).
    /// Idealne do krótkich SFX odtwarzanych wielokrotnie.
    /// </summary>
    public sealed class CachedSound : IDisposable
    {
        public float[] AudioData { get; }
        public WaveFormat WaveFormat { get; }

        public CachedSound(string path)
        {
            using var reader = new AudioFileReader(path); // dekoduje do float 32-bit
            // Wyrównujemy do 44.1kHz stereo (jeśli trzeba)
            using var resampled = new MediaFoundationResampler(reader, WaveFormat.CreateIeeeFloatWaveFormat(44100, 2))
            {
                ResamplerQuality = 60
            };

            var wholeFile = new System.Collections.Generic.List<float>();
            var readBuffer = new float[resampled.WaveFormat.SampleRate * resampled.WaveFormat.Channels];
            int samplesRead;
            do
            {
                samplesRead = resampled.Read(readBuffer, 0, readBuffer.Length);
                if (samplesRead > 0)
                {
                    for (int n = 0; n < samplesRead; n++)
                        wholeFile.Add(readBuffer[n]);
                }
            } while (samplesRead > 0);

            AudioData = wholeFile.ToArray();
            WaveFormat = resampled.WaveFormat;
        }

        public void Dispose()
        {
            // nic do zrobienia: przechowujemy tylko float[]
        }
    }
}