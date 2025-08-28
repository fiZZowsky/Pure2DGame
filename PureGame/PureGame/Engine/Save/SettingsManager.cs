using System;
using System.IO;
using System.Text.Json;

namespace PureGame.Engine.Save;

public static class SettingsManager
{
    public static string SettingsPath { get; set; } =
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "My2DEngine", "settings.json");

    private static readonly JsonSerializerOptions JsonOpts = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true
    };

    public static SettingsData Load()
    {
        try
        {
            if (File.Exists(SettingsPath))
            {
                var json = File.ReadAllText(SettingsPath);
                var data = JsonSerializer.Deserialize<SettingsData>(json, JsonOpts);
                if (data != null) return data;
            }
        }
        catch
        {
            // ignore invalid file
        }
        return new SettingsData();
    }

    public static void Save(SettingsData data)
    {
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(SettingsPath)!);
            var json = JsonSerializer.Serialize(data, JsonOpts);
            File.WriteAllText(SettingsPath, json);
        }
        catch
        {
            // ignore write errors
        }
    }
}