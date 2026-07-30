using System;

namespace StitchFluentOcrPro.Utilities
{
    public static class FileSizeFormatter
    {
        private static readonly string[] SizeSuffixes = { "B", "KB", "MB", "GB", "TB" };

        public static string FormatBytes(long bytes)
        {
            if (bytes < 0) return "0 B";
            if (bytes == 0) return "0 B";

            int mag = (int)Math.Log(bytes, 1024);
            mag = Math.Min(mag, SizeSuffixes.Length - 1);

            double adjustedSize = bytes / Math.Pow(1024, mag);
            return $"{adjustedSize:F2} {SizeSuffixes[mag]}";
        }
    }
}
