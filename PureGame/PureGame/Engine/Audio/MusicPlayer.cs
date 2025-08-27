using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using PureGame.Engine.Audio;
using System;

namespace PureGame.Engine.Audio
{
    public sealed class MusicPlayer : IDisposable
    {
        private WaveOutEvent? _output;
        private WaveStream? _reader;          // AudioFileReader (WaveStream)
        private LoopStream? _loopStream;      // opcjonalny loop
        private ISampleProvider? _pipeline;   // do Volume itp.
        private float _volume = 0.8f;

        public bool IsPlaying { get; private set; }
        public bool Loop { get; private set; }

        public float Volume
        {
            get => _volume;
            set
            {
                _volume = Math.Clamp(value, 0f, 1f);
                if (_pipeline is VolumeSampleProvider vsp)
                    vsp.Volume = _volume;
            }
        }

        public void Play(string path, bool loop = true)
        {
            Stop(); // zamknij poprzednie

            _reader = new AudioFileReader(path); // WaveStream + wbudowany Volume, ale użyjemy VolumeSampleProvider dla spójności
            Loop = loop;

            WaveStream src = _reader;
            if (Loop) src = _loopStream = new LoopStream(_reader) { EnableLooping = true };

            _pipeline = new SampleChannel(src, false);
            _pipeline = new VolumeSampleProvider(_pipeline) { Volume = _volume };

            _output = new WaveOutEvent();
            _output.Init(_pipeline);
            _output.Play();
            IsPlaying = true;
        }

        public void Pause()
        {
            if (_output == null) return;
            _output.Pause();
            IsPlaying = false;
        }

        public void Resume()
        {
            if (_output == null) return;
            _output.Play();
            IsPlaying = true;
        }

        public void Stop()
        {
            IsPlaying = false;
            _output?.Stop();
            _output?.Dispose(); _output = null;
            _loopStream?.Dispose(); _loopStream = null;
            _reader?.Dispose(); _reader = null;
            _pipeline = null;
        }

        public void Dispose() => Stop();
    }
}
