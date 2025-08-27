using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using PureGame.Engine.Audio;
using System;
using System.Collections.Generic;
using System.IO;

namespace PureGame.Engine.Audio
{
    public static class AudioManager
    {
        // Ustawienia głośności (0..1)
        public static float MasterVolume { get; set; } = 1.0f;
        public static float SfxVolume { get; set; } = 1.0f;
        public static float MusicVolume
        {
            get => _music?.Volume ?? 0.8f;
            set { if (_music != null) _music.Volume = value; }
        }

        // Mixer SFX
        private static readonly object _lock = new();
        private static WaveOutEvent? _sfxOutput;
        private static MixingSampleProvider? _sfxMixer; // miks wielu jednoczesnych SFX
        private static readonly Dictionary<string, CachedSound> _cache = new(StringComparer.OrdinalIgnoreCase);

        // Muzyka
        private static MusicPlayer? _music;

        // Ścieżka bazowa na assety
        public static string AudioRoot { get; set; } = Path.Combine(AppContext.BaseDirectory, "Content", "Audio");

        public static void Initialize()
        {
            // Mixer SFX w 44.1k stereo
            _sfxMixer = new MixingSampleProvider(WaveFormat.CreateIeeeFloatWaveFormat(44100, 2))
            {
                ReadFully = true
            };
            _sfxOutput = new WaveOutEvent();
            _sfxOutput.Init(_sfxMixer);
            _sfxOutput.Play();

            _music = new MusicPlayer { /* Volume = 0.8f (domyślnie) */ };
        }

        public static void Dispose()
        {
            StopMusic();

            _sfxOutput?.Stop();
            _sfxOutput?.Dispose(); _sfxOutput = null;

            lock (_lock)
            {
                foreach (var kv in _cache) kv.Value.Dispose();
                _cache.Clear();
            }

            _sfxMixer = null;
            _music?.Dispose(); _music = null;
        }

        // ---------- SFX ----------
        public static void PreloadSfx(string relativePathOrKey)
        {
            GetOrLoadCached(relativePathOrKey);
        }

        public static void PlaySfx(string relativePathOrKey, float volume = 1.0f)
        {
            if (_sfxMixer == null) return;

            var cached = GetOrLoadCached(relativePathOrKey);
            var provider = new CachedSoundSampleProvider(cached)
            {
                // skala: Master * SFX * lokalna
                Volume = Math.Clamp(MasterVolume * SfxVolume * volume, 0f, 1f)
            };

            lock (_lock)
            {
                _sfxMixer.AddMixerInput(provider);
            }
        }

        private static CachedSound GetOrLoadCached(string relativePathOrKey)
        {
            lock (_lock)
            {
                if (_cache.TryGetValue(relativePathOrKey, out var cs))
                    return cs;

                string path = relativePathOrKey;
                if (!File.Exists(path))
                    path = Path.Combine(AudioRoot, relativePathOrKey);

                if (!File.Exists(path))
                    throw new FileNotFoundException($"Audio file not found: {relativePathOrKey}");

                var cached = new CachedSound(path);
                _cache[relativePathOrKey] = cached;
                return cached;
            }
        }

        // ---------- Music ----------
        public static void PlayMusic(string relativePath, bool loop = true, float? volume = null)
        {
            if (_music == null) return;

            string path = Path.Combine(AudioRoot, relativePath);
            if (!File.Exists(path)) throw new FileNotFoundException($"Music file not found: {relativePath}");

            _music.Volume = volume ?? _music.Volume;
            _music.Play(path, loop);
        }

        public static void PauseMusic() => _music?.Pause();
        public static void ResumeMusic() => _music?.Resume();
        public static void StopMusic() => _music?.Stop();
    }
}
