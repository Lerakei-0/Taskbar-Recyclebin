using System.Runtime.InteropServices;

namespace RecycleBinTaskbar.Shared
{
    public readonly struct RecycleBinStatus
    {
        public long ItemCount { get; init; }
        public long TotalBytes { get; init; }
    }

    /// <summary>
    /// Queries the combined Recycle Bin status (item count + total size) across
    /// all drives using the native shell32 SHQueryRecycleBin API.
    /// </summary>
    public static class RecycleBinInfo
    {
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        private struct SHQUERYRBINFO
        {
            public int cbSize;
            public long i64Size;
            public long i64NumItems;
        }

        [DllImport("shell32.dll", CharSet = CharSet.Unicode)]
        private static extern int SHQueryRecycleBin(string pszRootPath, ref SHQUERYRBINFO pSHQueryRBInfo);

        /// <summary>Returns null if the query fails for any reason (e.g. no drives available).</summary>
        public static RecycleBinStatus? TryGetStatus()
        {
            var info = new SHQUERYRBINFO { cbSize = Marshal.SizeOf<SHQUERYRBINFO>() };

            // Passing null queries the Recycle Bin across all drives combined.
            int hr = SHQueryRecycleBin(null, ref info);
            if (hr != 0) // S_OK
                return null;

            return new RecycleBinStatus { ItemCount = info.i64NumItems, TotalBytes = info.i64Size };
        }

        public static string FormatSize(long bytes)
        {
            string[] units = { "B", "KB", "MB", "GB", "TB" };
            double size = bytes;
            int unit = 0;
            while (size >= 1024 && unit < units.Length - 1)
            {
                size /= 1024;
                unit++;
            }
            return $"{size.ToString("0.#", System.Globalization.CultureInfo.InvariantCulture)} {units[unit]}";
        }
    }
}
