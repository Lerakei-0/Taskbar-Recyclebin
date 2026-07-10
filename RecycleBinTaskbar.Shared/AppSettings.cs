using System;
using System.IO;
using System.Text.Json;

namespace RecycleBinTaskbar.Shared
{
    public class AppSettingsData
    {
        public string Language { get; set; } = Localization.DefaultLanguageCode;
    }

    /// <summary>
    /// Reads/writes a small settings.json under %LocalAppData%\RecycleBinTaskbar
    /// so both EmptyRecycleBin.exe and RecycleBinSettings.exe agree on the
    /// currently selected language, regardless of which folder the app runs from.
    /// </summary>
    public static class AppSettings
    {
        private static readonly string SettingsDir = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "RecycleBinTaskbar");

        private static readonly string SettingsFilePath = Path.Combine(SettingsDir, "settings.json");

        public static AppSettingsData Load()
        {
            try
            {
                if (File.Exists(SettingsFilePath))
                {
                    var json = File.ReadAllText(SettingsFilePath);
                    var data = JsonSerializer.Deserialize<AppSettingsData>(json);
                    if (data != null && !string.IsNullOrWhiteSpace(data.Language))
                        return data;
                }
            }
            catch
            {
                // Corrupt or unreadable settings file - fall back to defaults.
            }

            return new AppSettingsData();
        }

        public static void Save(AppSettingsData data)
        {
            Directory.CreateDirectory(SettingsDir);
            var json = JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(SettingsFilePath, json);
        }
    }
}
