using System.Runtime.InteropServices;
using System.Windows;
using RecycleBinTaskbar.Shared;

namespace RecycleBinTaskbar.Settings
{
    public partial class App : Application
    {
        [DllImport("shell32.dll", CharSet = CharSet.Unicode)]
        private static extern void SetCurrentProcessExplicitAppUserModelID(string appId);

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Must match the AppUserModelID baked into the pinned shortcut
            // (see ShortcutManager.AppId), or the Jump List registered from
            // this process's JumpList.SetJumpList() call won't be associated
            // with the pinned Recycle Bin shortcut and will appear to do nothing.
            SetCurrentProcessExplicitAppUserModelID(ShortcutManager.AppId);
        }
    }
}
