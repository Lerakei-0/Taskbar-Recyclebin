using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using RecycleBinTaskbar.Shared;
using Wpf.Ui.Controls;
// WPF also defines System.Windows.Localization, so alias ours to avoid CS0104.
using SharedLocalization = RecycleBinTaskbar.Shared.Localization;

namespace RecycleBinTaskbar.Settings
{
    public partial class SettingsWindow : FluentWindow
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

            ConfirmCheckBox.IsChecked = current.ConfirmBeforeEmptying;
            SoundCheckBox.IsChecked = current.PlaySoundWhenEmptying;
            NotificationCheckBox.IsChecked = current.ShowNotificationAfterEmptying;

            RefreshStatusReadout();
        }

        private void RefreshStatusReadout()
        {
            var status = RecycleBinInfo.TryGetStatus();
            if (status == null)
            {
                StatusReadout.Text = "Current status: unavailable.";
                return;
            }

            StatusReadout.Text = status.Value.ItemCount <= 0
                ? "Current status: Recycle Bin is empty."
                : $"Current status: {status.Value.ItemCount} item(s), {RecycleBinInfo.FormatSize(status.Value.TotalBytes)}.";
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            RefreshStatusReadout();
        }

        private void ApplyButton_Click(object sender, RoutedEventArgs e)
        {
            if (LanguageComboBox.SelectedItem is not LanguageStrings selected)
                return;

            if (!File.Exists(_mainExePath))
            {
                StatusText.Foreground = Brushes.IndianRed;
                StatusText.Text = "Could not find EmptyRecycleBin.exe next to this app. " +
                                   "Make sure both .exe files are in the same folder.";
                return;
            }

            try
            {
                var data = new AppSettingsData
                {
                    Language = selected.Code,
                    ConfirmBeforeEmptying = ConfirmCheckBox.IsChecked == true,
                    PlaySoundWhenEmptying = SoundCheckBox.IsChecked == true,
                    ShowNotificationAfterEmptying = NotificationCheckBox.IsChecked == true
                };

                AppSettings.Save(data);
                ShortcutManager.Apply(_mainExePath, selected.Code);
                RefreshStatusReadout();

                StatusText.Foreground = Brushes.MediumSeaGreen;
                StatusText.Text = $"Done! The taskbar jump list is now in {selected.NativeName}.";
            }
            catch (Exception ex)
            {
                StatusText.Foreground = Brushes.IndianRed;
                StatusText.Text = "Something went wrong: " + ex.Message;
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
