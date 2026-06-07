using System;
using System.IO;
using System.Text.Json;
using Vetra.Models;

namespace Vetra.Services;

public sealed class SettingsStoreService
{
    private readonly string _settingsPath;
    private readonly JsonSerializerOptions _jsonOptions = new() { WriteIndented = true };

    public SettingsStoreService()
    {
        var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        var settingsDirectory = Path.Combine(appData, "Vetra");
        Directory.CreateDirectory(settingsDirectory);

        _settingsPath = Path.Combine(settingsDirectory, "settings.json");
        Current = Load();
    }

    public AppSettings Current { get; }

    public void Save()
    {
        var json = JsonSerializer.Serialize(Current, _jsonOptions);
        File.WriteAllText(_settingsPath, json);
    }

    private AppSettings Load()
    {
        if (!File.Exists(_settingsPath))
        {
            return new AppSettings();
        }

        try
        {
            var json = File.ReadAllText(_settingsPath);
            return JsonSerializer.Deserialize<AppSettings>(json) ?? new AppSettings();
        }
        catch
        {
            return new AppSettings();
        }
    }
}

