using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text.Json;

namespace PureGame.Engine.Save
{
    public static class SaveSystem
    {
        public const string CurrentVersion = "1.0.0";

        public static string RootDir { get; set; } =
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "My2DEngine", "Saves");

        private static readonly JsonSerializerOptions JsonOpts = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false,
            AllowTrailingCommas = true
        };

        // ========================= API =========================

        public static void EnsureDirectories()
        {
            Directory.CreateDirectory(RootDir);
        }

        public static IEnumerable<SaveSlotInfo> ListSlots()
        {
            EnsureDirectories();
            foreach (var file in Directory.EnumerateFiles(RootDir, "*.sav"))
            {
                SaveSlotInfo? meta = TryReadMetadata(file);
                if (meta != null) yield return meta;
            }
        }

        public static void Delete(string slotName)
        {
            var path = SlotPath(slotName);
            if (File.Exists(path)) File.Delete(path);
        }

        public static void Save<T>(string slotName, T gameState, SaveUserMeta? userMeta = null)
        {
            EnsureDirectories();

            // Zawartość do zapisania (koperta)
            var envelope = SaveEnvelope<T>.Create(gameState, userMeta);

            // Serializuj do bufora
            byte[] json = JsonSerializer.SerializeToUtf8Bytes(envelope, JsonOpts);

            // Hash (zawartość bez pola Hash). Najprościej: ustaw Hash = null, policz hash, wpisz i zapisuj ponownie
            var envNoHash = envelope with { Hash = null };
            byte[] jsonNoHash = JsonSerializer.SerializeToUtf8Bytes(envNoHash, JsonOpts);
            var hash = ComputeSha256(jsonNoHash);
            envelope = envelope with { Hash = hash };
            json = JsonSerializer.SerializeToUtf8Bytes(envelope, JsonOpts);

            // Atomowy zapis
            var path = SlotPath(slotName);
            var tmp = path + ".tmp";
            File.WriteAllBytes(tmp, json);
            if (File.Exists(path))
            {
                var bak = path + ".bak";
                SafeMove(path, bak, overwrite: true);
            }
            SafeMove(tmp, path, overwrite: true);
        }

        public static T Load<T>(string slotName)
        {
            var path = SlotPath(slotName);
            if (!File.Exists(path))
                throw new FileNotFoundException($"Brak pliku zapisu dla slotu '{slotName}'", path);

            var json = File.ReadAllBytes(path);
            var env = JsonSerializer.Deserialize<SaveEnvelope<T>>(json, JsonOpts)
                      ?? throw new InvalidDataException("Nie udało się zdeserializować zapisu.");

            // Walidacja wersji (możesz rozbudować o migracje)
            if (env.Version is null)
                throw new InvalidDataException("Zapis bez wersji.");
            // Przykładowo: akceptuj tylko CurrentVersion
            if (env.Version != CurrentVersion)
                throw new InvalidDataException($"Nieobsługiwana wersja zapisu: {env.Version} (wymagana {CurrentVersion}).");

            // Weryfikacja hash
            var envNoHash = env with { Hash = null };
            var probe = JsonSerializer.SerializeToUtf8Bytes(envNoHash, JsonOpts);
            var expected = ComputeSha256(probe);
            if (!string.Equals(env.Hash, expected, StringComparison.Ordinal))
                throw new InvalidDataException("Integralność zapisu naruszona (zły hash).");

            return env.State!;
        }

        // ========================= Helpers =========================

        private static string SlotPath(string slotName)
        {
            var safe = string.Concat(slotName.Where(ch => char.IsLetterOrDigit(ch) || ch is '_' or '-'));
            if (string.IsNullOrWhiteSpace(safe)) throw new ArgumentException("Nieprawidłowa nazwa slotu.");
            return Path.Combine(RootDir, safe + ".sav");
        }

        private static void SafeMove(string src, string dst, bool overwrite)
        {
            if (overwrite && File.Exists(dst)) File.Delete(dst);
            File.Move(src, dst);
        }

        private static string ComputeSha256(ReadOnlySpan<byte> data)
        {
            using var sha = SHA256.Create();
            var hash = sha.ComputeHash(data.ToArray());
            return Convert.ToHexString(hash);
        }

        private static SaveSlotInfo? TryReadMetadata(string filePath)
        {
            try
            {
                var bytes = File.ReadAllBytes(filePath);
                using var doc = JsonDocument.Parse(bytes);
                var root = doc.RootElement;

                string? version = root.GetPropertyOrNull("version")?.GetString();
                string? hash = root.GetPropertyOrNull("hash")?.GetString();
                DateTime timestampUtc = root.GetPropertyOrNull("timestampUtc")?.GetDateTime() ?? DateTime.MinValue;

                var userMetaEl = root.GetPropertyOrNull("userMeta");
                string? slot = Path.GetFileNameWithoutExtension(filePath);
                string? title = userMetaEl?.GetPropertyOrNull("title")?.GetString();
                int playSeconds = userMetaEl?.GetPropertyOrNull("playtimeSeconds")?.GetInt32() ?? 0;

                return new SaveSlotInfo(slot!, title, version ?? "?", timestampUtc, playSeconds, filePath);
            }
            catch
            {
                return null;
            }
        }

        // ====== Records / Models ======
        private record SaveEnvelope<T>(
            string? Version,
            DateTime TimestampUtc,
            string? Hash,
            T State,
            SaveUserMeta? UserMeta
        )
        {
            public static SaveEnvelope<T> Create(T state, SaveUserMeta? meta)
                => new(CurrentVersion, DateTime.UtcNow, null, state, meta);
        }

        public record SaveUserMeta(string? Title = null, int PlaytimeSeconds = 0);

        public record SaveSlotInfo(
            string SlotName,
            string? Title,
            string Version,
            DateTime TimestampUtc,
            int PlaytimeSeconds,
            string FilePath
        );
    }

    internal static class JsonElExt
    {
        public static JsonElement? GetPropertyOrNull(this JsonElement el, string name)
            => el.TryGetProperty(name, out var v) ? v : null;
    }
}
