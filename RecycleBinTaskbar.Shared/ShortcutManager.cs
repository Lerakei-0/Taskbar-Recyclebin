using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Shell;

namespace RecycleBinTaskbar.Shared
{
    /// <summary>
    /// Creates/refreshes the desktop shortcut (with the correct AppUserModelID)
    /// and registers the taskbar Jump List, using localized strings for the
    /// shortcut description and the two Jump List entries.
    /// </summary>
    public static class ShortcutManager
    {
        // Must be unique and stable - this ties the pinned shortcut to this Jump List.
        public const string AppId = "RecycleBinTaskbar.EmptyRecycleBin";
        public const string IconPath = @"%SystemRoot%\System32\shell32.dll";
        public const int IconIndex = 32; // full recycle bin icon

        public static string GetShortcutPath() => Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
            "Recycle Bin.lnk");

        /// <summary>
        /// (Re)creates the desktop shortcut and Jump List for the given exe,
        /// using the given language code.
        /// </summary>
        public static void Apply(string exePath, string languageCode)
        {
            var strings = Localization.Get(languageCode);
            string shortcutPath = GetShortcutPath();

            CreateShortcutWithAppId(shortcutPath, exePath, AppId, IconPath, IconIndex, strings.ShortcutDescription);
            RegisterJumpList(exePath, strings);
        }

        // ----- Shortcut creation with a matching AppUserModelID -----

        private static void CreateShortcutWithAppId(string shortcutPath, string targetExe,
            string appId, string iconPath, int iconIndex, string description)
        {
            var link = (IShellLinkW)new ShellLinkObject();
            link.SetPath(targetExe);
            link.SetDescription(description);
            link.SetIconLocation(iconPath, iconIndex);

            var propStore = (IPropertyStore)link;
            var key = PKEY_AppUserModel_ID;
            var pv = PropVariant.FromString(appId);
            try
            {
                propStore.SetValue(ref key, ref pv);
                propStore.Commit();
            }
            finally
            {
                pv.Dispose();
            }

            ((IPersistFile)link).Save(shortcutPath, true);
        }

        private static PropertyKey PKEY_AppUserModel_ID = new PropertyKey
        {
            fmtid = new Guid("9F4C2855-9F79-4B39-A8D0-E1D42DE1D5F3"),
            pid = 5
        };

        [ComImport, Guid("00021401-0000-0000-C000-000000000046")]
        private class ShellLinkObject { }

        [ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown),
         Guid("000214F9-0000-0000-C000-000000000046")]
        private interface IShellLinkW
        {
            void GetPath([Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszFile, int cchMaxPath, IntPtr pfd, uint fFlags);
            void GetIDList(out IntPtr ppidl);
            void SetIDList(IntPtr pidl);
            void GetDescription([Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszName, int cchMaxName);
            void SetDescription([MarshalAs(UnmanagedType.LPWStr)] string pszName);
            void GetWorkingDirectory([Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszDir, int cchMaxPath);
            void SetWorkingDirectory([MarshalAs(UnmanagedType.LPWStr)] string pszDir);
            void GetArguments([Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszArgs, int cchMaxPath);
            void SetArguments([MarshalAs(UnmanagedType.LPWStr)] string pszArgs);
            void GetHotkey(out short pwHotkey);
            void SetHotkey(short wHotkey);
            void GetShowCmd(out int piShowCmd);
            void SetShowCmd(int iShowCmd);
            void GetIconLocation([Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszIconPath, int cchIconPath, out int piIcon);
            void SetIconLocation([MarshalAs(UnmanagedType.LPWStr)] string pszIconPath, int iIcon);
            void SetRelativePath([MarshalAs(UnmanagedType.LPWStr)] string pszPathRel, uint dwReserved);
            void Resolve(IntPtr hwnd, uint fFlags);
            void SetPath([MarshalAs(UnmanagedType.LPWStr)] string pszFile);
        }

        [ComImport, Guid("886D8EEB-8CF2-4446-8D02-CDBA1DBDCF99"),
         InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        private interface IPropertyStore
        {
            void GetCount(out uint cProps);
            void GetAt(uint iProp, out PropertyKey pkey);
            void GetValue(ref PropertyKey key, out PropVariant pv);
            void SetValue(ref PropertyKey key, ref PropVariant pv);
            void Commit();
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct PropertyKey
        {
            public Guid fmtid;
            public int pid;
        }

        [StructLayout(LayoutKind.Explicit)]
        private struct PropVariant : IDisposable
        {
            [FieldOffset(0)] private ushort vt;
            [FieldOffset(8)] private IntPtr pointerValue;

            public static PropVariant FromString(string value)
            {
                return new PropVariant
                {
                    vt = 31, // VT_LPWSTR
                    pointerValue = Marshal.StringToCoTaskMemUni(value)
                };
            }

            public void Dispose() => PropVariantClear(ref this);

            [DllImport("ole32.dll")]
            private static extern int PropVariantClear(ref PropVariant pvar);
        }

        // ----- Jump List -----

        private static void RegisterJumpList(string exePath, LanguageStrings strings)
        {
            // A WPF Application instance is required to own the Jump List. If one
            // is already running (e.g. we're being called from the Settings
            // window), reuse it and don't shut it down. If not (e.g. we're being
            // called from the plain console-style main exe), create a throwaway
            // one, apply the Jump List, then shut it down.
            bool ownsApp = Application.Current == null;
            var app = Application.Current ?? new Application();

            var jumpList = new JumpList
            {
                ShowRecentCategory = false,
                ShowFrequentCategory = false
            };

            jumpList.JumpItems.Add(new JumpTask
            {
                Title = strings.EmptyTitle,
                Description = strings.EmptyDescription,
                ApplicationPath = exePath,
                Arguments = "/empty",
                IconResourcePath = IconPath,
                IconResourceIndex = IconIndex,
                CustomCategory = "Actions"
            });

            jumpList.JumpItems.Add(new JumpTask
            {
                Title = strings.OpenTitle,
                Description = strings.OpenDescription,
                ApplicationPath = exePath,
                Arguments = "/open",
                IconResourcePath = IconPath,
                IconResourceIndex = IconIndex,
                CustomCategory = "Actions"
            });

            JumpList.SetJumpList(app, jumpList);
            jumpList.Apply();

            if (ownsApp)
            {
                // Give the shell a moment to pick up the registration before exiting.
                Thread.Sleep(300);
                app.Shutdown();
            }
        }
    }
}
