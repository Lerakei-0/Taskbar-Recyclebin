using System;
using System.Globalization;
using System.IO;
using System.Text.Json;

namespace RecycleBinTaskbar.Shared
{
    public class AppSettingsData
    {
        public string Language { get; set; } = Localization.DefaultLanguageCode;

        /// <summary>If true, Windows shows its native "Are you sure?" dialog before emptying.</summary>
        public bool ConfirmBeforeEmptying { get; set; } = false;

        /// <summary>If true, Windows plays its Recycle Bin empty sound.</summary>
        public bool PlaySoundWhenEmptying { get; set; } = false;

        /// <summary>If true, a toast notification is shown after the Recycle Bin is emptied.</summary>
        public bool ShowNotificationAfterEmptying { get; set; } = true;
    }

    /// <summary>
    /// Reads/writes a small settings.json under %LocalAppData%\RecycleBinTaskbar
    /// so both EmptyRecycleBin.exe and RecycleBinSettings.exe agree on the
    /// currently selected language and behavior toggles, regardless of which
    /// folder the app runs from.
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
                // Corrupt or unreadable settings file - fall back to defaults below.
            }

            // First run: no settings file yet. Try to match the language to the
            // system's display language instead of always defaulting to English.
            var defaults = new AppSettingsData { Language = DetectSystemLanguageCode() };
            try { Save(defaults); } catch { /* non-fatal if we can't persist yet */ }
            return defaults;
        }

        public static void Save(AppSettingsData data)
        {
            Directory.CreateDirectory(SettingsDir);
            var json = JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(SettingsFilePath, json);
        }

        private static string DetectSystemLanguageCode()
        {
            try
            {
                string twoLetter = CultureInfo.InstalledUICulture.TwoLetterISOLanguageName; // e.g. "en", "fr", "zh"

                // We only ship the Simplified Chinese variant.
                if (twoLetter == "zh")
                    return "zh-Hans";

                if (Localization.Languages.ContainsKey(twoLetter))
                    return twoLetter;
            }
            catch
            {
                // Ignore and fall back to the default below.
            }

            return Localization.DefaultLanguageCode;
        }
    }
}
