using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using RecycleBinTaskbar.Shared;
// WPF also defines System.Windows.Localization, so alias ours to avoid CS0104.
using SharedLocalization = RecycleBinTaskbar.Shared.Localization;

namespace RecycleBinTaskbar.Settings
{
    public partial class SettingsWindow : Window
    {
        private readonly string _mainExePath;

        public SettingsWindow()
        {
            InitializeComponent();

            // Assumes RecycleBinSettings.exe lives in the same folder as
            // EmptyRecycleBin.exe (the main app that owns the Jump List actions).
            _mainExePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "EmptyRecycleBin.exe");

            var languages = SharedLocalization.Languages.Values.OrderBy(l => l.NativeName).ToList();
            LanguageComboBox.ItemsSource = languages;

            var current = AppSettings.Load();
            var selected = languages.FirstOrDefault(l => l.Code == current.Language)
                           ?? languages.First(l => l.Code == SharedLocalization.DefaultLanguageCode);
            LanguageComboBox.SelectedItem = selected;
        }

        private void ApplyButton_Click(object sender, RoutedEventArgs e)
        {
            if (LanguageComboBox.SelectedItem is not LanguageStrings selected)
                return;

            if (!File.Exists(_mainExePath))
            {
                StatusText.Foreground = Brushes.Red;
                StatusText.Text = "Could not find EmptyRecycleBin.exe next to this app. " +
                                   "Make sure both .exe files are in the same folder.";
                return;
            }

            try
            {
                AppSettings.Save(new AppSettingsData { Language = selected.Code });
                ShortcutManager.Apply(_mainExePath, selected.Code);

                StatusText.Foreground = Brushes.Green;
                StatusText.Text = $"Done! The taskbar jump list is now in {selected.NativeName}.";
            }
            catch (Exception ex)
            {
                StatusText.Foreground = Brushes.Red;
                StatusText.Text = "Something went wrong: " + ex.Message;
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
