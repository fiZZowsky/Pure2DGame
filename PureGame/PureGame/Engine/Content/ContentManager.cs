using PureGame.Engine.Audio;
using PureGame.Engine.Graphics;
using PureGame.Engine.Audio;
using PureGame.Engine.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace PureGame.Engine.Content
{
    public static class ContentManager
    {
        // Główny katalog z assetami
        public static string ContentRoot { get; private set; } =
            Path.Combine(AppContext.BaseDirectory, "./Engine/Content");

        // Cache + refcount
        private class Entry<T> : IDisposable where T : class, IDisposable
        {
            public T Asset { get; }
            public int RefCount { get; private set; }
            public Entry(T asset) { Asset = asset; RefCount = 1; }
            public void AddRef() => RefCount++;
            public void Release()
            {
                RefCount--;
                if (RefCount <= 0) Asset.Dispose();
            }
            public void Dispose() => Asset.Dispose();
        }

        private static readonly object _lock = new();
        private static readonly Dictionary<string, Entry<Texture2D>> _textures = new(StringComparer.OrdinalIgnoreCase);
        private static readonly Dictionary<string, Entry<CachedSound>> _sounds = new(StringComparer.OrdinalIgnoreCase);
        private static readonly Dictionary<string, Entry<Shader>> _shaders = new(StringComparer.OrdinalIgnoreCase);

        public static void Initialize(string? root = null)
        {
            if (!string.IsNullOrWhiteSpace(root))
                ContentRoot = Path.GetFullPath(root);
            Directory.CreateDirectory(ContentRoot);
        }

        public static void UnloadAll()
        {
            lock (_lock)
            {
                foreach (var e in _textures.Values) e.Dispose();
                foreach (var e in _sounds.Values) e.Dispose();
                foreach (var e in _shaders.Values) e.Dispose();
                _textures.Clear();
                _sounds.Clear();
                _shaders.Clear();
            }
        }

        // ===== Ścieżki i IO =====
        public static string ResolvePath(string relativePath)
        {
            var p = Path.Combine(ContentRoot, relativePath);
            if (!File.Exists(p))
                throw new FileNotFoundException($"Content not found: {relativePath}", p);
            return p;
        }

        public static string LoadText(string relativePath)
        {
            var p = ResolvePath(relativePath);
            return File.ReadAllText(p);
        }

        public static T LoadJson<T>(string relativePath, JsonSerializerOptions? options = null)
        {
            var p = ResolvePath(relativePath);
            var text = File.ReadAllText(p);
            return JsonSerializer.Deserialize<T>(text, options ?? new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!;
        }

        // ===== Tekstury =====
        public static Texture2D LoadTexture(string relativePath)
        {
            lock (_lock)
            {
                if (_textures.TryGetValue(relativePath, out var entry))
                {
                    entry.AddRef();
                    return entry.Asset;
                }

                var p = ResolvePath(relativePath);
                var tex = new Texture2D(p);
                _textures[relativePath] = new Entry<Texture2D>(tex);
                return tex;
            }
        }

        public static void UnloadTexture(string relativePath)
        {
            lock (_lock)
            {
                if (_textures.TryGetValue(relativePath, out var e))
                {
                    e.Release();
                    if (e.RefCount <= 0)
                        _textures.Remove(relativePath);
                }
            }
        }

        // ===== Dźwięki (SFX w pamięci) =====
        public static CachedSound LoadSound(string relativePath)
        {
            lock (_lock)
            {
                if (_sounds.TryGetValue(relativePath, out var entry))
                {
                    entry.AddRef();
                    return entry.Asset;
                }

                var p = ResolvePath(relativePath);
                var s = new CachedSound(p);
                _sounds[relativePath] = new Entry<CachedSound>(s);
                return s;
            }
        }

        public static void UnloadSound(string relativePath)
        {
            lock (_lock)
            {
                if (_sounds.TryGetValue(relativePath, out var e))
                {
                    e.Release();
                    if (e.RefCount <= 0)
                        _sounds.Remove(relativePath);
                }
            }
        }

        // ===== Shadery (z pliku .vert/.frag lub inline) =====
        public static Shader LoadShaderFromFiles(string vertexRelativePath, string fragmentRelativePath)
        {
            var key = $"{vertexRelativePath}|{fragmentRelativePath}";
            lock (_lock)
            {
                if (_shaders.TryGetValue(key, out var entry))
                {
                    entry.AddRef();
                    return entry.Asset;
                }

                var vs = LoadText(vertexRelativePath);
                var fs = LoadText(fragmentRelativePath);
                var shader = new Shader(vs, fs);
                _shaders[key] = new Entry<Shader>(shader);
                return shader;
            }
        }

        public static void UnloadShader(string vertexRelativePath, string fragmentRelativePath)
        {
            var key = $"{vertexRelativePath}|{fragmentRelativePath}";
            lock (_lock)
            {
                if (_shaders.TryGetValue(key, out var e))
                {
                    e.Release();
                    if (e.RefCount <= 0)
                        _shaders.Remove(key);
                }
            }
        }
    }
}
