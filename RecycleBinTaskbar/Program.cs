using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using RecycleBinTaskbar.Shared;

namespace RecycleBinTaskbar
{
    /// <summary>
    /// A tiny helper app that:
    ///   - with no arguments: creates/refreshes a desktop shortcut carrying the
    ///     correct AppUserModelID, registers a taskbar Jump List with
    ///     "Empty Recycle Bin" / "Open Recycle Bin" entries (in the
    ///     currently selected language), then exits.
    ///   - with "/empty": empties the Recycle Bin, then exits.
    ///   - with "/open": opens the Recycle Bin folder, then exits.
    /// Run it once with no arguments after building, then pin the "Recycle Bin"
    /// shortcut it creates on your Desktop to the taskbar (see README.md).
    ///
    /// To change the language of the shortcut/Jump List, run
    /// RecycleBinSettings.exe (built from the sibling RecycleBinSettings
    /// project) from the same folder.
    /// </summary>
    public static class AppEntry
    {
        [DllImport("shell32.dll", CharSet = CharSet.Unicode)]
        private static extern int SHEmptyRecycleBin(IntPtr hwnd, string pszRootPath, uint dwFlags);

        [DllImport("shell32.dll", CharSet = CharSet.Unicode)]
        private static extern void SetCurrentProcessExplicitAppUserModelID(string appId);

        private const uint SHERB_NOCONFIRMATION = 0x00000001;
        private const uint SHERB_NOPROGRESSUI = 0x00000002;
        private const uint SHERB_NOSOUND = 0x00000004;

        [STAThread]
        public static void Main(string[] args)
        {
            SetCurrentProcessExplicitAppUserModelID(ShortcutManager.AppId);

            if (args.Length > 0 && args[0].Equals("/empty", StringComparison.OrdinalIgnoreCase))
            {
                var emptySettings = AppSettings.Load();

                uint flags = SHERB_NOPROGRESSUI;
                if (!emptySettings.ConfirmBeforeEmptying) flags |= SHERB_NOCONFIRMATION;
                if (!emptySettings.PlaySoundWhenEmptying) flags |= SHERB_NOSOUND;

                int result = SHEmptyRecycleBin(IntPtr.Zero, null, flags);

                // result is S_OK (0) on success. If the user cancelled the
                // confirmation dialog, this will be non-zero.
                if (result == 0)
                {
                    // Refresh the jump list so the item count/size header is
                    // up to date immediately, without needing to relaunch the app.
                    string currentExePath = Process.GetCurrentProcess().MainModule.FileName;
                    ShortcutManager.Apply(currentExePath, emptySettings.Language);

                    if (emptySettings.ShowNotificationAfterEmptying)
                    {
                        var strings = Localization.Get(emptySettings.Language);
                        ToastHelper.ShowSimpleToast(ShortcutManager.AppId, strings.ShortcutDescription, strings.EmptiedNotification);
                    }
                }

                return;
            }

            if (args.Length > 0 && args[0].Equals("/open", StringComparison.OrdinalIgnoreCase))
            {
                Process.Start(new ProcessStartInfo("explorer.exe", "shell:RecycleBinFolder")
                {
                    UseShellExecute = true
                });
                return;
            }

            string exePath = Process.GetCurrentProcess().MainModule.FileName;
            var settings = AppSettings.Load();
            ShortcutManager.Apply(exePath, settings.Language);
        }
    }
}
